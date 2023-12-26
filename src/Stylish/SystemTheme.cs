using System.Windows.Media;

namespace Stylish;

// TODO: Clean this up...
public record SystemTheme : Theme
{
    public SystemTheme ( )
    {
        UX = ChangeUX ( UX.Current );

        Synchronized = true;
    }

    public static readonly ThemeResourceKey ModeKey       = new ( nameof ( Mode       ) );
    public static readonly ThemeResourceKey WindowKey     = new ( nameof ( Window     ) );
    public static readonly ThemeResourceKey WindowTextKey = new ( nameof ( WindowText ) );
    public static readonly ThemeResourceKey AccentKey     = new ( nameof ( Accent     ) );
    public static readonly ThemeResourceKey AccentTextKey = new ( nameof ( AccentText ) );

    private Mode?  mode;
    private Brush? window;
    private Brush? windowText;
    private Brush? accent;
    private Brush? accentText;

    private SolidColorBrush uxWindow     = Brushes.Transparent;
    private SolidColorBrush uxWindowText = Brushes.Transparent;
    private SolidColorBrush uxAccent     = Brushes.Transparent;
    private SolidColorBrush uxAccentText = Brushes.Transparent;

    public UX UX { get; private set; }

    private UX ChangeUX ( UX value )
    {
        if ( uxWindow    .Color != value.Window     ) uxWindow     = new SolidColorBrush ( value.Window     );
        if ( uxWindowText.Color != value.WindowText ) uxWindowText = new SolidColorBrush ( value.WindowText );
        if ( uxAccent    .Color != value.Accent     ) uxAccent     = new SolidColorBrush ( value.Accent     );
        if ( uxAccentText.Color != value.AccentText ) uxAccentText = new SolidColorBrush ( value.AccentText );

        return value;
    }

    public Mode Mode
    {
        get => mode ?? UX.Mode;
        set
        {
            mode = value;

            Synchronized = false;
            UX           = ChangeUX ( UX.Get ( value ) );

            NotifyChanged ( );
        }
    }

    public Brush Window     { get => window     ?? uxWindow;     set => window     = value; }
    public Brush WindowText { get => windowText ?? uxWindowText; set => windowText = value; }
    public Brush Accent     { get => accent     ?? uxAccent;     set => accent     = value; }
    public Brush AccentText { get => accentText ?? uxAccentText; set => accentText = value; }

    private bool synchronized;
    public  bool Synchronized
    {
        get => synchronized;
        set
        {
            UX.CurrentChanged -= OnThemeChanged;

            if ( synchronized = value )
                UX.CurrentChanged += OnThemeChanged;
        }
    }

    private void OnThemeChanged ( object? sender, EventArgs e )
    {
        UX = ChangeUX ( UX.Current );

        OnUXChanged ( );

        NotifyChanged ( );
    }

    protected virtual void OnUXChanged ( ) { }

    protected override void Apply ( SetThemeProperty setThemeProperty )
    {
        ArgumentNullException.ThrowIfNull ( setThemeProperty );

        setThemeProperty ( ModeKey,       Mode       );
        setThemeProperty ( WindowKey,     Window     );
        setThemeProperty ( WindowTextKey, WindowText );
        setThemeProperty ( AccentKey,     Accent     );
        setThemeProperty ( AccentTextKey, AccentText );
    }

    protected override void Unapply ( SetThemeProperty setThemeProperty )
    {
        ArgumentNullException.ThrowIfNull ( setThemeProperty );

        setThemeProperty ( ModeKey       );
        setThemeProperty ( WindowKey     );
        setThemeProperty ( WindowTextKey );
        setThemeProperty ( AccentKey     );
        setThemeProperty ( AccentTextKey );
    }
}