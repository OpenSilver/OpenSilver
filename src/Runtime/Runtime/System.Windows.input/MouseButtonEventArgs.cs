

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// Provides event data for mouse button input events, for example System.Windows.UIElement.MouseLeftButtonDown
    /// and System.Windows.UIElement.MouseRightButtonUp.
    /// </summary>
    public partial class MouseButtonEventArgs : MouseEventArgs
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((MouseButtonEventHandler)handler)(target, this);
        }
    }
}
#endif