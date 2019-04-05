
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if MIGRATION
namespace System.Windows
{
    /// <summary>
    /// Provides data about a change in value to a dependency property as reported by
    /// particular routed events, including the previous and current value of the property
    /// that changed.
    /// </summary>
    /// <typeparam name="T">The type of the dependency property that has changed.</typeparam>
    public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
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
#else
// ----> See the class "RangeBaseValueChangedEventArgs"
#endif