
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
    /// Provides data for the Scroll event.
    /// </summary>
    public sealed class ScrollEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ScrollEventArgs class.
        /// </summary>
        /// <param name="newValue">The new Value of the ScrollBar.</param>
        /// <param name="scrollEventType">A ScrollEventType describing the event.</param>
        public ScrollEventArgs(double newValue, ScrollEventType scrollEventType)
        {
            this.NewValue = newValue;
            this.ScrollEventType = scrollEventType;
        }

        /// <summary>
        /// Gets the new Value of the ScrollBar.
        /// </summary>
        public double NewValue { get; private set; }

        /// <summary>
        /// Gets a ScrollEventType describing the event.
        /// </summary>
        public ScrollEventType ScrollEventType { get; private set; }
    }
}
