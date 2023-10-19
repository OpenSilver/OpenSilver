
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the context menu event.
    /// </summary>
    public sealed class ContextMenuEventArgs : RoutedEventArgs
    {
        public ContextMenuEventArgs(double pointerLeft, double pointerTop)
        {
            CursorLeft = pointerLeft;
            CursorTop = pointerTop;
        }

        /// <summary>
        /// Gets the horizontal position of the mouse.
        /// </summary>
        public double CursorLeft { get; }

        /// <summary>
        /// Gets the vertical position of the mouse.
        /// </summary>
        public double CursorTop { get; }
    }
}
