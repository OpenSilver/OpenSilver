
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
using System.Diagnostics;

namespace System.Windows
{
    /// <summary>
    /// Represents a routed event to the Silverlight event system.
    /// </summary>
    public sealed class RoutedEvent
    {
        private readonly string _name;
        private readonly Type _handlerType;
        private readonly Type _ownerType;
        private readonly RoutingStrategy _routingStrategy;
        private Delegate _classHandler;

        internal RoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            _name = name;
            _routingStrategy = routingStrategy;
            _handlerType = handlerType;
            _ownerType = ownerType;
        }

        /// <summary>
        ///     Returns the <see cref="RoutingStrategy"/> 
        ///     of the RoutedEvent
        /// </summary>
        /// <ExternalAPI/>
        internal RoutingStrategy RoutingStrategy
        {
            get { return _routingStrategy; }
        }

        /// <summary>
        /// Returns the string representation of the routed event.
        /// </summary>
        /// <returns>
        /// The name of the routed event.
        /// </returns>
        public override string ToString()
        {
            return $"{_ownerType.Name}.{_name}";
        }

        // Check to see if the given delegate is a legal handler for this type.
        //  It either needs to be a type that the registering class knows how to
        //  handle, or a RoutedEventHandler which we can handle without the help
        //  of the registering class.
        internal bool IsLegalHandler(Delegate handler)
        {
            Type handlerType = handler.GetType();

            return handlerType == _handlerType || handlerType == typeof(RoutedEventHandler);
        }

        internal static void RegisterClassHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent  == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            routedEvent._classHandler = handler;
        }

        internal static Delegate GetClassHandler(RoutedEvent routedEvent)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            return routedEvent._classHandler;
        }
    }

    /// <summary>
    ///     Routing Strategy can be either of or Bubble or Direct
    /// </summary>
    internal enum RoutingStrategy
    {
        /// <summary>
        ///     Bubble 
        /// </summary>
        /// <remarks>
        ///     Route the event starting at the source 
        ///     and ending with the root of the visual tree
        /// </remarks>
        Bubble,

        /// <summary>
        ///     Direct 
        /// </summary>
        /// <remarks>
        ///     Raise the event at the source only.
        /// </remarks>
        Direct
    }
}
