const string DestinationDirectory = "..\\..\\..\\..\\Stylish.Unicode.Emoji";
const string DataDirectory        = "..\\..\\..\\Data";

var emojis = (IReadOnlyCollection < UnicodeEmoji >) [ ];

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    var dataPath = Path.Combine ( DataDirectory, UnicodeEmoji.DataFileName );

    if ( AnsiConsole.Confirm ( "Download latest source data?" ) )
    {
        var unicodeVersion = AnsiConsole.Ask ( "Unicode version?", UnicodeEmoji.LatestVersion );

        await AnsiConsole.Status     ( )
                         .Spinner    ( Spinner.Known.BouncingBar )
                         .StartAsync ( "Downloading emojis...", Download )
                         .ConfigureAwait ( false );

        async Task Download ( StatusContext context )
        {
            using var data     = await UnicodeEmoji.DownloadSource ( unicodeVersion ).ConfigureAwait ( false );
            using var dataFile = File.Create ( dataPath );

            await data.CopyToAsync ( dataFile ).ConfigureAwait ( false );
        }
    }

    using var dataFile = File.Open ( dataPath, FileMode.Open );

    await AnsiConsole.Status     ( )
                     .Spinner    ( Spinner.Known.BouncingBar )
                     .StartAsync ( "Generating emojis...", Generate )
                     .ConfigureAwait ( false );

    async Task Generate ( StatusContext context )
    {
        emojis = await EmojiGenerator.Generate ( dataFile, DestinationDirectory ).ConfigureAwait ( false );
    }
}
catch ( Exception exception )
when  ( exception is IOException or InvalidOperationException )
{
    AnsiConsole.WriteException ( exception );

    return -1;
}

var tree = new Tree ( $"📁 { Path.GetFullPath ( DestinationDirectory ) }" );

tree.AddNode ( "📄 Emoji.cs" );
tree.AddNode ( "📄 EmojiGroup.cs" );
tree.AddNode ( "📄 EmojiMetadata.cs" );
tree.AddNode ( "📄 EmojiSubgroup.cs" );

var table = new Table ( ).SimpleBorder ( )
                         .BorderColor  ( Color.Grey );

table.AddColumn ( new TableColumn ( "Overview" ) );
table.AddColumn ( new TableColumn ( string.Empty ).Footer ( $"[grey]4 files, { emojis.Count } emojis[/]" ) );
table.AddRow    ( new Markup ( "[yellow]Files[/]" ), tree );

AnsiConsole.Write ( table );

return 0;