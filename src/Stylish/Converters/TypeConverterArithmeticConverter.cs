using System.ComponentModel;
using System.Globalization;

namespace Stylish;

public abstract class TypeConverterArithmeticConverter < T > : ArithmeticConverter < T >
{
    private static readonly TypeConverter converter = TypeDescriptor.GetConverter ( typeof ( T ) );

    protected override T Convert ( object value )
    {
        if ( value is T typed )
            return typed;

        ArgumentNullException.ThrowIfNull ( value );

        if ( converter.CanConvertFrom ( value.GetType ( ) ) &&
             converter.ConvertFrom    ( value ) is T converted )
            return converted;

        if ( System.Convert.ToString ( value, CultureInfo.InvariantCulture ) is { } text &&
             converter.ConvertFromInvariantString ( text ) is T convertedFromString )
            return convertedFromString;

        throw GetConvertFromException ( value );
    }
}