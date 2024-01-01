using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.Graphics.Dxgi.Common;
using Windows.Win32.Graphics.Gdi;

namespace Stylish.DirectWrite;


public abstract class Brush
{
    internal abstract ID2D1Brush Interface { get; }
}

public readonly struct Color : IEquatable<Color>
{
    public Color ( float r, float g, float b, float a )
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public float R { get; }
	public float G { get; }
	public float B { get; }
	public float A { get; }

    public override bool Equals ( object? obj ) => obj is Color other && Equals ( other );
    public override int GetHashCode ( ) => HashCode.Combine ( R, G, B, A );

    public static bool operator == ( Color left, Color right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( Color left, Color right )
    {
        return !( left == right );
    }

    public bool Equals ( Color other )
    {
        return R == other.R && G == other.G && B == other.B && A == other.A;
    }
}

public class SolidColorBrush : Brush
{
    internal readonly ID2D1SolidColorBrush sbrush;

    public SolidColorBrush ( RenderTarget renderTarget, Color color, float opacity, Matrix transform )
    {
        ArgumentNullException.ThrowIfNull ( renderTarget );

        var t = Unsafe.As < Matrix, D2D_MATRIX_3X2_F > ( ref transform );
        var c = Unsafe.As < Color, D2D1_COLOR_F > ( ref color );

        var properties = new D2D1_BRUSH_PROPERTIES { opacity = opacity, transform = t };

        renderTarget.Interface.CreateSolidColorBrush ( c, properties, out sbrush );
    }

    internal override ID2D1Brush Interface => sbrush;
}

[Flags]
public enum RenderOptions
{
	/// <summary>Text is vertically snapped to pixel boundaries and is not clipped to the layout rectangle.</summary>
	None = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_NONE,
	/// <summary>Text is not vertically snapped to pixel boundaries. This setting is recommended for text that is being animated.</summary>
	NoSnap = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_NO_SNAP,
	/// <summary>Text is clipped to the layout rectangle.</summary>
	Clip = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_CLIP,
	/// <summary>In Windows 8.1 and later, text is rendered using color versions of glyphs, if defined by the font.</summary>
	EnableColorFont = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT,
	/// <summary>Bitmap origins of color glyph bitmaps are not snapped.</summary>
	DisableColorBitmapSnapping = D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_DISABLE_COLOR_BITMAP_SNAPPING
}

public class PixelDensity : IEquatable < PixelDensity >
{
    private const float DipDpi   = 96.0f;
    private const float PointDpi = 72.0f;

    public static PixelDensity FromDpi ( float dpi )                      => new ( dpi,  dpi  );
    public static PixelDensity FromDpi ( float dpiX,     float dpiY     ) => new ( dpiX, dpiY );
    public static PixelDensity FromDip ( float dipSizeX, float dipSizeY ) => new ( dipSizeX * DipDpi, dipSizeY * DipDpi );
    public static PixelDensity FromDip ( float dipSize )                  => new ( dipSize  * DipDpi, dipSize  * DipDpi );

    private PixelDensity ( float dpiX, float dpiY )
    {
        PerInchX = dpiX;
        PerInchY = dpiY;
    }

    public float PerInchX { get; }
    public float PerInchY { get; }

    public float PerDip  => PerDipY;
    public float PerDipX => PerInchX / DipDpi;
    public float PerDipY => PerInchY / DipDpi;

    [ SuppressMessage ( "Performance", "CA1822:Mark members as static", Justification = "Discoverability" ) ]
    public float GetFontHeight      ( float fontSize ) => fontSize *          DipDpi / PointDpi;
    public float GetFontPixelHeight ( float fontSize ) => fontSize * PerDip * DipDpi / PointDpi;

    public int FromPixelWidth  ( int pixelWidth  ) => (int) ( pixelWidth  / PerDipX );
    public int FromPixelHeight ( int pixelHeight ) => (int) ( pixelHeight / PerDipY );
    public int ToPixelWidth    ( int width       ) => (int) ( width       * PerDipX );
    public int ToPixelHeight   ( int height      ) => (int) ( height      * PerDipY );

    public override bool Equals ( object? obj )
    {
        return obj is PixelDensity other && Equals ( other );
    }

    public bool Equals ( PixelDensity? other )
    {
        return PerInchX == other?.PerInchX && PerInchY == other?.PerInchY;
    }

    public override int GetHashCode ( )
    {
        return HashCode.Combine ( PerInchX, PerInchY );
    }

    public static bool operator == ( PixelDensity left, PixelDensity right )
    {
        return Equals ( left, right );
    }

    public static bool operator != ( PixelDensity left, PixelDensity right )
    {
        return ! ( left == right );
    }
}

public enum TextAntialiasMode
{
	/// <summary>Use the system default. See Remarks.</summary>
	Default = D2D1_TEXT_ANTIALIAS_MODE.D2D1_TEXT_ANTIALIAS_MODE_DEFAULT,
	/// <summary>Use ClearType antialiasing.</summary>
	ClearType = D2D1_TEXT_ANTIALIAS_MODE.D2D1_TEXT_ANTIALIAS_MODE_CLEARTYPE,
	/// <summary>Use grayscale antialiasing.</summary>
	Grayscale = D2D1_TEXT_ANTIALIAS_MODE.D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE,
	/// <summary>Do not use antialiasing.</summary>
	Aliased = D2D1_TEXT_ANTIALIAS_MODE.D2D1_TEXT_ANTIALIAS_MODE_ALIASED
}

public abstract class RenderTarget
{
    internal abstract ID2D1RenderTarget Interface { get; }
    private           bool              began;

    public TextAntialiasMode TextAntialiasMode
    {
        get => (TextAntialiasMode) Interface.GetTextAntialiasMode ( );
        set => Interface.SetTextAntialiasMode ( (D2D1_TEXT_ANTIALIAS_MODE) value );
    }

    public unsafe void Clear ( )
    {
        EnsureBegan ( );

        Interface.Clear ( default );
    }

    public unsafe void Render ( TextLayout textLayout, Brush brush, RenderOptions options )
    {
        Render ( 0, 0, textLayout, brush, options );
    }

    public unsafe void Render ( float x, float y, TextLayout textLayout, Brush brush, RenderOptions options )
    {
        ArgumentNullException.ThrowIfNull ( textLayout );
        ArgumentNullException.ThrowIfNull ( brush );

        EnsureBegan ( );

        Interface.DrawTextLayout(new ( ) { x = x, y = y },textLayout.Interface, brush.Interface, (D2D1_DRAW_TEXT_OPTIONS) options);
    }

    public unsafe void Flush ( )
    {
        if ( began )
        {
            Interface.EndDraw ( null, null );

            began = false;
        }
    }

    private void EnsureBegan ( )
    {
        if ( ! began )
        {
            Interface.BeginDraw ( );

            began = true;
        }
    }
}

public sealed class Window : RenderTarget
{
    private readonly ID2D1HwndRenderTarget renderTarget;
    private readonly HWND                  hwnd;

    internal override ID2D1RenderTarget Interface => renderTarget;

    public Window ( nint hwnd )
    {
        this.hwnd = new HWND ( hwnd );
        if(!PInvoke.GetClientRect ( this.hwnd, out var rect ))
            throw new Win32Exception ();

        var RtProps = new D2D1_RENDER_TARGET_PROPERTIES () { };
        var HwndRtProps = new D2D1_HWND_RENDER_TARGET_PROPERTIES () { hwnd = this.hwnd,
            pixelSize = new D2D_SIZE_U () { width = (uint) rect.Width, height = (uint) rect.Height },
            presentOptions = D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE };

        DirectX.D2D1.CreateHwndRenderTarget ( RtProps, HwndRtProps, out renderTarget );

        // TODO: Hook resize and dpi changed messages
        //       Add PixelDensity property
    }

    public int PixelWidth { get; private set; }
    public int PixelHeight { get; private set; }

    public void Resize ( )
    {
        if(!PInvoke.GetClientRect ( hwnd, out var rect ))
            throw new Win32Exception ();

        PixelWidth = rect.Width;
        PixelHeight = rect.Height;

        renderTarget.Resize(new D2D_SIZE_U () { width = (uint) rect.Width, height = (uint) rect.Height });
    }
}

public sealed class Surface : RenderTarget, IDisposable
{
    private readonly ID2D1DCRenderTarget renderTarget;
    private readonly HDC     memoryDc;
    private readonly HGDIOBJ oldBitmap;
    private          HBITMAP bitmap;

    internal override ID2D1RenderTarget Interface => renderTarget;

    public Surface ( int width, int height, float dpiX, float dpiY ) : this ( width, height, PixelDensity.FromDpi ( dpiX, dpiY ) ) { }
    public Surface ( int width, int height, PixelDensity pixelDensity )
    {
        ArgumentNullException.ThrowIfNull ( pixelDensity );

        var RtProps = new D2D1_RENDER_TARGET_PROPERTIES ( ) { pixelFormat = new ( ) { format    = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                                                                                      alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED },
                                                              dpiX  = pixelDensity.PerInchX,
                                                              dpiY  = pixelDensity.PerInchY,
                                                              usage = D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE };

        DirectX.D2D1.CreateDCRenderTarget ( RtProps, out renderTarget );

        PixelDensity = pixelDensity;
        Width        = width;
        Height       = height;
        PixelWidth   = pixelDensity.ToPixelWidth  ( width );
        PixelHeight  = pixelDensity.ToPixelHeight ( height );
        BitsPerPixel = 32;

        memoryDc = ReplaceBitmap ( memoryDc, out oldBitmap );
    }

    public PixelDensity PixelDensity { get; private set; }

    public int PixelWidth { get; private set; }
    public int PixelHeight { get; private set; }
    public int BitsPerPixel { get; }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public nint HDC     => memoryDc.Value;
    public nint HBitmap => bitmap  .Value; // TODO: Fail if began?

    public unsafe void CopyPixels < T > ( Span < T > span )
    {
        var x = span.GetPinnableReference ( );
        #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        CopyPixels ( &x, span.Length * sizeof ( T ) );
        #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    // TODO: GetDIBits fails
    private unsafe void CopyPixels ( void* span, int length )
    {
        var bi = new BITMAPINFO () { bmiHeader = new BITMAPINFOHEADER { biSize = (uint)sizeof (BITMAPINFOHEADER) }};
        var y = PInvoke.GetDIBits ( new HDC ( HDC ),new HBITMAP ( HBitmap ), 0, 1, null, &bi,DIB_USAGE.DIB_RGB_COLORS );

        if ( length < bi.bmiHeader.biSizeImage )
            throw new ArgumentException ( "Buffer too small" );

        // bi.bmiHeader.biCompression = 0;
        // stride = ((((biWidth * biBitCount) + 31) & ~31) >> 3);
        // biSizeImage = abs(biHeight) * stride;
        bi.bmiHeader.biSizeImage = 0;

        PInvoke.SelectObject ( memoryDc, oldBitmap );

        var x = PInvoke.GetDIBits ( new HDC ( HDC ),new HBITMAP ( HBitmap ), 0, (uint) PixelHeight, span, &bi,DIB_USAGE.DIB_RGB_COLORS );
        if ( x is 0 )
            throw new InvalidOperationException ( );

        PInvoke.SelectObject ( memoryDc, bitmap );
    }

    public unsafe void Resize ( int width, int height, float dpiX, float dpiY )
    {
        var pixelDensity = PixelDensity.FromDpi ( dpiX, dpiY );
        var pixelWidth   = pixelDensity.ToPixelWidth  ( width );
        var pixelHeight  = pixelDensity.ToPixelHeight ( height );

        Width  = width;
        Height = height;

        if ( PixelWidth != pixelWidth || PixelHeight != pixelHeight )
        {
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;

            ReplaceBitmap ( memoryDc, out var oldBitmap );

            PInvoke.DeleteObject ( oldBitmap );
        }

        if ( PixelDensity != pixelDensity )
        {
            PixelDensity = pixelDensity;

            renderTarget.SetDpi(dpiX, dpiY);
        }
    }

    private HDC ReplaceBitmap ( HDC memoryDc, out HGDIOBJ oldBitmap )
    {
        var hwnd = PInvoke.GetDesktopWindow ( );
        var desktopDc = PInvoke.GetWindowDC ( hwnd );

        if ( memoryDc.Value is 0 )
            memoryDc = PInvoke.CreateCompatibleDC ( desktopDc );

        bitmap = PInvoke.CreateCompatibleBitmap ( desktopDc, PixelWidth, PixelHeight );

        _ = PInvoke.ReleaseDC(hwnd, desktopDc);

        oldBitmap = PInvoke.SelectObject ( memoryDc, bitmap );

        renderTarget.BindDC ( memoryDc, new RECT ( 0, 0, PixelWidth, PixelHeight ) );

        return memoryDc;
    }

    public void Dispose ( )
    {
         PInvoke.SelectObject(memoryDc, oldBitmap);
            PInvoke.DeleteObject(bitmap);
            PInvoke.DeleteDC(memoryDc);
    }
}

[SuppressMessage ( "Design", "CA1069:Enums values should not be duplicated", Justification = "DWRITE_FONT_WEIGHT" )]
[SuppressMessage ( "Design", "CA1008:Enums should have zero value", Justification = "DWRITE_FONT_WEIGHT" )]
public enum FontWeight
{
	/// <summary>Predefined font weight : Thin (100).</summary>
	Thin = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_THIN,
	/// <summary>Predefined font weight : Extra-light (200).</summary>
	ExtraLight = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_EXTRA_LIGHT,
	/// <summary>Predefined font weight : Ultra-light (200).</summary>
	UltraLight = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_ULTRA_LIGHT,
	/// <summary>Predefined font weight : Light (300).</summary>
	Light = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_LIGHT,
	/// <summary>Predefined font weight : Semi-Light (350).</summary>
	SemiLight = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_SEMI_LIGHT,
	/// <summary>Predefined font weight : Normal (400).</summary>
	Normal = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_NORMAL,
	/// <summary>Predefined font weight : Regular (400).</summary>
	Regular = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_REGULAR,
	/// <summary>Predefined font weight : Medium (500).</summary>
	Medium = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_MEDIUM,
	/// <summary>Predefined font weight : Demi-bold (600).</summary>
	DemiBold = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_DEMI_BOLD,
	/// <summary>Predefined font weight : Semi-bold (600).</summary>
	SemiBold = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_SEMI_BOLD,
	/// <summary>Predefined font weight : Bold (700).</summary>
	Bold = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_BOLD,
	/// <summary>Predefined font weight : Extra-bold (800).</summary>
	ExtraBold = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_EXTRA_BOLD,
	/// <summary>Predefined font weight : Ultra-bold (800).</summary>
	UltraBold = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_ULTRA_BOLD,
	/// <summary>Predefined font weight : Black (900).</summary>
	Black = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_BLACK,
	/// <summary>Predefined font weight : Heavy (900).</summary>
	Heavy = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_HEAVY,
	/// <summary>Predefined font weight : Extra-black (950).</summary>
	ExtraBlack = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_EXTRA_BLACK,
	/// <summary>Predefined font weight : Ultra-black (950).</summary>
	UltraBlack = DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_ULTRA_BLACK
}

public enum FontStyle
{
	/// <summary>Font style : Normal.</summary>
	Normal = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL,
	/// <summary>Font style : Oblique.</summary>
	Oblique = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_OBLIQUE,
	/// <summary>Font style : Italic.</summary>
	Italic = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_ITALIC
}

[SuppressMessage ( "Design", "CA1027:Mark enums with FlagsAttribute", Justification = "DWRITE_FONT_STRETCH" )]
[SuppressMessage ( "Design", "CA1069:Enums values should not be duplicated", Justification = "DWRITE_FONT_STRETCH" )]
[SuppressMessage ( "Design", "CA1008:Enums should have zero value", Justification = "DWRITE_FONT_STRETCH" )]
public enum FontStretch
{
	/// <summary>Predefined font stretch : Ultra-condensed (1).</summary>
	UltraCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_CONDENSED,
	/// <summary>Predefined font stretch : Extra-condensed (2).</summary>
	ExtraCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_CONDENSED,
	/// <summary>Predefined font stretch : Condensed (3).</summary>
	Condensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_CONDENSED,
	/// <summary>Predefined font stretch : Semi-condensed (4).</summary>
	SemiCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_CONDENSED,
	/// <summary>Predefined font stretch : Normal (5).</summary>
	Normal = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,
	/// <summary>Predefined font stretch : Medium (5).</summary>
	Medium = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_MEDIUM,
	/// <summary>Predefined font stretch : Semi-expanded (6).</summary>
	SemiExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_EXPANDED,
	/// <summary>Predefined font stretch : Expanded (7).</summary>
	Expanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXPANDED,
	/// <summary>Predefined font stretch : Extra-expanded (8).</summary>
	ExtraExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_EXPANDED,
	/// <summary>Predefined font stretch : Ultra-expanded (9).</summary>
	UltraExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_EXPANDED
}

public enum TextAlignment
{
	/// <summary>The leading edge of the paragraph text is aligned to the leading edge of the layout box.</summary>
	Leading = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_LEADING,
	/// <summary>The trailing edge of the paragraph text is aligned to the trailing edge of the layout box.</summary>
	Trailing = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_TRAILING,
	/// <summary>The center of the paragraph text is aligned to the center of the layout box.</summary>
	Center = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_CENTER,
	/// <summary>Align text to the leading side, and also justify text to fill the lines.</summary>
	Justified = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_JUSTIFIED
}

public enum ParagraphAlignment
{
	/// <summary>The top of the text flow is aligned to the top edge of the layout box.</summary>
	Near = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_NEAR,
	/// <summary>The bottom of the text flow is aligned to the bottom edge of the layout box.</summary>
	Far = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_FAR,
	/// <summary>The center of the flow is aligned to the center of the layout box.</summary>
	Center = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_CENTER,
}

public enum WordWrapping
{
	/// <summary>Indicates that words are broken across lines to avoid text overflowing the layout box.</summary>
	Wrap = DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_WRAP,
	/// <summary>Indicates that words are kept within the same line even when it overflows the layout box. This option is often used with scrolling to reveal overflow text.</summary>
	NoWrap = DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_NO_WRAP,
	/// <summary>
	/// <para><div class="alert"><b>Note</b>  Windows 8.1 and later only.</div> <div> </div> Words are broken across lines to avoid text overflowing the layout box. Emergency wrapping occurs if the word is larger than the maximum width.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_word_wrapping#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	EmergencyBreak = DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_EMERGENCY_BREAK,
	/// <summary>
	/// <para><div class="alert"><b>Note</b>  Windows 8.1 and later only.</div> <div> </div> When emergency wrapping, only wrap whole words, never breaking words when the layout width is too small for even a single word.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_word_wrapping#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	WholeWord = DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_WHOLE_WORD,
	/// <summary>
	/// <para><div class="alert"><b>Note</b>  Windows 8.1 and later only.</div> <div> </div> Wrap between any valid character clusters.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_word_wrapping#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Character = DWRITE_WORD_WRAPPING.DWRITE_WORD_WRAPPING_CHARACTER
}

public enum ReadingDirection
{
	/// <summary>Indicates that reading progresses from left to right.</summary>
	LeftToRight = DWRITE_READING_DIRECTION.DWRITE_READING_DIRECTION_LEFT_TO_RIGHT,
	/// <summary>Indicates that reading progresses from right to left.</summary>
	RightToLeft = DWRITE_READING_DIRECTION.DWRITE_READING_DIRECTION_RIGHT_TO_LEFT,
	/// <summary>
	/// <para><div class="alert"><b>Note</b>  Windows 8.1 and later only.</div> <div> </div> Indicates that reading progresses from top to bottom.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_reading_direction#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	TopToBottom = DWRITE_READING_DIRECTION.DWRITE_READING_DIRECTION_TOP_TO_BOTTOM,
	/// <summary>
	/// <para><div class="alert"><b>Note</b>  Windows 8.1 and later only.</div> <div> </div> Indicates that reading progresses from bottom to top.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_reading_direction#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	BottomToTop = DWRITE_READING_DIRECTION.DWRITE_READING_DIRECTION_BOTTOM_TO_TOP
}

public enum FlowDirection
{
	/// <summary>Specifies that text lines are placed from top to bottom.</summary>
	TopToBottom = DWRITE_FLOW_DIRECTION.DWRITE_FLOW_DIRECTION_TOP_TO_BOTTOM,
	/// <summary>Specifies that text lines are placed from bottom to top.</summary>
	BottomToTop = DWRITE_FLOW_DIRECTION.DWRITE_FLOW_DIRECTION_BOTTOM_TO_TOP,
	/// <summary>Specifies that text lines are placed from left to right.</summary>
	LeftToRight = DWRITE_FLOW_DIRECTION.DWRITE_FLOW_DIRECTION_LEFT_TO_RIGHT,
	/// <summary>Specifies that text lines are placed from right to left.</summary>
	RightToLeft = DWRITE_FLOW_DIRECTION.DWRITE_FLOW_DIRECTION_RIGHT_TO_LEFT
}

/// <inheritdoc cref="DWRITE_TRIMMING_GRANULARITY" />
public enum TextTrimming
{
	/// <summary>No trimming occurs. Text flows beyond the layout width.</summary>
	None = DWRITE_TRIMMING_GRANULARITY.DWRITE_TRIMMING_GRANULARITY_NONE,
	/// <summary>Trimming occurs at a character cluster boundary.</summary>
	Character = DWRITE_TRIMMING_GRANULARITY.DWRITE_TRIMMING_GRANULARITY_CHARACTER,
	/// <summary>Trimming occurs at a word boundary.</summary>
	Word = DWRITE_TRIMMING_GRANULARITY.DWRITE_TRIMMING_GRANULARITY_WORD
}

public enum LineSpacingMethod
{
	/// <summary>Line spacing depends solely on the content, adjusting to accommodate the size of fonts and inline objects.</summary>
	Default = DWRITE_LINE_SPACING_METHOD.DWRITE_LINE_SPACING_METHOD_DEFAULT,
	/// <summary>Lines are explicitly set to uniform spacing, regardless of the size of fonts and inline objects. This can be useful to avoid the uneven appearance that can occur from font fallback.</summary>
	Uniform = DWRITE_LINE_SPACING_METHOD.DWRITE_LINE_SPACING_METHOD_UNIFORM,
	/// <summary>
	/// <para>Line spacing and baseline distances are proportional to the computed values based on the content, the size of the fonts and inline objects.</para>
	/// <para><div class="alert"><b>Note</b>  This value is only available on Windows 10 or later and it can be used with <a href="https://docs.microsoft.com/windows/win32/DirectWrite/idwritetextlayout3-setlinespacing">IDWriteTextLayout3::SetLineSpacing</a>, but can not be used with <a href="https://docs.microsoft.com/windows/win32/api/dwrite/nf-dwrite-idwritetextformat-setlinespacing">IDWriteTextFormat::SetLineSpacing</a>.</div> <div> </div></para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ne-dwrite-dwrite_line_spacing_method#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Proportional = DWRITE_LINE_SPACING_METHOD.DWRITE_LINE_SPACING_METHOD_PROPORTIONAL,
}

// public class Inline
// {
//     internal IDWriteInlineObject Interface;
// }

public class TextFormat
{
    internal IDWriteTextFormat Interface;

    public TextFormat ( string fontFamily, FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch, float fontSize, CultureInfo culture, TextAlignment textAlignment, ParagraphAlignment paragraphAlignment, WordWrapping wordWrapping, ReadingDirection readingDirection, FlowDirection flowDirection, float incrementalTabStop, TextTrimming trimming, Rune trimmingDelimiter, uint trimmingDelimiterCount, LineSpacingMethod lineSpacingMethod, float lineSpacing, float baseline )
    {
        ArgumentNullException.ThrowIfNull ( fontFamily );
        ArgumentNullException.ThrowIfNull ( culture );

        if ( fontWeight is default ( FontWeight ) )
            throw new ArgumentOutOfRangeException ( nameof ( fontWeight ), fontWeight, "Invalid font weight value" );

        FontFamily = fontFamily;
        FontWeight = fontWeight;
        FontStyle = fontStyle;
        FontStretch = fontStretch;
        FontSize = fontSize;
        Culture = culture;
        TextAlignment = textAlignment;
        ParagraphAlignment = paragraphAlignment;
        WordWrapping = wordWrapping;
        ReadingDirection = readingDirection;
        FlowDirection = flowDirection;
        IncrementalTabStop = incrementalTabStop;
        Trimming = trimming;
        TrimmingDelimiter = trimmingDelimiter;
        TrimmingDelimiterCount = trimmingDelimiterCount;
        LineSpacingMethod = lineSpacingMethod;
        LineSpacing = lineSpacing;
        Baseline = baseline;

        DirectX.DWrite.CreateTextFormat (fontFamily,
                                  null,
                                  (DWRITE_FONT_WEIGHT) fontWeight,
                                  (DWRITE_FONT_STYLE) fontStyle,
                                  (DWRITE_FONT_STRETCH) fontStretch,
                                  fontSize,
                                  culture.Name,
                                  out Interface );

        Interface.SetFlowDirection ( (DWRITE_FLOW_DIRECTION) flowDirection );
        if ( incrementalTabStop > 0f )
            Interface.SetIncrementalTabStop ( incrementalTabStop );
        Interface.SetLineSpacing ( (DWRITE_LINE_SPACING_METHOD) lineSpacingMethod, lineSpacing, baseline );
        Interface.SetParagraphAlignment ( (DWRITE_PARAGRAPH_ALIGNMENT) lineSpacingMethod );
        Interface.SetReadingDirection ( (DWRITE_READING_DIRECTION) readingDirection );
        Interface.SetTextAlignment ( (DWRITE_TEXT_ALIGNMENT) textAlignment );
        Interface.SetTrimming ( new DWRITE_TRIMMING {  granularity = (DWRITE_TRIMMING_GRANULARITY) trimming,
                                                       delimiter = (uint) trimmingDelimiter.Value,
                                                       delimiterCount = trimmingDelimiterCount }, null );
        Interface.SetWordWrapping ( (DWRITE_WORD_WRAPPING) wordWrapping );
    }

    public string FontFamily { get; }
    public FontWeight FontWeight { get; }
    public FontStyle FontStyle { get; }
    public FontStretch FontStretch { get; }
    public float FontSize { get; }
    public CultureInfo Culture { get; }

    public TextAlignment TextAlignment { get; }
    public ParagraphAlignment ParagraphAlignment { get; }
    public WordWrapping WordWrapping { get; }
    public ReadingDirection ReadingDirection { get; }
    public FlowDirection FlowDirection { get; }
    public float IncrementalTabStop { get; }
    
    public TextTrimming Trimming { get; }
    public Rune TrimmingDelimiter { get; }
    public uint TrimmingDelimiterCount { get; }
    // public Inline? TrimmingSign { get; }


    public LineSpacingMethod LineSpacingMethod { get; }
    public float LineSpacing { get; }
    public float Baseline { get; }
}

public readonly struct Matrix : IEquatable < Matrix >
{
    public static Matrix Identity { get; } = new Matrix ( 1f, 0f, 0f, 1f, 0f, 0f );

    public Matrix ( float m11, float m12, float m21, float m22, float offsetX, float offsetY )
    {
        M11     = m11;
        M12     = m12;
        M21     = m21;
        M22     = m22;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the horizontal scaling / cosine of rotation.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float M11 { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the vertical shear / sine of rotation.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float M12 { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the horizontal shear / negative sine of rotation.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float M21 { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the vertical scaling / cosine of rotation.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float M22 { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the horizontal shift (always orthogonal regardless of rotation).</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float OffsetX { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value indicating the vertical shift (always orthogonal regardless of rotation.)</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_matrix#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float OffsetY { get; }

    public override bool Equals ( object? obj )
    {
        return obj is Matrix other && Equals ( other );
    }

    public bool Equals ( Matrix other )
    {
        return M11 == other.M11 && M12 == other.M12 &&
               M21 == other.M21 && M22 == other.M22 &&
               OffsetX == other.OffsetX &&
               OffsetY == other.OffsetY;
    }

    public override int GetHashCode ( )
    {
        return HashCode.Combine ( M11, M12, M21, M22, OffsetX, OffsetY );
    }

    public static bool operator == ( Matrix left, Matrix right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( Matrix left, Matrix right )
    {
        return ! ( left == right );
    }
}

public class TextLayout
{
    public TextLayout ( string text, TextFormat format, float pixelsPerDip, Matrix? transform, bool useGdiNatural ) : this ( text, format, int.MaxValue, int.MaxValue, pixelsPerDip, transform, useGdiNatural ) { }
    public TextLayout ( string text, TextFormat format, int width, int height, float pixelsPerDip, Matrix? transform, bool useGdiNatural )
    {
        ArgumentNullException.ThrowIfNull ( text );
        ArgumentNullException.ThrowIfNull ( format );

        var t = (DWRITE_MATRIX?) null;
        if ( transform is { } tt )
            t = new DWRITE_MATRIX { m11 = tt.M11, m12 = tt.M12, m21 = tt.M21, m22 = tt.M22, dx = tt.OffsetX, dy = tt.OffsetY };

        DirectX.DWrite.CreateGdiCompatibleTextLayout ( text, (uint) text.Length, format.Interface, width, height, pixelsPerDip, t, new BOOL(useGdiNatural), out Interface );

        Text = text;
        Format = format;
        Width = width;
        Height = height;
        PixelsPerDip = pixelsPerDip;
        Transform = transform ?? Matrix.Identity;
        UseGdiNatural = useGdiNatural;
    }

    internal IDWriteTextLayout Interface;

    public string Text { get; set; }
    public TextFormat Format { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float PixelsPerDip { get; set; }
    public Matrix Transform { get; set; }
    public bool UseGdiNatural { get; set; }

    private float? minWidth;
    public float MinWidth => minWidth ??= DetermineMinWidth ( );

    private TextMetrics? metrics;
    public TextMetrics Metrics => metrics ??= GetTextMetrics ( );

    private OverhangMetrics? overhangMetrics;
    public OverhangMetrics OverhangMetrics => overhangMetrics ??= GetOverhangMetrics ( );

    private IReadOnlyList < LineMetrics >? lineMetrics;
    public IReadOnlyList < LineMetrics > LineMetrics => lineMetrics ??= GetLineMetrics ( );

    private IReadOnlyList < ClusterMetrics >? clusterMetrics;
    public IReadOnlyList < ClusterMetrics > ClusterMetrics => clusterMetrics ??= GetClusterMetrics ( );

    // TODO: Range methods + Interface.GetInlineObject ( 0, out var x, ) for the TrimmingSign

    private float DetermineMinWidth ( )
    {
        Interface.DetermineMinWidth ( out var minWidth );

        return minWidth;
    }

    private TextMetrics GetTextMetrics ( )
    {
        Interface.GetMetrics ( out var me );

        return new TextMetrics ( me.left, me.top, me.width, me.widthIncludingTrailingWhitespace, me.height, me.layoutWidth, me.layoutHeight, me.maxBidiReorderingDepth, me.lineCount );
    }

    private OverhangMetrics GetOverhangMetrics ( )
    {
        Interface.GetOverhangMetrics ( out var o );

        return new OverhangMetrics ( o.left, o.top, o.right, o.bottom );
    }

    // TODO: This is not working
    private LineMetrics [ ] GetLineMetrics ( )
    {
        var a = Array.Empty < DWRITE_LINE_METRICS > ( );

        Interface.GetLineMetrics ( a.AsSpan ( ), out var me );

        var b = new LineMetrics [ me ];

        var c = MemoryMarshal.Cast < LineMetrics, DWRITE_LINE_METRICS > ( b );

        Interface.GetLineMetrics ( c, out _ );

        return b;
    }

    // TODO: This is not working
    private ClusterMetrics [ ] GetClusterMetrics ( )
    {
        var a = Array.Empty < DWRITE_CLUSTER_METRICS > ( );

        Interface.GetClusterMetrics ( a.AsSpan ( ), out var me );

        var b = new ClusterMetrics [ me ];

        var c = MemoryMarshal.Cast < ClusterMetrics, DWRITE_CLUSTER_METRICS > ( b );

        Interface.GetClusterMetrics ( c, out _ );

        return b;
    }
}

public readonly struct OverhangMetrics : IEquatable<OverhangMetrics>
{
	/// <summary>
	/// <para>Type: <b>FLOAT</b> The distance from the left-most visible DIP to its left-alignment edge.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_overhang_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Left { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The distance from the top-most visible DIP to its top alignment edge.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_overhang_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Top { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The distance from the right-most visible DIP to its right-alignment edge.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_overhang_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Right { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The distance from the bottom-most visible DIP to its lower-alignment edge.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_overhang_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Bottom { get; }

    public OverhangMetrics ( float left, float top, float right, float bottom )
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public override bool Equals ( object? obj ) => obj is OverhangMetrics other && Equals ( other );
    public override int GetHashCode ( ) => HashCode.Combine ( Left, Top, Right, Bottom );

    public bool Equals ( OverhangMetrics other )
    {
        return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    }

    public static bool operator == ( OverhangMetrics left, OverhangMetrics right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( OverhangMetrics left, OverhangMetrics right )
    {
        return !( left == right );
    }
}

public readonly struct ClusterMetrics : IEquatable<ClusterMetrics>
{
    public ClusterMetrics ( float width, ushort length, ushort bitField )
    {
        Width = width;
        Length = length;
        BitField = bitField;
    }

    /// <summary>
    /// <para>Type: <b>FLOAT</b> The total advance width of all glyphs in the cluster.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_cluster_metrics#members">Read more on docs.microsoft.com</see>.</para>
    /// </summary>
    public float Width { get; }

	/// <summary>
	/// <para>Type: <b>UINT16</b> The number of text positions in the cluster.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_cluster_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ushort Length { get; }

	public ushort BitField { get; }

    public override bool Equals ( object? obj ) => obj is ClusterMetrics other && Equals ( other );
    public override int GetHashCode ( ) => HashCode.Combine ( Width, Length, BitField );

    public bool Equals ( ClusterMetrics other )
    {
        return Width == other.Width && Length == other.Length && BitField == other.BitField;
    }

    public static bool operator == ( ClusterMetrics left, ClusterMetrics right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( ClusterMetrics left, ClusterMetrics right )
    {
        return !( left == right );
    }
}

public readonly struct LineMetrics : IEquatable<LineMetrics>
{
    public LineMetrics ( uint length, uint trailingWhitespaceLength, uint newlineLength, float height, float baseline, bool isTrimmed )
    {
        Length = length;
        TrailingWhitespaceLength = trailingWhitespaceLength;
        NewlineLength = newlineLength;
        Height = height;
        Baseline = baseline;
        IsTrimmed = isTrimmed;
    }

	public uint Length { get; }
	public uint TrailingWhitespaceLength { get; }
	public uint NewlineLength { get; }
	public float Height { get; }
	public float Baseline { get; }
	public bool IsTrimmed { get; }

    public override bool Equals ( object? obj ) => obj is LineMetrics other && Equals ( other );
    public override int GetHashCode ( ) => HashCode.Combine ( Length, TrailingWhitespaceLength, NewlineLength, Height, Baseline, IsTrimmed );

    public bool Equals ( LineMetrics other )
    {
        return  Length == other.Length && TrailingWhitespaceLength == other.TrailingWhitespaceLength && NewlineLength == other.NewlineLength && Height == other.Height && Baseline == other.Baseline && IsTrimmed == other.IsTrimmed;
    }

    public static bool operator == ( LineMetrics left, LineMetrics right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( LineMetrics left, LineMetrics right )
    {
        return !( left == right );
    }
}

public readonly struct TextMetrics : IEquatable<TextMetrics>
{
    public TextMetrics ( float left, float top, float width, float widthIncludingTrailingWhitespace, float height, float layoutWidth, float layoutHeight, uint maxBidiReorderingDepth, uint lineCount )
    {
        Left = left;
        Top = top;
        Width = width;
        WidthIncludingTrailingWhitespace = widthIncludingTrailingWhitespace;
        Height = height;
        LayoutWidth = layoutWidth;
        LayoutHeight = layoutHeight;
        MaxBidiReorderingDepth = maxBidiReorderingDepth;
        LineCount = lineCount;
    }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value that indicates the left-most point of formatted text relative to the layout box, while excluding any glyph overhang.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Left { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value that indicates the top-most point of formatted text relative to the layout box, while excluding any glyph overhang.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Top { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> A value that indicates the width of the formatted text, while ignoring trailing whitespace at the end of each line.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Width { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The width of the formatted text, taking into account the trailing whitespace at the end of each line.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float WidthIncludingTrailingWhitespace { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The height of the formatted text. The height of an empty string is set to the same value as that of the default font.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float Height { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The initial width given to the layout. It can be either larger or smaller than the text content width, depending on whether the text was wrapped.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float LayoutWidth { get; }

	/// <summary>
	/// <para>Type: <b>FLOAT</b> Initial height given to the layout. Depending on the length of the text, it may be larger or smaller than the text content height.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float LayoutHeight { get; }

	/// <summary>
	/// <para>Type: <b>UINT32</b> The maximum reordering count of any line of text, used to calculate the most number of hit-testing boxes needed. If the layout has no bidirectional text, or no text at all, the minimum level is 1.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MaxBidiReorderingDepth { get; }

	/// <summary>
	/// <para>Type: <b>UINT32</b> Total number of lines.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/dwrite/ns-dwrite-dwrite_text_metrics#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint LineCount { get; }

    public override bool Equals ( object? obj ) => obj is TextMetrics other && Equals ( other );

    public override int GetHashCode ( )
    {
        var hash = new HashCode ( );
        hash.Add ( Left );
        hash.Add ( Top );
        hash.Add ( Width );
        hash.Add ( WidthIncludingTrailingWhitespace );
        hash.Add ( Height );
        hash.Add ( LayoutWidth );
        hash.Add ( LayoutHeight );
        hash.Add ( MaxBidiReorderingDepth );
        hash.Add ( LineCount );
        return hash.ToHashCode ( );
    }

    public bool Equals ( TextMetrics other )
    {
        return Left == other.Left && Top == other.Top && Width == other.Width && WidthIncludingTrailingWhitespace == other.WidthIncludingTrailingWhitespace && Height == other.Height && LayoutWidth == other.LayoutWidth && LayoutHeight == other.LayoutHeight && MaxBidiReorderingDepth == other.MaxBidiReorderingDepth && LineCount == other.LineCount;
    }

    public static bool operator == ( TextMetrics left, TextMetrics right )
    {
        return left.Equals ( right );
    }

    public static bool operator != ( TextMetrics left, TextMetrics right )
    {
        return !( left == right );
    }
}