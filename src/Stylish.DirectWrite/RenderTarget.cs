using Windows.Win32.Graphics.Direct2D;

using Stylish.DirectWrite.Brushes;

namespace Stylish.DirectWrite;

public abstract class RenderTarget
{
    internal abstract ID2D1RenderTarget Interface { get; }

    public TextAntialiasing TextAntialiasing
    {
        get => (TextAntialiasing) Interface.GetTextAntialiasMode ( );
        set => Interface.SetTextAntialiasMode ( (D2D1_TEXT_ANTIALIAS_MODE) value );
    }

    public unsafe void Clear ( )
    {
        EnsureBeganDraw ( );

        Interface.Clear ( default );
    }

    public void Draw ( TextLayout textLayout, Brush brush )
    {
        Draw ( 0f, 0f, textLayout, brush, RenderOptions.EnableColorFont );
    }

    public void Draw ( float x, float y, TextLayout textLayout, Brush brush )
    {
        Draw ( x, y, textLayout, brush, RenderOptions.EnableColorFont );
    }

    public void Draw ( TextLayout textLayout, Brush brush, RenderOptions options )
    {
        Draw ( 0f, 0f, textLayout, brush, options );
    }

    public unsafe void Draw ( float x, float y, TextLayout textLayout, Brush brush, RenderOptions options )
    {
        ArgumentNullException.ThrowIfNull ( textLayout );
        ArgumentNullException.ThrowIfNull ( brush );

        EnsureBeganDraw ( );

        Interface.DrawTextLayout ( new ( ) { x = x, y = y }, textLayout.Interface, brush.Interface, (D2D1_DRAW_TEXT_OPTIONS) options );
    }

    public unsafe void Render ( )
    {
        if ( beganDraw )
        {
            Interface.EndDraw ( null, null );

            beganDraw = false;
        }
    }

    private bool beganDraw;

    private void EnsureBeganDraw ( )
    {
        if ( ! beganDraw )
        {
            Interface.BeginDraw ( );

            beganDraw = true;
        }
    }
}