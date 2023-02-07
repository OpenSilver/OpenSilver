
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
using System.Collections.Generic;
using System.Diagnostics;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Input;
using System.Windows.Threading;
#else
using Windows.System;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class UIElement
    {
        internal static void NativeEventCallback(UIElement mouseTarget, UIElement keyboardTarget, object jsEventArg)
        {
            string type = OpenSilver.Interop.ExecuteJavaScriptString($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.type", false);
            switch (type)
            {
                case "mousemove":
                case "touchmove":
                    mouseTarget.ProcessOnMouseMove(jsEventArg);
                    break;

                case "mousedown":
                case "touchstart":
                    mouseTarget.ProcessOnMouseDown(jsEventArg);
                    break;

                case "mouseup":
                case "touchend":
                    mouseTarget.ProcessOnMouseUp(jsEventArg);
                    break;

                case "wheel":
                    mouseTarget.ProcessOnWheel(jsEventArg);
                    break;

                case "mouseenter":
                    mouseTarget.ProcessOnMouseEnter(jsEventArg);
                    break;

                case "mouseleave":
                    mouseTarget.ProcessOnMouseLeave(jsEventArg);
                    break;

                case "keydown":
                    keyboardTarget.ProcessOnKeyDown(jsEventArg);
                    break;

                case "keyup":
                    keyboardTarget.ProcessOnKeyUp(jsEventArg);
                    break;

                case "focus":
                    keyboardTarget.ProcessOnFocus(jsEventArg);
                    break;

                case "blur":
                    keyboardTarget.ProcessOnBlur(jsEventArg);
                    break;

                case "keypress":
                    keyboardTarget.ProcessOnKeyPress(jsEventArg);
                    break;

                case "input":
                    keyboardTarget.ProcessOnInput(jsEventArg);
                    break;
            }
        }

        private void ProcessOnMouseMove(object jsEventArg)
        {
#if MIGRATION
            ProcessPointerEvent(jsEventArg, MouseMoveEvent);
#else
            ProcessPointerEvent(jsEventArg, PointerMovedEvent);
#endif
        }

        private void ProcessOnMouseDown(object jsEventArg)
        {
            // 0: Main button pressed, usually the left button or the un - initialized state
            // 1: Auxiliary button pressed, usually the wheel button or the middle button(if present)
            // 2: Secondary button pressed, usually the right button
            // 3: Fourth button, typically the Browser Back button
            // 4: Fifth button, typically the Browser Forward button

            int button;
            int.TryParse(OpenSilver.Interop.ExecuteJavaScript($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.button").ToString(), out button);

            switch (button)
            {
                case 0:
#if MIGRATION
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        MouseLeftButtonDownEvent,
                        refreshClickCount: true);
#else
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        PointerPressedEvent,
                        refreshClickCount: true);
#endif
                    break;

                case 2:
#if MIGRATION
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        MouseRightButtonDownEvent,
                        refreshClickCount: true);
#endif
                    break;
            }
        }

        private void ProcessOnMouseUp(object jsEventArg)
        {
            // 0: Main button pressed, usually the left button or the un - initialized state
            // 1: Auxiliary button pressed, usually the wheel button or the middle button(if present)
            // 2: Secondary button pressed, usually the right button
            // 3: Fourth button, typically the Browser Back button
            // 4: Fifth button, typically the Browser Forward button

            int button;
            int.TryParse(OpenSilver.Interop.ExecuteJavaScript($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.button").ToString(), out button);

            switch (button)
            {
                case 0:
#if MIGRATION
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        MouseLeftButtonUpEvent);
#else
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        PointerReleasedEvent);
#endif
                    ProcessOnTapped(jsEventArg);
                    break;

                case 2:
                    ProcessOnMouseRightButtonUp(jsEventArg);
                    break;
            }

            Dispatcher.BeginInvoke(() =>
            {
                if (Pointer.INTERNAL_captured != null)
                {
#if MIGRATION
                    Pointer.INTERNAL_captured.ReleaseMouseCapture();
#else
                    Pointer.INTERNAL_captured.ReleasePointerCapture();
#endif
                }
            });
        }

        private void ProcessMouseButtonEvent(
            object jsEventArg,
            RoutedEvent routedEvent,
            bool refreshClickCount = false)
        {
            string eventVariable = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg);
            string eventType = OpenSilver.Interop.ExecuteJavaScriptString($"{eventVariable}.type", false);

#if MIGRATION
            MouseButtonEventArgs e = new MouseButtonEventArgs()
#else
            PointerRoutedEventArgs e = new PointerRoutedEventArgs()
#endif
            {
                RoutedEvent = routedEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };

            if (refreshClickCount)
            {
                e.RefreshClickCount(this);
            }

            // touchend event occurs only once as opposed to mouseup, so current UIElement is not the same as captured by touchstart event
            if (eventType == "touchend" || e.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                e.FillEventArgs(this, jsEventArg);

                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                RaiseEvent(e);
            }
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        private void ProcessOnTapped(object jsEventArg)
        {
            TappedRoutedEventArgs e = new TappedRoutedEventArgs
            {
                RoutedEvent = TappedEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };

            if (e.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                e.FillEventArgs(this, jsEventArg);

                RaiseEvent(e);
            }
        }

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
        private void ProcessOnMouseRightButtonUp(object jsEventArg)
        {
#if MIGRATION
            MouseButtonEventArgs e = new MouseButtonEventArgs()
            {
                RoutedEvent = MouseRightButtonUpEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };
#else
            RightTappedRoutedEventArgs e = new RightTappedRoutedEventArgs()
            {
                RoutedEvent = RightTappedEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };
#endif

            if (e.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                e.FillEventArgs(this, jsEventArg);

                RaiseEvent(e);
            }
        }

        private void ProcessOnWheel(object jsEventArg)
        {
#if MIGRATION
            MouseWheelEventArgs e = new MouseWheelEventArgs
            {
                RoutedEvent = MouseWheelEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };
#else
            PointerRoutedEventArgs e = new PointerRoutedEventArgs
            {
                RoutedEvent = PointerWheelChangedEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };
#endif

            if (e.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                e.FillEventArgs(this, jsEventArg);

#if MIGRATION
                // fill the Mouse Wheel delta:
                e.Delta = MouseWheelEventArgs.GetPointerWheelDelta(jsEventArg);
#endif

                RaiseEvent(e);
            }
        }

        internal bool IsPointerOver { get; private set; }

        private void ProcessOnMouseEnter(object jsEventArg)
        {
            IsPointerOver = true;

#if MIGRATION
            ProcessPointerEvent(jsEventArg, MouseEnterEvent);
#else
            ProcessPointerEvent(jsEventArg, PointerEnteredEvent);
#endif
        }

        private void ProcessPointerEvent(object jsEventArg, RoutedEvent routedEvent)
        {
#if MIGRATION
            MouseEventArgs e = new MouseEventArgs()
#else
            PointerRoutedEventArgs e = new PointerRoutedEventArgs()
#endif
            {
                RoutedEvent = routedEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };
            
            if (e.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                e.FillEventArgs(this, jsEventArg);
            
                // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                RaiseEvent(e);
            }
        }

        internal void RaiseMouseLeave()
        {
            Debug.Assert(IsPointerOver == true);
            IsPointerOver = false;

#if MIGRATION
            MouseEventArgs e = new MouseEventArgs
            {
                RoutedEvent = MouseLeaveEvent,
                OriginalSource = this,
            };
#else
            PointerRoutedEventArgs e = new PointerRoutedEventArgs
            {
                RoutedEvent = PointerExitedEvent,
                OriginalSource = this,
            };
#endif

            RaiseEvent(e);
        }

        private void ProcessOnMouseLeave(object jsEventArg)
        {
            IsPointerOver = false;

#if MIGRATION
            ProcessPointerEvent(jsEventArg, MouseLeaveEvent);
#else
            ProcessPointerEvent(jsEventArg, PointerExitedEvent);
#endif
        }

        private void ProcessOnInput(object jsEventArg) => OnTextInputInternal();

        internal virtual void OnTextInputInternal() { }

        private void ProcessOnKeyPress(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode").ToString(), out int keyCode))
            {
                return;
            }

            string inputText = ((char)keyCode).ToString();

            TextCompositionEventArgs e = new TextCompositionEventArgs
            {
                RoutedEvent = TextInputEvent,
                OriginalSource = this,
                Text = inputText,
                TextComposition = new TextComposition(""),
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };

            RaiseEvent(e);

            if (e.PreventDefault)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.preventDefault()");
            }
        }

        private void ProcessOnKeyDown(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode").ToString(), out int keyCode))
            {
                return;
            }

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            KeyEventArgs e = new KeyEventArgs()
#else
            KeyRoutedEventArgs e = new KeyRoutedEventArgs()
#endif
            {
                RoutedEvent = KeyDownEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            };

            // Add the key modifier to the eventArgs:
            e.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            RaiseEvent(e);

            if (e.Handled)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.preventDefault()");
            }
        }

        private void ProcessOnKeyUp(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript($"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArg)}.keyCode").ToString(), out int keyCode))
            {
                return;
            }

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            KeyEventArgs e = new KeyEventArgs()
#else
            KeyRoutedEventArgs e = new KeyRoutedEventArgs()
#endif
            {
                RoutedEvent = KeyUpEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
            };

            // Add the key modifier to the eventArgs:
            e.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            RaiseEvent(e);
        }

        private void ProcessOnFocus(object jsEventArg)
        {
            ProcessOnGotFocus(jsEventArg);
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        private void ProcessOnGotFocus(object jsEventArg)
        {
            RoutedEventArgs e = new RoutedEventArgs
            {
                RoutedEvent = GotFocusEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg
            };

            FocusManager.SetFocusedElement(this.INTERNAL_ParentWindow, this);

            RaiseEvent(e);
        }

        private void ProcessOnBlur(object jsEventArg)
        {
            ProcessOnLostFocus(jsEventArg);
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        private void ProcessOnLostFocus(object jsEventArg)
        {
            RoutedEventArgs e = new RoutedEventArgs
            {
                RoutedEvent = LostFocusEvent,
                OriginalSource = this,
                INTERNAL_OriginalJSEventArg = jsEventArg
            };

            RaiseEvent(e);
        }
    }
}
