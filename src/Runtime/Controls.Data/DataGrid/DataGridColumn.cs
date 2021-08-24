// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;


#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a <see cref="T:System.Windows.Controls.DataGrid" /> column.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "IComparable in Jolt has only one method, probably a false positive in FxCop.")]
    [StyleTypedProperty(Property = "CellStyle", StyleTargetType = typeof(DataGridCell))]
    [StyleTypedProperty(Property = "DragIndicatorStyle", StyleTargetType = typeof(ContentControl))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(DataGridColumnHeader))]
    public abstract class DataGridColumn : DependencyObject
    {
#region Constants

        internal const int DATAGRIDCOLUMN_maximumWidth = 65536;
        private const bool DATAGRIDCOLUMN_defaultIsReadOnly = false;

#endregion Constants

#region Data

        private List<string> _bindingPaths;
        private Style _cellStyle;
        private Binding _clipboardContentBinding;
        private int _displayIndexWithFiller;
        private Style _dragIndicatorStyle;
        private FrameworkElement _editingElement;
        private object _header;
        private DataGridColumnHeader _headerCell;
        private Style _headerStyle;
        private List<BindingInfo> _inputBindings;
        private bool? _isReadOnly;
        private double? _maxWidth;
        private double? _minWidth;
        private bool _settingWidthInternally;
        private DataGridLength? _width; // Null by default, null means inherit the Width from the DataGrid
        private Visibility _visibility;

#endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridColumn" /> class.
        /// </summary>
        protected internal DataGridColumn()
        {
            this._visibility = Visibility.Visible;
            this._displayIndexWithFiller = -1;
            this.IsInitialDesiredWidthDetermined = false;
            this.InheritsWidth = true;
        }

#region Dependency Properties

#endregion

#region Public Properties

        /// <summary>
        /// Actual visible width after Width, MinWidth, and MaxWidth setting at the Column level and DataGrid level
        /// have been taken into account
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (this.OwningGrid == null || double.IsNaN(this.Width.DisplayValue))
                {
                    return this.ActualMinWidth;
                }
                return this.Width.DisplayValue;
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering cells in the column.
        /// </summary>
        /// <returns>
        /// The style that is used when rendering cells in the column. The default is null.
        /// </returns>
        public Style CellStyle
        {
            get
            {
                return this._cellStyle;
            }
            set
            {
                if (_cellStyle != value)
                {
                    Style previousStyle = this._cellStyle;
                    this._cellStyle = value;
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnCellStyleChanged(this, previousStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can change the column display position by 
        /// dragging the column header.
        /// </summary>
        /// <returns>
        /// true if the user can drag the column header to a new position; otherwise, false. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserReorderColumns" /> property value.
        /// </returns>
        public bool CanUserReorder
        {
            get
            {
                if (this.CanUserReorderInternal.HasValue)
                {
                    return this.CanUserReorderInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.CanUserReorderColumns;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserResizeColumns;
                }
            }
            set
            {
                this.CanUserReorderInternal = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can adjust the column width using the mouse.
        /// </summary>
        /// <returns>
        /// true if the user can resize the column; false if the user cannot resize the column. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserResizeColumns" /> property value.
        /// </returns>
        public bool CanUserResize
        {
            get 
            {
                if (this.CanUserResizeInternal.HasValue)
                {
                    return this.CanUserResizeInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.CanUserResizeColumns;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserResizeColumns;
                }
            }
            set 
            { 
                this.CanUserResizeInternal = value;
                if (this.OwningGrid != null)
                {
                    this.OwningGrid.OnColumnCanUserResizeChanged(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can sort the column by clicking the column header.
        /// </summary>
        /// <returns>
        /// true if the user can sort the column; false if the user cannot sort the column. The default is the current <see cref="P:System.Windows.Controls.DataGrid.CanUserSortColumns" /> property value.
        /// </returns>
        public bool CanUserSort
        {
            get
            {
                if (this.CanUserSortInternal.HasValue)
                {
                    return this.CanUserSortInternal.Value;
                }
                else if (this.OwningGrid != null)
                {
                    string propertyPath = GetSortPropertyName();
                    Type propertyType = this.OwningGrid.DataConnection.DataType.GetNestedPropertyType(propertyPath);

                    // if the type is nullable, then we will compare the non-nullable type
                    if (TypeHelper.IsNullableType(propertyType))
                    {
                        propertyType = TypeHelper.GetNonNullableType(propertyType);
                    }

                    // return whether or not the property type can be compared
                    return (typeof(IComparable).IsAssignableFrom(propertyType)) ? true : false;
                }
                else
                {
                    return DataGrid.DATAGRID_defaultCanUserSortColumns;
                }
            }
            set
            {
                this.CanUserSortInternal = value;
            }
        }

        /// <summary>
        /// The binding that will be used to get or set cell content for the clipboard.
        /// </summary>
        public virtual Binding ClipboardContentBinding
        {
            get
            {
                return this._clipboardContentBinding;
            }
            set
            {
                this._clipboardContentBinding = value;
            }
        }

        /// <summary>
        /// Gets or sets the display position of the column relative to the other columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// </summary>
        /// <returns>
        /// The zero-based position of the column as it is displayed in the associated <see cref="T:System.Windows.Controls.DataGrid" />. The default is the index of the corresponding <see cref="P:System.Collections.ObjectModel.Collection`1.Item(System.Int32)" /> in the <see cref="P:System.Windows.Controls.DataGrid.Columns" /> collection.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// When setting this property, the specified value is less than -1 or equal to <see cref="F:System.Int32.MaxValue" />.
        /// 
        /// -or-
        /// 
        /// When setting this property on a column in a <see cref="T:System.Windows.Controls.DataGrid" />, the specified value is less than zero or greater than or equal to the number of columns in the <see cref="T:System.Windows.Controls.DataGrid" />.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// When setting this property, the <see cref="T:System.Windows.Controls.DataGrid" /> is already making <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> adjustments. For example, this exception is thrown when you attempt to set <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> in a <see cref="E:System.Windows.Controls.DataGrid.ColumnDisplayIndexChanged" /> event handler.
        /// 
        /// -or-
        /// 
        /// When setting this property, the specified value would result in a frozen column being displayed in the range of unfrozen columns, or an unfrozen column being displayed in the range of frozen columns.
        /// </exception>
        public int DisplayIndex
        {
            get
            {
                if (this.OwningGrid != null && this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                {
                    return _displayIndexWithFiller - 1;
                }
                else
                {
                    return _displayIndexWithFiller;
                }
            }
            set
            {
                if (value == Int32.MaxValue)
                {
                    throw DataGridError.DataGrid.ValueMustBeLessThan("value", "DisplayIndex", Int32.MaxValue);
                }
                if (this.OwningGrid != null)
                {
                    if (this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                    {
                        value++;
                    }
                    if (this._displayIndexWithFiller != value)
                    {
                        if (value < 0 || value >= this.OwningGrid.ColumnsItemsInternal.Count)
                        {
                            throw DataGridError.DataGrid.ValueMustBeBetween("value", "DisplayIndex", 0, true, this.OwningGrid.Columns.Count, false);
                        }
                        // Will throw an error if a visible frozen column is placed inside a non-frozen area or vice-versa.
                        this.OwningGrid.OnColumnDisplayIndexChanging(this, value);
                        this._displayIndexWithFiller = value;
                        try
                        {
                            this.OwningGrid.InDisplayIndexAdjustments = true;
                            this.OwningGrid.OnColumnDisplayIndexChanged(this);
                            this.OwningGrid.OnColumnDisplayIndexChanged_PostNotification();
                        }
                        finally
                        {
                            this.OwningGrid.InDisplayIndexAdjustments = false;
                        }
                    }
                }
                else
                {
                    if (value < -1)
                    {
                        throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "DisplayIndex", -1);
                    }
                    this._displayIndexWithFiller = value;
                }
            }
        }

        public Style DragIndicatorStyle
        {
            get
            {
                return _dragIndicatorStyle;
            }
            set
            {
                if (_dragIndicatorStyle != value)
                {
                    _dragIndicatorStyle = value;
                }
            }
        }

        public Style HeaderStyle
        {
            get
            {
                return this._headerStyle;
            }
            set
            {
                if (this._headerStyle != value)
                {
                    Style previousStyle = this._headerStyle;
                    this._headerStyle = value;
                    if (this._headerCell != null)
                    {
                        this._headerCell.EnsureStyle(previousStyle);
                    }
                }
            }
        }

        public object Header
        {
            get
            {
                return this._header;
            }
            set
            {
                if (this._header != value)
                {
                    this._header = value;
                    if (this._headerCell != null)
                    {
                        this._headerCell.Content = this._header;
                    }
                }
            }
        }

        public bool IsAutoGenerated
        {
            get;
            internal set;
        }
        
        public bool IsFrozen
        {
            get;
            internal set;
        }

        public bool IsReadOnly
        {
            get
            {
                if (this.OwningGrid == null)
                {
                    return this._isReadOnly ?? DATAGRIDCOLUMN_defaultIsReadOnly;
                }
                if (this._isReadOnly != null)
                {
                    return this._isReadOnly.Value || this.OwningGrid.IsReadOnly;
                }
                return this.OwningGrid.GetColumnReadOnlyState(this, DATAGRIDCOLUMN_defaultIsReadOnly);
            }
            set
            {
                if (value != this._isReadOnly)
                {
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnReadOnlyStateChanging(this, value);
                    }
                    this._isReadOnly = value;
                }
            }
        }

        public double MaxWidth
        {
            get
            {
                if (_maxWidth.HasValue)
                {
                    return _maxWidth.Value;
                }
                return double.PositiveInfinity;
            }
            set
            {
                if (value < 0)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", 0);
                }
                if (value < this.ActualMinWidth)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", "MinWidth");
                }
                if (!_maxWidth.HasValue || _maxWidth.Value != value)
                {
                    double oldValue = this.ActualMaxWidth;
                    _maxWidth = value;
                    if (this.OwningGrid != null && this.OwningGrid.ColumnsInternal != null)
                    {
                        this.OwningGrid.OnColumnMaxWidthChanged(this, oldValue);
                    }
                }
            }
        }

        public double MinWidth
        {
            get
            {
                if (_minWidth.HasValue)
                {
                    return _minWidth.Value;
                }
                return 0;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinWidth");
                }
                if (value < 0)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinWidth", 0);
                }
                if (double.IsPositiveInfinity(value))
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinWidth");
                }
                if (value > this.ActualMaxWidth)
                {
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinWidth", "MaxWidth");
                }
                if (!_minWidth.HasValue || _minWidth.Value != value)
                {
                    double oldValue = this.ActualMinWidth;
                    _minWidth = value;
                    if (this.OwningGrid != null && this.OwningGrid.ColumnsInternal != null)
                    {
                        this.OwningGrid.OnColumnMinWidthChanged(this, oldValue);
                    }
                }
            }
        }

        /// <summary>
        /// Holds the name of the member to use for sorting, if not using the default.
        /// </summary>
        public string SortMemberPath
        {
            get;
            set;
        }

        public Visibility Visibility
        {
            get
            {
                return this._visibility;
            }
            set
            {
                if (value != this.Visibility)
                {
                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnVisibleStateChanging(this);
                    }

                    this._visibility = value;

                    if (this._headerCell != null)
                    {
                        this._headerCell.Visibility = this._visibility;
                    }

                    if (this.OwningGrid != null)
                    {
                        this.OwningGrid.OnColumnVisibleStateChanged(this);
                    }
                }
            }
        }

        public DataGridLength Width
        {
            get
            {
                if (_width.HasValue)
                {
                    return _width.Value;
                }
                else if (this.OwningGrid != null)
                {
                    return this.OwningGrid.ColumnWidth;
                }
                else
                {
                    // We don't have a good choice here because we don't want to make this property nullable, see DevDiv Bugs 196581
                    return DataGridLength.Auto;
                }
            }
            set
            {
                if (!_width.HasValue || _width.Value != value)
                {
                    if (!_settingWidthInternally)
                    {
                        this.InheritsWidth = false;
                    }

                    if (this.OwningGrid != null)
                    {
                        DataGridLength width = CoerceWidth(value);
                        if (width.IsStar != this.Width.IsStar || DesignerProperties.IsInDesignTool)
                        {
                            // If a column has changed either from or to a star value, we want to recalculate all
                            // star column widths.  They are recalculated during Measure based off what the value we set here.
                            SetWidthInternalNoCallback(width);
                            this.IsInitialDesiredWidthDetermined = false;
                            this.OwningGrid.OnColumnWidthChanged(this);
                        }
                        else
                        {
                            // If a column width's value is simply changing, we resize it (to the right only).
                            Resize(width.Value, width.UnitType, width.DesiredValue, width.DisplayValue, false);
                        }
                    }
                    else
                    {
                        SetWidthInternalNoCallback(value);
                    }
                }
            }
        }

#endregion Public Properties

#region Internal Properties

        internal bool ActualCanUserResize
        {
            get
            {
                if (this.OwningGrid == null || this.OwningGrid.CanUserResizeColumns == false || this is DataGridFillerColumn)
                {
                    return false;
                }
                return this.CanUserResizeInternal ?? true;
            }
        }

        // MaxWidth from local setting or DataGrid setting
        internal double ActualMaxWidth
        {
            get
            {
                return _maxWidth ?? (this.OwningGrid != null ? this.OwningGrid.MaxColumnWidth : double.PositiveInfinity);
            }
        }

        // MinWidth from local setting or DataGrid setting
        internal double ActualMinWidth
        {
            get
            {
                double minWidth = _minWidth ?? (this.OwningGrid != null ? this.OwningGrid.MinColumnWidth : 0);
                if (this.Width.IsStar)
                {
                    return Math.Max(DataGrid.DATAGRID_minimumStarColumnWidth, minWidth);
                }
                return minWidth;
            }
        }

        internal List<string> BindingPaths
        {
            get
            {
                if (this._bindingPaths != null)
                {
                    return this._bindingPaths;
                }
                this._bindingPaths = CreateBindingPaths();
                return this._bindingPaths;
            }
        }

        internal bool? CanUserReorderInternal
        {
            get;
            set;
        }

        internal bool? CanUserResizeInternal
        {
            get;
            set;
        }

        internal bool? CanUserSortInternal
        {
            get;
            set;
        }

        internal bool DisplayIndexHasChanged
        {
            get;
            set;
        }

        internal int DisplayIndexWithFiller
        {
            get
            {
                return this._displayIndexWithFiller;
            }
            set
            {
                Debug.Assert(value >= -1);
                Debug.Assert(value < Int32.MaxValue);

                this._displayIndexWithFiller = value;
            }
        }

        internal bool HasHeaderCell
        {
            get
            {
                return this._headerCell != null;
            }
        }

        internal DataGridColumnHeader HeaderCell
        {
            get
            {
                if (this._headerCell == null)
                {
                    this._headerCell = CreateHeader();
                }
                return this._headerCell;
            }
        }
        
        internal int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Tracks whether or not this column inherits its Width value from the DataGrid.
        /// </summary>
        internal bool InheritsWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// When a column is initially added, we won't know its initial desired value
        /// until all rows have been measured.  We use this variable to track whether or
        /// not the column has been fully measured.
        /// </summary>
        internal bool IsInitialDesiredWidthDetermined
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether or not this column is visible.
        /// </summary>
        internal bool IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        internal double LayoutRoundedWidth
        {
            get;
            private set;
        }

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

#endregion Internal Properties

#region Public Methods

        public FrameworkElement GetCellContent(DataGridRow dataGridRow)
        {
            if (dataGridRow == null)
            {
                throw new ArgumentNullException("dataGridRow");
            }
            if (this.OwningGrid == null)
            {
                throw DataGridError.DataGrid.NoOwningGrid(this.GetType());
            }
            if (dataGridRow.OwningGrid == this.OwningGrid)
            {
                Debug.Assert(this.Index >= 0);
                Debug.Assert(this.Index < this.OwningGrid.ColumnsItemsInternal.Count);
                DataGridCell dataGridCell = dataGridRow.Cells[this.Index];
                if (dataGridCell != null)
                {
                    return dataGridCell.Content as FrameworkElement;
                }
            }
            return null;
        }

        public FrameworkElement GetCellContent(object dataItem)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException("dataItem");
            }
            if (this.OwningGrid == null)
            {
                throw DataGridError.DataGrid.NoOwningGrid(this.GetType());
            }
            Debug.Assert(this.Index >= 0);
            Debug.Assert(this.Index < this.OwningGrid.ColumnsItemsInternal.Count);
            DataGridRow dataGridRow = this.OwningGrid.GetRowFromItem(dataItem);
            if (dataGridRow == null)
            {
                return null;
            }
            return GetCellContent(dataGridRow);
        }

        /// <summary>
        /// Returns the column which contains the given element
        /// </summary>
        /// <param name="element">element contained in a column</param>
        /// <returns>Column that contains the element, or null if not found
        /// </returns>
        public static DataGridColumn GetColumnContainingElement(FrameworkElement element)
        {
            // Walk up the tree to find the DataGridCell or DataGridColumnHeader that contains the element
            DependencyObject parent = element;
            while (parent != null)
            {
                DataGridCell cell = parent as DataGridCell;
                if (cell != null)
                {
                    return cell.OwningColumn;
                }
                DataGridColumnHeader columnHeader = parent as DataGridColumnHeader;
                if (columnHeader != null)
                {
                    return columnHeader.OwningColumn;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

#endregion Public Methods

#region Protected Methods

        /// <summary>
        /// When overridden in a derived class, causes the column cell being edited to revert to the unedited value.
        /// </summary>
        /// <param name="editingElement">
        /// The element that the column displays for a cell in editing mode.
        /// </param>
        /// <param name="uneditedValue">
        /// The previous, unedited value in the cell being edited.
        /// </param>
        protected virtual void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        { }

        /// <summary>
        /// When overridden in a derived class, gets an editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">
        /// The cell that will contain the generated element.
        /// </param>
        /// <param name="dataItem">
        /// The data item represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </returns>
        protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

        /// <summary>
        /// When overridden in a derived class, gets a read-only element that is bound to the column's 
        /// <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">
        /// The cell that will contain the generated element.
        /// </param>
        /// <param name="dataItem">
        /// The data item represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new, read-only element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </returns>
        protected abstract FrameworkElement GenerateElement(DataGridCell cell, object dataItem);

        /// <summary>
        /// Called by a specific column type when one of its properties changed, 
        /// and its current cells need to be updated.
        /// </summary>
        /// <param name="propertyName">Indicates which property changed and caused this call</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.OwningGrid == null)
            {
                return;
            }
            this.OwningGrid.RefreshColumnElements(this, propertyName);
        }

        /// <summary>
        /// When overridden in a derived class, called when a cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">
        /// The element that the column displays for a cell in editing mode.
        /// </param>
        /// <param name="editingEventArgs">
        /// Information about the user gesture that is causing a cell to enter editing mode.
        /// </param>
        /// <returns>
        /// The unedited value.
        /// </returns>
        protected abstract object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs);

        /// <summary>
        /// Called by the DataGrid control when a column asked for its
        /// elements to be refreshed, typically because one of its properties changed.
        /// </summary>
        /// <param name="element">Indicates the element that needs to be refreshed</param>
        /// <param name="propertyName">Indicates which property changed and caused this call</param>
        protected internal virtual void RefreshCellContent(FrameworkElement element, string propertyName)
        {
        }

#endregion Protected Methods

#region Internal Methods

        internal void CancelCellEditInternal(FrameworkElement editingElement, object uneditedValue)
        {
            CancelCellEdit(editingElement, uneditedValue);
        }

        /// <summary>
        /// Coerces a DataGridLength to a valid value.  If any value components are double.NaN, this method
        /// coerces them to a proper initial value.  For star columns, the desired width is calculated based
        /// on the rest of the star columns.  For pixel widths, the desired value is based on the pixel value.
        /// For auto widths, the desired value is initialized as the column's minimum width.
        /// </summary>
        /// <param name="width">The DataGridLength to coerce.</param>
        /// <returns>The resultant (coerced) DataGridLength.</returns>
        internal DataGridLength CoerceWidth(DataGridLength width)
        {
            double desiredValue = width.DesiredValue;
            if (double.IsNaN(desiredValue))
            {
                if (width.IsStar && this.OwningGrid != null && this.OwningGrid.ColumnsInternal != null)
                {
                    double totalStarValues = 0;
                    double totalStarDesiredValues = 0;
                    double totalNonStarDisplayWidths = 0;
                    foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && c != this && !double.IsNaN(c.Width.DesiredValue)))
                    {
                        if (column.Width.IsStar)
                        {
                            totalStarValues += column.Width.Value;
                            totalStarDesiredValues += column.Width.DesiredValue;
                        }
                        else
                        {
                            totalNonStarDisplayWidths += column.ActualWidth;
                        }
                    }
                    if (totalStarValues == 0)
                    {
                        // Compute the new star column's desired value based on the available space if there are no other visible star columns
                        desiredValue = Math.Max(this.ActualMinWidth, this.OwningGrid.CellsWidth - totalNonStarDisplayWidths);
                    }
                    else
                    {
                        // Otherwise, compute its desired value based on those of other visible star columns
                        desiredValue = totalStarDesiredValues * width.Value / totalStarValues;
                    }
                }
                else if (width.IsAbsolute)
                {
                    desiredValue = width.Value;
                }
                else
                {
                    desiredValue = this.ActualMinWidth;
                }
            }

            double displayValue = width.DisplayValue;
            if (double.IsNaN(displayValue))
            {
                displayValue = desiredValue;
            }
            displayValue = Math.Max(this.ActualMinWidth, Math.Min(this.ActualMaxWidth, displayValue));

            return new DataGridLength(width.Value, width.UnitType, desiredValue, displayValue);
        }

        /// <summary>
        /// If the DataGrid is using using layout rounding, the pixel snapping will force all widths to
        /// whole numbers. Since the column widths aren't visual elements, they don't go through the normal
        /// rounding process, so we need to do it ourselves.  If we don't, then we'll end up with some
        /// pixel gaps and/or overlaps between columns.
        /// </summary>
        /// <param name="leftEdge"></param>
        internal void ComputeLayoutRoundedWidth(double leftEdge)
        {
            if (this.OwningGrid != null && this.OwningGrid.UseLayoutRounding)
            {
                double roundedLeftEdge = Math.Floor(leftEdge + 0.5);
                double roundedRightEdge = Math.Floor(leftEdge + this.ActualWidth + 0.5);
                this.LayoutRoundedWidth = roundedRightEdge - roundedLeftEdge;
            }
            else
            {
                this.LayoutRoundedWidth = this.ActualWidth;
            }
        }

        internal virtual List<string> CreateBindingPaths()
        {
            List<string> bindingPaths = new List<string>();
            List<BindingInfo> bindings = null;
            if (this._inputBindings == null && this.OwningGrid != null)
            {
                DataGridRow row = this.OwningGrid.EditingRow;
                if (row != null && row.Cells != null && row.Cells.Count > this.Index)
                {
                    // Finds the input bindings if they don't already exist
                    this.GenerateEditingElementInternal(row.Cells[this.Index], row.DataContext);
                }
            }
            if (this._inputBindings != null)
            {
                Debug.Assert(this.OwningGrid != null);

                // Use the editing bindings if they've already been created
                bindings = this._inputBindings;
            }
            if (bindings != null)
            {
                // We're going to return the path of every active binding
                foreach (BindingInfo binding in bindings)
                {
                    if (binding != null &&
                        binding.BindingExpression != null &&
                        binding.BindingExpression.ParentBinding != null &&
                        binding.BindingExpression.ParentBinding.Path != null)
                    {
                        bindingPaths.Add(binding.BindingExpression.ParentBinding.Path.Path);
                    }
                }
            }
            return bindingPaths;
        }

        internal virtual List<BindingInfo> CreateBindings(FrameworkElement element, object dataItem, bool twoWay)
        {
            return element.GetBindingInfo(dataItem, twoWay, false /*useBlockList*/, true /*searchChildren*/, typeof(DataGrid));
        }

        internal virtual DataGridColumnHeader CreateHeader()
        {
            DataGridColumnHeader result = new DataGridColumnHeader();
            result.OwningColumn = this;
            result.Content = this._header;
            result.EnsureStyle(null);

            return result;
        }

        /// <summary>
        /// Ensures that this column's width has been coerced to a valid value.
        /// </summary>
        internal void EnsureWidth()
        {
            SetWidthInternalNoCallback(CoerceWidth(this.Width));
        }

        internal FrameworkElement GenerateEditingElementInternal(DataGridCell cell, object dataItem)
        {
            if (this._editingElement == null)
            {
                this._editingElement = GenerateEditingElement(cell, dataItem);
            }
            if (this._inputBindings == null && this._editingElement != null)
            {
                this._inputBindings = CreateBindings(this._editingElement, dataItem, true /*twoWay*/);

                // Setup all of the active input bindings to support validation
                foreach (BindingInfo bindingData in this._inputBindings)
                {
                    if (bindingData.BindingExpression != null
                        && bindingData.BindingExpression.ParentBinding != null
                        && bindingData.BindingExpression.ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                    {
                        Binding binding = ValidationUtil.CloneBinding(bindingData.BindingExpression.ParentBinding);
                        if (binding != null)
                        {
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                            bindingData.Element.SetBinding(bindingData.BindingTarget, binding);
                            bindingData.BindingExpression = bindingData.Element.GetBindingExpression(bindingData.BindingTarget);
                        }
                    }
                }
            }
            return this._editingElement;
        }

        internal FrameworkElement GenerateElementInternal(DataGridCell cell, object dataItem)
        {
            return GenerateElement(cell, dataItem);
        }

        /// <summary>
        /// Gets the value of a cell according to the the specified binding.
        /// </summary>
        /// <param name="item">The item associated with a cell.</param>
        /// <param name="binding">The binding to get the value of.</param>
        /// <returns>The resultant cell value.</returns>
        internal object GetCellValue(object item, Binding binding)
        {
            Debug.Assert(this.OwningGrid != null);

            object content = null;
            if (binding != null)
            {
                this.OwningGrid.ClipboardContentControl.DataContext = item;
                this.OwningGrid.ClipboardContentControl.SetBinding(ContentControl.ContentProperty, binding);
                content = this.OwningGrid.ClipboardContentControl.GetValue(ContentControl.ContentProperty);
            }
            return content;
        }

        internal List<BindingInfo> GetInputBindings(FrameworkElement element, object dataItem)
        {
            if (this._inputBindings != null)
            {
                return this._inputBindings;
            }
            return CreateBindings(element, dataItem, true /*twoWay*/);
        }

        /// <summary>
        /// We get the sort description from the data source.  We don't worry whether we can modify sort -- perhaps the sort description
        /// describes an unchangeable sort that exists on the data.
        /// </summary>
        /// <returns></returns>
        internal SortDescription? GetSortDescription()
        {
            if (this.OwningGrid != null
                && this.OwningGrid.DataConnection != null
                && this.OwningGrid.DataConnection.SortDescriptions != null)
            {
                string propertyName = GetSortPropertyName();

                SortDescription sort = (new List<SortDescription>(this.OwningGrid.DataConnection.SortDescriptions))
                    .FirstOrDefault(s => s.PropertyName == propertyName);

                if (sort.PropertyName != null)
                {
                    return sort;
                }

                return null;
            }


            return null;
        }

        internal string GetSortPropertyName()
        {
            string result = this.SortMemberPath;

            if (String.IsNullOrEmpty(result))
            {
                DataGridBoundColumn boundColumn = this as DataGridBoundColumn;

                if (boundColumn != null && boundColumn.Binding != null && boundColumn.Binding.Path != null)
                {
                    result = boundColumn.Binding.Path.Path;
                }
            }

            return result;
        }

        internal object PrepareCellForEditInternal(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return PrepareCellForEdit(editingElement, editingEventArgs);
        }

        /// <summary>
        /// Clears the cached editing element.
        /// </summary>
        internal void RemoveEditingElement()
        {
            this._editingElement = null;
            this._inputBindings = null;
            this._bindingPaths = null;
        }



        /// <summary>
        /// Attempts to resize the column's width to the desired DisplayValue, but limits the final size
        /// to the column's minimum and maximum values.  If star sizing is being used, then the column
        /// can only decrease in size by the amount that the columns after it can increase in size.
        /// Likewise, the column can only increase in size if other columns can spare the width.
        /// </summary>
        /// <param name="value">The new Value.</param>
        /// <param name="unitType">The new UnitType.</param>
        /// <param name="desiredValue">The new DesiredValue.</param>
        /// <param name="displayValue">The new DisplayValue.</param>
        /// <param name="userInitiated">Whether or not this resize was initiated by a user action.</param>
        internal void Resize(double value, DataGridLengthUnitType unitType, double desiredValue, double displayValue, bool userInitiated)
        {
            Debug.Assert(this.OwningGrid != null);

            double newValue = value;
            double newDesiredValue = desiredValue;
            double newDisplayValue = Math.Max(this.ActualMinWidth, Math.Min(this.ActualMaxWidth, displayValue));
            DataGridLengthUnitType newUnitType = unitType;

            int starColumnsCount = 0;
            double totalDisplayWidth = 0;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                column.EnsureWidth();
                totalDisplayWidth += column.ActualWidth;
                starColumnsCount += (column != this && column.Width.IsStar) ? 1 : 0;
            }
            bool hasInfiniteAvailableWidth = !this.OwningGrid.RowsPresenterAvailableSize.HasValue || double.IsPositiveInfinity(this.OwningGrid.RowsPresenterAvailableSize.Value.Width);

            // If we're using star sizing, we can only resize the column as much as the columns to the
            // right will allow (i.e. until they hit their max or min widths).
            if (!hasInfiniteAvailableWidth && (starColumnsCount > 0 || (unitType == DataGridLengthUnitType.Star && this.Width.IsStar && userInitiated)))
            {
                double limitedDisplayValue = this.Width.DisplayValue;
                double availableIncrease = Math.Max(0, this.OwningGrid.CellsWidth - totalDisplayWidth);
                double desiredChange = newDisplayValue - this.Width.DisplayValue;
                if (desiredChange > availableIncrease)
                {
                    // The desired change is greater than the amount of available space,
                    // so we need to decrease the widths of columns to the right to make room.
                    desiredChange -= availableIncrease;
                    double actualChange = desiredChange + this.OwningGrid.DecreaseColumnWidths(this.DisplayIndex + 1, -desiredChange, userInitiated);
                    limitedDisplayValue += availableIncrease + actualChange;
                }
                else if (desiredChange > 0)
                {
                    // The desired change is positive but less than the amount of available space,
                    // so there's no need to decrease the widths of columns to the right.
                    limitedDisplayValue += desiredChange;
                }
                else
                {
                    // The desired change is negative, so we need to increase the widths of columns to the right.
                    limitedDisplayValue += desiredChange + this.OwningGrid.IncreaseColumnWidths(this.DisplayIndex + 1, -desiredChange, userInitiated);
                }
                if (this.ActualCanUserResize || (this.Width.IsStar && !userInitiated))
                {
                    newDisplayValue = limitedDisplayValue;
                }
            }

            if (userInitiated)
            {
                newDesiredValue = newDisplayValue;
                if (!this.Width.IsStar)
                {
                    this.InheritsWidth = false;
                    newValue = newDisplayValue;
                    newUnitType = DataGridLengthUnitType.Pixel;
                }
                else if (starColumnsCount > 0 && !hasInfiniteAvailableWidth)
                {
                    // Recalculate star weight of this column based on the new desired value
                    this.InheritsWidth = false;
                    newValue = (this.Width.Value * newDisplayValue) / this.ActualWidth;
                }
            }

            DataGridLength oldWidth = this.Width;
            SetWidthInternalNoCallback(new DataGridLength(Math.Min(double.MaxValue, newValue), newUnitType, newDesiredValue, newDisplayValue));
            if (this.Width != oldWidth)
            {
                this.OwningGrid.OnColumnWidthChanged(this);
            }
        }

        /// <summary>
        /// Sets the column's Width to a new DataGridLength with a different DesiredValue.
        /// </summary>
        /// <param name="desiredValue">The new DesiredValue.</param>
        internal void SetWidthDesiredValue(double desiredValue)
        {
            SetWidthInternalNoCallback(new DataGridLength(this.Width.Value, this.Width.UnitType, desiredValue, this.Width.DisplayValue));
        }

        /// <summary>
        /// Sets the column's Width to a new DataGridLength with a different DisplayValue.
        /// </summary>
        /// <param name="displayValue">The new DisplayValue.</param>
        internal void SetWidthDisplayValue(double displayValue)
        {
            SetWidthInternalNoCallback(new DataGridLength(this.Width.Value, this.Width.UnitType, this.Width.DesiredValue, displayValue));
        }

        /// <summary>
        /// Set the column's Width without breaking inheritance.
        /// </summary>
        /// <param name="width">The new Width.</param>
        internal void SetWidthInternal(DataGridLength width)
        {
            bool originalValue = _settingWidthInternally;
            _settingWidthInternally = true;
            try
            {
                this.Width = width;
            }
            finally
            {
                _settingWidthInternally = originalValue;
            }
        }

        /// <summary>
        /// Sets the column's Width directly, without any callback effects.
        /// </summary>
        /// <param name="width">The new Width.</param>
        internal void SetWidthInternalNoCallback(DataGridLength width)
        {
            _width = width;
        }

        /// <summary>
        /// Set the column's star value.  Whenever the star value changes, width inheritance is broken.
        /// </summary>
        /// <param name="value">The new star value.</param>
        internal void SetWidthStarValue(double value)
        {
            Debug.Assert(this.Width.IsStar);

            this.InheritsWidth = false;
            SetWidthInternalNoCallback(new DataGridLength(value, this.Width.UnitType, this.Width.DesiredValue, this.Width.DisplayValue));
        }

#endregion Internal Methods

#region Private Methods

#endregion Private Methods

    }
}
