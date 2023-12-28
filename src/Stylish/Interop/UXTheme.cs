using System.Runtime.InteropServices;
using System.Windows.Media;

using Microsoft.Win32;

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

    // TODO: Rename this...
    public static bool ShouldSystemUseDarkMode2 ( )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            return ShouldSystemUseDarkMode ( );

        var systemUsesLightTheme = Registry.GetValue ( ThemesKey + "\\Personalize", "SystemUsesLightTheme", null );

        return systemUsesLightTheme is 0;
    }

    public static bool ShouldAppsUseDarkMode2 ( )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            return ShouldAppsUseDarkMode ( );

        var appsUseLightTheme = Registry.GetValue ( ThemesKey + "\\Personalize", "AppsUseLightTheme", null );

        return appsUseLightTheme is 0 ? true  :
               appsUseLightTheme is 1 ? false :
               ShouldSystemUseDarkMode ( );
    }

    public static void SetPreferredAppMode ( PreferredAppMode appMode )
    {
        if ( OperatingSystem.IsWindowsVersionAtLeast ( 10, 0, 18362 ) )
            SetPreferredAppMode ( (int) appMode );
    }

    public static bool AllowDarkModeForWindow2 ( nint hwnd, bool enabled )
    {
        return AllowDarkModeForWindow ( hwnd, enabled );
    }

    public static string? GetCurrentThemePath ( )
    {
        return Registry.GetValue ( ThemesKey, "CurrentTheme", "aero.theme" ) as string;
    }

    public static string? ReadThemeName ( string path )
    {
        const int MAX_PATH = 260;

        var buffer = new char [ MAX_PATH ];
        var length = (int) GetPrivateProfileString ( "Theme", "DisplayName", null, buffer, (uint) buffer.Length, path );

        if ( length <= 0 )
            return null;

        var name = new string ( buffer, 0, length );
        if ( name [ 0 ] is not '@' )
            return name;

        length = SHLoadIndirectString ( Environment.ExpandEnvironmentVariables ( name ), buffer, buffer.Length, (nint) 0 );
        if ( length <= 0 )
            return null;

        return new string ( buffer, 0, length );
    }

    public static Color? ReadThemeColor ( string path, string key )
    {
        var buffer = new char [ 16 ];
        var length = (int) GetPrivateProfileString ( "Control Panel\\Colors", key, null, buffer, (uint) buffer.Length, path );

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

    #if ! NET6_0
    [ LibraryImport ( "UXTheme.dll", EntryPoint = "#132" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static partial bool ShouldAppsUseDarkMode ( );

    [ LibraryImport ( "UXTheme.dll", EntryPoint = "#138" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static partial bool ShouldSystemUseDarkMode ( );

    [ LibraryImport ( "UXTheme.dll", EntryPoint = "#135" ) ]
    private static partial void SetPreferredAppMode ( int mode );

    [ LibraryImport ( "UXTheme.dll", EntryPoint = "#133" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static partial bool AllowDarkModeForWindow ( nint hwnd, [ MarshalAs ( UnmanagedType.Bool ) ] bool enabled );

    [ LibraryImport ( "kernel32.dll", EntryPoint = "GetPrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16 ) ]
    private static partial uint GetPrivateProfileString ( string? section, string? key, string? defaultValue, [In, Out] char [ ] buffer, uint size, string path );

    [ LibraryImport ( "shlwapi.dll", StringMarshalling = StringMarshalling.Utf16 ) ]
    private static partial int SHLoadIndirectString ( string source, [In, Out] char [ ] buffer, int size, nint reserved );
    #else
    [ DllImport ( "UXTheme.dll", EntryPoint = "#132" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static extern bool ShouldAppsUseDarkMode ( );

    [ DllImport ( "UXTheme.dll", EntryPoint = "#138" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static extern bool ShouldSystemUseDarkMode ( );

    [ DllImport ( "UXTheme.dll", EntryPoint = "#135" ) ]
    private static extern void SetPreferredAppMode ( int mode );

    [ DllImport ( "UXTheme.dll", EntryPoint = "#133" ) ]
    [ return: MarshalAs ( UnmanagedType.Bool ) ]
    private static extern bool AllowDarkModeForWindow ( nint hwnd, [ MarshalAs ( UnmanagedType.Bool ) ] bool enabled );

    [ DllImport ( "kernel32.dll", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode ) ]
    private static extern uint GetPrivateProfileString ( string? section, string? key, string? defaultValue, [In, Out] char [ ] buffer, uint size, string path );

    [ DllImport ( "shlwapi.dll", CharSet = CharSet.Unicode ) ]
    private static extern int SHLoadIndirectString ( string source, [In, Out] char [ ] buffer, int size, nint reserved );
    #endif
}