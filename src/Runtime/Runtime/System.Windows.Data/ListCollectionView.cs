// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data
{
    /// <summary>
    /// Represents the collection view for collections that implement <see cref="IList"/>.
    /// </summary>
    public class ListCollectionView : CollectionView, IComparer, IEditableCollectionViewAddNewItem, IItemProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListCollectionView"/> class, using a supplied collection that 
        /// implements <see cref="IList"/>.
        /// </summary>
        /// <param name="list">
        /// The underlying collection, which must implement <see cref="IList"/>.
        /// </param>
        public ListCollectionView(IList list)
            : base(list)
        {
            if (AllowsCrossThreadChanges)
            {
                BindingOperations.AccessCollection(list,
                    () =>
                    {
                        ClearPendingChanges();
                        ShadowCollection = [.. (ICollection)SourceCollection];
                        _internalList = ShadowCollection;
                    },
                    false);
            }
            else
            {
                _internalList = list;
            }

            if (InternalList.Count == 0)    // don't call virtual IsEmpty in ctor
            {
                SetCurrent(null, -1, 0);
            }
            else
            {
                SetCurrent(InternalList[0], 0, 1);
            }

            _group = new CollectionViewGroupRoot(this);
            _group.GroupDescriptionChanged += new EventHandler(OnGroupDescriptionChanged);
            ((INotifyCollectionChanged)_group).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupChanged);
            ((INotifyCollectionChanged)_group.GroupDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(OnGroupByChanged);
        }

        /// <summary>
        /// Recreates the view.
        /// </summary>
        protected override void RefreshOverride()
        {
            if (AllowsCrossThreadChanges)
            {
                BindingOperations.AccessCollection(SourceCollection,
                    () =>
                    {
                        lock (SyncRoot)
                        {
                            ClearPendingChanges();
                            ShadowCollection = [.. (ICollection)SourceCollection];
                        }
                    },
                    false);
            }

            object oldCurrentItem = CurrentItem;
            int oldCurrentPosition = IsEmpty ? -1 : CurrentPosition;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

            // force currency off the collection (gives user a chance to save dirty information)
            OnCurrentChanging();

            PrepareLocalArray();

            if (oldIsCurrentBeforeFirst || IsEmpty)
            {
                SetCurrent(null, -1);
            }
            else if (oldIsCurrentAfterLast)
            {
                SetCurrent(null, InternalCount);
            }
            else // set currency back to old current item
            {
                // oldCurrentItem may be null

                // if there are duplicates, use the position of the first matching item
                int newPosition = InternalIndexOf(oldCurrentItem);

                if (newPosition < 0)
                {
                    // oldCurrentItem not found: move to first item
                    object newItem;
                    newPosition = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
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

            return InternalContains(item);
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


            if (position != CurrentPosition || !IsCurrentInSync)
            {
                object proposedCurrentItem = (0 <= position && position < InternalCount) ? InternalItemAt(position) : null;

                // ignore moves to the placeholder
                if (proposedCurrentItem != NewItemPlaceholder)
                {
                    if (OKToChangeCurrent())
                    {
                        bool oldIsCurrentAfterLast = IsCurrentAfterLast;
                        bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                        SetCurrent(proposedCurrentItem, position);

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
            }

            return IsCurrentInView;
        }

        /// <summary>
        /// Gets a value that indicates whether the collection view supports grouping.
        /// </summary>
        /// <returns>
        /// true if the collection view supports grouping; otherwise, false.
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
            get { return (IsGrouping) ? _group.Items : null; }
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
            return ActiveFilter == null || ActiveFilter(item);
        }

        /// <summary>
        /// Returns the index where the given data item belongs in the collection, or -1 if the index of that item is unknown.
        /// </summary>
        /// <param name="item">
        /// The object to check for in the collection.
        /// </param>
        /// <returns>
        /// The index of the item in the collection, or -1 if the item does not exist in the collection.
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

        /// <summary> Return -, 0, or +, according to whether o1 occurs before, at, or after o2 (respectively)
        /// </summary>
        /// <param name="o1">first object</param>
        /// <param name="o2">second object</param>
        /// <remarks>
        /// Compares items by their resp. index in the IList.
        /// </remarks>
        int IComparer.Compare(object o1, object o2)
        {
            return Compare(o1, o2);
        }

        /// <summary>
        /// Compares two objects and returns a value that indicates whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="o1">
        /// The first object to compare.
        /// </param>
        /// <param name="o2">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// Less than zero if o1 is less than o2, zero if o1 and o2 are equal, or greater than zero if o1 is greater than o2.
        /// </returns>
        protected virtual int Compare(object o1, object o2)
        {
            if (!IsGrouping)
            {
                if (ActiveComparer != null)
                    return ActiveComparer.Compare(o1, o2);

                int i1 = InternalList.IndexOf(o1);
                int i2 = InternalList.IndexOf(o2);
                return (i1 - i2);
            }
            else
            {
                int i1 = InternalIndexOf(o1);
                int i2 = InternalIndexOf(o2);
                return (i1 - i2);
            }
        }

        /// <summary>
        /// Returns an object that you can use to enumerate the items in the view.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that you can use to enumerate the items in the view.
        /// </returns>
        protected override IEnumerator GetEnumerator()
        {
            VerifyRefreshNotDeferred();

            return InternalGetEnumerator();
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
                if (_sort == null)
                    SetSortDescriptions(new SortDescriptionCollection());
                return _sort;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection view supports sorting.
        /// </summary>
        /// <returns>
        /// For a default instance of <see cref="ListCollectionView"/>, this property always returns true.
        /// </returns>
        public override bool CanSort
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports callback-based filtering.
        /// </summary>
        /// <returns>
        /// For a default instance of <see cref="ListCollectionView"/>, this property always returns true.
        /// </returns>
        public override bool CanFilter
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a method that is used to determine whether an item is suitable for inclusion in the view.
        /// </summary>
        /// <returns>
        ///  A delegate that represents the method that is used to determine whether an item is suitable for inclusion in the view.
        /// </returns>
        public override Predicate<object> Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                if (AllowsCrossThreadChanges)
                    VerifyAccess();
                if (IsAddingNew || IsEditingItem)
                    throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(Filter)));

                base.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets a custom object that implements <see cref="IComparer"/> to sort items in the view.
        /// </summary>
        /// <returns>
        /// The sort criteria as an implementation of <see cref="IComparer"/>.
        /// </returns>
        public IComparer CustomSort
        {
            get { return _customSort; }
            set
            {
                if (AllowsCrossThreadChanges)
                    VerifyAccess();
                if (IsAddingNew || IsEditingItem)
                    throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(CustomSort)));
                _customSort = value;

                SetSortDescriptions(null);

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets or sets a delegate to select the <see cref="GroupDescription"/> as a function of the parent group and its level.
        /// </summary>
        /// <returns>
        /// A method that provides the logic for the selection of the <see cref="GroupDescription"/> as a function of the parent 
        /// group and its level. The default value is null.
        /// </returns>
        [DefaultValue(null)]
        public virtual GroupDescriptionSelectorCallback GroupBySelector
        {
            get { return _group.GroupBySelector; }
            set
            {
                if (!CanGroup)
                    throw new NotSupportedException();
                if (IsAddingNew || IsEditingItem)
                    throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, "Grouping"));

                _group.GroupBySelector = value;

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets the estimated number of records.
        /// </summary>
        /// <returns>
        /// One of the following:
        /// 
        /// Value – Meaning
        /// -1 – Could not determine the count of the collection. This might be returned by a "virtualizing" view, 
        /// where the view deliberately does not account for all items in the underlying collection because the 
        /// view is trying to increase efficiency and minimize dependence on always having the whole collection 
        /// available.
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
            get { return InternalCount == 0; }
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
        /// Gets or sets the position of the new item placeholder in the <see cref="ListCollectionView"/>.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the position of the new item placeholder in the <see cref="ListCollectionView"/>.
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

                    if (!IsGrouping)
                    {
                        ProcessCollectionChangedWithAdjustedIndex(args, oldIndex, newIndex);
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
            get { return !IsEditingItem && !SourceList.IsFixedSize && CanConstructItem; }
        }

        /// <summary>
        /// Gets a value that indicates whether a specified object can be added to the collection.
        /// </summary>
        /// <returns>
        /// true if a specified object can be added to the collection; otherwise, false.
        /// </returns>
        public bool CanAddNewItem
        {
            get { return !IsEditingItem && !SourceList.IsFixedSize; }
        }

        bool CanConstructItem
        {
            get
            {
                if (!_isItemConstructorValid)
                {
                    EnsureItemConstructor();
                }

                return (_itemConstructor != null);
            }
        }

        void EnsureItemConstructor()
        {
            if (!_isItemConstructorValid)
            {
                Type itemType = GetItemType(true);
                if (itemType != null)
                {
                    _itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
                    _isItemConstructorValid = true;
                }
            }
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

            return AddNewCommon(_itemConstructor.Invoke(null));
        }

        /// <summary>
        /// Adds the specified object to the collection.
        /// </summary>
        /// <param name="newItem">
        /// The object to add to the collection.
        /// </param>
        /// <returns>
        /// The object that was added to the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// An object cannot be added to the <see cref="ListCollectionView"/> by using the <see cref="AddNewItem(object)"/> method.
        /// </exception>
        public object AddNewItem(object newItem)
        {
            VerifyRefreshNotDeferred();

            if (IsEditingItem)
            {
                CommitEdit();   // implicitly close a previous EditItem
            }

            CommitNew();        // implicitly close a previous AddNew

            if (!CanAddNewItem)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedForView, nameof(AddNewItem)));

            return AddNewCommon(newItem);
        }

        object AddNewCommon(object newItem)
        {
            BindingOperations.AccessCollection(SourceList,
                () =>
                {
                    ProcessPendingChanges();    // bring the shadow list up to date

                    _newItemIndex = -2; // this is a signal that the next Add event comes from AddNew
                    int index = SourceList.Add(newItem);

                    // if the source doesn't raise collection change events, fake one
                    if (!(SourceList is INotifyCollectionChanged))
                    {
                        // the index returned by IList.Add isn't always reliable
                        if (!ItemsControl.EqualsEx(newItem, SourceList[index]))
                        {
                            index = SourceList.IndexOf(newItem);
                        }

                        BeginAddNew(newItem, index);
                    }
                },
                true);

            Debug.Assert(_newItemIndex != -2 && ItemsControl.EqualsEx(newItem, _newItem), "AddNew did not raise expected events");

            MoveCurrentTo(newItem);

            ISupportInitialize isi = newItem as ISupportInitialize;
            isi?.BeginInit();

            IEditableObject ieo = newItem as IEditableObject;
            ieo?.BeginEdit();

            return newItem;
        }

        // Calling IList.Add() will raise an ItemAdded event.  We handle this specially
        // to adjust the position of the new item in the view (it should be adjacent
        // to the placeholder), and cache the new item for use by the other APIs
        // related to AddNew.  This method is called from ProcessCollectionChanged.
        void BeginAddNew(object newItem, int index)
        {
            Debug.Assert(_newItemIndex == -2 && _newItem == NoNewItem, "unexpected call to BeginAddNew");

            // remember the new item and its position in the underlying list
            SetNewItem(newItem);
            _newItemIndex = index;

            // adjust the position of the new item
            int position = -1;
            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                    position = UsesLocalArray ? InternalCount - 1 : _newItemIndex;
                    break;
                case NewItemPlaceholderPosition.AtBeginning:
                    position = 1;
                    break;
                case NewItemPlaceholderPosition.AtEnd:
                    position = InternalCount - 2;
                    break;
            }

            // raise events as if the new item appeared in the adjusted position
            ProcessCollectionChangedWithAdjustedIndex(
                                        new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                newItem,
                                                position),
                                        -1, position);
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

            // grouping works differently
            if (IsGrouping)
            {
                CommitNewForGrouping();
                return;
            }

            // from the POV of view clients, the new item is moving from its
            // position adjacent to the placeholder to its real position.
            // Remember its current position (have to do this before calling EndNew,
            // because InternalCount depends on "adding-new" mode).
            int fromIndex = 0;
            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                    fromIndex = UsesLocalArray ? InternalCount - 1 : _newItemIndex;
                    break;
                case NewItemPlaceholderPosition.AtBeginning:
                    fromIndex = 1;
                    break;
                case NewItemPlaceholderPosition.AtEnd:
                    fromIndex = InternalCount - 2;
                    break;
            }

            // End the AddNew transaction
            object newItem = EndAddNew(false);

            // Tell the view clients what happened to the new item
            int toIndex = AdjustBefore(NotifyCollectionChangedAction.Add, newItem, _newItemIndex);

            if (toIndex < 0)
            {
                // item is effectively removed (due to filter), raise a Remove event
                ProcessCollectionChangedWithAdjustedIndex(
                            new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                newItem,
                                                fromIndex),
                            fromIndex, -1);
            }
            else if (fromIndex == toIndex)
            {
                // item isn't moving, so no events are needed.  But the item does need
                // to be added to the local array.
                if (UsesLocalArray)
                {
                    if (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
                    {
                        --toIndex;
                    }
                    InternalList.Insert(toIndex, newItem);
                }
            }
            else
            {
                // item is moving
                ProcessCollectionChangedWithAdjustedIndex(
                            new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Move,
                                                newItem,
                                                toIndex, fromIndex),
                            fromIndex, toIndex);
            }
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
            int newItemIndex = _newItemIndex;
            object newItem = EndAddNew(false);

            // remove item from its temporary position
            _group.RemoveSpecialItem(index, newItem, false /*loading*/);

            // now pretend it just got added to the collection.  This will add it
            // to the internal list with sort/filter, and to the groups
            ProcessCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                newItem,
                                newItemIndex));
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

            // remove the new item from the underlying collection.  Normally the
            // collection will raise a Remove event, which we'll handle by calling
            // EndNew to leave AddNew mode.
            BindingOperations.AccessCollection(SourceList,
                () =>
                {
                    ProcessPendingChanges();
                    SourceList.RemoveAt(_newItemIndex);

                    // if the collection doesn't raise events, do the work explicitly on its behalf
                    if (_newItem != NoNewItem)
                    {
                        int index = AdjustBefore(NotifyCollectionChangedAction.Remove, _newItem, _newItemIndex);
                        object newItem = EndAddNew(true);

                        ProcessCollectionChangedWithAdjustedIndex(
                                    new NotifyCollectionChangedEventArgs(
                                                        NotifyCollectionChangedAction.Remove,
                                                        newItem,
                                                        index),
                                    index, -1);
                    }
                },
                true);
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
            get { return !IsEditingItem && !IsAddingNew && !SourceList.IsFixedSize; }
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
        /// <returns>
        /// The item to remove.
        /// </returns>
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

            BindingOperations.AccessCollection(SourceList,
                () =>
                {
                    ProcessPendingChanges();

                    // the pending changes may have moved (or even removed) the
                    // item.   Verify the index.
                    if (index >= InternalCount || !ItemsControl.EqualsEx(item, GetItemAt(index)))
                    {
                        index = InternalIndexOf(item);
                        if (index < 0)
                            return;
                    }

                    // convert the index from "view-relative" to "list-relative"
                    int delta = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;

                    int listIndex = index - delta;
                    bool raiseEvent = !(SourceList is INotifyCollectionChanged);

                    // remove the item from the list
                    try
                    {
                        _isRemoving = true;
                        if (UsesLocalArray || IsGrouping)
                        {
                            if (raiseEvent)
                            {
                                listIndex = SourceList.IndexOf(item);
                                SourceList.RemoveAt(listIndex);
                            }
                            else
                            {
                                SourceList.Remove(item);
                            }
                        }
                        else
                        {
                            SourceList.RemoveAt(listIndex);
                        }

                        // if the list doesn't raise CollectionChanged events, fake one
                        if (raiseEvent)
                        {
                            ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                        NotifyCollectionChangedAction.Remove,
                                                        item,
                                                        listIndex));
                        }
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

            object editItem = _editItem;
            IEditableObject ieo = _editItem as IEditableObject;
            SetEditItem(null);

            ieo?.EndEdit();

            // see if the item is entering or leaving the view
            int fromIndex = InternalIndexOf(editItem);
            bool wasInView = (fromIndex >= 0);
            bool isInView = wasInView ? PassesFilter(editItem)
                                    : SourceList.Contains(editItem) && PassesFilter(editItem);

            // editing may change the item's group names (and we can't tell whether
            // it really did).  The best we can do is remove the item and re-insert
            // it.
            if (IsGrouping)
            {
                if (wasInView)
                {
                    RemoveItemFromGroups(editItem);
                }
                if (isInView)
                {
                    AddItemToGroups(editItem);
                }
                return;
            }

            // the edit may cause the item to move.  If so, report it.
            if (UsesLocalArray)
            {
                IList list = InternalList;
                int delta = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
                int toIndex = -1;

                if (wasInView)
                {
                    if (!isInView)
                    {
                        // the item has been effectively removed
                        ProcessCollectionChangedWithAdjustedIndex(
                                    new NotifyCollectionChangedEventArgs(
                                                        NotifyCollectionChangedAction.Remove,
                                                        editItem,
                                                        fromIndex),
                                    fromIndex, -1);
                    }
                    else if (ActiveComparer != null)
                    {
                        // the item may have moved within the view
                        int localIndex = fromIndex - delta;
                        if (localIndex > 0 && ActiveComparer.Compare(list[localIndex - 1], editItem) > 0)
                        {
                            // the item has moved toward the front of the list
                            toIndex = list.Search(0, localIndex, editItem, ActiveComparer);
                            if (toIndex < 0)
                                toIndex = ~toIndex;
                        }
                        else if (localIndex < list.Count - 1 && ActiveComparer.Compare(editItem, list[localIndex + 1]) > 0)
                        {
                            // the item has moved toward the back of the list
                            toIndex = list.Search(localIndex + 1, list.Count - localIndex - 1, editItem, ActiveComparer);
                            if (toIndex < 0)
                                toIndex = ~toIndex;
                            --toIndex;      // because the item is leaving its old position
                        }

                        if (toIndex >= 0)
                        {
                            // the item has effectively moved
                            ProcessCollectionChangedWithAdjustedIndex(
                                        new NotifyCollectionChangedEventArgs(
                                                            NotifyCollectionChangedAction.Move,
                                                            editItem,
                                                            toIndex + delta, fromIndex),
                                        fromIndex, toIndex + delta);
                        }
                    }
                }
                else if (isInView)
                {
                    // the item has effectively been added
                    toIndex = AdjustBefore(NotifyCollectionChangedAction.Add, editItem, SourceList.IndexOf(editItem));
                    ProcessCollectionChangedWithAdjustedIndex(
                                new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Add,
                                            editItem,
                                            toIndex + delta),
                                -1, toIndex + delta);
                }
            }
        }

        /// <summary>
        /// Ends the edit transaction, and if possible, restores the original value to the item.
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
                OnPropertyChanged(nameof(CanAddNewItem));
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

        /// <summary>
        /// Occurs when the <see cref="CollectionView.AllowsCrossThreadChanges"/> property changes.
        /// </summary>
        protected override void OnAllowsCrossThreadChangesChanged()
        {
            if (AllowsCrossThreadChanges)
            {
                BindingOperations.AccessCollection(SourceCollection,
                    () =>
                    {
                        lock (SyncRoot)
                        {
                            ClearPendingChanges();
                            ShadowCollection = [.. (ICollection)SourceCollection];

                            if (!UsesLocalArray)
                            {
                                _internalList = ShadowCollection;
                            }
                        }
                    },
                    false);
            }
            else
            {
                ShadowCollection = null;
                if (!UsesLocalArray)
                {
                    _internalList = SourceList;
                }
            }
        }

        /// <summary>
        /// Called by the base class to notify the derived class that a <see cref="INotifyCollectionChanged.CollectionChanged"/>
        /// event has been posted to the message queue.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object that is added to the change log.
        /// </param>
        [Obsolete("Replaced by OnAllowsCrossThreadChangesChanged")]
        protected override void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args)
        {
        }

        /// <summary>
        /// Handles <see cref="INotifyCollectionChanged.CollectionChanged"/> events.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object to process.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="args"/> is null.
        /// </exception>
        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            ValidateCollectionChangedEventArgs(args);

            // adding or replacing an item can change CanAddNew, by providing a
            // non-null representative
            if (!_isItemConstructorValid)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                        OnPropertyChanged(nameof(CanAddNew));
                        break;
                }
            }

            int adjustedOldIndex = -1;
            int adjustedNewIndex = -1;

            // apply the change to the shadow copy
            if (AllowsCrossThreadChanges)
            {
                if (args.Action != NotifyCollectionChangedAction.Reset)
                {
                    if (args.Action != NotifyCollectionChangedAction.Remove && args.NewStartingIndex < 0
                        || args.Action != NotifyCollectionChangedAction.Add && args.OldStartingIndex < 0)
                    {
                        Debug.Assert(false, "Cannot update collection view from outside UIContext without index in event args");
                        return;     // support cross-thread changes from all collections
                    }
                    else
                    {
                        AdjustShadowCopy(args);
                    }
                }
            }

            // If the Action is Reset then we do a Refresh.
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                // implicitly cancel EditItem transactions
                if (IsEditingItem)
                {
                    ImplicitlyCancelEdit();
                }

                // adjust AddNew transactions, depending on whether the new item
                // survived the Reset
                if (IsAddingNew)
                {
                    _newItemIndex = SourceList.IndexOf(_newItem);
                    if (_newItemIndex < 0)
                    {
                        EndAddNew(true);
                    }
                }

                RefreshOrDefer();
                return; // the Refresh raises collection change event, so there's nothing left to do
            }

            if (args.Action == NotifyCollectionChangedAction.Add && _newItemIndex == -2)
            {
                // The Add event came from AddNew.
                BeginAddNew(args.NewItems[0], args.NewStartingIndex);
                return;
            }

            // If the Action is one that can be expected to have a valid NewItems[0] and NewStartingIndex then
            // adjust the index for filtering and sorting.
            if (args.Action != NotifyCollectionChangedAction.Remove)
            {
                adjustedNewIndex = AdjustBefore(NotifyCollectionChangedAction.Add, args.NewItems[0], args.NewStartingIndex);
            }

            // If the Action is one that can be expected to have a valid OldItems[0] and OldStartingIndex then
            // adjust the index for filtering and sorting.
            if (args.Action != NotifyCollectionChangedAction.Add)
            {
                adjustedOldIndex = AdjustBefore(NotifyCollectionChangedAction.Remove, args.OldItems[0], args.OldStartingIndex);

                // the new index needs further adjustment if the action removes (or moves)
                // something before it
                if (UsesLocalArray && adjustedOldIndex >= 0 && adjustedOldIndex < adjustedNewIndex)
                {
                    --adjustedNewIndex;
                }
            }

            // handle interaction with AddNew and EditItem
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (IsAddingNew && args.NewStartingIndex <= _newItemIndex)
                    {
                        ++_newItemIndex;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (IsAddingNew && args.OldStartingIndex < _newItemIndex)
                    {
                        --_newItemIndex;
                    }

                    // implicitly cancel AddNew and/or EditItem transactions if the relevant item is removed
                    object item = args.OldItems[0];

                    if (item == CurrentEditItem)
                    {
                        ImplicitlyCancelEdit();
                    }
                    else if (item == CurrentAddItem)
                    {
                        EndAddNew(true);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (IsAddingNew)
                    {
                        if (args.OldStartingIndex == _newItemIndex)
                        {
                            _newItemIndex = args.NewStartingIndex;
                        }
                        else if (args.OldStartingIndex < _newItemIndex && _newItemIndex <= args.NewStartingIndex)
                        {
                            --_newItemIndex;
                        }
                        else if (args.NewStartingIndex <= _newItemIndex && _newItemIndex < args.OldStartingIndex)
                        {
                            ++_newItemIndex;
                        }
                    }

                    if (ActiveComparer != null && adjustedOldIndex == adjustedNewIndex)
                    {
                        // when we're sorting, ignore Move from the underlying collection -
                        // the position is irrelevant
                        return;
                    }
                    break;
            }

            ProcessCollectionChangedWithAdjustedIndex(args, adjustedOldIndex, adjustedNewIndex);
        }

        void ProcessCollectionChangedWithAdjustedIndex(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex)
        {
            // Finding out the effective Action after filtering and sorting.
            //
            NotifyCollectionChangedAction effectiveAction = args.Action;
            if (adjustedOldIndex == adjustedNewIndex && adjustedOldIndex >= 0)
            {
                effectiveAction = NotifyCollectionChangedAction.Replace;
            }
            else if (adjustedOldIndex == -1) // old index is unknown
            {
                // we weren't told the old index, but it may have been in the view.
                if (adjustedNewIndex < 0)
                {
                    // The new item will not be in the filtered view,
                    // so an Add is a no-op and anything else is a Remove.
                    if (args.Action != NotifyCollectionChangedAction.Add)
                    {
                        effectiveAction = NotifyCollectionChangedAction.Remove;
                    }
                }
            }
            else if (adjustedOldIndex < -1) // old item is known to be NOT in filtered view
            {
                if (adjustedNewIndex >= 0)
                {
                    // item changes from filtered to unfiltered - effectively it's an Add
                    effectiveAction = NotifyCollectionChangedAction.Add;
                }
                else if (effectiveAction == NotifyCollectionChangedAction.Move)
                {
                    // filtered item has moved - nothing to do
                    return;
                }
                // otherwise since the old item wasn't in the filtered view, and the new
                // item would not be in the filtered view, this is a no-op.
                // (Except that we may have to remove an entry from the internal
                // live-filtered list.)
            }
            else // old item was in view
            {
                if (adjustedNewIndex < 0)
                {
                    effectiveAction = NotifyCollectionChangedAction.Remove;
                }
                else
                {
                    effectiveAction = NotifyCollectionChangedAction.Move;
                }
            }

            int delta = IsGrouping ? 0 :
                        (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ?
                        (IsAddingNew ? 2 : 1) : 0;

            int originalCurrentPosition = CurrentPosition;
            int oldCurrentPosition = CurrentPosition;
            object oldCurrentItem = CurrentItem;
            bool oldIsCurrentAfterLast = IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

            object oldItem = (args.OldItems != null && args.OldItems.Count > 0) ? args.OldItems[0] : null;
            object newItem = (args.NewItems != null && args.NewItems.Count > 0) ? args.NewItems[0] : null;

            // in the case of a replace that has a new adjustedPosition
            // (likely caused by sorting), the only way to effectively communicate
            // this change is through raising Remove followed by Insert.
            NotifyCollectionChangedEventArgs args2 = null;

            switch (effectiveAction)
            {
                case NotifyCollectionChangedAction.Add:
                    // when adding a filtered item, all we need to do is enroll it
                    // for live filtering
                    if (adjustedNewIndex == -2)
                    {
                        return;
                    }

                    // insert into private view
                    // (unless it's a special item - placeholder or new item)
                    bool isSpecialItem = (newItem == NewItemPlaceholder ||
                                            (IsAddingNew && ItemsControl.EqualsEx(_newItem, newItem)));
                    if (UsesLocalArray && !isSpecialItem)
                    {
                        InternalList.Insert(adjustedNewIndex - delta, newItem);
                    }

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForAdd(adjustedNewIndex);
                        args = new NotifyCollectionChangedEventArgs(effectiveAction, newItem, adjustedNewIndex);
                    }
                    else
                    {
                        AddItemToGroups(newItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    // when removing a filtered item, all we need do is remove it from
                    // live filtering
                    if (adjustedOldIndex == -2)
                    {
                        return;
                    }

                    // remove from private view, unless it's not there to start with
                    // (e.g. when CommitNew is applied to an item that fails the filter)
                    if (UsesLocalArray)
                    {
                        int localOldIndex = adjustedOldIndex - delta;

                        if (localOldIndex < InternalList.Count && ItemsControl.EqualsEx(InternalList[localOldIndex], oldItem))
                        {
                            InternalList.RemoveAt(localOldIndex);
                        }
                    }

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForRemove(adjustedOldIndex);
                        args = new NotifyCollectionChangedEventArgs(effectiveAction, args.OldItems[0], adjustedOldIndex);
                    }
                    else
                    {
                        RemoveItemFromGroups(oldItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    // when replacing a filtered item, all we need do is update
                    // the live filtering list
                    if (adjustedOldIndex == -2)
                    {
                        return;
                    }

                    // replace item in private view
                    if (UsesLocalArray)
                    {
                        InternalList[adjustedOldIndex - delta] = newItem;
                    }

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForReplace(adjustedOldIndex);
                        args = new NotifyCollectionChangedEventArgs(effectiveAction, args.NewItems[0], args.OldItems[0], adjustedOldIndex);
                    }
                    else
                    {
                        RemoveItemFromGroups(oldItem);
                        AddItemToGroups(newItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    // move within private view

                    bool simpleMove = ItemsControl.EqualsEx(oldItem, newItem);

                    if (UsesLocalArray)
                    {
                        int localOldIndex = adjustedOldIndex - delta;
                        int localNewIndex = adjustedNewIndex - delta;

                        // move the item to its new position, except in special cases
                        if (localOldIndex < InternalList.Count &&
                            ItemsControl.EqualsEx(InternalList[localOldIndex], oldItem))
                        {
                            if (NewItemPlaceholder != newItem)
                            {
                                // normal case - just move, and possibly replace
                                InternalList.Move(localOldIndex, localNewIndex);

                                if (!simpleMove)
                                {
                                    InternalList[localNewIndex] = newItem;
                                }
                            }
                            else
                            {
                                // moving the placeholder - just remove it
                                InternalList.RemoveAt(localOldIndex);
                            }
                        }
                        else
                        {
                            if (NewItemPlaceholder != newItem)
                            {
                                // old item wasn't present - just insert
                                // (this happens when the item is the object of CommitNew)
                                InternalList.Insert(localNewIndex, newItem);
                            }
                            else
                            {
                                // the remaining case - old item absent, new item is placeholder -
                                // is a no-op
                            }
                        }
                    }

                    if (!IsGrouping)
                    {
                        AdjustCurrencyForMove(adjustedOldIndex, adjustedNewIndex);

                        if (simpleMove)
                        {
                            // simple move
                            args = new NotifyCollectionChangedEventArgs(effectiveAction, args.OldItems[0], adjustedNewIndex, adjustedOldIndex);
                        }
                        else
                        {
                            // move/replace
                            args2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, args.NewItems, adjustedNewIndex);
                            args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, args.OldItems, adjustedOldIndex);
                        }
                    }
                    else
                    {
                        if (simpleMove)
                        {
                            // simple move
                            MoveItemWithinGroups(oldItem, adjustedOldIndex, adjustedNewIndex);
                        }
                        else
                        {
                            // move/replace
                            RemoveItemFromGroups(oldItem);
                            AddItemToGroups(newItem);
                        }
                    }
                    break;
                default:
                    Debug.Assert(false, string.Format(Strings.UnexpectedCollectionChangeAction, effectiveAction));
                    break;
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

            // base class will raise an event to our listeners
            if (!IsGrouping)
            {
                // we've already returned if (args.Action == NotifyCollectionChangedAction.Reset) above
                OnCollectionChanged(args);
                if (args2 != null)
                    OnCollectionChanged(args2);

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
            if (_currentElementWasRemoved)
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
        /// Returns the index of the specified item in the <see cref="InternalList"/>.
        /// </summary>
        /// <param name="item">
        /// The item to return an index for.
        /// </param>
        /// <returns>
        /// The index of the specified item in the <see cref="InternalList"/>.
        /// </returns>
        protected int InternalIndexOf(object item)
        {
            if (IsGrouping)
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
                        if (UsesLocalArray)
                        {
                            return InternalCount - 1;
                        }
                        break;

                    case NewItemPlaceholderPosition.AtBeginning:
                        return 1;

                    case NewItemPlaceholderPosition.AtEnd:
                        return InternalCount - 2;
                }
            }

            int index = InternalList.IndexOf(item);

            if (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && index >= 0)
            {
                index += IsAddingNew ? 2 : 1;
            }

            return index;
        }

        /// <summary>
        /// Returns the item at the given index in the <see cref="InternalList"/>.
        /// </summary>
        /// <param name="index">
        /// The index at which the item is located.
        /// </param>
        /// <returns>
        /// The item at the specified zero-based index in the view.
        /// </returns>
        protected object InternalItemAt(int index)
        {
            if (IsGrouping)
            {
                return _group.LeafAt(index);
            }

            switch (NewItemPlaceholderPosition)
            {
                case NewItemPlaceholderPosition.None:
                    if (IsAddingNew && UsesLocalArray)
                    {
                        if (index == InternalCount - 1)
                            return _newItem;
                    }
                    break;

                case NewItemPlaceholderPosition.AtBeginning:
                    if (index == 0)
                        return NewItemPlaceholder;
                    --index;

                    if (IsAddingNew)
                    {
                        if (index == 0)
                            return _newItem;

                        if (UsesLocalArray || index <= _newItemIndex)
                        {
                            --index;
                        }
                    }
                    break;

                case NewItemPlaceholderPosition.AtEnd:
                    if (index == InternalCount - 1)
                        return NewItemPlaceholder;
                    if (IsAddingNew)
                    {
                        if (index == InternalCount - 2)
                            return _newItem;
                        if (!UsesLocalArray && index >= _newItemIndex)
                            ++index;
                    }
                    break;
            }

            return InternalList[index];
        }

        /// <summary>
        /// Return a value that indicates whether the <see cref="InternalList"/> contains the item.
        /// </summary>
        /// <param name="item">
        /// The item to locate.
        /// </param>
        /// <returns>
        /// true if the <see cref="InternalList"/> contains the item; otherwise, false.
        /// </returns>
        protected bool InternalContains(object item)
        {
            if (item == NewItemPlaceholder)
                return (NewItemPlaceholderPosition != NewItemPlaceholderPosition.None);

            return (!IsGrouping) ? InternalList.Contains(item) : (_group.LeafIndexOf(item) >= 0);
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="InternalList"/>.
        /// </summary>
        /// <returns>
        /// An enumerator for the <see cref="InternalList"/>.
        /// </returns>
        protected IEnumerator InternalGetEnumerator()
        {
            if (!IsGrouping)
            {
                return new PlaceholderAwareEnumerator(this, InternalList.GetEnumerator(), NewItemPlaceholderPosition, _newItem);
            }
            else
            {
                return _group.GetLeafEnumerator();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a private copy of the data is needed for sorting and filtering.
        /// </summary>
        /// <returns>
        /// true if a private copy of the data is needed for sorting and filtering; otherwise, false. The default implementation 
        /// returns true if there is an <see cref="ActiveFilter"/> or <see cref="ActiveComparer"/>, or both.
        /// </returns>
        protected bool UsesLocalArray
        {
            get { return ActiveComparer != null || ActiveFilter != null; }
        }

        /// <summary>
        /// Gets the filtered and sorted list of items.
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/> on which filtering and sorting have been applied.
        /// </returns>
        protected IList InternalList
        {
            get { return _internalList; }
        }

        /// <summary>
        /// Gets or sets the current active comparer that is used in sorting.
        /// </summary>
        /// <returns>
        /// An <see cref="IComparer"/> object that is the active comparer.
        /// </returns>
        protected IComparer ActiveComparer
        {
            get { return _activeComparer; }
            set { _activeComparer = value; }
        }

        /// <summary>
        /// Gets or sets the current active <see cref="Filter"/> callback.
        /// </summary>
        /// <returns>
        /// The active <see cref="Filter"/> callback.
        /// </returns>
        protected Predicate<object> ActiveFilter
        {
            get { return _activeFilter; }
            set { _activeFilter = value; }
        }

        /// <summary>
        /// Gets a value that indicates whether there are groups in the view.
        /// </summary>
        /// <returns>
        /// true if there are groups in the view; otherwise, false.
        /// </returns>
        protected bool IsGrouping
        {
            get { return _isGrouping; }
        }

        /// <summary>
        /// Gets the number of records in the <see cref="InternalList"/>.
        /// </summary>
        /// <returns>
        /// The number of records in the <see cref="InternalList"/>.
        /// </returns>
        protected int InternalCount
        {
            get
            {
                if (IsGrouping)
                    return _group.ItemCount;

                int delta = (NewItemPlaceholderPosition == NewItemPlaceholderPosition.None) ? 0 : 1;
                if (UsesLocalArray && IsAddingNew)
                    ++delta;

                return delta + InternalList.Count;
            }
        }

        /// <summary>
        ///     Contains a snapshot of the ICollectionView.SourceCollection
        ///     at the time that a change notification is posted.
        ///     This is done in OnBeginChangeLogging.
        /// </summary>
        internal List<object> ShadowCollection
        {
            get { return _shadowCollection; }
            set { _shadowCollection = value; }
        }

        // why not protected? -> Need to rethink extensibility of ListCollView
        // Adjust the ShadowCopy so that it accurately reflects the state of the
        // Data Collection immediately after the CollectionChangeEvent
        internal void AdjustShadowCopy(NotifyCollectionChangedEventArgs e)
        {
            int tempIndex;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex > _unknownIndex)
                    {
                        ShadowCollection.Insert(e.NewStartingIndex, e.NewItems[0]);
                    }
                    else
                    {
                        ShadowCollection.Add(e.NewItems[0]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex > _unknownIndex)
                    {
                        ShadowCollection.RemoveAt(e.OldStartingIndex);
                    }
                    else
                    {
                        ShadowCollection.Remove(e.OldItems[0]);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex > _unknownIndex)
                    {
                        ShadowCollection[e.OldStartingIndex] = e.NewItems[0];
                    }
                    else
                    {
                        // allow the ShadowCollection to throw the IndexOutOfRangeException
                        // if the item is not found.
                        tempIndex = ShadowCollection.IndexOf(e.OldItems[0]);
                        ShadowCollection[tempIndex] = e.NewItems[0];
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    tempIndex = e.OldStartingIndex;
                    if (tempIndex < 0)
                    {
                        tempIndex = ShadowCollection.IndexOf(e.NewItems[0]);
                    }
                    ShadowCollection.RemoveAt(tempIndex);
                    ShadowCollection.Insert(e.NewStartingIndex, e.NewItems[0]);
                    break;

                default:
                    throw new NotSupportedException(string.Format(Strings.UnexpectedCollectionChangeAction, e.Action));
            }
        }

        // returns true if this ListCollectionView has sort descriptions,
        // without tripping off lazy creation of .SortDescriptions collection
        internal bool HasSortDescriptions
        {
            get { return ((_sort != null) && (_sort.Count > 0)); }
        }

        // return an appropriate comparer.   Common logic used by ListCollectionView
        // and by CollectionViewGroupInternal.
        internal static IComparer PrepareComparer(IComparer customSort, SortDescriptionCollection sort, Func<object, CollectionView> lazyGetCollectionView, object state)
        {
            if (customSort != null)
            {
                return customSort;
            }

            if (sort != null && sort.Count > 0)
            {
                CollectionView view = lazyGetCollectionView(state);
                Debug.Assert(view != null, "lazyGetCollectionView should not return null");

#if WPF
                if (view.SourceCollection != null)
                {
                    IComparer xmlComparer = SystemXmlHelper.PrepareXmlComparer(view.SourceCollection, sort, view.Culture);
                    if (xmlComparer != null)
                    {
                        return xmlComparer;
                    }
                }
#endif

                return new SortFieldComparer(sort, view.Culture);
            }

            return null;
        }

        // true if CurrentPosition points to item within view
        private bool IsCurrentInView
        {
            get { return (0 <= CurrentPosition && CurrentPosition < InternalCount); }
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

        private IList SourceList
        {
            get { return SourceCollection as IList; }
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
        /// Create, filter and sort the local index array.
        /// called from Refresh(), override in derived classes as needed.
        /// </summary>
        private void PrepareLocalArray()
        {
            PrepareShaping();

            IList list = AllowsCrossThreadChanges ? ShadowCollection : (SourceCollection as IList);

            if (!UsesLocalArray)
            {
                // if there's no sort/filter, just use the collection's array
                _internalList = list;
            }
            else
            {
                // otherwise use a private copy - either a simple list or a LiveShapingList
                int size = list.Count;
                var localList = new List<object>(size);

                // filter the collection's array into the local array
                for (int k = 0; k < size; ++k)
                {
                    if (IsAddingNew && k == _newItemIndex)
                        continue;       // the AddNew item is held separately

                    object item = list[k];
                    if (ActiveFilter == null || ActiveFilter(item))
                    {
                        localList.Add(item);
                    }
                }

                // sort the local array
                if (ActiveComparer != null)
                {
                    localList.Sort(ActiveComparer);
                }

                _internalList = localList;
            }

            PrepareGroups();
        }

        private void MoveCurrencyOffDeletedElement(int oldCurrentPosition)
        {
            int lastPosition = InternalCount - 1;   // OK if last is -1
            // if position falls beyond last position, move back to last position
            int newPosition = (oldCurrentPosition < lastPosition) ? oldCurrentPosition : lastPosition;

            // reset this to false before raising events to avoid problems in re-entrancy
            _currentElementWasRemoved = false;

            OnCurrentChanging();

            if (newPosition < 0)
                SetCurrent(null, newPosition);
            else
                SetCurrent(InternalItemAt(newPosition), newPosition);

            OnCurrentChanged();
        }

        // Convert the collection's index to an index into the view.
        // Return -1 if the index is unknown or moot (Reset events).
        // Return -2 if the event doesn't apply to this view.
        private int AdjustBefore(NotifyCollectionChangedAction action, object item, int index)
        {
            // index is not relevant to Reset events
            if (action == NotifyCollectionChangedAction.Reset)
                return -1;

            if (item == NewItemPlaceholder)
            {
                return (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
                        ? 0 : InternalCount - 1;
            }
            else if (IsAddingNew && NewItemPlaceholderPosition != NewItemPlaceholderPosition.None &&
                        ItemsControl.EqualsEx(item, _newItem))
            {
                // we should only get here when removing the AddNew item - i.e. from CancelNew -
                // and only when the placeholder is active.
                // In that case the item's index in the view is 1 when the placeholder
                // is AtBeginning, and just before the placeholder when it's AtEnd.
                // The numerical value for the latter case dependds on whether there's
                // a sort/filter or not, i.e. whether we're using a local array.  That's
                // because the item has already been removed from the collection, but
                // not from the local array.  (DDB 201860)
                return (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
                        ? 1 : UsesLocalArray ? InternalCount - 2 : index;
            }

            int delta = IsGrouping ? 0 :
                        (NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
                        ? (IsAddingNew ? 2 : 1) : 0;
            IList ilFull = (AllowsCrossThreadChanges ? ShadowCollection : SourceCollection) as IList;

            // validate input
            if (index < -1 || index > ilFull.Count)
                throw new InvalidOperationException(string.Format(Strings.CollectionChangeIndexOutOfRange, index, ilFull.Count));

            if (action == NotifyCollectionChangedAction.Add)
            {
                if (index >= 0)
                {
                    if (!ItemsControl.EqualsEx(item, ilFull[index]))
                        throw new InvalidOperationException(string.Format(Strings.AddedItemNotAtIndex, index));
                }
                else
                {
                    // event didn't specify index - determine it the hard way
                    index = ilFull.IndexOf(item);
                    if (index < 0)
                        throw new InvalidOperationException(Strings.AddedItemNotInCollection);
                }
            }

            // if there's no sort or filter, use the index into the full array
            if (!UsesLocalArray)
            {
                if (IsAddingNew)
                {
                    if (NewItemPlaceholderPosition != NewItemPlaceholderPosition.None &&
                        index > _newItemIndex)
                    {
                        --index;        // the new item has been artificially moved elsewhere
                    }
                }

                return (index < 0) ? index : index + delta;
            }

            if (action == NotifyCollectionChangedAction.Add)
            {
                // if the item isn't in the filter, return -2
                if (!this.PassesFilter(item))
                    return -2;

                // search the local array
                if (!UsesLocalArray)
                {
                    index = -1;
                }
                else if (ActiveComparer != null)
                {
                    // if there's a sort order, use binary search
                    index = InternalList.Search(item, ActiveComparer);
                    if (index < 0)
                        index = ~index;
                }
                else
                {
                    // otherwise, do a linear search
                    index = MatchingSearch(item, index, ilFull, InternalList);
                }
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                if (!IsAddingNew || item != _newItem)
                {
                    // a deleted item should already be in the local array
                    index = InternalList.IndexOf(item);

                    // but may not be, if it was already filtered out (can't use
                    // PassesFilter here, because the item could have changed
                    // while it was out of our sight)
                    if (index < 0)
                        return -2;
                }
                else
                {
                    // the new item is in a special position
                    switch (NewItemPlaceholderPosition)
                    {
                        case NewItemPlaceholderPosition.None:
                            return InternalCount - 1;
                        case NewItemPlaceholderPosition.AtBeginning:
                            return 1;
                        case NewItemPlaceholderPosition.AtEnd:
                            return InternalCount - 2;
                    }
                }
            }
            else
            {
                index = -1;
            }

            return (index < 0) ? index : index + delta;
        }

        int MatchingSearch(object item, int index, IList ilFull, IList ilPartial)
        {
            // do a linear search of the full array, advancing
            // localIndex past elements that appear in the local array,
            // until either (a) reaching the position of the item in the
            // full array, or (b) falling off the end of the local array.
            // localIndex is now the desired index.
            // One small wrinkle:  we have to ignore the target item in
            // the local array (this arises in a Move event).
            int fullIndex = 0, localIndex = 0;

            while (fullIndex < index && localIndex < InternalList.Count)
            {
                if (ItemsControl.EqualsEx(ilFull[fullIndex], ilPartial[localIndex]))
                {
                    // match - current item passes filter.  Skip it.
                    ++fullIndex;
                    ++localIndex;
                }
                else if (ItemsControl.EqualsEx(item, ilPartial[localIndex]))
                {
                    // skip over an unmatched copy of the target item
                    // (this arises in a Move event)
                    ++localIndex;
                }
                else
                {
                    // no match - current item fails filter.  Ignore it.
                    ++fullIndex;
                }
            }

            return localIndex;
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
        private void AdjustCurrencyForRemove(int index)
        {
            // adjust current index if deletion is earlier
            if (index < CurrentPosition)
            {
                SetCurrent(CurrentItem, CurrentPosition - 1);
            }
            // remember to move currency off the deleted element
            else if (index == CurrentPosition)
            {
                _currentElementWasRemoved = true;
            }
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
        private void AdjustCurrencyForReplace(int index)
        {
            // remember to move currency off the deleted element
            if (index == CurrentPosition)
            {
                _currentElementWasRemoved = true;
            }
        }

        // build the shaping information from the relevant properties
        private void PrepareShaping()
        {
            // sort:  prepare the comparer
            ActiveComparer = PrepareComparer(_customSort, _sort, static state => (ListCollectionView)state, this);

            // filter:  prepare the Predicate<object> filter
            ActiveFilter = Filter;

            // group : prepare the group descriptions

            // discard old groups
            _group.Clear();

            // initialize the synthetic top level group
            _group.Initialize();

            _isGrouping = (_group.GroupBy != null);
        }

        // set new SortDescription collection; rehook collection change notification handler
        private void SetSortDescriptions(SortDescriptionCollection descriptions)
        {
            if (_sort != null)
            {
                ((INotifyCollectionChanged)_sort).CollectionChanged -= new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
            }

            _sort = descriptions;

            if (_sort != null)
            {
                Debug.Assert(_sort.Count == 0, "must be empty SortDescription collection");
                ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptionsChanged);
            }
        }

        // SortDescription was added/removed, refresh CollectionView
        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsAddingNew || IsEditingItem)
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, "Sorting"));

            // adding to SortDescriptions overrides custom sort
            if (_sort.Count > 0)
            {
                _customSort = null;
            }

            RefreshOrDefer();
        }

        // divide the data items into groups
        void PrepareGroups()
        {
            // if there's no grouping, there's nothing to do
            if (!_isGrouping)
                return;

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
                    ilc.ResetList(InternalList);
                }
                else
                {
                    _group.ActiveComparer = new CollectionViewGroupInternal.IListComparer(InternalList);
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

            for (int k = 0, n = InternalList.Count; k < n; ++k)
            {
                object item = InternalList[k];

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

        // An item has moved.  Update the groups
        void MoveItemWithinGroups(object item, int oldIndex, int newIndex)
        {
            _group.MoveWithinSubgroups(item, InternalList, oldIndex, newIndex);
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

        private IList _internalList;
        private readonly CollectionViewGroupRoot _group;
        private bool _isGrouping;
        private IComparer _activeComparer;
        private Predicate<object> _activeFilter;
        private SortDescriptionCollection _sort;
        private IComparer _customSort;
        private List<object> _shadowCollection;
        private bool _currentElementWasRemoved;  // true if we need to MoveCurrencyOffDeletedElement
        private object _newItem = NoNewItem;
        private object _editItem;
        private int _newItemIndex;  // position _newItem in the source collection
        private NewItemPlaceholderPosition _newItemPlaceholderPosition;
        private bool _isItemConstructorValid;
        private ConstructorInfo _itemConstructor;
        private List<Action> _deferredActions;
        private bool _isRemoving;

        private const int _unknownIndex = -1;
    }

    /// <summary>
    /// Represents a method that is used to provide custom logic to select the <see cref="GroupDescription"/>
    /// based on the parent group and its level.
    /// </summary>
    /// <param name="group">
    /// The parent group.
    /// </param>
    /// <param name="level">
    /// The level of group.
    /// </param>
    /// <returns>
    /// The <see cref="GroupDescription"/> chosen based on the parent group and its level.
    /// </returns>
    public delegate GroupDescription GroupDescriptionSelectorCallback(CollectionViewGroup group, int level);
}