// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the <see cref="E:System.Windows.Controls.DataGrid.ColumnReordering" /> event.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridColumnReorderingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridColumnReorderingEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridColumn"></param>
        public DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn)
        {
            this.Column = dataGridColumn;
        }

        #region Public Properties

        /// <summary>
        /// The column being moved.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// The popup indicator displayed while dragging.  If null and Handled = true, then do not display a tooltip.
        /// </summary>
        public Control DragIndicator
        {
            get;
            set;
        }

        /// <summary>
        /// UIElement to display at the insertion position.  If null and Handled = true, then do not display an insertion indicator.
        /// </summary>
        public Control DropLocationIndicator
        {
            get;
            set;
        }

        #endregion Public Properties
    }
}
