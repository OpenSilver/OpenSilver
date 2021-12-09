// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text;
#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays data in a customizable grid.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [TemplatePart(Name = DataGrid.DATAGRID_elementRowsPresenterName, Type = typeof(DataGridRowsPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementColumnHeadersPresenterName, Type = typeof(DataGridColumnHeadersPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementFrozenColumnScrollBarSpacerName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementHorizontalScrollbarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementValidationSummary, Type = typeof(ValidationSummary))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementVerticalScrollbarName, Type = typeof(ScrollBar))]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = "CellStyle", StyleTargetType = typeof(DataGridCell))]
    [StyleTypedProperty(Property = "ColumnHeaderStyle", StyleTargetType = typeof(DataGridColumnHeader))]
    [StyleTypedProperty(Property = "DragIndicatorStyle", StyleTargetType = typeof(ContentControl))]
    [StyleTypedProperty(Property = "DropLocationIndicatorStyle", StyleTargetType = typeof(ContentControl))]
    [StyleTypedProperty(Property = "RowHeaderStyle", StyleTargetType = typeof(DataGridRowHeader))]
    [StyleTypedProperty(Property = "RowStyle", StyleTargetType = typeof(DataGridRow))]
    public partial class DataGrid : Control
    {
#region Constants
        
        private const string DATAGRID_elementRowsPresenterName = "RowsPresenter";
        private const string DATAGRID_elementColumnHeadersPresenterName = "ColumnHeadersPresenter";
        private const string DATAGRID_elementFrozenColumnScrollBarSpacerName = "FrozenColumnScrollBarSpacer";
        private const string DATAGRID_elementHorizontalScrollbarName = "HorizontalScrollbar";
        private const string DATAGRID_elementRowHeadersPresenterName = "RowHeadersPresenter";
        private const string DATAGRID_elementTopLeftCornerHeaderName = "TopLeftCornerHeader";
        private const string DATAGRID_elementTopRightCornerHeaderName = "TopRightCornerHeader";
        private const string DATAGRID_elementValidationSummary = "ValidationSummary";
        private const string DATAGRID_elementVerticalScrollbarName = "VerticalScrollbar";

        private const bool DATAGRID_defaultAutoGenerateColumns = true;
        internal const bool DATAGRID_defaultCanUserReorderColumns = true;
        internal const bool DATAGRID_defaultCanUserResizeColumns = true;
        internal const bool DATAGRID_defaultCanUserSortColumns = true;
        private const DataGridRowDetailsVisibilityMode DATAGRID_defaultRowDetailsVisibility = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        private const DataGridSelectionMode DATAGRID_defaultSelectionMode = DataGridSelectionMode.Extended;

        /// <summary>
        /// The default order to use for columns when there is no <see cref="DisplayAttribute.Order"/>
        /// value available for the property.
        /// </summary>
        /// <remarks>
        /// The value of 10,000 comes from the DataAnnotations spec, allowing
        /// some properties to be ordered at the beginning and some at the end.
        /// </remarks>
        private const int DATAGRID_defaultColumnDisplayOrder = 10000;

        private const double DATAGRID_horizontalGridLinesThickness = 1;
        private const double DATAGRID_minimumRowHeaderWidth = 4;
        private const double DATAGRID_minimumColumnHeaderHeight = 4;
        internal const double DATAGRID_maximumStarColumnWidth = 10000;
        internal const double DATAGRID_minimumStarColumnWidth = 0.001;
        private const double DATAGRID_mouseWheelDelta = 48.0;
        private const double DATAGRID_maxHeadersThickness = 32768;

        private const double DATAGRID_defaultRowHeight = 22;
        internal const double DATAGRID_defaultRowGroupSublevelIndent = 20;
        private const double DATAGRID_defaultMinColumnWidth = 20;
        private const double DATAGRID_defaultMaxColumnWidth = double.PositiveInfinity;
        
#endregion Constants

#region Data

        // DataGrid Template Parts
        private DataGridColumnHeadersPresenter _columnHeadersPresenter;
        private ScrollBar _hScrollBar;
        private DataGridRowsPresenter _rowsPresenter;
        private ValidationSummary _validationSummary;
        private ScrollBar _vScrollBar;
        
        private byte _autoGeneratingColumnOperationCount;
        private bool _autoSizingColumns;
        private List<ValidationResult> _bindingValidationResults;
        private ContentControl _clipboardContentControl;
        private IndexToValueTable<Visibility> _collapsedSlotsTable;
        // this is a workaround only for the scenarios where we need it, it is not all encompassing nor always updated
        private UIElement _clickedElement;
        private DataGridCellCoordinates _currentCellCoordinates;
        // used to store the current column during a Reset
        private int _desiredCurrentColumnIndex;
        private int _editingColumnIndex;
        private RoutedEventArgs _editingEventArgs;
        private bool _executingLostFocusActions;
        private bool _flushCurrentCellChanged;
        private bool _focusEditingControl;
        private DependencyObject _focusedObject;
        private DataGridRow _focusedRow;
        private FrameworkElement _frozenColumnScrollBarSpacer;
        // the sum of the widths in pixels of the scrolling columns preceding 
        // the first displayed scrolling column
        private double _horizontalOffset;
        private byte _horizontalScrollChangesIgnored;
        private bool _ignoreNextScrollBarsLayout;
        private List<ValidationResult> _indeiValidationResults;
        // Nth row of rows 0..N that make up the RowHeightEstimate
        private int _lastEstimatedRow;
        private List<DataGridRow> _loadedRows;
        // prevents reentry into the VerticalScroll event handler
        private Queue<Action> _lostFocusActions;
        private bool _makeFirstDisplayedCellCurrentCellPending;
        private bool _measured;
        private int? _mouseOverRowIndex;    // -1 is used for the 'new row'
        // the number of pixels of the firstDisplayedScrollingCol which are not displayed
        private double _negHorizontalOffset;
        // the number of pixels of DisplayData.FirstDisplayedScrollingRow which are not displayed
        private int _noCurrentCellChangeCount;
        private int _noSelectionChangeCount;
        private DataGridCellCoordinates _previousAutomationFocusCoordinates;
        private DataGridColumn _previousCurrentColumn;
        private object _previousCurrentItem;
        private List<ValidationResult> _propertyValidationResults;
        private ObservableCollection<Style> _rowGroupHeaderStyles;
        // To figure out what the old RowGroupHeaderStyle was for each level, we need to keep a copy
        // of the list.  The old style important so we don't blow away styles set directly on the RowGroupHeader
        private List<Style> _rowGroupHeaderStylesOld;
        private double[] _rowGroupHeightsByLevel;
        private double _rowHeaderDesiredWidth;
        private Size? _rowsPresenterAvailableSize;
        private bool _scrollingByHeight;
        private DataGridSelectedItemsCollection _selectedItems;
        private ValidationSummaryItem _selectedValidationSummaryItem;
        private IndexToValueTable<Visibility> _showDetailsTable;
        private bool _successfullyUpdatedSelection;
        private bool _temporarilyResetCurrentCell;
        private ContentControl _topLeftCornerHeader;
        private INotifyCollectionChanged _topLevelGroup;
        private ContentControl _topRightCornerHeader;
        private object _uneditedValue; // Represents the original current cell value at the time it enters editing mode.
        private string _updateSourcePath;
        private Dictionary<INotifyDataErrorInfo, string> _validationItems;
        private List<ValidationResult> _validationResults;
        private byte _verticalScrollChangesIgnored;

        // An approximation of the sum of the heights in pixels of the scrolling rows preceding 
        // the first displayed scrolling row.  Since the scrolled off rows are discarded, the grid
        // does not know their actual height. The heights used for the approximation are the ones
        // set as the rows were scrolled off.
        private double _verticalOffset;

#endregion Data

#region Events
        /// <summary>
        /// Occurs one time for each public, non-static property in the bound data type when the 
        /// <see cref="P:System.Windows.Controls.DataGrid.ItemsSource" /> property is changed and the 
        /// <see cref="P:System.Windows.Controls.DataGrid.AutoGenerateColumns" /> property is true.
        /// </summary>
        public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

        /// <summary>
        /// Occurs before a cell or row enters editing mode. 
        /// </summary>
        public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

        /// <summary>
        /// Occurs after cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

        /// <summary>
        /// Occurs immediately before cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> 
        /// property of a column changes.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

        /// <summary>
        /// Occurs when the user drops a column header that was being dragged using the mouse.
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> ColumnHeaderDragCompleted;

        /// <summary>
        /// Occurs one or more times while the user drags a column header using the mouse. 
        /// </summary>
        public event EventHandler<DragDeltaEventArgs> ColumnHeaderDragDelta;

        /// <summary>
        /// Occurs when the user begins dragging a column header using the mouse. 
        /// </summary>
        public event EventHandler<DragStartedEventArgs> ColumnHeaderDragStarted;

        /// <summary>
        /// Raised when column reordering ends, to allow subscribers to clean up.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnReordered;
        
        /// <summary>
        /// Raised when starting a column reordering action.  Subscribers to this event can
        /// set tooltip and caret UIElements, constrain tooltip position, indicate that
        /// a preview should be shown, or cancel reordering.
        /// </summary>
        public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

        /// <summary>
        /// This event is raised by OnCopyingRowClipboardContent method after the default row content is prepared.
        /// Event listeners can modify or add to the row clipboard content.
        /// </summary>
        public event EventHandler<DataGridRowClipboardEventArgs> CopyingRowClipboardContent;

        /// <summary>
        /// Occurs when a different cell becomes the current cell.
        /// </summary>
        public event EventHandler<EventArgs> CurrentCellChanged;

        /// <summary>
        /// Occurs after a <see cref="T:System.Windows.Controls.DataGridRow" /> 
        /// is instantiated, so that you can customize it before it is used.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> LoadingRow;

        /// <summary>
        /// Occurs when a new row details template is applied to a row, so that you can customize 
        /// the details section before it is used.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

        /// <summary>
        /// Occurs before a DataGridRowGroupHeader header is used.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> LoadingRowGroup;

        /// <summary>
        /// Occurs when a cell in a <see cref="T:System.Windows.Controls.DataGridTemplateColumn" /> enters editing mode.
        /// 
        /// </summary>
        public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsVisibilityMode" /> 
        /// property value changes.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

        /// <summary>
        /// Occurs when the row has been successfully committed or cancelled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;

        /// <summary>
        /// Occurs immediately before the row has been successfully committed or cancelled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGrid.SelectedItem" /> or 
        /// <see cref="P:System.Windows.Controls.DataGrid.SelectedItems" /> property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when a <see cref="T:System.Windows.Controls.DataGridRow" /> 
        /// object becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> UnloadingRow;

        /// <summary>
        /// Occurs when the DataGridRowGroupHeader is available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> UnloadingRowGroup;

        /// <summary>
        /// Occurs when a row details element becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

#endregion Events

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGrid" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification="_minRowHeight should be 0.")]
        public DataGrid()
        {
            base.CustomLayout = true;
            this.TabNavigation = KeyboardNavigationMode.Once;
            this.KeyDown += new KeyEventHandler(DataGrid_KeyDown);
            this.KeyUp += new KeyEventHandler(DataGrid_KeyUp);
            this.GotFocus += new RoutedEventHandler(DataGrid_GotFocus);
            this.LostFocus += new RoutedEventHandler(DataGrid_LostFocus);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(DataGrid_IsEnabledChanged);

            this._loadedRows = new List<DataGridRow>();
            this._lostFocusActions = new Queue<Action>();
            this._selectedItems = new DataGridSelectedItemsCollection(this);
            this._rowGroupHeaderStyles = new ObservableCollection<Style>();
            this._rowGroupHeaderStyles.CollectionChanged += RowGroupHeaderStyles_CollectionChanged;
            this._rowGroupHeaderStylesOld = new List<Style>();
            this.RowGroupHeadersTable = new IndexToValueTable<DataGridRowGroupInfo>();
            this._validationItems = new Dictionary<INotifyDataErrorInfo, string>();
            this._validationResults = new List<ValidationResult>();
            this._bindingValidationResults = new List<ValidationResult>();
            this._propertyValidationResults = new List<ValidationResult>();
            this._indeiValidationResults = new List<ValidationResult>();

            this.DisplayData = new DataGridDisplayData(this);
            this.ColumnsInternal = CreateColumnsInstance();

            this.RowHeightEstimate = DATAGRID_defaultRowHeight;
            this.RowDetailsHeightEstimate = 0;
            this._rowHeaderDesiredWidth = 0;

            this.DataConnection = new DataGridDataConnection(this);
            this._showDetailsTable = new IndexToValueTable<Visibility>();
            this._collapsedSlotsTable = new IndexToValueTable<Visibility>();

            this.AnchorSlot = -1;
            this._lastEstimatedRow = -1;
            this._editingColumnIndex = -1;
            this._mouseOverRowIndex = null;
            this.CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);

            this.RowGroupHeaderHeightEstimate = DATAGRID_defaultRowHeight;

            DefaultStyleKey = typeof(DataGrid);
        }

#region Dependency Properties

#region AlternatingRowBackground

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint the background of odd-numbered rows.
        /// </summary>
        /// <returns>
        /// The brush that is used to paint the background of odd-numbered rows. The default is a 
        /// <see cref="T:System.Windows.Media.SolidColorBrush" /> with a 
        /// <see cref="P:System.Windows.Media.SolidColorBrush.Color" /> value of white (ARGB value #00FFFFFF).
        /// </returns>
        public Brush AlternatingRowBackground
        {
            get { return GetValue(AlternatingRowBackgroundProperty) as Brush; }
            set { SetValue(AlternatingRowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.AlternatingRowBackground" /> 
        /// dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.AlternatingRowBackground" /> 
        /// dependency property.
        /// </returns>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty = 
            DependencyProperty.Register(
                "AlternatingRowBackground", 
                typeof(Brush), 
                typeof(DataGrid), 
                new PropertyMetadata(OnAlternatingRowBackgroundPropertyChanged));

        private static void OnAlternatingRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }
#endregion AlternatingRowBackground

#region AreRowDetailsFrozen
        /// <summary>
        /// Gets or sets a value that indicates whether the row details sections remain 
        /// fixed at the width of the display area or can scroll horizontally.
        /// </summary>
        public bool AreRowDetailsFrozen
        {
            get { return (bool)GetValue(AreRowDetailsFrozenProperty); }
            set { SetValue(AreRowDetailsFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowDetailsFrozenProperty =
            DependencyProperty.Register(
                "AreRowDetailsFrozen",
                typeof(bool),
                typeof(DataGrid),
                null);
#endregion AreRowDetailsFrozen

#region AreRowGroupHeadersFrozen
        /// <summary>
        /// Gets or sets a value that indicates whether the row group header sections
        /// remain fixed at the width of the display area or can scroll horizontally.
        /// </summary>
        public bool AreRowGroupHeadersFrozen
        {
            get { return (bool)GetValue(AreRowGroupHeadersFrozenProperty); }
            set { SetValue(AreRowGroupHeadersFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowGroupHeadersFrozenProperty =
            DependencyProperty.Register(
                "AreRowGroupHeadersFrozen",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, OnAreRowGroupHeadersFrozenPropertyChanged));

        private static void OnAreRowGroupHeadersFrozenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            ProcessFrozenColumnCount(dataGrid);

            // Update elements in the RowGroupHeader that were previously frozen
            if ((bool)e.NewValue)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    foreach (UIElement element in dataGrid._rowsPresenter.Children)
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.ClearFrozenStates();
                        }
                    }
                }
            }
        }
#endregion AreRowGroupHeadersFrozen

#region AutoGenerateColumns
        /// <summary>
        /// Gets or sets a value that indicates whether columns are created 
        /// automatically when the <see cref="P:System.Windows.Controls.DataGrid.ItemsSource" /> property is set.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the AutoGenerateColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(
                "AutoGenerateColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(OnAutoGenerateColumnsPropertyChanged));

        private static void OnAutoGenerateColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            bool value = (bool)e.NewValue;
            if (value)
            {
                dataGrid.InitializeElements(false /*recycleRows*/);
            }
            else
            {
                dataGrid.RemoveAutoGeneratedColumns();
            }
        }
#endregion AutoGenerateColumns

#region CanUserReorderColumns
        /// <summary>
        /// Gets or sets a value that indicates whether the user can change 
        /// the column display order by dragging column headers with the mouse.
        /// </summary>
        public bool CanUserReorderColumns
        {
            get { return (bool)GetValue(CanUserReorderColumnsProperty); }
            set { SetValue(CanUserReorderColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserReorderColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserReorderColumnsProperty =
            DependencyProperty.Register(
                "CanUserReorderColumns",
                typeof(bool),
                typeof(DataGrid),
                null);
#endregion CanUserReorderColumns

#region CanUserResizeColumns
        /// <summary>
        /// Gets or sets a value that indicates whether the user can adjust column widths using the mouse.
        /// </summary>
        public bool CanUserResizeColumns
        {
            get { return (bool)GetValue(CanUserResizeColumnsProperty); }
            set { SetValue(CanUserResizeColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserResizeColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserResizeColumnsProperty =
            DependencyProperty.Register(
                "CanUserResizeColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(OnCanUserResizeColumnsPropertyChanged));

        /// <summary>
        /// CanUserResizeColumns property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its CanUserResizeColumns.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnCanUserResizeColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            dataGrid.EnsureHorizontalLayout();
        }
#endregion CanUserResizeColumns

#region CanUserSortColumns
        /// <summary>
        /// Gets or sets a value that indicates whether the user can sort columns by clicking the column header.
        /// </summary>
        public bool CanUserSortColumns
        {
            get { return (bool)GetValue(CanUserSortColumnsProperty); }
            set { SetValue(CanUserSortColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserSortColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserSortColumnsProperty =
            DependencyProperty.Register(
                "CanUserSortColumns",
                typeof(bool),
                typeof(DataGrid),
                null);
#endregion CanUserSortColumns

#region CellStyle
        /// <summary>
        /// Gets or sets the style that is used when rendering the data grid cells.
        /// </summary>
        public Style CellStyle
        {
            get { return GetValue(CellStyleProperty) as Style; }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.CellStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellStyleProperty =
            DependencyProperty.Register(
                "CellStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(OnCellStylePropertyChanged));

        private static void OnCellStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    foreach (DataGridCell cell in row.Cells)
                    {
                        cell.EnsureStyle(previousStyle);
                    }
                    row.FillerCell.EnsureStyle(previousStyle);
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
#endregion CellStyle

#region ClipboardCopyMode
        /// <summary>
        /// The property which determines how DataGrid content is copied to the Clipboard.
        /// </summary>
        public DataGridClipboardCopyMode ClipboardCopyMode
        {
            get { return (DataGridClipboardCopyMode)GetValue(ClipboardCopyModeProperty); }
            set { SetValue(ClipboardCopyModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.ClipboardCopyMode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipboardCopyModeProperty =
            DependencyProperty.Register(
                "ClipboardCopyMode",
                typeof(DataGridClipboardCopyMode),
                typeof(DataGrid),
                new PropertyMetadata(DataGridClipboardCopyMode.ExcludeHeader));
#endregion ClipboardCopyMode

#region ColumnHeaderHeight
        /// <summary>
        /// Gets or sets the height of the column headers row.
        /// </summary>
        public double ColumnHeaderHeight
        {
            get { return (double)GetValue(ColumnHeaderHeightProperty); }
            set { SetValue(ColumnHeaderHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderHeightProperty =
            DependencyProperty.Register(
                "ColumnHeaderHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnColumnHeaderHeightPropertyChanged));

        /// <summary>
        /// ColumnHeaderHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnHeaderHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnHeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;
                if (value < DATAGRID_minimumColumnHeaderHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_minimumColumnHeaderHeight);
                }
                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_maxHeadersThickness);
                }
                dataGrid.InvalidateMeasure();
            }
        }
#endregion ColumnHeaderHeight

#region ColumnHeaderStyle
        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return GetValue(ColumnHeaderStyleProperty) as Style; }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register("ColumnHeaderStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(OnColumnHeaderStylePropertyChanged));

        private static void OnColumnHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    column.HeaderCell.EnsureStyle(previousStyle);
                }
                if (dataGrid.ColumnsInternal.FillerColumn != null)
                {
                    dataGrid.ColumnsInternal.FillerColumn.HeaderCell.EnsureStyle(previousStyle);
                }
            }
        }
#endregion ColumnHeaderStyle

#region ColumnWidth
        /// <summary>
        /// Gets or sets the standard width or automatic sizing mode of columns in the control.
        /// </summary>
        public DataGridLength ColumnWidth
        {
            get { return (DataGridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(
                "ColumnWidth",
                typeof(DataGridLength),
                typeof(DataGrid),
                new PropertyMetadata(DataGridLength.Auto, OnColumnWidthPropertyChanged));

        /// <summary>
        /// ColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
            {
                if (column.InheritsWidth)
                {
                    column.SetWidthInternalNoCallback(dataGrid.ColumnWidth);
                }
            }

            dataGrid.EnsureHorizontalLayout();
        }
#endregion ColumnWidth

#region DragIndicatorStyle

        /// <summary>
        /// Gets or sets the style that is used when rendering the drag indicator
        /// that is displayed while dragging column headers.
        /// </summary>
        public Style DragIndicatorStyle
        {
            get { return GetValue(DragIndicatorStyleProperty) as Style; }
            set { SetValue(DragIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DragIndicatorStyle" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorStyleProperty =
            DependencyProperty.Register(
                "DragIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);
#endregion DragIndicatorStyle

#region DropLocationIndicatorStyle

        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        public Style DropLocationIndicatorStyle
        {
            get { return GetValue(DropLocationIndicatorStyleProperty) as Style; }
            set { SetValue(DropLocationIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DropLocationIndicatorStyle" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DropLocationIndicatorStyleProperty =
            DependencyProperty.Register(
                "DropLocationIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);
#endregion DropLocationIndicatorStyle

#region FrozenColumnCount

        /// <summary>
        /// Gets or sets the number of columns that the user cannot scroll horizontally.
        /// </summary>
        public int FrozenColumnCount
        {
            get { return (int)GetValue(FrozenColumnCountProperty); }
            set { SetValue(FrozenColumnCountProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.FrozenColumnCount" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty FrozenColumnCountProperty =
            DependencyProperty.Register(
                "FrozenColumnCount",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(OnFrozenColumnCountPropertyChanged));

        private static void OnFrozenColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                if ((int)e.NewValue < 0)
                {
                    dataGrid.SetValueNoCallback(DataGrid.FrozenColumnCountProperty, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "FrozenColumnCount", 0);
                }
                ProcessFrozenColumnCount(dataGrid);
            }
        }

        private static void ProcessFrozenColumnCount(DataGrid dataGrid)
        {
            dataGrid.CorrectColumnFrozenStates();
            dataGrid.ComputeScrollBarsLayout();

            dataGrid.InvalidateColumnHeadersArrange();
            dataGrid.InvalidateCellsArrange();
        }

#endregion FrozenColumnCount

#region GridLinesVisibility
        /// <summary>
        /// Gets or sets a value that indicates which grid lines separating inner cells are shown.
        /// </summary>
        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get { return (DataGridGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the GridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register(
                "GridLinesVisibility",
                typeof(DataGridGridLinesVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnGridLinesVisibilityPropertyChanged));

        /// <summary>
        /// GridLinesProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its GridLines.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnGridLinesVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureGridLines();
                row.InvalidateHorizontalArrange();
            }
        }
#endregion GridLinesVisibility

#region HeadersVisibility
        /// <summary>
        /// Gets or sets a value that indicates the visibility of row and column headers.
        /// </summary>
        public DataGridHeadersVisibility HeadersVisibility
        {
            get { return (DataGridHeadersVisibility)GetValue(HeadersVisibilityProperty); }
            set { SetValue(HeadersVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HeadersVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadersVisibilityProperty =
            DependencyProperty.Register(
                "HeadersVisibility",
                typeof(DataGridHeadersVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnHeadersVisibilityPropertyChanged));

        /// <summary>
        /// HeadersVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HeadersVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHeadersVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            DataGridHeadersVisibility newValue = (DataGridHeadersVisibility)e.NewValue;
            DataGridHeadersVisibility oldValue = (DataGridHeadersVisibility)e.OldValue;

            Func<DataGridHeadersVisibility, DataGridHeadersVisibility, bool> hasFlags = (DataGridHeadersVisibility value, DataGridHeadersVisibility flags) => ((value & flags) == flags);

            bool newValueCols = hasFlags(newValue, DataGridHeadersVisibility.Column);
            bool newValueRows = hasFlags(newValue, DataGridHeadersVisibility.Row);
            bool oldValueCols = hasFlags(oldValue, DataGridHeadersVisibility.Column);
            bool oldValueRows = hasFlags(oldValue, DataGridHeadersVisibility.Row);

            // Columns
            if (newValueCols != oldValueCols)
            {
                if (dataGrid._columnHeadersPresenter != null)
                {
                    dataGrid.EnsureColumnHeadersVisibility();
                    if (!newValueCols)
                    {
                        dataGrid._columnHeadersPresenter.Measure(Size.Empty);
                    }
                    else
                    {
                        dataGrid.EnsureVerticalGridLines();
                    }
                    dataGrid.InvalidateMeasure();
                }
            }

            // Rows
            if (newValueRows != oldValueRows)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    foreach (FrameworkElement element in dataGrid._rowsPresenter.Children)
                    {
                        DataGridRow row = element as DataGridRow;
                        if (row != null)
                        {
                            row.EnsureHeaderStyleAndVisibility(null);
                            if (newValueRows)
                            {
                                row.ApplyState(false /*animate*/);
                                row.EnsureHeaderVisibility();
                            }
                        }
                        else
                        {
                            DataGridRowGroupHeader rowGroupHeader = element as DataGridRowGroupHeader;
                            if (rowGroupHeader != null)
                            {
                                rowGroupHeader.EnsureHeaderStyleAndVisibility(null);
                            }
                        }
                    }
                    dataGrid.InvalidateRowHeightEstimate();
                    dataGrid.InvalidateRowsMeasure(true /*invalidateIndividualElements*/);
                }
            }

            // 

            if (dataGrid._topLeftCornerHeader != null)
            {
                dataGrid._topLeftCornerHeader.Visibility = newValueRows && newValueCols ? Visibility.Visible : Visibility.Collapsed;
                if (dataGrid._topLeftCornerHeader.Visibility == Visibility.Collapsed)
                {
                    dataGrid._topLeftCornerHeader.Measure(Size.Empty);
                }
            }
        }
#endregion HeadersVisibility

#region HorizontalGridLinesBrush
        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating rows.
        /// </summary>
        public Brush HorizontalGridLinesBrush
        {
            get { return GetValue(HorizontalGridLinesBrushProperty) as Brush; }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register(
                "HorizontalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(OnHorizontalGridLinesBrushPropertyChanged));

        /// <summary>
        /// HorizontalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() && dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }
            }
        }
#endregion HorizontalGridLinesBrush

#region HorizontalScrollBarVisibility
        /// <summary>
        /// Gets or sets a value that indicates how the horizontal scroll bar is displayed.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "HorizontalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnHorizontalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// HorizontalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() &&
                (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue &&
                dataGrid._hScrollBar != null)
            {
                dataGrid.InvalidateMeasure();
            }
        }
#endregion HorizontalScrollBarVisibility

#region IsReadOnly
        /// <summary>
        /// Gets or sets a value that indicates whether the user can edit the values in the control.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsReadOnly.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                bool value = (bool)e.NewValue;
                if (value && !dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);
                }
            }
        }
#endregion IsReadOnly

#region IsValid
        /// <summary>
        /// Gets a value that indicates whether data in the grid is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }
            internal set
            {
                if (value != this.IsValid)
                {
                    if (value)
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateValid);
                    }
                    else
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateInvalid, VisualStates.StateValid);
                    }
                    this.SetValueNoCallback(IsValidProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, (OnIsValidPropertyChanged)));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                dataGrid.SetValueNoCallback(DataGrid.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }
#endregion IsValid

#region ItemsSource
        /// <summary>
        /// Gets or sets a collection that is used to generate the content of the control.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataGrid),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ItemsSource.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                Debug.Assert(dataGrid.DataConnection != null);

                if (dataGrid.LoadingOrUnloadingRow)
                {
                    dataGrid.SetValueNoCallback(ItemsSourceProperty, e.OldValue);
                    throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
                }

                // Try to commit edit on the old DataSource, but force a cancel if it fails
                if (!dataGrid.CommitEdit())
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false);
                }

                dataGrid.DataConnection.UnWireEvents(dataGrid.DataConnection.DataSource);
                dataGrid.DataConnection.ClearDataProperties();
                dataGrid.ClearRowGroupHeadersTable();

                // The old selected indexes are no longer relevant. There's a perf benefit from
                // updating the selected indexes with a null DataSource, because we know that all
                // of the previously selected indexes have been removed from selection
                dataGrid.DataConnection.DataSource = null;
                dataGrid._selectedItems.UpdateIndexes();
                dataGrid.CoerceSelectedItem();

                // Wrap an IEnumerable in an ICollectionView if it's not already one
                bool setDefaultSelection = false;
                IEnumerable newItemsSource = (IEnumerable)e.NewValue;
                if (newItemsSource != null && !(newItemsSource is ICollectionView))
                {
                    dataGrid.DataConnection.DataSource = DataGridDataConnection.CreateView(newItemsSource);
                }
                else
                {
                    dataGrid.DataConnection.DataSource = newItemsSource;
                    setDefaultSelection = true;
                }

                if (dataGrid.DataConnection.DataSource != null)
                {
                    // Setup the column headers
                    if (dataGrid.DataConnection.DataType != null)
                    {
                        foreach (DataGridBoundColumn boundColumn in dataGrid.ColumnsInternal.GetDisplayedColumns(column => column is DataGridBoundColumn))
                        {
                            boundColumn.SetHeaderFromBinding();
                        }
                    }
                    dataGrid.DataConnection.WireEvents(dataGrid.DataConnection.DataSource);
                }

                // Wait for the current cell to be set before we raise any SelectionChanged events
                dataGrid._makeFirstDisplayedCellCurrentCellPending = true;

                // Clear out the old rows and remove the generated columns
                dataGrid.ClearRows(false /*recycle*/);
                dataGrid.RemoveAutoGeneratedColumns();

                // Set the SlotCount (from the data count and number of row group headers) before we make the default selection
                dataGrid.PopulateRowGroupHeadersTable();
                dataGrid.SelectedItem = null;
                if (dataGrid.DataConnection.CollectionView != null && setDefaultSelection)
                {
                    dataGrid.SelectedItem = dataGrid.DataConnection.CollectionView.CurrentItem;
                }

                // Treat this like the DataGrid has never been measured because all calculations at
                // this point are invalid until the next layout cycle.  For instance, the ItemsSource
                // can be set when the DataGrid is not part of the visual tree
                dataGrid._measured = false;
                dataGrid.InvalidateMeasure();
            }
        }

#endregion ItemsSource

#region MaxColumnWidth
        /// <summary>
        /// Gets or sets the maximum width of columns in the <see cref="T:System.Windows.Controls.DataGrid" /> . 
        /// </summary>
        public double MaxColumnWidth
        {
            get { return (double)GetValue(MaxColumnWidthProperty); }
            set { SetValue(MaxColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxColumnWidthProperty =
            DependencyProperty.Register(
                "MaxColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMaxColumnWidth, OnMaxColumnWidthPropertyChanged));

        /// <summary>
        /// MaxColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaxColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MaxColumnWidth");
                }
                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", 0);
                }
                if (dataGrid.MinColumnWidth > newValue)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", "MinColumnWidth");
                }
                foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
                {
                    dataGrid.OnColumnMaxWidthChanged(column, Math.Min(column.MaxWidth, oldValue));
                }
            }
        }
#endregion MaxColumnWidth

#region MinColumnWidth
        /// <summary>
        /// Gets or sets the minimum width of columns in the <see cref="T:System.Windows.Controls.DataGrid" />. 
        /// </summary>
        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MinColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(
                "MinColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMinColumnWidth, OnMinColumnWidthPropertyChanged));

        /// <summary>
        /// MinColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMinColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                if (double.IsNaN(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinColumnWidth");
                }
                if (newValue < 0)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinColumnWidth", 0);
                }
                if (double.IsPositiveInfinity(newValue))
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinColumnWidth");
                }
                if (dataGrid.MaxColumnWidth < newValue)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinColumnWidth", "MaxColumnWidth");
                }
                foreach (DataGridColumn column in dataGrid.ColumnsInternal.GetDisplayedColumns())
                {
                    dataGrid.OnColumnMinWidthChanged(column, Math.Max(column.MinWidth, oldValue));
                }
            }
        }
#endregion MinColumnWidth

#region RowBackground
        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint row backgrounds.
        /// </summary>
        public Brush RowBackground
        {
            get { return GetValue(RowBackgroundProperty) as Brush; }
            set { SetValue(RowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowBackground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register("RowBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(OnRowBackgroundPropertyChanged));

        private static void OnRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            // Go through the Displayed rows and update the background
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }
#endregion RowBackground

#region RowDetailsTemplate
        /// <summary>
        /// Gets or sets the template that is used to display the content of the details section of rows.
        /// </summary>
        public DataTemplate RowDetailsTemplate
        {
            get { return GetValue(RowDetailsTemplateProperty) as DataTemplate; }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsTemplateProperty =
            DependencyProperty.Register(
                "RowDetailsTemplate",
                typeof(DataTemplate),
                typeof(DataGrid),
                new PropertyMetadata(OnRowDetailsTemplatePropertyChanged));

        private static void OnRowDetailsTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            // Update the RowDetails templates if necessary
            if (dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    if (dataGrid.GetRowDetailsVisibility(row.Index) == Visibility.Visible)
                    {
                        // DetailsPreferredHeight is initialized when the DetailsElement's size changes.
                        row.ApplyDetailsTemplate(false /*initializeDetailsPreferredHeight*/);
                    }
                }
            }

            dataGrid.UpdateRowDetailsHeightEstimate();
            dataGrid.InvalidateMeasure();
        }

#endregion RowDetailsTemplate

#region RowDetailsVisibilityMode
        /// <summary>
        /// Gets or sets a value that indicates when the details sections of rows are displayed.
        /// </summary>
        public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
        {
            get { return (DataGridRowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty); }
            set { SetValue(RowDetailsVisibilityModeProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsVisibilityMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsVisibilityModeProperty =
            DependencyProperty.Register(
                "RowDetailsVisibilityMode",
                typeof(DataGridRowDetailsVisibilityMode),
                typeof(DataGrid),
                new PropertyMetadata(OnRowDetailsVisibilityModePropertyChanged));

        /// <summary>
        /// RowDetailsVisibilityModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowDetailsVisibilityMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowDetailsVisibilityModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            dataGrid.UpdateRowDetailsVisibilityMode((DataGridRowDetailsVisibilityMode)e.NewValue);
        }
#endregion RowDetailsVisibilityMode

#region RowHeight
        /// <summary>
        /// Gets or sets the standard height of rows in the control.
        /// </summary>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(
                "RowHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeightPropertyChanged));

        /// <summary>
        /// RowHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "This parameter is exposed to the user as a 'RowHeight' dependency property.")]
        private static void OnRowHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;

                if (value < DataGridRow.DATAGRIDROW_minimumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeight", 0);
                }
                if (value > DataGridRow.DATAGRIDROW_maximumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeight", DataGridRow.DATAGRIDROW_maximumHeight);
                }

                dataGrid.InvalidateRowHeightEstimate();
                // Re-measure all the rows due to the Height change
                dataGrid.InvalidateRowsMeasure(true);
                // DataGrid needs to update the layout information and the ScrollBars
                dataGrid.InvalidateMeasure();
            }
        }
#endregion RowHeight

#region RowHeaderWidth
        /// <summary>
        /// Gets or sets the width of the row header column.
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeaderWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderWidthProperty =
            DependencyProperty.Register(
                "RowHeaderWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeaderWidthPropertyChanged));

        /// <summary>
        /// RowHeaderWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeaderWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowHeaderWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;

                if (value < DATAGRID_minimumRowHeaderWidth)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_minimumRowHeaderWidth);
                }
                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_maxHeadersThickness);
                }
                dataGrid.EnsureRowHeaderWidth();
            }
        }
#endregion RowHeaderWidth

#region RowHeaderStyle
        /// <summary>
        /// Gets or sets the style that is used when rendering the row headers. 
        /// </summary>
        public Style RowHeaderStyle
        {
            get { return GetValue(RowHeaderStyleProperty) as Style; }
            set { SetValue(RowHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowHeaderStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderStyleProperty = DependencyProperty.Register("RowHeaderStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(OnRowHeaderStylePropertyChanged));

        private static void OnRowHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null && dataGrid._rowsPresenter != null)
            {
                // Set HeaderStyle for displayed rows
                Style previousStyle = e.OldValue as Style;
                foreach (UIElement element in dataGrid._rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        row.EnsureHeaderStyleAndVisibility(previousStyle);
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.EnsureHeaderStyleAndVisibility(previousStyle);
                        }
                    }
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
#endregion RowHeaderStyle

#region RowStyle
        /// <summary>
        /// Gets or sets the style that is used when rendering the rows.
        /// </summary>
        public Style RowStyle
        {
            get { return GetValue(RowStyleProperty) as Style; }
            set { SetValue(RowStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register(
                "RowStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(OnRowStylePropertyChanged));

        private static void OnRowStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    // Set the style for displayed rows if it has not already been set
                    foreach (DataGridRow row in dataGrid.GetAllRows())
                    {
                        EnsureElementStyle(row, e.OldValue as Style, e.NewValue as Style);
                    }
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
#endregion RowStyle

#region SelectionMode
        /// <summary>
        /// Gets or sets the selection behavior of the data grid.
        /// </summary>
        public DataGridSelectionMode SelectionMode
        {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(DataGridSelectionMode),
                typeof(DataGrid),
                new PropertyMetadata(OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectionMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
            }
        }
#endregion SelectionMode

#region SelectedIndex
        /// <summary>
        /// Gets or sets the index of the current selection.
        /// </summary>
        /// <returns>
        /// The index of the current selection, or -1 if the selection is empty.
        /// </returns> 
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        /// <summary>
        /// SelectedIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedIndex.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                int index = (int)e.NewValue;

                // GetDataItem returns null if index is >= Count, we do not check newValue 
                // against Count here to avoid enumerating through an Enumerable twice
                // Setting SelectedItem coerces the finally value of the SelectedIndex
                object newSelectedItem = (index < 0) ? null : dataGrid.DataConnection.GetDataItem(index);
                dataGrid.SelectedItem = newSelectedItem;
                if (dataGrid.SelectedItem != newSelectedItem)
                {
                    d.SetValueNoCallback(e.Property, e.OldValue);
                }
            }
        }
#endregion SelectedIndex

#region SelectedItem
        /// <summary>
        /// Gets or sets the data item corresponding to the selected row.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(DataGrid),
                new PropertyMetadata(OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedItem.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            if (!dataGrid.AreHandlersSuspended())
            {
                int rowIndex = (e.NewValue == null) ? -1 : dataGrid.DataConnection.IndexOf(e.NewValue);
                if (rowIndex == -1)
                {
                    // If the Item is null or it's not found, clear the Selection
                    if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                    {
                        // Edited value couldn't be committed or aborted
                        d.SetValueNoCallback(e.Property, e.OldValue);
                        return;
                    }

                    // Clear all row selections
                    dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
                }
                else
                {
                    int slot = dataGrid.SlotFromRowIndex(rowIndex);
                    if (slot != dataGrid.CurrentSlot)
                    {
                        if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                        {
                            // Edited value couldn't be committed or aborted
                            d.SetValueNoCallback(e.Property, e.OldValue);
                            return;
                        }
                        if (slot >= dataGrid.SlotCount || slot < -1)
                        {
                            if (dataGrid.DataConnection.CollectionView != null)
                            {
                                dataGrid.DataConnection.CollectionView.MoveCurrentToPosition(rowIndex);
                            }
                        }
                    }

                    int oldSelectedIndex = dataGrid.SelectedIndex;
                    dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, rowIndex);
                    try
                    {
                        dataGrid._noSelectionChangeCount++;
                        int columnIndex = dataGrid.CurrentColumnIndex;

                        if (columnIndex == -1)
                        {
                            columnIndex = dataGrid.FirstDisplayedNonFillerColumnIndex;
                        }
                        if (dataGrid.IsSlotOutOfSelectionBounds(slot))
                        {
                            dataGrid.ClearRowSelection(slot /*slotException*/, true /*resetAnchorSlot*/);
                            return;
                        }

                        dataGrid.UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, false /*scrollIntoView*/);
                    }
                    finally
                    {
                        dataGrid.NoSelectionChangeCount--;
                    }

                    if (!dataGrid._successfullyUpdatedSelection)
                    {
                        dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, oldSelectedIndex);
                        d.SetValueNoCallback(e.Property, e.OldValue);
                    }
                }
            }
        }
#endregion SelectedItem

#region VerticalGridLinesBrush
        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating columns. 
        /// </summary>
        public Brush VerticalGridLinesBrush
        {
            get { return GetValue(VerticalGridLinesBrushProperty) as Brush; }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register(
                "VerticalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(OnVerticalGridLinesBrushPropertyChanged));

        /// <summary>
        /// VerticalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }
            }
        }
#endregion VerticalGridLinesBrush

#region VerticalScrollBarVisibility
        /// <summary>
        /// Gets or sets a value that indicates how the vertical scroll bar is displayed.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "VerticalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnVerticalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// VerticalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() &&
                (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue &&
                dataGrid._vScrollBar != null)
            {
                dataGrid.InvalidateMeasure();
            }
        }
#endregion VerticalScrollBarVisibility

#endregion Dependency Properties

#region Public Properties    

        /// <summary>
        /// Gets a collection that contains all the columns in the control.
        /// </summary>      
        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                // we use a backing field here because the field's type
                // is a subclass of the property's
                return this.ColumnsInternal;
            }
        }

        /// <summary>
        /// Gets or sets the column that contains the current cell.
        /// </summary>
        public DataGridColumn CurrentColumn
        {
            get
            {
                if (this.CurrentColumnIndex == -1)
                {
                    return null;
                }
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                return this.ColumnsItemsInternal[this.CurrentColumnIndex];
            }
            set
            {
                DataGridColumn dataGridColumn = value;
                if (dataGridColumn == null)
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
                }
                if (this.CurrentColumn != dataGridColumn)
                {
                    if (dataGridColumn.OwningGrid != this)
                    {
                        // Provided column does not belong to this DataGrid
                        throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
                    }
                    if (dataGridColumn.Visibility == Visibility.Collapsed)
                    {
                        // CurrentColumn cannot be set to an invisible column
                        throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
                    }
                    if (this.CurrentSlot == -1)
                    {
                        // There is no current row so the current column cannot be set
                        throw DataGridError.DataGrid.NoCurrentRow();
                    }
                    bool beginEdit = this._editingColumnIndex != -1;
                    if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
                    {
                        // Edited value couldn't be committed or aborted
                        return;
                    }
                    this.UpdateSelectionAndCurrency(dataGridColumn.Index, this.CurrentSlot, DataGridSelectionAction.None, false /*scrollIntoView*/);
                    Debug.Assert(this._successfullyUpdatedSelection);
                    if (beginEdit && 
                        this._editingColumnIndex == -1 && 
                        this.CurrentSlot != -1 &&
                        this.CurrentColumnIndex != -1 && 
                        this.CurrentColumnIndex == dataGridColumn.Index &&
                        dataGridColumn.OwningGrid == this &&
                        !GetColumnEffectiveReadOnlyState(dataGridColumn))
                    {
                        // Returning to editing mode since the grid was in that mode prior to the EndCellEdit call above.
                        BeginCellEdit(new RoutedEventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the row group header.
        /// </summary>
        public ObservableCollection<Style> RowGroupHeaderStyles
        {
            get
            {
                return _rowGroupHeaderStyles;
            }
        }

        /// <summary>
        /// Gets a list that contains the data items corresponding to the selected rows.
        /// </summary>
        public IList SelectedItems
        {
            get { return _selectedItems as IList; }
        }

#endregion Public Properties

#region Protected Properties

        /// <summary>
        /// Gets the data item bound to the row that contains the current cell.
        /// </summary>
        protected object CurrentItem
        {
            get
            {
                if (this.CurrentSlot == -1 || this.ItemsSource /*this.DataConnection.DataSource*/ == null || this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                {
                    return null;
                }
                return this.DataConnection.GetDataItem(RowIndexFromSlot(this.CurrentSlot));
            }
        }

#endregion Protected Properties

#region Internal Properties

        internal int AnchorSlot
        {
            get;
            private set;
        }

        internal double ActualRowHeaderWidth
        {
            get
            {
                if (!this.AreRowHeadersVisible)
                {
                    return 0;
                }
                else
                {
                    return !double.IsNaN(this.RowHeaderWidth) ? this.RowHeaderWidth : this.RowHeadersDesiredWidth;
                }
            }
        }

        internal double ActualRowsPresenterHeight
        {
            get
            {
                if (this._rowsPresenter != null)
                {
                    return this._rowsPresenter.ActualHeight;
                }
                return 0;
            }
        }

        internal bool AreColumnHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column;
            }
        }

        internal bool AreRowHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;
            }
        }

        /// <summary>
        /// Indicates whether or not at least one auto-sizing column is waiting for all the rows
        /// to be measured before its final width is determined.
        /// </summary>
        internal bool AutoSizingColumns
        {
            get
            {
                return _autoSizingColumns;
            }
            set
            {
                if (_autoSizingColumns && value == false && this.ColumnsInternal != null)
                {
                    double adjustment = this.CellsWidth - this.ColumnsInternal.VisibleEdgedColumnsWidth;
                    this.AdjustColumnWidths(0, adjustment, false);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        column.IsInitialDesiredWidthDetermined = true;
                    }
                    this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                    this.ComputeScrollBarsLayout();
                    InvalidateColumnHeadersMeasure();
                    InvalidateRowsMeasure(true);
                }
                _autoSizingColumns = value;
            }
        }

        internal double AvailableSlotElementRoom
        {
            get;
            set;
        }

        // Height currently available for cells this value is smaller.  This height is reduced by the existence of ColumnHeaders
        // or a horizontal scrollbar.  Layout is asynchronous so changes to the ColumnHeaders or the horizontal scrollbar are 
        // not reflected immediately.
        internal double CellsHeight
        {
            get
            {
                return this.RowsPresenterAvailableSize.HasValue ? this.RowsPresenterAvailableSize.Value.Height : 0;
            }
        }

        // Width currently available for cells this value is smaller.  This width is reduced by the existence of RowHeaders
        // or a vertical scrollbar.  Layout is asynchronous so changes to the RowHeaders or the vertical scrollbar are
        // not reflected immediately
        internal double CellsWidth
        {
            get
            {
                double rowsWidth = double.PositiveInfinity;
                if (this.RowsPresenterAvailableSize.HasValue)
                {
                    rowsWidth = Math.Max(0, this.RowsPresenterAvailableSize.Value.Width - this.ActualRowHeaderWidth);
                }
                return double.IsPositiveInfinity(rowsWidth) ? this.ColumnsInternal.VisibleEdgedColumnsWidth : rowsWidth;
            }
        }

        /// <summary>
        /// This is an empty content control that's used during the DataGrid's copy procedure
        /// to determine the value of a ClipboardContentBinding for a particular column and item.
        /// </summary>
        internal ContentControl ClipboardContentControl
        {
            get
            {
                if (this._clipboardContentControl == null)
                {
                    this._clipboardContentControl = new ContentControl();
                }
                return this._clipboardContentControl;
            }
        }

        internal DataGridColumnHeadersPresenter ColumnHeaders
        {
            get
            {
                return this._columnHeadersPresenter;
            }
        }

        internal DataGridColumnCollection ColumnsInternal
        {
            get;
            private set;
        }

        internal List<DataGridColumn> ColumnsItemsInternal
        {
            get
            {
                return this.ColumnsInternal.ItemsInternal;
            }
        }

        internal bool ContainsFocus
        {
            get;
            private set;
        }

        internal int CurrentColumnIndex
        {
            get
            {
                return this.CurrentCellCoordinates.ColumnIndex;
            }

            private set
            {
                this.CurrentCellCoordinates.ColumnIndex = value;
            }
        }

        internal int CurrentSlot
        {
            get
            {
                return this.CurrentCellCoordinates.Slot;
            }

            private set
            {
                this.CurrentCellCoordinates.Slot = value;
            }
        }

        internal DataGridDataConnection DataConnection
        {
            get;
            private set;
        }

        internal DataGridDisplayData DisplayData
        {
            get;
            private set;
        }

        internal int EditingColumnIndex
        {
            get
            {
                return this._editingColumnIndex;
            }
        }

        internal DataGridRow EditingRow
        {
            get;
            private set;
        }

        internal double FirstDisplayedScrollingColumnHiddenWidth
        {
            get
            {
                return this._negHorizontalOffset;
            }
        }

        // When the RowsPresenter's width increases, the HorizontalOffset will be incorrect until
        // the scrollbar's layout is recalculated, which doesn't occur until after the cells are measured.
        // This property exists to account for this scenario, and avoid collapsing the incorrect cells.
        internal double HorizontalAdjustment
        {
            get;
            private set;
        }

        internal static double HorizontalGridLinesThickness
        {
            get
            {
                return DATAGRID_horizontalGridLinesThickness;
            }
        }

        // the sum of the widths in pixels of the scrolling columns preceding 
        // the first displayed scrolling column
        internal double HorizontalOffset
        {
            get
            {
                return this._horizontalOffset;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                double widthNotVisible = Math.Max(0, this.ColumnsInternal.VisibleEdgedColumnsWidth - this.CellsWidth);
                if (value > widthNotVisible)
                {
                    value = widthNotVisible;
                }
                if (value == this._horizontalOffset)
                {
                    return;
                }

                if (this._hScrollBar != null && value != this._hScrollBar.Value)
                {
                    this._hScrollBar.Value = value;
                }
                this._horizontalOffset = value;

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // update the lastTotallyDisplayedScrollingCol
                ComputeDisplayedColumns();
            }
        }

        internal ScrollBar HorizontalScrollBar
        {
            get
            {
                return _hScrollBar;
            }
        }

        internal bool LoadingOrUnloadingRow
        {
            get;
            private set;
        }

        internal bool InDisplayIndexAdjustments
        {
            get;
            set;
        }

        internal int? MouseOverRowIndex
        {
            get
            {
                return this._mouseOverRowIndex;
            }
            set
            {
                if (this._mouseOverRowIndex != value)
                {
                    DataGridRow oldMouseOverRow = null;
                    if (_mouseOverRowIndex.HasValue)
                    {
                        int oldSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                        if (IsSlotVisible(oldSlot))
                        {
                            oldMouseOverRow = this.DisplayData.GetDisplayedElement(oldSlot) as DataGridRow;
                        }
                    }

                    _mouseOverRowIndex = value;

                    // State for the old row needs to be applied after setting the new value
                    if (oldMouseOverRow != null)
                    {
                        oldMouseOverRow.ApplyState(true /*animate*/);
                    }

                    if (_mouseOverRowIndex.HasValue)
                    {
                        int newSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                        if (IsSlotVisible(newSlot))
                        {
                            DataGridRow newMouseOverRow = this.DisplayData.GetDisplayedElement(newSlot) as DataGridRow;
                            Debug.Assert(newMouseOverRow != null);
                            if (newMouseOverRow != null)
                            {
                                newMouseOverRow.ApplyState(true /*animate*/);
                            }
                        }
                    }
                }
            }
        }

        internal double NegVerticalOffset
        {
            get;
            private set;
        }

        internal int NoCurrentCellChangeCount
        {
            get
            {
                return this._noCurrentCellChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                this._noCurrentCellChangeCount = value;
                if (value == 0)
                {
                    FlushCurrentCellChanged();
                }
            }
        }

        internal double RowDetailsHeightEstimate
        {
            get;
            private set;
        }

        internal double RowHeadersDesiredWidth
        {
            get
            {
                return _rowHeaderDesiredWidth;
            }
            set
            {
                // We only auto grow
                if (_rowHeaderDesiredWidth < value)
                {
                    double oldActualRowHeaderWidth = this.ActualRowHeaderWidth;
                    _rowHeaderDesiredWidth = value;
                    if (oldActualRowHeaderWidth != this.ActualRowHeaderWidth)
                    {
                        EnsureRowHeaderWidth();
                    }
                }
            }
        }

        internal double RowGroupHeaderHeightEstimate
        {
            get;
            private set;
        }

        internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable
        {
            get;
            private set;
        }

        internal double[] RowGroupSublevelIndents
        {
            get;
            private set;
        }

        internal double RowHeightEstimate
        {
            get;
            private set;
        }

        internal Size? RowsPresenterAvailableSize
        {
            get
            {
                return this._rowsPresenterAvailableSize;
            }
            set
            {
                if (this._rowsPresenterAvailableSize.HasValue && value.HasValue && value.Value.Width > this.RowsPresenterAvailableSize.Value.Width)
                {
                    // When the available cells width increases, the horizontal offset can be incorrect.
                    // Store away an adjustment to use during the CellsPresenter's measure, so that the
                    // ShouldDisplayCell method correctly determines if a cell will be in view.
                    //
                    //     |   h. offset   |       new available cells width          |
                    //     |-------------->|----------------------------------------->|
                    //      __________________________________________________        |
                    //     |           |           |             |            |       |
                    //     |  column0  |  column1  |   column2   |  column3   |<----->|
                    //     |           |           |             |            |  adj. |
                    //
                    double adjustment = (this._horizontalOffset + value.Value.Width) - this.ColumnsInternal.VisibleEdgedColumnsWidth;
                    this.HorizontalAdjustment = Math.Min(this.HorizontalOffset, Math.Max(0, adjustment));
                }
                else
                {
                    this.HorizontalAdjustment = 0;
                }
                this._rowsPresenterAvailableSize = value;
            }
        }

        // This flag indicates whether selection has actually changed during a selection operation,
        // and exists to ensure that FlushSelectionChanged doesn't unnecessarily raise SelectionChanged.
        internal bool SelectionHasChanged
        {
            get;
            set;
        }

        internal int SlotCount
        {
            get;
            private set;
        }

        internal bool UpdatedStateOnMouseLeftButtonDown
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether or not to use star-sizing logic.  If the DataGrid has infinite available space,
        /// then star sizing doesn't make sense.  In this case, all star columns grow to a predefined size of
        /// 10,000 pixels in order to show the developer that star columns shouldn't be used.
        /// </summary>
        internal bool UsesStarSizing
        {
            get
            {
                if (this.ColumnsInternal != null)
                {
                    return this.ColumnsInternal.VisibleStarColumnCount > 0 &&
                        (!this.RowsPresenterAvailableSize.HasValue || !double.IsPositiveInfinity(this.RowsPresenterAvailableSize.Value.Width));
                }
                return false;
            }
        }

        internal ScrollBar VerticalScrollBar
        {
            get
            {
                return _vScrollBar;
            }
        }

        internal int VisibleSlotCount
        {
            get;
            set;
        }

#endregion Internal Properties

#region Private Properties

        private DataGridCellCoordinates CurrentCellCoordinates
        {
            get
            {
                return this._currentCellCoordinates;
            }

            set
            {
                this._currentCellCoordinates = value;
            }
        }

        private int FirstDisplayedNonFillerColumnIndex
        {
            get
            {
                DataGridColumn column = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                if (column != null)
                {
                    if (column.IsFrozen)
                    {
                        return column.Index;
                    }
                    else
                    {
                        if (this.DisplayData.FirstDisplayedScrollingCol >= column.Index)
                        {
                            return this.DisplayData.FirstDisplayedScrollingCol;
                        }
                        else
                        {
                            return column.Index;
                        }
                    }
                }
                return -1;
            }
        }

        private int NoSelectionChangeCount
        {
            get
            {
                return this._noSelectionChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                this._noSelectionChangeCount = value;
                if (value == 0)
                {
                    FlushSelectionChanged();
                }
            }
        }

#endregion Private Properties

#region Public Methods

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit()
        {
            return BeginEdit(null);
        }

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <param name="editingEventArgs">Provides information about the user gesture that caused the call to BeginEdit. Can be null.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot);

            if (GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                // Current column is read-only
                return false;
            }
            return BeginCellEdit(editingEventArgs);
        }

        /// <summary>
        /// Cancels editing mode and restores the original value.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit()
        {
            return CancelEdit(DataGridEditingUnit.Row);
        }

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit(DataGridEditingUnit editingUnit)
        {
            return this.CancelEdit(editingUnit, true /*raiseEvents*/);
        }

        /// <summary>
        /// Commits editing mode and pushes changes to the backend.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit()
        {
            return CommitEdit(DataGridEditingUnit.Row, true);
        }

        /// <summary>
        /// Commits editing mode for the specified DataGridEditingUnit and pushes changes to the backend.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to commit edit for a Cell or Row.</param>
        /// <param name="exitEditingMode">Editing mode is left if True.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
        {
            if (!EndCellEdit(DataGridEditAction.Commit, editingUnit == DataGridEditingUnit.Cell ? exitEditingMode : true, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
            {
                return false;
            }
            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, true /*raiseEvents*/);
            }
            return true;
        }

        /// <summary>
        /// Returns the Group at the indicated level or null if the item is not in the ItemsSource
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="groupLevel">groupLevel</param>
        /// <returns>The group the given item falls under or null if the item is not in the ItemsSource</returns>
        public CollectionViewGroup GetGroupFromItem(object item, int groupLevel)
        {
            int itemIndex = this.DataConnection.IndexOf(item);
            if (itemIndex == -1)
            {
                return null;
            }
            int groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(itemIndex));
            DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            while (rowGroupInfo != null && rowGroupInfo.Level != groupLevel)
            {
                groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(rowGroupInfo.Slot);
                rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            }
            return rowGroupInfo == null ? null : rowGroupInfo.CollectionViewGroup;
        }

        /// <summary>
        /// Scrolls the specified item or RowGroupHeader and/or column into view.
        /// If item is not null: scrolls the row representing the item into view;
        /// If column is not null: scrolls the column into view;
        /// If both item and column are null, the method returns without scrolling.
        /// </summary>
        /// <param name="item">an item from the DataGrid's items source or a CollectionViewGroup from the collection view</param>
        /// <param name="column">a column from the DataGrid's columns collection</param>
        public void ScrollIntoView(object item, DataGridColumn column)
        {
            if ((column == null && (item == null || this.FirstDisplayedNonFillerColumnIndex == -1)) 
                || (column != null && column.OwningGrid != this))
            {
                // no-op
                return;
            }
            if (item == null)
            {
                // scroll column into view
                this.ScrollSlotIntoView(column.Index, this.DisplayData.FirstScrollingSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
            else
            {
                int slot = -1;
                CollectionViewGroup collectionViewGroup = item as CollectionViewGroup;
                DataGridRowGroupInfo rowGroupInfo = null;
                if (collectionViewGroup != null)
                {
                    rowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                    if (rowGroupInfo == null)
                    {
                        Debug.Assert(false);
                        return;
                    }
                    slot = rowGroupInfo.Slot;
                }
                else
                {
                    // the row index will be set to -1 if the item is null or not in the list
                    int rowIndex = this.DataConnection.IndexOf(item);
                    if (rowIndex == -1)
                    {
                        return;
                    }
                    slot = SlotFromRowIndex(rowIndex);
                }

                int columnIndex = (column == null) ? this.FirstDisplayedNonFillerColumnIndex : column.Index;

                if (_collapsedSlotsTable.Contains(slot))
                {
                    // We need to expand all parent RowGroups so that the slot is visible
                    if (rowGroupInfo != null)
                    {
                        ExpandRowGroupParentChain(rowGroupInfo.Level - 1, rowGroupInfo.Slot);
                    }
                    else
                    {
                        rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(this.RowGroupHeadersTable.GetPreviousIndex(slot));
                        Debug.Assert(rowGroupInfo != null);
                        if (rowGroupInfo != null)
                        {
                            ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                        }
                    }

                    // Update Scrollbar and display information
                    this.NegVerticalOffset = 0;
                    SetVerticalOffset(0);
                    ResetDisplayedRows();
                    this.DisplayData.FirstScrollingSlot = 0;
                    ComputeScrollBarsLayout();
                }

                ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
        }

#endregion Public Methods

#region Protected Methods

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.DataGridRow" />.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.DataGridRow" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_makeFirstDisplayedCellCurrentCellPending)
            {
                MakeFirstDisplayedCellCurrentCell();
            }

            if (this.ActualWidth != finalSize.Width)
            {
                // If our final width has changed, we might need to update the filler
                InvalidateColumnHeadersArrange();
                InvalidateCellsArrange();
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.DataGridRow" /> to prepare for 
        /// arranging them during the 
        /// <see cref="M:System.Windows.Controls.DataGridRow.ArrangeOverride(System.Windows.Size)" /> pass. 
        /// </summary>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.DataGridRow" /> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that 
        /// child elements should not exceed.
        /// </param>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Delay layout until after the initial measure to avoid invalid calculations when the 
            // DataGrid is not part of the visual tree
            if (!_measured)
            {
                _measured = true;

                // We don't need to clear the rows because it was already done when the ItemsSource changed
                RefreshRowsAndColumns(false /*clearRows*/);

                // Update our estimates now that the DataGrid has all of the information necessary
                UpdateRowDetailsHeightEstimate();

                // Update frozen columns to account for columns added prior to loading or autogenerated columns
                if (this.FrozenColumnCountWithFiller > 0)
                {
                    ProcessFrozenColumnCount(this);
                }
            }

            Size desiredSize;
            // This is a shortcut to skip layout if we don't have any columns
            if (this.ColumnsInternal.VisibleEdgedColumnsWidth == 0)
            {
                if (_hScrollBar != null && _hScrollBar.Visibility != Visibility.Collapsed)
                {
                    _hScrollBar.Visibility = Visibility.Collapsed;
                }
                if (_vScrollBar != null && _vScrollBar.Visibility != Visibility.Collapsed)
                {
                    _vScrollBar.Visibility = Visibility.Collapsed;
                }
                desiredSize = base.MeasureOverride(availableSize);
            }
            else
            {
                if (_rowsPresenter != null)
                {
                    _rowsPresenter.InvalidateMeasure();
                }

                InvalidateColumnHeadersMeasure();

                desiredSize = base.MeasureOverride(availableSize);

                ComputeScrollBarsLayout();
            }

            return desiredSize;
        }

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // The template has changed, so we need to refresh the visuals
            this._measured = false;

            if (this._columnHeadersPresenter != null)
            {
                // If we're applying a new template, we want to remove the old column headers first
                this._columnHeadersPresenter.Children.Clear();
            }
            _columnHeadersPresenter = GetTemplateChild(DATAGRID_elementColumnHeadersPresenterName) as DataGridColumnHeadersPresenter;
            if (_columnHeadersPresenter != null)
            {
                if (this.ColumnsInternal.FillerColumn != null)
                {
                    this.ColumnsInternal.FillerColumn.IsRepresented = false;
                }
                _columnHeadersPresenter.OwningGrid = this;
                // Columns were added before before our Template was applied, add the ColumnHeaders now
                foreach (DataGridColumn column in this.ColumnsItemsInternal)
                {
                    InsertDisplayedColumnHeader(column);
                }
            }

            if (this._rowsPresenter != null)
            {
                // If we're applying a new template, we want to remove the old rows first
                this.UnloadElements(false /*recycle*/);
            }
            _rowsPresenter = GetTemplateChild(DATAGRID_elementRowsPresenterName) as DataGridRowsPresenter;
            if (_rowsPresenter != null)
            {
                _rowsPresenter.OwningGrid = this;
                InvalidateRowHeightEstimate();
                UpdateRowDetailsHeightEstimate();
            }
            _frozenColumnScrollBarSpacer = GetTemplateChild(DATAGRID_elementFrozenColumnScrollBarSpacerName) as FrameworkElement;

            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll -= new ScrollEventHandler(HorizontalScrollBar_Scroll);
            }
            _hScrollBar = GetTemplateChild(DATAGRID_elementHorizontalScrollbarName) as ScrollBar;
            if (_hScrollBar != null)
            {
                _hScrollBar.IsTabStop = false;
                _hScrollBar.Maximum = 0.0;
                _hScrollBar.Orientation = Orientation.Horizontal;
                _hScrollBar.Visibility = Visibility.Collapsed;
                _hScrollBar.Scroll += new ScrollEventHandler(HorizontalScrollBar_Scroll);
            }

            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll -= new ScrollEventHandler(VerticalScrollBar_Scroll);
            }
            _vScrollBar = GetTemplateChild(DATAGRID_elementVerticalScrollbarName) as ScrollBar;
            if (_vScrollBar != null)
            {
                _vScrollBar.IsTabStop = false;
                _vScrollBar.Maximum = 0.0;
                _vScrollBar.Orientation = Orientation.Vertical;
                _vScrollBar.Visibility = Visibility.Collapsed;
                _vScrollBar.Scroll += new ScrollEventHandler(VerticalScrollBar_Scroll);
            }

            _topLeftCornerHeader = GetTemplateChild(DATAGRID_elementTopLeftCornerHeaderName) as ContentControl;
            EnsureTopLeftCornerHeader(); // EnsureTopLeftCornerHeader checks for a null _topLeftCornerHeader;
            _topRightCornerHeader = GetTemplateChild(DATAGRID_elementTopRightCornerHeaderName) as ContentControl;

            if (this._validationSummary != null)
            {
                this._validationSummary.FocusingInvalidControl -= new EventHandler<FocusingInvalidControlEventArgs>(ValidationSummary_FocusingInvalidControl);
                this._validationSummary.SelectionChanged -= new EventHandler<SelectionChangedEventArgs>(ValidationSummary_SelectionChanged);
            }
            this._validationSummary = GetTemplateChild(DATAGRID_elementValidationSummary) as ValidationSummary;
            if (this._validationSummary != null)
            {
                // The ValidationSummary defaults to using its parent if Target is null, so the only
                // way to prevent it from automatically picking up errors is to set it to some useless element.
                if (this._validationSummary.Target == null)
                {
                    this._validationSummary.Target = new Rectangle();
                }

                this._validationSummary.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(ValidationSummary_FocusingInvalidControl);
                this._validationSummary.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ValidationSummary_SelectionChanged);
                if (DesignerProperties.IsInDesignTool)
                {
                    Debug.Assert(this._validationSummary.Errors != null);
                    // Do not add the default design time errors when in design mode.
                    this._validationSummary.Errors.Clear();
                }
            }

            UpdateDisabledVisual();
        }

        /// <summary>
        /// Raises the AutoGeneratingColumn event.
        /// </summary>
        protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            EventHandler<DataGridAutoGeneratingColumnEventArgs> handler = this.AutoGeneratingColumn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the BeginningEdit event.
        /// </summary>
        protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
        {
            EventHandler<DataGridBeginningEditEventArgs> handler = this.BeginningEdit;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the CellEditEnded event.
        /// </summary>
        protected virtual void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
        {
            EventHandler<DataGridCellEditEndedEventArgs> handler = this.CellEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just ended edit
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the CellEditEnding event.
        /// </summary>
        protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            EventHandler<DataGridCellEditEndingEventArgs> handler = this.CellEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// This method raises the CopyingRowClipboardContent event.
        /// </summary>
        /// <param name="e">Contains the necessary information for generating the row clipboard content.</param>
        protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs e)
        {
            EventHandler<DataGridRowClipboardEventArgs> handler = this.CopyingRowClipboardContent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridAutomationPeer(this);
        }

        /// <summary>
        /// Raises the CurrentCellChanged event.
        /// </summary>
        protected virtual void OnCurrentCellChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.CurrentCellChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationCellSelectedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }
        }

        /// <summary>
        /// Raises the LoadingRow event for row preparation.
        /// </summary>
        protected virtual void OnLoadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.LoadingRow;
            if (handler != null)
            {
                Debug.Assert(!this._loadedRows.Contains(e.Row));
                this._loadedRows.Add(e.Row);
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
                Debug.Assert(this._loadedRows.Contains(e.Row));
                this._loadedRows.Remove(e.Row);
            }
        }

        /// <summary>
        /// Raises the LoadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.LoadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the LoadingRowDetails for row details preparation
        /// </summary>
        protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.LoadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Scrolls the DataGrid according to the direction of the delta.
        /// </summary>
        /// <param name="e">MouseWheelEventArgs</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsEnabled && !e.Handled && this.DisplayData.NumDisplayedScrollingElements > 0)
            {
                double scrollHeight = 0;
                if (e.Delta > 0)
                {
                    scrollHeight = Math.Max(-_verticalOffset, -DATAGRID_mouseWheelDelta);
                }
                else if (e.Delta < 0)
                {
                    if (_vScrollBar != null && this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                    {
                        scrollHeight = Math.Min(Math.Max(0, _vScrollBar.Maximum - _verticalOffset), DATAGRID_mouseWheelDelta);
                    }
                    else
                    {
                        double maximum = this.EdgedRowsHeightCalculated - this.CellsHeight;
                        scrollHeight = Math.Min(Math.Max(0, maximum - _verticalOffset), DATAGRID_mouseWheelDelta);
                    }
                }
                if (scrollHeight != 0)
                {
                    this.DisplayData.PendingVerticalScrollHeight = scrollHeight;
                    InvalidateRowsMeasure(false /*invalidateIndividualRows*/);
                    InvalidateRowsArrange(); // force invalidate
                    e.Handled = true;
                }
            }
        }


        /// <summary>
        /// Raises the PreparingCellForEdit event.
        /// </summary>
        protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            EventHandler<DataGridPreparingCellForEditEventArgs> handler = this.PreparingCellForEdit;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just began edit because now
            // its editable content has been loaded
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnded event.
        /// </summary>
        protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
        {
            EventHandler<DataGridRowEditEndedEventArgs> handler = this.RowEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the row that just ended edit because the edits
            // to its associated item have either been committed or reverted
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnding event.
        /// </summary>
        protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            EventHandler<DataGridRowEditEndingEventArgs> handler = this.RowEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionChanged event and clears the _selectionChanged.
        /// This event won't get raised again until after _selectionChanged is set back to true.
        /// </summary>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationSelectionEvents(e);
                }
            }
        }

        /// <summary>
        /// Raises the UnloadingRow event for row recycling.
        /// </summary>
        protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.UnloadingRow;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the UnloadingRowDetails event
        /// </summary>
        protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.UnloadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the UnLoadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.UnloadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

#endregion Protected Methods

#region Internal Methods

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <param name="raiseEvents">Specifies whether or not to raise editing events</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
        {
            if (!EndCellEdit(DataGridEditAction.Cancel, true, this.ContainsFocus /*keepFocus*/, raiseEvents))
            {
                return false;
            }
            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Cancel, true, raiseEvents);
            }
            return true;
        }

        /// <summary>
        /// call when: selection changes or SelectedItems object changes
        /// </summary>
        internal void CoerceSelectedItem()
        {
            object selectedItem = null;

            if (this.SelectionMode == DataGridSelectionMode.Extended &&
                this.CurrentSlot != -1 &&
                _selectedItems.ContainsSlot(this.CurrentSlot))
            {
                selectedItem = this.CurrentItem;
            }
            else if (_selectedItems.Count > 0)
            {
                selectedItem = _selectedItems[0];
            }

            this.SetValueNoCallback(SelectedItemProperty, selectedItem);

            // Update the SelectedIndex
            int newIndex = -1;
            if (selectedItem != null)
            {
                newIndex = this.DataConnection.IndexOf(selectedItem);
            }
            this.SetValueNoCallback(SelectedIndexProperty, newIndex);
        }

        internal static DataGridCell GetOwningCell(FrameworkElement element)
        {
            Debug.Assert(element != null);
            DataGridCell cell = element as DataGridCell;
            while (element != null && cell == null)
            {
                element = element.Parent as FrameworkElement;
                cell = element as DataGridCell;
            }
            return cell;
        }

        internal IEnumerable<object> GetSelectionInclusive(int startRowIndex, int endRowIndex)
        {
            int endSlot = SlotFromRowIndex(endRowIndex);
            foreach (int slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
            {
                if (slot > endSlot)
                {
                    break;
                }
                yield return this.DataConnection.GetDataItem(RowIndexFromSlot(slot));
            }
        }

        internal void InitializeElements(bool recycleRows)
        {
            try
            {
                this._noCurrentCellChangeCount++;

                // The underlying collection has changed and our editing row (if there is one)
                // is no longer relevant, so we should force a cancel edit.
                CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);

                // We want to persist selection throughout a reset, so store away the selected items
                List<object> selectedItemsCache = new List<object>(this._selectedItems.SelectedItemsCache);

                if (recycleRows)
                {
                    RefreshRows(recycleRows /*recycleRows*/, true /*clearRows*/);
                }
                else
                {
                    RefreshRowsAndColumns(true /*clearRows*/);
                }

                // Re-select the old items
                this._selectedItems.SelectedItemsCache = selectedItemsCache;
                CoerceSelectedItem();
                if (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
                {
                    UpdateRowDetailsVisibilityMode(this.RowDetailsVisibilityMode);
                }

                // The currently displayed rows may have incorrect visual states because of the selection change
                ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
            }
        }

        // 
        internal bool IsDoubleClickRecordsClickOnCall(UIElement element)
        {
            if (_clickedElement == element)
            {
                _clickedElement = null;
                return true;
            }
            else
            {
                _clickedElement = element;
                return false;
            }
        }

        // Returns the item or the CollectionViewGroup that is used as the DataContext for a given slot.
        // If the DataContext is an item, rowIndex is set to the index of the item within the collection
        internal object ItemFromSlot(int slot, ref int rowIndex)
        {
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (groupInfo != null)
                {
                    return groupInfo.CollectionViewGroup;
                }
            }
            else
            {
                rowIndex = RowIndexFromSlot(slot);
                return this.DataConnection.GetDataItem(rowIndex);
            }
            return null;
        }

        internal void OnRowDetailsChanged()
        {
            if (!_scrollingByHeight)
            {
                // Update layout when RowDetails are expanded or collapsed, just updating the vertical scroll bar is not enough 
                // since rows could be added or removed
                InvalidateMeasure();
            }
        }

        internal bool ProcessDownKey()
        {
            bool shift, ctrl;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return ProcessDownKeyInternal(shift, ctrl);
        }

        internal bool ProcessEndKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessEndKey(shift, ctrl);
        }

        internal bool ProcessEnterKey()
        {
            bool ctrl, shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessEnterKey(shift, ctrl);
        }

        internal bool ProcessHomeKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessHomeKey(shift, ctrl);
        }

        internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
        {
            if (this._horizontalScrollChangesIgnored > 0)
            {
                return;
            }

            // If the user scrolls with the buttons, we need to update the new value of the scroll bar since we delay
            // this calculation.  If they scroll in another other way, the scroll bar's correct value has already been set
            double scrollBarValueDifference = 0;
            if (scrollEventType == ScrollEventType.SmallIncrement)
            {
                scrollBarValueDifference = GetHorizontalSmallScrollIncrease();
            }
            else if (scrollEventType == ScrollEventType.SmallDecrement)
            {
                scrollBarValueDifference = -GetHorizontalSmallScrollDecrease();
            }
            this._horizontalScrollChangesIgnored++;
            try
            {
                if (scrollBarValueDifference != 0)
                {
                    Debug.Assert(this._horizontalOffset + scrollBarValueDifference >= 0);
                    this._hScrollBar.Value = this._horizontalOffset + scrollBarValueDifference;
                }
                UpdateHorizontalOffset(this._hScrollBar.Value);
            }
            finally
            {
                this._horizontalScrollChangesIgnored--;
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }
        }

        internal bool ProcessLeftKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessLeftKey(shift, ctrl);
        }

        internal bool ProcessNextKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessNextKey(shift, ctrl);
        }

        internal bool ProcessPriorKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessPriorKey(shift, ctrl);
        }

        internal bool ProcessRightKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessRightKey(shift, ctrl);
        }

        /// <summary>
        /// Selects items and updates currency based on parameters
        /// </summary>
        /// <param name="columnIndex">column index to make current</param>
        /// <param name="item">data item or CollectionViewGroup to make current</param>
        /// <param name="backupSlot">slot to use in case the item is no longer valid</param>
        /// <param name="action">selection action to perform</param>
        /// <param name="scrollIntoView">whether or not the new current item should be scrolled into view</param>
        internal void ProcessSelectionAndCurrency(int columnIndex, object item, int backupSlot, DataGridSelectionAction action, bool scrollIntoView)
        {
            this._noSelectionChangeCount++;
            this._noCurrentCellChangeCount++;
            try
            {
                int slot = -1;
                CollectionViewGroup group = item as CollectionViewGroup;
                if (group != null)
                {
                    DataGridRowGroupInfo groupInfo = this.RowGroupInfoFromCollectionViewGroup(group);
                    if (groupInfo != null)
                    {
                        slot = groupInfo.Slot;
                    }
                }
                else
                {
                    slot = this.SlotFromRowIndex(this.DataConnection.IndexOf(item));
                }
                if (slot == -1)
                {
                    slot = backupSlot;
                }
                if (slot < 0 || slot > this.SlotCount)
                {
                    return;
                }

                switch (action)
                {
                    case DataGridSelectionAction.AddCurrentToSelection:
                        SetRowSelection(slot, true /*isSelected*/, true /*setAnchorIndex*/);
                        break;
                    case DataGridSelectionAction.RemoveCurrentFromSelection:
                        SetRowSelection(slot, false /*isSelected*/, false /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.SelectFromAnchorToCurrent:
                        if (this.SelectionMode == DataGridSelectionMode.Extended && this.AnchorSlot != -1)
                        {
                            int anchorSlot = this.AnchorSlot;
                            ClearRowSelection(slot /*slotException*/, false /*resetAnchorSlot*/);
                            if (slot <= anchorSlot)
                            {
                                SetRowsSelection(slot, anchorSlot);
                            }
                            else
                            {
                                SetRowsSelection(anchorSlot, slot);
                            }
                        }
                        else
                        {
                            goto case DataGridSelectionAction.SelectCurrent;
                        }
                        break;
                    case DataGridSelectionAction.SelectCurrent:
                        ClearRowSelection(slot /*rowIndexException*/, true /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.None:
                        break;
                }

                if (this.CurrentSlot != slot || (this.CurrentColumnIndex != columnIndex && columnIndex != -1))
                {
                    if (columnIndex == -1)
                    {
                        if (this.CurrentColumnIndex != -1)
                        {
                            columnIndex = this.CurrentColumnIndex;
                        }
                        else
                        {
                            DataGridColumn firstVisibleColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                            if (firstVisibleColumn != null)
                            {
                                columnIndex = firstVisibleColumn.Index;
                            }
                        }
                    }
                    if (columnIndex != -1)
                    {
                        if (!SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, SlotFromRowIndex(this.SelectedIndex) != slot /*endRowEdit*/)
                            || (scrollIntoView && !ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, false /*forceHorizontalScroll*/)))
                        {
                            return;
                        }
                    }
                }
                this._successfullyUpdatedSelection = true;
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        internal bool ProcessUpKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessUpKey(shift, ctrl);
        }

        internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
        {
            if (this._verticalScrollChangesIgnored > 0)
            {
                return;
            }
            Debug.Assert(DoubleUtil.LessThanOrClose(this._vScrollBar.Value, this._vScrollBar.Maximum));

            this._verticalScrollChangesIgnored++;
            try
            {
                Debug.Assert(this._vScrollBar != null);
                if (scrollEventType == ScrollEventType.SmallIncrement)
                {
                    this.DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
                    double newVerticalOffset = this._verticalOffset + this.DisplayData.PendingVerticalScrollHeight;
                    if (newVerticalOffset > this._vScrollBar.Maximum)
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= newVerticalOffset - this._vScrollBar.Maximum;
                    }
                }
                else if (scrollEventType == ScrollEventType.SmallDecrement)
                {
                    if (DoubleUtil.GreaterThan(this.NegVerticalOffset, 0))
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= this.NegVerticalOffset;
                    }
                    else
                    {
                        int previousScrollingSlot = this.GetPreviousVisibleSlot(this.DisplayData.FirstScrollingSlot);
                        if (previousScrollingSlot >= 0)
                        {
                            ScrollSlotIntoView(previousScrollingSlot, false /*scrolledHorizontally*/);
                        }
                        return;
                    }
                }
                else
                {
                    this.DisplayData.PendingVerticalScrollHeight = this._vScrollBar.Value - this._verticalOffset;
                }

                if (!DoubleUtil.IsZero(this.DisplayData.PendingVerticalScrollHeight))
                {
                    // Invalidate so the scroll happens on idle
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);
                    InvalidateRowsArrange(); // force invalidate
                }
                // 
            }
            finally
            {
                this._verticalScrollChangesIgnored--;
            }
        }

        internal void RefreshRowsAndColumns(bool clearRows)
        {
            if (_measured)
            {
                try
                {
                    this._noCurrentCellChangeCount++;

                    if (clearRows)
                    {
                        ClearRows(false);
                        ClearRowGroupHeadersTable();
                        PopulateRowGroupHeadersTable();
                    }
                    if (this.AutoGenerateColumns)
                    {
                        //Column auto-generation refreshes the rows too
                        AutoGenerateColumnsPrivate();
                    }
                    foreach (DataGridColumn column in this.ColumnsItemsInternal)
                    {
                        //We don't need to refresh the state of AutoGenerated column headers because they're up-to-date
                        if (!column.IsAutoGenerated && column.HasHeaderCell)
                        {
                            column.HeaderCell.ApplyState(false);
                        }
                    }

                    RefreshRows(false /*recycleRows*/, false /*clearRows*/);

                    if (this.Columns.Count > 0 && this.CurrentColumnIndex == -1)
                    {
                        MakeFirstDisplayedCellCurrentCell();
                    }
                    else
                    {
                        this._makeFirstDisplayedCellCurrentCellPending = false;
                        this._desiredCurrentColumnIndex = -1;
                        FlushCurrentCellChanged();
                    }
                }
                finally
                {
                    this.NoCurrentCellChangeCount--;
                }
            }
            else
            {
                if (clearRows)
                {
                    ClearRows(false /*recycle*/);
                }
                ClearRowGroupHeadersTable();
                PopulateRowGroupHeadersTable();
            }
        }

        internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
        {
            Debug.Assert(columnIndex >= 0 && columnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= -1 && this.DisplayData.FirstDisplayedScrollingCol < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.DisplayData.LastTotallyDisplayedScrollingCol >= -1 && this.DisplayData.LastTotallyDisplayedScrollingCol < this.ColumnsItemsInternal.Count);
            Debug.Assert(!IsSlotOutOfBounds(slot));
            Debug.Assert(this.DisplayData.FirstScrollingSlot >= -1 && this.DisplayData.FirstScrollingSlot < this.SlotCount);
            Debug.Assert(this.ColumnsItemsInternal[columnIndex].IsVisible);

            if (this.CurrentColumnIndex >= 0 &&
                (this.CurrentColumnIndex != columnIndex || this.CurrentSlot != slot))
            {
                if (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) || IsInnerCellOutOfBounds(columnIndex, slot))
                {
                    return false;
                }
            }

            double oldHorizontalOffset = this.HorizontalOffset;

            //scroll horizontally unless we're on a RowGroupHeader and we're not forcing horizontal scrolling
            if ((forceHorizontalScroll || (slot != -1 && !this.RowGroupHeadersTable.Contains(slot)))
                && !ScrollColumnIntoView(columnIndex))
            {
                return false;
            }

            //scroll vertically
            if (!ScrollSlotIntoView(slot, oldHorizontalOffset != this.HorizontalOffset /*scrolledHorizontally*/))
            {
                return false;
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }
            return true;
        }

        // Convenient overload that commits the current edit.
        internal bool SetCurrentCellCore(int columnIndex, int slot)
        {
            return SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, true /*endRowEdit*/);
        }

        internal void UpdateHorizontalOffset(double newValue)
        {
            if (this.HorizontalOffset != newValue)
            {
                this.HorizontalOffset = newValue;

                InvalidateColumnHeadersMeasure();
                InvalidateColumnHeadersArrange();    // force invalidate

                InvalidateRowsMeasure(true);
                InvalidateRowsArrange();    // force invalidate
            }
        }

        internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
        {
            this._successfullyUpdatedSelection = false;
            
            this._noSelectionChangeCount++;
            this._noCurrentCellChangeCount++;
            try
            {
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                    columnIndex == this.ColumnsInternal.RowGroupSpacerColumn.Index)
                {
                    columnIndex = -1;
                }
                if (IsSlotOutOfSelectionBounds(slot) || (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)))
                {
                    return false;
                }

                int newCurrentPosition = -1;
                object item = ItemFromSlot(slot, ref newCurrentPosition);

                if (this.EditingRow != null && slot != this.EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, true))
                {
                    return false;
                }

                if (this.DataConnection.CollectionView != null &&
                    this.DataConnection.CollectionView.CurrentPosition != newCurrentPosition)
                {
                    this.DataConnection.MoveCurrentTo(item, slot, columnIndex, action, scrollIntoView);
                }
                else
                {
                    this.ProcessSelectionAndCurrency(columnIndex, item, slot, action, scrollIntoView);
                }
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }

            return this._successfullyUpdatedSelection;
        }

        internal void UpdateStateOnCurrentChanged(object currentItem, int currentPosition)
        {
            if (currentItem == this.CurrentItem && currentItem == this.SelectedItem && currentPosition == this.SelectedIndex)
            {
                // The DataGrid's CurrentItem is already up-to-date, so we don't need to do anything
                return;
            }

            int columnIndex = this.CurrentColumnIndex;
            if (columnIndex == -1)
            {
                if (this.IsColumnOutOfBounds(this._desiredCurrentColumnIndex) ||
                    (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && this._desiredCurrentColumnIndex == this.ColumnsInternal.RowGroupSpacerColumn.Index))
                {
                    columnIndex = this.FirstDisplayedNonFillerColumnIndex;
                }
                else
                {
                    columnIndex = this._desiredCurrentColumnIndex;
                }
            }
            this._desiredCurrentColumnIndex = -1;

            try
            {
                this._noSelectionChangeCount++;
                this._noCurrentCellChangeCount++;

                if (!this.CommitEdit())
                {
                    this.CancelEdit(DataGridEditingUnit.Row, false);
                }

                this.ClearRowSelection(true);
                if (currentItem == null)
                {
                    SetCurrentCellCore(-1, -1);
                }
                else
                {
                    int slot = SlotFromRowIndex(currentPosition);
                    this.ProcessSelectionAndCurrency(columnIndex, currentItem, slot, DataGridSelectionAction.SelectCurrent, false);
                }
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        internal bool UpdateStateOnMouseLeftButtonDown(MouseButtonEventArgs mouseButtonEventArgs, int columnIndex, int slot, bool allowEdit)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.UpdateStateOnMouseLeftButtonDown(mouseButtonEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
        }

        internal void UpdateVerticalScrollBar()
        {
            if (this._vScrollBar != null && this._vScrollBar.Visibility == Visibility.Visible)
            {
                double cellsHeight = this.CellsHeight;
                double edgedRowsHeightCalculated = this.EdgedRowsHeightCalculated;
                UpdateVerticalScrollBar(
                    edgedRowsHeightCalculated > cellsHeight /*needVertScrollbar*/,
                    this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible /*forceVertScrollbar*/,
                    edgedRowsHeightCalculated,
                    cellsHeight);
            }
        }

        /// <summary>
        /// If the editing element has focus, this method will set focus to the DataGrid itself
        /// in order to force the element to lose focus.  It will then wait for the editing element's
        /// LostFocus event, at which point it will perform the specified action.
        /// 
        /// NOTE: It is important to understand that the specified action will be performed when the editing
        /// element loses focus only if this method returns true.  If it returns false, then the action
        /// will not be performed later on, and should instead be performed by the caller, if necessary.
        /// </summary>
        /// <param name="action">Action to perform after the editing element loses focus</param>
        /// <returns>True if the editing element had focus and the action was cached away; false otherwise</returns>
        internal bool WaitForLostFocus(Action action)
        {
            if (this.EditingRow != null && this.EditingColumnIndex != -1 && !_executingLostFocusActions)
            {
                DataGridColumn editingColumn = this.ColumnsItemsInternal[this.EditingColumnIndex];
                FrameworkElement editingElement = editingColumn.GetCellContent(this.EditingRow);
                if (editingElement != null && editingElement.ContainsChild(_focusedObject))
                {
                    Debug.Assert(_lostFocusActions != null);
                    _lostFocusActions.Enqueue(action);
                    editingElement.LostFocus += new RoutedEventHandler(EditingElement_LostFocus);
                    this.IsTabStop = true;
                    this.Focus();
                    return true;
                }
            }
            return false;
        }

#endregion Internal Methods

#region Private Methods

        private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
        {
            DataGridCell newCell = new DataGridCell();
            PopulateCellContent(false /*isCellEdited*/, column, row, newCell);
            if (row.OwningGrid != null)
            {
                newCell.OwningColumn = column;
                newCell.Visibility = column.Visibility;
            }
            newCell.EnsureStyle(null);
            row.Cells.Insert(column.Index, newCell);
        }

        private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot);
            Debug.Assert(!GetColumnEffectiveReadOnlyState(this.CurrentColumn));
            Debug.Assert(this.CurrentColumn.IsVisible);

            if (this._editingColumnIndex != -1)
            {
                // Current cell is already in edit mode
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
                return true;
            }

            // Get or generate the editing row if it doesn't exist
            DataGridRow dataGridRow = this.EditingRow;
            if (dataGridRow == null)
            {
                Debug.Assert(!this.RowGroupHeadersTable.Contains(this.CurrentSlot));
                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    dataGridRow = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                    Debug.Assert(dataGridRow != null);
                }
                else
                {
                    dataGridRow = GenerateRow(RowIndexFromSlot(this.CurrentSlot), this.CurrentSlot);
                }
            }
            Debug.Assert(dataGridRow != null);

            // Cache these to see if they change later
            int currentRowIndex = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // Raise the BeginningEdit event
            DataGridCell dataGridCell = dataGridRow.Cells[this.CurrentColumnIndex];
            DataGridBeginningEditEventArgs e = new DataGridBeginningEditEventArgs(this.CurrentColumn, dataGridRow, editingEventArgs);
            OnBeginningEdit(e);
            if (e.Cancel
                || currentRowIndex != this.CurrentSlot
                || currentColumnIndex != this.CurrentColumnIndex
                || !GetRowSelection(this.CurrentSlot)
                || (this.EditingRow == null && !BeginRowEdit(dataGridRow)))
            {
                // If either BeginningEdit was canceled, currency/selection was changed in the event handler,
                // or we failed opening the row for edit, then we can no longer continue BeginCellEdit
                return false;
            }
            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this.EditingRow.Slot == this.CurrentSlot);

            // Finally, we can prepare the cell for editing
            this._editingColumnIndex = this.CurrentColumnIndex;
            this._editingEventArgs = editingEventArgs;
            this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
            PopulateCellContent(true /*isCellEdited*/, this.CurrentColumn, dataGridRow, dataGridCell);
            return true;
        }

        private bool BeginRowEdit(DataGridRow dataGridRow)
        {
            Debug.Assert(this.EditingRow == null);
            Debug.Assert(dataGridRow != null);

            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);

            if (this.DataConnection.BeginEdit(dataGridRow.DataContext))
            {
                this.EditingRow = dataGridRow;
                this.GenerateEditingElements();
                this.ValidateEditingRow(false /*scrollIntoView*/, true /*wireEvents*/);

                // Raise the automation invoke event for the row that just began edit
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                {
                    peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, dataGridRow);
                }
                return true;
             }
            return false;
        }

        private bool CancelRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return true;
            }
            Debug.Assert(this.EditingRow != null && this.EditingRow.Index >= -1);
            Debug.Assert(this.EditingRow.Slot < this.SlotCount);
            Debug.Assert(this.CurrentColumn != null);

            object dataItem = this.EditingRow.DataContext;
            if (!this.DataConnection.CancelEdit(dataItem))
            {
                return false;
            }
            foreach (DataGridColumn column in this.Columns)
            {
                if (!exitEditingMode && column.Index == this._editingColumnIndex && column is DataGridBoundColumn)
                {
                    continue;
                }
                PopulateCellContent(!exitEditingMode && column.Index == this._editingColumnIndex /*isCellEdited*/, column, this.EditingRow, this.EditingRow.Cells[column.Index]);
            }
            return true;
        }

        private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
        {
            if (forCurrentCellChange)
            {
                if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/))
                {
                    return false;
                }
                if (this.CurrentSlot != slot &&
                    !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                {
                    return false;
                }
            }

            if (IsColumnOutOfBounds(columnIndex))
            {
                return false;
            }
            if (slot >= this.SlotCount)
            {
                // Current cell was reset because the commit deleted row(s).
                // Since the user wants to change the current cell, we don't
                // want to end up with no current cell. We pick the last row 
                // in the grid which may be the 'new row'.
                int lastSlot = this.LastVisibleSlot;
                if (forCurrentCellChange &&
                    this.CurrentColumnIndex == -1 &&
                    lastSlot != -1)
                {
                    SetAndSelectCurrentCell(columnIndex, lastSlot, false /*forceCurrentCellSelection (unused here)*/);
                }
                // Interrupt operation because it has become invalid.
                return false;
            }
            return true;
        }

        private bool CommitRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return true;
            }
            Debug.Assert(this.EditingRow != null && this.EditingRow.Index >= -1);
            Debug.Assert(this.EditingRow.Slot < this.SlotCount);

            if (!ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/))
            {
                return false;
            }

            this.DataConnection.EndEdit(this.EditingRow.DataContext);

            if (!exitEditingMode)
            {
                this.DataConnection.BeginEdit(this.EditingRow.DataContext);
            }
            return true;
        }

        private void CompleteCellsCollection(DataGridRow dataGridRow)
        {
            Debug.Assert(dataGridRow != null);
            int cellsInCollection = dataGridRow.Cells.Count;
            if (this.ColumnsItemsInternal.Count > cellsInCollection)
            {
                for (int columnIndex = cellsInCollection; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
                {
                    AddNewCellPrivate(dataGridRow, this.ColumnsItemsInternal[columnIndex]);
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ComputeScrollBarsLayout()
        {
            if (this._ignoreNextScrollBarsLayout)
            {
                this._ignoreNextScrollBarsLayout = false;
                // 


            }
            double cellsWidth = this.CellsWidth;
            double cellsHeight = this.CellsHeight;

            bool allowHorizScrollbar = false;
            bool forceHorizScrollbar = false;
            double horizScrollBarHeight = 0;
            if (_hScrollBar != null)
            {
                forceHorizScrollbar = this.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowHorizScrollbar = forceHorizScrollbar || (this.ColumnsInternal.VisibleColumnCount > 0 &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);
                // Compensate if the horizontal scrollbar is already taking up space
                if (!forceHorizScrollbar && _hScrollBar.Visibility == Visibility.Visible)
                {
                    cellsHeight += this._hScrollBar.DesiredSize.Height;
                }
                horizScrollBarHeight = _hScrollBar.Height + _hScrollBar.Margin.Top + _hScrollBar.Margin.Bottom;
            }
            bool allowVertScrollbar = false;
            bool forceVertScrollbar = false;
            double vertScrollBarWidth = 0;
            if (_vScrollBar != null)
            {
                forceVertScrollbar = this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowVertScrollbar = forceVertScrollbar || (this.ColumnsItemsInternal.Count > 0 &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);
                // Compensate if the vertical scrollbar is already taking up space
                if (!forceVertScrollbar && _vScrollBar.Visibility == Visibility.Visible)
                {
                    cellsWidth += _vScrollBar.DesiredSize.Width;
                }
                vertScrollBarWidth = _vScrollBar.Width + _vScrollBar.Margin.Left + _vScrollBar.Margin.Right;
            }

            // Now cellsWidth is the width potentially available for displaying data cells.
            // Now cellsHeight is the height potentially available for displaying data cells.

            bool needHorizScrollbar = false;
            bool needVertScrollbar = false;

            double totalVisibleWidth = this.ColumnsInternal.VisibleEdgedColumnsWidth;
            double totalVisibleFrozenWidth = this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

            UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
            double totalVisibleHeight = this.EdgedRowsHeightCalculated;

            if (!forceHorizScrollbar && !forceVertScrollbar)
            {
                bool needHorizScrollbarWithoutVertScrollbar = false;

                if (allowHorizScrollbar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    double oldDataHeight = cellsHeight;
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needHorizScrollbarWithoutVertScrollbar = needHorizScrollbar = true;
                    if (allowVertScrollbar && (DoubleUtil.LessThanOrClose(totalVisibleWidth - cellsWidth, vertScrollBarWidth) ||
                        DoubleUtil.LessThanOrClose(cellsWidth - totalVisibleFrozenWidth, vertScrollBarWidth)))
                    {
                        // Would we still need a horizontal scrollbar without the vertical one?
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                        if (this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                        {
                            needHorizScrollbar = DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth - vertScrollBarWidth);
                        }
                    }

                    if (!needHorizScrollbar)
                    {
                        // Restore old data height because turns out a horizontal scroll bar wouldn't make sense
                        cellsHeight = oldDataHeight;
                    }
                }

                UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                if (allowVertScrollbar &&
                    DoubleUtil.GreaterThan(cellsHeight, 0) &&
                    DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                    this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // we compute the number of visible columns only after we set up the vertical scroll bar.
                ComputeDisplayedColumns();

                if (allowHorizScrollbar &&
                    needVertScrollbar && !needHorizScrollbar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    cellsWidth += vertScrollBarWidth;
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needVertScrollbar = false;

                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                    if (cellsHeight > 0 &&
                        vertScrollBarWidth <= cellsWidth &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        Debug.Assert(cellsWidth >= 0);
                        needVertScrollbar = true;
                    }
                    if (needVertScrollbar)
                    {
                        needHorizScrollbar = true;
                    }
                    else
                    {
                        needHorizScrollbar = needHorizScrollbarWithoutVertScrollbar;
                    }
                }
            }
            else if (forceHorizScrollbar && !forceVertScrollbar)
            {
                if (allowVertScrollbar)
                {
                    if (cellsHeight > 0 &&
                        DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        Debug.Assert(cellsWidth >= 0);
                        needVertScrollbar = true;
                    }
                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }
                needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }
            else if (!forceHorizScrollbar && forceVertScrollbar)
            {
                if (allowHorizScrollbar)
                {
                    if (cellsWidth > 0 &&
                        DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight) &&
                        DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                        DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth))
                    {
                        cellsHeight -= horizScrollBarHeight;
                        Debug.Assert(cellsHeight >= 0);
                        needHorizScrollbar = true;
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                    }
                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }
                needVertScrollbar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
            }
            else
            {
                Debug.Assert(forceHorizScrollbar && forceVertScrollbar);
                Debug.Assert(allowHorizScrollbar && allowVertScrollbar);
                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
                needVertScrollbar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
                needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }

            UpdateHorizontalScrollBar(needHorizScrollbar, forceHorizScrollbar, totalVisibleWidth, totalVisibleFrozenWidth, cellsWidth);
            UpdateVerticalScrollBar(needVertScrollbar, forceVertScrollbar, totalVisibleHeight, cellsHeight);

            if (this._topRightCornerHeader != null)
            {
                // Show the TopRightHeaderCell based on vertical ScrollBar visibility
                if (this.AreColumnHeadersVisible &&
                    this._vScrollBar != null && this._vScrollBar.Visibility == Visibility.Visible)
                {
                    this._topRightCornerHeader.Visibility = Visibility.Visible;
                }
                else
                {
                    this._topRightCornerHeader.Visibility = Visibility.Collapsed;
                }
            }
            this.DisplayData.FullyRecycleElements();
        }

        /// <summary>
        /// Create an ValidationSummaryItem for a given ValidationResult, by finding all cells related to the
        /// validation error and adding them as separate ValidationSummaryItemSources.
        /// </summary>
        /// <param name="validationResult">ValidationResult</param>
        /// <returns>ValidationSummaryItem</returns>
        private ValidationSummaryItem CreateValidationSummaryItem(ValidationResult validationResult)
        {
            Debug.Assert(validationResult != null);
            Debug.Assert(this._validationSummary != null);
            Debug.Assert(this.EditingRow != null);

            ValidationSummaryItem validationSummaryItem = new ValidationSummaryItem(validationResult.ErrorMessage);
            validationSummaryItem.Context = validationResult;

            string messageHeader = null;
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
            {
                foreach (string property in validationResult.MemberNames)
                {
                    if (!string.IsNullOrEmpty(property) && column.BindingPaths.Contains(property))
                    {
                        validationSummaryItem.Sources.Add(new ValidationSummaryItemSource(property, this.EditingRow.Cells[column.Index]));
                        if (string.IsNullOrEmpty(messageHeader) && column.Header != null)
                        {
                            messageHeader = column.Header.ToString();
                        }
                    }
                }
            }

            Debug.Assert(validationSummaryItem.ItemType == ValidationSummaryItemType.ObjectError);
            if (this._propertyValidationResults.ContainsEqualValidationResult(validationResult))
            {
                validationSummaryItem.MessageHeader = messageHeader;
                validationSummaryItem.ItemType = ValidationSummaryItemType.PropertyError;
            }

            return validationSummaryItem;
        }

        /// <summary>
        /// Handles the current editing element's LostFocus event by performing any actions that
        /// were cached by the WaitForLostFocus method.
        /// </summary>
        /// <param name="sender">Editing element</param>
        /// <param name="e">RoutedEventArgs</param>
        private void EditingElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement editingElement = sender as FrameworkElement;
            if (editingElement != null)
            {
                editingElement.LostFocus -= new RoutedEventHandler(EditingElement_LostFocus);
                if (this.EditingRow != null && this.EditingColumnIndex != -1)
                {
                    this.FocusEditingCell(true);
                }
                Debug.Assert(_lostFocusActions != null);
                try
                {
                    _executingLostFocusActions = true;
                    while (_lostFocusActions.Count > 0)
                    {
                        _lostFocusActions.Dequeue()();
                    }
                }
                finally
                {
                    _executingLostFocusActions = false;
                }
            }
        }

        // Makes sure horizontal layout is updated to reflect any changes that affect it
        private void EnsureHorizontalLayout()
        {
            this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            InvalidateColumnHeadersMeasure();

            // Invalidate Column Header when edit text or resize column width
            InvalidateColumnHeadersArrange(); // force invalidate

            InvalidateRowsMeasure(true);
            InvalidateRowsArrange();    // force invalidate

            InvalidateMeasure();

            InvalidateCellsArrange();    // force invalidate
        }

        private void EnsureRowHeaderWidth()
        {
            if (this.AreRowHeadersVisible)
            {
                if (this.AreColumnHeadersVisible)
                {
                    EnsureTopLeftCornerHeader();
                }

                if (_rowsPresenter != null)
                {

                    bool updated = false;

                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        DataGridRow row = element as DataGridRow;
                        if (row != null)
                        {
                            // If the RowHeader resulted in a different width the last time it was measured, we need
                            // to re-measure it
                            if (row.HeaderCell != null && row.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                row.HeaderCell.InvalidateMeasure();
                                updated = true;
                            }
                        }
                        else
                        {
                            DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                            if (groupHeader != null && groupHeader.HeaderCell != null && groupHeader.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                groupHeader.HeaderCell.InvalidateMeasure();
                                updated = true;
                            }
                        }
                    }

                    if (updated)
                    {
                        // We need to update the width of the horizontal scrollbar if the rowHeaders' width actually changed
                        InvalidateMeasure();
                    }
                }
            }
        }

        private void EnsureRowsPresenterVisibility()
        {
            if (_rowsPresenter != null)
            {
                // RowCount doesn't need to be considered, doing so might cause extra Visibility changes
                _rowsPresenter.Visibility = this.ColumnsInternal.FirstVisibleNonFillerColumn == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void EnsureTopLeftCornerHeader()
        {
            if (_topLeftCornerHeader != null)
            {
                _topLeftCornerHeader.Visibility = this.HeadersVisibility == DataGridHeadersVisibility.All ? Visibility.Visible : Visibility.Collapsed;

                if (_topLeftCornerHeader.Visibility == Visibility.Visible)
                {
                    if (!double.IsNaN(this.RowHeaderWidth))
                    {
                        // RowHeaderWidth is set explicitly so we should use that
                        _topLeftCornerHeader.Width = this.RowHeaderWidth;
                    }
                    else if (this.VisibleSlotCount > 0)
                    {
                        // RowHeaders AutoSize and we have at least 1 row so take the desired width
                        _topLeftCornerHeader.Width = this.RowHeadersDesiredWidth;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the ValidationSummary's FocusingInvalidControl event and begins edit on the cells
        /// that are associated with the selected error.
        /// </summary>
        /// <param name="sender">ValidationSummary</param>
        /// <param name="e">FocusingInvalidControlEventArgs</param>
        private void ValidationSummary_FocusingInvalidControl(object sender, FocusingInvalidControlEventArgs e)
        {
            Debug.Assert(this._validationSummary != null);
            if (this.EditingRow == null || !ScrollSlotIntoView(this.EditingRow.Slot, false /*scrolledHorizontally*/))
            {
                return;
            }

            // We need to focus the DataGrid in case the focused element gets removed when we end edit.
            if ((this._editingColumnIndex == -1 || (Focus() && EndCellEdit(DataGridEditAction.Commit, true, true, true)))
                && e.Item != null && e.Target != null && this._validationSummary.Errors.Contains(e.Item))
            {
                DataGridCell cell = e.Target.Control as DataGridCell;
                if (cell != null && cell.OwningGrid == this && cell.OwningColumn != null && cell.OwningColumn.IsVisible)
                {
                    Debug.Assert(cell.ColumnIndex >= 0 && cell.ColumnIndex < this.ColumnsInternal.Count);

                    // Begin editing the next relevant cell
                    UpdateSelectionAndCurrency(cell.ColumnIndex, this.EditingRow.Slot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    if (this._successfullyUpdatedSelection)
                    {
                        BeginCellEdit(new RoutedEventArgs());
                        if (!IsColumnDisplayed(this.CurrentColumnIndex))
                        {
                            ScrollColumnIntoView(this.CurrentColumnIndex);
                        }
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the ValidationSummary's SelectionChanged event and changes which cells are displayed as invalid.
        /// </summary>
        /// <param name="sender">ValidationSummary</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void ValidationSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ValidationSummary only supports single-selection mode.
            if (e.AddedItems.Count == 1)
            {
                this._selectedValidationSummaryItem = e.AddedItems[0] as ValidationSummaryItem;
            }

            this.UpdateValidationStatus();
        }

        // Recursively expands parent RowGroupHeaders from the top down
        private void ExpandRowGroupParentChain(int level, int slot)
        {
            if (level < 0)
            {
                return;
            }
            int previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(slot + 1);
            DataGridRowGroupInfo rowGroupInfo = null;
            while (previousHeaderSlot >= 0)
            {
                rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(previousHeaderSlot);
                Debug.Assert(rowGroupInfo != null);
                if (level == rowGroupInfo.Level)
                {
                    if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    {
                        // Keep going up the chain
                        ExpandRowGroupParentChain(level - 1, rowGroupInfo.Slot - 1);
                    }
                    if (rowGroupInfo.Visibility != Visibility.Visible)
                    {
                        EnsureRowGroupVisibility(rowGroupInfo, Visibility.Visible, false);
                    }
                    return;
                }
                else
                {
                    previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(previousHeaderSlot);
                }
            }
        }

        /// <summary>
        /// Searches through the DataGrid's ValidationSummary for any errors that use the given
        /// ValidationResult as the ValidationSummaryItem's Context value.
        /// </summary>
        /// <param name="context">ValidationResult</param>
        /// <returns>ValidationSummaryItem or null if not found</returns>
        private ValidationSummaryItem FindValidationSummaryItem(ValidationResult context)
        {
            Debug.Assert(context != null);
            Debug.Assert(this._validationSummary != null);
            foreach (ValidationSummaryItem ValidationSummaryItem in this._validationSummary.Errors)
            {
                if (context.Equals(ValidationSummaryItem.Context))
                {
                    return ValidationSummaryItem;
                }
            }
            return null;
        }

        private void InvalidateCellsArrange()
        {
            foreach (DataGridRow row in GetAllRows())
            {
                row.InvalidateHorizontalArrange();
            }
        }
        
        private void InvalidateColumnHeadersArrange()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.InvalidateArrange();
            }
        }

        private void InvalidateColumnHeadersMeasure()
        {
            if (_columnHeadersPresenter != null)
            {
                EnsureColumnHeadersVisibility();
                _columnHeadersPresenter.InvalidateMeasure();
            }
        }
        
        private void InvalidateRowsArrange()
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateArrange();
            }
        }

        private void InvalidateRowsMeasure(bool invalidateIndividualElements)
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateMeasure();

                if (invalidateIndividualElements)
                {
                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        element.InvalidateMeasure();
                    }
                }
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!this.ContainsFocus)
            {
                this.ContainsFocus = true;
                ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                {
                    DataGridRow row = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                    if (row != null)
                    {
                        row.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
                    }
                }
            }

            // Keep track of which row contains the newly focused element
            DataGridRow focusedRow = null;
            DependencyObject focusedElement = e.OriginalSource as DependencyObject;
            _focusedObject = focusedElement;
            while (focusedElement != null)
            {
                focusedRow = focusedElement as DataGridRow;
                if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
                {
                    ResetFocusedRow();
                    _focusedRow = focusedRow.Visibility == Visibility.Visible ? focusedRow : null;
                    break;
                }
                focusedElement = VisualTreeHelper.GetParent(focusedElement);
            }

            // If the DataGrid itself got focus, we actually want the automation focus to be on the current element
            if (e.OriginalSource == this && AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationFocusChangedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }
        }

        private void DataGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDisabledVisual();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessDataGridKey(e);
            }
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && this.CurrentColumnIndex != -1 && e.OriginalSource == this)
            {
                bool success = ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                Debug.Assert(success);
                if (this.CurrentColumnIndex != -1 && this.SelectedItem == null)
                {
                    SetRowSelection(this.CurrentSlot, true /*isSelected*/, true /*setAnchorSlot*/);
                }
            }
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            _focusedObject = null;
            if (this.ContainsFocus)
            {
                bool focusLeftDataGrid = true;
                bool dataGridWillReceiveRoutedEvent = true;
                object focusedObject = FocusManager.GetFocusedElement();
                DependencyObject focusedDependencyObject = focusedObject as DependencyObject;

                while (focusedDependencyObject != null)
                {
                    if (focusedDependencyObject == this)
                    {
                        focusLeftDataGrid = false;
                        break;
                    }

                    // Walk up the visual tree.  If we hit the root, try using the framework element's
                    // parent.  We do this because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = VisualTreeHelper.GetParent(focusedDependencyObject);
                    if (parent == null)
                    {
                        FrameworkElement element = focusedDependencyObject as FrameworkElement;
                        if (element != null)
                        {
                            parent = element.Parent;
                            if (parent != null)
                            {
                                dataGridWillReceiveRoutedEvent = false;
                            }
                        }
                    }
                    focusedDependencyObject = parent;
                }

                if (focusLeftDataGrid)
                {
                    this.ContainsFocus = false;
                    if (this.EditingRow != null)
                    {
                        CommitEdit(DataGridEditingUnit.Row, true /*exitEditingMode*/);
                    }
                    ResetFocusedRow();
                    ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                    if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                    {
                        DataGridRow row = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                        if (row != null)
                        {
                            row.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
                        }
                    }
                }
                else if (!dataGridWillReceiveRoutedEvent)
                {
                    FrameworkElement focusedElement = focusedObject as FrameworkElement;
                    if (focusedElement != null)
                    {
                        focusedElement.LostFocus += new RoutedEventHandler(ExternalEditingElement_LostFocus);
                    }
                }
            }
        }

        private void EditingElement_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added && e.Error.Exception != null && e.Error.ErrorContent != null)
            {
                ValidationResult validationResult = new ValidationResult(e.Error.ErrorContent.ToString(), new List<string>() { this._updateSourcePath });
                this._bindingValidationResults.AddIfNew(validationResult);
            }
        }

        private void EditingElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= new RoutedEventHandler(EditingElement_Loaded);
            }
            PreparingCellForEditPrivate(element);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
        {
            if (this._editingColumnIndex == -1)
            {
                return true;
            }
            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this._editingColumnIndex >= 0);
            Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

            // Cache these to see if they change later
            int currentSlot = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // We're ready to start ending, so raise the event
            DataGridCell editingCell = this.EditingRow.Cells[this._editingColumnIndex];
            FrameworkElement editingElement = editingCell.Content as FrameworkElement;
            if (editingElement == null)
            {
                return false;
            }
            if (raiseEvents)
            {
                DataGridCellEditEndingEventArgs e = new DataGridCellEditEndingEventArgs(this.CurrentColumn, this.EditingRow, editingElement, editAction);
                OnCellEditEnding(e);
                if (e.Cancel)
                {
                    // CellEditEnding has been cancelled
                    return false;
                }

                // Ensure that the current cell wasn't changed in the user's CellEditEnding handler
                if (this._editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }
                Debug.Assert(this.EditingRow != null);
                Debug.Assert(this.EditingRow.Slot == currentSlot);
                Debug.Assert(this._editingColumnIndex != -1);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            }

            this._bindingValidationResults.Clear();

            // If we're canceling, let the editing column repopulate its old value if it wants
            if (editAction == DataGridEditAction.Cancel)
            {
                this.CurrentColumn.CancelCellEditInternal(editingElement, this._uneditedValue);

                // Ensure that the current cell wasn't changed in the user column's CancelCellEdit
                if (this._editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }
                Debug.Assert(this.EditingRow != null);
                Debug.Assert(this.EditingRow.Slot == currentSlot);
                Debug.Assert(this._editingColumnIndex != -1);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);

                // Re-validate
                this.ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/);
            }

            // If we're committing, explicitly update the source but watch out for any validation errors
            if (editAction == DataGridEditAction.Commit)
            {
                foreach (BindingInfo bindingData in this.CurrentColumn.GetInputBindings(editingElement, this.CurrentItem))
                {
                    Debug.Assert(bindingData.BindingExpression.ParentBinding != null);
                    this._updateSourcePath = bindingData.BindingExpression.ParentBinding.Path != null ? bindingData.BindingExpression.ParentBinding.Path.Path : null;
                    bindingData.Element.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
                    bindingData.BindingExpression.UpdateSource();
                    bindingData.Element.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
                }

                // Re-validate
                this.ValidateEditingRow(true /*scrollIntoView*/, false /*wireEvents*/);

                if (this._bindingValidationResults.Count > 0)
                {
                    ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                    return false;
                }
            }

            if (exitEditingMode)
            {
                this._editingColumnIndex = -1;
                editingCell.ApplyCellState(true /*animate*/);

                //
                this.IsTabStop = true;
                if (keepFocus && editingElement.ContainsFocusedElement())
                {
                    this.Focus();
                }

                PopulateCellContent(!exitEditingMode /*isCellEdited*/, this.CurrentColumn, this.EditingRow, editingCell);
            }

            // We're done, so raise the CellEditEnded event
            if (raiseEvents)
            {
                OnCellEditEnded(new DataGridCellEditEndedEventArgs(this.CurrentColumn, this.EditingRow, editAction));
            }

            // There's a chance that somebody reopened this cell for edit within the CellEditEnded handler,
            // so we should return false if we were supposed to exit editing mode, but we didn't
            return !(exitEditingMode && currentColumnIndex == this._editingColumnIndex);
        }

        private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
        {
            if (this.EditingRow == null || this.DataConnection.CommittingEdit)
            {
                return true;
            }
            if (this._editingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents && 
                !((this.DataConnection.EditableCollectionView != null && this.DataConnection.EditableCollectionView.CanCancelEdit) || (this.EditingRow.DataContext is IEditableObject))))
            {
                // Ending the row edit will fail immediately under the following conditions:
                // 1. We haven't ended the cell edit yet.
                // 2. We're trying to cancel edit when the underlying DataType is not an IEditableObject,
                //    because we have no way to properly restore the old value.  We will only allow this to occur
                //    if raiseEvents == false, which means we're internally forcing a cancel.
                return false;
            }
            DataGridRow editingRow = this.EditingRow;

            if (raiseEvents)
            {
                DataGridRowEditEndingEventArgs e = new DataGridRowEditEndingEventArgs(this.EditingRow, editAction);
                OnRowEditEnding(e);
                if (e.Cancel)
                {
                    // RowEditEnding has been cancelled
                    return false;
                }

                // Editing states might have been changed in the RowEditEnding handlers
                if (this._editingColumnIndex != -1)
                {
                    return false;
                }
                if (editingRow != this.EditingRow)
                {
                    return true;
                }
            }

            // Call the appropriate commit or cancel methods
            if (editAction == DataGridEditAction.Commit)
            {
                if (!CommitRowEdit(exitEditingMode))
                {
                    return false;
                }
            }
            else
            {
                if (!CancelRowEdit(exitEditingMode) && raiseEvents)
                {
                    // We failed to cancel edit so we should abort unless we're forcing a cancel
                    return false;
                }
            }
            ResetValidationStatus();

            // Update the previously edited row's state
            if (exitEditingMode && editingRow == this.EditingRow)
            {
                // Unwire the INDEI event handlers
                foreach (INotifyDataErrorInfo indei in this._validationItems.Keys)
                {
                    indei.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
                }
                this._validationItems.Clear();
                this.RemoveEditingElements();
                ResetEditingRow();
            }

            // Raise the RowEditEnded event
            if (raiseEvents)
            {
                OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));
            }

            return true;
        }

        private void EnsureColumnHeadersVisibility()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Visibility = this.AreColumnHeadersVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Applies the given Style to the Row if it's supposed to use DataGrid.RowStyle
        private static void EnsureElementStyle(FrameworkElement element, Style oldDataGridStyle, Style newDataGridStyle)
        {
            Debug.Assert(element != null);

            // Apply the DataGrid style if the row was using the old DataGridRowStyle before
            if (element != null && (element.Style == null || element.Style == oldDataGridStyle))
            {
                element.SetStyleWithType(newDataGridStyle);
            }
        }

        private void EnsureVerticalGridLines()
        {
            if (this.AreColumnHeadersVisible)
            {
                double totalColumnsWidth = 0;
                foreach (DataGridColumn column in this.ColumnsInternal)
                {
                    totalColumnsWidth += column.ActualWidth;

                    column.HeaderCell.SeparatorVisibility = (column != this.ColumnsInternal.LastVisibleColumn || totalColumnsWidth < this.CellsWidth) ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }

            foreach (DataGridRow row in GetAllRows())
            {
                row.EnsureGridLines();
            }
        }

        /// <summary>
        /// Exits editing mode without trying to commit or revert the editing, and 
        /// without repopulating the edited row's cell.
        /// </summary>
        private void ExitEdit(bool keepFocus)
        {
            if (this.EditingRow == null || this.DataConnection.CommittingEdit)
            {
                Debug.Assert(this._editingColumnIndex == -1);
                return;
            }

            if (this._editingColumnIndex != -1)
            {
                Debug.Assert(this._editingColumnIndex >= 0);
                Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
                Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

                this._editingColumnIndex = -1;
                this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(false /*animate*/);
            }
            //
            this.IsTabStop = true;
            if (this.IsSlotVisible(this.EditingRow.Slot))
            {
                this.EditingRow.ApplyState(true /*animate*/);
            }
            ResetEditingRow();
            if (keepFocus)
            {
                bool success = Focus();
                Debug.Assert(success);
            }
        }

        private void ExternalEditingElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.LostFocus -= new RoutedEventHandler(ExternalEditingElement_LostFocus);
                DataGrid_LostFocus(sender, e);
            }
        }

        private void FlushCurrentCellChanged()
        {
            if (this._makeFirstDisplayedCellCurrentCellPending)
            {
                return;
            }
            if (this.SelectionHasChanged)
            {
                // selection is changing, don't raise CurrentCellChanged until it's done
                this._flushCurrentCellChanged = true;
                FlushSelectionChanged();
                return;
            }

            // We don't want to expand all intermediate currency positions, so we only expand
            // the last current item before we flush the event
            if (this._collapsedSlotsTable.Contains(this.CurrentSlot))
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(this.RowGroupHeadersTable.GetPreviousIndex(this.CurrentSlot));
                Debug.Assert(rowGroupInfo != null);
                if (rowGroupInfo != null)
                {
                    this.ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                }
            }

            if (this.CurrentColumn != this._previousCurrentColumn
                || this.CurrentItem != this._previousCurrentItem)
            {
                this.CoerceSelectedItem();
                this._previousCurrentColumn = this.CurrentColumn;
                this._previousCurrentItem = this.CurrentItem;

                OnCurrentCellChanged(EventArgs.Empty);
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && this.CurrentCellCoordinates != this._previousAutomationFocusCoordinates)
            {
                this._previousAutomationFocusCoordinates = new DataGridCellCoordinates(this.CurrentCellCoordinates);

                // If the DataGrid itself has focus, we want to move automation focus to the new current element
                if (FocusManager.GetFocusedElement() == this)
                {
                    if (AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
                    {
                        peer.RaiseAutomationFocusChangedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                    }
                }
            }

            this._flushCurrentCellChanged = false;
        }

        private void FlushSelectionChanged()
        {
            if (this.SelectionHasChanged && this._noSelectionChangeCount == 0 && !this._makeFirstDisplayedCellCurrentCellPending)
            {
                this.CoerceSelectedItem();
                if (this.NoCurrentCellChangeCount != 0)
                {
                    // current cell is changing, don't raise SelectionChanged until it's done
                    return;
                }
                this.SelectionHasChanged = false;

                if (this._flushCurrentCellChanged)
                {
                    FlushCurrentCellChanged();
                }

                SelectionChangedEventArgs e = this._selectedItems.GetSelectionChangedEventArgs();
                if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0)
                {
                    OnSelectionChanged(e);
                }
            }
        }

        private bool FocusEditingCell(bool setFocus)
        {
            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);
            Debug.Assert(this._editingColumnIndex != -1);

            //

            this.IsTabStop = false;
            this._focusEditingControl = false;

            bool success = false;
            DataGridCell dataGridCell = this.EditingRow.Cells[this._editingColumnIndex];
            if (setFocus)
            {
                success = dataGridCell.ContainsFocusedElement() ? true : dataGridCell.Focus();
                this._focusEditingControl = !success;
            }
            return success;
        }

        /// <summary>
        /// This method formats a row (specified by a DataGridRowClipboardEventArgs) into
        /// a single string to be added to the Clipboard when the DataGrid is copying its contents.
        /// </summary>
        /// <param name="e">DataGridRowClipboardEventArgs</param>
        /// <returns>The formatted string.</returns>
        private string FormatClipboardContent(DataGridRowClipboardEventArgs e)
        {
            StringBuilder text = new StringBuilder();
            for (int cellIndex = 0; cellIndex < e.ClipboardRowContent.Count; cellIndex++)
            {
                DataGridClipboardCellContent cellContent = e.ClipboardRowContent[cellIndex];
                if (cellContent != null)
                {
                    text.Append(cellContent.Content);
                }
                if (cellIndex < e.ClipboardRowContent.Count - 1)
                {
                    text.Append('\t');
                }
                else
                {
                    text.Append('\r');
                    text.Append('\n');
                }
            }
            return text.ToString();
        }

        // Calculates the amount to scroll for the ScrollLeft button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollDecrease()
        {
            // If the first column is covered up, scroll to the start of it when the user clicks the left button
            if (_negHorizontalOffset > 0)
            {
                return _negHorizontalOffset;
            }
            else
            {
                // The entire first column is displayed, show the entire previous column when the user clicks
                // the left button
                DataGridColumn previousColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(
                    this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
                if (previousColumn != null)
                {
                    return GetEdgedColumnWidth(previousColumn);
                }
                else
                {
                    // There's no previous column so don't move
                    return 0;
                }
            }
        }

        // Calculates the amount to scroll for the ScrollRight button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstDisplayedScrollingCol >= 0)
            {
                return GetEdgedColumnWidth(this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) - _negHorizontalOffset;
            }
            return 0;
        }

        // Calculates the amount the ScrollDown button should scroll
        // This is a method rather than a property to emphasize that calculations are taking place
        private double GetVerticalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstScrollingSlot >= 0)
            {
                return GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot) - this.NegVerticalOffset;
            }
            return 0;
        }

        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessHorizontalScroll(e.ScrollEventType);
        }    

        private bool IsColumnOutOfBounds(int columnIndex)
        {
            return columnIndex >= this.ColumnsItemsInternal.Count || columnIndex < 0;
        }

        private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
        {
            return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfBounds(slot);
        }

        private bool IsInnerCellOutOfSelectionBounds(int columnIndex, int slot)
        {
            return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfSelectionBounds(slot);
        }

        private bool IsSlotOutOfBounds(int slot)
        {
            return slot >= this.SlotCount || slot < -1 || _collapsedSlotsTable.Contains(slot);
        }

        private bool IsSlotOutOfSelectionBounds(int slot)
        {
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                Debug.Assert(slot >= 0 && slot < this.SlotCount);
                return false;
            }
            else
            {
                int rowIndex = RowIndexFromSlot(slot);
                return rowIndex < 0 || rowIndex >= this.DataConnection.Count;
            }
        }

        private void MakeFirstDisplayedCellCurrentCell()
        {
            if (this.CurrentColumnIndex != -1)
            {
                this._makeFirstDisplayedCellCurrentCellPending = false;
                this._desiredCurrentColumnIndex = -1;
                this.FlushCurrentCellChanged();
                return;
            }
            if (this.SlotCount != SlotFromRowIndex(this.DataConnection.Count))
            {
                this._makeFirstDisplayedCellCurrentCellPending = true;
                return;
            }

            // No current cell, therefore no selection either - try to set the current cell to the
            // ItemsSource's ICollectionView.CurrentItem if it exists, otherwise use the first displayed cell.
            int slot = 0;
            if (this.DataConnection.CollectionView != null)
            {
                if (this.DataConnection.CollectionView.IsCurrentBeforeFirst ||
                    this.DataConnection.CollectionView.IsCurrentAfterLast)
                {
                    slot = this.RowGroupHeadersTable.Contains(0) ? 0 : -1;
                }
                else
                {
                    slot = SlotFromRowIndex(this.DataConnection.CollectionView.CurrentPosition);
                }
            }
            else
            {
                if (this.SelectedIndex == -1)
                {
                    // Try to default to the first row
                    slot = SlotFromRowIndex(0);
                    if (!this.IsSlotVisible(slot))
                    {
                        slot = -1;
                    }
                }
                else
                {
                    slot = SlotFromRowIndex(this.SelectedIndex);
                }
            }
            int columnIndex = this.FirstDisplayedNonFillerColumnIndex;
            if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < this.ColumnsItemsInternal.Count)
            {
                columnIndex = _desiredCurrentColumnIndex;
            }

            SetAndSelectCurrentCell(columnIndex,
                                    slot,
                                    false /*forceCurrentCellSelection*/);
            this.AnchorSlot = slot;
            this._makeFirstDisplayedCellCurrentCellPending = false;
            this._desiredCurrentColumnIndex = -1;
            FlushCurrentCellChanged();
        }

        private void PopulateCellContent(bool isCellEdited,
                                         DataGridColumn dataGridColumn,
                                         DataGridRow dataGridRow,
                                         DataGridCell dataGridCell)
        {
            Debug.Assert(dataGridColumn != null);
            Debug.Assert(dataGridRow != null);
            Debug.Assert(dataGridCell != null);

            FrameworkElement element = null;
            DataGridBoundColumn dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
            if (isCellEdited)
            {
                // Generate EditingElement and apply column style if available
                element = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.EditingElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.EditingElementStyle);
                    }

                    // Subscribe to the new element's events
                    element.Loaded += new RoutedEventHandler(EditingElement_Loaded);
                }
            }
            else
            {
                // Generate Element and apply column style if available
                element = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.ElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.ElementStyle);
                    }
                }
            }

            dataGridCell.Content = element;
        }

        private void PreparingCellForEditPrivate(FrameworkElement editingElement)
        {
            if (this._editingColumnIndex == -1 ||
                this.CurrentColumnIndex == -1 ||
                this.EditingRow.Cells[this.CurrentColumnIndex].Content != editingElement)
            {
                // The current cell has changed since the call to BeginCellEdit, so the fact
                // that this element has loaded is no longer relevant
                return;
            }

            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this._editingColumnIndex >= 0);
            Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

            FocusEditingCell(this.ContainsFocus || this._focusEditingControl /*setFocus*/);

            // Prepare the cell for editing and raise the PreparingCellForEdit event for all columns
            DataGridColumn dataGridColumn = this.CurrentColumn;
            this._uneditedValue = dataGridColumn.PrepareCellForEditInternal(editingElement, this._editingEventArgs);
            OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(dataGridColumn, this.EditingRow, this._editingEventArgs, editingElement));
        }

        private bool ProcessAKey()
        {
            bool ctrl, shift, alt;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift, out alt);

            if (ctrl && !shift && !alt && this.SelectionMode == DataGridSelectionMode.Extended)
            {
                SelectAll();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the case where a 'Copy' key ('C' or 'Insert') has been pressed.  If pressed in combination with
        /// the control key, and the necessary prerequisites are met, the DataGrid will copy its contents
        /// to the Clipboard as text.
        /// </summary>
        /// <returns>Whether or not the DataGrid handled the key press.</returns>
        private bool ProcessCopyKey()
        {
            bool ctrl, shift, alt;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift, out alt);

            if (ctrl && !shift && !alt && this.ClipboardCopyMode != DataGridClipboardCopyMode.None && this.SelectedItems.Count > 0)
            {
                StringBuilder textBuilder = new StringBuilder();

                if (this.ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
                {
                    DataGridRowClipboardEventArgs headerArgs = new DataGridRowClipboardEventArgs(null, true);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        headerArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(null, column, column.Header));
                    }
                    this.OnCopyingRowClipboardContent(headerArgs);
                    textBuilder.Append(FormatClipboardContent(headerArgs));
                }

                for (int index = 0; index < this.SelectedItems.Count; index++)
                {
                    object item = this.SelectedItems[index];
                    DataGridRowClipboardEventArgs itemArgs = new DataGridRowClipboardEventArgs(item, false);
                    foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
                    {
                        object content = column.GetCellValue(item, column.ClipboardContentBinding);
                        itemArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, column, content));
                    }
                    this.OnCopyingRowClipboardContent(itemArgs);
                    textBuilder.Append(FormatClipboardContent(itemArgs));
                }

                string text = textBuilder.ToString();

                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch (SecurityException)
                    {
                        // We will get a SecurityException if the user does not allow access to the clipboard.
                    }
                    return true;
                }
            }
            return false;
        }

        private bool ProcessDataGridKey(KeyEventArgs e)
        {
            bool focusDataGrid = false;
            switch (e.Key)
            {
                case Key.Tab:
                    return ProcessTabKey(e);

                case Key.Up:
                    focusDataGrid = ProcessUpKey();
                    break;

                case Key.Down:
                    focusDataGrid = ProcessDownKey();
                    break;

                case Key.PageDown:
                    focusDataGrid = ProcessNextKey();
                    break;

                case Key.PageUp:
                    focusDataGrid = ProcessPriorKey();
                    break;

                case Key.Left:
                    focusDataGrid = this.FlowDirection == FlowDirection.LeftToRight ? ProcessLeftKey() : ProcessRightKey();
                    break;

                case Key.Right:
                    focusDataGrid = this.FlowDirection == FlowDirection.LeftToRight ? ProcessRightKey() : ProcessLeftKey();
                    break;

                case Key.F2:
                    return ProcessF2Key(e);

                case Key.Home:
                    focusDataGrid = ProcessHomeKey();
                    break;

                case Key.End:
                    focusDataGrid = ProcessEndKey();
                    break;

                case Key.Enter:
                    focusDataGrid = ProcessEnterKey();
                    break;

                case Key.Escape:
                    return ProcessEscapeKey();

                case Key.A:
                    return ProcessAKey();

                case Key.C:
                    return ProcessCopyKey();

                case Key.Insert:
                    return ProcessCopyKey();
            }
            if (focusDataGrid && this.IsTabStop)
            {
                this.Focus();
            }
            return focusDataGrid;
        }

        private bool ProcessDownKeyInternal(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int lastSlot = this.LastVisibleSlot;
            if (firstVisibleColumnIndex == -1 || lastSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessDownKeyInternal(shift, ctrl); }))
            {
                return true;
            }

            int nextSlot = -1;
            if (this.CurrentSlot != -1)
            {
                nextSlot = this.GetNextVisibleSlot(this.CurrentSlot);
                if (nextSlot >= this.SlotCount)
                {
                    nextSlot = -1;
                }
            }

            _noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = this.FirstVisibleSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        // Ctrl without Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (nextSlot == -1)
                    {
                        return true;
                    }
                    if (shift)
                    {
                        // Shift without Ctrl
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Ctrl nor Shift
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessEndKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            int lastVisibleSlot = this.LastVisibleSlot;
            if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessEndKey(shift, ctrl); }))
            {
                return true;
            }

            this._noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(lastVisibleColumnIndex, lastVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessEnterKey(bool shift, bool ctrl)
        {
            int oldCurrentSlot = this.CurrentSlot;

            if (!ctrl)
            {
                // If Enter was used by a TextBox, we shouldn't handle the key
                TextBox focusedTextBox = FocusManager.GetFocusedElement() as TextBox;
                if (focusedTextBox != null && focusedTextBox.AcceptsReturn)
                {
                    return false;
                }

                if (this.WaitForLostFocus(delegate { this.ProcessEnterKey(shift, ctrl); }))
                {
                    return true;
                }

                // Enter behaves like down arrow - it commits the potential editing and goes down one cell.
                if (!ProcessDownKeyInternal(false, ctrl))
                {
                    return false;
                }
            }
            else if (this.WaitForLostFocus(delegate { this.ProcessEnterKey(shift, ctrl); }))
            {
                return true;
            }

            // Try to commit the potential editing
            if (oldCurrentSlot == this.CurrentSlot && EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/) && this.EditingRow != null)
            {
                EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/);
                ScrollIntoView(this.CurrentItem, this.CurrentColumn);
            }

            return true;
        }

        private bool ProcessEscapeKey()
        {
            if (this.WaitForLostFocus(delegate { this.ProcessEscapeKey(); }))
            {
                return true;
            }

            if (this._editingColumnIndex != -1)
            {
                // Revert the potential cell editing and exit cell editing.
                EndCellEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/);
                return true;
            }
            else if (this.EditingRow != null)
            {
                // Revert the potential row editing and exit row editing.
                EndRowEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*raiseEvents*/);
                return true;
            }
            return false;
        }

        private bool ProcessF2Key(KeyEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (!shift && !ctrl &&
                this._editingColumnIndex == -1 && this.CurrentColumnIndex != -1 && GetRowSelection(this.CurrentSlot) &&
                !GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                if (ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/))
                {
                    BeginCellEdit(e);
                }
                return true;
            }

            return false;
        }

        private bool ProcessHomeKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessHomeKey(shift, ctrl); }))
            {
                return true;
            }

            this._noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessLeftKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessLeftKey(shift, ctrl); }))
            {
                return true;
            }

            int previousVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    previousVisibleColumnIndex = dataGridColumn.Index;
                }
            }

            this._noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    if (this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                    {
                        CollapseRowGroup(this.RowGroupHeadersTable.GetValueAt(this.CurrentSlot).CollectionViewGroup, false /*collapseAllSubgroups*/);
                    }
                    else if (this.CurrentColumnIndex == -1)
                    {
                        UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (previousVisibleColumnIndex == -1)
                        {
                            return true;
                        }
                        UpdateSelectionAndCurrency(previousVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        // Ctrl Left <==> Home
        private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
        {
            this._noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                    Debug.Assert(_selectedItems.Count == 0);
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(firstVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessNextKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessNextKey(shift, ctrl); }))
            {
                return true;
            }

            int nextPageSlot = this.CurrentSlot == -1 ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            Debug.Assert(nextPageSlot != -1);
            int slot = GetNextVisibleSlot(nextPageSlot);

            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            while (scrollCount > 0 && slot < this.SlotCount)
            {
                nextPageSlot = slot;
                scrollCount--;
                slot = GetNextVisibleSlot(slot);
            }

            this._noSelectionChangeCount++;
            try
            {
                DataGridSelectionAction action;
                int columnIndex;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? action = DataGridSelectionAction.SelectFromAnchorToCurrent
                        : action = DataGridSelectionAction.SelectCurrent;
                }
                UpdateSelectionAndCurrency(columnIndex, nextPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessPriorKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessPriorKey(shift, ctrl); }))
            {
                return true;
            }

            int previousPageSlot = (this.CurrentSlot == -1) ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            Debug.Assert(previousPageSlot != -1);

            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            int slot = GetPreviousVisibleSlot(previousPageSlot);
            while (scrollCount > 0 && slot != -1)
            {
                previousPageSlot = slot;
                scrollCount--;
                slot = GetPreviousVisibleSlot(slot);
            }
            Debug.Assert(previousPageSlot != -1);

            this._noSelectionChangeCount++;
            try
            {
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                UpdateSelectionAndCurrency(columnIndex, previousPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessRightKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessRightKey(shift, ctrl); }))
            {
                return true;
            }

            int nextVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    nextVisibleColumnIndex = dataGridColumn.Index;
                }
            }
            this._noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    if (this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                    {
                        ExpandRowGroup(this.RowGroupHeadersTable.GetValueAt(this.CurrentSlot).CollectionViewGroup, false /*expandAllSubgroups*/);
                    }
                    else if (this.CurrentColumnIndex == -1)
                    {
                        int firstVisibleColumnIndex = this.ColumnsInternal.FirstVisibleColumn == null ? -1 : this.ColumnsInternal.FirstVisibleColumn.Index;
                        UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (nextVisibleColumnIndex == -1)
                        {
                            return true;
                        }
                        UpdateSelectionAndCurrency(nextVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        // Ctrl Right <==> End
        private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
        {
            this._noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(lastVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessTabKey(KeyEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return this.ProcessTabKey(e, shift, ctrl);
        }

        private bool ProcessTabKey(KeyEventArgs e, bool shift, bool ctrl)
        {
            if (ctrl || this._editingColumnIndex == -1 || this.IsReadOnly)
            {
                //Go to the next/previous control on the page when 
                // - Ctrl key is used
                // - Potential current cell is not edited, or the datagrid is read-only. 
                return false;
            }

            // Try to locate a writable cell before/after the current cell
            Debug.Assert(this.CurrentColumnIndex != -1);
            Debug.Assert(this.CurrentSlot != -1);

            int neighborVisibleWritableColumnIndex, neighborSlot;
            DataGridColumn dataGridColumn;
            if (shift)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetPreviousVisibleSlot(this.CurrentSlot);
                if (this.EditingRow != null)
                {
                    while (neighborSlot != -1 && this.RowGroupHeadersTable.Contains(neighborSlot))
                    {
                        neighborSlot = GetPreviousVisibleSlot(neighborSlot);
                    }
                }
            }
            else
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetNextVisibleSlot(this.CurrentSlot);
                if (this.EditingRow != null)
                {
                    while (neighborSlot < this.SlotCount && this.RowGroupHeadersTable.Contains(neighborSlot))
                    {
                        neighborSlot = GetNextVisibleSlot(neighborSlot);
                    }
                }
            }
            neighborVisibleWritableColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;

            if (neighborVisibleWritableColumnIndex == -1 && (neighborSlot == -1 || neighborSlot >= this.SlotCount))
            {
                // There is no previous/next row and no previous/next writable cell on the current row
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessTabKey(e, shift, ctrl); }))
            {
                return true;
            }

            int targetSlot = -1, targetColumnIndex = -1;

            this._noSelectionChangeCount++;
            try
            {
                if (neighborVisibleWritableColumnIndex == -1)
                {
                    targetSlot = neighborSlot;
                    if (shift)
                    {
                        Debug.Assert(this.ColumnsInternal.LastVisibleWritableColumn != null);
                        targetColumnIndex = this.ColumnsInternal.LastVisibleWritableColumn.Index;
                    }
                    else
                    {
                        Debug.Assert(this.ColumnsInternal.FirstVisibleWritableColumn != null);
                        targetColumnIndex = this.ColumnsInternal.FirstVisibleWritableColumn.Index;
                    }
                }
                else
                {
                    targetSlot = this.CurrentSlot;
                    targetColumnIndex = neighborVisibleWritableColumnIndex;
                }

                DataGridSelectionAction action;
                if (targetSlot != this.CurrentSlot || (this.SelectionMode == DataGridSelectionMode.Extended))
                {
                    if (IsSlotOutOfBounds(targetSlot))
                    {
                        return true;
                    }
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(targetColumnIndex, targetSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this._successfullyUpdatedSelection && !this.RowGroupHeadersTable.Contains(targetSlot))
            {
                BeginCellEdit(e);
            }

            // Return true to say we handled the key event even if the operation was unsuccessful. If we don't
            // say we handled this event, the framework will continue to process the tab key and change focus.
            return true;
        }

        private bool ProcessUpKey(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            if (this.WaitForLostFocus(delegate { this.ProcessUpKey(shift, ctrl); }))
            {
                return true;
            }

            int previousVisibleSlot = (this.CurrentSlot != -1) ? GetPreviousVisibleSlot(this.CurrentSlot) : -1;

            this._noSelectionChangeCount++;

            try
            {
                int slot;
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    slot = firstVisibleSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        // Ctrl without Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (previousVisibleSlot == -1)
                    {
                        return true;
                    }
                    if (shift)
                    {
                        // Shift without Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Shift nor Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, slot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Remove(dataGridColumn.HeaderCell);
            }
        }

        private void RemoveDisplayedColumnHeaders()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Clear();
            }
            this.ColumnsInternal.FillerColumn.IsRepresented = false;
        }

        private bool ResetCurrentCellCore()
        {
            return (this.CurrentColumnIndex == -1 || SetCurrentCellCore(-1, -1));
        } 

        private void ResetEditingRow()
        {
            if (this.EditingRow != null
                && this.EditingRow != this._focusedRow
                && !IsSlotVisible(this.EditingRow.Slot))
            {
                // Unload the old editing row if it's off screen
                this.EditingRow.Clip = null;
                UnloadRow(this.EditingRow);
                this.DisplayData.FullyRecycleElements();
            }
            this.EditingRow = null;
        }

        private void ResetFocusedRow()
        {
            if (this._focusedRow != null
                && this._focusedRow != this.EditingRow
                && !IsSlotVisible(this._focusedRow.Slot))
            {
                // Unload the old focused row if it's off screen
                this._focusedRow.Clip = null;
                UnloadRow(this._focusedRow);
                this.DisplayData.FullyRecycleElements();
            }
            this._focusedRow = null;
        }

        private void ResetValidationStatus()
        {
            // Clear the invalid status of the Cell, Row and DataGrid
            if (this.EditingRow != null)
            {
                this.EditingRow.IsValid = true;
                if (this.EditingRow.Index != -1)
                {
                    foreach (DataGridCell cell in this.EditingRow.Cells)
                    {
                        if (!cell.IsValid)
                        {
                            cell.IsValid = true;
                            cell.ApplyCellState(true);
                        }
                    }
                    this.EditingRow.ApplyState(true);
                }
            }
            this.IsValid = true;

            // Clear the previous validation results
            this._validationResults.Clear();

            // Hide the error list if validation succeeded
            if (this._validationSummary != null && this._validationSummary.Errors.Count > 0)
            {
                this._validationSummary.Errors.Clear();
                if (this.EditingRow != null)
                {
                    int editingRowSlot = this.EditingRow.Slot;

                    InvalidateMeasure();
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        // It's possible that the DataContext or ItemsSource has changed by the time we reach this code,
                        // so we need to ensure that the editing row still exists before scrolling it into view
                        if (!IsSlotOutOfBounds(editingRowSlot))
                        {
                            ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                        }
                    });
                }
            }
        }

        private void RowGroupHeaderStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_rowsPresenter != null)
            {
                Style oldLastStyle = _rowGroupHeaderStylesOld.Count > 0 ? _rowGroupHeaderStylesOld[_rowGroupHeaderStylesOld.Count - 1] : null;
                while (_rowGroupHeaderStylesOld.Count < _rowGroupHeaderStyles.Count)
                {
                    _rowGroupHeaderStylesOld.Add(oldLastStyle);
                }

                Style lastStyle = _rowGroupHeaderStyles.Count > 0 ? _rowGroupHeaderStyles[_rowGroupHeaderStyles.Count - 1] : null;
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
                        Style oldStyle = groupHeader.Level < _rowGroupHeaderStylesOld.Count ? _rowGroupHeaderStylesOld[groupHeader.Level] : oldLastStyle; 
                        Style newStyle = groupHeader.Level < _rowGroupHeaderStyles.Count ? _rowGroupHeaderStyles[groupHeader.Level] : lastStyle;
                        EnsureElementStyle(groupHeader, oldStyle, newStyle);
                    }
                }
            }
            _rowGroupHeaderStylesOld.Clear();
            foreach (Style style in _rowGroupHeaderStyles)
            {
                _rowGroupHeaderStylesOld.Add(style);
            }
        }

        private void SelectAll()
        {
            SetRowsSelection(0, this.SlotCount - 1);
        }

        private void SetAndSelectCurrentCell(int columnIndex,
                                             int slot,
                                             bool forceCurrentCellSelection)
        {
            DataGridSelectionAction action = forceCurrentCellSelection ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.None;
            UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
        }

        // columnIndex = 2, rowIndex = -1 --> current cell belongs to the 'new row'.
        // columnIndex = 2, rowIndex = 2 --> current cell is an inner cell
        // columnIndex = -1, rowIndex = -1 --> current cell is reset
        // columnIndex = -1, rowIndex = 2 --> Unexpected
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
        {
            Debug.Assert(columnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(slot < this.SlotCount);
            Debug.Assert(columnIndex == -1 || this.ColumnsItemsInternal[columnIndex].IsVisible);
            Debug.Assert(!(columnIndex > -1 && slot == -1));

            if (columnIndex == this.CurrentColumnIndex &&
                slot == this.CurrentSlot)
            {
                Debug.Assert(this.DataConnection != null);
                Debug.Assert(this._editingColumnIndex == -1 || this._editingColumnIndex == this.CurrentColumnIndex);
                Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot || this.DataConnection.CommittingEdit);
                return true;
            }

            UIElement oldDisplayedElement = null;
            DataGridCellCoordinates oldCurrentCell = new DataGridCellCoordinates(this.CurrentCellCoordinates);

            object newCurrentItem = null;
            if (!this.RowGroupHeadersTable.Contains(slot))
            {
                int rowIndex = this.RowIndexFromSlot(slot);
                if (rowIndex >= 0 && rowIndex < this.DataConnection.Count)
                {
                    newCurrentItem = this.DataConnection.GetDataItem(rowIndex);
                }
            }

            if (this.CurrentColumnIndex > -1)
            {
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this.CurrentSlot < this.SlotCount);

                if (!IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot) &&
                    this.IsSlotVisible(oldCurrentCell.Slot))
                {
                    oldDisplayedElement = this.DisplayData.GetDisplayedElement(oldCurrentCell.Slot);
                }

                if (!this.RowGroupHeadersTable.Contains(oldCurrentCell.Slot) && !this._temporarilyResetCurrentCell)
                {
                    bool keepFocus = this.ContainsFocus;
                    if (commitEdit)
                    {
                        if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, keepFocus, true /*raiseEvents*/))
                        {
                            return false;
                        }
                        // Resetting the current cell: setting it to (-1, -1) is not considered setting it out of bounds
                        if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfSelectionBounds(columnIndex, slot)) ||
                            IsInnerCellOutOfSelectionBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot))
                        {
                            return false;
                        }
                        if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        this.CancelEdit(DataGridEditingUnit.Row, false);
                        ExitEdit(keepFocus);
                    }
                }
            }

            if (newCurrentItem != null)
            {
                slot = this.SlotFromRowIndex(this.DataConnection.IndexOf(newCurrentItem));
            }
            if (slot == -1 && columnIndex != -1)
            {
                return false;
            }
            this.CurrentColumnIndex = columnIndex;
            this.CurrentSlot = slot;


            if (this._temporarilyResetCurrentCell)
            {
                if (columnIndex != -1)
                {
                    this._temporarilyResetCurrentCell = false;
                }
            }
            if (!this._temporarilyResetCurrentCell && this._editingColumnIndex != -1)
            {
                this._editingColumnIndex = columnIndex;
            }

            if (oldDisplayedElement != null)
            {
                DataGridRow row = oldDisplayedElement as DataGridRow;
                if (row != null)
                {
                    // Don't reset the state of the current cell if we're editing it because that would put it in an invalid state
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, !(this._temporarilyResetCurrentCell && row.IsEditing && this._editingColumnIndex == oldCurrentCell.ColumnIndex));
                }
                else
                {
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, false /*applyCellState*/);
                }
            }

            if (this.CurrentColumnIndex > -1)
            {
                Debug.Assert(this.CurrentSlot > -1);
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this.CurrentSlot < this.SlotCount);
                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                }
            }

            return true;
        }

        private void SetVerticalOffset(double newVerticalOffset)
        {
            _verticalOffset = newVerticalOffset;
            if (_vScrollBar != null && !DoubleUtil.AreClose(newVerticalOffset, _vScrollBar.Value))
            {
                _vScrollBar.Value = _verticalOffset;
            }
        }

        /// <summary>
        /// Determines whether or not a specific validation result should be displayed in the ValidationSummary.
        /// </summary>
        /// <param name="validationResult">Validation result to display.</param>
        /// <returns>True if it should be added to the ValidationSummary, false otherwise.</returns>
        private bool ShouldDisplayValidationResult(ValidationResult validationResult)
        {
            if (this.EditingRow != null)
            {
                return !this._bindingValidationResults.ContainsEqualValidationResult(validationResult) ||
                    this.EditingRow.DataContext is IDataErrorInfo || this.EditingRow.DataContext is INotifyDataErrorInfo;
            }
            return false;
        }

        private void UpdateCurrentState(UIElement displayedElement, int columnIndex, bool applyCellState)
        {
            DataGridRow row = displayedElement as DataGridRow;
            if (row != null)
            {
                if (this.AreRowHeadersVisible)
                {
                    row.ApplyHeaderStatus(true /*animate*/);
                }
                DataGridCell cell = row.Cells[columnIndex];
                if (applyCellState)
                {
                    cell.ApplyCellState(true /*animate*/);
                }
            }
            else
            {
                DataGridRowGroupHeader groupHeader = displayedElement as DataGridRowGroupHeader;
                if (groupHeader != null)
                {
                    groupHeader.ApplyState(true /*useTransitions*/);
                    if (this.AreRowHeadersVisible)
                    {
                        groupHeader.ApplyHeaderStatus(true /*animate*/);
                    }
                }
            }
        }

        private void UpdateDisabledVisual()
        {
            if (this.IsEnabled)
            {
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
        }

        private void UpdateHorizontalScrollBar(bool needHorizScrollbar, bool forceHorizScrollbar, double totalVisibleWidth, double totalVisibleFrozenWidth, double cellsWidth)
        {
            if (this._hScrollBar != null)
            {
                if (needHorizScrollbar || forceHorizScrollbar)
                {
                    //          viewportSize
                    //        v---v
                    //|<|_____|###|>|
                    //  ^     ^
                    //  min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsWidth / (max - cellsWidth)

                    // always zero
                    this._hScrollBar.Minimum = 0;
                    if (needHorizScrollbar)
                    {
                        // maximum travel distance -- not the total width
                        this._hScrollBar.Maximum = totalVisibleWidth - cellsWidth;
                        Debug.Assert(totalVisibleFrozenWidth >= 0);
                        if (this._frozenColumnScrollBarSpacer != null)
                        {
                            this._frozenColumnScrollBarSpacer.Width = totalVisibleFrozenWidth;
                        }
                        Debug.Assert(this._hScrollBar.Maximum >= 0);

                        // width of the scrollable viewing area
                        double viewPortSize = Math.Max(0, cellsWidth - totalVisibleFrozenWidth);
                        this._hScrollBar.ViewportSize = viewPortSize;
                        this._hScrollBar.LargeChange = viewPortSize;
                        // The ScrollBar should be in sync with HorizontalOffset at this point.  There's a resize case
                        // where the ScrollBar will coerce an old value here, but we don't want that
                        if (this._hScrollBar.Value != this._horizontalOffset)
                        {
                            this._hScrollBar.Value = this._horizontalOffset;
                        }
                        this._hScrollBar.IsEnabled = true;
                    }
                    else
                    {
                        this._hScrollBar.Maximum = 0;
                        this._hScrollBar.ViewportSize = 0;
                        this._hScrollBar.IsEnabled = false;
                    }

                    if (this._hScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for
                        this._ignoreNextScrollBarsLayout = true;
                        // which no processing is needed.
                        this._hScrollBar.Visibility = Visibility.Visible;
                        if (this._hScrollBar.DesiredSize.Height == 0)
                        {
                            // We need to know the height for the rest of layout to work correctly so measure it now
                            this._hScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                    }
                }
                else
                {
                    this._hScrollBar.Maximum = 0;
                    if (this._hScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._hScrollBar.Visibility = Visibility.Collapsed;
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

        private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
        {
            int itemCount = this.DataConnection.Count;
            if (this._rowsPresenter != null && itemCount > 0)
            {
                Visibility newDetailsVisibility = Visibility.Collapsed;
                switch (newDetailsMode)
                {
                    case DataGridRowDetailsVisibilityMode.Visible:
                        newDetailsVisibility = Visibility.Visible;
                        this._showDetailsTable.AddValues(0, itemCount, Visibility.Visible);
                        break;
                    case DataGridRowDetailsVisibilityMode.Collapsed:
                        newDetailsVisibility = Visibility.Collapsed;
                        this._showDetailsTable.AddValues(0, itemCount, Visibility.Collapsed);
                        break;
                    case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
                        this._showDetailsTable.Clear();
                        break;
                }

                bool updated = false;
                foreach (DataGridRow row in this.GetAllRows())
                {
                    if (row.Visibility == Visibility.Visible)
                    {
                        if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                        {
                            // For VisibleWhenSelected, we need to calculate the value for each individual row
                            newDetailsVisibility = this._selectedItems.ContainsSlot(row.Slot) ? Visibility.Visible : Visibility.Collapsed;
                        }
                        if (row.DetailsVisibility != newDetailsVisibility)
                        {
                            updated = true;
                            row.SetDetailsVisibilityInternal(newDetailsVisibility, true /* raiseNotification */, false /* animate */);
                        }
                    }
                }
                if (updated)
                {
                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);

                    InvalidateRowsArrange(); // force invalidate
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool UpdateStateOnMouseLeftButtonDown(MouseButtonEventArgs mouseButtonEventArgs, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
        {
            bool beginEdit;

            Debug.Assert(slot >= 0);

            // Before changing selection, check if the current cell needs to be committed, and
            // check if the current row needs to be committed. If any of those two operations are required and fail, 
            // do not change selection, and do not change current cell.

            bool wasInEdit = this.EditingColumnIndex != -1;

            if (IsSlotOutOfBounds(slot))
            {
                return true;
            }

            if (wasInEdit && (columnIndex != this.EditingColumnIndex || slot != this.CurrentSlot) &&
                this.WaitForLostFocus(delegate { this.UpdateStateOnMouseLeftButtonDown(mouseButtonEventArgs, columnIndex, slot, allowEdit, shift, ctrl); }))
            {
                return true;
            }

            try
            {
                this._noSelectionChangeCount++;

                beginEdit = allowEdit &&
                            this.CurrentSlot == slot &&
                            columnIndex != -1 &&
                            (wasInEdit || this.CurrentColumnIndex == columnIndex) &&
                            !GetColumnEffectiveReadOnlyState(this.ColumnsItemsInternal[columnIndex]);

                DataGridSelectionAction action;
                if (this.SelectionMode == DataGridSelectionMode.Extended && shift)
                {
                    // Shift select multiple rows
                    action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else if (GetRowSelection(slot))  // Unselecting single row or Selecting a previously multi-selected row
                {
                    if (!ctrl && this.SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0)
                    {
                        // Unselect everything except the row that was clicked on
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else if (ctrl && this.EditingRow == null)
                    {
                        action = DataGridSelectionAction.RemoveCurrentFromSelection;
                    }
                    else
                    {
                        action = DataGridSelectionAction.None;
                    }
                }
                else // Selecting a single row or multi-selecting with Ctrl
                {
                    if (this.SelectionMode == DataGridSelectionMode.Single || !ctrl)
                    {
                        // Unselect the currectly selected rows except the new selected row
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        action = DataGridSelectionAction.AddCurrentToSelection;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this._successfullyUpdatedSelection && beginEdit && BeginCellEdit(mouseButtonEventArgs))
            {
                FocusEditingCell(true /*setFocus*/);
            }

            return true;
        }

        /// <summary>
        /// Updates the DataGrid's validation results, modifies the ValidationSummary's items,
        /// and sets the IsValid states of the UIElements.
        /// </summary>
        /// <param name="newValidationResults">New validation results.</param>
        /// <param name="scrollIntoView">If the validation results have changed, scrolls the editing row into view.</param>
        private void UpdateValidationResults(List<ValidationResult> newValidationResults, bool scrollIntoView)
        {
            bool validationResultsChanged = false;
            Debug.Assert(this.EditingRow != null);

            // Remove the validation results that have been fixed
            List<ValidationResult> removedValidationResults = new List<ValidationResult>();
            foreach (ValidationResult oldValidationResult in this._validationResults)
            {
                if (oldValidationResult != null && !newValidationResults.ContainsEqualValidationResult(oldValidationResult))
                {
                    removedValidationResults.Add(oldValidationResult);
                    validationResultsChanged = true;
                }
            }
            foreach (ValidationResult removedValidationResult in removedValidationResults)
            {
                this._validationResults.Remove(removedValidationResult);
                if (this._validationSummary != null)
                {
                    ValidationSummaryItem removedValidationSummaryItem = this.FindValidationSummaryItem(removedValidationResult);
                    if (removedValidationSummaryItem != null)
                    {
                        this._validationSummary.Errors.Remove(removedValidationSummaryItem);
                    }
                }
            }

            // Add any validation results that were just introduced
            foreach (ValidationResult newValidationResult in newValidationResults)
            {
                if (newValidationResult != null && !this._validationResults.ContainsEqualValidationResult(newValidationResult))
                {
                    this._validationResults.Add(newValidationResult);
                    if (this._validationSummary != null && ShouldDisplayValidationResult(newValidationResult))
                    {
                        ValidationSummaryItem newValidationSummaryItem = this.CreateValidationSummaryItem(newValidationResult);
                        if (newValidationSummaryItem != null)
                        {
                            this._validationSummary.Errors.Add(newValidationSummaryItem);
                        }
                    }
                    validationResultsChanged = true;
                }
            }

            if (validationResultsChanged)
            {
                this.UpdateValidationStatus();
            }
            if (!this.IsValid && scrollIntoView)
            {
                // Scroll the row with the error into view.
                int editingRowSlot = this.EditingRow.Slot;
                if (this._validationSummary != null)
                {
                    // If the number of errors has changed, then the ValidationSummary will be a different size,
                    // and we need to delay our call to ScrollSlotIntoView
                    this.InvalidateMeasure();
                    this.Dispatcher.BeginInvoke(delegate
                    {
                        // It's possible that the DataContext or ItemsSource has changed by the time we reach this code,
                        // so we need to ensure that the editing row still exists before scrolling it into view
                        if (!this.IsSlotOutOfBounds(editingRowSlot))
                        {
                            this.ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                        }
                    });
                }
                else
                {
                    this.ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                }
            }
        }

        /// <summary>
        /// Updates the IsValid states of the DataGrid, the EditingRow and its cells. All cells related to
        /// property-level errors are set to Invalid.  If there is an object-level error selected in the
        /// ValidationSummary, then its associated cells will also be flagged (if there are any).
        /// </summary>
        private void UpdateValidationStatus()
        {
            if (this.EditingRow != null)
            {
                foreach (DataGridCell cell in this.EditingRow.Cells)
                {
                    bool isCellValid = true;

                    Debug.Assert(cell.OwningColumn != null);
                    if (!cell.OwningColumn.IsReadOnly)
                    {
                        foreach (ValidationResult validationResult in this._validationResults)
                        {
                            if (this._propertyValidationResults.ContainsEqualValidationResult(validationResult) ||
                                this._selectedValidationSummaryItem != null && this._selectedValidationSummaryItem.Context == validationResult)
                            {
                                foreach (string bindingPath in validationResult.MemberNames)
                                {
                                    if (cell.OwningColumn.BindingPaths.Contains(bindingPath))
                                    {
                                        isCellValid = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (cell.IsValid != isCellValid)
                    {
                        cell.IsValid = isCellValid;
                        cell.ApplyCellState(true /*animate*/);
                    }
                }
                bool isRowValid = this._validationResults.Count == 0;
                if (this.EditingRow.IsValid != isRowValid)
                {
                    this.EditingRow.IsValid = isRowValid;
                    this.EditingRow.ApplyState(true /*animate*/);
                }
                this.IsValid = isRowValid;
            }
            else
            {
                this.IsValid = true;
            }

        }

        private void UpdateVerticalScrollBar(bool needVertScrollbar, bool forceVertScrollbar, double totalVisibleHeight, double cellsHeight)
        {
            if (this._vScrollBar != null)
            {
                if (needVertScrollbar || forceVertScrollbar)
                {
                    //          viewportSize
                    //        v---v
                    //|<|_____|###|>|
                    //  ^     ^
                    //  min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / max
                    // ->              = cellsHeight

                    // always zero
                    this._vScrollBar.Minimum = 0;
                    if (needVertScrollbar && !double.IsInfinity(cellsHeight))
                    {
                        // maximum travel distance -- not the total height
                        this._vScrollBar.Maximum = totalVisibleHeight - cellsHeight;
                        Debug.Assert(this._vScrollBar.Maximum >= 0);

                        // total height of the display area
                        this._vScrollBar.ViewportSize = cellsHeight;
                        this._vScrollBar.LargeChange = cellsHeight;
                        this._vScrollBar.IsEnabled = true;
                    }
                    else
                    {
                        this._vScrollBar.Maximum = 0;
                        this._vScrollBar.ViewportSize = 0;
                        this._vScrollBar.IsEnabled = false;
                    }

                    if (this._vScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._vScrollBar.Visibility = Visibility.Visible;
                        if (this._vScrollBar.DesiredSize.Width == 0)
                        {
                            // We need to know the width for the rest of layout to work correctly so measure it now
                            this._vScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }
                else
                {
                    this._vScrollBar.Maximum = 0;
                    if (this._vScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._vScrollBar.Visibility = Visibility.Collapsed;
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

        /// <summary>
        /// Validates the current editing row and updates the visual states.
        /// </summary>
        /// <param name="scrollIntoView">If true, will scroll the editing row into view when a new error is introduced.</param>
        /// <param name="wireEvents">If true, subscribes to the asynchronous INDEI ErrorsChanged events.</param>
        /// <returns>True if the editing row is valid, false otherwise.</returns>
        private bool ValidateEditingRow(bool scrollIntoView, bool wireEvents)
        {
            this._propertyValidationResults.Clear();
            this._indeiValidationResults.Clear();

            if (this.EditingRow != null)
            {
                Object dataItem = this.EditingRow.DataContext;
                Debug.Assert(dataItem != null);

                // Validate using the Validator.
                List<ValidationResult> validationResults = new List<ValidationResult>();
                ValidationContext context = new ValidationContext(dataItem, null, null);
                Validator.TryValidateObject(dataItem, context, validationResults, true);

                // Add any existing exception errors (in case we're editing a cell).
                // Note: these errors will only be displayed in the ValidationSummary if the
                // editing data item implements IDEI or INDEI.
                foreach (ValidationResult validationResult in this._bindingValidationResults)
                {
                    validationResults.AddIfNew(validationResult);
                    this._propertyValidationResults.Add(validationResult);
                }

                // IDEI entity validation.
                this.ValidateIdei(dataItem as IDataErrorInfo, null, null, validationResults);

                // INDEI entity validation.
                this.ValidateIndei(dataItem as INotifyDataErrorInfo, null, null, null, validationResults, wireEvents);

                // IDEI and INDEI property validation.
                foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
                {
                    foreach (string bindingPath in column.BindingPaths)
                    {
                        string declaringPath = null;
                        object declaringItem = dataItem;
                        string bindingProperty = bindingPath;

                        // Check for nested paths.
                        int lastIndexOfSeparator = bindingPath.LastIndexOfAny(new char[] {TypeHelper.PropertyNameSeparator, TypeHelper.LeftIndexerToken});
                        if (lastIndexOfSeparator >= 0)
                        {
                            declaringPath = bindingPath.Substring(0, lastIndexOfSeparator);
                            declaringItem = TypeHelper.GetNestedPropertyValue(dataItem, declaringPath);
                            if (bindingProperty[lastIndexOfSeparator] == TypeHelper.LeftIndexerToken)
                            {
                                bindingProperty = TypeHelper.PrependDefaultMemberName(declaringItem, bindingPath.Substring(lastIndexOfSeparator));
                            }
                            else
                            {
                                bindingProperty = bindingPath.Substring(lastIndexOfSeparator + 1);
                            }
                        }

                        // IDEI property validation.
                        this.ValidateIdei(declaringItem as IDataErrorInfo, bindingProperty, bindingPath, validationResults);

                        // INDEI property validation.
                        this.ValidateIndei(declaringItem as INotifyDataErrorInfo, bindingProperty, bindingPath, declaringPath, validationResults, wireEvents);
                    }
                }

                // Merge the new validation results with the existing ones.
                this.UpdateValidationResults(validationResults, scrollIntoView);

                // Return false if there are validation errors.
                if (!this.IsValid)
                {
                    return false;
                }
            }

            // Return true if there are no errors or there is no editing row.
            this.ResetValidationStatus();
            return true;
        }

        /// <summary>
        /// Checks an IDEI data object for errors for the specified property. New errors are added to the
        /// list of validation results.
        /// </summary>
        /// <param name="idei">IDEI object to validate.</param>
        /// <param name="bindingProperty">Name of the property to validate.</param>
        /// <param name="bindingPath">Path of the binding.</param>
        /// <param name="validationResults">List of results to add to.</param>
        private void ValidateIdei(IDataErrorInfo idei, string bindingProperty, string bindingPath, List<ValidationResult> validationResults)
        {
            if (idei != null)
            {
                string errorString = null;
                if (string.IsNullOrEmpty(bindingProperty))
                {
                    Debug.Assert(string.IsNullOrEmpty(bindingPath));
                    ValidationUtil.CatchNonCriticalExceptions(() => { errorString = idei.Error; });
                    if (!string.IsNullOrEmpty(errorString))
                    {
                        validationResults.AddIfNew(new ValidationResult(errorString));
                    }
                }
                else
                {
                    ValidationUtil.CatchNonCriticalExceptions(() => { errorString = idei[bindingProperty]; });
                    if (!string.IsNullOrEmpty(errorString))
                    {
                        ValidationResult validationResult = new ValidationResult(errorString, new List<string>() { bindingPath });
                        validationResults.AddIfNew(validationResult);
                        this._propertyValidationResults.Add(validationResult);
                    }
                }
            }
        }

        /// <summary>
        /// Checks an INDEI data object for errors on the specified path. New errors are added to the
        /// list of validation results.
        /// </summary>
        /// <param name="indei">INDEI object to validate.</param>
        /// <param name="bindingProperty">Name of the property to validate.</param>
        /// <param name="bindingPath">Path of the binding.</param>
        /// <param name="declaringPath">Path of the INDEI object.</param>
        /// <param name="validationResults">List of results to add to.</param>
        /// <param name="wireEvents">True if the ErrorsChanged event should be subscribed to.</param>
        private void ValidateIndei(INotifyDataErrorInfo indei, string bindingProperty, string bindingPath, string declaringPath, List<ValidationResult> validationResults, bool wireEvents)
        {
            if (indei != null)
            {
                if (indei.HasErrors)
                {
                    IEnumerable errors = null;
                    ValidationUtil.CatchNonCriticalExceptions(() => { errors = indei.GetErrors(bindingProperty); });
                    if (errors != null)
                    {
                        foreach (object errorItem in errors)
                        {
                            if (errorItem != null)
                            {
                                string errorString = null;
                                ValidationUtil.CatchNonCriticalExceptions(() => { errorString = errorItem.ToString(); });
                                if (!string.IsNullOrEmpty(errorString))
                                {
                                    ValidationResult validationResult;
                                    if (!string.IsNullOrEmpty(bindingProperty))
                                    {
                                        validationResult = new ValidationResult(errorString, new List<string>() { bindingPath });
                                        this._propertyValidationResults.Add(validationResult);
                                    }
                                    else
                                    {
                                        Debug.Assert(string.IsNullOrEmpty(bindingPath));
                                        validationResult = new ValidationResult(errorString);
                                    }
                                    validationResults.AddIfNew(validationResult);
                                    this._indeiValidationResults.AddIfNew(validationResult);
                                }
                            }
                        }
                    }
                }
                if (wireEvents)
                {
                    indei.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
                    if (!this._validationItems.ContainsKey(indei))
                    {
                        this._validationItems.Add(indei, declaringPath);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the asynchronous INDEI errors that occur while the DataGrid is in editing mode.
        /// </summary>
        /// <param name="sender">INDEI item whose errors changed.</param>
        /// <param name="e">Error event arguments.</param>
        private void ValidationItem_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            INotifyDataErrorInfo indei = sender as INotifyDataErrorInfo;
            if (this._validationItems.ContainsKey(indei))
            {
                Debug.Assert(this.EditingRow != null);

                // Determine the binding path.
                string bindingPath = this._validationItems[indei];
                if (string.IsNullOrEmpty(bindingPath))
                {
                    bindingPath = e.PropertyName;
                }
                else if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName.IndexOf(TypeHelper.LeftIndexerToken) >= 0)
                {
                    bindingPath += TypeHelper.RemoveDefaultMemberName(e.PropertyName);
                }
                else
                {
                    bindingPath += TypeHelper.PropertyNameSeparator + e.PropertyName;
                }

                // Remove the old errors.
                List<ValidationResult> validationResults = new List<ValidationResult>();
                foreach (ValidationResult validationResult in this._validationResults)
                {
                    ValidationResult oldValidationResult = this._indeiValidationResults.FindEqualValidationResult(validationResult);
                    if (oldValidationResult != null && oldValidationResult.ContainsMemberName(bindingPath))
                    {
                        this._indeiValidationResults.Remove(oldValidationResult);
                    }
                    else
                    {
                        validationResults.Add(validationResult);
                    }
                }

                // Find any new errors and update the visuals.
                this.ValidateIndei(indei, e.PropertyName, bindingPath, null, validationResults, false /*wireEvents*/);
                this.UpdateValidationResults(validationResults, false /*scrollIntoView*/);
                
                // If we're valid now then reset our status.
                if (this.IsValid)
                {
                    this.ResetValidationStatus();
                }
            }
            else if (indei != null)
            {
                indei.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(ValidationItem_ErrorsChanged);
            }
        }

        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessVerticalScroll(e.ScrollEventType);
        }

#endregion Private Methods
    }
}
