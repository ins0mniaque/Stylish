using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Metrics;

/// <inheritdoc cref="DWRITE_OVERHANG_METRICS" />
public readonly struct OverhangMetrics : IEquatable < OverhangMetrics >
{
    public OverhangMetrics ( float left, float top, float right, float bottom )
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }

    /// <inheritdoc cref="DWRITE_OVERHANG_METRICS.left" />
    public float Left { get; }

    /// <inheritdoc cref="DWRITE_OVERHANG_METRICS.top" />
    public float Top { get; }

    /// <inheritdoc cref="DWRITE_OVERHANG_METRICS.right" />
    public float Right { get; }

    /// <inheritdoc cref="DWRITE_OVERHANG_METRICS.bottom" />
    public float Bottom { get; }

    public override int  GetHashCode ( )             => HashCode.Combine ( Left, Top, Right, Bottom );
    public override bool Equals      ( object? obj ) => obj is OverhangMetrics other && Equals ( other );
    public          bool Equals      ( OverhangMetrics other )
    {
        return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    }

    public static bool operator == ( OverhangMetrics left, OverhangMetrics right ) => left.Equals ( right );
    public static bool operator != ( OverhangMetrics left, OverhangMetrics right ) => ! ( left == right );
}