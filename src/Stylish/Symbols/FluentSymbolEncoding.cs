using System.Text;

namespace Stylish.Symbols;

public static class FluentSymbolEncoding
{
    private const int FilledBit = 0x100000;

    public static FluentSymbolVariant GetVariant ( this FluentSymbol symbol ) => ( (int) symbol & FilledBit ) is 0 ? FluentSymbolVariant.Regular :
                                                                                                                     FluentSymbolVariant.Filled;

    public static Rune   ToRune  ( this FluentSymbol symbol ) => new Rune ( (int) symbol & ~FilledBit );
    public static string ToGlyph ( this FluentSymbol symbol ) => symbol.ToRune ( ).ToString ( );
}
