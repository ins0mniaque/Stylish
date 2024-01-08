using Stylish.Symbols;

namespace Stylish;

public class SegoeIconExtension : FontIconExtension < SegoeIcon, SegoeSymbol >
{
    public SegoeIconExtension ( )                    { }
    public SegoeIconExtension ( SegoeSymbol symbol ) { Symbol = symbol; }
}
