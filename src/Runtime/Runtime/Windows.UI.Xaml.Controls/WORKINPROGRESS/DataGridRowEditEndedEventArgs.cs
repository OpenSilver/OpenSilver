﻿

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
    //
    // Summary:
    //     Provides data for the System.Windows.Controls.DataGrid.RowEditEnded event.
    public class DataGridRowEditEndedEventArgs : EventArgs
    {
        //
        // Summary:
        //     Instantiates a new instance of the System.Windows.Controls.DataGridRowEditEndedEventArgs
        //     class.
        //
        // Parameters:
        //   row:
        //     The row that has just exited edit mode.
        //
        //   editAction:
        //     The System.Windows.Controls.DataGridEditAction that indicates whether the edit
        //     was committed or canceled.
        public DataGridRowEditEndedEventArgs(DataGridRow row, DataGridEditAction editAction)
        { }

        //
        // Summary:
        //     Gets the System.Windows.Controls.DataGridEditAction that indicates whether the
        //     edit was committed or canceled.
        //
        // Returns:
        //     An enumeration value that indicates whether this edit event was committed or
        //     canceled.
        public DataGridEditAction EditAction { get; }
        //
        // Summary:
        //     Gets the row that has just exited edit mode.
        //
        // Returns:
        //     The row that has just exited edit mode.
        public DataGridRow Row { get; }
    }
}
#endif