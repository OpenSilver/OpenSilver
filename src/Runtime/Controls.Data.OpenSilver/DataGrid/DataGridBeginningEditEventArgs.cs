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
/// Provides data for the <see cref="E:System.Windows.Controls.DataGrid.BeginningEdit" /> event.
/// </summary>
/// <QualityBand>Mature</QualityBand>
public class DataGridBeginningEditEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:System.Windows.Controls.DataGridBeginningEditEventArgs" /> class.
        /// </summary>
        /// <param name="column">
        /// The column that contains the cell to be edited.
        /// </param>
        /// <param name="row">
        /// The row that contains the cell to be edited.
        /// </param>
        /// <param name="editingEventArgs">
        /// Information about the user gesture that caused the cell to enter edit mode.
        /// </param>
        public DataGridBeginningEditEventArgs(DataGridColumn column,
                                              DataGridRow row,
                                              RoutedEventArgs editingEventArgs)
        {
            this.Column = column;
            this.Row = row;
            this.EditingEventArgs = editingEventArgs;
        }

        #region Properties

        /// <summary>
        /// Gets the column that contains the cell to be edited.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about the user gesture that caused the cell to enter edit mode.
        /// </summary>
        public RoutedEventArgs EditingEventArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the row that contains the cell to be edited.
        /// </summary>
        public DataGridRow Row
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
