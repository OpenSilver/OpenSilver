
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
    /// Represents methods that will handle various routed events that track property
    /// value changes.
    /// </summary>
    /// <typeparam name="T">The type of the property value where changes in value are reported.</typeparam>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data. Specific event definitions will constrain System.Windows.RoutedPropertyChangedEventArgs`1
    /// to a type, with the type parameter of the constraint matching the type parameter
    /// constraint of a delegate implementation.</param>
    public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> e);
}
