// Copyright (C) 2003 by Microsoft Corporation.  All rights reserved.

using CSHTML5.Internal;
using OpenSilver.Internal.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal class CollectionView : ICollectionView, INotifyCollectionChanged, INotifyPropertyChanged
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

#region Constructors

        /// <summary>
        /// Create a view to given collection.
        /// </summary>
        /// <remarks>
        /// Note that this instance of CollectionView is bound to the
        /// UI thread dispatcher of the caller of this constructor.
        /// </remarks>
        /// <param name="collection">underlying collection</param>
        public CollectionView(IEnumerable collection)
            : this(collection, 0)
        {
        }

        internal CollectionView(IEnumerable collection, int moveToFirst)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            SetFlag(CollectionViewFlags.AllowsCrossThreadChanges, false);

            _sourceCollection = collection;

            // forward collection change events from underlying collection to our listeners.
            INotifyCollectionChanged incc = collection as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                SetFlag(CollectionViewFlags.IsDynamic, true);
            }

            // set currency to the first item if available
            object currentItem = null;
            int currentPosition = -1;
            if (moveToFirst >= 0)
            {
                IEnumerator e = collection.GetEnumerator();
                if (e.MoveNext())
                {
                    currentItem = e.Current;
                    currentPosition = 0;
                }

                IDisposable d = e as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
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

#endregion Constructors

#region Public Interfaces

#region ICollectionView

        /// <summary>
        /// Culture to use during sorting.
        /// </summary>
        public virtual CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_culture != value)
                {
                    _culture = value;
                    OnPropertyChanged(CulturePropertyName);
                }
            }
        }

        /// <summary>
        /// Returns the underlying collection.
        /// </summary>
        public virtual IEnumerable SourceCollection
        {
            get { return _sourceCollection; }
        }

        /// <summary>
        /// Filter is a callback set by the consumer of the ICollectionView
        /// and used by the implementation of the ICollectionView to determine if an
        /// item is suitable for inclusion in the view.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support filtering and will throw a NotSupportedException.
        /// Use <seealso cref="CanFilter"/> property to test if filtering is supported before
        /// assigning a non-null value.
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
        /// Indicates whether or not this ICollectionView can do any filtering.
        /// When false, set <seealso cref="Filter"/> will throw an exception.
        /// </summary>
        public virtual bool CanFilter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Collection of Sort criteria to sort items in this view over the SourceCollection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Simpler implementations do not support sorting and will return an empty
        /// and immutable / read-only SortDescription collection.
        /// Attempting to modify such a collection will cause NotSupportedException.
        /// Use <seealso cref="CanSort"/> property on CollectionView to test if sorting is supported
        /// before modifying the returned collection.
        /// </p>
        /// <p>
        /// One or more sort criteria in form of <seealso cref="SortDescription"/>
        /// can be added, each specifying a property and direction to sort by.
        /// </p>
        /// </remarks>
        public virtual SortDescriptionCollection SortDescriptions
        {
            get { return SortDescriptionCollection.Empty; }
        }

        /// <summary>
        /// Test if this ICollectionView supports sorting before adding
        /// to <seealso cref="SortDescriptions"/>.
        /// </summary>
        public virtual bool CanSort
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if this view really supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public virtual bool CanGroup
        {
            get { return false; }
        }

        /// <summary>
        /// The description of grouping, indexed by level.
        /// </summary>
        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        /// <summary>
        /// The top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public virtual ReadOnlyObservableCollection<object> Groups
        {
            get { return null; }
        }

        /// <summary>
        /// Return the "current item" for this view
        /// </summary>
        /// <remarks>
        /// Only wrapper classes (those that pass currency handling calls to another internal
        /// CollectionView) should override CurrentItem; all other derived classes
        /// should use SetCurrent() to update the current values stored in the base class.
        /// </remarks>
        public virtual object CurrentItem
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentItem;
            }
        }

        /// <summary>
        /// The ordinal position of the <seealso cref="CurrentItem"/> within the (optionally
        /// sorted and filtered) view.
        /// </summary>
        /// <returns>
        /// -1 if the CurrentPosition is unknown, because the collection does not have an
        /// effective notion of indices, or because CurrentPosition is being forcibly changed
        /// due to a CollectionChange.
        /// </returns>
        /// <remarks>
        /// Only wrapper classes (those that pass currency handling calls to another internal
        /// CollectionView) should override CurrenPosition; all other derived classes
        /// should use SetCurrent() to update the current values stored in the base class.
        /// </remarks>
        public virtual int CurrentPosition
        {
            get
            {
                VerifyRefreshNotDeferred();

                return _currentPosition;
            }
        }

        /// <summary>
        /// Return true if <seealso cref="CurrentItem"/> is beyond the end (End-Of-File).
        /// </summary>
        public virtual bool IsCurrentAfterLast
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        /// <summary>
        /// Return true if <seealso cref="CurrentItem"/> is before the beginning (Beginning-Of-File).
        /// </summary>
        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                VerifyRefreshNotDeferred();

                return CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        ///<summary>
        /// Raise this event before changing currency.
        ///</summary>
        public virtual event CurrentChangingEventHandler CurrentChanging;

        ///<summary>
        ///Raise this event after changing currency.
        ///</summary>
        public virtual event EventHandler CurrentChanged;

        /// <summary>
        /// Return true if the item belongs to this view.  No assumptions are
        /// made about the item. This method will behave similarly to IList.Contains().
        /// </summary>
        /// <remarks>
        /// <p>If the caller knows that the item belongs to the
        /// underlying collection, it is more efficient to call PassesFilter.
        /// If the underlying collection is only of type IEnumerable, this method
        /// is a O(N) operation</p>
        /// </remarks>
        public virtual bool Contains(object item)
        {
            VerifyRefreshNotDeferred();

            return (IndexOf(item) >= 0);
        }

        /// <summary>
        /// Enter a Defer Cycle.
        /// Defer cycles are used to coalesce changes to the ICollectionView.
        /// </summary>
        public virtual IDisposable DeferRefresh()
        {
#if false
            if (AllowsCrossThreadChanges)
                VerifyAccess();
#endif

            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
                throw new InvalidOperationException(string.Format("'{0}' is not allowed during an AddNew or EditItem transaction.", "DeferRefresh"));

            ++_deferLevel;
            return new DeferHelper(this);
        }

        /// <summary>
        /// Move <seealso cref="CurrentItem"/> to the given item.
        /// If the item is not found, move to BeforeFirst.
        /// </summary>
        /// <param name="item">Move CurrentItem to this item.</param>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
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
        /// Move <seealso cref="CurrentItem"/> to the first item.
        /// </summary>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
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
        /// Move <seealso cref="CurrentItem"/> to the last item.
        /// </summary>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
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
        /// Move <seealso cref="CurrentItem"/> to the next item.
        /// </summary>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
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
        /// Move <seealso cref="CurrentItem"/> to the item at the given index.
        /// </summary>
        /// <param name="position">Move CurrentItem to this index</param>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
        public virtual bool MoveCurrentToPosition(int position)
        {
            VerifyRefreshNotDeferred();

            if (position < -1 || position > Count)
                throw new ArgumentOutOfRangeException("position");

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
        /// Move <seealso cref="CurrentItem"/> to the previous item.
        /// </summary>
        /// <returns>true if <seealso cref="CurrentItem"/> points to an item within the view.</returns>
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
        /// Re-create the view, using any <seealso cref="SortDescriptions"/> and/or <seealso cref="Filter"/>.
        /// </summary>
        public virtual void Refresh()
        {
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
                throw new InvalidOperationException(string.Format("'{0}' is not allowed during an AddNew or EditItem transaction.", "Refresh"));

            RefreshInternal();
        }

        internal void RefreshInternal()
        {
#if false
            if (AllowsCrossThreadChanges)
                VerifyAccess();
#endif

            RefreshOverride();

            SetFlag(CollectionViewFlags.NeedsRefresh, false);
        }

#endregion ICollectionView

#region IEnumerable

        /// <summary>
        /// Returns an object that enumerates the items in this view.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#endregion IEnumerable

#endregion Public Interfaces

#region Public Methods

        /// <summary>
        /// Return true if the item belongs to this view.  The item is assumed to belong to the
        /// underlying DataCollection;  this method merely takes filters into account.
        /// It is commonly used during collection-changed notifications to determine if the added/removed
        /// item requires processing.
        /// Returns true if no filter is set on collection view.
        /// </summary>
        public virtual bool PassesFilter(object item)
        {
            if (CanFilter && Filter != null)
                return Filter(item);

            return true;
        }

        /// <summary> Return the index where the given item belongs, or -1 if this index is unknown.
        /// </summary>
        /// <remarks>
        /// If this method returns an index other than -1, it must always be true that
        /// view[index-1] &lt; item &lt;= view[index], where the comparisons are done via
        /// the view's IComparer.Compare method (if any).
        /// (This method is used by a listener's (e.g. System.Windows.Controls.ItemsControl)
        /// CollectionChanged event handler to speed up its reaction to insertion and deletion of items.
        /// If IndexOf is  not implemented, a listener does a binary search using IComparer.Compare.)
        /// </remarks>
        /// <param name="item">data item</param>
        public virtual int IndexOf(object item)
        {
            VerifyRefreshNotDeferred();

            return EnumerableWrapper.IndexOf(item);
        }

        /// <summary>
        /// Retrieve item at the given zero-based index in this CollectionView.
        /// </summary>
        /// <remarks>
        /// <p>The index is evaluated with any SortDescriptions or Filter being set on this CollectionView.
        /// If the underlying collection is only of type IEnumerable, this method
        /// is a O(N) operation.</p>
        /// <p>When deriving from CollectionView, override this method to provide
        /// a more efficient implementation.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        public virtual object GetItemAt(int index)
        {
            // only check lower bound because Count could be expensive
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            return EnumerableWrapper[index];
        }

#endregion Public Methods

#region Public Properties

        /// <summary>
        /// Return the number of items (or -1, meaning "don't know");
        /// if a Filter is set, this counts only items that pass the filter.
        /// </summary>
        /// <remarks>
        /// <p>If the underlying collection is only of type IEnumerable, this count
        /// is a O(N) operation; this Count value will be cached until the
        /// collection changes again.</p>
        /// <p>When deriving from CollectionView, override this property to provide
        /// a more efficient implementation.</p>
        /// </remarks>
        public virtual int Count
        {
            get
            {
                VerifyRefreshNotDeferred();

                return EnumerableWrapper.Count;
            }
        }

        /// <summary>
        /// Returns true if the resulting (filtered) view is emtpy.
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return EnumerableWrapper.IsEmpty; }
        }

        /// <summary>
        ///     Returns true if this view needs to be refreshed.
        /// </summary>
        public virtual bool NeedsRefresh
        {
            get { return CheckFlag(CollectionViewFlags.NeedsRefresh); }
        }

        /// <summary>
        ///     Returns true if this view is in use (i.e. if anyone
        ///     is listening to its events).
        /// </summary>
        public virtual bool IsInUse
        {
            get
            {
                return CollectionChanged != null || PropertyChanged != null ||
                       CurrentChanged != null || CurrentChanging != null;
            }
        }

        /// <summary>
        ///     Gets the object that is in the collection to represent a new item.
        /// </summary>
        public static object NewItemPlaceholder
        {
            get { return _newItemPlaceholder; }
        }

#endregion Public Properties

#region Public Events

        /// <summary>
        /// Raise this event when the (filtered) view changes
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

#region IPropertyChange implementation

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
        /// Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged"/>).
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged"/>).
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged;

#endregion IPropertyChange implementation

#endregion Public Events

#region Protected Methods

        /// <summary>
        /// Re-create the view, using any <seealso cref="SortDescriptions"/> and/or <seealso cref="Filter"/>.
        /// </summary>
        protected virtual void RefreshOverride()
        {
            if (SortDescriptions.Count > 0)
                throw new InvalidOperationException(string.Format("If SortDescriptions is overridden in derived classes, then must also override '{0}'.", "Refresh()"));

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
        /// Returns an object that enumerates the items in this view.
        /// </summary>
        protected virtual IEnumerator GetEnumerator()
        {
            VerifyRefreshNotDeferred();

            if (SortDescriptions.Count > 0)
                throw new InvalidOperationException(string.Format("If SortDescriptions is overridden in derived classes, then must also override '{0}'.", "GetEnumerator()"));

            return EnumerableWrapper.GetEnumerator();
        }

        /// <summary>
        ///     Notify listeners that this View has changed
        /// </summary>
        /// <remarks>
        ///     CollectionViews (and sub-classes) should take their filter/sort/grouping
        ///     into account before calling this method to forward CollectionChanged events.
        /// </remarks>
        /// <param name="args">
        ///     The NotifyCollectionChangedEventArgs to be passed to the EventHandler
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            unchecked
            { ++_timestamp; }    // invalidate enumerators because of a change

            if (CollectionChanged != null)
                CollectionChanged(this, args);

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
        /// set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// CollectionViews (and sub-classes) should use this method to update
        /// the Current__ values.
        /// </remarks>
        protected void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : IsEmpty ? 0 : Count;
            SetCurrent(newItem, newPosition, count);
        }

        /// <summary>
        /// set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// This method can be called from a constructor - it does not call
        /// any virtuals.  The 'count' parameter is substitute for the real Count,
        /// used only when newItem is null.
        /// In that case, this method sets IsCurrentAfterLast to true if and only
        /// if newPosition >= count.  This distinguishes between a null belonging
        /// to the view and the dummy null when CurrentPosition is past the end.
        /// </remarks>
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
        /// ask listeners (via <seealso cref="ICollectionView.CurrentChanging"/> event) if it's OK to change currency
        /// </summary>
        /// <returns>false if a listener cancels the change, true otherwise</returns>
        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            OnCurrentChanging(args);
            return (!args.Cancel);
        }

        /// <summary>
        /// Raise a CurrentChanging event that is not cancelable.
        /// Internally, CurrentPosition is set to -1.
        /// This is called by CollectionChanges (Remove and Refresh) that affect the CurrentItem.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This CurrentChanging event cannot be canceled.
        /// </exception>
        protected void OnCurrentChanging()
        {
            _currentPosition = -1;
            OnCurrentChanging(uncancelableCurrentChangingEventArgs);
        }

        /// <summary>
        /// Raises the CurrentChanging event
        /// </summary>
        /// <param name="args">
        ///     CancelEventArgs used by the consumer of the event.  args.Cancel will
        ///     be true after this call if the CurrentItem should not be changed for
        ///     any reason.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     This CurrentChanging event cannot be canceled.
        /// </exception>
        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (_currentChangedMonitor.Busy)
            {
                if (args.IsCancelable)
                    args.Cancel = true;
                return;
            }

            if (CurrentChanging != null)
            {
                CurrentChanging(this, args);
            }
        }

        /// <summary>
        /// Raises the CurrentChanged event
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            if (CurrentChanged != null && _currentChangedMonitor.Enter())
            {
                using (_currentChangedMonitor)
                {
                    CurrentChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Must be implemented by the derived classes to process a single change on the
        ///     UI thread.  The UI thread will have already been entered by now.
        /// </summary>
        /// <param name="args">
        ///     The NotifyCollectionChangedEventArgs to be processed.
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

        ///<summary>
        ///     Handle CollectionChanged events.
        ///
        ///     Calls ProcessCollectionChanged() or
        ///     posts the change to the Dispatcher to process on the correct thread.
        ///</summary>
        /// <remarks>
        ///     User should override <see cref="ProcessCollectionChanged"/>
        /// </remarks>
        /// <param name="sender">
        /// </param>
        /// <param name="args">
        /// </param>
        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (CheckFlag(CollectionViewFlags.ShouldProcessCollectionChanged))
            {
                if (!AllowsCrossThreadChanges)
                {
#if false
                    if (!CheckAccess())
                        throw new NotSupportedException("This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
#endif
                    ProcessCollectionChanged(args);
                }
            }
        }

        /// <summary>
        ///     Refresh, or mark that refresh is needed when defer cycle completes.
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

#endregion Protected Methods

#region Protected Properties

        /// <summary>
        ///     Returns true if this view supports CollectionChanged events raised
        ///     by the source collection on a foreign thread (a thread different
        ///     from the Dispatcher's thread).
        /// </summary>
        /// <notes>
        ///     The value of this property depends on the synchronization information
        ///     registered for the source collection via
        ///     BindingOperations.EnableCollectionSynchronization.
        ///     The value is set when the view is created.
        /// </notes>
        protected bool AllowsCrossThreadChanges
        {
            get { return CheckFlag(CollectionViewFlags.AllowsCrossThreadChanges); }
        }

        /// <summary>
        /// IsRefreshDeferred returns true if there
        /// is still an outstanding DeferRefresh in
        /// use.  If at all possible, derived classes
        /// should not call Refresh if IsRefreshDeferred
        /// is true.
        /// </summary>
        protected bool IsRefreshDeferred
        {
            get
            {
                return _deferLevel > 0;
            }
        }

        /// <summary>
        /// IsCurrentInSync returns true if CurrentItem and CurrentPosition are
        /// up-to-date with the state and content of the collection.
        /// </summary>
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

#endregion Protected Properties

#region Internal Methods

        // helper to validate that we are not in the middle of a DeferRefresh
        // and throw if that is the case.
        internal void VerifyRefreshNotDeferred()
        {
#if false
            if (AllowsCrossThreadChanges)
                VerifyAccess();
#endif

            // If the Refresh is being deferred to change filtering or sorting of the
            // data by this CollectionView, then CollectionView will not reflect the correct
            // state of the underlying data.

            if (IsRefreshDeferred)
                throw new InvalidOperationException("Cannot change or check the contents or Current position of CollectionView while Refresh is being deferred.");
        }

        internal void InvalidateEnumerableWrapper()
        {
#if NETSTANDARD
            IndexedEnumerable wrapper = (IndexedEnumerable)Interlocked.Exchange(ref _enumerableWrapper, null);
#else // BRIDGE
            IndexedEnumerable wrapper = _enumerableWrapper;
            _enumerableWrapper = null;
#endif
            if (wrapper != null)
            {
                wrapper.Invalidate();
            }
        }

#if WPF

#if NETSTANDARD
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
#else
        internal ReadOnlyCollection<ItemPropertyInfo> GetItemProperties()
        {
            IEnumerable collection = SourceCollection;
            if (collection == null)
                return null;

            IEnumerable properties = null;

            Type itemType;
            object item;

            if ((itemType = GetItemType(false)) != null)
            {
                // If we know the item type, use its properties.
                properties = itemType.GetProperties();
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
                    properties = item.GetType().GetProperties();
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
                PropertyInfo pi;

                if ((pi = property as PropertyInfo) != null)
                {
                    list.Add(new ItemPropertyInfo(pi.Name, pi.PropertyType, pi));
                }
            }

            // return the result as a read-only collection
            return new ReadOnlyCollection<ItemPropertyInfo>(list);
        }
#endif

#endif // WPF

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

                        if (type == typeof(Object))
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
            if (d != null)
            {
                d.Dispose();
            }

            return result;
        }

#endregion Internal Methods

#region Internal Properties

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

#endregion Internal Properties

        //------------------------------------------------------
        //
        //  Internal Types
        //
        //------------------------------------------------------

#region Internal Types

        internal sealed class PlaceholderAwareEnumerator : IEnumerator
        {
            enum Position { BeforePlaceholder, OnPlaceholder, OnNewItem, AfterPlaceholder }

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
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

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

            CollectionView _collectionView;
            IEnumerator _baseEnumerator;
            NewItemPlaceholderPosition _placeholderPosition;
            Position _position;
            object _newItem;
            int _timestamp;
        }

#endregion Internal Types

#region Private Properties

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
#if NETSTANDARD
                    Interlocked.CompareExchange(ref _enumerableWrapper, newWrapper, null);
#else // BRIDGE
                    if (_enumerableWrapper == null)
                    {
                        _enumerableWrapper = newWrapper;
                    }
#endif
                }

                return _enumerableWrapper;
            }
        }

#endregion Private Properties

#region Private Methods

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

        private void ValidateCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");
                    if (e.OldStartingIndex < 0)
                        throw new InvalidOperationException("Collection Remove event must specify item position.");
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");
                    if (e.NewStartingIndex < 0)
                        throw new InvalidOperationException("Cannot Move items to an unknown position (-1).");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
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

#endregion Private Methods

#region Private Types

        private class DeferHelper : IDisposable
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

#if NETSTANDARD
                GC.SuppressFinalize(this);
#endif
            }

            private CollectionView _collectionView;
        }


        // this class helps prevent reentrant calls
        private class SimpleMonitor : IDisposable
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
#if NETSTANDARD
                GC.SuppressFinalize(this);
#endif
            }

            public bool Busy { get { return _entered; } }

            bool _entered;
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

#endregion Private Types

#region Private Fields

        IEnumerable _sourceCollection;  // the underlying collection
        CultureInfo _culture;           // culture to use when sorting
        SimpleMonitor _currentChangedMonitor = new SimpleMonitor();
        int _deferLevel;
        IndexedEnumerable _enumerableWrapper;
        Predicate<object> _filter;
        object _currentItem;
        int _currentPosition;
        CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged |
                                     CollectionViewFlags.NeedsRefresh;
        bool _currentElementWasRemovedOrReplaced;
        static object _newItemPlaceholder = new INTERNAL_NamedObject("NewItemPlaceholder");
        object _syncObject = new object();
        int _timestamp;
        static readonly string IEnumerableT = typeof(IEnumerable<>).Name;
        internal static readonly object NoNewItem = new INTERNAL_NamedObject("NoNewItem");

        // since there's nothing in the uncancelable event args that is mutable,
        // just create one instance to be used universally.
        static readonly CurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new CurrentChangingEventArgs(false);

        internal const string CountPropertyName = "Count";
        internal const string IsEmptyPropertyName = "IsEmpty";
        internal const string CulturePropertyName = "Culture";
        internal const string CurrentPositionPropertyName = "CurrentPosition";
        internal const string CurrentItemPropertyName = "CurrentItem";
        internal const string IsCurrentBeforeFirstPropertyName = "IsCurrentBeforeFirst";
        internal const string IsCurrentAfterLastPropertyName = "IsCurrentAfterLast";

#endregion Private Fields
    }
}
