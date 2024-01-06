// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using SW = Microsoft.Windows;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows
{
    /// <summary>
    /// This class contains extension methods that mimic the 
    /// AddHandler/RemoveHandler methods for routed events.
    /// </summary>
    /// <remarks>
    /// These methods are in the System.Windows namespace so that they can be
    /// used as extension methods without bringing the Microsoft.Windows
    /// namespace into the scope.
    /// </remarks>
    public static class UIElementExtendedRoutedEventExtensions
    {
        /// <summary>
        /// A list of all the target events.
        /// </summary>
        private static List<SW.ExtendedRoutedEvent> targetEvents =
            new List<SW.ExtendedRoutedEvent>
                {
                    SW.DragDrop.DragEnterEvent,
                    SW.DragDrop.DragOverEvent,
                    SW.DragDrop.DragLeaveEvent,
                    SW.DragDrop.DropEvent,
                };

        /// <summary>
        /// Adds a routed event handler for a specified routed event, adding the handler
        /// to the handler collection on the current element.
        /// </summary>
        /// <param name="that">The element to add a handler for.</param>
        /// <param name="routedEvent">An identifier for the routed event to be handled.</param>
        /// <param name="handler">A reference to the handler implementation.</param>
        /// <param name="handledEventsToo">True to register the handler such that it is invoked even when the routed
        /// event is marked handled in its event data; false to register the handler
        /// with the default condition that it will not be invoked if the routed event
        /// is already marked handled. The default is false.</param>
        public static void AddHandler(this UIElement that, SW.ExtendedRoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            SW.DragEventHandler dragEventHandler = handler as SW.DragEventHandler;
            if (targetEvents.Contains(routedEvent) && dragEventHandler == null)
            {
                throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeDragEventHandler);
            }

            if (routedEvent == SW.DragDrop.DragEnterEvent)
            {
                SW.DragDrop.AddDragEnterHandler(that, dragEventHandler, handledEventsToo);
            }
            else if (routedEvent == SW.DragDrop.DragOverEvent)
            {
                SW.DragDrop.AddDragOverHandler(that, dragEventHandler, handledEventsToo);
            }
            else if (routedEvent == SW.DragDrop.DragLeaveEvent)
            {
                SW.DragDrop.AddDragLeaveHandler(that, dragEventHandler, handledEventsToo);
            }
            else if (routedEvent == SW.DragDrop.DropEvent)
            {
                SW.DragDrop.AddDropHandler(that, dragEventHandler, handledEventsToo);
            }
            else if (routedEvent == SW.DragDrop.GiveFeedbackEvent)
            {
                SW.GiveFeedbackEventHandler giveFeedbackHandler = handler as SW.GiveFeedbackEventHandler;
                if (giveFeedbackHandler == null)
                {
                    throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeGiveFeedbackEventHandler);
                }
                SW.DragDrop.AddGiveFeedbackHandler(that, giveFeedbackHandler, handledEventsToo);
            }
            else if (routedEvent == SW.DragDrop.QueryContinueDragEvent)
            {
                SW.QueryContinueDragEventHandler queryContinueFeedbackHandler = handler as SW.QueryContinueDragEventHandler;
                if (queryContinueFeedbackHandler == null)
                {
                    throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeQueryContinueDragEventHandler);
                }
                SW.DragDrop.AddQueryContinueDragHandler(that, queryContinueFeedbackHandler, handledEventsToo);
            }
        }

        /// <summary>
        /// Removes a routed event handler for a specified routed event, removing the handler
        /// from the handler collection on the current element.
        /// </summary>
        /// <param name="that">The element to remove the handler from.</param>
        /// <param name="routedEvent">An identifier for the routed event to be handled.</param>
        /// <param name="handler">A reference to the handler implementation.</param>
        public static void RemoveHandler(this UIElement that, SW.ExtendedRoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            SW.DragEventHandler dragEventHandler = handler as SW.DragEventHandler;
            if (targetEvents.Contains(routedEvent) && dragEventHandler == null)
            {
                throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeDragEventHandler);
            }

            if (routedEvent == SW.DragDrop.DragEnterEvent)
            {
                SW.DragDrop.RemoveDragEnterHandler(that, dragEventHandler);
            }
            else if (routedEvent == SW.DragDrop.DragOverEvent)
            {
                SW.DragDrop.RemoveDragOverHandler(that, dragEventHandler);
            }
            else if (routedEvent == SW.DragDrop.DragLeaveEvent)
            {
                SW.DragDrop.RemoveDragLeaveHandler(that, dragEventHandler);
            }
            else if (routedEvent == SW.DragDrop.DropEvent)
            {
                SW.DragDrop.RemoveDropHandler(that, dragEventHandler);
            }
            else if (routedEvent == SW.DragDrop.GiveFeedbackEvent)
            {
                SW.GiveFeedbackEventHandler giveFeedbackHandler = handler as SW.GiveFeedbackEventHandler;
                if (giveFeedbackHandler == null)
                {
                    throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeGiveFeedbackEventHandler);
                }
                SW.DragDrop.RemoveGiveFeedbackHandler(that, giveFeedbackHandler);
            }
            else if (routedEvent == SW.DragDrop.QueryContinueDragEvent)
            {
                SW.QueryContinueDragEventHandler queryContinueFeedbackHandler = handler as SW.QueryContinueDragEventHandler;
                if (queryContinueFeedbackHandler == null)
                {
                    throw new InvalidOperationException(Resource.UIElementExtendedRoutedEventExtensions_TypeOfHandlerMustBeQueryContinueDragEventHandler);
                }
                SW.DragDrop.RemoveQueryContinueDragHandler(that, queryContinueFeedbackHandler);
            }
        }
    }
}
