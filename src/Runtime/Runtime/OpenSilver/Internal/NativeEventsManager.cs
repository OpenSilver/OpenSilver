
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
            Interop.ExecuteJavaScriptAsync("document._attachEventListeners($0, $1, $2)", _owner.INTERNAL_OuterDomElement, _handler, _isFocusable);
        }

        public void DetachEvents()
        {
            Interop.ExecuteJavaScriptAsync("document._removeEventListeners($0)", _owner.INTERNAL_OuterDomElement);
        }

        private void NativeEventCallback(object jsEventArg)
        {
            UIElement.NativeEventCallback(MouseTarget, KeyboardTarget, jsEventArg);
        }
    }
}
