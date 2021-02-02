

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

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{    //
    // Summary:
    //     Provides data for the System.Windows.Controls.DataGrid.CellEditEnding event.
    public class DataGridCellEditEndingEventArgs : CancelEventArgs
    {
        //
        // Summary:
        //     Instantiates a new instance of the System.Windows.Controls.DataGridCellEditEndingEventArgs
        //     class.
        //
        // Parameters:
        //   column:
        //     The column that contains the cell that is about to exit edit mode.
        //
        //   row:
        //     The row that contains the cell that is about to exit edit mode.
        //
        //   editingElement:
        //     The element displayed when the cell is in edit mode.
        //
        //   editAction:
        //     The System.Windows.Controls.DataGridEditAction that indicates whether the edit
        //     will be committed or canceled.
        public DataGridCellEditEndingEventArgs(DataGridColumn column, DataGridRow row, FrameworkElement editingElement, DataGridEditAction editAction)
        { }

        //
        // Summary:
        //     Gets the column that contains the cell that is about to exit edit mode.
        //
        // Returns:
        //     The column that contains the cell that is about to exit edit mode.
        public DataGridColumn Column { get; }
        //
        // Summary:
        //     Gets the System.Windows.Controls.DataGridEditAction that indicates whether the
        //     edit will be committed or canceled.
        //
        // Returns:
        //     An enumeration value that indicates whether this edit event will be committed
        //     or canceled.
        public DataGridEditAction EditAction { get; }
        //
        // Summary:
        //     Gets the element displayed when the cell is in edit mode.
        //
        // Returns:
        //     The element displayed when the cell is in edit mode.
        public FrameworkElement EditingElement { get; }
        //
        // Summary:
        //     Gets the row that contains the cell that is about to exit edit mode.
        //
        // Returns:
        //     The row that contains the cell that is about to exit edit mode.
        public DataGridRow Row { get; }
    }
}
#endif