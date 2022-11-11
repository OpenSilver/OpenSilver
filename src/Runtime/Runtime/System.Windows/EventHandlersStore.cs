
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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    ///     Container for the event handlers
    /// </summary>
    /// <remarks>
    ///     EventHandlersStore is a hashtable of handlers for a given RoutedEvent
    /// </remarks>
    internal sealed class EventHandlersStore
    {
        private readonly Dictionary<RoutedEvent, List<RoutedEventHandlerInfo>> _entries;

        /// <summary>
        ///     Constructor for EventHandlersStore
        /// </summary>
        public EventHandlersStore()
        {
            _entries = new Dictionary<RoutedEvent, List<RoutedEventHandlerInfo>>();
        }

        /// <summary>
        ///     Adds a routed event handler for the given 
        ///     RoutedEvent to the store
        /// </summary>
        public void AddRoutedEventHandler(
            RoutedEvent routedEvent,
            Delegate handler,
            bool handledEventsToo)
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

            // Create a new RoutedEventHandler
            RoutedEventHandlerInfo routedEventHandlerInfo =
                new RoutedEventHandlerInfo(handler, handledEventsToo);

            if (!_entries.TryGetValue(routedEvent, out List<RoutedEventHandlerInfo> handlers))
            {
                _entries[routedEvent] = handlers = new List<RoutedEventHandlerInfo>(1);
            }

            handlers.Add(routedEventHandlerInfo);
        }

        /// <summary>
        ///     Removes an instance of the specified 
        ///     routed event handler for the given 
        ///     RoutedEvent from the store
        /// </summary>
        /// <remarks>
        ///     NOTE: This method does nothing if no 
        ///     matching handler instances are found 
        ///     in the store
        /// </remarks>
        public void RemoveRoutedEventHandler(RoutedEvent routedEvent, Delegate handler)
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

            if (_entries.TryGetValue(routedEvent, out List<RoutedEventHandlerInfo> handlers) && handlers != null)
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    if (handlers[i].Handler == handler)
                    {
                        handlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // Returns Handlers for the given key
        internal List<RoutedEventHandlerInfo> this[RoutedEvent key]
        {
            get
            {
                Debug.Assert(key != null, "Search key cannot be null");

                if (_entries.TryGetValue(key, out List<RoutedEventHandlerInfo> handlers))
                {
                    return handlers;
                }

                return null;
            }
        }
    }
}