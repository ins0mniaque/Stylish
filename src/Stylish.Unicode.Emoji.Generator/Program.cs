const string Destination = "..\\..\\..\\..\\Stylish.Unicode.Emoji";

var emojis = (IReadOnlyCollection < UnicodeEmoji >) Array.Empty < UnicodeEmoji > ( );

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    await AnsiConsole.Status     ( )
                     .Spinner    ( Spinner.Known.BouncingBar )
                     .StartAsync ( "Generating emojis...",
                                   async ctx => emojis = await EmojiGenerator.Generate ( Destination )
                                                                             .ConfigureAwait ( false ) )
                     .ConfigureAwait ( false );
}
catch ( Exception exception )
when  ( exception is IOException or InvalidOperationException )
{
    AnsiConsole.WriteException ( exception );

    return -1;
}

var tree = new Tree ( $"📁 { Path.GetFullPath ( Destination ) }" );

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