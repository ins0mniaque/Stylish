using System.ComponentModel;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Dxgi.Common;
using Windows.Win32.Graphics.Gdi;

namespace Stylish.DirectWrite;

public sealed class Surface : RenderTarget, IDisposable
{
    private readonly ID2D1DCRenderTarget renderTarget;
    private readonly HDC                 memoryDc;
    private readonly HGDIOBJ             oldBitmap;
    private          HBITMAP             bitmap;

    public Surface ( int width, int height, float dpiX, float dpiY ) : this ( width, height, PixelDensity.FromDpi ( dpiX, dpiY ) ) { }
    public Surface ( int width, int height, PixelDensity pixelDensity )
    {
        Width        = width;
        Height       = height;
        PixelDensity = pixelDensity;
        PixelWidth   = pixelDensity.ToPixelWidth  ( width );
        PixelHeight  = pixelDensity.ToPixelHeight ( height );
        BitsPerPixel = 32;

        var properties = new D2D1_RENDER_TARGET_PROPERTIES { pixelFormat = new ( ) { format    = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                                                                                     alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED },
                                                             dpiX  = pixelDensity.PerInchX,
                                                             dpiY  = pixelDensity.PerInchY,
                                                             usage = D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE };

        DirectX.D2D1.CreateDCRenderTarget ( properties, out renderTarget );

        memoryDc = ReplaceBitmap ( memoryDc, out oldBitmap );
    }

    internal override ID2D1RenderTarget Interface => renderTarget;

    public int Width  { get; private set; }
    public int Height { get; private set; }

    public PixelDensity PixelDensity { get; private set; }

    public int PixelWidth   { get; private set; }
    public int PixelHeight  { get; private set; }
    public int BitsPerPixel { get; }

    public int  Length  => PixelHeight * ( ( ( PixelWidth * BitsPerPixel + 31 ) & ~31 ) >> 3 );
    public nint HDC     => memoryDc.Value;
    public nint HBitmap => bitmap  .Value;

    public unsafe void CopyTo ( byte [ ] buffer )            => CopyTo ( buffer.AsSpan ( ) );
    public unsafe void CopyTo ( byte [ ] buffer, int index ) => CopyTo ( buffer.AsSpan ( ) [ index.. ] );
    public unsafe void CopyTo ( Span < byte > span )
    {
        fixed ( byte* buffer = span )
            CopyTo ( buffer, span.Length * sizeof ( byte ) );
    }

    private unsafe void CopyTo ( void* buffer, int length )
    {
        if ( length < Length )
            throw new ArgumentException ( "Buffer too small", nameof ( buffer ) );

        var info = new BITMAPINFO ( ) { bmiHeader = new ( )
        {
            biSize     = (uint) sizeof ( BITMAPINFOHEADER ),
            biWidth    = PixelWidth,
            biHeight   = PixelHeight,
            biPlanes   = 1,
            biBitCount = (ushort) BitsPerPixel
        } };

        try
        {
            PInvoke.SelectObject ( memoryDc, oldBitmap );

            var count = PInvoke.GetDIBits ( memoryDc, bitmap, 0, (uint) PixelHeight, buffer, &info, DIB_USAGE.DIB_RGB_COLORS );
            if ( count is 0 )
                throw new Win32Exception ( );
        }
        finally
        {
            PInvoke.SelectObject ( memoryDc, bitmap );
        }
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

    private HDC ReplaceBitmap ( HDC memoryDC, out HGDIOBJ oldBitmap )
    {
        var hwnd = PInvoke.GetDesktopWindow ( );
        var hDC  = PInvoke.GetWindowDC ( hwnd );

        if ( memoryDC.Value is (nint) 0 )
            memoryDC = PInvoke.CreateCompatibleDC ( hDC );

        bitmap = PInvoke.CreateCompatibleBitmap ( hDC, PixelWidth, PixelHeight );

        _ = PInvoke.ReleaseDC ( hwnd, hDC );

        oldBitmap = PInvoke.SelectObject ( memoryDC, bitmap );

        renderTarget.BindDC ( memoryDC, new RECT ( 0, 0, PixelWidth, PixelHeight ) );

        return memoryDC;
    }

    public void Dispose ( )
    {
        if ( bitmap.Value is not (nint) 0 )
        {
            PInvoke.SelectObject ( memoryDc, oldBitmap );
            PInvoke.DeleteObject ( bitmap );
            PInvoke.DeleteDC     ( memoryDc );

            bitmap = default;
        }
    }
}