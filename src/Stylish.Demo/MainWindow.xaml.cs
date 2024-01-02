using System.Globalization;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stylish.Demo;

public partial class MainWindow
{
    public MainWindow ( )
    {
        InitializeComponent ( );

        var dpi2 = DirectWrite.DirectX.GetSystemPixelDensity ( );
        var dpi3 = DirectWrite.DirectX.GetWindowPixelDensity ( new WindowInteropHelper ( this ).EnsureHandle ( ) );
        var dpi = VisualTreeHelper.GetDpi ( this );

        var textFormat = new DirectWrite.TextFormat ( FontFamily.Source, 48 );
        var textLayout = new DirectWrite.TextLayout ( "🐋📈🎏🔨💃🙆🐔💌💠😾", textFormat, (float) dpi.PixelsPerDip );
        var textLayout2 = new DirectWrite.TextLayout ( "ABCDEFGH", textFormat, (float) dpi.PixelsPerDip );

        using var surface = new DirectWrite.Surface ( (int) textLayout.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout.Metrics.Height, (float) dpi.PixelsPerInchX, (float) dpi.PixelsPerInchY );

        var brush = new DirectWrite.Brushes.SolidColorBrush ( surface, new DirectWrite.Brushes.Color ( 1, 0, 0, 0 ) );

        surface.Draw ( textLayout, brush );

        surface.Render ( );

        Image.Source = Imaging.CreateBitmapSourceFromHBitmap(surface.HBitmap, (nint) 0, new System.Windows.Int32Rect ( 0, 0, surface.PixelWidth, surface.PixelHeight ),
                                                           BitmapSizeOptions.FromWidthAndHeight( surface.Width, surface.Height ) );

        // surface.Resize ( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout2.Metrics.Height, (float) dpi.PixelsPerInchX, (float) dpi.PixelsPerInchY );
        surface.Clear ( );

        surface.Draw ( textLayout2, brush );

        surface.Render ( );

        var pw2 = surface.PixelDensity.ToPixelWidth  ( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace );
        var ph2 = surface.PixelDensity.ToPixelHeight ( (int) textLayout2.Metrics.Height );

        Image2.Source = Imaging.CreateBitmapSourceFromHBitmap(surface.HBitmap, (nint) 0, new System.Windows.Int32Rect ( 0, 0, pw2, ph2 ),
                                                              BitmapSizeOptions.FromWidthAndHeight( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout2.Metrics.Height ) );

        var test = textLayout2.LineMetrics;
        var testB = textLayout.ClusterMetrics;

        test?.ToString ( );
    }

    protected override void OnRender ( DrawingContext drawingContext )
    {
        base.OnRender ( drawingContext );
    }

    private void RichTextBox_TextChanged ( object sender, System.Windows.Controls.TextChangedEventArgs e )
    {
        foreach ( var x in e.Changes )
        {
            if ( x.AddedLength > 0 )
            {
                var range = new TextRange ( RichTextBox.Document.ContentStart.GetPositionAtOffset ( x.Offset ),
                    RichTextBox.Document.ContentStart.GetPositionAtOffset ( x.Offset + x.AddedLength ) );

                if ( range.Text.Contains ( 'a', StringComparison.Ordinal ) )
                {
                    var lol = RichTextBox.Selection.Start.GetOffsetToPosition ( RichTextBox.Selection.Start );

                    range.Text = range.Text.Replace ( 'a', 'b' );
                    // RichTextBox.Document.ContentStart.Paragraph.Inlines.Add ( );

                    RichTextBox.Selection.Select ( RichTextBox.Selection.Start.GetPositionAtOffset ( lol + 1 ), RichTextBox.Selection.End.GetPositionAtOffset ( lol + 1 ) );
                }
            }
        }
    }
}