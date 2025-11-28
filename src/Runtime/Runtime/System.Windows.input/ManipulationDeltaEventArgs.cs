
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
/// Provides data for the <see cref="UIElement.ManipulationDelta"/> event.
/// </summary>
[OpenSilver.NotImplemented]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ManipulationDeltaEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManipulationDeltaEventArgs"/> class.
    /// </summary>
    public ManipulationDeltaEventArgs() { }

    /// <summary>
    /// Gets the accumulated changes of the current manipulation, as a <see cref="ManipulationDelta"/>.
    /// </summary>
    /// <returns>
    /// The accumulated changes of the current manipulation.
    /// </returns>
    public ManipulationDelta CumulativeManipulation { get; }

    /// <summary>
    /// Gets the most recent changes of the current manipulation, as a <see cref="ManipulationDelta"/>.
    /// </summary>
    /// <returns>
    /// The most recent changes of the current manipulation.
    /// </returns>
    public ManipulationDelta DeltaManipulation { get; }

    /// <summary>
    /// Gets whether the <see cref="UIElement.ManipulationDelta"/> event occurs during inertia.
    /// </summary>
    /// <returns>
    /// true if the <see cref="UIElement.ManipulationDelta"/> event occurs during inertia; false if the event 
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
    /// Gets the rates of the most recent changes to the manipulation.
    /// </summary>
    /// <returns>
    /// The rates of the most recent changes to the manipulation.
    /// </returns>
    public ManipulationVelocities Velocities { get; }

    /// <summary>
    /// Completes the manipulation without inertia.
    /// </summary>
    public void Complete() { }

    public void StartInertia() { }
}
