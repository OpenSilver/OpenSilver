
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Determines the granularity of time selection
    /// by a popup. Hours and minutes are always used.
    /// </summary>
    public enum PopupTimeSelectionMode
    {
        /// <summary>
        /// Hours, Minutes and Seconds.
        /// </summary>
        AllowSecondsSelection,

        /// <summary>
        /// Hours and Minutes.
        /// </summary>
        HoursAndMinutesOnly
    }
}
