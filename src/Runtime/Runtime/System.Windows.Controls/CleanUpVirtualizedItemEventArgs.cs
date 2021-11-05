﻿

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


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public class CleanUpVirtualizedItemEventArgs : RoutedEventArgs
	{
		internal CleanUpVirtualizedItemEventArgs(UIElement element, object value)
		{
			UIElement = element;
			Value = value;
		}

		public bool Cancel { get; set; }
		public UIElement UIElement { get; private set; }
		public Object Value { get; private set; }
	}
}