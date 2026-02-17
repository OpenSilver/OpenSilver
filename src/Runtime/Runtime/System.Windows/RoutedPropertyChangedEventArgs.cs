
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

namespace System.Windows;

/// <summary>
/// Represents methods that will handle various routed events that track property value changes.
/// </summary>
/// <typeparam name="T">
/// The type of the property value where changes in value are reported.
/// </typeparam>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data. Specific event definitions will constrain <see cref="RoutedPropertyChangedEventArgs{T}"/>
/// to a type, with the type parameter of the constraint matching the type parameter constraint of a delegate 
/// implementation.
/// </param>
public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> e);

/// <summary>
/// Provides data about a change in value to a dependency property as reported by particular routed events, 
/// including the previous and current value of the property that changed.
/// </summary>
/// <typeparam name="T">
/// The type of the dependency property that has changed.
/// </typeparam>
public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedPropertyChangedEventArgs{T}"/> class, with 
    /// provided old and new values.
    /// </summary>
    /// <param name="oldValue">
    /// Previous value of the property, prior to the event being raised.
    /// </param>
    /// <param name="newValue">
    /// Current value of the property at the time of the event.
    /// </param>
    public RoutedPropertyChangedEventArgs(T oldValue, T newValue)
    {
        NewValue = newValue;
        OldValue = oldValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedPropertyChangedEventArgs{T}"/> class, with 
    /// provided old and new values, and an event identifier.
    /// </summary>
    /// <param name="oldValue">
    /// Previous value of the property, prior to the event being raised.
    /// </param>
    /// <param name="newValue">
    /// Current value of the property at the time of the event.
    /// </param>
    /// <param name="routedEvent">
    /// Identifier of the routed event that this arguments class carries information for.
    /// </param>
    public RoutedPropertyChangedEventArgs(T oldValue, T newValue, RoutedEvent routedEvent)
        : this(oldValue, newValue)
    {
        RoutedEvent = routedEvent;
    }

    /// <summary>
    /// Gets the new value of a property as reported by a property changed event.
    /// </summary>
    /// <returns>
    /// The generic value. In a practical implementation of the <see cref="RoutedPropertyChangedEventArgs{T}"/>, 
    /// the generic type of this property is replaced with the constrained type of the implementation.
    /// </returns>
    public T NewValue { get; private set; }

    /// <summary>
    /// Gets the previous value of the property as reported by a property changed event.
    /// </summary>
    /// <returns>
    /// The generic value. In a practical implementation of the <see cref="RoutedPropertyChangedEventArgs{T}"/>,
    /// the generic type of this property is replaced with the constrained type of the implementation.
    /// </returns>
    public T OldValue { get; private set; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
    {
        ((RoutedPropertyChangedEventHandler<T>)genericHandler)(genericTarget, this);
    }
}
