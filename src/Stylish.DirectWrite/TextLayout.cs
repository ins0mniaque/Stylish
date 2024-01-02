using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.DirectWrite;

using Stylish.DirectWrite.Metrics;

namespace Stylish.DirectWrite;

public class TextLayout
{
    public TextLayout ( string text, TextFormat format, float pixelsPerDip, int width = int.MaxValue, int height = int.MaxValue, Matrix? transform = null, TextAntialiasing antialiasing = TextAntialiasing.Default )
    {
        ArgumentNullException.ThrowIfNull ( text );
        ArgumentNullException.ThrowIfNull ( format );

        var matrix = transform ?? Matrix.Identity;
        var dwrite = matrix != Matrix.Identity ? Unsafe.As < Matrix, DWRITE_MATRIX > ( ref matrix ) : (DWRITE_MATRIX?) null;

        DirectX.DWrite.CreateGdiCompatibleTextLayout ( text, (uint) text.Length, format.Interface,
                                                       width, height, pixelsPerDip, dwrite,
                                                       new BOOL ( antialiasing is not TextAntialiasing.Aliased ),
                                                       out var textLayout );

        Interface = textLayout;

        Text         = text;
        Format       = format;
        Width        = width;
        Height       = height;
        PixelDensity = PixelDensity.FromDip ( pixelsPerDip );
        Transform    = transform ?? Matrix.Identity;
        Antialiasing = antialiasing;
    }

    internal IDWriteTextLayout Interface { get; }

    public string           Text         { get; }
    public TextFormat       Format       { get; }
    public float            Width        { get; }
    public float            Height       { get; }
    public PixelDensity     PixelDensity { get; }
    public Matrix           Transform    { get; }
    public TextAntialiasing Antialiasing { get; }

    private float? minWidth;
    public  float  MinWidth => minWidth ??= DetermineMinWidth ( );

    private TextMetrics? metrics;
    public  TextMetrics  Metrics => metrics ??= GetTextMetrics ( );

    private OverhangMetrics? overhangMetrics;
    public  OverhangMetrics  OverhangMetrics => overhangMetrics ??= GetOverhangMetrics ( );

    private IReadOnlyList < LineMetrics >? lineMetrics;
    public  IReadOnlyList < LineMetrics >  LineMetrics => lineMetrics ??= GetLineMetrics ( );

    private IReadOnlyList < ClusterMetrics >? clusterMetrics;
    public  IReadOnlyList < ClusterMetrics >  ClusterMetrics => clusterMetrics ??= GetClusterMetrics ( );

    // TODO: This is not working
    public unsafe Inline? GetInline ( uint position )
    {
        Interface.GetInlineObject ( position, out var inline, null );

        return inline is null ? null : new Inline ( inline );
    }

    // TODO: HitTest and Range methods

    private float DetermineMinWidth ( )
    {
        Interface.DetermineMinWidth ( out var minWidth );

        return minWidth;
    }

    private TextMetrics GetTextMetrics ( )
    {
        Interface.GetMetrics ( out var metrics );

        return new TextMetrics ( metrics.left, metrics.top, metrics.width, metrics.widthIncludingTrailingWhitespace, metrics.height,
                                 metrics.layoutWidth, metrics.layoutHeight, metrics.maxBidiReorderingDepth, metrics.lineCount );
    }

    private OverhangMetrics GetOverhangMetrics ( )
    {
        Interface.GetOverhangMetrics ( out var overhang );

        return new OverhangMetrics ( overhang.left, overhang.top, overhang.right, overhang.bottom );
    }

    private LineMetrics [ ] GetLineMetrics ( )
    {
        var lineMetrics = new LineMetrics [ Metrics.LineCount ];
        var dwrite      = MemoryMarshal.Cast < LineMetrics, DWRITE_LINE_METRICS > ( lineMetrics );

        Interface.GetLineMetrics ( dwrite, out var lineCount );

        if ( lineMetrics.Length != lineCount )
            Array.Resize ( ref lineMetrics, (int) lineCount );

        return lineMetrics;
    }

    private unsafe ClusterMetrics [ ] GetClusterMetrics ( )
    {
        var clusterMetrics = new ClusterMetrics [ Encoding.UTF32.GetByteCount ( Text ) / 4 ];
        var dwrite         = MemoryMarshal.Cast < ClusterMetrics, DWRITE_CLUSTER_METRICS > ( clusterMetrics );

        Interface.GetClusterMetrics ( dwrite, out var clusterCount );

        if ( clusterMetrics.Length != clusterCount )
            Array.Resize ( ref clusterMetrics, (int) clusterCount );

        return clusterMetrics;
    }
}