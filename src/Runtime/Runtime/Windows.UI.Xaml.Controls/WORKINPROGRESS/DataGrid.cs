

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

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowStyle" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.RowStyle" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register(nameof(RowStyle), typeof(Style), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnRowStylePropertyChanged)));

        /// <summary>
        /// Gets or sets the style that is used when rendering the rows.
        /// </summary>
        /// <returns>
        /// The style applied to rows. The default is null.
        /// </returns>
        public Style RowStyle
        {
            get => GetValue(RowStyleProperty) as Style;
            set => SetValue(RowStyleProperty, value);
        }

        private static void OnRowStylePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.GridLinesVisibility" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.GridLinesVisibility" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridLinesVisibilityProperty = DependencyProperty.Register(nameof(GridLinesVisibility), typeof(DataGridGridLinesVisibility), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnGridLinesVisibilityPropertyChanged)));

        /// <summary>
        /// Gets or sets a value that indicates which grid lines separating inner cells are shown.
        /// </summary>
        /// <returns>
        /// One of the enumeration values indicating which grid lines are shown. The default is <see cref="F:System.Windows.Controls.DataGridGridLinesVisibility.All" />.
        /// </returns>
        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get => (DataGridGridLinesVisibility)GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, (object)value);
        }

        private static void OnGridLinesVisibilityPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
#endif