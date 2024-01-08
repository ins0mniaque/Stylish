using System.Runtime.InteropServices;
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

    [ StructLayout ( LayoutKind.Sequential ) ]
    private struct Bgra32 { public byte B, G, R, A; }

    public static unsafe WriteableBitmap Tint ( this BitmapSource source, Color color )
    {
        ArgumentNullException.ThrowIfNull ( source );

        if ( source.Format != PixelFormats.Bgra32 && source.Format != PixelFormats.Bgr32 )
            source = new ColorConvertedBitmap ( source,
                                                new ColorContext ( source.Format ),
                                                new ColorContext ( PixelFormats.Bgra32 ),
                                                PixelFormats.Bgra32 );

        var tint      = new Bgra32 { A = byte.MaxValue, B = color.B, G = color.G, R = color.R };
        var writeable = source as WriteableBitmap ?? new WriteableBitmap ( source );
        var stride    = writeable.BackBufferStride * 8 / writeable.Format.BitsPerPixel;
        var buffer    = new Span < Bgra32 > ( writeable.BackBuffer.ToPointer ( ), writeable.PixelHeight * stride );

        for ( var x = 0; x < writeable.PixelWidth; x++ )
        {
            for ( var y = 0; y < writeable.PixelHeight; y++ )
            {
                ref var bgra = ref buffer [ x + y * stride ];

                var alpha = (byte) ( bgra.A * ( 1140 * bgra.B + 5870 * bgra.G + 2989 * bgra.R - 2550000 ) / -2550000 );

                bgra   = tint;
                bgra.A = alpha;
            }
        }

        return writeable;
    }
}