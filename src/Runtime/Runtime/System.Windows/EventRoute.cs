
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
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
///     Container for the route to be followed 
///     by a RoutedEvent when raised
/// </summary>
/// <remarks>
///     EventRoute constitues <para/>
///     a non-null <see cref="RoutedEvent"/>
///     and <para/>
///     an ordered list of (target object, handler list)
///     pairs <para/>
///     <para/>
///
///     It facilitates adding new entries to this list
///     and also allows for the handlers in the list 
///     to be invoked
/// </remarks>
internal sealed class EventRoute
{
    /// <summary>
    ///     Constructor for <see cref="EventRoute"/> given
    ///     the associated <see cref="RoutedEvent"/>
    /// </summary>
    /// <param name="routedEvent">
    ///     Non-null <see cref="RoutedEvent"/> to be associated with 
    ///     this <see cref="EventRoute"/>
    /// </param>
    public EventRoute(RoutedEvent routedEvent)
    {
        ArgumentNullException.ThrowIfNull(routedEvent);

        RoutedEvent = routedEvent;

        // Changed the initialization size to 16 
        // to achieve performance gain based 
        // on standard app behavior
        _routeItemList = new List<RouteItem>(16);
    }

    /// <summary>
    ///     Adds this handler for the 
    ///     specified target to the route
    /// </summary>
    /// <remarks>
    ///     NOTE: It is not an error to add a 
    ///     handler for a particular target instance 
    ///     twice (handler will simply be called twice). 
    /// </remarks>
    /// <param name="target">
    ///     Target object whose handler is to be 
    ///     added to the route
    /// </param>
    /// <param name="handler">
    ///     Handler to be added to the route
    /// </param>
    /// <param name="handledEventsToo">
    ///     Flag indicating whether or not the listener wants to 
    ///     hear about events that have already been handled
    /// </param>
    public void Add(object target, Delegate handler, bool handledEventsToo)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(handler);

        _routeItemList.Add(new RouteItem(target, new RoutedEventHandlerInfo(handler, handledEventsToo)));
    }

    /// <summary>
    ///     Invokes all the handlers that have been 
    ///     added to the route
    /// </summary>
    /// <param name="args">
    ///     <see cref="RoutedEventArgs"/> that carry
    ///     all the details specific to this RoutedEvent
    /// </param>
    internal void InvokeHandlers(RoutedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Source is null)
        {
            throw new ArgumentException(Strings.SourceNotSet);
        }

        if (args.RoutedEvent != RoutedEvent)
        {
            throw new ArgumentException(Strings.Mismatched_RoutedEvent);
        }

        if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct || args.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
        {
            foreach (RouteItem routeItem in _routeItemList)
            {
                routeItem.InvokeHandler(args);
            }
        }
        else
        {
            int endTargetIndex = _routeItemList.Count - 1;
            int startTargetIndex;

            while (endTargetIndex >= 0)
            {
                // For tunnel events we need to invoke handlers for the last target first. 
                // However the handlers for that individual target must be fired in the right order. 
                // Eg. Class Handlers must be fired before Instance Handlers.
                object currTarget = _routeItemList[endTargetIndex].Target;
                for (startTargetIndex = endTargetIndex; startTargetIndex >= 0; startTargetIndex--)
                {
                    if (_routeItemList[startTargetIndex].Target != currTarget)
                    {
                        break;
                    }
                }

                for (int i = startTargetIndex + 1; i <= endTargetIndex; i++)
                {
                    _routeItemList[i].InvokeHandler(args);
                }

                endTargetIndex = startTargetIndex;
            }
        }
    }

    /// <summary>
    ///     Cleanup all the references within the data
    /// </summary>
    internal void Clear()
    {
        RoutedEvent = null;

        _routeItemList.Clear();
    }

    internal RoutedEvent RoutedEvent { get; set; }

    // Stores the routed event handlers to be 
    // invoked for the associated RoutedEvent
    private readonly List<RouteItem> _routeItemList;
}
