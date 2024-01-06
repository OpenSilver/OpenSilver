// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using SW = Microsoft.Windows;

namespace Microsoft.Windows
{
    /// <summary>
    /// A set of extension methods for DependencyObjects.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static class DependencyObjectExtensions
    {
        /// <summary>
        /// Raises a routed event by executing an operation on an element and
        /// all of its ancestors.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments.</typeparam>
        /// <param name="that">The element to raise the event on.</param>
        /// <param name="eventArgs">Information about the event.</param>
        /// <param name="action">An action that raises the event on a given
        /// element.</param>
        /// <returns>The event args after the event has been raised.</returns>
        internal static T RaiseRoutedEvent<T>(this DependencyObject that, T eventArgs, Action<DependencyObject, T> action)
                    where T : ExtendedRoutedEventArgs
        {
            action(that, eventArgs);
            foreach (DependencyObject ancestor in that.GetVisualAncestors())
            {
                action(ancestor, eventArgs);
            }
            return eventArgs;
        }

        /// <summary>
        /// Raises the attached DragEnter event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnDragEnter(this DependencyObject element, SW.DragEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnDragEnter(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<SW.DragEventHandler, SW.DragEventArgs> handlers = element.GetDragEnterHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }

        /// <summary>
        /// Raises the attached DragOver event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnDragOver(this DependencyObject element, SW.DragEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnDragOver(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<SW.DragEventHandler, SW.DragEventArgs> handlers = element.GetDragOverHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }

        /// <summary>
        /// Raises the attached DragLeave event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnDragLeave(this DependencyObject element, SW.DragEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnDragLeave(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<SW.DragEventHandler, SW.DragEventArgs> handlers = element.GetDragLeaveHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }

        /// <summary>
        /// Raises the attached Drop event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnDrop(this DependencyObject element, SW.DragEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnDrop(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<SW.DragEventHandler, SW.DragEventArgs> handlers = element.GetDropHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }

        /// <summary>
        /// Raises the attached GiveFeedback event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnGiveFeedback(this DependencyObject element, SW.GiveFeedbackEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnGiveFeedback(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<GiveFeedbackEventHandler, SW.GiveFeedbackEventArgs> handlers = element.GetGiveFeedbackHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }

        /// <summary>
        /// Raises the attached QueryContinueDrag event on a element.
        /// </summary>
        /// <param name="element">The element to raise the event on.</param>
        /// <param name="args">Information about the event.</param>
        internal static void OnQueryContinueDrag(this DependencyObject element, SW.QueryContinueDragEventArgs args)
        {
            if (!args.Handled)
            {
                IAcceptDrop acceptDrop = element as IAcceptDrop;
                if (acceptDrop != null)
                {
                    acceptDrop.OnQueryContinueDrag(args);
                }
            }
            ExtendedRoutedEventHandlerCollection<SW.QueryContinueDragEventHandler, SW.QueryContinueDragEventArgs> handlers = element.GetQueryContinueDragHandlers();
            if (handlers != null)
            {
                handlers.Raise(args);
            }
        }
    }
}