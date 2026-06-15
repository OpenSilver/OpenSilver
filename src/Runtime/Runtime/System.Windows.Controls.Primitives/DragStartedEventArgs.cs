
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
/// Provides information about the <see cref="Thumb.DragStarted"/> event that occurs when a user drags a 
/// <see cref="Thumb"/> control with the mouse.
/// </summary>
public class DragStartedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragStartedEventArgs"/> class.
    /// </summary>
    /// <param name="horizontalOffset">
    /// The horizontal offset of the mouse click with respect to the screen coordinates of the <see cref="Thumb"/>.
    /// </param>
    /// <param name="verticalOffset">
    /// The vertical offset of the mouse click with respect to the screen coordinates of the <see cref="Thumb"/>.
    /// </param>
    public DragStartedEventArgs(double horizontalOffset, double verticalOffset)
    {
        HorizontalOffset = horizontalOffset;
        VerticalOffset = verticalOffset;
        RoutedEvent = Thumb.DragStartedEvent;
    }

    /// <summary>
    /// Gets the horizontal offset of the mouse click relative to the screen coordinates of the <see cref="Thumb"/>.
    /// </summary>
    /// <returns>
    /// The horizontal offset of the mouse click with respect to the upper-left corner of the bounding box of the 
    /// <see cref="Thumb"/>. There is no default value.
    /// </returns>
    public double HorizontalOffset { get; }

    /// <summary>
    /// Gets the vertical offset of the mouse click relative to the screen coordinates of the <see cref="Thumb"/>.
    /// </summary>
    /// <returns>
    /// The horizontal offset of the mouse click with respect to the upper-left corner of the bounding box of the 
    /// <see cref="Thumb"/>. There is no default value.
    /// </returns>
    public double VerticalOffset { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((DragStartedEventHandler)genericHandler)(genericTarget, this);
}
