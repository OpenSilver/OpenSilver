
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

using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;

namespace System.Windows
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

        private static void RegisterEvents(Type type)
        {
            RoutedEvent.RegisterClassHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMoveThunk));
            RoutedEvent.RegisterClassHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownThunk));
            RoutedEvent.RegisterClassHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(OnMouseRightButtonDownThunk));
            RoutedEvent.RegisterClassHandler(MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheelThunk));
            RoutedEvent.RegisterClassHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpThunk));
            RoutedEvent.RegisterClassHandler(MouseEnterEvent, new MouseEventHandler(OnMouseEnterThunk));
            RoutedEvent.RegisterClassHandler(MouseLeaveEvent, new MouseEventHandler(OnMouseLeaveThunk));
            RoutedEvent.RegisterClassHandler(TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStartThunk));
            RoutedEvent.RegisterClassHandler(TextInputEvent, new TextCompositionEventHandler(OnTextInputThunk));
            RoutedEvent.RegisterClassHandler(TappedEvent, new TappedEventHandler(OnTappedThunk));
            RoutedEvent.RegisterClassHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler(OnMouseRightButtonUpThunk));
            RoutedEvent.RegisterClassHandler(KeyDownEvent, new KeyEventHandler(OnKeyDownThunk));
            RoutedEvent.RegisterClassHandler(KeyUpEvent, new KeyEventHandler(OnKeyUpThunk));
            RoutedEvent.RegisterClassHandler(GotFocusEvent, new RoutedEventHandler(OnGotFocusThunk));
            RoutedEvent.RegisterClassHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocusThunk));
            RoutedEvent.RegisterClassHandler(LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureThunk));
        }

        private static void OnMouseMoveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseMove(e);

        private static void OnMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonDown(e);

        private static void OnMouseRightButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonDown(e);

        private static void OnMouseWheelThunk(object sender, MouseWheelEventArgs e) => ((UIElement)sender).OnMouseWheel(e);

        private static void OnMouseLeftButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonUp(e);

        private static void OnMouseEnterThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseEnter(e);

        private static void OnMouseLeaveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseLeave(e);

        private static void OnTextInputStartThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInputStart(e);

        private static void OnTextInputThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInput(e);

        private static void OnTappedThunk(object sender, TappedRoutedEventArgs e) => ((UIElement)sender).OnTapped(e);

        private static void OnMouseRightButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonUp(e);

        private static void OnKeyDownThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnKeyDown(e);

        private static void OnKeyUpThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnKeyUp(e);

        private static void OnGotFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnGotFocus(e);

        private static void OnLostFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnLostFocus(e);

        private static void OnLostMouseCaptureThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnLostMouseCapture(e);

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

        public static readonly RoutedEvent MouseMoveEvent;

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// moved, while within this element.
        /// </summary>
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

        /// <summary>
        /// Raises the PointerMoved event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>

        protected virtual void OnMouseMove(MouseEventArgs eventArgs)
        {
        }

        #endregion

        #region Pointer pressed event

        public static readonly RoutedEvent MouseLeftButtonDownEvent;

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// </summary>
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

        /// <summary>
        /// Raises the PointerPressed event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
        {
        }

        #endregion

        #region MouseRightButtonDown

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

        #endregion

        #region PointerWheelChanged event (or MouseWheel)

        /// <summary>
        /// Identifies the <see cref="MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent;

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

        /// <summary>
        /// Raises the PointerWheelChanged event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseWheel(MouseWheelEventArgs eventArgs)
        {
        }

        #endregion


        #region Pointer released event

        public static readonly RoutedEvent MouseLeftButtonUpEvent;

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// released, while within this element.
        /// </summary>
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

        /// <summary>
        /// Raises the PointerReleased event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
        {
        }

        #endregion

        #region Pointer entered event

        public static readonly RoutedEvent MouseEnterEvent;

        /// <summary>
        /// Occurs when a pointer enters the hit test area of this element.
        /// </summary>
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

        /// <summary>
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseEnter(MouseEventArgs eventArgs)
        {
        }

        #endregion

        #region Pointer exited event

        public static readonly RoutedEvent MouseLeaveEvent;

        /// <summary>
        /// Occurs when a pointer leaves the hit test area of this element.
        /// </summary>
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

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseLeave(MouseEventArgs eventArgs)
        {
        }

        #endregion

        #region Text events

        /// <summary>
        /// Identifies the <see cref="TextInputStart"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputStartEvent;

        /// <summary>
        /// Occurs when a UI element initially gets text in a device-independent manner.
        /// </summary>
        public event TextCompositionEventHandler TextInputStart
        {
            add
            {
                AddHandler(TextInputStartEvent, value, false);
            }
            remove
            {
                RemoveHandler(TextInputStartEvent, value);
            }
        }

        /// <summary>
        /// Called before the <see cref="TextInputStart"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TextCompositionEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnTextInputStart(TextCompositionEventArgs e)
        {
        }

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
        /// Called before the <see cref="TextInput"/> event occurs.
        /// </summary>
        /// <param name="eventArgs">
        /// A <see cref="TextCompositionEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnTextInput(TextCompositionEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Identifies the <see cref="TextInputUpdate"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputUpdateEvent;

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

        public static readonly RoutedEvent MouseRightButtonUpEvent;

        /// <summary>
        /// Occurs when a right-tap input stimulus happens while the pointer is over
        /// the element.
        /// </summary>
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

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs eventArgs)
        {
        }

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
        protected virtual void OnKeyDown(KeyEventArgs eventArgs)
        {
        }

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
        protected virtual void OnKeyUp(KeyEventArgs eventArgs)
        {
        }

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

        #region LostMouseCapture event

        internal static readonly RoutedEvent LostMouseCaptureEvent;

        public event MouseEventHandler LostMouseCapture
        {
            add
            {
                AddHandler(LostMouseCaptureEvent, value, false);
            }
            remove
            {
                RemoveHandler(LostMouseCaptureEvent, value);
            }
        }

        /// <summary>
        /// Called before the <see cref="LostMouseCapture"/> event occurs to provide
        /// handling for the event in a derived class without attaching a delegate.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnLostMouseCapture(MouseEventArgs e) { }

        #endregion

        internal virtual INTERNAL_HtmlDomElementReference GetFocusTarget() => OuterDiv;

        public virtual void INTERNAL_AttachToDomEvents() { }

        public virtual void INTERNAL_DetachFromDomEvents() { }

        internal virtual UIElement MouseTarget => this;

        internal virtual UIElement KeyboardTarget => this;

        internal bool IsPointerOver { get; set; }
    }
}