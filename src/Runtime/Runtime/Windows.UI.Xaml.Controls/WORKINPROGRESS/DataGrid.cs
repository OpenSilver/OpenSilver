

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
        [OpenSilver.NotImplemented]
        public event EventHandler<EventArgs> CurrentCellChanged;

        /// <summary>
        ///     Called just before a cell will change to edit mode
        ///     to allow handlers to prevent the cell from entering edit mode.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

        /// <summary>
        ///     Raised just before cell editing is ended.
        ///     Gives handlers the opportunity to cancel the operation.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

        /// <summary>
        ///     Called after a cell has changed to editing mode to allow
        ///     handlers to modify the contents of the cell.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

        [OpenSilver.NotImplemented]
        public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

        /// <summary>
        /// Gets or sets the column that contains the current cell.
        /// </summary>
        /// <returns>
        /// The column that contains the current cell.
        /// </returns>
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register(nameof(RowStyle), typeof(Style), typeof(DataGrid), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(DataGrid.OnRowStylePropertyChanged)));
#else
        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register(nameof(RowStyle), typeof(Style), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnRowStylePropertyChanged)));
#endif
        /// <summary>
        /// Gets or sets the style that is used when rendering the rows.
        /// </summary>
        /// <returns>
        /// The style applied to rows. The default is null.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Style RowStyle
        {
            get => GetValue(RowStyleProperty) as Style;
            set => SetValue(RowStyleProperty, value);
        }


        private static void OnRowStylePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.GridLinesVisibility" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.GridLinesVisibility" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty GridLinesVisibilityProperty = DependencyProperty.Register(nameof(GridLinesVisibility), typeof(DataGridGridLinesVisibility), typeof(DataGrid), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(DataGrid.OnGridLinesVisibilityPropertyChanged)));
#else
        public static readonly DependencyProperty GridLinesVisibilityProperty = DependencyProperty.Register(nameof(GridLinesVisibility), typeof(DataGridGridLinesVisibility), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnGridLinesVisibilityPropertyChanged)));
#endif

        /// <summary>
        /// Gets or sets a value that indicates which grid lines separating inner cells are shown.
        /// </summary>
        /// <returns>
        /// One of the enumeration values indicating which grid lines are shown. The default is <see cref="F:System.Windows.Controls.DataGridGridLinesVisibility.All" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get => (DataGridGridLinesVisibility)GetValue(GridLinesVisibilityProperty);
            set => SetValue(GridLinesVisibilityProperty, (object)value);
        }


        private static void OnGridLinesVisibilityPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsVisibilityMode" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsVisibilityMode" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty RowDetailsVisibilityModeProperty = DependencyProperty.Register(nameof(RowDetailsVisibilityMode), typeof(DataGridRowDetailsVisibilityMode), typeof(DataGrid), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure, OnRowDetailsVisibilityModePropertyChanged));
#else
        public static readonly DependencyProperty RowDetailsVisibilityModeProperty = DependencyProperty.Register(nameof(RowDetailsVisibilityMode), typeof(DataGridRowDetailsVisibilityMode), typeof(DataGrid), new PropertyMetadata(OnRowDetailsVisibilityModePropertyChanged));
#endif

        /// <summary>
        /// Gets or sets a value that indicates when the details sections of rows are displayed.
        /// </summary>
        /// <returns>
        /// An enumeration value that specifies the visibility of row details.
        /// The default is <see cref="F:System.Windows.Controls.DataGridRowDetailsVisibilityMode.VisibleWhenSelected" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
        {
            get => (DataGridRowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty);
            set => SetValue(DataGrid.RowDetailsVisibilityModeProperty, value);
        }

        private static void OnRowDetailsVisibilityModePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.VerticalScrollBarVisibility" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.VerticalScrollBarVisibility" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnVerticalScrollBarVisibilityPropertyChanged)));

        /// <summary>
        /// Gets or sets a value that indicates how the vertical scroll bar is displayed.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the vertical scroll bar visibility.
        /// The default is <see cref="F:System.Windows.Controls.ScrollBarVisibility.Auto" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        private static void OnVerticalScrollBarVisibilityPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.HorizontalScrollBarVisibility" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.HorizontalScrollBarVisibility" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnHorizontalScrollBarVisibilityPropertyChanged)));

        /// <summary>
        /// Gets or sets a value that indicates how the horizontal scroll bar is displayed.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the horizontal scroll bar visibility.
        /// The default is <see cref="F:System.Windows.Controls.ScrollBarVisibility.Auto" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        private static void OnHorizontalScrollBarVisibilityPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.CanUserReorderColumns" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.CanUserReorderColumns" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CanUserReorderColumnsProperty = DependencyProperty.Register(nameof(CanUserReorderColumns), typeof(bool), typeof(DataGrid), null);

        /// <summary>
        /// Gets or sets a value that indicates whether the user can change the column display order by dragging column headers with the mouse.
        /// </summary>
        /// <returns>
        /// true if the user can reorder columns; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool CanUserReorderColumns
        {
            get => (bool)GetValue(CanUserReorderColumnsProperty);
            set => SetValue(CanUserReorderColumnsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.HeadersVisibility" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.HeadersVisibility" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty HeadersVisibilityProperty = DependencyProperty.Register(nameof(HeadersVisibility), typeof(DataGridHeadersVisibility), typeof(DataGrid), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(DataGrid.OnHeadersVisibilityPropertyChanged)));
#else
        public static readonly DependencyProperty HeadersVisibilityProperty = DependencyProperty.Register(nameof(HeadersVisibility), typeof(DataGridHeadersVisibility), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnHeadersVisibilityPropertyChanged)));
#endif

        /// <summary>
        /// Gets or sets a value that indicates the visibility of row and column headers.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that indicates the visibility of row and column headers.
        /// The default is <see cref="F:System.Windows.Controls.DataGridHeadersVisibility.Column" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public DataGridHeadersVisibility HeadersVisibility
        {
            get => (DataGridHeadersVisibility)GetValue(HeadersVisibilityProperty);
            set => SetValue(HeadersVisibilityProperty, value);
        }

        private static void OnHeadersVisibilityPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.CanUserResizeColumns" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.CanUserResizeColumns" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CanUserResizeColumnsProperty = DependencyProperty.Register(nameof(CanUserResizeColumns), typeof(bool), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnCanUserResizeColumnsPropertyChanged)));

        /// <summary>
        /// Gets or sets a value that indicates whether the user can adjust column widths using the mouse.
        /// </summary>
        /// <returns>
        /// true if the user can resize columns; otherwise, false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool CanUserResizeColumns
        {
            get => (bool)GetValue(CanUserResizeColumnsProperty);
            set => SetValue(CanUserResizeColumnsProperty, value);
        }

        private static void OnCanUserResizeColumnsPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsTemplate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsTemplate" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RowDetailsTemplateProperty = DependencyProperty.Register(nameof(RowDetailsTemplate), typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnRowDetailsTemplatePropertyChanged)));

        /// <summary>
        /// Gets or sets the template that is used to display the content of the details section of rows.
        /// </summary>
        /// <returns>
        /// The template that is used to display row details. The default is null.
        /// </returns>
        [OpenSilver.NotImplemented]
        public DataTemplate RowDetailsTemplate
        {
            get => GetValue(RowDetailsTemplateProperty) as DataTemplate;
            set => SetValue(RowDetailsTemplateProperty, value);
        }

        private static void OnRowDetailsTemplatePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.ColumnWidth" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.ColumnWidth" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(nameof(ColumnWidth), typeof(DataGridLength), typeof(DataGrid), new FrameworkPropertyMetadata((object)DataGridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, DataGrid.OnColumnWidthPropertyChanged));
#else
        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(nameof(ColumnWidth), typeof(DataGridLength), typeof(DataGrid), new PropertyMetadata((object)DataGridLength.Auto, DataGrid.OnColumnWidthPropertyChanged));
#endif

        /// <summary>
        /// Gets or sets the standard width or automatic sizing mode of columns in the control.
        /// </summary>
        /// <returns>
        /// A structure that represents the standard width or automatic sizing mode of columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// The default is <see cref="P:System.Windows.Controls.DataGridLength.Auto" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public DataGridLength ColumnWidth
        {
            get => (DataGridLength)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        private static void OnColumnWidthPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.FrozenColumnCount" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.FrozenColumnCount" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FrozenColumnCountProperty = DependencyProperty.Register(nameof(FrozenColumnCount), typeof(int), typeof(DataGrid), new PropertyMetadata(new PropertyChangedCallback(DataGrid.OnFrozenColumnCountPropertyChanged)));

        /// <summary>
        /// Gets or sets the number of columns that the user cannot scroll horizontally.
        /// </summary>
        /// <returns>
        /// The number of non-scrolling columns.
        /// </returns>
        [OpenSilver.NotImplemented]
        public int FrozenColumnCount
        {
            get => (int)GetValue(FrozenColumnCountProperty);
            set => SetValue(FrozenColumnCountProperty, value);
        }

        private static void OnFrozenColumnCountPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.MinColumnWidth" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.MinColumnWidth" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(DataGrid), new FrameworkPropertyMetadata((object)20.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(DataGrid.OnMinColumnWidthPropertyChanged)));
#else
        public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(DataGrid), new PropertyMetadata((object)20.0, new PropertyChangedCallback(DataGrid.OnMinColumnWidthPropertyChanged)));
#endif

        /// <summary>
        /// Gets or sets the minimum width of columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// </summary>
        /// <returns>
        /// The minimum column width in pixels. The default is 20.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// When setting this property, the specified value is less than zero or greater than <see cref="P:System.Windows.Controls.DataGrid.MaxColumnWidth" />.
        /// </exception>
        [OpenSilver.NotImplemented]
        public double MinColumnWidth
        {
            get => (double)GetValue(MinColumnWidthProperty);
            set => SetValue(MinColumnWidthProperty, value);
        }

        private static void OnMinColumnWidthPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The default height of a row.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for RowHeight.
        /// </summary>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(DataGrid), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
#else
        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(DataGrid), new PropertyMetadata(double.NaN, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
#endif

        /// <summary>
        ///     The default minimum height of a row.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double MinRowHeight
        {
            get { return (double)GetValue(MinRowHeightProperty); }
            set { SetValue(MinRowHeightProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for MinRowHeight.
        /// </summary>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty MinRowHeightProperty = DependencyProperty.Register(nameof(MinRowHeight), typeof(double), typeof(DataGrid), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
#else
        public static readonly DependencyProperty MinRowHeightProperty = DependencyProperty.Register(nameof(MinRowHeight), typeof(double), typeof(DataGrid), new PropertyMetadata(0.0, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
#endif

        /// <summary>
        ///     Notifies each CellsPresenter about property changes.
        /// </summary>
        private static void OnNotifyCellsPresenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //((DataGrid)d).NotifyPropertyChanged(d, e, DataGridNotificationTarget.CellsPresenter);
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.CanUserSortColumns" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.CanUserSortColumns" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CanUserSortColumnsProperty = DependencyProperty.Register(nameof(CanUserSortColumns), typeof(bool), typeof(DataGrid), null);

        /// <summary>
        /// Gets or sets a value that indicates whether the user can sort columns by clicking the column header.
        /// </summary>
        /// <returns>
        /// true if the user can sort columns; otherwise, false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool CanUserSortColumns
        {
            get => (bool)GetValue(CanUserSortColumnsProperty);
            set => SetValue(CanUserSortColumnsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DragIndicatorStyle" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.DragIndicatorStyle" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DragIndicatorStyleProperty = DependencyProperty.Register(nameof(DragIndicatorStyle), typeof(Style), typeof(DataGrid), null);

        /// <summary>
        /// Gets or sets the style that is used when rendering the drag indicator that is displayed while dragging column headers.
        /// </summary>
        /// <returns>
        /// The style applied to column headers.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Style DragIndicatorStyle
        {
            get => GetValue(DragIndicatorStyleProperty) as Style;
            set => SetValue(DragIndicatorStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DropLocationIndicatorStyle" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.DropLocationIndicatorStyle" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DropLocationIndicatorStyleProperty = DependencyProperty.Register(nameof(DropLocationIndicatorStyle), typeof(Style), typeof(DataGrid), null);

        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        /// <returns>
        /// The style applied to column headers.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Style DropLocationIndicatorStyle
        {
            get => GetValue(DropLocationIndicatorStyleProperty) as Style;
            set => SetValue(DropLocationIndicatorStyleProperty, value);
        }

        /// <summary>
        ///     Raises the CommitEdit command.
        ///     If a cell is currently being edited, commits any pending changes to the cell, but
        ///     leaves any pending changes to the row. This should mean that changes are propagated
        ///     from the editing environment to the pending row.
        ///     If a cell is not currently being edited, then commits any pending rows.
        /// </summary>
        /// <returns>true if the current cell or row exits edit mode, false otherwise.</returns>
        [OpenSilver.NotImplemented]
        public bool CommitEdit()
        {
            //if (IsEditingCurrentCell)
            //{
            //    return CommitEdit(DataGridEditingUnit.Cell, true);
            //}
            //else if (IsEditingRowItem || IsAddingNewItem)
            //{
            //    return CommitEdit(DataGridEditingUnit.Row, true);
            //}
            //
            //return true; // No one is in edit mode
            return false;
        }

        //
        // Summary:
        //     Causes the data grid to enter editing mode for the current cell and current row,
        //     unless the data grid is already in editing mode.
        //
        // Returns:
        //     true if the data grid enters editing mode; otherwise, false.
        [OpenSilver.NotImplemented]
        public bool BeginEdit()
        {
            return false;
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether the row details sections remain fixed
        //     at the width of the display area or can scroll horizontally.
        //
        // Returns:
        //     true if the row details sections are prevented from scrolling horizontally; otherwise,
        //     false. The default is false.
        [OpenSilver.NotImplemented]
        public bool AreRowDetailsFrozen { get; set; }

        //
        // Summary:
        //     Gets a value that indicates whether data in the grid is valid.
        //
        // Returns:
        //     true if the data is valid; otherwise, false.
        [OpenSilver.NotImplemented]
        public bool IsValid { get; }

        //
        // Summary:
        //     Occurs when a row edit has been committed or canceled.
        [OpenSilver.NotImplemented]
        public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;
    }
}
#endif