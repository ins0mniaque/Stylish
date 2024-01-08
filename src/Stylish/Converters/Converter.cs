using System.ComponentModel;

namespace Stylish.Converters;

public abstract class Converter
{
    protected static Exception GetConvertFromException ( object? value )                       => TypeConverterException.GetConvertFromExceptionFor ( value );
    protected static Exception GetConvertToException   ( object? value, Type destinationType ) => TypeConverterException.GetConvertToExceptionFor   ( value, destinationType );

    private sealed class TypeConverterException : TypeConverter
    {
        public static Exception GetConvertFromExceptionFor ( object? value )                       => new TypeConverterException ( ).GetConvertFromException ( value );
        public static Exception GetConvertToExceptionFor   ( object? value, Type destinationType ) => new TypeConverterException ( ).GetConvertToException   ( value, destinationType );
    }
}