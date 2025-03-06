
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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
///     Container for the event handlers
/// </summary>
/// <remarks>
///     EventHandlersStore is a hashtable of handlers for a given RoutedEvent
/// </remarks>
internal sealed class EventHandlersStore : IEnumerable<(int GlobalIndex, List<RoutedEventHandlerInfo> Handlers)>
{
    private readonly Dictionary<int, List<RoutedEventHandlerInfo>> _entries;

    /// <summary>
    ///     Constructor for EventHandlersStore
    /// </summary>
    public EventHandlersStore()
    {
        _entries = new Dictionary<int, List<RoutedEventHandlerInfo>>();
    }

    /// <summary>
    ///     Adds a routed event handler for the given 
    ///     RoutedEvent to the store
    /// </summary>
    public void AddRoutedEventHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
        Debug.Assert(routedEvent is not null);
        Debug.Assert(handler is not null);
        Debug.Assert(routedEvent.IsLegalHandler(handler), Strings.HandlerTypeIllegal);

        // Create a new RoutedEventHandler
        var routedEventHandlerInfo = new RoutedEventHandlerInfo(handler, handledEventsToo);

        if (!_entries.TryGetValue(routedEvent.GlobalIndex, out List<RoutedEventHandlerInfo> handlers))
        {
            _entries[routedEvent.GlobalIndex] = handlers = new List<RoutedEventHandlerInfo>(1);
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
        Debug.Assert(routedEvent is not null);
        Debug.Assert(handler is not null);
        Debug.Assert(routedEvent.IsLegalHandler(handler), Strings.HandlerTypeIllegal);

        if (_entries.TryGetValue(routedEvent.GlobalIndex, out List<RoutedEventHandlerInfo> handlers))
        {
            if (handlers.Count == 1 && handlers[0].Handler == handler)
            {
                // this is the only handler for this event and it's being removed, reclaim space.
                _entries.Remove(routedEvent.GlobalIndex);
            }
            else
            {
                // When a matching instance is found remove it
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
    }

    // Returns Handlers for the given key
    public List<RoutedEventHandlerInfo> Get(RoutedEvent routedEvent)
    {
        return _entries.TryGetValue(routedEvent.GlobalIndex, out List<RoutedEventHandlerInfo> handlers) ? handlers : null;
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<(int, List<RoutedEventHandlerInfo>)> IEnumerable<(int GlobalIndex, List<RoutedEventHandlerInfo> Handlers)>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<(int GlobalIndex, List<RoutedEventHandlerInfo> Handlers)>
    {
        private Dictionary<int, List<RoutedEventHandlerInfo>>.Enumerator _enumerator;

        public Enumerator(EventHandlersStore store)
        {
            _enumerator = store._entries.GetEnumerator();
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public (int GlobalIndex, List<RoutedEventHandlerInfo> Handlers) Current
        {
            get
            {
                KeyValuePair<int, List<RoutedEventHandlerInfo>> current = _enumerator.Current;
                return (current.Key, current.Value);
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() => _enumerator.Dispose();

        void IEnumerator.Reset() => ((IEnumerator)_enumerator).Reset();
    }
}
