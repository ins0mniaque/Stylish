using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Stylish;

[ MarkupExtensionReturnType ( typeof ( Geometry ) ) ]
public class TextGeometry : MarkupExtension
{
    [ Category ( "Appearance" ) ]
    public string? Text { get; set; }

    /// <inheritdoc cref="FrameworkElement.FlowDirection" />
    [ Category ( "Appearance" ) ]
    public FlowDirection FlowDirection { get; set; }

    /// <inheritdoc cref="Control.FontFamily" />
    [ Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.Font ) ]
    public FontFamily FontFamily { get; set; } = SystemFonts.MessageFontFamily;

    /// <inheritdoc cref="Control.FontSize" />
    [ TypeConverter ( typeof ( FontSizeConverter ) ) ]
    [ Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.None ) ]
    public double FontSize { get; set; } = SystemFonts.MessageFontSize;

    /// <inheritdoc cref="Control.FontStretch" />
    [ Category ( "Appearance" ) ]
    public FontStretch FontStretch { get; set; } = FontStretches.Normal;

    /// <inheritdoc cref="Control.FontStyle" />
    [ Category ( "Appearance" ) ]
    public FontStyle FontStyle { get; set; } = FontStyles.Normal;

    /// <inheritdoc cref="Control.FontWeight" />
    [ Category ( "Appearance" ) ]
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;

    [ Category ( "Appearance" ) ]
    public double? PixelsPerDip { get; set; }

    [ Category ( "Appearance" ) ]
    public TextFormattingMode TextFormattingMode { get; set; }

    public override object ProvideValue ( IServiceProvider serviceProvider )
    {
        if ( Text is null )
            return Geometry.Empty;

        var pixelsPerDip = PixelsPerDip ?? 1.0; // TODO: GetDC ( 0 ), GetDeviceCaps ( NativeMethods.LOGPIXELSY ), ReleaseDC ( ); / 96.0;
        var typeface     = new Typeface      ( FontFamily, FontStyle, FontWeight, FontStretch );
        var formatted    = new FormattedText ( Text, CultureInfo.CurrentUICulture, FlowDirection,
                                               typeface, FontSize, Brushes.Black, null, TextFormattingMode, pixelsPerDip );

        return formatted.BuildGeometry ( default );
    }
}