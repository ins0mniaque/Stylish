using System.Windows;

using Stylish.Symbols;

namespace Stylish;

public class FluentIcon : FontIcon < FluentSymbol >
{
    public static FontResourceKey RegularFontKey { get; } = new FontResourceKey ( "FluentIcon.Regular.Font" );
    public static FontResourceKey FilledFontKey  { get; } = new FontResourceKey ( "FluentIcon.Filled.Font" );

    public FluentIcon ( )
    {
        SetResourceReference ( FontFamilyProperty, RegularFontKey );
    }

    protected override void OnSymbolChanged ( DependencyPropertyChangedEventArgs e )
    {
        if ( Symbol.GetVariant ( ) is FluentSymbolVariant.Filled )
            SetResourceReference ( FontFamilyProperty, FilledFontKey );
        else
            SetResourceReference ( FontFamilyProperty, RegularFontKey );

        Glyph = Symbol.ToGlyph ( );
    }
}