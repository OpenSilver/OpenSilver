
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

using CSHTML5.Internal;
using System;
using System.Collections.Generic;

#if MIGRATION
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
#else
using Windows.UI;
using Windows.UI.Xaml.Input;
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
    /// Represents a <see cref="DataGrid"/> row.
    /// </summary>
    public partial class DataGridRow : Control
    {
        public DataGridRow()
        {
            CanBeInheritanceContext = false;
            IsInheritanceContextSealed = true;
        }

        internal UIElement _headerUIElement;
        internal ObjectRepresentationInRow _representationInRow = new ObjectRepresentationInRow();
        internal DataGrid _datagrid;
        internal int _rowIndex; //todo: replace this with the method GetIndex? (see wpf)
        private DataGridSelectionMode _currentSelectionMode;

        internal static DataTemplate DefaultTemplateForExtendedSelectionMode = CreateDefaultTemplateForExtendedSelectionMode();

        private static DataTemplate CreateDefaultTemplateForExtendedSelectionMode()
        {
            DataTemplate template = new DataTemplate();
            template.SetMethodToInstantiateFrameworkTemplate(GenerateDefaultHeaderTemplateForExtendedSelectionMode);
            return template;
        }

#if MIGRATION
        public event MouseButtonEventHandler MouseLeftButtonUp;
#else
        public event PointerEventHandler MouseLeftButtonUp;
#endif

        //this allows us to generate a header to allow selection (and most importantly deselection) of elements in the Grid.
        private static TemplateInstance GenerateDefaultHeaderTemplateForExtendedSelectionMode(FrameworkElement templateOwner = null) //The TemplateOwner parameter is made necessary by ControlTemplates but can be kept at null in DataTemplate.
        {
            TemplateInstance templateInstance = new TemplateInstance();
            templateInstance.TemplateOwner = templateOwner;
            Border border = new Border();
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Stretch;
            border.Background = new SolidColorBrush(Colors.Gray);
            border.TemplatedParent = templateOwner;
            CheckBox checkbox = new CheckBox();
            checkbox.HorizontalAlignment = HorizontalAlignment.Center;
            checkbox.VerticalAlignment = VerticalAlignment.Center;
            Binding b = new Binding("IsSelected");
            b.Mode = BindingMode.TwoWay;
            checkbox.SetBinding(CheckBox.IsCheckedProperty, b);
            checkbox.TemplatedParent = templateOwner;
            border.Child = checkbox;
            templateInstance.TemplateContent = border;
            return templateInstance;
        }

#if MIGRATION
        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#else
        public void OnMouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (MouseLeftButtonUp != null)
            {
#if MIGRATION
                MouseLeftButtonUp(this, new MouseButtonEventArgs());
#else
                MouseLeftButtonUp(this, new PointerRoutedEventArgs());
#endif
            }
        }

        //public object Header //todo: see what it should actually be.
        //{
        //    get { return (object)GetValue(HeaderProperty); }
        //    set { SetValue(HeaderProperty, value); }
        //}
        //public static readonly DependencyProperty HeaderProperty =
        //    DependencyProperty.Register("Header", typeof(object), typeof(DataGridRow), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate), 
                typeof(DataTemplate), 
                typeof(DataGridRow), 
                new PropertyMetadata(null, HeaderTemplate_Changed));

        private static void HeaderTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridRow dataGridRow = (DataGridRow)d;
            if (dataGridRow != null)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGridRow._datagrid))
                {
                    if (dataGridRow._headerUIElement != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGridRow._headerUIElement))
                    {
                        //we remove the header that was there before the new one:
                        dataGridRow._datagrid.RemoveElementFromGrid(dataGridRow._headerUIElement);
                    }
                    //we add the new header
                    DataTemplate template = (DataTemplate)e.NewValue;
                    if (template != null)
                    {
                        dataGridRow._headerUIElement = template.INTERNAL_InstantiateFrameworkTemplate();
                        Grid.SetRow(dataGridRow._headerUIElement, dataGridRow._rowIndex);
                        Grid.SetColumn(dataGridRow._headerUIElement, 0);
                        if (dataGridRow._headerUIElement is FrameworkElement)
                        {
                            ((FrameworkElement)dataGridRow._headerUIElement).DataContext = dataGridRow;
                        }
                        bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
                        if (isCSSGrid)
                        {
                            if (dataGridRow._headerUIElement is Control)
                            {
                                ((Control)dataGridRow._headerUIElement).BorderBrush = dataGridRow._datagrid.HorizontalGridLinesBrush;
                                ((Control)dataGridRow._headerUIElement).BorderThickness = new Thickness(1, 0, 1, 1);
                            }
                            else if (dataGridRow._headerUIElement is Border)
                            {
                                ((Border)dataGridRow._headerUIElement).BorderBrush = dataGridRow._datagrid.HorizontalGridLinesBrush;
                                ((Border)dataGridRow._headerUIElement).BorderThickness = new Thickness(1, 0, 1, 1);
                            }
                        }
                        dataGridRow._datagrid.AddElementToGrid(dataGridRow._headerUIElement);
                    }
                }
            }
        }

        internal void ChangeIntoSelectionMode(DataGridSelectionMode newSelectionMode)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_datagrid))
            {
                if (newSelectionMode == DataGridSelectionMode.Extended)
                {
                    if (_currentSelectionMode != DataGridSelectionMode.Extended)
                    {
                        _currentSelectionMode = DataGridSelectionMode.Extended;
                    }
                }
                else
                {
                    if (_headerUIElement != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(_headerUIElement))
                    {
                        _datagrid.RemoveElementFromGrid(_headerUIElement);
                    }
                }
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected), 
                typeof(bool), 
                typeof(DataGridRow), 
                new PropertyMetadata(false, IsSelected_Changed));

        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: throw event Selected or UnSelected
            DataGridRow row = (DataGridRow)d;

            if (row._representationInRow != null && row._representationInRow.ElementsInRow != null && row._representationInRow.ElementsInRow.Count > 0)
            {
                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    row._datagrid.SelectItem(row._representationInRow.ElementsInRow[0].Item);
                }
                else
                {
                    row._datagrid.UnselectItem(row._representationInRow.ElementsInRow[0].Item);
                }
            }
        }

        internal void INTERNAL_SetRowIndex(int rowIndex)
        {
            _rowIndex = rowIndex;
            if (_headerUIElement != null)
            {
                Grid.SetRow(_headerUIElement, rowIndex);
            }
            foreach (UIElement element in _representationInRow.ElementsInRow)
            {
                Grid.SetRow(element, rowIndex);
            }

            //NOTE: RowDefinition should not change since we remove the row itself from the grid when we remove an item from the list (therefore, the RowDefinition's index changes at the same time as the rest)
        }

        // the left click event in the cell must raise the MouseLeftButtonUp event of the DataGridRow
        internal void INTERNAL_SetRowEvents()
        {
            foreach (DataGridCell element in _representationInRow.ElementsInRow)
            {

#if MIGRATION
                element.MouseLeftButtonUp += OnMouseLeftButtonUp;
#else
                element.PointerReleased += OnMouseLeftButtonUp;
#endif
            }
        }

        internal partial class ObjectRepresentationInRow
        {
            internal List<DataGridCell> ElementsInRow = new List<DataGridCell>();
            internal RowDefinition RowDefinition;
        }

        internal void VisuallyRefreshItemSelection(bool newSelectionState)
        {
            foreach (DataGridCell currentCell in _representationInRow.ElementsInRow)
            {
                currentCell.IsSelected = newSelectionState;
            }
        }

        /// <summary>
        ///     attempts to get the cell at the given index.
        /// </summary>
        internal DataGridCell TryGetCell(int index)
        {
            if (index < _representationInRow.ElementsInRow.Count && index >= 0)
            {
                DataGridCell cell = _representationInRow.ElementsInRow[index];

                if (cell != null)
                    return cell;
            }

            return null;
        }

        #region DataContext

        // reuse the dataContext code from FrameworkElement
        internal void INTERNAL_SetDataContext()
        {
            if (_representationInRow.ElementsInRow.Count > 0)
                DataContext = _representationInRow.ElementsInRow[0].Item;
        }

        #endregion
    }
}