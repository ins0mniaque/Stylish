using System.Reflection;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Stylish.Tests;

public class SymbolTests
{
    static SymbolTests ( ) => Assembly.Load ( nameof ( Stylish ) );

    [ Fact ]
    public Task InsertsAsInline ( ) => STA ( ( ) =>
    {
        var symbol = new Symbol ( );
        var textBlock = Parse < TextBlock > (
            $"""
            <TextBlock>
                <ß:Symbol Emoji="GrinningFace" />
            </TextBlock>
            """ );

        Assert.Empty  ( textBlock.Text );
        Assert.Single ( textBlock.Inlines );
    } );

    private static T Parse < T > ( string xaml )
    {
        const string xmlns = @" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""" +
                             @" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""" +
                             @" xmlns:ß=""urn:stylish:schema""";

        var insertAt = xaml.IndexOfAny ( new [  ] { ' ', '\t', '\n', '/', '>' } );

        return (T) XamlReader.Parse ( string.Concat ( xaml [ ..insertAt ], xmlns, xaml [ insertAt.. ] ) );
    }

    private static async Task STA ( Action test )
    {
        using var semaphore = new SemaphoreSlim ( 0, 1 );

        var thread = new Thread ( ( ) =>
        {
            test ( );

            semaphore.Release ( );
        } );

        thread.SetApartmentState ( ApartmentState.STA );
        thread.Start ( );

        await semaphore.WaitAsync ( ).ConfigureAwait ( false );
    }
}