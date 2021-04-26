#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	//
	// Summary:
	//     Encapsulates the selection state for the System.Windows.Controls.RichTextBox
	//     control.
    [OpenSilver.NotImplemented]
	public sealed class TextSelection
	{
		//
		// Summary:
		//     Gets a System.Windows.Documents.TextPointer that represents the end of the current
		//     selection.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that represents the end of the current
		//     selection.
        [OpenSilver.NotImplemented]
		public TextPointer End { get; }
		//
		// Summary:
		//     Gets a System.Windows.Documents.TextPointer that represents the beginning of
		//     the current selection.
		//
		// Returns:
		//     A System.Windows.Documents.TextPointer that represents the beginning of the current
		//     selection.
        [OpenSilver.NotImplemented]
		public TextPointer Start { get; }
		//
		// Summary:
		//     Gets or sets the plain text contents of the current selection.
		//
		// Returns:
		//     A string that contains the plain text contents of the current selection.
        [OpenSilver.NotImplemented]
		public string Text { get; set; }
		//
		// Summary:
		//     Gets or sets the XAML representation of the current selection.
		//
		// Returns:
		//     A System.String that is a XAML representation of the current selection. This
		//     XAML representation is the same XAML that is applied to the clipboard for a copy
		//     operation.
        [OpenSilver.NotImplemented]
		public string Xaml { get; set; }

		//
		// Summary:
		//     Applies the specified formatting property and value to the current selection.
		//
		// Parameters:
		//   formattingProperty:
		//     A formatting property to apply.
		//
		//   value:
		//     The value for the formatting property.
        [OpenSilver.NotImplemented]
		public void ApplyPropertyValue(DependencyProperty formattingProperty, object value)
		{
			
		}
		
		//
		// Summary:
		//     Gets the value of the specified formatting property on the current selection.
		//
		// Parameters:
		//   formattingProperty:
		//     A formatting property to get the value of on the current selection.
		//
		// Returns:
		//     An object that indicates the value of the specified formatting property on the
		//     current selection.
        [OpenSilver.NotImplemented]
		public object GetPropertyValue(DependencyProperty formattingProperty)
		{
			return default(object);
		}
		
		//
		// Summary:
		//     Inserts or replaces the content at the current selection as a System.Windows.Documents.TextElement.
		//
		// Parameters:
		//   element:
		//     The System.Windows.Documents.TextElement to be inserted.
        [OpenSilver.NotImplemented]
		public void Insert(TextElement element)
		{
			
		}
		
		//
		// Summary:
		//     Updates the current selection, taking two System.Windows.Documents.TextPointer
		//     positions to indicate the updated selection.
		//
		// Parameters:
		//   anchorPosition:
		//     A fixed anchor position that marks one end of the updated selection.
		//
		//   movingPosition:
		//     A movable position that marks the other end of the updated selection.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     Position specifies a position from a different System.Windows.Controls.RichTextBox
		//     associated with the current position.
        [OpenSilver.NotImplemented]
		public void Select(TextPointer anchorPosition, TextPointer movingPosition)
		{
			
		}
	}
}

#endif