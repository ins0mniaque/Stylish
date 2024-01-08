using System.Windows;
using System.Windows.Media;

namespace Stylish.Controls;

public abstract class VisualHost : FrameworkElement
{
    protected override Size MeasureOverride ( Size availableSize )
    {
        if ( visual is null )
            ChangeVisual ( GenerateChild ( ref availableSize ), availableSize );

        return size;
    }

    protected abstract Visual? GenerateChild ( ref Size availableSize );

    protected void InvalidateChild ( )
    {
        if ( oldVisual is not null && visual is not null )
            throw new InvalidOperationException ( );

        oldVisual = visual;
        visual    = null;
    }

    private Visual? oldVisual;
    private Visual? visual;
    private Size    size;

    private void ChangeVisual ( Visual? newVisual, Size newSize )
    {
        if ( oldVisual is not null )
        {
            RemoveLogicalChild ( oldVisual );
            RemoveVisualChild  ( oldVisual );

            oldVisual = null;
        }

        visual = newVisual;
        size   = newSize;

        if ( newVisual is not null )
        {
            AddLogicalChild ( visual );
            AddVisualChild  ( visual );
        }
    }

    protected override int VisualChildrenCount => visual is null ? 0 : 1;

    protected override Visual GetVisualChild ( int index )
    {
        if ( visual is null || index is not 0 )
            throw new ArgumentOutOfRangeException ( nameof ( index ) );

        return visual;
    }

    protected static DependencyProperty RegisterVisualProperty < TOwner > ( DependencyProperty property )
    {
        ArgumentNullException.ThrowIfNull ( property );

        if ( property.DefaultMetadata is FrameworkPropertyMetadata metadata )
        {
            var options = FrameworkPropertyMetadataOptions.AffectsMeasure;
            if ( metadata.Inherits )
                options |= FrameworkPropertyMetadataOptions.Inherits;

            return property.AddOwner ( typeof ( TOwner ), new FrameworkPropertyMetadata ( metadata.DefaultValue, options, InvalidateChild ) );
        }

        return property.AddOwner ( typeof ( TOwner ), new PropertyMetadata ( property.DefaultMetadata.DefaultValue, InvalidateChild ) );
    }

    protected static DependencyProperty RegisterVisualProperty < TOwner, T > ( string name, T defaultValue, FrameworkPropertyMetadataOptions options = FrameworkPropertyMetadataOptions.AffectsMeasure )
    {
        return DependencyProperty.Register ( name, typeof ( T ), typeof ( TOwner ), new FrameworkPropertyMetadata ( defaultValue, options, InvalidateChild ) );
    }

    private static void InvalidateChild ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        ( (VisualHost) d ).InvalidateChild ( );
    }
}