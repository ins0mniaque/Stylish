using System.Windows;
using System.Windows.Interop;

using Stylish.Interop;

namespace Stylish;

public class Window : System.Windows.Window
{
    static Window ( )
    {
        DefaultStyleKeyProperty.OverrideMetadata ( typeof ( Window ), new FrameworkPropertyMetadata ( typeof ( Window ) ) );
    }

    public Window ( )
    {
        SetResourceReference ( StyleProperty, typeof ( Window ) );
    }

    [ AttachedPropertyBrowsableForType ( typeof ( System.Windows.Window ) ) ]
    public static Mode GetMode ( System.Windows.Window window )
    {
        ArgumentNullException.ThrowIfNull ( window );

        return (Mode) window.GetValue ( ModeProperty );
    }

    public static void SetMode ( System.Windows.Window window, Mode mode )
    {
        ArgumentNullException.ThrowIfNull ( window );

        window.SetValue ( ModeProperty, mode );
    }

    public Mode Mode
    {
        get => (Mode) GetValue ( ModeProperty );
        set => SetValue ( ModeProperty, value );
    }

    public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached ( nameof ( Mode ), typeof ( Mode ), typeof ( Window ), new ( Mode.Light, OnModeChanged ) );

    private static void OnModeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        var window = (System.Windows.Window) d;
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
            var window = (System.Windows.Window) sender!;
            var hwnd   = new WindowInteropHelper ( window ).Handle;
            var dark   = GetMode ( window ) is Mode.Dark;

            window.SourceInitialized -= SetMode;

            Dwm    .UseImmersiveDarkMode ( hwnd, dark );
            UXTheme.SetPreferredAppMode  ( dark ? PreferredAppMode.AllowDark : PreferredAppMode.Default );
        }
    }

    [ AttachedPropertyBrowsableForType ( typeof ( System.Windows.Window ) ) ]
    public static Material GetMaterial ( System.Windows.Window window )
    {
        ArgumentNullException.ThrowIfNull ( window );

        return (Material) window.GetValue ( MaterialProperty );
    }

    public static void SetMaterial ( System.Windows.Window window, Material material )
    {
        ArgumentNullException.ThrowIfNull ( window );

        window.SetValue ( MaterialProperty, material );
    }

    public Material Material
    {
        get => (Material) GetValue ( MaterialProperty );
        set => SetValue ( MaterialProperty, value );
    }

    public static readonly DependencyProperty MaterialProperty = DependencyProperty.RegisterAttached ( nameof ( Material ), typeof ( Material ), typeof ( Window ), new ( Material.Auto, OnMaterialChanged ) );

    private static void OnMaterialChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        var window = (System.Windows.Window) d;
        var hwnd   = new WindowInteropHelper ( window ).Handle;

        if ( hwnd is not 0 )
            Dwm.SetSystemBackdrop ( hwnd, (Material) e.NewValue );
        else
            window.SourceInitialized += SetSystemBackdropType;

        static void SetSystemBackdropType ( object? sender, EventArgs e )
        {
            var window = (System.Windows.Window) sender!;
            var hwnd   = new WindowInteropHelper ( window ).Handle;

            window.SourceInitialized -= SetSystemBackdropType;

            Dwm.SetSystemBackdrop ( hwnd, GetMaterial ( window ) );
        }
    }
}