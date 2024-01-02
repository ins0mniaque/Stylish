using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Formatting;

/// <inheritdoc cref="DWRITE_FONT_STYLE" />
public enum FontStyle
{
    /// <inheritdoc cref="DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL" />
    Normal = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL,

    /// <inheritdoc cref="DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_OBLIQUE" />
    Oblique = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_OBLIQUE,

    /// <inheritdoc cref="DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_ITALIC" />
    Italic = DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_ITALIC
}