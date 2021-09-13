#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Represents the method that handles the System.Windows.Controls.RichTextBox.ContentChanged
	//     event.
	//
	// Parameters:
	//   sender:
	//     The object where the event handler is attached.
	//
	//   e:
	//     The event data.
	public delegate void ContentChangedEventHandler(object sender, ContentChangedEventArgs e);
}
