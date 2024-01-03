using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Stylish.Demo;

public class ColorRichTextBox : RichTextBox
{
    protected override void OnTextChanged ( TextChangedEventArgs e )
    {
        ArgumentNullException.ThrowIfNull ( e );

        base.OnTextChanged ( e );

        foreach ( var change in e.Changes )
        {
            var contentStart = Document.ContentStart;

            if ( change.AddedLength > 0 )
            {
                var changeStart = contentStart.GetPositionAtOffset ( change.Offset );
                var changeRange = new TextRange ( changeStart, changeStart.GetPositionAtOffset ( change.AddedLength ) );
                var emojiRange  = FindTextInRange ( changeRange, "a", StringComparison.Ordinal );

                if ( emojiRange is null )
                    continue;

                var caret           = CaretPosition;
                var changeIsAtCaret = emojiRange.Start.CompareTo ( caret ) <  0 &&
                                      emojiRange.End  .CompareTo ( caret ) >= 0;

                BeginChange ( );

                emojiRange.Text = string.Empty;

                _ = new InlineUIContainer ( new ColorTextBlock { Text = "😾" }, emojiRange.End ) { BaselineAlignment = BaselineAlignment.TextBottom };

                if ( changeIsAtCaret )
                    CaretPosition = emojiRange.End.GetNextInsertionPosition ( LogicalDirection.Forward );

                EndChange ( );
            }
        }
    }

    private static TextRange? FindTextInRange ( TextRange searchRange, string searchText, StringComparison comparisonType )
    {
        int offset = searchRange.Text.IndexOf ( searchText, comparisonType );
        if ( offset < 0 )
            return null;

        var start = GetTextPositionAtOffset ( searchRange.Start, offset );
        var range = new TextRange ( start, GetTextPositionAtOffset ( start, searchText.Length ) );

        return range;
    }

    private static TextPointer? GetTextPositionAtOffset ( TextPointer? position, int characterCount )
    {
        while ( position is not null )
        {
            if ( position.GetPointerContext ( LogicalDirection.Forward ) is TextPointerContext.Text )
            {
                var count = position.GetTextRunLength ( LogicalDirection.Forward );
                if ( characterCount <= count )
                    return position.GetPositionAtOffset ( characterCount );

                characterCount -= count;
            }

            var nextContextPosition = position.GetNextContextPosition ( LogicalDirection.Forward );
            if ( nextContextPosition is null )
                return position;

            position = nextContextPosition;
        }

        return position;
    }
}