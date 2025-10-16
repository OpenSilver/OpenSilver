// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Defines CollectionViewSource object, the markup-accessible entry
//              point to CollectionView.
//

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Markup;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data
{
    /// <summary>
    /// The Extensible Application Markup Language (XAML) proxy of a <see cref="Data.CollectionView"/> class.
    /// </summary>
    public class CollectionViewSource : DependencyObject, ISupportInitialize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewSource"/> class.
        /// </summary>
        public CollectionViewSource()
        {
            _sort = new SortDescriptionCollection();
            ((INotifyCollectionChanged)_sort).CollectionChanged += new NotifyCollectionChangedEventHandler(OnForwardedCollectionChanged);

            _groupBy = new ObservableCollection<GroupDescription>();
            ((INotifyCollectionChanged)_groupBy).CollectionChanged += new NotifyCollectionChangedEventHandler(OnForwardedCollectionChanged);
        }

        private static readonly DependencyPropertyKey ViewPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(View),
                typeof(ICollectionView),
                typeof(CollectionViewSource),
                new FrameworkPropertyMetadata((ICollectionView)null));

        /// <summary>
        /// Identifies the <see cref="View"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the view object that is currently associated with this instance of <see cref="CollectionViewSource"/>.
        /// </summary>
        /// <returns>
        /// The view object that is currently associated with this instance of <see cref="CollectionViewSource"/>.
        /// </returns>
        [ReadOnly(true)]
        public ICollectionView View
        {
            get
            {
                return GetOriginalView(CollectionView);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(object),
                typeof(CollectionViewSource),
                new FrameworkPropertyMetadata(null, OnSourceChanged),
                IsSourceValid);

        /// <summary>
        /// Gets or sets the collection object from which to create this view.
        /// </summary>
        /// <returns>
        /// The collection object from which to create this view. The default is null.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified value when setting this property is not null or an <see cref="IEnumerable"/> implementation.
        /// -or-The specified value when setting this property is an <see cref="ICollectionView"/> implementation.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The specified value implements <see cref="ICollectionViewFactory"/> but its <see cref="ICollectionViewFactory.CreateView"/> 
        /// method returns an <see cref="ICollectionView"/> with one or more of the following inconsistencies:
        /// <see cref="ICollectionView.CanFilter"/> is false but <see cref="ICollectionView.Filter"/> is not null.
        /// <see cref="ICollectionView.CanSort"/> is false but <see cref="ICollectionView.SortDescriptions"/> is not empty.
        /// <see cref="ICollectionView.CanGroup"/> is false but <see cref="ICollectionView.GroupDescriptions"/> is not empty.
        /// </exception>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValueInternal(SourceProperty, value); }
        }

        /// <summary>
        /// Called when SourceProperty is invalidated on "d."
        /// </summary>
        /// <param name="d">The object on which the property was invalidated.</param>
        /// <param name="e">Argument.</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource ctrl = (CollectionViewSource)d;

            ctrl.OnSourceChanged(e.OldValue, e.NewValue);
            ctrl.EnsureView();
        }

        /// <summary>
        /// Invoked when the <see cref="Source"/> property changes.
        /// </summary>
        /// <param name="oldSource">
        /// The old value of the <see cref="Source"/> property.
        /// </param>
        /// <param name="newSource">
        /// The new value of the <see cref="Source"/> property.
        /// </param>
        protected virtual void OnSourceChanged(object oldSource, object newSource)
        {
        }

        private static bool IsSourceValid(object o)
        {
            return (o == null || o is IEnumerable || o is IListSource || o is DataSourceProvider) && o is not ICollectionView;
        }

        private static bool IsValidSourceForView(object o)
        {
            return o is IEnumerable || o is IListSource;
        }

        /// <summary>
        /// Identifies the <see cref="CollectionViewType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollectionViewTypeProperty =
            DependencyProperty.Register(
                nameof(CollectionViewType),
                typeof(Type),
                typeof(CollectionViewSource),
                new FrameworkPropertyMetadata(null, OnCollectionViewTypeChanged),
                IsCollectionViewTypeValid);

        /// <summary>
        /// Gets or sets the desired view type.
        /// </summary>
        /// <remarks>
        /// The desired view type.
        /// </remarks>
        public Type CollectionViewType
        {
            get { return (Type)GetValue(CollectionViewTypeProperty); }
            set { SetValueInternal(CollectionViewTypeProperty, value); }
        }

        private static void OnCollectionViewTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource ctrl = (CollectionViewSource)d;

            Type oldCollectionViewType = (Type)e.OldValue;
            Type newCollectionViewType = (Type)e.NewValue;

            if (!ctrl._isInitializing)
                throw new InvalidOperationException(Strings.CollectionViewTypeIsInitOnly);

            ctrl.OnCollectionViewTypeChanged(oldCollectionViewType, newCollectionViewType);
            ctrl.EnsureView();
        }

        /// <summary>
        /// Invoked when the <see cref="CollectionViewType"/> property changes.
        /// </summary>
        /// <param name="oldCollectionViewType">
        /// The old value of the <see cref="CollectionViewType"/> property.
        /// </param>
        /// <param name="newCollectionViewType">
        /// The new value of the <see cref="CollectionViewType"/> property.
        /// </param>
        protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
        {
        }

        private static bool IsCollectionViewTypeValid(object o)
        {
            Type type = (Type)o;

            return type == null || typeof(ICollectionView).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets or sets the culture that is used for operations such as sorting and comparisons.
        /// </summary>
        /// <returns>
        /// The culture that is used for operations such as sorting and comparisons.
        /// </returns>
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; OnForwardedPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="SortDescription"/> objects that describes how the items in the 
        /// collection are sorted in the view.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="SortDescription"/> objects that describes how the items in the collection are 
        /// sorted in the view.
        /// </returns>
        public SortDescriptionCollection SortDescriptions
        {
            get { return _sort; }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GroupDescription"/> objects that describes how the items in the 
        /// collection are grouped in the view.
        /// </summary>
        /// <returns>
        /// An <see cref="ObservableCollection{T}"/> of <see cref="GroupDescription"/> objects that describes how the 
        /// items in the collection are grouped in the view.
        /// </returns>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return _groupBy; }
        }

        /// <summary>
        /// Provides filtering logic.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When adding a handler to this event, the <see cref="View"/> property value has a <see cref="ICollectionView.CanFilter"/> 
        /// property value of false.
        /// </exception>
        public event FilterEventHandler Filter
        {
            add
            {
                // Get existing event hanlders
                FilterEventHandler handlers = FilterHandlersField.GetValue(this);
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
                FilterHandlersField.SetValue(this, handlers);

                OnForwardedPropertyChanged();
            }
            remove
            {
                // Get existing event hanlders
                FilterEventHandler handlers = FilterHandlersField.GetValue(this);
                if (handlers != null)
                {
                    // Remove the given handler
                    handlers = (FilterEventHandler)Delegate.Remove(handlers, value);
                    if (handlers == null)
                    {
                        // Clear the value because there are no more handlers
                        FilterHandlersField.ClearValue(this);
                    }
                    else
                    {
                        // Set the remaining handlers
                        FilterHandlersField.SetValue(this, handlers);
                    }
                }

                OnForwardedPropertyChanged();
            }
        }

        /// <summary>
        /// Returns the default view for the given source.
        /// </summary>
        /// <param name="source">
        /// An object reference to the binding source.
        /// </param>
        /// <returns>
        /// Returns an <see cref="ICollectionView"/> object that is the default view for the given source collection.
        /// </returns>
        public static ICollectionView GetDefaultView(object source)
        {
            return GetOriginalView(GetDefaultCollectionView(source, true));
        }

        // a version of the previous method that doesn't create the view (bug 108595)
        private static ICollectionView LazyGetDefaultView(object source)
        {
            return GetOriginalView(GetDefaultCollectionView(source, false));
        }

        /// <summary>
        /// Returns a value that indicates whether the given view is the default view for the <see cref="Source"/> collection.
        /// </summary>
        /// <param name="view">
        /// The view object to check.
        /// </param>
        /// <returns>
        /// true if the given view is the default view for the <see cref="Source"/> collection or if the given view is null; 
        /// otherwise, false.
        /// </returns>
        public static bool IsDefaultView(ICollectionView view)
        {
            if (view != null)
            {
                object source = view.SourceCollection;
                return GetOriginalView(view) == LazyGetDefaultView(source);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> object that you can use to dispose of the calling object.
        /// </returns>
        public IDisposable DeferRefresh()
        {
            return new DeferHelper(this);
        }

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

        // Returns the CollectionView currently affiliate with this CollectionViewSource.
        // This may be a CollectionViewProxy over the original view.
        internal CollectionView CollectionView
        {
            get
            {
                ICollectionView view = (ICollectionView)GetValue(ViewProperty);

                if (view != null && !_isViewInitialized)
                {
                    // leak prevention: re-fetch ViewRecord instead of keeping a reference to it,
                    // to be sure that we don't inadvertently keep it alive.
                    object source = Source;
                    DataSourceProvider dataProvider = source as DataSourceProvider;

                    // if the source is DataSourceProvider, use its Data instead
                    if (dataProvider != null)
                    {
                        source = dataProvider.Data;
                    }

                    if (source != null)
                    {
                        DataBindEngine engine = DataBindEngine.CurrentDataBindEngine;
                        ViewRecord viewRecord = engine.GetViewRecord(source, this, CollectionViewType, true, null);
                        if (viewRecord != null)
                        {
                            viewRecord.InitializeView();
                            _isViewInitialized = true;
                        }
                    }
                }

                return (CollectionView)view;
            }
        }

        // Return the default view for the given source.  This view is never
        // affiliated with any CollectionViewSource.  It may be a
        // CollectionViewProxy over the original view
        internal static CollectionView GetDefaultCollectionView(object source, bool createView, Func<object, object> GetSourceItem = null)
        {
            if (!IsValidSourceForView(source))
                return null;

            DataBindEngine engine = DataBindEngine.CurrentDataBindEngine;
            ViewRecord viewRecord = engine.GetViewRecord(source, DefaultSource, null, createView, GetSourceItem);

            return (viewRecord != null) ? (CollectionView)viewRecord.View : null;
        }

        /// <summary>
        /// Return the default view for the given source.  This view is never
        /// affiliated with any CollectionViewSource.  The internal version sets
        /// the culture on the view from the xml:Lang of the host object.
        /// </summary>
        internal static CollectionView GetDefaultCollectionView(object source, DependencyObject d, Func<object, object> GetSourceItem = null)
        {
            CollectionView view = GetDefaultCollectionView(source, true, GetSourceItem);

            // at first use of a view, set its culture from the xml:lang of the
            // element that's using the view
            if (view != null && view.Culture == null)
            {
                XmlLanguage language = (d != null) ? (XmlLanguage)d.GetValue(FrameworkElement.LanguageProperty) : null;
                if (language != null)
                {
                    try
                    {
                        view.Culture = language.GetSpecificCulture();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }

            return view;
        }

        // Obtain the view affiliated with the current source.  This may create
        // a new view, or re-use an existing one.
        private void EnsureView()
        {
            EnsureView(Source, CollectionViewType);
        }

        private void EnsureView(object source, Type collectionViewType)
        {
            if (_isInitializing || _deferLevel > 0)
                return;

            DataSourceProvider dataProvider = source as DataSourceProvider;

            // listen for DataChanged events from an DataSourceProvider
            if (dataProvider != _dataProvider)
            {
                if (_dataProvider != null)
                {
                    if (_weakEventToken != null)
                    {
                        _weakEventToken.Dispose();
                        _weakEventToken = null;
                    }
                }

                _dataProvider = dataProvider;

                if (_dataProvider != null)
                {
                    _weakEventToken = WeakEvent.Subscribe<CollectionViewSource, DataSourceProvider, EventArgs>(
                        this,
                        _dataProvider,
                        static (instance, sender, args) => instance.OnDataChanged(sender, args),
                        static (handler, source) => source.DataChanged -= new EventHandler(handler),
                        static (handler, source) => source.DataChanged += new EventHandler(handler));

                    _dataProvider.InitialLoad();
                }
            }

            // if the source is DataSourceProvider, use its Data instead
            if (dataProvider != null)
            {
                source = dataProvider.Data;
            }

            // get the view
            ICollectionView view = null;

            if (source != null)
            {
                DataBindEngine engine = DataBindEngine.CurrentDataBindEngine;
                ViewRecord viewRecord = engine.GetViewRecord(source, this, collectionViewType, true,
                    (object x) =>
                    {
                        BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(this, SourceProperty);
                        return beb?.GetSourceItem(x);
                    });

                if (viewRecord != null)
                {
                    view = viewRecord.View;
                    _isViewInitialized = viewRecord.IsInitialized;

                    // bring view up to date with the CollectionViewSource
                    if (_version != viewRecord.Version)
                    {
                        ApplyPropertiesToView(view);
                        viewRecord.Version = _version;
                    }
                }
            }

            // update the View property
            SetValueInternal(ViewPropertyKey, view);
        }

        // Forward properties from the CollectionViewSource to the CollectionView
        private void ApplyPropertiesToView(ICollectionView view)
        {
            if (view == null || _deferLevel > 0)
                return;

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
                    throw new InvalidOperationException(string.Format(Strings.CannotSortView, view));

                // Filter
                Predicate<object> filter;
                if (FilterHandlersField.GetValue(this) != null)
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
                    throw new InvalidOperationException(string.Format(Strings.CannotFilterView, view));

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
                    throw new InvalidOperationException(string.Format(Strings.CannotGroupView, view));
            }
        }

        // return the original (un-proxied) view for the given view
        private static ICollectionView GetOriginalView(ICollectionView view)
        {
            for (CollectionViewProxy proxy = view as CollectionViewProxy;
                    proxy != null;
                    proxy = view as CollectionViewProxy)
            {
                view = proxy.ProxiedView;
            }

            return view;
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
            FilterEventHandler handlers = FilterHandlersField.GetValue(this);

            if (handlers != null)
            {
                handlers(this, args);
            }

            return args.Accepted;
        }

        private void OnDataChanged(object sender, EventArgs e)
        {
            EnsureView();
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
            unchecked { ++_version; }

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

        private sealed class DeferHelper : IDisposable
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
                GC.SuppressFinalize(this);
            }

            private CollectionViewSource _target;
        }

        // This class is used to break the reference chain from a collection
        // view to a UI element (typically Window or Page), created when the
        // app adds a handler (belonging to the Window or Page) to the Filter
        // event.  This class uses a weak reference to the CollectionViewSource
        // to break the chain and avoid a leak (bug 123012)
        private sealed class FilterStub
        {
            public FilterStub(CollectionViewSource parent)
            {
                _parent = new WeakReference<CollectionViewSource>(parent);
                FilterWrapper = new Predicate<object>(WrapFilter);
            }

            public Predicate<object> FilterWrapper { get; }

            private bool WrapFilter(object item)
            {
                if (_parent.TryGetTarget(out CollectionViewSource parent))
                {
                    return parent.WrapFilter(item);
                }
                else
                {
                    return true;
                }
            }

            private readonly WeakReference<CollectionViewSource> _parent;
        }

        // properties that get forwarded to the view
        private CultureInfo _culture;
        private readonly SortDescriptionCollection _sort;
        private readonly ObservableCollection<GroupDescription> _groupBy;

        // other state
        private bool _isInitializing;
        private bool _isViewInitialized; // view is initialized when it is first retrieved externally
        private int _version;       // timestamp of last change to a forwarded property
        private int _deferLevel;    // counts nested calls to BeginDefer
        private DataSourceProvider _dataProvider;  // DataSourceProvider whose DataChanged event we want
        private FilterStub _filterStub;    // used to support the Filter event
        private WeakEventToken _weakEventToken;

        // the placeholder source for all default views
        private static readonly CollectionViewSource DefaultSource = new CollectionViewSource();

        // This uncommon field is used to store the handlers for the Filter event
        private static readonly UncommonField<FilterEventHandler> FilterHandlersField = new();
    }
}
