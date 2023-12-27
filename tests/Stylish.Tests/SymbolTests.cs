using System.Windows.Controls;
using System.Windows.Documents;

namespace Stylish.Tests;

[ TestClass ]
public class SymbolTests : StylishTests
{
    [ TestMethod ]
    public Task FormatsCorrectly ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock Text="{ß:Symbol Emoji=GrinningFace, Format=Emoji: {0}}" />
            """ );

        Assert.AreEqual ( "Emoji: 😀", textBlock.Text );
    } );

    [ TestMethod ]
    public Task InsertsStringAsText ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock Text="{ß:Symbol Emoji=GrinningFace}" />
            """ );

        Assert.AreEqual ( "😀", textBlock.Text );
    } );

    [ TestMethod ]
    public Task InsertsInline ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                <ß:Symbol Emoji="GrinningFace" />
            </TextBlock>
            """ );

        Assert.AreEqual ( 1, textBlock.Inlines.Count );
        Assert.IsInstanceOfType < Run > ( textBlock.Inlines.FirstInline );
        Assert.AreEqual ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ TestMethod ]
    public Task InsertsInlineAsInlined ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                Emoji: <ß:Symbol Emoji="GrinningFace" />
            </TextBlock>
            """ );

        Assert.AreEqual ( 2, textBlock.Inlines.Count );
        Assert.IsInstanceOfType < Run > ( textBlock.Inlines.LastInline );
        Assert.AreEqual ( "😀", ( (Run) textBlock.Inlines.LastInline ).Text );
    } );

    [ TestMethod ]
    public Task InsertsStringAsContent ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl Content="{ß:Symbol Emoji=GrinningFace}" />
            """ );

        Assert.AreEqual ( "😀", contentControl.Content );
    } );

    [ TestMethod ]
    public Task InsertsStringInContent ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl>
                <ß:Symbol Emoji="GrinningFace" />
            </ContentControl>
            """ );

        Assert.AreEqual ( "😀", contentControl.Content );
    } );

    [ TestMethod ]
    public Task InsertsTextBlockInPanel ( ) => STA ( ( ) =>
    {
        var stackPanel = Parse < StackPanel > (
            """
            <StackPanel>
                <ß:Symbol Emoji="GrinningFace" />
            </StackPanel>
            """ );

        Assert.AreEqual ( 1, stackPanel.Children.Count );
        Assert.IsInstanceOfType < TextBlock > ( stackPanel.Children [ 0 ] );

        var textBlock = (TextBlock) stackPanel.Children [ 0 ];

        Assert.AreEqual ( 1, textBlock.Inlines.Count );
        Assert.IsInstanceOfType < Run > ( textBlock.Inlines.FirstInline );
        Assert.AreEqual ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ TestMethod ]
    public Task IsValidAsSetterValue ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                <TextBlock.Style>
                    <Style TargetType="TextBlock">
                        <Setter Property="Text" Value="{ß:Symbol Emoji=GrinningFace}" />
                    </Style>
                </TextBlock.Style>
            </TextBlock>
            """ );

        Assert.AreEqual ( 1, textBlock.Inlines.Count );
        Assert.IsInstanceOfType < Run > ( textBlock.Inlines.FirstInline );
        Assert.AreEqual ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ TestMethod ]
    public Task IsValidAsTriggerValue ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                <TextBlock.Style>
                    <Style TargetType="TextBlock">
                        <Setter Property="Tag" Value="{ß:Symbol Emoji=GrinningFace}" />
                        <Style.Triggers>
                            <Trigger Property="Tag" Value="{ß:Symbol Emoji=GrinningFace}">
                                <Setter Property="Text" Value="Triggered" />
                            </Trigger>
                        </Style.Triggers>
                    </Style>
                </TextBlock.Style>
            </TextBlock>
            """ );

        Assert.AreEqual ( "Triggered", textBlock.Text );
    } );
}