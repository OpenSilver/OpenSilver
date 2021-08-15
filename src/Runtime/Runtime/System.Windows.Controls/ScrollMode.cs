

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines constants that specify scrolling behavior for ScrollViewer and other
    /// parts involved in scrolling scenarios.
    /// </summary>
    public enum ScrollMode
    {
        /// <summary>
        /// Scrolling is disabled.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Scrolling is enabled.
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// Scrolling is enabled but behavior uses a "rails" manipulation mode.
        /// </summary>
        Auto = 2,
    }
}