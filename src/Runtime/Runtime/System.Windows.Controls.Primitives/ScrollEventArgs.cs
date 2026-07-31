
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

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Represents the method that will handle the <see cref="ScrollBar.Scroll"/> routed event that 
/// occurs when the <see cref="Thumb"/> of a <see cref="ScrollBar"/> moves.
/// </summary>
public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

/// <summary>
/// Provides data for a <see cref="ScrollBar.Scroll"/> event that occurs when the <see cref="Thumb"/> 
/// of a <see cref="ScrollBar"/> moves.
/// </summary>
public sealed class ScrollEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes an instance of the <see cref="ScrollEventArgs"/> class by using the specified
    /// <see cref="Primitives.ScrollEventType"/> enumeration value and the new location of the 
    /// <see cref="Thumb"/> control in the <see cref="ScrollBar"/>.
    /// </summary>
    /// <param name="newValue">
    /// The value that corresponds to the new location of the <see cref="Thumb"/> in the 
    /// <see cref="ScrollBar"/>.
    /// </param>
    /// <param name="scrollEventType">
    /// A <see cref="Primitives.ScrollEventType"/> enumeration value that describes the type of 
    /// <see cref="Thumb"/> movement that caused the event.
    /// </param>
    public ScrollEventArgs(double newValue, ScrollEventType scrollEventType)
        : this(scrollEventType, newValue)
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="ScrollEventArgs"/> class by using the specified
    /// <see cref="Primitives.ScrollEventType"/> enumeration value and the new location of the 
    /// <see cref="Thumb"/> control in the <see cref="ScrollBar"/>.
    /// </summary>
    /// <param name="scrollEventType">
    /// A <see cref="Primitives.ScrollEventType"/> enumeration value that describes the type of 
    /// <see cref="Thumb"/> movement that caused the event.
    /// </param>
    /// <param name="newValue">
    /// The value that corresponds to the new location of the <see cref="Thumb"/> in the 
    /// <see cref="ScrollBar"/>.
    /// </param>
    public ScrollEventArgs(ScrollEventType scrollEventType, double newValue)
    {
        NewValue = newValue;
        ScrollEventType = scrollEventType;
        RoutedEvent = ScrollBar.ScrollEvent;
    }

    /// <summary>
    /// Gets a value that represents the new location of the <see cref="Thumb"/> in the 
    /// <see cref="ScrollBar"/>.
    /// </summary>
    /// <returns>
    /// The value that corresponds to the new position of the <see cref="Thumb"/> in the 
    /// <see cref="ScrollBar"/>.
    /// </returns>
    public double NewValue { get; }

    /// <summary>
    /// Gets the <see cref="Primitives.ScrollEventType"/> enumeration value that describes 
    /// the change in the <see cref="Thumb"/> position that caused this event.
    /// </summary>
    /// <returns>
    /// A <see cref="Primitives.ScrollEventType"/> enumeration value that describes the type 
    /// of <see cref="Thumb"/> movement that caused the <see cref="ScrollBar.Scroll"/> event.
    /// </returns>
    public ScrollEventType ScrollEventType { get; }

    /// <summary>
    /// Performs the appropriate type casting to call the type-safe <see cref="ScrollEventHandler"/>
    /// delegate for the <see cref="ScrollBar.Scroll"/> event.
    /// </summary>
    /// <param name="genericHandler">
    /// The event handler to call.
    /// </param>
    /// <param name="genericTarget">
    /// The current object along the event's route.
    /// </param>
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        => ((ScrollEventHandler)genericHandler)(genericTarget, this);
}
