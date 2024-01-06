using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stylish;

[ SuppressMessage ( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Serialization" ) ]
public sealed record GitTag ( string Ref, [ property: JsonPropertyName ( "Node_Id" ) ] string NodeId, Uri Url )
{
    public string Name => Ref.Replace ( "refs/tags/", string.Empty, StringComparison.Ordinal ).Trim ( );

    public static async Task < IReadOnlyList < GitTag > > Download ( string owner, string repository, CancellationToken cancellationToken = default )
    {
        const string UriFormat = "https://api.github.com/repos/{0}/{1}/git/refs/tags";

        var uri = new Uri ( string.Format ( CultureInfo.InvariantCulture, UriFormat, owner, repository ) );

        using var httpClient = new HttpClient ( );

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd ( "Mozilla/5.0 (compatible; AcmeInc/1.0)" );

        return await httpClient.GetFromJsonAsync < IReadOnlyList < GitTag > > ( uri, cancellationToken ).ConfigureAwait ( false ) ??
               throw new JsonException ( "Error parsing git tags" );
    }
}