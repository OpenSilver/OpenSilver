

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
    /// Defines constants that specify whether single or multiple item selections
    /// are supported by a System.Windows.Controls.DataGrid control.
    /// </summary>
    public enum DataGridSelectionMode
    {
        /// <summary>
        /// Only one item in the System.Windows.Controls.DataGrid can be selected at
        /// a time.
        /// </summary>
        Single = 0,

        /// <summary>
        /// Multiple items in the System.Windows.Controls.DataGrid can be selected at
        /// the same time.
        /// </summary>
        Extended = 1,
    }
}