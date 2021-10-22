

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
    //
    // Summary:
    //     Provides data for the System.Windows.Controls.DataGrid.CellEditEnded event.
    [OpenSilver.NotImplemented]
    public class DataGridCellEditEndedEventArgs : EventArgs
    {
        //
        // Summary:
        //     Instantiates a new instance of the System.Windows.Controls.DataGridCellEditEndedEventArgs
        //     class.
        //
        // Parameters:
        //   column:
        //     The column that contains the cell that has just exited edit mode.
        //
        //   row:
        //     The row that contains the cell that has just exited edit mode.
        //
        //   editAction:
        //     The System.Windows.Controls.DataGridEditAction that indicates whether the edit
        //     was committed or canceled.
        [OpenSilver.NotImplemented]
        public DataGridCellEditEndedEventArgs(DataGridColumn column, DataGridRow row, DataGridEditAction editAction)
        {
            Column = column;
            Row = row;
            EditAction = editAction;
        }

        //
        // Summary:
        //     Gets the column that contains the cell that has just exited edit mode.
        //
        // Returns:
        //     The column that contains the cell that has just exited edit mode.
        [OpenSilver.NotImplemented]
        public DataGridColumn Column { get; }
        //
        // Summary:
        //     Gets the System.Windows.Controls.DataGridEditAction that indicates whether the
        //     edit was committed or canceled.
        //
        // Returns:
        //     An enumeration value that indicates whether this edit event was committed or
        //     canceled.
        [OpenSilver.NotImplemented]
        public DataGridEditAction EditAction { get; }
        //
        // Summary:
        //     Gets the row that contains the cell that has just exited edit mode.
        //
        // Returns:
        //     The row that contains the cell that has just exited edit mode.
        [OpenSilver.NotImplemented]
        public DataGridRow Row { get; }
    }
}
