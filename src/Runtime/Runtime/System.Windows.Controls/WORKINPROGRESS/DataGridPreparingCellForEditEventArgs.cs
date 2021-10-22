

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
    //     Provides data for the System.Windows.Controls.DataGrid.PreparingCellForEdit event.
    [OpenSilver.NotImplemented]
    public class DataGridPreparingCellForEditEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Controls.DataGridPreparingCellForEditEventArgs
        //     class.
        //
        // Parameters:
        //   column:
        //     The column that contains the cell to be edited.
        //
        //   row:
        //     The row that contains the cell to be edited.
        //
        //   editingEventArgs:
        //     Information about the user gesture that caused the cell to enter edit mode.
        //
        //   editingElement:
        //     The element that the column displays for a cell in editing mode.
        [OpenSilver.NotImplemented]
        public DataGridPreparingCellForEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs, FrameworkElement editingElement)
        { }

        //
        // Summary:
        //     Gets the column that contains the cell to be edited.
        //
        // Returns:
        //     The column that contains the cell to be edited.
        [OpenSilver.NotImplemented]
        public DataGridColumn Column { get; }
        //
        // Summary:
        //     Gets the element that the column displays for a cell in editing mode.
        //
        // Returns:
        //     The element that the column displays for a cell in editing mode.
        [OpenSilver.NotImplemented]
        public FrameworkElement EditingElement { get; }
        //
        // Summary:
        //     Gets information about the user gesture that caused the cell to enter edit mode.
        //
        // Returns:
        //     Information about the user gesture that caused the cell to enter edit mode.
        [OpenSilver.NotImplemented]
        public RoutedEventArgs EditingEventArgs { get; }
        //
        // Summary:
        //     Gets the row that contains the cell to be edited.
        //
        // Returns:
        //     The row that contains the cell to be edited.
        [OpenSilver.NotImplemented]
        public DataGridRow Row { get; }
    }
}
