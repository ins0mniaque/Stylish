using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Stylish.Interop;

public static partial class Dwm
{
    public static bool UseImmersiveDarkMode ( nint handle, bool enabled )
    {
        const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 17763 ) )
        {
            var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18985 ) )
                attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;

            var value = enabled ? 1 : 0;

            return DwmSetWindowAttribute ( handle, attribute, ref value, Marshal.SizeOf < int > ( ) ) is S_OK;
        }

        return false;
    }

    public static bool SetSystemBackdrop ( nint hwnd, Material material )
    {
        var attribute = 0;
        var value     = 0;

        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 22523 ) )
        {
            const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

            const int DWMSBT_AUTO            = 0;
            const int DWMSBT_NONE            = 1;
            const int DWMSBT_MAINWINDOW      = 2;
            const int DWMSBT_TRANSIENTWINDOW = 3;
            const int DWMSBT_TABBEDWINDOW    = 4;

            attribute = DWMWA_SYSTEMBACKDROP_TYPE;
            value     = material switch
            {
                Material.Auto    => DWMSBT_AUTO,
                Material.None    => DWMSBT_NONE,
                Material.Mica    => DWMSBT_MAINWINDOW,
                Material.Acrylic => DWMSBT_TRANSIENTWINDOW,
                Material.MicaAlt => DWMSBT_TABBEDWINDOW,
                _                => DWMSBT_AUTO
            };
        }
        else if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 22000 ) )
        {
            const int DWMWA_MICA_EFFECT = 1029;

            attribute = DWMWA_MICA_EFFECT;
            value     = material switch
            {
                Material.Auto    => 1,
                Material.None    => 0,
                Material.Mica    => 1,
                Material.Acrylic => 1,
                Material.MicaAlt => 1,
                _                => 0
            };
        }

        var success = attribute is not 0 && DwmSetWindowAttribute ( hwnd, attribute, ref value, Marshal.SizeOf < int > ( ) ) is S_OK;

        if ( success && material is not Material.None )
            RemoveBackground ( hwnd );
        else
            RestoreBackground ( hwnd );

        return success;
    }

    private static void RemoveBackground ( nint hwnd )
    {
        var hwndSource = HwndSource.FromHwnd ( hwnd );

        if ( hwndSource.CompositionTarget is { } compositionTarget )
            compositionTarget.BackgroundColor = Colors.Transparent;

        if ( hwndSource.RootVisual is Window window )
        {
            StoreRemovedBackground ( window );

            window.SetCurrentValue ( Window.BackgroundProperty, Brushes.Transparent );
        }
    }

    private static void RestoreBackground ( nint hwnd )
    {
        var hwndSource = HwndSource.FromHwnd ( hwnd );
        var surface    = SystemColors.WindowColor;

        if ( hwndSource.RootVisual is Window window )
        {
            RestoreRemovedBackground ( window );

            surface = window.Background is SolidColorBrush brush && brush.Color.A is not 0 ? brush.Color : surface;
        }

        if ( hwndSource.CompositionTarget is { } compositionTarget && compositionTarget.BackgroundColor.A is 0 )
            compositionTarget.BackgroundColor = surface;
    }

    private static void StoreRemovedBackground ( Window window )
    {
        if ( window.GetValue ( RemovedBackgroundPropertyKey.DependencyProperty ) is null )
            window.SetValue ( RemovedBackgroundPropertyKey, window.ReadLocalValue ( Window.BackgroundProperty ) );
    }

    private static void RestoreRemovedBackground ( Window window )
    {
        if ( window.ReadLocalValue ( RemovedBackgroundPropertyKey.DependencyProperty ) is { } background && background != DependencyProperty.UnsetValue )
        {
            window.SetCurrentValue ( Window.BackgroundProperty, background );
            window.ClearValue      ( RemovedBackgroundPropertyKey );
        }
    }

    private static readonly DependencyPropertyKey RemovedBackgroundPropertyKey = DependencyProperty.RegisterAttachedReadOnly ( "RemovedBackground", typeof ( object ), typeof ( Dwm ), new ( ) );

    private const int S_OK = 0;

    [ StructLayout ( LayoutKind.Sequential ) ]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    #if ! NET6_0
    [ LibraryImport ( "dwmapi.dll" ) ]
    private static partial int DwmSetWindowAttribute ( nint hwnd, int attribute, ref int value, int size );
    #else
    [ DllImport ( "dwmapi.dll" ) ]
    private static extern int DwmSetWindowAttribute ( nint hwnd, int attribute, ref int value, int size );
    #endif
}