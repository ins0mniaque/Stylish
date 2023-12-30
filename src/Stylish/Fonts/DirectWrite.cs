using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.Graphics.Dxgi.Common;
using Windows.Win32.Graphics.Gdi;

namespace Stylish;

// TODO: InheritDoc
public class FormattedText2
{
    // NOTE: RenderTarget goes in here...

    // public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, double pixelsPerDip) : this ( ) { }
    // public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution, double pixelsPerDip) : this ( ) { }
    // public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode, double pixelsPerDip)
    // {
    //     
    // }

    public double Extent { get; }
    public FlowDirection FlowDirection { get; set; }
    public double Height { get; }
    public double LineHeight { get; set; }
    public int MaxLineCount { get; set; }
    public double MaxTextHeight { get; set; }
    public double MaxTextWidth { get; set; }
    public double OverhangLeading { get; }
    public double OverhangAfter { get; }
    public double Baseline { get; }
    public double OverhangTrailing { get; }
    public double PixelsPerDip { get; set; }
    public string Text { get; } = string.Empty;
    public TextAlignment TextAlignment { get; set; }
    public TextTrimming Trimming { get; set; }
    public double MinWidth { get; }
    public double Width { get; }
    public double WidthIncludingTrailingWhitespace { get; }

    private FormattedText2 ( ID2D1DCRenderTarget renderTarget,
                            IDWriteTextLayout layout,
                            ID2D1Brush brush )
    {
        this.renderTarget = renderTarget;
        this.layout = layout;
        this.brush = brush;
    }

    private ID2D1DCRenderTarget renderTarget;
    private IDWriteTextLayout layout;
    private ID2D1Brush brush;

    public unsafe void Render ( DirectWriteContext directWriteContext, Point origin )
    {
        ArgumentNullException.ThrowIfNull ( directWriteContext );

        renderTarget.BindDC ( new HDC  ( directWriteContext.HDC ),
                              new RECT ( 0, 0,
                                         (int) directWriteContext.Size.Width,
                                         (int) directWriteContext.Size.Height ) );

        renderTarget.BeginDraw ( );

        renderTarget.DrawTextLayout ( new D2D_POINT_2F { x = (float) origin.X, y = (float) origin.Y }, layout, brush, D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT );

        renderTarget.EndDraw (null, null);
    }
}

// NOTE: Brush and Antialias is on RenterTarget...
public sealed class DirectWriteContext : IDisposable
{
    private readonly HDC memoryDc;
    private readonly HBITMAP bitmap;
    private readonly HGDIOBJ oldBitmap;

    public DirectWriteContext ( Size size, double dpiX, double dpiY )
    {
        var scaleX = dpiX / 96.0;
        var scaleY = dpiY / 96.0;

        var hwnd = PInvoke.GetDesktopWindow ( );
        var desktopDc = PInvoke.GetWindowDC ( hwnd );
        memoryDc = PInvoke.CreateCompatibleDC ( desktopDc );
        _ = PInvoke.ReleaseDC(hwnd, desktopDc);

        bitmap = PInvoke.CreateCompatibleBitmap ( desktopDc, (int) ( size.Width * scaleX ), (int) ( size.Height * scaleY ) );
        oldBitmap = PInvoke.SelectObject ( memoryDc, bitmap );

        Size = new Size ( (int) ( size.Width * scaleX ), (int) ( size.Height * scaleY ) );
    }

    public Size Size { get; }

    public void Render ( FormattedText2 formattedText, Point origin )
    {
        ArgumentNullException.ThrowIfNull ( formattedText );

        formattedText.Render ( this, origin );
    }

    public nint HDC     => memoryDc.Value;
    public nint HBitmap => bitmap  .Value;

    public void Dispose ( )
    {
         PInvoke.SelectObject(memoryDc, oldBitmap);
            PInvoke.DeleteObject(bitmap);
            PInvoke.DeleteDC(memoryDc);
    }
}

// TODO: Own lib; just has HBITMAP access to make it not dependant on WPF.
public static class DirectWrite
{
    public static bool IsSupported => dWrite.Value is not null && d2d1.Value is not null;

    private static readonly ThreadLocal < ID2D1Factory?   > d2d1   = new ( Cre );
    private static readonly Lazy        < IDWriteFactory? > dWrite = new ( Cre2 );

    private static ID2D1Factory   D2D1   => d2d1  .Value ?? throw new NotSupportedException ( );
    private static IDWriteFactory DWrite => dWrite.Value ?? throw new NotSupportedException ( );

    private static ID2D1Factory? Cre ( )
    {
        if ( PInvoke.D2D1CreateFactory ( D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof ( ID2D1Factory ).GUID, null, out var pFactory ) == HRESULT.S_OK )
            return (ID2D1Factory) pFactory;

        return null;
    }

    private static IDWriteFactory? Cre2 ( )
    {
        if ( PInvoke.DWriteCreateFactory ( DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, typeof ( IDWriteFactory ).GUID, out var dFactory ) == HRESULT.S_OK )
            return (IDWriteFactory) dFactory;

        return null;
    }

    public static unsafe BitmapSource Lol ( string str, double fontSize, Typeface typeface, double dpiX, double dpiY, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity )
    {
        ArgumentNullException.ThrowIfNull ( typeface );

        var scaleX = dpiX / 96.0;
        var scaleY = dpiY / 96.0;

        DWrite.CreateTextFormat (typeface.FontFamily.Source,
                                  null,
                                  (DWRITE_FONT_WEIGHT) typeface.Weight.ToOpenTypeWeight ( ),
                                  typeface.Style == FontStyles.Normal ? DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL :
                                  typeface.Style == FontStyles.Italic ? DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_ITALIC :
                                  typeface.Style == FontStyles.Oblique ? DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_OBLIQUE :
                                                                         DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL,
                                  (DWRITE_FONT_STRETCH) typeface.Stretch.ToOpenTypeStretch ( ),
                                  (float) fontSize,
                                  "en-us",
                                  out var text );

        text.SetReadingDirection (DWRITE_READING_DIRECTION.DWRITE_READING_DIRECTION_LEFT_TO_RIGHT);

        str ??= string.Empty;

        var layoutWidth  = double.IsPositiveInfinity ( maxWidth  ) ? int.MaxValue : (int) maxWidth;
        var layoutHeight = double.IsPositiveInfinity ( maxHeight ) ? int.MaxValue : (int) maxHeight;

        DWrite.CreateGdiCompatibleTextLayout ( str, (uint) str.Length, text, layoutWidth, layoutHeight, (float) scaleY, null, new BOOL(true), out var layout );

        layout.GetMetrics ( out var metrics );


        var RtProps = new D2D1_RENDER_TARGET_PROPERTIES () { pixelFormat = new D2D1_PIXEL_FORMAT { format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                                                             alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED },
                                                             dpiX  = (float) dpiX, dpiY = (float) dpiY,
                                                             usage = D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE};

        D2D1.CreateDCRenderTarget ( RtProps, out var renderTarget );

        renderTarget.CreateSolidColorBrush ( new D2D1_COLOR_F { a = 1.0f, r = 0.0f, g = 0.0f, b = 0.0f }, null, out var brush );

        // renderTarget.SetAntialiasMode();
        // renderTarget.SetTextAntialiasMode();
        // renderTarget.SetTextRenderingParams();


        var hwnd = PInvoke.GetDesktopWindow ( );
        var desktopDc = PInvoke.GetWindowDC ( hwnd );
        var memoryDc = PInvoke.CreateCompatibleDC ( desktopDc );

        var bitmap = PInvoke.CreateCompatibleBitmap ( desktopDc, (int) ( metrics.widthIncludingTrailingWhitespace * scaleX ), (int) ( metrics.height * scaleY ) );
        var oldBitmap = PInvoke.SelectObject ( memoryDc, bitmap );

        var rect = new RECT ( 0, 0, (int) ( metrics.widthIncludingTrailingWhitespace * scaleX ), (int) ( metrics.height * scaleY ) );

        renderTarget.BindDC ( memoryDc, rect );

        renderTarget.BeginDraw ( );

        renderTarget.DrawTextLayout(new D2D_POINT_2F(),layout, brush, D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT);

        renderTarget.EndDraw (null, null);

        BitmapSource result;
        try
        {
            result = Imaging.CreateBitmapSourceFromHBitmap(bitmap, (nint) 0, new Int32Rect ( 0, 0, (int) ( metrics.widthIncludingTrailingWhitespace * scaleX ), (int) ( metrics.height * scaleY ) ),
                                                           BitmapSizeOptions.FromWidthAndHeight((int) ( metrics.widthIncludingTrailingWhitespace * 1.0 ), (int) ( metrics.height * 1.0 ) ) );
        }
        finally
        {
            PInvoke.SelectObject(memoryDc, oldBitmap);
            PInvoke.DeleteObject(bitmap);
            PInvoke.DeleteDC(memoryDc);
            _ = PInvoke.ReleaseDC(new HWND ( hwnd ), desktopDc);
        }

        return result;
    }
}