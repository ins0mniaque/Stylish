using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Stylish;

public abstract class ColorIconElement : IconElement
{
    /// <inheritdoc cref="Control.Background" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Brush Background
    {
        get => (Brush) GetValue ( BackgroundProperty );
        set => SetValue ( BackgroundProperty, value );
    }

    public static readonly DependencyProperty BackgroundProperty = RegisterVisualProperty < ColorIconElement > ( TextElement.BackgroundProperty );
}