using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Stylish;

public abstract class ArithmeticConverter < T > : Converter, IValueConverter
{
    public T? Operand { get; set; }

    public object? Convert     ( object? value, Type? targetType, object? parameter, CultureInfo? culture ) => Convert ( Operate, value, parameter );
    public object? ConvertBack ( object? value, Type? targetType, object? parameter, CultureInfo? culture ) => Convert ( Inverse, value, parameter );

    protected abstract T Convert ( object value );
    protected abstract T Operate ( T left, T right );
    protected abstract T Inverse ( T left, T right );

    private object? Convert ( Func < T, T, T > operate, object? value, object? parameter )
    {
        if ( value is null || Operand is null && parameter is null )
            return DependencyProperty.UnsetValue;

        try
        {
            var left  = Convert ( value );
            var right = Operand ?? Convert ( parameter! );

            return operate ( left, right );
        }
        catch ( Exception exception )
        when  ( exception is FormatException or InvalidCastException or ArithmeticException )
        {
            return DependencyProperty.UnsetValue;
        }
    }
}