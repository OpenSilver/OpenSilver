
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

namespace System.Windows.Automation
{
    /// <summary>
    /// Contains values that specify the <see cref="ToggleState" /> of a UI automation element.
    /// </summary>
    public enum ToggleState
    {
        /// <summary>
        /// The UI automation element is not selected, checked, marked, or otherwise activated.
        /// </summary>
        Off,
        /// <summary>
        /// The UI automation element  is selected, checked, marked, or otherwise activated.
        /// </summary>
        On,
        /// <summary>
        /// The UI automation element is in an indeterminate state.
        /// </summary>
        Indeterminate,
    }
}
