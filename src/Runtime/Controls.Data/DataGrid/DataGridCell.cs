// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;#endif
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents an individual <see cref="T:System.Windows.Controls.DataGrid" /> cell.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = DATAGRIDCELL_elementRightGridLine, Type = typeof(Rectangle))]

    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateRegular, GroupName = VisualStates.GroupCurrent)]
    [TemplateVisualState(Name = VisualStates.StateCurrent, GroupName = VisualStates.GroupCurrent)]
    [TemplateVisualState(Name = VisualStates.StateDisplay, GroupName = VisualStates.GroupInteraction)]
    [TemplateVisualState(Name = VisualStates.StateEditing, GroupName = VisualStates.GroupInteraction)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    public sealed partial class DataGridCell : ContentControl
    {
#region Constants

        private const string DATAGRIDCELL_elementRightGridLine = "RightGridLine";

#endregion Constants

#region Data

        private Rectangle _rightGridLine;

#endregion Data

        public DataGridCell()
        {
            CustomLayout = true;
            this.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(DataGridCell_MouseLeftButtonDown), true);
            this.MouseEnter += new MouseEventHandler(DataGridCell_MouseEnter);
            this.MouseLeave += new MouseEventHandler(DataGridCell_MouseLeave);

            DefaultStyleKey = typeof(DataGridCell);
        }

#region Dependency Properties

#region IsValid
        /// <summary>
        /// Gets a value that indicates whether the data in a cell is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }
            internal set
            {
                this.SetValueNoCallback(IsValidProperty, value);
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGridCell),
                new PropertyMetadata(true, (OnIsValidPropertyChanged)));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridCell that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridCell dataGridCell = (DataGridCell)d;
            if (!dataGridCell.AreHandlersSuspended())
            {
                dataGridCell.SetValueNoCallback(DataGridCell.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }
#endregion IsValid

#endregion Dependency Properties

#region Public Properties

#endregion Public Properties


#region Protected Properties

#endregion Protected Properties


#region Internal Properties

        internal double ActualRightGridLineWidth
        {
            get
            {
                if (_rightGridLine != null)
                {
                    return _rightGridLine.ActualWidth;
                }
                return 0;
            }
        }

        internal int ColumnIndex
        {
            get
            {
                if (this.OwningColumn == null)
                {
                    return -1;
                }
                return this.OwningColumn.Index;
            }
        }

        internal bool IsCurrent
        {
            get
            {
                Debug.Assert(this.OwningGrid != null && this.OwningColumn != null && this.OwningRow != null);
                return this.OwningGrid.CurrentColumnIndex == this.OwningColumn.Index &&
                       this.OwningGrid.CurrentSlot == this.OwningRow.Slot;
            }
        }

        internal DataGridColumn OwningColumn
        {
            get;
            set;
        }

        internal DataGrid OwningGrid
        {
            get
            {
                if (this.OwningRow != null && this.OwningRow.OwningGrid != null)
                {
                    return this.OwningRow.OwningGrid;
                }
                if (this.OwningColumn != null)
                {
                    return this.OwningColumn.OwningGrid;
                }
                return null;
            }
        }

        internal DataGridRow OwningRow
        {
            get;
            set;
        }

        internal int RowIndex
        {
            get
            {
                if (this.OwningRow == null)
                {
                    return -1;
                }
                return this.OwningRow.Index;
            }
        }

#endregion Internal Properties


#region Private Properties

        private bool IsEdited
        {
            get
            {
                Debug.Assert(this.OwningGrid != null);
                return this.OwningGrid.EditingRow == this.OwningRow &&
                       this.OwningGrid.EditingColumnIndex == this.ColumnIndex;
            }
        }

        private bool IsMouseOver
        {
            get
            {
                return this.OwningRow != null && this.OwningRow.MouseOverColumnIndex == this.ColumnIndex;
            }
            set
            {
                Debug.Assert(this.OwningRow != null);
                if (value != this.IsMouseOver)
                {
                    if (value)
                    {
                        this.OwningRow.MouseOverColumnIndex = this.ColumnIndex;
                    }
                    else
                    {
                        this.OwningRow.MouseOverColumnIndex = null;
                    }
                }
            }
        }

#endregion Private Properties


#region Public Methods

#endregion Public Methods


#region Protected Methods

        /// <summary>
        /// Builds the visual tree for the cell control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyCellState(false /*animate*/);
            
            this._rightGridLine = GetTemplateChild(DATAGRIDCELL_elementRightGridLine) as Rectangle;
            if (_rightGridLine != null && this.OwningColumn == null)
            {
                // Turn off the right GridLine for filler cells
                _rightGridLine.Visibility = Visibility.Collapsed;
            }
            else
            {
                EnsureGridLine(null);
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            if (this.OwningGrid != null &&
                this.OwningColumn != null &&
                this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
            {
                return new DataGridCellAutomationPeer(this);
            }
            return base.OnCreateAutomationPeer();
        }

#endregion Protected Methods

        
#region Internal Methods

        internal void ApplyCellState(bool animate)
        {
            if (this.OwningGrid == null || this.OwningColumn == null || this.OwningRow == null || this.OwningRow.Visibility == Visibility.Collapsed || this.OwningRow.Slot == -1)
            {
                return;
            }

            // CommonStates
            if (this.IsMouseOver)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateNormal);
            }

            // SelectionStates
            if (this.OwningRow.IsSelected)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateUnselected);
            }

            // FocusStates
            if (this.OwningGrid.ContainsFocus)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateUnfocused);
            }

            // CurrentStates
            if (this.IsCurrent)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateCurrent, VisualStates.StateRegular);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateRegular);
            }

            // Interaction states
            if (this.IsEdited)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateEditing, VisualStates.StateDisplay);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateDisplay);
            }

            // Validation states
            if (this.IsValid)
            {
                VisualStates.GoToState(this, animate, VisualStates.StateValid);
            }
            else
            {
                VisualStates.GoToState(this, animate, VisualStates.StateInvalid, VisualStates.StateValid);
            }
        }

        /// <summary>
        /// Ensures that the correct Style is applied to this object.
        /// </summary>
        /// <param name="previousStyle">Caller's previous associated Style</param>
        internal void EnsureStyle(Style previousStyle)
        {
            if (this.Style != null
                && (this.OwningColumn == null || this.Style != this.OwningColumn.CellStyle)
                && (this.OwningGrid == null || this.Style != this.OwningGrid.CellStyle)
                && (this.Style != previousStyle))
            {
                return;
            }

            Style style = null;
            if (this.OwningColumn != null)
            {
                style = this.OwningColumn.CellStyle;
            }
            if (style == null && this.OwningGrid != null)
            {
                style = this.OwningGrid.CellStyle;
            }
            this.SetStyleWithType(style);
        }

        // Makes sure the right gridline has the proper stroke and visibility. If lastVisibleColumn is specified, the 
        // right gridline will be collapsed if this cell belongs to the lastVisibileColumn and there is no filler column
        internal void EnsureGridLine(DataGridColumn lastVisibleColumn)
        {
            if (this.OwningGrid != null && _rightGridLine != null)
            {
                if (this.OwningGrid.VerticalGridLinesBrush != null && this.OwningGrid.VerticalGridLinesBrush != _rightGridLine.Fill)
                {
                    _rightGridLine.Fill = this.OwningGrid.VerticalGridLinesBrush;
                }

                Visibility newVisibility = (this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || this.OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All) &&
                    (this.OwningGrid.ColumnsInternal.FillerColumn.IsActive || this.OwningColumn != lastVisibleColumn) 
                    ? Visibility.Visible : Visibility.Collapsed;

                if (newVisibility != _rightGridLine.Visibility)
                {
                    _rightGridLine.Visibility = newVisibility;
                }
            }
        }

#endregion Internal Methods


#region Private Methods

        private void DataGridCell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.OwningRow != null)
            {
                this.IsMouseOver = true;
            }
        }

        private void DataGridCell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.OwningRow != null)
            {
                this.IsMouseOver = false;
            }
        }

        private void DataGridCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // OwningGrid is null for TopLeftHeaderCell and TopRightHeaderCell because they have no OwningRow
            if (this.OwningGrid != null)
            {
                if (!e.Handled && this.OwningGrid.IsTabStop)
                {
                    bool success = this.OwningGrid.Focus();
                    Debug.Assert(success);
                }
                if (this.OwningRow != null)
                {
                    Debug.Assert(sender is DataGridCell);
                    Debug.Assert(sender == this);
                    e.Handled = this.OwningGrid.UpdateStateOnMouseLeftButtonDown(e, this.ColumnIndex, this.OwningRow.Slot, !e.Handled);
                    this.OwningGrid.UpdatedStateOnMouseLeftButtonDown = true;
                }
            }
        }

#endregion Private Methods
    }
}
