
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
// ----> See the class "System.Windows.RoutedPropertyChangedEventArgs"
#else
namespace Windows.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Provides data about a change in range value for the ValueChanged event.
    /// </summary>
    public sealed class RangeBaseValueChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the RangeBaseValueChangedEventArgs class.
        /// </summary>
        /// <param name="newValue">The new value of a range value property.</param>
        /// <param name="oldValue">The previous value of a range value property.</param>
        public RangeBaseValueChangedEventArgs(double newValue, double oldValue)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        /// <summary>
        /// Gets the new value of a range value property.
        /// </summary>
        public double NewValue { get; private set; }

        /// <summary>
        /// Gets the previous value of a range value property.
        /// </summary>
        public double OldValue { get; private set; }
    }
}
#endif