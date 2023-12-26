using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

namespace Stylish;

[ Ambient, Localizability ( LocalizationCategory.Ignore ), UsableDuringInitialization ( true ) ]
[ SuppressMessage ( "Design", "CA1010:Generic interface should also be implemented", Justification = "ResourceDictionary" ) ]
public class Styles : ResourceDictionary
{
    private static readonly Uri Uri = new ( "pack://application:,,,/Stylish;component/Stylish.xaml" );

    public Styles ( )
    {
        Source = Uri;
    }
}