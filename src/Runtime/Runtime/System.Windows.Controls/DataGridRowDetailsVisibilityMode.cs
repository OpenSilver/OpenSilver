

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
    /// Specifies constants that define when <see cref="T:System.Windows.Controls.DataGrid" /> row
    /// details are displayed.
    /// </summary>
    public enum DataGridRowDetailsVisibilityMode
    {
        /// <summary>The row details section is displayed only for selected rows.</summary>
        VisibleWhenSelected,
        /// <summary>The row details section is displayed for all rows.</summary>
        Visible,
        /// <summary>The row details section is not displayed for any rows.</summary>
        Collapsed
    }
}
