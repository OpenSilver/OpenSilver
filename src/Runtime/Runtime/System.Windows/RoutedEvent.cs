
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a routed event to the Silverlight event system.
    /// </summary>
    public sealed class RoutedEvent
    {
        private readonly string _name;
        private readonly Type _handlerType;
        private readonly Type _ownerType;

        internal RoutedEvent(string name, Type handlerType, Type ownerType)
        {
            _name = name;
            _handlerType = handlerType;
            _ownerType = ownerType;
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
    }
}
