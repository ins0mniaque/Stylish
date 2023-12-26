using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

namespace Stylish;

[ Ambient, Localizability ( LocalizationCategory.Ignore ), UsableDuringInitialization ( true ) ]
[ SuppressMessage ( "Design", "CA1010:Generic interface should also be implemented", Justification = "ResourceDictionary" ) ]
public class ThemeResourceDictionary : ResourceDictionary
{
    private Theme? theme;
    public  Theme? Theme
    {
        get => theme;
        set
        {
            if ( theme is not null )
            {
                theme.Changed -= ApplyTheme;

                if ( value is null )
                    theme.Unapply ( this );
            }

            theme = value;

            if ( theme is not null )
            {
                theme.Changed += ApplyTheme;

                theme.Apply ( this );
            }
        }
    }

    private void ApplyTheme ( object? sender, EventArgs e ) => Theme?.Apply ( this );
}