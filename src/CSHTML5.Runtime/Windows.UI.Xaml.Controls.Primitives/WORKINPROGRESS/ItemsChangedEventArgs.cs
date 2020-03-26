

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


#if WORKINPROGRESS
using System;
using System.Collections.Specialized;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	//
	// Summary:
	//     Provides data for the System.Windows.Controls.ItemContainerGenerator.ItemsChanged
	//     event.
	public partial class ItemsChangedEventArgs : EventArgs
	{
		//
		// Summary:
		//     Gets the action that occurred on the items collection.
		//
		// Returns:
		//     Returns the action that occurred.
		public NotifyCollectionChangedAction Action { get; }
		//
		// Summary:
		//     Gets the number of items that were involved in the change.
		//
		// Returns:
		//     Integer that represents the number of items involved in the change.
		public int ItemCount { get; }
		//
		// Summary:
		//     Gets the number of UI elements involved in the change.
		//
		// Returns:
		//     Integer that represents the number of UI elements involved in the change.
		public int ItemUICount { get; }
		//
		// Summary:
		//     Gets the position in the collection before the change occurred.
		//
		// Returns:
		//     Returns a System.Windows.Controls.Primitives.GeneratorPosition.
		public GeneratorPosition OldPosition { get; }
		//
		// Summary:
		//     Gets the position in the collection where the change occurred.
		//
		// Returns:
		//     Returns a System.Windows.Controls.Primitives.GeneratorPosition.
		public GeneratorPosition Position { get; }
	}
}
#endif
