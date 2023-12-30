using System.Windows.Interop;
using System.Windows.Media;

namespace Stylish.Demo;

public partial class MainWindow
{
    public MainWindow ( )
    {
        InitializeComponent ( );

        var dpi = VisualTreeHelper.GetDpi ( this );
        var typeface = new Typeface ( FontFamily, FontStyle, FontWeight, FontStretch );

        Image.Source = DirectWrite.Lol ( "🐋📈🎏🔨💃🙆🐔💌💠😾", 48, typeface, dpi.PixelsPerInchX, dpi.PixelsPerInchY );
        Image2.Source = DirectWrite.Lol ( "ABCDEFGH", 48, typeface, dpi.PixelsPerInchX, dpi.PixelsPerInchY );
    }

    protected override void OnRender ( DrawingContext drawingContext )
    {
        base.OnRender ( drawingContext );
    }
}