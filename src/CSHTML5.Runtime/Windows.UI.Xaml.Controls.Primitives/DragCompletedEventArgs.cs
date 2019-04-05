
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
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
