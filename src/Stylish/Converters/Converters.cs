namespace Stylish;

[ System.Diagnostics.CodeAnalysis.SuppressMessage ( "Naming", "CA1724:Type names should not match namespaces", Justification = "None" ) ]
public static class Converters
{
    public static CornerRadiusScaleConverter LeftCornerRadius   { get; } = new ( ) { Operand = new ( 1.0, 0.0, 0.0, 1.0 ) };
    public static CornerRadiusScaleConverter TopCornerRadius    { get; } = new ( ) { Operand = new ( 1.0, 1.0, 0.0, 0.0 ) };
    public static CornerRadiusScaleConverter RightCornerRadius  { get; } = new ( ) { Operand = new ( 0.0, 1.0, 1.0, 0.0 ) };
    public static CornerRadiusScaleConverter BottomCornerRadius { get; } = new ( ) { Operand = new ( 0.0, 0.0, 1.0, 1.0 ) };

    public static ThicknessScaleConverter LeftThickness   { get; } = new ( ) { Operand = new ( 1.0, 0.0, 0.0, 0.0 ) };
    public static ThicknessScaleConverter TopThickness    { get; } = new ( ) { Operand = new ( 0.0, 1.0, 0.0, 0.0 ) };
    public static ThicknessScaleConverter RightThickness  { get; } = new ( ) { Operand = new ( 0.0, 0.0, 1.0, 0.0 ) };
    public static ThicknessScaleConverter BottomThickness { get; } = new ( ) { Operand = new ( 0.0, 0.0, 0.0, 1.0 ) };

    public static ThicknessScaleConverter LeftlessThickness   { get; } = new ( ) { Operand = new ( 0.0, 1.0, 1.0, 1.0 ) };
    public static ThicknessScaleConverter ToplessThickness    { get; } = new ( ) { Operand = new ( 1.0, 0.0, 1.0, 1.0 ) };
    public static ThicknessScaleConverter RightlessThickness  { get; } = new ( ) { Operand = new ( 1.0, 1.0, 0.0, 1.0 ) };
    public static ThicknessScaleConverter BottomlessThickness { get; } = new ( ) { Operand = new ( 1.0, 1.0, 1.0, 0.0 ) };
}