using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Stylish;

[ Ambient, Localizability ( LocalizationCategory.Ignore ), UsableDuringInitialization ( true ) ]
[ SuppressMessage ( "Design", "CA1010:Generic interface should also be implemented", Justification = "ResourceDictionary" ) ]
public class Fonts : ResourceDictionary
{
    private static readonly Uri FontsAssemblyUri = new Uri ( "pack://application:,,,/Stylish.Fonts;component/" );

    private const string FontsAssemblyName = "Stylish.Fonts";

    private const string RegularFontName = "FluentSystemIcons-Regular";
    private const string FilledFontName  = "FluentSystemIcons-Filled";
    private const string SegoeFontName   = "Segoe Fluent Icons, Segoe MDL2 Assets";
    private const string EmojiFontName   = "Segoe UI Emoji";
    private const string EmojiFontKey    = "Emoji.Font";

    public Fonts ( )
    {
        var fonts = TryLoadFontsAssembly ( );

        if ( fonts is not null )
        {
            var regular = new FontFamily ( FontsAssemblyUri, "./#" + RegularFontName );
            var filled  = new FontFamily ( FontsAssemblyUri, "./#" + FilledFontName  );

            Add ( FluentIcon.RegularFontKey, regular );
            Add ( FluentIcon.FilledFontKey,  filled  );
        }
        else
        {
            var regular = new FontFamily ( RegularFontName );
            var filled  = new FontFamily ( FilledFontName  );

            Add ( FluentIcon.RegularFontKey, regular );
            Add ( FluentIcon.FilledFontKey,  filled  );
        }

        var segoe = new FontFamily ( SegoeFontName );

        Add ( SegoeIcon.FontKey, segoe );

        var emoji = new FontFamily ( EmojiFontName );

        Add ( new FontResourceKey ( EmojiFontKey ), emoji );
    }

    private static Assembly? TryLoadFontsAssembly ( )
    {
        try
        {
            return Assembly.Load ( FontsAssemblyName );
        }
        catch ( FileNotFoundException )
        {
            return null;
        }
    }
}