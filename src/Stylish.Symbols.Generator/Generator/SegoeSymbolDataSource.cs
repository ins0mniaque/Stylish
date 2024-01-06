namespace Stylish;

public class SegoeSymbolDataSource
{
    public SegoeSymbolDataSource ( string path )
    {
        Path = path;

        FluentPath = System.IO.Path.Combine ( Path, "segoe-fluent-icons-font.md" );
        MDL2Path   = System.IO.Path.Combine ( Path, "segoe-ui-symbol-font.md"  );
    }

    public string Path { get; }

    private string FluentPath  { get; }
    private string MDL2Path    { get; }

    public async Task Download ( CancellationToken cancellationToken = default )
    {
        const string FluentUri = "https://raw.githubusercontent.com/MicrosoftDocs/windows-dev-docs/docs/hub/apps/design/style/segoe-fluent-icons-font.md";
        const string MDL2Uri   = "https://raw.githubusercontent.com/MicrosoftDocs/windows-dev-docs/docs/hub/apps/design/style/segoe-ui-symbol-font.md";

        var fluentUri = new Uri ( FluentUri );
        var mdl2Uri   = new Uri ( MDL2Uri );

        await Download ( fluentUri, FluentPath, cancellationToken ).ConfigureAwait ( false );
        await Download ( mdl2Uri,   MDL2Path,   cancellationToken ).ConfigureAwait ( false );
    }

    public Stream OpenFluent ( ) => File.Open ( FluentPath, FileMode.Open );
    public Stream OpenMDL2   ( ) => File.Open ( MDL2Path,   FileMode.Open );

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