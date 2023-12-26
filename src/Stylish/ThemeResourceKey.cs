using System.Reflection;
using System.Windows;

namespace Stylish;

public class ThemeResourceKey : ResourceKey
{
    public ThemeResourceKey ( string key )
    {
        ArgumentNullException.ThrowIfNull ( key );

        Key      = key;
        HashCode = key.GetHashCode ( StringComparison.Ordinal );
    }

    private string Key      { get; }
    private int    HashCode { get; }

    public override Assembly? Assembly => null;

    public override bool Equals ( object? obj ) => obj is ThemeResourceKey key &&
                                                   key.HashCode == HashCode &&
                                                   key.Key      == Key;

    public override int    GetHashCode ( ) => HashCode;
    public override string ToString    ( ) => Key;
}