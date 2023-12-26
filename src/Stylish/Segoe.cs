using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

using Stylish.Fonts;

namespace Stylish;

public class Segoe : Symbol, ISupportInitialize
{
    private const string FontName = "Segoe UI";
    private const string EmojiFontName = "Segoe UI Emoji";
    private const string FluentFontName = "Segoe Fluent Icons";
    private const string MDL2FontName   = "Segoe MDL2 Assets";
    private const string SymbolFontName     = "Segoe UI Symbol";

    static Segoe ( )
    {
        CheckFontAvailability ( );
    }

    public static bool UseMostRecentGlyph { get; set; } // = true;

    public static bool IsAvailable       { get; private set; }
    public static bool IsEmojiAvailable  { get; private set; }
    public static bool IsFluentAvailable { get; private set; }
    public static bool IsMDL2Available   { get; private set; }
    public static bool IsSymbolAvailable { get; private set; }

    public static void CheckFontAvailability ( )
    {
        using var fonts = new InstalledFontCollection ( );

        IsAvailable       = false;
        IsEmojiAvailable  = false;
        IsFluentAvailable = false;
        IsMDL2Available   = false;
        IsSymbolAvailable = false;

        foreach ( var fontFamily in fonts.Families )
        {
            if      ( fontFamily.Name is FontName       ) IsAvailable       = true;
            else if ( fontFamily.Name is EmojiFontName  ) IsEmojiAvailable  = true;
            else if ( fontFamily.Name is FluentFontName ) IsFluentAvailable = true;
            else if ( fontFamily.Name is MDL2FontName   ) IsMDL2Available   = true;
            else if ( fontFamily.Name is SymbolFontName ) IsSymbolAvailable = true;
        }
    }

    public SegoeFluentIcons Fluent  { get; set; }
    public SegoeMDL2Assets  MDL2    { get; set; }
    public SegoeUISymbol    Symbol  { get; set; }

    protected override object? ProvideValue ( IServiceProvider serviceProvider, object target, object? targetProperty )
    {
        if ( targetProperty == TextElement.FontFamilyProperty )
            return BindFontFamily ( serviceProvider, target );

        return base.ProvideValue ( serviceProvider, target, targetProperty );
    }

    private FontFamily? FontFamily       { get; set; }
    private FontFamily? EmojiFontFamily  { get; set; }
    private FontFamily? FluentFontFamily { get; set; }
    private FontFamily? MDL2FontFamily   { get; set; }
    private FontFamily? SymbolFontFamily { get; set; }

    protected override string? SelectGlyph ( FontFamily? fontFamily, out FontFamily? fontFamilyToSet )
    {
        Detect ( fontFamily, out var isSegoeRequested, out var isEmojiRequested, out var isFluentRequested, out var isMDL2Requested, out var isSymbolRequested );

        var isAnyRequested = isFluentRequested || isMDL2Requested || isSymbolRequested;

        if ( ! isAnyRequested )
            isFluentRequested = isMDL2Requested = isSymbolRequested = true;

        fontFamilyToSet = null;

        if ( IsFluentAvailable && isFluentRequested && Fluent is not SegoeFluentIcons.None )
        {
            if ( ! isAnyRequested )
                fontFamilyToSet = FluentFontFamily ??= new FontFamily ( FluentFontName );

            return Fluent.ToUnicode ( );
        }

        if ( IsMDL2Available && isMDL2Requested && MDL2 is not SegoeMDL2Assets.None )
        {
            if ( ! isAnyRequested )
                fontFamilyToSet = MDL2FontFamily ??= new FontFamily ( MDL2FontName );

            return MDL2.ToUnicode ( );
        }

        if ( IsSymbolAvailable && isSymbolRequested && Symbol is not SegoeUISymbol.None )
        {
            if ( ! isAnyRequested )
                fontFamilyToSet = SymbolFontFamily ??= new FontFamily ( SymbolFontName );

            return Symbol.ToUnicode ( );
        }

        if ( Emoji is not Emoji.None )
        {
            if ( ! isEmojiRequested && IsEmojiAvailable )
                fontFamilyToSet = EmojiFontFamily ??= new FontFamily ( EmojiFontName );
            else if ( ! isSegoeRequested && IsAvailable )
                fontFamilyToSet = FontFamily ??= new FontFamily ( FontName );

            return Emoji.ToUnicode ( Variant );
        }

        return null;
    }

    public void BeginInit ( ) { }
    public void EndInit ( )
    {
        if ( ! UseMostRecentGlyph )
            return;

        if ( Symbol is not SegoeUISymbol.None && MDL2 is SegoeMDL2Assets.None )
            MDL2 = Symbol.ToMDL2 ( );

        if ( MDL2 is not SegoeMDL2Assets.None && Fluent is SegoeFluentIcons.None )
            Fluent = MDL2.ToFluent ( );
    }

    private static void Detect ( FontFamily? fontFamily, out bool isSegoeRequested, out bool isEmojiRequested, out bool isFluentRequested, out bool isMDL2Requested, out bool isSymbolRequested )
    {
        isSegoeRequested  = fontFamily?.Source.Contains ( FontName,       StringComparison.OrdinalIgnoreCase ) ?? false;
        isEmojiRequested  = fontFamily?.Source.Contains ( EmojiFontName,  StringComparison.OrdinalIgnoreCase ) ?? false;
        isFluentRequested = fontFamily?.Source.Contains ( FluentFontName, StringComparison.OrdinalIgnoreCase ) ?? false;
        isMDL2Requested   = fontFamily?.Source.Contains ( MDL2FontName,   StringComparison.OrdinalIgnoreCase ) ?? false;
        isSymbolRequested = fontFamily?.Source.Contains ( SymbolFontName, StringComparison.OrdinalIgnoreCase ) ?? false;
    }

    private sealed class Lolz : IValueConverter
    {
        public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is FontFamily fontFamily )
                return AddIconFontsTo ( fontFamily );

            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture ) => throw new NotImplementedException ( );
    }

    private static FontFamily AddIconFontsTo ( FontFamily fontFamily )
    {
        var source = fontFamily.Source;

        if ( IsFluentAvailable && ! fontFamily.Source.Contains ( FluentFontName, StringComparison.OrdinalIgnoreCase ) )
            source += ", " + FluentFontName;
        if ( IsMDL2Available && ! fontFamily.Source.Contains ( MDL2FontName, StringComparison.OrdinalIgnoreCase ) )
            source += ", " + MDL2FontName;
        if ( IsSymbolAvailable && ! fontFamily.Source.Contains ( SymbolFontName, StringComparison.OrdinalIgnoreCase ) )
            source += ", " + SymbolFontName;

        if ( fontFamily.Source == source )
            return fontFamily;

        return new FontFamily ( source );
    }

    private static object BindFontFamily ( IServiceProvider serviceProvider, object target )
    {
        if ( target is Window window )
            return AddIconFontsTo ( window.FontFamily );

        var binding = new Binding { Path = new PropertyPath ( "(0)", TextElement.FontFamilyProperty ),
                                        RelativeSource = new RelativeSource { AncestorType= typeof ( DependencyObject ) },
                                        Converter = new Lolz () };



            return binding.ProvideValue ( serviceProvider );
    }
}