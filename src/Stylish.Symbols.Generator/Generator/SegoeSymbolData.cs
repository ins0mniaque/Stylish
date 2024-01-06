using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Stylish;

public partial record SegoeSymbolData ( string Name, int Value )
{
    private static readonly Regex parser = GenerateParser ( );

    public static async IAsyncEnumerable < SegoeSymbolData > Parse ( Stream source, [ EnumeratorCancellation ] CancellationToken cancellationToken = default )
    {
        using var reader = new StreamReader ( source );

        var input = await reader.ReadToEndAsync ( cancellationToken ).ConfigureAwait ( false );

        foreach ( Match match in parser.Matches ( input ) )
            yield return new SegoeSymbolData ( match.Groups [ "Name" ].Value.Trim ( ),
                                               int.Parse ( match.Groups [ "Value" ].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture ) );
    }

    [ GeneratedRegex ( @"<td>(?<Value>[a-fA-F0-9]{4,5}?)</td>\s*<td>(?<Name>.*?)</td>\s*</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline ) ]
    private static partial Regex GenerateParser ( );
}