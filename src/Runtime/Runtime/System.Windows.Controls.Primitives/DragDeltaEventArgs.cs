
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
    /// Provides data for the DragDelta event that occurs one or more times when
    /// a user drags a Thumb control with the mouse.
    /// </summary>
    public class DragDeltaEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragDeltaEventArgs class.
        /// </summary>
        /// <param name="horizontalChange">The horizontal change in the Thumb position since the last DragDelta event.</param>
        /// <param name="verticalChange">The vertical change in the Thumb position since the last DragDelta event.</param>
        public DragDeltaEventArgs(double horizontalChange, double verticalChange)
        {
            this.HorizontalChange = horizontalChange;
            this.VerticalChange = verticalChange;
        }

        /// <summary>
        /// Gets the horizontal change in the Thumb position since the last DragDelta event.
        /// </summary>
        public double HorizontalChange { get; private set; }

        /// <summary>
        /// Gets the vertical change in the Thumb position since the last DragDelta event.
        /// </summary>
        public double VerticalChange { get; private set; }
    }
}
