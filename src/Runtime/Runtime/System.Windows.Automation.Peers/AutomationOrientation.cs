
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

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Specifies the orientation direction in which a control can be presented.
    /// </summary>
    public enum AutomationOrientation
    {
        /// <summary>
        /// The control does not have an orientation.
        /// </summary>
        None,
        /// <summary>
        /// The control is presented horizontally.
        /// </summary>
        Horizontal,
        /// <summary>
        /// The control is presented vertically.
        /// </summary>
        Vertical,
    }
}
