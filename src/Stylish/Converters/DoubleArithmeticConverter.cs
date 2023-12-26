using System.Globalization;

namespace Stylish;

public abstract class DoubleArithmeticConverter : ArithmeticConverter < double >
{
    protected override double Convert ( object value ) => System.Convert.ToDouble ( value, CultureInfo.InvariantCulture );
}

public sealed class DoubleAddConverter : DoubleArithmeticConverter
{
    protected override double Operate ( double left, double right ) => left + right;
    protected override double Inverse ( double left, double right ) => left - right;
}

public sealed class DoubleScaleConverter : DoubleArithmeticConverter
{
    protected override double Operate ( double left, double right ) => left * right;
    protected override double Inverse ( double left, double right ) => left / right;
}