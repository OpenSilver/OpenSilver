
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
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// controls that can be set to a value within a range.
    /// </summary>
    public interface IRangeValueProvider
    {
        /// <summary>
        /// Gets a value that indicates whether the value of a control is read-only.
        /// </summary>
        /// <returns>
        /// true if the value is read-only; false if it can be modified.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the value that is added to or subtracted from the <see cref="Value" /> 
        /// property when a large change is made, such as with the PAGE DOWN key.
        /// </summary>
        /// <returns>
        /// The large-change value that is supported by the control, or null if the control 
        /// does not support <see cref="LargeChange" />.
        /// </returns>
        double LargeChange { get; }

        /// <summary>
        /// Gets the maximum range value that is supported by the control.
        /// </summary>
        /// <returns>
        /// The maximum value that is supported by the control, or null if the control does 
        /// not support <see cref="Maximum" />.
        /// </returns>
        double Maximum { get; }

        /// <summary>
        /// Gets the minimum range value that is supported by the control.
        /// </summary>
        /// <returns>
        /// The minimum value that is supported by the control, or null if the control does 
        /// not support <see cref="Minimum" />.
        /// </returns>
        double Minimum { get; }

        /// <summary>
        /// Sets the value of the control.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        void SetValue(double value);

        /// <summary>
        /// Gets the value that is added to or subtracted from the <see cref="Value" /> 
        /// property when a small change is made, such as with an arrow key.
        /// </summary>
        /// <returns>
        /// The small-change value supported by the control, or null if the control does 
        /// not support <see cref="SmallChange" />.
        /// </returns>
        double SmallChange { get; }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <returns>
        /// The value of the control, or null if the control does not support <see cref="Value" />.
        /// </returns>
        double Value { get; }
    }
}
