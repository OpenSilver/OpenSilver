

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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies constants that define which grid lines separating <see cref="T:System.Windows.Controls.DataGrid" /> inner cells are shown.
    /// </summary>
    [Flags]
    public enum DataGridGridLinesVisibility
    {
        /// <summary>No grid lines are shown.</summary>
        None = 0,
        /// <summary>Only horizontal grid lines, which separate rows, are shown.</summary>
        Horizontal = 1,
        /// <summary>Only vertical grid lines, which separate columns, are shown.</summary>
        Vertical = 2,
        /// <summary>Both horizontal and vertical grid lines are shown.</summary>
        All = Vertical | Horizontal, // 0x00000003
    }
}
