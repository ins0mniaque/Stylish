using Windows.Win32.Graphics.Direct2D;

namespace Stylish.DirectWrite.Brushes;

public abstract class Brush
{
    internal abstract ID2D1Brush Interface { get; }
}