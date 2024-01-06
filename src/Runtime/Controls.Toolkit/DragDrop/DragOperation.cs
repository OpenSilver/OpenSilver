// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using SW = Microsoft.Windows;

namespace Microsoft.Windows
{
    /// <summary>
    /// An asynchronous drag operation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal class DragOperation : IObservable<SW.DragDropEffects>
    {
        /// <summary>
        /// The interval at which to pulse DragOver events when the mouse
        /// isn't moving.
        /// </summary>
        private const int MouseOverPulseIntervalInMilliseconds = 250;

        /// <summary>
        /// Gets or sets a value indicating whether a drag operation is in 
        /// process.
        /// </summary>
        private static bool IsDragging { get; set; }

        /// <summary>
        /// Information retrieved when the drag operation began.
        /// </summary>
        private SW.DragEventArgs _dragStartEventArgs;

        /// <summary>
        /// The effects authorized by the drag source.
        /// </summary>
        private SW.DragDropEffects _allowedEffects = SW.DragDropEffects.None;

        /// <summary>
        /// Information about the last drag event.
        /// </summary>
        private SW.DragEventArgs _lastDragEventArgs = null;

        /// <summary>
        /// Information about the last give feedback event.
        /// </summary>
        private SW.GiveFeedbackEventArgs _lastGiveFeedbackEventArgs;

        private DragStartingHelper _dragStartingHelper;
        private DragStartedHelper _dragStartedHelper;
        private IObserver<SW.DragDropEffects> _observer;

        /// <summary>
        /// The source of the data being dragged.
        /// </summary>
        private DependencyObject _dragSource;

        /// <summary>
        /// The state of the input keys relevant to drag operations.
        /// </summary>
        private SW.DragDropKeyStates _keyStates;

        /// <summary>
        /// The element currently being dragged over.
        /// </summary>
        private DependencyObject _currentDragOverElement;

        /// <summary>
        /// A value indicating whether the escape key is pressed.
        /// </summary>
        private bool _escapePressed;

        /// <summary>
        /// Gets or sets the state of the input keys relevant to drag 
        /// operations.
        /// </summary>
        private SW.DragDropKeyStates KeyStates
        {
            get
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    _keyStates |= SW.DragDropKeyStates.ControlKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.ControlKey;
                }
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    _keyStates |= SW.DragDropKeyStates.ShiftKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.ShiftKey;
                }
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    _keyStates |= SW.DragDropKeyStates.AltKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.AltKey;
                }

                return _keyStates;
            }
            set
            {
                _keyStates = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DragOperation class.
        /// </summary>
        /// <param name="dragSource">The source of the drag operation.</param>
        /// <param name="data">The data associated with the drag operation.
        /// </param>
        /// <param name="allowedEffects">The allowed effects of the drag 
        /// operation.
        /// </param>
        /// <param name="initialKeyState">The initial state of the keys relevant 
        /// to drag operations.</param>
        public DragOperation(DependencyObject dragSource, object data, SW.DragDropEffects allowedEffects, SW.DragDropKeyStates initialKeyState)
        {
            SW.IDataObject dataObject = data as SW.IDataObject;
            if (dataObject == null)
            {
                dataObject = new DataObject(data);
            }

            _allowedEffects = allowedEffects;
            KeyStates = initialKeyState;

            _dragSource = dragSource;
            SW.DragEventArgs dragStartEventArgs =
                new SW.DragEventArgs()
                {
                    OriginalSource = dragSource,
                    AllowedEffects = allowedEffects,
                    Effects = allowedEffects,
                    Data = dataObject
                };

            _lastDragEventArgs = dragStartEventArgs;
            dragStartEventArgs.AllowedEffects = allowedEffects;
            dragStartEventArgs.Data = dataObject;

            this._dragStartEventArgs = dragStartEventArgs;
        }

        /// <summary>
        /// This method starts the drag operation and sends the results to 
        /// an observer.
        /// </summary>
        /// <param name="observer">The observer listening for the result of
        /// the drag operation.</param>
        /// <returns>A disposable object used to detach from a drag operation.
        /// </returns>
        public IDisposable Subscribe(IObserver<SW.DragDropEffects> observer)
        {
            if (IsDragging)
            {
                throw new InvalidOperationException("A drag operation is already in progress.");
            }
            IsDragging = true;

            OnDragStarting(_dragStartEventArgs);

            _observer = observer;

            return EmptyDisposable.Instance;
        }

        private void OnDragStarting(SW.DragEventArgs eventArgs)
        {
            // Always execute a QueryContinueDrag first.
            SW.QueryContinueDragEventArgs args = OnDragSourceQueryContinueDrag();
            if (!(args.Handled && args.Action == SW.DragAction.Cancel))
            {
                OnDragSourceGiveFeedback(_lastDragEventArgs);
                OnDragStarted(eventArgs);
            }

            if (Application.Current != null)
            {
                _dragStartingHelper = new DragStartingHelper(this);
            }
        }

        private void OnDragStarted(SW.DragEventArgs eventArgs)
        {
            _dragStartedHelper = new DragStartedHelper(this);
        }

        private void OnDragSourceQueryContinueDrag(SW.QueryContinueDragEventArgs queryContinueDragEventArgs)
        {
            if (queryContinueDragEventArgs.Handled && (queryContinueDragEventArgs.Action == SW.DragAction.Drop || queryContinueDragEventArgs.Action == SW.DragAction.Cancel))
            {
                if (queryContinueDragEventArgs.Action == SW.DragAction.Drop)
                {
                    OnTargetDrop();
                }
                else if (queryContinueDragEventArgs.Action == SW.DragAction.Cancel)
                {
                    OnCancel();
                }
            }
        }

        private void OnDragCompleted(SW.DragDropEffects effects)
        {
            IsDragging = false;

            _dragStartedHelper?.Dispose();
            _dragStartedHelper = null;

            _dragStartingHelper?.Dispose();
            _dragStartingHelper = null;

            _observer?.OnNext(effects);
            _observer = null;
        }

        /// <summary>
        /// Retrieves the original source of a new DragOver event.  Attempts to
        /// determine the original source by finding the the deepest element
        /// in the tree that the mouse is over.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The original source of a new DragOver event.</returns>
        private static UIElement GetDragOverOriginalSource(SW.DragEventArgs args)
        {
            UIElement originalSource = args.OriginalSource as UIElement;
            // Use the previous original source and go to its root.
            // Note: this won't work if a popup appears on top of the
            // original source but it will work if the previous original source
            // is inside of a popup.
            UIElement rootVisual = originalSource.GetVisualAncestors().OfType<UIElement>().LastOrDefault();
            if (rootVisual != null)
            {
                // If the original source disappears (ex. a popup disappears),
                // use the root visual.
                rootVisual = Application.Current.RootVisual;

                originalSource =
                    FunctionalProgramming.TraverseBreadthFirst(
                        rootVisual,
                        node => node.GetVisualChildren().OfType<UIElement>(),
                        node => new Rect(new Point(0, 0), node.GetSize()).Contains(args.GetPosition(node)))
                        .LastOrDefault();
            }
            return originalSource;
        }

        /// <summary>
        /// Raises a routed drag event and stores information about the drag
        /// event.
        /// </summary>
        /// <param name="element">An element that accepts a drop.</param>
        /// <param name="eventArgs">Information about the drag event.</param>
        /// <param name="raiseAction">An action that raises the specific drag event.
        /// </param>
        /// <returns>Information about the drag event that may have been
        /// modified by handlers.</returns>
        private SW.DragEventArgs RaiseRoutedDragEvent(DependencyObject element, SW.DragEventArgs eventArgs, Action<DependencyObject, SW.DragEventArgs> raiseAction)
        {
            SW.DragEventArgs dragEventArgs =
                new SW.DragEventArgs(eventArgs)
                {
                    Effects = eventArgs.AllowedEffects,
                    OriginalSource = element
                };

            if ((bool)element.GetValue(UIElement.AllowDropProperty))
            {
                element.RaiseRoutedEvent(
                    dragEventArgs,
                    (obj, args) =>
                    {
                        raiseAction(obj, args);
                    });
            }
            else
            {
                dragEventArgs.Effects = DragDropEffects.None;
                dragEventArgs.Handled = true;
            }

            _lastDragEventArgs = dragEventArgs;

            return dragEventArgs;
        }

        /// <summary>
        /// Raises the GiveFeedback and QueryContinueDrag events on the drag
        /// source.
        /// </summary>
        /// <param name="args">Information about the last drag event.</param>
        private void RaiseDragSourceEvents(SW.DragEventArgs args)
        {
            OnDragSourceGiveFeedback(args);

            OnDragSourceQueryContinueDrag();
        }

        /// <summary>
        /// Returns the allowed effects by analyzing the state of the keys 
        /// pressed.  If the control key is pressed the user is requesting a 
        /// copy. If copy is available the effect will be only copy, if not 
        /// available the effect will be None.
        /// </summary>
        /// <param name="allowedEffects">The allowed effects.</param>
        /// <returns>The effects of the drag operation.</returns>
        private SW.DragDropEffects GetDragDropEffects(SW.DragDropEffects allowedEffects)
        {
            if ((KeyStates & SW.DragDropKeyStates.ControlKey) == SW.DragDropKeyStates.ControlKey)
            {
                if ((KeyStates & SW.DragDropKeyStates.ShiftKey) == SW.DragDropKeyStates.ShiftKey)
                {
                    if ((allowedEffects & SW.DragDropEffects.Link) == SW.DragDropEffects.Link)
                    {
                        return SW.DragDropEffects.Link;
                    }
                    else
                    {
                        return SW.DragDropEffects.None;
                    }
                }
                else if ((allowedEffects & SW.DragDropEffects.Copy) == SW.DragDropEffects.Copy)
                {
                    return SW.DragDropEffects.Copy;
                }
                else
                {
                    return SW.DragDropEffects.None;
                }
            }
            else if ((allowedEffects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
            {
                return SW.DragDropEffects.Move;
            }
            else
            {
                return allowedEffects;
            }
        }

        /// <summary>
        /// Raises the GiveFeedback event on the drag source.
        /// </summary>
        /// <param name="args">Information about the GiveFeedback event.
        /// </param>
        private void OnDragSourceGiveFeedback(SW.DragEventArgs args)
        {
            SW.DragDropEffects effects = _allowedEffects;

            if (args.Handled)
            {
                effects = _allowedEffects & args.Effects;
            }
            else
            {
                effects = GetDragDropEffects(_allowedEffects);
            }

            _lastGiveFeedbackEventArgs =
                new SW.GiveFeedbackEventArgs()
                {
                    Effects = effects,
                    OriginalSource = _dragSource,
                };

            _dragSource.RaiseRoutedEvent(
                _lastGiveFeedbackEventArgs,
                (accept, e) => accept.OnGiveFeedback(e));
        }

        /// <summary>
        /// Raises the QueryContinueDragEvent on the drag source.
        /// </summary>
        /// <returns>Information about the QueryContinueDrag event.</returns>
        private SW.QueryContinueDragEventArgs OnDragSourceQueryContinueDrag()
        {
            SW.QueryContinueDragEventArgs queryContinueDragEventArgs =
                new SW.QueryContinueDragEventArgs()
                {
                    Action = SW.DragAction.Continue,
                    EscapePressed = _escapePressed,
                    KeyStates = KeyStates,
                    OriginalSource = _dragSource
                };

            DependencyObject dragSource = _dragSource;
            if (dragSource != null)
            {
                queryContinueDragEventArgs =
                    dragSource.RaiseRoutedEvent(
                        queryContinueDragEventArgs,
                        (acc, e) => acc.OnQueryContinueDrag(e));
            }

            if (!queryContinueDragEventArgs.Handled)
            {
                if (queryContinueDragEventArgs.EscapePressed)
                {
                    queryContinueDragEventArgs.Action = SW.DragAction.Cancel;
                    queryContinueDragEventArgs.Handled = true;
                }
                else if ((queryContinueDragEventArgs.KeyStates & SW.DragDropKeyStates.LeftMouseButton) != SW.DragDropKeyStates.LeftMouseButton)
                {
                    queryContinueDragEventArgs.Action = (_lastGiveFeedbackEventArgs == null || _lastGiveFeedbackEventArgs.Effects != SW.DragDropEffects.None) ? SW.DragAction.Drop : SW.DragAction.Cancel;
                    queryContinueDragEventArgs.Handled = true;
                }
            }

            OnDragSourceQueryContinueDrag(queryContinueDragEventArgs);

            return queryContinueDragEventArgs;
        }

        /// <summary>
        /// This method is invoked when the drag operation is cancelled.
        /// </summary>
        private void OnCancel()
        {
            OnDragLeave();
            OnDragCompleted(SW.DragDropEffects.None);
        }

        /// <summary>
        /// This method raises the Drop event.
        /// </summary>
        private void OnTargetDrop()
        {
            UIElement dropTarget = _lastDragEventArgs.OriginalSource as UIElement;
            if (dropTarget != null && dropTarget.AllowDrop)
            {
                SW.DragEventArgs dropEventArgs =
                    RaiseRoutedDragEvent(dropTarget, _lastDragEventArgs, (acc, ev) => acc.OnDrop(ev));

                // Regardless of whether event args is handled and what the allowed effects are
                // WPF drag and drop always returns the effects after a drop event.
                OnDragCompleted(dropEventArgs.Effects);
            }
            else
            {
                OnDragCompleted(SW.DragDropEffects.None);
            }
        }

        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        private void OnDragEnter(object sender, SW.DragEventArgs eventArgs)
        {
            OnDragLeave();

            _currentDragOverElement = eventArgs.OriginalSource as DependencyObject;
            DependencyObject acceptDrop = (DependencyObject)sender;
            eventArgs =
                RaiseRoutedDragEvent(
                    acceptDrop,
                    eventArgs,
                    (ancestor, args) => ancestor.OnDragEnter(args));

            RaiseDragSourceEvents(eventArgs);
        }

        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        private void OnDragOver(object sender, SW.DragEventArgs eventArgs)
        {
            UIElement target = eventArgs.OriginalSource as UIElement;
            if (_currentDragOverElement != target)
            {
                OnDragEnter(sender, new SW.DragEventArgs(eventArgs));
            }

            DependencyObject acceptDrop = (DependencyObject)sender;
            eventArgs =
               RaiseRoutedDragEvent(
                   acceptDrop,
                   eventArgs,
                   (ancestor, args) => ancestor.OnDragOver(args));

            RaiseDragSourceEvents(eventArgs);
        }

        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        private void OnDragLeave()
        {
            if (_currentDragOverElement != null)
            {
                SW.DragEventArgs eventArgs = new SW.DragEventArgs(_lastDragEventArgs) { OriginalSource = _currentDragOverElement };

                DependencyObject acceptDrop = (DependencyObject)_currentDragOverElement;
                eventArgs =
                    RaiseRoutedDragEvent(
                        acceptDrop,
                        eventArgs,
                        (ancestor, args) => ancestor.OnDragLeave(args));
            }
        }

        private sealed class EmptyDisposable : IDisposable
        {
            private EmptyDisposable() { }

            public static EmptyDisposable Instance { get; } = new EmptyDisposable();

            public void Dispose() { }
        }

        private sealed class DragStartingHelper : IDisposable
        {
            private readonly DragOperation _owner;

            private readonly UIElement _rootVisual;
            private readonly UIElement[] _popupRoots;

            private DragDropKeyStates _states;

            public DragStartingHelper(DragOperation owner)
            {
                _owner = owner;
                _states = _owner.KeyStates;

                _rootVisual = Application.Current.RootVisual;
                _popupRoots = VisualTreeExtensions.GetVisualDescendants(_rootVisual)
                    .OfType<Popup>()
                    .Select(popup => popup.Child)
                    .Where(popupRoot => popupRoot != null)
                    .ToArray();

                SubscribeToEvents();
            }

            public void Dispose() => UnSubscribeFromEvents();

            private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                _states |= DragDropKeyStates.LeftMouseButton;
                _owner.KeyStates = _states;
                RaiseDragSourceEvents();
            }

            private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                _states &= ~DragDropKeyStates.LeftMouseButton;
                _owner.KeyStates = _states;
                RaiseDragSourceEvents();
            }

            private void OnKeyDown(object sender, KeyEventArgs e)
            {
                _states |= UIElementExtensions.ToDragDropKeyStates(e.Key);
                _owner.KeyStates = _states;
                if (e.Key == Key.Escape)
                {
                    _owner._escapePressed = true;
                }
                RaiseDragSourceEvents();
            }

            private void OnKeyUp(object sender, KeyEventArgs e)
            {
                _states &= ~UIElementExtensions.ToDragDropKeyStates(e.Key);
                _owner.KeyStates = _states;
                if (e.Key == Key.Escape)
                {
                    _owner._escapePressed = false;
                }
                RaiseDragSourceEvents();
            }

            private void RaiseDragSourceEvents() => _owner.RaiseDragSourceEvents(_owner._lastDragEventArgs);

            private void SubscribeToEvents()
            {
                _rootVisual.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                _rootVisual.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
                _rootVisual.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
                _rootVisual.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
                foreach (UIElement popupRoot in _popupRoots)
                {
                    popupRoot.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                    popupRoot.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
                    popupRoot.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
                    popupRoot.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
                }
            }

            private void UnSubscribeFromEvents()
            {
                _rootVisual.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
                _rootVisual.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp));
                _rootVisual.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnKeyDown));
                _rootVisual.RemoveHandler(UIElement.KeyUpEvent, new KeyEventHandler(OnKeyUp));
                foreach (UIElement popupRoot in _popupRoots)
                {
                    popupRoot.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
                    popupRoot.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp));
                    popupRoot.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnKeyDown));
                    popupRoot.RemoveHandler(UIElement.KeyUpEvent, new KeyEventHandler(OnKeyUp));
                }
            }
        }

        private sealed class DragStartedHelper : IDisposable
        {
            private readonly DragOperation _owner;
            private readonly DispatcherTimer _timer;

            private readonly UIElement _rootVisual;
            private readonly UIElement[] _popupRoots;

            private SW.DragEventArgs _dragEventArgs;

            public DragStartedHelper(DragOperation owner)
            {
                _owner = owner;

                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(MouseOverPulseIntervalInMilliseconds),
                };
                _timer.Tick += new EventHandler(OnTimerTick);

                _rootVisual = Application.Current.RootVisual;
                _popupRoots = VisualTreeExtensions.GetVisualDescendants(_rootVisual)
                    .OfType<Popup>()
                    .Select(popup => popup.Child)
                    .Where(popupRoot => popupRoot != null)
                    .ToArray();

                SubscribeToEvents();
            }

            public void Dispose()
            {
                UnSubscribeFromEvents();

                _timer.Stop();
            }

            private void OnTimerTick(object sender, EventArgs e)
            {
                UIElement originalSource = _dragEventArgs.OriginalSource as UIElement;

                if (originalSource != null)
                {
                    Point pointWithinOriginalSource = _dragEventArgs.GetPosition((UIElement)_dragEventArgs.OriginalSource);

                    if (!new Rect(new Point(0, 0), originalSource.GetSize()).Contains(pointWithinOriginalSource))
                    {
                        originalSource = GetDragOverOriginalSource(_dragEventArgs);
                    }
                }

                if (IsDragging)
                {
                    _owner.OnDragOver(originalSource, new SW.DragEventArgs(_dragEventArgs) { OriginalSource = originalSource });
                }
            }

            private void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (e.OriginalSource != null)
                {
                    _dragEventArgs = new SW.DragEventArgs(_owner._dragStartEventArgs)
                    {
                        OriginalSource = e.OriginalSource,
                        MouseEventArgs = e,
                    };

                    UIElement originalSource = _dragEventArgs.OriginalSource as UIElement;

                    if (originalSource != null)
                    {
                        Point pointWithinOriginalSource = _dragEventArgs.GetPosition((UIElement)_dragEventArgs.OriginalSource);

                        if (!new Rect(new Point(0, 0), originalSource.GetSize()).Contains(pointWithinOriginalSource))
                        {
                            originalSource = GetDragOverOriginalSource(_dragEventArgs);
                        }
                    }

                    if (IsDragging)
                    {
                        _owner.OnDragOver(originalSource, new SW.DragEventArgs(_dragEventArgs) { OriginalSource = originalSource });
                    }

                    _timer.Stop();
                    _timer.Start();
                }
            }

            private void SubscribeToEvents()
            {
                _rootVisual.MouseMove += new MouseEventHandler(OnMouseMove);
                foreach (UIElement popupRoot in _popupRoots)
                {
                    popupRoot.MouseMove += new MouseEventHandler(OnMouseMove);
                }
            }

            private void UnSubscribeFromEvents()
            {
                _rootVisual.MouseMove -= new MouseEventHandler(OnMouseMove);
                foreach (UIElement popupRoot in _popupRoots)
                {
                    popupRoot.MouseMove -= new MouseEventHandler(OnMouseMove);
                }
            }
        }
    }
}