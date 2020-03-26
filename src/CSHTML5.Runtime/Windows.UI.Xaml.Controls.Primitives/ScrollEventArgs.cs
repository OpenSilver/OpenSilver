

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
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Provides data for the Scroll event.
    /// </summary>
    public sealed partial class ScrollEventArgs : RoutedEventArgs
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
