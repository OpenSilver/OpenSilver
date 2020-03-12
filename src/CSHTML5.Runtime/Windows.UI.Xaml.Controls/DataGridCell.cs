

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a cell of a System.Windows.Controls.DataGrid control.
    /// </summary>
    public partial class DataGridCell : ButtonBase
    {
        //For some reason in the wpf comments, below was "Gets or sets the column that the cell is in." although the set part is internal.
        /// <summary>
        /// Gets the column that the cell is in.
        /// </summary>
        public DataGridColumn Column
        {
            get { return (DataGridColumn)GetValue(ColumnProperty); }
            internal set { SetValue(ColumnProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridCell.Column dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(DataGridColumn), typeof(DataGridCell), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is in edit mode.
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridCell.IsEditing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(DataGridCell), new PropertyMetadata(false, IsEditing_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void IsEditing_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridCell cell = (DataGridCell)d;
            ((FrameworkElement)cell.Content).IsEnabled = (bool)e.NewValue;
            cell.ManageIsEditingChange(e);
        }

        private void ManageIsEditingChange(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Column.EnterEditionMode(this);
            }
            else
            {
                Column.LeaveEditionMode(this);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridCell.IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataGridCell), new PropertyMetadata(false, IsSelected_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: call event accordingly to the new Value + set the Background/Foreground
            DataGridCell dataGridCell = (DataGridCell)d;

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(dataGridCell))
            {
                bool newValue = (bool)e.NewValue;
                    if (newValue)
                    {
                        dataGridCell.Background = dataGridCell.Column._parent.SelectedItemBackground;
                        dataGridCell.Foreground = dataGridCell.Column._parent.SelectedItemForeground;
                        //todo: event. (inside or outside of this if?)
                    }
                    else
                    {
                        int RowIndex = Grid.GetRow(dataGridCell);

                        if (RowIndex % 2 == 0 && RowIndex != 0) // gray color
                        {
                            dataGridCell.Background = dataGridCell.Column._parent.AlternatingRowBackground ?? dataGridCell.Column._parent.Background;
                        }
                        else // white color
                        {
                            dataGridCell.Background = dataGridCell.Column._parent.RowBackground ?? dataGridCell.Column._parent.Background;
                        }
                        dataGridCell.Foreground = dataGridCell.Column._parent.UnselectedItemForeground ?? dataGridCell.Column._parent.Foreground;

                        //todo: event. (inside or outside of this if?)
                    }
            }
        }

        // if the content of the cell is clickable, it will mark this cell as selected
        internal void OnSelectedFromContent(object sender, RoutedEventArgs e)
        {
            Column._parent.SelectedItem = this.Item;
        }

#if MIGRATION
        public void OnClickOnContent(object sender, MouseButtonEventArgs e)
#else
        public void OnClickOnContent(object sender, PointerRoutedEventArgs e)
#endif
        {
            OnClick();
        }

        // this method will be called just after the creation of the content, so that we can register the "PointerReleased" event before any other registerers, thus ensuring that the event handler is called before the other event handlers. This order of calls is important because we set the selected item, which allows the user to know - when processing the click - which cell is concerned.
        internal void RegisterToContentPressEvent()
        {
            if (Content != null)
            {
                if (Content is Control)
                {
                    Control control = (Control)Content;

#if MIGRATION
                    control.MouseLeftButtonUp += OnClickOnContent;
#else
                    control.PointerReleased += OnClickOnContent;
#endif
                }
            }
        }

        internal object Item { get; set; } //we keep that so that we can find the item's row and column when it is clicked


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic div = base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
            dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            style.overflow = "hidden";
            style.boxSizing = "border-box";
            //Why do we need the box-sizing here:
            // the cell for this has width: 100% and height: 100%
            // BUT it doesn't naturally include the border-width (at least in Firefox)
            // it causes the div to be bigger than its container, which takes its size from its children.
            // Then most clicks on the DataGrid (and maybe other things) cause the container to look at the size of its content and thus becomes bigger to fit the size of this div, which then becomes bigger with it because of its 100%.
            return div;
        }
    }
}
