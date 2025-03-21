
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
using System.Windows.Markup;

namespace System.Windows;

/// <summary>
/// Represents and identifies a routed event and declares its characteristics.
/// </summary>
[TypeConverter(typeof(RoutedEventConverter))]
public sealed class RoutedEvent
{
    // Constructor for a RoutedEvent (is internal to the EventManager and is onvoked when a new RoutedEvent is registered)
    internal RoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
    {
        Name = name;
        RoutingStrategy = routingStrategy;
        HandlerType = handlerType;
        OwnerType = ownerType;

        GlobalIndex = GlobalEventManager.GetNextAvailableGlobalIndex(this);
    }

    /// <summary>
    /// Index in GlobalEventManager 
    /// </summary>
    internal int GlobalIndex { get; }

    /// <summary>
    /// Gets the identifying name of the routed event.
    /// </summary>
    /// <returns>
    /// The name of the routed event.
    /// </returns>
    public string Name { get; }

    /// <summary>
    /// Gets the routing strategy of the routed event.
    /// </summary>
    /// <returns>
    /// One of the enumeration values. The default is the enumeration default, <see cref="RoutingStrategy.Bubble"/>.
    /// </returns>
    public RoutingStrategy RoutingStrategy { get; }

    /// <summary>
    /// Gets the handler type of the routed event.
    /// </summary>
    /// <returns>
    /// The handler type of the routed event.
    /// </returns>
    public Type HandlerType { get; }

    /// <summary>
    /// Gets the registered owner type of the routed event.
    /// </summary>
    /// <returns>
    /// The owner type of the routed event.
    /// </returns>
    public Type OwnerType { get; }

    /// <summary>
    /// Associates another owner type with the routed event represented by a <see cref="RoutedEvent"/> 
    /// instance, and enables routing of the event and its handling.
    /// </summary>
    /// <param name="ownerType">
    /// The type where the routed event is added.
    /// </param>
    /// <returns>
    /// The identifier field for the event. This return value should be used to set a public static 
    /// read-only field that will store the identifier for the representation of the routed event on 
    /// the owning type. This field is typically defined with public access, because user code must 
    /// reference the field in order to attach any instance handlers for the routed event when using 
    /// the <see cref="UIElement.AddHandler(RoutedEvent, Delegate, bool)"/> utility method.
    /// </returns>
    public RoutedEvent AddOwner(Type ownerType)
    {
        GlobalEventManager.AddOwner(this, ownerType);
        return this;
    }

    /// <summary>
    /// Returns the string representation of this <see cref="RoutedEvent"/>.
    /// </summary>
    /// <returns>
    /// A string representation for this object, which is identical to the value returned by <see cref="Name"/>.
    /// </returns>
    public override string ToString() => $"{OwnerType.Name}.{Name}";

    // Check to see if the given delegate is a legal handler for this type.
    //  It either needs to be a type that the registering class knows how to
    //  handle, or a RoutedEventHandler which we can handle without the help
    //  of the registering class.
    internal bool IsLegalHandler(Delegate handler)
    {
        Type handlerType = handler.GetType();
        return handlerType == HandlerType || handlerType == typeof(RoutedEventHandler);
    }
}
