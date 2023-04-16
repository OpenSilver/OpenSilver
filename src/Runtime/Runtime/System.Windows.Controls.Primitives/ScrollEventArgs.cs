
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
    public sealed class ScrollEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ScrollEventArgs class.
        /// </summary>
        /// <param name="newValue">The new Value of the ScrollBar.</param>
        /// <param name="scrollEventType">A ScrollEventType describing the event.</param>
        public ScrollEventArgs(double newValue, ScrollEventType scrollEventType)
            : this(scrollEventType, newValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollEventArgs"/> class.
        /// </summary>
        /// <param name="scrollEventType">
        /// A <see cref="ScrollEventType"/> describing the event.
        /// </param>
        /// <param name="newValue">
        /// The new <see cref="RangeBase.Value"/> of the <see cref="ScrollBar"/>.
        /// </param>
        public ScrollEventArgs(ScrollEventType scrollEventType, double newValue)
        {
            NewValue = newValue;
            ScrollEventType = scrollEventType;
        }

        /// <summary>
        /// Gets the new <see cref="RangeBase.Value"/> of the <see cref="ScrollBar"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="RangeBase.Value"/> of the <see cref="ScrollBar"/> after the event.
        /// </returns>
        public double NewValue { get; }

        /// <summary>
        /// Gets a <see cref="ScrollEventType"/> describing the event.
        /// </summary>
        /// <returns>
        /// A <see cref="ScrollEventType"/> describing the event.
        /// </returns>
        public ScrollEventType ScrollEventType { get; }
    }
}
