
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
    internal class NativeEventsManager : IDisposable
    {
        private bool _disposed;
        private JavascriptCallback _handler;
        private UIElement _owner;
        private readonly bool _isFocusable;

        internal NativeEventsManager(UIElement uie, UIElement mouseEventTarget, UIElement keyboardEventTarget, bool isFocusable)
        {
            _owner = uie;
            _isFocusable = isFocusable;
            MouseTarget = mouseEventTarget;
            KeyboardTarget = keyboardEventTarget;

            if (Interop.IsRunningInTheSimulator)
            {
                _handler = JavascriptCallback.Create(new Action<object>(NativeEventCallback));
            }
            else
            {
                _handler = JavascriptCallback.Create(new Func<object, string>(jsEventArg =>
                {
                    NativeEventCallback(jsEventArg);
                    return string.Empty;
                }));
            }
        }

        internal UIElement MouseTarget { get; private set; }

        internal UIElement KeyboardTarget { get; private set; }

        public void AttachEvents()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("NativeEventsManager");
            }

            string sHandler = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_handler);
            if (_owner.INTERNAL_OuterDomElement is INTERNAL_HtmlDomElementReference domRef)
            {
                Interop.ExecuteJavaScriptAsync($@"document._attachEventListeners(""{domRef.UniqueIdentifier}"", {sHandler}, {(_isFocusable ? "true" : "false")})");
            }
            else
            {
                string sOuter = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_owner.INTERNAL_OuterDomElement);
                Interop.ExecuteJavaScriptAsync($"document._attachEventListeners({sOuter}, {sHandler}, {(_isFocusable ? "true" : "false")})");
            }
        }

        private void DetachEvents()
        {
            if (_owner.INTERNAL_OuterDomElement is INTERNAL_HtmlDomElementReference domRef)
            {
                Interop.ExecuteJavaScriptAsync($@"document._removeEventListeners(""{domRef.UniqueIdentifier}"")");
            }
            else
            {
                string sOuter = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_owner.INTERNAL_OuterDomElement);
                Interop.ExecuteJavaScriptAsync($"document._removeEventListeners({sOuter})");
            }
        }

        public void Dispose()
        {
            _disposed = true;
            DetachEvents();
            _handler.Dispose();
            _handler = null;
            _owner = null;
            MouseTarget = null;
            KeyboardTarget = null;
        }

        private void NativeEventCallback(object jsEventArg)
        {
            UIElement.NativeEventCallback(MouseTarget, KeyboardTarget, jsEventArg);
        }
    }
}
