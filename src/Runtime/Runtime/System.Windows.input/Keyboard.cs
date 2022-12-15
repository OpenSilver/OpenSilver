
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

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

namespace System.Windows.Input
{
    /// <summary>
    /// Represents the keyboard device.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Gets the set of <see cref="ModifierKeys"/> that are currently pressed.
        /// </summary>
        public static ModifierKeys Modifiers
            => (ModifierKeys)OpenSilver.Interop.ExecuteJavaScriptInt32("document.modifiersPressed", false);

        internal static bool IsFocusable(Control control)
            => control.IsConnectedToLiveTree && control.IsVisible && control.IsEnabled && control.IsTabStop;
    }
}
