
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

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// controls that have an intrinsic value that does not span a range and that can 
    /// be represented as a string.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Sets the value of a control.
        /// </summary>
        /// <param name="value">
        /// The value to set. The provider is responsible for converting the value to 
        /// the appropriate data type.
        /// </param>
        void SetValue(string value);

        /// <summary>
        /// Gets a value that indicates whether the value of a control is read-only.
        /// </summary>
        /// <returns>
        /// true if the value is read-only; false if it can be modified.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <returns>
        /// The value of the control.
        /// </returns>
        string Value { get; }
    }
}
