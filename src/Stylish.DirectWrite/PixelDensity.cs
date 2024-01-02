using System.Diagnostics.CodeAnalysis;

namespace Stylish.DirectWrite;

/// <summary>Represents pixel density (DPI) information</summary>
public readonly struct PixelDensity : IEquatable < PixelDensity >
{
    private const float DipDpi   = 96f;
    private const float PointDpi = 72f;

    public static PixelDensity FromDpi ( float dpi )                       => new ( dpi,  dpi  );
    public static PixelDensity FromDpi ( float dpiX,     float dpiY )      => new ( dpiX, dpiY );
    public static PixelDensity FromDip ( float dipWidth, float dipHeight ) => new ( dipWidth * DipDpi, dipHeight * DipDpi );
    public static PixelDensity FromDip ( float dipSize )                   => new ( dipSize  * DipDpi, dipSize   * DipDpi );

    private PixelDensity ( float dpiX, float dpiY )
    {
        dipDpiOffsetX = dpiX - DipDpi;
        dipDpiOffsetY = dpiY - DipDpi;
    }

    private readonly float dipDpiOffsetX;
    private readonly float dipDpiOffsetY;

    /// <summary>DPI along X axis</summary>
    public float PerInchX => dipDpiOffsetX + DipDpi;

    /// <summary>DPI along Y axis</summary>
    public float PerInchY => dipDpiOffsetY + DipDpi;

    /// <summary>Pixels per DIP (device independent pixel) at which text should be rendered</summary>
    public float PerDip => PerDipY;

    /// <summary>Pixels per DIP (device independent pixel) along X axis</summary>
    public float PerDipX => PerInchX / DipDpi;

    /// <summary>Pixels per DIP (device independent pixel) along Y axis</summary>
    public float PerDipY => PerInchY / DipDpi;

    [ SuppressMessage ( "Performance", "CA1822:Mark members as static", Justification = "Discoverability" ) ]
    public float FromFontHeight      ( float height      ) => height      / (          DipDpi / PointDpi );
    public float FromFontPixelHeight ( float pixelHeight ) => pixelHeight / ( PerDip * DipDpi / PointDpi );

    [ SuppressMessage ( "Performance", "CA1822:Mark members as static", Justification = "Discoverability" ) ]
    public float ToFontHeight      ( float fontSize ) => fontSize *          DipDpi / PointDpi;
    public float ToFontPixelHeight ( float fontSize ) => fontSize * PerDip * DipDpi / PointDpi;

    public int FromPixelWidth  ( int pixelWidth  ) => (int) ( pixelWidth  / PerDipX );
    public int FromPixelHeight ( int pixelHeight ) => (int) ( pixelHeight / PerDipY );
    public int ToPixelWidth    ( int width       ) => (int) ( width       * PerDipX );
    public int ToPixelHeight   ( int height      ) => (int) ( height      * PerDipY );

    public override int  GetHashCode ( )                    => HashCode.Combine ( PerInchX, PerInchY );
    public override bool Equals      ( object?      obj   ) => obj is PixelDensity other && Equals ( other );
    public          bool Equals      ( PixelDensity other ) => PerInchX == other.PerInchX && PerInchY == other.PerInchY;

    public static bool operator == ( PixelDensity left, PixelDensity right ) => Equals ( left, right );
    public static bool operator != ( PixelDensity left, PixelDensity right ) => ! ( left == right );
}