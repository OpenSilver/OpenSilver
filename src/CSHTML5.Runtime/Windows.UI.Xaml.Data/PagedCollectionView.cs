﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    /// <summary>
    /// Allows to split a data source into multiple paged sources and to expose only the current page
    /// </summary>
    /// <remarks>
    /// <p>the order of the operations is: Filtering, Sorting, Grouping</p>
    /// </remarks>
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
    public partial class PagedCollectionView : IPagedCollectionView, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
#else
    public partial class PagedCollectionView : IEnumerable, INotifyCollectionChanged
#endif
    {
        // the child views
        List<CollectionViewGroupInternal> _views = new List<CollectionViewGroupInternal>();

        // the list of pages
        List<ICollection<object>> _pages = new List<ICollection<object>>();

        // the collection that represents the output data source. This collection is modified with page index, but when the orignal source changes, this collection is recreated
        Collection<object> _collectionInteface = new Collection<object>();

        // remember the originalDataSource
        IEnumerable _originalDataSource;

        // remember the originalDataSource before any copy
        internal IEnumerable _originalDataSourceWithoutCopy;

        // something very similar in the real PagedCollectionView
        INTERNAL_Operations _operations;

        // While processing pages programmatically, we do not want to raise the CollectionChanged event, because it would be raised too often.
        bool _avoidCollectionChangedEvent;

        // set the original data. Source can't be change later if it's not an observable collection
        public PagedCollectionView(IEnumerable source)
        {
            _originalDataSource = source != null ? source : new Collection<object>();

            Init();

            //ScheduleRefresh();
            Refresh();
        }

        #region Internal and Private Methods

        // add a new child view in the paged collection (not necessarily a leaf)
        internal void AddView(CollectionViewGroupInternal view)
        {
            _views.Add(view);
        }

        // return the iterable list of all the views
        internal IEnumerator GetViews()
        {
            return _views.GetEnumerator();
        }

        // allow to not recreate a new PagedCollectionView to change the data
        internal void SetIEnumerableAsCollectionSource(IEnumerable newSource, bool copySource = false)
        {
            _originalDataSourceWithoutCopy = newSource;

            if (!(newSource is PagedCollectionView))
            {
                _avoidCollectionChangedEvent = true;

                if (_originalDataSource is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)_originalDataSource).CollectionChanged -= Source_CollectionChanged;
                }

                _originalDataSource = copySource ? CopySource(newSource) : newSource;

                if (_originalDataSource != null)
                {
                    if (_originalDataSource is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)_originalDataSource).CollectionChanged += Source_CollectionChanged;
                    }

                    Refresh(); // Note: this is "Refresh" instead of "ScheduleRefresh" because it needs to be done ilmediate. the reason is that the calling code will then set DataGrid.ItemsSource which will cause an ItemsSource_Changed and then the update of the DataGrid. 
                }
                else
                {
                    _views.Clear();
                    _pages.Clear();
                }

                _avoidCollectionChangedEvent = false;

                //ChangeOutputColletion();
            }
        }

        // get all the views that are considered as leaf
        internal List<CollectionViewGroupInternal> GetLeaves()
        {
            List<CollectionViewGroupInternal> leaves = new List<CollectionViewGroupInternal>();

            foreach (CollectionViewGroupInternal view in _views)
            {
                if (view.IsLeaf)
                    leaves.Add(view);
            }

            return leaves;
        }

        // copy the original source
        IEnumerable CopySource(IEnumerable source)
        {
            if (source != null)
            {
                Collection<object> newSource;

                if (source is INotifyCollectionChanged)
                    newSource = new ObservableCollection<object>();
                else
                    newSource = new Collection<object>();

                foreach (object item in source)
                {
                    newSource.Add(item);
                }

                return newSource;
            }

            return null;
        }

        // Return the view without any data processing, that is a copy of the original source
        CollectionViewGroupInternal GetDefaultView()
        {
            return _views.Count == 0 ? null : _views[_views.Count - 1];
        }

        // return the view just after the filter pass (the filter pass is always the first operation to be processed)
        CollectionViewGroupInternal GetFilterView()
        {
            if ((_filter.FilterUsingAnEvent != null || _filter.FilterUsingAPredicate != null) && _operations.HasFilteringBeenDone() && _views.Count >= 2)
            {
                return _views[_views.Count - 2];
            }

            return null;
        }

        // register the events
        void Init()
        {
            GroupDescriptions = new ObservableCollection<PropertyGroupDescription>();
            SortDescriptions = new ObservableCollection<PropertySortDescription>();

            GroupDescriptions.CollectionChanged += OnGroupingChanged;
            SortDescriptions.CollectionChanged += OnSortingChanged;

            if (_originalDataSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)_originalDataSource).CollectionChanged += Source_CollectionChanged;
            }
        }

        // represents the index of the leaf that is used, can be different than 0 if grouping operations are used
        //todo: check how grouping really works
        private int _index = 0;
        private int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                ConstrainIndexToRemainInsideAllowedRange();

                CreatePages();
            }
        }

        // check if the index is inside the allowed range, and if not, modify it
        void ConstrainIndexToRemainInsideAllowedRange()
        {
            List<CollectionViewGroupInternal> leaves = GetLeaves();
            if (leaves.Count == 0)
                _index = -1; // mean return original source

            if (_index >= leaves.Count)
                _index = leaves.Count - 1; // mean return the last leaf
        }

        // create all the operation tree with a new source
        void CreateTree()
        {
            _views.Clear();

            _operations = new INTERNAL_Operations(this); // we reset the operations to do

            ICollection<object> newCollection = new Collection<object>();

            foreach (object obj in _originalDataSource)
            {
                newCollection.Add(obj);
            }

            CollectionViewGroupInternal treeRoot = new CollectionViewGroupInternal(newCollection.ToList<object>(), null, _operations);

            _views.Add(treeRoot); // we make sure that the treeRoot is the last element to be added to the _views collection because the call to the CollectionViewGroupInternal contructor creates a tree via recursion, which by itself adds the branches in _views.

            ConstrainIndexToRemainInsideAllowedRange(); // because some leaves can disappear if it's not the first generation (ie. if it has already been refreshed once)
        }

        // recreate all the pages
        void CreatePages()
        {
            _avoidCollectionChangedEvent = true; // Otherwise the CollectionChanged event would be raised too many times.

            _pages.Clear();

            if (Index != -1)// if there is at least one leaf
            {
                int numberOfElementsInPage = 0;

                Collection<object> page = new Collection<object>();

                foreach (object item in GetLeaves()[Index].Items)
                {
                    if (numberOfElementsInPage == 0)
                    {
                        page = new Collection<object>();
                    }

                    numberOfElementsInPage++;
                    page.Add(item);

                    if (numberOfElementsInPage >= PageSize && PageSize != -1) // -1 means never create a second page
                    {
                        numberOfElementsInPage = 0;
                        _pages.Add(page);
                    }
                }

                if (_pages.Count == 0 || _pages[_pages.Count - 1] != page) // we add the last page only if it's not the same page as before
                    _pages.Add(page);
            }

            PageIndex = VerifyPageIndex(PageIndex); // because the number of pages may have changed

            _avoidCollectionChangedEvent = false;

            ChangeOutputColletion();

        }

        // return the closest existing index to indexRequested
        int VerifyPageIndex(int indexRequested)
        {
            if (indexRequested >= _pages.Count)
                return _pages.Count - 1; // -1 because zero-based index

            if (indexRequested < 0)
                return 0;

            return indexRequested;
        }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event EventHandler<EventArgs> PageChanged;

#if WORKINPROGRESS && !CSHTML5NETSTANDARD
        public event EventHandler<PageChangingEventArgs> PageChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endif
        // old version 
        /*
        void ChangeOutputColletion()
        {
            _collectionInteface.Clear();

            if (_pages.Count != 0)
            {
                foreach (object item in _pages[_PageIndex])
                {
                    _collectionInteface.Add(item);
                }
            }

            // 2 calls because replace does not work, (NotifyCollectionChangedEventArgs accept both new and old or reset that doesn't clear the grid)
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _collectionInteface, 0));
            }
        }
         */

        // run this method every time the exposed view has changed
        void ChangeOutputColletion()
        {
            if (!_avoidCollectionChangedEvent)
            {
                Collection<object> oldItems = _collectionInteface;

                _collectionInteface = new Collection<object>();

                if (_pages.Count != 0)
                {
                    foreach (object item in _pages[_PageIndex])
                    {
                        _collectionInteface.Add(item);
                    }
                }

                if (CollectionChanged != null)
                {
                    //Note: we make two calls because "replace" does not work in the DataGrid at the time of writing.
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _collectionInteface, 0));
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                    this.OnPropertyChanged("Count");
#endif
                }
            }
        }

        // occurs when the original collection has changed
        void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // all the action types need a complete refresh and a modification of the source to keep a good behaviour

            //todo-perfs: do not recreate all the tree, but for all elements that have changed remove them, then reapply all the operations
            ScheduleRefresh();
        }

        // occurs if the user adds a grouping operation to the list
        void OnGroupingChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleRefresh();
        }

        // occurs if the user adds a sorting operation to the list
        void OnSortingChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleRefresh();
        }

#endregion

#region Public Methods

        // refresh all the sub-group, this method is used only to refresh the filtering, but since an external method is used to filter, 
        // we should not run the method multiple times with the same element
        public void Refresh()
        {
            // Use CreateTree instead to allow the changes in data source to be applied by user if the collection is not observable
            CreateTree();

            CreatePages(); // refreshIndex does not run CreatePages, so we manually recreate the pages
        }

        bool _refreshPending = false;
        void ScheduleRefresh()
        {
            if (!_refreshPending) // This ensures that the "BeginInvoke" method is only called once, and it is not called again until its delegate has been executed.
            {
                _refreshPending = true;
                INTERNAL_DispatcherHelpers.QueueAction(() => // We use a dispatcher to avoid refreshing every time (the result is as if we waited for the last change to be done before the thread is free). We use "INTERNAL_DispatcherHelpers.QueueAction" instead of "Dispatcher.BeginInvoke" because it has better performance than calling Dispatcher.BeginInvoke directly.
                {
                    _refreshPending = false;

                    Refresh();
                });
            }
        }

        // Filter that allow two type of filter instead of one, see: CollectionViewSource
        internal FilterDescription _filter;

        public Predicate<Object> Filter
        {
            get { return _filter != null ? _filter.FilterUsingAPredicate : null; }
            set
            {
                _filter = new FilterDescription(value);
            }
        }

        // return the enumerator of the current page
        public IEnumerator GetEnumerator()
        {
            return _collectionInteface.GetEnumerator();
        }

#region Public methods to navigate

        public void MoveToFirstPage()
        {
            PageIndex = 0;
        }

        public void MoveToLastPage()
        {
            PageIndex = _pages.Count - 1;
        }

        public void MoveToNextPage()
        {
            PageIndex++;
        }

        public void MoveToPage(int index)
        {
            PageIndex = index;
        }

        public void MoveToPreviousPage()
        {
            PageIndex--;
        }

#endregion

        //Returns a value that indicates if a specified item has passed the filter
        public bool PassesFilter(object item)
        {
            CollectionViewGroupInternal filterView = GetFilterView();
            if (filterView != null)
            {
                return filterView.Items.Contains(item);
            }
            return true; // if no filter, we consider the filter as passed
        }

#if WORKINPROGRESS && !CSHTML5NETSTANDARD
        bool IPagedCollectionView.MoveToFirstPage()
        {
            MoveToFirstPage();
            return true;
        }

        bool IPagedCollectionView.MoveToLastPage()
        {
            MoveToLastPage();
            return true;
        }

        bool IPagedCollectionView.MoveToNextPage()
        {
            MoveToNextPage();
            return true;
        }

        bool IPagedCollectionView.MoveToPreviousPage()
        {
            MoveToPreviousPage();
            return true;
        }

        bool IPagedCollectionView.MoveToPage(int pageIndex)
        {
            MoveToPage(pageIndex);
            return true;
        }
#endif

        // Gets the top-level groups, constructed according to the descriptions specified in the GroupDescriptions property.
        public Collection<IEnumerable> Groups
        {
            get
            {
                Collection<IEnumerable> Leaves = new Collection<IEnumerable>();

                foreach (CollectionViewGroupInternal group in GetLeaves())
                {
                    Leaves.Add(group.Items);
                }

                return Leaves;
            }
        }

#endregion

#region Properties

        // Gets the number of elements in the view after filtering, sorting, and paging.
        public int Count { get { return _pages[_PageIndex].Count; } }

        // Gets the total number of items in the view before paging is applied.
        public int TotalItemCount
        {
            get
            {
                if (Index != -1 && _originalDataSource != null)
                {
                    var leaves = GetLeaves();
                    if (leaves.Count > Index)
                        return leaves[Index].Items.Count;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        // Gets the zero-based index of the current page.
        private int _PageIndex = 0;
        public int PageIndex
        {
            get { return _PageIndex; }

            private set
            {
                int newPageIndex = VerifyPageIndex(value);
                if (newPageIndex != _PageIndex) // ChangeOutputColletion can take lot of time, because it refreshes the controls that use this as source
                {
                    int oldCount = this.Count;
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                    OnPageChanging(newPageIndex);
#endif
                    _PageIndex = newPageIndex;
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                    OnPropertyChanged("PageIndex");
#endif
                    OnPageChanged();

#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                    if (this.Count != oldCount)
                    {
                        OnPropertyChanged("Count");
                    }
#endif
                }
            }
        }

#if WORKINPROGRESS && !CSHTML5NETSTANDARD
        private void OnPageChanging(int newPageIndex)
        {
            _isPageChanging = true;
            if (PageChanging != null)
            {
                PageChanging(this, new PageChangingEventArgs(newPageIndex));
            }
        }
#endif

        private void OnPageChanged()
        {
            if (PageChanged != null)
            {
                PageChanged(this, new EventArgs());
            }
            ChangeOutputColletion();
            _isPageChanging = false;
        }

        // Gets or sets the number of items to display on a page.
        // -1 means infinity
        private int _pageSize = -1;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                int oldCount = this.Count;
                _pageSize = value;
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                OnPropertyChanged("PageSize");
#endif
                if (_views.Count != 0)
                    CreatePages();
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                // if the count has changed
                if (this.Count != oldCount)
                {
                    this.OnPropertyChanged("Count");
                }
#endif
            }
        }

        // Gets the IEnumerable collection underlying this view.
        public IEnumerable SourceCollection { get { return _originalDataSource; } }

        // get or set the list of grouping operations
        public ObservableCollection<PropertyGroupDescription> GroupDescriptions { get; set; }

        // get or set the list of sorting operations
        public ObservableCollection<PropertySortDescription> SortDescriptions { get; set; }

        public bool CanChangePage
        {
            get
            {
                return true;
            }
        }

        private bool _isPageChanging;
        public bool IsPageChanging
        {
            get
            {
                return _isPageChanging;
            }
        }

        public int ItemCount
        {
            get
            {
                return TotalItemCount;
            }
        }

#endregion
    }
}
