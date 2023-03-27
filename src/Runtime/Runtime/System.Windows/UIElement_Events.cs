
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

#if MIGRATION
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    partial class UIElement
    {
        internal const int MAX_ELEMENTS_IN_ROUTE = 4096;

        private EventHandlersStore _eventHandlersStore;

        private void EnsureEventHandlersStore()
        {
            if (_eventHandlersStore == null)
            {
                _eventHandlersStore = new EventHandlersStore();
            }
        }

#if MIGRATION
        private static void RegisterEvents(Type type)
        {
            RoutedEvent.RegisterClassHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMoveThunk));
            RoutedEvent.RegisterClassHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownThunk));
            RoutedEvent.RegisterClassHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(OnMouseRightButtonDownThunk));
            RoutedEvent.RegisterClassHandler(MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheelThunk));
            RoutedEvent.RegisterClassHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpThunk));
            RoutedEvent.RegisterClassHandler(MouseEnterEvent, new MouseEventHandler(OnMouseEnterThunk));
            RoutedEvent.RegisterClassHandler(MouseLeaveEvent, new MouseEventHandler(OnMouseLeaveThunk));
            RoutedEvent.RegisterClassHandler(TextInputEvent, new TextCompositionEventHandler(OnTextInputThunk));
            RoutedEvent.RegisterClassHandler(TappedEvent, new TappedEventHandler(OnTappedThunk));
            RoutedEvent.RegisterClassHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler(OnMouseRightButtonUpThunk));
            RoutedEvent.RegisterClassHandler(KeyDownEvent, new KeyEventHandler(OnKeyDownThunk));
            RoutedEvent.RegisterClassHandler(KeyUpEvent, new KeyEventHandler(OnKeyUpThunk));
            RoutedEvent.RegisterClassHandler(GotFocusEvent, new RoutedEventHandler(OnGotFocusThunk));
            RoutedEvent.RegisterClassHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocusThunk));
        }

        private static void OnMouseMoveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseMove(e);

        private static void OnMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonDown(e);

        private static void OnMouseRightButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonDown(e);

        private static void OnMouseWheelThunk(object sender, MouseWheelEventArgs e) => ((UIElement)sender).OnMouseWheel(e);

        private static void OnMouseLeftButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonUp(e);

        private static void OnMouseEnterThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseEnter(e);

        private static void OnMouseLeaveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseLeave(e);

        private static void OnTextInputThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInput(e);

        private static void OnTappedThunk(object sender, TappedRoutedEventArgs e) => ((UIElement)sender).OnTapped(e);

        private static void OnMouseRightButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonUp(e);

        private static void OnKeyDownThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnKeyDown(e);

        private static void OnKeyUpThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnKeyUp(e);

        private static void OnGotFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnGotFocus(e);

        private static void OnLostFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnLostFocus(e);
#else
        private static void RegisterEvents(Type type)
        {
            RoutedEvent.RegisterClassHandler(PointerMovedEvent, new PointerEventHandler(OnMouseMoveThunk));
            RoutedEvent.RegisterClassHandler(PointerPressedEvent, new PointerEventHandler(OnMouseLeftButtonDownThunk));
            RoutedEvent.RegisterClassHandler(PointerWheelChangedEvent, new PointerEventHandler(OnMouseWheelThunk));
            RoutedEvent.RegisterClassHandler(PointerReleasedEvent, new PointerEventHandler(OnMouseLeftButtonUpThunk));
            RoutedEvent.RegisterClassHandler(PointerEnteredEvent, new PointerEventHandler(OnMouseEnterThunk));
            RoutedEvent.RegisterClassHandler(PointerExitedEvent, new PointerEventHandler(OnMouseLeaveThunk));
            RoutedEvent.RegisterClassHandler(TextInputEvent, new TextCompositionEventHandler(OnTextInputThunk));
            RoutedEvent.RegisterClassHandler(TappedEvent, new TappedEventHandler(OnTappedThunk));
            RoutedEvent.RegisterClassHandler(RightTappedEvent, new RightTappedEventHandler(OnMouseRightButtonUpThunk));
            RoutedEvent.RegisterClassHandler(KeyDownEvent, new KeyEventHandler(OnKeyDownThunk));
            RoutedEvent.RegisterClassHandler(KeyUpEvent, new KeyEventHandler(OnKeyUpThunk));
            RoutedEvent.RegisterClassHandler(GotFocusEvent, new RoutedEventHandler(OnGotFocusThunk));
            RoutedEvent.RegisterClassHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocusThunk));
        }

        private static void OnMouseMoveThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerMoved(e);

        private static void OnMouseLeftButtonDownThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerPressed(e);

        private static void OnMouseWheelThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerWheelChanged(e);

        private static void OnMouseLeftButtonUpThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerReleased(e);

        private static void OnMouseEnterThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerEntered(e);

        private static void OnMouseLeaveThunk(object sender, PointerRoutedEventArgs e) => ((UIElement)sender).OnPointerExited(e);

        private static void OnTextInputThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInput(e);

        private static void OnTappedThunk(object sender, TappedRoutedEventArgs e) => ((UIElement)sender).OnTapped(e);

        private static void OnMouseRightButtonUpThunk(object sender, RightTappedRoutedEventArgs e) => ((UIElement)sender).OnRightTapped(e);

        private static void OnKeyDownThunk(object sender, KeyRoutedEventArgs e) => ((UIElement)sender).OnKeyDown(e);

        private static void OnKeyUpThunk(object sender, KeyRoutedEventArgs e) => ((UIElement)sender).OnKeyUp(e);

        private static void OnGotFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnGotFocus(e);

        private static void OnLostFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnLostFocus(e);
#endif

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

        /// <summary>
        /// Raise the events specified by
        /// <see cref="RoutedEventArgs.RoutedEvent"/>
        /// </summary>
        internal void RaiseEvent(RoutedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.OriginalSource == null)
            {
                e.OriginalSource = this;
            }

            EventRoute route = EventRouteFactory.FetchObject(e.RoutedEvent);

            BuildRouteHelper(this, route, e);
            
            route.InvokeHandlers(e);

            EventRouteFactory.RecycleObject(route);
        }

        internal static void BuildRouteHelper(DependencyObject e, EventRoute route, RoutedEventArgs args)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.RoutedEvent != route.RoutedEvent)
            {
                throw new ArgumentException("RoutedEvent in RoutedEventArgs and EventRoute are mismatched.");
            }

            if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct)
            {
                UIElement uiElement = e as UIElement;

                // Add this element to route
                if (uiElement != null)
                {
                    uiElement.AddToEventRoute(route, args);
                }
            }
            else
            {
                int cElements = 0;

                while (e != null)
                {
                    UIElement uiElement = e as UIElement;

                    // Protect against infinite loops by limiting the number of elements
                    // that we will process.
                    if (cElements++ > MAX_ELEMENTS_IN_ROUTE)
                    {
                        throw new InvalidOperationException("Potential cycle in tree found while building the event route.");
                    }

                    // Invoke BuildRouteCore
                    if (uiElement != null)
                    {
                        // Add this element to route
                        uiElement.AddToEventRoute(route, args);

                        // Get element's visual parent
                        e = VisualTreeHelper.GetParent(uiElement);
                    }
                }
            }
        }

        /// <summary>
        ///     Add the event handlers for this element to the route.
        /// </summary>
        internal void AddToEventRoute(EventRoute route, RoutedEventArgs e)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            route.Add(this, RoutedEvent.GetClassHandler(e.RoutedEvent), false);

            EventHandlersStore store = _eventHandlersStore;
            if (store != null)
            {
                List<RoutedEventHandlerInfo> instanceListeners = store[e.RoutedEvent];

                // Add all instance listeners for this UIElement
                if (instanceListeners != null)
                {
                    for (int i = 0; i < instanceListeners.Count; i++)
                    {
                        route.Add(this, instanceListeners[i].Handler, instanceListeners[i].InvokeHandledEventsToo);
                    }
                }
            }
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
        }
#else
        protected virtual void OnPointerMoved(PointerRoutedEventArgs eventArgs)
        {
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
        }
#else
        protected virtual void OnPointerPressed(PointerRoutedEventArgs eventArgs)
        {
        }
#endif

        #endregion

        #region MouseRightButtonDown (no equivalent in UWP)

#if MIGRATION

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonDown"/> routed event.
        /// </summary>
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
        }

#endif

        #endregion

        #region PointerWheelChanged event (or MouseWheel)

#if MIGRATION
        /// <summary>
        /// Identifies the <see cref="MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent;
#else
        /// <summary>
        /// Identifies the <see cref="PointerWheelChanged"/> routed event.
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
        }
#else
        protected virtual void OnPointerWheelChanged(PointerRoutedEventArgs eventArgs)
        {
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
        }
#else
        protected virtual void OnPointerReleased(PointerRoutedEventArgs eventArgs)
        {
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
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseEnter(MouseEventArgs eventArgs)
        {
        }
#else
        protected virtual void OnPointerEntered(PointerRoutedEventArgs eventArgs)
        {
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
                AddHandler(MouseLeaveEvent, value, false);
            }
            remove
            {
                RemoveHandler(MouseLeaveEvent, value);
            }
        }
#else
        public event PointerEventHandler PointerExited
        {
            add
            {
                AddHandler(PointerExitedEvent, value, false);
            }
            remove
            {
                RemoveHandler(PointerExitedEvent, value);
            }
        }
#endif

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseLeave(MouseEventArgs eventArgs)
        {
        }
#else
        protected virtual void OnPointerExited(PointerRoutedEventArgs eventArgs)
        {
        }
#endif

        #endregion

        #region Text events

        /// <summary>
        /// Identifies the <see cref="TextInput"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputEvent;

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
        /// Raises the TextInput event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTextInput(TextCompositionEventArgs eventArgs)
        {
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
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTapped(TappedRoutedEventArgs eventArgs)
        {
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
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs eventArgs)
        {
        }
#else
        protected virtual void OnRightTapped(RightTappedRoutedEventArgs eventArgs)
        {
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
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyDown(KeyEventArgs eventArgs)
        {
        }
#else
        protected virtual void OnKeyDown(KeyRoutedEventArgs eventArgs)
        {
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
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
#if MIGRATION
        protected virtual void OnKeyUp(KeyEventArgs eventArgs)
        {
        }
#else
        protected virtual void OnKeyUp(KeyRoutedEventArgs eventArgs)
        {
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
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnGotFocus(RoutedEventArgs eventArgs)
        {
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
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnLostFocus(RoutedEventArgs eventArgs)
        {
        }

        #endregion

        #region Allow or Prevent Focus events for IsTabStop

        internal virtual object GetFocusTarget() => INTERNAL_OuterDomElement;

        #endregion

        public virtual void INTERNAL_AttachToDomEvents()
        {
            AddEventListeners();
        }

        public virtual void INTERNAL_DetachFromDomEvents()
        {
        }

        internal virtual void AddEventListeners()
        {
            InputManager.Current.AddEventListeners(this, false);
        }

        internal virtual UIElement MouseTarget => this;

        internal virtual UIElement KeyboardTarget => this;

        internal bool IsPointerOver { get; set; }

        internal void RaiseMouseLeave()
        {
            Debug.Assert(IsPointerOver == true);
            IsPointerOver = false;

#if MIGRATION
            var e = new MouseEventArgs
            {
                RoutedEvent = MouseLeaveEvent,
                OriginalSource = this,
            };
#else
            var e = new PointerRoutedEventArgs
            {
                RoutedEvent = PointerExitedEvent,
                OriginalSource = this,
            };
#endif

            RaiseEvent(e);
        }

        internal virtual void OnTextInputInternal() { }
    }
}