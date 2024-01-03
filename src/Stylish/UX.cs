using System.IO;
using System.Windows;
using System.Windows.Media;

using Microsoft.Win32;

using Stylish.Interop;

namespace Stylish;

public record UX ( string Name, Mode Mode, Color Window, Color WindowText, Color Accent, Color AccentText )
{
    static UX ( )
    {
        SystemEvents.UserPreferenceChanged += Refresh;
    }

    public static event EventHandler? HighContrastChanged;
    public static event EventHandler? CurrentChanged;
    public static event EventHandler? SystemChanged;

    private static UX? light;
    public  static UX  Light => light ??= Read ( Mode.Light );

    private static UX? dark;
    public  static UX  Dark => dark ??= Read ( Mode.Dark );

    private static UX? highContrast;
    public  static UX  HighContrast => highContrast ??= Read ( Mode.HighContrast );

    private static UX? current;
    public  static UX  Current => current ??= Get ( GetCurrentMode ( ) );

    private static UX? system;
    public  static UX  System => system ??= Get ( GetSystemMode ( ) );

    public static UX Get ( Mode mode ) => mode switch
    {
        Mode.Light        => Light,
        Mode.Dark         => Dark,
        Mode.HighContrast => HighContrast,
        _                 => Light
    };

    private static void Refresh ( object sender, UserPreferenceChangedEventArgs e )
    {
        if ( light is not null ) TrySetTheme ( ref light, Read ( Mode.Light ) );
        if ( dark  is not null ) TrySetTheme ( ref dark,  Read ( Mode.Dark  ) );

        if ( highContrast is not null && TrySetTheme ( ref highContrast, Read ( Mode.HighContrast ) ) )
            HighContrastChanged?.Invoke ( null, EventArgs.Empty );

        if ( current is not null && TrySetTheme ( ref current, Get ( GetCurrentMode ( ) ) ) )
            CurrentChanged?.Invoke ( null, EventArgs.Empty );

        if ( system is not null && TrySetTheme ( ref system, Get ( GetSystemMode ( ) ) ) )
            SystemChanged?.Invoke ( null, EventArgs.Empty );
    }

    private static Mode GetCurrentMode ( ) => SystemParameters.HighContrast ? Mode.HighContrast : UXTheme.ShouldAppsUseDarkMode   ( ) ? Mode.Dark : Mode.Light;
    private static Mode GetSystemMode  ( ) => SystemParameters.HighContrast ? Mode.HighContrast : UXTheme.ShouldSystemUseDarkMode ( ) ? Mode.Dark : Mode.Light;

    private static UX Read ( Mode mode )
    {
        if ( mode is Mode.HighContrast )
        {
            if ( UXTheme.GetCurrentThemePath ( ) is not { } currentTheme )
                return DefaultHighContrastTheme;

            var name       = UXTheme.ReadThemeName  ( currentTheme )                ?? Path.GetFileNameWithoutExtension ( currentTheme );
            var window     = UXTheme.ReadThemeColor ( currentTheme, "Window"      ) ?? DefaultHighContrastTheme.Window;
            var windowText = UXTheme.ReadThemeColor ( currentTheme, "WindowText"  ) ?? DefaultHighContrastTheme.WindowText;
            var accent     = UXTheme.ReadThemeColor ( currentTheme, "Hilight"     ) ?? DefaultHighContrastTheme.Accent;
            var accentText = UXTheme.ReadThemeColor ( currentTheme, "HilightText" ) ?? DefaultHighContrastTheme.AccentText;

            return new UX ( name, Mode.HighContrast, window, windowText, accent, accentText );
        }
        else
        {
            var window     = mode is Mode.Light ? LightWindow : DarkWindow;
            var windowText = GetTextColor ( window );
            var accent     = SystemParameters.WindowGlassColor;
            var accentText = GetTextColor ( accent );

            return new UX ( mode.ToString ( ), mode, window, windowText, accent, accentText );
        }
    }

    private static bool TrySetTheme ( ref UX? field, UX? value )
    {
        var changed = ! object.Equals ( field, value );
        if ( changed )
            field = value;

        return changed;
    }

    public static Color GetTextColor ( Color c ) => IsLightColor ( c ) ? LightText : DarkText;
    public static bool  IsLightColor ( Color c ) => 5 * c.G + 2 * c.R + c.B <= 8 * 128;
    public static bool  IsDarkColor  ( Color c ) => 5 * c.G + 2 * c.R + c.B >  8 * 128;

    private static Color LightWindow { get; } = Color.FromRgb ( 0xFA, 0xFA, 0xFA );
    private static Color LightText   { get; } = Colors.White;
    private static Color DarkWindow  { get; } = Color.FromRgb ( 0x20, 0x20, 0x20 );
    private static Color DarkText    { get; } = Colors.Black;

    private static UX DefaultHighContrastTheme { get; } = new
    (
        "Desert", Mode.HighContrast,
        Color.FromRgb ( 0xFF, 0xFA, 0xEF ),
        Color.FromRgb ( 0x3D, 0x3D, 0x3D ),
        Color.FromRgb ( 0x90, 0x39, 0x09 ),
        Color.FromRgb ( 0xFF, 0xF5, 0xE3 )
    );
}