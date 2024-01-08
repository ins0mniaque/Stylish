using Stylish.Symbols;

namespace Stylish.Controls;

public class FluentIconExtension : FontIconExtension < FluentIcon, FluentSymbol >
{
    public FluentIconExtension ( )                     { }
    public FluentIconExtension ( FluentSymbol symbol ) { Symbol = symbol; }
}