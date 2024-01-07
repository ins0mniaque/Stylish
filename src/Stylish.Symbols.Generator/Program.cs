const string DestinationDirectory = "..\\..\\..\\..\\Stylish\\Symbols";
const string DataDirectory        = "..\\..\\..\\Data";

var fluentSymbols = (IReadOnlyCollection < FluentSymbolData >) [ ];
var segoeSymbols  = (IReadOnlyCollection < SegoeSymbolData >)  [ ];

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    var fluentSymbolDataSource = new FluentSymbolDataSource ( DataDirectory );
    var segoeSymbolDataSource  = new SegoeSymbolDataSource  ( DataDirectory );

    if ( AnsiConsole.Confirm ( "Download latest source data?" ) )
    {
        var versions = await FluentSymbolDataSource.GetVersions ( ).ConfigureAwait ( false );
        if ( versions is null )
            throw new InvalidOperationException ( );

        var prompt  = new SelectionPrompt < string > ( ).AddChoices ( versions );
        var version = AnsiConsole.Prompt ( prompt );

        if ( ! string.IsNullOrEmpty ( version ) )
            await AnsiConsole.Status     ( )
                             .Spinner    ( Spinner.Known.BouncingBar )
                             .StartAsync ( "Downloading source data...", Download )
                             .ConfigureAwait ( false );

        async Task Download ( StatusContext context )
        {
            await fluentSymbolDataSource.Download ( version ).ConfigureAwait ( false );
            await segoeSymbolDataSource .Download ( )        .ConfigureAwait ( false );
        }
    }

    await AnsiConsole.Status     ( )
                     .Spinner    ( Spinner.Known.BouncingBar )
                     .StartAsync ( "Generating symbols...", Generate )
                     .ConfigureAwait ( false );

    async Task Generate ( StatusContext context )
    {
        fluentSymbols = await FluentSymbolGenerator.Generate ( fluentSymbolDataSource, DestinationDirectory ).ConfigureAwait ( false );
        segoeSymbols  = await SegoeSymbolGenerator .Generate ( segoeSymbolDataSource,  DestinationDirectory ).ConfigureAwait ( false );
    }
}
catch ( Exception exception )
when  ( exception is IOException or InvalidOperationException )
{
    AnsiConsole.WriteException ( exception );

    return -1;
}

var tree = new Tree ( $"📁 { Path.GetFullPath ( DestinationDirectory ) }" );

tree.AddNode ( "📄 FluentSymbol.cs" );
tree.AddNode ( "📄 SegoeSymbol.cs" );

var table = new Table ( ).SimpleBorder ( )
                         .BorderColor  ( Color.Grey );

table.AddColumn ( new TableColumn ( "Overview" ) );
table.AddColumn ( new TableColumn ( string.Empty ).Footer ( $"[grey]2 files, { fluentSymbols.Count + segoeSymbols.Count } symbols[/]" ) );
table.AddRow    ( new Markup ( "[yellow]Files[/]" ), tree );

AnsiConsole.Write ( table );

return 0;