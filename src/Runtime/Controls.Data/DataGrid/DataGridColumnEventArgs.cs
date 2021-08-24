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
    /// Provides data for <see cref="T:System.Windows.Controls.DataGrid" /> column-related events.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridColumnEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridColumnEventArgs" /> class.
        /// </summary>
        /// <param name="column">The column that the event occurs for.</param>
        public DataGridColumnEventArgs(DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            this.Column = column;
        }

        /// <summary>
        /// Gets the column that the event occurs for.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }
    }
}
