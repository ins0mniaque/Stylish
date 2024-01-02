using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite;

public class Inline
{
    internal Inline ( IDWriteInlineObject inline )
    {
        Interface = inline;
    }

    internal IDWriteInlineObject Interface { get; }
}