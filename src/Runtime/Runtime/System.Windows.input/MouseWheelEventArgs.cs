
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
    public class MouseWheelEventArgs : MouseEventArgs
    {
        internal override void InvokeHandler(Delegate handler, object target)
        {
            ((MouseWheelEventHandler)handler)(target, this);
        }

        public new int Delta { get; internal set; }

        internal static int GetPointerWheelDelta(object jsEventArg)
        {
            string sJsArg = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
            return OpenSilver.Interop.ExecuteJavaScriptInt32(
                $"(function (wheelArg) {{ if (wheelArg.wheelDelta != undefined) return wheelArg.wheelDelta; else return (wheelArg.delta || 0); }})({sJsArg})");

            // Note: wheelDelta originated from the js mousewheel event which is deprecated. It still exists in the js wheel event but should still be considered deprecated (it also has never existed on FireFox).
            // We're using it because I could not find another way of getting the values in the same way as was returned in Silverlight.
            // What happens is as follows:
            // Scroll on the physical device --> A certain scroll value --> value translated in the impact value (depends on settings - see below) --> impact value registered in the js eventArgs.
            //                            SL returns this ↑, wheelDelta (deprecated) as well                                                              delta returns this ↑
            // If we change the mouse settings in Windows so the mouse wheel scrolls more or fewer lines, wheelDelta WILL NOT change, delta WILL change.
        }
    }
}
