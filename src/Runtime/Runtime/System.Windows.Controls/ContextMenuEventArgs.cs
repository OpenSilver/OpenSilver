
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the context menu event.
    /// </summary>
    public sealed class ContextMenuEventArgs : RoutedEventArgs
    {
        public ContextMenuEventArgs(double pointerLeft, double pointerTop)
        {
#if MIGRATION
            CursorLeft = pointerLeft;
            CursorTop = pointerTop;
#else
            PointerLeft = pointerLeft;
            PointerTop = pointerTop;
#endif
        }

        /// <summary>
        /// Gets the horizontal position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorLeft { get; }
#else
        public double PointerLeft { get; }
#endif

        /// <summary>
        /// Gets the vertical position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorTop { get; }
#else
        public double PointerTop { get; }
#endif
    }
}
