using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Stylish.Controls;

public class FontIcon : IconElement
{
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public string? Glyph
    {
        get => (string?) GetValue ( GlyphProperty );
        set => SetValue ( GlyphProperty, value );
    }

    public static readonly DependencyProperty GlyphProperty = RegisterVisualProperty < FontIcon, string? > ( nameof ( Glyph ), null );

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

    private FormattedText? formattedGlyph;

    protected override Size MeasureIcon ( )
    {
        var iconSize = default ( Size );

        formattedGlyph = null;
        if ( Glyph is not null && Foreground is not null )
            formattedGlyph = FormatText ( Glyph, FontFamily, Foreground );

        if ( formattedGlyph is not null )
        {
            iconSize.Width  = Math.Max ( iconSize.Width,  formattedGlyph.WidthIncludingTrailingWhitespace );
            iconSize.Height = Math.Max ( iconSize.Height, formattedGlyph.Height );
        }

        return iconSize;
    }

    protected override void DrawIcon ( DrawingContext drawingContext )
    {
        ArgumentNullException.ThrowIfNull ( drawingContext );

        if ( formattedGlyph is not null )
            drawingContext.DrawText ( formattedGlyph, default );

        formattedGlyph = null;
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
}