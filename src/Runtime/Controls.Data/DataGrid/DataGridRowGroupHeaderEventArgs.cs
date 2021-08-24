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
    /// EventArgs used for the DataGrid's LoadingRowGroup and UnloadingRowGroup events
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataGridRowGroupHeaderEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a DataGridRowGroupHeaderEventArgs instance
        /// </summary>
        /// <param name="rowGroupHeader"></param>
        public DataGridRowGroupHeaderEventArgs(DataGridRowGroupHeader rowGroupHeader)
        {
            this.RowGroupHeader = rowGroupHeader;
        }

        /// <summary>
        /// DataGridRowGroupHeader associated with this instance
        /// </summary>
        public DataGridRowGroupHeader RowGroupHeader
        {
            get;
            private set;
        }
    }
}
