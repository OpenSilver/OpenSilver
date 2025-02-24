// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data
{
    /// <summary>
    /// Represents the <see cref="CollectionView"/> for collections that implement <see cref="IBindingList"/>, such as Microsoft 
    /// ActiveX Data Objects (ADO) data views.
    /// </summary>
    public sealed class BindingListCollectionView : CollectionView, IComparer, IEditableCollectionView, IItemProperties
    {
        /// <summary>
        /// Initializes an instance of <see cref="BindingListCollectionView"/> over the given list.
        /// </summary>
        /// <param name="list">
        /// The underlying <see cref="IBindingList"/>.
        /// </param>
        public BindingListCollectionView(IBindingList list)
            : base(list)
        {
            InternalList = list;
            _blv = list as IBindingListView;
#if WPF
            _isDataView = SystemDataHelper.IsDataView(list);
#else
            _isDataView = false;
#endif

            SubscribeToChanges();

            _group = new CollectionViewGroupRoot(this);
            _group.GroupDescriptionChanged += new EventHandler(OnGroupDescriptionChanged);
            ((INotifyCollectionChanged)_group).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupChanged);
            ((INotifyCollectionChanged)_group.GroupDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupByChanged);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified item in the underlying collection belongs to the view.
        /// </summary>
        /// <param name="item">
        /// The item to check.
        /// </param>
        /// <returns>
        /// true if the specified item belongs to the view or if there is not filter set on the collection view; otherwise, false.
        /// </returns>
        public override bool PassesFilter(object item)
        {
            if (IsCustomFilterSet)
                return Contains(item);  // need to ask inner list, not cheap but only way to determine
            else
                return true;    // every item is contained
        }

        /// <summary>
        /// Returns a value that indicates whether a given item belongs to the collection view.
        /// </summary>
        /// <param name="item">
        /// The object to check.
        /// </param>
        /// <returns>
        /// true if the item belongs to the collection view; otherwise, false.
        /// </returns>
        public override bool Contains(object item)
        {
            VerifyRefreshNotDeferred();

            return (item == NewItemPlaceholder) ? (NewItemPlaceholderPosition != NewItemPlaceholderPosition.None)
                                                : CollectionProxy.Contains(item);
        }

        /// <summary>
        /// Sets the item at the specified index to be the <see cref="CollectionView.CurrentItem"/> in the view.
        /// </summary>
        /// <param name="position">
        /// The index to set the <see cref="CollectionView.CurrentItem"/> to.
        /// </param>
        /// <returns>
        /// true if the resulting <see cref="CollectionView.CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The index is out of range.
        /// </exception>
        public override bool MoveCurrentToPosition(int position)
        {
            VerifyRefreshNotDeferred();

            if (position < -1 || position > InternalCount)
                throw new ArgumentOutOfRangeException(nameof(position));

            _MoveTo(position);
            return IsCurrentInView;
        }

        /// <summary> Return -, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively)
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="o2">second object</param>
        /// <remarks>
        /// Compares items by their resp. index in the IList.
        /// </remarks>
        int IComparer.Compare(object o1, object o2)
        {
            int i1 = InternalIndexOf(o1);
            int i2 = InternalIndexOf(o2);
            return (i1 - i2);
        }

        /// <summary>
        /// Returns the index at which the given item belongs in the collection view.
        /// </summary>
        /// <param name="item">
        /// The object to look for in the collection.
        /// </param>
        /// <returns>
        /// The index of the item in the collection, or -1 if the item does not exist in the collection view.
        /// </returns>
        public override int IndexOf(object item)
        {
            VerifyRefreshNotDeferred();

            return InternalIndexOf(item);
        }

        /// <summary>
        /// Retrieves the item at the specified position in the view.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which the item is located.
        /// </param>
        /// <returns>
        /// The item at the specified position in the view.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If index is out of range.
        /// </exception>
        public override object GetItemAt(int index)
        {
            VerifyRefreshNotDeferred();

            return InternalItemAt(index);
        }

        /// <inheritdoc />
        protected override IEnumerator GetEnumerator()
        {
            VerifyRefreshNotDeferred();

            return InternalGetEnumerator();
        }

        /// <summary>
        /// Detaches the underlying collection from this collection view to enable the collection view to be garbage collected.
        /// </summary>
        public override void DetachFromSourceCollection()
        {
            if (InternalList != null && InternalList.SupportsChangeNotification)
            {
                InternalList.ListChanged -= new ListChangedEventHandler(OnListChanged);
            }

            InternalList = null;

            base.DetachFromSourceCollection();
        }

        /// <summary>
        /// Gets a collection of <see cref="SortDescription"/> objects that describes how the items in the collection 
        /// are sorted in the view.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="SortDescription"/> objects that describe how the items in the collection are 
        /// sorted in the view.
        /// </returns>
        public override SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (InternalList.SupportsSorting)
                {
                    if (_sort == null)
                    {
                        bool allowAdvancedSorting = _blv != null && _blv.SupportsAdvancedSorting;
                        _sort = new BindingListSortDescriptionCollection(allowAdvancedSorting);
                        ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
                    }
                    return _sort;
                }
                else
                    return SortDescriptionCollection.Empty;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection supports sorting.
        /// </summary>
        /// <returns>
        /// For a default instance of <see cref="BindingListCollectionView"/> this property always returns true.
        /// </returns>
        public override bool CanSort
        {
            get
            {
                return InternalList.SupportsSorting;
            }
        }

        private IComparer ActiveComparer
        {
            get { return _comparer; }
            set
            {
                _comparer = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports callback-based filtering.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public override bool CanFilter
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a custom filter.
        /// </summary>
        /// <returns>
        /// A string that specifies how the items are filtered.
        /// </returns>
        public string CustomFilter
        {
            get { return _customFilter; }
            set
            {
                if (!CanCustomFilter)
                    throw new NotSupportedException(Strings.BindingListCannotCustomFilter);
                if (IsAddingNew || IsEditingItem)
                    throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(CustomFilter)));
                if (AllowsCrossThreadChanges)
                    VerifyAccess();

                _customFilter = value;

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports custom filtering.
        /// </summary>
        /// <returns>
        /// true if the view supports custom filtering; otherwise, false.
        /// </returns>
        public bool CanCustomFilter
        {
            get
            {
                return ((_blv != null) && _blv.SupportsFiltering);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports grouping.
        /// </summary>
        /// <returns>
        /// For a default instance of <see cref="BindingListCollectionView"/> this property always returns true.
        /// </returns>
        public override bool CanGroup
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a collection of <see cref="GroupDescription"/> objects that describe how the items in the collection 
        /// are grouped in the view.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="GroupDescription"/> objects that describe how the items in the collection are 
        /// grouped in the view.
        /// </returns>
        public override ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return _group.GroupDescriptions; }
        }

        /// <summary>
        /// Gets the top-level groups.
        /// </summary>
        /// <returns>
        /// A read-only collection of the top-level groups, or null if there are no groups.
        /// </returns>
        public override ReadOnlyObservableCollection<object> Groups
        {
            get { return (_isGrouping) ? _group.Items : null; }
        }

        /// <summary>
        /// Gets or sets a delegate to select the <see cref="GroupDescription"/> as a function of the parent group and its level.
        /// </summary>
        /// <returns>
        /// A method that provides the logic for the selection of the <see cref="GroupDescription"/> as a function of the parent 
        /// group and its level. The default is null.
        /// </returns>
        [DefaultValue(null)]
        public GroupDescriptionSelectorCallback GroupBySelector
        {
            get { return _group.GroupBySelector; }
            set
            {
                if (!CanGroup)
                    throw new NotSupportedException();
                if (IsAddingNew || IsEditingItem)
                    throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(GroupBySelector)));

                _group.GroupBySelector = value;

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets the estimated number of records in the collection.
        /// </summary>
        /// <returns>
        /// One of the following:
        /// 
        /// Value – Meaning
        /// -1 – Could not determine the count of the collection. This might be returned by a "virtualizing" view, where 
        /// the view deliberately does not account for all items in the underlying collection because the view is attempting 
        /// to increase efficiency and minimize dependence on always having the entire collection available.
        /// 
        /// any other integer – The count of the collection.
        /// </returns>
        public override int Count
        {
            get
            {
                VerifyRefreshNotDeferred();

                return InternalCount;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the resulting (filtered) view is empty.
        /// </summary>
        /// <returns>
        /// true if the resulting view is empty; otherwise, false.
        /// </returns>
        public override bool IsEmpty
        {
            get
            {
                return (NewItemPlaceholderPosition == NewItemPlaceholderPosition.None &&
                            CollectionProxy.Count == 0);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the list of items (after applying the sort and filters, if any) 
        /// is already in the correct order for grouping.
        /// </summary>
        /// <returns>
        /// true if the list of items is already in the correct order for grouping; otherwise, false.
        /// </returns>
        public bool IsDataInGroupOrder
        {
            get { return _group.IsDataInGroupOrder; }
            set { _group.IsDataInGroupOrder = value; }
        }

        /// <summary>
        /// Gets or sets the position of the new item placeholder in the <see cref="BindingListCollectionView"/>.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the position of the new item placeholder in the <see cref="BindingListCollectionView"/>.
        /// </returns>
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get { return _newItemPlaceholderPosition; }
            set
            {
                VerifyRefreshNotDeferred();

                if (value != _newItemPlaceholderPosition && IsAddingNew)
                    throw new InvalidOperationException(
                        string.Format(Strings.MemberNotAllowedDuringTransaction, nameof(NewItemPlaceholderPosition), nameof(AddNew)));

                if (value != _newItemPlaceholderPosition && _isRemoving)
                {
                    DeferAction(() => { NewItemPlaceholderPosition = value; });
                    return;
                }

                NotifyCollectionChangedEventArgs args = null;
                int oldIndex = -1, newIndex = -1;

                // we're adding, removing, or moving the placeholder.
                // Determine the appropriate events.
                switch (value)
                {
                    case NewItemPlaceholderPosition.None:
                        switch (_newItemPlaceholderPosition)
                        {
                            case NewItemPlaceholderPosition.None:
                                break;
                            case NewItemPlaceholderPosition.AtBeginning:
                                oldIndex = 0;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                NewItemPlaceholder,
                                                oldIndex);
                                break;
                            case NewItemPlaceholderPosition.AtEnd:
                                oldIndex = InternalCount - 1;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                NewItemPlaceholder,
                                                oldIndex);
                                break;
                        }
                        break;

                    case NewItemPlaceholderPosition.AtBeginning:
                        switch (_newItemPlaceholderPosition)
                        {
                            case NewItemPlaceholderPosition.None:
                                newIndex = 0;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                NewItemPlaceholder,
                                                newIndex);
                                break;
                            case NewItemPlaceholderPosition.AtBeginning:
                                break;
                            case NewItemPlaceholderPosition.AtEnd:
                                oldIndex = InternalCount - 1;
                                newIndex = 0;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Move,
                                                NewItemPlaceholder,
                                                newIndex,
                                                oldIndex);
                                break;
                        }
                        break;

                    case NewItemPlaceholderPosition.AtEnd:
                        switch (_newItemPlaceholderPosition)
                        {
                            case NewItemPlaceholderPosition.None:
                                newIndex = InternalCount;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                NewItemPlaceholder,
                                                newIndex);
                                break;
                            case NewItemPlaceholderPosition.AtBeginning:
                                oldIndex = 0;
                                newIndex = InternalCount - 1;
                                args = new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Move,
                                                NewItemPlaceholder,
                                                newIndex,
                                                oldIndex);
                                break;
                            case NewItemPlaceholderPosition.AtEnd:
                                break;
                        }
                        break;
                }

                // now make the change and raise the events
                if (args != null)
                {
                    _newItemPlaceholderPosition = value;

                    if (!_isGrouping)
                    {
                        base.OnCollectionChanged(null, args);
                    }
                    else
                    {
                        if (oldIndex >= 0)
                        {
                            int index = (oldIndex == 0) ? 0 : _group.Items.Count - 1;
                            _group.RemoveSpecialItem(index, NewItemPlaceholder, false /*loading*/);
                        }
                        if (newIndex >= 0)
                        {
                            int index = (newIndex == 0) ? 0 : _group.Items.Count;
                            _group.InsertSpecialItem(index, NewItemPlaceholder, false /*loading*/);
                        }
                    }

                    OnPropertyChanged(nameof(NewItemPlaceholderPosition));
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a new item can be added to the collection.
        /// </summary>
        /// <returns>
        /// true if a new item can be added to the collection; otherwise, false.
        /// </returns>
        public bool CanAddNew
        {
            get { return !IsEditingItem && InternalList.AllowNew; }
        }

        /// <summary>
        /// Starts an add transaction and returns the pending new item.
        /// </summary>
        /// <returns>
        /// The pending new item.
        /// </returns>
        public object AddNew()
        {
            VerifyRefreshNotDeferred();

            if (IsEditingItem)
            {
                CommitEdit();   // implicitly close a previous EditItem
            }

            CommitNew();        // implicitly close a previous AddNew

            if (!CanAddNew)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(AddNew)));

            object newItem = null;
            BindingOperations.AccessCollection(InternalList,
                () =>
                {
                    ProcessPendingChanges();

                    _newItemIndex = -2; // this is a signal that the next ItemAdded event comes from AddNew
                    newItem = InternalList.AddNew();
                },
                true);

            Debug.Assert(_newItemIndex != -2 && newItem == _newItem, "AddNew did not raise expected events");

            MoveCurrentTo(newItem);

            ISupportInitialize isi = newItem as ISupportInitialize;
            isi?.BeginInit();

            // DataView.AddNew calls BeginEdit on the new item, but other implementations
            // of IBL don't.  Make up for them.
            if (!IsDataView)
            {
                IEditableObject ieo = newItem as IEditableObject;
                ieo?.BeginEdit();
            }

            return newItem;
        }

        // Calling IBL.AddNew() will raise an ItemAdded event.  We handle this specially
        // to adjust the position of the new item in the view (it should be adjacent
        // to the placeholder), and cache the new item for use by the other APIs
        // related to AddNew.  This method is called from ProcessCollectionChanged.
        // The index gives the adjusted position of the newItem in the view;  this
        // differs from its position in the source collection by 1 if we've added
        // a placeholder at the beginning.
        void BeginAddNew(object newItem, int index)
        {
            Debug.Assert(_newItemIndex == -2 && _newItem == NoNewItem, "unexpected call to BeginAddNew");

            // remember the new item and its position in the underlying list
            SetNewItem(newItem);
            _newItemIndex = index;

            // adjust the position of the new item
            // (not needed when grouping, as we'll be inserting into the group structure)
            int position = index;
            if (!_isGrouping)
            {
                switch (NewItemPlaceholderPosition)
                {
                    case NewItemPlaceholderPosition.None:
                        break;
                    case NewItemPlaceholderPosition.AtBeginning:
                        --_newItemIndex;
                        position = 1;
                        break;
                    case NewItemPlaceholderPosition.AtEnd:
                        position = InternalCount - 2;
                        break;
                }
            }

            // raise events as if the new item appeared in the adjusted position
            ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Add,
                                            newItem,
                                            position));
        }

        /// <summary>
        /// Ends the add transaction and saves the pending new item.
        /// </summary>
        public void CommitNew()
        {
            if (IsEditingItem)
                throw new InvalidOperationException(
                    string.Format(Strings.MemberNotAllowedDuringTransaction, nameof(CommitNew), nameof(EditItem)));
            VerifyRefreshNotDeferred();

            if (_newItem == NoNewItem)
                return;

            // commit the new item
            ICancelAddNew ican = InternalList as ICancelAddNew;
            IEditableObject ieo;

            BindingOperations.AccessCollection(InternalList,
                () =>
                {
                    ProcessPendingChanges();

                    if (ican != null)
                    {
                        ican.EndNew(_newItemIndex);
                    }
                    else if ((ieo = _newItem as IEditableObject) != null)
                    {
                        ieo.EndEdit();
                    }
                },
                true);

            // DataView raises events that cause us to update the view
            // correctly (including leaving AddNew mode).  BindingList<T> does not
            // raise these events.  If they haven't happened, do the work now.
            if (_newItem != NoNewItem)
            {
                int delta = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
                NotifyCollectionChangedEventArgs args = ProcessCommitNew(_newItemIndex, _newItemIndex + delta);
                if (args != null)
                {
                    base.OnCollectionChanged(InternalList, args);
                }
            }
        }

        /// <summary>
        /// Ends the add transaction and discards the pending new item.
        /// </summary>
        public void CancelNew()
        {
            if (IsEditingItem)
                throw new InvalidOperationException(
                    string.Format(Strings.MemberNotAllowedDuringTransaction, nameof(CancelNew), nameof(EditItem)));
            VerifyRefreshNotDeferred();

            if (_newItem == NoNewItem)
                return;

            // cancel the AddNew
            ICancelAddNew ican = InternalList as ICancelAddNew;
            IEditableObject ieo;

            BindingOperations.AccessCollection(InternalList,
                () =>
                {
                    ProcessPendingChanges();

                    if (ican != null)
                    {
                        ican.CancelNew(_newItemIndex);
                    }
                    else if ((ieo = _newItem as IEditableObject) != null)
                    {
                        ieo.CancelEdit();
                    }
                },
                true);

            // DataView raises events that cause us to update the view
            // correctly (including leaving AddNew mode).  BindingList<T> does not
            // raise these events.  If they haven't happened, do the work now.
            if (_newItem != NoNewItem)
            {
                Debug.Assert(true);
            }
        }

        // Common functionality used by CommitNew, CancelNew, and when the
        // new item is removed by Remove or Refresh.
        object EndAddNew(bool cancel)
        {
            object newItem = _newItem;

            SetNewItem(NoNewItem);  // leave "adding-new" mode

            IEditableObject ieo = newItem as IEditableObject;
            if (ieo != null)
            {
                if (cancel)
                {
                    ieo.CancelEdit();
                }
                else
                {
                    ieo.EndEdit();
                }
            }

            ISupportInitialize isi = newItem as ISupportInitialize;
            isi?.EndInit();

            return newItem;
        }

        NotifyCollectionChangedEventArgs ProcessCommitNew(int fromIndex, int toIndex)
        {
            if (_isGrouping)
            {
                CommitNewForGrouping();
                return null;
            }

            // CommitNew either causes the list to raise an event, or not.
            // In either case, leave AddNew mode and raise a Move event if needed.
            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                    break;
                case NewItemPlaceholderPosition.AtBeginning:
                    fromIndex = 1;
                    break;
                case NewItemPlaceholderPosition.AtEnd:
                    fromIndex = InternalCount - 2;
                    break;
            }

            object newItem = EndAddNew(false);

            NotifyCollectionChangedEventArgs result = null;
            if (fromIndex != toIndex)
            {
                result = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItem, toIndex, fromIndex);
            }

            return result;
        }

        void CommitNewForGrouping()
        {
            // for grouping we cannot pretend that the new item moves to a different position,
            // since it may actually appear in several new positions (belonging to several groups).
            // Instead, we remove the item from its temporary position, then add it to the groups
            // as if it had just been added to the underlying collection.
            int index;
            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                default:
                    index = _group.Items.Count - 1;
                    break;
                case NewItemPlaceholderPosition.AtBeginning:
                    index = 1;
                    break;
                case NewItemPlaceholderPosition.AtEnd:
                    index = _group.Items.Count - 2;
                    break;
            }

            // End the AddNew transaction
            object newItem = EndAddNew(false);

            // remove item from its temporary position
            _group.RemoveSpecialItem(index, newItem, false /*loading*/);

            // add it to the groups
            AddItemToGroups(newItem);
        }

        /// <summary>
        /// Gets a value that indicates whether an add transaction is in progress.
        /// </summary>
        /// <returns>
        /// true if an add transaction is in progress; otherwise, false.
        /// </returns>
        public bool IsAddingNew
        {
            get { return (_newItem != NoNewItem); }
        }

        /// <summary>
        /// Gets the item that is being added during the current add transaction.
        /// </summary>
        /// <returns>
        /// The item that is being added if <see cref="IsAddingNew"/> is true; otherwise, null.
        /// </returns>
        public object CurrentAddItem
        {
            get { return IsAddingNew ? _newItem : null; }
        }

        void SetNewItem(object item)
        {
            if (!ItemsControl.EqualsEx(item, _newItem))
            {
                _newItem = item;

                OnPropertyChanged(nameof(CurrentAddItem));
                OnPropertyChanged(nameof(IsAddingNew));
                OnPropertyChanged(nameof(CanRemove));
            }
        }

        /// <summary>
        /// Gets a value that indicates whether an item can be removed from the collection.
        /// </summary>
        /// <returns>
        /// true if an item can be removed from the collection; otherwise, false.
        /// </returns>
        public bool CanRemove
        {
            get { return !IsEditingItem && !IsAddingNew && InternalList.AllowRemove; }
        }

        /// <summary>
        /// Removes the item at the specified position from the collection.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0 or greater than the number of items in the collection view.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (IsEditingItem || IsAddingNew)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(RemoveAt)));
            VerifyRefreshNotDeferred();

            RemoveImpl(GetItemAt(index), index);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        public void Remove(object item)
        {
            if (IsEditingItem || IsAddingNew)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(Remove)));
            VerifyRefreshNotDeferred();

            int index = InternalIndexOf(item);
            if (index >= 0)
            {
                RemoveImpl(item, index);
            }
        }

        void RemoveImpl(object item, int index)
        {
            if (item == NewItemPlaceholder)
                throw new InvalidOperationException(Strings.RemovingPlaceholder);

            BindingOperations.AccessCollection(InternalList,
                () =>
                {
                    ProcessPendingChanges();

                    // the pending changes may have moved (or even removed) the
                    // item.   Verify the index.
                    if (index >= InternalList.Count || !ItemsControl.EqualsEx(item, GetItemAt(index)))
                    {
                        index = InternalList.IndexOf(item);
                        if (index < 0)
                            return;
                    }

                    // convert the index from "view-relative" to "list-relative"
                    if (_isGrouping)
                    {
                        index = InternalList.IndexOf(item);
                    }
                    else
                    {
                        int delta = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
                        index = index - delta;
                    }

                    // remove the item from the list
                    try
                    {
                        _isRemoving = true;
                        InternalList.RemoveAt(index);
                    }
                    finally
                    {
                        _isRemoving = false;
                        DoDeferredActions();
                    }
                },
                true);
        }

        /// <summary>
        /// Begins an edit transaction of the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to edit.
        /// </param>
        public void EditItem(object item)
        {
            VerifyRefreshNotDeferred();

            if (item == NewItemPlaceholder)
                throw new ArgumentException(Strings.CannotEditPlaceholder, nameof(item));

            if (IsAddingNew)
            {
                if (ItemsControl.EqualsEx(item, _newItem))
                    return;     // EditItem(newItem) is a no-op

                CommitNew();    // implicitly close a previous AddNew
            }

            CommitEdit();   // implicitly close a previous EditItem transaction

            SetEditItem(item);

            IEditableObject ieo = item as IEditableObject;
            ieo?.BeginEdit();
        }

        /// <summary>
        /// Ends the edit transaction and saves the pending changes.
        /// </summary>
        public void CommitEdit()
        {
            if (IsAddingNew)
                throw new InvalidOperationException(
                    string.Format(Strings.MemberNotAllowedDuringTransaction, nameof(CommitEdit), nameof(AddNew)));
            VerifyRefreshNotDeferred();

            if (_editItem == null)
                return;

            IEditableObject ieo = _editItem as IEditableObject;
            object editItem = _editItem;
            SetEditItem(null);

            if (ieo != null)
            {
                BindingOperations.AccessCollection(InternalList,
                    () =>
                    {
                        ProcessPendingChanges();
                        ieo.EndEdit();
                    },
                    true);
            }

            // editing may change the item's group names (and we can't tell whether
            // it really did).  The best we can do is remove the item and re-insert
            // it.
            if (_isGrouping)
            {
                RemoveItemFromGroups(editItem);
                AddItemToGroups(editItem);
                return;
            }
        }

        /// <summary>
        /// Ends the edit transaction and, if possible, restores the original value to the item.
        /// </summary>
        public void CancelEdit()
        {
            if (IsAddingNew)
                throw new InvalidOperationException(
                    string.Format(Strings.MemberNotAllowedDuringTransaction, nameof(CancelEdit), nameof(AddNew)));
            VerifyRefreshNotDeferred();

            if (_editItem == null)
                return;

            IEditableObject ieo = _editItem as IEditableObject;
            SetEditItem(null);

            if (ieo != null)
            {
                ieo.CancelEdit();
            }
            else
                throw new InvalidOperationException(Strings.CancelEditNotSupported);
        }

        private void ImplicitlyCancelEdit()
        {
            IEditableObject ieo = _editItem as IEditableObject;
            SetEditItem(null);

            ieo?.CancelEdit();
        }

        /// <summary>
        /// Gets a value that indicates whether the collection view can discard pending changes and restore the original 
        /// values of an edited object.
        /// </summary>
        /// <returns>
        /// true if the collection view can discard pending changes and restore the original values of an edited object;
        /// otherwise, false.
        /// </returns>
        public bool CanCancelEdit
        {
            get { return (_editItem is IEditableObject); }
        }

        /// <summary>
        /// Gets a value that indicates whether an edit transaction is in progress.
        /// </summary>
        /// <returns>
        /// true if an edit transaction is in progress; otherwise, false.
        /// </returns>
        public bool IsEditingItem
        {
            get { return (_editItem != null); }
        }

        /// <summary>
        /// Gets the item in the collection that is being edited.
        /// </summary>
        /// <returns>
        /// The item in the collection that is being edited if <see cref="IsEditingItem"/> is true; otherwise, null.
        /// </returns>
        public object CurrentEditItem
        {
            get { return _editItem; }
        }

        void SetEditItem(object item)
        {
            if (!ItemsControl.EqualsEx(item, _editItem))
            {
                _editItem = item;

                OnPropertyChanged(nameof(CurrentEditItem));
                OnPropertyChanged(nameof(IsEditingItem));
                OnPropertyChanged(nameof(CanCancelEdit));
                OnPropertyChanged(nameof(CanAddNew));
                OnPropertyChanged(nameof(CanRemove));
            }
        }

        /// <summary>
        /// Gets a collection of objects that describes the properties of the items in the collection.
        /// </summary>
        /// <returns>
        /// A collection of objects that describes the properties of the items in the collection.
        /// </returns>
        public ReadOnlyCollection<ItemPropertyInfo> ItemProperties
        {
            get { return GetItemProperties(); }
        }

        /// <inheritdoc />
        protected override void RefreshOverride()
        {
            object oldCurrentItem = CurrentItem;
            int oldCurrentPosition = IsEmpty ? 0 : CurrentPosition;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

            // force currency off the collection (gives user a chance to save dirty information)
            OnCurrentChanging();

            // changing filter and sorting will cause the inner IBindingList(View) to
            // raise refresh action; ignore those until done setting filter/sort
            _ignoreInnerRefresh = true;

            // IBindingListView can support filtering
            if (IsCustomFilterSet || _isFiltered)
            {
                BindingOperations.AccessCollection(InternalList,
                    () =>
                    {
                        if (IsCustomFilterSet)
                        {
                            _isFiltered = true;
                            _blv.Filter = _customFilter;
                        }
                        else if (_isFiltered)
                        {
                            // app has cleared filter
                            _isFiltered = false;
                            _blv.RemoveFilter();
                        }
                    },
                    true);
            }

            if ((_sort != null) && (_sort.Count > 0) && (CollectionProxy != null) && (CollectionProxy.Count > 0))
            {
                // convert Avalon SortDescription collection to .Net
                // (i.e. string property names become PropertyDescriptors)
                ListSortDescriptionCollection sorts = ConvertSortDescriptionCollection(_sort);

                if (sorts.Count > 0)
                {
                    _isSorted = true;
                    BindingOperations.AccessCollection(InternalList,
                        () =>
                        {
                            if (_blv == null)
                                InternalList.ApplySort(sorts[0].PropertyDescriptor, sorts[0].SortDirection);
                            else
                                _blv.ApplySort(sorts);
                        },
                        true);
                }
                ActiveComparer = new SortFieldComparer(_sort, Culture);
            }
            else if (_isSorted)
            {
                // undo any previous sorting
                _isSorted = false;
                BindingOperations.AccessCollection(InternalList,
                    () =>
                    {
                        InternalList.RemoveSort();
                    },
                    true);
                ActiveComparer = null;
            }

            InitializeGrouping();

            // refresh cached list with any changes
            PrepareCachedList();

            PrepareGroups();

            // reset currency
            if (oldIsCurrentBeforeFirst || IsEmpty)
            {
                SetCurrent(null, -1);
            }
            else if (oldIsCurrentAfterLast)
            {
                SetCurrent(null, InternalCount);
            }
            else
            {
                // oldCurrentItem may be null

                // if there are duplicates, use the position of the first matching item
                //ISSUE windows#868101 DataRowView.IndexOf(oldCurrentItem) returns wrong index, wrong current item gets restored
                int newPosition = InternalIndexOf(oldCurrentItem);

                if (newPosition < 0)
                {
                    // oldCurrentItem not found: move to first item
                    object newItem;
                    newPosition = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ?
                                1 : 0;
                    if (newPosition < InternalCount && (newItem = InternalItemAt(newPosition)) != NewItemPlaceholder)
                    {
                        SetCurrent(newItem, newPosition);
                    }
                    else
                    {
                        SetCurrent(null, -1);
                    }
                }
                else
                {
                    SetCurrent(oldCurrentItem, newPosition);
                }
            }

            _ignoreInnerRefresh = false;

            // tell listeners everything has changed
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnCurrentChanged();

            if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                OnPropertyChanged(IsCurrentAfterLastPropertyName);

            if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                OnPropertyChanged(IsCurrentBeforeFirstPropertyName);

            if (oldCurrentPosition != CurrentPosition)
                OnPropertyChanged(CurrentPositionPropertyName);

            if (oldCurrentItem != CurrentItem)
                OnPropertyChanged(CurrentItemPropertyName);
        }

        /// <inheritdoc />
        protected override void OnAllowsCrossThreadChangesChanged()
        {
            PrepareCachedList();
        }

        void PrepareCachedList()
        {
            if (AllowsCrossThreadChanges)
            {
                BindingOperations.AccessCollection(InternalList,
                    () =>
                    {
                        RebuildLists();
                    },
                    false);
            }
            else
            {
                RebuildListsCore();
            }
        }

        // this must be called under read-access protection to InternalList
        void RebuildLists()
        {
            lock (SyncRoot)
            {
                ClearPendingChanges();
                RebuildListsCore();
            }
        }

        void RebuildListsCore()
        {
            _cachedList = [.. InternalList];

            if (AllowsCrossThreadChanges)
            {
                _shadowList = [.. InternalList];
            }
            else
            {
                _shadowList = null;
            }
        }

        /// <inheritdoc />
        [Obsolete("Replaced by OnAllowsCrossThreadChangesChanged")]
        protected override void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args)
        {
        }


        /// <inheritdoc />
        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            bool shouldRaiseEvent = false;

            ValidateCollectionChangedEventArgs(args);

            int originalCurrentPosition = CurrentPosition;
            int oldCurrentPosition = CurrentPosition;
            object oldCurrentItem = CurrentItem;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
            bool moveCurrency = false;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (_newItemIndex == -2)
                    {
                        // The ItemAdded event came from AddNew.
                        BeginAddNew(args.NewItems[0], args.NewStartingIndex);
                        return;
                    }
                    else if (_isGrouping)
                        AddItemToGroups(args.NewItems[0]);
                    else
                    {
                        AdjustCurrencyForAdd(args.NewStartingIndex);
                        shouldRaiseEvent = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (_isGrouping)
                        RemoveItemFromGroups(args.OldItems[0]);
                    else
                    {
                        moveCurrency = AdjustCurrencyForRemove(args.OldStartingIndex);
                        shouldRaiseEvent = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (_isGrouping)
                    {
                        RemoveItemFromGroups(args.OldItems[0]);
                        AddItemToGroups(args.NewItems[0]);
                    }
                    else
                    {
                        moveCurrency = AdjustCurrencyForReplace(args.NewStartingIndex);
                        shouldRaiseEvent = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (!_isGrouping)
                    {
                        AdjustCurrencyForMove(args.OldStartingIndex, args.NewStartingIndex);
                        shouldRaiseEvent = true;
                    }
                    else
                    {
                        _group.MoveWithinSubgroups(args.OldItems[0], InternalList, args.OldStartingIndex, args.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (_isGrouping)
                        RefreshOrDefer();
                    else
                        shouldRaiseEvent = true;
                    break;

                default:
                    throw new NotSupportedException(string.Format(Strings.UnexpectedCollectionChangeAction, args.Action));
            }

            if (AllowsCrossThreadChanges)
            {
                AdjustShadowCopy(args);
            }


            // remember whether scalar properties of the view have changed.
            // They may change again during the collection change event, so we
            // need to do the test before raising that event.
            bool afterLastHasChanged = (IsCurrentAfterLast != oldIsCurrentAfterLast);
            bool beforeFirstHasChanged = (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst);
            bool currentPositionHasChanged = (CurrentPosition != oldCurrentPosition);
            bool currentItemHasChanged = (CurrentItem != oldCurrentItem);

            // take a new snapshot of the scalar properties, so that we can detect
            // changes made during the collection change event
            oldIsCurrentAfterLast = IsCurrentAfterLast;
            oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
            oldCurrentPosition = CurrentPosition;
            oldCurrentItem = CurrentItem;

            if (shouldRaiseEvent)
            {
                OnCollectionChanged(args);

                // Any scalar properties that changed don't need a further notification,
                // but do need a new snapshot
                if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                {
                    afterLastHasChanged = false;
                    oldIsCurrentAfterLast = IsCurrentAfterLast;
                }
                if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                {
                    beforeFirstHasChanged = false;
                    oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;
                }
                if (CurrentPosition != oldCurrentPosition)
                {
                    currentPositionHasChanged = false;
                    oldCurrentPosition = CurrentPosition;
                }
                if (CurrentItem != oldCurrentItem)
                {
                    currentItemHasChanged = false;
                    oldCurrentItem = CurrentItem;
                }
            }

            // currency has to change after firing the deletion event,
            // so event handlers have the right picture
            if (moveCurrency)
            {
                MoveCurrencyOffDeletedElement(originalCurrentPosition);

                // changes to the scalar properties need notification
                afterLastHasChanged = afterLastHasChanged || (IsCurrentAfterLast != oldIsCurrentAfterLast);
                beforeFirstHasChanged = beforeFirstHasChanged || (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst);
                currentPositionHasChanged = currentPositionHasChanged || (CurrentPosition != oldCurrentPosition);
                currentItemHasChanged = currentItemHasChanged || (CurrentItem != oldCurrentItem);
            }

            // notify that the properties have changed.  We may end up doing
            // double notification for properties that change during the collection
            // change event, but that's not harmful.  Detecting the double change
            // is more trouble than it's worth.
            if (afterLastHasChanged)
                OnPropertyChanged(IsCurrentAfterLastPropertyName);

            if (beforeFirstHasChanged)
                OnPropertyChanged(IsCurrentBeforeFirstPropertyName);

            if (currentPositionHasChanged)
                OnPropertyChanged(CurrentPositionPropertyName);

            if (currentItemHasChanged)
                OnPropertyChanged(CurrentItemPropertyName);
        }

        /// <summary>
        /// Protected accessor to private count.
        /// </summary>
        private int InternalCount
        {
            get
            {
                if (_isGrouping)
                    return _group.ItemCount;

                return ((NewItemPlaceholderPosition == NewItemPlaceholderPosition.None) ? 0 : 1) +
                        CollectionProxy.Count;
            }
        }

        private bool IsDataView
        {
            get { return _isDataView; }
        }

        /// <summary>
        /// Return index of item in the internal list.
        /// </summary>
        private int InternalIndexOf(object item)
        {
            if (_isGrouping)
            {
                return _group.LeafIndexOf(item);
            }

            if (item == NewItemPlaceholder)
            {
                switch (NewItemPlaceholderPosition)
                {
                    case NewItemPlaceholderPosition.None:
                        return -1;

                    case NewItemPlaceholderPosition.AtBeginning:
                        return 0;

                    case NewItemPlaceholderPosition.AtEnd:
                        return InternalCount - 1;
                }
            }
            else if (IsAddingNew && ItemsControl.EqualsEx(item, _newItem))
            {
                switch (NewItemPlaceholderPosition)
                {
                    case NewItemPlaceholderPosition.None:
                        break;

                    case NewItemPlaceholderPosition.AtBeginning:
                        return 1;

                    case NewItemPlaceholderPosition.AtEnd:
                        return InternalCount - 2;
                }
            }

            int index = CollectionProxy.IndexOf(item);

            // When you delete the last item from the list,
            // ADO returns a bad value.  Item will be "invalid", in the
            // sense that it is not connected to a table.  But IndexOf(item)
            // returns 10, even though there are only 10 entries in the list.
            // Looks like they're just returning item.Index without checking
            // anything.  So we have to do the checking for them.
            if (index >= CollectionProxy.Count)
            {
                index = -1;
            }

            if (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && index >= 0)
            {
                index += IsAddingNew ? 2 : 1;
            }

            return index;
        }

        /// <summary>
        /// Return item at the given index in the internal list.
        /// </summary>
        private object InternalItemAt(int index)
        {
            if (_isGrouping)
            {
                return _group.LeafAt(index);
            }

            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                    break;

                case NewItemPlaceholderPosition.AtBeginning:
                    if (index == 0)
                        return NewItemPlaceholder;
                    --index;

                    if (IsAddingNew)
                    {
                        if (index == 0)
                            return _newItem;
                        if (index <= _newItemIndex + 1)
                            --index;
                    }
                    break;

                case NewItemPlaceholderPosition.AtEnd:
                    if (index == InternalCount - 1)
                        return NewItemPlaceholder;
                    if (IsAddingNew && index == InternalCount - 2)
                        return _newItem;
                    break;
            }

            return CollectionProxy[index];
        }

        /// <summary>
        /// Return true if internal list contains the item.
        /// </summary>
        private bool InternalContains(object item)
        {
            if (item == NewItemPlaceholder)
                return (NewItemPlaceholderPosition != NewItemPlaceholderPosition.None);

            return (!_isGrouping) ? CollectionProxy.Contains(item) : (_group.LeafIndexOf(item) >= 0);
        }

        /// <summary>
        /// Return an enumerator for the internal list.
        /// </summary>
        private IEnumerator InternalGetEnumerator()
        {
            if (!_isGrouping)
            {
                return new PlaceholderAwareEnumerator(this, CollectionProxy.GetEnumerator(), NewItemPlaceholderPosition, _newItem);
            }
            else
            {
                return _group.GetLeafEnumerator();
            }
        }

        // Adjust the ShadowCopy so that it accurately reflects the state of the
        // Data Collection immediately after the CollectionChangeEvent
        private void AdjustShadowCopy(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _shadowList.Insert(e.NewStartingIndex, e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _shadowList.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _shadowList[e.OldStartingIndex] = e.NewItems[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    _shadowList.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        // true if CurrentPosition points to item within view
        private bool IsCurrentInView
        {
            get { return (0 <= CurrentPosition && CurrentPosition < InternalCount); }
        }

        // move to a given index
        private void _MoveTo(int proposed)
        {
            if (proposed == CurrentPosition || IsEmpty)
                return;

            object proposedCurrentItem = (0 <= proposed && proposed < InternalCount) ? GetItemAt(proposed) : null;

            if (proposedCurrentItem == NewItemPlaceholder)
                return;         // ignore moves to the placeholder

            if (OKToChangeCurrent())
            {
                bool oldIsCurrentAfterLast = IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                SetCurrent(proposedCurrentItem, proposed);

                OnCurrentChanged();

                // notify that the properties have changed.
                if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                    OnPropertyChanged(IsCurrentAfterLastPropertyName);

                if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                    OnPropertyChanged(IsCurrentBeforeFirstPropertyName);

                OnPropertyChanged(CurrentPositionPropertyName);
                OnPropertyChanged(CurrentItemPropertyName);
            }
        }

        // subscribe to change notifications
        private void SubscribeToChanges()
        {
            if (InternalList.SupportsChangeNotification)
            {
                BindingOperations.AccessCollection(InternalList,
                    () =>
                    {
                        InternalList.ListChanged += new ListChangedEventHandler(OnListChanged);
                        RebuildLists();
                    },
                    false);
            }
        }

        // IBindingList has changed
        // At this point we may not have entered the UIContext, but
        // the call to base.OnCollectionChanged will marshall the change over
        private void OnListChanged(object sender, ListChangedEventArgs args)
        {
            if (_ignoreInnerRefresh && (args.ListChangedType == ListChangedType.Reset))
                return;

            NotifyCollectionChangedEventArgs forwardedArgs = null;
            object item = null;
            int delta = _isGrouping ? 0 : (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
            int index = args.NewIndex;

            switch (args.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    // Some implementations of IBindingList raise an extra ItemAdded event
                    // when the new item (from a previous call to AddNew) is "committed".
                    // [The IBindingList documentation suggests that all implementations
                    // should do this, but only DataView seems to obey this rather
                    // bizarre requirement.]  We will ignore these extra events, unless
                    // they arise from a commit that we initiated.  There's
                    // no way to detect them from the event args;  we do it the same
                    // way WinForms.DataGridView does - by comparing counts.
                    if (InternalList.Count == _cachedList.Count)
                    {
                        if (IsAddingNew && index == _newItemIndex)
                        {
                            Debug.Assert(_newItem == InternalList[index], "unexpected item while committing AddNew");
                            forwardedArgs = ProcessCommitNew(index + delta, index + delta);
                        }
                    }
                    else
                    {
                        // normal ItemAdded event
                        item = InternalList[index];
                        forwardedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index + delta);
                        _cachedList.Insert(index, item);
                        if (InternalList.Count != _cachedList.Count)
                            throw new InvalidOperationException(string.Format(Strings.InconsistentBindingList, InternalList, args.ListChangedType));
                        if (index <= _newItemIndex)
                        {
                            ++_newItemIndex;
                        }
                    }
                    break;

                case ListChangedType.ItemDeleted:
                    item = _cachedList[index];
                    _cachedList.RemoveAt(index);
                    if (InternalList.Count != _cachedList.Count)
                        throw new InvalidOperationException(string.Format(Strings.InconsistentBindingList, InternalList, args.ListChangedType));
                    if (index < _newItemIndex)
                    {
                        --_newItemIndex;
                    }

                    // implicitly cancel AddNew and/or EditItem transactions if the relevant item is removed
                    if (item == CurrentEditItem)
                    {
                        ImplicitlyCancelEdit();
                    }
                    if (item == CurrentAddItem)
                    {
                        EndAddNew(true);

                        switch (NewItemPlaceholderPosition)
                        {
                            case NewItemPlaceholderPosition.AtBeginning:
                                index = 0;
                                break;
                            case NewItemPlaceholderPosition.AtEnd:
                                index = InternalCount - 1;
                                break;
                        }
                    }

                    forwardedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index + delta);
                    break;

                case ListChangedType.ItemMoved:
                    if (IsAddingNew && args.OldIndex == _newItemIndex)
                    {
                        // ItemMoved applied to the new item.  We assume this is the result
                        // of committing a new item when a sort is in effect - the item
                        // moves to its sorted position.  There's no way to verify this assumption.
                        item = _newItem;
                        Debug.Assert(item == InternalList[index], "unexpected item while committing AddNew");
                        forwardedArgs = ProcessCommitNew(args.OldIndex, index + delta);
                    }
                    else
                    {
                        // normal ItemMoved event
                        item = InternalList[index];
                        forwardedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index + delta, args.OldIndex + delta);
                        if (args.OldIndex < _newItemIndex && _newItemIndex < args.NewIndex)
                        {
                            --_newItemIndex;
                        }
                        else if (args.NewIndex <= _newItemIndex && _newItemIndex < args.OldIndex)
                        {
                            ++_newItemIndex;
                        }
                    }

                    _cachedList.RemoveAt(args.OldIndex);
                    _cachedList.Insert(args.NewIndex, item);
                    if (InternalList.Count != _cachedList.Count)
                        throw new InvalidOperationException(string.Format(Strings.InconsistentBindingList, InternalList, args.ListChangedType));
                    break;

                case ListChangedType.ItemChanged:
                    if (!_itemsRaisePropertyChanged.HasValue)
                    {
                        // check whether individual items raise PropertyChanged events
                        // (DataRowView does)
                        item = InternalList[args.NewIndex];
                        _itemsRaisePropertyChanged = (item is INotifyPropertyChanged);
                    }

                    // if items raise PropertyChanged, we can ignore ItemChanged;
                    // otherwise, treat it like a Reset
                    if (!_itemsRaisePropertyChanged.Value)
                    {
                        goto case ListChangedType.Reset;
                    }
                    break;

                case ListChangedType.Reset:
                // treat all other changes like Reset
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                    // implicitly cancel EditItem transactions
                    if (IsEditingItem)
                    {
                        ImplicitlyCancelEdit();
                    }

                    // adjust AddNew transactions, depending on whether the new item
                    // survived the Reset
                    if (IsAddingNew)
                    {
                        _newItemIndex = InternalList.IndexOf(_newItem);
                        if (_newItemIndex < 0)
                        {
                            EndAddNew(true);
                        }
                    }

                    RefreshOrDefer();
                    break;
            }

            if (forwardedArgs != null)
            {
                base.OnCollectionChanged(sender, forwardedArgs);
            }
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForAdd(int index)
        {
            if (InternalCount == 1)
            {
                // added first item; set current at BeforeFirst
                SetCurrent(null, -1);
            }
            else if (index <= CurrentPosition)  // adjust current index if insertion is earlier
            {
                int newPosition = CurrentPosition + 1;
                if (newPosition < InternalCount)
                {
                    // CurrentItem might be out of sync if underlying list is not INCC
                    // or if this Add is the result of a Replace (Rem + Add)
                    SetCurrent(GetItemAt(newPosition), newPosition);
                }
                else
                {
                    SetCurrent(null, InternalCount);
                }
            }
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        // return true if the current item was removed
        private bool AdjustCurrencyForRemove(int index)
        {
            bool result = (index == CurrentPosition);

            // adjust current index if deletion is earlier
            if (index < CurrentPosition)
            {
                SetCurrent(CurrentItem, CurrentPosition - 1);
            }

            return result;
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForMove(int oldIndex, int newIndex)
        {
            if (oldIndex == CurrentPosition)
            {
                // moving the current item - currency moves with the item (bug 1942184)
                SetCurrent(GetItemAt(newIndex), newIndex);
            }
            else if (oldIndex < CurrentPosition && CurrentPosition <= newIndex)
            {
                // moving an item from before current position to after -
                // current item shifts back one position
                SetCurrent(CurrentItem, CurrentPosition - 1);
            }
            else if (newIndex <= CurrentPosition && CurrentPosition < oldIndex)
            {
                // moving an item from after current position to before -
                // current item shifts ahead one position
                SetCurrent(CurrentItem, CurrentPosition + 1);
            }
            // else no change necessary
        }

        // fix up CurrentPosition and CurrentItem after a collection change
        // return true if the current item was replaced
        private bool AdjustCurrencyForReplace(int index)
        {
            bool result = (index == CurrentPosition);

            if (result)
            {
                SetCurrent(GetItemAt(index), index);
            }

            return result;
        }

        private void MoveCurrencyOffDeletedElement(int oldCurrentPosition)
        {
            int lastPosition = InternalCount - 1;   // OK if last is -1
            // if position falls beyond last position, move back to last position
            int newPosition = (oldCurrentPosition < lastPosition) ? oldCurrentPosition : lastPosition;

            OnCurrentChanging();

            if (newPosition < 0)
                SetCurrent(null, newPosition);
            else
                SetCurrent(InternalItemAt(newPosition), newPosition);

            OnCurrentChanged();
        }

        private IList CollectionProxy
        {
            get
            {
                if (_shadowList != null)
                    return _shadowList;
                else
                    return InternalList;
            }
        }
        /// <summary>
        /// Accessor to private _internalList field.
        /// </summary>
        private IBindingList InternalList
        {
            get { return _internalList; }
            set { _internalList = value; }
        }

        private bool IsCustomFilterSet
        {
            get { return ((_blv != null) && !String.IsNullOrEmpty(_customFilter)); }
        }

        // can the group name(s) for an item change after we've grouped the item?
        private bool CanGroupNamesChange
        {
            // There's no way we can deduce this - the app has to tell us.
            // If this is true, removing a grouped item is quite difficult.
            // We cannot rely on its group names to tell us which group we inserted
            // it into (they may have been different at insertion time), so we
            // have to do a linear search.
            get { return true; }
        }

        // SortDescription was added/removed, refresh CollView
        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, "Sorting"));

            RefreshOrDefer();
        }

        // convert from Avalon SortDescriptions to the corresponding .NET collection
        private ListSortDescriptionCollection ConvertSortDescriptionCollection(SortDescriptionCollection sorts)
        {
            PropertyDescriptorCollection pdc;
            ITypedList itl;
            Type itemType;

            if ((itl = InternalList as ITypedList) != null)
            {
                pdc = itl.GetItemProperties(null);
            }
            else if ((itemType = GetItemType(true)) != null)
            {
                pdc = TypeDescriptor.GetProperties(itemType);
            }
            else
            {
                pdc = null;
            }

            if ((pdc == null) || (pdc.Count == 0))
                throw new ArgumentException(Strings.CannotDetermineSortByPropertiesForCollection);

            ListSortDescription[] sortDescriptions = new ListSortDescription[sorts.Count];
            for (int i = 0; i < sorts.Count; i++)
            {
                PropertyDescriptor dd = pdc.Find(sorts[i].PropertyName, true);
                if (dd == null)
                {
                    string typeName = itl.GetListName(null);
                    throw new ArgumentException(string.Format(Strings.PropertyToSortByNotFoundOnType, typeName, sorts[i].PropertyName));
                }
                ListSortDescription sd = new ListSortDescription(dd, sorts[i].Direction);
                sortDescriptions[i] = sd;
            }

            return new ListSortDescriptionCollection(sortDescriptions);
        }

        // initialization for grouping that should happen before preparing the local array
        void InitializeGrouping()
        {
            // discard old groups
            _group.Clear();

            // initialize the synthetic top level group
            _group.Initialize();

            _isGrouping = (_group.GroupBy != null);
        }


        // divide the data items into groups
        void PrepareGroups()
        {
            if (!_isGrouping)
                return;

            IList list = CollectionProxy;

            // reset the grouping comparer
            IComparer comparer = ActiveComparer;
            if (comparer != null)
            {
                _group.ActiveComparer = comparer;
            }
            else
            {
                CollectionViewGroupInternal.IListComparer ilc = _group.ActiveComparer as CollectionViewGroupInternal.IListComparer;
                if (ilc != null)
                {
                    ilc.ResetList(list);
                }
                else
                {
                    _group.ActiveComparer = new CollectionViewGroupInternal.IListComparer(list);
                }
            }

            // loop through the sorted/filtered list of items, dividing them
            // into groups (with special cases for placeholder and new item)
            if (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
            {
                _group.InsertSpecialItem(0, NewItemPlaceholder, true /*loading*/);
                if (IsAddingNew)
                {
                    _group.InsertSpecialItem(1, _newItem, true /*loading*/);
                }
            }

            for (int k = 0, n = list.Count; k < n; ++k)
            {
                object item = list[k];

                if (!IsAddingNew || !ItemsControl.EqualsEx(_newItem, item))
                {
                    _group.AddToSubgroups(item, true /*loading*/);
                }
            }

            if (IsAddingNew && NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtBeginning)
            {
                _group.InsertSpecialItem(_group.Items.Count, _newItem, true /*loading*/);
            }
            if (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                _group.InsertSpecialItem(_group.Items.Count, NewItemPlaceholder, true /*loading*/);
            }
        }

        // For the Group to report collection changed
        void OnGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AdjustCurrencyForAdd(e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                AdjustCurrencyForRemove(e.OldStartingIndex);
            }
            OnCollectionChanged(e);
        }

        // The GroupDescriptions collection changed
        void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, "Grouping"));

            // This is a huge change.  Just refresh the view.
            RefreshOrDefer();
        }

        // A group description for one of the subgroups changed
        void OnGroupDescriptionChanged(object sender, EventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, "Grouping"));

            // This is a huge change.  Just refresh the view.
            RefreshOrDefer();
        }

        // An item was inserted into the collection.  Update the groups.
        void AddItemToGroups(object item)
        {
            if (IsAddingNew && item == _newItem)
            {
                int index;
                switch (NewItemPlaceholderPosition)
                {
                    case NewItemPlaceholderPosition.None:
                    default:
                        index = _group.Items.Count;
                        break;
                    case NewItemPlaceholderPosition.AtBeginning:
                        index = 1;
                        break;
                    case NewItemPlaceholderPosition.AtEnd:
                        index = _group.Items.Count - 1;
                        break;
                }

                _group.InsertSpecialItem(index, item, false /*loading*/);
            }
            else
            {
                _group.AddToSubgroups(item, false /*loading*/);
            }
        }

        // An item was removed from the collection.  Update the groups.
        void RemoveItemFromGroups(object item)
        {
            if (CanGroupNamesChange || _group.RemoveFromSubgroups(item))
            {
                // the item didn't appear where we expected it to.
                _group.RemoveItemFromSubgroupsByExhaustiveSearch(item);
            }
        }

        private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException(Strings.RangeActionsNotSupported);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException(Strings.RangeActionsNotSupported);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                        throw new NotSupportedException(Strings.RangeActionsNotSupported);
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException(Strings.RangeActionsNotSupported);
                    if (e.NewStartingIndex < 0)
                        throw new InvalidOperationException(Strings.CannotMoveToUnknownPosition);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    throw new NotSupportedException(string.Format(Strings.UnexpectedCollectionChangeAction, e.Action));
            }
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        // defer work until the current activity completes
        private void DeferAction(Action action)
        {
            if (_deferredActions == null)
            {
                _deferredActions = new List<Action>();
            }
            _deferredActions.Add(action);
        }

        // perform the deferred work, if any
        private void DoDeferredActions()
        {
            if (_deferredActions != null)
            {
                List<Action> deferredActions = _deferredActions;
                _deferredActions = null;

                foreach (Action action in deferredActions)
                {
                    action();
                }
            }
        }

        private sealed class BindingListSortDescriptionCollection : SortDescriptionCollection
        {
            internal BindingListSortDescriptionCollection(bool allowMultipleDescriptions)
            {
                _allowMultipleDescriptions = allowMultipleDescriptions;
            }

            /// <summary>
            /// called by base class ObservableCollection&lt;T&gt; when an item is added to list;
            /// </summary>
            protected override void InsertItem(int index, SortDescription item)
            {
                if (!_allowMultipleDescriptions && (this.Count > 0))
                {
                    throw new InvalidOperationException(Strings.BindingListCanOnlySortByOneProperty);
                }
                base.InsertItem(index, item);
            }

            private readonly bool _allowMultipleDescriptions;
        }

        private IBindingList _internalList;
        private readonly CollectionViewGroupRoot _group;
        private bool _isGrouping;
        private readonly IBindingListView _blv;
        private BindingListSortDescriptionCollection _sort;
        private List<object> _shadowList;
        private bool _isSorted;
        private IComparer _comparer;
        private string _customFilter;
        private bool _isFiltered;
        private bool _ignoreInnerRefresh;
        private bool? _itemsRaisePropertyChanged;
        private readonly bool _isDataView;
        private object _newItem = NoNewItem;
        private object _editItem;
        private int _newItemIndex;  // position of _newItem in the source collection
        private NewItemPlaceholderPosition _newItemPlaceholderPosition;
        private List<Action> _deferredActions;
        private bool _isRemoving;

        // to handle ItemRemoved directly, we need to remember the items -
        // IBL's event args tell us the index, not the item itself
        private List<object> _cachedList;
    }
}