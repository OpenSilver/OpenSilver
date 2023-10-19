
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
    /// Exposes methods and properties to support UI automation client access to controls 
    /// that can cycle through a set of states and maintain a particular state.
    /// </summary>
    public interface IToggleProvider
    {
        /// <summary>
        /// Cycles through the toggle states of a control.
        /// </summary>
        void Toggle();

        /// <summary>
        /// Gets the toggle state of the control.
        /// </summary>
        /// <returns>
        /// The toggle state of the control, as a value of the enumeration.
        /// </returns>
        ToggleState ToggleState { get; }
    }
}
