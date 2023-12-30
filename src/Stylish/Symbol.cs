using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

using Stylish.Fonts;
using Stylish.Unicode;

namespace Stylish;

public class Symbol : MarkupExtension
{
    public Unicode.Emoji Emoji   { get; set; }
    public Variant       Variant { get; set; }
    public string?       Format  { get; set; }

    public sealed override object ProvideValue ( IServiceProvider serviceProvider )
    {
        var provider = (IProvideValueTarget?) serviceProvider?.GetService ( typeof ( IProvideValueTarget ) );
        var target   = provider?.TargetObject;

        if ( serviceProvider is null || target is null )
            return this;

        if ( ProvideValue ( serviceProvider, target, provider?.TargetProperty ) is { } overridden )
            return overridden;

        if ( provider?.TargetProperty is DependencyProperty property && property.PropertyType != typeof ( string ) && property.PropertyType != typeof ( object ) )
            throw new InvalidOperationException ( $"{ GetType ( ).FullName } is not a valid value for { target.GetType ( ).Name }.{ property.Name }" );

        if ( provider?.TargetProperty is PropertyInfo property2 && property2.PropertyType != typeof ( string ) && property2.PropertyType != typeof ( object )  )
            throw new InvalidOperationException ( $"{ GetType ( ).FullName } is not a valid value for { target.GetType ( ).Name }.{ property2.Name }" );

        if ( target is DependencyObject element and not TriggerBase )
            return SelectGlyph ( element );
        if ( target is InlineCollection inlines )
            return SelectGlyph ( inlines );
        if ( target is UIElementCollection elements )
            return SelectGlyph ( elements );

        if ( provider?.TargetProperty is null && target is not ICollection < object > or ICollection < string > )
            throw new InvalidOperationException ( $"{ GetType ( ).FullName } is not valid inside { target.GetType ( ).Name }" );

        return SelectGlyph ( );
    }

    protected virtual object? ProvideValue ( IServiceProvider serviceProvider, object target, object? targetProperty )
    {
        return null;
    }

    protected virtual string? SelectGlyph ( FontFamily? fontFamily, out FontFamily? fontFamilyToSet )
    {
        fontFamilyToSet = null;

        if ( Emoji is not Unicode.Emoji.None )
            return Emoji.ToUnicode ( Variant );

        return null;
    }

    private Run SelectGlyph ( InlineCollection inlines )
    {
        var fontFamily = GetFontFamily ( inlines );

        var glyph = SelectGlyph ( fontFamily, out var fontFamilyToSet );
        var text  = Format is not null ? string.Format ( CultureInfo.InvariantCulture, Format, glyph ) : glyph ?? string.Empty;

        if ( fontFamilyToSet is not null )
            return new Run { Text = text, FontFamily = fontFamilyToSet };

        return new Run { Text = text };
    }

    private TextBlock SelectGlyph ( UIElementCollection elements )
    {
        var fontFamily = GetFontFamily ( elements );

        var glyph = SelectGlyph ( fontFamily, out var fontFamilyToSet );
        var text  = Format is not null ? string.Format ( CultureInfo.InvariantCulture, Format, glyph ) : glyph ?? string.Empty;

        if ( fontFamilyToSet is not null )
            return new TextBlock ( new Run { Text = text, FontFamily = fontFamilyToSet } );

        return new TextBlock ( new Run { Text = text } );
    }

    private string SelectGlyph ( DependencyObject element )
    {
        var fontFamily = TextElement.GetFontFamily ( element );

        var glyph = SelectGlyph ( fontFamily, out var fontFamilyToSet );

        var text  = Format is not null ? string.Format ( CultureInfo.InvariantCulture, Format, glyph ) : glyph ?? string.Empty;

        if ( fontFamily is not null && fontFamilyToSet is not null )
            TextElement.SetFontFamily ( element, Format is not null ? new FontFamily ( string.Concat ( fontFamily.Source, ", ", fontFamilyToSet.Source ) ) :
                                                                      fontFamilyToSet );

        return text;
    }

    private string SelectGlyph ( )
    {
        var glyph = SelectGlyph ( null, out _ );

        var text  = Format is not null ? string.Format ( CultureInfo.InvariantCulture, Format, glyph ) : glyph ?? string.Empty;

        return text;
    }

    private static FontFamily GetFontFamily ( InlineCollection inlines )
    {
        if ( inlines.FirstInline is not null )
            return TextElement.GetFontFamily ( inlines.FirstInline.Parent ?? inlines.FirstInline );

        var run = new Run ( );
        inlines.Add ( run );
        var f = TextElement.GetFontFamily ( run.Parent ?? run );
        inlines.Remove ( run );
        return f;
    }

    private static FontFamily GetFontFamily ( UIElementCollection elements )
    {
        if ( elements.Count > 0 )
            return TextElement.GetFontFamily ( LogicalTreeHelper.GetParent ( elements [ 0 ] ) ?? elements [ 0 ] );

        var run = new UIElement ( );
        elements.Add ( run );
        var f = TextElement.GetFontFamily ( LogicalTreeHelper.GetParent ( run ) ?? run );
        elements.Remove ( run );
        return f;
    }
}