using System.Windows;
using System.Windows.Media;

namespace Stylish;

public static class Element
{
    public static CornerRadius GetCornerRadius ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (CornerRadius) element.GetValue ( CornerRadiusProperty );
    }

    public static void SetCornerRadius ( DependencyObject element, CornerRadius cornerRadius )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( CornerRadiusProperty, cornerRadius );
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached ( "CornerRadius", typeof ( CornerRadius ), typeof ( Element ) );

    public static Brush? GetHighlight ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (Brush?) element.GetValue ( HighlightProperty );
    }

    public static void SetHighlight ( DependencyObject element, Brush? brush )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( HighlightProperty, brush );
    }

    public static readonly DependencyProperty HighlightProperty = DependencyProperty.RegisterAttached ( "Highlight", typeof ( Brush ), typeof ( Element ) );

    public static double GetHighlightOpacity ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (double) element.GetValue ( HighlightProperty );
    }

    public static void SetHighlightOpacity ( DependencyObject element, double opacity )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( HighlightOpacityProperty, opacity );
    }

    public static readonly DependencyProperty HighlightOpacityProperty = DependencyProperty.RegisterAttached ( "HighlightOpacity", typeof ( double ), typeof ( Element ) );

    public static Brush? GetIconBrush ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (Brush?) element.GetValue ( IconBrushProperty );
    }

    public static void SetIconBrush ( DependencyObject element, Brush? brush )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( IconBrushProperty, brush );
    }

    public static readonly DependencyProperty IconBrushProperty = DependencyProperty.RegisterAttached ( "IconBrush", typeof ( Brush ), typeof ( Element ), new ( Brushes.Black ) );

    public static double GetIconSize ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (double) element.GetValue ( IconSizeProperty );
    }

    public static void SetIconSize ( DependencyObject element, double size )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( IconSizeProperty, size );
    }

    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.RegisterAttached ( "IconSize", typeof ( double ), typeof ( Element ), new ( SystemFonts.MessageFontSize ) );
}