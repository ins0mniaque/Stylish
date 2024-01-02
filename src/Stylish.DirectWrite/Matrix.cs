using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite;

/// <inheritdoc cref="D2D_MATRIX_3X2_F" />
public readonly struct Matrix : IEquatable < Matrix >
{
    public static Matrix Identity { get; } = new Matrix ( 1f, 0f, 0f, 1f, 0f, 0f );

    public Matrix ( float m11, float m12, float m21, float m22, float offsetX, float offsetY )
    {
        M11     = m11;
        M12     = m12;
        M21     = m21;
        M22     = m22;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    /// <inheritdoc cref="DWRITE_MATRIX.M11" />
    public float M11 { get; }

    /// <inheritdoc cref="DWRITE_MATRIX.M12" />
    public float M12 { get; }

    /// <inheritdoc cref="DWRITE_MATRIX.M21" />
    public float M21 { get; }

    /// <inheritdoc cref="DWRITE_MATRIX.M22" />
    public float M22 { get; }

    /// <inheritdoc cref="DWRITE_MATRIX.dx" />
    public float OffsetX { get; }

    /// <inheritdoc cref="DWRITE_MATRIX.dy" />
    public float OffsetY { get; }

    public override int  GetHashCode ( )               => HashCode.Combine ( M11, M12, M21, M22, OffsetX, OffsetY );
    public override bool Equals      ( object? obj   ) => obj is Matrix other && Equals ( other );
    public          bool Equals      ( Matrix  other )
    {
        return M11 == other.M11 && M12 == other.M12 &&
               M21 == other.M21 && M22 == other.M22 &&
               OffsetX == other.OffsetX &&
               OffsetY == other.OffsetY;
    }

    public static bool operator == ( Matrix left, Matrix right ) => left.Equals ( right );
    public static bool operator != ( Matrix left, Matrix right ) => ! ( left == right );
}