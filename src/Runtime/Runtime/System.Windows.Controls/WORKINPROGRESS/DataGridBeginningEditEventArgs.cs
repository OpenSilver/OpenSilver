

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

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //
    // Summary:
    //     Provides data for the System.Windows.Controls.DataGrid.BeginningEdit event.
    [OpenSilver.NotImplemented]
    public class DataGridBeginningEditEventArgs : CancelEventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Controls.DataGridBeginningEditEventArgs
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
        [OpenSilver.NotImplemented]
        public DataGridBeginningEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs)
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
