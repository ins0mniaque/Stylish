using System.ComponentModel;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Stylish.DirectWrite;

public sealed class Window : RenderTarget
{
    private readonly ID2D1HwndRenderTarget renderTarget;
    private readonly HWND                  hwnd;

    internal override ID2D1RenderTarget Interface => renderTarget;

    public Window ( nint hwnd )
    {
        this.hwnd = new HWND ( hwnd );
        if ( ! PInvoke.GetClientRect ( this.hwnd, out var rect ) )
            throw new Win32Exception ( );

        PixelWidth  = rect.Width;
        PixelHeight = rect.Height;

        var properties     = new D2D1_RENDER_TARGET_PROPERTIES ( );
        var hwndProperties = new D2D1_HWND_RENDER_TARGET_PROPERTIES { hwnd           = this.hwnd,
                                                                      pixelSize      = new ( ) { width = (uint) PixelWidth, height = (uint) PixelHeight },
                                                                      presentOptions = D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE };

        DirectX.D2D1.CreateHwndRenderTarget ( properties, hwndProperties, out renderTarget );

        // TODO: Hook resize and dpi changed messages
        //       Add PixelDensity property
    }

    public int PixelWidth  { get; private set; }
    public int PixelHeight { get; private set; }

    public void Resize ( )
    {
        if( ! PInvoke.GetClientRect ( hwnd, out var rect ) )
            throw new Win32Exception ();

        PixelWidth  = rect.Width;
        PixelHeight = rect.Height;

        renderTarget.Resize ( new D2D_SIZE_U ( ) { width = (uint) PixelWidth, height = (uint) PixelHeight } );
    }
}