using System.Globalization;

namespace Stylish;

public class FluentSymbolDataSource
{
    public static IEnumerable < (string Name, Uri Uri) > GetReferences ( string version )
    {
        const string RegularUriFormat = "https://github.com/{0}/{1}/blob/{2}/icons_regular.md";
        const string FilledUriFormat  = "https://github.com/{0}/{1}/blob/{2}/icons_filled.md";

        yield return ("FluentSystemIcons (Regular)", new Uri ( string.Format ( CultureInfo.InvariantCulture, RegularUriFormat, "microsoft", "fluentui-system-icons", version ) ));
        yield return ("FluentSystemIcons (Filled)",  new Uri ( string.Format ( CultureInfo.InvariantCulture, FilledUriFormat,  "microsoft", "fluentui-system-icons", version ) ));
    }

    public FluentSymbolDataSource ( string path )
    {
        Path = path;

        RegularPath = System.IO.Path.Combine ( Path, "FluentSystemIcons-Regular.json" );
        FilledPath  = System.IO.Path.Combine ( Path, "FluentSystemIcons-Filled.json"  );
        VersionPath = System.IO.Path.Combine ( Path, "FluentSystemIcons.version"      );
    }

    public string Path { get; }

    private string RegularPath { get; }
    private string FilledPath  { get; }
    private string VersionPath { get; }

    public static async Task < string [ ] > GetVersions ( CancellationToken cancellationToken = default )
    {
        var tags = await GitTag.Download ( "microsoft", "fluentui-system-icons", cancellationToken ).ConfigureAwait ( false );
        if ( tags is null )
            throw new InvalidOperationException ( );

        return tags.Reverse ( ).Select ( tag => tag.Name ).ToArray ( );
    }

    public async Task Download ( string version, CancellationToken cancellationToken = default )
    {
        const string RegularUriFormat = "https://raw.githubusercontent.com/{0}/{1}/{2}/fonts/FluentSystemIcons-Regular.json";
        const string FilledUriFormat  = "https://raw.githubusercontent.com/{0}/{1}/{2}/fonts/FluentSystemIcons-Filled.json";

        var regularUri = new Uri ( string.Format ( CultureInfo.InvariantCulture, RegularUriFormat, "microsoft", "fluentui-system-icons", version ) );
        var filledUri  = new Uri ( string.Format ( CultureInfo.InvariantCulture, FilledUriFormat,  "microsoft", "fluentui-system-icons", version ) );

        await Download ( regularUri, RegularPath, cancellationToken ).ConfigureAwait ( false );
        await Download ( filledUri,  FilledPath,  cancellationToken ).ConfigureAwait ( false );

        await File.WriteAllTextAsync ( VersionPath, version, cancellationToken ).ConfigureAwait ( false );
    }

    public Stream OpenRegular ( ) => File.Open ( RegularPath, FileMode.Open );
    public Stream OpenFilled  ( ) => File.Open ( FilledPath,  FileMode.Open );

    public Task < string > ReadVersion ( CancellationToken cancellationToken = default )
    {
        return File.ReadAllTextAsync ( VersionPath, cancellationToken );
    }

    private static async Task Download ( Uri uri, string destination, CancellationToken cancellationToken )
    {
        using var httpClient = new HttpClient ( );

        var response = await httpClient.GetAsync ( uri, cancellationToken ).ConfigureAwait ( false );

        response.EnsureSuccessStatusCode ( );

        var stream = await response.Content.ReadAsStreamAsync ( cancellationToken ).ConfigureAwait ( false );

        using var file = File.Create ( destination );

        await stream.CopyToAsync ( file, cancellationToken ).ConfigureAwait ( false );
    }
}