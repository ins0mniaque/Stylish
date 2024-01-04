using System.Globalization;
using System.Runtime.CompilerServices;

namespace Stylish.Unicode;

public record UnicodeEmoji ( string Group, string Subgroup, string Name, string Value, string Status, double Version )
{
    public const double LatestVersion = 15.1;

    public static Uri GetSourceUri ( double unicodeVersion = LatestVersion )
    {
        const string SourceUri = "https://unicode.org/Public/emoji/{0:0.0}/emoji-test.txt";

        return new Uri ( string.Format ( CultureInfo.InvariantCulture, SourceUri, unicodeVersion ) );
    }

    public static async Task < Stream > DownloadSource ( double unicodeVersion = LatestVersion, CancellationToken cancellationToken = default )
    {
        using var httpClient = new HttpClient ( );

        var response = await httpClient.GetAsync ( GetSourceUri ( unicodeVersion ), cancellationToken ).ConfigureAwait ( false );

        response.EnsureSuccessStatusCode ( );

        return await response.Content.ReadAsStreamAsync ( cancellationToken ).ConfigureAwait ( false );
    }

    public static async IAsyncEnumerable < UnicodeEmoji > Parse ( Stream source, [ EnumeratorCancellation ] CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull ( source );

        const string GroupPrefix    = "# group: ";
        const string SubgroupPrefix = "# subgroup: ";
        const string CommentPrefix  = "#";

        using var reader = new StreamReader ( source );

        var group    = string.Empty;
        var subgroup = string.Empty;

        while ( ! reader.EndOfStream )
        {
            var line = await reader.ReadLineAsync ( cancellationToken ).ConfigureAwait ( false );
            if ( string.IsNullOrWhiteSpace ( line ) )
                continue;

            if      (   line.StartsWith ( GroupPrefix,    StringComparison.Ordinal ) ) group    = line [ GroupPrefix   .Length.. ];
            else if (   line.StartsWith ( SubgroupPrefix, StringComparison.Ordinal ) ) subgroup = line [ SubgroupPrefix.Length.. ];
            else if ( ! line.StartsWith ( CommentPrefix,  StringComparison.Ordinal ) ) yield return ParseEmoji ( group, subgroup, line );
        }
    }

    private static readonly char [ ] separators = [ ';', '#' ];

    private static UnicodeEmoji ParseEmoji ( string group, string subgroup, string line )
    {
        var parts  = line.Split ( separators, 3 );
        var status = parts [ 1 ].Trim ( );

        var versionAndName = parts [ 2 ].Split ( 'E', 2 ) [ 1 ].Split ( ' ', 2 );
        var version        = double.Parse ( versionAndName [ 0 ], CultureInfo.InvariantCulture );

        var name   = char.ToUpperInvariant ( versionAndName [ 1 ] [ 0 ] ) + versionAndName [ 1 ] [ 1.. ];
        var values = parts [ 0 ].Trim   ( )
                                .Split  ( ' ', StringSplitOptions.RemoveEmptyEntries )
                                .Select ( hex => int.Parse ( hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture ) )
                                .Select ( char.ConvertFromUtf32 );

        return new UnicodeEmoji ( group, subgroup, name, string.Concat ( values ), status, version );
    }
}