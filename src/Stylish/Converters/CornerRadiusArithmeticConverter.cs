using System.Windows;

namespace Stylish;

public abstract class CornerRadiusArithmeticConverter : TypeConverterArithmeticConverter < CornerRadius > { }

public sealed class CornerRadiusAddConverter : CornerRadiusArithmeticConverter
{
    protected override CornerRadius Operate ( CornerRadius left, CornerRadius right )
    {
        return new CornerRadius ( left.TopLeft     + right.TopLeft,
                                  left.TopRight    + right.TopRight,
                                  left.BottomLeft  + right.BottomLeft,
                                  left.BottomRight + right.BottomRight );
    }

    protected override CornerRadius Inverse ( CornerRadius left, CornerRadius right )
    {
        return new CornerRadius ( left.TopLeft     - right.TopLeft,
                                  left.TopRight    - right.TopRight,
                                  left.BottomLeft  - right.BottomLeft,
                                  left.BottomRight - right.BottomRight );
    }
}

public sealed class CornerRadiusScaleConverter : CornerRadiusArithmeticConverter
{
    protected override CornerRadius Operate ( CornerRadius left, CornerRadius right )
    {
        return new CornerRadius ( left.TopLeft     * right.TopLeft,
                                  left.TopRight    * right.TopRight,
                                  left.BottomLeft  * right.BottomLeft,
                                  left.BottomRight * right.BottomRight );
    }

    protected override CornerRadius Inverse ( CornerRadius left, CornerRadius right )
    {
        return new CornerRadius ( left.TopLeft     / right.TopLeft,
                                  left.TopRight    / right.TopRight,
                                  left.BottomLeft  / right.BottomLeft,
                                  left.BottomRight / right.BottomRight );
    }
}