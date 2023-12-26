using System.Windows;
using System.Windows.Interop;

using Stylish.Interop;

namespace Stylish;

public static class Setting
{
    [ AttachedPropertyBrowsableForType ( typeof ( Window ) ) ]
    public static Mode GetMode ( this Window window )
    {
        ArgumentNullException.ThrowIfNull ( window );

        return (Mode) window.GetValue ( ModeProperty );
    }

    public static void SetMode ( this Window window, Mode mode )
    {
        ArgumentNullException.ThrowIfNull ( window );

        window.SetValue ( ModeProperty, mode );
    }

    public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached ( "Mode", typeof ( Mode ), typeof ( Setting ), new ( Mode.Light, OnModeChanged ) );

    private static void OnModeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        var window = (Window) d;
        var hwnd   = new WindowInteropHelper ( window ).Handle;
        var dark   = e.NewValue is Mode.Dark;

        if ( hwnd is not 0 )
        {
            Dwm    .UseImmersiveDarkMode ( hwnd, dark );
            UXTheme.SetPreferredAppMode  ( dark ? PreferredAppMode.AllowDark : PreferredAppMode.Default );

        }
        else
            window.SourceInitialized += SetMode;

        static void SetMode ( object? sender, EventArgs e )
        {
            var window = (Window) sender!;
            var hwnd   = new WindowInteropHelper ( window ).Handle;
            var dark   = GetMode ( window ) is Mode.Dark;

            window.SourceInitialized -= SetMode;

            Dwm    .UseImmersiveDarkMode ( hwnd, dark );
            UXTheme.SetPreferredAppMode  ( dark ? PreferredAppMode.AllowDark : PreferredAppMode.Default );
        }
    }

    [ AttachedPropertyBrowsableForType ( typeof ( Window ) ) ]
    public static Material GetMaterial ( this Window window )
    {
        ArgumentNullException.ThrowIfNull ( window );

        return (Material) window.GetValue ( MaterialProperty );
    }

    public static void SetMaterial ( this Window window, Material material )
    {
        ArgumentNullException.ThrowIfNull ( window );

        window.SetValue ( MaterialProperty, material );
    }

    public static readonly DependencyProperty MaterialProperty = DependencyProperty.RegisterAttached ( "Material", typeof ( Material ), typeof ( Setting ), new ( Material.Auto, OnMaterialChanged ) );

    private static void OnMaterialChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        var window = (Window) d;
        var hwnd   = new WindowInteropHelper ( window ).Handle;

        if ( hwnd is not 0 )
            Dwm.SetSystemBackdrop ( hwnd, (Material) e.NewValue );
        else
            window.SourceInitialized += SetSystemBackdropType;

        static void SetSystemBackdropType ( object? sender, EventArgs e )
        {
            var window = (Window) sender!;
            var hwnd   = new WindowInteropHelper ( window ).Handle;

            window.SourceInitialized -= SetSystemBackdropType;

            Dwm.SetSystemBackdrop ( hwnd, GetMaterial ( window ) );
        }
    }
}