
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

#if MIGRATION
using System.Windows.Common;
using System.Windows.Data;
#else
using System;
using Windows.UI.Xaml.Common;
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
        //private DataGridSelectionAction _selectionActionForCurrentChanged;

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
                if (_dataType == null && _owner.Items != null)
                {
                    _dataType = _owner.Items[0].GetType();
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
            return
                type.IsEnum
                || type == typeof(String)
                || type == typeof(Char)
                || type == typeof(DateTime)
                || type == typeof(Boolean)
                || type == typeof(Byte)
                || type == typeof(SByte)
                || type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64);
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
                        if (propertyInfo == null)
                        {
                            // Either the data type is read-only, the property doesn't exist, or it does exist but is read-only
                            return true;
                        }

                        // Check if EditableAttribute is defined on the property and if it indicates uneditable
                        object[] attributes = propertyInfo.GetCustomAttributes(typeof(EditableAttribute), true);
                        if (attributes != null && attributes.Length > 0)
                        {
                            EditableAttribute editableAttribute = attributes[0] as EditableAttribute;
                            if (!editableAttribute.AllowEdit)
                            {
                                return true;
                            }
                        }
                        propertyType = propertyInfo.PropertyType.GetNonNullableType();
                    }
                    return propertyInfo == null || !propertyInfo.CanWrite || !this.AllowEdit || !CanEdit(propertyType);
                }
                else
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

        internal void MoveCurrentTo(object item, int backupSlot, int columnIndex, bool scrollIntoView)
        {
            if (this.CollectionView != null)
            {
                this._expectingCurrentChanged = true;
                this._columnForCurrentChanged = columnIndex;
                this._itemToSelectOnCurrentChanged = item;
                this._scrollForCurrentChanged = scrollIntoView;
                this._backupSlotForCurrentChanged = backupSlot;

                this.CollectionView.MoveCurrentTo(item is CollectionViewGroup ? null : item);

                this._expectingCurrentChanged = false;
            }
        }

        #endregion Internal Methods

        #region Private methods

        private void UpdateDataProperties()
        {
            Type dataType = this.DataType;

            if (this.DataSource != null && dataType != null && !DataTypeIsPrimitive(dataType))
            {
                _dataProperties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            else
            {
                _dataProperties = null;
            }
        }

        #endregion Private Methods
    }

    internal class DataGridRowGroupInfo
    {
        public DataGridRowGroupInfo(
            CollectionViewGroup collectionViewGroup,
            Visibility visibility,
            int level,
            int slot,
            int lastSubItemSlot)
        {
            this.CollectionViewGroup = collectionViewGroup;
            this.Visibility = visibility;
            this.Level = level;
            this.Slot = slot;
            this.LastSubItemSlot = lastSubItemSlot;
        }

        public CollectionViewGroup CollectionViewGroup
        {
            get;
            private set;
        }

        public int LastSubItemSlot
        {
            get;
            set;
        }

        public int Level
        {
            get;
            private set;
        }

        public int Slot
        {
            get;
            set;
        }

        public Visibility Visibility
        {
            get;
            set;
        }
    }
}