
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

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Provides data for the DragCompleted event that occurs when a user completes
    /// a drag operation with the mouse of a Thumb control.
    /// </summary>
    public class DragCompletedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragCompletedEventArgs class.
        /// </summary>
        /// <param name="horizontalChange">The horizontal change in position of the Thumb control, resulting from the drag operation.</param>
        /// <param name="verticalChange">The vertical change in position of the Thumb control, resulting from the drag operation.</param>
        /// <param name="canceled">A value that indicates whether the drag operation was canceled by a call to the CancelDrag method.</param>
        public DragCompletedEventArgs(double horizontalChange, double verticalChange, bool canceled)
        {
            this.HorizontalChange = horizontalChange;
            this.VerticalChange = verticalChange;
            this.Canceled = canceled;
        }

        /// <summary>
        /// Gets a value that indicates whether the drag operation was canceled.
        /// </summary>
        public bool Canceled { get; private set; }

        /// <summary>
        /// Gets the horizontal distance between the current mouse position and the thumb
        /// coordinates.
        /// </summary>
        public double HorizontalChange { get; private set;}

        /// <summary>
        /// Gets the vertical distance between the current mouse position and the thumb coordinates.
        /// </summary>
        public double VerticalChange { get; private set; }
    }
}
