

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
    /*
    // Summary:
    //     Defines constants that specify whether cells, rows, or both, are used for
    //     selection in a System.Windows.Controls.DataGrid control.
    public enum DataGridSelectionUnit
    {
        // Summary:
        //     Only cells are selectable. Clicking a cell selects the cell. Clicking a row
        //     or column header does nothing.
        Cell = 0,
        //
        // Summary:
        //     Only full rows are selectable. Clicking a cell or a row header selects the
        //     full row.
        FullRow = 1,
        //
        // Summary:
        //     Cells and rows are selectable. Clicking a cell selects only the cell. Clicking
        //     a row header selects the full row.
        CellOrRowHeader = 2,
    }
     * */
}