
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
/// Provides information about the <see cref="Thumb.DragCompleted"/> event that occurs when a user 
/// completes a drag operation with the mouse of a <see cref="Thumb"/> control.
/// </summary>
public class DragCompletedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="horizontalChange">
    /// The horizontal change in position of the <see cref="Thumb"/> control, resulting from the 
    /// drag operation.
    /// </param>
    /// <param name="verticalChange">
    /// The vertical change in position of the <see cref="Thumb"/> control, resulting from the drag 
    /// operation.
    /// </param>
    /// <param name="canceled">
    /// A Boolean value that indicates whether the drag operation was canceled by a call to the 
    /// <see cref="Thumb.CancelDrag"/> method.
    /// </param>
    public DragCompletedEventArgs(double horizontalChange, double verticalChange, bool canceled)
    {
        HorizontalChange = horizontalChange;
        VerticalChange = verticalChange;
        Canceled = canceled;
        RoutedEvent = Thumb.DragCompletedEvent;
    }

    /// <summary>
    /// Gets whether the drag operation for a <see cref="Thumb"/> was canceled by a call to the 
    /// <see cref="Thumb.CancelDrag"/> method.
    /// </summary>
    /// <returns>
    /// true if a drag operation was canceled; otherwise, false.
    /// </returns>
    public bool Canceled { get; private set; }

    /// <summary>
    /// Gets the horizontal change in position of the <see cref="Thumb"/> after the user drags the 
    /// control with the mouse.
    /// </summary>
    /// <returns>
    /// The horizontal difference between the point at which the user pressed the left mouse button 
    /// and the point at which the user released the button during a drag operation of a <see cref="Thumb"/> 
    /// control. There is no default value.
    /// </returns>
    public double HorizontalChange { get; private set; }

    /// <summary>
    /// Gets the vertical change in position of the <see cref="Thumb"/> after the user drags the 
    /// control with the mouse.
    /// </summary>
    /// <returns>
    /// The vertical difference between the point at which the user pressed the left mouse button 
    /// and the point at which the user released the button during a drag operation of a 
    /// <see cref="Thumb"/> control. There is no default value.
    /// </returns>
    public double VerticalChange { get; private set; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((DragCompletedEventHandler)genericHandler)(genericTarget, this);
}
