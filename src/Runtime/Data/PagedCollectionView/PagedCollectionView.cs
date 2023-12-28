//-----------------------------------------------------------------------
// <copyright file="PagedCollectionView.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Common;

namespace System.Windows.Data
{
    /// <summary>
    /// PagedCollectionView view over an IEnumerable.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "WPF Compatability")]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "WPF Compatibility")]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "WPF Compatibility for naming")]
    public sealed class PagedCollectionView : ICollectionView, IPagedCollectionView, IEditableCollectionView, INotifyPropertyChanged
    {
        ////------------------------------------------------------
        ////
        ////  Constants
        ////
        ////------------------------------------------------------ 

#region Static Fields and Constants

        /// <summary>
        /// Since there's nothing in the un-cancelable event args that is mutable,
        /// just create one instance to be used universally.
        /// </summary>
        private static readonly CurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new CurrentChangingEventArgs(false);

#endregion Static Fields and Constants

        ////------------------------------------------------------
        ////
        ////  Private Fields
        ////
        ////------------------------------------------------------ 

#region Private Fields

        /// <summary>
        /// Value that we cache for the PageIndex if we are in a DeferRefresh,
        /// and the user has attempted to move to a different page.
        /// </summary>
        private int _cachedPageIndex = -1;

        /// <summary>
        /// Value that we cache for the PageSize if we are in a DeferRefresh,
        /// and the user has attempted to change the PageSize.
        /// </summary>
        private int _cachedPageSize;

        /// <summary>
        /// CultureInfo used in this PagedCollectionView
        /// </summary>
        private CultureInfo _culture;

        /// <summary>
        /// Private accessor for the Monitor we use to prevent recursion
        /// </summary>
        private SimpleMonitor _currentChangedMonitor = new SimpleMonitor();

        /// <summary>
        /// Private accessor for the CurrentItem
        /// </summary>
        private object _currentItem;

        /// <summary>
        /// Private accessor for the CurrentPosition
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// The number of requests to defer Refresh()
        /// </summary>
        private int _deferLevel;

        /// <summary>
        /// The item we are currently editing
        /// </summary>
        private object _editItem;

        /// <summary>
        /// Private accessor for the Filter
        /// </summary>
        private Predicate<object> _filter;

        /// <summary>
        /// Private accessor for the CollectionViewFlags
        /// </summary>
        private CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged;

        /// <summary>
        /// Private accessor for the Grouping data
        /// </summary>
        private CollectionViewGroupRoot _group;

        /// <summary>
        /// Private accessor for the InternalList
        /// </summary>
        private IList _internalList;

        /// <summary>
        /// Keeps track of whether groups have been applied to the
        /// collection already or not. Note that this can still be set
        /// to false even though we specify a GroupDescription, as the 
        /// collection may not have gone through the PrepareGroups function.
        /// </summary>
        private bool _isGrouping;

        /// <summary>
        /// Private accessor for indicating whether we want to point to the temporary grouping data for calculations
        /// </summary>
        private bool _isUsingTemporaryGroup;

        /// <summary>
        /// ConstructorInfo obtained from reflection for generating new items
        /// </summary>
        private ConstructorInfo _itemConstructor;

        /// <summary>
        /// Whether we have the correct ConstructorInfo information for the ItemConstructor
        /// </summary>
        private bool _itemConstructorIsValid;

        /// <summary>
        /// The new item we are getting ready to add to the collection
        /// </summary>
        private object _newItem;

        /// <summary>
        /// Private accessor for the PageIndex
        /// </summary>
        private int _pageIndex = -1;

        /// <summary>
        /// Private accessor for the PageSize
        /// </summary>
        private int _pageSize;

        /// <summary>
        /// Whether the source needs to poll for changes
        /// (if it did not implement INotifyCollectionChanged)
        /// </summary>
        private bool _pollForChanges;

        /// <summary>
        /// Private accessor for the SortDescriptions
        /// </summary>
        private SortDescriptionCollection _sortDescriptions;

        /// <summary>
        /// Private accessor for the SourceCollection
        /// </summary>
        private IEnumerable _sourceCollection;

        /// <summary>
        /// Private accessor for the Grouping data on the entire collection
        /// </summary>
        private CollectionViewGroupRoot _temporaryGroup;

        /// <summary>
        /// Timestamp used to see if there was a collection change while 
        /// processing enumerator changes
        /// </summary>
        private int _timestamp;

        /// <summary>
        /// Private accessor for the TrackingEnumerator
        /// </summary>
        private IEnumerator _trackingEnumerator;

#endregion Private Fields

        ////------------------------------------------------------
        ////
        ////  Constructors
        ////
        ////------------------------------------------------------

#region Constructors

        /// <summary>
        /// Helper constructor that sets default values for isDataSorted and isDataInGroupOrder.
        /// </summary>
        /// <param name="source">The source for the collection</param>
        public PagedCollectionView(IEnumerable source)
            : this(source, false /*isDataSorted*/, false /*isDataInGroupOrder*/)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PagedCollectionView class.
        /// </summary>
        /// <param name="source">The source for the collection</param>
        /// <param name="isDataSorted">Determines whether the source is already sorted</param>
        /// <param name="isDataInGroupOrder">Whether the source is already in the correct order for grouping</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "We want deriving classes to be able to override the refresh operation")]
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Cannot use underscore in name to differentiate")]
        public PagedCollectionView(IEnumerable source, bool isDataSorted, bool isDataInGroupOrder)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this._sourceCollection = source;

            this.SetFlag(CollectionViewFlags.IsDataSorted, isDataSorted);
            this.SetFlag(CollectionViewFlags.IsDataInGroupOrder, isDataInGroupOrder);

            this._temporaryGroup = new CollectionViewGroupRoot(this, isDataInGroupOrder);
            this._group = new CollectionViewGroupRoot(this, false);
            this._group.GroupDescriptionChanged += new EventHandler(this.OnGroupDescriptionChanged);
            this._group.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupByChanged);

            this.CopySourceToInternalList();
            this._trackingEnumerator = source.GetEnumerator();

            // set currency
            if (this._internalList.Count > 0)
            {
                this.SetCurrent(this._internalList[0], 0, 1);
            }
            else
            {
                this.SetCurrent(null, -1, 0);
            }

            // Set flag for whether the collection is empty
            this.SetFlag(CollectionViewFlags.CachedIsEmpty, this.Count == 0);

            // If the source doesn't raise collection change events, try to
            // detect changes by polling the enumerator
            this._pollForChanges = !(source is INotifyCollectionChanged);

            // If we implement INotifyCollectionChanged
            if (!this._pollForChanges)
            {
                (source as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate (object sender, NotifyCollectionChangedEventArgs args)
                { this.ProcessCollectionChanged(args); });
            }
        }

#endregion Constructors

        ////------------------------------------------------------
        ////
        ////  Private Delegates
        ////
        ////------------------------------------------------------

#region Private Delegates

        /// <summary>
        /// Delegate used to asynchronously trigger a page move.
        /// </summary>
        /// <param name="pageIndex">Requested page index</param>
        private delegate void RequestPageMoveDelegate(int pageIndex);

#endregion Private Delegates

        ////------------------------------------------------------
        ////
        ////  Events
        ////
        ////------------------------------------------------------

#region Events

        /// <summary>
        /// Raise this event when the (filtered) view changes
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// CollectionChanged event (per INotifyCollectionChanged).
        /// </summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { this.CollectionChanged += value; }
            remove { this.CollectionChanged -= value; }
        }

        /// <summary>
        /// Raised when the CurrentItem property changed
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Raised when the CurrentItem property is changing
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Raised when a page index change completed
        /// </summary>
        public event EventHandler<EventArgs> PageChanged;

        /// <summary>
        /// Raised when a page index change is requested
        /// </summary>
        public event EventHandler<PageChangingEventArgs> PageChanging;

        /// <summary>
        /// PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged event (per INotifyPropertyChanged)
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

#endregion Events

        ////------------------------------------------------------
        ////
        ////  Enumerators
        ////
        ////------------------------------------------------------

#region Enumerators

        /// <summary>
        /// Enum for CollectionViewFlags
        /// </summary>
        [Flags]
        private enum CollectionViewFlags
        {
            /// <summary>
            /// Whether the list of items (after applying the sort and filters, if any) 
            /// is already in the correct order for grouping. 
            /// </summary>
            IsDataInGroupOrder = 0x01,

            /// <summary>
            /// Whether the source collection is already sorted according to the SortDescriptions collection
            /// </summary>
            IsDataSorted = 0x02,

            /// <summary>
            /// Whether we should process the collection changed event
            /// </summary>
            ShouldProcessCollectionChanged = 0x04,

            /// <summary>
            /// Whether the current item is before the first
            /// </summary>
            IsCurrentBeforeFirst = 0x08,

            /// <summary>
            /// Whether the current item is after the last
            /// </summary>
            IsCurrentAfterLast = 0x10,

            /// <summary>
            /// Whether we need to refresh
            /// </summary>
            NeedsRefresh = 0x20,

            /// <summary>
            /// Whether we cache the IsEmpty value
            /// </summary>
            CachedIsEmpty = 0x40,

            /// <summary>
            /// Indicates whether a page index change is in process or not
            /// </summary>
            IsPageChanging = 0x80,

            /// <summary>
            /// Whether we need to move to another page after EndDefer
            /// </summary>
            IsMoveToPageDeferred = 0x100,

            /// <summary>
            /// Whether we need to update the PageSize after EndDefer
            /// </summary>
            IsUpdatePageSizeDeferred = 0x200
        }

#endregion Enumerators

        ////------------------------------------------------------
        ////
        ////  Public Properties
        ////
        ////------------------------------------------------------

#region Public Properties

        /// <summary>
        /// Gets a value indicating whether the view supports AddNew.
        /// </summary>
        public bool CanAddNew
        {
            get
            {
                return !this.IsEditingItem &&
                    (this.SourceList != null && !this.SourceList.IsFixedSize && this.CanConstructItem);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view supports the notion of "pending changes" 
        /// on the current edit item.  This may vary, depending on the view and the particular
        /// item.  For example, a view might return true if the current edit item
        /// implements IEditableObject, or if the view has special knowledge about 
        /// the item that it can use to support rollback of pending changes.
        /// </summary>
        public bool CanCancelEdit
        {
            get { return this._editItem is global::System.ComponentModel.IEditableObject; }
        }

        /// <summary>
        /// Gets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        public bool CanChangePage
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether we support filtering with this ICollectionView.
        /// </summary>
        public bool CanFilter
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this view supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public bool CanGroup
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the view supports Remove and RemoveAt.
        /// </summary>
        public bool CanRemove
        {
            get
            {
                return !this.IsEditingItem && !this.IsAddingNew &&
                    (this.SourceList != null && !this.SourceList.IsFixedSize);
            }
        }

        /// <summary>
        /// Gets a value indicating whether we support sorting with this ICollectionView.
        /// </summary>
        public bool CanSort
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the number of records in the view after 
        /// filtering, sorting, and paging.
        /// </summary>
        public int Count
        {
            get
            {
                this.EnsureCollectionInSync();
                this.VerifyRefreshNotDeferred();

                // if we have paging
                if (this.PageSize > 0 && this.PageIndex > -1)
                {
                    if (this.IsGrouping && !this._isUsingTemporaryGroup)
                    {
                        return this._group.ItemCount;
                    }
                    else
                    {
                        return Math.Max(0, Math.Min(this.PageSize, this.InternalCount - (this._pageSize * this.PageIndex)));
                    }
                }
                else
                {
                    if (this.IsGrouping)
                    {
                        if (this._isUsingTemporaryGroup)
                        {
                            return this._temporaryGroup.ItemCount;
                        }
                        else
                        {
                            return this._group.ItemCount;
                        }
                    }
                    else
                    {
                        return this.InternalCount;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Culture to use during sorting.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return this._culture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (this._culture != value)
                {
                    this._culture = value;
                    this.OnPropertyChanged("Culture");
                }
            }
        }

        /// <summary>
        /// Gets the new item when an AddNew transaction is in progress
        /// Otherwise it returns null.
        /// </summary>
        public object CurrentAddItem
        {
            get
            {
                return this._newItem;
            }

            private set
            {
                if (this._newItem != value)
                {
                    Debug.Assert(value == null || this._newItem == null, "Old and new _newItem values are unexpectedly non null");
                    this._newItem = value;
                    this.OnPropertyChanged("IsAddingNew");
                    this.OnPropertyChanged("CurrentAddItem");
                }
            }
        }

        /// <summary>
        /// Gets the affected item when an EditItem transaction is in progress
        /// Otherwise it returns null.
        /// </summary>
        public object CurrentEditItem
        {
            get
            {
                return this._editItem;
            }

            private set
            {
                if (this._editItem != value)
                {
                    Debug.Assert(value == null || this._editItem == null, "Old and new _editItem values are unexpectedly non null");
                    bool oldCanCancelEdit = this.CanCancelEdit;
                    this._editItem = value;
                    this.OnPropertyChanged("IsEditingItem");
                    this.OnPropertyChanged("CurrentEditItem");
                    if (oldCanCancelEdit != this.CanCancelEdit)
                    {
                        this.OnPropertyChanged("CanCancelEdit");
                    }
                }
            }
        }

        /// <summary> 
        /// Gets the "current item" for this view 
        /// </summary>
        public object CurrentItem
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this._currentItem;
            }
        }

        /// <summary>
        /// Gets the ordinal position of the CurrentItem within the 
        /// (optionally sorted and filtered) view.
        /// </summary>
        public int CurrentPosition
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this._currentPosition;
            }
        }

        /// <summary>
        /// Gets or sets the Filter, which is a callback set by the consumer of the ICollectionView
        /// and used by the implementation of the ICollectionView to determine if an
        /// item is suitable for inclusion in the view.
        /// </summary>        
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support filtering and will throw a NotSupportedException.
        /// Use <seealso cref="CanFilter"/> property to test if filtering is supported before
        /// assigning a non-null value.
        /// </exception>
        public Predicate<object> Filter
        {
            get
            {
                return this._filter;
            }

            set
            {
                if (this.IsAddingNew || this.IsEditingItem)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Filter"));
                }

                if (!this.CanFilter)
                {
                    throw new NotSupportedException(PagedCollectionViewResources.CannotFilter);
                }

                if (this._filter != value)
                {
                    this._filter = value;
                    this.RefreshOrDefer();
                    this.OnPropertyChanged("Filter");
                }
            }
        }

        /// <summary>
        /// Gets the description of grouping, indexed by level.
        /// </summary>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this._group != null ? this._group.GroupDescriptions : null;
            }
        }

        /// <summary>
        /// Gets the top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                if (!this.IsGrouping)
                {
                    return null;
                }

                CollectionViewGroupRoot group = this.RootGroup;
                return group != null ? group.Items : null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an "AddNew" transaction is in progress.
        /// </summary>
        public bool IsAddingNew
        {
            get { return this._newItem != null; }
        }

        /// <summary> 
        /// Gets a value indicating whether currency is beyond the end (End-Of-File). 
        /// </summary>
        /// <returns>Whether IsCurrentAfterLast</returns>
        public bool IsCurrentAfterLast
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        /// <summary> 
        /// Gets a value indicating whether currency is before the beginning (Beginning-Of-File). 
        /// </summary>
        /// <returns>Whether IsCurrentBeforeFirst</returns>
        public bool IsCurrentBeforeFirst
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an EditItem transaction is in progress.
        /// </summary>
        public bool IsEditingItem
        {
            get { return this._editItem != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the resulting (filtered) view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                this.EnsureCollectionInSync();
                return this.InternalCount == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a page index change is in process or not.
        /// </summary>
        public bool IsPageChanging
        {
            get
            {
                return this.CheckFlag(CollectionViewFlags.IsPageChanging);
            }

            private set
            {
                if (this.CheckFlag(CollectionViewFlags.IsPageChanging) != value)
                {
                    this.SetFlag(CollectionViewFlags.IsPageChanging, value);
                    this.OnPropertyChanged("IsPageChanging");
                }
            }
        }

        /// <summary>
        /// Gets the minimum number of items known to be in the source collection
        /// that verify the current filter if any
        /// </summary>
        public int ItemCount
        {
            get
            {
                return this.InternalList.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this view needs to be refreshed.
        /// </summary>
        public bool NeedsRefresh
        {
            get { return this.CheckFlag(CollectionViewFlags.NeedsRefresh); }
        }

        /// <summary>
        /// Gets or sets whether to include a placeholder for a new item, and if so,
        /// where to put it. Only the value NewItemPlaceholderPosition.None is supported.
        /// </summary>
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                return NewItemPlaceholderPosition.None;
            }

            set
            {
                if ((NewItemPlaceholderPosition)value != NewItemPlaceholderPosition.None)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture,
                            PagedCollectionViewResources.InvalidEnumArgument,
                            "value",
                            value.ToString(),
                            typeof(NewItemPlaceholderPosition).Name));
                }
            }
        }

        /// <summary>
        /// Gets the current page we are on. (zero based)
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this._pageIndex;
            }
        }

        /// <summary>
        /// Gets or sets the number of items to display on a page. If the
        /// PageSize = 0, then we are not paging, and will display all items
        /// in the collection. Otherwise, we will have separate pages for 
        /// the items to display.
        /// </summary>
        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(PagedCollectionViewResources.InvalidPageSize);
                }

                // if the Refresh is currently deferred, cache the desired PageSize
                // and set the flag so that once the defer is over, we can then
                // update the PageSize.
                if (this.IsRefreshDeferred)
                {
                    // set cached value and flag so that we update the PageSize on EndDefer
                    this._cachedPageSize = value;
                    this.SetFlag(CollectionViewFlags.IsUpdatePageSizeDeferred, true);
                    return;
                }

                // to see whether or not to fire an OnPropertyChanged
                int oldCount = this.Count;

                if (this._pageSize != value)
                {
                    // Remember current currency values for upcoming OnPropertyChanged notifications
                    object oldCurrentItem = this.CurrentItem;
                    int oldCurrentPosition = this.CurrentPosition;
                    bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                    bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                    // Check if there is a current edited or new item so changes can be committed first.
                    if (this.CurrentAddItem != null || this.CurrentEditItem != null)
                    {
                        // Check with the ICollectionView.CurrentChanging listeners if it's OK to
                        // change the currency. If not, then we can't fire the event to allow them to
                        // commit their changes. So, we will not be able to change the PageSize.
                        if (!this.OkToChangeCurrent())
                        {
                            throw new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit);
                        }

                        // Currently CommitNew()/CommitEdit()/CancelNew()/CancelEdit() can't handle committing or 
                        // cancelling an item that is no longer on the current page. That's acceptable and means that
                        // the potential this._newItem or this._editItem needs to be committed before this PageSize change.
                        // The reason why we temporarily reset currency here is to give a chance to the bound
                        // controls to commit or cancel their potential edits/addition. The DataForm calls ForceEndEdit()
                        // for example as a result of changing currency.
                        this.SetCurrentToPosition(-1);
                        this.RaiseCurrencyChanges(true /*fireChangedEvent*/, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

                        // If the bound controls did not successfully end their potential item editing/addition, we 
                        // need to throw an exception to show that the PageSize change failed. 
                        if (this.CurrentAddItem != null || this.CurrentEditItem != null)
                        {
                            throw new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit);
                        }
                    }

                    this._pageSize = value;
                    this.OnPropertyChanged("PageSize");

                    if (this._pageSize == 0)
                    {
                        // update the groups for the current page
                        this.PrepareGroups();

                        // if we are not paging
                        this.MoveToPage(-1);
                    }
                    else if (this._pageIndex != 0)
                    {
                        if (!this.CheckFlag(CollectionViewFlags.IsMoveToPageDeferred))
                        {
                            // if the temporaryGroup was not created yet and is out of sync
                            // then create it so that we can use it as a refernce while paging.
                            if (this.IsGrouping && this._temporaryGroup.ItemCount != this.InternalList.Count)
                            {
                                this.PrepareTemporaryGroups();
                            }

                            this.MoveToFirstPage();
                        }
                    }
                    else if (this.IsGrouping)
                    {
                        // if the temporaryGroup was not created yet and is out of sync
                        // then create it so that we can use it as a refernce while paging.
                        if (this._temporaryGroup.ItemCount != this.InternalList.Count)
                        {
                            // update the groups that get created for the
                            // entire collection as well as the current page
                            this.PrepareTemporaryGroups();
                        }

                        // update the groups for the current page
                        this.PrepareGroupsForCurrentPage();
                    }

                    // if the count has changed
                    if (this.Count != oldCount)
                    {
                        this.OnPropertyChanged("Count");
                    }

                    // reset currency values
                    this.ResetCurrencyValues(oldCurrentItem, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

                    // send a notification that our collection has been updated
                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Reset));

                    // now raise currency changes at the end
                    this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
                }
            }
        }

        /// <summary>
        /// Gets the Sort criteria to sort items in collection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Clear a sort criteria by assigning SortDescription.Empty to this property.
        /// One or more sort criteria in form of <seealso cref="SortDescription"/>
        /// can be used, each specifying a property and direction to sort by.
        /// </p>
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Simpler implementations do not support sorting and will throw a NotSupportedException.
        /// Use <seealso cref="CanSort"/> property to test if sorting is supported before adding
        /// to SortDescriptions.
        /// </exception>
        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this._sortDescriptions == null)
                {
                    this.SetSortDescriptions(new SortDescriptionCollection());
                }

                return this._sortDescriptions;
            }
        }

        /// <summary>
        /// Gets the source of the IEnumerable collection we are using for our view.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get { return this._sourceCollection; }
        }

        /// <summary>
        /// Gets the total number of items in the view before paging is applied.
        /// </summary>
        public int TotalItemCount
        {
            get
            {
                return this.InternalList.Count;
            }
        }

#endregion Public Properties

        ////------------------------------------------------------
        ////
        ////  Private Properties
        ////
        ////------------------------------------------------------

#region Private Properties

        /// <summary>
        /// Gets a value indicating whether we have a valid ItemConstructor of the correct type
        /// </summary>
        private bool CanConstructItem
        {
            get
            {
                if (!this._itemConstructorIsValid)
                {
                    this.EnsureItemConstructor();
                }

                return this._itemConstructor != null;
            }
        }

        /// <summary>
        /// Gets the private count without taking paging or
        /// placeholders into account
        /// </summary>
        private int InternalCount
        {
            get { return this.InternalList.Count; }
        }

        /// <summary>
        /// Gets the InternalList
        /// </summary>
        private IList InternalList
        {
            get { return this._internalList; }
        }

        /// <summary>
        /// Gets a value indicating whether CurrentItem and CurrentPosition are
        /// up-to-date with the state and content of the collection.
        /// </summary>
        private bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return this.GetItemAt(this.CurrentPosition).Equals(this.CurrentItem);
                }
                else
                {
                    return this.CurrentItem == null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current item is in the view
        /// </summary>
        private bool IsCurrentInView
        {
            get
            {
                this.VerifyRefreshNotDeferred();

                // Calling IndexOf will check whether the specified currentItem
                // is within the (paged) view.
                return this.IndexOf(this.CurrentItem) >= 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not we have grouping 
        /// taking place in this collection.
        /// </summary>
        private bool IsGrouping
        {
            get { return this._isGrouping; }
        }

        /// <summary>
        /// Gets a value indicating whether there
        /// is still an outstanding DeferRefresh in
        /// use.  If at all possible, derived classes
        /// should not call Refresh if IsRefreshDeferred
        /// is true.
        /// </summary>
        private bool IsRefreshDeferred
        {
            get { return this._deferLevel > 0; }
        }

        /// <summary>
        /// Gets whether the current page is empty and we need
        /// to move to a previous page.
        /// </summary>
        private bool NeedToMoveToPreviousPage
        {
            get { return (this.PageSize > 0 && this.Count == 0 && this.PageIndex != 0 && this.PageCount == this.PageIndex); }
        }

        /// <summary>
        /// Gets a value indicating whether we are on the last local page
        /// </summary>
        private bool OnLastLocalPage
        {
            get
            {
                if (this.PageSize == 0)
                {
                    return false;
                }

                Debug.Assert(this.PageCount > 0, "Unexpected PageCount <= 0");

                // if we have no items (PageCount==1) or there is just one page
                if (this.PageCount == 1)
                {
                    return true;
                }

                return (this.PageIndex == this.PageCount - 1);
            }
        }

        /// <summary>
        /// Gets the number of pages we currently have
        /// </summary>
        private int PageCount
        {
            get { return (this._pageSize > 0) ? Math.Max(1, (int)Math.Ceiling((double)this.ItemCount / this._pageSize)) : 0; }
        }

        /// <summary>
        /// Gets the root of the Group that we expose to the user
        /// </summary>
        private CollectionViewGroupRoot RootGroup
        {
            get
            {
                return this._isUsingTemporaryGroup ? this._temporaryGroup : this._group;
            }
        }

        /// <summary>
        /// Gets the SourceCollection as an IList
        /// </summary>
        private IList SourceList
        {
            get { return this.SourceCollection as IList; }
        }

        /// <summary>
        /// Gets Timestamp used by the NewItemAwareEnumerator to determine if a
        /// collection change has occurred since the enumerator began.  (If so,
        /// MoveNext should throw.)
        /// </summary>
        private int Timestamp
        {
            get { return this._timestamp; }
        }

        /// <summary>
        /// Gets a value indicating whether a private copy of the data 
        /// is needed for sorting, filtering, and paging. We want any deriving 
        /// classes to also be able to access this value to see whether or not 
        /// to use the default source collection, or the internal list.
        /// </summary>
        private bool UsesLocalArray
        {
            get { return this.SortDescriptions.Count > 0 || this.Filter != null || this._pageSize > 0 || this.GroupDescriptions.Count > 0; }
        }

#endregion Private Properties

        ////------------------------------------------------------
        ////
        ////  Indexers
        ////
        ////------------------------------------------------------

#region Indexers

        /// <summary>
        /// Return the item at the specified index
        /// </summary>
        /// <param name="index">Index of the item we want to retrieve</param>
        /// <returns>The item at the specified index</returns>
        public object this[int index]
        {
            get { return this.GetItemAt(index); }
        }

#endregion Indexers

        ////------------------------------------------------------
        ////
        ////  Public Methods
        ////
        ////------------------------------------------------------

#region Public Methods

        /// <summary>
        /// Add a new item to the underlying collection.  Returns the new item.
        /// After calling AddNew and changing the new item as desired, either
        /// CommitNew or CancelNew" should be called to complete the transaction.
        /// </summary>
        /// <returns>The new item we are adding</returns>
        public object AddNew()
        {
            this.EnsureCollectionInSync();
            this.VerifyRefreshNotDeferred();

            if (this.IsEditingItem)
            {
                // Implicitly close a previous EditItem
                this.CommitEdit();
            }

            // Implicitly close a previous AddNew
            this.CommitNew();

            // Checking CanAddNew will validate that we have the correct itemConstructor
            if (!this.CanAddNew)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedForView, "AddNew"));
            }

            object newItem = null;

            if (this._itemConstructor != null)
            {
                newItem = this._itemConstructor.Invoke(null);
            }

            try
            {
                // temporarily disable the CollectionChanged event
                // handler so filtering, sorting, or grouping
                // doesn't get applied yet
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, false);

                if (this.SourceList != null)
                {
                    this.SourceList.Add(newItem);
                }
            }
            finally
            {
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, true);
            }

            // Modify our _trackingEnumerator so that it shows that our collection is "up to date" 
            // and will not refresh for now.
            this._trackingEnumerator = this._sourceCollection.GetEnumerator();

            int addIndex;
            int removeIndex = -1;

            // Adjust index based on where it should be displayed in view.
            if (this.PageSize > 0)
            {
                // if the page is full (Count==PageSize), then replace last item (Count-1).
                // otherwise, we just append at end (Count).
                addIndex = this.Count - ((this.Count == this.PageSize) ? 1 : 0);

                // if the page is full, remove the last item to make space for the new one.
                removeIndex = (this.Count == this.PageSize) ? addIndex : -1;
            }
            else
            {
                // for non-paged lists, we want to insert the item 
                // as the last item in the view
                addIndex = this.Count;
            }

            // if we need to remove an item from the view due to paging
            if (removeIndex > -1)
            {
                object removeItem = this.GetItemAt(removeIndex);
                if (this.IsGrouping)
                {
                    this._group.RemoveFromSubgroups(removeItem);
                }

                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        removeItem,
                        removeIndex));
            }

            // add the new item to the internal list
            this._internalList.Insert(this.ConvertToInternalIndex(addIndex), newItem);
            this.OnPropertyChanged("ItemCount");

            object oldCurrentItem = this.CurrentItem;
            int oldCurrentPosition = this.CurrentPosition;
            bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

            this.AdjustCurrencyForAdd(null, addIndex);

            if (this.IsGrouping)
            {
                this._group.InsertSpecialItem(this._group.Items.Count, newItem, false);
                if (this.PageSize > 0)
                {
                    this._temporaryGroup.InsertSpecialItem(this._temporaryGroup.Items.Count, newItem, false);
                }
            }

            // fire collection changed.
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    newItem,
                    addIndex));

            this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

            // set the current new item
            this.CurrentAddItem = newItem;

            this.MoveCurrentTo(newItem);

            // if the new item is editable, call BeginEdit on it
            IEditableObject editableObject = newItem as IEditableObject;
            if (editableObject != null)
            {
                editableObject.BeginEdit();
            }

            return newItem;
        }

        /// <summary>
        /// Complete the transaction started by <seealso cref="EditItem"/>.
        /// The pending changes (if any) to the item are discarded.
        /// </summary>
        public void CancelEdit()
        {
            if (this.IsAddingNew)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelEdit", "AddNew"));
            }
            else if (!this.CanCancelEdit)
            {
                throw new InvalidOperationException(PagedCollectionViewResources.CancelEditNotSupported);
            }

            this.VerifyRefreshNotDeferred();

            if (this.CurrentEditItem == null)
            {
                return;
            }

            object editItem = this.CurrentEditItem;
            this.CurrentEditItem = null;

            global::System.ComponentModel.IEditableObject ieo = editItem as global::System.ComponentModel.IEditableObject;
            if (ieo != null)
            {
                ieo.CancelEdit();
            }
            else
            {
                throw new InvalidOperationException(PagedCollectionViewResources.CancelEditNotSupported);
            }
        }

        /// <summary>
        /// Complete the transaction started by AddNew. The new
        /// item is removed from the collection.
        /// </summary>
        public void CancelNew()
        {
            if (this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelNew", "EditItem"));
            }

            this.VerifyRefreshNotDeferred();

            if (this.CurrentAddItem == null)
            {
                return;
            }

            // get index of item before it is removed
            int index = this.IndexOf(this.CurrentAddItem);

            // remove the new item from the underlying collection
            try
            {
                // temporarily disable the CollectionChanged event
                // handler so filtering, sorting, or grouping
                // doesn't get applied yet
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, false);

                if (this.SourceList != null)
                {
                    this.SourceList.Remove(this.CurrentAddItem);
                }
            }
            finally
            {
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, true);
            }

            // Modify our _trackingEnumerator so that it shows that our collection is "up to date" 
            // and will not refresh for now.
            this._trackingEnumerator = this._sourceCollection.GetEnumerator();

            // fire the correct events
            if (this.CurrentAddItem != null)
            {
                object newItem = this.EndAddNew(true);

                int addIndex = -1;

                // Adjust index based on where it should be displayed in view.
                if (this.PageSize > 0 && !this.OnLastLocalPage)
                {
                    // if there is paging and we are not on the last page, we need
                    // to bring in an item from the next page.
                    addIndex = this.Count - 1;
                }

                // remove the new item from the internal list 
                this.InternalList.Remove(newItem);

                if (this.IsGrouping)
                {
                    this._group.RemoveSpecialItem(this._group.Items.Count - 1, newItem, false);
                    if (this.PageSize > 0)
                    {
                        this._temporaryGroup.RemoveSpecialItem(this._temporaryGroup.Items.Count - 1, newItem, false);
                    }
                }

                this.OnPropertyChanged("ItemCount");

                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                this.AdjustCurrencyForRemove(index);

                // fire collection changed.
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        newItem,
                        index));

                this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

                // if we need to add an item into the view due to paging
                if (addIndex > -1)
                {
                    int internalIndex = this.ConvertToInternalIndex(addIndex);
                    object addItem = null;
                    if (this.IsGrouping)
                    {
                        addItem = this._temporaryGroup.LeafAt(internalIndex);
                        this._group.AddToSubgroups(addItem, false /*loading*/);
                    }
                    else
                    {
                        addItem = this.InternalItemAt(internalIndex);
                    }

                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            addItem,
                            this.IndexOf(addItem)));
                }
            }
        }

        /// <summary>
        /// Complete the transaction started by <seealso cref="EditItem"/>.
        /// The pending changes (if any) to the item are committed.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Handles multiple input types and scenarios")]
        public void CommitEdit()
        {
            if (this.IsAddingNew)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitEdit", "AddNew"));
            }

            this.VerifyRefreshNotDeferred();

            if (this.CurrentEditItem == null)
            {
                return;
            }

            object editItem = this.CurrentEditItem;
            this.CurrentEditItem = null;

            global::System.ComponentModel.IEditableObject ieo = editItem as global::System.ComponentModel.IEditableObject;
            if (ieo != null)
            {
                ieo.EndEdit();
            }

            if (this.UsesLocalArray)
            {
                // first remove the item from the array so that we can insert into the correct position
                int removeIndex = this.IndexOf(editItem);
                int internalRemoveIndex = this.InternalIndexOf(editItem);
                this._internalList.Remove(editItem);

                // check whether to restore currency to the item being edited
                object restoreCurrencyTo = (editItem == this.CurrentItem) ? editItem : null;

                if (removeIndex >= 0 && this.IsGrouping)
                {
                    // we can't just call RemoveFromSubgroups, as the group name
                    // for the item may have changed during the edit.
                    this._group.RemoveItemFromSubgroupsByExhaustiveSearch(editItem);
                    if (this.PageSize > 0)
                    {
                        this._temporaryGroup.RemoveItemFromSubgroupsByExhaustiveSearch(editItem);
                    }
                }

                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                // only adjust currency and fire the event if we actually removed the item
                if (removeIndex >= 0)
                {
                    this.AdjustCurrencyForRemove(removeIndex);

                    // raise the remove event so we can next insert it into the correct place
                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            editItem,
                            removeIndex));
                }

                // check to see that the item will be added back in
                bool passedFilter = this.PassesFilter(editItem);

                // if we removed all items from the current page,
                // move to the previous page. we do not need to 
                // fire additional notifications, as moving the page will
                // trigger a reset.
                if (this.NeedToMoveToPreviousPage && !passedFilter)
                {
                    this.MoveToPreviousPage();
                    return;
                }

                // next process adding it into the correct location
                this.ProcessInsertToCollection(editItem, internalRemoveIndex);

                int pageStartIndex = this.PageIndex * this.PageSize;
                int nextPageStartIndex = pageStartIndex + this.PageSize;

                if (this.IsGrouping)
                {
                    int leafIndex = -1;
                    if (passedFilter && this.PageSize > 0)
                    {
                        this._temporaryGroup.AddToSubgroups(editItem, false /*loading*/);
                        leafIndex = this._temporaryGroup.LeafIndexOf(editItem);
                    }

                    // if we are not paging, we should just be able to add the item.
                    // otherwise, we need to validate that it is within the current page.
                    if (passedFilter && (this.PageSize == 0 ||
                       (pageStartIndex <= leafIndex && nextPageStartIndex > leafIndex)))
                    {
                        this._group.AddToSubgroups(editItem, false /*loading*/);
                        int addIndex = this.IndexOf(editItem);
                        this.AdjustCurrencyForEdit(restoreCurrencyTo, addIndex);
                        this.OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                editItem,
                                addIndex));
                    }
                    else if (this.PageSize > 0)
                    {
                        int addIndex = -1;
                        if (passedFilter && leafIndex < pageStartIndex)
                        {
                            // if the item was added to an earlier page, then we need to bring
                            // in the item that would have been pushed down to this page
                            addIndex = pageStartIndex;
                        }
                        else if (!this.OnLastLocalPage && removeIndex >= 0)
                        {
                            // if the item was added to a later page, then we need to bring in the
                            // first item from the next page
                            addIndex = nextPageStartIndex - 1;
                        }

                        object addItem = this._temporaryGroup.LeafAt(addIndex);
                        if (addItem != null)
                        {
                            this._group.AddToSubgroups(addItem, false /*loading*/);
                            addIndex = this.IndexOf(addItem);
                            this.AdjustCurrencyForEdit(restoreCurrencyTo, addIndex);
                            this.OnCollectionChanged(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Add,
                                    addItem,
                                    addIndex));
                        }
                    }
                }
                else
                {
                    // if we are still within the view
                    int addIndex = this.IndexOf(editItem);
                    if (addIndex >= 0)
                    {
                        this.AdjustCurrencyForEdit(restoreCurrencyTo, addIndex);
                        this.OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                editItem,
                                addIndex));
                    }
                    else if (this.PageSize > 0)
                    {
                        // calculate whether the item was inserted into the previous page
                        bool insertedToPreviousPage = this.PassesFilter(editItem) &&
                            (this.InternalIndexOf(editItem) < this.ConvertToInternalIndex(0));
                        addIndex = insertedToPreviousPage ? 0 : this.Count - 1;

                        // don't fire the event if we are on the last page
                        // and we don't have any items to bring in.
                        if (insertedToPreviousPage || (!this.OnLastLocalPage && removeIndex >= 0))
                        {
                            this.AdjustCurrencyForEdit(restoreCurrencyTo, addIndex);
                            this.OnCollectionChanged(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Add,
                                    this.GetItemAt(addIndex),
                                    addIndex));
                        }
                    }
                }

                // now raise currency changes at the end
                this.RaiseCurrencyChanges(true, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
            }
            else if (!this.Contains(editItem))
            {
                // if the item did not belong to the collection, add it
                this.InternalList.Add(editItem);
            }
        }

        /// <summary>
        /// Complete the transaction started by AddNew. We follow the WPF
        /// convention in that the view's sort, filter, and paging
        /// specifications (if any) are applied to the new item.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Handles multiple input types and scenarios")]
        public void CommitNew()
        {
            if (this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitNew", "EditItem"));
            }

            this.VerifyRefreshNotDeferred();

            if (this.CurrentAddItem == null)
            {
                return;
            }

            // End the AddNew transaction
            object newItem = this.EndAddNew(false);

            // keep track of the current item
            object previousCurrentItem = this.CurrentItem;

            // Modify our _trackingEnumerator so that it shows that our collection is "up to date" 
            // and will not refresh for now.
            this._trackingEnumerator = this._sourceCollection.GetEnumerator();

            if (this.UsesLocalArray)
            {
                // first remove the item from the array so that we can insert into the correct position
                int removeIndex = this.Count - 1;
                int internalIndex = this._internalList.IndexOf(newItem);
                this._internalList.Remove(newItem);

                if (this.IsGrouping)
                {
                    this._group.RemoveSpecialItem(this._group.Items.Count - 1, newItem, false);
                    if (this.PageSize > 0)
                    {
                        this._temporaryGroup.RemoveSpecialItem(this._temporaryGroup.Items.Count - 1, newItem, false);
                    }
                }

                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                this.AdjustCurrencyForRemove(removeIndex);

                // raise the remove event so we can next insert it into the correct place
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        newItem,
                        removeIndex));

                // check to see that the item will be added back in
                bool passedFilter = this.PassesFilter(newItem);

                // next process adding it into the correct location
                this.ProcessInsertToCollection(newItem, internalIndex);

                int pageStartIndex = this.PageIndex * this.PageSize;
                int nextPageStartIndex = pageStartIndex + this.PageSize;

                if (this.IsGrouping)
                {
                    int leafIndex = -1;
                    if (passedFilter && this.PageSize > 0)
                    {
                        this._temporaryGroup.AddToSubgroups(newItem, false /*loading*/);
                        leafIndex = this._temporaryGroup.LeafIndexOf(newItem);
                    }

                    // if we are not paging, we should just be able to add the item.
                    // otherwise, we need to validate that it is within the current page.
                    if (passedFilter && (this.PageSize == 0 ||
                       (pageStartIndex <= leafIndex && nextPageStartIndex > leafIndex)))
                    {
                        this._group.AddToSubgroups(newItem, false /*loading*/);
                        int addIndex = this.IndexOf(newItem);

                        // adjust currency to either the previous current item if possible
                        // or to the item at the end of the list where the new item was.
                        if (previousCurrentItem != null)
                        {
                            if (this.Contains(previousCurrentItem))
                            {
                                this.AdjustCurrencyForAdd(previousCurrentItem, addIndex);
                            }
                            else
                            {
                                this.AdjustCurrencyForAdd(this.GetItemAt(this.Count - 1), addIndex);
                            }
                        }

                        this.OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                newItem,
                                addIndex));
                    }
                    else
                    {
                        if (!passedFilter && (this.PageSize == 0 || this.OnLastLocalPage))
                        {
                            this.AdjustCurrencyForRemove(removeIndex);
                        }
                        else if (this.PageSize > 0)
                        {
                            int addIndex = -1;
                            if (passedFilter && leafIndex < pageStartIndex)
                            {
                                // if the item was added to an earlier page, then we need to bring
                                // in the item that would have been pushed down to this page
                                addIndex = pageStartIndex;
                            }
                            else if (!this.OnLastLocalPage)
                            {
                                // if the item was added to a later page, then we need to bring in the
                                // first item from the next page
                                addIndex = nextPageStartIndex - 1;
                            }

                            object addItem = this._temporaryGroup.LeafAt(addIndex);
                            if (addItem != null)
                            {
                                this._group.AddToSubgroups(addItem, false /*loading*/);
                                addIndex = this.IndexOf(addItem);

                                // adjust currency to either the previous current item if possible
                                // or to the item at the end of the list where the new item was.
                                if (previousCurrentItem != null)
                                {
                                    if (this.Contains(previousCurrentItem))
                                    {
                                        this.AdjustCurrencyForAdd(previousCurrentItem, addIndex);
                                    }
                                    else
                                    {
                                        this.AdjustCurrencyForAdd(this.GetItemAt(this.Count - 1), addIndex);
                                    }
                                }

                                this.OnCollectionChanged(
                                    new NotifyCollectionChangedEventArgs(
                                        NotifyCollectionChangedAction.Add,
                                        addItem,
                                        addIndex));
                            }
                        }
                    }
                }
                else
                {
                    // if we are still within the view
                    int addIndex = this.IndexOf(newItem);
                    if (addIndex >= 0)
                    {
                        this.AdjustCurrencyForAdd(newItem, addIndex);
                        this.OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                newItem,
                                addIndex));
                    }
                    else
                    {
                        if (!passedFilter && (this.PageSize == 0 || this.OnLastLocalPage))
                        {
                            this.AdjustCurrencyForRemove(removeIndex);
                        }
                        else if (this.PageSize > 0)
                        {
                            bool insertedToPreviousPage = this.InternalIndexOf(newItem) < this.ConvertToInternalIndex(0);
                            addIndex = insertedToPreviousPage ? 0 : this.Count - 1;

                            // don't fire the event if we are on the last page
                            // and we don't have any items to bring in.
                            if (insertedToPreviousPage || !this.OnLastLocalPage)
                            {
                                this.AdjustCurrencyForAdd(null, addIndex);
                                this.OnCollectionChanged(
                                    new NotifyCollectionChangedEventArgs(
                                        NotifyCollectionChangedAction.Add,
                                        this.GetItemAt(addIndex),
                                        addIndex));
                            }
                        }
                    }
                }

                // we want to fire the current changed event, even if we kept
                // the same current item and position, since the item was
                // removed/added back to the collection
                this.RaiseCurrencyChanges(true, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
            }
        }

        /// <summary>
        /// Return true if the item belongs to this view.  No assumptions are
        /// made about the item. This method will behave similarly to IList.Contains().
        /// If the caller knows that the item belongs to the
        /// underlying collection, it is more efficient to call PassesFilter.
        /// </summary>
        /// <param name="item">The item we are checking to see whether it is within the collection</param>
        /// <returns>Boolean value of whether or not the collection contains the item</returns>
        public bool Contains(object item)
        {
            this.EnsureCollectionInSync();
            this.VerifyRefreshNotDeferred();
            return this.IndexOf(item) >= 0;
        }

        /// <summary>
        /// Enter a Defer Cycle.
        /// Defer cycles are used to coalesce changes to the ICollectionView.
        /// </summary>
        /// <returns>IDisposable used to notify that we no longer need to defer, when we dispose</returns>
        public IDisposable DeferRefresh()
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "DeferRefresh"));
            }

            ++this._deferLevel;
            return new DeferHelper(this);
        }

        /// <summary>
        /// Begins an editing transaction on the given item.  The transaction is
        /// completed by calling either CommitEdit or CancelEdit.  Any changes made 
        /// to the item during the transaction are considered "pending", provided 
        /// that the view supports the notion of "pending changes" for the given item.
        /// </summary>
        /// <param name="item">Item we want to edit</param>
        public void EditItem(object item)
        {
            this.VerifyRefreshNotDeferred();

            if (this.IsAddingNew)
            {
                if (Object.Equals(item, this.CurrentAddItem))
                {
                    // EditItem(newItem) is a no-op
                    return;
                }

                // implicitly close a previous AddNew
                this.CommitNew();
            }

            // implicitly close a previous EditItem transaction
            this.CommitEdit();

            this.CurrentEditItem = item;

            global::System.ComponentModel.IEditableObject ieo = item as global::System.ComponentModel.IEditableObject;
            if (ieo != null)
            {
                ieo.BeginEdit();
            }
        }

        /// <summary> 
        /// Implementation of IEnumerable.GetEnumerator().
        /// This provides a way to enumerate the members of the collection
        /// without changing the currency.
        /// </summary>
        /// <returns>IEnumerator for the collection</returns>
        public IEnumerator GetEnumerator()
        {
            this.EnsureCollectionInSync();
            this.VerifyRefreshNotDeferred();

            if (this.IsGrouping)
            {
                CollectionViewGroupRoot group = this.RootGroup;
                return group != null ? group.GetLeafEnumerator() : null;
            }

            // if we are paging
            if (this.PageSize > 0)
            {
                List<object> list = new List<object>();

                // if we are in the middle of asynchronous load
                if (this.PageIndex < 0)
                {
                    return list.GetEnumerator();
                }

                for (int index = this._pageSize * this.PageIndex;
                    index < (int)Math.Min(this._pageSize * (this.PageIndex + 1), this.InternalList.Count);
                    index++)
                {
                    list.Add(this.InternalList[index]);
                }

                return new NewItemAwareEnumerator(this, list.GetEnumerator(), this.CurrentAddItem);
            }
            else
            {
                return new NewItemAwareEnumerator(this, this.InternalList.GetEnumerator(), this.CurrentAddItem);
            }
        }

        /// <summary>
        /// Interface Implementation for GetEnumerator()
        /// </summary>
        /// <returns>IEnumerator that we get from our internal collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Retrieve item at the given zero-based index in this PagedCollectionView, after the source collection
        /// is filtered, sorted, and paged.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        /// <param name="index">Index of the item we want to retrieve</param>
        /// <returns>Item at specified index</returns>
        public object GetItemAt(int index)
        {
            this.EnsureCollectionInSync();
            this.VerifyRefreshNotDeferred();

            // for indicies larger than the count
            if (index >= this.Count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (this.IsGrouping)
            {
                CollectionViewGroupRoot group = this.RootGroup;
                return group != null ?
                    group.LeafAt(this._isUsingTemporaryGroup ? this.ConvertToInternalIndex(index) : index) :
                    null;
            }

            if (this.IsAddingNew && this.UsesLocalArray && index == this.Count - 1)
            {
                return this.CurrentAddItem;
            }

            return this.InternalItemAt(this.ConvertToInternalIndex(index));
        }

        /// <summary> 
        /// Return the index where the given item appears, or -1 if doesn't appear.
        /// </summary>
        /// <param name="item">Item we are searching for</param>
        /// <returns>Index of specified item</returns>
        public int IndexOf(object item)
        {
            this.EnsureCollectionInSync();
            this.VerifyRefreshNotDeferred();

            if (this.IsGrouping)
            {
                CollectionViewGroupRoot group = this.RootGroup;
                return group != null ? group.LeafIndexOf(item) : -1;
            }
            if (this.IsAddingNew && Object.Equals(item, this.CurrentAddItem) && this.UsesLocalArray)
            {
                return this.Count - 1;
            }

            int internalIndex = this.InternalIndexOf(item);

            if (this.PageSize > 0 && internalIndex != -1)
            {
                if ((internalIndex >= (this.PageIndex * this._pageSize)) &&
                    (internalIndex < ((this.PageIndex + 1) * this._pageSize)))
                {
                    return internalIndex - (this.PageIndex * this._pageSize);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return internalIndex;
            }
        }

        /// <summary> 
        /// Move to the given item. 
        /// </summary>
        /// <param name="item">Item we want to move the currency to</param>
        /// <returns>Whether the operation was successful</returns>
        public bool MoveCurrentTo(object item)
        {
            this.VerifyRefreshNotDeferred();

            // if already on item, don't do anything
            if (Object.Equals(this.CurrentItem, item))
            {
                // also check that we're not fooled by a false null currentItem
                if (item != null || this.IsCurrentInView)
                {
                    return this.IsCurrentInView;
                }
            }

            // if the item is not found IndexOf() will return -1, and
            // the MoveCurrentToPosition() below will move current to BeforeFirst
            // The IndexOf function takes into account paging, filtering, and sorting
            return this.MoveCurrentToPosition(this.IndexOf(item));
        }

        /// <summary> 
        /// Move to the first item. 
        /// </summary>
        /// <returns>Whether the operation was successful</returns>
        public bool MoveCurrentToFirst()
        {
            this.VerifyRefreshNotDeferred();

            return this.MoveCurrentToPosition(0);
        }

        /// <summary> 
        /// Move to the last item. 
        /// </summary>
        /// <returns>Whether the operation was successful</returns>
        public bool MoveCurrentToLast()
        {
            this.VerifyRefreshNotDeferred();

            int index = this.Count - 1;

            return this.MoveCurrentToPosition(index);
        }

        /// <summary> 
        /// Move to the next item. 
        /// </summary>
        /// <returns>Whether the operation was successful</returns>
        public bool MoveCurrentToNext()
        {
            this.VerifyRefreshNotDeferred();

            int index = this.CurrentPosition + 1;

            if (index <= this.Count)
            {
                return this.MoveCurrentToPosition(index);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move CurrentItem to this index
        /// </summary>
        /// <param name="position">Position we want to move the currency to</param>
        /// <returns>True if the resulting CurrentItem is an item within the view; otherwise False</returns>
        public bool MoveCurrentToPosition(int position)
        {
            this.VerifyRefreshNotDeferred();

            // We want to allow the user to set the currency to just
            // beyond the last item. EnumerableCollectionView in WPF
            // also checks (position > this.Count) though the ListCollectionView
            // looks for (position >= this.Count).
            if (position < -1 || position > this.Count)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if ((position != this.CurrentPosition || !this.IsCurrentInSync)
                && this.OkToChangeCurrent())
            {
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                this.SetCurrentToPosition(position);
                this.OnCurrentChanged();

                if (this.IsCurrentAfterLast != oldIsCurrentAfterLast)
                {
                    this.OnPropertyChanged("IsCurrentAfterLast");
                }

                if (this.IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
                {
                    this.OnPropertyChanged("IsCurrentBeforeFirst");
                }

                this.OnPropertyChanged("CurrentPosition");
                this.OnPropertyChanged("CurrentItem");
            }

            return this.IsCurrentInView;
        }

        /// <summary> 
        /// Move to the previous item. 
        /// </summary>
        /// <returns>Whether the operation was successful</returns>
        public bool MoveCurrentToPrevious()
        {
            this.VerifyRefreshNotDeferred();

            int index = this.CurrentPosition - 1;

            if (index >= -1)
            {
                return this.MoveCurrentToPosition(index);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the first page.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveToFirstPage()
        {
            return this.MoveToPage(0);
        }

        /// <summary>
        /// Moves to the last page.
        /// The move is only attempted when TotalItemCount is known.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveToLastPage()
        {
            if (this.TotalItemCount != -1 && this.PageSize > 0)
            {
                return this.MoveToPage(this.PageCount - 1);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the page after the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveToNextPage()
        {
            return this.MoveToPage(this._pageIndex + 1);
        }

        /// <summary>
        /// Requests a page move to page <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">Index of the target page</param>
        /// <returns>Whether or not the move was successfully initiated.</returns>
        public bool MoveToPage(int pageIndex)
        {
            // Boundary checks for negative pageIndex
            if (pageIndex < -1)
            {
                return false;
            }

            // if the Refresh is deferred, cache the requested PageIndex so that we
            // can move to the desired page when EndDefer is called.
            if (this.IsRefreshDeferred)
            {
                // set cached value and flag so that we move to the page on EndDefer
                this._cachedPageIndex = pageIndex;
                this.SetFlag(CollectionViewFlags.IsMoveToPageDeferred, true);
                return false;
            }

            // check for invalid pageIndex
            if (pageIndex == -1 && this.PageSize > 0)
            {
                return false;
            }

            // Check if the target page is out of bound, or equal to the current page
            if (pageIndex >= this.PageCount || this._pageIndex == pageIndex)
            {
                return false;
            }

            // Check with the ICollectionView.CurrentChanging listeners if it's OK to move
            // on to another page
            if (!this.OkToChangeCurrent())
            {
                return false;
            }

            if (this.RaisePageChanging(pageIndex) && pageIndex != -1)
            {
                // Page move was cancelled. Abort the move, but only if the target index isn't -1.
                return false;
            }

            // Check if there is a current edited or new item so changes can be committed first.
            if (this.CurrentAddItem != null || this.CurrentEditItem != null)
            {
                // Remember current currency values for upcoming OnPropertyChanged notifications
                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                // Currently CommitNew()/CommitEdit()/CancelNew()/CancelEdit() can't handle committing or 
                // cancelling an item that is no longer on the current page. That's acceptable and means that
                // the potential this._newItem or this._editItem needs to be committed before this page move.
                // The reason why we temporarily reset currency here is to give a chance to the bound
                // controls to commit or cancel their potential edits/addition. The DataForm calls ForceEndEdit()
                // for example as a result of changing currency.
                this.SetCurrentToPosition(-1);
                this.RaiseCurrencyChanges(true /*fireChangedEvent*/, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

                // If the bound controls did not successfully end their potential item editing/addition, the 
                // page move needs to be aborted. 
                if (this.CurrentAddItem != null || this.CurrentEditItem != null)
                {
                    // Since PageChanging was raised and not cancelled, a PageChanged notification needs to be raised
                    // even though the PageIndex actually did not change.
                    this.RaisePageChanged();

                    // Restore original currency
                    Debug.Assert(this.CurrentItem == null, "Unexpected this.CurrentItem != null");
                    Debug.Assert(this.CurrentPosition == -1, "Unexpected this.CurrentPosition != -1");
                    Debug.Assert(this.IsCurrentBeforeFirst, "Unexpected this.IsCurrentBeforeFirst == false");
                    Debug.Assert(!this.IsCurrentAfterLast, "Unexpected this.IsCurrentAfterLast == true");

                    this.SetCurrentToPosition(oldCurrentPosition);
                    this.RaiseCurrencyChanges(false /*fireChangedEvent*/, null /*oldCurrentItem*/, -1 /*oldCurrentPosition*/,
                        true /*oldIsCurrentBeforeFirst*/, false /*oldIsCurrentAfterLast*/);

                    return false;
                }

                // Finally raise a CurrentChanging notification for the upcoming currency change
                // that will occur in CompletePageMove(pageIndex).
                this.OnCurrentChanging();
            }

            this.IsPageChanging = true;
            this.CompletePageMove(pageIndex);

            return true;
        }

        /// <summary>
        /// Moves to the page before the current page we are on.
        /// </summary>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveToPreviousPage()
        {
            return this.MoveToPage(this._pageIndex - 1);
        }

        /// <summary>
        /// Return true if the item belongs to this view.  The item is assumed to belong to the
        /// underlying DataCollection;  this method merely takes filters into account.
        /// It is commonly used during collection-changed notifications to determine if the added/removed
        /// item requires processing.
        /// Returns true if no filter is set on collection view.
        /// </summary>
        /// <param name="item">The item to compare against the Filter</param>
        /// <returns>Whether the item passes the filter</returns>
        public bool PassesFilter(object item)
        {
            if (this.Filter != null)
            {
                return this.Filter(item);
            }

            return true;
        }

        /// <summary>
        /// Re-create the view, using any SortDescriptions and/or Filters.
        /// </summary>
        public void Refresh()
        {
            IEditableCollectionView ecv = this as IEditableCollectionView;
            if (ecv != null && (ecv.IsAddingNew || ecv.IsEditingItem))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Refresh"));
            }

            this.RefreshInternal();
        }

        /// <summary>
        /// Remove the given item from the underlying collection. It
        /// needs to be in the current filtered, sorted, and paged view
        /// to call this.
        /// </summary>
        /// <param name="item">Item we want to remove</param>
        public void Remove(object item)
        {
            int index = this.IndexOf(item);
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
        }

        /// <summary>
        /// Remove the item at the given index from the underlying collection.
        /// The index is interpreted with respect to the view (filtered, sorted,
        /// and paged list).
        /// </summary>
        /// <param name="index">Index of the item we want to remove</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index", PagedCollectionViewResources.IndexOutOfRange);
            }

            if (this.IsEditingItem || this.IsAddingNew)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "RemoveAt"));
            }
            else if (!this.CanRemove)
            {
                throw new InvalidOperationException(PagedCollectionViewResources.RemoveNotSupported);
            }

            this.VerifyRefreshNotDeferred();

            // convert the index from "view-relative" to "list-relative"
            object item = this.GetItemAt(index);

            // before we remove the item, see if we are not on the last page
            // and will have to bring in a new item to replace it
            bool replaceItem = this.PageSize > 0 && !this.OnLastLocalPage;

            try
            {
                // temporarily disable the CollectionChanged event
                // handler so filtering, sorting, or grouping
                // doesn't get applied yet
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, false);

                if (this.SourceList != null)
                {
                    this.SourceList.Remove(item);
                }
            }
            finally
            {
                this.SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, true);
            }

            // Modify our _trackingEnumerator so that it shows that our collection is "up to date" 
            // and will not refresh for now.
            this._trackingEnumerator = this._sourceCollection.GetEnumerator();

            Debug.Assert(index == this.IndexOf(item), "IndexOf returned unexpected value");

            // remove the item from the internal list
            this._internalList.Remove(item);

            if (this.IsGrouping)
            {
                if (this.PageSize > 0)
                {
                    this._temporaryGroup.RemoveFromSubgroups(item);
                }
                this._group.RemoveFromSubgroups(item);
            }

            object oldCurrentItem = this.CurrentItem;
            int oldCurrentPosition = this.CurrentPosition;
            bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

            this.AdjustCurrencyForRemove(index);

            // fire remove notification
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item,
                    index));

            this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

            // if we removed all items from the current page,
            // move to the previous page. we do not need to 
            // fire additional notifications, as moving the page will
            // trigger a reset.
            if (this.NeedToMoveToPreviousPage)
            {
                this.MoveToPreviousPage();
                return;
            }

            // if we are paging, we may have to fire another notification for the item
            // that needs to replace the one we removed on this page.
            if (replaceItem)
            {
                // we first need to add the item into the current group
                if (this.IsGrouping)
                {
                    object newItem = this._temporaryGroup.LeafAt((this.PageSize * (this.PageIndex + 1)) - 1);
                    if (newItem != null)
                    {
                        this._group.AddToSubgroups(newItem, false /*loading*/);
                    }
                }

                // fire the add notification
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        this.GetItemAt(this.PageSize - 1),
                        this.PageSize - 1));
            }
        }

#endregion Public Methods

        ////------------------------------------------------------
        ////
        ////  Static Methods
        ////
        ////------------------------------------------------------

#region Static Methods

        /// <summary>
        /// Helper for SortList to handle nested properties (e.g. Address.Street)
        /// </summary>
        /// <param name="item">parent object</param>
        /// <param name="propertyPath">property names path</param>
        /// <param name="propertyType">property type that we want to check for</param>
        /// <returns>child object</returns>
        internal static object InvokePath(object item, string propertyPath, Type propertyType)
        {
            Exception exception;
            object propertyValue = TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out exception);
            if (exception != null)
            {
                throw exception;
            }
            return propertyValue;
        }

#endregion Static Methods

        ////------------------------------------------------------
        ////
        ////  Private Methods
        ////
        ////------------------------------------------------------

#region Private Methods

        /// <summary>
        /// Fix up CurrentPosition and CurrentItem after a collection change
        /// </summary>
        /// <param name="newCurrentItem">Item that we want to set currency to</param>
        /// <param name="index">Index of item involved in the collection change</param>
        private void AdjustCurrencyForAdd(object newCurrentItem, int index)
        {
            if (newCurrentItem != null)
            {
                int newItemIndex = this.IndexOf(newCurrentItem);

                // if we already have the correct currency set, we don't 
                // want to unnecessarily fire events
                if (newItemIndex >= 0 && (newItemIndex != this.CurrentPosition || !this.IsCurrentInSync))
                {
                    this.OnCurrentChanging();
                    this.SetCurrent(newCurrentItem, newItemIndex);
                }
                return;
            }

            if (this.Count == 1)
            {
                if (this.CurrentItem != null || this.CurrentPosition != -1)
                {
                    // fire current changing notification
                    this.OnCurrentChanging();
                }

                // added first item; set current at BeforeFirst
                this.SetCurrent(null, -1);
            }
            else if (index <= this.CurrentPosition)
            {
                // fire current changing notification
                this.OnCurrentChanging();

                // adjust current index if insertion is earlier
                int newPosition = this.CurrentPosition + 1;
                if (newPosition >= this.Count)
                {
                    // if currency was on last item and it got shifted up,
                    // keep currency on last item.
                    newPosition = this.Count - 1;
                }
                this.SetCurrent(this.GetItemAt(newPosition), newPosition);
            }
        }

        /// <summary>
        /// Fix up CurrentPosition and CurrentItem after a collection change
        /// </summary>
        /// <param name="newCurrentItem">Item that we want to set currency to</param>
        /// <param name="index">Index of item involved in the collection change</param>
        private void AdjustCurrencyForEdit(object newCurrentItem, int index)
        {
            if (newCurrentItem != null && this.IndexOf(newCurrentItem) >= 0)
            {
                this.OnCurrentChanging();
                this.SetCurrent(newCurrentItem, this.IndexOf(newCurrentItem));
                return;
            }

            if (index <= this.CurrentPosition)
            {
                // fire current changing notification
                this.OnCurrentChanging();

                // adjust current index if insertion is earlier
                int newPosition = this.CurrentPosition + 1;
                if (newPosition < this.Count)
                {
                    // CurrentItem might be out of sync if underlying list is not INCC
                    // or if this Add is the result of a Replace (Rem + Add)
                    this.SetCurrent(this.GetItemAt(newPosition), newPosition);
                }
                else
                {
                    this.SetCurrent(null, this.Count);
                }
            }
        }

        /// <summary>
        /// Fix up CurrentPosition and CurrentItem after a collection change
        /// The index can be -1 if the item was removed from a previous page
        /// </summary>
        /// <param name="index">Index of item involved in the collection change</param>
        private void AdjustCurrencyForRemove(int index)
        {
            // adjust current index if deletion is earlier
            if (index < this.CurrentPosition)
            {
                // fire current changing notification
                this.OnCurrentChanging();

                this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1);
            }

            // adjust current index if > Count
            if (this.CurrentPosition >= this.Count)
            {
                // fire current changing notification
                this.OnCurrentChanging();

                this.SetCurrentToPosition(this.Count - 1);
            }

            // make sure that current position and item are in sync
            if (!this.IsCurrentInSync)
            {
                // fire current changing notification
                this.OnCurrentChanging();

                this.SetCurrentToPosition(this.CurrentPosition);
            }
        }

        /// <summary>
        /// Returns true if specified flag in flags is set.
        /// </summary>
        /// <param name="flags">Flag we are checking for</param>
        /// <returns>Whether the specified flag is set</returns>
        private bool CheckFlag(CollectionViewFlags flags)
        {
            return (this._flags & flags) != 0;
        }

        /// <summary>
        /// Called to complete the page move operation to set the
        /// current page index.
        /// </summary>
        /// <param name="pageIndex">Final page index</param>
        private void CompletePageMove(int pageIndex)
        {
            Debug.Assert(this._pageIndex != pageIndex, "Unexpected this._pageIndex == pageIndex");

            // to see whether or not to fire an OnPropertyChanged
            int oldCount = this.Count;
            object oldCurrentItem = this.CurrentItem;
            int oldCurrentPosition = this.CurrentPosition;
            bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

            this._pageIndex = pageIndex;

            // update the groups
            if (this.IsGrouping && this.PageSize > 0)
            {
                this.PrepareGroupsForCurrentPage();
            }

            // update currency
            if (this.Count >= 1)
            {
                this.SetCurrent(this.GetItemAt(0), 0);
            }
            else
            {
                this.SetCurrent(null, -1);
            }

            this.IsPageChanging = false;
            this.OnPropertyChanged("PageIndex");
            this.RaisePageChanged();

            // if the count has changed
            if (this.Count != oldCount)
            {
                this.OnPropertyChanged("Count");
            }

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            // Always raise CurrentChanged since the calling method MoveToPage(pageIndex) raised CurrentChanging.
            this.RaiseCurrencyChanges(true /*fireChangedEvent*/, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
        }

        /// <summary>
        /// Convert a value for the index passed in to the index it would be
        /// relative to the InternalIndex property.
        /// </summary>
        /// <param name="index">Index to convert</param>
        /// <returns>Value for the InternalIndex</returns>
        private int ConvertToInternalIndex(int index)
        {
            Debug.Assert(index > -1, "Unexpected index == -1");
            if (this.PageSize > 0)
            {
                return (this._pageSize * this.PageIndex) + index;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// Copy all items from the source collection to the internal list for processing.
        /// </summary>
        private void CopySourceToInternalList()
        {
            this._internalList = new List<object>();

            IEnumerator enumerator = this.SourceCollection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                this._internalList.Add(enumerator.Current);
            }
        }

        /// <summary>
        /// Common functionality used by CommitNew, CancelNew, and when the
        /// new item is removed by Remove or Refresh.
        /// </summary>
        /// <param name="cancel">Whether we canceled the add</param>
        /// <returns>The new item we ended adding</returns>
        private object EndAddNew(bool cancel)
        {
            object newItem = this.CurrentAddItem;

            this.CurrentAddItem = null;    // leave "adding-new" mode

            global::System.ComponentModel.IEditableObject ieo = newItem as global::System.ComponentModel.IEditableObject;
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

            return newItem;
        }

        /// <summary>
        /// Subtracts from the deferLevel counter and calls Refresh() if there are no other defers
        /// </summary>
        private void EndDefer()
        {
            --this._deferLevel;

            if (this._deferLevel == 0)
            {
                if (this.CheckFlag(CollectionViewFlags.IsUpdatePageSizeDeferred))
                {
                    this.SetFlag(CollectionViewFlags.IsUpdatePageSizeDeferred, false);
                    this.PageSize = this._cachedPageSize;
                }

                if (this.CheckFlag(CollectionViewFlags.IsMoveToPageDeferred))
                {
                    this.SetFlag(CollectionViewFlags.IsMoveToPageDeferred, false);
                    this.MoveToPage(this._cachedPageIndex);
                    this._cachedPageIndex = -1;
                }

                if (this.CheckFlag(CollectionViewFlags.NeedsRefresh))
                {
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Makes sure that the ItemConstructor is set for the correct type
        /// </summary>
        private void EnsureItemConstructor()
        {
            if (!this._itemConstructorIsValid)
            {
                Type itemType = this.GetItemType(true);
                if (itemType != null)
                {
                    this._itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
                    this._itemConstructorIsValid = true;
                }
            }
        }

        /// <summary>
        ///  If the IEnumerable has changed, bring the collection up to date.
        ///  (This isn't necessary if the IEnumerable is also INotifyCollectionChanged
        ///  because we keep the collection in sync incrementally.)
        /// </summary>
        private void EnsureCollectionInSync()
        {
            // if the IEnumerable is not a INotifyCollectionChanged
            if (this._pollForChanges)
            {
                try
                {
                    this._trackingEnumerator.MoveNext();
                }
                catch (InvalidOperationException)
                {
                    // When the collection has been modified, calling MoveNext()
                    // on the enumerator throws an InvalidOperationException, stating
                    // that the collection has been modified. Therefore, we know when
                    // to update our internal collection.
                    this._trackingEnumerator = this.SourceCollection.GetEnumerator();
                    this.RefreshOrDefer();
                }
            }
        }

        /// <summary>
        /// Helper function used to determine the type of an item
        /// </summary>
        /// <param name="useRepresentativeItem">Whether we should use a representative item</param>
        /// <returns>The type of the items in the collection</returns>
        private Type GetItemType(bool useRepresentativeItem)
        {
            Type collectionType = this.SourceCollection.GetType();
            Type[] interfaces = collectionType.GetInterfaces();

            // Look for IEnumerable<T>.  All generic collections should implement
            // this.  We loop through the interface list, rather than call
            // GetInterface(IEnumerableT), so that we handle an ambiguous match
            // (by using the first match) without an exception.
            for (int i = 0; i < interfaces.Length; ++i)
            {
                Type interfaceType = interfaces[i];
                if (interfaceType.Name == typeof(IEnumerable<>).Name)
                {
                    // found IEnumerable<>, extract T
                    Type[] typeParameters = interfaceType.GetGenericArguments();
                    if (typeParameters.Length == 1)
                    {
                        return typeParameters[0];
                    }
                }
            }

            // No generic information found.  Use a representative item instead.
            if (useRepresentativeItem)
            {
                // get type of a representative item
                object item = this.GetRepresentativeItem();
                if (item != null)
                {
                    return item.GetType();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a representative item from the collection
        /// </summary>
        /// <returns>An item that can represent the collection</returns>
        private object GetRepresentativeItem()
        {
            if (this.IsEmpty)
            {
                return null;
            }

            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object item = enumerator.Current;
                // Since this collection view does not support a NewItemPlaceholder, 
                // simply return the first non-null item.
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Return index of item in the internal list.
        /// </summary>
        /// <param name="item">The item we are checking</param>
        /// <returns>Integer value on where in the InternalList the object is located</returns>
        private int InternalIndexOf(object item)
        {
            return this.InternalList.IndexOf(item);
        }

        /// <summary>
        /// Return item at the given index in the internal list.
        /// </summary>
        /// <param name="index">The index we are checking</param>
        /// <returns>The item at the specified index</returns>
        private object InternalItemAt(int index)
        {
            if (index >= 0 && index < this.InternalList.Count)
            {
                return this.InternalList[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Ask listeners (via ICollectionView.CurrentChanging event) if it's OK to change currency
        /// </summary>
        /// <returns>False if a listener cancels the change, True otherwise</returns>
        private bool OkToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            this.OnCurrentChanging(args);
            return !args.Cancel;
        }

        /// <summary>
        ///     Notify listeners that this View has changed
        /// </summary>
        /// <remarks>
        ///     CollectionViews (and sub-classes) should take their filter/sort/grouping/paging
        ///     into account before calling this method to forward CollectionChanged events.
        /// </remarks>
        /// <param name="args">
        ///     The NotifyCollectionChangedEventArgs to be passed to the EventHandler
        /// </param>
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            unchecked
            {
                // invalidate enumerators because of a change
                ++this._timestamp;
            }

            if (this.CollectionChanged != null)
            {
                if (args.Action != NotifyCollectionChangedAction.Add || this.PageSize == 0 || args.NewStartingIndex < this.Count)
                {
                    this.CollectionChanged(this, args);
                }
            }

            // Collection changes change the count unless an item is being
            // replaced within the collection.
            if (args.Action != NotifyCollectionChangedAction.Replace)
            {
                this.OnPropertyChanged("Count");
            }

            bool listIsEmpty = this.IsEmpty;
            if (listIsEmpty != this.CheckFlag(CollectionViewFlags.CachedIsEmpty))
            {
                this.SetFlag(CollectionViewFlags.CachedIsEmpty, listIsEmpty);
                this.OnPropertyChanged("IsEmpty");
            }
        }

        /// <summary>
        /// Raises the CurrentChanged event
        /// </summary>
        private void OnCurrentChanged()
        {
            if (this.CurrentChanged != null && this._currentChangedMonitor.Enter())
            {
                using (this._currentChangedMonitor)
                {
                    this.CurrentChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raise a CurrentChanging event that is not cancelable.
        /// This is called by CollectionChanges (Add, Remove, and Refresh) that 
        /// affect the CurrentItem.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This CurrentChanging event cannot be canceled.
        /// </exception>
        private void OnCurrentChanging()
        {
            this.OnCurrentChanging(uncancelableCurrentChangingEventArgs);
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
        private void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (this._currentChangedMonitor.Busy)
            {
                if (args.IsCancelable)
                {
                    args.Cancel = true;
                }

                return;
            }

            if (this.CurrentChanging != null)
            {
                this.CurrentChanging(this, args);
            }
        }

        /// <summary>
        /// GroupBy changed handler
        /// </summary>
        /// <param name="sender">CollectionViewGroup whose GroupBy has changed</param>
        /// <param name="e">Arguments for the NotifyCollectionChanged event</param>
        private void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Grouping"));
            }

            this.RefreshOrDefer();
        }

        /// <summary>
        /// GroupDescription changed handler
        /// </summary>
        /// <param name="sender">CollectionViewGroup whose GroupDescription has changed</param>
        /// <param name="e">Arguments for the GroupDescriptionChanged event</param>
        private void OnGroupDescriptionChanged(object sender, EventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Grouping"));
            }

            // we want to make sure that the data is refreshed before we try to move to a page
            // since the refresh would take care of the filtering, sorting, and grouping.
            this.RefreshOrDefer();

            if (this.PageSize > 0)
            {
                if (this.IsRefreshDeferred)
                {
                    // set cached value and flag so that we move to first page on EndDefer
                    this._cachedPageIndex = 0;
                    this.SetFlag(CollectionViewFlags.IsMoveToPageDeferred, true);
                }
                else
                {
                    this.MoveToFirstPage();
                }
            }
        }

        /// <summary>
        /// Raises a PropertyChanged event.
        /// </summary>
        /// <param name="e">PropertyChangedEventArgs for this change</param>
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property name for the property that changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets up the ActiveComparer for the CollectionViewGroupRoot specified
        /// </summary>
        /// <param name="groupRoot">The CollectionViewGroupRoot</param>
        private void PrepareGroupingComparer(CollectionViewGroupRoot groupRoot)
        {
            if (groupRoot == this._temporaryGroup || this.PageSize == 0)
            {
                CollectionViewGroupInternal.ListComparer listComparer = groupRoot.ActiveComparer as CollectionViewGroupInternal.ListComparer;
                if (listComparer != null)
                {
                    listComparer.ResetList(this.InternalList);
                }
                else
                {
                    groupRoot.ActiveComparer = new CollectionViewGroupInternal.ListComparer(this.InternalList);
                }
            }
            else if (groupRoot == this._group)
            {
                // create the new comparer based on the current _temporaryGroup
                groupRoot.ActiveComparer = new CollectionViewGroupInternal.CollectionViewGroupComparer(this._temporaryGroup);
            }
        }

        /// <summary>
        /// Use the GroupDescriptions to place items into their respective groups.
        /// This assumes that there is no paging, so we just group the entire collection
        /// of items that the CollectionView holds.
        /// </summary>
        private void PrepareGroups()
        {
            // we should only use this method if we aren't paging
            Debug.Assert(this.PageSize == 0, "Unexpected PageSize != 0");

            this._group.Clear();
            this._group.Initialize();

            this._group.IsDataInGroupOrder = this.CheckFlag(CollectionViewFlags.IsDataInGroupOrder);

            // set to false so that we access internal collection items
            // instead of the group items, as they have been cleared
            this._isGrouping = false;

            if (this._group.GroupDescriptions.Count > 0)
            {
                for (int num = 0, count = this._internalList.Count; num < count; ++num)
                {
                    object item = this._internalList[num];
                    if (item != null && (!this.IsAddingNew || !object.Equals(this.CurrentAddItem, item)))
                    {
                        this._group.AddToSubgroups(item, true /*loading*/);
                    }
                }
                if (this.IsAddingNew)
                {
                    this._group.InsertSpecialItem(this._group.Items.Count, this.CurrentAddItem, true);
                }
            }

            this._isGrouping = this._group.GroupBy != null;

            // now we set the value to false, so that subsequent adds will insert
            // into the correct groups.
            this._group.IsDataInGroupOrder = false;

            // reset the grouping comparer
            this.PrepareGroupingComparer(this._group);
        }

        /// <summary>
        /// Use the GroupDescriptions to place items into their respective groups.
        /// Because of the fact that we have paging, it is possible that we are only
        /// going to need a subset of the items to be displayed. However, before we 
        /// actually group the entire collection, we can't display the items in the
        /// correct order. We therefore want to just create a temporary group with
        /// the entire collection, and then using this data we can create the group
        /// that is exposed with just the items we need.
        /// </summary>
        private void PrepareTemporaryGroups()
        {
            this._temporaryGroup = new CollectionViewGroupRoot(this, this.CheckFlag(CollectionViewFlags.IsDataInGroupOrder));

            foreach (GroupDescription gd in this._group.GroupDescriptions)
            {
                this._temporaryGroup.GroupDescriptions.Add(gd);
            }

            this._temporaryGroup.Initialize();

            // set to false so that we access internal collection items
            // instead of the group items, as they have been cleared
            this._isGrouping = false;

            if (this._temporaryGroup.GroupDescriptions.Count > 0)
            {
                for (int num = 0, count = this._internalList.Count; num < count; ++num)
                {
                    object item = this._internalList[num];
                    if (item != null && (!this.IsAddingNew || !object.Equals(this.CurrentAddItem, item)))
                    {
                        this._temporaryGroup.AddToSubgroups(item, true /*loading*/);
                    }
                }
                if (this.IsAddingNew)
                {
                    this._temporaryGroup.InsertSpecialItem(this._temporaryGroup.Items.Count, this.CurrentAddItem, true);
                }
            }

            this._isGrouping = this._temporaryGroup.GroupBy != null;

            // reset the grouping comparer
            this.PrepareGroupingComparer(this._temporaryGroup);
        }

        /// <summary>
        /// Update our Groups private accessor to point to the subset of data
        /// covered by the current page, or to display the entire group if paging is not
        /// being used.
        /// </summary>
        private void PrepareGroupsForCurrentPage()
        {
            this._group.Clear();
            this._group.Initialize();

            // set to indicate that we will be pulling data from the temporary group data
            this._isUsingTemporaryGroup = true;

            // since we are getting our data from the temporary group, it should
            // already be in group order
            this._group.IsDataInGroupOrder = true;
            this._group.ActiveComparer = null;

            if (this.GroupDescriptions.Count > 0)
            {
                for (int num = 0, count = this.Count; num < count; ++num)
                {
                    object item = this.GetItemAt(num);
                    if (item != null && (!this.IsAddingNew || !object.Equals(this.CurrentAddItem, item)))
                    {
                        this._group.AddToSubgroups(item, true /*loading*/);
                    }
                }
                if (this.IsAddingNew)
                {
                    this._group.InsertSpecialItem(this._group.Items.Count, this.CurrentAddItem, true);
                }
            }

            // set flag to indicate that we do not need to access the temporary data any longer
            this._isUsingTemporaryGroup = false;

            // now we set the value to false, so that subsequent adds will insert
            // into the correct groups.
            this._group.IsDataInGroupOrder = false;

            // reset the grouping comparer
            this.PrepareGroupingComparer(this._group);

            this._isGrouping = this._group.GroupBy != null;
        }

        /// <summary>
        /// Create, filter and sort the local index array.
        /// called from Refresh(), override in derived classes as needed.
        /// </summary>
        /// <param name="enumerable">new IEnumerable to associate this view with</param>
        /// <returns>new local array to use for this view</returns>
        private IList PrepareLocalArray(IEnumerable enumerable)
        {
            Debug.Assert(enumerable != null, "Input list to filter/sort should not be null");

            // filter the collection's array into the local array
            List<object> localList = new List<object>();

            foreach (object item in enumerable)
            {
                if (this.Filter == null || this.PassesFilter(item))
                {
                    localList.Add(item);
                }
            }

            // sort the local array
            if (!this.CheckFlag(CollectionViewFlags.IsDataSorted) && this.SortDescriptions.Count > 0)
            {
                localList = this.SortList(localList);
            }

            return localList;
        }

        /// <summary>
        /// Process an Add operation from an INotifyCollectionChanged event handler.
        /// </summary>
        /// <param name="addedItem">Item added to the source collection</param>
        /// <param name="addIndex">Index item was added into</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Handles multiple input types and scenarios")]
        private void ProcessAddEvent(object addedItem, int addIndex)
        {
            // item to fire remove notification for if necessary
            object removeNotificationItem = null;
            if (this.PageSize > 0 && !this.IsGrouping)
            {
                removeNotificationItem = (this.Count == this.PageSize) ?
                    this.GetItemAt(this.PageSize - 1) : null;
            }

            // process the add by filtering and sorting the item
            this.ProcessInsertToCollection(
                addedItem,
                addIndex);

            // next check if we need to add an item into the current group
            bool needsGrouping = false;
            if (this.Count == 1 && this.GroupDescriptions.Count > 0)
            {
                // if this is the first item being added
                // we want to setup the groups with the
                // correct element type comparer
                if (this.PageSize > 0)
                {
                    this.PrepareGroupingComparer(this._temporaryGroup);
                }
                this.PrepareGroupingComparer(this._group);
            }

            if (this.IsGrouping)
            {
                int leafIndex = -1;

                if (this.PageSize > 0)
                {
                    this._temporaryGroup.AddToSubgroups(addedItem, false /*loading*/);
                    leafIndex = this._temporaryGroup.LeafIndexOf(addedItem);
                }

                // if we are not paging, we should just be able to add the item.
                // otherwise, we need to validate that it is within the current page.
                if (this.PageSize == 0 || (this.PageIndex + 1) * this.PageSize > leafIndex)
                {
                    needsGrouping = true;

                    int pageStartIndex = this.PageIndex * this.PageSize;

                    // if the item was inserted on a previous page
                    if (pageStartIndex > leafIndex && this.PageSize > 0)
                    {
                        addedItem = this._temporaryGroup.LeafAt(pageStartIndex);
                    }

                    // if we're grouping and have more items than the 
                    // PageSize will allow, remove the last item
                    if (this.PageSize > 0 && this._group.ItemCount == this.PageSize)
                    {
                        removeNotificationItem = this._group.LeafAt(this.PageSize - 1);
                        this._group.RemoveFromSubgroups(removeNotificationItem);
                    }
                }
            }

            // if we are paging, we may have to fire another notification for the item
            // that needs to be removed for the one we added on this page.
            if (this.PageSize > 0 && !this.OnLastLocalPage &&
               (((this.IsGrouping && removeNotificationItem != null) ||
               (!this.IsGrouping && (this.PageIndex + 1) * this.PageSize > this.InternalIndexOf(addedItem)))))
            {
                if (removeNotificationItem != null && removeNotificationItem != addedItem)
                {
                    this.AdjustCurrencyForRemove(this.PageSize - 1);

                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            removeNotificationItem,
                            this.PageSize - 1));
                }
            }

            // if we need to add the item into the current group
            // that will be displayed
            if (needsGrouping)
            {
                this._group.AddToSubgroups(addedItem, false /*loading*/);
            }

            int addedIndex = this.IndexOf(addedItem);

            // if the item is within the current page
            if (addedIndex >= 0)
            {
                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                this.AdjustCurrencyForAdd(null, addedIndex);

                // fire add notification
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        addedItem,
                        addedIndex));

                this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
            }
            else if (this.PageSize > 0)
            {
                // otherwise if the item was added into a previous page
                int internalIndex = this.IsGrouping ? this._group.LeafIndexOf(addedItem) : this.InternalIndexOf(addedItem);
                if (internalIndex < this.ConvertToInternalIndex(0))
                {
                    // fire add notification for item pushed in
                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            this.GetItemAt(0),
                            0));
                }
            }
        }

        /// <summary>
        /// Process CollectionChanged event on source collection 
        /// that implements INotifyCollectionChanged.
        /// </summary>
        /// <param name="args">
        /// The NotifyCollectionChangedEventArgs to be processed.
        /// </param>
        private void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            // if we do not want to handle the CollectionChanged event, return
            if (!this.CheckFlag(CollectionViewFlags.ShouldProcessCollectionChanged))
            {
                return;
            }

            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                // if we have no items now, clear our own internal list
                if (!this.SourceCollection.GetEnumerator().MoveNext())
                {
                    this._internalList.Clear();
                }

                // calling Refresh, will fire the collectionchanged event
                this.RefreshOrDefer();
                return;
            }

            object addedItem = (args.NewItems != null) ? args.NewItems[0] : null;
            object removedItem = (args.OldItems != null) ? args.OldItems[0] : null;

            // fire notifications for removes
            if (args.Action == NotifyCollectionChangedAction.Remove ||
                args.Action == NotifyCollectionChangedAction.Replace)
            {
                this.ProcessRemoveEvent(removedItem, args.Action == NotifyCollectionChangedAction.Replace);
            }

            // fire notifications for adds
            if ((args.Action == NotifyCollectionChangedAction.Add ||
                args.Action == NotifyCollectionChangedAction.Replace) &&
                (this.Filter == null || this.PassesFilter(addedItem)))
            {
                this.ProcessAddEvent(addedItem, args.NewStartingIndex);
            }
            if (args.Action != NotifyCollectionChangedAction.Replace)
            {
                this.OnPropertyChanged("ItemCount");
            }
        }

        /// <summary>
        /// Process a Remove operation from an INotifyCollectionChanged event handler.
        /// </summary>
        /// <param name="removedItem">Item removed from the source collection</param>
        /// <param name="isReplace">Whether this was part of a Replace operation</param>
        private void ProcessRemoveEvent(object removedItem, bool isReplace)
        {
            int internalRemoveIndex = -1;

            if (this.IsGrouping)
            {
                internalRemoveIndex = this.PageSize > 0 ? this._temporaryGroup.LeafIndexOf(removedItem) :
                    this._group.LeafIndexOf(removedItem);
            }
            else
            {
                internalRemoveIndex = this.InternalIndexOf(removedItem);
            }

            int removeIndex = this.IndexOf(removedItem);

            // remove the item from the collection
            this._internalList.Remove(removedItem);

            // only fire the remove if it was removed from either the current page, or a previous page
            bool needToRemove = (this.PageSize == 0 && removeIndex >= 0) || (internalRemoveIndex < (this.PageIndex + 1) * this.PageSize);

            if (this.IsGrouping)
            {
                if (this.PageSize > 0)
                {
                    this._temporaryGroup.RemoveFromSubgroups(removedItem);
                }

                if (needToRemove)
                {
                    this._group.RemoveFromSubgroups(removeIndex >= 0 ? removedItem : this._group.LeafAt(0));
                }
            }

            if (needToRemove)
            {
                object oldCurrentItem = this.CurrentItem;
                int oldCurrentPosition = this.CurrentPosition;
                bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
                bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

                this.AdjustCurrencyForRemove(removeIndex);

                // fire remove notification 
                // if we removed from current page, remove from removeIndex,
                // if we removed from previous page, remove first item (index=0)
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        removedItem,
                        Math.Max(0, removeIndex)));

                this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

                // if we removed all items from the current page,
                // move to the previous page. we do not need to 
                // fire additional notifications, as moving the page will
                // trigger a reset.
                if (this.NeedToMoveToPreviousPage && !isReplace)
                {
                    this.MoveToPreviousPage();
                    return;
                }

                // if we are paging, we may have to fire another notification for the item
                // that needs to replace the one we removed on this page.
                if (this.PageSize > 0 && this.Count == this.PageSize)
                {
                    // we first need to add the item into the current group
                    if (this.IsGrouping)
                    {
                        object newItem = this._temporaryGroup.LeafAt((this.PageSize * (this.PageIndex + 1)) - 1);
                        if (newItem != null)
                        {
                            this._group.AddToSubgroups(newItem, false /*loading*/);
                        }
                    }

                    // fire the add notification
                    this.OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            this.GetItemAt(this.PageSize - 1),
                            this.PageSize - 1));
                }
            }
        }

        /// <summary>
        /// Handles adding an item into the collection, and applying sorting, filtering, grouping, paging.
        /// </summary>
        /// <param name="item">Item to insert in the collection</param>
        /// <param name="index">Index to insert item into</param>
        private void ProcessInsertToCollection(object item, int index)
        {
            // first check to see if it passes the filter
            if (this.Filter == null || this.PassesFilter(item))
            {
                if (this.SortDescriptions.Count > 0)
                {
                    // create the SortFieldComparer to use
                    SortFieldComparer sortFieldComparer = new SortFieldComparer(this);

                    // check if the item would be in sorted order if inserted into the specified index
                    // otherwise, calculate the correct sorted index
                    if (index < 0 || /* if item was not originally part of list */
                        (index > 0 && (sortFieldComparer.Compare(item, this.InternalItemAt(index - 1)) < 0)) || /* item has moved up in the list */
                        ((index < this.InternalList.Count - 1) && (sortFieldComparer.Compare(item, this.InternalItemAt(index)) > 0))) /* item has moved down in the list */
                    {
                        index = sortFieldComparer.FindInsertIndex(item, this._internalList);
                    }
                }

                // make sure that the specified insert index is within the valid range
                // otherwise, just add it to the end. the index can be set to an invalid
                // value if the item was originally not in the collection, on a different
                // page, or if it had been previously filtered out.
                if (index < 0 || index > this._internalList.Count)
                {
                    index = this._internalList.Count;
                }

                this._internalList.Insert(index, item);
            }
        }

        /// <summary>
        /// Raises Currency Change events
        /// </summary>
        /// <param name="fireChangedEvent">Whether to fire the CurrentChanged event even if the parameters have not changed</param>
        /// <param name="oldCurrentItem">CurrentItem before processing changes</param>
        /// <param name="oldCurrentPosition">CurrentPosition before processing changes</param>
        /// <param name="oldIsCurrentBeforeFirst">IsCurrentBeforeFirst before processing changes</param>
        /// <param name="oldIsCurrentAfterLast">IsCurrentAfterLast before processing changes</param>
        private void RaiseCurrencyChanges(bool fireChangedEvent, object oldCurrentItem, int oldCurrentPosition, bool oldIsCurrentBeforeFirst, bool oldIsCurrentAfterLast)
        {
            // fire events for currency changes
            if (fireChangedEvent || this.CurrentItem != oldCurrentItem || this.CurrentPosition != oldCurrentPosition)
            {
                this.OnCurrentChanged();
            }
            if (this.CurrentItem != oldCurrentItem)
            {
                this.OnPropertyChanged("CurrentItem");
            }
            if (this.CurrentPosition != oldCurrentPosition)
            {
                this.OnPropertyChanged("CurrentPosition");
            }
            if (this.IsCurrentAfterLast != oldIsCurrentAfterLast)
            {
                this.OnPropertyChanged("IsCurrentAfterLast");
            }
            if (this.IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
            {
                this.OnPropertyChanged("IsCurrentBeforeFirst");
            }
        }

        /// <summary>
        /// Raises the PageChanged event
        /// </summary>
        private void RaisePageChanged()
        {
            EventHandler<EventArgs> handler = this.PageChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the PageChanging event
        /// </summary>
        /// <param name="newPageIndex">Index of the requested page</param>
        /// <returns>True if the event is cancelled (e.Cancel was set to True), False otherwise</returns>
        private bool RaisePageChanging(int newPageIndex)
        {
            EventHandler<PageChangingEventArgs> handler = this.PageChanging;
            if (handler != null)
            {
                PageChangingEventArgs pageChangingEventArgs = new PageChangingEventArgs(newPageIndex);
                handler(this, pageChangingEventArgs);
                return pageChangingEventArgs.Cancel;
            }

            return false;
        }

        /// <summary>
        /// Will call RefreshOverride and clear the NeedsRefresh flag
        /// </summary>
        private void RefreshInternal()
        {
            this.RefreshOverride();
            this.SetFlag(CollectionViewFlags.NeedsRefresh, false);
        }

        /// <summary>
        /// Refresh, or mark that refresh is needed when defer cycle completes.
        /// </summary>
        private void RefreshOrDefer()
        {
            if (this.IsRefreshDeferred)
            {
                this.SetFlag(CollectionViewFlags.NeedsRefresh, true);
            }
            else
            {
                this.RefreshInternal();
            }
        }

        /// <summary>
        /// Re-create the view, using any SortDescriptions. 
        /// Also updates currency information.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Handles multiple input types and scenarios")]
        private void RefreshOverride()
        {
            object oldCurrentItem = this.CurrentItem;
            int oldCurrentPosition = this.CurrentPosition;
            bool oldIsCurrentAfterLast = this.IsCurrentAfterLast;
            bool oldIsCurrentBeforeFirst = this.IsCurrentBeforeFirst;

            // set IsGrouping to false
            this._isGrouping = false;

            // force currency off the collection (gives user a chance to save dirty information)
            this.OnCurrentChanging();

            // if there's no sort/filter/paging/grouping, just use the collection's array
            if (this.UsesLocalArray)
            {
                try
                {
                    // apply filtering/sorting through the PrepareLocalArray method
                    this._internalList = (IList)this.PrepareLocalArray(this._sourceCollection);

                    // apply grouping
                    if (this.PageSize == 0)
                    {
                        this.PrepareGroups();
                    }
                    else
                    {
                        this.PrepareTemporaryGroups();
                        this.PrepareGroupsForCurrentPage();
                    }
                }
                catch (TargetInvocationException e)
                {
                    // If there's an exception while invoking PrepareLocalArray,
                    // we want to unwrap it and throw its inner exception
                    if (e.InnerException != null)
                    {
                        throw e.InnerException;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                this.CopySourceToInternalList();
            }

            // check if PageIndex is still valid after filter/sort
            if (this.PageSize > 0 &&
                this.PageIndex > 0 &&
                this.PageIndex >= this.PageCount)
            {
                this.MoveToPage(this.PageCount - 1);
            }

            // reset currency values
            this.ResetCurrencyValues(oldCurrentItem, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            // now raise currency changes at the end
            this.RaiseCurrencyChanges(false, oldCurrentItem, oldCurrentPosition, oldIsCurrentBeforeFirst, oldIsCurrentAfterLast);
        }

        /// <summary>
        /// Set currency back to the previous value it had if possible. If the item is no longer in view
        /// then either use the first item in the view, or if the list is empty, use null.
        /// </summary>
        /// <param name="oldCurrentItem">CurrentItem before processing changes</param>
        /// <param name="oldIsCurrentBeforeFirst">IsCurrentBeforeFirst before processing changes</param>
        /// <param name="oldIsCurrentAfterLast">IsCurrentAfterLast before processing changes</param>
        private void ResetCurrencyValues(object oldCurrentItem, bool oldIsCurrentBeforeFirst, bool oldIsCurrentAfterLast)
        {
            if (oldIsCurrentBeforeFirst || this.IsEmpty)
            {
                this.SetCurrent(null, -1);
            }
            else if (oldIsCurrentAfterLast)
            {
                this.SetCurrent(null, this.Count);
            }
            else
            {
                // try to set currency back to old current item
                // if there are duplicates, use the position of the first matching item
                int newPosition = this.IndexOf(oldCurrentItem);

                // if the old current item is no longer in view
                if (newPosition < 0)
                {
                    // if we are adding a new item, set it as the current item, otherwise, set it to null
                    newPosition = 0;

                    if (newPosition < this.Count)
                    {
                        this.SetCurrent(this.GetItemAt(newPosition), newPosition);
                    }
                    else if (!this.IsEmpty)
                    {
                        this.SetCurrent(this.GetItemAt(0), 0);
                    }
                    else
                    {
                        this.SetCurrent(null, -1);
                    }
                }
                else
                {
                    this.SetCurrent(oldCurrentItem, newPosition);
                }
            }
        }

        /// <summary>
        /// Set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// CollectionViews (and sub-classes) should use this method to update
        /// the Current values.
        /// </remarks>
        /// <param name="newItem">New CurrentItem</param>
        /// <param name="newPosition">New CurrentPosition</param>
        private void SetCurrent(object newItem, int newPosition)
        {
            int count = (newItem != null) ? 0 : (this.IsEmpty ? 0 : this.Count);
            this.SetCurrent(newItem, newPosition, count);
        }

        /// <summary>
        /// Set CurrentItem and CurrentPosition, no questions asked!
        /// </summary>
        /// <remarks>
        /// This method can be called from a constructor - it does not call
        /// any virtuals.  The 'count' parameter is substitute for the real Count,
        /// used only when newItem is null.
        /// In that case, this method sets IsCurrentAfterLast to true if and only
        /// if newPosition >= count.  This distinguishes between a null belonging
        /// to the view and the dummy null when CurrentPosition is past the end.
        /// </remarks>
        /// <param name="newItem">New CurrentItem</param>
        /// <param name="newPosition">New CurrentPosition</param>
        /// <param name="count">Numbers of items in the collection</param>
        private void SetCurrent(object newItem, int newPosition, int count)
        {
            if (newItem != null)
            {
                // non-null item implies position is within range.
                // We ignore count - it's just a placeholder
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, false);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                // empty collection - by convention both flags are true and position is -1
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                // null item, possibly within range.
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }

            this._currentItem = newItem;
            this._currentPosition = newPosition;
        }

        /// <summary>
        /// Just move it. No argument check, no events, just move current to position.
        /// </summary>
        /// <param name="position">Position to move the current item to</param>
        private void SetCurrentToPosition(int position)
        {
            if (position < 0)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetCurrent(null, -1);
            }
            else if (position >= this.Count)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                this.SetCurrent(null, this.Count);
            }
            else
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst | CollectionViewFlags.IsCurrentAfterLast, false);
                this.SetCurrent(this.GetItemAt(position), position);
            }
        }

        /// <summary>
        /// Sets the specified Flag(s)
        /// </summary>
        /// <param name="flags">Flags we want to set</param>
        /// <param name="value">Value we want to set these flags to</param>
        private void SetFlag(CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                this._flags = this._flags | flags;
            }
            else
            {
                this._flags = this._flags & ~flags;
            }
        }

        /// <summary>
        /// Set new SortDescription collection; re-hook collection change notification handler
        /// </summary>
        /// <param name="descriptions">SortDescriptionCollection to set the property value to</param>
        private void SetSortDescriptions(SortDescriptionCollection descriptions)
        {
            if (this._sortDescriptions != null)
            {
                ((INotifyCollectionChanged)this._sortDescriptions).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
            }

            this._sortDescriptions = descriptions;

            if (this._sortDescriptions != null)
            {
                Debug.Assert(this._sortDescriptions.Count == 0, "must be empty SortDescription collection");
                ((INotifyCollectionChanged)this._sortDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
            }
        }

        /// <summary>
        /// SortDescription was added/removed, refresh PagedCollectionView
        /// </summary>
        /// <param name="sender">Sender that triggered this handler</param>
        /// <param name="e">NotifyCollectionChangedEventArgs for this change</param>
        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Sorting"));
            }

            // we want to make sure that the data is refreshed before we try to move to a page
            // since the refresh would take care of the filtering, sorting, and grouping.
            this.RefreshOrDefer();

            if (this.PageSize > 0)
            {
                if (this.IsRefreshDeferred)
                {
                    // set cached value and flag so that we move to first page on EndDefer
                    this._cachedPageIndex = 0;
                    this.SetFlag(CollectionViewFlags.IsMoveToPageDeferred, true);
                }
                else
                {
                    this.MoveToFirstPage();
                }
            }

            this.OnPropertyChanged("SortDescriptions");
        }

        /// <summary>
        /// Sort the List based on the SortDescriptions property.
        /// </summary>
        /// <param name="list">List of objects to sort</param>
        /// <returns>The sorted list</returns>
        private List<object> SortList(List<object> list)
        {
            Debug.Assert(list != null, "Input list to sort should not be null");

            IEnumerable<object> seq = (IEnumerable<object>)list;
            IComparer<object> comparer = new CultureSensitiveComparer(this.Culture);

            foreach (SortDescription sort in this.SortDescriptions)
            {
                string propertyPath = sort.PropertyName;
                Type propertyType = null;

                // find the propertyType from the first non-null instance
                // in the list
                foreach (object item in list)
                {
                    if (item != null)
                    {
                        propertyType = item.GetType().GetNestedPropertyType(propertyPath);
                        break;
                    }
                }

                IOrderedEnumerable<object> orderedEnum = seq as IOrderedEnumerable<object>;

                switch (sort.Direction)
                {
                    case global::System.ComponentModel.ListSortDirection.Ascending:
                        if (orderedEnum != null)
                        {
                            // thenby
                            seq = orderedEnum.ThenBy(item => InvokePath(item, propertyPath, propertyType), comparer);
                        }
                        else
                        {
                            // orderby
                            seq = seq.OrderBy(item => InvokePath(item, propertyPath, propertyType), comparer);
                        }

                        break;
                    case global::System.ComponentModel.ListSortDirection.Descending:
                        if (orderedEnum != null)
                        {
                            // thenby
                            seq = orderedEnum.ThenByDescending(item => InvokePath(item, propertyPath, propertyType), comparer);
                        }
                        else
                        {
                            // orderby
                            seq = seq.OrderByDescending(item => InvokePath(item, propertyPath, propertyType), comparer);
                        }

                        break;
                    default:
                        break;
                }
            }

            return seq.ToList();
        }

        /// <summary>
        /// Helper to validate that we are not in the middle of a DeferRefresh
        /// and throw if that is the case.
        /// </summary>
        private void VerifyRefreshNotDeferred()
        {
            // If the Refresh is being deferred to change filtering or sorting of the
            // data by this PagedCollectionView, then PagedCollectionView will not reflect the correct
            // state of the underlying data.
            if (this.IsRefreshDeferred)
            {
                throw new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred);
            }
        }

#endregion Private Methods

        ////------------------------------------------------------
        ////
        ////  Private Classes
        ////
        ////------------------------------------------------------

#region Private Classes

        /// <summary>
        /// Used to keep track of Defer calls on the PagedCollectionView, which
        /// will prevent the user from calling Refresh() on the view. In order
        /// to allow refreshes again, the user will have to call IDisposable.Dispose,
        /// to end the Defer operation.
        /// </summary>
        private class DeferHelper : IDisposable
        {
            /// <summary>
            /// Private reference to the CollectionView that created this DeferHelper
            /// </summary>
            private PagedCollectionView collectionView;

            /// <summary>
            /// Initializes a new instance of the DeferHelper class
            /// </summary>
            /// <param name="collectionView">CollectionView that created this DeferHelper</param>
            public DeferHelper(PagedCollectionView collectionView)
            {
                this.collectionView = collectionView;
            }

            /// <summary>
            /// Cleanup method called when done using this class
            /// </summary>
            public void Dispose()
            {
                if (this.collectionView != null)
                {
                    this.collectionView.EndDefer();
                    this.collectionView = null;
                }
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// A simple monitor class to help prevent re-entrant calls
        /// </summary>
        private class SimpleMonitor : IDisposable
        {
            /// <summary>
            /// Whether the monitor is entered
            /// </summary>
            private bool entered;

            /// <summary>
            /// Gets a value indicating whether we have been entered or not
            /// </summary>
            public bool Busy
            {
                get { return this.entered; }
            }

            /// <summary>
            /// Sets a value indicating that we have been entered
            /// </summary>
            /// <returns>Boolean value indicating whether we were already entered</returns>
            public bool Enter()
            {
                if (this.entered)
                {
                    return false;
                }

                this.entered = true;
                return true;
            }

            /// <summary>
            /// Cleanup method called when done using this class
            /// </summary>
            public void Dispose()
            {
                this.entered = false;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// IEnumerator generated using the new item taken into account
        /// </summary>
        private class NewItemAwareEnumerator : IEnumerator
        {
            private enum Position
            {
                /// <summary>
                /// Whether the position is before the new item
                /// </summary>
                BeforeNewItem,

                /// <summary>
                /// Whether the position is on the new item that is being created
                /// </summary>
                OnNewItem,

                /// <summary>
                /// Whether the position is after the new item
                /// </summary>
                AfterNewItem
            }

            /// <summary>
            /// Initializes a new instance of the NewItemAwareEnumerator class.
            /// </summary>
            /// <param name="collectionView">The PagedCollectionView we are creating the enumerator for</param>
            /// <param name="baseEnumerator">The baseEnumerator that we pass in</param>
            /// <param name="newItem">The new item we are adding to the collection</param>
            public NewItemAwareEnumerator(PagedCollectionView collectionView, IEnumerator baseEnumerator, object newItem)
            {
                this._collectionView = collectionView;
                this._timestamp = collectionView.Timestamp;
                this._baseEnumerator = baseEnumerator;
                this._newItem = newItem;
            }

            /// <summary>
            /// Implements the MoveNext function for IEnumerable
            /// </summary>
            /// <returns>Whether we can move to the next item</returns>
            public bool MoveNext()
            {
                if (this._timestamp != this._collectionView.Timestamp)
                {
                    throw new InvalidOperationException(PagedCollectionViewResources.EnumeratorVersionChanged);
                }

                switch (this._position)
                {
                    case Position.BeforeNewItem:
                        if (this._baseEnumerator.MoveNext() &&
                                    (this._newItem == null || this._baseEnumerator.Current != this._newItem
                                            || this._baseEnumerator.MoveNext()))
                        {
                            // advance base, skipping the new item
                        }
                        else if (this._newItem != null)
                        {
                            // if base has reached the end, move to new item
                            this._position = Position.OnNewItem;
                        }
                        else
                        {
                            return false;
                        }
                        return true;
                }

                // in all other cases, simply advance base, skipping the new item
                this._position = Position.AfterNewItem;
                return this._baseEnumerator.MoveNext() &&
                    (this._newItem == null
                        || this._baseEnumerator.Current != this._newItem
                        || this._baseEnumerator.MoveNext());
            }

            /// <summary>
            /// Gets the Current value for IEnumerable
            /// </summary>
            public object Current
            {
                get
                {
                    return (this._position == Position.OnNewItem) ? this._newItem : this._baseEnumerator.Current;
                }
            }

            /// <summary>
            /// Implements the Reset function for IEnumerable
            /// </summary>
            public void Reset()
            {
                this._position = Position.BeforeNewItem;
                this._baseEnumerator.Reset();
            }

            /// <summary>
            /// CollectionView that we are creating the enumerator for
            /// </summary>
            private PagedCollectionView _collectionView;

            /// <summary>
            /// The Base Enumerator that we are passing in
            /// </summary>
            private IEnumerator _baseEnumerator;

            /// <summary>
            /// The position we are appending items to the enumerator
            /// </summary>
            private Position _position;

            /// <summary>
            /// Reference to any new item that we want to add to the collection
            /// </summary>
            private object _newItem;

            /// <summary>
            /// Timestamp to let us know whether there have been updates to the collection
            /// </summary>
            private int _timestamp;
        }

#endregion Private Classes
    }

    /// <summary>
    /// IComparer class to sort by class property value (using reflection).
    /// </summary>
    internal class SortFieldComparer : IComparer
    {
#region Constructors

        internal SortFieldComparer() { }

        /// <summary>
        /// Create a comparer, using the SortDescription and a Type;
        /// tries to find a reflection PropertyInfo for each property name
        /// </summary>
        /// <param name="collectionView">CollectionView that contains list of property names and direction to sort by</param>
        public SortFieldComparer(ICollectionView collectionView)
        {
            this._collectionView = collectionView;
            this._sortFields = collectionView.SortDescriptions;
            this._fields = CreatePropertyInfo(this._sortFields);
            this._comparer = new CultureSensitiveComparer(collectionView.Culture);
        }

#endregion

#region Public Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">first item to compare</param>
        /// <param name="y">second item to compare</param>
        /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
        /// <remarks>
        /// Compares the 2 items using the list of property names and directions.
        /// </remarks>
        public int Compare(object x, object y)
        {
            int result = 0;

            // compare both objects by each of the properties until property values don't match
            for (int k = 0; k < this._fields.Length; ++k)
            {
                // if the property type is not yet determined, try
                // obtaining it from the objects
                Type propertyType = this._fields[k].PropertyType;
                if (propertyType == null)
                {
                    if (x != null)
                    {
                        this._fields[k].PropertyType = x.GetType().GetNestedPropertyType(this._fields[k].PropertyPath);
                        propertyType = this._fields[k].PropertyType;
                    }
                    if (this._fields[k].PropertyType == null && y != null)
                    {
                        this._fields[k].PropertyType = y.GetType().GetNestedPropertyType(this._fields[k].PropertyPath);
                        propertyType = this._fields[k].PropertyType;
                    }
                }

                object v1 = this._fields[k].GetValue(x);
                object v2 = this._fields[k].GetValue(y);

                // this will handle the case with string comparisons
                if (propertyType == typeof(string))
                {
                    result = this._comparer.Compare(v1, v2);
                }
                else
                {
                    // try to also set the value for the comparer if this was 
                    // not already calculated
                    IComparer comparer = this._fields[k].Comparer;
                    if (propertyType != null && comparer == null)
                    {
                        this._fields[k].Comparer = (typeof(Comparer<>).MakeGenericType(propertyType).GetProperty("Default")).GetValue(null, null) as IComparer;
                        comparer = this._fields[k].Comparer;
                    }

                    result = (comparer != null) ? comparer.Compare(v1, v2) : 0 /*both values equal*/;
                }

                if (this._fields[k].Descending)
                {
                    result = -result;
                }

                if (result != 0)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Steps through the given list using the comparer to find where
        /// to insert the specified item to maintain sorted order
        /// </summary>
        /// <param name="x">Item to insert into the list</param>
        /// <param name="list">List where we want to insert the item</param>
        /// <returns>Index where we should insert into</returns>
        public int FindInsertIndex(object x, IList list)
        {
            int min = 0;
            int max = list.Count - 1;
            int index;

            // run a binary search to find the right index
            // to insert into.
            while (min <= max)
            {
                index = (min + max) / 2;

                int result = this.Compare(x, list[index]);
                if (result == 0)
                {
                    return index;
                }
                else if (result > 0)
                {
                    min = index + 1;
                }
                else
                {
                    max = index - 1;
                }
            }

            return min;
        }

#endregion

#region Private Methods

        private static SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortFields)
        {
            SortPropertyInfo[] fields = new SortPropertyInfo[sortFields.Count];
            for (int k = 0; k < sortFields.Count; ++k)
            {
                // remember PropertyPath and Direction, used when actually sorting
                fields[k].PropertyPath = sortFields[k].PropertyName;
                fields[k].Descending = (sortFields[k].Direction == ListSortDirection.Descending);
            }
            return fields;
        }

#endregion

#region Private Fields

        struct SortPropertyInfo
        {
            internal IComparer Comparer;
            internal bool Descending;
            internal string PropertyPath;
            internal Type PropertyType;

            internal object GetValue(object o)
            {
                object value;
                if (String.IsNullOrEmpty(this.PropertyPath))
                {
                    value = (this.PropertyType == o.GetType()) ? o : null;
                }
                else
                {
                    value = PagedCollectionView.InvokePath(o, this.PropertyPath, this.PropertyType);
                }

                return value;
            }
        }

        private ICollectionView _collectionView;
        private SortPropertyInfo[] _fields;
        private SortDescriptionCollection _sortFields;
        private IComparer<object> _comparer;

#endregion
    }

    /// <summary>
    /// Creates a comparer class that takes in a CultureInfo as a parameter,
    /// which it will use when comparing strings.
    /// </summary>
    internal class CultureSensitiveComparer : IComparer<object>
    {
        /// <summary>
        /// Private accessor for the CultureInfo of our comparer
        /// </summary>
        private CultureInfo _culture;

        /// <summary>
        /// Creates a comparer which will respect the CultureInfo
        /// that is passed in when comparing strings.
        /// </summary>
        /// <param name="culture">The CultureInfo to use in string comparisons</param>
        public CultureSensitiveComparer(CultureInfo culture)
            : base()
        {
            this._culture = culture ?? CultureInfo.InvariantCulture;
        }

#region IComparer<object> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">first item to compare</param>
        /// <param name="y">second item to compare</param>
        /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
        /// <remarks>
        /// Compares the 2 items using the specified CultureInfo for string and using the default object comparer for all other objects.
        /// </remarks>
        public int Compare(object x, object y)
        {
            if (x == null)
            {
                if (y != null)
                {
                    return -1;
                }
                return 0;
            }
            if (y == null)
            {
                return 1;
            }

            // at this point x and y are not null
            if (x.GetType() == typeof(string) && y.GetType() == typeof(string))
            {
                return this._culture.CompareInfo.Compare((string)x, (string)y);
            }
            else
            {
                return Comparer<object>.Default.Compare(x, y);
            }
        }

#endregion
    }

    /// <summary>
    /// Represents a method that is used to provide custom logic to select 
    /// the GroupDescription based on the parent group and its level. 
    /// </summary>
    /// <param name="group">The parent group.</param>
    /// <param name="level">The level of group.</param>
    /// <returns>The GroupDescription chosen based on the parent group and its level.</returns>
    public delegate GroupDescription GroupDescriptionSelectorCallback(CollectionViewGroup group, int level);
}
