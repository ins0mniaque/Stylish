using System.Runtime.CompilerServices;

using Windows.Win32;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Stylish.DirectWrite.Brushes;

public class SolidColorBrush : Brush
{
    private readonly ID2D1SolidColorBrush brush;

    public SolidColorBrush ( RenderTarget renderTarget, Color color, float opacity = 1f, Matrix? transform = null )
    {
        ArgumentNullException.ThrowIfNull ( renderTarget );

        Color     = color;
        Opacity   = opacity;
        Transform = transform ?? Matrix.Identity;

        var matrix     = Transform;
        var properties = new D2D1_BRUSH_PROPERTIES { opacity   = opacity,
                                                     transform = Unsafe.As < Matrix, D2D_MATRIX_3X2_F > ( ref matrix ) };

        renderTarget.Interface.CreateSolidColorBrush ( Unsafe.As < Color, D2D1_COLOR_F > ( ref color ), properties, out brush );
    }

    public Color  Color     { get; }
    public float  Opacity   { get; }
    public Matrix Transform { get; }

    internal override ID2D1Brush Interface => brush;
}