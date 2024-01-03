using System.Runtime.InteropServices;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        [ DllImport ( "UXTheme.dll", EntryPoint = "#133" ) ]
        [ return: MarshalAs ( UnmanagedType.Bool ) ]
        internal static extern bool AllowDarkModeForWindow ( nint hwnd, [ MarshalAs ( UnmanagedType.Bool ) ] bool enabled );

        [ DllImport ( "UXTheme.dll", EntryPoint = "#135" ) ]
        internal static extern void SetPreferredAppMode ( UXTheme.PreferredAppMode mode );

        [ DllImport ( "UXTheme.dll", EntryPoint = "#132" ) ]
        [ return: MarshalAs ( UnmanagedType.Bool ) ]
        internal static extern bool ShouldAppsUseDarkMode ( );

        [ DllImport ( "UXTheme.dll", EntryPoint = "#138" ) ]
        [ return: MarshalAs ( UnmanagedType.Bool ) ]
        internal static extern bool ShouldSystemUseDarkMode ( );
    }

    namespace UXTheme
    {
        internal enum PreferredAppMode
        {
            Default,
            AllowDark,
            ForceDark,
            ForceLight
        }
    }
}