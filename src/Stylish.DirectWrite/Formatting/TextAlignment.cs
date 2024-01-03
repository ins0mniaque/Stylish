using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Formatting;

/// <inheritdoc cref="DWRITE_TEXT_ALIGNMENT" />
public enum TextAlignment
{
    /// <inheritdoc cref="DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_LEADING" />
    Leading = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_LEADING,

    /// <inheritdoc cref="DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_TRAILING" />
    Trailing = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_TRAILING,

    /// <inheritdoc cref="DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_CENTER" />
    Center = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_CENTER,

    /// <inheritdoc cref="DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_JUSTIFIED" />
    Justified = DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_JUSTIFIED
}