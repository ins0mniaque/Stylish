using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Metrics;

/// <inheritdoc cref="DWRITE_TEXT_METRICS" />
public readonly struct TextMetrics : IEquatable < TextMetrics >
{
    public TextMetrics ( float left, float top, float width, float widthIncludingTrailingWhitespace, float height, float layoutWidth, float layoutHeight, uint maxBidiReorderingDepth, uint lineCount )
    {
        Left                             = left;
        Top                              = top;
        Width                            = width;
        WidthIncludingTrailingWhitespace = widthIncludingTrailingWhitespace;
        Height                           = height;
        LayoutWidth                      = layoutWidth;
        LayoutHeight                     = layoutHeight;
        MaxBidiReorderingDepth           = maxBidiReorderingDepth;
        LineCount                        = lineCount;
    }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.left" />
    public float Left { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.top" />
    public float Top { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.width" />
    public float Width { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.widthIncludingTrailingWhitespace" />
    public float WidthIncludingTrailingWhitespace { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.height" />
    public float Height { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.layoutWidth" />
    public float LayoutWidth { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.layoutHeight" />
    public float LayoutHeight { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.maxBidiReorderingDepth" />
    public uint MaxBidiReorderingDepth { get; }

    /// <inheritdoc cref="DWRITE_TEXT_METRICS.lineCount" />
    public uint LineCount { get; }

    public override int GetHashCode ( )
    {
        var hash = new HashCode ( );

        hash.Add ( Left );
        hash.Add ( Top );
        hash.Add ( Width );
        hash.Add ( WidthIncludingTrailingWhitespace );
        hash.Add ( Height );
        hash.Add ( LayoutWidth );
        hash.Add ( LayoutHeight );
        hash.Add ( MaxBidiReorderingDepth );
        hash.Add ( LineCount );

        return hash.ToHashCode ( );
    }

    public override bool Equals ( object? obj ) => obj is TextMetrics other && Equals ( other );
    public          bool Equals ( TextMetrics other )
    {
        return Left                             == other.Left                             &&
               Top                              == other.Top                              &&
               Width                            == other.Width                            &&
               WidthIncludingTrailingWhitespace == other.WidthIncludingTrailingWhitespace &&
               Height                           == other.Height                           &&
               LayoutWidth                      == other.LayoutWidth                      &&
               LayoutHeight                     == other.LayoutHeight                     &&
               MaxBidiReorderingDepth           == other.MaxBidiReorderingDepth           &&
               LineCount                        == other.LineCount;
    }

    public static bool operator == ( TextMetrics left, TextMetrics right ) => left.Equals ( right );
    public static bool operator != ( TextMetrics left, TextMetrics right ) => ! ( left == right );
}