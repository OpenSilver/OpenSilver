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
/// Provides information just before a cell exits editing mode.
/// </summary>
/// <QualityBand>Preview</QualityBand>
public class DataGridCellEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Instantiates a new instance of this class.
        /// </summary>
        /// <param name="column">The column of the cell that is about to exit edit mode.</param>
        /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
        /// <param name="editingElement">The editing element within the cell.</param>
        /// <param name="editAction">The editing action that will be taken.</param>
        public DataGridCellEditEndingEventArgs(DataGridColumn column,
                                               DataGridRow row,
                                               FrameworkElement editingElement,
                                               DataGridEditAction editAction)
        {
            this.Column = column;
            this.Row = row;
            this.EditingElement = editingElement;
            this.EditAction = editAction;
        }

        #region Properties

        /// <summary>
        /// The column of the cell that is about to exit edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// The edit action to take when leaving edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get;
            private set;
        }

        /// <summary>
        /// The editing element within the cell. 
        /// </summary>
        public FrameworkElement EditingElement
        {
            get;
            private set;
        }

        /// <summary>
        /// The row container of the cell container that is about to exit edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
