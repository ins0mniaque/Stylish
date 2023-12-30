using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Stylish;

public class FontIcon : ColorIconElement
{
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public string? Glyph
    {
        get => (string?) GetValue ( GlyphProperty );
        set => SetValue ( GlyphProperty, value );
    }

    public static readonly DependencyProperty GlyphProperty = RegisterVisualProperty < FontIcon, string? > ( nameof ( Glyph ), null );

    [ Bindable ( true ), Category ( "Appearance" ) ]
    public string? BackgroundGlyph
    {
        get => (string?) GetValue ( BackgroundGlyphProperty );
        set => SetValue ( BackgroundGlyphProperty, value );
    }

    public static readonly DependencyProperty BackgroundGlyphProperty = RegisterVisualProperty < FontIcon, string? > ( nameof ( BackgroundGlyph ), null );

    /// <inheritdoc cref="Control.FontFamily" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.Font ) ]
    public FontFamily FontFamily
    {
        get => (FontFamily) GetValue ( FontFamilyProperty );
        set => SetValue ( FontFamilyProperty, value );
    }

    /// <inheritdoc cref="Control.FontFamilyProperty" />
    public static readonly DependencyProperty FontFamilyProperty = RegisterVisualProperty < FontIcon > ( TextElement.FontFamilyProperty );

    /// <inheritdoc cref="Control.FontSize" />
    [ TypeConverter ( typeof ( FontSizeConverter ) ) ]
    [ Bindable ( true ), Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.None ) ]
    public double FontSize
    {
        get => (double) GetValue ( FontSizeProperty );
        set => SetValue ( FontSizeProperty, value );
    }

    /// <inheritdoc cref="Control.FontSizeProperty" />
    public static readonly DependencyProperty FontSizeProperty = RegisterVisualProperty < FontIcon > ( TextElement.FontSizeProperty );

    /// <inheritdoc cref="Control.FontStretch" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontStretch FontStretch
    {
        get => (FontStretch) GetValue ( FontStretchProperty );
        set => SetValue ( FontStretchProperty, value );
    }

    /// <inheritdoc cref="Control.FontStretchProperty" />
    public static readonly DependencyProperty FontStretchProperty = RegisterVisualProperty < FontIcon > ( TextElement.FontStretchProperty );

    /// <inheritdoc cref="Control.FontStyle" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontStyle FontStyle
    {
        get => (FontStyle) GetValue ( FontStyleProperty );
        set => SetValue ( FontStyleProperty, value );
    }

    /// <inheritdoc cref="Control.FontStyleProperty" />
    public static readonly DependencyProperty FontStyleProperty = RegisterVisualProperty < FontIcon > ( TextElement.FontStyleProperty );

    /// <inheritdoc cref="Control.FontWeight" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontWeight FontWeight
    {
        get => (FontWeight) GetValue ( FontWeightProperty );
        set => SetValue ( FontWeightProperty, value );
    }

    /// <inheritdoc cref="Control.FontWeightProperty" />
    public static readonly DependencyProperty FontWeightProperty = RegisterVisualProperty < FontIcon > ( TextElement.FontWeightProperty );

    [ Bindable ( true ), Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.Font ) ]
    public FontFamily BackgroundFontFamily
    {
        get => (FontFamily) GetValue ( BackgroundFontFamilyProperty );
        set => SetValue ( BackgroundFontFamilyProperty, value );
    }

    public static readonly DependencyProperty BackgroundFontFamilyProperty = RegisterVisualProperty < FontIcon, FontFamily > ( nameof ( BackgroundFontFamily ), SystemFonts.MessageFontFamily );

    protected override void OnDpiChanged ( DpiScale oldDpi, DpiScale newDpi )
    {
        base.OnDpiChanged ( oldDpi, newDpi );

        InvalidateMeasure ( );
        InvalidateChild   ( );
    }

    protected override void OnPropertyChanged ( DependencyPropertyChangedEventArgs e )
    {
        base.OnPropertyChanged ( e );

        // TODO: Override metadata
        if ( e.Property == FlowDirectionProperty )
        {
            InvalidateMeasure ( );
            InvalidateChild   ( );
        }
    }

    protected FormattedText FormatText ( string text, FontFamily fontFamily, Brush foreground )
    {
        var typeface           = new Typeface ( fontFamily, FontStyle, FontWeight, FontStretch );
        var numberSubstitution = new NumberSubstitution ( NumberSubstitution.GetCultureSource   ( this ),
                                                          NumberSubstitution.GetCultureOverride ( this ),
                                                          NumberSubstitution.GetSubstitution    ( this ) );

        return new FormattedText ( text, CultureInfo.CurrentUICulture, FlowDirection,
                                   typeface, FontSize, foreground, numberSubstitution,
                                   TextOptions.GetTextFormattingMode ( this ),
                                   VisualTreeHelper.GetDpi ( this ).PixelsPerDip );
    }

    private FormattedText? backgroundText;
    private FormattedText? foregroundText;

    protected override Size MeasureIcon ( )
    {
        var iconSize = default ( Size );

        backgroundText = null;
        if ( BackgroundGlyph is not null && Background is not null )
            backgroundText = FormatText ( BackgroundGlyph, BackgroundFontFamily, Background );

        if ( backgroundText is not null )
        {
            iconSize.Width  = Math.Max ( iconSize.Width,  backgroundText.WidthIncludingTrailingWhitespace );
            iconSize.Height = Math.Max ( iconSize.Height, backgroundText.Height );
        }

        foregroundText = null;
        if ( Glyph is not null && Foreground is not null )
            foregroundText = FormatText ( Glyph, FontFamily, Foreground );

        if ( foregroundText is not null )
        {
            iconSize.Width  = Math.Max ( iconSize.Width,  foregroundText.WidthIncludingTrailingWhitespace );
            iconSize.Height = Math.Max ( iconSize.Height, foregroundText.Height );
        }

        return iconSize;
    }

    protected override void DrawIcon ( DrawingContext drawingContext )
    {
        ArgumentNullException.ThrowIfNull ( drawingContext );

        drawingContext.DrawText ( backgroundText, default );
        drawingContext.DrawText ( foregroundText, default );

        backgroundText = null;
        foregroundText = null;
    }
}

public abstract class FontIcon < TSymbol > : FontIcon where TSymbol : struct
{
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public TSymbol Symbol
    {
        get => (TSymbol) GetValue ( SymbolProperty );
        set => SetValue ( SymbolProperty, value );
    }

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register ( nameof ( Symbol ),
                                      typeof ( TSymbol ),
                                      typeof ( FontIcon < TSymbol > ),
                                      new FrameworkPropertyMetadata ( default ( TSymbol ),
                                                                      static void (d, e) => ( (FontIcon < TSymbol >) d ).OnSymbolChanged ( e ) ) );

    protected abstract void OnSymbolChanged ( DependencyPropertyChangedEventArgs e );

    [ EditorBrowsable ( EditorBrowsableState.Never ) ]
    public new string? Glyph
    {
        get => (string?) GetValue ( GlyphProperty );
        set => SetValue ( GlyphProperty, value );
    }

    [ EditorBrowsable ( EditorBrowsableState.Never ) ]
    public new FontFamily FontFamily
    {
        get => (FontFamily) GetValue ( FontFamilyProperty );
        set => SetValue ( FontFamilyProperty, value );
    }

    [ EditorBrowsable ( EditorBrowsableState.Never ) ]
    public new string? BackgroundGlyph
    {
        get => (string?) GetValue ( BackgroundGlyphProperty );
        set => SetValue ( BackgroundGlyphProperty, value );
    }

    [ EditorBrowsable ( EditorBrowsableState.Never ) ]
    public new FontFamily BackgroundFontFamily
    {
        get => (FontFamily) GetValue ( BackgroundFontFamilyProperty );
        set => SetValue ( BackgroundFontFamilyProperty, value );
    }
}