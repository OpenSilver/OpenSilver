
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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