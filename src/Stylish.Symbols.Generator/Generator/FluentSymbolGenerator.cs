namespace Stylish;

public static class FluentSymbolGenerator
{
    public static async Task < FluentSymbolData [ ] > Generate ( FluentSymbolDataSource dataSource, string destination, CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull ( dataSource );

        var symbols = new List < FluentSymbolData > ( );
        var version = await dataSource.ReadVersion ( cancellationToken ).ConfigureAwait ( false );

        using var regularSource = dataSource.OpenRegular ( );
        using var filledSource  = dataSource.OpenFilled  ( );

        await foreach ( var regular in FluentSymbolData.Parse ( regularSource, cancellationToken ) )
            symbols.Add ( regular );

        await foreach ( var filled in FluentSymbolData.Parse ( filledSource, cancellationToken ) )
            symbols.Add ( filled );

        return symbols.ToArray ( );
    }
}