// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Windows
{
    /// <summary>
    /// Stores a collection of routed event handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the event handler.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event arguments.
    /// </typeparam>
    /// <QualityBand>Experimental</QualityBand>
    internal class ExtendedRoutedEventHandlerCollection<THandler, TEventArgs>
        where TEventArgs : ExtendedRoutedEventArgs
    {
        /// <summary>
        /// A collection of handlers and flags indicating whether to raise 
        /// invoke the handler if the event has already been handled.
        /// </summary>
        private List<Tuple<bool, THandler>> _handlers = new List<Tuple<bool, THandler>>();

        /// <summary>
        /// Gets an action invoked when the event is raised.
        /// </summary>
        public Action<THandler, TEventArgs> RaiseAction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ExtendedRoutedEventHandlerCollection class.
        /// </summary>
        /// <param name="raiseAction">The action invoked when the event is
        /// raised.</param>
        public ExtendedRoutedEventHandlerCollection(Action<THandler, TEventArgs> raiseAction)
        {
            this.RaiseAction = raiseAction;
        }

        /// <summary>
        /// Adds a handler to the collection.
        /// </summary>
        /// <param name="handler">The handler to add to the collection.</param>
        public void Add(THandler handler)
        {
            _handlers.Add(Tuple.Create(false, handler));
        }

        /// <summary>
        /// Adds a handler to the collection.
        /// </summary>
        /// <param name="handler">The handler to add to the collection.</param>
        /// <param name="handledEventsToo">A value indicating whether to invoke
        /// the handler if the event has been handled.</param>
        public void Add(THandler handler, bool handledEventsToo)
        {
            _handlers.Add(Tuple.Create(handledEventsToo, handler));
        }

        /// <summary>
        /// Removes a handler from the collection.
        /// </summary>
        /// <param name="handler">The handler to remove from the collection.
        /// </param>
        public void Remove(THandler handler)
        {
            Tuple<bool, THandler> tupleContainingHandler = _handlers.FirstOrDefault(flagAndHandler => object.ReferenceEquals(flagAndHandler.Item2, handler));
            if (tupleContainingHandler != null)
            {
                _handlers.Remove(tupleContainingHandler);
            }
        }

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        public void Raise(TEventArgs args)
        {
            foreach (Tuple<bool, THandler> handler in _handlers)
            {
                if (handler.Item1 || (!args.Handled))
                {
                    RaiseAction(handler.Item2, args);
                }
            }
        }
    }
}