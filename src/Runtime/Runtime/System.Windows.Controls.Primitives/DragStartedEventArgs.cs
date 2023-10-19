
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
    /// Provides data for the DragStarted event that occurs when a user drags a Thumb control with the mouse.
    /// </summary>
    public class DragStartedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragStartedEventArgs class.
        /// </summary>
        /// <param name="horizontalOffset">The horizontal distance between the current mouse position and the thumb coordinates.</param>
        /// <param name="verticalOffset">The vertical distance between the current mouse position and the thumb coordinates.</param>
        public DragStartedEventArgs(double horizontalOffset, double verticalOffset)
        {
            this.HorizontalOffset = horizontalOffset;
            this.VerticalOffset = verticalOffset;
        }

        /// <summary>
        /// Gets the horizontal distance between the current mouse position and the thumb
        /// coordinates.
        /// </summary>
        public double HorizontalOffset { get; private set; }

        /// <summary>
        /// Gets the vertical distance between the current mouse position and the thumb
        /// coordinates.
        /// </summary>
        public double VerticalOffset { get; private set; }
    }
}
