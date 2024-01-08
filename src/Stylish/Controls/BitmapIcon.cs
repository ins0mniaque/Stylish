using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stylish.Controls;

[ ContentProperty ( nameof ( Source ) ) ]
public class BitmapIcon : IconElement
{
    /// <inheritdoc cref="Image.Source" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public BitmapSource Source
    {
        get => (BitmapSource) GetValue ( SourceProperty );
        set => SetValue ( SourceProperty, value );
    }

    public static readonly DependencyProperty SourceProperty = RegisterVisualProperty < BitmapIcon, BitmapSource? > ( nameof ( Source ), null );

    [ Bindable ( true ), Category ( "Appearance" ) ]
    public bool ShowAsMonochrome
    {
        get => (bool) GetValue ( ShowAsMonochromeProperty );
        set => SetValue ( ShowAsMonochromeProperty, value );
    }

    public static readonly DependencyProperty ShowAsMonochromeProperty = RegisterVisualProperty < BitmapIcon, bool > ( nameof ( ShowAsMonochrome ), true );

    protected override Size MeasureIcon ( )
    {
        return Source is null ? default : new Size ( Source.PixelWidth, Source.PixelHeight );
    }

    protected override void DrawIcon ( DrawingContext drawingContext )
    {
        ArgumentNullException.ThrowIfNull ( drawingContext );

        var source = Source;
        if ( ShowAsMonochrome && Foreground is SolidColorBrush brush )
            source = source.Tint ( brush.Color );

        drawingContext.DrawImage ( source, new Rect ( default, MeasureIcon ( ) ) );
    }
}