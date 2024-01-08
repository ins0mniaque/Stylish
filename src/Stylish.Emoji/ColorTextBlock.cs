using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stylish.Controls;

public class ColorTextBlock : VisualHost
{
    public string Text
    {
        get => (string) GetValue ( TextProperty );
        set => SetValue ( TextProperty, value );
    }

    public static readonly DependencyProperty TextProperty = RegisterVisualProperty < ColorTextBlock, string? > ( nameof ( Text ), null );

    protected override Visual? GenerateChild ( ref Size availableSize )
    {
        var dpi = VisualTreeHelper.GetDpi ( this );
        var textFormat = new DirectWrite.TextFormat ( TextElement.GetFontFamily ( this ).Source, (float) TextElement.GetFontSize ( this ) );
        var textLayout = new DirectWrite.TextLayout ( Text, textFormat, (float) dpi.PixelsPerDip );

        // TODO: Reuse surface
        using var surface = new DirectWrite.Surface ( (int) textLayout.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout.Metrics.Height, (float) dpi.PixelsPerInchX, (float) dpi.PixelsPerInchY );

        var brush = new DirectWrite.Brushes.SolidColorBrush ( surface, new DirectWrite.Brushes.Color ( 1, 0, 0, 0 ) );

        surface.Draw ( textLayout, brush );

        surface.Render ( );

        var bitmap = Imaging.CreateBitmapSourceFromHBitmap ( surface.HBitmap,
                                                             (nint) 0,
                                                             new ( 0, 0, surface.PixelWidth, surface.PixelHeight ),
                                                             BitmapSizeOptions.FromEmptyOptions ( ) );

        var drawingVisual  = new DrawingVisual ( );
        var drawingContext = drawingVisual.RenderOpen ( );

        drawingContext.DrawImage ( bitmap, new Rect ( 0, 0, surface.Width, surface.Height ));

        drawingContext.Close ( );

        availableSize = new Size ( surface.Width, surface.Height );

        return drawingVisual;
    }

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
}