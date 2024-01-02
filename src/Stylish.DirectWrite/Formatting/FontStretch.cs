using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Graphics.DirectWrite;

namespace Stylish.DirectWrite.Formatting;

/// <inheritdoc cref="DWRITE_FONT_STRETCH" />
[ SuppressMessage ( "Design", "CA1027:Mark enums with FlagsAttribute",        Justification = "DWRITE_FONT_STRETCH" ) ]
[ SuppressMessage ( "Design", "CA1069:Enums values should not be duplicated", Justification = "DWRITE_FONT_STRETCH" ) ]
[ SuppressMessage ( "Design", "CA1008:Enums should have zero value",          Justification = "DWRITE_FONT_STRETCH" ) ]
public enum FontStretch
{
    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_CONDENSED" />
    UltraCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_CONDENSED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_CONDENSED" />
    ExtraCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_CONDENSED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_CONDENSED" />
    Condensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_CONDENSED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_CONDENSED" />
    SemiCondensed = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_CONDENSED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL" />
    Normal = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_MEDIUM" />
    Medium = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_MEDIUM,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_EXPANDED" />
    SemiExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_SEMI_EXPANDED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXPANDED" />
    Expanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXPANDED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_EXPANDED" />
    ExtraExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_EXTRA_EXPANDED,

    /// <inheritdoc cref="DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_EXPANDED" />
    UltraExpanded = DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_ULTRA_EXPANDED
}