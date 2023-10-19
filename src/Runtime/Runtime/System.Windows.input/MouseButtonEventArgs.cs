
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

namespace System.Windows.Input
{
    /// <summary>
    /// Provides event data for mouse button input events, for example <see cref="UIElement.MouseLeftButtonDown"/>
    /// and <see cref="UIElement.MouseRightButtonUp"/>.
    /// </summary>
    public class MouseButtonEventArgs : MouseEventArgs
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((MouseButtonEventHandler)handler)(target, this);
        }
    }
}
