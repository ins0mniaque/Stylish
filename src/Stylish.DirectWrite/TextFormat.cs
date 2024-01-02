using System.Globalization;
using System.Text;

using Windows.Win32;
using Windows.Win32.Graphics.DirectWrite;

using Stylish.DirectWrite.Formatting;

namespace Stylish.DirectWrite;

public class TextFormat
{
    public TextFormat ( string             fontFamily,
                        float              fontSize,
                        FontWeight         fontWeight             = FontWeight.Normal,
                        FontStyle          fontStyle              = FontStyle.Normal,
                        FontStretch        fontStretch            = FontStretch.Normal,
                        CultureInfo?       culture                = default,
                        TextAlignment      textAlignment          = TextAlignment.Leading,
                        ParagraphAlignment paragraphAlignment     = ParagraphAlignment.Near,
                        WordWrapping       wordWrapping           = WordWrapping.Wrap,
                        ReadingDirection   readingDirection       = ReadingDirection.LeftToRight,
                        FlowDirection      flowDirection          = FlowDirection.TopToBottom,
                        float              incrementalTabStop     = 0f,
                        TextTrimming       trimming               = TextTrimming.None,
                        Rune               trimmingDelimiter      = default,
                        uint               trimmingDelimiterCount = 0U,
                        LineSpacingMethod  lineSpacingMethod      = LineSpacingMethod.Default,
                        float              lineSpacing            = 0f,
                        float              baseline               = 0f )
    {
        ArgumentNullException.ThrowIfNull ( fontFamily );

        if ( fontWeight is default ( FontWeight ) )
            throw new ArgumentOutOfRangeException ( nameof ( fontWeight ), fontWeight, "Invalid font weight value" );

        FontFamily  = fontFamily;
        FontSize    = fontSize;
        FontWeight  = fontWeight;
        FontStyle   = fontStyle;
        FontStretch = fontStretch;
        Culture     = culture ?? CultureInfo.CurrentUICulture;

        DirectX.DWrite.CreateTextFormat ( fontFamily,
                                          null,
                                          (DWRITE_FONT_WEIGHT)  fontWeight,
                                          (DWRITE_FONT_STYLE)   fontStyle,
                                          (DWRITE_FONT_STRETCH) fontStretch,
                                          fontSize,
                                          Culture.Name,
                                          out var textFormat );

        Interface = textFormat;

        if ( incrementalTabStop is 0f )
            incrementalTabStop = textFormat.GetIncrementalTabStop ( );

        TextAlignment      = textAlignment;
        ParagraphAlignment = paragraphAlignment;
        WordWrapping       = wordWrapping;
        ReadingDirection   = readingDirection;
        FlowDirection      = flowDirection;
        IncrementalTabStop = incrementalTabStop;

        Trimming               = trimming;
        TrimmingDelimiter      = trimmingDelimiter;
        TrimmingDelimiterCount = trimmingDelimiterCount;

        LineSpacingMethod = lineSpacingMethod;
        LineSpacing       = lineSpacing;
        Baseline          = baseline;

        Interface.SetTextAlignment      ( (DWRITE_TEXT_ALIGNMENT)      textAlignment );
        Interface.SetParagraphAlignment ( (DWRITE_PARAGRAPH_ALIGNMENT) lineSpacingMethod );
        Interface.SetWordWrapping       ( (DWRITE_WORD_WRAPPING)       wordWrapping );
        Interface.SetReadingDirection   ( (DWRITE_READING_DIRECTION)   readingDirection );
        Interface.SetFlowDirection      ( (DWRITE_FLOW_DIRECTION)      flowDirection );
        Interface.SetIncrementalTabStop ( incrementalTabStop );
        Interface.SetLineSpacing        ( (DWRITE_LINE_SPACING_METHOD) lineSpacingMethod, lineSpacing, baseline );
        Interface.SetTrimming           ( new DWRITE_TRIMMING { granularity    = (DWRITE_TRIMMING_GRANULARITY) trimming,
                                                                delimiter      = (uint) trimmingDelimiter.Value,
                                                                delimiterCount = trimmingDelimiterCount },
                                          TrimmingSign?.Interface );
    }

    internal IDWriteTextFormat Interface { get; }

    public string      FontFamily  { get; }
    public float       FontSize    { get; }
    public FontWeight  FontWeight  { get; }
    public FontStyle   FontStyle   { get; }
    public FontStretch FontStretch { get; }
    public CultureInfo Culture     { get; }

    public TextAlignment      TextAlignment      { get; }
    public ParagraphAlignment ParagraphAlignment { get; }
    public WordWrapping       WordWrapping       { get; }
    public ReadingDirection   ReadingDirection   { get; }
    public FlowDirection      FlowDirection      { get; }
    public float              IncrementalTabStop { get; }

    public TextTrimming Trimming               { get; }
    public Rune         TrimmingDelimiter      { get; }
    public uint         TrimmingDelimiterCount { get; }
    public Inline?      TrimmingSign           { get; }

    public LineSpacingMethod LineSpacingMethod { get; }
    public float             LineSpacing       { get; }
    public float             Baseline          { get; }
}