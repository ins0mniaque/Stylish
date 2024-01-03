using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;

namespace Stylish.Interop;

public static partial class Dwm
{
    public static unsafe bool UseImmersiveDarkMode ( nint handle, bool enabled )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 17763 ) )
        {
            const DWMWINDOWATTRIBUTE DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = (DWMWINDOWATTRIBUTE) 19;

            var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18985 ) )
                attribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

            var value = enabled ? 1 : 0;

            return PInvoke.DwmSetWindowAttribute ( new HWND ( handle ), attribute, Unsafe.AsPointer ( ref value ), (uint) Marshal.SizeOf < int > ( ) ) == HRESULT.S_OK;
        }

        return false;
    }

    public static unsafe bool SetSystemBackdrop ( nint hwnd, Material material )
    {
        var attribute = default ( DWMWINDOWATTRIBUTE );
        var value     = 0;

        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 22523 ) )
        {
            attribute = DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE;
            value     = material switch
            {
                Material.Auto    => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_AUTO,
                Material.None    => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_NONE,
                Material.Mica    => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW,
                Material.Acrylic => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW,
                Material.MicaAlt => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TABBEDWINDOW,
                _                => (int) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_AUTO
            };
        }
        else if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 22000 ) )
        {
            const DWMWINDOWATTRIBUTE DWMWA_MICA_EFFECT = (DWMWINDOWATTRIBUTE) 1029;

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

        var success = attribute is not default ( DWMWINDOWATTRIBUTE ) && PInvoke.DwmSetWindowAttribute ( new HWND ( hwnd ), attribute, Unsafe.AsPointer ( ref value ), (uint) Marshal.SizeOf < int > ( ) ) == HRESULT.S_OK;

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
}