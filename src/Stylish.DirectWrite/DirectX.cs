using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

[ assembly: SupportedOSPlatform ( "windows6.1" ) ]

namespace Stylish.DirectWrite;

public static class DirectX
{
    public static bool IsSupported => dWrite.Value is not null && d2d1.Value is not null;

    public static PixelDensity GetSystemPixelDensity ( )
    {
        D2D1.ReloadSystemMetrics ( );
        D2D1.GetDesktopDpi       ( out var dpiX, out var dpiY );

        return PixelDensity.FromDpi ( dpiX, dpiY );
    }

    public static PixelDensity GetWindowPixelDensity ( nint hwnd )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 14393 ) )
            return PixelDensity.FromDpi ( PInvoke.GetDpiForWindow ( new HWND ( hwnd ) ) );

        if ( OperatingSystem.IsWindowsVersionAtLeast ( 8, 1 ) )
        {
            var monitor = PInvoke.MonitorFromWindow ( new HWND ( hwnd ), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY );
            if ( PInvoke.GetDpiForMonitor ( monitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY ) != HRESULT.S_OK )
                throw new Win32Exception ( );

            return PixelDensity.FromDpi ( dpiX, dpiY );
        }

        var hdc   = PInvoke.GetDC         ( new HWND ( hwnd ) );
        var dpliX = PInvoke.GetDeviceCaps ( hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX );
        var dpliY = PInvoke.GetDeviceCaps ( hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY );
        _         = PInvoke.ReleaseDC     ( new HWND ( hwnd ), hdc );

        return PixelDensity.FromDpi ( dpliX, dpliY );
    }

    private static readonly ThreadLocal < ID2D1Factory?   > d2d1   = new ( CreateD2D1Factory );
    private static readonly Lazy        < IDWriteFactory? > dWrite = new ( CreateDWriteFactory );

    internal static ID2D1Factory   D2D1   => d2d1  .Value ?? throw new Win32Exception ( D2D1FactoryWin32Error   );
    internal static IDWriteFactory DWrite => dWrite.Value ?? throw new Win32Exception ( DWriteFactoryWin32Error );

    private static int D2D1FactoryWin32Error;
    private static int DWriteFactoryWin32Error;

    private static ID2D1Factory? CreateD2D1Factory ( )
    {
        if ( PInvoke.D2D1CreateFactory ( D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof ( ID2D1Factory ).GUID, null, out var pFactory ) == HRESULT.S_OK )
            return (ID2D1Factory) pFactory;

        D2D1FactoryWin32Error = Marshal.GetLastWin32Error ( );

        return null;
    }

    private static IDWriteFactory? CreateDWriteFactory ( )
    {
        if ( PInvoke.DWriteCreateFactory ( DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, typeof ( IDWriteFactory ).GUID, out var dFactory ) == HRESULT.S_OK )
            return (IDWriteFactory) dFactory;

        DWriteFactoryWin32Error = Marshal.GetLastWin32Error ( );

        return null;
    }
}