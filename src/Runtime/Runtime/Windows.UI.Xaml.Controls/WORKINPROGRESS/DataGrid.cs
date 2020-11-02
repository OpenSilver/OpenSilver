

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
    /// Displays data in a customizable grid.
    /// </summary>
    public partial class DataGrid
    {
        /// <summary>
        /// Occurs when a different cell becomes the current cell.
        /// </summary>
        public event EventHandler<EventArgs> CurrentCellChanged;

        /// <summary>
        /// Gets or sets the column that contains the current cell.
        /// </summary>
        /// <returns>
        /// The column that contains the current cell.
        /// </returns>
        public DataGridColumn CurrentColumn { get; set; }

        /// <summary>
        /// Scrolls the <see cref="T:System.Windows.Controls.DataGrid" /> vertically to display
        /// the row for the specified data item and scrolls the <see cref="T:System.Windows.Controls.DataGrid" />
        /// horizontally to display the specified column.
        /// </summary>
        /// <param name="item">
        /// The data item (row) to scroll to.
        /// </param>
        /// <param name="column">
        /// The column to scroll to.
        /// </param>
        public void ScrollIntoView(object item, DataGridColumn column)
        {
            throw new NotImplementedException();
        }
    }
}
#endif