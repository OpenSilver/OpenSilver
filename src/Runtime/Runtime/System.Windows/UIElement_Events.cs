
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

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.System;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    partial class UIElement
    {
        private EventHandlersStore _eventHandlersStore;

        private void EnsureEventHandlersStore()
        {
            if (_eventHandlersStore == null)
            {
                _eventHandlersStore = new EventHandlersStore();
            }
        }

        /// <summary>
        /// Adds a routed event handler for a specified routed event, adding the handler
        /// to the handler collection on the current element. Specify handledEventsToo as
        /// true to have the provided handler be invoked for routed event that had already
        /// been marked as handled by another element along the event route.
        /// </summary>
        /// <param name="routedEvent">
        /// An identifier for the routed event to be handled.
        /// </param>
        /// <param name="handler">
        /// A reference to the handler implementation.
        /// </param>
        /// <param name="handledEventsToo">
        /// true to register the handler such that it is invoked even when the routed event
        /// is marked handled in its event data; false to register the handler with the default
        /// condition that it will not be invoked if the routed event is already marked handled.
        /// The default is false. Do not routinely ask to rehandle a routed event. For more
        /// information, see Remarks.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// routedEvent or handler is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// routedEvent does not represent a supported routed event.-or-handler does not
        /// implement a supported delegate.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Attempted to add handler for an event not supported by the current platform variation.
        /// </exception>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException("Handler type is mismatched.");
            }

            EnsureEventHandlersStore();
            _eventHandlersStore.AddRoutedEventHandler(routedEvent, handler, handledEventsToo);

            HookUpRoutedEvent(routedEvent);
        }

        /// <summary>
        /// Removes the specified routed event handler from this <see cref="UIElement"/>.
        /// </summary>
        /// <param name="routedEvent">
        /// The identifier of the routed event for which the handler is attached.
        /// </param>
        /// <param name="handler">
        /// The specific handler implementation to remove from the event handler collection
        /// on this <see cref="UIElement"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// routedEvent or handler is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// routedEvent does not represent a supported routed event.-or-handler does not
        /// implement a supported delegate.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Attempted to remove handler for an event not supported by the current platform
        /// variation.
        /// </exception>
        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException("Handler type is mismatched.");
            }

            EventHandlersStore store = _eventHandlersStore;
            if (store != null)
            {
                store.RemoveRoutedEventHandler(routedEvent, handler);
            }
        }

        private RoutedEventHandlerInfo[] GetRoutedEventHandlers(RoutedEvent key)
        {
            if (_eventHandlersStore != null)
            {
                List<RoutedEventHandlerInfo> handlers = _eventHandlersStore[key];
                if (handlers != null)
                {
                    return handlers.ToArray();
                }
            }

#if BRIDGE
            return EmptyArray<RoutedEventHandlerInfo>.Value;
#else
            return Array.Empty<RoutedEventHandlerInfo>();
#endif
        }

        private void InvokeRoutedEventHandlers(RoutedEvent routedEvent, RoutedEventArgs eventArgs)
        {
            RoutedEventHandlerInfo[] handlers = GetRoutedEventHandlers(routedEvent);
            for (int i = 0; i < handlers.Length; i++)
            {
                RoutedEventHandlerInfo handler = handlers[i];
                handler.InvokeHandler(this, eventArgs);
            }
        }

        static bool ignoreMouseEvents = false; // This boolean is useful because we want to ignore mouse events when touch events have happened so the same user inputs are not handled twice. (Note: when using touch events, the browsers fire the touch events at the moment of the action, then throw the mouse events once touchend is fired)
        private static DispatcherTimer _ignoreMouseEventsTimer = null;
        private void _ignoreMouseEventsTimer_Tick(object sender, object e)
        {
            ignoreMouseEvents = false;
            _ignoreMouseEventsTimer.Stop();
        }

        #region Pointer moved event

#if MIGRATION
        public static readonly RoutedEvent MouseMoveEvent;
#else
        public static readonly RoutedEvent PointerMovedEvent;
#endif

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// moved, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseMove
        {
            add
            {
                AddHandler(MouseMoveEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseMoveEvent, value);
            }
        }
#else
        public event PointerEventHandler PointerMoved
        {
            add
            {
                AddHandler(PointerMovedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerMovedEvent, value);
            }
        }
#endif


        /// <summary>
        /// Raises the PointerMoved event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>

#if MIGRATION
        protected virtual void OnMouseMove(MouseEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseMoveEvent, eventArgs);
        }
#else
        protected virtual void OnPointerMoved(PointerRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(PointerMovedEvent, eventArgs);
        }
#endif


        #endregion

        #region Pointer pressed event

#if MIGRATION
        public static readonly RoutedEvent MouseLeftButtonDownEvent;
#else
        public static readonly RoutedEvent PointerPressedEvent;
#endif

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseLeftButtonDown
        {
            add
            {
                AddHandler(MouseLeftButtonDownEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseLeftButtonDownEvent, value);
            }
        }
#else
        public event PointerEventHandler PointerPressed
        {
            add
            {
                AddHandler(PointerPressedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerPressedEvent, value);
            }
        }
#endif


        /// <summary>
        /// Raises the PointerPressed event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseLeftButtonDownEvent, eventArgs);
        }
#else
        protected virtual void OnPointerPressed(PointerRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(PointerPressedEvent, eventArgs);
        }
#endif


        #endregion

        #region MouseRightButtonDown (no equivalent in UWP)

#if MIGRATION

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonDown"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent MouseRightButtonDownEvent;

        public event MouseButtonEventHandler MouseRightButtonDown
        {
            add
            {
                AddHandler(MouseRightButtonDownEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseRightButtonDownEvent, value);
            }
        }

        /// <summary>
        /// Raises the MouseRightButtonDown event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseRightButtonDownEvent, eventArgs);
        }

#endif

        #endregion

        #region PointerWheelChanged event (or MouseWheel)

#if MIGRATION
        /// <summary>
        /// Identifies the <see cref="UIElement.MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent;
#else
        /// <summary>
        /// Identifies the <see cref="UIElement.PointerWheelChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PointerWheelChangedEvent;
#endif

#if MIGRATION
        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event MouseWheelEventHandler MouseWheel
        {
            add
            {
                AddHandler(MouseWheelEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseWheelEvent, value);
            }
        }
#else
        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event PointerEventHandler PointerWheelChanged
        {
            add
            {
                AddHandler(PointerWheelChangedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerWheelChangedEvent, value);
            }
        }
#endif


        /// <summary>
        /// Raises the PointerWheelChanged event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseWheel(MouseWheelEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseWheelEvent, eventArgs);
        }
#else
        protected virtual void OnPointerWheelChanged(PointerRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(PointerWheelChangedEvent, eventArgs);
        }
#endif


#if MIGRATION
        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        void ProcessOnPointerWheelChangedEvent(object jsEventArg)
        {
            var eventArgs = new MouseWheelEventArgs()
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                //fill the Mouse Wheel delta:
                eventArgs.Delta = MouseWheelEventArgs.GetPointerWheelDelta(jsEventArg);

                OnMouseWheel(eventArgs);

                if (eventArgs.Handled)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                }
            }
        }
#endif

        #endregion


        #region Pointer released event

#if MIGRATION
        public static readonly RoutedEvent MouseLeftButtonUpEvent;
#else
        public static readonly RoutedEvent PointerReleasedEvent;
#endif

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// released, while within this element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseLeftButtonUp
        {
            add
            {
                AddHandler(MouseLeftButtonUpEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseLeftButtonUpEvent, value);
            }
        }
#else
        public event PointerEventHandler PointerReleased
        {
            add
            {
                AddHandler(PointerReleasedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerReleasedEvent, value);
            }
        }
#endif

        /// <summary>
        /// Raises the PointerReleased event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>

#if MIGRATION
        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseLeftButtonUpEvent, eventArgs);
        }
#else
        protected virtual void OnPointerReleased(PointerRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(PointerReleasedEvent, eventArgs);
        }
#endif

        #endregion


        #region Pointer entered event

#if MIGRATION
        public static readonly RoutedEvent MouseEnterEvent;
#else
        public static readonly RoutedEvent PointerEnteredEvent;
#endif

        /// <summary>
        /// Occurs when a pointer enters the hit test area of this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseEnter
        {
            add
            {
                AddHandler(MouseEnterEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseEnterEvent, value);
            }
        }
#else
        public event PointerEventHandler PointerEntered
        {
            add
            {
                AddHandler(PointerEnteredEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerEnteredEvent, value);
            }
        }
#endif


        /// <summary>
        /// Creates the eventArgs from the infos in the javascript's version of the event then raises the PointerEntered event
        /// </summary>
#if MIGRATION
        private void ProcessOnMouseEnter(object jsEventArg)
        {
            ProcessPointerEvent(jsEventArg, OnMouseEnter, preventTextSelectionWhenPointerIsCaptured: true);
        }
#else
        private void ProcessOnPointerEntered(object jsEventArg)
        {
            ProcessPointerEvent(jsEventArg, OnPointerEntered, preventTextSelectionWhenPointerIsCaptured: true);
        }
#endif

        /// <summary>
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseEnter(MouseEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseEnterEvent, eventArgs);
        }
#else
        protected virtual void OnPointerEntered(PointerRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(PointerEnteredEvent, eventArgs);
        }
#endif

        #endregion


        #region Pointer exited event

#if MIGRATION
        public static readonly RoutedEvent MouseLeaveEvent;
#else
        public static readonly RoutedEvent PointerExitedEvent;
#endif

        /// <summary>
        /// Occurs when a pointer leaves the hit test area of this element.
        /// </summary>
#if MIGRATION
        public event MouseEventHandler MouseLeave
        {
            add
            {
                StartManagingPointerPositionForPointerExitedEvent();
                AddHandler(MouseLeaveEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseLeaveEvent, value);
                if (_eventHandlersStore != null)
                {
                    List<RoutedEventHandlerInfo> handlers = _eventHandlersStore[MouseLeaveEvent];
                    if (handlers == null || handlers.Count == 0)
                    {
                        StopManagingPointerPositionForPointerExitedEvent();
                    }
                }
            }
        }
#else
        public event PointerEventHandler PointerExited
        {
            add
            {
                StartManagingPointerPositionForPointerExitedEvent();
                AddHandler(PointerExitedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerExitedEvent, value);
                if (_eventHandlersStore != null)
                {
                    List<RoutedEventHandlerInfo> handlers = _eventHandlersStore[PointerExitedEvent];
                    if (handlers == null || handlers.Count == 0)
                    {
                        StopManagingPointerPositionForPointerExitedEvent();
                    }
                }
            }
        }
#endif

        internal bool isAlreadySubscribedToMouseEnterAndLeave = false;
        /// <summary>
        /// IMPORTANT NOTE: This boolean is only updated in the case the PointerExited/MouseLeave event is used on this element.
        /// It states whether the pointer is currently inside the UIElement.
        /// </summary>
        internal bool INTERNAL_isPointerInside = false;
        void SetIsPointerInsideToTrue() { this.INTERNAL_isPointerInside = true; }
        void StartManagingPointerPositionForPointerExitedEvent()
        {
            if (!isAlreadySubscribedToMouseEnterAndLeave && this.INTERNAL_OuterDomElement != null)
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.addEventListener(""mouseenter"", $1, false);", this.INTERNAL_OuterDomElement, (Action)SetIsPointerInsideToTrue);

                isAlreadySubscribedToMouseEnterAndLeave = true;
            }
        }
        void StopManagingPointerPositionForPointerExitedEvent()
        {
            if (isAlreadySubscribedToMouseEnterAndLeave && this.INTERNAL_OuterDomElement != null)
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.removeEventListener(""mouseenter"", $1);", this.INTERNAL_OuterDomElement, (Action)SetIsPointerInsideToTrue);

                INTERNAL_isPointerInside = false; //don't know if this is useful but just in case.
                isAlreadySubscribedToMouseEnterAndLeave = false;
            }
        }


        internal void StartManagingPointerPositionForPointerExitedEventIfNeeded()
        {
#if MIGRATION
            if (_domEventManagersStore != null && _domEventManagersStore.ContainsKey(MouseLeaveEvent))
#else
            if (_domEventManagersStore != null && _domEventManagersStore.ContainsKey(PointerExitedEvent))
#endif
            {
                StartManagingPointerPositionForPointerExitedEvent();
            }
        }

        /// <summary>
        /// Creates the eventArgs from the infos in the javascript's version of the event then raises the PointerExited event
        /// </summary>
#if MIGRATION
        internal void ProcessOnMouseLeave(object jsEventArg)
        {
            ProcessPointerEvent(jsEventArg, OnMouseLeave, preventTextSelectionWhenPointerIsCaptured: true);
        }
#else
        internal void ProcessOnPointerExited(object jsEventArg)
        {
            ProcessPointerEvent(jsEventArg, OnPointerExited, preventTextSelectionWhenPointerIsCaptured: true);
        }
#endif

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected internal virtual void OnMouseLeave(MouseEventArgs eventArgs)
        {
            INTERNAL_isPointerInside = false;
            InvokeRoutedEventHandlers(MouseLeaveEvent, eventArgs);
        }
#else
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs eventArgs)
        {
            INTERNAL_isPointerInside = false;
            InvokeRoutedEventHandlers(PointerExitedEvent, eventArgs);
        }
#endif

        #endregion


        #region Text events

        /// <summary>
        /// Identifies the <see cref="TextInput"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputEvent;

        /// <summary>
        /// Raises the TextInput event
        /// </summary>
        void ProcessOnTextInput(object jsEventArg)
        {
            var inputText = OpenSilver.Interop.ExecuteJavaScript("$0.data", jsEventArg).ToString();
            if (inputText == null)
                return;

#if MIGRATION
            var eventArgs = new TextCompositionEventArgs()
#else
            var eventArgs = new TextCompositionEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Text = inputText,
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled"),
                TextComposition = new TextComposition("")
            };

            OnTextInput(eventArgs);

            if (eventArgs.Handled)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                OpenSilver.Interop.ExecuteJavaScript("$0.preventDefault()", jsEventArg);
            }
        }

        /// <summary>
        /// Raises the TextInput event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTextInput(TextCompositionEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(TextInputEvent, eventArgs);
        }

        /// <summary>
        /// Identifies the <see cref="TextInputStart"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputStartEvent;

        /// <summary>
        /// Identifies the <see cref="TextInputUpdate"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputUpdateEvent;

        /// <summary>
        /// Occurs when a UI element gets text in a device-independent manner.
        /// </summary>
        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event TextCompositionEventHandler TextInput
        {
            add
            {
                AddHandler(TextInputEvent, value, false);
            }
            remove
            {
                RemoveHandler(TextInputEvent, value);
            }
        }

        /// <summary>
        /// Occurs when a UI element initially gets text in a device-independent manner.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInputStart;

        /// <summary>
        /// Occurs when text continues to be composed via an input method editor (IME).
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInputUpdate;

        #endregion


        #region Tapped event

        public static readonly RoutedEvent TappedEvent;

        /// <summary>
        /// Occurs when an otherwise unhandled Tap interaction occurs over the hit test
        /// area of this element.
        /// </summary>
        public event TappedEventHandler Tapped
        {
            add
            {
                AddHandler(TappedEvent, value, false);
            }
            remove
            {
                RemoveHandler(TappedEvent, value);
            }
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        void ProcessOnTapped(object jsEventArg)
        {
            var eventArgs = new TappedRoutedEventArgs()
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                OnTapped(eventArgs);

                if (eventArgs.Handled)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                }
            }
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTapped(TappedRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(TappedEvent, eventArgs);
        }

        #endregion


        #region RightTapped (aka MouseRightButtonUp) event

#if MIGRATION
        public static readonly RoutedEvent MouseRightButtonUpEvent;
#else
        public static readonly RoutedEvent RightTappedEvent;
#endif

        /// <summary>
        /// Occurs when a right-tap input stimulus happens while the pointer is over
        /// the element.
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler MouseRightButtonUp
        {
            add
            {
                AddHandler(MouseRightButtonUpEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseRightButtonUpEvent, value);
            }
        }
#else
        public event RightTappedEventHandler RightTapped
        {
            add
            {
                AddHandler(RightTappedEvent, value, false);
            }
            remove
            {
                RemoveHandler(RightTappedEvent, value);
            }
        }
#endif

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
#if MIGRATION
        void ProcessOnMouseRightButtonUp(object jsEventArg)
#else
        void ProcessOnRightTapped(object jsEventArg)
#endif
        {
#if MIGRATION
            var eventArgs = new MouseButtonEventArgs()
#else
            var eventArgs = new RightTappedRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
            };

            if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
            {
                // Fill the position of the pointer and the key modifiers:
                eventArgs.FillEventArgs(this, jsEventArg);

                // Prevent the default behavior (which is to show the browser context menu):
                OpenSilver.Interop.ExecuteJavaScript(@"
                    if ($0.preventDefault)
                        $0.preventDefault();", jsEventArg);

#if MIGRATION
                OnMouseRightButtonUp(eventArgs);
#else
                OnRightTapped(eventArgs);
#endif

                if (eventArgs.Handled)
                {
                    OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                }
            }
        }

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(MouseRightButtonUpEvent, eventArgs);
        }
#else
        protected virtual void OnRightTapped(RightTappedRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(RightTappedEvent, eventArgs);
        }
#endif

        #endregion

        #region KeyDown event

        public static readonly RoutedEvent KeyDownEvent;

        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyDown
        {
            add
            {
                AddHandler(KeyDownEvent, value, false);
            }
            remove
            {
                RemoveHandler(KeyDownEvent, value);
            }
        }

        /// <summary>
        /// Raises the OnKeyDown event
        /// </summary>
        void ProcessOnKeyDown(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg).ToString(), out int keyCode))
                return;

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            var eventArgs = new KeyEventArgs()
#else
            var eventArgs = new KeyRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
            };

            // Add the key modifier to the eventArgs:
            eventArgs.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            OnKeyDown(eventArgs);

            if (eventArgs.Handled)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                OpenSilver.Interop.ExecuteJavaScript("$0.preventDefault()", jsEventArg);
            }
        }

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyDown(KeyEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(KeyDownEvent, eventArgs);
        }
#else
        protected virtual void OnKeyDown(KeyRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(KeyDownEvent, eventArgs);
        }
#endif

        #endregion


        #region KeyUp event

        public static readonly RoutedEvent KeyUpEvent;

        /// <summary>
        /// Occurs when a keyboard key is released while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyUp
        {
            add
            {
                AddHandler(KeyUpEvent, value, false);
            }
            remove
            {
                RemoveHandler(KeyUpEvent, value);
            }
        }

        /// <summary>
        /// Raises the OnKeyUp event
        /// </summary>
        void ProcessOnKeyUp(object jsEventArg)
        {
            if (!int.TryParse(OpenSilver.Interop.ExecuteJavaScript("$0.keyCode", jsEventArg).ToString(), out int keyCode))
                return;

#if MIGRATION
            keyCode = INTERNAL_VirtualKeysHelpers.FixKeyCodeForSilverlight(keyCode);
            var eventArgs = new KeyEventArgs()
#else
            var eventArgs = new KeyRoutedEventArgs()
#endif
            {
                INTERNAL_OriginalJSEventArg = jsEventArg,
                PlatformKeyCode = keyCode,
                Key = INTERNAL_VirtualKeysHelpers.GetKeyFromKeyCode(keyCode),
                Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
            };

            // Add the key modifier to the eventArgs:
            eventArgs.AddKeyModifiersAndUpdateDocumentValue(jsEventArg);

            OnKeyUp(eventArgs);

            if (eventArgs.Handled)
            {
                OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
            }
        }

        /// <summary>
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyUp(KeyEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(KeyUpEvent, eventArgs);
        }
#else
        protected virtual void OnKeyUp(KeyRoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(KeyUpEvent, eventArgs);
        }
#endif

        #endregion

        #region GotFocus event

        public static readonly RoutedEvent GotFocusEvent;

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// Note that ONLY sender's informations are currently filled (not pointer's)
        /// </summary>
        public event RoutedEventHandler GotFocus //todo: fill everything and remove the note above
        {
            add
            {
                AddHandler(GotFocusEvent, value, false);
            }
            remove
            {
                RemoveHandler(GotFocusEvent, value);
            }
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        void ProcessOnGotFocus(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };

            FocusManager.SetFocusedElement(this.INTERNAL_ParentWindow, this);

            OnGotFocus(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnGotFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnGotFocus(RoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(GotFocusEvent, eventArgs);
        }

        #endregion

        #region Lostfocus event

        public static readonly RoutedEvent LostFocusEvent;

        /// <summary>
        /// Occurs when a UIElement loses focus.
        /// </summary>
        public event RoutedEventHandler LostFocus
        {
            add
            {
                AddHandler(LostFocusEvent, value, false);
            }
            remove
            {
                RemoveHandler(LostFocusEvent, value);
            }
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        void ProcessOnLostFocus(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };

            OnLostFocus(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnLostFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnLostFocus(RoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(LostFocusEvent, eventArgs);
        }

        #endregion


        #region Handle keyboard events when focused

        /// <summary>
        /// Set this boolean to true for the UIElements that have to react to keyboard events when they are focused.
        /// It activates the mechanic used to handle those events.
        /// It is used with the method OnKeyDownWhenFocused which is overriden and does the specific handling of the events.
        /// </summary>
        internal bool _reactsToKeyboardEventsWhenFocused = false;

        void StartListeningToKeyboardEvents(object sender, RoutedEventArgs e)
        {
            //the following test is for cases such as focusing a TextBox that is inside a Button for example.
            if (!Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", e.INTERNAL_OriginalJSEventArg)))
            {
                this.KeyDown -= OnKeyDownWhenFocused; //just in case but we shouldn't need it (if we need it here, it means that the keyboard events kept getting taken into consideration even though it didn't have the focus).
                this.KeyDown += OnKeyDownWhenFocused;
            }
        }

        void StopListeningToKeyboardEvents(object sender, RoutedEventArgs e)
        {
            this.KeyDown -= OnKeyDownWhenFocused; //just in case but we shouldn't need it (if we need it here, it means that the button kept getting triggered by keyboard events even though it didn't have the focus).
        }

        /// <summary>
        /// This method is to be overriden by UIElements that react to keyboard events when they are focused, to handle those events properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if MIGRATION
        internal virtual void OnKeyDownWhenFocused(object sender, KeyEventArgs e)
#else
        internal virtual void OnKeyDownWhenFocused(object sender, KeyRoutedEventArgs e)
#endif
        {
            //if(e.Key == ...)
            //{
            //    Do something...
            //}
        }

        #endregion


        #region Allow or Prevent Focus events for IsTabStop

        //todo-perf: use the code that is in the "first try at this" region rather than creating a full-fledged event and find out why it doesn't work (going from IsTabStop = false to IsTabStop = true doesn't succeed to remove the listener that calls blur on the dom element and it is thus for the element to get the focus despite having IsTabStop to true).

        #region GotFocusForIsTabStop event

        private static readonly RoutedEvent GotFocusForIsTabStopEvent;

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// Note that ONLY sender's informations are currently filled (not pointer's)
        /// </summary>
        private event RoutedEventHandler GotFocusForIsTabStop
        {
            add
            {
                AddHandler(GotFocusForIsTabStopEvent, value, false);
            }
            remove
            {
                RemoveHandler(GotFocusForIsTabStopEvent, value);
            }
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        void ProcessOnGotFocusForIsTabStop(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                INTERNAL_OriginalJSEventArg = jsEventArg
            };
            OnGotFocusForIsTabStop(eventArgs); //todo: should we skip this method if "handled" is true? (test by overriding "OnGotFocus" method below and see how it works in this case in WPF)
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        private void OnGotFocusForIsTabStop(RoutedEventArgs eventArgs)
        {
            InvokeRoutedEventHandlers(GotFocusForIsTabStopEvent, eventArgs);
        }

        #endregion

        private bool INTERNAL_AreFocusEventsAllowed = true;

        internal void PreventFocusEvents()
        {
            if (INTERNAL_AreFocusEventsAllowed)
            {
                INTERNAL_AreFocusEventsAllowed = false;

                INTERNAL_DetachFromFocusEvents();

                GotFocusForIsTabStop -= UIElement_GotFocusForIsTabStop; //just in case.
                GotFocusForIsTabStop += UIElement_GotFocusForIsTabStop;
            }
        }

        internal virtual object GetFocusTarget() => INTERNAL_OuterDomElement;

        void UIElement_GotFocusForIsTabStop(object sender, RoutedEventArgs e)
        {
            UIElement element = (UIElement)sender; //jsEvent should be called "sender" but I kept the former implementation so I also kept the name.
            var elementToBlur = element.GetFocusTarget();
            if (elementToBlur != null)
                OpenSilver.Interop.ExecuteJavaScript(@"$0.blur()", elementToBlur);
        }

        internal void AllowFocusEvents()
        {
            if (!INTERNAL_AreFocusEventsAllowed)
            {
                INTERNAL_AreFocusEventsAllowed = true;

                INTERNAL_AttachToFocusEvents();
                GotFocusForIsTabStop -= UIElement_GotFocusForIsTabStop; //just in case.
            }
        }

        /// <summary>
        /// DO NOT USE THIS, IT IS ONLY FOR THE ALLOWFOCUSEVENTS METHOD
        /// </summary>
        private void INTERNAL_AttachToFocusEvents()
        {
            Type[] methodParameters = { typeof(RoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(GotFocusEvent, typeof(UIElement), nameof(OnGotFocus), methodParameters))
            {
                HookUpRoutedEvent(GotFocusEvent);
            }

            if (ShouldHookUpRoutedEvent(LostFocusEvent, typeof(UIElement), nameof(OnLostFocus), methodParameters))
            {
                HookUpRoutedEvent(LostFocusEvent);
            }
        }

        /// <summary>
        /// DO NOT USE THIS, IT IS ONLY FOR THE PREVENTFOCUSEVENTS METHOD
        /// </summary>
        private void INTERNAL_DetachFromFocusEvents()
        {
            UnHookRoutedEvent(GotFocusEvent);
            UnHookRoutedEvent(LostFocusEvent);
        }

        #endregion

        void ProcessMouseButtonEvent(
            object jsEventArg,
#if MIGRATION
            Action<MouseButtonEventArgs> onEvent,
#else
            Action<PointerRoutedEventArgs> onEvent,
#endif
            bool preventTextSelectionWhenPointerIsCaptured = false,
            bool checkForDivsThatAbsorbEvents = false,  //Note: this is currently true only for PointerPressed and PointerReleased
                                                        //because those are the events we previously attached ourselves to for TextBox
                                                        //so that it would set the event to handled to prevent the click in a TextBox (to change the text) located
                                                        //in a button or any other control that reacts to clicks from also triggering the click from that control
            bool refreshClickCount = false)
        {
            bool isMouseEvent = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0.type.startsWith('mouse')", jsEventArg));
            if (!(ignoreMouseEvents && isMouseEvent)) //Ignore mousedown, mousemove and mouseup if the touch equivalents have been handled.
            {
#if MIGRATION
                var eventArgs = new MouseButtonEventArgs()
#else
                var eventArgs = new PointerRoutedEventArgs()
#endif
                {
                    INTERNAL_OriginalJSEventArg = jsEventArg,
                    Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
                };
                if (!eventArgs.Handled && checkForDivsThatAbsorbEvents)
                {
                    eventArgs.Handled = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", jsEventArg));
                }

                if (refreshClickCount)
                {
                    eventArgs.RefreshClickCount(this);
                }

                if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
                {
                    // Fill the position of the pointer and the key modifiers:
                    eventArgs.FillEventArgs(this, jsEventArg);

                    // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                    onEvent(eventArgs);

                    if (eventArgs.Handled)
                    {
                        OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                    }
                }

                //Prevent text selection when the pointer is captured:
                if (preventTextSelectionWhenPointerIsCaptured && Pointer.INTERNAL_captured != null)
                {
                    OpenSilver.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
                }
                bool isTouchEndEvent = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0.type == 'touchend'", jsEventArg));
                if (isTouchEndEvent) //prepare to ignore the mouse events since they were already handled as touch events
                {
                    ignoreMouseEvents = true;
                    if (_ignoreMouseEventsTimer == null)
                    {
                        _ignoreMouseEventsTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) }; //I arbitrarily picked 100ms because at 30ms with throttling x6, it didn't work every time (but sometimes did so it should be alright, also, I tested with 100ms and it worked everytime)
                        _ignoreMouseEventsTimer.Tick += _ignoreMouseEventsTimer_Tick;

                    }
                    _ignoreMouseEventsTimer.Stop();
                    _ignoreMouseEventsTimer.Start();
                }
            }
        }

        void ProcessPointerEvent(
            object jsEventArg,
#if MIGRATION
            Action<MouseEventArgs> onEvent,
#else
            Action<PointerRoutedEventArgs> onEvent,
#endif
            bool preventTextSelectionWhenPointerIsCaptured = false,
            bool checkForDivsThatAbsorbEvents = false,  //Note: this is currently true only for PointerPressed and PointerReleased
                                                        //because those are the events we previously attached ourselves to for TextBox
                                                        //so that it would set the event to handled to prevent the click in a TextBox (to change the text) located
                                                        //in a button or any other control that reacts to clicks from also triggering the click from that control
            bool refreshClickCount = false)
        {
            bool isMouseEvent = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0.type.startsWith('mouse')", jsEventArg));
            if (!(ignoreMouseEvents && isMouseEvent)) //Ignore mousedown, mousemove and mouseup if the touch equivalents have been handled.
            {
#if MIGRATION
                var eventArgs = new MouseEventArgs()
#else
                var eventArgs = new PointerRoutedEventArgs()
#endif
                {
                    INTERNAL_OriginalJSEventArg = jsEventArg,
                    Handled = ((OpenSilver.Interop.ExecuteJavaScript("$0.handled", jsEventArg) ?? "").ToString() == "handled")
                };
                if (!eventArgs.Handled && checkForDivsThatAbsorbEvents)
                {
                    eventArgs.Handled = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", jsEventArg));
                }

                if (refreshClickCount)
                {
                    eventArgs.RefreshClickCount(this);
                }

                if (eventArgs.CheckIfEventShouldBeTreated(this, jsEventArg))
                {
                    // Fill the position of the pointer and the key modifiers:
                    eventArgs.FillEventArgs(this, jsEventArg);

                    // Raise the event (if it was not already marked as "handled" by a child element in the visual tree):
                    onEvent(eventArgs);

                    if (eventArgs.Handled)
                    {
                        OpenSilver.Interop.ExecuteJavaScript("$0.handled = 'handled'", jsEventArg);
                    }
                }

                //Prevent text selection when the pointer is captured:
                if (preventTextSelectionWhenPointerIsCaptured && Pointer.INTERNAL_captured != null)
                {
                    OpenSilver.Interop.ExecuteJavaScript(@"window.getSelection().removeAllRanges()");
                }
                bool isTouchEndEvent = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("$0.type == 'touchend'", jsEventArg));
                if (isTouchEndEvent) //prepare to ignore the mouse events since they were already handled as touch events
                {
                    ignoreMouseEvents = true;
                    if (_ignoreMouseEventsTimer == null)
                    {
                        _ignoreMouseEventsTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) }; //I arbitrarily picked 100ms because at 30ms with throttling x6, it didn't work every time (but sometimes did so it should be alright, also, I tested with 100ms and it worked everytime)
                        _ignoreMouseEventsTimer.Tick += _ignoreMouseEventsTimer_Tick;

                    }
                    _ignoreMouseEventsTimer.Stop();
                    _ignoreMouseEventsTimer.Start();
                }
            }
        }

        public virtual void INTERNAL_AttachToDomEvents()
        {
            if (_reactsToKeyboardEventsWhenFocused)
            {
                GotFocus -= StartListeningToKeyboardEvents;
                GotFocus += StartListeningToKeyboardEvents;
                LostFocus -= StopListeningToKeyboardEvents;
                LostFocus += StopListeningToKeyboardEvents;
            }

#if MIGRATION
            Type[] methodParameters = new Type[] { typeof(MouseEventArgs) };

            if (ShouldHookUpRoutedEvent(MouseMoveEvent, typeof(UIElement), nameof(OnMouseMove), methodParameters))
            {
                HookUpRoutedEvent(MouseMoveEvent);
            }

            if (ShouldHookUpRoutedEvent(MouseEnterEvent, typeof(UIElement), nameof(OnMouseEnter), methodParameters))
            {
                HookUpRoutedEvent(MouseEnterEvent);
            }

            if (ShouldHookUpRoutedEvent(MouseLeaveEvent, typeof(UIElement), nameof(OnMouseLeave), methodParameters))
            {
                HookUpRoutedEvent(MouseLeaveEvent);
            }

            methodParameters = new Type[] { typeof(TappedRoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(TappedEvent, typeof(UIElement), nameof(OnTapped), methodParameters))
            {
                HookUpRoutedEvent(TappedEvent);
            }

            methodParameters = new Type[] { typeof(MouseButtonEventArgs) };

            if (ShouldHookUpRoutedEvent(MouseLeftButtonDownEvent, typeof(UIElement), nameof(OnMouseLeftButtonDown), methodParameters))
            {
                HookUpRoutedEvent(MouseLeftButtonDownEvent);
            }

            if (ShouldHookUpRoutedEvent(MouseLeftButtonUpEvent, typeof(UIElement), nameof(OnMouseLeftButtonUp), methodParameters))
            {
                HookUpRoutedEvent(MouseLeftButtonUpEvent);
            }

            if (ShouldHookUpRoutedEvent(MouseRightButtonDownEvent, typeof(UIElement), nameof(OnMouseRightButtonDown), methodParameters))
            {
                HookUpRoutedEvent(MouseRightButtonDownEvent);
            }

            if (ShouldHookUpRoutedEvent(MouseRightButtonUpEvent, typeof(UIElement), nameof(OnMouseRightButtonUp), methodParameters))
            {
                HookUpRoutedEvent(MouseRightButtonUpEvent);
            }

            methodParameters = new Type[] { typeof(MouseWheelEventArgs) };

            if (ShouldHookUpRoutedEvent(MouseWheelEvent, typeof(UIElement), nameof(OnMouseWheel), methodParameters))
            {
                HookUpRoutedEvent(MouseWheelEvent);
            }

            methodParameters = new Type[] { typeof(KeyEventArgs) };

            if (ShouldHookUpRoutedEvent(KeyDownEvent, typeof(UIElement), nameof(OnKeyDown), methodParameters))
            {
                HookUpRoutedEvent(KeyDownEvent);
            }

            if (ShouldHookUpRoutedEvent(KeyUpEvent, typeof(UIElement), nameof(OnKeyUp), methodParameters))
            {
                HookUpRoutedEvent(KeyUpEvent);
            }

            methodParameters = new Type[] { typeof(RoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(GotFocusEvent, typeof(UIElement), nameof(OnGotFocus), methodParameters))
            {
                HookUpRoutedEvent(GotFocusEvent);
            }

            if (ShouldHookUpRoutedEvent(LostFocusEvent, typeof(UIElement), nameof(OnLostFocus), methodParameters))
            {
                HookUpRoutedEvent(LostFocusEvent);
            }

            methodParameters = new Type[] { typeof(TextCompositionEventArgs) };

            if (ShouldHookUpRoutedEvent(TextInputEvent, typeof(UIElement), nameof(OnTextInput), methodParameters))
            {
                HookUpRoutedEvent(TextInputEvent);
            }
#else
            Type[] methodParameters = new Type[] { typeof(PointerRoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(PointerMovedEvent, typeof(UIElement), nameof(OnPointerMoved), methodParameters))
            {
                HookUpRoutedEvent(PointerMovedEvent);
            }

            if (ShouldHookUpRoutedEvent(PointerPressedEvent, typeof(UIElement), nameof(OnPointerPressed), methodParameters))
            {
                HookUpRoutedEvent(PointerPressedEvent);
            }

            if (ShouldHookUpRoutedEvent(PointerReleasedEvent, typeof(UIElement), nameof(OnPointerReleased), methodParameters))
            {
                HookUpRoutedEvent(PointerReleasedEvent);
            }

            if (ShouldHookUpRoutedEvent(PointerEnteredEvent, typeof(UIElement), nameof(OnPointerEntered), methodParameters))
            {
                HookUpRoutedEvent(PointerEnteredEvent);
            }

            if (ShouldHookUpRoutedEvent(PointerExitedEvent, typeof(UIElement), nameof(OnPointerExited), methodParameters))
            {
                HookUpRoutedEvent(PointerExitedEvent);
            }

            if (ShouldHookUpRoutedEvent(PointerWheelChangedEvent, typeof(UIElement), nameof(OnPointerWheelChanged), methodParameters))
            {
                HookUpRoutedEvent(PointerWheelChangedEvent);
            }
            
            methodParameters = new Type[] { typeof(TappedRoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(TappedEvent, typeof(UIElement), nameof(OnTapped), methodParameters))
            {
                HookUpRoutedEvent(TappedEvent);
            }

            methodParameters = new Type[] { typeof(RightTappedRoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(RightTappedEvent, typeof(UIElement), nameof(OnRightTapped), methodParameters))
            {
                HookUpRoutedEvent(RightTappedEvent);
            }

            methodParameters = new Type[] { typeof(KeyRoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(KeyDownEvent, typeof(UIElement), nameof(OnKeyDown), methodParameters))
            {
                HookUpRoutedEvent(KeyDownEvent);
            }

            if (ShouldHookUpRoutedEvent(KeyUpEvent, typeof(UIElement), nameof(OnKeyUp), methodParameters))
            {
                HookUpRoutedEvent(KeyUpEvent);
            }

            methodParameters = new Type[] { typeof(RoutedEventArgs) };

            if (ShouldHookUpRoutedEvent(GotFocusEvent, typeof(UIElement), nameof(OnGotFocus), methodParameters))
            {
                HookUpRoutedEvent(GotFocusEvent);
            }

            if (ShouldHookUpRoutedEvent(LostFocusEvent, typeof(UIElement), nameof(OnLostFocus), methodParameters))
            {
                HookUpRoutedEvent(LostFocusEvent);
            }

            methodParameters = new Type[] { typeof(TextCompositionEventArgs) };

            if (ShouldHookUpRoutedEvent(TextInputEvent, typeof(UIElement), nameof(OnTextInput), methodParameters))
            {
                HookUpRoutedEvent(TextInputEvent);
            }
#endif
        }

        public virtual void INTERNAL_DetachFromDomEvents()
        {
#if MIGRATION
            UnHookRoutedEvent(MouseMoveEvent);
            UnHookRoutedEvent(MouseLeftButtonDownEvent);
            UnHookRoutedEvent(MouseLeftButtonUpEvent);
            UnHookRoutedEvent(MouseEnterEvent);
            UnHookRoutedEvent(MouseLeaveEvent);
            UnHookRoutedEvent(MouseWheelEvent);
            UnHookRoutedEvent(TappedEvent);
            UnHookRoutedEvent(MouseRightButtonUpEvent);
            UnHookRoutedEvent(KeyDownEvent);
            UnHookRoutedEvent(KeyUpEvent);
            UnHookRoutedEvent(GotFocusEvent);
            UnHookRoutedEvent(LostFocusEvent);
            UnHookRoutedEvent(TextInputEvent);
#else
            UnHookRoutedEvent(PointerMovedEvent);
            UnHookRoutedEvent(PointerPressedEvent);
            UnHookRoutedEvent(PointerReleasedEvent);
            UnHookRoutedEvent(PointerEnteredEvent);
            UnHookRoutedEvent(PointerExitedEvent);
            UnHookRoutedEvent(PointerWheelChangedEvent);
            UnHookRoutedEvent(TappedEvent);
            UnHookRoutedEvent(RightTappedEvent);
            UnHookRoutedEvent(KeyDownEvent);
            UnHookRoutedEvent(KeyUpEvent);
            UnHookRoutedEvent(GotFocusEvent);
            UnHookRoutedEvent(LostFocusEvent);
            UnHookRoutedEvent(TextInputEvent);
#endif
        }
    }
}