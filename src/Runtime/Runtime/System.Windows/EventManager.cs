
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

using System.ComponentModel;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Provides event-related utility methods that register routed events for class owners and add class handlers.
/// </summary>
public static class EventManager
{
    /// <summary>
    /// Registers a new routed event with the Windows Presentation Foundation (WPF) event system.
    /// </summary>
    /// <param name="name">
    /// The name of the routed event. The name must be unique within the owner type and cannot be null or an empty string.
    /// </param>
    /// <param name="routingStrategy">
    /// The routing strategy of the event as a value of the enumeration.
    /// </param>
    /// <param name="handlerType">
    /// The type of the event handler. This must be a delegate type and cannot be null.
    /// </param>
    /// <param name="ownerType">
    /// The owner class type of the routed event. This cannot be null.
    /// </param>
    /// <returns>
    /// The identifier for the newly registered routed event. This identifier object can now be stored as a static 
    /// field in a class and then used as a parameter for methods that attach handlers to the event. The routed event 
    /// identifier is also used for other event system APIs.
    /// </returns>
    public static RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (routingStrategy != RoutingStrategy.Tunnel &&
            routingStrategy != RoutingStrategy.Bubble &&
            routingStrategy != RoutingStrategy.Direct)
        {
            throw new InvalidEnumArgumentException(nameof(routingStrategy), (int)routingStrategy, typeof(RoutingStrategy));
        }

        if (handlerType is null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        if (ownerType is null)
        {
            throw new ArgumentNullException(nameof(ownerType));
        }

        if (GlobalEventManager.GetRoutedEventFromName(name, ownerType, false) != null)
        {
            throw new ArgumentException(string.Format(Strings.DuplicateEventName, name, ownerType));
        }

        return GlobalEventManager.RegisterRoutedEvent(name, routingStrategy, handlerType, ownerType, false);
    }

    internal static RoutedEvent RegisterCoreEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (routingStrategy != RoutingStrategy.Tunnel &&
            routingStrategy != RoutingStrategy.Bubble &&
            routingStrategy != RoutingStrategy.Direct)
        {
            throw new InvalidEnumArgumentException(nameof(routingStrategy), (int)routingStrategy, typeof(RoutingStrategy));
        }

        if (handlerType is null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        if (ownerType is null)
        {
            throw new ArgumentNullException(nameof(ownerType));
        }

        if (GlobalEventManager.GetRoutedEventFromName(name, ownerType, false) != null)
        {
            throw new ArgumentException(string.Format(Strings.DuplicateEventName, name, ownerType));
        }

        return GlobalEventManager.RegisterRoutedEvent(name, routingStrategy, handlerType, ownerType, true);
    }

    /// <summary>
    /// Registers a class handler for a particular routed event.
    /// </summary>
    /// <param name="routedEvent">
    /// The routed event identifier of the event to handle.
    /// </param>
    /// <param name="handler">
    /// A reference to the class handler implementation.
    /// </param>
    public static void RegisterClassHandler<TClassType>(RoutedEvent routedEvent, Delegate handler)
        where TClassType : DependencyObject, IUIElement
    {
        RegisterClassHandler<TClassType>(routedEvent, handler, false);
    }

    /// <summary>
    /// Registers a class handler for a particular routed event.
    /// </summary>
    /// <param name="classType">
    /// The type of the class that is declaring class handling.
    /// </param>
    /// <param name="routedEvent">
    /// The routed event identifier of the event to handle.
    /// </param>
    /// <param name="handler">
    /// A reference to the class handler implementation.
    /// </param>
    public static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, Delegate handler)
        => RegisterClassHandler(classType, routedEvent, handler, false);

    /// <summary>
    /// Registers a class handler for a particular routed event, with the option to handle events where 
    /// event data is already marked handled.
    /// </summary>
    /// <param name="routedEvent">
    /// The routed event identifier of the event to handle.
    /// </param>
    /// <param name="handler">
    /// A reference to the class handler implementation.
    /// </param>
    /// <param name="handledEventsToo">
    /// true to invoke this class handler even if arguments of the routed event have been marked as handled; 
    /// false to retain the default behavior of not invoking the handler on any marked-handled event.
    /// </param>
    public static void RegisterClassHandler<TClassType>(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        where TClassType : DependencyObject, IUIElement
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

        GlobalEventManager.RegisterClassHandler(typeof(TClassType), routedEvent, handler, handledEventsToo);
    }

    /// <summary>
    /// Registers a class handler for a particular routed event, with the option to handle events where 
    /// event data is already marked handled.
    /// </summary>
    /// <param name="classType">
    /// The type of the class that is declaring class handling.
    /// </param>
    /// <param name="routedEvent">
    /// The routed event identifier of the event to handle.
    /// </param>
    /// <param name="handler">
    /// A reference to the class handler implementation.
    /// </param>
    /// <param name="handledEventsToo">
    /// true to invoke this class handler even if arguments of the routed event have been marked as handled; 
    /// false to retain the default behavior of not invoking the handler on any marked-handled event.
    /// </param>
    public static void RegisterClassHandler(Type classType, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
        if (classType is null)
        {
            throw new ArgumentNullException(nameof(classType));
        }

        if (routedEvent is null)
        {
            throw new ArgumentNullException(nameof(routedEvent));
        }

        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        if (!typeof(DependencyObject).IsAssignableFrom(classType) || !typeof(IUIElement).IsAssignableFrom(classType))
        {
            throw new ArgumentException(Strings.ClassTypeIllegal);
        }

        if (!routedEvent.IsLegalHandler(handler))
        {
            throw new ArgumentException(Strings.HandlerTypeIllegal);
        }

        GlobalEventManager.RegisterClassHandler(classType, routedEvent, handler, handledEventsToo);
    }

    /// <summary>
    /// Returns identifiers for routed events that have been registered to the event system.
    /// </summary>
    /// <returns>
    /// An array of type <see cref="RoutedEvent"/> that contains the registered objects.
    /// </returns>
    public static RoutedEvent[] GetRoutedEvents() => GlobalEventManager.GetRoutedEvents();

    /// <summary>
    /// Finds all routed event identifiers for events that are registered with the provided owner type.
    /// </summary>
    /// <param name="ownerType">
    /// The type to start the search with. Base classes are included in the search.
    /// </param>
    /// <returns>
    /// An array of matching routed event identifiers if any match is found; otherwise, null.
    /// </returns>
    public static RoutedEvent[] GetRoutedEventsForOwner(Type ownerType)
    {
        if (ownerType is null)
        {
            throw new ArgumentNullException(nameof(ownerType));
        }

        return GlobalEventManager.GetRoutedEventsForOwner(ownerType);
    }

    /// <summary>
    ///     Finds a <see cref="RoutedEvent"/> with a 
    ///     matching <see cref="RoutedEvent.Name"/> 
    ///     and <see cref="RoutedEvent.OwnerType"/>
    /// </summary>
    /// <remarks>
    ///     More specifically finds a 
    ///     <see cref="RoutedEvent"/> with a matching 
    ///     <see cref="RoutedEvent.Name"/> starting 
    ///     on the <see cref="RoutedEvent.OwnerType"/> 
    ///     and looking at its super class types <para/>
    ///     <para/>
    ///
    ///     If no matches are found, this method returns null
    /// </remarks>
    /// <param name="name">
    ///     <see cref="RoutedEvent.Name"/> to be matched
    /// </param>
    /// <param name="ownerType">
    ///     <see cref="RoutedEvent.OwnerType"/> to start
    ///     search with and follow through to super class types
    /// </param>
    /// <returns>
    ///     Matching <see cref="RoutedEvent"/>
    /// </returns>
    internal static RoutedEvent GetRoutedEventFromName(string name, Type ownerType)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (ownerType is null)
        {
            throw new ArgumentNullException(nameof(ownerType));
        }

        return GlobalEventManager.GetRoutedEventFromName(name, ownerType, true);
    }
}
