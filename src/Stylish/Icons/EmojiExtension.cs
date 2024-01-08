namespace Stylish;

public class EmojiExtension : FontIconExtension < Emoji, Unicode.Emoji >
{
    public EmojiExtension ( )                      { }
    public EmojiExtension ( Unicode.Emoji symbol ) { Symbol = symbol; }
}