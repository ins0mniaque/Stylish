using System.Text;

namespace Stylish.Symbols;

public static class SegoeSymbolEncoding
{
    public static Rune   ToRune  ( this SegoeSymbol symbol ) => new Rune ( (int) symbol );
    public static string ToGlyph ( this SegoeSymbol symbol ) => symbol.ToRune ( ).ToString ( );
}