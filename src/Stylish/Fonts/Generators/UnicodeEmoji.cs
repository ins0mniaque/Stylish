using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Stylish.Fonts.Generators;

[ SuppressMessage ( "Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>" ) ]
public record UnicodeEmoji ( string Group, string Subgroup, string Name, string Value, string Status, double Version )
{
    public const double LatestVersion = 15.1;

    public static Uri GetSourceUri ( double unicodeVersion = LatestVersion )
    {
        const string SourceUrl = "https://unicode.org/Public/emoji/{0:0.0}/emoji-test.txt";

        return new Uri ( string.Format ( CultureInfo.InvariantCulture, SourceUrl, unicodeVersion ) );
    }

    public static async IAsyncEnumerable < UnicodeEmoji > Download ( double unicodeVersion = LatestVersion, [ EnumeratorCancellation ] CancellationToken cancellationToken = default )
    {
        using var httpClient = new HttpClient ( );

        var response = await httpClient.GetAsync ( GetSourceUri ( unicodeVersion ), cancellationToken );

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync ( cancellationToken );
        using var reader = new StreamReader ( stream );

        await foreach ( var emoji in Parse ( reader, cancellationToken ).WithCancellation ( cancellationToken ) )
            yield return emoji;
    }

    public static async IAsyncEnumerable < UnicodeEmoji > Parse ( StreamReader reader, [ EnumeratorCancellation ] CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull ( reader );

        const string groupPrefix = "# group: ";
        const string subgroupPrefix = "# subgroup: ";
        const string commentPrefix = "#";

        var group = string.Empty;
        var subgroup = string.Empty;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith(groupPrefix, StringComparison.Ordinal))
            {
                group = line.Substring(groupPrefix.Length);
                continue;
            }

            if (line.StartsWith(subgroupPrefix, StringComparison.Ordinal))
            {
                subgroup = line.Substring(subgroupPrefix.Length);
                continue;
            }

            if (line.StartsWith(commentPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            yield return ParseEmoji(group, subgroup, line);
        }
    }

    private static UnicodeEmoji ParseEmoji(string group, string subgroup, string line)
    {
        var parts = line.Split(new[] { ';', '#' }, 3);
        var status = parts[1].Trim();

        var versionAndName = parts[2].Split('E', 2)[1].Split(' ', 2);
        var version = double.Parse(versionAndName[0], CultureInfo.InvariantCulture);

        var name = char.ToUpperInvariant ( versionAndName[1][0] ) + versionAndName[1][1..];

        var surrogates = parts[0].Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
            .Select(x => char.ConvertFromUtf32(x));

        var value = string.Concat(surrogates);

        return new UnicodeEmoji (group, subgroup, name, value, status, version);
    }
}
