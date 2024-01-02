using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Metrics;

/// <inheritdoc cref="DWRITE_LINE_METRICS" />
public readonly struct LineMetrics : IEquatable < LineMetrics >
{
    public LineMetrics ( uint length, uint trailingWhitespaceLength, uint newlineLength, float height, float baseline, bool isTrimmed )
    {
        Length                   = length;
        TrailingWhitespaceLength = trailingWhitespaceLength;
        NewlineLength            = newlineLength;
        Height                   = height;
        Baseline                 = baseline;
        IsTrimmed                = isTrimmed;
    }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.length" />
    public uint Length { get; }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.trailingWhitespaceLength" />
    public uint TrailingWhitespaceLength { get; }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.newlineLength" />
    public uint NewlineLength { get; }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.height" />
    public float Height { get; }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.baseline" />
    public float Baseline { get; }

    /// <inheritdoc cref="DWRITE_LINE_METRICS.isTrimmed" />
    public bool IsTrimmed { get; }

    public override int  GetHashCode ( )             => HashCode.Combine ( Length, TrailingWhitespaceLength, NewlineLength, Height, Baseline, IsTrimmed );
    public override bool Equals      ( object? obj ) => obj is LineMetrics other && Equals ( other );
    public          bool Equals      ( LineMetrics other )
    {
        return Length                   == other.Length                   &&
               TrailingWhitespaceLength == other.TrailingWhitespaceLength &&
               NewlineLength            == other.NewlineLength            &&
               Height                   == other.Height                   &&
               Baseline                 == other.Baseline                 &&
               IsTrimmed                == other.IsTrimmed;
    }

    public static bool operator == ( LineMetrics left, LineMetrics right ) => left.Equals ( right );
    public static bool operator != ( LineMetrics left, LineMetrics right ) => ! ( left == right );
}