using System.Windows;

using Stylish.Symbols;

namespace Stylish;

public class Icon : FontIcon < SegoeFluentIcons >
{
    public static FontResourceKey FontKey           { get; } = new FontResourceKey ( "Icon.Font" ); 
    public static FontResourceKey BackgroundFontKey { get; } = new FontResourceKey ( "Icon.Background.Font" ); 

    public Icon ( )
    {
        SetResourceReference ( FontFamilyProperty,           FontKey           );
        SetResourceReference ( BackgroundFontFamilyProperty, BackgroundFontKey );
    }

    protected override void OnSymbolChanged ( DependencyPropertyChangedEventArgs e )
    {
        Glyph = Symbol.ToUnicode ( );
    }
}