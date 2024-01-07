using System.Windows;

using Stylish.Symbols;

namespace Stylish;

public class SegoeIcon : FontIcon < SegoeSymbol >
{
    public static FontResourceKey FontKey { get; } = new FontResourceKey ( "SegoeIcon.Font" );

    public SegoeIcon ( )
    {
        SetResourceReference ( FontFamilyProperty, FontKey );
    }

    protected override void OnSymbolChanged ( DependencyPropertyChangedEventArgs e )
    {
        Glyph = Symbol.ToGlyph ( );
    }
}