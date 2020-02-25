using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if WORKINPROGRESS
	//
	// Summary:
	//     Provides data for the System.Windows.UIElement.TextInput routed event.
	public sealed partial class TextCompositionEventArgs : RoutedEventArgs
	{
		//
		// Summary:
		//     Gets or sets a value that marks the routed event as handled, and prevents most
		//     handlers along the event route from handling the same event again.
		//
		// Returns:
		//     true to mark the routed event handled. false to leave the routed event unhandled,
		//     which permits the event to potentially route further and be acted on by other
		//     handlers. The default is false.
		public bool Handled { get; set; }
		//
		// Summary:
		//     Gets or sets the text string that of the text composition.
		//
		// Returns:
		//     The text string of the text composition.
		public string Text { get; }
		//
		// Summary:
		//     Gets or sets the text in the composition as a System.Windows.Input.TextComposition
		//     object.
		//
		// Returns:
		//     The text in the composition, as a System.Windows.Input.TextComposition object.
		public TextComposition TextComposition { get; }
	}
#endif
}
