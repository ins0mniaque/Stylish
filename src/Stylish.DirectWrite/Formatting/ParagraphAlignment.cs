using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Formatting;

/// <inheritdoc cref="DWRITE_PARAGRAPH_ALIGNMENT" />
public enum ParagraphAlignment
{
    /// <inheritdoc cref="DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_NEAR" />
    Near = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_NEAR,

    /// <inheritdoc cref="DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_FAR" />
    Far = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_FAR,

    /// <inheritdoc cref="DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_CENTER" />
    Center = DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_CENTER,
}