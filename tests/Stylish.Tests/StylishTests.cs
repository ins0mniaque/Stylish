using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Markup;

namespace Stylish.Tests;

public abstract class StylishTests
{
    static StylishTests ( ) => Assembly.Load ( nameof ( Stylish ) );

    private static readonly char [ ] xamlTagEnds = [ ' ', '\t', '\n', '/', '>' ];

    protected static T Parse < T > ( string xaml )
    {
        ArgumentNullException.ThrowIfNull ( xaml );

        const string xmlns = @" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""" +
                             @" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""" +
                             @" xmlns:ß=""urn:stylish:schema""";

        var insertAt = xaml.IndexOfAny ( xamlTagEnds );

        return (T) XamlReader.Parse ( string.Concat ( xaml [ ..insertAt ], xmlns, xaml [ insertAt.. ] ) );
    }

    [ SuppressMessage ( "Design", "CA1031:Do not catch general exception types", Justification = "ExceptionDispatchInfo" ) ]
    protected static async Task STA ( Action action )
    {
        using var semaphore = new SemaphoreSlim ( 0, 1 );

        var error  = (ExceptionDispatchInfo?) null;
        var thread = new Thread ( Run );

        thread.SetApartmentState ( ApartmentState.STA );
        thread.Start ( );

        await semaphore.WaitAsync ( ).ConfigureAwait ( false );

        error?.Throw ( );

        void Run ( )
        {
            try
            {
                action ( );
            }
            catch ( Exception exception )
            {
                error = ExceptionDispatchInfo.Capture ( exception );
            }

            semaphore.Release ( );
        }
    }
}