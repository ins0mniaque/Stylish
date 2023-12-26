using System.Windows;

namespace Stylish;

public delegate void SetThemeProperty ( object key, object? value = null );

public abstract record Theme
{
    public static readonly ThemeResourceKey ThemeKey = new ( nameof ( Theme ) );

    public event EventHandler? Changed;

    public void Apply   ( FrameworkElement        element   ) => ApplyCore   ( SetThemePropertyFor ( element   ) );
    public void Apply   ( FrameworkContentElement element   ) => ApplyCore   ( SetThemePropertyFor ( element   ) );
    public void Apply   ( ResourceDictionary      resources ) => ApplyCore   ( SetThemePropertyFor ( resources ) );
    public void Unapply ( FrameworkElement        element   ) => UnapplyCore ( SetThemePropertyFor ( element   ) );
    public void Unapply ( FrameworkContentElement element   ) => UnapplyCore ( SetThemePropertyFor ( element   ) );
    public void Unapply ( ResourceDictionary      resources ) => UnapplyCore ( SetThemePropertyFor ( resources ) );

    protected abstract void Apply   ( SetThemeProperty setThemeProperty );
    protected abstract void Unapply ( SetThemeProperty setThemeProperty );

    protected void NotifyChanged ( )
    {
        Changed?.Invoke ( this, EventArgs.Empty );
    }

    private void ApplyCore ( SetThemeProperty setThemeProperty )
    {
        setThemeProperty ( ThemeKey, this );

        Apply ( setThemeProperty );
    }

    private void UnapplyCore ( SetThemeProperty setThemeProperty )
    {
        setThemeProperty ( ThemeKey );

        Unapply ( setThemeProperty );
    }

    private static SetThemeProperty SetThemePropertyFor ( FrameworkElement        element   ) => (key, value) => SetThemeProperty ( element.Resources, element.TryFindResource, key, value );
    private static SetThemeProperty SetThemePropertyFor ( FrameworkContentElement element   ) => (key, value) => SetThemeProperty ( element.Resources, element.TryFindResource, key, value );
    private static SetThemeProperty SetThemePropertyFor ( ResourceDictionary      resources ) => (key, value) => SetThemeProperty ( resources, key => resources [ key ],        key, value );

    private static void SetThemeProperty ( ResourceDictionary resources, Func < object, object > find, object key, object? value )
    {
        if ( find ( key ) is { } resource )
        {
            if ( resource.Equals ( value ) )
                return;

            resources.Remove ( key );
        }

        if ( value is not null )
            resources [ key ] = value;
    }

    [ AttachedPropertyBrowsableForType ( typeof ( FrameworkElement        ) ) ]
    [ AttachedPropertyBrowsableForType ( typeof ( FrameworkContentElement ) ) ]
    public static Theme? GetTheme ( DependencyObject element )
    {
        ArgumentNullException.ThrowIfNull ( element );

        return (Theme?) element.GetValue ( ThemeProperty ) ??
               ( ( element as FrameworkElement        )?.TryFindResource ( ThemeKey ) as Theme ) ??
               ( ( element as FrameworkContentElement )?.TryFindResource ( ThemeKey ) as Theme );
    }

    public static void SetTheme ( DependencyObject element, Theme? theme )
    {
        ArgumentNullException.ThrowIfNull ( element );

        element.SetValue ( ThemeProperty, theme );
    }

    public  static readonly DependencyProperty    ThemeProperty           = DependencyProperty.RegisterAttached         ( "Theme",        typeof ( Theme ),        typeof ( Theme ), new ( null, OnThemeChanged ) );
    private static readonly DependencyPropertyKey ThemeChangedPropertyKey = DependencyProperty.RegisterAttachedReadOnly ( "ThemeChanged", typeof ( EventHandler ), typeof ( Theme ), new ( null ) );

    private static void OnThemeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
        if ( e.OldValue is Theme oldTheme )
        {
            if      ( d is FrameworkElement        fe  ) fe .Unloaded -= DetachThemeChanged;
            else if ( d is FrameworkContentElement fce ) fce.Unloaded -= DetachThemeChanged;

            if ( d.GetValue ( ThemeChangedPropertyKey.DependencyProperty ) is EventHandler themeChanged )
            {
                oldTheme.Changed -= themeChanged;

                d.ClearValue ( ThemeChangedPropertyKey );
            }

            if ( e.NewValue is null )
                oldTheme.Unapply ( d );
        }

        if ( e.NewValue is Theme newTheme )
        {
            if      ( d is FrameworkElement        fe  ) fe .Unloaded += DetachThemeChanged;
            else if ( d is FrameworkContentElement fce ) fce.Unloaded += DetachThemeChanged;

            var themeChanged = (EventHandler) ( (o, e) => newTheme.Apply ( d ) );

            d.SetValue ( ThemeChangedPropertyKey, themeChanged );

            newTheme.Changed += themeChanged;

            newTheme.Apply ( d );
        }
    }

    private void Apply ( DependencyObject d )
    {
        if      ( d is FrameworkElement        fe  ) Apply ( fe );
        else if ( d is FrameworkContentElement fce ) Apply ( fce );
    }

    private void Unapply ( DependencyObject d )
    {
        if      ( d is FrameworkElement        fe  ) Unapply ( fe );
        else if ( d is FrameworkContentElement fce ) Unapply ( fce );
    }

    private static void DetachThemeChanged ( object sender, RoutedEventArgs e )
    {
        var d = (DependencyObject) sender;

        if      ( d is FrameworkElement        fe  ) fe .Unloaded -= DetachThemeChanged;
        else if ( d is FrameworkContentElement fce ) fce.Unloaded -= DetachThemeChanged;

        if ( d.GetValue ( ThemeChangedPropertyKey.DependencyProperty ) is EventHandler themeChanged )
        {
            if ( d.GetValue ( ThemeProperty ) is Theme theme )
                theme.Changed -= themeChanged;

            d.ClearValue ( ThemeChangedPropertyKey );
        }
    }
}