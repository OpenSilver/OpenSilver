

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


#if MIGRATION
// ----> See the class "System.Windows.RoutedPropertyChangedEventArgs"
#else
namespace Windows.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Provides data about a change in range value for the ValueChanged event.
    /// </summary>
    public sealed partial class RangeBaseValueChangedEventArgs : RoutedEventArgs
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