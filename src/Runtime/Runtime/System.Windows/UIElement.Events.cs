
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
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows
{
    partial class UIElement
    {
        internal const int MAX_ELEMENTS_IN_ROUTE = 4096;

        internal EventHandlersStore EventHandlersStore { get; private set; }

        private void EnsureEventHandlersStore() => EventHandlersStore ??= new EventHandlersStore();

        private static void RegisterEvents()
        {
            EventManager.RegisterClassHandler<UIElement>(MouseMoveEvent, new MouseEventHandler(OnMouseMoveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseRightButtonDownEvent, new MouseButtonEventHandler(OnMouseRightButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheelThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseEnterEvent, new MouseEventHandler(OnMouseEnterThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseLeaveEvent, new MouseEventHandler(OnMouseLeaveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStartThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TextInputEvent, new TextCompositionEventHandler(OnTextInputThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TappedEvent, new TappedEventHandler(OnTappedThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseRightButtonUpEvent, new MouseButtonEventHandler(OnMouseRightButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(KeyDownEvent, new KeyEventHandler(OnKeyDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(KeyUpEvent, new KeyEventHandler(OnKeyUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(GotFocusEvent, new RoutedEventHandler(OnGotFocusThunk), false);
            EventManager.RegisterClassHandler<UIElement>(LostFocusEvent, new RoutedEventHandler(OnLostFocusThunk), false);
            EventManager.RegisterClassHandler<UIElement>(LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureThunk), false);
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
        /// Adds a routed event handler for a specified routed event, adding the handler to the 
        /// handler collection on the current element.
        /// </summary>
        /// <param name="routedEvent">
        /// An identifier for the routed event to be handled.
        /// </param>
        /// <param name="handler">
        /// A reference to the handler implementation.
        /// </param>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler) => AddHandler(routedEvent, handler, false);

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
            if (routedEvent is null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException(Strings.HandlerTypeIllegal);
            }

            EnsureEventHandlersStore();
            EventHandlersStore.AddRoutedEventHandler(routedEvent, handler, handledEventsToo);
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
            if (routedEvent is null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException(Strings.HandlerTypeIllegal);
            }

            EventHandlersStore?.RemoveRoutedEventHandler(routedEvent, handler);
        }

        /// <summary>
        /// Raises a specific routed event. The <see cref="RoutedEvent"/> to be raised is 
        /// identified within the <see cref="RoutedEventArgs"/> instance that is provided 
        /// (as the <see cref="RoutedEventArgs.RoutedEvent"/> property of that event data).
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedEventArgs"/> that contains the event data and also identifies 
        /// the event to raise.
        /// </param>
        public void RaiseEvent(RoutedEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            e.ClearUserInitiated();

            RaiseEventImpl(this, e);
        }

        private static void RaiseEventImpl(UIElement sender, RoutedEventArgs args)
        {
            EventRoute route = EventRouteFactory.FetchObject(args.RoutedEvent);

            // Set Source
            args.Source = sender;

            BuildRouteHelper(sender, route, args);

            route.InvokeHandlers(args);

            // Reset Source to OriginalSource
            args.Source = args.OriginalSource;

            EventRouteFactory.RecycleObject(route);
        }

        private static void BuildRouteHelper(UIElement e, EventRoute route, RoutedEventArgs args)
        {
            Debug.Assert(route is not null);
            Debug.Assert(args is not null);

            if (args.Source is null)
            {
                throw new ArgumentException(Strings.SourceNotSet);
            }

            if (args.RoutedEvent != route.RoutedEvent)
            {
                throw new ArgumentException(Strings.Mismatched_RoutedEvent);
            }

            if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct)
            {
                // Add this element to route
                e.AddToEventRoute(route, args);
            }
            else
            {
                int cElements = 0;

                while (e != null)
                {
                    // Protect against infinite loops by limiting the number of elements
                    // that we will process.
                    if (cElements++ > MAX_ELEMENTS_IN_ROUTE)
                    {
                        throw new InvalidOperationException(Strings.TreeLoop);
                    }

                    // Invoke BuildRouteCore
                    // Add this element to route
                    e.AddToEventRoute(route, args);

                    // Get element's visual parent
                    e = VisualTreeHelper.GetParent(e) as UIElement;
                }
            }
        }

        /// <summary>
        /// Adds handlers to the specified <see cref="EventRoute"/> for the current <see cref="UIElement"/> event handler 
        /// collection.
        /// </summary>
        /// <param name="route">
        /// The event route that handlers are added to.
        /// </param>
        /// <param name="e">
        /// The event data that is used to add the handlers. This method uses the <see cref="RoutedEventArgs.RoutedEvent"/>
        /// property of the event data to create the handlers.
        /// </param>
        private void AddToEventRoute(EventRoute route, RoutedEventArgs e)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Get class listeners for this UIElement
            RoutedEventHandlerInfoList classListeners =
                GlobalEventManager.GetDTypedClassListeners(DependencyObjectType, e.RoutedEvent);

            // Add all class listeners for this UIElement
            while (classListeners != null)
            {
                for (int i = 0; i < classListeners.Handlers.Length; i++)
                {
                    route.Add(this, classListeners.Handlers[i].Handler, classListeners.Handlers[i].InvokeHandledEventsToo);
                }

                classListeners = classListeners.Next;
            }

            if (EventHandlersStore is EventHandlersStore store)
            {
                if (store.Get(e.RoutedEvent) is List<RoutedEventHandlerInfo> instanceListeners)
                {
                    // Add all instance listeners for this UIElement
                    foreach (RoutedEventHandlerInfo info in instanceListeners)
                    {
                        route.Add(this, info.Handler, info.InvokeHandledEventsToo);
                    }
                }
            }
        }

        #region Pointer moved event

        /// <summary>
        /// Identifies the <see cref="MouseMove"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseMoveEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseMove),
                RoutingStrategy.Bubble,
                typeof(MouseEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// moved, while within this element.
        /// </summary>
        public event MouseEventHandler MouseMove
        {
            add => AddHandler(MouseMoveEvent, value, false);
            remove => RemoveHandler(MouseMoveEvent, value);
        }

        /// <summary>
        /// Raises the PointerMoved event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseMove(MouseEventArgs e) { }

        #endregion

        #region Pointer pressed event

        /// <summary>
        /// Identifies the <see cref="MouseLeftButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeftButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseLeftButtonDown),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonDown
        {
            add => AddHandler(MouseLeftButtonDownEvent, value, false);
            remove => RemoveHandler(MouseLeftButtonDownEvent, value);
        }

        /// <summary>
        /// Raises the PointerPressed event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }

        #endregion

        #region MouseRightButtonDown

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseRightButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseRightButtonDown),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        public event MouseButtonEventHandler MouseRightButtonDown
        {
            add => AddHandler(MouseRightButtonDownEvent, value, false);
            remove => RemoveHandler(MouseRightButtonDownEvent, value);
        }

        /// <summary>
        /// Raises the MouseRightButtonDown event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e) { }

        #endregion

        #region PointerWheelChanged event (or MouseWheel)

        /// <summary>
        /// Identifies the <see cref="MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseWheel),
                RoutingStrategy.Bubble,
                typeof(MouseWheelEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event MouseWheelEventHandler MouseWheel
        {
            add => AddHandler(MouseWheelEvent, value, false);
            remove => RemoveHandler(MouseWheelEvent, value);
        }

        /// <summary>
        /// Raises the PointerWheelChanged event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseWheel(MouseWheelEventArgs e) { }

        #endregion


        #region Pointer released event

        /// <summary>
        /// Identifies the <see cref="MouseLeftButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeftButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseLeftButtonUp),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// released, while within this element.
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonUp
        {
            add => AddHandler(MouseLeftButtonUpEvent, value, false);
            remove => RemoveHandler(MouseLeftButtonUpEvent, value);
        }

        /// <summary>
        /// Raises the PointerReleased event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }

        #endregion

        #region Pointer entered event

        /// <summary>
        /// Identifies the <see cref="MouseEnter"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseEnterEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseEnter),
                RoutingStrategy.Direct,
                typeof(MouseEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a pointer enters the hit test area of this element.
        /// </summary>
        public event MouseEventHandler MouseEnter
        {
            add => AddHandler(MouseEnterEvent, value, false);
            remove => RemoveHandler(MouseEnterEvent, value);
        }

        /// <summary>
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseEnter(MouseEventArgs e) { }

        #endregion

        #region Pointer exited event

        /// <summary>
        /// Identifies the <see cref="MouseLeave"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeaveEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseLeave),
                RoutingStrategy.Direct,
                typeof(MouseEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a pointer leaves the hit test area of this element.
        /// </summary>
        public event MouseEventHandler MouseLeave
        {
            add => AddHandler(MouseLeaveEvent, value, false);
            remove => RemoveHandler(MouseLeaveEvent, value);
        }

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseLeave(MouseEventArgs e) { }

        #endregion

        #region Text events

        /// <summary>
        /// Identifies the <see cref="TextInputStart"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputStartEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextInputStart),
                RoutingStrategy.Bubble,
                typeof(TextCompositionEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a UI element initially gets text in a device-independent manner.
        /// </summary>
        public event TextCompositionEventHandler TextInputStart
        {
            add => AddHandler(TextInputStartEvent, value, false);
            remove => RemoveHandler(TextInputStartEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="TextInputStart"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TextCompositionEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnTextInputStart(TextCompositionEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="TextInput"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextInput),
                RoutingStrategy.Bubble,
                typeof(TextCompositionEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a UI element gets text in a device-independent manner.
        /// </summary>
        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event TextCompositionEventHandler TextInput
        {
            add => AddHandler(TextInputEvent, value, false);
            remove => RemoveHandler(TextInputEvent, value);
        }

        /// <summary>
        /// Called before the <see cref="TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TextCompositionEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnTextInput(TextCompositionEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="TextInputUpdate"/> routed event.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly RoutedEvent TextInputUpdateEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextInputUpdate),
                RoutingStrategy.Bubble,
                typeof(TextCompositionEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when text continues to be composed via an input method editor (IME).
        /// </summary>
        [OpenSilver.NotImplemented]
        public event TextCompositionEventHandler TextInputUpdate
        {
            add => AddHandler(TextInputUpdateEvent, value, false);
            remove => RemoveHandler(TextInputUpdateEvent, value);
        }

        #endregion

        #region Tapped event

        /// <summary>
        /// Identifies the <see cref="Tapped"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TappedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Tapped),
                RoutingStrategy.Bubble,
                typeof(TappedEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when an otherwise unhandled Tap interaction occurs over the hit test
        /// area of this element.
        /// </summary>
        public event TappedEventHandler Tapped
        {
            add => AddHandler(TappedEvent, value, false);
            remove => RemoveHandler(TappedEvent, value);
        }

        /// <summary>
        /// Raises the Tapped event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnTapped(TappedRoutedEventArgs e) { }

        #endregion

        #region RightTapped (aka MouseRightButtonUp) event

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseRightButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseRightButtonUp),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a right-tap input stimulus happens while the pointer is over
        /// the element.
        /// </summary>
        public event MouseButtonEventHandler MouseRightButtonUp
        {
            add => AddHandler(MouseRightButtonUpEvent, value, false);
            remove => RemoveHandler(MouseRightButtonUpEvent, value);
        }

        /// <summary>
        /// Raises the RightTapped event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs e) { }

        #endregion

        #region KeyDown event

        /// <summary>
        /// Identifies the <see cref="KeyDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent KeyDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(KeyDown),
                RoutingStrategy.Bubble,
                typeof(KeyEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a keyboard key is pressed while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyDown
        {
            add => AddHandler(KeyDownEvent, value, false);
            remove => RemoveHandler(KeyDownEvent, value);
        }

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnKeyDown(KeyEventArgs e) { }

        #endregion

        #region KeyUp event

        /// <summary>
        /// Identifies the <see cref="KeyUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent KeyUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(KeyUp),
                RoutingStrategy.Bubble,
                typeof(KeyEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a keyboard key is released while the UIElement has focus.
        /// </summary>
        public event KeyEventHandler KeyUp
        {
            add => AddHandler(KeyUpEvent, value, false);
            remove => RemoveHandler(KeyUpEvent, value);
        }

        /// <summary>
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnKeyUp(KeyEventArgs e) { }

        #endregion

        #region GotFocus event

        /// <summary>
        /// Identifies the <see cref="GotFocus"/> routed event.
        /// </summary>
        public static readonly RoutedEvent GotFocusEvent =
            EventManager.RegisterRoutedEvent(
                nameof(GotFocus),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// pressed, while within this element.
        /// Note that ONLY sender's informations are currently filled (not pointer's)
        /// </summary>
        public event RoutedEventHandler GotFocus
        {
            add => AddHandler(GotFocusEvent, value, false);
            remove => RemoveHandler(GotFocusEvent, value);
        }

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnGotFocus(RoutedEventArgs e) { }

        #endregion

        #region Lostfocus event

        /// <summary>
        /// Identifies the <see cref="LostFocus"/> routed event.
        /// </summary>
        public static readonly RoutedEvent LostFocusEvent =
            EventManager.RegisterRoutedEvent(
                nameof(LostFocus),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a UIElement loses focus.
        /// </summary>
        public event RoutedEventHandler LostFocus
        {
            add => AddHandler(LostFocusEvent, value, false);
            remove => RemoveHandler(LostFocusEvent, value);
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnLostFocus(RoutedEventArgs e) { }

        #endregion

        #region LostMouseCapture event

        /// <summary>
        /// Identifies the <see cref="LostMouseCapture"/> routed event.
        /// </summary>
        public static readonly RoutedEvent LostMouseCaptureEvent =
            EventManager.RegisterRoutedEvent(
                nameof(LostMouseCapture),
                RoutingStrategy.Bubble,
                typeof(MouseEventHandler),
                typeof(UIElement));

        public event MouseEventHandler LostMouseCapture
        {
            add => AddHandler(LostMouseCaptureEvent, value, false);
            remove => RemoveHandler(LostMouseCaptureEvent, value);
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