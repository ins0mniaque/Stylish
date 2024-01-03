using System.Windows.Documents;

namespace Stylish.Demo;

public partial class MainWindow
{
    public MainWindow ( )
    {
        InitializeComponent ( );
    }

    private void RichTextBox_TextChanged ( object sender, System.Windows.Controls.TextChangedEventArgs e )
    {
        foreach ( var change in e.Changes )
        {
            if ( change.AddedLength > 0 )
            {
                var range = new TextRange ( RichTextBox.Document.ContentStart.GetPositionAtOffset ( change.Offset ),
                                            RichTextBox.Document.ContentStart.GetPositionAtOffset ( change.Offset + change.AddedLength ) );

                if ( range.Text.Contains ( 'a', StringComparison.Ordinal ) )
                {
                    var caret = RichTextBox.CaretPosition;

                    range.Text = range.Text.Replace ( 'a', 'b' );

                    RichTextBox.CaretPosition = caret.GetNextInsertionPosition ( LogicalDirection.Forward );
                }
            }
        }
    }
}