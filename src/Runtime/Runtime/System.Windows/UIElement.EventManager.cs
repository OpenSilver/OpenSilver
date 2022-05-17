
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
        private NativeEventsManager _eventsManager;

        internal static void NativeEventCallback(UIElement mouseTarget, UIElement keyboardTarget, object jsEventArg)
        {
            string type = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0.type", jsEventArg));
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

                case "focusin":
                    keyboardTarget.ProcessOnFocusIn(jsEventArg);
                    break;

                case "focusout":
                    keyboardTarget.ProcessOnFocusOut(jsEventArg);
                    break;

                case "keypress":
                    keyboardTarget.ProcessOnInput(jsEventArg);
                    break;

                case "input":
                    keyboardTarget.ProcessOnTextUpdated(jsEventArg);
                    break;
            }
        }

        private void ProcessOnMouseMove(object jsEventArg)
        {
            string eventType = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0.type", jsEventArg));
            if (!(ignoreMouseEvents && eventType == "mousemove"))
            {
#if MIGRATION
                ProcessPointerEvent(jsEventArg, MouseMoveEvent);
#else
                ProcessPointerEvent(jsEventArg, PointerMovedEvent);
#endif
            }

            if (eventType == "touchend")
            {
                ignoreMouseEvents = true;
                if (_ignoreMouseEventsTimer == null)
                {
                    _ignoreMouseEventsTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };
                    _ignoreMouseEventsTimer.Tick += _ignoreMouseEventsTimer_Tick;
                }
                _ignoreMouseEventsTimer.Stop();
                _ignoreMouseEventsTimer.Start();
            }
        }

        private void ProcessOnMouseDown(object jsEventArg)
        {
            // 0: Main button pressed, usually the left button or the un - initialized state
            // 1: Auxiliary button pressed, usually the wheel button or the middle button(if present)
            // 2: Secondary button pressed, usually the right button
            // 3: Fourth button, typically the Browser Back button
            // 4: Fifth button, typically the Browser Forward button

            int button;
            int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.button", jsEventArg).ToString(), out button);

            switch (button)
            {
                case 0:
#if MIGRATION
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        MouseLeftButtonDownEvent,
                        preventTextSelectionWhenPointerIsCaptured: true,
                        refreshClickCount: true);
#else
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        PointerPressedEvent,
                        preventTextSelectionWhenPointerIsCaptured: true,
                        refreshClickCount: true);
#endif
                    break;

                case 2:
#if MIGRATION
                    ProcessMouseButtonEvent(
                        jsEventArg,
                        MouseRightButtonDownEvent,
                        preventTextSelectionWhenPointerIsCaptured: true,
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
            int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.button", jsEventArg).ToString(), out button);

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
        }

        private void ProcessMouseButtonEvent(
            object jsEventArg,
            RoutedEvent routedEvent,
            bool preventTextSelectionWhenPointerIsCaptured = false,
            bool refreshClickCount = false)
        {
            string eventType = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0.type", jsEventArg));
            if (!(ignoreMouseEvents && eventType.StartsWith("mouse"))) //Ignore mousedown and mouseup if the touch equivalents have been handled.
            {
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

                if (e.CheckIfEventShouldBeTreated(this, jsEventArg))
                {
                    // Fill the position of the pointer and the key modifiers:
                    e.FillEventArgs(this, jsEventArg);

                    // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                    RaiseEvent(e);
                }

                //Prevent text selection when the pointer is captured:
                if (preventTextSelectionWhenPointerIsCaptured && Pointer.INTERNAL_captured != null)
                {
                    OpenSilver.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
                }

                if (eventType == "touchend") //prepare to ignore the mouse events since they were already handled as touch events
                {
                    ignoreMouseEvents = true;
                    if (_ignoreMouseEventsTimer == null)
                    {
                        _ignoreMouseEventsTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };
                        _ignoreMouseEventsTimer.Tick += _ignoreMouseEventsTimer_Tick;
                    }
                    _ignoreMouseEventsTimer.Stop();
                    _ignoreMouseEventsTimer.Start();
                }
            }
        }

        // This boolean is useful because we want to ignore mouse events when touch events have happened
        // so the same user inputs are not handled twice. (Note: when using touch events, the browsers
        // fire the touch events at the moment of the action, then throw the mouse events once touchend
        // is fired)
        private static bool ignoreMouseEvents = false;
        private static DispatcherTimer _ignoreMouseEventsTimer = null;
        
        private void _ignoreMouseEventsTimer_Tick(object sender, object e)
        {
            ignoreMouseEvents = false;
            _ignoreMouseEventsTimer.Stop();
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
            
            //Prevent text selection when the pointer is captured:
            if (Pointer.INTERNAL_captured != null)
            {
                OpenSilver.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
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

        private void ProcessOnInput(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg).ToString(), out int keyCode))
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

            if (this is Controls.TextBox)
            {
                (this as Controls.TextBox).INTERNAL_CheckTextInputHandled(e, jsEventArg);
            }
        }

        // This callback will be triggered textinput is not handled
        private void ProcessOnTextUpdated(object jsEventArg)
        {
            if (this is Controls.TextBox)
            {
                (this as Controls.TextBox).INTERNAL_TextUpdated();
            }
        }

        private void ProcessOnKeyDown(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg).ToString(), out int keyCode))
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
                OpenSilver.Interop.ExecuteJavaScript("$0.preventDefault()", jsEventArg);
            }
        }

        private void ProcessOnKeyUp(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg).ToString(), out int keyCode))
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

        private void ProcessOnFocusIn(object jsEventArg)
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

        private void ProcessOnFocusOut(object jsEventArg)
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
