// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class DataGridDataConnection
    {
        #region Data

        private int _backupSlotForCurrentChanged;
        private int _columnForCurrentChanged;
        private PropertyInfo[] _dataProperties;
        private IEnumerable _dataSource;
        private Type _dataType;
        private bool _expectingCurrentChanged;
        private object _itemToSelectOnCurrentChanged;
        private DataGrid _owner;
        private bool _scrollForCurrentChanged;
        private DataGridSelectionAction _selectionActionForCurrentChanged;

        #endregion Data

        public DataGridDataConnection(DataGrid owner)
        {
            this._owner = owner;
        }

        #region Properties

        public bool AllowEdit
        {
            get
            {
                if (this.List == null)
                {
                    return true;
                }
                else
                {
                    return !this.List.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// True if the collection view says it can sort.
        /// </summary>
        public bool AllowSort
        {
            get
            {
                if (this.CollectionView == null ||
                    (this.EditableCollectionView != null && (this.EditableCollectionView.IsAddingNew || this.EditableCollectionView.IsEditingItem)))
                {
                    return false;
                }
                else
                {
                    return this.CollectionView.CanSort;
                }
            }
        }

        public ICollectionView CollectionView
        {
            get
            {
                return this.DataSource as ICollectionView;
            }
        }

        public bool CommittingEdit
        {
            get;
            private set;
        }

        public int Count
        {
            get
            {
                IList list = this.List;
                if (list != null)
                {
                    return list.Count;
                }

                PagedCollectionView collectionView = this.DataSource as PagedCollectionView;
                if (collectionView != null)
                {
                    return collectionView.Count;
                }

                int count = 0;
                IEnumerable enumerable = this.DataSource;
                if (enumerable != null)
                {
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    if (enumerator != null)
                    {
                        while (enumerator.MoveNext())
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
        }

        public bool DataIsPrimitive
        {
            get
            {
                return DataTypeIsPrimitive(this.DataType);
            }
        }

        public PropertyInfo[] DataProperties
        {
            get
            {
                if (_dataProperties == null)
                {
                    UpdateDataProperties();
                }
                return _dataProperties;
            }
        }

        public IEnumerable DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
                // Because the DataSource is changing, we need to reset our cached values for DataType and DataProperties,
                // which are dependent on the current DataSource
                _dataType = null;
                UpdateDataProperties();
            }
        }

        public Type DataType
        {
            get
            {
                // We need to use the raw ItemsSource as opposed to DataSource because DataSource
                // may be the ItemsSource wrapped in a collection view, in which case we wouldn't
                // be able to take T to be the type if we're given IEnumerable<T>
                if (_dataType == null && _owner.ItemsSource != null)
                {
                    _dataType = _owner.ItemsSource.GetItemType();
                }
                return _dataType;
            }
        }

        public IEditableCollectionView EditableCollectionView
        {
            get
            {
                return this.DataSource as IEditableCollectionView;
            }
        }

        public bool EventsWired
        {
            get;
            private set;
        }

        private bool IsGrouping
        {
            get
            {
                return (this.CollectionView != null)
                    && (this.CollectionView.CanGroup)
                    && (this.CollectionView.GroupDescriptions != null)
                    && (this.CollectionView.GroupDescriptions.Count > 0);
            }
        }

        public IList List
        {
            get
            {
                return this.DataSource as IList;
            }
        }

        public bool ShouldAutoGenerateColumns
        {
            get
            {
                return this._owner.AutoGenerateColumns
                    && (this._owner.ColumnsInternal.AutogeneratedColumnCount == 0)
                    && ((this.DataProperties != null && this.DataProperties.Length > 0) || this.DataIsPrimitive);
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this.CollectionView != null && this.CollectionView.CanSort)
                {
                    return this.CollectionView.SortDescriptions;
                }
                else
                {
                    return (SortDescriptionCollection)null;
                }
            }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Puts the entity into editing mode if possible
        /// </summary>
        /// <param name="dataItem">The entity to edit</param>
        /// <returns>True if editing was started</returns>
        public bool BeginEdit(object dataItem)
        {
            if (dataItem == null)
            {
                return false;
            }

            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                if (editableCollectionView.IsEditingItem && (dataItem == editableCollectionView.CurrentEditItem))
                {
                    return true;
                }
                else
                {
                    editableCollectionView.EditItem(dataItem);
                    return editableCollectionView.IsEditingItem;
                }
            }

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.BeginEdit();
                return true;
            }

            return true;
        }

        /// <summary>
        /// Cancels the current entity editing and exits the editing mode.
        /// </summary>
        /// <param name="dataItem">The entity being edited</param>
        /// <returns>True if a cancellation operation was invoked.</returns>
        public bool CancelEdit(object dataItem)
        {
            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                if (editableCollectionView.CanCancelEdit)
                {
                    editableCollectionView.CancelEdit();
                    return true;
                }
                return false;
            }

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.CancelEdit();
                return true;
            }

            return true;
        }

        public static bool CanEdit(Type type)
        {
            Debug.Assert(type != null);

            type = type.GetNonNullableType();

            return
                type.IsEnum
                || type == typeof(System.String)
                || type == typeof(System.Char)
                || type == typeof(System.DateTime)
                || type == typeof(System.Boolean)
                || type == typeof(System.Byte)
                || type == typeof(System.SByte)
                || type == typeof(System.Single)
                || type == typeof(System.Double)
                || type == typeof(System.Decimal)
                || type == typeof(System.Int16)
                || type == typeof(System.Int32)
                || type == typeof(System.Int64)
                || type == typeof(System.UInt16)
                || type == typeof(System.UInt32)
                || type == typeof(System.UInt64);
        }

        /// <summary>
        /// Commits the current entity editing and exits the editing mode.
        /// </summary>
        /// <param name="dataItem">The entity being edited</param>
        /// <returns>True if a commit operation was invoked.</returns>
        public bool EndEdit(object dataItem)
        {
            IEditableCollectionView editableCollectionView = this.EditableCollectionView;
            if (editableCollectionView != null)
            {
                // IEditableCollectionView.CommitEdit can potentially change currency. If it does,
                // we don't want to attempt a second commit inside our CurrentChanging event handler.
                this._owner.NoCurrentCellChangeCount++;
                this.CommittingEdit = true;
                try
                {
                    editableCollectionView.CommitEdit();
                }
                finally
                {
                    this._owner.NoCurrentCellChangeCount--;
                    this.CommittingEdit = false;
                }
                return true;
            }

            IEditableObject editableDataItem = dataItem as IEditableObject;
            if (editableDataItem != null)
            {
                editableDataItem.EndEdit();
            }

            return true;
        }

        // Assumes index >= 0, returns null if index >= Count
        public object GetDataItem(int index)
        {
            Debug.Assert(index >= 0);
            
            IList list = this.List;
            if (list != null)
            {
                return (index < list.Count) ? list[index] : null;
            }

            PagedCollectionView collectionView = this.DataSource as PagedCollectionView;
            if (collectionView != null)
            {
                return (index < collectionView.Count) ? collectionView.GetItemAt(index) : null;
            }

            IEnumerable enumerable = this.DataSource;
            if (enumerable != null)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                int i = -1;
                while (enumerator.MoveNext() && i < index)
                {
                    i++;
                    if (i == index)
                    {
                        return enumerator.Current;
                    }
                }
            }
            return null;
        }

        public bool GetPropertyIsReadOnly(string propertyName)
        {
            if (this.DataType != null)
            {
                if (!String.IsNullOrEmpty(propertyName))
                {
                    Type propertyType = this.DataType;
                    PropertyInfo propertyInfo = null;
                    List<string> propertyNames = TypeHelper.SplitPropertyPath(propertyName);
                    for (int i = 0; i < propertyNames.Count; i++)
                    {
                        object[] index = null;
                        propertyInfo = propertyType.GetPropertyOrIndexer(propertyNames[i], out index);
                        if (propertyInfo == null || propertyType.GetIsReadOnly() || propertyInfo.GetIsReadOnly())
                        {
                            // Either the data type is read-only, the property doesn't exist, or it does exist but is read-only
                            return true;
                        }

                        // Check if EditableAttribute is defined on the property and if it indicates uneditable
                        object[] attributes = propertyInfo.GetCustomAttributes(typeof(EditableAttribute), true);
                        if (attributes != null && attributes.Length > 0)
                        {
                            EditableAttribute editableAttribute = attributes[0] as EditableAttribute;
                            Debug.Assert(editableAttribute != null);
                            if (!editableAttribute.AllowEdit)
                            {
                                return true;
                            }
                        }
                        propertyType = propertyInfo.PropertyType.GetNonNullableType();
                    }
                    return propertyInfo == null || !propertyInfo.CanWrite || !this.AllowEdit || !CanEdit(propertyType);
                }
                else if (this.DataType.GetIsReadOnly())
                {
                    return true;
                }
            }
            return !this.AllowEdit;
        }

        public int IndexOf(object dataItem)
        {
            IList list = this.List;
            if (list != null)
            {
                return list.IndexOf(dataItem);
            }

            PagedCollectionView cv = this.DataSource as PagedCollectionView;
            if (cv != null)
            {
                return cv.IndexOf(dataItem);
            }

            IEnumerable enumerable = this.DataSource;
            if (enumerable != null && dataItem != null)
            {
                int index = 0;
                foreach (object dataItemTmp in enumerable)
                {
                    if ((dataItem == null && dataItemTmp == null) ||
                        dataItem.Equals(dataItemTmp))
                    {
                        return index;
                    }
                    index++;
                }
            }
            return -1;
        }

        #endregion Public methods

        #region Internal Methods

        internal void ClearDataProperties()
        {
            this._dataProperties = null;
        }

        /// <summary>
        /// Creates a collection view around the DataGrid's source. ICollectionViewFactory is
        /// used if the source implements it. Otherwise a PagedCollectionView is returned.
        /// </summary>
        /// <param name="source">Enumerable source for which to create a view</param>
        /// <returns>ICollectionView view over the provided source</returns>
        internal static ICollectionView CreateView(IEnumerable source)
        {
            Debug.Assert(source != null, "source unexpectedly null");
            Debug.Assert(!(source is ICollectionView), "source is an ICollectionView");

            ICollectionView collectionView = null;

            ICollectionViewFactory collectionViewFactory = source as ICollectionViewFactory;
            if (collectionViewFactory != null)
            {
                // If the source is a collection view factory, give it a chance to produce a custom collection view.
                collectionView = collectionViewFactory.CreateView();
                // Intentionally not catching potential exception thrown by ICollectionViewFactory.CreateView().
            }
            if (collectionView == null)
            {
                // If we still do not have a collection view, default to a PagedCollectionView.
                collectionView = new PagedCollectionView(source);
            }
            return collectionView;
        }

        internal static bool DataTypeIsPrimitive(Type dataType)
        {
            if (dataType != null)
            {
                Type type = TypeHelper.GetNonNullableType(dataType);  // no-opt if dataType isn't nullable
                return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(Decimal);
            }
            else
            {
                return false;
            }
        }

        internal void MoveCurrentTo(object item, int backupSlot, int columnIndex, DataGridSelectionAction action, bool scrollIntoView)
        {
            if (this.CollectionView != null)
            {
                this._expectingCurrentChanged = true;
                this._columnForCurrentChanged = columnIndex;
                this._itemToSelectOnCurrentChanged = item;
                this._selectionActionForCurrentChanged = action;
                this._scrollForCurrentChanged = scrollIntoView;
                this._backupSlotForCurrentChanged = backupSlot;

                this.CollectionView.MoveCurrentTo(item is CollectionViewGroup ? null : item);

                this._expectingCurrentChanged = false;
            }
        }

        internal void UnWireEvents(IEnumerable value)
        {
            INotifyCollectionChanged notifyingDataSource = value as INotifyCollectionChanged;
            if (notifyingDataSource != null)
            {
                notifyingDataSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(NotifyingDataSource_CollectionChanged);
            }

            if (this.SortDescriptions != null)
            {
                ((INotifyCollectionChanged)this.SortDescriptions).CollectionChanged -= new NotifyCollectionChangedEventHandler(CollectionView_SortDescriptions_CollectionChanged);
            }

            if (this.CollectionView != null)
            {
                this.CollectionView.CurrentChanged -= new EventHandler(CollectionView_CurrentChanged);
                this.CollectionView.CurrentChanging -= new CurrentChangingEventHandler(CollectionView_CurrentChanging);
            }

            this.EventsWired = false;
        }

        internal void WireEvents(IEnumerable value)
        {
            INotifyCollectionChanged notifyingDataSource = value as INotifyCollectionChanged;
            if (notifyingDataSource != null)
            {
                notifyingDataSource.CollectionChanged += new NotifyCollectionChangedEventHandler(NotifyingDataSource_CollectionChanged);
            }

            if (this.SortDescriptions != null)
            {
                ((INotifyCollectionChanged)this.SortDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionView_SortDescriptions_CollectionChanged);
            }

            if (this.CollectionView != null)
            {
                this.CollectionView.CurrentChanged += new EventHandler(CollectionView_CurrentChanged);
                this.CollectionView.CurrentChanging += new CurrentChangingEventHandler(CollectionView_CurrentChanging);
            }

            this.EventsWired = true;
        }

        #endregion Internal Methods

        #region Private methods

        private void CollectionView_CurrentChanged(object sender, EventArgs e)
        {
            if (this._expectingCurrentChanged)
            {
                // Committing Edit could cause our item to move to a group that no longer exists.  In
                // this case, we need to update the item.
                CollectionViewGroup collectionViewGroup = _itemToSelectOnCurrentChanged as CollectionViewGroup;
                if (collectionViewGroup != null)
                {
                    DataGridRowGroupInfo groupInfo = this._owner.RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                    if (groupInfo == null)
                    {
                        // Move to the next slot if the target slot isn't visible                        
                        if (!this._owner.IsSlotVisible(_backupSlotForCurrentChanged))
                        {
                            _backupSlotForCurrentChanged = this._owner.GetNextVisibleSlot(_backupSlotForCurrentChanged);
                        }
                        // Move to the next best slot if we've moved past all the slots.  This could happen if multiple
                        // groups were removed.
                        if (_backupSlotForCurrentChanged >= this._owner.SlotCount)
                        {
                            _backupSlotForCurrentChanged = this._owner.GetPreviousVisibleSlot(this._owner.SlotCount);
                        }
                        // Update the itemToSelect
                        int newCurrentPosition = -1;
                        _itemToSelectOnCurrentChanged = this._owner.ItemFromSlot(_backupSlotForCurrentChanged, ref newCurrentPosition);
                    }
                }

                this._owner.ProcessSelectionAndCurrency(
                    this._columnForCurrentChanged,
                    this._itemToSelectOnCurrentChanged,
                    this._backupSlotForCurrentChanged,
                    this._selectionActionForCurrentChanged,
                    this._scrollForCurrentChanged);
            }
            else if (this.CollectionView != null)
            {
                this._owner.UpdateStateOnCurrentChanged(this.CollectionView.CurrentItem, this.CollectionView.CurrentPosition);
            }
        }

        private void CollectionView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (this._owner.NoCurrentCellChangeCount == 0 &&
                !this._expectingCurrentChanged &&
                !this.CommittingEdit &&
                !this._owner.CommitEdit())
            {
                // If CommitEdit failed, then the user has most likely input invalid data.
                // We should cancel the current change if we can, otherwise we have to abort the edit.
                if (e.IsCancelable)
                {
                    e.Cancel = true;
                }
                else
                {
                    this._owner.CancelEdit(DataGridEditingUnit.Row, false);
                }
            }
        }

        private void CollectionView_SortDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._owner.ColumnsItemsInternal.Count == 0)
            {
                return;
            }

            // refresh sort description
            foreach (DataGridColumn column in this._owner.ColumnsItemsInternal)
            {
                column.HeaderCell.ApplyState(true);
            }
        }

        private void NotifyingDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._owner.LoadingOrUnloadingRow)
            {
                throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(e.NewItems != null, "Unexpected NotifyCollectionChangedAction.Add notification");
                    if (ShouldAutoGenerateColumns)
                    {
                        // The columns are also affected (not just rows) in this case so we need to reset everything
                        this._owner.InitializeElements(false /*recycleRows*/);
                    }
                    else if (!this.IsGrouping)
                    {
                        // If we're grouping then we handle this through the CollectionViewGroup notifications
                        // According to WPF, Add is a single item operation
                        Debug.Assert(e.NewItems.Count == 1);
                        this._owner.InsertRowAt(e.NewStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    IList removedItems = e.OldItems;
                    if (removedItems == null || e.OldStartingIndex < 0)
                    {
                        Debug.Assert(false, "Unexpected NotifyCollectionChangedAction.Remove notification");
                        return;
                    }
                    if (!this.IsGrouping)
                    {
                        // If we're grouping then we handle this through the CollectionViewGroup notifications
                        // According to WPF, Remove is a single item operation
                        foreach (object item in e.OldItems)
                        {
                            Debug.Assert(item != null);
                            this._owner.RemoveRowAt(e.OldStartingIndex, item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotSupportedException(); // 

                case NotifyCollectionChangedAction.Reset:
                    // Did the data type change during the reset?  If not, we can recycle
                    // the existing rows instead of having to clear them all.  We still need to clear our cached
                    // values for DataType and DataProperties, though, because the collection has been reset.
                    Type previousDataType = this._dataType;
                    this._dataType = null;
                    if (previousDataType != this.DataType)
                    {
                        ClearDataProperties();
                        this._owner.InitializeElements(false /*recycleRows*/);
                    }
                    else
                    {
                        this._owner.InitializeElements(!ShouldAutoGenerateColumns /*recycleRows*/);
                    }
                    break;
            }
        }


        private void UpdateDataProperties()
        {
            Type dataType = this.DataType;

            if (this.DataSource != null && dataType != null && !DataTypeIsPrimitive(dataType))
            {
                _dataProperties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Debug.Assert(_dataProperties != null);
            }
            else
            {
                _dataProperties = null;
            }
        }

        #endregion Private Methods
    }
}
