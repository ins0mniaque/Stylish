using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Stylish;

public record FluentSymbolData ( string Name, string Variant, int Size, int Value )
{
    public static async IAsyncEnumerable < FluentSymbolData > Parse ( Stream source, [ EnumeratorCancellation ] CancellationToken cancellationToken = default )
    {
        var entries = await JsonSerializer.DeserializeAsync < IReadOnlyDictionary < string, int > > ( source, cancellationToken: cancellationToken ).ConfigureAwait ( false ) ??
                      throw new JsonException ( "Error parsing symbols" );

        foreach ( var entry in entries )
        {
            var parts = entry.Key.Replace ( "ic_fluent_", string.Empty, StringComparison.Ordinal ).Split ( '_' );

            parts [ 0 ] = char.ToUpperInvariant ( parts [ 0 ] [ 0 ] ) + parts [ 0 ] [ 1.. ];

            yield return new FluentSymbolData ( string.Join ( ' ', parts [ ..^2 ] ), parts [ ^1 ], int.Parse ( parts [ ^2 ], CultureInfo.InvariantCulture ), entry.Value );
        }
    }
}