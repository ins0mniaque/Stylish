namespace Stylish;

public static class SegoeSymbolGenerator
{
    public static async Task < SegoeSymbolData [ ] > Generate ( SegoeSymbolDataSource dataSource, string destination, CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull ( dataSource );

        var symbols = new List < SegoeSymbolData > ( );

        using var fluentSource = dataSource.OpenFluent ( );
        using var mdl2Source   = dataSource.OpenMDL2   ( );

        await foreach ( var regular in SegoeSymbolData.Parse ( fluentSource, cancellationToken ) )
            symbols.Add ( regular );

        await foreach ( var filled in SegoeSymbolData.Parse ( mdl2Source, cancellationToken ) )
            symbols.Add ( filled );

        return symbols.ToArray ( );
    }
}