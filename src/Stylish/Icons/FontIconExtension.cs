using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Stylish;

public abstract class FontIconExtension < TIcon, TSymbol > : MarkupExtension where TIcon : FontIcon < TSymbol >, new ( ) where TSymbol : struct
{
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public TSymbol Symbol { get; set; }

    /// <inheritdoc cref="Control.FontSize" />
    [ TypeConverter ( typeof ( FontSizeConverter ) ) ]
    [ Bindable ( true ), Category ( "Appearance" ) ]
    [ Localizability ( LocalizationCategory.None ) ]
    public double? FontSize { get; set; }

    /// <inheritdoc cref="Control.FontStretch" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontStretch? FontStretch { get; set; }

    /// <inheritdoc cref="Control.FontStyle" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontStyle? FontStyle { get; set; }

    /// <inheritdoc cref="Control.FontWeight" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public FontWeight? FontWeight { get; set; }

    /// <inheritdoc cref="Control.Foreground" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Brush? Foreground { get; set; }

    /// <inheritdoc cref="Control.Padding" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Thickness? Padding { get; set; }

    /// <inheritdoc cref="Viewbox.Stretch" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Stretch? Stretch { get; set; }

    /// <inheritdoc cref="Viewbox.StretchDirection" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public StretchDirection? StretchDirection { get; set; }

    public sealed override object ProvideValue ( IServiceProvider serviceProvider )
    {
        var provider = (IProvideValueTarget?) serviceProvider?.GetService ( typeof ( IProvideValueTarget ) );
        var target   = provider?.TargetObject;

        if ( serviceProvider is null || target is null )
            return this;

        return CreateIcon ( );
    }

    protected virtual TIcon CreateIcon ( )
    {
        var icon = new TIcon { Symbol = Symbol };

        if ( FontSize         is { } fontSize         ) icon.FontSize         = fontSize;
        if ( FontStretch      is { } fontStretch      ) icon.FontStretch      = fontStretch;
        if ( FontStyle        is { } fontStyle        ) icon.FontStyle        = fontStyle;
        if ( FontWeight       is { } fontWeight       ) icon.FontWeight       = fontWeight;
        if ( Foreground       is { } foreground       ) icon.Foreground       = foreground;
        if ( Padding          is { } padding          ) icon.Padding          = padding;
        if ( Stretch          is { } stretch          ) icon.Stretch          = stretch;
        if ( StretchDirection is { } stretchDirection ) icon.StretchDirection = stretchDirection;

        return icon;
    }
}