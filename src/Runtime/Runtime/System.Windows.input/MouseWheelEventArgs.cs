
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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if MIGRATION
    public partial class MouseWheelEventArgs : MouseEventArgs
#else
    public partial class PointerPointProperties
#endif
    {
#if MIGRATION
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((MouseWheelEventHandler)handler)(target, this);
        }
#endif

#if MIGRATION
        public new int Delta { get; internal set; }
#endif

        internal static int GetPointerWheelDelta(object jsEventArg)
        {
            int rawDelta = Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.delta || 0", jsEventArg)); // we use || 0 just in case delta is undefined.
            if (Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0.wheelDelta != undefined", jsEventArg)))
            {
                rawDelta = Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.wheelDelta", jsEventArg));
                // Note: wheelDelta originated from the js mousewheel event which is deprecated. It still exists in the js wheel event but should still be considered deprecated (it also has never existed on FireFox).
                // We're using it because I could not find another way of getting the values in the same way as was returned in Silverlight.
                // What happens is as follows:
                // Scroll on the physical device --> A certain scroll value --> value translated in the impact value (depends on settings - see below) --> impact value registered in the js eventArgs.
                //                            SL returns this ↑, wheelDelta (deprecated) as well                                                              delta returns this ↑
                // If we change the mouse settings in Windows so the mouse wheel scrolls more or fewer lines, wheelDelta WILL NOT change, delta WILL change.
            }

            return rawDelta;
        }
    }
}
