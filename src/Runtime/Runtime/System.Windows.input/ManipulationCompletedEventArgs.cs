
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Provides data for the <see cref="UIElement.ManipulationCompleted"/> event.
/// </summary>
[OpenSilver.NotImplemented]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ManipulationCompletedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManipulationCompletedEventArgs"/> class.
    /// </summary>
    public ManipulationCompletedEventArgs() { }

    /// <summary>
    /// Gets the velocities that are used for the manipulation.
    /// </summary>
    /// <returns>
    /// The velocities that are used for the manipulation.
    /// </returns>
    public ManipulationVelocities FinalVelocities { get; }

    /// <summary>
    /// Gets whether the <see cref="UIElement.ManipulationCompleted"/> event occurs during inertia.
    /// </summary>
    /// <returns>
    /// true if the <see cref="UIElement.ManipulationCompleted"/> event occurs during inertia; false if the event 
    /// occurs while the user's input device has contact with the element.
    /// </returns>
    public bool IsInertial { get; }

    /// <summary>
    /// Gets the container that defines the coordinates for the manipulation.
    /// </summary>
    /// <returns>
    /// The container element.
    /// </returns>
    public UIElement ManipulationContainer { get; }

    /// <summary>
    /// Gets the point from which the manipulation originated.
    /// </summary>
    /// <returns>
    /// The point from which the manipulation originated.
    /// </returns>
    public Point ManipulationOrigin { get; }

    /// <summary>
    /// Gets the total transformation that occurs during the current manipulation.
    /// </summary>
    /// <returns>
    /// The total transformation that occurs during the current manipulation.
    /// </returns>
    public ManipulationDelta TotalManipulation { get; }
}
