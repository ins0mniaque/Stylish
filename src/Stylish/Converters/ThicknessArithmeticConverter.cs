using System.Windows;

namespace Stylish.Converters;

public abstract class ThicknessArithmeticConverter : TypeConverterArithmeticConverter < Thickness > { }

public sealed class ThicknessAddConverter : ThicknessArithmeticConverter
{
    protected override Thickness Operate ( Thickness left, Thickness right )
    {
        return new Thickness ( left.Left   + right.Left,
                               left.Top    + right.Top,
                               left.Right  + right.Right,
                               left.Bottom + right.Bottom );
    }

    protected override Thickness Inverse ( Thickness left, Thickness right )
    {
        return new Thickness ( left.Left   - right.Left,
                               left.Top    - right.Top,
                               left.Right  - right.Right,
                               left.Bottom - right.Bottom );
    }
}

public sealed class ThicknessScaleConverter : ThicknessArithmeticConverter
{
    protected override Thickness Operate ( Thickness left, Thickness right )
    {
        return new Thickness ( left.Left   * right.Left,
                               left.Top    * right.Top,
                               left.Right  * right.Right,
                               left.Bottom * right.Bottom );
    }

    protected override Thickness Inverse ( Thickness left, Thickness right )
    {
        return new Thickness ( left.Left   / right.Left,
                               left.Top    / right.Top,
                               left.Right  / right.Right,
                               left.Bottom / right.Bottom );
    }
}