
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHTML5.Internal;
#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    //[TemplatePart(Name = "PART_LeftHeaderGripper", Type = typeof(Thumb))]
    //[TemplatePart(Name = "PART_RightHeaderGripper", Type = typeof(Thumb))]
    /// <summary>
    /// Represents an individual System.Windows.Controls.DataGrid column header.
    /// </summary>
    public class DataGridColumnHeader : ButtonBase//, IProvideDataGridColumn
    {
        

        // Summary:
        //     Initializes a new instance of the System.Windows.Controls.Primitives.DataGridColumnHeader
        //     class.
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.Primitives.DataGridColumnHeader
        /// class.
        /// </summary>
        public DataGridColumnHeader()
        {
            this.DefaultStyleKey = typeof(DataGridColumnHeader);
        }

        /// <summary>
        /// Gets the System.Windows.Controls.DataGridColumn associated with this column
        /// header.
        /// </summary>
        public DataGridColumn Column { get; internal set; }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic div = base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
            dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            style.boxSizing = "border-box";
            //Why do we need the box-sizing here:
            // the cell for this has width: 100% and height: 100%
            // BUT it doesn't naturally include the border-width (at least in Firefox)
            // it causes the div to be bigger than its container, which takes its size from its children.
            // Then most clicks on the DataGrid (and maybe other things) cause the container to look at the size of its content and thus becomes bigger to fit the size of this div, which then becomes bigger with it because of its 100%.
            return div;
        }

#region not supported stuff
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.CanUserSort
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.CanUserSort
        ////     dependency property.
        //public static readonly DependencyProperty CanUserSortProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.DisplayIndex
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.DisplayIndex
        ////     dependency property.
        //public static readonly DependencyProperty DisplayIndexProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.IsFrozen
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.IsFrozen
        ////     dependency property.
        //public static readonly DependencyProperty IsFrozenProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorBrush
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorBrush
        ////     dependency property.
        //public static readonly DependencyProperty SeparatorBrushProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorVisibility
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorVisibility
        ////     dependency property.
        //public static readonly DependencyProperty SeparatorVisibilityProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Controls.Primitives.DataGridColumnHeader.SortDirection
        ////     dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Controls.Primitives.DataGridColumnHeader.SortDirection
        ////     dependency property.
        //public static readonly DependencyProperty SortDirectionProperty;

        //// Summary:
        ////     Gets a value that indicates whether the user can click this column header
        ////     to sort the System.Windows.Controls.DataGrid by the associated column.
        ////
        //// Returns:
        ////     true if the user can click this column header to sort the System.Windows.Controls.DataGrid
        ////     by the associated column; otherwise, false.
        //public bool CanUserSort { get; }
        ////
        //// Summary:
        ////     Gets the key that references the style for displaying column headers during
        ////     a header drag operation.
        ////
        //// Returns:
        ////     The style key for floating column headers.
        //public static ComponentResourceKey ColumnFloatingHeaderStyleKey { get; }
        ////
        //// Summary:
        ////     Gets the key that references the style for the drop location indicator during
        ////     a header drag operation.
        ////
        //// Returns:
        ////     The style key for the drop location indicator.
        //public static ComponentResourceKey ColumnHeaderDropSeparatorStyleKey { get; }
        ////
        //// Summary:
        ////     Gets the display position of the column associated with this column header
        ////     relative to the other columns in the System.Windows.Controls.DataGrid.
        ////
        //// Returns:
        ////     The display position of associated column relative to the other columns in
        ////     the System.Windows.Controls.DataGrid.
        //public int DisplayIndex { get; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether the column associated with this column
        ////     header is prevented from scrolling horizontally.
        ////
        //// Returns:
        ////     true if the associated column is prevented from scrolling horizontally; otherwise,
        ////     false.
        //public bool IsFrozen { get; }
        ////
        //// Summary:
        ////     Gets or sets the System.Windows.Media.Brush used to paint the column header
        ////     separator lines.
        ////
        //// Returns:
        ////     The brush used to paint column header separator lines.
        //public Brush SeparatorBrush { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the user interface (UI) visibility of the column header separator
        ////     lines.
        ////
        //// Returns:
        ////     The UI visibility of the column header separator lines. The default is System.Windows.Visibility.Visible.
        //public Visibility SeparatorVisibility { get; set; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether the System.Windows.Controls.DataGrid
        ////     is sorted by the associated column and whether the column values are in ascending
        ////     or descending order.
        ////
        //// Returns:
        ////     The sort direction of the column or null if unsorted.
        //public System.ComponentModel.ListSortDirection? SortDirection { get; }

        //// Summary:
        ////     Builds the visual tree for the column header when a new template is applied.
        //public override void OnApplyTemplate();
        ////
        //// Summary:
        ////     Raises the System.Windows.Controls.Primitives.ButtonBase.Click event and
        ////     initiates sorting.
        //protected override void OnClick();
        ////
        //// Summary:
        ////     Returns a new System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer
        ////     for this column header.
        ////
        //// Returns:
        ////     A new automation peer for this column header.
        //protected override AutomationPeer OnCreateAutomationPeer();
        ////
        ////
        //// Parameters:
        ////   e:
        ////     The event data for the System.Windows.Input.Mouse.LostMouseCapture event.
        //protected override void OnLostMouseCapture(MouseEventArgs e);
        ////
        ////
        //// Parameters:
        ////   e:
        ////     The event data.
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e);
        ////
        ////
        //// Parameters:
        ////   e:
        ////     The event data.
        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e);
        ////
        ////
        //// Parameters:
        ////   e:
        ////     The event data.
        //protected override void OnMouseMove(MouseEventArgs e);
#endregion
    }
}