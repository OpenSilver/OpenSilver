
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
        private const int ID_UNSUPPORTED = -1;
        private const int ID_MOUSEMOVE = 0;
        private const int ID_MOUSEDOWN = 1;
        private const int ID_MOUSEUP = 2;
        private const int ID_WHEEL = 3;
        private const int ID_MOUSEENTER = 4;
        private const int ID_MOUSELEAVE = 5;
        private const int ID_INPUT = 6;
        private const int ID_KEYDOWN = 7;
        private const int ID_KEYUP = 8;
        private const int ID_FOCUSIN = 9;
        private const int ID_FOCUSOUT = 10;

        private static readonly Dictionary<RoutedEvent, int> RoutedEventToEventManagerID;

        private static readonly Func<UIElement, DOMEventManager>[] EventManagerFactory =
            new Func<UIElement, DOMEventManager>[11]
            {
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[2] { "mousemove", "touchmove" }, uie.ProcessOnMouseMove, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[2] { "mousedown", "touchstart" }, uie.ProcessOnMouseDown, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[2] { "mouseup", "touchend" }, uie.ProcessOnMouseUp, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "wheel" }, uie.ProcessOnWheel, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "mouseenter" }, uie.ProcessOnMouseEnter, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "mouseleave" }, uie.ProcessOnMouseLeave, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "input" }, uie.ProcessOnInput, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "keydown" }, uie.ProcessOnKeyDown, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "keyup" }, uie.ProcessOnKeyUp, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "focusin" }, uie.ProcessOnFocusIn, true),
                uie => new DOMEventManager(() => uie.INTERNAL_OuterDomElement, new string[1] { "focusout" }, uie.ProcessOnFocusOut, true),
            };

        private DOMEventManager[] _eventManagersStore;

        private static int GetEventManagerID(RoutedEvent routedEvent)
        {
            if (RoutedEventToEventManagerID.TryGetValue(routedEvent, out int id))
            {
                return id;
            }

            return ID_UNSUPPORTED;
        }

        private void EnsureEventManagersStore()
        {
            if (_eventManagersStore == null)
            {
                _eventManagersStore = new DOMEventManager[11];
            }
        }

        private DOMEventManager GetOrCreateEventManager(int eventManagerID)
        {
            Debug.Assert(eventManagerID >= 0 && eventManagerID <= 10);

            EnsureEventManagersStore();

            DOMEventManager eventManager = _eventManagersStore[eventManagerID];
            if (eventManager == null)
            {
                _eventManagersStore[eventManagerID] = eventManager = EventManagerFactory[eventManagerID](this);
            }

            return eventManager;
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
            string inputText = OpenSilver.Interop.ExecuteJavaScript("$0.data", jsEventArg).ToString();
            if (inputText == null)
            {
                return;
            }

            TextCompositionEventArgs e = new TextCompositionEventArgs
            {
                RoutedEvent = TextInputEvent,
                OriginalSource = this,
                Text = inputText,
                TextComposition = new TextComposition(""),
                INTERNAL_OriginalJSEventArg = jsEventArg,
            };

            RaiseEvent(e);
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

        private bool ShouldHookUpMouseMoveEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(MouseMoveEvent, typeof(UIElement), nameof(OnMouseMove), new Type[1] { typeof(MouseEventArgs) });
#else
            return ShouldWireUpRoutedEvent(PointerMovedEvent, typeof(UIElement), nameof(OnPointerMoved), new Type[1] { typeof(PointerRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpMouseDownEvent()
        {
#if MIGRATION
            Type[] types = new Type[1] { typeof(MouseButtonEventArgs) };

            return ShouldWireUpRoutedEvent(MouseLeftButtonDownEvent, typeof(UIElement), nameof(OnMouseLeftButtonDown), types) ||
                   ShouldWireUpRoutedEvent(MouseRightButtonDownEvent, typeof(UIElement), nameof(OnMouseRightButtonDown), types);
#else
            return ShouldWireUpRoutedEvent(PointerPressedEvent, typeof(UIElement), nameof(OnPointerPressed), new Type[1] { typeof(PointerRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpMouseUpEvent()
        {
#if MIGRATION
            Type[] types = new Type[1] { typeof(MouseButtonEventArgs) };

            return ShouldWireUpRoutedEvent(MouseLeftButtonUpEvent, typeof(UIElement), nameof(OnMouseLeftButtonUp), types) ||
                   ShouldWireUpRoutedEvent(MouseRightButtonUpEvent, typeof(UIElement), nameof(OnMouseRightButtonUp), types) ||
                   ShouldWireUpRoutedEvent(TappedEvent, typeof(UIElement), nameof(OnTapped), new Type[1] { typeof(TappedRoutedEventArgs) });
#else
            return ShouldWireUpRoutedEvent(PointerReleasedEvent, typeof(UIElement), nameof(OnPointerReleased), new Type[1] { typeof(PointerRoutedEventArgs) }) ||
                   ShouldWireUpRoutedEvent(RightTappedEvent, typeof(UIElement), nameof(OnRightTapped), new Type[1] { typeof(RightTappedRoutedEventArgs) }) ||
                   ShouldWireUpRoutedEvent(TappedEvent, typeof(UIElement), nameof(OnTapped), new Type[1] { typeof(TappedRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpWheelEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(MouseWheelEvent, typeof(UIElement), nameof(OnMouseWheel), new Type[1] { typeof(MouseWheelEventArgs) });
#else
            return ShouldWireUpRoutedEvent(PointerWheelChangedEvent, typeof(UIElement), nameof(OnPointerWheelChanged), new Type[1] { typeof(PointerRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpMouseEnterEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(MouseEnterEvent, typeof(UIElement), nameof(OnMouseEnter), new Type[1] { typeof(MouseEventArgs) });
#else
            return ShouldWireUpRoutedEvent(PointerEnteredEvent, typeof(UIElement), nameof(OnPointerEntered), new Type[1] { typeof(PointerRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpMouseLeaveEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(MouseLeaveEvent, typeof(UIElement), nameof(OnMouseLeave), new Type[1] { typeof(MouseEventArgs) });
#else
            return ShouldWireUpRoutedEvent(PointerExitedEvent, typeof(UIElement), nameof(OnPointerExited), new Type[1] { typeof(PointerRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpInputEvent()
        {
            return ShouldWireUpRoutedEvent(TextInputEvent, typeof(UIElement), nameof(OnTextInput), new Type[1] { typeof(TextCompositionEventArgs) });
        }

        private bool ShouldHookUpKeyDownEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(KeyDownEvent, typeof(UIElement), nameof(OnKeyDown), new Type[1] { typeof(KeyEventArgs) });
#else
            return ShouldWireUpRoutedEvent(KeyDownEvent, typeof(UIElement), nameof(OnKeyDown), new Type[1] { typeof(KeyRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpKeyUpEvent()
        {
#if MIGRATION
            return ShouldWireUpRoutedEvent(KeyUpEvent, typeof(UIElement), nameof(OnKeyUp), new Type[1] { typeof(KeyEventArgs) });
#else
            return ShouldWireUpRoutedEvent(KeyUpEvent, typeof(UIElement), nameof(OnKeyUp), new Type[1] { typeof(KeyRoutedEventArgs) });
#endif
        }

        private bool ShouldHookUpFocusInEvent()
        {
            return ShouldWireUpRoutedEvent(GotFocusEvent, typeof(UIElement), nameof(OnGotFocus), new Type[1] { typeof(RoutedEventArgs) });
        }

        private bool ShouldHookUpFocusOutEvent()
        {
            return ShouldWireUpRoutedEvent(LostFocusEvent, typeof(UIElement), nameof(OnLostFocus), new Type[1] { typeof(RoutedEventArgs) });
        }

        private void HookUpMouseMoveEvent()
        {
            HookUpDOMEvent(ID_MOUSEMOVE);
        }

        private void HookUpMouseDownEvent()
        {
            HookUpDOMEvent(ID_MOUSEDOWN);
        }

        private void HookUpMouseUpEvent()
        {
            HookUpDOMEvent(ID_MOUSEUP);
        }

        private void HookUpWheelEvent()
        {
            HookUpDOMEvent(ID_WHEEL);
        }

        private void HookUpMouseEnterEvent()
        {
            HookUpDOMEvent(ID_MOUSEENTER);
        }

        private void HookUpMouseLeaveEvent()
        {
            HookUpDOMEvent(ID_MOUSELEAVE);
        }

        private void HookUpInputEvent()
        {
            HookUpDOMEvent(ID_INPUT);
        }

        private void HookUpKeyDownEvent()
        {
            HookUpDOMEvent(ID_KEYDOWN);
        }

        private void HookUpKeyUpEvent()
        {
            HookUpDOMEvent(ID_KEYUP);
        }

        private void HookUpFocusInEvent()
        {
            HookUpDOMEvent(ID_FOCUSIN);
        }

        private void HookUpFocusOutEvent()
        {
            HookUpDOMEvent(ID_FOCUSOUT);
        }

        private void HookUpDOMEvent(int id)
        {
            GetOrCreateEventManager(id).AttachToDomEvents();
        }

        private bool ShouldWireUpRoutedEvent(RoutedEvent routedEvent, Type ownerType, string methodName, Type[] methodParameters)
        {
            if (_eventHandlersStore != null)
            {
                List<RoutedEventHandlerInfo> handlers = _eventHandlersStore[routedEvent];
                if (handlers != null && handlers.Count > 0)
                {
                    return true;
                }
            }

            return INTERNAL_EventsHelper.IsEventCallbackOverridden(this, ownerType, methodName, methodParameters);
        }

        private void WireUpRoutedEvent(RoutedEvent routedEvent)
        {
            int id = GetEventManagerID(routedEvent);

            if (id != ID_UNSUPPORTED)
            {
                HookUpDOMEvent(id);

                if (id == ID_MOUSEENTER)
                {
                    HookUpDOMEvent(ID_MOUSELEAVE);
                }
                else if (id == ID_MOUSELEAVE)
                {
                    HookUpDOMEvent(ID_MOUSEENTER);
                }
            }
        }

        private void UnHookDOMEvents()
        {
            if (_eventManagersStore == null)
            {
                return;
            }

            for (int i = 0; i < _eventManagersStore.Length; i++)
            {
                DOMEventManager manager = _eventManagersStore[i];
                if (manager != null)
                {
                    manager.DetachFromDomEvents();
                }
            }
        }

        private void UnHookDOMEvent(int id)
        {
            if (_eventManagersStore == null)
            {
                return;
            }

            DOMEventManager manager = _eventManagersStore[id];
            if (manager != null)
            {
                manager.DetachFromDomEvents();
            }
        }
    }
}
