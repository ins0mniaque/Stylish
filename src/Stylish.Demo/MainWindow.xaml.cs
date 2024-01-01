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
        var dpi = VisualTreeHelper.GetDpi ( this );

        var textFormat = new DirectWrite.TextFormat ( FontFamily.Source, DirectWrite.FontWeight.Normal, default, DirectWrite.FontStretch.Normal, 48, CultureInfo.CurrentUICulture, default, default, default, default, default, default, default, default, default, default, default, default );
        var textLayout = new DirectWrite.TextLayout ( "🐋📈🎏🔨💃🙆🐔💌💠😾", textFormat, int.MaxValue, int.MaxValue, (float) dpi.PixelsPerDip, null, true );
        var textLayout2 = new DirectWrite.TextLayout ( "ABCDEFGH", textFormat, int.MaxValue, int.MaxValue, (float) dpi.PixelsPerDip, null, true );

        using var surface = new DirectWrite.Surface ( (int) textLayout.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout.Metrics.Height, (float) dpi.PixelsPerInchX, (float) dpi.PixelsPerInchY );

        var brush = new DirectWrite.SolidColorBrush ( surface, new DirectWrite.Color ( 0, 0, 0, 1 ), 1, DirectWrite.Matrix.Identity );

        surface.Render ( textLayout, brush, DirectWrite.RenderOptions.EnableColorFont );

        surface.Flush ( );

        Image.Source = Imaging.CreateBitmapSourceFromHBitmap(surface.HBitmap, (nint) 0, new System.Windows.Int32Rect ( 0, 0, surface.PixelWidth, surface.PixelHeight ),
                                                           BitmapSizeOptions.FromWidthAndHeight( surface.Width, surface.Height ) );

        // surface.Resize ( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout2.Metrics.Height, (float) dpi.PixelsPerInchX, (float) dpi.PixelsPerInchY );
        surface.Clear ( );

        surface.Render ( textLayout2, brush, DirectWrite.RenderOptions.EnableColorFont );

        surface.Flush ( );

        var pw2 = surface.PixelDensity.ToPixelWidth  ( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace );
        var ph2 = surface.PixelDensity.ToPixelHeight ( (int) textLayout2.Metrics.Height );

        Image2.Source = Imaging.CreateBitmapSourceFromHBitmap(surface.HBitmap, (nint) 0, new System.Windows.Int32Rect ( 0, 0, pw2, ph2 ),
                                                              BitmapSizeOptions.FromWidthAndHeight( (int) textLayout2.Metrics.WidthIncludingTrailingWhitespace, (int) textLayout2.Metrics.Height ) );

        var test = new byte [ surface.PixelWidth * surface.PixelHeight * surface.BitsPerPixel / 8 ];

        // surface.CopyPixels ( test.AsSpan ( ) );

        test.ToString ( );
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