// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Windows
{
    /// <summary>
    /// Provides helper methods and fields for initiating drag-and-drop operations,
    /// including a method to begin a drag-and-drop operation, and facilities for
    /// adding and removing drag-and-drop related event handlers.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class DragDrop
    {
        /// <summary>
        /// Identifies the System.Windows.DragDrop.DragEnter attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent DragEnterEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Identifies the System.Windows.UIElement.DragLeave attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent DragLeaveEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Identifies the System.Windows.UIElement.DragOver attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent DragOverEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Identifies the System.Windows.UIElement.Drop attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent DropEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Identifies the System.Windows.UIElement.GiveFeedback attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent GiveFeedbackEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Identifies the System.Windows.UIElement.QueryContinueDrag attached event.
        /// </summary>
        public static readonly ExtendedRoutedEvent QueryContinueDragEvent = new ExtendedRoutedEvent();

        /// <summary>
        /// Gets a value indicating whether a drag is in progress.
        /// </summary>
        public static bool IsDragInProgress { get { return _dragOperationInProgress != null; } }

        /// <summary>
        /// The drag operation in progress.
        /// </summary>
        private static DragOperation _dragOperationInProgress;

        /// <summary>
        /// An event that is raised when a drag operation is completed.
        /// </summary>
        public static event EventHandler<DragDropCompletedEventArgs> DragDropCompleted;

        /// <summary>
        /// Raises the DragCompleted event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        private static void OnDragCompleted(DragDropCompletedEventArgs args)
        {
            _dragOperationInProgress = null;
            EventHandler<DragDropCompletedEventArgs> handler = DragDropCompleted;
            if (handler != null)
            {
                handler(null, args);
            }
        }

        #region public attached DragEventHandler DragEnterHandler
        /// <summary>
        /// Removes a handler from the attached DragEnter event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveDragEnterHandler(DependencyObject element, DragEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = element.GetDragEnterHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragEnter event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the handler if the event is handled.</param>
        internal static void AddDragEnterHandler(DependencyObject element, DragEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> originalHandlers = element.GetDragEnterHandlers();
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(DragEnterHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragEnter event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddDragEnterHandler(DependencyObject element, DragEventHandler handler)
        {
            AddDragEnterHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the drag enter handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> GetDragEnterHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>)element.GetValue(DragEnterHandlerProperty);
        }

        /// <summary>
        /// Identifies the DragEnterHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty DragEnterHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DragEnterHandler",
                typeof(ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached DragEventHandler DragEnterHandler

        #region public attached DragEventHandler DragOverHandler
        /// <summary>
        /// Removes a handler from the attached DragOver event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveDragOverHandler(DependencyObject element, DragEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = element.GetDragOverHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragOver event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the handler if the event is handled.</param>
        internal static void AddDragOverHandler(DependencyObject element, DragEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> originalHandlers = element.GetDragOverHandlers();
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(DragOverHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragOver event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddDragOverHandler(DependencyObject element, DragEventHandler handler)
        {
            AddDragOverHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the drag Over handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> GetDragOverHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>)element.GetValue(DragOverHandlerProperty);
        }

        /// <summary>
        /// Identifies the DragOverHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty DragOverHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DragOverHandler",
                typeof(ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached DragEventHandler DragOverHandler

        #region public attached DragEventHandler DragLeaveHandler
        /// <summary>
        /// Removes a handler from the attached DragLeave event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveDragLeaveHandler(DependencyObject element, DragEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = element.GetDragLeaveHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragLeave event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the handler if the event is handled.</param>
        internal static void AddDragLeaveHandler(DependencyObject element, DragEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> originalHandlers = element.GetDragLeaveHandlers();
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(DragLeaveHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached DragLeave event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddDragLeaveHandler(DependencyObject element, DragEventHandler handler)
        {
            AddDragLeaveHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the drag Leave handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> GetDragLeaveHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>)element.GetValue(DragLeaveHandlerProperty);
        }

        /// <summary>
        /// Identifies the DragLeaveHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty DragLeaveHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DragLeaveHandler",
                typeof(ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached DragEventHandler DragLeaveHandler

        #region public attached DragEventHandler DropHandler
        /// <summary>
        /// Removes a handler from the attached Drop event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveDropHandler(DependencyObject element, DragEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = element.GetDropHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached Drop event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the handler if the event is handled.</param>
        internal static void AddDropHandler(DependencyObject element, DragEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> originalHandlers = element.GetDropHandlers();
            ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(DropHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached Drop event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddDropHandler(DependencyObject element, DragEventHandler handler)
        {
            AddDropHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the drag Leave handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs> GetDropHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>)element.GetValue(DropHandlerProperty);
        }

        /// <summary>
        /// Identifies the DropHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DropHandler",
                typeof(ExtendedRoutedEventHandlerCollection<DragEventHandler, DragEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached DragEventHandler DropHandler

        #region public attached GiveFeedbackEventHandler GiveFeedbackHandler
        /// <summary>
        /// Removes a handler from the attached GiveFeedback event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveGiveFeedbackHandler(DependencyObject element, GiveFeedbackEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs> handlers = element.GetGiveFeedbackHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached GiveFeedback event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the 
        /// handler if the event has been handled.</param>
        internal static void AddGiveFeedbackHandler(DependencyObject element, GiveFeedbackEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs> originalHandlers = element.GetGiveFeedbackHandlers();
            ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(GiveFeedbackHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached GiveFeedback event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddGiveFeedbackHandler(DependencyObject element, GiveFeedbackEventHandler handler)
        {
            AddGiveFeedbackHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the GiveFeedback Leave handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs> GetGiveFeedbackHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs>)element.GetValue(GiveFeedbackHandlerProperty);
        }

        /// <summary>
        /// Identifies the GiveFeedbackHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty GiveFeedbackHandlerProperty =
            DependencyProperty.RegisterAttached(
                "GiveFeedbackHandler",
                typeof(ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, GiveFeedbackEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached GiveFeedbackEventHandler GiveFeedbackHandler

        #region public attached QueryContinueDragEventHandler QueryContinueDragHandler
        /// <summary>
        /// Removes a handler from the attached QueryContinueDrag event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void RemoveQueryContinueDragHandler(DependencyObject element, QueryContinueDragEventHandler handler)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs> handlers = element.GetQueryContinueDragHandlers();
            if (handlers != null)
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Adds a handler to the attached QueryContinueDrag event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        /// <param name="handledEventsToo">A value Indicating whether to invoke the 
        /// handler if the event has been handled.</param>
        internal static void AddQueryContinueDragHandler(DependencyObject element, QueryContinueDragEventHandler handler, bool handledEventsToo)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs> originalHandlers = element.GetQueryContinueDragHandlers();
            ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs> handlers = originalHandlers ?? new ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs>((h, a) => h(element, a));
            handlers.Add(handler, handledEventsToo);
            if (handlers != originalHandlers)
            {
                element.SetValue(QueryContinueDragHandlerProperty, handlers);
            }
        }

        /// <summary>
        /// Adds a handler to the attached QueryContinueDrag event.
        /// </summary>
        /// <param name="element">The DependencyObject to attach an event handler for.</param>
        /// <param name="handler">The event handler.</param>
        internal static void AddQueryContinueDragHandler(DependencyObject element, QueryContinueDragEventHandler handler)
        {
            AddQueryContinueDragHandler(element, handler, false);
        }

        /// <summary>
        /// Gets the QueryContinueDrag Leave handler.
        /// </summary>
        /// <param name="element">The element to attach the event handler to.</param>
        /// <returns>The event handler.</returns>
        internal static ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs> GetQueryContinueDragHandlers(this DependencyObject element)
        {
            return (ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs>)element.GetValue(QueryContinueDragHandlerProperty);
        }

        /// <summary>
        /// Identifies the QueryContinueDragHandler dependency property.
        /// </summary>
        internal static readonly DependencyProperty QueryContinueDragHandlerProperty =
            DependencyProperty.RegisterAttached(
                "QueryContinueDragHandler",
                typeof(ExtendedRoutedEventHandlerCollection<QueryContinueDragEventHandler, QueryContinueDragEventArgs>),
                typeof(DragDrop),
                new PropertyMetadata(null));
        #endregion public attached QueryContinueDragEventHandler QueryContinueDragHandler

        /// <summary>
        /// Initiates a drag-and-drop operation.
        /// </summary>
        /// <param name="dragSource">A reference to the dependency object that is the source of the data being
        /// dragged.</param>
        /// <param name="data">A data object that contains the data being dragged.</param>
        /// <param name="allowedEffects">One of the System.Windows.DragDropEffects values that specifies permitted
        /// effects of the drag-and-drop operation.</param>
        /// <param name="initialKeyState">The initial key state when the drag operation begins.</param>
        public static void DoDragDrop(DependencyObject dragSource, object data, DragDropEffects allowedEffects, DragDropKeyStates initialKeyState)
        {
            // TODO: Throw if IsDragDropInProgress
            _dragOperationInProgress =
                new DragOperation(dragSource, data, allowedEffects, initialKeyState);

            _dragOperationInProgress
                 .Subscribe(new Observable<DragDropEffects>(
                     effects => OnDragCompleted(new DragDropCompletedEventArgs { Effects = effects })));
        }

        private sealed class Observable<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;

            public Observable(Action<T> onNext)
            {
                Debug.Assert(onNext != null);
                _onNext = onNext;
            }

            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(T value) => _onNext?.Invoke(value);
        }
    }
}