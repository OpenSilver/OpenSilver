#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Documents;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Documents;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class RichTextBox : Control
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.RichTextBox.IsReadOnly dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.RichTextBox.IsReadOnly dependency
		//     property.
		public static readonly DependencyProperty IsReadOnlyProperty;
		
		public string Xaml
		{
			get;
			set;
		}
		
		//
		// Summary:
		//     Occurs when the content changes in a System.Windows.Controls.RichTextBox.
		public event ContentChangedEventHandler ContentChanged;
		
		//
		// Summary:
		//     Gets or sets a value that determines whether the System.Windows.Controls.RichTextBox
		//     allows and displays the newline or return characters when the ENTER or RETURN
		//     keys are pressed.
		//
		// Returns:
		//     true if the System.Windows.Controls.RichTextBox allows newline characters; otherwise,
		//     false. The default is true.
		public bool AcceptsReturn { get; set; }
		
		//
		// Summary:
		//     Gets or sets how text wrapping occurs if a line of text extends beyond the available
		//     width of the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     One of the System.Windows.TextWrapping values. The default is System.Windows.TextWrapping.Wrap.
		public TextWrapping TextWrapping { get; set; }
		
		//
		// Summary:
		//     Gets the System.Windows.Documents.TextSelection in the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.TextSelection that represents the selected text in
		//     the System.Windows.Controls.RichTextBox.
		public TextSelection Selection { get; }
		
		//
		// Summary:
		//     Gets a System.Windows.Documents.TextPointer that indicates the start of content
		//     in the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that indicates the start of content in
		//     the System.Windows.Controls.RichTextBox.
		public TextPointer ContentStart { get; }
		
		//
		// Summary:
		//     Gets the contents of the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     A System.Windows.Documents.BlockCollection that contains the contents of the
		//     System.Windows.Controls.RichTextBox.
		public BlockCollection Blocks { get; }
		
		//
		// Summary:
		//     Returns a System.Windows.Documents.TextPointer that indicates the closest insertion
		//     position for the specified point.
		//
		// Parameters:
		//   point:
		//     A point in the coordinate space of the System.Windows.Controls.RichTextBox for
		//     which the closest insertion position is retrieved.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that indicates the closest insertion position
		//     for the specified point.
		public TextPointer GetPositionFromPoint(Point point)
		{
			return default(TextPointer);
		}
		
		//
		// Summary:
		//     Gets or sets a value that determines whether the user can change the text in
		//     the System.Windows.Controls.RichTextBox.
		//
		// Returns:
		//     true if the System.Windows.Controls.RichTextBox is read-only; otherwise, false.
		//     The default is false.
		public bool IsReadOnly { get; set; }
	}
}
#endif