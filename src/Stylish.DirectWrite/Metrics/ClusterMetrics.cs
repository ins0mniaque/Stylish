using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Metrics;

/// <inheritdoc cref="DWRITE_CLUSTER_METRICS" />
public readonly struct ClusterMetrics : IEquatable < ClusterMetrics >
{
    public ClusterMetrics ( float width, ushort length, ushort bitField )
    {
        Width    = width;
        Length   = length;
        BitField = bitField;
    }

    /// <inheritdoc cref="DWRITE_CLUSTER_METRICS.width" />
    public float Width { get; }

    /// <inheritdoc cref="DWRITE_CLUSTER_METRICS.length" />
    public ushort Length { get; }

    /// <inheritdoc cref="DWRITE_CLUSTER_METRICS._bitfield" />
    public ushort BitField { get; }

    public override int  GetHashCode ( )             => HashCode.Combine ( Width, Length, BitField );
    public override bool Equals      ( object? obj ) => obj is ClusterMetrics other && Equals ( other );
    public          bool Equals      ( ClusterMetrics other )
    {
        return Width == other.Width && Length == other.Length && BitField == other.BitField;
    }

    public static bool operator == ( ClusterMetrics left, ClusterMetrics right ) => left.Equals ( right );
    public static bool operator != ( ClusterMetrics left, ClusterMetrics right ) => ! ( left == right );
}