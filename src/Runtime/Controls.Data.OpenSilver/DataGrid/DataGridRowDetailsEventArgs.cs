// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the <see cref="E:System.Windows.Controls.DataGrid.LoadingRowDetails" />, <see cref="E:System.Windows.Controls.DataGrid.UnloadingRowDetails" />, 
    /// and <see cref="E:System.Windows.Controls.DataGrid.RowDetailsVisibilityChanged" /> events.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridRowDetailsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridRowDetailsEventArgs" /> class. 
        /// </summary>
        /// <param name="row">The row that the event occurs for.</param>
        /// <param name="detailsElement">The row details section as a framework element.</param>
        public DataGridRowDetailsEventArgs(DataGridRow row, FrameworkElement detailsElement)
        {
            this.Row = row;
            this.DetailsElement = detailsElement;
        }

        /// <summary>
        /// Gets the row details section as a framework element.
        /// </summary>
        public FrameworkElement DetailsElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the row that the event occurs for.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }
    }
}
