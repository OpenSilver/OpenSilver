
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
