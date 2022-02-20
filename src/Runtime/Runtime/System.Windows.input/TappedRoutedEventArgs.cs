

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


using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides event data for the Tapped event.
    /// </summary>
    public sealed partial class TappedRoutedEventArgs // Note: normally it does not inherit from "PointerRoutedEventArgs", but this lets us reuse code.
#if MIGRATION
        : MouseEventArgs
#else
        : PointerRoutedEventArgs
#endif
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((TappedEventHandler)handler)(target, this);
        }
    }
}
