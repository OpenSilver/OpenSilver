// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Base implementation of ICollectionView that enforces
// affinity to the UI thread dispatcher.
//

using OpenSilver.Internal;
using OpenSilver.Internal.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace System.Windows.Data
{
    /// <summary>
    /// Represents a view for grouping, sorting, filtering, and navigating a data collection.
    /// </summary>
    public class CollectionView : DispatcherObject, ICollectionView, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionView"/> class that represents a view of the specified collection.
        /// </summary>
        /// <param name="collection">
        /// The underlying collection.
        /// </param>
        public CollectionView(IEnumerable collection)
            : this(collection, 0)
        {
        }

        internal CollectionView(IEnumerable collection, int moveToFirst)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _engine = DataBindEngine.CurrentDataBindEngine;

            if (!_engine.IsShutDown)
            {
                SynchronizationInfo syncInfo = _engine.ViewManager.GetSynchronizationInfo(collection);
                SetFlag(CollectionViewFlags.AllowsCrossThreadChanges, syncInfo.IsSynchronized);
            }
            else
            {
                // WPF doesn't really support doing anything on a thread whose dispatcher
                // has been shut down.  But for app-compat we should limp along
                // as well as we did in 4.0.  This means avoiding anything that
                // touches the ViewManager.
                moveToFirst = -1;
            }

            _sourceCollection = collection;

            // forward collection change events from underlying collection to our listeners.
            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                // BindingListCollectionView already listens to IBindingList.ListChanged;
                // Don't double-subscribe (bug 452474, 607512)
                IBindingList ibl;
                if (this is not BindingListCollectionView ||
                    ((ibl = collection as IBindingList) != null && !ibl.SupportsChangeNotification))
                {
                    incc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                }
                SetFlag(CollectionViewFlags.IsDynamic, true);
            }

            // set currency to the first item if available
            object currentItem = null;
            int currentPosition = -1;
            if (moveToFirst >= 0)
            {
                BindingOperations.AccessCollection(collection,
                    () =>
                    {
                        IEnumerator e = collection.GetEnumerator();
                        if (e.MoveNext())
                        {
                            currentItem = e.Current;
                            currentPosition = 0;
                        }

                        IDisposable d = e as IDisposable;
                        d?.Dispose();
                    },
                    false);
            }

            _currentItem = currentItem;
            _currentPosition = currentPosition;
            SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, _currentPosition < 0);
            SetFlag(CollectionViewFlags.IsCurrentAfterLast, _currentPosition < 0);
            SetFlag(CollectionViewFlags.CachedIsEmpty, _currentPosition < 0);
        }

        internal CollectionView(IEnumerable collection, bool shouldProcessCollectionChanged)
            : this(collection)
        {
            SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, shouldProcessCollectionChanged);
        }

        /// <summary>
        /// Gets or sets the culture information to use during sorting.
        /// </summary>
        /// <returns>
        /// The culture information to use during sorting.
        /// </returns>
        public virtual CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                if (_culture != value)
                {
                    _culture = value;
                    OnPropertyChanged(CulturePropertyName);
                }
            }
        }

        /// <summary>
        /// Returns the underlying unfiltered collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> object that is the underlying collection.
        /// </returns>
        public virtual IEnumerable SourceCollection
        {
            get { return _sourceCollection; }
        }

        /// <summary>
        /// Gets or sets a method used to determine if an item is suitable for inclusion in the view.
        /// </summary>
        /// <returns>
        /// A delegate that represents the method used to determine if an item is suitable for inclusion in the view.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The current implementation does not support filtering.
        /// </exception>
        public virtual Predicate<object> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (!CanFilter)
                    throw new NotSupportedException();

                _filter = value;

                RefreshOrDefer();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports filtering.
        /// </summary>
        /// <returns>
        /// true if the view supports filtering; otherwise, false. The default is true.
        /// </returns>
        public virtual bool CanFilter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="SortDescription"/> structures that describes how the items in the collection 
        /// are sorted in the view.
        /// </summary>
        /// <returns>
        /// An empty <see cref="SortDescriptionCollection"/> in all cases.
        /// </returns>
        public virtual SortDescriptionCollection SortDescriptions
        {
            get { return SortDescriptionCollection.Empty; }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports sorting.
        /// </summary>
        /// <returns>
        /// false in all cases.
        /// </returns>
        public virtual bool CanSort
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the view supports grouping.
        /// </summary>
        /// <returns>
        /// false in all cases.
        /// </returns>
        public virtual bool CanGroup
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a collection of <see cref="GroupDescription"/> objects that describes how the items in the collection are 
        /// grouped in the view.
        /// </summary>
        /// <returns>
        /// null in all cases.
        /// </returns>
        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a collection of the top-level groups that is constructed based on the <see cref="GroupDescriptions"/> property.
        /// </summary>
        /// <returns>
        /// null in all cases.
        /// </returns>
        public virtual ReadOnlyObservableCollection<object> Groups
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the current item in the view.
        /// </summary>
        /// <returns>
        /// The current item of the view. By default, the first item of the collection starts as the current item.
        /// </returns>
        public virtual object CurrentItem
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentItem;
            }
        }

        /// <summary>
        /// Gets the ordinal position of the <see cref="CurrentItem"/> within the (optionally sorted and filtered) view.
        /// </summary>
        /// <returns>
        /// The ordinal position of the <see cref="CurrentItem"/> within the (optionally sorted and filtered) view.
        /// </returns>
        public virtual int CurrentPosition
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentPosition;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is beyond the end of the collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="CurrentItem"/> of the view is beyond the end of the collection; otherwise, false.
        /// </returns>
        public virtual bool IsCurrentAfterLast
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is before the beginning of the collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="CurrentItem"/> of the view is before the beginning of the collection; otherwise, false.
        /// </returns>
        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> is changing.
        /// </summary>
        public virtual event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Occurs after the <see cref="CurrentItem"/> has changed.
        /// </summary>
        public virtual event EventHandler CurrentChanged;

        /// <summary>
        /// Returns a value that indicates whether the specified item belongs to the view.
        /// </summary>
        /// <param name="item">
        /// The object to check.
        /// </param>
        /// <returns>
        /// true if the item belongs to the view; otherwise, false.
        /// </returns>
        public virtual bool Contains(object item)
        {
            VerifyRefreshNotDeferred();

            return (IndexOf(item) >= 0);
        }

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> object that you can use to dispose of the calling object.
        /// </returns>
        public virtual IDisposable DeferRefresh()
        {
            if (AllowsCrossThreadChanges)
                VerifyAccess();

            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(DeferRefresh)));

            ++_deferLevel;
            return new DeferHelper(this);
        }

        /// <summary>
        /// Sets the specified item to be the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <param name="item">
        /// The item to set as the <see cref="CurrentItem"/>.
        /// </param>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentTo(object item)
        {
            VerifyRefreshNotDeferred();

            // if already on item, or item is the placeholder, don't do anything
            if (ItemsControl.EqualsEx(CurrentItem, item) || ItemsControl.EqualsEx(NewItemPlaceholder, item))
            {
                // also check that we're not fooled by a false null _currentItem
                if (item != null || IsCurrentInView)
                    return IsCurrentInView;
            }

            int index = -1;
            IEditableCollectionView ecv = this as IEditableCollectionView;
            bool isNewItem = (ecv != null && ecv.IsAddingNew && ItemsControl.EqualsEx(item, ecv.CurrentAddItem));

            // Note: Silverlight adds a null check here (probably to avoid
            // NullReferenceException in the PassesFilter method) while WPF
            // doesn't.
            if (isNewItem || item == null || PassesFilter(item))
            {
                // if the item is not found IndexOf() will return -1, and
                // the MoveCurrentToPosition() below will move current to BeforeFirst
                index = IndexOf(item);
            }

            return MoveCurrentToPosition(index);
        }

        /// <summary>
        /// Sets the first item in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentToFirst()
        {
            VerifyRefreshNotDeferred();

            int index = 0;
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
            {
                index = 1;
            }

            return MoveCurrentToPosition(index);
        }

        /// <summary>
        /// Sets the last item in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentToLast()
        {
            VerifyRefreshNotDeferred();

            int index = Count - 1;
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                index -= 1;
            }

            return MoveCurrentToPosition(index);
        }

        /// <summary>
        /// Sets the item after the <see cref="CurrentItem"/> in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentToNext()
        {
            VerifyRefreshNotDeferred();

            int index = CurrentPosition + 1;
            int count = Count;
            IEditableCollectionView ecv = this as IEditableCollectionView;

            if (ecv != null && index == 0 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
            {
                index = 1;
            }
            if (ecv != null && index == count - 1 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                index = count;
            }

            if (index <= count)
            {
                return MoveCurrentToPosition(index);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the item at the specified index to be the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <param name="position">
        /// The index to set the <see cref="CurrentItem"/> to.
        /// </param>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentToPosition(int position)
        {
            VerifyRefreshNotDeferred();

            if (position < -1 || position > Count)
                throw new ArgumentOutOfRangeException(nameof(position));

            // ignore request to move onto the placeholder
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null &&
                    ((position == 0 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ||
                     (position == Count - 1 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)))
            {
                return IsCurrentInView;
            }

            if ((position != CurrentPosition || !IsCurrentInSync)
                && OKToChangeCurrent())
            {
                bool oldIsCurrentAfterLast = IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = IsCurrentBeforeFirst;

                _MoveCurrentToPosition(position);
                OnCurrentChanged();

                if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                    OnPropertyChanged(IsCurrentAfterLastPropertyName);

                if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                    OnPropertyChanged(IsCurrentBeforeFirstPropertyName);

                OnPropertyChanged(CurrentPositionPropertyName);
                OnPropertyChanged(CurrentItemPropertyName);
            }

            return IsCurrentInView;
        }

        /// <summary>
        /// Sets the item before the <see cref="CurrentItem"/> in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="CurrentItem"/> is an item within the view; otherwise, false.
        /// </returns>
        public virtual bool MoveCurrentToPrevious()
        {
            VerifyRefreshNotDeferred();

            int index = CurrentPosition - 1;
            int count = Count;
            IEditableCollectionView ecv = this as IEditableCollectionView;

            if (ecv != null && index == count - 1 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd)
            {
                index = count - 2;
            }
            if (ecv != null && index == 0 && ecv.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
            {
                index = -1;
            }

            if (index >= -1)
            {
                return MoveCurrentToPosition(index);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Re-creates the view.
        /// </summary>
        public virtual void Refresh()
        {
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
                throw new InvalidOperationException(string.Format(Strings.MemberNotAllowedDuringAddOrEdit, nameof(Refresh)));

            RefreshInternal();
        }

        internal void RefreshInternal()
        {
            if (AllowsCrossThreadChanges)
                VerifyAccess();

            RefreshOverride();

            SetFlag(CollectionViewFlags.NeedsRefresh, false);
        }

        /// <summary>
        /// Returns an object that enumerates the items in this view.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
        public virtual bool PassesFilter(object item)
        {
            if (CanFilter && Filter != null)
                return Filter(item);

            return true;
        }

        /// <summary>
        /// Returns the index at which the specified item is located.
        /// </summary>
        /// <param name="item">
        /// The item to locate.
        /// </param>
        /// <returns>
        /// The index at which the specified item is located, or -1 if the item is unknown.
        /// </returns>
        public virtual int IndexOf(object item)
        {
            VerifyRefreshNotDeferred();

            return EnumerableWrapper.IndexOf(item);
        }

        /// <summary>
        /// Retrieves the item at the specified zero-based index in the view.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to retrieve.
        /// </param>
        /// <returns>
        /// The item at the specified zero-based index in the view.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.
        /// </exception>
        public virtual object GetItemAt(int index)
        {
            // only check lower bound because Count could be expensive
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return EnumerableWrapper[index];
        }

        /// <summary>
        /// Removes the reference to the underlying collection from the <see cref="CollectionView"/>.
        /// </summary>
        public virtual void DetachFromSourceCollection()
        {
            INotifyCollectionChanged incc = _sourceCollection as INotifyCollectionChanged;
            if (incc != null)
            {
                IBindingList ibl;
                if (this is not BindingListCollectionView ||
                    ((ibl = _sourceCollection as IBindingList) != null && !ibl.SupportsChangeNotification))
                {
                    incc.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                }
            }

            _sourceCollection = null;
        }

        /// <summary>
        /// Gets the number of records in the view.
        /// </summary>
        /// <returns>
        /// The number of records in the view, or -1 if the number of records is unknown.
        /// </returns>
        public virtual int Count
        {
            get
            {
                VerifyRefreshNotDeferred();

                return EnumerableWrapper.Count;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the resulting (filtered) view is empty.
        /// </summary>
        /// <returns>
        /// true if the resulting view is empty; otherwise, false.
        /// </returns>
        public virtual bool IsEmpty
        {
            get { return EnumerableWrapper.IsEmpty; }
        }

        /// <summary>
        /// Returns an object that you can use to compare items in the view.
        /// </summary>
        /// <returns>
        /// An <see cref="IComparer"/> object that you can use to compare items in the view.
        /// </returns>
        public virtual IComparer Comparer
        {
            get { return this as IComparer; }
        }

        /// <summary>
        /// Gets a value that indicates whether the view needs to be refreshed.
        /// </summary>
        /// <returns>
        /// true if the view needs to be refreshed; otherwise, false.
        /// </returns>
        public virtual bool NeedsRefresh
        {
            get { return CheckFlag(CollectionViewFlags.NeedsRefresh); }
        }

        /// <summary>
        /// Gets a value that indicates whether any object is subscribing to the events of this <see cref="CollectionView"/>.
        /// </summary>
        /// <returns>
        /// true if any object is subscribing to the events of this <see cref="CollectionView"/>; otherwise, false.
        /// </returns>
        public virtual bool IsInUse
        {
            get
            {
                return CollectionChanged != null || PropertyChanged != null ||
                        CurrentChanged != null || CurrentChanging != null;
            }
        }

        /// <summary>
        /// Gets the object that is in the collection to represent a new item.
        /// </summary>
        /// <returns>
        /// The object that is in the collection to represent a new item.
        /// </returns>
        public static object NewItemPlaceholder
        {
            get { return _newItemPlaceholder; }
        }

        /// <summary>
        /// Occurs when the view has changed.
        /// </summary>
        protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// CollectionChanged event (per <see cref="INotifyCollectionChanged" />).
        /// </summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                CollectionChanged += value;
            }
            remove
            {
                CollectionChanged -= value;
            }
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged"/>).
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event using the specified arguments.
        /// </summary>
        /// <param name="e">
        /// Arguments of the event being raised.
        /// </param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Re-creates the view.
        /// </summary>
        protected virtual void RefreshOverride()
        {
            if (SortDescriptions.Count > 0)
                throw new InvalidOperationException(string.Format(Strings.ImplementOtherMembersWithSort, "Refresh()"));

            object oldCurrentItem = _currentItem;
            bool oldIsCurrentAfterLast = CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            bool oldIsCurrentBeforeFirst = CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            int oldCurrentPosition = _currentPosition;

            // force currency off the collection (gives user a chance to save dirty information)
            OnCurrentChanging();

            InvalidateEnumerableWrapper();

            if (IsEmpty || oldIsCurrentBeforeFirst)
            {
                _MoveCurrentToPosition(-1);
            }
            else if (oldIsCurrentAfterLast)
            {
                _MoveCurrentToPosition(Count);
            }
            else if (oldCurrentItem != null) // set currency back to old current item, or first if not found
            {
                int index = EnumerableWrapper.IndexOf(oldCurrentItem);
                if (index < 0)
                {
                    index = 0;
                }
                _MoveCurrentToPosition(index);
            }


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
        /// Returns an object that you can use to enumerate the items in the view.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that you can use to enumerate the items in the view.
        /// </returns>
        protected virtual IEnumerator GetEnumerator()
        {
            VerifyRefreshNotDeferred();

            if (SortDescriptions.Count > 0)
                throw new InvalidOperationException(string.Format(Strings.ImplementOtherMembersWithSort, "GetEnumerator()"));

            return EnumerableWrapper.GetEnumerator();
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object to pass to the event handler.
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(args);

            unchecked { ++_timestamp; }    // invalidate enumerators because of a change

            CollectionChanged?.Invoke(this, args);

            // Collection changes change the count unless an item is being
            // replaced or moved within the collection.
            if (args.Action != NotifyCollectionChangedAction.Replace &&
                args.Action != NotifyCollectionChangedAction.Move)
            {
                OnPropertyChanged(CountPropertyName);
            }

            bool isEmpty = IsEmpty;
            if (isEmpty != CheckFlag(CollectionViewFlags.CachedIsEmpty))
            {
                SetFlag(CollectionViewFlags.CachedIsEmpty, isEmpty);
                OnPropertyChanged(IsEmptyPropertyName);
            }
        }

        /// <summary>
        /// Sets the specified item and index as the values of the <see cref="CurrentItem"/> and <see cref="CurrentPosition"/> properties.
        /// </summary>
        /// <param name="newItem">
        /// The item to set as the <see cref="CurrentItem"/>.
        /// </param>
        /// <param name="newPosition">
        /// The value to set as the <see cref="CurrentPosition"/> property value.
        /// </param>
        protected void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : IsEmpty ? 0 : Count;
            SetCurrent(newItem, newPosition, count);
        }

        /// <summary>
        /// Sets the specified item and index as the values of the <see cref="CurrentItem"/> and <see cref="CurrentPosition"/> 
        /// properties. This method can be called from a constructor of a derived class.
        /// </summary>
        /// <param name="newItem">
        /// The item to set as the <see cref="CurrentItem"/>.
        /// </param>
        /// <param name="newPosition">
        /// The value to set as the <see cref="CurrentPosition"/> property value.
        /// </param>
        /// <param name="count">
        /// The number of items in the <see cref="CollectionView"/>.
        /// </param>
        protected void SetCurrent(object newItem, int newPosition, int count)
        {
            if (newItem != null)
            {
                // non-null item implies position is within range.
                // We ignore count - it's just a placeholder
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, false);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                // empty collection - by convention both flags are true and position is -1
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                // null item, possibly within range.
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }

            _currentItem = newItem;
            _currentPosition = newPosition;
        }

        /// <summary>
        /// Returns a value that indicates whether the view can change which item is the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// false if a listener cancels the change; otherwise, true.
        /// </returns>
        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            OnCurrentChanging(args);
            return (!args.Cancel);
        }

        /// <summary>
        /// Raises a <see cref="CurrentChanging"/> event that is not cancelable.
        /// </summary>
        protected void OnCurrentChanging()
        {
            _currentPosition = -1;
            OnCurrentChanging(uncancelableCurrentChangingEventArgs);
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanging"/> event with the specified arguments.
        /// </summary>
        /// <param name="args">
        /// Information about the event.
        /// </param>
        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(args);

            if (_currentChangedMonitor.Busy)
            {
                if (args.IsCancelable)
                    args.Cancel = true;
                return;
            }

            CurrentChanging?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            EventHandler currentChanged = CurrentChanged;
            if (currentChanged != null && _currentChangedMonitor.Enter())
            {
                using (_currentChangedMonitor)
                {
                    currentChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, processes a single change on the UI thread.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object to process.
        /// </param>
        protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            //
            // Steps for ProcessCollectionChanged:
            //
            // 1) Validate that the values in the args are acceptable.
            // 2) Translate the indices if necessary.
            // 3) Raise CollectionChanged.
            // 4) Adjust Currency.
            // 5) Raise any PropertyChanged events that apply.
            //

            ValidateCollectionChangedEventArgs(args);

            object oldCurrentItem = _currentItem;
            bool oldIsCurrentAfterLast = CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            bool oldIsCurrentBeforeFirst = CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            int oldCurrentPosition = _currentPosition;
            bool raiseChanged = false;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (PassesFilter(args.NewItems[0]))
                    {
                        raiseChanged = true;
                        AdjustCurrencyForAdd(args.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (PassesFilter(args.OldItems[0]))
                    {
                        raiseChanged = true;
                        AdjustCurrencyForRemove(args.OldStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (PassesFilter(args.OldItems[0]) || PassesFilter(args.NewItems[0]))
                    {
                        raiseChanged = true;
                        AdjustCurrencyForReplace(args.OldStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (PassesFilter(args.NewItems[0]))
                    {
                        raiseChanged = true;
                        AdjustCurrencyForMove(args.OldStartingIndex, args.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // collection has completely changed
                    RefreshOrDefer();
                    return;     // Refresh already raises the event
            }

            // we've already returned if (args.Action == NotifyCollectionChangedAction.Reset) above
            if (raiseChanged)
                OnCollectionChanged(args);

            // currency has to change after firing the deletion event,
            // so event handlers have the right picture
            if (_currentElementWasRemovedOrReplaced)
            {
                MoveCurrencyOffDeletedElement();
                _currentElementWasRemovedOrReplaced = false;
            }


            // notify that the properties have changed.
            if (IsCurrentAfterLast != oldIsCurrentAfterLast)
                OnPropertyChanged(IsCurrentAfterLastPropertyName);

            if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                OnPropertyChanged(IsCurrentBeforeFirstPropertyName);

            if (_currentPosition != oldCurrentPosition)
                OnPropertyChanged(CurrentPositionPropertyName);

            if (_currentItem != oldCurrentItem)
                OnPropertyChanged(CurrentItemPropertyName);
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object to pass to the event handler.
        /// </param>
        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (CheckFlag(CollectionViewFlags.ShouldProcessCollectionChanged))
            {
                if (!AllowsCrossThreadChanges)
                {
                    if (!CheckAccess())
                        throw new NotSupportedException(Strings.MultiThreadedCollectionChangeNotSupported);
                    ProcessCollectionChanged(args);
                }
                else
                {
                    PostChange(args);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="AllowsCrossThreadChanges"/> property changes.
        /// </summary>
        protected virtual void OnAllowsCrossThreadChangesChanged()
        {
        }

        /// <summary>
        /// Clears unprocessed changed to the collection.
        /// </summary>
        protected void ClearPendingChanges()
        {
            lock (_changeLogLock)
            {
                _changeLog.Clear();
                _tempChangeLog.Clear();
            }
        }

        /// <summary>
        /// Ensures that all pending changes to the collection have been committed.
        /// </summary>
        protected void ProcessPendingChanges()
        {
            lock (_changeLogLock)
            {
                ProcessChangeLog(_changeLog, true);
                _changeLog.Clear();
            }
        }

        /// <summary>
        /// Called by the base class to notify the derived class that an <see cref="INotifyCollectionChanged.CollectionChanged"/> 
        /// event has been posted to the message queue.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> object that is added to the change log.
        /// </param>
        [Obsolete("Replaced by OnAllowsCrossThreadChangesChanged")]
        protected virtual void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args)
        {
        }

        /// <summary>
        /// Clears any pending changes from the change log.
        /// </summary>
        [Obsolete("Replaced by ClearPendingChanges")]
        protected void ClearChangeLog()
        {
            ClearPendingChanges();
        }

        /// <summary>
        /// Refreshes the view or specifies that the view needs to be refreshed when the defer cycle completes.
        /// </summary>
        protected void RefreshOrDefer()
        {
            if (IsRefreshDeferred)
            {
                SetFlag(CollectionViewFlags.NeedsRefresh, true);
            }
            else
            {
                RefreshInternal();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the underlying collection provides change notifications.
        /// </summary>
        /// <returns>
        /// true if the underlying collection provides change notifications; otherwise, false.
        /// </returns>
        protected bool IsDynamic
        {
            get
            {
                return CheckFlag(CollectionViewFlags.IsDynamic);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a thread other than the one that created the <see cref="CollectionView"/> 
        /// can change the <see cref="SourceCollection"/>.
        /// </summary>
        /// <returns>
        /// true if a thread other than the one that created the <see cref="CollectionView"/> can change the 
        /// <see cref="SourceCollection"/>; otherwise, false.
        /// </returns>
        protected bool AllowsCrossThreadChanges
        {
            get { return CheckFlag(CollectionViewFlags.AllowsCrossThreadChanges); }
        }

        internal void SetAllowsCrossThreadChanges(bool value)
        {
            bool oldValue = CheckFlag(CollectionViewFlags.AllowsCrossThreadChanges);
            if (oldValue == value)
                return;

            SetFlag(CollectionViewFlags.AllowsCrossThreadChanges, value);
            OnAllowsCrossThreadChangesChanged();
        }

        /// <summary>
        /// Gets a value that indicates whether it has been necessary to update the change log because a 
        /// <see cref="CollectionChanged"/> notification has been received on a different thread without 
        /// first entering the user interface (UI) thread dispatcher.
        /// </summary>
        /// <returns>
        /// true if it has been necessary to update the change log because a <see cref="CollectionChanged"/>
        /// notification has been received on a different thread without first entering the user interface 
        /// (UI) thread dispatcher; otherwise, false.
        /// </returns>
        protected bool UpdatedOutsideDispatcher
        {
            get { return AllowsCrossThreadChanges; }
        }

        /// <summary>
        /// Gets a value that indicates whether there is an outstanding <see cref="DeferRefresh"/> in use.
        /// </summary>
        /// <returns>
        /// true if there is an outstanding <see cref="DeferRefresh"/> in use; otherwise, false.
        /// </returns>
        protected bool IsRefreshDeferred
        {
            get
            {
                return _deferLevel > 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> is at the <see cref="CurrentPosition"/>.
        /// </summary>
        /// <returns>
        /// true if the <see cref="CurrentItem"/> is in the view and at the <see cref="CurrentPosition"/>; otherwise, false.
        /// </returns>
        protected bool IsCurrentInSync
        {
            get
            {
                if (IsCurrentInView)
                    return GetItemAt(CurrentPosition) == CurrentItem;
                else
                    return CurrentItem == null;
            }
        }

        /// <summary>
        /// This method is for use by an agent that manages a set of
        /// one or more views.  Normal applications should not use it directly.
        /// </summary>
        /// <remarks>
        /// It is used to control the lifetime of the view, so that it gets
        /// garbage-collected at the right time.
        /// </remarks>
        internal void SetViewManagerData(object value)
        {
            object[] array;

            if (_vmData == null)
            {
                // 90% case - store a single value directly
                _vmData = value;
            }
            else if ((array = _vmData as object[]) == null)
            {
                // BindingListCollectionView appears in the table for both
                // DataTable and DataView - keep both references (bug 1745899)
                _vmData = new object[] { _vmData, value };
            }
            else
            {
                // in case a view is held by more than two tables, keep all
                // references.  This doesn't happen in current code, but there's
                // nothing preventing it, either.
                object[] newArray = new object[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[array.Length] = value;
                _vmData = newArray;
            }
        }

        // helper to validate that we are not in the middle of a DeferRefresh
        // and throw if that is the case.
        internal void VerifyRefreshNotDeferred()
        {
            if (AllowsCrossThreadChanges)
                VerifyAccess();

            // If the Refresh is being deferred to change filtering or sorting of the
            // data by this CollectionView, then CollectionView will not reflect the correct
            // state of the underlying data.

            if (IsRefreshDeferred)
                throw new InvalidOperationException(Strings.NoCheckOrChangeWhenDeferred);
        }

        internal void InvalidateEnumerableWrapper()
        {
            IndexedEnumerable wrapper = (IndexedEnumerable)Interlocked.Exchange(ref _enumerableWrapper, null);
            wrapper?.Invalidate();
        }

        internal ReadOnlyCollection<ItemPropertyInfo> GetItemProperties()
        {
            IEnumerable collection = SourceCollection;
            if (collection == null)
                return null;

            IEnumerable properties = null;

            ITypedList itl = collection as ITypedList;
            Type itemType;
            object item;

            if (itl != null)
            {
                // ITypedList has the information
                properties = itl.GetItemProperties(null);
            }
            else if ((itemType = GetItemType(false)) != null)
            {
                // If we know the item type, use its properties.
                properties = TypeDescriptor.GetProperties(itemType);
            }
            else if ((item = GetRepresentativeItem()) != null)
            {
                // If we have a representative item, use its properties.
                // It's cheaper to use the item type, but we cannot do that
                // when all we know is a representative item.  If the item
                // has synthetic properties (via ICustomTypeDescriptor or
                // TypeDescriptorProvider), they don't show up on the type -
                // only on the item.
                ICustomTypeProvider ictp = item as ICustomTypeProvider;
                if (ictp == null)
                {
                    properties = TypeDescriptor.GetProperties(item);
                }
                else
                {
                    properties = ictp.GetCustomType().GetProperties();
                }
            }

            if (properties == null)
                return null;

            // convert the properties to ItemPropertyInfo
            List<ItemPropertyInfo> list = new List<ItemPropertyInfo>();
            foreach (object property in properties)
            {
                PropertyDescriptor pd;
                PropertyInfo pi;

                if ((pd = property as PropertyDescriptor) != null)
                {
                    list.Add(new ItemPropertyInfo(pd.Name, pd.PropertyType, pd));
                }
                else if ((pi = property as PropertyInfo) != null)
                {
                    list.Add(new ItemPropertyInfo(pi.Name, pi.PropertyType, pi));
                }
            }

            // return the result as a read-only collection
            return new ReadOnlyCollection<ItemPropertyInfo>(list);
        }

        internal Type GetItemType(bool useRepresentativeItem)
        {
            Type collectionType = SourceCollection.GetType();
            Type[] interfaces = collectionType.GetInterfaces();

            // Look for IEnumerable<T>.  All generic collections should implement
            // this.  We loop through the interface list, rather than call
            // GetInterface(IEnumerableT), so that we handle an ambiguous match
            // (by using the first match) without an exception.
            for (int i = 0; i < interfaces.Length; ++i)
            {
                Type interfaceType = interfaces[i];

                if (interfaceType.Name == IEnumerableT)
                {
                    // found IEnumerable<>, extract T
                    Type[] typeParameters = interfaceType.GetGenericArguments();
                    if (typeParameters.Length == 1)
                    {
                        Type type = typeParameters[0];

                        if (typeof(ICustomTypeProvider).IsAssignableFrom(type))
                        {
                            // if the item type can point to a custom type
                            // for reflection, we need the custom type.
                            // We can only get it from a representative item.
                            break;
                        }

                        if (type == typeof(object))
                        {
                            // IEnumerable<Object> is useless;  we need a representative
                            // item.   But keep going - perhaps IEnumerable<T> shows up later.
                            continue;
                        }

                        return type;
                    }
                }
            }

            // No generic information found.  Use a representative item instead.
            if (useRepresentativeItem)
            {
                // get type of a representative item
                object item = GetRepresentativeItem();
                return GetReflectionType(item);
            }

            return null;
        }

        private static Type GetReflectionType(object item)
        {
            if (item == null)
                return null;

            ICustomTypeProvider ictp = item as ICustomTypeProvider;
            if (ictp == null)
                return item.GetType();
            else
                return ictp.GetCustomType();
        }

        internal object GetRepresentativeItem()
        {
            if (IsEmpty)
                return null;

            object result = null;
            IEnumerator ie = this.GetEnumerator();
            while (ie.MoveNext())
            {
                object item = ie.Current;
                if (item != null && item != NewItemPlaceholder)
                {
                    result = item;
                    break;
                }
            }

            IDisposable d = ie as IDisposable;
            d?.Dispose();

            return result;
        }

        internal virtual void GetCollectionChangedSources(int level, Action<int, object, bool?, List<string>> format, List<string> sources)
        {
            format(level, this, null, sources);
            if (_sourceCollection != null)
            {
                format(level + 1, _sourceCollection, null, sources);
            }
        }

        internal object SyncRoot
        {
            get { return _syncObject; }
        }

        // Timestamp is used by the PlaceholderAwareEnumerator to determine if a
        // collection change has occurred since the enumerator began.  (If so,
        // MoveNext should throw.)
        internal int Timestamp
        {
            get { return _timestamp; }
        }

        internal sealed class PlaceholderAwareEnumerator : IEnumerator
        {
            private enum Position { BeforePlaceholder, OnPlaceholder, OnNewItem, AfterPlaceholder }

            public PlaceholderAwareEnumerator(CollectionView collectionView, IEnumerator baseEnumerator, NewItemPlaceholderPosition placeholderPosition, object newItem)
            {
                _collectionView = collectionView;
                _timestamp = collectionView.Timestamp;
                _baseEnumerator = baseEnumerator;
                _placeholderPosition = placeholderPosition;
                _newItem = newItem;
            }

            public bool MoveNext()
            {
                if (_timestamp != _collectionView.Timestamp)
                    throw new InvalidOperationException(Strings.EnumeratorVersionChanged);

                switch (_position)
                {
                    case Position.BeforePlaceholder:
                        // AtBeginning - move to the placeholder
                        if (_placeholderPosition == NewItemPlaceholderPosition.AtBeginning)
                        {
                            _position = Position.OnPlaceholder;
                        }
                        // None or AtEnd - advance base, skipping the new item
                        else if (_baseEnumerator.MoveNext() &&
                                    (_newItem == NoNewItem || _baseEnumerator.Current != _newItem
                                            || _baseEnumerator.MoveNext()))
                        {
                        }
                        // if base has reached the end, move to new item or placeholder
                        else if (_newItem != NoNewItem)
                        {
                            _position = Position.OnNewItem;
                        }
                        else if (_placeholderPosition == NewItemPlaceholderPosition.None)
                        {
                            return false;
                        }
                        else
                        {
                            _position = Position.OnPlaceholder;
                        }
                        return true;

                    case Position.OnPlaceholder:
                        // AtBeginning - move from placeholder to new item (if present)
                        if (_newItem != NoNewItem && _placeholderPosition == NewItemPlaceholderPosition.AtBeginning)
                        {
                            _position = Position.OnNewItem;
                            return true;
                        }
                        break;

                    case Position.OnNewItem:
                        // AtEnd - move from new item to placeholder
                        if (_placeholderPosition == NewItemPlaceholderPosition.AtEnd)
                        {
                            _position = Position.OnPlaceholder;
                            return true;
                        }
                        break;
                }

                // in all other cases, simply advance base, skipping the new item
                _position = Position.AfterPlaceholder;
                return (_baseEnumerator.MoveNext() &&
                            (_newItem == NoNewItem || _baseEnumerator.Current != _newItem
                                            || _baseEnumerator.MoveNext()));
            }

            public object Current
            {
                get
                {
                    return (_position == Position.OnPlaceholder) ? CollectionView.NewItemPlaceholder
                        : (_position == Position.OnNewItem) ? _newItem
                        : _baseEnumerator.Current;
                }
            }

            public void Reset()
            {
                _position = Position.BeforePlaceholder;
                _baseEnumerator.Reset();
            }

            private readonly CollectionView _collectionView;
            private readonly IEnumerator _baseEnumerator;
            private readonly NewItemPlaceholderPosition _placeholderPosition;
            private Position _position;
            private readonly object _newItem;
            private readonly int _timestamp;
        }

        private bool IsCurrentInView
        {
            get
            {
                VerifyRefreshNotDeferred();
                return (0 <= CurrentPosition && CurrentPosition < Count);
            }
        }

        private IndexedEnumerable EnumerableWrapper
        {
            get
            {
                if (_enumerableWrapper == null)
                {
                    IndexedEnumerable newWrapper = new IndexedEnumerable(SourceCollection, new Predicate<object>(this.PassesFilter));
                    Interlocked.CompareExchange(ref _enumerableWrapper, newWrapper, null);
                }

                return _enumerableWrapper;
            }
        }

        // Just move it.  No argument check, no events, just move current to position.
        private void _MoveCurrentToPosition(int position)
        {
            if (position < 0)
            {
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                SetCurrent(null, -1);
            }
            else if (position >= Count)
            {
                SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                SetCurrent(null, Count);
            }
            else
            {
                SetFlag(CollectionViewFlags.IsCurrentBeforeFirst | CollectionViewFlags.IsCurrentAfterLast, false);
                SetCurrent(EnumerableWrapper[position], position);
            }
        }

        private void MoveCurrencyOffDeletedElement()
        {
            int lastPosition = Count - 1;
            // if position falls beyond last position, move back to last position
            int newPosition = (_currentPosition < lastPosition) ? _currentPosition : lastPosition;

            // ignore cancel, there's no choice in this currency change
            OnCurrentChanging();
            _MoveCurrentToPosition(newPosition);
            OnCurrentChanged();
        }

        private void EndDefer()
        {
            --_deferLevel;

            if (_deferLevel == 0 && CheckFlag(CollectionViewFlags.NeedsRefresh))
            {
                Refresh();
            }
        }

        /// <summary>
        ///     DeferProcessing is to be called from OnCollectionChanged by derived classes  that
        ///     wish to process the remainder of a changeLog after allowing other events to be
        ///     processed.
        /// </summary>
        /// <param name="changeLog">
        ///     List of NotifyCollectionChangedEventArgs that could not be precessed.
        /// </param>
        private void DeferProcessing(List<NotifyCollectionChangedEventArgs> changeLog)
        {
            Debug.Assert(changeLog != null && changeLog.Count > 0, "don't defer when there's no work");

            lock (SyncRoot)
            {
                lock (_changeLogLock)
                {
                    _changeLog.InsertRange(0, changeLog);

                    if (_databindOperation != null)
                    {
                        _engine.ChangeCost(_databindOperation, changeLog.Count);
                    }
                    else
                    {
                        _databindOperation = _engine.Marshal(new DispatcherOperationCallback(ProcessInvoke), null, changeLog.Count);
                    }
                }
            }
        }

        /// <summary>
        ///     Must be implemented by the derived classes to process changes on the
        ///     UI thread.  Called by ProcessInvoke wich is called by the Dispatcher, so
        ///     the UI thread will have allready been entered by now.
        /// </summary>
        /// <param name="changeLog">
        ///     List of NotifyCollectionChangedEventArgs that is to be processed.
        /// </param>
        /// <param name="processAll"></param>
        private List<NotifyCollectionChangedEventArgs> ProcessChangeLog(List<NotifyCollectionChangedEventArgs> changeLog, bool processAll = false)
        {
            int currentIndex = 0;
            bool mustDeferProcessing = false;
            long beginTime = DateTime.Now.Ticks;

            for (; currentIndex < changeLog.Count && !mustDeferProcessing; currentIndex++)
            {
                ProcessCollectionChanged(changeLog[currentIndex]);

                if (!processAll)
                {
                    mustDeferProcessing = DateTime.Now.Ticks - beginTime > DataBindEngine.CrossThreadThreshold;
                }
            }

            if (mustDeferProcessing && currentIndex < changeLog.Count)
            {
                // create an unprocessed subset of changeLog
                changeLog.RemoveRange(0, currentIndex);
                return changeLog;
            }

            return null;
        }

        // returns true if ANY flag in flags is set.
        private bool CheckFlag(CollectionViewFlags flags)
        {
            return (_flags & flags) != 0;
        }

        private void SetFlag(CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                _flags = _flags | flags;
            }
            else
            {
                _flags = _flags & ~flags;
            }
        }

        // Post a change on the UI thread Dispatcher and updated the _changeLog.
        private void PostChange(NotifyCollectionChangedEventArgs args)
        {
            lock (SyncRoot)
            {
                lock (_changeLogLock)
                {
                    // we can ignore everything before a Reset
                    if (args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        _changeLog.Clear();
                    }

                    if (_changeLog.Count == 0 && CheckAccess())
                    {
                        // when a change arrives on the UI thread and there are
                        // no pending cross-thread changes, process the event
                        // synchronously.   This is important for editing operations
                        // (AddNew, Remove), which expect to get notified about
                        // the changes they make directly.
                        ProcessCollectionChanged(args);
                    }
                    else
                    {
                        // the change (or another pending change) arrived on the
                        // wrong thread.  Marshal it to the UI thread.
                        _changeLog.Add(args);

                        if (_databindOperation == null)
                        {
                            _databindOperation = _engine.Marshal(
                                new DispatcherOperationCallback(ProcessInvoke),
                                null, _changeLog.Count);
                        }
                    }
                }
            }
        }

        // Callback that is passed to Dispatcher.BeginInvoke in PostChange
        private object ProcessInvoke(object arg)
        {
            // work on a private copy of the change log, so that other threads
            // can add to the main change log
            lock (SyncRoot)
            {
                lock (_changeLogLock)
                {
                    _databindOperation = null;
                    _tempChangeLog = _changeLog;
                    _changeLog = new List<NotifyCollectionChangedEventArgs>();
                }
            }

            // process the changes
            List<NotifyCollectionChangedEventArgs> unprocessedChanges = ProcessChangeLog(_tempChangeLog);

            // if changes remain (because we ran out of time), reschedule them
            if (unprocessedChanges != null && unprocessedChanges.Count > 0)
            {
                DeferProcessing(unprocessedChanges);
            }

            _tempChangeLog = s_emptyList;

            return null;
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
                    if (e.OldStartingIndex < 0)
                        throw new InvalidOperationException(Strings.RemovedItemNotFound);
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

        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForAdd(int index)
        {
            // adjust current index if insertion is earlier
            if (Count == 1)
                _currentPosition = -1;
            else if (index <= _currentPosition)
            {
                ++_currentPosition;

                if (_currentPosition < Count)
                {
                    _currentItem = EnumerableWrapper[_currentPosition];
                }
            }
        }

        // Fix up CurrentPosition and CurrentItem after an item was removed.
        private void AdjustCurrencyForRemove(int index)
        {
            // adjust current index if deletion is earlier
            if (index < _currentPosition)
                --_currentPosition;

            // move currency off the deleted element
            else if (index == _currentPosition)
            {
                _currentElementWasRemovedOrReplaced = true;
            }
        }

        // Fix up CurrentPosition and CurrentItem after an item was moved.
        private void AdjustCurrencyForMove(int oldIndex, int newIndex)
        {
            // if entire move was before or after current item, then there
            // is nothing that needs to be done.
            if ((oldIndex < CurrentPosition && newIndex < CurrentPosition)
                || (oldIndex > CurrentPosition && newIndex > CurrentPosition))
                return;

            if (oldIndex <= CurrentPosition)
                AdjustCurrencyForRemove(oldIndex);
            else if (newIndex <= CurrentPosition)
                AdjustCurrencyForAdd(newIndex);
        }


        // fix up CurrentPosition and CurrentItem after a collection change
        private void AdjustCurrencyForReplace(int index)
        {
            // CurrentItem was replaced
            if (index == _currentPosition)
            {
                _currentElementWasRemovedOrReplaced = true;
            }
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private sealed class DeferHelper : IDisposable
        {
            public DeferHelper(CollectionView collectionView)
            {
                _collectionView = collectionView;
            }

            public void Dispose()
            {
                if (_collectionView != null)
                {
                    _collectionView.EndDefer();
                    _collectionView = null;
                }

                GC.SuppressFinalize(this);
            }

            private CollectionView _collectionView;
        }


        // this class helps prevent reentrant calls
        private sealed class SimpleMonitor : IDisposable
        {
            public bool Enter()
            {
                if (_entered)
                    return false;

                _entered = true;
                return true;
            }

            public void Dispose()
            {
                _entered = false;
                GC.SuppressFinalize(this);
            }

            public bool Busy { get { return _entered; } }

            private bool _entered;
        }

        [Flags]
        private enum CollectionViewFlags
        {
            UpdatedOutsideDispatcher = 0x2,
            ShouldProcessCollectionChanged = 0x4,
            IsCurrentBeforeFirst = 0x8,
            IsCurrentAfterLast = 0x10,
            IsDynamic = 0x20,
            IsDataInGroupOrder = 0x40,
            NeedsRefresh = 0x80,
            AllowsCrossThreadChanges = 0x100,
            CachedIsEmpty = 0x200,
        }

        private readonly object _changeLogLock = new();

        private List<NotifyCollectionChangedEventArgs> _changeLog = new();
        private List<NotifyCollectionChangedEventArgs> _tempChangeLog = s_emptyList;

        private DataBindOperation _databindOperation;
        private object _vmData;            // view manager's private data
        private IEnumerable _sourceCollection;  // the underlying collection
        private CultureInfo _culture;           // culture to use when sorting
        private readonly SimpleMonitor _currentChangedMonitor = new SimpleMonitor();
        private int _deferLevel;
        private IndexedEnumerable _enumerableWrapper;
        private Predicate<object> _filter;
        private object _currentItem;
        private int _currentPosition;
        private CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged | CollectionViewFlags.NeedsRefresh;
        private bool _currentElementWasRemovedOrReplaced;
        private static readonly object _newItemPlaceholder = new NamedObject("NewItemPlaceholder");
        private readonly object _syncObject = new();
        private readonly DataBindEngine _engine;
        private int _timestamp;

        private static readonly List<NotifyCollectionChangedEventArgs> s_emptyList = new();
        private static readonly string IEnumerableT = typeof(IEnumerable<>).Name;
        internal static readonly object NoNewItem = new NamedObject("NoNewItem");

        // since there's nothing in the uncancelable event args that is mutable,
        // just create one instance to be used universally.
        private static readonly CurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new(false);

        internal const string CountPropertyName = "Count";
        internal const string IsEmptyPropertyName = "IsEmpty";
        internal const string CulturePropertyName = "Culture";
        internal const string CurrentPositionPropertyName = "CurrentPosition";
        internal const string CurrentItemPropertyName = "CurrentItem";
        internal const string IsCurrentBeforeFirstPropertyName = "IsCurrentBeforeFirst";
        internal const string IsCurrentAfterLastPropertyName = "IsCurrentAfterLast";
    }
}
