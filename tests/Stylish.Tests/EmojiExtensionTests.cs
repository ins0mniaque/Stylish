using System.Windows.Controls;

using Stylish.Controls;

namespace Stylish.Tests;

[ TestClass ]
public class EmojiExtensionTests : StylishTests
{
    [ TestMethod ]
    public Task ConstructsFromDefaultCtor ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl Content="{ß:Emoji Symbol=GrinningFace}" />
            """ );

        Assert.IsInstanceOfType < Emoji > ( contentControl.Content );
        Assert.AreEqual ( "😀", ( (Emoji) contentControl.Content ).Glyph );
    } );

    [ TestMethod ]
    public Task ConstructsFromSymbolCtor ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl Content="{ß:Emoji GrinningFace}" />
            """ );

        Assert.IsInstanceOfType < Emoji > ( contentControl.Content );
        Assert.AreEqual ( "😀", ( (Emoji) contentControl.Content ).Glyph );
    } );

    [ TestMethod ]
    public Task IsValidAsSetterValue ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl>
                <ContentControl.Style>
                    <Style TargetType="ContentControl">
                        <Setter Property="Content" Value="{ß:Emoji GrinningFace}" />
                    </Style>
                </ContentControl.Style>
            </ContentControl>
            """ );

        Assert.IsInstanceOfType < Emoji > ( contentControl.Content );
        Assert.AreEqual ( "😀", ( (Emoji) contentControl.Content ).Glyph );
    } );
}