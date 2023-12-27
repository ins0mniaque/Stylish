using System.Windows.Controls;
using System.Windows.Documents;

namespace Stylish.Tests;

public class SymbolTests : StylishTests
{
    [ Fact ]
    public Task FormatsCorrectly ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock Text="{ß:Symbol Emoji=GrinningFace, Format=Emoji: {0}}" />
            """ );

        Assert.Equal ( "Emoji: 😀", textBlock.Text );
    } );

    [ Fact ]
    public Task InsertsStringAsText ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock Text="{ß:Symbol Emoji=GrinningFace}" />
            """ );

        Assert.Equal ( "😀", textBlock.Text );
    } );

    [ Fact ]
    public Task InsertsInline ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                <ß:Symbol Emoji="GrinningFace" />
            </TextBlock>
            """ );

        Assert.Single ( textBlock.Inlines );
        Assert.IsType < Run > ( textBlock.Inlines.FirstInline );
        Assert.Equal  ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ Fact ]
    public Task InsertsInlineAsInlined ( ) => STA ( ( ) =>
    {
        var textBlock = Parse < TextBlock > (
            """
            <TextBlock>
                Emoji: <ß:Symbol Emoji="GrinningFace" />
            </TextBlock>
            """ );

        Assert.Equal ( 2, textBlock.Inlines.Count );
        Assert.IsType < Run > ( textBlock.Inlines.LastInline );
        Assert.Equal ( "😀", ( (Run) textBlock.Inlines.LastInline ).Text );
    } );

    [ Fact ]
    public Task InsertsStringAsContent ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl Content="{ß:Symbol Emoji=GrinningFace}" />
            """ );

        Assert.Equal ( "😀", contentControl.Content );
    } );

    [ Fact ]
    public Task InsertsStringInContent ( ) => STA ( ( ) =>
    {
        var contentControl = Parse < ContentControl > (
            """
            <ContentControl>
                <ß:Symbol Emoji="GrinningFace" />
            </ContentControl>
            """ );

        Assert.Equal ( "😀", contentControl.Content );
    } );

    [ Fact ]
    public Task InsertsTextBlockInPanel ( ) => STA ( ( ) =>
    {
        var stackPanel = Parse < StackPanel > (
            """
            <StackPanel>
                <ß:Symbol Emoji="GrinningFace" />
            </StackPanel>
            """ );

        Assert.Single ( stackPanel.Children );
        Assert.IsType < TextBlock > ( stackPanel.Children [ 0 ] );

        var textBlock = (TextBlock) stackPanel.Children [ 0 ];

        Assert.Single ( textBlock.Inlines );
        Assert.IsType < Run > ( textBlock.Inlines.FirstInline );
        Assert.Equal  ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ Fact ]
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

        Assert.Single ( textBlock.Inlines );
        Assert.IsType < Run > ( textBlock.Inlines.FirstInline );
        Assert.Equal  ( "😀", ( (Run) textBlock.Inlines.FirstInline ).Text );
    } );

    [ Fact ]
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

        Assert.Equal ( "Triggered", textBlock.Text );
    } );
}