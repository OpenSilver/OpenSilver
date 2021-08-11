

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
    public sealed partial class ContextMenuEventArgs : RoutedEventArgs
    {
        double _pointerLeft;
        double _pointerTop;

        public ContextMenuEventArgs(double pointerLeft, double pointerTop)
        {
            _pointerLeft = pointerLeft;
            _pointerTop = pointerTop;
        }

        /// <summary>
        /// Gets the horizontal position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorLeft
#else
        public double PointerLeft
#endif
        {
            get
            {
                return _pointerLeft;
            }
        }

        /// <summary>
        /// Gets the vertical position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorTop
#else
        public double PointerTop
#endif
        {
            get
            {
                return _pointerTop;
            }
        }

        //protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget);
    }
}
