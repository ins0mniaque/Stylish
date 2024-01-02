using Windows.Win32.Graphics.Direct2D.Common;

namespace Stylish.DirectWrite.Brushes;

/// <inheritdoc cref="D2D1_COLOR_F" />
public readonly struct Color : IEquatable < Color >
{
    public Color ( float a, float r, float g, float b )
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public float R { get; }
    public float G { get; }
    public float B { get; }
    public float A { get; }

    public override int  GetHashCode ( )               => HashCode.Combine ( R, G, B, A );
    public override bool Equals      ( object? obj   ) => obj is Color other && Equals ( other );
    public          bool Equals      ( Color   other ) => R == other.R && G == other.G && B == other.B && A == other.A;

    public static bool operator == ( Color left, Color right ) => left.Equals ( right );
    public static bool operator != ( Color left, Color right ) => ! ( left == right );
}