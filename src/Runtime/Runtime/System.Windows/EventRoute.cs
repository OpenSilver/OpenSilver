
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

using OpenSilver.Internal;
using System.Collections.Generic;

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
        _sourceItemList = new List<SourceItem>(16);
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
    /// <remarks>
    ///     NOTE: If the <see cref="RoutingStrategy"/> 
    ///     of the associated <see cref="RoutedEvent"/> 
    ///     is <see cref="RoutingStrategy.Bubble"/>
    ///     the last handlers added are the 
    ///     last ones invoked <para/>
    ///     However if the <see cref="RoutingStrategy"/> 
    ///     of the associated <see cref="RoutedEvent"/> 
    ///     is <see cref="RoutingStrategy.Tunnel"/>, 
    ///     the last handlers added are the 
    ///     first ones invoked 
    /// </remarks>
    /// <param name="source">
    ///     <see cref="RoutedEventArgs.Source"/> 
    ///     that raised the RoutedEvent
    /// </param>
    /// <param name="args">
    ///     <see cref="RoutedEventArgs"/> that carry
    ///     all the details specific to this RoutedEvent
    /// </param>
    internal void InvokeHandlers(object source, RoutedEventArgs args) => InvokeHandlersImpl(source, args, false);

    internal void ReInvokeHandlers(object source, RoutedEventArgs args) => InvokeHandlersImpl(source, args, true);

    internal void InvokeHandlersImpl(object source, RoutedEventArgs args, bool reRaised)
    {
        ArgumentNullException.ThrowIfNull(source);
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
            int endSourceChangeIndex = 0;

            // If the RoutingStrategy of the associated is 
            // Bubble the handlers for the last target 
            // added are the last ones invoked
            // Invoke class listeners
            for (int i = 0; i < _routeItemList.Count; i++)
            {
                // Query for new source only if we are 
                // past the range of the previous source change
                if (i >= endSourceChangeIndex)
                {
                    // Get the source at this point in the bubble route and also 
                    // the index at which this source change seizes to apply
                    object newSource = GetBubbleSource(i, out endSourceChangeIndex);

                    // Set appropriate source
                    // The first call to setsource seems redundant 
                    // but is necessary because the source could have 
                    // been modified during BuildRoute call and hence 
                    // may need to be reset to the original source.
                    // Note: we skip this logic if reRaised is set, which is done when we're trying
                    //       to convert MouseDown/Up into a MouseLeft/RightButtonDown/Up
                    if (!reRaised)
                    {
                        if (newSource is null)
                        {
                            args.Source = source;
                        }
                        else
                        {
                            args.Source = newSource;
                        }
                    }
                }

                // Invoke listeners

                _routeItemList[i].InvokeHandler(args);
            }
        }
        else
        {
            int startSourceChangeIndex = _routeItemList.Count;
            int endTargetIndex = _routeItemList.Count - 1;
            int startTargetIndex;

            // If the RoutingStrategy of the associated is 
            // Tunnel the handlers for the last target 
            // added are the first ones invoked
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
                    // Query for new source only if we are 
                    // past the range of the previous source change
                    if (i < startSourceChangeIndex)
                    {
                        // Get the source at this point in the tunnel route and also 
                        // the index at which this source change seizes to apply
                        object newSource = GetTunnelSource(i, out startSourceChangeIndex);

                        // Set appropriate source
                        // The first call to setsource seems redundant 
                        // but is necessary because the source could have 
                        // been modified during BuildRoute call and hence 
                        // may need to be reset to the original source.
                        if (newSource is null)
                        {
                            args.Source = source;
                        }
                        else
                        {
                            args.Source = newSource;
                        }
                    }

                    // Invoke listeners
                    _routeItemList[i].InvokeHandler(args);
                }

                endTargetIndex = startTargetIndex;
            }
        }
    }

    internal RoutedEvent RoutedEvent { get; set; }

    // Add the given source to the source item list
    // indicating what the source will be this point 
    // onwards in the route
    internal void AddSource(object source)
    {
        int startIndex = _routeItemList.Count;
        _sourceItemList.Add(new SourceItem(startIndex, source));
    }

    // Determine what the RoutedEventArgs.Source should be, at this
    // point in the bubble. Also the endIndex output parameter tells 
    // you the exact index of the handlersList at which this source 
    // change ceases to apply
    private object GetBubbleSource(int index, out int endIndex)
    {
        // If the Source never changes during the route execution,
        // then we're done (just return null).
        if (_sourceItemList.Count == 0)
        {
            endIndex = _routeItemList.Count;
            return null;
        }

        // Similarly, if we're not to the point of the route of the first Source
        // change, simply return null.
        if (index < _sourceItemList[0].StartIndex)
        {
            endIndex = _sourceItemList[0].StartIndex;
            return null;
        }

        // See if we should be using one of the intermediate
        // sources
        for (int i = 0; i < _sourceItemList.Count - 1; i++)
        {
            if (index >= _sourceItemList[i].StartIndex && index < _sourceItemList[i + 1].StartIndex)
            {
                endIndex = _sourceItemList[i + 1].StartIndex;
                return _sourceItemList[i].Source;
            }
        }

        // If we get here, we're on the last one,
        // so return that.            
        endIndex = _routeItemList.Count;
        return _sourceItemList[_sourceItemList.Count - 1].Source;
    }

    // Determine what the RoutedEventArgs.Source should be, at this
    // point in the tunnel. Also the startIndex output parameter tells 
    // you the exact index of the handlersList at which this source 
    // change starts to apply
    private object GetTunnelSource(int index, out int startIndex)
    {
        // If the Source never changes during the route execution,
        // then we're done (just return null).
        if (_sourceItemList.Count == 0)
        {
            startIndex = 0;
            return null;
        }

        // Similarly, if we're past the point of the route of the first Source
        // change, simply return null.
        if (index < _sourceItemList[0].StartIndex)
        {
            startIndex = 0;
            return null;
        }

        // See if we should be using one of the intermediate
        // sources
        for (int i = 0; i < _sourceItemList.Count - 1; i++)
        {
            if (index >= _sourceItemList[i].StartIndex && index < _sourceItemList[i + 1].StartIndex)
            {
                startIndex = _sourceItemList[i].StartIndex;
                return _sourceItemList[i].Source;
            }
        }

        // If we get here, we're on the last one, so return that.            
        startIndex = _sourceItemList[_sourceItemList.Count - 1].StartIndex;
        return _sourceItemList[_sourceItemList.Count - 1].Source;
    }

    /// <summary>
    ///     Cleanup all the references within the data
    /// </summary>
    internal void Clear()
    {
        RoutedEvent = null;

        _routeItemList.Clear();
        _sourceItemList.Clear();
    }

    // Stores the routed event handlers to be 
    // invoked for the associated RoutedEvent
    private readonly List<RouteItem> _routeItemList;

    // Stores Source Items for separated trees
    private readonly List<SourceItem> _sourceItemList;
}
