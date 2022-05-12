
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
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
    internal class NativeEventsManager
    {
        private readonly Delegate _handler;
        private readonly UIElement _owner;
        private readonly bool _isFocusable;

        internal NativeEventsManager(UIElement uie, UIElement mouseEventTarget, UIElement keyboardEventTarget, bool isFocusable)
        {
            _owner = uie;
            _isFocusable = isFocusable;
            MouseTarget = mouseEventTarget;
            KeyboardTarget = keyboardEventTarget;

            if (Interop.IsRunningInTheSimulator)
            {
                _handler = new Action<object>(NativeEventCallback);
            }
            else
            {
                _handler = new Func<object, string>(jsEventArg =>
                {
                    NativeEventCallback(jsEventArg);
                    return string.Empty;
                });
            }
        }

        internal UIElement MouseTarget { get; }

        internal UIElement KeyboardTarget { get; }

        public void AttachEvents()
        {
            string UniqueIdentifier = (_owner.INTERNAL_OuterDomElement as CSHTML5.Internal.INTERNAL_HtmlDomElementReference).UniqueIdentifier;
            string sHandler = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_handler);
            Interop.ExecuteJavaScriptFastAsync($@"document._attachEventListeners(""{UniqueIdentifier}"", {sHandler}, {(_isFocusable ? "true" : "false")})");
        }

        public void DetachEvents()
        {
            string UniqueIdentifier = (_owner.INTERNAL_OuterDomElement as CSHTML5.Internal.INTERNAL_HtmlDomElementReference).UniqueIdentifier;
            Interop.ExecuteJavaScriptFastAsync($@"document._removeEventListeners(""{UniqueIdentifier}"")");
        }

        private void NativeEventCallback(object jsEventArg)
        {
            UIElement.NativeEventCallback(MouseTarget, KeyboardTarget, jsEventArg);
        }
    }
}
