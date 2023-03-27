
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
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
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

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
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            RouteItem routeItem = new RouteItem(target, new RoutedEventHandlerInfo(handler, handledEventsToo));

            _routeItemList.Add(routeItem);
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
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            for (int i = 0; i < _routeItemList.Count; i++)
            {
                _routeItemList[i].InvokeHandler(args);
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
}
