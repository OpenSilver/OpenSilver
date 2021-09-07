

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


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
	//
	// Summary:
	//     Provides data for the System.Windows.UIElement.TextInput routed event.
    [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		public bool Handled { get; set; }
		//
		// Summary:
		//     Gets or sets the text string that of the text composition.
		//
		// Returns:
		//     The text string of the text composition.
        [OpenSilver.NotImplemented]
		public string Text { get; }
		//
		// Summary:
		//     Gets or sets the text in the composition as a System.Windows.Input.TextComposition
		//     object.
		//
		// Returns:
		//     The text in the composition, as a System.Windows.Input.TextComposition object.
        [OpenSilver.NotImplemented]
		public TextComposition TextComposition { get; }
	}
}
