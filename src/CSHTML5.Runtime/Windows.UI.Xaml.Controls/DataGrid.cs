﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    /// Represents a control that displays data in a customizable grid.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <DataGrid x:Name="DataGrid1" Margin="0,10,0,0" AutoGenerateColumns="True" IsReadOnly="True" HorizontalGridLinesBrush="Gray" VerticalGridLinesBrush="Gray" HorizontalAlignment="Left">
    ///     <DataGrid.ColumnHeaderStyle>
    ///         <Style TargetType="DataGridColumnHeader">
    ///             <Setter Property="Background" Value="Gray"/>
    ///             <Setter Property="Foreground" Value="White"/>
    ///         </Style>
    ///     </DataGrid.ColumnHeaderStyle>
    /// </DataGrid>
    /// </code>
    /// <code lang="C#">
    /// DataGrid1.ItemsSource = Planet.GetListOfPlanets();
    /// public partial class Planet
    /// {
    ///    public string Name { get; set; }
    ///    public string ImagePath { get; set; }
    ///
    ///    public static ObservableCollection&lt;Planet&gt; GetListOfPlanets()
    ///    {
    ///        return new ObservableCollection&lt;Planet&gt;()
    ///        {
    ///            new Planet() { Name = "Mercury", ImagePath = "ms-appx:/Planets/Mercury.png" },
    ///            new Planet() { Name = "Venus", ImagePath = "ms-appx:/Planets/Venus.png" },
    ///            new Planet() { Name = "Earth", ImagePath = "ms-appx:/Planets/Earth.png" },
    ///            new Planet() { Name = "Mars", ImagePath = "ms-appx:/Planets/Mars.png" },
    ///            new Planet() { Name = "Jupiter", ImagePath = "ms-appx:/Planets/Jupiter.png" },
    ///            new Planet() { Name = "Saturn", ImagePath = "ms-appx:/Planets/Saturn.png" },
    ///            new Planet() { Name = "Uranus", ImagePath = "ms-appx:/Planets/Uranus.png" },
    ///            new Planet() { Name = "Neptune", ImagePath = "ms-appx:/Planets/Neptune.png" }
    ///            new Planet() { Name = "Pluto", ImagePath = "ms-appx:/Planets/Pluto.png" }
    ///        };
    ///    }
    ///} 
    /// </code>
    /// </example>
    public partial class DataGrid : MultiSelector
    {
        Dictionary<object, DataGridRow> _objectsToDisplay = new Dictionary<object, DataGridRow>();
        Grid _grid;
        bool _areItemsInVisualTree = false;
        UIElement _currentEditionElement = null;
        DataGridCell _currentCell = null;

        // members for paging
        PagedCollectionView _pagedView;
        DataPager _pagerUI = new DataPager();
        IEnumerable _oldOldValue; // When the user sets the value of ItemsSource, the DataGrid changes the ItemsSource again in order to replace it with a PagedCollectionView (which allows for sorting, filtering, grouping, etc.). 

        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.DataGrid class.
        /// </summary>
        public DataGrid()
        {
            Columns = new ObservableCollection<DataGridColumn>();

            _pagedView = new PagedCollectionView(null);
            _pagedView.PageSize = 20;
            _pagerUI.Source = _pagedView;
            _pagerUI.PageSize = 20;
            _pagerUI.DisplayMode = DataPager_DisplayMode.FirstLastPreviousNext;
        }

        public DataPager DataPager { get { return _pagerUI; } }

        protected virtual void OnLoadingRow(DataGridRowEventArgs e)
        {
            if (LoadingRow != null)
            {
                LoadingRow(this, e);
            }
        }

        [Obsolete("Use AlternatingRowBackground instead")]
        public Brush UnselectedItemAlternateBackground
        {
            get { return AlternatingRowBackground; }
            set { AlternatingRowBackground = value; }
        }
       
        /// <summary>
        /// Gets or sets the alternative bakground color of the Items that are not selected.
        /// </summary>
        public Brush AlternatingRowBackground
        {
            get { return (Brush)GetValue(AlternatingRowBackgroundProperty); }
            set { SetValue(AlternatingRowBackgroundProperty, value); }
        }
        /// <summary>
        /// Identifies the AlternatingRowBackground dependency property
        /// </summary>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty =
            DependencyProperty.Register("AlternatingRowBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString("#f0f0e9"))));




        /// <summary>
        /// Gets or sets the vertical padding in all cells
        /// </summary>
        public double VerticalCellPadding
        {
            get { return (double)GetValue(VerticalCellPaddingProperty); }
            set { SetValue(VerticalCellPaddingProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalCellPadding dependency property
        /// </summary>
        public static readonly DependencyProperty VerticalCellPaddingProperty =
            DependencyProperty.Register("VerticalCellPadding", typeof(double), typeof(DataGrid), new PropertyMetadata(3d));

        /// <summary>
        ///     Event that is fired just before a row is prepared.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> LoadingRow;

        /// <summary>
        ///     Event that is fired just before a row is cleared.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> UnloadingRow;

        /// <summary>
        ///     Updates the reference to this DataGrid in the list of columns.
        /// </summary>
        internal void UpdateDataGridReference()
        {
            foreach (DataGridColumn column in Columns)
            {
                column._parent = this;
            }
        }

        // Returns:
        //     true if columns are created automatically; otherwise, false. The registered
        //     default is true. For more information about what can influence the value,
        //     see System.Windows.DependencyProperty.
        /// <summary>
        /// Gets or sets a value that indicates whether the columns are created automatically.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.AutoGenerateColumns dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(DataGrid), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the style applied to all column headers in the System.Windows.Controls.DataGrid.
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.ColumnHeaderStyle dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderStyleProperty =
            DependencyProperty.Register("ColumnHeaderStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(null, ColumnHeaderStyle_Changed));

        private static void ColumnHeaderStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid)
            {
                DataGrid dataGrid = (DataGrid)d;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
                {
                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        column.SetHeaderStyleIfColumnsStyleNotSet((Style)e.NewValue);
                    }
                }
            }
        }

        public double ColumnHeaderHeight
        {
            get { return (double)GetValue(ColumnHeaderHeightProperty); }
            set { SetValue(ColumnHeaderHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderHeightProperty =
            DependencyProperty.Register("ColumnHeaderHeight", typeof(double), typeof(DataGrid), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the style applied to all cells in the System.Windows.Controls.DataGrid.
        /// </summary>
        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.CellStyle dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty CellStyleProperty =
            DependencyProperty.Register("CellStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(null, CellStyle_Changed));

        private static void CellStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid)
            {
                DataGrid dataGrid = (DataGrid)d;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
                {
                    foreach (FrameworkElement cell in dataGrid._grid.Children)
                    {
                        if (cell is DataGridCell) //Note: it can also be DataGridColumnHeader
                        {
                            DataGridCell cellAsDataGridCell = (DataGridCell)cell;
                            DataGridColumn column = cellAsDataGridCell.Column;
                            if (column.CellStyle != null)
                            {
                                cellAsDataGridCell.Style = column.CellStyle;
                            }
                            else if (dataGrid.CellStyle != null)
                            {
                                cellAsDataGridCell.Style = dataGrid.CellStyle;
                            }
                        }
                    }
                }
            }
        }

        private ObservableCollection<DataGridColumn> AutoGeneratedColumns;
        /// <summary>
        /// Gets a collection that contains all the columns in the System.Windows.Controls.DataGrid.
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.Columns dependency property.</summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid), new PropertyMetadata(null, Columns_Changed));

        static void Columns_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo
            //var dataGrid = (DataGrid)d;
            //ObservableCollection<DataGridColumn> newValue = (ObservableCollection<DataGridColumn>)e.NewValue;
            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
            //{
            //    if (newValue is ObservableCollection<DataGridColumn>)
            //    {
            //        //todo ?
            //    }
            //}
        }


        /// <summary>
        /// Gets or sets the maximum number of items to display on a page.
        /// -1 means infinity. Default is 20.
        /// </summary>
        public int PageSize
        {
            get
            {
                if (_pagerUI != null)
                    return _pagerUI.PageSize;
                else
                    return -1;
            }
            set
            {
                if (_pagerUI != null)
                    _pagerUI.PageSize = value;
            }
        }

        // Returns:
        //     true if the rows and cells are read-only; otherwise, false. The registered
        //     default is false. For more information about what can influence the value,
        //     see System.Windows.DependencyProperty.
        /// <summary>
        /// Gets or sets a value that indicates whether the user can edit values in the
        /// System.Windows.Controls.DataGrid.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataGrid), new PropertyMetadata(false, IsReadonly_Changed));

        private static void IsReadonly_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: manage this
        }

        /// <summary>
        /// Gets or sets a value that indicates whether entering Edit Mode requires two steps
        /// (like in Silverlight) rather than one. 'True' means that the user must first select
        /// a cell and then click again to enter Edit Mode. 'False' means that the DataGrid
        /// enters Edit Mode immediately when a cell is clicked, even if the cell is not
        /// selected yet (it will get selected too). Default is false.
        /// </summary>
        public bool EnableTwoStepsEditMode
        {
            get { return (bool)GetValue(EnableTwoStepsEditModeProperty); }
            set { SetValue(EnableTwoStepsEditModeProperty, value); }
        }

        public static readonly DependencyProperty EnableTwoStepsEditModeProperty =
            DependencyProperty.Register("EnableTwoStepsEditMode", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));



        /// <summary>
        /// Gets or sets a value that indicates how rows and cells are selected in the
        /// System.Windows.Controls.DataGrid.
        /// </summary>
        public DataGridSelectionMode SelectionMode
        {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(DataGridSelectionMode), typeof(DataGrid), new PropertyMetadata(DataGridSelectionMode.Single, SelectionMode_Changed));

        private static void SelectionMode_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                DataGrid dataGrid = (DataGrid)d;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
                {
                    foreach (DataGridRow dataGridRow in
#if BRIDGE
                        INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(dataGrid._objectsToDisplay)
#else
                        dataGrid._objectsToDisplay.Values
#endif
                        )
                    {
                        dataGridRow.ChangeIntoSelectionMode((DataGridSelectionMode)(e.NewValue));
                    }
                    dataGrid.UpdateChildrenInVisualTree(dataGrid.Items, dataGrid.Items, true);
                    dataGrid.UpdateUIStructure();
                }
            }
        }

        protected override void ManageSelectedIndex_Changed(DependencyPropertyChangedEventArgs e)
        {
            base.ManageSelectedIndex_Changed(e);
            if ((e.OldValue != null && (int)e.OldValue >= 0) && SelectionMode == DataGridSelectionMode.Single)
            {
                INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
            }
            if (e.NewValue != null && ((int)e.NewValue >= 0))
            {
                if (SelectionMode == DataGridSelectionMode.Single)
                {
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems);
                    SelectedItems.Add(Items[(int)e.NewValue]);
                }
                else if (INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, e.NewValue)) //todo: remove the cast once the bug with the IList elements undefined in javascript is fixed.
                {
                    SelectedItems.Add(Items[(int)e.NewValue]);
                }
            }
        }

        ///// <summary>
        ///// Gets or sets a value that indicates whether rows, cells, or both can be selected
        ///// in the System.Windows.Controls.DataGrid.
        ///// </summary>
        //public DataGridSelectionUnit SelectionUnit
        //{
        //    get { return (DataGridSelectionUnit)GetValue(SelectionUnitProperty); }
        //    set { SetValue(SelectionUnitProperty, value); }
        //}

        //public static readonly DependencyProperty SelectionUnitProperty =
        //    DependencyProperty.Register("SelectionUnit", typeof(DataGridSelectionUnit), typeof(DataGrid), new PropertyMetadata(DataGridSelectionUnit.FullRow));

        //todo: remove this once we will support ControlTemplates        
        /// <summary>
        /// Temporary solution to allow the choice of the DataGrid's horizontal lines colors
        /// </summary>
        /// <exclude/>
        public Brush HorizontalGridLinesBrush
        {
            get { return (Brush)GetValue(HorizontalGridLinesBrushProperty); }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.HorizontalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register("HorizontalGridLinesBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(new SolidColorBrush(Colors.Gray), HorizontalGridLinesBrush_Changed));

        private static void HorizontalGridLinesBrush_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = ((DataGrid)sender);
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
            {
                dataGrid._grid.INTERNAL_StringToSetHorizontalGridLinesInCss = dataGrid.GenerateStringForGridLines((Brush)e.NewValue);
                Grid_InternalHelpers.RefreshGridLines(dataGrid._grid, dataGrid._grid._currentCellsStructure, dataGrid._grid.ColumnDefinitions, dataGrid._grid.RowDefinitions);
            }
        }



        /// <summary>
        /// Gets or set the template for the row headers.
        /// </summary>
        public DataTemplate RowHeaderTemplate
        {
            get { return (DataTemplate)GetValue(RowHeaderTemplateProperty); }
            set { SetValue(RowHeaderTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.RowHeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderTemplateProperty =
            DependencyProperty.Register("RowHeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null, RowHeaderTemplate_Changed));

        private static void RowHeaderTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            //we apply the new Header to all the rows currently displayed :

            DataTemplate newValue = dataGrid.GetActiveRowHeaderTemplate();

            foreach (DataGridRow row in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(dataGrid._objectsToDisplay)
#else
                dataGrid._objectsToDisplay.Values
#endif
                )
            {
                row.HeaderTemplate = newValue;
            }
        }

        private DataTemplate GetActiveRowHeaderTemplate()
        {
            if (RowHeaderTemplate == null)
            {
                if (SelectionMode == DataGridSelectionMode.Extended)
                {
                    return DataGridRow.DefaultTemplateForExtendedSelectionMode;
                }
                else
                {
                    return null;
                }
            }
            return RowHeaderTemplate;
        }




        //todo: remove this once we will support ControlTemplates
        /// <summary>
        /// Temporary solution to allow the choice of the DataGrid's vertical lines colors
        /// </summary>
        /// <exclude/>
        public Brush VerticalGridLinesBrush
        {
            get { return (Brush)GetValue(VerticalGridLinesBrushProperty); }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGrid.VerticalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register("VerticalGridLinesBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(new SolidColorBrush(Colors.Gray), VerticalGridLinesBrush_Changed));

        private static void VerticalGridLinesBrush_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = ((DataGrid)sender);
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGrid))
            {
                dataGrid._grid.INTERNAL_StringToSetVerticalGridLinesInCss = dataGrid.GenerateStringForGridLines((Brush)e.NewValue);
                Grid_InternalHelpers.RefreshGridLines(dataGrid._grid, dataGrid._grid._currentCellsStructure, dataGrid._grid.ColumnDefinitions, dataGrid._grid.RowDefinitions);
            }
        }

        private string GenerateStringForGridLines(Brush brush)
        {
            string str = "thin ";
            if (brush is SolidColorBrush)
            {
                str += "solid ";
                str += ((SolidColorBrush)brush).INTERNAL_ToHtmlString();
            }
            else //todo: else if(brush is ....)
            {
                str += "solid black"; //we set this as the default value because why not
            }
            return str;
        }

        /// <exclude/>
        internal protected override void INTERNAL_OnAttachedToVisualTree()
        {
            if (_grid == null)
            {
                MakeUIStructure();
                _grid.INTERNAL_StringToSetVerticalGridLinesInCss = "thin solid black";
                _grid.INTERNAL_StringToSetHorizontalGridLinesInCss = "thin solid black";
            }
            AttachGridToVisualTree();
            //UpdateChildrenInVisualTree(Items, Items);
            if (_renderedItemsPanel == null && ItemsPanel != null)
            {
                UpdateItemsPanel(ItemsPanel);
            }
            else if (_renderedItemsPanel != null)
            {
                //we set the new Items (which will refresh the display)
                if (_objectsToDisplay != null && _objectsToDisplay.Count > 0)
                {
                    //when we arrive here, it means that the DataGrid has been removed from the visual tree, then put back in (for example in a tab)
                    _areItemsInVisualTree = true;
                    UpdateChildrenInVisualTree(_objectsToDisplay.Keys, Items, forceUpdateAllChildren: true);
                }
                else
                {
                    UpdateChildrenInVisualTree(null, Items);
                }
            }
        }

        internal protected override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            _areItemsInVisualTree = false;
        }


        /// <summary>
        /// Creates the structure: creates the grid that will contain the elements and sets its columns
        /// </summary>
        private void MakeUIStructure()
        {
            //Note: The Columns property must be filled before calling this method, otherwise, it's pointless
            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            if (AutoGenerateColumns)
            {
                GenerateColumns();
            }
            GenerateFakeColumnToTheEnd();
            AddColumns(Columns);
        }

        private void UpdateUIStructure() //this method has to be called everytime a change in the columns appears because we cannot just add the new columns (the order has to be respected).
        {
            if (_grid != null) //it should always be the case
            {
                //INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_grid, this); //we remove the grid from the visual tree because apparently, the columns are not correctly created while the grid is in the visual tree
                if (Columns != null)
                {
                    // remove all the headers in the grid because they are not deleted automatically and the headers texts will overlap.
                    RemoveAllHeaders();

                    //remove the columns from the Grid:
                    _grid.ColumnDefinitions.Clear();

                    //add the columns back:
                    if (AutoGenerateColumns)
                    {
                        GenerateColumns();
                    }
                    GenerateFakeColumnToTheEnd();//we add the fake column back

                    AddColumns(Columns);
                }
                //AttachGridToVisualTree();

            }
            else
            {
                throw new Exception("Problem while changing DataGrid.ItemsSource, value cannot be null. Please report this issue to support@cshtml5.com.");
            }
        }

        protected override void UpdateItemsPanel(ItemsPanelTemplate newTemplate)
        {
            _renderedItemsPanel = new StackPanel();
        }

        internal void AddElementToGrid(UIElement elementToAdd)
        {
            //the row and column will be set directly from the method that created the element.
            _grid.Children.Add(elementToAdd);
        }
        internal void RemoveElementFromGrid(UIElement elementToRemove)
        {
            _grid.Children.Remove(elementToRemove);
        }

        internal void RemoveAllHeaders()
        {
            List<UIElement> ToRemove = new List<UIElement>();
            foreach (UIElement element in _grid.Children)
            {
                if (element is DataGridColumnHeader)
                    ToRemove.Add(element);
            }

            for (int i = 0; i < ToRemove.Count; i++)
            {
                _grid.Children.Remove(ToRemove[i]);
            }
        }

        private void AddColumns(ObservableCollection<DataGridColumn> columnsToAdd)
        {
            if (columnsToAdd != null)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_grid, this); //NOTE1: we remove the grid from the visual tree because each addition of a column or a row makes it recreate the whole dom for the grid, so might as well remove it completely from the visual tree, make the changes, then put it back.
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_pagerUI, this);

                //we add the column for the rows' headers:
                ColumnDefinition columnDefinitionForHeader = new ColumnDefinition(); //todo: remove the first column when the SelectionMode is Single (do not forget every change it implies)
                columnDefinitionForHeader.Width = GridLength.Auto;
                if (SelectionMode == DataGridSelectionMode.Single) //if the selectionMode is Single, we don't want to see the CheckBox.
                {
                    //we make sure the first column has a null width:
                    columnDefinitionForHeader.Width = new GridLength(0);
                }
                _grid.ColumnDefinitions.Add(columnDefinitionForHeader);


                //we create a list of the headers to add to the Grid because _grid.Children.Add(columnHeader); would trigger
                //CollectionChanged on Grid.Children, which would result in a crash because all the ColumnDefinitions are not 
                //added to the grid yet. These headers will be added after the foreach on the Columns.
                List<DataGridColumnHeader> headers = new List<DataGridColumnHeader>();


                bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();

                //we add the columns for the Data:
                int currentColumnIndex = 1;
                foreach (DataGridColumn column in columnsToAdd)
                {
                    column._parent = this;
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    column._gridColumn = columnDefinition;
                    if (column.Visibility == Visibility.Collapsed)
                    {
                        column._gridColumn.Visibility = Visibility.Collapsed;
                    }
                    DataGridColumn.UpdateGridColumnWidth(column);
                    _grid.ColumnDefinitions.Add(columnDefinition);
                    DataGridColumnHeader columnHeader = column.GetHeader();

                    columnHeader.HorizontalContentAlignment = HorizontalAlignment.Center;
                    columnHeader.VerticalContentAlignment = VerticalAlignment.Center;
                    columnHeader.FontWeight = FontWeights.Bold;
                    if(!double.IsNaN(ColumnHeaderHeight))
                    {
                        columnHeader.Height = ColumnHeaderHeight;
                    }


                    // If a local column style is defined, we use it, otherwise we use the global one:
                    if (column.HeaderStyle != null)
                    {
                        columnHeader.Style = column.HeaderStyle;
                    }
                    else if (ColumnHeaderStyle != null)
                    {
                        columnHeader.Style = ColumnHeaderStyle;
                    }

                    Grid.SetColumn(columnHeader, currentColumnIndex);
                    headers.Add(columnHeader);

                    //we set column.IsReadOnly = true if the user explicitely set its binding in OneWay mode:
                    if (column is DataGridBoundColumn)
                    {
                        Binding b = column.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty);
                        if (b != null)
                        {
                            if (b.Mode == BindingMode.OneWay && b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                            {
                                column.IsReadOnly = true;
                            }
                        }
                    }
                    ++currentColumnIndex;
                }

                foreach (DataGridColumnHeader header in headers)
                {
                    if (isCSSGrid)
                    {
                        Brush brush = HorizontalGridLinesBrush;
                        int HorizontalLinesThickness = HorizontalGridLinesBrush.Opacity > 0 ? 1 : 0;
                        int VerticalLinesThickness = VerticalGridLinesBrush.Opacity > 0 ? 1 : 0;
                        if (HorizontalLinesThickness == 0)
                        {
                            brush = VerticalGridLinesBrush;
                        }
                        header.BorderBrush = brush;
                        header.BorderThickness = new Thickness((Grid.GetColumn(header) == 1 ? VerticalLinesThickness : 0),
                                                               HorizontalLinesThickness,
                                                               VerticalLinesThickness,
                                                               HorizontalLinesThickness);
                    }
                    int u = Grid.GetColumn(header);
                    _grid.Children.Add(header);
                }

                AttachGridToVisualTree(); //We no longer change the structure of the grid so we put it back in the visual tree (see NOTE1).

            }
        }

        protected override void ManageCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (_renderedItemsPanel != null)
                    {
                        //we check if we added in ItemsSource or in Items: If ItemsSource is empty or null, then we added through Items.Add(..).
                        bool isItemsSourceEmptyOrNull = (_itemsSourceAsSetByUser == null);
                        if (!isItemsSourceEmptyOrNull)
                        {
                            foreach (object element in _itemsSourceAsSetByUser)
                            {
                                isItemsSourceEmptyOrNull = false;
                                break;
                            }
                        }

                        if (AutoGenerateColumns && isItemsSourceEmptyOrNull)
                        {
                            if (Items.Count == 1) //we only need to create the columns when it is the first element we add.
                            {
                                UpdateUIStructure();
                            }
                        }

                        //todo-performance: uncomment the following if/else code:
                        //if (e.NewStartingIndex == Items.Count - e.NewItems.Count)
                        //{
                        //    foreach (object item in e.NewItems)
                        //    {
                        //        AddChild(item);
                        //    }
                        //}
                        //else
                        //{
                        //we generate the list of the children before the change:
                        List<object> oldChildren = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(Items); //we use this to make a clone of the list in Items
                        //we remove the elements that have been added to Items:
                        int i = e.NewItems.Count - 1;
                        while (i >= 0)
                        {
                            oldChildren.RemoveAt(e.NewStartingIndex + i);
                            --i;
                        }
                        //now we can update the ItemsControl:
                        UpdateChildrenInVisualTree(oldChildren, Items, true);
                        //}
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (_renderedItemsPanel != null)
                    {
                        //we generate the list of the children before the change:
                        List<object> oldChildren2 = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(Items); //we use this to make a clone of the list in Items
                        //we add the elements that have been removed to Items:
                        List<object> oldItems = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(e.OldItems);
                        for (int i = 0; i < oldItems.Count; ++i)
                        {
                            string str = e.OldStartingIndex.ToString();
                            if (e.OldStartingIndex >= 0)
                            {
                                oldChildren2.Insert(e.OldStartingIndex + i, oldItems[i]);
                            }
                            else
                            {
                                oldChildren2.Add(oldItems[i]);
                            }
                        }

                        //todo-performance: uncomment the following if/else code:
                        //if (e.OldStartingIndex == Items.Count)
                        //{
                        //    foreach (object item in oldItems)
                        //    {
                        //        TryRemoveChildItemFromVisualTree(item, e.OldStartingIndex, oldChildren2);
                        //    }
                        //}
                        //else
                        //{
                        //now we can update the ItemsControl:
                        UpdateChildrenInVisualTree(oldChildren2, Items, true);
                        //}
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    //todo
                    break;
                case NotifyCollectionChangedAction.Move:
                    //todo
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (_renderedItemsPanel != null)
                    {
                        //we generate a list with the elements that were in the DataGrid:
                        RemoveAllChildren();
                    }
                    break;
                default:
                    //todo
                    break;
            }
        }

        /// <summary>
        /// Adds a row to the DataGrid. NOTE: The element can only be added at the end of the list.
        /// </summary>
        /// <param name="childData"></param>
        private bool AddChild(object childData)
        {
            // Verify that the object we are adding is not already present (our implementation does not support adding twice the same object to the DataGrid source). //todo: support this?
            if (!_objectsToDisplay.ContainsKey(childData))
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                _grid.RowDefinitions.Add(rowDefinition);

                DataGridRow objectRow = new DataGridRow();
                objectRow._datagrid = this;
                //we add the header:
                objectRow._representationInRow.RowDefinition = rowDefinition;
                objectRow._rowIndex = _grid.RowDefinitions.Count - 1;
                objectRow.HeaderTemplate = GetActiveRowHeaderTemplate();

                bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();

                int currentColumnIndex = 1; //1 because of the column for the header
                foreach (DataGridColumn column in Columns)
                {
                    DataGridCell cell = new DataGridCell();
                    cell.Item = childData;
                    cell.DataContext = childData;
                    cell.Column = column;
                    FrameworkElement f = column.GenerateElement(childData);
                    Grid.SetRow(cell, _grid.RowDefinitions.Count - 1);
                    Grid.SetColumn(cell, currentColumnIndex);
                    cell.Content = f;
                    cell.RegisterToContentPressEvent();
                    objectRow._representationInRow.ElementsInRow.Add(cell);
                    cell.Click += CellElement_Click;
                    _grid.Children.Add(cell);
                    if (isCSSGrid)
                    {
                        Brush brush = HorizontalGridLinesBrush;
                        int HorizontalLinesThickness = HorizontalGridLinesBrush.Opacity > 0 ? 1 : 0;
                        int VerticalLinesThickness = VerticalGridLinesBrush.Opacity > 0 ? 1 : 0;
                        if (HorizontalLinesThickness == 0)
                        {
                            brush = VerticalGridLinesBrush;
                        }
                        cell.BorderBrush = brush;
                        cell.BorderThickness = new Thickness((currentColumnIndex == 1 ? VerticalLinesThickness : 0),
                                                            (_grid.RowDefinitions.Count == 1 ? HorizontalLinesThickness : 0),
                                                            VerticalLinesThickness,
                                                            HorizontalLinesThickness);
                        cell.Padding = new Thickness(4, VerticalCellPadding, 4, VerticalCellPadding);
                    }

                    int elementRow = _grid.RowDefinitions.Count - 1;

                    if (elementRow % 2 == 0 && elementRow != 0)//todo: we assumed that the cell was not selected, if it can be selected at the moment where we add it (programatically for example), add the management of the case where it is selected.
                    {
                        cell.Background = AlternatingRowBackground;
                    }
                    else
                    {
                        cell.Background = RowBackground;
                    }
                    cell.Foreground = UnselectedItemForeground;

                    if (column.CellStyle != null)
                    {
                        cell.Style = column.CellStyle;
                    }
                    else if(CellStyle != null)
                    {
                        cell.Style = CellStyle;
                    }
                    currentColumnIndex++;
                }

                objectRow.INTERNAL_SetRowEvents();

                _objectsToDisplay.Add(childData, objectRow);

                objectRow.INTERNAL_SetDataContext();

                OnLoadingRow(new DataGridRowEventArgs(objectRow));

                return true;
            }
            else
            {
                return false;
            }
        }

        void CellElement_Click(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            if (_currentCell != cell)
            {
                //-----------------------
                // The clicked cell was not already selected
                //-----------------------

                if (SelectionMode == DataGridSelectionMode.Single && SelectedItem != null)
                {
                    UnselectItem(SelectedItem);
                }
                if (_currentCell != null)
                {
                    LeaveCellEditionMode();
                }
                SelectItem(cell.Item);
                _currentCell = cell;

                // Enter Edit Mode (upon selection) only if the option "EnableTwoStepsEditMode" is not enabled (otherwise the user must first select the cell, like in Silverlight):
                if (!EnableTwoStepsEditMode)
                {
                    EnterCellEditionMode(cell);
                }
            }
            else
            {
                //-----------------------
                // The clicked cell was already selected
                //-----------------------

                // Enter Edit Mode:
                EnterCellEditionMode(cell);
            }
        }

        private void RemoveChild(object childData)
        {
            DataGridRow child = _objectsToDisplay[childData];
            foreach (UIElement element in child._representationInRow.ElementsInRow)
            {
                _grid.Children.Remove(element);
            }
            _grid.RowDefinitions.Remove(child._representationInRow.RowDefinition);
            _objectsToDisplay.Remove(childData);
        }

        private void RemoveAllChildren()
        {
            foreach (DataGridRow child in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_objectsToDisplay)
#else
                _objectsToDisplay.Values
# endif
                )
            {
                foreach (UIElement element in child._representationInRow.ElementsInRow)
                {
                    _grid.Children.Remove(element);
                }
                _grid.RowDefinitions.Remove(child._representationInRow.RowDefinition);
            }
            _objectsToDisplay.Clear();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue == null)
            {
                //--------------
                // We enter here if the user is clearing the DataGrid by setting ItemsSource to null.
                //--------------

                // Refresh the DataGrid:
                base.OnItemsSourceChanged(oldValue, newValue);
            }
            else
            {
                //--------------
                // We enter here if the user is setting the ItemsSource of the DataGrid to a non-null value.
                //--------------

                // IMPORTANT: There are two passes here because this method is executed twice (it is re-entrant).

                if (ItemsSource != _pagedView)
                {
                    //--------------
                    // PASS 1
                    //--------------

                    // we remember the oldvalue because it is the one that will be used as the base to
                    // incrementally refresh the DataGrid during PASS 2. When we re-enter this method
                    // during PASS 2 (cf. notes below) the "oldValue" becomes the "newValue", so we use
                    // the "oldOldValue" to access the original oldValue.
                    _oldOldValue = oldValue;

                    // prepare the PagedView
                    _pagedView.SetIEnumerableAsCollectionSource(newValue);

                    // we override the ItemsSource specified by the user so as to used a PagedView as the source of the data.
                    //todo: with the current implementation, bindings are lost, so the user cannot replace the ItemsSource via a binding.
                    ItemsSource = _pagedView; // IMPORTANT: This causes re-entrance into this very same method, thus leading to PASS 2.
                }
                else
                {
                    //--------------
                    // PASS 2 (after re-entrance into this method, due to the code "ItemsSource = _pagedView" above)
                    //--------------

                    // Refresh the DataGrid:
                    base.OnItemsSourceChanged(_oldOldValue, newValue);
                }
            }
        }

        //todo: add the management of MyDataGrid.Items.Add(sth). Not urgent because it is not handled correctly in wpf
        //      a way to do this would be to override UpdateChildrenInVisualTree instead of OnItemsSourceChanged (I am not sure and am not working on this specific matter right now so I let it as a commentary)
        /// <summary>
        /// Prepares the DataGrid for the update of its display (removes the autogenerated Columns if needed and registers to the ItemsSource.CollectionChanged event.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            // Do something only if the ItemsSource has changed:
            if (newValue != oldValue || !_areItemsInVisualTree)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    Type oldType = null;
                    if (oldValue != null)
                    {
                        foreach (var oldItem in oldValue)
                        {
                            oldType = oldItem.GetType();
                            break;
                        }
                    }
                    Type newType = null;
                    if (newValue != null)
                    {
                        foreach (var newItem in newValue)
                        {
                            newType = newItem.GetType();
                            break;
                        }
                    }

                    if (oldType != newType)
                    {
                        if (AutoGeneratedColumns != null)
                        {
                            //We clear the Autogenerated columns (not sure if useful):
                            foreach (DataGridColumn column in AutoGeneratedColumns)
                            {
                                Columns.Remove(column);
                            }
                            AutoGeneratedColumns.Clear();
                        }
                        if (newType != null)
                        {
                            if (_grid != null)
                            {
                                UpdateUIStructure();
                            }
                            else
                            {
                                MakeUIStructure();
                            }
                        }
                    }
                }

                UpdateDataGridReference();
                //UpdateChildrenInVisualTree(oldValue, newValue);
            }
        }

        //todo: this method should be called when the user clicks on an element inside a row that is already selected (might have been done)
        //first version: it is called directly once the user clicks on a cell
        private void EnterCellEditionMode(DataGridCell cell)
        {
            if (!IsReadOnly && IsEnabled && !cell.Column.IsReadOnly)
            {
                object item = cell.Item;
                int cellColumn = Grid.GetColumn(cell);
                DataGridColumn column = Columns.ElementAt(cellColumn - 1); //todo: replace this by cell.Column ?
                //we add the editable version of the cell:
                UIElement editableVersionOfCell = column.GenerateEditingElement(item);
                if (editableVersionOfCell != null) //it is null when we use the same element as the one during non-edition time.
                {
                    //We add a DataGridCell to contain the generated element so we can apply the cellSyle on it:
                    DataGridCell editableVersionOfCellContainer = new DataGridCell();
                    editableVersionOfCellContainer.Column = column;
                    editableVersionOfCellContainer.IsSelected = true;
                    if (column.CellStyle != null)
                    {
                        editableVersionOfCellContainer.Style = column.CellStyle;
                    }
                    else if (column._parent.CellStyle != null)
                    {
                        editableVersionOfCellContainer.Style = column._parent.CellStyle;
                    }
                    editableVersionOfCellContainer.Content = editableVersionOfCell;
                    editableVersionOfCell = editableVersionOfCellContainer;



                    //We hide the element formerly displayed:
                    cell.Visibility = Visibility.Collapsed;

                    Grid.SetRow(editableVersionOfCell, Grid.GetRow(cell));
                    Grid.SetColumn(editableVersionOfCell, cellColumn);
                    editableVersionOfCell.LostFocus += CurrentEditionElement_LostFocus;

                    _grid.Children.Add(editableVersionOfCell);
                }
                else
                {
                    ((FrameworkElement)cell.Content).LostFocus -= CurrentEditionElement_LostFocus;
                    ((FrameworkElement)cell.Content).LostFocus += CurrentEditionElement_LostFocus;
                }
                _currentCell = cell;
                cell.IsEditing = true;
                //we keep a trace of the editable version of the cell so we can remove it later on:
                _currentEditionElement = editableVersionOfCell;
            }
        }


        void CurrentEditionElement_LostFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).LostFocus -= CurrentEditionElement_LostFocus;
            LeaveCellEditionMode();
        }

        private void LeaveCellEditionMode()
        {
            if (_currentCell != null)
            {
                if (!IsReadOnly && IsEnabled && !_currentCell.Column.IsReadOnly) //note: this test is particularly useful to avoid setting _currentCell.IsEditing to false, which sets its content.isEnabled to false (and thus disables HyperlinkButtons for example, or any Button for that matter...)
                {
                    //todo: Do we want to do this when IsEnabled is false? Test the case where a cell is in edition mode when we set IsEnabled to false on the DataGrid, then set it back to true (and variants).
                    object temp = _currentCell.DataContext;
                    _currentCell.DataContext = null;
                    _currentCell.DataContext = temp;
                    _currentCell.Visibility = Visibility.Visible;
                    if (!(_currentCell.Column is DataGridTemplateColumn))
                    {
                        _currentCell.IsEditing = false;
                    }

                    if (_currentEditionElement != null)
                    {
                        ((FrameworkElement)_currentEditionElement).DataContext = null; //Note: this is here because we had to set the DataContext locally for this element (since it is directly put in the grid and not in a DataGridCell)
                        _grid.Children.Remove(_currentEditionElement);
                    }
                }
                _currentCell = null;
                _currentEditionElement = null;
            }
        }

        private void GenerateColumns()
        {
            // 0. clear the possibly previously generated columns
            // 1. go through all the fields
            // 2. for each field, add the column that conrresponds to its type to the Columns collection, with the Field's name
            // 3. enjoy.
            if (AutoGeneratedColumns != null)
            {
                foreach (DataGridColumn column in AutoGeneratedColumns)
                {
                    Columns.Remove(column);
                }
                AutoGeneratedColumns.Clear();
            }
            if (Items != null || _itemsSourceAsSetByUser != null)
            {
                Type type = null;
                //we try to get the type of the items to display from the ItemsSource (better case scenario since it doesn't require the Collection to contain any element)
                if (_itemsSourceAsSetByUser != null)
                {
                    Type type1 = _itemsSourceAsSetByUser.GetType();

                    if (_itemsSourceAsSetByUser is PagedCollectionView)
                    {
                        type1 = ((PagedCollectionView)_itemsSourceAsSetByUser)._originalDataSourceWithoutCopy.GetType();
                    }

                    //BRIDGETODO : verify the code below matches
#if !BRIDGE
                    if (type1.GenericTypeArguments.Length > 0)
                    {
                        type = type1.GenericTypeArguments[0];
                    }
#else
                    if (type1.GetGenericArguments().Length > 0)
                    {
                        type = type1.GetGenericArguments()[0];
                    }
#endif
                }
                //if we didn't get the type from the ItemsSource, we try to get it from the first element of Items, if there is any
                if (type == null)
                {
                    if (Items != null && Items.Count > 0)
                    {
                        type = Items[0].GetType();
                    }
                }
                if (type != null)
                {
                    if (AutoGeneratedColumns == null)
                    {
                        AutoGeneratedColumns = new ObservableCollection<DataGridColumn>();
                    }
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        string propertyName = property.Name;
                        Type propertyType = property.PropertyType;
                        DataGridBoundColumn column;
                        Binding binding = new Binding(propertyName);

                        if (typeof(String).IsAssignableFrom(propertyType))
                        {
                            column = new DataGridTextColumn();
                        }
                        else if (typeof(bool).IsAssignableFrom(propertyType))
                        {
                            column = new DataGridCheckBoxColumn();
                        }
                        else if (typeof(Uri).IsAssignableFrom(propertyType))
                        {
                            column = new DataGridHyperlinkColumn();
                        }
                        else if (typeof(Enum).IsAssignableFrom(propertyType))
                        {
                            column = new DataGridComboBoxColumn();
                        }
                        else
                        {
                            column = new DataGridTextColumn(); //--> todo: check if the value is a ToString as expected.
                                                               //BRIDGETODO : default value see IsValueType in Bridge
#if !BRIDGE
                            column.IsReadOnly = !propertyType.IsValueType;
#else
                            column.IsReadOnly = false; //arbitrary value
#endif
                        }
                        column.IsAutoGenerated = true;
                        column.Header = propertyName;
                        column.SetBinding(DataGridBoundColumn.BindingProperty, binding);
                        //column.Binding = binding;
                        AutoGeneratedColumns.Add(column);
                        Columns.Add(column);
                    }
                }
            }
        }

        private void GenerateFakeColumnToTheEnd()
        {
            // Thsi method adds a fake "Star" column at the end if there is no "Star" column, so that the DataGrid takes the whole space.

            if (AutoGeneratedColumns == null)
                AutoGeneratedColumns = new ObservableCollection<DataGridColumn>();

            // Check if there is at least one column that has a Star width:
            bool atLeastOneColumnHasStarSize = false;
            foreach (DataGridColumn column in Columns)
            {
                if (column.Width.IsStar)
                    atLeastOneColumnHasStarSize = true;
            }

            // Add the fake "Star" column:
            if (!atLeastOneColumnHasStarSize)
            {
                var fakeColumn = new DataGridTextColumn()
                {
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                };
                fakeColumn.Header = "";
                Binding binding = new Binding("PROPERTY_THAT_DOES_NOT_EXIST"); // "PROPERTY_THAT_DOES_NOT_EXIST" is an arbitrary text to ensure that the binding is broken.
                fakeColumn.SetBinding(DataGridBoundColumn.BindingProperty, binding);

                AutoGeneratedColumns.Add(fakeColumn);
                Columns.Add(fakeColumn);
            }
        }

        protected override void UpdateChildrenInVisualTree(IEnumerable oldChildrenEnumerable, IEnumerable newChildrenEnumerable, bool forceUpdateAllChildren = false) // "forceUpdateAllChildren" is used to remove all the children and add them back, for example when the ItemsPanel changes.
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _renderedItemsPanel != null)
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_grid, this);
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_pagerUI, this);
                //we should not arrive in here from the user's code since the user cannot set items.

                List<object> oldChildren = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(oldChildrenEnumerable);
                List<object> newChildren = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(newChildrenEnumerable);

                //todo: test scenario: Add ItemsSource, change it for something that is not IEnumerable, change it for something that is IEnumerable

                //we empty the oldChildren Collection if the elements are already not displayed:
                if (!_areItemsInVisualTree && oldChildren != null)
                {
                    if (_objectsToDisplay != null)
                    {
                        _objectsToDisplay.Clear();
                    }
                    oldChildren.Clear();
                }

                if (forceUpdateAllChildren) //if we want to update all the children (for example because Columns changed), we need to remove the old versions of the children
                {
                    if (oldChildren != null)
                    {
                        foreach (object oldChild in oldChildren)
                        {
                            RemoveChild(oldChild);
                        }
                    }
                    if (newChildren != null)
                    {
                        foreach (object newChild in newChildren)
                        {
                            AddChild(newChild);
                        }
                    }
                }
                else
                {
                    int currentIndexInOldChildren = 0;
                    if (newChildren != null)
                    {
                        int currentIndexOfChild = 1; //we start at 1 instead of 0 because the Headers take the first Row.
                        foreach (object newChild in newChildren)
                        {
                            bool newChildSuccessfullyAdded = true;
                            if (oldChildren != null)
                            {
                                //we remove the old children that are not the current new child until we find it:
                                while (currentIndexInOldChildren < oldChildren.Count && newChild != oldChildren[currentIndexInOldChildren])
                                {
                                    RemoveChild(oldChildren[currentIndexInOldChildren]);
                                    ++currentIndexInOldChildren;
                                }
                            }
                            if (oldChildren != null && currentIndexInOldChildren < oldChildren.Count) //it means that newChild == oldChildren[currentIndexInOldChildren] since we got out of the while
                            {
                                //we need to update the value of the Grid.Row of the element:
                                DataGridRow currentDataGridRow = _objectsToDisplay[newChild];
                                currentDataGridRow.INTERNAL_SetRowIndex(currentIndexOfChild);
                                ++currentIndexInOldChildren; //we let the current oldChild since it is the new child
                            }
                            else //we need to add the new child (all the old children have been removed)
                            {
                                newChildSuccessfullyAdded = AddChild(newChild);
                            }

                            if (newChildSuccessfullyAdded)
                                ++currentIndexOfChild; //this allows us to update the row of the elements that stay in the visual tree (because there can be other items that were before them that have been removed)
                        }
                    }
                    //we remove the remaining old children:
                    if (oldChildren != null)
                    {
                        while (currentIndexInOldChildren < oldChildren.Count)
                        {
                            RemoveChild(oldChildren[currentIndexInOldChildren]);
                            ++currentIndexInOldChildren;
                        }
                    }
                }
                _areItemsInVisualTree = true;

                AttachGridToVisualTree();
            }
            else
                _areItemsInVisualTree = false;
        }

        void AttachGridToVisualTree()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(_grid))
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_grid, this);

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_pagerUI))
                    throw new Exception("The DataPager is already attached and cannot be attached twice.");

                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_pagerUI, this);
            }

            // Work around a vertical alignment issue due to the fact that we attach both the DataGrid and the DataPager (cf. ticket #1290):
            if (this.VerticalAlignment != VerticalAlignment.Top)
                this.VerticalAlignment = VerticalAlignment.Top;
        }

        protected override void UnselectAllItems()
        {
            foreach (DataGridRow row in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_objectsToDisplay)
#else
                _objectsToDisplay.Values
#endif
                )
            {
                row.IsSelected = false;
                row.VisuallyRefreshItemSelection(false);
            }
        }

        protected override void SetItemVisualSelectionState(object item, bool newState)
        {
            if (item != null && _objectsToDisplay.ContainsKey(item))
            {
                DataGridRow itemRow = _objectsToDisplay[item];
                itemRow.IsSelected = newState;
                itemRow.VisuallyRefreshItemSelection(newState);
                if (newState)
                {
                    SelectedItem = item;
                }
            }
        }

        internal void SelectItem(object item)
        {
            //todo: manage the event of selectionChanged ?
            if (SelectionMode == DataGridSelectionMode.Single)
            {
                //Note: no SelectedItems.Clear() because SelectedItems.Add causes the SelectItem(object) method to be called, which makes the clear which unselects the item then the Add changes the value of Selec...
                SelectedItems.Add(item);
            }
            else
            {
                if (!INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, item)) //todo: remove the cast once the bug with the IList elements undefined in javascript is fixed.
                {
                    SelectedItems.Add(item);//this will cause the method VisuallySelectItem to be called by MultiSelector.SelectedItems_CollectionChanged through RefreshSelectedItems
                }
            }
        }

        internal void UnselectItem(object item)
        {
            //todo: manage the event of selectionChanged ?
            if (SelectionMode == DataGridSelectionMode.Single)
            {
                INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Clear(SelectedItems); //todo: remove the cast once the bug with the IList elements undefined in javascript is fixed.
            }
            else
            {
                if (INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(SelectedItems, item)) //todo: remove the cast once the bug with the IList elements undefined in javascript is fixed.
                {
                    INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Remove(SelectedItems, item); //todo: remove the cast once the bug with the IList elements undefined in javascript is fixed.
                }
            }

        }

    }


}
