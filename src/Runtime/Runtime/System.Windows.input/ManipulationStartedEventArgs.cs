
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
/// Provides data for the <see cref="UIElement.ManipulationStarted"/> event.
/// </summary>
[OpenSilver.NotImplemented]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ManipulationStartedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManipulationStartedEventArgs"/> class.
    /// </summary>
    public ManipulationStartedEventArgs() { }

    /// <summary>
    /// Gets the container that defines the coordinates for the manipulation.
    /// </summary>
    /// <returns>
    /// The container element.
    /// </returns>
    public UIElement ManipulationContainer { get; set; }

    /// <summary>
    /// Gets the point from which the manipulation originated.
    /// </summary>
    /// <returns>
    /// The point from which the manipulation originated.
    /// </returns>
    public Point ManipulationOrigin { get; }

    /// <summary>
    /// Completes the manipulation without inertia.
    /// </summary>
    public void Complete() { }
}
