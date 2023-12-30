using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Stylish;

[ ContentProperty ( nameof ( Data ) ) ]
public class PathIcon : ColorIconElement
{
    /// <inheritdoc cref="Path.Data" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public Geometry Data
    {
        get => (Geometry) GetValue ( DataProperty );
        set => SetValue ( DataProperty, value );
    }

    public static readonly DependencyProperty DataProperty = RegisterVisualProperty < PathIcon > ( Path.DataProperty );

    /// <inheritdoc cref="Shape.StrokeThickness" />
    [ Bindable ( true ), Category ( "Appearance" ) ]
    public double StrokeThickness
    {
        get => (double) GetValue ( StrokeThicknessProperty );
        set => SetValue ( StrokeThicknessProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeThicknessProperty" />
    public static readonly DependencyProperty StrokeThicknessProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeThicknessProperty );

    /// <inheritdoc cref="Shape.StrokeStartLineCap" />
    public PenLineCap StrokeStartLineCap
    {
        get => (PenLineCap) GetValue ( StrokeStartLineCapProperty );
        set => SetValue ( StrokeStartLineCapProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeStartLineCapProperty" />
    public static readonly DependencyProperty StrokeStartLineCapProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeStartLineCapProperty );

    /// <inheritdoc cref="Shape.StrokeEndLineCap" />
    public PenLineCap StrokeEndLineCap
    {
        get => (PenLineCap) GetValue ( StrokeEndLineCapProperty );
        set => SetValue ( StrokeEndLineCapProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeEndLineCapProperty" />
    public static readonly DependencyProperty StrokeEndLineCapProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeEndLineCapProperty );
 
    /// <inheritdoc cref="Shape.StrokeDashCap" />
    public PenLineCap StrokeDashCap
    {
        get => (PenLineCap) GetValue ( StrokeDashCapProperty );
        set => SetValue ( StrokeDashCapProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeDashCapProperty" />
    public static readonly DependencyProperty StrokeDashCapProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeDashCapProperty );
 
    /// <inheritdoc cref="Shape.StrokeLineJoin" />
    public PenLineJoin StrokeLineJoin
    {
        get => (PenLineJoin) GetValue ( StrokeLineJoinProperty );
        set => SetValue ( StrokeLineJoinProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeLineJoinProperty" />
    public static readonly DependencyProperty StrokeLineJoinProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeLineJoinProperty );
 
    /// <inheritdoc cref="Shape.StrokeMiterLimit" />
    public double StrokeMiterLimit
    {
        get => (double) GetValue ( StrokeMiterLimitProperty );
        set => SetValue ( StrokeMiterLimitProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeMiterLimitProperty" />
    public static readonly DependencyProperty StrokeMiterLimitProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeMiterLimitProperty );

    /// <inheritdoc cref="Shape.StrokeDashOffset" />
    public double StrokeDashOffset
    {
        get => (double) GetValue ( StrokeDashOffsetProperty );
        set => SetValue ( StrokeDashOffsetProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeDashOffsetProperty" />
    public static readonly DependencyProperty StrokeDashOffsetProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeDashOffsetProperty );

    /// <inheritdoc cref="Shape.StrokeDashArray" />
    [ SuppressMessage ( "Usage", "CA2227:Collection properties should be read only", Justification = "Shape.StrokeDashArray" ) ]
    public DoubleCollection StrokeDashArray
    {
        get => (DoubleCollection) GetValue ( StrokeDashArrayProperty );
        set => SetValue ( StrokeDashArrayProperty, value );
    }

    /// <inheritdoc cref="Shape.StrokeDashArrayProperty" />
    public static readonly DependencyProperty StrokeDashArrayProperty = RegisterVisualProperty < PathIcon > ( Shape.StrokeDashArrayProperty );

    private Pen?     pen;
    private Size     size;
    private Geometry sizeGeometry  = Geometry.Empty;
    private double   sizeThickness = double.NegativeInfinity;

    protected override Size MeasureIcon ( )
    {
        pen = new Pen ( Foreground, StrokeThickness )
        {
            StartLineCap = StrokeStartLineCap,
            EndLineCap   = StrokeEndLineCap,
            DashCap      = StrokeDashCap,
            LineJoin     = StrokeLineJoin,
            MiterLimit   = StrokeMiterLimit,
            DashStyle    = new DashStyle ( StrokeDashArray, StrokeDashOffset )
        };

        if ( sizeGeometry != Data || sizeThickness != StrokeThickness )
        {
            size          = Data.GetRenderBounds ( pen ).Size;
            sizeGeometry  = Data;
            sizeThickness = StrokeThickness;
        }

        return size.IsEmpty ? default : size;
    }

    protected override void DrawIcon ( DrawingContext drawingContext )
    {
        ArgumentNullException.ThrowIfNull ( drawingContext );

        drawingContext.DrawGeometry ( Background, pen, Data );

        pen = null;
    }
}