// Copyright (C) Microsoft Corporation.  All rights reserved.

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using CSHTML5.Internal;
using OpenSilver.Internal.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// The XAML proxy of a collection view class.
    /// </summary>
    public class CollectionViewSource : DependencyObject, ISupportInitialize
    {
        #region Constructors

        //
        //  Constructors
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewSource"/> class.
        /// </summary>
        public CollectionViewSource()
        {
            _sort = new SortDescriptionCollection();
            ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(OnForwardedCollectionChanged);

            _groupBy = new ObservableCollection<GroupDescription>();
            ((INotifyCollectionChanged)_groupBy).CollectionChanged += new NotifyCollectionChangedEventHandler(OnForwardedCollectionChanged);

            this.CachedViews = new Dictionary<object, ViewRecord>();
        }

        #endregion Constructors

        #region Public Properties

        //
        //  Public Properties
        //

        /// <summary>
        ///     Identifies the <see cref="CollectionViewSource.View"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty
            = DependencyProperty.Register(
                    "View",
                    typeof(ICollectionView),
                    typeof(CollectionViewSource),
                    new PropertyMetadata((ICollectionView)null));

        /// <summary>
        /// Gets the view object that is currently associated with this instance of <see cref="CollectionViewSource"/>.
        /// </summary>
        /// <returns>
        /// The view object that is currently associated with this instance of <see cref="CollectionViewSource"/>.
        /// </returns>
        public ICollectionView View
        {
            get
            {
                //return GetOriginalView(CollectionView);
                return CollectionView;
            }
        }

        /// <summary>
        /// Identifies <see cref="CollectionViewSource.Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(
                    "Source",
                    typeof(object),
                    typeof(CollectionViewSource),
                    new PropertyMetadata(
                            (object)null,
                            new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// Gets or sets the collection object from which to create this view.
        /// </summary>
        /// <returns>
        /// The collection object from which to create this view. The default is null.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified value when setting this property is not null or an <see cref="IEnumerable"/>
        /// implementation.-or-The specified value when setting this property is an <see cref="ICollectionView"/>
        /// implementation.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The specified value implements <see cref="ICollectionViewFactory"/> but
        /// its <see cref="ICollectionViewFactory.CreateView"/> method returns an
        /// <see cref="ICollectionView"/> with one or more of the following inconsistencies:<see cref="ICollectionView.CanFilter"/>
        /// is false but <see cref="ICollectionView.Filter"/> is not null.<see cref="ICollectionView.CanSort"/>
        /// is false but <see cref="ICollectionView.SortDescriptions"/> is not empty.<see cref="ICollectionView.CanGroup"/>
        /// is false but <see cref="ICollectionView.GroupDescriptions"/> is not empty.
        /// </exception>
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        ///     Called when SourceProperty is invalidated on "d."
        /// </summary>
        /// <param name="d">The object on which the property was invalidated.</param>
        /// <param name="e">Argument.</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource ctrl = (CollectionViewSource)d;
            if (!IsSourceValid(e.NewValue))
            {
                throw new InvalidOperationException("Unsupported type of source for a collection view.");
            }

            ctrl.OnSourceChanged(e.OldValue, e.NewValue);
            ctrl.EnsureView();
        }

        /// <summary>
        /// Invoked when the <see cref="CollectionViewSource.Source"/> property changes.
        /// </summary>
        /// <param name="oldSource">
        /// The old value of the <see cref="CollectionViewSource.Source"/> property.
        /// </param>
        /// <param name="newSource">
        /// The new value of the <see cref="CollectionViewSource.Source"/> property.
        /// </param>
        protected virtual void OnSourceChanged(object oldSource, object newSource)
        {

        }

        private static bool IsSourceValid(object o)
        {
            return (o == null || o is IEnumerable) && !(o is ICollectionView);
        }

        /// <summary>
        /// Gets or sets the cultural information for any operations of the view that might
        /// differ by culture, such as sorting.
        /// </summary>
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; OnForwardedPropertyChanged(); }
        }

        /// <summary>
        /// Gets a collection of <see cref="SortDescription"/> objects that describe
        /// how the items in the collection are sorted in the view.
        /// </summary>
        public SortDescriptionCollection SortDescriptions
        {
            get { return _sort; }
        }

        /// <summary>
        /// Gets a collection of <see cref="GroupDescription"/> objects that describe
        /// how items in the collection are grouped in the view.
        /// </summary>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return _groupBy; }
        }

        #endregion Public Properties

        #region Public Events

        /// <summary>
        /// Provides filtering logic.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When adding a handler to this event, the <see cref="CollectionViewSource.View"/>
        /// property value has a <see cref="ICollectionView.CanFilter"/> property
        /// value of false.
        /// </exception>
        public event FilterEventHandler Filter
        {
            add
            {
                // Get existing event hanlders
                FilterEventHandler handlers = _filterHandlers;
                if (handlers != null)
                {
                    // combine to a multicast delegate
                    handlers = (FilterEventHandler)Delegate.Combine(handlers, value);
                }
                else
                {
                    handlers = value;
                }
                // Set the delegate
                _filterHandlers = handlers;

                OnForwardedPropertyChanged();
            }
            remove
            {
                // Get existing event hanlders
                FilterEventHandler handlers = _filterHandlers;
                if (handlers != null)
                {
                    // Remove the given handler
                    handlers = (FilterEventHandler)Delegate.Remove(handlers, value);
                    if (handlers == null)
                    {
                        // Clear the value because there are no more handlers
                        _filterHandlers = null;
                    }
                    else
                    {
                        // Set the remaining handlers
                        _filterHandlers = handlers;
                    }
                }

                OnForwardedPropertyChanged();
            }
        }

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the view and delay
        /// automatic refresh.
        /// </summary>
        public IDisposable DeferRefresh()
        {
            return new DeferHelper(this);
        }

        #endregion Public Methods

        //
        //  Interfaces
        //

        #region ISupportInitialize

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void ISupportInitialize.BeginInit()
        {
            _isInitializing = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void ISupportInitialize.EndInit()
        {
            _isInitializing = false;
            EnsureView();
        }

        #endregion ISupportInitialize

        #region Protected Methods

        /// <summary>
        /// Invoked when the collection view type changes.
        /// </summary>
        /// <param name="oldCollectionViewType">
        /// The old collection view type.
        /// </param>
        /// <param name="newCollectionViewType">
        /// The new collection view type.
        /// </param>
        protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
        {

        }

        #endregion Protected Methods

        #region Internal Properties

        //
        //  Internal Properties
        //

        // Returns the CollectionView currently affiliate with this CollectionViewSource.
        // This may be a CollectionViewProxy over the original view.
        internal ICollectionView CollectionView
        {
            get
            {
                ICollectionView view = (ICollectionView)GetValue(ViewProperty);

                if (view != null && !_isViewInitialized)
                {
                    // leak prevention: re-fetch ViewRecord instead of keeping a reference to it,
                    // to be sure that we don't inadvertently keep it alive.
                    object source = Source;

                    if (source != null)
                    {
                        ViewRecord viewRecord = GetViewRecord(source);
                        if (viewRecord != null)
                        {
                            viewRecord.InitializeView();
                            _isViewInitialized = true;
                        }
                    }
                }

                return view;
            }
        }

        #endregion Internal Properties

        #region Private Properties

        private IDictionary<object, ViewRecord> CachedViews
        {
            get;
        }

        #endregion Private Properties

        #region Private Methods

        // Obtain the view affiliated with the current source.  This may create
        // a new view, or re-use an existing one.
        private void EnsureView()
        {
            EnsureView(Source);
        }

        private void EnsureView(object source)
        {
            if (_isInitializing || _deferLevel > 0)
                return;

            // get the view
            ICollectionView view = null;

            if (source != null)
            {
                ViewRecord viewRecord = GetViewRecord(source);

                if (viewRecord != null)
                {
                    view = viewRecord.View;
                    _isViewInitialized = viewRecord.IsInitialized;

                    // bring view up to date with the CollectionViewSource
                    if (_version != viewRecord.Version)
                    {
                        //ApplyPropertiesToView(view);
                        viewRecord.Version = _version;
                    }
                }
            }

            // update the View property
            SetValue(ViewProperty, view);
        }

        private ViewRecord GetViewRecord(object source)
        {
            // Order of precendence in acquiring the View:
            // 1) If the CollectionView for this collection has been cached, then
            //    return the cached instance.
            // 2) If the collection is an ICollectionViewFactory use ICVF.CreateView()
            //    from the collection
            // 3) If the collection is an IList return a new ListCollectionView
            // 4) If the collection is an IEnumerable, return a new EnumerableCollectionView
            // 5) return null

            // if the view already exists, just return it
            ViewRecord viewRecord = GetExistingViewRecord(source);
            if (viewRecord != null)
            {
                return viewRecord;
            }

            ICollectionView icv = null;

            ICollectionViewFactory icvf = source as ICollectionViewFactory;
            if (icvf != null)
            {
                // collection is a view factory - call its factory method
                icv = icvf.CreateView();
            }
            else
            {
                // collection is not a factory - create an appropriate view
                IList il = source as IList;
                if (il != null)
                {
                    icv = new ListCollectionView(il);
                }
                else
                {
                    // collection is not IList, wrap it
                    IEnumerable ie = source as IEnumerable;
                    if (ie != null)
                    {
                        icv = new EnumerableCollectionView(ie);
                    }
                }
            }

            // if we got a view, add it to the tables
            if (icv != null)
            {
                viewRecord = new ViewRecord(icv);
                this.CachedViews[source] = viewRecord;
            }

            return viewRecord;
        }

        private ViewRecord GetExistingViewRecord(object source)
        {
            if (this.CachedViews.ContainsKey(source))
            {
                return this.CachedViews[source];
            }
            return null;
        }

        // Forward properties from the CollectionViewSource to the CollectionView
        private void ApplyPropertiesToView(ICollectionView view)
        {
            if (view == null || _deferLevel > 0)
                return;

#if false // no live shaping
            ICollectionViewLiveShaping liveView = view as ICollectionViewLiveShaping;
#endif // no live shaping

            using (view.DeferRefresh())
            {
                int i, n;

                // Culture
                if (Culture != null)
                {
                    view.Culture = Culture;
                }

                // Sort
                if (view.CanSort)
                {
                    view.SortDescriptions.Clear();
                    for (i = 0, n = SortDescriptions.Count; i < n; ++i)
                    {
                        view.SortDescriptions.Add(SortDescriptions[i]);
                    }
                }
                else if (SortDescriptions.Count > 0)
                    throw new InvalidOperationException(string.Format("'{0}' view does not support sorting.", view));

                // Filter
                Predicate<object> filter;
                if (_filterHandlers != null)
                {
                    filter = FilterWrapper;
                }
                else
                {
                    filter = null;
                }

                if (view.CanFilter)
                {
                    view.Filter = filter;
                }
                else if (filter != null)
                    throw new InvalidOperationException(string.Format("'{0}' view does not support filtering.", view));

                // GroupBy
                if (view.CanGroup)
                {
                    view.GroupDescriptions.Clear();
                    for (i = 0, n = GroupDescriptions.Count; i < n; ++i)
                    {
                        view.GroupDescriptions.Add(GroupDescriptions[i]);
                    }
                }
                else if (GroupDescriptions.Count > 0)
                    throw new InvalidOperationException(string.Format("'{0}' view does not support grouping.", view));

#if false // no live shaping
                // Live shaping
                if (liveView != null)
                {
                    ObservableCollection<string> properties;

                    // sorting
                    if (liveView.CanChangeLiveSorting)
                    {
                        liveView.IsLiveSorting = IsLiveSortingRequested;
                        properties = liveView.LiveSortingProperties;
                        properties.Clear();

                        if (IsLiveSortingRequested)
                        {
                            foreach (string s in LiveSortingProperties)
                            {
                                properties.Add(s);
                            }
                        }
                    }

                    CanChangeLiveSorting = liveView.CanChangeLiveSorting;
                    IsLiveSorting = liveView.IsLiveSorting;

                    // filtering
                    if (liveView.CanChangeLiveFiltering)
                    {
                        liveView.IsLiveFiltering = IsLiveFilteringRequested;
                        properties = liveView.LiveFilteringProperties;
                        properties.Clear();

                        if (IsLiveFilteringRequested)
                        {
                            foreach (string s in LiveFilteringProperties)
                            {
                                properties.Add(s);
                            }
                        }
                    }

                    CanChangeLiveFiltering = liveView.CanChangeLiveFiltering;
                    IsLiveFiltering = liveView.IsLiveFiltering;

                    // grouping
                    if (liveView.CanChangeLiveGrouping)
                    {
                        liveView.IsLiveGrouping = IsLiveGroupingRequested;
                        properties = liveView.LiveGroupingProperties;
                        properties.Clear();

                        if (IsLiveGroupingRequested)
                        {
                            foreach (string s in LiveGroupingProperties)
                            {
                                properties.Add(s);
                            }
                        }
                    }

                    CanChangeLiveGrouping = liveView.CanChangeLiveGrouping;
                    IsLiveGrouping = liveView.IsLiveGrouping;
                }
                else
                {
                    CanChangeLiveSorting = false;
                    IsLiveSorting = null;
                    CanChangeLiveFiltering = false;
                    IsLiveFiltering = null;
                    CanChangeLiveGrouping = false;
                    IsLiveGrouping = null;
                }
#endif // no live shaping
            }
        }

        private Predicate<object> FilterWrapper
        {
            get
            {
                if (_filterStub == null)
                {
                    _filterStub = new FilterStub(this);
                }

                return _filterStub.FilterWrapper;
            }
        }

        private bool WrapFilter(object item)
        {
            FilterEventArgs args = new FilterEventArgs(item);
            FilterEventHandler handlers = _filterHandlers;

            if (handlers != null)
            {
                handlers(this, args);
            }

            return args.Accepted;
        }

        // a change occurred in one of the collections that we forward to the view
        private void OnForwardedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnForwardedPropertyChanged();
        }

        // a change occurred in one of the properties that we forward to the view
        private void OnForwardedPropertyChanged()
        {
            // increment the version number.  This causes the change to get applied
            // to dormant views when they become active.
            unchecked
            { ++_version; }

            // apply the change to the current view
            ApplyPropertiesToView(View);
        }

        // defer changes
        private void BeginDefer()
        {
            ++_deferLevel;
        }

        private void EndDefer()
        {
            if (--_deferLevel == 0)
            {
                EnsureView();
            }
        }

#endregion Private Methods

#region Private Types

        //
        //  Private Types
        //

        private class DeferHelper : IDisposable
        {
            public DeferHelper(CollectionViewSource target)
            {
                _target = target;
                _target.BeginDefer();
            }

            public void Dispose()
            {
                if (_target != null)
                {
                    CollectionViewSource target = _target;
                    _target = null;
                    target.EndDefer();
                }
#if NETSTANDARD
                GC.SuppressFinalize(this);
#endif
            }

            private CollectionViewSource _target;
        }

        // This class is used to break the reference chain from a collection
        // view to a UI element (typically Window or Page), created when the
        // app adds a handler (belonging to the Window or Page) to the Filter
        // event.  This class uses a weak reference to the CollectionViewSource
        // to break the chain and avoid a leak (bug 123012)
        private class FilterStub
        {
            public FilterStub(CollectionViewSource parent)
            {
#if NETSTANDARD
                _parent = new WeakReference(parent);
#else // BRIDGE
                _parent = parent;
#endif
                _filterWrapper = new Predicate<object>(WrapFilter);
            }

            public Predicate<object> FilterWrapper
            {
                get { return _filterWrapper; }
            }

            bool WrapFilter(object item)
            {
#if NETSTANDARD
                CollectionViewSource parent = (CollectionViewSource)_parent.Target;
#else // BRIDGE
                CollectionViewSource parent = _parent;
#endif
                if (parent != null)
                {
                    return parent.WrapFilter(item);
                }
                else
                {
                    return true;
                }
            }

#if NETSTANDARD
            WeakReference _parent;
#else // BRIDGE
            CollectionViewSource _parent;
#endif
            Predicate<object> _filterWrapper;
        }

#endregion Private Types

#region Private Data

        //
        //  Private Data
        //

        // properties that get forwarded to the view
        CultureInfo _culture;
        SortDescriptionCollection _sort;
        ObservableCollection<GroupDescription> _groupBy;

        // other state
        bool _isInitializing;
        bool _isViewInitialized; // view is initialized when it is first retrieved externally
        int _version;       // timestamp of last change to a forwarded property
        int _deferLevel;    // counts nested calls to BeginDefer
        FilterStub _filterStub;    // used to support the Filter event

        // Store the handlers for the Filter event
        private FilterEventHandler _filterHandlers;

        #endregion Private Data
    }

    internal class ViewRecord
    {
        internal ViewRecord(ICollectionView view)
        {
            _view = view;
            _version = -1;
        }

        internal ICollectionView View
        {
            get { return _view; }
        }

        internal int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        internal bool IsInitialized
        {
            get { return _isInitialized; }
        }

        internal void InitializeView()
        {
            _view.MoveCurrentToFirst();
            _isInitialized = true;
        }

        ICollectionView _view;
        int _version;
        bool _isInitialized = false;
    }
}
