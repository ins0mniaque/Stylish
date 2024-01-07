using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

using Microsoft.CSharp;

namespace Stylish;

public static class FluentSymbolGenerator
{
    public static async Task < IReadOnlyCollection < FluentSymbolData > > Generate ( FluentSymbolDataSource dataSource, string destination, CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull ( dataSource );

        var symbols = new List < FluentSymbolData > ( );
        var version = await dataSource.ReadVersion ( cancellationToken ).ConfigureAwait ( false );

        using var regularSource = dataSource.OpenRegular ( );
        using var filledSource  = dataSource.OpenFilled  ( );

        await foreach ( var regular in FluentSymbolData.Parse ( regularSource, cancellationToken ) )
            symbols.Add ( FixData ( regular ) );

        await foreach ( var filled in FluentSymbolData.Parse ( filledSource, cancellationToken ) )
            symbols.Add ( FixData ( filled ) );

        using var fluentSymbolFile = File.CreateText ( Path.Combine ( destination, "FluentSymbol.cs" ) );

        await fluentSymbolFile.GenerateHeader  ( "Fluent UI System Icons", version, cancellationToken ).ConfigureAwait ( false );
        await fluentSymbolFile.GenerateSymbols ( symbols,                           cancellationToken ).ConfigureAwait ( false );

        return symbols.ToArray ( );
    }

    private static async Task GenerateHeader ( this TextWriter destination, string summary, string version, CancellationToken cancellationToken )
    {
        await WriteLine ( "// <auto-generated />" );
        await WriteLine ( "namespace Stylish.Symbols;" );
        await WriteLine ( );

        await WriteLine ( "/// <summary>" );
        await WriteLine ( "/// {0} (Version {1:0.0})", summary, version );
        await WriteLine ( "/// </summary>" );
        await WriteLine ( "/// <remarks>" );
        await WriteLine ( "/// References: {0}", string.Join ( " / ", FluentSymbolDataSource.GetReferences ( version ).Select ( reference => string.Format ( CultureInfo.InvariantCulture, "<see href=\"{1}\">{0}</see>", reference.Name, reference.Uri ) ) ) );
        await WriteLine ( "/// </remarks>" );

        ConfiguredTaskAwaitable WriteLine ( string? format = null, params object [  ] args )
        {
            return destination.WriteLineAsync ( string.Format ( CultureInfo.InvariantCulture, format ?? string.Empty, args ).AsMemory ( ), cancellationToken ).ConfigureAwait ( false );
        }
    }

    private static async Task GenerateSymbols ( this TextWriter destination, IReadOnlyCollection < FluentSymbolData > symbols, CancellationToken cancellationToken )
    {
        await WriteLine ( "public enum FluentSymbol" );
        await WriteLine ( "{{" );
        await WriteLine ( "    None," );

        var identifiers = new HashSet < string > ( symbols.Count );

        foreach ( var symbol in symbols )
        {
            var alt        = 1;
            var identifier = CreateIdentifier ( symbol.Name ) + ( symbol.Variant == "filled" ? "Filled" : "" ) + symbol.Size;
            while ( ! identifiers.Add ( identifier ) )
                identifier = CreateIdentifier ( symbol.Name ) + string.Concat ( Enumerable.Repeat ( "Alt", alt++ ) ) + ( symbol.Variant == "filled" ? "Filled" : "" ) + symbol.Size;

            await WriteLine ( );
            await WriteLine ( "    /// <summary>{0}: \\u{1:X4}</summary>", FormatSummary ( symbol.Name, symbol.Variant, symbol.Size ), symbol.Value );
            await WriteLine ( "    {0} = 0x{1:X4},", identifier, symbol.Value | ( symbol.Variant == "filled" ? 0x100000 : 0x00000 ) );
        }

        await destination.WriteAsync ( "}".AsMemory ( ), cancellationToken ).ConfigureAwait ( false );
        await destination.FlushAsync ( cancellationToken )                  .ConfigureAwait ( false );

        ConfiguredTaskAwaitable WriteLine ( string? format = null, params object [  ] args )
        {
            return destination.WriteLineAsync ( string.Format ( CultureInfo.InvariantCulture, format ?? string.Empty, args ).AsMemory ( ), cancellationToken ).ConfigureAwait ( false );
        }
    }

    private static FluentSymbolData FixData ( FluentSymbolData symbol )
    {
        if ( symbol.Name.StartsWith ( "Textbox", StringComparison.Ordinal ) )
            return symbol with { Name = "Text box" + symbol.Name [ 7.. ] };

        if ( symbol.Name.StartsWith ( "Reorder", StringComparison.Ordinal ) )
            return symbol with { Name = "Re order" + symbol.Name [ 7.. ] };

        return symbol;
    }

    private static string FormatSummary ( string name, string variant, int size )
    {
        return SecurityElement.Escape ( $"{ name } ({ ToTitleCase ( variant ) }), size { size }" );
    }

    private static readonly CSharpCodeProvider codeProvider   = new ( );
    private static readonly char [ ]           wordSeparators = [ ' ' ];

    private static string CreateIdentifier ( string name )
    {
        var words = name.Split    ( wordSeparators, StringSplitOptions.RemoveEmptyEntries )
                        .Select   ( ToTitleCase );

        return codeProvider.CreateValidIdentifier ( RemoveDiacritics ( string.Concat ( words ) ) );
    }

    private static string ToTitleCase      ( string text ) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase ( text );
    private static string RemoveDiacritics ( string text ) => string.Concat    ( text.Normalize ( NormalizationForm.FormD )
                                                                                     .Where     ( c => CharUnicodeInfo.GetUnicodeCategory ( c ) is not UnicodeCategory.NonSpacingMark ) )
                                                                    .Normalize ( NormalizationForm.FormC );
}