// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;


#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class DataGrid
    {
#region Internal Properties

        internal bool AreRowBottomGridLinesRequired
        {
            get
            {
                return (this.GridLinesVisibility == DataGridGridLinesVisibility.Horizontal || this.GridLinesVisibility == DataGridGridLinesVisibility.All) && this.HorizontalGridLinesBrush != null;
            }
        }

        internal int FirstVisibleSlot
        {
            get
            {
                return (this.SlotCount > 0) ? GetNextVisibleSlot(-1) : -1;
            }
        }

        internal int FrozenColumnCountWithFiller
        {
            get
            {
                int count = this.FrozenColumnCount;
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && (this.AreRowGroupHeadersFrozen || count > 0))
                {
                    // Either the RowGroupHeaders are frozen by default or the user set a frozen column count.  In both cases, we need to freeze
                    // one more column than the what the public value says
                    count++;
                }
                return count;
            }
        }

        internal int LastVisibleSlot
        {
            get
            {
                return (this.SlotCount > 0) ? this.GetPreviousVisibleSlot(this.SlotCount) : -1;
            }
        }

#endregion Internal Properties

#region Private Properties

        // Cumulated height of all known rows, including the gridlines and details section.
        // This property returns an approximation of the actual total row heights and also
        // updates the RowHeightEstimate
        private double EdgedRowsHeightCalculated
        {
            get
            {
                // If we're not displaying any rows or if we have infinite space the, relative height of our rows is 0
                if (this.DisplayData.LastScrollingSlot == -1 || double.IsPositiveInfinity(this.AvailableSlotElementRoom))
                {
                    return 0;
                }
                Debug.Assert(this.DisplayData.LastScrollingSlot >= 0);
                Debug.Assert(_verticalOffset >= 0);
                Debug.Assert(this.NegVerticalOffset >= 0);

                // Height of all rows above the viewport
                double totalRowsHeight = _verticalOffset - this.NegVerticalOffset;

                // Add the height of all the rows currently displayed, AvailableRowRoom
                // is not always up to date enough for this
                foreach (UIElement element in this.DisplayData.GetScrollingElements())
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        totalRowsHeight += row.TargetHeight;
                    }
                    else
                    {
                        totalRowsHeight += element.DesiredSize.Height;
                    }
                }

                // Details up to and including viewport
                int detailsCount = GetDetailsCountInclusive(0, this.DisplayData.LastScrollingSlot);

                // Subtract details that were accounted for from the totalRowsHeight
                totalRowsHeight -= detailsCount * this.RowDetailsHeightEstimate;

                // Update the RowHeightEstimate if we have more row information
                if (this.DisplayData.LastScrollingSlot >= _lastEstimatedRow)
                {
                    _lastEstimatedRow = this.DisplayData.LastScrollingSlot;
                    this.RowHeightEstimate = totalRowsHeight / (_lastEstimatedRow + 1 - _collapsedSlotsTable.GetIndexCount(0, _lastEstimatedRow));
                }

                // Calculate estimates for what's beyond the viewport
                if (this.VisibleSlotCount > this.DisplayData.NumDisplayedScrollingElements)
                {
                    int remainingRowCount = (this.SlotCount - this.DisplayData.LastScrollingSlot - _collapsedSlotsTable.GetIndexCount(this.DisplayData.LastScrollingSlot, this.SlotCount - 1) - 1);

                    // Add estimation for the cell heights of all rows beyond our viewport
                    totalRowsHeight += this.RowHeightEstimate * remainingRowCount;

                    // Add the rest of the details beyond the viewport
                    detailsCount += GetDetailsCountInclusive(this.DisplayData.LastScrollingSlot + 1, this.SlotCount - 1);
                }

                // 
                double totalDetailsHeight = detailsCount * this.RowDetailsHeightEstimate;

                return totalRowsHeight + totalDetailsHeight;
            }
        }

#endregion Private Properties

#region Public Methods

        /// <summary>
        /// Collapses the DataGridRowGroupHeader that represents a given CollectionViewGroup
        /// </summary>
        /// <param name="collectionViewGroup">CollectionViewGroup</param>
        /// <param name="collapseAllSubgroups">Set to true to collapse all Subgroups</param>
        public void CollapseRowGroup(CollectionViewGroup collectionViewGroup, bool collapseAllSubgroups)
        {
            if (this.WaitForLostFocus(delegate { this.CollapseRowGroup(collectionViewGroup, collapseAllSubgroups); }) ||
                collectionViewGroup == null || !this.CommitEdit())
            {
                return;
            }

            EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), Visibility.Collapsed, true);

            if (collapseAllSubgroups)
            {
                foreach (object groupObj in collectionViewGroup.Items)
                {
                    CollectionViewGroup subGroup = groupObj as CollectionViewGroup;
                    if (subGroup != null)
                    {
                        CollapseRowGroup(subGroup, collapseAllSubgroups);
                    }
                }
            }
        }

        /// <summary>
        /// Expands the DataGridRowGroupHeader that represents a given CollectionViewGroup
        /// </summary>
        /// <param name="collectionViewGroup">CollectionViewGroup</param>
        /// <param name="expandAllSubgroups">Set to true to expand all Subgroups</param>
        public void ExpandRowGroup(CollectionViewGroup collectionViewGroup, bool expandAllSubgroups)
        {
            if (this.WaitForLostFocus(delegate { this.ExpandRowGroup(collectionViewGroup, expandAllSubgroups); }) ||
                collectionViewGroup == null || !this.CommitEdit())
                if (collectionViewGroup == null || !this.CommitEdit())
            {
                return;
            }

            EnsureRowGroupVisibility(RowGroupInfoFromCollectionViewGroup(collectionViewGroup), Visibility.Visible, true);

            if (expandAllSubgroups)
            {
                foreach (object groupObj in collectionViewGroup.Items)
                {
                    CollectionViewGroup subGroup = groupObj as CollectionViewGroup;
                    if (subGroup != null)
                    {
                        ExpandRowGroup(subGroup, expandAllSubgroups);
                    }
                }
            }
        }

        // 






















#endregion Public Methods

#region Protected Methods

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.DataGrid.RowDetailsVisibilityChanged" /> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnRowDetailsVisibilityChanged(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.RowDetailsVisibilityChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

#endregion Protected Methods

#region Internal Methods

        /// <summary>
        /// Clears the entire selection. Displayed rows are deselected explicitly to visualize
        /// potential transition effects
        /// </summary>
        internal void ClearRowSelection(bool resetAnchorSlot)
        {
            if (resetAnchorSlot)
            {
                this.AnchorSlot = -1;
            }
            if (_selectedItems.Count > 0)
            {
                this._noSelectionChangeCount++;
                try
                {
                    // Individually deselecting displayed rows to view potential transitions
                    for (int slot = this.DisplayData.FirstScrollingSlot;
                         slot > -1 && slot <= this.DisplayData.LastScrollingSlot;
                         slot++)
                    {
                        DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                        if (row != null)
                        {
                            if (_selectedItems.ContainsSlot(row.Slot))
                            {
                                SelectSlot(row.Slot, false);
                            }
                        }
                    }
                    _selectedItems.ClearRows();
                    this.SelectionHasChanged = true;
                }
                finally
                {
                    this.NoSelectionChangeCount--;
                }
            }
        }

        /// <summary>
        /// Clears the entire selection except the indicated row. Displayed rows are deselected explicitly to 
        /// visualize potential transition effects. The row indicated is selected if it is not already.
        /// </summary>
        internal void ClearRowSelection(int slotException, bool setAnchorSlot)
        {
            this._noSelectionChangeCount++;
            try
            {
                bool exceptionAlreadySelected = false;
                if (_selectedItems.Count > 0)
                {
                    // Individually deselecting displayed rows to view potential transitions
                    for (int slot = this.DisplayData.FirstScrollingSlot;
                         slot > -1 && slot <= this.DisplayData.LastScrollingSlot;
                         slot++)
                    {
                        if (slot != slotException && _selectedItems.ContainsSlot(slot))
                        {
                            SelectSlot(slot, false);
                            this.SelectionHasChanged = true;
                        }
                    }
                    exceptionAlreadySelected = _selectedItems.ContainsSlot(slotException);
                    int selectedCount = _selectedItems.Count;
                    if (selectedCount > 0)
                    {
                        if (selectedCount > 1)
                        {
                            this.SelectionHasChanged = true;
                        }
                        else
                        {
                            int currentlySelectedSlot = _selectedItems.GetIndexes().First();
                            if (currentlySelectedSlot != slotException)
                            {
                                this.SelectionHasChanged = true;
                            }
                        }
                        _selectedItems.ClearRows();
                    }
                }
                if (exceptionAlreadySelected)
                {
                    // Exception row was already selected. It just needs to be marked as selected again.
                    // No transition involved.
                    _selectedItems.SelectSlot(slotException, true /*select*/);
                    if (setAnchorSlot)
                    {
                        this.AnchorSlot = slotException;
                    }
                }
                else
                {
                    // Exception row was not selected. It needs to be selected with potential transition
                    SetRowSelection(slotException, true /*isSelected*/, setAnchorSlot);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        internal int GetCollapsedSlotCount(int startSlot, int endSlot)
        {
            return _collapsedSlotsTable.GetIndexCount(startSlot, endSlot);
        }

        internal int GetNextVisibleSlot(int slot)
        {
            return _collapsedSlotsTable.GetNextGap(slot);
        }

        internal int GetPreviousVisibleSlot(int slot)
        {
            return _collapsedSlotsTable.GetPreviousGap(slot);
        }

        internal Visibility GetRowDetailsVisibility(int rowIndex)
        {
            return GetRowDetailsVisibility(rowIndex, this.RowDetailsVisibilityMode);   
        }

        internal Visibility GetRowDetailsVisibility(int rowIndex, DataGridRowDetailsVisibilityMode gridLevelRowDetailsVisibility)
        {
            Debug.Assert(rowIndex != -1);
            if (_showDetailsTable.Contains(rowIndex))
            {
                // The user explicity set DetailsVisibility on a row so we should respect that
                return _showDetailsTable.GetValueAt(rowIndex);
            }
            else
            {
                if (gridLevelRowDetailsVisibility == DataGridRowDetailsVisibilityMode.Visible ||
                    (gridLevelRowDetailsVisibility == DataGridRowDetailsVisibilityMode.VisibleWhenSelected &&
                     _selectedItems.ContainsSlot(SlotFromRowIndex(rowIndex))))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Returns the row associated to the provided backend data item.
        /// </summary>
        /// <param name="dataItem">backend data item</param>
        /// <returns>null if the DataSource is null, the provided item in not in the source, or the item is not displayed; otherwise, the associated Row</returns>
        internal DataGridRow GetRowFromItem(object dataItem)
        {
            int rowIndex = this.DataConnection.IndexOf(dataItem);
            if (rowIndex < 0)
            {
                return null;
            }
            int slot = SlotFromRowIndex(rowIndex);
            return IsSlotVisible(slot) ? this.DisplayData.GetDisplayedElement(slot) as DataGridRow : null;
        }

        internal bool GetRowSelection(int slot)
        {
            Debug.Assert(slot != -1);
            return _selectedItems.ContainsSlot(slot);
        }

        internal void InsertElementAt(int slot, int rowIndex, object item, DataGridRowGroupInfo groupInfo, bool isCollapsed)
        {
            Debug.Assert(slot >= 0 && slot <= this.SlotCount);

            bool isRow = rowIndex != -1;
            if (isCollapsed)
            {
                InsertElement(slot, null /*element*/, true /*updateVerticalScrollBarOnly*/, true /*isCollapsed*/, isRow);
            }
            else if (SlotIsDisplayed(slot))
            {
                // Row at that index needs to be displayed
                if (isRow)
                {
                    InsertElement(slot, GenerateRow(rowIndex, slot, item), false /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
                }
                else
                {
                    InsertElement(slot, GenerateRowGroupHeader(slot, groupInfo), false /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
                }
            }
            else
            {
                InsertElement(slot, null, this._vScrollBar == null || this._vScrollBar.Visibility == Visibility.Visible /*updateVerticalScrollBarOnly*/, false /*isCollapsed*/, isRow);
            }
        }

        internal void InsertRowAt(int rowIndex)
        {
            int slot = SlotFromRowIndex(rowIndex);
            object item = this.DataConnection.GetDataItem(rowIndex);
            // isCollapsed below is always false because we only use the method if we're not grouping
            InsertElementAt(slot, rowIndex, item, null/*DataGridRowGroupInfo*/, false /*isCollapsed*/);
        }

        internal bool IsColumnDisplayed(int columnIndex)
        {
            return columnIndex >= this.FirstDisplayedNonFillerColumnIndex && columnIndex <= this.DisplayData.LastTotallyDisplayedScrollingCol;
        }

        internal bool IsRowRecyclable(DataGridRow row)
        {
            return (row != this.EditingRow && row != this._focusedRow);
        }

        internal bool IsSlotVisible(int slot)
        {
            return slot >= this.DisplayData.FirstScrollingSlot
               && slot <= this.DisplayData.LastScrollingSlot
               && slot != -1
               && !_collapsedSlotsTable.Contains(slot);
        }

        // detailsElement is the FrameworkElement created by the DetailsTemplate
        internal void OnUnloadingRowDetails(DataGridRow row, FrameworkElement detailsElement)
        {
            OnUnloadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
        }

        // detailsElement is the FrameworkElement created by the DetailsTemplate
        internal void OnLoadingRowDetails(DataGridRow row, FrameworkElement detailsElement)
        {
            OnLoadingRowDetails(new DataGridRowDetailsEventArgs(row, detailsElement));
        }

        internal void OnRowDetailsVisibilityPropertyChanged(int rowIndex, Visibility visibility)
        {
            Debug.Assert(rowIndex >= 0 && rowIndex < this.SlotCount);

            _showDetailsTable.AddValue(rowIndex, visibility);
        }

        internal void OnRowGroupHeaderToggled(DataGridRowGroupHeader groupHeader, Visibility newVisibility, bool setCurrent)
        {
            Debug.Assert(groupHeader.RowGroupInfo.CollectionViewGroup.ItemCount > 0);

            if (this.WaitForLostFocus(delegate { this.OnRowGroupHeaderToggled(groupHeader, newVisibility, setCurrent); }) || !this.CommitEdit())
            {
                return;
            }

            if (setCurrent && this.CurrentSlot != groupHeader.RowGroupInfo.Slot)
            {
                // Most of the time this is set by the MouseLeftButtonDown handler but validation could cause that code path to fail
                UpdateSelectionAndCurrency(this.CurrentColumnIndex, groupHeader.RowGroupInfo.Slot, DataGridSelectionAction.SelectCurrent, false /*scrollIntoView*/);
            }

            UpdateRowGroupVisibility(groupHeader.RowGroupInfo, newVisibility, true /*isDisplayed*/);

            ComputeScrollBarsLayout();
            // We need force arrange since our Scrollings Rows could update without automatically triggering layout
            InvalidateRowsArrange();
        }

        internal void OnRowsMeasure()
        {
            if (!DoubleUtil.IsZero(this.DisplayData.PendingVerticalScrollHeight))
            {
                ScrollSlotsByHeight(this.DisplayData.PendingVerticalScrollHeight);
                this.DisplayData.PendingVerticalScrollHeight = 0;
            }
        }

        internal void OnSublevelIndentUpdated(DataGridRowGroupHeader groupHeader, double newValue)
        {
            Debug.Assert(this.DataConnection.CollectionView != null);
            Debug.Assert(this.RowGroupSublevelIndents != null);

            int groupLevelCount = this.DataConnection.CollectionView.GroupDescriptions.Count;
            Debug.Assert(groupHeader.Level >= 0 && groupHeader.Level < groupLevelCount);

            double oldValue = this.RowGroupSublevelIndents[groupHeader.Level];
            if (groupHeader.Level > 0)
            {
                oldValue -= this.RowGroupSublevelIndents[groupHeader.Level - 1];
            }
            // Update the affected values in our table by the amount affected
            double change = newValue - oldValue;
            for (int i = groupHeader.Level; i < groupLevelCount; i++)
            {
                this.RowGroupSublevelIndents[i] += change;
                Debug.Assert(this.RowGroupSublevelIndents[i] >= 0);
            }

            EnsureRowGroupSpacerColumnWidth(groupLevelCount);
        }
        
        internal void RefreshRows(bool recycleRows, bool clearRows)
        {
            if (_measured)
            {
                // _desiredCurrentColumnIndex is used in MakeFirstDisplayedCellCurrentCell to set the
                // column position back to what it was before the refresh
                this._desiredCurrentColumnIndex = this.CurrentColumnIndex;
                double verticalOffset = this._verticalOffset;
                if (this.DisplayData.PendingVerticalScrollHeight > 0)
                {
                    // Use the pending vertical scrollbar position if there is one, in the case that the collection
                    // has been reset multiple times in a row.
                    verticalOffset = this.DisplayData.PendingVerticalScrollHeight;
                }
                this._verticalOffset = 0;
                this.NegVerticalOffset = 0;

                if (clearRows)
                {
                    ClearRows(recycleRows);
                    ClearRowGroupHeadersTable();
                    PopulateRowGroupHeadersTable();
                }

                RefreshRowGroupHeaders();

                // Update the CurrentSlot because it might have changed
                if (recycleRows && this.DataConnection.CollectionView != null)
                {
                    this.CurrentSlot = this.DataConnection.CollectionView.CurrentPosition == -1
                        ? -1 : SlotFromRowIndex(this.DataConnection.CollectionView.CurrentPosition);
                    if (this.CurrentSlot == -1)
                    {
                        SetCurrentCellCore(-1, -1);
                    }
                }

                if (this.DataConnection != null && this.ColumnsItemsInternal.Count > 0)
                {
                    AddSlots(this.DataConnection.Count + this.RowGroupHeadersTable.IndexCount);

                    InvalidateMeasure();
                }

                EnsureRowGroupSpacerColumn();

                if (this.VerticalScrollBar != null)
                {
                    this.DisplayData.PendingVerticalScrollHeight = Math.Min(verticalOffset, this.VerticalScrollBar.Maximum);
                }
            }
            else
            {
                if (clearRows)
                {
                    ClearRows(recycleRows /*recycle*/);
                }
                ClearRowGroupHeadersTable();
                PopulateRowGroupHeadersTable();
            }
        }

        internal void RemoveRowAt(int rowIndex, object item)
        {
            RemoveElementAt(SlotFromRowIndex(rowIndex), item, true);
        }

        internal DataGridRowGroupInfo RowGroupInfoFromCollectionViewGroup(CollectionViewGroup collectionViewGroup)
        {
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.CollectionViewGroup == collectionViewGroup)
                {
                    return rowGroupInfo;
                }
            }
            return null;
        }

        internal int RowIndexFromSlot(int slot)
        {
            return slot - this.RowGroupHeadersTable.GetIndexCount(0, slot);
        }

        internal bool ScrollSlotIntoView(int slot, bool scrolledHorizontally)
        {
            Debug.Assert(_collapsedSlotsTable.Contains(slot) || !IsSlotOutOfBounds(slot));

            if (scrolledHorizontally && this.DisplayData.FirstScrollingSlot <= slot && this.DisplayData.LastScrollingSlot >= slot)
            {
                // If the slot is displayed and we scrolled horizontally, column virtualization could cause the rows to grow.
                // As a result we need to force measure on the rows we're displaying and recalculate our First and Last slots 
                // so they're accurate
                foreach (DataGridRow row in this.DisplayData.GetScrollingRows())
                {
                    row.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
            }

            if (this.DisplayData.FirstScrollingSlot < slot && this.DisplayData.LastScrollingSlot > slot)
            {
                // The row is already displayed in its entirety
                return true;
            }
            else if (this.DisplayData.FirstScrollingSlot == slot && slot != -1)
            {
                if (!DoubleUtil.IsZero(this.NegVerticalOffset))
                {
                    // First displayed row is partially scrolled of. Let's scroll it so that this.NegVerticalOffset becomes 0.
                    this.DisplayData.PendingVerticalScrollHeight = -this.NegVerticalOffset;
                    InvalidateRowsMeasure(false /*invalidateIndividualRows*/);
                }
                return true;
            }

            double deltaY = 0;
            int firstFullSlot;
            if (this.DisplayData.FirstScrollingSlot > slot)
            {
                // Scroll up to the new row so it becomes the first displayed row
                firstFullSlot = this.DisplayData.FirstScrollingSlot - 1;
                if (DoubleUtil.GreaterThan(this.NegVerticalOffset, 0))
                {
                    deltaY = -this.NegVerticalOffset;
                }
                deltaY -= GetSlotElementsHeight(slot, firstFullSlot);
                if (this.DisplayData.FirstScrollingSlot - slot > 1)
                {
                    // 

                    ResetDisplayedRows();
                }
                this.NegVerticalOffset = 0;
                UpdateDisplayedRows(slot, this.CellsHeight);
            }
            else if (this.DisplayData.LastScrollingSlot <= slot)
            {
                // Scroll down to the new row so it's entirely displayed.  If the height of the row
                // is greater than the height of the DataGrid, then show the top of the row at the top
                // of the grid
                firstFullSlot = this.DisplayData.LastScrollingSlot;
                // Figure out how much of the last row is cut off
                double rowHeight = GetExactSlotElementHeight(this.DisplayData.LastScrollingSlot);
                double availableHeight = this.AvailableSlotElementRoom + rowHeight;
                if (DoubleUtil.AreClose(rowHeight, availableHeight))
                {
                    if (this.DisplayData.LastScrollingSlot == slot)
                    {
                        // We're already at the very bottom so we don't need to scroll down further
                        return true;
                    }
                    else
                    {
                        // We're already showing the entire last row so don't count it as part of the delta
                        firstFullSlot++;
                    }
                }
                else if (rowHeight > availableHeight)
                {
                    firstFullSlot++;
                    deltaY += rowHeight - availableHeight;
                }
                // sum up the height of the rest of the full rows
                if (slot >= firstFullSlot)
                {
                    deltaY += GetSlotElementsHeight(firstFullSlot, slot);
                }
                // If the first row we're displaying is no longer adjacent to the rows we have
                // simply discard the ones we have
                if (slot - this.DisplayData.LastScrollingSlot > 1)
                {
                    ResetDisplayedRows();
                }
                if (DoubleUtil.GreaterThanOrClose(GetExactSlotElementHeight(slot), this.CellsHeight))
                {
                    // The entire row won't fit in the DataGrid so we start showing it from the top
                    this.NegVerticalOffset = 0;
                    UpdateDisplayedRows(slot, this.CellsHeight);
                }
                else
                {
                    UpdateDisplayedRowsFromBottom(slot);
                }
            }

            this._verticalOffset += deltaY;
            if (this._verticalOffset < 0 || this.DisplayData.FirstScrollingSlot == 0)
            {
                // We scrolled too far because a row's height was larger than its approximation
                this._verticalOffset = this.NegVerticalOffset;
            }

            // 
            Debug.Assert(DoubleUtil.LessThanOrClose(this.NegVerticalOffset, this._verticalOffset));

            SetVerticalOffset(_verticalOffset);

            InvalidateMeasure();
            InvalidateRowsMeasure(false /*invalidateIndividualRows*/);

            return true;
        }

        internal void SetRowSelection(int slot, bool isSelected, bool setAnchorSlot)
        {
            Debug.Assert(!(!isSelected && setAnchorSlot));
            Debug.Assert(!IsSlotOutOfSelectionBounds(slot));
            this._noSelectionChangeCount++;
            try
            {
                if (this.SelectionMode == DataGridSelectionMode.Single && isSelected)
                {
                    Debug.Assert(_selectedItems.Count <= 1);
                    if (_selectedItems.Count > 0)
                    {
                        int currentlySelectedSlot = _selectedItems.GetIndexes().First();
                        if (currentlySelectedSlot != slot)
                        {
                            SelectSlot(currentlySelectedSlot, false);
                            this.SelectionHasChanged = true;
                        }
                    }
                }
                if (_selectedItems.ContainsSlot(slot) != isSelected)
                {
                    SelectSlot(slot, isSelected);
                    this.SelectionHasChanged = true;
                }
                if (setAnchorSlot)
                {
                    this.AnchorSlot = slot;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        // For now, all scenarios are for isSelected == true.
        internal void SetRowsSelection(int startSlot, int endSlot /*, bool isSelected*/)
        {
            Debug.Assert(startSlot >= 0 && startSlot < this.SlotCount);
            Debug.Assert(endSlot >= 0 && endSlot < this.SlotCount);
            Debug.Assert(startSlot <= endSlot);

            this._noSelectionChangeCount++;
            try
            {
                if (/*isSelected &&*/ !_selectedItems.ContainsAll(startSlot, endSlot))
                {
                    // At least one row gets selected
                    SelectSlots(startSlot, endSlot, true);
                    this.SelectionHasChanged = true;
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
        }

        internal int SlotFromRowIndex(int rowIndex)
        {
            return rowIndex + this.RowGroupHeadersTable.GetIndexCountBeforeGap(0, rowIndex);
        }

#endregion Internal Methods

#region Private Methods

        private void AddSlotElement(int slot, UIElement element)
        {
#if DEBUG
            DataGridRow row = element as DataGridRow;
            if (row != null)
            {
                Debug.Assert(row.OwningGrid == this);
                Debug.Assert(row.Cells.Count == this.ColumnsItemsInternal.Count);

                int columnIndex = 0;
                foreach (DataGridCell dataGridCell in row.Cells)
                {
                    Debug.Assert(dataGridCell.OwningRow == row);
                    Debug.Assert(dataGridCell.OwningColumn == this.ColumnsItemsInternal[columnIndex]);
                    columnIndex++;
                }
            }
#endif
            Debug.Assert(slot == this.SlotCount);

            OnAddedElement_Phase1(slot, element);
            this.SlotCount++;
            this.VisibleSlotCount++;
            OnAddedElement_Phase2(slot, false /*updateVerticalScrollBarOnly*/);
            OnElementsChanged(true /*grew*/);
        }

        private void AddSlots(int totalSlots)
        {
            this.SlotCount = 0;
            this.VisibleSlotCount = 0;
            IEnumerator<int> groupSlots = null;
            int nextGroupSlot = -1;
            if (this.RowGroupHeadersTable.RangeCount > 0)
            {
                groupSlots = this.RowGroupHeadersTable.GetIndexes().GetEnumerator();
                if (groupSlots != null && groupSlots.MoveNext())
                {
                    nextGroupSlot = groupSlots.Current;
                }
            }
            int slot = 0;
            int addedRows = 0;
            while (slot < totalSlots && this.AvailableSlotElementRoom > 0)
            {
                if (slot == nextGroupSlot)
                {
                    DataGridRowGroupInfo groupRowInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    AddSlotElement(slot, GenerateRowGroupHeader(slot, groupRowInfo));
                    nextGroupSlot = groupSlots.MoveNext() ? groupSlots.Current : -1;
                }
                else
                {
                    AddSlotElement(slot, GenerateRow(addedRows, slot));
                    addedRows++;
                }
                slot++;
            }

            if (slot < totalSlots)
            {
                this.SlotCount += totalSlots - slot;
                this.VisibleSlotCount += totalSlots - slot;
                OnAddedElement_Phase2(0, this._vScrollBar == null || this._vScrollBar.Visibility == Visibility.Visible /*updateVerticalScrollBarOnly*/);
                OnElementsChanged(true /*grew*/);
            }
        }

        private void ApplyDisplayedRowsState(int startSlot, int endSlot)
        {
            int firstSlot = Math.Max(this.DisplayData.FirstScrollingSlot, startSlot);
            int lastSlot = Math.Min(this.DisplayData.LastScrollingSlot, endSlot);

            if (firstSlot >= 0)
            {
                Debug.Assert(lastSlot >= firstSlot);
                int slot = GetNextVisibleSlot(firstSlot - 1);
                while (slot <= lastSlot)
                {
                    DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                    if (row != null)
                    {
                        row.ApplyState(true /*animate*/);
                    }
                    slot = GetNextVisibleSlot(slot);
                }
            }
        }

        private void ClearRowGroupHeadersTable()
        {
            // Detach existing handlers on CollectionViewGroup.Items.CollectionChanged
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (groupInfo.CollectionViewGroup.Items != null)
                {
                    ((INotifyCollectionChanged)groupInfo.CollectionViewGroup.Items).CollectionChanged -= CollectionViewGroup_CollectionChanged;
                }
            }
            if (this._topLevelGroup != null)
            {
                // The PagedCollectionView reuses the top level group so we need to detach any existing or else we'll get duplicate handers here
                this._topLevelGroup.CollectionChanged -= CollectionViewGroup_CollectionChanged;
                this._topLevelGroup = null;
            }

            this.RowGroupHeadersTable.Clear();
            // Unfortunately PagedCollectionView does not allow us to preserve expanded or collapsed states for RowGroups since
            // the CollectionViewGroups are recreated when a Reset happens.  This is true in both SL and WPF
            _collapsedSlotsTable.Clear();

            _rowGroupHeightsByLevel = null;
            RowGroupSublevelIndents = null;
        }

        private void ClearRows(bool recycle)
        {
            // Need to clean up recycled rows even if the RowCount is 0
            SetCurrentCellCore(-1, -1, false /*commitEdit*/, false /*endRowEdit*/);
            ClearRowSelection(true /*resetAnchorSlot*/);
            UnloadElements(recycle);

            this._showDetailsTable.Clear();
            this.SlotCount = 0;
            this.NegVerticalOffset = 0;
            SetVerticalOffset(0);
            ComputeScrollBarsLayout();
        }

        // Updates _collapsedSlotsTable and returns the number of pixels that were collapsed
        private double CollapseSlotsInTable(int startSlot, int endSlot, ref int slotsExpanded, int lastDisplayedSlot, ref double heightChangeBelowLastDisplayedSlot)
        {
            int firstSlot = startSlot;
            int lastSlot;
            double totalHeightChange = 0;
            // Figure out which slots actually need to be expanded since some might already be collapsed
            while (firstSlot <= endSlot)
            {
                firstSlot = _collapsedSlotsTable.GetNextGap(firstSlot - 1);
                int nextCollapsedSlot = _collapsedSlotsTable.GetNextIndex(firstSlot) - 1;
                lastSlot = nextCollapsedSlot == -2 ? endSlot : Math.Min(endSlot, nextCollapsedSlot);

                if (firstSlot <= lastSlot)
                {
                    double heightChange = GetHeightEstimate(firstSlot, lastSlot);
                    totalHeightChange -= heightChange;
                    slotsExpanded -= lastSlot - firstSlot + 1;

                    if (lastSlot > lastDisplayedSlot)
                    {
                        if (firstSlot > lastDisplayedSlot)
                        {
                            heightChangeBelowLastDisplayedSlot -= heightChange;
                        }
                        else
                        {
                            heightChangeBelowLastDisplayedSlot -= GetHeightEstimate(lastDisplayedSlot + 1, lastSlot);
                        }
                    }

                    firstSlot = lastSlot + 1;
                }
            }

            // Update _collapsedSlotsTable in one bulk operation
            _collapsedSlotsTable.AddValues(startSlot, endSlot - startSlot + 1, Visibility.Collapsed);

            return totalHeightChange;
        }

        private void CollectionViewGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemCount")
            {
                DataGridRowGroupInfo rowGroupInfo = RowGroupInfoFromCollectionViewGroup(sender as CollectionViewGroup);
                if (rowGroupInfo != null && IsSlotVisible(rowGroupInfo.Slot))
                {
                    DataGridRowGroupHeader rowGroupHeader = this.DisplayData.GetDisplayedElement(rowGroupInfo.Slot) as DataGridRowGroupHeader;
                    if (rowGroupHeader != null)
                    {
                        rowGroupHeader.UpdateTitleElements();
                    }
                }
            }
        }

        // This method is necessary for incrementing the LastSubItemSlot property of the group ancestors
        // because CorrectSlotsAfterInsertion only increments those that come after the specified group
        private void CorrectLastSubItemSlotsAfterInsertion(DataGridRowGroupInfo subGroupInfo)
        {
            int subGroupSlot;
            int subGroupLevel;
            while (subGroupInfo != null)
            {
                subGroupLevel = subGroupInfo.Level;
                subGroupInfo.LastSubItemSlot++;

                while (subGroupInfo != null && subGroupInfo.Level >= subGroupLevel)
                {
                    subGroupSlot = this.RowGroupHeadersTable.GetPreviousIndex(subGroupInfo.Slot);
                    subGroupInfo = this.RowGroupHeadersTable.GetValueAt(subGroupSlot);
                }
            }
        }

        private static void CorrectRowAfterDeletion(DataGridRow row, bool rowDeleted)
        {
            row.Slot--;
            if (rowDeleted)
            {
                row.Index--;
            }
        }

        private static void CorrectRowAfterInsertion(DataGridRow row, bool rowInserted)
        {
            row.Slot++;
            if (rowInserted)
            {
                row.Index++;
            }
        }

        /// <summary>
        /// Adjusts the index of all displayed, loaded and edited rows after a row was deleted.
        /// Removes the deleted row from the list of loaded rows if present.
        /// </summary>
        private void CorrectSlotsAfterDeletion(int slotDeleted, bool wasRow)
        {
            Debug.Assert(slotDeleted >= 0);

            // Take care of the non-visible loaded rows
            for (int index = 0; index < this._loadedRows.Count; )
            {
                DataGridRow dataGridRow = this._loadedRows[index];
                if (this.IsSlotVisible(dataGridRow.Slot))
                {
                    index++;
                }
                else
                {
                    if (dataGridRow.Slot > slotDeleted)
                    {
                        CorrectRowAfterDeletion(dataGridRow, wasRow);
                        index++;
                    }
                    else if (dataGridRow.Slot == slotDeleted)
                    {
                        this._loadedRows.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            // Take care of the non-visible edited row
            if (this.EditingRow != null &&
                !this.IsSlotVisible(this.EditingRow.Slot) &&
                this.EditingRow.Slot > slotDeleted)
            {
                CorrectRowAfterDeletion(this.EditingRow, wasRow);
            }

            // Take care of the non-visible focused row
            if (this._focusedRow != null &&
                this._focusedRow != this.EditingRow &&
                !this.IsSlotVisible(this._focusedRow.Slot) &&
                this._focusedRow.Slot > slotDeleted)
            {
                CorrectRowAfterDeletion(this._focusedRow, wasRow);
            }

            // Take care of the visible rows
            foreach (DataGridRow row in this.DisplayData.GetScrollingRows())
            {
                if (row.Slot > slotDeleted)
                {
                    CorrectRowAfterDeletion(row, wasRow);
                    row.EnsureBackground();
                }
            }

            // Update the RowGroupHeaders
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.Slot > slotDeleted)
                {
                    rowGroupInfo.Slot--;
                }
                if (rowGroupInfo.LastSubItemSlot >= slotDeleted)
                {
                    rowGroupInfo.LastSubItemSlot--;
                }
            }

            // Update which row we've calculated the RowHeightEstimate up to
            if (_lastEstimatedRow >= slotDeleted)
            {
                _lastEstimatedRow--;
            }
        }

        /// <summary>
        /// Adjusts the index of all displayed, loaded and edited rows after rows were deleted.
        /// </summary>
        private void CorrectSlotsAfterInsertion(int slotInserted, bool isCollapsed, bool rowInserted)
        {
            Debug.Assert(slotInserted >= 0);

            // Take care of the non-visible loaded rows
            foreach (DataGridRow dataGridRow in this._loadedRows)
            {
                if (!this.IsSlotVisible(dataGridRow.Slot) && dataGridRow.Slot >= slotInserted)
                {
                    DataGrid.CorrectRowAfterInsertion(dataGridRow, rowInserted);
                }
            }

            // Take care of the non-visible focused row
            if (this._focusedRow != null &&
                this._focusedRow != EditingRow &&
                !(this.IsSlotVisible(_focusedRow.Slot) || ((_focusedRow.Slot == slotInserted) && isCollapsed)) &&
                this._focusedRow.Slot >= slotInserted)
            {
                DataGrid.CorrectRowAfterInsertion(this._focusedRow, rowInserted);
            }

            // Take care of the visible rows
            foreach (DataGridRow row in this.DisplayData.GetScrollingRows())
            {
                if (row.Slot >= slotInserted)
                {
                    DataGrid.CorrectRowAfterInsertion(row, rowInserted);
                    row.EnsureBackground();
                }
            }

            // Re-calculate the EditingRow's Slot and Index and ensure that it is still selected.
            if (this.EditingRow != null)
            {
                this.EditingRow.Index = this.DataConnection.IndexOf(this.EditingRow.DataContext);
                this.EditingRow.Slot = this.SlotFromRowIndex(this.EditingRow.Index);
            }

            // Update the RowGroupHeaders
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes(slotInserted))
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo.Slot >= slotInserted)
                {
                    rowGroupInfo.Slot++;
                }

                // We are purposefully checking GT and not GTE because the equality case is handled
                // by the CorrectLastSubItemSlotsAfterInsertion method
                if (rowGroupInfo.LastSubItemSlot > slotInserted)
                {
                    rowGroupInfo.LastSubItemSlot++;
                }
            }

            // Update which row we've calculated the RowHeightEstimate up to
            if (_lastEstimatedRow >= slotInserted)
            {
                _lastEstimatedRow++;
            }
        }

        private int CountAndPopulateGroupHeaders(object group, int rootSlot, int level)
        {
            int treeCount = 1;

            CollectionViewGroup collectionViewGroup = group as CollectionViewGroup;
            if (collectionViewGroup != null)
            {
                if (collectionViewGroup.Items != null && collectionViewGroup.Items.Count > 0)
                {
                    ((INotifyCollectionChanged)collectionViewGroup.Items).CollectionChanged += CollectionViewGroup_CollectionChanged;
                    if (collectionViewGroup.Items[0] is CollectionViewGroup)
                    {
                        foreach (object subGroup in collectionViewGroup.Items)
                        {
                            treeCount += CountAndPopulateGroupHeaders(subGroup, rootSlot + treeCount, level + 1);
                        }
                    }
                    else
                    {
                        // Optimization: don't walk to the bottom level nodes
                        treeCount += collectionViewGroup.Items.Count;
                    }
                }
                this.RowGroupHeadersTable.AddValue(rootSlot, new DataGridRowGroupInfo(collectionViewGroup, Visibility.Visible, level, rootSlot, rootSlot + treeCount - 1));
            }
            return treeCount;
        }

        private void CollectionViewGroup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If we receive this event when the number of GroupDescriptions is different than what we have already
            // accounted for, that means the ICollectionView is still in the process of updating its groups.  It will
            // send a reset notification when it's done, at which point we can update our visuals.

            if (this._rowGroupHeightsByLevel != null && 
                this.DataConnection.CollectionView != null &&
                this.DataConnection.CollectionView.GroupDescriptions != null &&
                this.DataConnection.CollectionView.GroupDescriptions.Count == this._rowGroupHeightsByLevel.Length)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.CollectionViewGroup_CollectionChanged_Add(sender, e);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.CollectionViewGroup_CollectionChanged_Remove(sender, e);
                        break;
                }
            }
        }

        private void CollectionViewGroup_CollectionChanged_Add(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                // We need to figure out the CollectionViewGroup that the sender belongs to.  We could cache
                // it by tagging the collections ahead of time, but I think the extra storage might not be worth
                // it since this lookup should be performant enough
                int insertSlot = -1;
                DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(sender);
                CollectionViewGroup group = e.NewItems[0] as CollectionViewGroup;

                if (parentGroupInfo != null)
                {
                    if (group != null || parentGroupInfo.Level == -1)
                    {
                        insertSlot = parentGroupInfo.Slot + 1;
                        // For groups, we need to skip over subgroups to find the correct slot
                        DataGridRowGroupInfo groupInfo;
                        for (int i = 0; i < e.NewStartingIndex; i++)
                        {
                            do
                            {
                                insertSlot = this.RowGroupHeadersTable.GetNextIndex(insertSlot);
                                groupInfo = this.RowGroupHeadersTable.GetValueAt(insertSlot);
                            }
                            while (groupInfo != null && groupInfo.Level > parentGroupInfo.Level + 1);
                            if (groupInfo == null)
                            {
                                // We couldn't find the subchild so this should go at the end
                                insertSlot = this.SlotCount;
                            }
                        }

                    }
                    else
                    {
                        // For items the slot is a simple calculation
                        insertSlot = parentGroupInfo.Slot + e.NewStartingIndex + 1;
                    }
                }

                // This could not be found when new GroupDescriptions are added to the PagedCollectionView
                if (insertSlot != -1)
                {
                    bool isCollapsed = (parentGroupInfo != null) && ((parentGroupInfo.Visibility == Visibility.Collapsed) || _collapsedSlotsTable.Contains(parentGroupInfo.Slot));
                    if (group != null)
                    {
                        if (group.Items != null)
                        {
                            ((INotifyCollectionChanged)group.Items).CollectionChanged += CollectionViewGroup_CollectionChanged;
                        }
                        DataGridRowGroupInfo newGroupInfo = new DataGridRowGroupInfo(group, Visibility.Visible, parentGroupInfo.Level + 1, insertSlot, insertSlot);
                        InsertElementAt(insertSlot, -1 /*rowIndex*/, null /*item*/, newGroupInfo, isCollapsed);
                        this.RowGroupHeadersTable.AddValue(insertSlot, newGroupInfo);
                    }
                    else
                    {
                        // Assume we're adding a new row
                        int rowIndex = this.DataConnection.IndexOf(e.NewItems[0]);
                        Debug.Assert(rowIndex != -1);
                        if (this.SlotCount == 0 && this.DataConnection.ShouldAutoGenerateColumns)
                        {
                            AutoGenerateColumnsPrivate();
                        }
                        InsertElementAt(insertSlot, rowIndex, e.NewItems[0]/*item*/, null/*rowGroupInfo*/, isCollapsed);
                    }

                    CorrectLastSubItemSlotsAfterInsertion(parentGroupInfo);
                    if (parentGroupInfo.LastSubItemSlot - parentGroupInfo.Slot == 1)
                    {
                        // We just added the first item to a RowGroup so the header should transition from Empty to either Expanded or Collapsed
                        EnsureAnscestorsExpanderButtonChecked(parentGroupInfo);
                    }
                }
            }
        }

        private void CollectionViewGroup_CollectionChanged_Remove(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(e.OldItems.Count == 1);
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                CollectionViewGroup removedGroup = e.OldItems[0] as CollectionViewGroup;
                if (removedGroup != null)
                {
                    if (removedGroup.Items != null)
                    {
                        ((INotifyCollectionChanged)removedGroup.Items).CollectionChanged -= CollectionViewGroup_CollectionChanged;
                    }
                    DataGridRowGroupInfo groupInfo = RowGroupInfoFromCollectionViewGroup(removedGroup);
                    Debug.Assert(groupInfo != null);
                    if ((groupInfo.Level == _rowGroupHeightsByLevel.Length - 1) && (removedGroup.Items != null) && (removedGroup.Items.Count > 0))
                    {
                        Debug.Assert((groupInfo.LastSubItemSlot - groupInfo.Slot) == removedGroup.Items.Count);
                        // If we're removing a leaf Group then remove all of its items before removing the Group
                        for (int i = 0; i < removedGroup.Items.Count; i++)
                        {
                            RemoveElementAt(groupInfo.Slot + 1, removedGroup.Items[i] /*item*/, true /*isRow*/);
                        }
                    }
                    RemoveElementAt(groupInfo.Slot, null /*item*/, false /*isRow*/);
                }
                else
                {
                    // A single item was removed from a leaf group
                    DataGridRowGroupInfo parentGroupInfo = GetParentGroupInfo(sender);
                    if (parentGroupInfo != null)
                    {
                        int slot;
                        if (parentGroupInfo.CollectionViewGroup == null && this.RowGroupHeadersTable.IndexCount > 0)
                        {
                            // In this case, we're removing from the root group.  If there are other groups, then this must
                            // be the new item row that doesn't belong to any group because if there are other groups then
                            // this item cannot be a child of the root group.
                            slot = this.SlotCount - 1;
                        }
                        else
                        {
                            slot = parentGroupInfo.Slot + e.OldStartingIndex + 1;
                        }
                        RemoveElementAt(slot, e.OldItems[0], true /*isRow*/);
                    }
                }
            }
        }

        private void EnsureAnscestorsExpanderButtonChecked(DataGridRowGroupInfo parentGroupInfo)
        {
            if (IsSlotVisible(parentGroupInfo.Slot))
            {
                DataGridRowGroupHeader ancestorGroupHeader = this.DisplayData.GetDisplayedElement(parentGroupInfo.Slot) as DataGridRowGroupHeader;
                while (ancestorGroupHeader != null)
                {
                    ancestorGroupHeader.EnsureExpanderButtonIsChecked();
                    if (ancestorGroupHeader.Level > 0)
                    {
                        int slot = this.RowGroupHeadersTable.GetPreviousIndex(ancestorGroupHeader.RowGroupInfo.Slot);
                        if (IsSlotVisible(slot))
                        {
                            ancestorGroupHeader = this.DisplayData.GetDisplayedElement(slot) as DataGridRowGroupHeader;
                            continue;
                        }
                    }
                    break;
                }
            }
        }

        private void EnsureRowDetailsVisibility(DataGridRow row, bool raiseNotification, bool animate)
        {
            // Show or hide RowDetails based on DataGrid settings
            row.SetDetailsVisibilityInternal(GetRowDetailsVisibility(row.Index), raiseNotification, animate);
        }

        private IEnumerable<DataGridRow> GetAllRows()
        {
            if (_rowsPresenter != null)
            {
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        yield return row;
                    }
                }
            }
        }

        // Returns the number of rows with details visible between lowerBound and upperBound exclusive.
        // As of now, the caller needs to account for Collapsed slots.  This method assumes everything
        // is visible
        private int GetDetailsCountInclusive(int lowerBound, int upperBound)
        {
            int indexCount = upperBound - lowerBound + 1;
            if (indexCount <= 0)
            {
                return 0;
            }
            if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible)
            {
                // Total rows minus ones which explicity turned details off minus the RowGroupHeaders
                return indexCount - _showDetailsTable.GetIndexCount(lowerBound, upperBound, Visibility.Collapsed) - this.RowGroupHeadersTable.GetIndexCount(lowerBound, upperBound);
            }
            else if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                // Total rows with details explicitly turned on
                return _showDetailsTable.GetIndexCount(lowerBound, upperBound, Visibility.Visible);
            }
            else if (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
            {
                // Total number of remaining rows that are selected
                return _selectedItems.GetIndexCount(lowerBound, upperBound);
            }
            Debug.Assert(false); // Shouldn't ever happen
            return 0;
        }

        private void EnsureRowGroupSpacerColumn()
        {
            bool spacerColumnChanged = this.ColumnsInternal.EnsureRowGrouping(!this.RowGroupHeadersTable.IsEmpty);
            if (spacerColumnChanged)
            {
                if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented && this.CurrentColumnIndex == 0)
                {
                    this.CurrentColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                }

                ProcessFrozenColumnCount(this);
            }
        }

        private void EnsureRowGroupSpacerColumnWidth(int groupLevelCount)
        {
            if (groupLevelCount == 0)
            {
                this.ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(0);
            }
            else
            {
                this.ColumnsInternal.RowGroupSpacerColumn.Width = new DataGridLength(this.RowGroupSublevelIndents[groupLevelCount - 1]);
            }
        }

        private void EnsureRowGroupVisibility(DataGridRowGroupInfo rowGroupInfo, Visibility visibility, bool setCurrent)
        {
            if (rowGroupInfo == null)
            {
                return;
            }
            if (rowGroupInfo.Visibility != visibility)
            {
                if (this.IsSlotVisible(rowGroupInfo.Slot))
                {
                    DataGridRowGroupHeader rowGroupHeader = this.DisplayData.GetDisplayedElement(rowGroupInfo.Slot) as DataGridRowGroupHeader;
                    Debug.Assert(rowGroupHeader != null);
                    rowGroupHeader.ToggleExpandCollapse(visibility, setCurrent);
                }
                else
                {
                    if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    {
                        // Somewhere up the parent chain, there's a collapsed header so all the slots remain the same and
                        // we just need to mark this header with the new visibility
                        rowGroupInfo.Visibility = visibility;
                    }
                    else
                    {
                        if (rowGroupInfo.Slot < this.DisplayData.FirstScrollingSlot)
                        {
                            double heightChange = UpdateRowGroupVisibility(rowGroupInfo, visibility, false /*isDisplayed*/);
                            // Use epsilon instead of 0 here so that in the off chance that our estimates put the vertical offset negative
                            // the user can still scroll to the top since the offset is non-zero
                            SetVerticalOffset(Math.Max(DoubleUtil.DBL_EPSILON, _verticalOffset + heightChange));
                        }
                        else
                        {
                            UpdateRowGroupVisibility(rowGroupInfo, visibility, false /*isDisplayed*/);
                        }
                        UpdateVerticalScrollBar();
                    }
                }
            }
        }

        // Expands slots from startSlot to endSlot inclusive and adds the amount expanded in this suboperation to
        // the given totalHeightChanged of the entire operation
        private void ExpandSlots(int startSlot, int endSlot, bool isDisplayed, ref int slotsExpanded, ref double totalHeightChange)
        {
            double heightAboveStartSlot = 0;
            if (isDisplayed)
            {
                int slot = this.DisplayData.FirstScrollingSlot;
                while (slot < startSlot)
                {
                    heightAboveStartSlot += GetExactSlotElementHeight(slot);
                    slot = GetNextVisibleSlot(slot);
                }

                // First make the bottom rows available for recycling so we minimize element creation when expanding
                for (int i = 0; (i < endSlot - startSlot + 1) && (this.DisplayData.LastScrollingSlot > endSlot); i++)
                {
                    RemoveDisplayedElement(this.DisplayData.LastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                }
            }

            // Figure out which slots actually need to be expanded since some might already be collapsed
            double currentHeightChange = 0;
            int firstSlot = startSlot;
            int lastSlot = endSlot;
            while (firstSlot <= endSlot)
            {
                firstSlot = _collapsedSlotsTable.GetNextIndex(firstSlot - 1);
                if (firstSlot == -1)
                {
                    break;
                }
                lastSlot = Math.Min(endSlot, _collapsedSlotsTable.GetNextGap(firstSlot) - 1);

                if (firstSlot <= lastSlot)
                {
                    if (!isDisplayed)
                    {
                        // Estimate the height change if the slots aren't displayed.  If they are displayed, we can add real values
                        double headerHeight;
                        double rowCount = lastSlot - firstSlot - GetRowGroupHeaderCount(firstSlot, lastSlot, Visibility.Collapsed, out headerHeight) + 1;
                        double detailsCount = GetDetailsCountInclusive(firstSlot, lastSlot);
                        currentHeightChange += headerHeight + (detailsCount * this.RowDetailsHeightEstimate) + (rowCount * this.RowHeightEstimate);
                    }
                    slotsExpanded += lastSlot - firstSlot + 1;
                    firstSlot = lastSlot + 1;
                }
            }

            // Update _collapsedSlotsTable in one bulk operation
            _collapsedSlotsTable.RemoveValues(startSlot, endSlot - startSlot + 1);

            if (isDisplayed)
            {
                double availableHeight = this.CellsHeight - heightAboveStartSlot;
                // Actually expand the displayed slots up to what we can display
                for (int i = startSlot; (i <= endSlot) && (currentHeightChange < availableHeight); i++)
                {
                    FrameworkElement insertedElement = InsertDisplayedElement(i, false /*updateSlotInformation*/);
                    currentHeightChange += insertedElement.DesiredSize.Height;
                    if (i > this.DisplayData.LastScrollingSlot)
                    {
                        this.DisplayData.LastScrollingSlot = i;
                    }
                }
            }

            // Update the total height for the entire Expand operation
            totalHeightChange += currentHeightChange;
        }

        /// <summary>
        /// Creates all the editing elements for the current editing row, so the bindings
        /// all exist during validation.
        /// </summary>
        private void GenerateEditingElements()
        {
            if (this.EditingRow != null && this.EditingRow.Cells != null)
            {
                Debug.Assert(this.EditingRow.Cells.Count == this.ColumnsItemsInternal.Count);
                foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns(c => c.IsVisible && !c.IsReadOnly))
                {
                    column.GenerateEditingElementInternal(this.EditingRow.Cells[column.Index], this.EditingRow.DataContext);
                }
            }
        }

        /// <summary>
        /// Returns a row for the provided index. The row gets first loaded through the LoadingRow event.
        /// </summary>
        private DataGridRow GenerateRow(int rowIndex, int slot)
        {
            return GenerateRow(rowIndex, slot, this.DataConnection.GetDataItem(rowIndex));
        }

        /// <summary>
        /// Returns a row for the provided index. The row gets first loaded through the LoadingRow event.
        /// </summary>
        private DataGridRow GenerateRow(int rowIndex, int slot, object dataContext)
        {
            Debug.Assert(rowIndex > -1);
            DataGridRow dataGridRow = GetGeneratedRow(dataContext);
            if (dataGridRow == null)
            {
                dataGridRow = this.DisplayData.GetUsedRow() ?? new DataGridRow();
                dataGridRow.Index = rowIndex;
                dataGridRow.Slot = slot;
                dataGridRow.OwningGrid = this;
                dataGridRow.DataContext = dataContext;
                CompleteCellsCollection(dataGridRow);

                OnLoadingRow(new DataGridRowEventArgs(dataGridRow));

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.UpdateRowPeerEventsSource(dataGridRow);
                }
            }
            return dataGridRow;
        }

        private DataGridRowGroupHeader GenerateRowGroupHeader(int slot, DataGridRowGroupInfo rowGroupInfo)
        {
            Debug.Assert(slot > -1);
            Debug.Assert(rowGroupInfo != null);

            DataGridRowGroupHeader groupHeader = this.DisplayData.GetUsedGroupHeader() ?? new DataGridRowGroupHeader();
            groupHeader.OwningGrid = this;
            groupHeader.RowGroupInfo = rowGroupInfo;
            groupHeader.DataContext = rowGroupInfo.CollectionViewGroup;
            groupHeader.Level = rowGroupInfo.Level;

            // Set the RowGroupHeader's PropertyName. Unfortunately, CollectionViewGroup doesn't have this
            // so we have to set it manually
            Debug.Assert(this.DataConnection.CollectionView != null && groupHeader.Level < this.DataConnection.CollectionView.GroupDescriptions.Count);
            PropertyGroupDescription propertyGroupDescription = this.DataConnection.CollectionView.GroupDescriptions[groupHeader.Level] as PropertyGroupDescription;
            if (propertyGroupDescription != null)
            {
                if (this.DataConnection.DataType == null)
                {
                    groupHeader.PropertyName = propertyGroupDescription.PropertyName;
                }
                else
                {
                    groupHeader.PropertyName = this.DataConnection.DataType.GetDisplayName(propertyGroupDescription.PropertyName) ?? propertyGroupDescription.PropertyName;
                }
            }

            INotifyPropertyChanged inpc = rowGroupInfo.CollectionViewGroup as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= new PropertyChangedEventHandler(CollectionViewGroup_PropertyChanged);
                inpc.PropertyChanged += new PropertyChangedEventHandler(CollectionViewGroup_PropertyChanged);
            }
            groupHeader.UpdateTitleElements();

            OnLoadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.UpdateRowGroupHeaderPeerEventsSource(groupHeader);
            }

            return groupHeader;
        }

        /// <summary>
        /// Returns the exact row height, whether it is currently displayed or not.
        /// The row is generated and added to the displayed rows in case it is not already displayed.
        /// The horizontal gridlines thickness are added.
        /// </summary>
        private double GetExactSlotElementHeight(int slot)
        {
            Debug.Assert((slot >= 0 ) && slot < this.SlotCount);

            if (this.IsSlotVisible(slot))
            {
                Debug.Assert(this.DisplayData.GetDisplayedElement(slot) != null);
                return this.DisplayData.GetDisplayedElement(slot).DesiredSize.Height;
            }

            FrameworkElement slotElement = InsertDisplayedElement(slot, true /*updateSlotInformation*/);
            Debug.Assert(slotElement != null);
            return slotElement.DesiredSize.Height;
        }

        // Returns an estimate for the height of the slots between fromSlot and toSlot
        private double GetHeightEstimate(int fromSlot, int toSlot)
        {
            double headerHeight;
            double rowCount = toSlot - fromSlot - GetRowGroupHeaderCount(fromSlot, toSlot, Visibility.Visible, out headerHeight) + 1;
            double detailsCount = GetDetailsCountInclusive(fromSlot, toSlot);

            return headerHeight + (detailsCount * this.RowDetailsHeightEstimate) + (rowCount * this.RowHeightEstimate);
        }

        private DataGridRowGroupInfo GetParentGroupInfo(object collection)
        {
            if (collection == this.DataConnection.CollectionView.Groups)
            {
                // If the new item is a root level element, it has no parent group, so create an empty RowGroupInfo
                return new DataGridRowGroupInfo(null, Visibility.Visible, -1, -1, -1);
            }
            else
            {
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
                {
                    DataGridRowGroupInfo groupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    if (groupInfo.CollectionViewGroup.Items == collection)
                    {
                        return groupInfo;
                    }
                }
            }
            return null;
        }

        // Returns the inclusive count of expanded RowGroupHeaders from startSlot to endSlot
        private int GetRowGroupHeaderCount(int startSlot, int endSlot, Visibility? visibility, out double headersHeight)
        {
            int count = 0;
            headersHeight = 0;
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes(startSlot))
            {
                if (slot > endSlot)
                {
                    return count;
                }
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (!visibility.HasValue ||
                    (visibility == Visibility.Visible && !_collapsedSlotsTable.Contains(slot)) ||
                    (visibility == Visibility.Collapsed && _collapsedSlotsTable.Contains(slot)))
                {
                    count++;
                    headersHeight += _rowGroupHeightsByLevel[rowGroupInfo.Level];
                }
            }
            return count;
        }

        /// <summary>
        /// If the provided slot is displayed, returns the exact height.
        /// If the slot is not displayed, returns a default height.
        /// </summary>
        private double GetSlotElementHeight(int slot)
        {
            Debug.Assert(slot >= 0 && slot < this.SlotCount);
            if (this.IsSlotVisible(slot))
            {
                Debug.Assert(this.DisplayData.GetDisplayedElement(slot) != null);
                return this.DisplayData.GetDisplayedElement(slot).DesiredSize.Height;
            }
            else
            {
                DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                if (rowGroupInfo != null)
                {
                    return _rowGroupHeightsByLevel[rowGroupInfo.Level];
                }

                // Assume it's a row since we're either not grouping or it wasn't a RowGroupHeader
                return this.RowHeightEstimate + (GetRowDetailsVisibility(slot) == Visibility.Visible ? this.RowDetailsHeightEstimate : 0);
            }
        }

        /// <summary>
        /// Cumulates the approximate height of the rows from fromRowIndex to toRowIndex included.
        /// Including the potential gridline thickness.
        /// </summary>
        private double GetSlotElementsHeight(int fromSlot, int toSlot)
        {
            Debug.Assert(toSlot >= fromSlot);

            double height = 0;
            for (int slot = fromSlot; slot <= toSlot; slot++)
            {
                height += GetSlotElementHeight(slot);
            }
            return height;
        }

        /// <summary>
        /// Checks if the row for the provided dataContext has been generated and is present
        /// in either the loaded rows, pre-fetched rows, or editing row. 
        /// The displayed rows are *not* searched. Returns null if the row does not belong to those 3 categories.
        /// </summary>
        private DataGridRow GetGeneratedRow(object dataContext)
        {
            // Check the list of rows being loaded via the LoadingRow event.
            DataGridRow dataGridRow = GetLoadedRow(dataContext);
            if (dataGridRow != null)
            {
                return dataGridRow;
            }

            // Check the potential editing row.
            if (this.EditingRow != null && dataContext == this.EditingRow.DataContext)
            {
                return this.EditingRow;
            }

            // Check the potential focused row.
            if (this._focusedRow != null && dataContext == this._focusedRow.DataContext)
            {
                return this._focusedRow;
            }

            return null;
        }

        private DataGridRow GetLoadedRow(object dataContext)
        {
            foreach (DataGridRow dataGridRow in this._loadedRows)
            {
                if (dataGridRow.DataContext == dataContext)
                {
                    return dataGridRow;
                }
            }
            return null;
        }

        private FrameworkElement InsertDisplayedElement(int slot, bool updateSlotInformation)
        {
            FrameworkElement slotElement;
            if (this.RowGroupHeadersTable.Contains(slot))
            {
                slotElement = GenerateRowGroupHeader(slot, this.RowGroupHeadersTable.GetValueAt(slot)/*rowGroupInfo*/);
            }
            else
            {
                // If we're grouping, the GroupLevel needs to be fixed later by methods calling this 
                // which end up inserting rows. We don't do it here because elements could be inserted 
                // from top to bottom or bottom to up so it's better to do in one pass
                slotElement = GenerateRow(RowIndexFromSlot(slot), slot);
            }
            InsertDisplayedElement(slot, slotElement, false /*wasNewlyAdded*/, updateSlotInformation);
            return slotElement;
        }

        private void InsertDisplayedElement(int slot, UIElement element, bool wasNewlyAdded, bool updateSlotInformation)
        {
            // We can only support creating new rows that are adjacent to the currently visible rows
            // since they need to be added to the visual tree for us to Measure them.
            Debug.Assert(this.DisplayData.FirstScrollingSlot == -1 || slot >= GetPreviousVisibleSlot(this.DisplayData.FirstScrollingSlot) && slot <= GetNextVisibleSlot(this.DisplayData.LastScrollingSlot));
            Debug.Assert(element != null);
            
            if (this._rowsPresenter != null)
            {
                DataGridRowGroupHeader groupHeader = null;
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    LoadRowVisualsForDisplay(row);

                    if (IsRowRecyclable(row))
                    {
                        if (!row.IsRecycled)
                        {
                            Debug.Assert(!this._rowsPresenter.Children.Contains(element));
                            _rowsPresenter.Children.Add(row);
                        }
                    }
                    else
                    {
                        element.Clip = null;
                        Debug.Assert(row.Index == RowIndexFromSlot(slot));
                    }
                }
                else
                {
                    groupHeader = element as DataGridRowGroupHeader;
                    Debug.Assert(groupHeader != null);  // Nothig other and Rows and RowGroups now
                    if (groupHeader != null)
                    {
                        groupHeader.TotalIndent = (groupHeader.Level == 0) ? 0 : this.RowGroupSublevelIndents[groupHeader.Level - 1];
                        if (!groupHeader.IsRecycled)
                        {
                            _rowsPresenter.Children.Add(element);
                        }
                        groupHeader.LoadVisualsForDisplay();

                        Style lastStyle = _rowGroupHeaderStyles.Count > 0 ? _rowGroupHeaderStyles[_rowGroupHeaderStyles.Count - 1] : null;
                        EnsureElementStyle(groupHeader, groupHeader.Style, groupHeader.Level < _rowGroupHeaderStyles.Count ? _rowGroupHeaderStyles[groupHeader.Level] : lastStyle);
                    }
                }

                // Measure the element and update AvailableRowRoom
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                this.AvailableSlotElementRoom -= element.DesiredSize.Height;

                if (groupHeader != null)
                {
                    _rowGroupHeightsByLevel[groupHeader.Level] = groupHeader.DesiredSize.Height;
                }

                if (row != null && this.RowHeightEstimate == DataGrid.DATAGRID_defaultRowHeight && double.IsNaN(row.Height))
                {
                    this.RowHeightEstimate = element.DesiredSize.Height;
                }
            }

            if (wasNewlyAdded)
            {
                this.DisplayData.CorrectSlotsAfterInsertion(slot, element, false /*isCollapsed*/);
            }
            else
            {
                this.DisplayData.LoadScrollingSlot(slot, element, updateSlotInformation);
            }
        }

        private void InsertElement(int slot, UIElement element, bool updateVerticalScrollBarOnly, bool isCollapsed, bool isRow)
        {
            Debug.Assert(slot >= 0 && slot <= this.SlotCount);

            OnInsertingElement(slot, true /*firstInsertion*/, isCollapsed);   // will throw an exception if the insertion is illegal

            OnInsertedElement_Phase1(slot, element, isCollapsed, isRow);
            this.SlotCount++;
            if (!isCollapsed)
            {
                this.VisibleSlotCount++;
            }
            OnInsertedElement_Phase2(slot, updateVerticalScrollBarOnly, isCollapsed);
        }

        private void InvalidateRowHeightEstimate()
        {
            // Start from scratch and assume that we haven't estimated any rows
            _lastEstimatedRow = -1;
        }

        private void OnAddedElement_Phase1(int slot, UIElement element)
        {
            Debug.Assert(slot >= 0);

            // Row needs to be potentially added to the displayed rows
            if (SlotIsDisplayed(slot))
            {
                InsertDisplayedElement(slot, element, true /*wasNewlyAdded*/, true);
            }
        }

        private void OnAddedElement_Phase2(int slot, bool updateVerticalScrollBarOnly)
        {
            if (slot < this.DisplayData.FirstScrollingSlot - 1)
            {
                // The element was added above our viewport so it pushes the VerticalOffset down
                double elementHeight = this.RowGroupHeadersTable.Contains(slot) ? this.RowGroupHeaderHeightEstimate : this.RowHeightEstimate;

                SetVerticalOffset(_verticalOffset + elementHeight);
            }
            if (updateVerticalScrollBarOnly)
            {
                UpdateVerticalScrollBar();
            }
            else
            {
                ComputeScrollBarsLayout();
                // Reposition rows in case we use a recycled one
                InvalidateRowsArrange();
            }
        }

        private void OnElementsChanged(bool grew)
        {
            if (grew &&
                this.ColumnsItemsInternal.Count > 0 &&
                this.CurrentColumnIndex == -1)
            {
                MakeFirstDisplayedCellCurrentCell();
            }
        }

        private void OnInsertedElement_Phase1(int slot, UIElement element, bool isCollapsed, bool isRow)
        {
            Debug.Assert(slot >= 0);

            // Fix the Index of all following rows
            CorrectSlotsAfterInsertion(slot, isCollapsed, isRow);

            // Next, same effect as adding a row
            if (element != null)
            {
#if DEBUG
                DataGridRow dataGridRow = element as DataGridRow;
                if (dataGridRow != null)
                {
                    Debug.Assert(dataGridRow.Cells.Count == this.ColumnsItemsInternal.Count);

                    int columnIndex = 0;
                    foreach (DataGridCell dataGridCell in dataGridRow.Cells)
                    {
                        Debug.Assert(dataGridCell.OwningRow == dataGridRow);
                        Debug.Assert(dataGridCell.OwningColumn == this.ColumnsItemsInternal[columnIndex]);
                        columnIndex++;
                    }
                }
#endif
                Debug.Assert(!isCollapsed);
                OnAddedElement_Phase1(slot, element);
            }
            else if ((slot <= this.DisplayData.FirstScrollingSlot) || (isCollapsed && (slot <= this.DisplayData.LastScrollingSlot)))
            {
                this.DisplayData.CorrectSlotsAfterInsertion(slot, null /*row*/, isCollapsed);
            }
        }

        private void OnInsertedElement_Phase2(int slot, bool updateVerticalScrollBarOnly, bool isCollapsed)
        {
            Debug.Assert(slot >= 0);

            if (!isCollapsed)
            {
                // Same effect as adding a row
                OnAddedElement_Phase2(slot, updateVerticalScrollBarOnly);
            }
        }

        private void OnInsertingElement(int slotInserted,
                                    bool firstInsertion,
                                    bool isCollapsed)
        {
            // Reset the current cell's address if it's after the inserted row.
            if (firstInsertion)
            {
                if (this.CurrentSlot != -1 && slotInserted <= this.CurrentSlot)
                {
                    // The underlying data was already added, therefore we need to avoid accessing any back-end data since we might be off by 1 row.
                    this._temporarilyResetCurrentCell = true;
                    bool success = SetCurrentCellCore(-1, -1);
                    Debug.Assert(success);
                }
            }

            this._showDetailsTable.InsertIndex(slotInserted);
            // Update the slot ranges for the RowGroupHeaders before updating the _selectedItems table,
            // because it's dependent on the slots being correct with regards to grouping.
            this.RowGroupHeadersTable.InsertIndex(slotInserted);
            this._selectedItems.InsertIndex(slotInserted);

            if (isCollapsed)
            {
                _collapsedSlotsTable.InsertIndexAndValue(slotInserted, Visibility.Collapsed);
            }
            else
            {
                _collapsedSlotsTable.InsertIndex(slotInserted);
            }

            // If we've inserted rows before the current selected item, update its index
            if (slotInserted <= this.SelectedIndex)
            {
                this.SetValueNoCallback(SelectedIndexProperty, this.SelectedIndex + 1);
            }
        }

        private void OnRemovedElement(int slotDeleted, object itemDeleted)
        {
            this.SlotCount--;
            bool wasCollapsed = this._collapsedSlotsTable.Contains(slotDeleted);
            if (!wasCollapsed)
            {
                this.VisibleSlotCount--;
            }

            // If we're deleting the focused row, we need to clear the cached value
            if (this._focusedRow != null && this._focusedRow.Slot == slotDeleted)
            {
                ResetFocusedRow();
            }

            // The element needs to be potentially removed from the displayed elements
            UIElement elementDeleted = null;
            if (slotDeleted <= this.DisplayData.LastScrollingSlot)
            {
                if ((slotDeleted >= this.DisplayData.FirstScrollingSlot) && !wasCollapsed)
                {
                    elementDeleted = this.DisplayData.GetDisplayedElement(slotDeleted);
                    // We need to retrieve the Element before updating the tables, but we need
                    // to update the tables before updating DisplayData in RemoveDisplayedElement
                    UpdateTablesForRemoval(slotDeleted, itemDeleted);

                    // Displayed row is removed
                    RemoveDisplayedElement(elementDeleted, slotDeleted, true /*wasDeleted*/, true /*updateSlotInformation*/);
                }
                else
                {
                    UpdateTablesForRemoval(slotDeleted, itemDeleted);

                    // Removed row is not in view, just update the DisplayData
                    this.DisplayData.CorrectSlotsAfterDeletion(slotDeleted, wasCollapsed);
                }
            }
            else
            {
                // The element was removed beyond the viewport so we just need to update the tables
                UpdateTablesForRemoval(slotDeleted, itemDeleted);
            }

            // If a row was removed before the currently selected row, update its index
            if (slotDeleted < this.SelectedIndex)
            {
                this.SetValueNoCallback(SelectedIndexProperty, this.SelectedIndex - 1);
            }

            if (!wasCollapsed)
            {
                if (slotDeleted >= this.DisplayData.LastScrollingSlot && elementDeleted == null)
                {
                    // Deleted Row is below our Viewport, we just need to adjust the scrollbar
                    UpdateVerticalScrollBar();
                }
                else
                {
                    if (elementDeleted != null)
                    {
                        // Deleted Row is within our Viewport, update the AvailableRowRoom
                        this.AvailableSlotElementRoom += elementDeleted.DesiredSize.Height;
                    }
                    else
                    {
                        // Deleted Row is above our Viewport, update the vertical offset
                        SetVerticalOffset(Math.Max(0, _verticalOffset - this.RowHeightEstimate));
                    }

                    ComputeScrollBarsLayout();
                    // Reposition rows in case we use a recycled one
                    InvalidateRowsArrange();
                }
            }
        }

        private void OnRemovingElement(int slotDeleted)
        {
            // Note that the row needs to be deleted no matter what. The underlying data row was already deleted.

            Debug.Assert(slotDeleted >= 0 && slotDeleted < this.SlotCount);
            this._temporarilyResetCurrentCell = false; 

            // Reset the current cell's address if it's on the deleted row, or after it.
            if (this.CurrentSlot != -1 && slotDeleted <= this.CurrentSlot)
            {
                this._desiredCurrentColumnIndex = this.CurrentColumnIndex;
                if (slotDeleted == this.CurrentSlot)
                {
                    // No editing is committed since the underlying entity was already deleted.
                    bool success = SetCurrentCellCore(-1, -1, false /*commitEdit*/, false /*endRowEdit*/);
                    Debug.Assert(success);
                }
                else
                {
                    // Underlying data of deleted row is gone. It cannot be accessed anymore. Skip the commit of the editing.
                    this._temporarilyResetCurrentCell = true;
                    bool success = SetCurrentCellCore(-1, -1);
                    Debug.Assert(success);
                }
            }
        }

        // Makes sure the row shows the proper visuals for selection, currency, details, etc.
        private void LoadRowVisualsForDisplay(DataGridRow row)
        {
            // If the row has been recycled, reapply the BackgroundBrush
            if (row.IsRecycled)
            {
                row.EnsureBackground();
                row.ApplyCellsState(false /*animate*/);
            }
            else if (row == this.EditingRow)
            {
                // 

                row.ApplyCellsState(false /*animate*/);
            }

            // Set the Row's Style if we one's defined at the DataGrid level and the user didn't
            // set one at the row level
            EnsureElementStyle(row, null, this.RowStyle);
            row.EnsureHeaderStyleAndVisibility(null);

            // Check to see if the row contains the CurrentCell, apply its state.
            if (this.CurrentColumnIndex != -1 &&
                this.CurrentSlot != -1 &&
                row.Index == this.CurrentSlot)
            {
                row.Cells[this.CurrentColumnIndex].ApplyCellState(false /*animate*/);
            }

            if (row.IsSelected || row.IsRecycled)
            {
                row.ApplyState(false);
            }

            // Show or hide RowDetails based on DataGrid settings
            EnsureRowDetailsVisibility(row, false /*raiseNotification*/, false /*animate*/);
        }

        private void PopulateRowGroupHeadersTable()
        {
            if (this.DataConnection.CollectionView != null
                && this.DataConnection.CollectionView.CanGroup
                && this.DataConnection.CollectionView.Groups != null)
            {
                int totalSlots = 0;
                this._topLevelGroup = (INotifyCollectionChanged)this.DataConnection.CollectionView.Groups;
                this._topLevelGroup.CollectionChanged += CollectionViewGroup_CollectionChanged;
                foreach (object group in this.DataConnection.CollectionView.Groups)
                {
                    totalSlots += CountAndPopulateGroupHeaders(group, totalSlots, 0);
                }
            }
            this.SlotCount = this.DataConnection.Count + this.RowGroupHeadersTable.IndexCount;
            this.VisibleSlotCount = this.SlotCount;
        }

        private void RefreshRowGroupHeaders()
        {
            if (this.DataConnection.CollectionView != null
                && this.DataConnection.CollectionView.CanGroup
                && this.DataConnection.CollectionView.Groups != null
                && this.DataConnection.CollectionView.GroupDescriptions != null
                && this.DataConnection.CollectionView.GroupDescriptions.Count > 0)
            {
                // Initialize our array for the height of the RowGroupHeaders by Level.
                // If the Length is the same, we can reuse the old array
                int groupLevelCount = this.DataConnection.CollectionView.GroupDescriptions.Count;
                if (_rowGroupHeightsByLevel == null || _rowGroupHeightsByLevel.Length != groupLevelCount)
                {
                    _rowGroupHeightsByLevel = new double[groupLevelCount];
                    for (int i = 0; i < groupLevelCount; i++)
                    {
                        // Default height for now, the actual heights are updated as the RowGroupHeaders
                        // are added and measured
                        _rowGroupHeightsByLevel[i] = DATAGRID_defaultRowHeight;
                    }
                }
                if (this.RowGroupSublevelIndents == null || this.RowGroupSublevelIndents.Length != groupLevelCount)
                {
                    this.RowGroupSublevelIndents = new double[groupLevelCount];
                    double indent;
                    for (int i = 0; i < groupLevelCount; i++)
                    {
                        DataGridRowGroupHeader rowGroupHeader = null;
                        indent = DATAGRID_defaultRowGroupSublevelIndent;
                        if (i < this.RowGroupHeaderStyles.Count && this.RowGroupHeaderStyles[i] != null)
                        {
                            // Due to Silverlight Bugs 22038, we have to actually set the Style to read
                            // setter values instead of simply enumerating through the setters
                            if (rowGroupHeader == null)
                            {
                                rowGroupHeader = new DataGridRowGroupHeader();
                            }
                            rowGroupHeader.Style = this.RowGroupHeaderStyles[i];
                            if (rowGroupHeader.SublevelIndent != DataGrid.DATAGRID_defaultRowGroupSublevelIndent)
                            {
                                indent = rowGroupHeader.SublevelIndent;
                            }
                        }
                        this.RowGroupSublevelIndents[i] = indent;
                        if (i > 0)
                        {
                            this.RowGroupSublevelIndents[i] += this.RowGroupSublevelIndents[i - 1];
                        }
                    }
                }
                EnsureRowGroupSpacerColumnWidth(groupLevelCount);
            }
        }

        private void RemoveDisplayedElement(int slot, bool wasDeleted, bool updateSlotInformation)
        {
            Debug.Assert(slot >= this.DisplayData.FirstScrollingSlot &&
                         slot <= this.DisplayData.LastScrollingSlot);

            RemoveDisplayedElement(this.DisplayData.GetDisplayedElement(slot), slot, wasDeleted, updateSlotInformation);
        }

        // Removes an element from display either because it was deleted or it was scrolled out of view.
        // If the element was provided, it will be the element removed; otherwise, the element will be
        // retrieved from the slot information
        private void RemoveDisplayedElement(UIElement element, int slot, bool wasDeleted, bool updateSlotInformation)
        {
            DataGridRow dataGridRow = element as DataGridRow;
            if (dataGridRow != null)
            {
                if (IsRowRecyclable(dataGridRow))
                {
                    UnloadRow(dataGridRow);
                }
                else
                {
                    dataGridRow.Clip = new RectangleGeometry();
                }
            }
            else
            {
                DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                if (groupHeader != null)
                {
                    OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                    this.DisplayData.AddRecylableRowGroupHeader(groupHeader);
                }
                else if (_rowsPresenter != null)
                {
                    _rowsPresenter.Children.Remove(element);
                }
            }

            // Update DisplayData
            if (wasDeleted)
            {
                this.DisplayData.CorrectSlotsAfterDeletion(slot, false /*wasCollapsed*/);
            }
            else
            {
                this.DisplayData.UnloadScrollingElement(slot, updateSlotInformation, false /*wasDeleted*/);
            }
        }

        /// <summary>
        /// Removes all of the editing elements for the row that is just leaving editing mode.
        /// </summary>
        private void RemoveEditingElements()
        {
            if (this.EditingRow != null && this.EditingRow.Cells != null)
            {
                Debug.Assert(this.EditingRow.Cells.Count == this.ColumnsItemsInternal.Count);
                foreach (DataGridColumn column in this.Columns)
                {
                    column.RemoveEditingElement();
                }
            }
        }

        private void RemoveElementAt(int slot, object item, bool isRow)
        {
            Debug.Assert(slot >= 0 && slot < this.SlotCount);

            OnRemovingElement(slot);

            CorrectSlotsAfterDeletion(slot, isRow);

            OnRemovedElement(slot, item);
        }

        private void RemoveNonDisplayedRows(int newFirstDisplayedSlot, int newLastDisplayedSlot)
        {
            while (this.DisplayData.FirstScrollingSlot < newFirstDisplayedSlot)
            {
                // Need to add rows above the lastDisplayedScrollingRow
                RemoveDisplayedElement(this.DisplayData.FirstScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
            }
            while (this.DisplayData.LastScrollingSlot > newLastDisplayedSlot)
            {
                // Need to remove rows below the lastDisplayedScrollingRow
                RemoveDisplayedElement(this.DisplayData.LastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
            }
        }

        private void ResetDisplayedRows()
        {
            if (this.UnloadingRow != null || this.UnloadingRowGroup != null)
            {
                foreach (UIElement element in this.DisplayData.GetScrollingElements())
                {
                    // Raise Unloading Row for all the rows we're displaying
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        if (IsRowRecyclable(row))
                        {
                            OnUnloadingRow(new DataGridRowEventArgs(row));
                        }
                    }
                    else 
                    {
                        // Raise Unloading Row for all the RowGroupHeaders we're displaying
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                        }
                    }
                }
            }
            this.DisplayData.ClearElements(true /*recycleRows*/);
            this.AvailableSlotElementRoom = this.CellsHeight;
        }

        /// <summary>
        /// Determines whether the row at the provided index must be displayed or not.
        /// </summary>
        private bool SlotIsDisplayed(int slot)
        {
            Debug.Assert(slot >= 0);

            if (slot >= this.DisplayData.FirstScrollingSlot &&
                slot <= this.DisplayData.LastScrollingSlot)
            {
                // Additional row takes the spot of a displayed row - it is necessarilly displayed
                return true;
            }
            else if (this.DisplayData.FirstScrollingSlot == -1 &&
                     this.CellsHeight > 0 &&
                     this.CellsWidth > 0)
            {
                return true;
            }
            else if (slot == GetNextVisibleSlot(this.DisplayData.LastScrollingSlot))
            {
                if (this.AvailableSlotElementRoom > 0)
                {
                    // There is room for this additional row
                    return true;
                }
            }
            return false;
        }

        // Updates display information and displayed rows after scrolling the given number of pixels
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ScrollSlotsByHeight(double height)
        {
            Debug.Assert(this.DisplayData.FirstScrollingSlot >= 0);
            Debug.Assert(!DoubleUtil.IsZero(height));

            _scrollingByHeight = true;
            try
            {
                double deltaY = 0;
                int newFirstScrollingSlot = this.DisplayData.FirstScrollingSlot;
                double newVerticalOffset = this._verticalOffset + height;
                if (height > 0)
                {
                    // Scrolling Down
                    int lastVisibleSlot = GetPreviousVisibleSlot(this.SlotCount);
                    if (this._vScrollBar != null && DoubleUtil.AreClose(this._vScrollBar.Maximum, newVerticalOffset))
                    {
                        // We've scrolled to the bottom of the ScrollBar, automatically place the user at the very bottom
                        // of the DataGrid.  If this produces very odd behavior, evaluate the coping strategy used by
                        // OnRowMeasure(Size).  For most data, this should be unnoticeable.
                        ResetDisplayedRows();
                        UpdateDisplayedRowsFromBottom(lastVisibleSlot);
                        newFirstScrollingSlot = this.DisplayData.FirstScrollingSlot;
                    }
                    else
                    {
                        deltaY = GetSlotElementHeight(newFirstScrollingSlot) - this.NegVerticalOffset;
                        if (DoubleUtil.LessThan(height, deltaY))
                        {
                            // We've merely covered up more of the same row we're on
                            this.NegVerticalOffset += height;
                        }
                        else
                        {
                            // Figure out what row we've scrolled down to and update the value for this.NegVerticalOffset
                            this.NegVerticalOffset = 0;
                            // 

                            if (height > 2 * this.CellsHeight &&
                                (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.VisibleWhenSelected || this.RowDetailsTemplate == null))
                            {
                                // Very large scroll occurred. Instead of determining the exact number of scrolled off rows,
                                // let's estimate the number based on this.RowHeight.
                                ResetDisplayedRows();
                                double singleRowHeightEstimate = this.RowHeightEstimate + (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible ? this.RowDetailsHeightEstimate : 0);
                                int scrolledToSlot = newFirstScrollingSlot + (int)(height / singleRowHeightEstimate);
                                scrolledToSlot += _collapsedSlotsTable.GetIndexCount(newFirstScrollingSlot, newFirstScrollingSlot + scrolledToSlot);
                                newFirstScrollingSlot = Math.Min(GetNextVisibleSlot(scrolledToSlot), lastVisibleSlot);
                            }
                            else
                            {
                                while (DoubleUtil.LessThanOrClose(deltaY, height))
                                {
                                    if (newFirstScrollingSlot < lastVisibleSlot)
                                    {
                                        if (this.IsSlotVisible(newFirstScrollingSlot))
                                        {
                                            // Make the top row available for reuse
                                            RemoveDisplayedElement(newFirstScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                                        }
                                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                                    }
                                    else
                                    {
                                        // We're being told to scroll beyond the last row, ignore the extra
                                        this.NegVerticalOffset = 0;
                                        break;
                                    }

                                    double rowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                                    double remainingHeight = height - deltaY;
                                    if (DoubleUtil.LessThanOrClose(rowHeight, remainingHeight))
                                    {
                                        deltaY += rowHeight;
                                    }
                                    else
                                    {
                                        this.NegVerticalOffset = remainingHeight;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Scrolling Up
                    if (DoubleUtil.GreaterThanOrClose(height + this.NegVerticalOffset, 0))
                    {
                        // We've merely exposing more of the row we're on
                        this.NegVerticalOffset += height;
                    }
                    else
                    {
                        // Figure out what row we've scrolled up to and update the value for this.NegVerticalOffset
                        deltaY = -this.NegVerticalOffset;
                        this.NegVerticalOffset = 0;
                        // 

                        if (height < -2 * this.CellsHeight &&
                            (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.VisibleWhenSelected || this.RowDetailsTemplate == null))
                        {
                            // Very large scroll occurred. Instead of determining the exact number of scrolled off rows,
                            // let's estimate the number based on this.RowHeight.
                            if (newVerticalOffset == 0)
                            {
                                newFirstScrollingSlot = 0;
                            }
                            else
                            {
                                double singleRowHeightEstimate = this.RowHeightEstimate + (this.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible ? this.RowDetailsHeightEstimate : 0);
                                int scrolledToSlot = newFirstScrollingSlot + (int)(height / singleRowHeightEstimate);
                                scrolledToSlot -= _collapsedSlotsTable.GetIndexCount(scrolledToSlot, newFirstScrollingSlot);

                                newFirstScrollingSlot = Math.Max(0, GetPreviousVisibleSlot(scrolledToSlot + 1));
                            }
                            ResetDisplayedRows();
                        }
                        else
                        {
                            int lastScrollingSlot = this.DisplayData.LastScrollingSlot;
                            while (DoubleUtil.GreaterThan(deltaY, height))
                            {
                                if (newFirstScrollingSlot > 0)
                                {
                                    if (this.IsSlotVisible(lastScrollingSlot))
                                    {
                                        // Make the bottom row available for reuse
                                        RemoveDisplayedElement(lastScrollingSlot, false /*wasDeleted*/, true /*updateSlotInformation*/);
                                        lastScrollingSlot = GetPreviousVisibleSlot(lastScrollingSlot);
                                    }
                                    newFirstScrollingSlot = GetPreviousVisibleSlot(newFirstScrollingSlot);
                                }
                                else
                                {
                                    this.NegVerticalOffset = 0;
                                    break;
                                }
                                double rowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                                double remainingHeight = height - deltaY;
                                if (DoubleUtil.LessThanOrClose(rowHeight + remainingHeight, 0))
                                {
                                    deltaY -= rowHeight;
                                }
                                else
                                {
                                    this.NegVerticalOffset = rowHeight + remainingHeight;
                                    break;
                                }
                            }
                        }
                    }
                    if (DoubleUtil.GreaterThanOrClose(0, newVerticalOffset) && newFirstScrollingSlot != 0)
                    {
                        // We've scrolled to the top of the ScrollBar, automatically place the user at the very top
                        // of the DataGrid.  If this produces very odd behavior, evaluate the RowHeight estimate.  
                        // strategy. For most data, this should be unnoticeable.
                        ResetDisplayedRows();
                        this.NegVerticalOffset = 0;
                        UpdateDisplayedRows(0, this.CellsHeight);
                        newFirstScrollingSlot = 0;
                    }
                }

                double firstRowHeight = GetExactSlotElementHeight(newFirstScrollingSlot);
                if (DoubleUtil.LessThan(firstRowHeight, this.NegVerticalOffset))
                {
                    // We've scrolled off more of the first row than what's possible.  This can happen
                    // if the first row got shorter (Ex: Collpasing RowDetails) or if the user has a recycling
                    // cleanup issue.  In this case, simply try to display the next row as the first row instead
                    if (newFirstScrollingSlot < this.SlotCount - 1)
                    {
                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                        Debug.Assert(newFirstScrollingSlot != -1);
                    }
                    this.NegVerticalOffset = 0;
                }

                UpdateDisplayedRows(newFirstScrollingSlot, this.CellsHeight);

                double firstElementHeight = GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot);
                if (DoubleUtil.GreaterThan(this.NegVerticalOffset, firstElementHeight))
                {
                    int firstElementSlot = this.DisplayData.FirstScrollingSlot;
                    // We filled in some rows at the top and now we have a NegVerticalOffset that's greater than the first element
                    while (newFirstScrollingSlot > 0 && DoubleUtil.GreaterThan(this.NegVerticalOffset, firstElementHeight))
                    {
                        int previousSlot = GetPreviousVisibleSlot(firstElementSlot);
                        if (previousSlot == -1)
                        {
                            this.NegVerticalOffset = 0;
                            this._verticalOffset = 0;
                        }
                        else
                        {
                            this.NegVerticalOffset -= firstElementHeight;
                            this._verticalOffset = Math.Max(0, this._verticalOffset - firstElementHeight);
                            firstElementSlot = previousSlot;
                            firstElementHeight = GetExactSlotElementHeight(firstElementSlot);
                        }
                    }
                    // We could be smarter about this, but it's not common so we wouldn't gain much from optimizing here
                    if (firstElementSlot != this.DisplayData.FirstScrollingSlot)
                    {
                        UpdateDisplayedRows(firstElementSlot, this.CellsHeight);
                    }
                }

                Debug.Assert(this.DisplayData.FirstScrollingSlot >= 0);
                Debug.Assert(GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot) > this.NegVerticalOffset);

                if (this.DisplayData.FirstScrollingSlot == 0)
                {
                    this._verticalOffset = this.NegVerticalOffset;
                }
                else if (DoubleUtil.GreaterThan(this.NegVerticalOffset, newVerticalOffset))
                {
                    // The scrolled-in row was larger than anticipated. Adjust the DataGrid so the ScrollBar thumb
                    // can stay in the same place
                    this.NegVerticalOffset = newVerticalOffset;
                    this._verticalOffset = newVerticalOffset;
                }
                else
                {
                    this._verticalOffset = newVerticalOffset;
                }

                Debug.Assert(!(this._verticalOffset == 0 && this.NegVerticalOffset == 0 && this.DisplayData.FirstScrollingSlot > 0));

                SetVerticalOffset(_verticalOffset);

                this.DisplayData.FullyRecycleElements();

                Debug.Assert(DoubleUtil.GreaterThanOrClose(this.NegVerticalOffset, 0));
                Debug.Assert(DoubleUtil.GreaterThanOrClose(this._verticalOffset, this.NegVerticalOffset));

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
            finally
            {
                _scrollingByHeight = false;
            }
        }

        private void SelectDisplayedElement(int slot)
        {
            Debug.Assert(IsSlotVisible(slot));
            FrameworkElement element = this.DisplayData.GetDisplayedElement(slot) as FrameworkElement;
            DataGridRow row = this.DisplayData.GetDisplayedElement(slot) as DataGridRow;
            if (row != null)
            {
                row.ApplyState(true /*animate*/);
                EnsureRowDetailsVisibility(row, true /*raiseNotification*/, true /*animate*/);
            }
            else
            {
                // Assume it's a RowGroupHeader
                DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                groupHeader.ApplyState(true /*useTransitions*/);
            }
        }

        private void SelectSlot(int slot, bool isSelected)
        {
            _selectedItems.SelectSlot(slot, isSelected);
            if (this.IsSlotVisible(slot))
            {
                SelectDisplayedElement(slot);
            }
        }

        private void SelectSlots(int startSlot, int endSlot, bool isSelected)
        {
            _selectedItems.SelectSlots(startSlot, endSlot, isSelected);
            
            // Apply the correct row state for display rows and also expand or collapse detail accordingly
            int firstSlot = Math.Max(this.DisplayData.FirstScrollingSlot, startSlot);
            int lastSlot = Math.Min(this.DisplayData.LastScrollingSlot, endSlot);

            for (int slot = firstSlot; slot <= lastSlot; slot++)
            {
                if (IsSlotVisible(slot))
                {
                    SelectDisplayedElement(slot);
                }
            }
        }

        private void UnloadElements(bool recycle)
        {
            // Since we're unloading all the elements, we can't be in editing mode anymore,
            // so commit if we can, otherwise force cancel.
            if (!this.CommitEdit())
            {
                this.CancelEdit(DataGridEditingUnit.Row, false);
            }
            this.ResetEditingRow();

            // Make sure to clear the focused row (because it's no longer relevant).
            if (this._focusedRow != null)
            {
                ResetFocusedRow();
                Focus();
            }

            if (this._rowsPresenter != null)
            {
                foreach (UIElement element in this._rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        // Raise UnloadingRow for any row that was visible
                        if (this.IsSlotVisible(row.Slot))
                        {
                            OnUnloadingRow(new DataGridRowEventArgs(row));
                        }
                        row.DetachFromDataGrid(recycle && row.IsRecyclable /*recycle*/);
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            if (this.IsSlotVisible(groupHeader.RowGroupInfo.Slot))
                            {
                                OnUnloadingRowGroup(new DataGridRowGroupHeaderEventArgs(groupHeader));
                            }
                        }
                    }
                }

                if (!recycle)
                {
                    this._rowsPresenter.Children.Clear();
                }
            }
            this.DisplayData.ClearElements(recycle);

            // Update the AvailableRowRoom since we're displaying 0 rows now
            this.AvailableSlotElementRoom = this.CellsHeight;
            this.VisibleSlotCount = 0;
        }

        private void UnloadRow(DataGridRow dataGridRow)
        {
            Debug.Assert(dataGridRow != null);
            Debug.Assert(this._rowsPresenter != null);
            Debug.Assert(this._rowsPresenter.Children.Contains(dataGridRow));

            if (this._loadedRows.Contains(dataGridRow))
            {
                return; // The row is still referenced, we can't release it.
            }

            // Raise UnloadingRow regardless of whether the row will be recycled
            OnUnloadingRow(new DataGridRowEventArgs(dataGridRow));

            // 
            bool recycleRow = this.CurrentSlot != dataGridRow.Index;

            // Don't recycle if the row has a custom Style set
            recycleRow &= (dataGridRow.Style == null || dataGridRow.Style == this.RowStyle);

            if (recycleRow)
            {
                this.DisplayData.AddRecylableRow(dataGridRow);
            }
            else
            {
                // 
                this._rowsPresenter.Children.Remove(dataGridRow);
                dataGridRow.DetachFromDataGrid(false);
            }
        }
        
        private void UpdateDisplayedRows(int newFirstDisplayedSlot, double displayHeight)
        {
            Debug.Assert(!_collapsedSlotsTable.Contains(newFirstDisplayedSlot));
            int firstDisplayedScrollingSlot = newFirstDisplayedSlot;
            int lastDisplayedScrollingSlot = -1;
            double deltaY = -this.NegVerticalOffset;
            int visibleScrollingRows = 0;

            if (DoubleUtil.LessThanOrClose(displayHeight, 0) || this.SlotCount == 0 || this.ColumnsItemsInternal.Count == 0)
            {
                return;
            }

            if (firstDisplayedScrollingSlot == -1)
            {
                // 0 is fine because the element in the first slot cannot be collapsed
                firstDisplayedScrollingSlot = 0;
            }

            int slot = firstDisplayedScrollingSlot;
            while (slot < this.SlotCount && !DoubleUtil.GreaterThanOrClose(deltaY, displayHeight))
            {
                deltaY += GetExactSlotElementHeight(slot);
                visibleScrollingRows++;
                lastDisplayedScrollingSlot = slot;
                slot = GetNextVisibleSlot(slot);
            }

            while (DoubleUtil.LessThan(deltaY, displayHeight) && slot >= 0)
            {
                slot = GetPreviousVisibleSlot(firstDisplayedScrollingSlot);
                if (slot >= 0)
                {
                    deltaY += GetExactSlotElementHeight(slot);
                    firstDisplayedScrollingSlot = slot;
                    visibleScrollingRows++;
                }
            }
            // If we're up to the first row, and we still have room left, uncover as much of the first row as we can
            if (firstDisplayedScrollingSlot == 0 && DoubleUtil.LessThan(deltaY, displayHeight))
            {
                double newNegVerticalOffset = Math.Max(0, this.NegVerticalOffset - displayHeight + deltaY);
                deltaY += this.NegVerticalOffset - newNegVerticalOffset;
                this.NegVerticalOffset = newNegVerticalOffset;
            }

            if (DoubleUtil.GreaterThan(deltaY, displayHeight) || (DoubleUtil.AreClose(deltaY, displayHeight) && DoubleUtil.GreaterThan(this.NegVerticalOffset, 0)))
            {
                this.DisplayData.NumTotallyDisplayedScrollingElements = visibleScrollingRows - 1;
            }
            else
            {
                this.DisplayData.NumTotallyDisplayedScrollingElements = visibleScrollingRows;
            }
            if (visibleScrollingRows == 0)
            {
                firstDisplayedScrollingSlot = -1;
                Debug.Assert(lastDisplayedScrollingSlot == -1);
            }

            Debug.Assert(lastDisplayedScrollingSlot < this.SlotCount, "lastDisplayedScrollingRow larger than number of rows");

            RemoveNonDisplayedRows(firstDisplayedScrollingSlot, lastDisplayedScrollingSlot);

            Debug.Assert(this.DisplayData.NumDisplayedScrollingElements >= 0, "the number of visible scrolling rows can't be negative");
            Debug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0, "the number of totally visible scrolling rows can't be negative");
            Debug.Assert(this.DisplayData.FirstScrollingSlot < this.SlotCount, "firstDisplayedScrollingRow larger than number of rows");
            Debug.Assert(this.DisplayData.FirstScrollingSlot == firstDisplayedScrollingSlot);
            Debug.Assert(this.DisplayData.LastScrollingSlot == lastDisplayedScrollingSlot);
        }

        // Similar to UpdateDisplayedRows except that it starts with the LastDisplayedScrollingRow
        // and computes the FirstDisplayScrollingRow instead of doing it the other way around.  We use this
        // when scrolling down to a full row
        private void UpdateDisplayedRowsFromBottom(int newLastDisplayedScrollingRow)
        {
            Debug.Assert(!_collapsedSlotsTable.Contains(newLastDisplayedScrollingRow));

            int lastDisplayedScrollingRow = newLastDisplayedScrollingRow;
            int firstDisplayedScrollingRow = -1;
            double displayHeight = this.CellsHeight;
            double deltaY = 0;
            int visibleScrollingRows = 0;

            if (DoubleUtil.LessThanOrClose(displayHeight, 0) || this.SlotCount == 0 || this.ColumnsItemsInternal.Count == 0)
            {
                this.ResetDisplayedRows();
                return;
            }

            if (lastDisplayedScrollingRow == -1)
            {
                lastDisplayedScrollingRow = 0;
            }

            int slot = lastDisplayedScrollingRow;
            while (DoubleUtil.LessThan(deltaY, displayHeight) && slot >= 0)
            {
                deltaY += GetExactSlotElementHeight(slot);
                visibleScrollingRows++;
                firstDisplayedScrollingRow = slot;
                slot = GetPreviousVisibleSlot(slot);
            }

            this.DisplayData.NumTotallyDisplayedScrollingElements = deltaY > displayHeight ? visibleScrollingRows - 1 : visibleScrollingRows;

            Debug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0);
            Debug.Assert(lastDisplayedScrollingRow < this.SlotCount, "lastDisplayedScrollingRow larger than number of rows");

            this.NegVerticalOffset = Math.Max(0, deltaY - displayHeight);

            RemoveNonDisplayedRows(firstDisplayedScrollingRow, lastDisplayedScrollingRow);

            Debug.Assert(this.DisplayData.NumDisplayedScrollingElements >= 0, "the number of visible scrolling rows can't be negative");
            Debug.Assert(this.DisplayData.NumTotallyDisplayedScrollingElements >= 0, "the number of totally visible scrolling rows can't be negative");
            Debug.Assert(this.DisplayData.FirstScrollingSlot < this.SlotCount, "firstDisplayedScrollingRow larger than number of rows");
        }

        private void UpdateRowDetailsHeightEstimate()
        {
            if (_rowsPresenter != null && _measured && this.RowDetailsTemplate != null)
            {
                FrameworkElement detailsContent = this.RowDetailsTemplate.LoadContent() as FrameworkElement;
                if (detailsContent != null)
                {
                    _rowsPresenter.Children.Add(detailsContent);
                    if (this.VisibleSlotCount > 0)
                    {
                        detailsContent.DataContext = this.DataConnection.GetDataItem(0);
                    }
                    detailsContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.RowDetailsHeightEstimate = detailsContent.DesiredSize.Height;
                    _rowsPresenter.Children.Remove(detailsContent);
                }
            }
        }

        // This method does not check the state of the parent RowGroupHeaders, it assumes they're ready for this newVisibility to
        // be applied this header
        // Returns the number of pixels that were expanded or (collapsed); however, if we're expanding displayed rows, we only expand up
        // to what we can display
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private double UpdateRowGroupVisibility(DataGridRowGroupInfo targetRowGroupInfo, Visibility newVisibility, bool isDisplayed)
        {
            double heightChange = 0;
            int slotsExpanded = 0;
            int startSlot = targetRowGroupInfo.Slot + 1;
            int endSlot;

            targetRowGroupInfo.Visibility = newVisibility;
            if (newVisibility == Visibility.Visible)
            {
                // Expand
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
                {
                    if (slot >= startSlot)
                    {
                        DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                        if (rowGroupInfo.Level <= targetRowGroupInfo.Level)
                        {
                            break;
                        }
                        if (rowGroupInfo.Visibility == Visibility.Collapsed)
                        {
                            // Skip over the items in collapsed subgroups
                            endSlot = rowGroupInfo.Slot;
                            ExpandSlots(startSlot, endSlot, isDisplayed, ref slotsExpanded, ref heightChange);
                            startSlot = rowGroupInfo.LastSubItemSlot + 1;
                        }
                    }
                }
                if (targetRowGroupInfo.LastSubItemSlot >= startSlot)
                {
                    ExpandSlots(startSlot, targetRowGroupInfo.LastSubItemSlot, isDisplayed, ref slotsExpanded, ref heightChange);
                }
                if (isDisplayed)
                {
                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
                }
            }
            else
            {
                // Collapse
                endSlot = this.SlotCount - 1;
                foreach (int slot in this.RowGroupHeadersTable.GetIndexes(targetRowGroupInfo.Slot + 1))
                {
                    DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(slot);
                    if (rowGroupInfo.Level <= targetRowGroupInfo.Level)
                    {
                        endSlot = slot - 1;
                        break;
                    }
                }

                int oldLastDisplayedSlot = this.DisplayData.LastScrollingSlot;
                int endDisplayedSlot = Math.Min(endSlot, this.DisplayData.LastScrollingSlot);
                if (isDisplayed)
                {
                    // We need to remove all the displayed slots that aren't already collapsed
                    int elementsToRemove = endDisplayedSlot - startSlot + 1 - _collapsedSlotsTable.GetIndexCount(startSlot, endDisplayedSlot);

                    if (_focusedRow != null && _focusedRow.Slot >= startSlot && _focusedRow.Slot <= endSlot)
                    {
                        Debug.Assert(this.EditingRow == null);
                        // Don't call ResetFocusedRow here because we're already cleaning it up below, and we don't want to FullyRecycle yet
                        _focusedRow = null;
                    }

                    for (int i = 0; i < elementsToRemove; i++)
                    {
                        RemoveDisplayedElement(startSlot, false /*wasDeleted*/, false /*updateSlotInformation*/);
                    }
                }

                double heightChangeBelowLastDisplayedSlot = 0;
                if (this.DisplayData.FirstScrollingSlot >= startSlot && this.DisplayData.FirstScrollingSlot <= endSlot)
                {
                    // Our first visible slot was collapsed, find the replacement
                    int collapsedSlotsAbove = this.DisplayData.FirstScrollingSlot - startSlot - _collapsedSlotsTable.GetIndexCount(startSlot, this.DisplayData.FirstScrollingSlot);
                    Debug.Assert(collapsedSlotsAbove > 0);
                    int newFirstScrollingSlot = GetNextVisibleSlot(this.DisplayData.FirstScrollingSlot);
                    while (collapsedSlotsAbove > 1 && newFirstScrollingSlot < this.SlotCount)
                    {
                        collapsedSlotsAbove--;
                        newFirstScrollingSlot = GetNextVisibleSlot(newFirstScrollingSlot);
                    }
                    heightChange += CollapseSlotsInTable(startSlot, endSlot, ref slotsExpanded, oldLastDisplayedSlot, ref heightChangeBelowLastDisplayedSlot);
                    if (isDisplayed)
                    {
                        if (newFirstScrollingSlot >= this.SlotCount)
                        {
                            // No visible slots below, look up
                            UpdateDisplayedRowsFromBottom(targetRowGroupInfo.Slot);
                        }
                        else
                        {
                            UpdateDisplayedRows(newFirstScrollingSlot, this.CellsHeight);
                        }
                    }
                }
                else
                {
                    heightChange += CollapseSlotsInTable(startSlot, endSlot, ref slotsExpanded, oldLastDisplayedSlot, ref heightChangeBelowLastDisplayedSlot);
                }

                if (this.DisplayData.LastScrollingSlot >= startSlot && this.DisplayData.LastScrollingSlot <= endSlot)
                {
                    // Collapsed the last scrolling row, we need to update it
                    this.DisplayData.LastScrollingSlot = GetPreviousVisibleSlot(this.DisplayData.LastScrollingSlot);
                }

                // Collapsing could cause the vertical offset to move up if we collapsed a lot of slots
                // near the bottom of the DataGrid.  To do this, we compare the height we collapsed to
                // the distance to the last visible row and adjust the scrollbar if we collapsed more
                if (isDisplayed && this._verticalOffset > 0)
                {
                    int lastVisibleSlot = GetPreviousVisibleSlot(this.SlotCount);
                    int slot = GetNextVisibleSlot(oldLastDisplayedSlot);
                    // AvailableSlotElementRoom ends up being the amount of the last slot that is partially scrolled off
                    // as a negative value, heightChangeBelowLastDisplayed slot is also a negative value since we're collapsing
                    double heightToLastVisibleSlot = this.AvailableSlotElementRoom + heightChangeBelowLastDisplayedSlot;
                    while ((heightToLastVisibleSlot > heightChange) && (slot < lastVisibleSlot))
                    {
                        heightToLastVisibleSlot -= GetSlotElementHeight(slot);
                        slot = GetNextVisibleSlot(slot);
                    }
                    if (heightToLastVisibleSlot > heightChange)
                    {
                        double newVerticalOffset = _verticalOffset + heightChange - heightToLastVisibleSlot;
                        if (newVerticalOffset > 0)
                        {
                            SetVerticalOffset(newVerticalOffset);
                        }
                        else
                        {
                            // Collapsing causes the vertical offset to go to 0 so we should go back to the first row.
                            ResetDisplayedRows();
                            this.NegVerticalOffset = 0;
                            SetVerticalOffset(0);
                            int firstDisplayedRow = GetNextVisibleSlot(-1);
                            UpdateDisplayedRows(firstDisplayedRow, this.CellsHeight);
                        }
                    }
                }
            }

            // Update VisibleSlotCount
            this.VisibleSlotCount += slotsExpanded;

            return heightChange;
        }

        private void UpdateTablesForRemoval(int slotDeleted, object itemDeleted)
        {
            if (this.RowGroupHeadersTable.Contains(slotDeleted))
            {
                // A RowGroupHeader was removed
                this.RowGroupHeadersTable.RemoveIndexAndValue(slotDeleted);
                _collapsedSlotsTable.RemoveIndexAndValue(slotDeleted);
                _selectedItems.DeleteSlot(slotDeleted);
            }
            else
            {
                // Update the ranges of selected rows
                if (_selectedItems.ContainsSlot(slotDeleted))
                {
                    this.SelectionHasChanged = true;
                }
                _selectedItems.Delete(slotDeleted, itemDeleted);
                this.RowGroupHeadersTable.RemoveIndex(slotDeleted);
                _collapsedSlotsTable.RemoveIndex(slotDeleted);
            }
        }

#endregion Private Methods

#if DEBUG
        internal void PrintRowGroupInfo()
        {
            Debug.WriteLine("-----------------------------------------------RowGroupHeaders");
            foreach (int slot in this.RowGroupHeadersTable.GetIndexes())
            {
                DataGridRowGroupInfo info = this.RowGroupHeadersTable.GetValueAt(slot);
                Debug.WriteLine(String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1} Slot:{2} Last:{3} Level:{4}", info.CollectionViewGroup.Name, info.Visibility.ToString(), slot, info.LastSubItemSlot, info.Level));
            }
            Debug.WriteLine("-----------------------------------------------CollapsedSlots");
            _collapsedSlotsTable.PrintIndexes();
        }
#endif
    }
}
