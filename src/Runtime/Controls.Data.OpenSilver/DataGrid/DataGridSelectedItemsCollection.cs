// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class DataGridSelectedItemsCollection : IList
    {
        #region Data

        private List<object> _oldSelectedItemsCache;
        private IndexToValueTable<bool> _oldSelectedSlotsTable;
        private List<object> _selectedItemsCache;
        private IndexToValueTable<bool> _selectedSlotsTable;

        #endregion Data

        public DataGridSelectedItemsCollection(DataGrid owningGrid)
        {
            this.OwningGrid = owningGrid;
            this._oldSelectedItemsCache = new List<object>();
            this._oldSelectedSlotsTable = new IndexToValueTable<bool>();
            this._selectedItemsCache = new List<object>();
            this._selectedSlotsTable = new IndexToValueTable<bool>();
        }

        #region IList Implementation

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= this._selectedSlotsTable.IndexCount)
                {
                    throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, this._selectedSlotsTable.IndexCount, false);
                }
                int slot = this._selectedSlotsTable.GetNthIndex(index);
                Debug.Assert(slot > -1);
                return this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot));
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Add(object dataItem)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                throw DataGridError.DataGrid.ItemIsNotContainedInTheItemsSource("dataItem");
            }
            Debug.Assert(itemIndex >= 0);

            int slot = this.OwningGrid.SlotFromRowIndex(itemIndex);
            if (this._selectedSlotsTable.RangeCount == 0)
            {
                this.OwningGrid.SelectedItem = dataItem;
            }
            else
            {
                this.OwningGrid.SetRowSelection(slot, true /*isSelected*/, false /*setAnchorSlot*/);
            }
            return _selectedSlotsTable.IndexOf(slot);
        }

        public void Clear()
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            if (this._selectedSlotsTable.RangeCount > 0)
            {
                // Clearing the selection does not reset the potential current cell.
                if (!this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                {
                    // Edited value couldn't be committed or aborted
                    return;
                }
                this.OwningGrid.ClearRowSelection(true /*resetAnchorSlot*/);
            }
        }

        public bool Contains(object dataItem)
        {
            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return false;
            }
            Debug.Assert(itemIndex >= 0);

            return ContainsSlot(this.OwningGrid.SlotFromRowIndex(itemIndex));
        }

        public int IndexOf(object dataItem)
        {
            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return -1;
            }
            Debug.Assert(itemIndex >= 0);
            int slot = this.OwningGrid.SlotFromRowIndex(itemIndex);
            return _selectedSlotsTable.IndexOf(slot);
        }

        public void Insert(int index, object dataItem)
        {
            throw new NotSupportedException();
        }

        public void Remove(object dataItem)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            int itemIndex = this.OwningGrid.DataConnection.IndexOf(dataItem);
            if (itemIndex == -1)
            {
                return;
            }
            Debug.Assert(itemIndex >= 0);

            if (itemIndex == this.OwningGrid.CurrentSlot &&
                !this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
            {
                // Edited value couldn't be committed or aborted
                return;
            }

            this.OwningGrid.SetRowSelection(itemIndex, false /*isSelected*/, false /*setAnchorSlot*/);
        }

        public void RemoveAt(int index)
        {
            if (this.OwningGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
            }

            if (index < 0 || index >= this._selectedSlotsTable.IndexCount)
            {
                throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, this._selectedSlotsTable.IndexCount, false);
            }
            int rowIndex = this._selectedSlotsTable.GetNthIndex(index);
            Debug.Assert(rowIndex > -1);

            if (rowIndex == this.OwningGrid.CurrentSlot &&
                !this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
            {
                // Edited value couldn't be committed or aborted
                return;
            }

            this.OwningGrid.SetRowSelection(rowIndex, false /*isSelected*/, false /*setAnchorSlot*/);
        }

        #endregion IList Implementation

        #region ICollection Implementation

        public int Count
        {
            get
            {
                return _selectedSlotsTable.IndexCount;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public void CopyTo(System.Array array, int index)
        {
            // 
            throw new NotImplementedException();
        }

        #endregion ICollection Implementation

        #region IEnumerable Implementation

        public IEnumerator GetEnumerator()
        {
            Debug.Assert(this.OwningGrid != null);
            Debug.Assert(this.OwningGrid.DataConnection != null);
            Debug.Assert(_selectedSlotsTable != null);

            foreach (int slot in _selectedSlotsTable.GetIndexes())
            {
                int rowIndex = this.OwningGrid.RowIndexFromSlot(slot);
                Debug.Assert(rowIndex > -1);
                yield return this.OwningGrid.DataConnection.GetDataItem(rowIndex);
            }
        }

        #endregion IEnumerable Implementation

        #region Internal Properties

        internal DataGrid OwningGrid
        {
            get;
            private set;
        }

        internal List<object> SelectedItemsCache
        {
            get
            {
                return this._selectedItemsCache;
            }
            set
            {
                this._selectedItemsCache = value;
                UpdateIndexes();
            }
        }

        #endregion

        #region Internal Methods

        internal void ClearRows()
        {
            this._selectedSlotsTable.Clear();
            this._selectedItemsCache.Clear();
        }

        internal bool ContainsSlot(int slot)
        {
            return this._selectedSlotsTable.Contains(slot);
        }

        internal bool ContainsAll(int startSlot, int endSlot)
        {
            int itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
            while (itemSlot <= endSlot)
            {
                // Skip over the RowGroupHeaderSlots
                int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endSlot : Math.Min(endSlot, nextRowGroupHeaderSlot - 1);
                if (!_selectedSlotsTable.ContainsAll(itemSlot, lastItemSlot))
                {
                    return false;
                }
                itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
            }
            return true;
        }

        // Called when an item is deleted from the ItemsSource as opposed to just being unselected
        internal void Delete(int slot, object item)
        {
            if (this._oldSelectedSlotsTable.Contains(slot))
            {
                this.OwningGrid.SelectionHasChanged = true;
            }
            DeleteSlot(slot);
            this._selectedItemsCache.Remove(item);
        }

        internal void DeleteSlot(int slot)
        {
            this._selectedSlotsTable.RemoveIndex(slot);
            this._oldSelectedSlotsTable.RemoveIndex(slot);
        }

        // Returns the inclusive index count between lowerBound and upperBound of all indexes with the given value
        internal int GetIndexCount(int lowerBound, int upperBound)
        {
            return this._selectedSlotsTable.GetIndexCount(lowerBound, upperBound, true);
        }

        internal IEnumerable<int> GetIndexes()
        {
            return this._selectedSlotsTable.GetIndexes();
        }

        internal IEnumerable<int> GetSlots(int startSlot)
        {
            return this._selectedSlotsTable.GetIndexes(startSlot);
        }

        internal SelectionChangedEventArgs GetSelectionChangedEventArgs()
        {
            List<object> addedSelectedItems = new List<object>();
            List<object> removedSelectedItems = new List<object>();

            // Compare the old selected indexes with the current selection to determine which items
            // have been added and removed since the last time this method was called
            foreach (int newSlot in this._selectedSlotsTable.GetIndexes())
            {
                object newItem = this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(newSlot));
                if (this._oldSelectedSlotsTable.Contains(newSlot))
                {
                    this._oldSelectedSlotsTable.RemoveValue(newSlot);
                    this._oldSelectedItemsCache.Remove(newItem);
                }
                else
                {
                    addedSelectedItems.Add(newItem);
                }
            }
            foreach (object oldItem in this._oldSelectedItemsCache)
            {
                removedSelectedItems.Add(oldItem);
            }

            // The current selection becomes the old selection
            this._oldSelectedSlotsTable = this._selectedSlotsTable.Copy();
            this._oldSelectedItemsCache = new List<object>(this._selectedItemsCache);

            return new SelectionChangedEventArgs(removedSelectedItems, addedSelectedItems);
        }

        internal void InsertIndex(int slot)
        {
            this._selectedSlotsTable.InsertIndex(slot);
            this._oldSelectedSlotsTable.InsertIndex(slot);

            // It's possible that we're inserting an item that was just removed.  If that's the case,
            // and the re-inserted item used to be selected, we want to update the _oldSelectedSlotsTable
            // to include the item's new index within the collection.
            int rowIndex = this.OwningGrid.RowIndexFromSlot(slot);
            if (rowIndex != -1)
            {
                object insertedItem = this.OwningGrid.DataConnection.GetDataItem(rowIndex);
                if (insertedItem != null && this._oldSelectedItemsCache.Contains(insertedItem))
                {
                    this._oldSelectedSlotsTable.AddValue(slot, true);
                }
            }
        }

        internal void SelectSlot(int slot, bool select)
        {
            if (this.OwningGrid.RowGroupHeadersTable.Contains(slot))
            {
                return;
            }
            if (select)
            {
                if (!this._selectedSlotsTable.Contains(slot))
                {
                    this._selectedItemsCache.Add(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                }
                this._selectedSlotsTable.AddValue(slot, true);
            }
            else
            {
                if (this._selectedSlotsTable.Contains(slot))
                {
                    this._selectedItemsCache.Remove(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                }
                this._selectedSlotsTable.RemoveValue(slot);
            }
        }

        internal void SelectSlots(int startSlot, int endSlot, bool select)
        {
            int itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
            int endItemSlot = this.OwningGrid.RowGroupHeadersTable.GetPreviousGap(endSlot + 1);
            if (select)
            {
                while (itemSlot <= endItemSlot)
                {
                    // Add the newly selected item slots by skipping over the RowGroupHeaderSlots
                    int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                    int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endItemSlot : Math.Min(endItemSlot, nextRowGroupHeaderSlot - 1);
                    for (int slot = itemSlot; slot <= lastItemSlot; slot++)
                    {
                        if (!this._selectedSlotsTable.Contains(slot))
                        {
                            this._selectedItemsCache.Add(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                        }
                    }
                    this._selectedSlotsTable.AddValues(itemSlot, lastItemSlot - itemSlot + 1, true);
                    itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
                }
            }
            else
            {
                while (itemSlot <= endItemSlot)
                {
                    // Remove the unselected item slots by skipping over the RowGroupHeaderSlots
                    int nextRowGroupHeaderSlot = this.OwningGrid.RowGroupHeadersTable.GetNextIndex(itemSlot);
                    int lastItemSlot = nextRowGroupHeaderSlot == -1 ? endItemSlot : Math.Min(endItemSlot, nextRowGroupHeaderSlot - 1);
                    for (int slot = itemSlot; slot <= lastItemSlot; slot++)
                    {
                        if (this._selectedSlotsTable.Contains(slot))
                        {
                            this._selectedItemsCache.Remove(this.OwningGrid.DataConnection.GetDataItem(this.OwningGrid.RowIndexFromSlot(slot)));
                        }
                    }
                    this._selectedSlotsTable.RemoveValues(itemSlot, lastItemSlot - itemSlot + 1);
                    itemSlot = this.OwningGrid.RowGroupHeadersTable.GetNextGap(lastItemSlot);
                }
            }
        }

        internal void UpdateIndexes()
        {
            this._oldSelectedSlotsTable.Clear();
            this._selectedSlotsTable.Clear();

            if (this.OwningGrid.DataConnection.DataSource == null)
            {
                if (this.SelectedItemsCache.Count > 0)
                {
                    this.OwningGrid.SelectionHasChanged = true;
                    this.SelectedItemsCache.Clear();
                }
            }
            else
            {
                List<object> tempSelectedItemsCache = new List<object>();
                foreach (object item in this._selectedItemsCache)
                {
                    int index = this.OwningGrid.DataConnection.IndexOf(item);
                    if (index != -1)
                    {
                        tempSelectedItemsCache.Add(item);
                        this._selectedSlotsTable.AddValue(this.OwningGrid.SlotFromRowIndex(index), true);
                    }
                }
                foreach (object item in this._oldSelectedItemsCache)
                {
                    int index = this.OwningGrid.DataConnection.IndexOf(item);
                    if (index == -1)
                    {
                        this.OwningGrid.SelectionHasChanged = true;
                    }
                    else
                    {
                        this._oldSelectedSlotsTable.AddValue(this.OwningGrid.SlotFromRowIndex(index), true);
                    }
                }
                this._selectedItemsCache = tempSelectedItemsCache;
            }

        }

        #endregion Internal Methods
    }
}
