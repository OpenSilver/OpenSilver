

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


#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies constants that define which <see cref="T:System.Windows.Controls.DataGrid" /> header cells are displayed.
    /// </summary>
    [Flags]
    public enum DataGridHeadersVisibility
    {
        /// <summary>Both column and row header cells are displayed.</summary>
        All = 3,
        /// <summary>Only column header cells are displayed.</summary>
        Column = 1,
        /// <summary>Only row header cells are displayed.</summary>
        Row = 2,
        /// <summary>No header cells are displayed.</summary>
        None = 0,
    }
}
#endif