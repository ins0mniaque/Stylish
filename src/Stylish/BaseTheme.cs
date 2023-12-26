using System.Windows.Media;

namespace Stylish;

public record BaseTheme : SystemTheme
{
    public static readonly ThemeResourceKey ErrorKey = new ( nameof ( Error ) );

    public Brush Error { get; set; } = Brushes.Crimson;

    protected override void Apply ( SetThemeProperty setThemeProperty )
    {
        ArgumentNullException.ThrowIfNull ( setThemeProperty );

        base.Apply ( setThemeProperty );

        setThemeProperty ( ErrorKey, Error );
    }

    protected override void Unapply ( SetThemeProperty setThemeProperty )
    {
        ArgumentNullException.ThrowIfNull ( setThemeProperty );

        base.Unapply ( setThemeProperty );

        setThemeProperty ( ErrorKey );
    }
}