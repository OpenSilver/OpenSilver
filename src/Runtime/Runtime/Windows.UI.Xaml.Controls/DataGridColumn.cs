

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
#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a System.Windows.Controls.DataGrid column.
    /// </summary>
    public abstract partial class DataGridColumn : DependencyObject
    {
        private DataGridColumnHeader _header;
        internal ColumnDefinition _gridColumn;
        internal DataGrid _parent;

        protected DataGridColumn()
        {
            CanBeInheritanceContext = false;
            IsInheritanceContextSealed = true;
        }

        /// <summary>
        /// Gets or sets the content of the column header.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(DataGridColumn),
                new PropertyMetadata(null, OnHeaderChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            if (column._header != null)
            {
                column._header.Content = e.NewValue;
            }
        }

        /// <summary>
        /// This method allows to set the header's style without changing the DataGridColumn's 
        /// HeaderStyle Property. This way, we will still be able to set a new header via 
        /// DataGrid's ColumnHeaderStyle property.
        /// </summary>
        /// <param name="style">
        /// The new style to apply to the header
        /// </param>
        internal void SetHeaderStyleIfColumnsStyleNotSet(Style style)
        {
            if (this._header != null && this.HeaderStyle == null)
            {
                this._header.Style = style;
            }
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the column header.
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.HeaderStyle"/> dependency
        /// property.</summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(
                nameof(HeaderStyle),
                typeof(Style),
                typeof(DataGridColumn),
                new PropertyMetadata(null, OnHeaderStyleChanged));

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            if (column._header != null)
            {
                column._header.Style = (Style)e.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the template that defines the visual representation of the column
        /// header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.HeaderTemplate"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(DataGridColumn),
                new PropertyMetadata(null, OnHeaderTemplateChanged));

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            if (column._header != null)
            {
                column._header.ContentTemplate = (DataTemplate)e.NewValue;
            }
        }

        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.CellStyle"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty CellStyleProperty =
            DependencyProperty.Register(
                nameof(CellStyle),
                typeof(Style),
                typeof(DataGridColumn),
                new PropertyMetadata(null, OnCellStyleChanged));

        private static void OnCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            //todo: apply the style to all the cells of the column currently in the DataGrid
        }

        /// <summary>
        /// Gets a value that indicates whether the column is auto-generated.
        /// The default is <c>false</c>.
        /// </summary>
        public bool IsAutoGenerated
        {
            get { return (bool)GetValue(IsAutoGeneratedProperty); }
            internal set { SetValue(IsAutoGeneratedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.IsAutoGenerated"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsAutoGeneratedProperty =
            DependencyProperty.Register(
                nameof(IsAutoGenerated),
                typeof(bool),
                typeof(DataGridColumn),
                new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.IsAutoGenerated"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(DataGridColumn),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the column width or automatic sizing mode.
        /// </summary>
        public DataGridLength Width
        {
            get { return (DataGridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DataGridColumn.Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(DataGridLength),
                typeof(DataGridColumn),
                new PropertyMetadata(DataGridLength.Auto, OnWidthChanged));

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            if (column._gridColumn != null)
            {
                UpdateGridColumnWidth(column);
            }
        }

        internal static void UpdateGridColumnWidth(DataGridColumn column)
        {
            DataGridLength newValue = column.Width;
            if (column._parent != null)
            {
                GridUnitType newGridUnitType;
                switch (newValue.UnitType)
                {
                    case DataGridLengthUnitType.Auto:
                        newGridUnitType = GridUnitType.Auto;
                        break;
                    case DataGridLengthUnitType.Pixel:
                        newGridUnitType = GridUnitType.Pixel;
                        break;
                    case DataGridLengthUnitType.Star:
                        newGridUnitType = GridUnitType.Star;
                        break;
                    //case DataGridLengthUnitType.SizeToCells:
                    //    //todo: probably Auto
                    //    break;
                    //case DataGridLengthUnitType.SizeToHeader:
                    //    //todo: probably Auto
                    //    break;
                    default:
                        newGridUnitType = GridUnitType.Auto;
                        break;
                }
                column._gridColumn.Width = new GridLength(newValue.Value, newGridUnitType);
            }
        }

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(
                nameof(Visibility),
                typeof(Visibility),
                typeof(DataGridColumn),
                new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = (DataGridColumn)d;
            if (column._gridColumn != null)
            {
                column._gridColumn.Visibility = (Visibility)e.NewValue; //Note: this could set the localValue when it should be the VisualStateValue.
            }
        }

        /// <summary>
        ///     Retrieves the visual tree that was generated for a particular row and column.
        /// </summary>
        /// <param name="dataItem">The row that corresponds to the desired cell.</param>
        /// <returns>The element if found, null otherwise.</returns>
        public FrameworkElement GetCellContent(object dataItem)
        {
            if (_parent != null && dataItem != null)
            {
                DataGridRow row = _parent.ItemContainerGenerator.ContainerFromItem(dataItem) as DataGridRow;
                if (row != null)
                {
                    return GetCellContent(row);
                }
            }

            return null;
        }

        /// <summary>
        ///     Retrieves the visual tree that was generated for a particular row and column.
        /// </summary>
        /// <param name="dataGridRow">The row that corresponds to the desired cell.</param>
        /// <returns>The element if found, null otherwise.</returns>
        public FrameworkElement GetCellContent(DataGridRow dataGridRow)
        {
            if (_parent != null && dataGridRow != null)
            {
                int columnIndex = _parent.Columns.IndexOf(this);
                if (columnIndex >= 0)
                {
                    DataGridCell cell = dataGridRow.TryGetCell(columnIndex);
                    if (cell != null)
                    {
                        return cell.Content as FrameworkElement;
                    }
                }
            }

            return null;
        }

        private void Header_OnClick(object sender, RoutedEventArgs e)
        {
            //todo: sort
        }

        internal DataGridColumnHeader GetHeader()
        {
            DataGridColumnHeader header = new DataGridColumnHeader();
            header.Column = this;

            header.Content = Header;
            if (HeaderTemplate != null)
            {
                header.ContentTemplate = HeaderTemplate;
            }
            if (HeaderStyle != null)
            {
                header.Style = HeaderStyle;
            }
            else if (_parent.ColumnHeaderStyle != null)
            {
                header.Style = _parent.ColumnHeaderStyle;
            }

            header.Click += Header_OnClick;

            _header = header;
            return header;
        }

        /// <summary>
        /// When overridden in a derived class, gets an editing element that is bound
        /// to the <see cref="DataGridBoundColumn.Binding"/> property value
        /// of the column.
        /// </summary>
        /// <param name="childData">
        /// The data item that is represented by the row that contains the intended cell.
        /// </param>
        /// <returns>
        /// A new editing element that is bound to the <see cref="DataGridBoundColumn.Binding"/>
        /// property value of the column.
        /// </returns>
        internal abstract FrameworkElement GenerateEditingElement(object childData); //todo: the proper signature is: protected abstract FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem);

        /// <summary>
        /// When overridden in a derived class, gets a read-only element that is bound to the column's 
        /// <see cref="DataGridBoundColumn.Binding" /> property value.
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

        internal FrameworkElement GenerateElementInternal(DataGridCell cell, object dataItem)
        {
            return GenerateElement(cell, dataItem);
        }

        internal virtual void EnterEditionMode(DataGridCell dataGridCell) 
        {
        }

        internal virtual void LeaveEditionMode(DataGridCell dataGridCell)
        {
        }
    }
}
