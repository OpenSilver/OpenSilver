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
/// Provides information just after a cell has exited editing mode.
/// </summary>
/// <QualityBand>Preview</QualityBand>
public class DataGridCellEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Instantiates a new instance of this class.
        /// </summary>
        /// <param name="column">The column of the cell that has just exited edit mode.</param>
        /// <param name="row">The row container of the cell container that has just exited edit mode.</param>
        /// <param name="editAction">The editing action that has been taken.</param>
        public DataGridCellEditEndedEventArgs(DataGridColumn column, DataGridRow row, DataGridEditAction editAction)
        {
            this.Column = column;
            this.Row = row;
            this.EditAction = editAction;
        }

        #region Properties

        /// <summary>
        /// The column of the cell that has just exited edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// The edit action taken when leaving edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get;
            private set;
        }

        /// <summary>
        /// The row container of the cell container that has just exited edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
