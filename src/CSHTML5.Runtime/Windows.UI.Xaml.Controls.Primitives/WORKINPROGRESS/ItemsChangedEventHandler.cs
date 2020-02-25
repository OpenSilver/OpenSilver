#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	//
	// Summary:
	//     Represents the method that will handle the System.Windows.Controls.ItemContainerGenerator.ItemsChanged
	//     event.
	//
	// Parameters:
	//   sender:
	//     The source of the event.
	//
	//   e:
	//     The event data.
	public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);
}
#endif