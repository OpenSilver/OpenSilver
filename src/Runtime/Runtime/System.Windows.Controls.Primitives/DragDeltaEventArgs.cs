
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
/// Provides information about the <see cref="Thumb.DragDelta"/> event that occurs one or more times when 
/// a user drags a <see cref="Thumb"/> control with the mouse.
/// </summary>
public class DragDeltaEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragDeltaEventArgs"/> class.
    /// </summary>
    /// <param name="horizontalChange">
    /// The horizontal change in the <see cref="Thumb"/> position since the last <see cref="Thumb.DragDelta"/> 
    /// event.
    /// </param>
    /// <param name="verticalChange">
    /// The vertical change in the <see cref="Thumb"/> position since the last <see cref="Thumb.DragDelta"/> 
    /// event.
    /// </param>
    public DragDeltaEventArgs(double horizontalChange, double verticalChange)
    {
        HorizontalChange = horizontalChange;
        VerticalChange = verticalChange;
        RoutedEvent = Thumb.DragDeltaEvent;
    }

    /// <summary>
    /// Gets the horizontal distance that the mouse has moved since the previous <see cref="Thumb.DragDelta"/>
    /// event when the user drags the <see cref="Thumb"/> control with the mouse.
    /// </summary>
    /// <returns>
    /// A horizontal change in position of the mouse during a drag operation. There is no default value.
    /// </returns>
    public double HorizontalChange { get; }

    /// <summary>
    /// Gets the vertical distance that the mouse has moved since the previous <see cref="Thumb.DragDelta"/>
    /// event when the user drags the <see cref="Thumb"/> with the mouse.
    /// </summary>
    /// <returns>
    /// A vertical change in position of the mouse during a drag operation. There is no default value.
    /// </returns>
    public double VerticalChange { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((DragDeltaEventHandler)genericHandler)(genericTarget, this);
}
