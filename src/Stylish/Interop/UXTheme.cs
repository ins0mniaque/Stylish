using System.Runtime.CompilerServices;
using System.Windows.Media;

using Microsoft.Win32;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace Stylish.Interop;

public enum PreferredAppMode
{
    Default,
    AllowDark,
    ForceDark,
    ForceLight
};

public static partial class UXTheme
{
    private const string ThemesKey = "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes";

    public static bool ShouldSystemUseDarkMode ( )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            return PInvoke.ShouldSystemUseDarkMode ( );

        var systemUsesLightTheme = Registry.GetValue ( ThemesKey + "\\Personalize", "SystemUsesLightTheme", null );

        return systemUsesLightTheme is 0;
    }

    public static bool ShouldAppsUseDarkMode ( )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            return PInvoke.ShouldAppsUseDarkMode ( );

        var appsUseLightTheme = Registry.GetValue ( ThemesKey + "\\Personalize", "AppsUseLightTheme", null );

        return appsUseLightTheme is 0 ? true  :
               appsUseLightTheme is 1 ? false :
               PInvoke.ShouldSystemUseDarkMode ( );
    }

    public static void SetPreferredAppMode ( PreferredAppMode appMode )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            PInvoke.SetPreferredAppMode ( (Windows.Win32.UXTheme.PreferredAppMode) appMode );
    }

    public static bool AllowDarkModeForWindow2 ( nint hwnd, bool enabled )
    {
        return PInvoke.AllowDarkModeForWindow ( hwnd, enabled );
    }

    public static string? GetCurrentThemePath ( )
    {
        return Registry.GetValue ( ThemesKey, "CurrentTheme", "aero.theme" ) as string;
    }

    public static unsafe string? ReadThemeName ( string path )
    {
        const int MAX_PATH = 260;

        var buffer = new char [ MAX_PATH ];
        var pwstr  = new PWSTR ( (char*) Unsafe.AsPointer ( ref buffer [ 0 ] ) );
        var length = (int) PInvoke.GetPrivateProfileString ( "Theme", "DisplayName", null, pwstr, (uint) buffer.Length, path );

        if ( length <= 0 )
            return null;

        var name = new string ( buffer, 0, length );
        if ( name [ 0 ] is not '@' )
            return name;

        length = PInvoke.SHLoadIndirectString ( Environment.ExpandEnvironmentVariables ( name ), pwstr, (uint) buffer.Length );
        if ( length <= 0 )
            return null;

        return new string ( buffer, 0, length );
    }

    public static unsafe Color? ReadThemeColor ( string path, string key )
    {
        var buffer = new char [ 16 ];
        var pwstr  = new PWSTR ( (char*) Unsafe.AsPointer ( ref buffer [ 0 ] ) );
        var length = (int) PInvoke.GetPrivateProfileString ( "Control Panel\\Colors", key, null, pwstr, (uint) buffer.Length, path );

        if ( length <= 0 )
            return null;

        var value  = buffer.AsSpan ( ..length );
        var spaceG = value.IndexOf ( ' ' );
        var spaceB = value [ ( spaceG + 1 ).. ].IndexOf ( ' ' ) + spaceG + 1;
        if ( spaceB <= spaceG )
            return null;

        if ( byte.TryParse ( value [ ..spaceG ],             out var r ) &&
             byte.TryParse ( value [ (spaceG + 1)..spaceB ], out var g ) &&
             byte.TryParse ( value [ (spaceB + 1)..       ], out var b ) )
            return Color.FromRgb ( r, g, b );

        return null;
    }
}