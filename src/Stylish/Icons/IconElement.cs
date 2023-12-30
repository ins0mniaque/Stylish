using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Stylish;

public abstract class IconElement : VisualHost
{
    static IconElement ( )
    {
        FocusableProperty                   .OverrideMetadata ( typeof ( IconElement ), new FrameworkPropertyMetadata ( false ) );
        KeyboardNavigation.IsTabStopProperty.OverrideMetadata ( typeof ( IconElement ), new FrameworkPropertyMetadata ( false ) );
        HorizontalAlignmentProperty         .OverrideMetadata ( typeof ( IconElement ), new FrameworkPropertyMetadata ( HorizontalAlignment.Center ) );
        VerticalAlignmentProperty           .OverrideMetadata ( typeof ( IconElement ), new FrameworkPropertyMetadata ( VerticalAlignment  .Center ) );
    }

    /// <inheritdoc cref="Control.Foreground" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Brush Foreground
    {
        get => (Brush) GetValue ( ForegroundProperty );
        set => SetValue ( ForegroundProperty, value );
    }

    public static readonly DependencyProperty ForegroundProperty = RegisterVisualProperty < IconElement > ( TextElement.ForegroundProperty );

    /// <inheritdoc cref="Control.Padding" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Thickness Padding
    {
        get => (Thickness) GetValue ( PaddingProperty );
        set => SetValue ( PaddingProperty, value );
    }

    /// <inheritdoc cref="Control.PaddingProperty" />
    public static readonly DependencyProperty PaddingProperty = RegisterVisualProperty < IconElement > ( Control.PaddingProperty );

    /// <inheritdoc cref="Viewbox.Stretch" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Stretch Stretch
    {
        get => (Stretch) GetValue ( StretchProperty );
        set => SetValue ( StretchProperty, value );
    }

    /// <inheritdoc cref="Viewbox.StretchProperty" />
    public static readonly DependencyProperty StretchProperty = RegisterVisualProperty < IconElement > ( Viewbox.StretchProperty );
 
    /// <inheritdoc cref="Viewbox.StretchDirection" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public StretchDirection StretchDirection
    {
        get => (StretchDirection) GetValue ( StretchDirectionProperty );
        set => SetValue ( StretchDirectionProperty, value );
    }

    /// <inheritdoc cref="Viewbox.StretchDirectionProperty" />
    public static readonly DependencyProperty StretchDirectionProperty = RegisterVisualProperty < IconElement > ( Viewbox.StretchDirectionProperty );

    [ Bindable ( true ), Category ( "Behavior" ) ]
    public bool IsSurfaceHitTestVisible
    {
        get => (bool) GetValue ( IsSurfaceHitTestVisibleProperty );
        set => SetValue ( IsSurfaceHitTestVisibleProperty, value );
    }

    public static readonly DependencyProperty IsSurfaceHitTestVisibleProperty = RegisterVisualProperty < IconElement, bool > ( nameof ( IsSurfaceHitTestVisible ), true );

    protected override Visual? GenerateChild ( ref Size availableSize )
    {
        var padding = Padding;

        availableSize = new Size ( Math.Max ( 0.0, availableSize.Width  - padding.Left - padding.Right  ),
                                   Math.Max ( 0.0, availableSize.Height - padding.Top  - padding.Bottom ) );

        if ( availableSize.Width is 0.0 || availableSize.Height is 0.0 )
        {
            availableSize = default;
            return null;
        }

        var iconSize = MeasureIcon ( );

        if ( double.IsPositiveInfinity ( availableSize.Width ) )
            availableSize.Width = iconSize.Width;

        if ( double.IsPositiveInfinity ( availableSize.Height ) )
            availableSize.Height = iconSize.Height;

        if ( availableSize.Width is 0.0 || availableSize.Height is 0.0 )
        {
            availableSize = default;
            return null;
        }

        var drawingVisual  = new DrawingVisual ( );
        var drawingContext = drawingVisual.RenderOpen ( );

        var translate = GetPaddingTransform ( padding );
        if ( translate is not null )
            drawingContext.PushTransform ( translate );

        var scale = GetStretchTransform ( Stretch, StretchDirection, iconSize, availableSize );

        if ( scale is not null )
        {
            drawingContext.PushTransform ( scale );

            iconSize = new Size ( iconSize.Width  * scale.ScaleX,
                                  iconSize.Height * scale.ScaleY );
        }

        iconSize = new Size ( iconSize.Width  + padding.Left + padding.Right,
                              iconSize.Height + padding.Top  + padding.Bottom );

        DrawIcon ( drawingContext );

        if ( scale is not null )
            drawingContext.Pop ( );

        if ( translate is not null )
            drawingContext.Pop ( );

        if ( IsSurfaceHitTestVisible )
            drawingContext.DrawRectangle ( Brushes.Transparent, null, new Rect ( default, iconSize ) );

        drawingContext.Close ( );

        availableSize = iconSize;

        return drawingVisual;
    }

    protected abstract Size MeasureIcon ( );
    protected abstract void DrawIcon    ( DrawingContext drawingContext );

    private static TranslateTransform? GetPaddingTransform ( Thickness padding )
    {
        if ( padding.Left is not 0 || padding.Top is not 0 )
            return new TranslateTransform { X = padding.Left, Y = padding.Top };

        return null;
    }

    private static ScaleTransform? GetStretchTransform ( Stretch stretch, StretchDirection direction, Size iconSize, Size availableSize )
    {
        var scale = stretch is not Stretch.None &&
                    ! double.IsPositiveInfinity ( availableSize.Width  ) &&
                    ! double.IsPositiveInfinity ( availableSize.Height ) &&
                    ( availableSize.Width  > iconSize.Width  && direction is StretchDirection.UpOnly   or StretchDirection.Both ||
                      availableSize.Width  < iconSize.Width  && direction is StretchDirection.DownOnly or StretchDirection.Both ||
                      availableSize.Height > iconSize.Height && direction is StretchDirection.UpOnly   or StretchDirection.Both ||
                      availableSize.Height < iconSize.Height && direction is StretchDirection.DownOnly or StretchDirection.Both );

        if ( ! scale )
            return null;

        var scaleX = availableSize.Width  / iconSize.Width;
        var scaleY = availableSize.Height / iconSize.Height;

        if ( stretch is Stretch.Uniform )
            scaleX = scaleY = Math.Min ( scaleX, scaleY );
        else if ( stretch is Stretch.UniformToFill )
            scaleX = scaleY = Math.Max ( scaleX, scaleY );

        return new ScaleTransform { ScaleX = scaleX,
                                    ScaleY = scaleY };
    }
}