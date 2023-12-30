using System.Windows;

using Stylish.Fonts;

namespace Stylish;

public class Emoji : FontIcon < Unicode.Emoji >
{
    public static FontResourceKey FontKey { get; } = new FontResourceKey ( "Emoji.Font" ); 

    public Emoji ( )
    {
        SetResourceReference ( FontFamilyProperty, FontKey );
    }

    protected override void OnSymbolChanged ( DependencyPropertyChangedEventArgs e ) => Glyph = Symbol.ToUnicode ( );
}