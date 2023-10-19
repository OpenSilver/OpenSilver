
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

namespace System.Windows
{
    /// <summary>
    /// Provides data about a change in value to a dependency property as reported by
    /// particular routed events, including the previous and current value of the property
    /// that changed.
    /// </summary>
    /// <typeparam name="T">The type of the dependency property that has changed.</typeparam>
    public partial class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
    { 
        /// <summary>
        /// Initializes a new instance of the System.Windows.RoutedPropertyChangedEventArgs`1
        /// class, with provided old and new values.
        /// </summary>
        /// <param name="oldValue">The previous value of the property, before the event was raised.</param>
        /// <param name="newValue">The current value of the property at the time of the event.</param>
        public RoutedPropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        /// <summary>
        /// Gets the new value of a property as reported by a property-changed event.
        /// </summary>
        public T NewValue { get; private set; }

        /// <summary>
        /// Gets the previous value of the property as reported by a property-changed event.
        /// </summary>
        public T OldValue { get; private set; }
    }
}
