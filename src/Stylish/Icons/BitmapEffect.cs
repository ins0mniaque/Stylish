using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Stylish;

public static class BitmapEffect
{
    public static unsafe BitmapSource ToBitmap ( this ImageSource source )
    {
        if ( source is BitmapSource bitmap )
            return bitmap;

        var bounds = new Rect ( );
        var image  = new Image { Source = source };

        if ( source is DrawingImage drawing )
        {
            bounds.Width  = drawing.Drawing.Bounds.Width;
            bounds.Height = drawing.Drawing.Bounds.Height;
        }
        else if ( source is D3DImage d3d )
        {
            bounds.Width  = d3d.PixelWidth;
            bounds.Height = d3d.PixelHeight;
        }

        image.Arrange ( bounds );

        var dpi    = VisualTreeHelper.GetDpi ( image );
        var target = new RenderTargetBitmap ( (int) bounds.Width, (int) bounds.Height, dpi.DpiScaleX, dpi.DpiScaleY, PixelFormats.Bgra32 );

        target.Render ( image );

        return target;
    }

    // TODO: AVX bitwise AND FF000000FF000000... OR 00XXXXXX00XXXXXX...
    public static unsafe WriteableBitmap Tint ( this BitmapSource source, Color color )
    {
        ArgumentNullException.ThrowIfNull ( source );

        if ( source.Format != PixelFormats.Bgra32 && source.Format != PixelFormats.Bgr32 )
            source = new ColorConvertedBitmap ( source,
                                                new ColorContext ( source.Format ),
                                                new ColorContext ( PixelFormats.Bgra32 ),
                                                PixelFormats.Bgra32 );

        const int AlphaMask = unchecked ( (int) 0xFF000000 );

        var tint = color.B | color.G << 8 | color.R << 16;

        var writeable = source as WriteableBitmap ?? new WriteableBitmap ( source );
        var stride    = writeable.BackBufferStride * 8 / writeable.Format.BitsPerPixel;
        var buffer    = new Span < int > ( writeable.BackBuffer.ToPointer ( ), writeable.PixelHeight * stride );

        for ( var x = 0; x < writeable.PixelWidth; x++ )
        {
            for ( var y = 0; y < writeable.PixelHeight; y++ )
            {
                var index = x + y * stride;

                buffer [ index ] = ( buffer [ index ] & AlphaMask ) | tint;
            }
        }

        return writeable;
    }
}