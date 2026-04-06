
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
using OpenSilver;
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
            EventManager.RegisterClassHandler<UIElement>(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDownThunk), true);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDownThunk), true);
            EventManager.RegisterClassHandler<UIElement>(Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUpThunk), true);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUpThunk), true);
            EventManager.RegisterClassHandler<UIElement>(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseRightButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseRightButtonDownEvent, new MouseButtonEventHandler(OnMouseRightButtonDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(PreviewMouseRightButtonUpEvent, new MouseButtonEventHandler(OnPreviewMouseRightButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(MouseRightButtonUpEvent, new MouseButtonEventHandler(OnMouseRightButtonUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.PreviewMouseMoveEvent, new MouseEventHandler(OnPreviewMouseMoveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMoveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheelThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheelThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseEnterEvent, new MouseEventHandler(OnMouseEnterThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.MouseLeaveEvent, new MouseEventHandler(OnMouseLeaveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.GotMouseCaptureEvent, new MouseEventHandler(OnGotMouseCaptureThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStartThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TextInputEvent, new TextCompositionEventHandler(OnTextInputThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TextInputUpdateEvent, new TextCompositionEventHandler(OnTextInputUpdateThunk), false);
            EventManager.RegisterClassHandler<UIElement>(TappedEvent, new TappedEventHandler(OnTappedThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Keyboard.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDownThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Keyboard.PreviewKeyUpEvent, new KeyEventHandler(OnPreviewKeyUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUpThunk), false);
            EventManager.RegisterClassHandler<UIElement>(GotFocusEvent, new RoutedEventHandler(OnGotFocusThunk), false);
            EventManager.RegisterClassHandler<UIElement>(LostFocusEvent, new RoutedEventHandler(OnLostFocusThunk), false);
            EventManager.RegisterClassHandler<UIElement>(DragEnterEvent, new DragEventHandler(OnDragEnterThunk), false);
            EventManager.RegisterClassHandler<UIElement>(DragLeaveEvent, new DragEventHandler(OnDragLeaveThunk), false);
            EventManager.RegisterClassHandler<UIElement>(DropEvent, new DragEventHandler(OnDropThunk), false);
            EventManager.RegisterClassHandler<UIElement>(DragOverEvent, new DragEventHandler(OnDragOverThunk), false);
            EventManager.RegisterClassHandler<UIElement>(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(OnPreviewExecutedThunk), false);
            EventManager.RegisterClassHandler<UIElement>(CommandManager.ExecutedEvent, new ExecutedRoutedEventHandler(OnExecutedThunk), false);
            EventManager.RegisterClassHandler<UIElement>(CommandManager.PreviewCanExecuteEvent, new CanExecuteRoutedEventHandler(OnPreviewCanExecuteThunk), false);
            EventManager.RegisterClassHandler<UIElement>(CommandManager.CanExecuteEvent, new CanExecuteRoutedEventHandler(OnCanExecuteThunk), false);
        }

        private static void OnPreviewMouseDownThunk(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            if (!e.Handled)
            {
                uie.OnPreviewMouseDown(e);
            }

            // Always raise this "sub-event", but we pass along the handledness.
            CrackMouseButtonEventAndReRaiseEvent(uie, e);
        }

        private static void OnMouseDownThunk(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            if (!e.Handled)
            {
                CommandManager.TranslateInput(uie, e);
            }

            if (!e.Handled)
            {
                uie.OnMouseDown(e);
            }

            // Always raise this "sub-event", but we pass along the handledness.
            CrackMouseButtonEventAndReRaiseEvent(uie, e);
        }

        private static void OnPreviewMouseUpThunk(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            if (!e.Handled)
            {
                uie.OnPreviewMouseUp(e);
            }

            // Always raise this "sub-event", but we pass along the handledness.
            CrackMouseButtonEventAndReRaiseEvent(uie, e);
        }

        private static void OnMouseUpThunk(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            if (!e.Handled)
            {
                uie.OnMouseUp(e);
            }

            // Always raise this "sub-event", but we pass along the handledness.
            CrackMouseButtonEventAndReRaiseEvent(uie, e);
        }

        private static void OnPreviewMouseMoveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnPreviewMouseMove(e);

        private static void OnMouseMoveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseMove(e);

        private static void OnPreviewMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnPreviewMouseLeftButtonDown(e);

        private static void OnMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonDown(e);

        private static void OnPreviewMouseRightButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnPreviewMouseRightButtonDown(e);

        private static void OnMouseRightButtonDownThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonDown(e);

        private static void OnPreviewMouseWheelThunk(object sender, MouseWheelEventArgs e) => ((UIElement)sender).OnPreviewMouseWheel(e);

        private static void OnMouseWheelThunk(object sender, MouseWheelEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            CommandManager.TranslateInput(uie, e);

            if (!e.Handled)
            {
                uie.OnMouseWheel(e);
            }
        }

        private static void OnPreviewMouseLeftButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnPreviewMouseLeftButtonUp(e);

        private static void OnMouseLeftButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseLeftButtonUp(e);

        private static void OnPreviewMouseRightButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnPreviewMouseRightButtonUp(e);

        private static void OnMouseRightButtonUpThunk(object sender, MouseButtonEventArgs e) => ((UIElement)sender).OnMouseRightButtonUp(e);

        private static void OnMouseEnterThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseEnter(e);

        private static void OnMouseLeaveThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnMouseLeave(e);

        private static void OnTextInputStartThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInputStart(e);

        private static void OnTextInputThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInput(e);

        private static void OnTextInputUpdateThunk(object sender, TextCompositionEventArgs e) => ((UIElement)sender).OnTextInputUpdate(e);

        private static void OnTappedThunk(object sender, TappedRoutedEventArgs e) => ((UIElement)sender).OnTapped(e);

        private static void OnPreviewKeyDownThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnPreviewKeyDown(e);

        private static void OnKeyDownThunk(object sender, KeyEventArgs e)
        {
            UIElement uie = (UIElement)sender;

            CommandManager.TranslateInput(uie, e);

            if (!e.Handled)
            {
                uie.OnKeyDown(e);
            }
        }

        private static void OnPreviewKeyUpThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnPreviewKeyUp(e);

        private static void OnKeyUpThunk(object sender, KeyEventArgs e) => ((UIElement)sender).OnKeyUp(e);

        private static void OnGotFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnGotFocus(e);

        private static void OnLostFocusThunk(object sender, RoutedEventArgs e) => ((UIElement)sender).OnLostFocus(e);

        private static void OnGotMouseCaptureThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnGotMouseCapture(e);

        private static void OnLostMouseCaptureThunk(object sender, MouseEventArgs e) => ((UIElement)sender).OnLostMouseCapture(e);

        private static void OnDragEnterThunk(object sender, DragEventArgs e) => ((UIElement)sender).OnDragEnter(e);

        private static void OnDragLeaveThunk(object sender, DragEventArgs e) => ((UIElement)sender).OnDragLeave(e);

        private static void OnDropThunk(object sender, DragEventArgs e) => ((UIElement)sender).OnDrop(e);

        private static void OnDragOverThunk(object sender, DragEventArgs e) => ((UIElement)sender).OnDragOver(e);

        private static void OnPreviewExecutedThunk(object sender, ExecutedRoutedEventArgs e)
        {
            // Command Manager will determine if preview or regular event.
            CommandManager.OnExecuted(sender, e);
        }

        private static void OnExecutedThunk(object sender, ExecutedRoutedEventArgs e)
        {
            // Command Manager will determine if preview or regular event.
            CommandManager.OnExecuted(sender, e);
        }

        private static void OnPreviewCanExecuteThunk(object sender, CanExecuteRoutedEventArgs e)
        {
            // Command Manager will determine if preview or regular event.
            CommandManager.OnCanExecute(sender, e);
        }

        private static void OnCanExecuteThunk(object sender, CanExecuteRoutedEventArgs e)
        {
            // Command Manager will determine if preview or regular event.
            CommandManager.OnCanExecute(sender, e);
        }

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
            ArgumentNullException.ThrowIfNull(routedEvent);
            ArgumentNullException.ThrowIfNull(handler);

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
            ArgumentNullException.ThrowIfNull(routedEvent);
            ArgumentNullException.ThrowIfNull(handler);

            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException(Strings.HandlerTypeIllegal);
            }

            EventHandlersStore?.RemoveRoutedEventHandler(routedEvent, handler);
        }

        internal static void AddHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
        {
            switch (d)
            {
                case UIElement uiElement:
                    uiElement.AddHandler(routedEvent, handler);
                    break;

                case IUIElement iuiElement:
                    iuiElement.AddHandler(routedEvent, handler, false);
                    break;

                default:
                    throw new ArgumentException(string.Format(Strings.Invalid_IInputElement, d.GetType()));
            }
        }

        internal static void RemoveHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
        {
            switch (d)
            {
                case UIElement uiElement:
                    uiElement.RemoveHandler(routedEvent, handler);
                    break;

                case IUIElement iuiElement:
                    iuiElement.RemoveHandler(routedEvent, handler);
                    break;

                default:
                    throw new ArgumentException(string.Format(Strings.Invalid_IInputElement, d.GetType()));
            }
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
            ArgumentNullException.ThrowIfNull(e);

            e.ClearUserInitiated();

            RaiseEventImpl(this, e);
        }

        /// <summary>
        ///     "Trusted" internal flavor of RaiseEvent.
        ///     Used to set the User-initated RaiseEvent.
        /// </summary>
        internal void RaiseEvent(RoutedEventArgs args, bool trusted)
        {
            Debug.Assert(args is not null);

            if (trusted)
            {
                RaiseTrustedEvent(args);
            }
            else
            {
                args.ClearUserInitiated();

                RaiseEventImpl(this, args);
            }
        }

        internal void RaiseTrustedEvent(RoutedEventArgs args)
        {
            Debug.Assert(args is not null);

            // Try/finally to ensure that UserInitiated bit is cleared.
            args.MarkAsUserInitiated();

            try
            {
                RaiseEventImpl(this, args);
            }
            finally
            {
                // Clear the bit - just to guarantee it's not used again
                args.ClearUserInitiated();
            }
        }

        private static RoutedEvent CrackMouseButtonEvent(MouseButtonEventArgs e)
        {
            RoutedEvent newEvent = null;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (e.RoutedEvent == Mouse.PreviewMouseDownEvent)
                    {
                        newEvent = PreviewMouseLeftButtonDownEvent;
                    }
                    else if (e.RoutedEvent == Mouse.MouseDownEvent)
                    {
                        newEvent = MouseLeftButtonDownEvent;
                    }
                    else if (e.RoutedEvent == Mouse.PreviewMouseUpEvent)
                    {
                        newEvent = PreviewMouseLeftButtonUpEvent;
                    }
                    else
                    {
                        newEvent = MouseLeftButtonUpEvent;
                    }
                    break;
                case MouseButton.Right:
                    if (e.RoutedEvent == Mouse.PreviewMouseDownEvent)
                    {
                        newEvent = PreviewMouseRightButtonDownEvent;
                    }
                    else if (e.RoutedEvent == Mouse.MouseDownEvent)
                    {
                        newEvent = MouseRightButtonDownEvent;
                    }
                    else if (e.RoutedEvent == Mouse.PreviewMouseUpEvent)
                    {
                        newEvent = PreviewMouseRightButtonUpEvent;
                    }
                    else
                    {
                        newEvent = MouseRightButtonUpEvent;
                    }
                    break;
                default:
                    // No wrappers exposed for the other buttons.
                    break;
            }
            return newEvent;
        }

        private static void CrackMouseButtonEventAndReRaiseEvent(UIElement uie, MouseButtonEventArgs e)
        {
            if (CrackMouseButtonEvent(e) is RoutedEvent newEvent)
            {
                ReRaiseEventAs(uie, e, newEvent);
            }
        }

        /// <summary>
        ///     Re-raises an event with as a different RoutedEvent.
        /// </summary>
        /// <remarks>
        ///     Only used internally.  Added to support cracking generic MouseButtonDown/Up events
        ///     into MouseLeft/RightButtonDown/Up events.
        /// </remarks>
        /// <param name="uie">
        ///     The Source associated with the RoutedEventArgs
        /// </param>
        /// <param name="args">
        ///     RoutedEventsArgs to re-raise with a new RoutedEvent
        /// </param>
        /// <param name="newEvent">
        ///     The new RoutedEvent to be associated with the RoutedEventArgs
        /// </param>
        private static void ReRaiseEventAs(UIElement uie, RoutedEventArgs args, RoutedEvent newEvent)
        {
            // Preseve and change the RoutedEvent
            RoutedEvent preservedRoutedEvent = args.RoutedEvent;
            args.OverrideRoutedEvent(newEvent);

            // Preserve Source
            object preservedSource = args.Source;

            EventRoute route = EventRouteFactory.FetchObject(args.RoutedEvent);

            // Build the route and invoke the handlers
            BuildRouteHelper(uie, route, args);

            route.InvokeHandlers(args);

            // Restore Source
            args.OverrideSource(preservedSource);

            // Restore RoutedEvent
            args.OverrideRoutedEvent(preservedRoutedEvent);

            // Recycle the route object
            EventRouteFactory.RecycleObject(route);
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

                    // Add this element to route
                    e.AddToEventRoute(route, args);

                    // Get element's visual parent
                    DependencyObject parent = VisualTreeHelper.GetParent(e);
                    if (parent is null && !args.RoutedEvent.IsCoreEvent)
                    {
                        parent = LogicalTreeHelper.GetParent(e);
                    }

                    e = parent as UIElement;
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
            ArgumentNullException.ThrowIfNull(route);
            ArgumentNullException.ThrowIfNull(e);

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

            // Allow FrameworkElememnt to add event handlers in styles
            AddToEventRouteCore(route, e);
        }

        /// <summary>
        /// This virtual method is to be overridden in FrameworkElement to be able to add handlers for styles
        /// </summary>
        internal virtual void AddToEventRouteCore(EventRoute route, RoutedEventArgs args) { }

        #region MouseDown

        /// <summary>
        /// Identifies the <see cref="PreviewMouseDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseDownEvent = Mouse.PreviewMouseDownEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when any mouse button is pressed while the pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseDown
        {
            add => AddHandler(Mouse.PreviewMouseDownEvent, value, false);
            remove => RemoveHandler(Mouse.PreviewMouseDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseDown"/> routed event reaches an element in 
        /// its route that is derived from this class. Implement this method to add class handling for 
        /// this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that one or more mouse buttons were pressed.
        /// </param>
        protected virtual void OnPreviewMouseDown(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseDownEvent = Mouse.MouseDownEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when any mouse button is pressed while the pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler MouseDown
        {
            add => AddHandler(Mouse.MouseDownEvent, value, false);
            remove => RemoveHandler(Mouse.MouseDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseDown"/> routed event reaches an element in its route 
        /// that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. This event data reports 
        /// details about the mouse button that was pressed and the handled state.
        /// </param>
        protected virtual void OnMouseDown(MouseButtonEventArgs e) { }

        #endregion

        #region MouseUp

        /// <summary>
        /// Identifies the <see cref="PreviewMouseUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseUpEvent = Mouse.PreviewMouseUpEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when any mouse button is released while the mouse pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseUp
        {
            add => AddHandler(Mouse.PreviewMouseUpEvent, value, false);
            remove => RemoveHandler(Mouse.PreviewMouseUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseUp"/> routed event reaches an element in its route 
        /// that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports that one 
        /// or more mouse buttons were released.
        /// </param>
        protected virtual void OnPreviewMouseUp(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseUpEvent = Mouse.MouseUpEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when any mouse button is released over this element.
        /// </summary>
        public event MouseButtonEventHandler MouseUp
        {
            add => AddHandler(Mouse.MouseUpEvent, value, false);
            remove => RemoveHandler(Mouse.MouseUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseUp"/> routed event reaches an element in its route that 
        /// is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports that 
        /// the mouse button was released.
        /// </param>
        protected virtual void OnMouseUp(MouseButtonEventArgs e) { }

        #endregion

        #region MouseMove

        /// <summary>
        /// Identifies the <see cref="PreviewMouseMove"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseMoveEvent = Mouse.PreviewMouseMoveEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when the mouse pointer moves while the mouse pointer is over this element.
        /// </summary>
        public event MouseEventHandler PreviewMouseMove
        {
            add => AddHandler(Mouse.PreviewMouseMoveEvent, value, false);
            remove => RemoveHandler(Mouse.PreviewMouseMoveEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseMove"/> routed event reaches an element in its 
        /// route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnPreviewMouseMove(MouseEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseMove"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseMoveEvent = Mouse.MouseMoveEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is
        /// moved, while within this element.
        /// </summary>
        public event MouseEventHandler MouseMove
        {
            add => AddHandler(Mouse.MouseMoveEvent, value, false);
            remove => RemoveHandler(Mouse.MouseMoveEvent, value);
        }

        /// <summary>
        /// Raises the PointerMoved event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseMove(MouseEventArgs e) { }

        #endregion

        #region MouseLeftButtonDown

        /// <summary>
        /// Identifies the <see cref="PreviewMouseLeftButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseLeftButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PreviewMouseLeftButtonDown),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the left mouse button is pressed while the mouse pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseLeftButtonDown
        {
            add => AddHandler(PreviewMouseLeftButtonDownEvent, value, false);
            remove => RemoveHandler(PreviewMouseLeftButtonDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseLeftButtonDown"/> routed event reaches an 
        /// element in its route that is derived from this class. Implement this method to add class 
        /// handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the left mouse button was pressed.
        /// </param>
        protected virtual void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseLeftButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeftButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseLeftButtonDown),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is pressed, while 
        /// within this element.
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonDown
        {
            add => AddHandler(MouseLeftButtonDownEvent, value, false);
            remove => RemoveHandler(MouseLeftButtonDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseLeftButtonDown"/> routed event is raised on this 
        /// element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the left mouse button was pressed.
        /// </param>
        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }

        #endregion

        #region MouseRightButtonDown

        /// <summary>
        /// Identifies the <see cref="PreviewMouseRightButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseRightButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PreviewMouseRightButtonDown),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the right mouse button is pressed while the mouse pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseRightButtonDown
        {
            add => AddHandler(PreviewMouseRightButtonDownEvent, value, false);
            remove => RemoveHandler(PreviewMouseRightButtonDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseRightButtonDown"/> routed event reaches an 
        /// element in its route that is derived from this class. Implement this method to add class 
        /// handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the right mouse button was pressed.
        /// </param>
        protected virtual void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseRightButtonDownEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseRightButtonDown),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the right mouse button is pressed while the mouse pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler MouseRightButtonDown
        {
            add => AddHandler(MouseRightButtonDownEvent, value, false);
            remove => RemoveHandler(MouseRightButtonDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseRightButtonDown"/> routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for 
        /// this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the right mouse button was pressed.
        /// </param>
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e) { }

        #endregion

        #region MouseWheel

        /// <summary>
        /// Identifies the <see cref="PreviewMouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseWheelEvent = Mouse.MouseWheelEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over this element.
        /// </summary>
        public event MouseWheelEventHandler PreviewMouseWheel
        {
            add => AddHandler(Mouse.PreviewMouseWheelEvent, value, false);
            remove => RemoveHandler(Mouse.PreviewMouseWheelEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseWheel"/> routed event reaches an element in
        /// its route that is derived from this class. Implement this method to add class handling for 
        /// this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseWheelEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnPreviewMouseWheel(MouseWheelEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseWheel"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseWheelEvent = Mouse.MouseWheelEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when the user rotates the mouse wheel while the mouse pointer is over
        /// a <see cref="UIElement"/>, or the <see cref="UIElement"/> has focus.
        /// </summary>
        public event MouseWheelEventHandler MouseWheel
        {
            add => AddHandler(Mouse.MouseWheelEvent, value, false);
            remove => RemoveHandler(Mouse.MouseWheelEvent, value);
        }

        /// <summary>
        /// Raises the PointerWheelChanged event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseWheel(MouseWheelEventArgs e) { }

        #endregion

        #region MouseLeftButtonUp

        /// <summary>
        /// Identifies the <see cref="PreviewMouseLeftButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseLeftButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PreviewMouseLeftButtonUp),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse pointer is over this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseLeftButtonUp
        {
            add => AddHandler(PreviewMouseLeftButtonUpEvent, value, false);
            remove => RemoveHandler(PreviewMouseLeftButtonUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseLeftButtonUp"/> routed event reaches an 
        /// element in its route that is derived from this class. Implement this method to add class 
        /// handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports
        /// that the left mouse button was released.
        /// </param>
        protected virtual void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseLeftButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeftButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseLeftButtonUp),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the pointer device that previously initiated a Press action is released, 
        /// while within this element.
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonUp
        {
            add => AddHandler(MouseLeftButtonUpEvent, value, false);
            remove => RemoveHandler(MouseLeftButtonUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseLeftButtonUp"/> routed event reaches an 
        /// element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data 
        /// reports that the left mouse button was released.
        /// </param>
        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }

        #endregion

        #region MouseEnter

        /// <summary>
        /// Identifies the <see cref="MouseEnter"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseEnterEvent = Mouse.MouseEnterEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a pointer enters the hit test area of this element.
        /// </summary>
        public event MouseEventHandler MouseEnter
        {
            add => AddHandler(Mouse.MouseEnterEvent, value, false);
            remove => RemoveHandler(Mouse.MouseEnterEvent, value);
        }

        /// <summary>
        /// Raises the PointerEntered event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseEnter(MouseEventArgs e) { }

        #endregion

        #region MouseLeave

        /// <summary>
        /// Identifies the <see cref="MouseLeave"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseLeaveEvent = Mouse.MouseLeaveEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a pointer leaves the hit test area of this element.
        /// </summary>
        public event MouseEventHandler MouseLeave
        {
            add => AddHandler(Mouse.MouseLeaveEvent, value, false);
            remove => RemoveHandler(Mouse.MouseLeaveEvent, value);
        }

        /// <summary>
        /// Raises the PointerExited event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnMouseLeave(MouseEventArgs e) { }

        #endregion

        #region TextInputStart, TextInput, TextInputUpdate

        /// <summary>
        /// Identifies the <see cref="TextInputStart"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TextInputStartEvent =
            EventManager.RegisterCoreEvent(
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
            EventManager.RegisterCoreEvent(
                nameof(TextInput),
                RoutingStrategy.Bubble,
                typeof(TextCompositionEventHandler),
                typeof(UIElement));

        [NotImplemented]
        protected virtual void OnPreviewTextInput(TextCompositionEventArgs e) { }

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
            EventManager.RegisterCoreEvent(
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

        /// <summary>
        /// Called before the <see cref="TextInputUpdate"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TextCompositionEventArgs"/> that contains the event data
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnTextInputUpdate(TextCompositionEventArgs e) { }

        #endregion

        #region Tapped

        /// <summary>
        /// Identifies the <see cref="Tapped"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TappedEvent =
            EventManager.RegisterCoreEvent(
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

        #region MouseRightButtonUp

        /// <summary>
        /// Identifies the <see cref="PreviewMouseRightButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseRightButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PreviewMouseRightButtonUp),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when the right mouse button is released while the mouse pointer is over 
        /// this element.
        /// </summary>
        public event MouseButtonEventHandler PreviewMouseRightButtonUp
        {
            add => AddHandler(PreviewMouseRightButtonUpEvent, value, false);
            remove => RemoveHandler(PreviewMouseRightButtonUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewMouseRightButtonUp"/> routed event 
        /// reaches an element in its route that is derived from this class. Implement this 
        /// method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event 
        /// data reports that the right mouse button was released.
        /// </param>
        protected virtual void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="MouseRightButtonUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MouseRightButtonUpEvent =
            EventManager.RegisterRoutedEvent(
                nameof(MouseRightButtonUp),
                RoutingStrategy.Direct,
                typeof(MouseButtonEventHandler),
                typeof(UIElement));

        /// <summary>
        /// Occurs when a right-tap input stimulus happens while the pointer is over the element.
        /// </summary>
        public event MouseButtonEventHandler MouseRightButtonUp
        {
            add => AddHandler(MouseRightButtonUpEvent, value, false);
            remove => RemoveHandler(MouseRightButtonUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="MouseRightButtonUp"/> routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling 
        /// for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the right mouse button was released.
        /// </param>
        protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs e) { }

        #endregion

        #region KeyDown event

        /// <summary>
        /// Identifies the <see cref="PreviewKeyDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewKeyDownEvent = Keyboard.PreviewKeyDownEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a key is pressed while focus is on this element.
        /// </summary>
        public event KeyEventHandler PreviewKeyDown
        {
            add => AddHandler(Keyboard.PreviewKeyDownEvent, value, false);
            remove => RemoveHandler(Keyboard.PreviewKeyDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="PreviewKeyDown"/> attached event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling 
        /// for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnPreviewKeyDown(KeyEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="KeyDown"/> routed event.
        /// </summary>
        public static readonly RoutedEvent KeyDownEvent = Keyboard.KeyDownEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a key is pressed while focus is on this element.
        /// </summary>
        public event KeyEventHandler KeyDown
        {
            add => AddHandler(Keyboard.KeyDownEvent, value, false);
            remove => RemoveHandler(Keyboard.KeyDownEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled Keyboard.KeyDown attached event reaches an element in its route that 
        /// is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnKeyDown(KeyEventArgs e) { }

        #endregion

        #region KeyUp event

        /// <summary>
        /// Identifies the <see cref="PreviewKeyUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewKeyUpEvent = Keyboard.PreviewKeyUpEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a key is released while focus is on this element.
        /// </summary>
        public event KeyEventHandler PreviewKeyUp
        {
            add => AddHandler(Keyboard.PreviewKeyUpEvent, value, false);
            remove => RemoveHandler(Keyboard.PreviewKeyUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled Keyboard.PreviewKeyUp attached event reaches an element in its route 
        /// that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnPreviewKeyUp(KeyEventArgs e) { }

        /// <summary>
        /// Identifies the <see cref="KeyUp"/> routed event.
        /// </summary>
        public static readonly RoutedEvent KeyUpEvent = Keyboard.KeyUpEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when a key is released while focus is on this element.
        /// </summary>
        public event KeyEventHandler KeyUp
        {
            add => AddHandler(Keyboard.KeyUpEvent, value, false);
            remove => RemoveHandler(Keyboard.KeyUpEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled Keyboard.KeyUp attached event reaches an element in its route that 
        /// is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnKeyUp(KeyEventArgs e) { }

        #endregion

        #region GotFocus event

        /// <summary>
        /// Identifies the <see cref="GotFocus"/> routed event.
        /// </summary>
        public static readonly RoutedEvent GotFocusEvent =
            EventManager.RegisterCoreEvent(
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
            EventManager.RegisterCoreEvent(
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

        #region GotMouseCapture

        /// <summary>
        /// Identifies the <see cref="GotMouseCapture"/> routed event.
        /// </summary>
        public static readonly RoutedEvent GotMouseCaptureEvent = Mouse.GotMouseCaptureEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when this element captures the mouse.
        /// </summary>
        public event MouseEventHandler GotMouseCapture
        {
            add => AddHandler(GotMouseCaptureEvent, value, false);
            remove => RemoveHandler(GotMouseCaptureEvent, value);
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.GotMouseCapture attached event reaches an element in its route 
        /// that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnGotMouseCapture(MouseEventArgs e) { }

        #endregion

        #region LostMouseCapture

        /// <summary>
        /// Identifies the <see cref="LostMouseCapture"/> routed event.
        /// </summary>
        public static readonly RoutedEvent LostMouseCaptureEvent = Mouse.LostMouseCaptureEvent.AddOwner(typeof(UIElement));

        /// <summary>
        /// Occurs when the <see cref="UIElement"/> loses mouse capture.
        /// </summary>
        public event MouseEventHandler LostMouseCapture
        {
            add => AddHandler(Mouse.LostMouseCaptureEvent, value, false);
            remove => RemoveHandler(Mouse.LostMouseCaptureEvent, value);
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

        internal virtual HtmlElementReference GetFocusTarget() => OuterDiv;

        public virtual void INTERNAL_AttachToDomEvents() { }

        public virtual void INTERNAL_DetachFromDomEvents() { }

        internal virtual UIElement MouseTarget => this;

        internal virtual UIElement KeyboardTarget => this;
    }
}