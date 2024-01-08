using Stylish.Symbols;

namespace Stylish;

public class FluentIconExtension : FontIconExtension < FluentIcon, FluentSymbol >
{
    public FluentIconExtension ( )                     { }
    public FluentIconExtension ( FluentSymbol symbol ) { Symbol = symbol; }
}