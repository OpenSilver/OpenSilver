
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
using System.Runtime.CompilerServices;
using OpenSilver.Internal;

namespace System.Windows;

internal static class GlobalEventManager
{
    // Registers a RoutedEvent with the given details
    // NOTE: The Name must be unique within the given OwnerType
    internal static RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType, bool isCoreEvent)
    {
        Debug.Assert(GetRoutedEventFromName(name, ownerType, false) is null, "RoutedEvent name must be unique within a given OwnerType");

        lock (Synchronized)
        {
            // Create a new RoutedEvent
            // Requires GlobalLock to access _countRoutedEvents
            var routedEvent = new RoutedEvent(
                name,
                routingStrategy,
                handlerType,
                ownerType,
                isCoreEvent);

            AddOwner(routedEvent, ownerType);

            return routedEvent;
        }
    }

    // Register a Class Handler
    // NOTE: Handler Type must be the 
    // same as the one specified when 
    // registering the corresponding RoutedEvent
    internal static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
        Debug.Assert(typeof(DependencyObject).IsAssignableFrom(classType) && typeof(IUIElement).IsAssignableFrom(classType),
            "Class Handlers can be registered only for UIElement and their sub types");
        Debug.Assert(routedEvent.IsLegalHandler(handler), "Handler Type mismatch");

        // We map the classType to a DType use DTypeMap for storage
        DependencyObjectType dType = DependencyObjectType.FromSystemTypeInternal(classType);

        // Get the updated EventHandlersStore for the given DType
        GetDTypedClassListeners(dType, routedEvent, out ClassHandlersStore classListenersLists, out int index);

        // Reuired to update storage
        lock (Synchronized)
        {
            // Add new routed event handler and get the updated set of handlers
            RoutedEventHandlerInfoList updatedClassListeners =
                classListenersLists.AddToExistingHandlers(index, handler, handledEventsToo);

            // Update Sub Classes
            foreach (var pair in _dTypedClassListeners)
            {
                if (pair.Key.IsSubclassOf(dType))
                {
                    classListenersLists = pair.Value;
                    classListenersLists.UpdateSubClassHandlers(routedEvent, updatedClassListeners);
                }
            }
        }
    }

    // Returns a copy of the list of registered RoutedEvents
    // Returns a copy of the list so the original cannot be modified
    internal static RoutedEvent[] GetRoutedEvents()
    {
        RoutedEvent[] routedEvents;

        lock (Synchronized)
        {
            routedEvents = RegisteredEventList.ToArray();
        }

        return routedEvents;
    }

    internal static void AddOwner(RoutedEvent routedEvent, Type ownerType)
    {
        // If the ownerType is a subclass of DependencyObject 
        // we map it to a DType use DTypeMap for storage else 
        // we use the more generic Hashtable.
        if (ownerType == typeof(DependencyObject) || ownerType.IsSubclassOf(typeof(DependencyObject)))
        {
            DependencyObjectType dType = DependencyObjectType.FromSystemTypeInternal(ownerType);

            // Get the ItemList of RoutedEvents for the given OwnerType
            // Requires GlobalLock to access _dTypedRoutedEventList
            if (!_dTypedRoutedEventList.TryGetValue(dType, out List<RoutedEvent> ownerRoutedEventList))
            {
                // Create an ItemList of RoutedEvents for the 
                // given OwnerType if one does not already exist
                ownerRoutedEventList = new List<RoutedEvent>(1);
                _dTypedRoutedEventList[dType] = ownerRoutedEventList;
            }

            // Add the newly created 
            // RoutedEvent to the ItemList
            // Requires GlobalLock to access ownerRoutedEventList
            if (!ownerRoutedEventList.Contains(routedEvent))
            {
                ownerRoutedEventList.Add(routedEvent);
            }
        }
        else
        {
            // Get the ItemList of RoutedEvents for the given OwnerType
            // Requires GlobalLock to access _ownerTypedRoutedEventList
            if (!_ownerTypedRoutedEventList.TryGetValue(ownerType, out List<RoutedEvent> ownerRoutedEventList))
            {
                // Create an ItemList of RoutedEvents for the 
                // given OwnerType if one does not already exist
                ownerRoutedEventList = new List<RoutedEvent>(1);
                _ownerTypedRoutedEventList[ownerType] = ownerRoutedEventList;
            }

            // Add the newly created 
            // RoutedEvent to the ItemList
            // Requires GlobalLock to access ownerRoutedEventList
            if (!ownerRoutedEventList.Contains(routedEvent))
            {
                ownerRoutedEventList.Add(routedEvent);
            }
        }
    }

    // Returns a RoutedEvents that match 
    // the ownerType input param
    // If not found returns null
    internal static RoutedEvent[] GetRoutedEventsForOwner(Type ownerType)
    {
        if (ownerType == typeof(DependencyObject) || ownerType.IsSubclassOf(typeof(DependencyObject)))
        {
            // Search DTypeMap
            DependencyObjectType dType = DependencyObjectType.FromSystemTypeInternal(ownerType);

            // Get the ItemList of RoutedEvents for the given DType
            if (_dTypedRoutedEventList.TryGetValue(dType, out List<RoutedEvent> ownerRoutedEventList))
            {
                return ownerRoutedEventList.ToArray();
            }
        }
        else // Search Hashtable
        {
            // Get the ItemList of RoutedEvents for the given OwnerType
            if (_ownerTypedRoutedEventList.TryGetValue(ownerType, out List<RoutedEvent> ownerRoutedEventList))
            {
                return ownerRoutedEventList.ToArray();
            }
        }

        // No match found
        return null;
    }

    // Returns a RoutedEvents that match 
    // the name and ownerType input params
    // If not found returns null
    internal static RoutedEvent GetRoutedEventFromName(string name, Type ownerType, bool includeSupers)
    {
        if (ownerType == typeof(DependencyObject) || ownerType.IsSubclassOf(typeof(DependencyObject)))
        {
            // Search DTypeMap
            DependencyObjectType dType = DependencyObjectType.FromSystemTypeInternal(ownerType);

            while (dType != null)
            {
                // Ensure static constructor of type has run
                RuntimeHelpers.RunClassConstructor(dType.SystemType.TypeHandle);

                // Get the ItemList of RoutedEvents for the given DType
                if (_dTypedRoutedEventList.TryGetValue(dType, out List<RoutedEvent> ownerRoutedEventList))
                {
                    // Check for RoutedEvent with matching name in the ItemList
                    foreach (RoutedEvent routedEvent in ownerRoutedEventList)
                    {
                        if (routedEvent.Name.Equals(name))
                        {
                            // Return if found match
                            return routedEvent;
                        }
                    }
                }

                // If not found match yet check for BaseType if specified to do so
                dType = includeSupers ? dType.BaseType : null;
            }
        }
        else
        {
            // Search Hashtable
            while (ownerType != null)
            {
                // Ensure static constructor of type has run
                RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);

                // Get the ItemList of RoutedEvents for the given OwnerType
                if (_ownerTypedRoutedEventList.TryGetValue(ownerType, out List<RoutedEvent> ownerRoutedEventList))
                {
                    // Check for RoutedEvent with matching name in the ItemList
                    foreach (RoutedEvent routedEvent in ownerRoutedEventList)
                    {
                        if (routedEvent.Name.Equals(name))
                        {
                            // Return if found match
                            return routedEvent;
                        }
                    }
                }

                // If not found match yet check for BaseType if specified to do so
                ownerType = includeSupers ? ownerType.BaseType : null;
            }
        }

        // No match found
        return null;
    }

    // Returns the list of class listeners for the given 
    // DType and RoutedEvent
    // NOTE: Returns null if no matches found
    // Helper method for GetClassListeners
    // Invoked only when trying to build the event route
    internal static RoutedEventHandlerInfoList GetDTypedClassListeners(DependencyObjectType dType, RoutedEvent routedEvent)
    {
        // Class Forwarded
        return GetDTypedClassListeners(dType, routedEvent, out _, out _);
    }

    // Returns the list of class listeners for the given 
    // DType and RoutedEvent
    // NOTE: Returns null if no matches found
    // Helper method for GetClassListeners
    // Invoked when trying to build the event route 
    // as well as when registering a new class handler
    private static RoutedEventHandlerInfoList GetDTypedClassListeners(
        DependencyObjectType dType,
        RoutedEvent routedEvent,
        out ClassHandlersStore classListenersLists,
        out int index)
    {
        RoutedEventHandlerInfoList handlers;

        // Get the ClassHandlersStore for the given DType
        if (_dTypedClassListeners.TryGetValue(dType, out classListenersLists))
        {
            // Get the handlers for the given DType and RoutedEvent
            index = classListenersLists.GetHandlersIndex(routedEvent);
            if (index != -1)
            {
                handlers = classListenersLists.GetExistingHandlers(index);
                return handlers;
            }
        }

        lock (Synchronized)
        {
            // Search the DTypeMap for the list of matching RoutedEventHandlerInfo
            handlers = GetUpdatedDTypedClassListeners(dType, routedEvent, out classListenersLists, out index);
        }

        return handlers;
    }

    // Helper method for GetDTypedClassListeners
    // Returns updated list of class listeners for the given 
    // DType and RoutedEvent
    // NOTE: Returns null if no matches found
    // Invoked when trying to build the event route 
    // as well as when registering a new class handler
    private static RoutedEventHandlerInfoList GetUpdatedDTypedClassListeners(
        DependencyObjectType dType,
        RoutedEvent routedEvent,
        out ClassHandlersStore classListenersLists,
        out int index)
    {
        RoutedEventHandlerInfoList handlers;

        // Get the ClassHandlersStore for the given DType
        if (_dTypedClassListeners.TryGetValue(dType, out classListenersLists))
        {
            // Get the handlers for the given DType and RoutedEvent
            index = classListenersLists.GetHandlersIndex(routedEvent);
            if (index != -1)
            {
                handlers = classListenersLists.GetExistingHandlers(index);
                return handlers;
            }
        }

        // Since matching handlers were not found at this level 
        // browse base classes to check for registered class handlers
        DependencyObjectType tempDType = dType;
        RoutedEventHandlerInfoList tempHandlers = null;
        int tempIndex = -1;
        while (tempIndex == -1 && tempDType.Id != _dependencyObjectType.Id)
        {
            tempDType = tempDType.BaseType;
            if (_dTypedClassListeners.TryGetValue(tempDType, out ClassHandlersStore tempClassListenersLists))
            {
                // Get the handlers for the DType and RoutedEvent
                tempIndex = tempClassListenersLists.GetHandlersIndex(routedEvent);
                if (tempIndex != -1)
                {
                    tempHandlers = tempClassListenersLists.GetExistingHandlers(tempIndex);
                }
            }
        }

        if (classListenersLists is null)
        {
            if (dType.SystemType == typeof(UIElement))
            {
                classListenersLists = new ClassHandlersStore(20); // Based on the number of class handlers for these classes
            }
            else
            {
                classListenersLists = new ClassHandlersStore(1);
            }

            _dTypedClassListeners[dType] = classListenersLists;
        }

        index = classListenersLists.CreateHandlersLink(routedEvent, tempHandlers);

        return tempHandlers;
    }

    internal static int GetNextAvailableGlobalIndex(RoutedEvent routedEvent)
    {
        int index;
        lock (Synchronized)
        {
            // Prevent GlobalIndex from overflow. RoutedEvents are meant to be static members and are to be registered 
            // only via static constructors. However there is no cheap way of ensuring this, without having to do a stack walk. Hence 
            // concievably people could register RoutedEvents via instance methods and therefore cause the GlobalIndex to 
            // overflow. This check will explicitly catch this error, instead of silently malfuntioning.
            if (RegisteredEventList.Count >= int.MaxValue)
            {
                throw new InvalidOperationException(Strings.TooManyRoutedEvents);
            }

            index = RegisteredEventList.Count;
            RegisteredEventList.Add(routedEvent);
        }
        return index;
    }

    // must be used within a lock of GlobalEventManager.Synchronized
    internal static List<RoutedEvent> RegisteredEventList { get; } = new(100);

    // This is an efficient  Hashtable of ItemLists keyed on DType
    // Each ItemList holds the registered RoutedEvents for that OwnerType
    private static readonly Dictionary<DependencyObjectType, List<RoutedEvent>> _dTypedRoutedEventList = new(10); // Initialization sizes based on typical MSN scenario

    // This is a Hashtable of ItemLists keyed on OwnerType
    // Each ItemList holds the registered RoutedEvents for that OwnerType
    private static readonly Dictionary<Type, List<RoutedEvent>> _ownerTypedRoutedEventList = new(10); // Initialization sizes based on typical MSN scenario

    // This is an efficient Hashtable of ItemLists keyed on DType
    // Each ItemList holds the registered RoutedEvent class handlers for that ClassType
    private static readonly Dictionary<DependencyObjectType, ClassHandlersStore> _dTypedClassListeners = new(100); // Initialization sizes based on typical Expression Blend startup scenario

    // This is the cached value for the DType of DependencyObject
    private static readonly DependencyObjectType _dependencyObjectType = DependencyObjectType.FromSystemTypeInternal(typeof(DependencyObject));

    private static readonly object Synchronized = new();

    // Container for the class event handlers ClassHandlersStore constitues lists of RoutedEventHandlerInfo
    // keyed on the RoutedEvent.
    private sealed class ClassHandlersStore
    {
        // Stores list of ClassHandlers keyed on RoutedEvent
        private readonly List<ClassHandlers> _eventHandlersList;

        // Constructor for ClassHandlersStore
        internal ClassHandlersStore(int size)
        {
            _eventHandlersList = new List<ClassHandlers>(size);
        }

        // Adds a routed event handler at the given index of the store
        // Returns updated set of handlers
        // NOTE: index must be valid, i.e. not -1
        internal RoutedEventHandlerInfoList AddToExistingHandlers(
            int index,
            Delegate handler,
            bool handledEventsToo)
        {
            Debug.Assert(index != -1, "There should exist a set of handlers for the given routedEvent");

            // Create a new RoutedEventHandler
            var routedEventHandlerInfo = new RoutedEventHandlerInfo(handler, handledEventsToo);

            // Check if we need to create a new node in the linked list
            RoutedEventHandlerInfoList handlers = _eventHandlersList[index].Handlers;
            if (handlers is null || !_eventHandlersList[index].HasSelfHandlers)
            {
                // Create a new node in the linked list of class 
                // handlers for this type and routed event.
                handlers = new RoutedEventHandlerInfoList
                {
                    Handlers = [routedEventHandlerInfo],
                    Next = _eventHandlersList[index].Handlers
                };
                _eventHandlersList[index].Handlers = handlers;
                _eventHandlersList[index].HasSelfHandlers = true;
            }
            else
            {
                // Add this handler to the existing node in the linked list 
                // of class handlers for this type and routed event.
                int length = handlers.Handlers.Length;
                RoutedEventHandlerInfo[] mergedHandlers = new RoutedEventHandlerInfo[length + 1];
                Array.Copy(handlers.Handlers, 0, mergedHandlers, 0, length);
                mergedHandlers[length] = routedEventHandlerInfo;
                handlers.Handlers = mergedHandlers;
            }

            return handlers;
        }

        // Returns EventHandlers stored at the given index in the datastructure
        // NOTE: index must be valid, i.e. not -1
        internal RoutedEventHandlerInfoList GetExistingHandlers(int index)
        {
            Debug.Assert(index != -1, "There should exist a set of handlers for the given index");

            return _eventHandlersList[index].Handlers;
        }

        // Creates reference to given handlers and RoutedEvent
        // Returns the index at which the new reference was added
        // NOTE: There should not exist a set of handlers for the 
        // given routedEvent
        internal int CreateHandlersLink(RoutedEvent routedEvent, RoutedEventHandlerInfoList handlers)
        {
            Debug.Assert(GetHandlersIndex(routedEvent) == -1, "There should not exist a set of handlers for the given routedEvent");

            _eventHandlersList.Add(new ClassHandlers(routedEvent, handlers));

            return _eventHandlersList.Count - 1;
        }

        // Update Sub Class Handlers with the given base class listeners
        // NOTE : Do not wastefully try to update subclass listeners when 
        // base class listeners are null
        internal void UpdateSubClassHandlers(RoutedEvent routedEvent, RoutedEventHandlerInfoList baseClassListeners)
        {
            Debug.Assert(baseClassListeners != null, "Update only when there are base class listeners to be updated");

            // Get the handlers index corresponding to the given RoutedEvent
            int index = GetHandlersIndex(routedEvent);
            if (index != -1)
            {
                bool hasSelfHandlers = _eventHandlersList[index].HasSelfHandlers;

                // Fetch the handlers for your baseType that the current node knows of

                RoutedEventHandlerInfoList handlers = hasSelfHandlers ?
                    _eventHandlersList[index].Handlers.Next :
                    _eventHandlersList[index].Handlers;

                bool needToChange = false;

                // If the current node has baseType handlers check if the baseClassListeners 
                // provided is for a super type of that baseType. If it is then you will 
                // replace the baseType handlers for the current node with the provided 
                // baseClassListeners. If the given baseClassListeners is for a sub type 
                // of the current nodes's baseType then we do not need to update the current node.
                //
                // Example: Consider the following class hierarchy A -> B -> C. 
                //
                // Now imagine that we register class handlers in the following order.
                // 1. Register class handler for A.
                // - A's linked list will be A -> NULL.
                // - B's linked list will be NULL.
                // - C's linked list will be NULL.
                // 2. Register class handler for C.
                // - A's linked list will be A -> NULL.
                // - B's linkedList will be NULL.
                // - C's linked list will be C -> A -> NULL.
                // 3. Register class handler for B.
                // - A's linked list will be A -> NULL.
                // - B's linkedList will be B -> A -> NULL.
                // - While updating C's linked list we are given B's linked list for the baseClassListers. 
                //   Now we want to check if B is a super type of A which is the current baseType that C 
                //   knows of. The contains check below determines this. Since it is we now replace C.Next 
                //   to be B. Thus we get C -> B -> A -> NULL.
                //
                // Now imagine that we register class handlers in the following order.
                // 1. Register class handler for C.
                // - A's linked list will be NULL.
                // - B's linked list will be NULL.
                // - C's linked list will be C -> NULL.
                // 2. Register class handler for B.
                // - A's linked list will be NULL.
                // - B's linkedList will be B -> NULL.
                // - While updating C's linked list we are given B's linked list for the baseClassListeners. 
                //   Since C does not know of any baseType listeners already it takes the given 
                //   baseClassListeners as is. Thus it has C -> B -> NULL
                // 3. Register class handler for A.
                // - A's linked list will be A -> NULL.
                // - B's linkedList will be B -> A -> NULL.
                // - While updating C's linked list we are given A's linked list for the baseClassListers. 
                //   Now we want to check if A is a super type of B which is the current baseType that C 
                //   knows of. The contains check below determines this. Since it isn't we do not need to 
                //   change the linked list for C. Since B's linked list has already been updated we get 
                //   C -> B - > A -> NULL.

                if (handlers != null)
                {
                    if (baseClassListeners.Next != null && baseClassListeners.Next.Contains(handlers))
                    {
                        needToChange = true;
                    }
                }

                // If the current node does not have any baseType handlers then if will 
                // simply use the given baseClassListeners.

                else
                {
                    needToChange = true;
                }

                if (needToChange)
                {
                    // If current node has self handlers then its next pointer 
                    // needs update if not the current node needs update.

                    if (hasSelfHandlers)
                    {
                        _eventHandlersList[index].Handlers.Next = baseClassListeners;
                    }
                    else
                    {
                        _eventHandlersList[index].Handlers = baseClassListeners;
                    }
                }
            }
        }

        // Returns EventHandlers Index for the given RoutedEvent
        internal int GetHandlersIndex(RoutedEvent routedEvent)
        {
            // Linear Search
            for (int i = 0; i < _eventHandlersList.Count; i++)
            {
                if (_eventHandlersList[i].RoutedEvent == routedEvent)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    // Stores ClassHandlers for the given RoutedEvent
    private sealed class ClassHandlers
    {
        internal readonly RoutedEvent RoutedEvent;
        internal RoutedEventHandlerInfoList Handlers;
        internal bool HasSelfHandlers;

        public ClassHandlers(RoutedEvent routedEvent, RoutedEventHandlerInfoList handlers)
        {
            RoutedEvent = routedEvent;
            Handlers = handlers;
            HasSelfHandlers = false;
        }

        public override bool Equals(object o) => Equals((ClassHandlers)o);

        public bool Equals(ClassHandlers classHandlers) => classHandlers.RoutedEvent == RoutedEvent && classHandlers.Handlers == Handlers;

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(ClassHandlers classHandlers1, ClassHandlers classHandlers2) => classHandlers1.Equals(classHandlers2);

        public static bool operator !=(ClassHandlers classHandlers1, ClassHandlers classHandlers2) => !classHandlers1.Equals(classHandlers2);
    }
}

/// <summary>
/// This data-structure represents a linked list of all the Class Handlers for a type and its base types.
/// </summary>
internal sealed class RoutedEventHandlerInfoList
{
    internal RoutedEventHandlerInfo[] Handlers;
    internal RoutedEventHandlerInfoList Next;

    internal bool Contains(RoutedEventHandlerInfoList handlers)
    {
        RoutedEventHandlerInfoList tempHandlers = this;
        while (tempHandlers != null)
        {
            if (tempHandlers == handlers)
            {
                return true;
            }

            tempHandlers = tempHandlers.Next;
        }

        return false;
    }
}
