#if MIGRATION
using System.Windows.Controls;

namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	//
	// Summary:
	//     Encapsulates the selection state for the System.Windows.Controls.RichTextBox
	//     control.
	public sealed class TextSelection
	{
		private IRichTextBoxPresenter _presenter;
		private int _start, _length;

        internal TextSelection(IRichTextBoxPresenter presenter, int start, int length)
        {
			_presenter = presenter;
			_start = start;
			_length = length;
        }
		
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
		public string Text {
			get => _presenter.GetText(_start, _length);
			set => _presenter.SetText(_start, _length, value);
		}

		//
		// Summary:
		//     Gets or sets the XAML representation of the current selection.
		//
		// Returns:
		//     A System.String that is a XAML representation of the current selection. This
		//     XAML representation is the same XAML that is applied to the clipboard for a copy
		//     operation.
		public string Xaml 
		{
            get
            {
				return _presenter.GetContents(_start, _length);
            }
            set
            {
				//TODO: implement
            }
		}

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
		public void ApplyPropertyValue(DependencyProperty formattingProperty, object value)
		{
			_presenter.SetPropertyValue(formattingProperty, value, _start, _length);
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
		public object GetPropertyValue(DependencyProperty formattingProperty)
		{
			return _presenter.GetPropertyValue(formattingProperty, _start, _length);
		}
		
		//
		// Summary:
		//     Inserts or replaces the content at the current selection as a System.Windows.Documents.TextElement.
		//
		// Parameters:
		//   element:
		//     The System.Windows.Documents.TextElement to be inserted.
		public void Insert(TextElement element)
		{
			//TODO: support other TextElements
			if(element is Run run)
            {
				_presenter.SetText(_start, _length, run.Text);
            }
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
