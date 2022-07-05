using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Defines the selection behavior for a series.</summary>
    public enum SeriesSelectionMode
    {
        None,
        Single,
        Multiple,
    }

    /// <summary>
    /// Implements a series that is defined by one or more instances of the DefinitionSeries class.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ContentProperty("SeriesDefinitions")]
    [TemplatePart(Name = "ItemContainer", Type = typeof(DelegatingListBox))]
    [TemplatePart(Name = "SeriesArea", Type = typeof(Grid))]
    public abstract class DefinitionSeries : Control, ISeries, IAxisListener, IRangeProvider, IValueMarginProvider, IDataProvider, ISeriesHost, IRequireSeriesHost, IResourceDictionaryDispenser
    {
        /// <summary>Identifies the DependentAxis dependency property.</summary>
        public static readonly DependencyProperty DependentAxisProperty = DependencyProperty.Register(nameof(DependentAxis), typeof(IAxis), typeof(DefinitionSeries), new PropertyMetadata(new PropertyChangedCallback(DefinitionSeries.OnDependentAxisChanged)));
        /// <summary>Identifies the IndependentAxis dependency property.</summary>
        public static readonly DependencyProperty IndependentAxisProperty = DependencyProperty.Register(nameof(IndependentAxis), typeof(IAxis), typeof(DefinitionSeries), new PropertyMetadata(new PropertyChangedCallback(DefinitionSeries.OnIndependentAxisChanged)));
        /// <summary>
        /// Identifies the ActualDependentAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualDependentAxisProperty = DependencyProperty.Register(nameof(ActualDependentAxis), typeof(IAxis), typeof(DefinitionSeries), (PropertyMetadata)null);
        /// <summary>
        /// Identifies the ActualIndependentAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualIndependentAxisProperty = DependencyProperty.Register(nameof(ActualIndependentAxis), typeof(IAxis), typeof(DefinitionSeries), (PropertyMetadata)null);
        /// <summary>Identifies the SelectionMode dependency property.</summary>
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(SeriesSelectionMode), typeof(DefinitionSeries), new PropertyMetadata((object)SeriesSelectionMode.None, new PropertyChangedCallback(DefinitionSeries.OnSelectionModeChanged)));
        /// <summary>Identifies the SelectedIndex dependency property.</summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(DefinitionSeries), new PropertyMetadata((object)-1));
        /// <summary>Identifies the SelectedItem dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DefinitionSeries), (PropertyMetadata)null);
        /// <summary>
        /// Stores an aggregated collection of legend items from the series definitions.
        /// </summary>
        private readonly AggregatedObservableCollection<object> _legendItems = new AggregatedObservableCollection<object>();
        /// <summary>
        /// Stores the collection of SeriesDefinitions that define the series.
        /// </summary>
        private readonly ObservableCollection<SeriesDefinition> _seriesDefinitions = (ObservableCollection<SeriesDefinition>)new UniqueObservableCollection<SeriesDefinition>();
        /// <summary>
        /// Stores a mirror collection of ISeries corresponding directly to the collection of SeriesDefinitions.
        /// </summary>
        /// <remarks>
        /// Not using ObservableCollectionListAdapter because of race condition on ItemsChanged event
        /// </remarks>
        private readonly ObservableCollection<ISeries> _seriesDefinitionsAsISeries = new ObservableCollection<ISeries>();
        /// <summary>
        /// Keeps the SeriesDefinitions collection synchronized with the Children collection of the SeriesArea.
        /// </summary>
        private readonly ObservableCollectionListAdapter<UIElement> _seriesAreaChildrenListAdapter = new ObservableCollectionListAdapter<UIElement>();
        /// <summary>Stores the clip geometry for the ItemContainer.</summary>
        private readonly RectangleGeometry _clipGeometry = new RectangleGeometry();
        /// <summary>
        /// Tracks the collection of DataItem that are queued for update.
        /// </summary>
        private readonly List<DefinitionSeries.DataItem> _queueUpdateDataItemPlacement_DataItems = new List<DefinitionSeries.DataItem>();
        /// <summary>
        /// Stores a reference to the backing collection for the SelectedItems property.
        /// </summary>
        private ObservableCollection<object> _selectedItems = new ObservableCollection<object>();
        /// <summary>Name of the SeriesArea property.</summary>
        private const string SeriesAreaName = "SeriesArea";
        /// <summary>Name of the ItemContainer property.</summary>
        private const string ItemContainerName = "ItemContainer";
        /// <summary>
        /// Stores a reference to the ItemContainer template part.
        /// </summary>
        private DelegatingListBox _itemContainer;
        /// <summary>
        /// Tracks whether the dependent axis values changed for the next update.
        /// </summary>
        private bool _queueUpdateDataItemPlacement_DependentAxisValuesChanged;
        /// <summary>
        /// Tracks whether the independent axis values changed for the next update.
        /// </summary>
        private bool _queueUpdateDataItemPlacement_IndependentAxisValuesChanged;
        /// <summary>
        /// Tracks whether the SelectedItems collection is being synchronized (to prevent reentrancy).
        /// </summary>
        private bool _synchronizingSelectedItems;
        /// <summary>Stores the SeriesHost for the series.</summary>
        private ISeriesHost _seriesHost;

        /// <summary>
        /// Gets or sets a value indicating whether the series is 100% stacked (versus normally stacked).
        /// </summary>
        protected bool IsStacked100 { get; set; }

        /// <summary>
        /// Gets the collection of DataItems representing the data of the series.
        /// </summary>
        protected ObservableCollection<DefinitionSeries.DataItem> DataItems { get; private set; }

        /// <summary>Gets the SeriesArea template part instance.</summary>
        protected Panel SeriesArea { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DefinitionSeries class.
        /// </summary>
        protected DefinitionSeries()
        {
#if !MIGRATION
            this.DefaultStyleKey = (object)typeof(DefinitionSeries);
#endif
            this._seriesDefinitions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SeriesDefinitionsCollectionChanged);
            this._seriesAreaChildrenListAdapter.Collection = (IEnumerable)this._seriesDefinitions;
            this._selectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SelectedItemsCollectionChanged);
            this.DataItems = new ObservableCollection<DefinitionSeries.DataItem>();
#if MIGRATION
            this.DefaultStyleKey = (object)typeof(DefinitionSeries);
#endif
        }

        /// <summary>Gets or sets the dependent axis of the series.</summary>
        public IAxis DependentAxis
        {
            get
            {
                return (IAxis)this.GetValue(DefinitionSeries.DependentAxisProperty);
            }
            set
            {
                this.SetValue(DefinitionSeries.DependentAxisProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the DependentAxis dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnDependentAxisChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DefinitionSeries)o).OnDependentAxisChanged((IAxis)e.OldValue, (IAxis)e.NewValue);
        }

        /// <summary>Handles changes to the DependentAxis property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnDependentAxisChanged(IAxis oldValue, IAxis newValue)
        {
            if (null == this.ActualDependentAxis)
                return;
            this.EnsureAxes(true, false, false);
        }

        /// <summary>Gets or sets the independent axis of the series.</summary>
        public IAxis IndependentAxis
        {
            get
            {
                return (IAxis)this.GetValue(DefinitionSeries.IndependentAxisProperty);
            }
            set
            {
                this.SetValue(DefinitionSeries.IndependentAxisProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the IndependentAxis dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnIndependentAxisChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DefinitionSeries)o).OnIndependentAxisChanged((IAxis)e.OldValue, (IAxis)e.NewValue);
        }

        /// <summary>Handles changes to the IndependentAxis property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisChanged(IAxis oldValue, IAxis newValue)
        {
            if (null == this.ActualIndependentAxis)
                return;
            this.EnsureAxes(false, true, false);
        }

        /// <summary>Gets the rendered dependent axis of the series.</summary>
        public IAxis ActualDependentAxis
        {
            get
            {
                return (IAxis)this.GetValue(DefinitionSeries.ActualDependentAxisProperty);
            }
            protected set
            {
                this.SetValue(DefinitionSeries.ActualDependentAxisProperty, (object)value);
            }
        }

        /// <summary>Gets the rendered independent axis of the series.</summary>
        public IAxis ActualIndependentAxis
        {
            get
            {
                return (IAxis)this.GetValue(DefinitionSeries.ActualIndependentAxisProperty);
            }
            protected set
            {
                this.SetValue(DefinitionSeries.ActualIndependentAxisProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets the ActualDependentAxis as an IRangeAxis instance.
        /// </summary>
        protected IRangeAxis ActualDependentRangeAxis
        {
            get
            {
                return (IRangeAxis)this.ActualDependentAxis;
            }
        }

        /// <summary>Gets the collection of legend items for the series.</summary>
        public ObservableCollection<object> LegendItems
        {
            get
            {
                return (ObservableCollection<object>)this._legendItems;
            }
        }

        /// <summary>Gets or sets the SeriesHost for the series.</summary>
        public ISeriesHost SeriesHost
        {
            get
            {
                return this._seriesHost;
            }
            set
            {
                if (null != this._seriesHost)
                {
                    this._seriesHost.ResourceDictionariesChanged -= new EventHandler(this.SeriesHostResourceDictionariesChanged);
                    if (null != this.ActualDependentAxis)
                    {
                        this.ActualDependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                        this.ActualDependentAxis = (IAxis)null;
                    }
                    if (null != this.ActualIndependentAxis)
                    {
                        this.ActualIndependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                        this.ActualIndependentAxis = (IAxis)null;
                    }
                    foreach (SeriesDefinition seriesDefinition in this.SeriesDefinitions)
                        this.SeriesDefinitionItemsSourceChanged(seriesDefinition, seriesDefinition.ItemsSource, (IEnumerable)null);
                }
                this._seriesHost = value;
                this.SeriesHostResourceDictionariesChanged((object)null, (EventArgs)null);
                if (null == this._seriesHost)
                    return;
                this._seriesHost.ResourceDictionariesChanged += new EventHandler(this.SeriesHostResourceDictionariesChanged);
                foreach (SeriesDefinition seriesDefinition in this.SeriesDefinitions)
                    this.SeriesDefinitionItemsSourceChanged(seriesDefinition, (IEnumerable)null, seriesDefinition.ItemsSource);
            }
        }

        /// <summary>
        /// Gets or sets the collection of SeriesDefinitions that define the series.
        /// </summary>
        public Collection<SeriesDefinition> SeriesDefinitions
        {
            get
            {
                return (Collection<SeriesDefinition>)this._seriesDefinitions;
            }
            set
            {
                throw new NotSupportedException("DefinitionSeries.SeriesDefinitions: SetterNotSupported");
            }
        }

        /// <summary>Gets or sets the SelectionMode property.</summary>
        public SeriesSelectionMode SelectionMode
        {
            get
            {
                return (SeriesSelectionMode)this.GetValue(DefinitionSeries.SelectionModeProperty);
            }
            set
            {
                this.SetValue(DefinitionSeries.SelectionModeProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the SelectionMode dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnSelectionModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DefinitionSeries)o).OnSelectionModeChanged((SeriesSelectionMode)e.OldValue, (SeriesSelectionMode)e.NewValue);
        }

        /// <summary>Handles changes to the SelectionMode property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnSelectionModeChanged(SeriesSelectionMode oldValue, SeriesSelectionMode newValue)
        {
            if (null == this._itemContainer)
                return;
            switch (newValue)
            {
                case SeriesSelectionMode.None:
                    this._itemContainer.SelectedItem = (object)null;
                    this._itemContainer.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                    break;
                case SeriesSelectionMode.Single:
                    this._itemContainer.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                    break;
                case SeriesSelectionMode.Multiple:
                    this._itemContainer.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
                    break;
            }
        }

        /// <summary>Gets or sets the SelectedIndex property.</summary>
        public int SelectedIndex
        {
            get
            {
                return (int)this.GetValue(DefinitionSeries.SelectedIndexProperty);
            }
            set
            {
                this.SetValue(DefinitionSeries.SelectedIndexProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the SelectedItem property.</summary>
        public object SelectedItem
        {
            get
            {
                return this.GetValue(DefinitionSeries.SelectedItemProperty);
            }
            set
            {
                this.SetValue(DefinitionSeries.SelectedItemProperty, value);
            }
        }

        /// <summary>Gets the currently selected items.</summary>
        /// <remarks>
        /// This property is meant to be used when SelectionMode is Multiple. If the selection mode is Single the correct property to use is SelectedItem.
        /// </remarks>
        public IList SelectedItems
        {
            get
            {
                return (IList)this._selectedItems;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the SelectedItems collection.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._synchronizingSelectedItems)
                return;
            try
            {
                this._synchronizingSelectedItems = true;
                if (null != this._itemContainer)
                {
                    if (NotifyCollectionChangedAction.Reset == e.Action)
                    {
                        if (0 < this._itemContainer.SelectedItems.Count)
                            this._itemContainer.SelectedItems.Clear();
                        foreach (object obj in this._selectedItems.SelectMany<object, DefinitionSeries.DataItem>((Func<object, IEnumerable<DefinitionSeries.DataItem>>)(v => this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => object.Equals(di.Value, v))))))
                            this._itemContainer.SelectedItems.Add(obj);
                    }
                    else
                    {
                        if (null != e.OldItems)
                        {
                            foreach (object obj in e.OldItems.CastWrapper<object>().SelectMany<object, DefinitionSeries.DataItem>((Func<object, IEnumerable<DefinitionSeries.DataItem>>)(v => this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => object.Equals(di.Value, v))))))
                                this._itemContainer.SelectedItems.Remove(obj);
                        }
                        if (null != e.NewItems)
                        {
                            foreach (object obj in e.NewItems.CastWrapper<object>().SelectMany<object, DefinitionSeries.DataItem>((Func<object, IEnumerable<DefinitionSeries.DataItem>>)(v => this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => object.Equals(di.Value, v))))))
                                this._itemContainer.SelectedItems.Add(obj);
                        }
                    }
                }
            }
            finally
            {
                this._synchronizingSelectedItems = false;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ItemContainer class.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ItemContainerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DefinitionSeries.DataItem[] array1 = e.RemovedItems.CastWrapper<DefinitionSeries.DataItem>().ToArray<DefinitionSeries.DataItem>();
            DefinitionSeries.DataItem[] array2 = e.AddedItems.CastWrapper<DefinitionSeries.DataItem>().ToArray<DefinitionSeries.DataItem>();
            if (!this._synchronizingSelectedItems)
            {
                try
                {
                    this._synchronizingSelectedItems = true;
                    foreach (object obj in ((IEnumerable<DefinitionSeries.DataItem>)array1).Select<DefinitionSeries.DataItem, object>((Func<DefinitionSeries.DataItem, object>)(di => di.Value)))
                        this._selectedItems.Remove(obj);
                    foreach (object obj in ((IEnumerable<DefinitionSeries.DataItem>)array2).Select<DefinitionSeries.DataItem, object>((Func<DefinitionSeries.DataItem, object>)(di => di.Value)))
                        this._selectedItems.Add(obj);
                }
                finally
                {
                    this._synchronizingSelectedItems = false;
                }
            }
            IList array3 = (IList)((IEnumerable<DefinitionSeries.DataItem>)array1).Select<DefinitionSeries.DataItem, object>((Func<DefinitionSeries.DataItem, object>)(di => di.Value)).ToArray<object>();
            IList array4 = (IList)((IEnumerable<DefinitionSeries.DataItem>)array2).Select<DefinitionSeries.DataItem, object>((Func<DefinitionSeries.DataItem, object>)(di => di.Value)).ToArray<object>();
            SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
            if (null == selectionChanged)
                return;
            selectionChanged((object)this, new SelectionChangedEventArgs(array3, array4));
        }

        /// <summary>
        /// Occurs when the selection of a DefinitionSeries changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Builds the visual tree for the control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (null != this._itemContainer)
            {
                this._itemContainer.PrepareContainerForItem = (Action<DependencyObject, object>)null;
                this._itemContainer.ClearContainerForItem = (Action<DependencyObject, object>)null;
                this._itemContainer.ItemsSource = (IEnumerable)null;
                this._itemContainer.Clip = (Geometry)null;
                this._itemContainer.SizeChanged -= new SizeChangedEventHandler(this.ItemContainerSizeChanged);
                this._itemContainer.SelectionChanged -= new SelectionChangedEventHandler(this.ItemContainerSelectionChanged);
                this._itemContainer.ClearValue(Selector.SelectedIndexProperty);
                this._itemContainer.ClearValue(Selector.SelectedItemProperty);
            }
            base.OnApplyTemplate();
            this.SeriesArea = this.GetTemplateChild("SeriesArea") as Panel;
            if (null != this.SeriesArea)
            {
                this._seriesAreaChildrenListAdapter.TargetList = (IList)this.SeriesArea.Children;
                this._seriesAreaChildrenListAdapter.Populate();
            }
            this._itemContainer = this.GetTemplateChild("ItemContainer") as DelegatingListBox;
            if (null != this._itemContainer)
            {
                this._itemContainer.PrepareContainerForItem = new Action<DependencyObject, object>(this.PrepareContainerForItem);
                this._itemContainer.ClearContainerForItem = new Action<DependencyObject, object>(this.ClearContainerForItem);
                this._itemContainer.ItemsSource = (IEnumerable)this.DataItems;
                this._itemContainer.Clip = (Geometry)this._clipGeometry;
                this._itemContainer.SizeChanged += new SizeChangedEventHandler(this.ItemContainerSizeChanged);
                this._itemContainer.SelectionChanged += new SelectionChangedEventHandler(this.ItemContainerSelectionChanged);
                this._itemContainer.SetBinding(Selector.SelectedIndexProperty, new Binding("SelectedIndex")
                {
                    Source = (object)this,
                    Mode = BindingMode.TwoWay
                });
                this._itemContainer.SetBinding(Selector.SelectedItemProperty, new Binding("SelectedItem")
                {
                    Source = (object)this,
                    Mode = BindingMode.TwoWay,
                    Converter = (IValueConverter)new DefinitionSeries.SelectedItemToDataItemConverter(this.DataItems)
                });
            }
            this.OnSelectionModeChanged(SeriesSelectionMode.None, this.SelectionMode);
            this.SelectedItemsCollectionChanged((object)null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        private void PrepareContainerForItem(DependencyObject element, object item)
        {
            DefinitionSeries.DataItem dataItem = (DefinitionSeries.DataItem)item;
            DataPoint dataPoint = this.CreateDataPoint();
            dataItem.DataPoint = dataPoint;
            dataPoint.DataContext = dataItem.Value;
            dataPoint.SetBinding(DataPoint.DependentValueProperty, dataItem.SeriesDefinition.DependentValueBinding);
            dataPoint.SetBinding(DataPoint.IndependentValueProperty, dataItem.SeriesDefinition.IndependentValueBinding);
            dataPoint.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualDataPointStyle")
            {
                Source = (object)dataItem.SeriesDefinition
            });
            dataPoint.DependentValueChanged += new RoutedPropertyChangedEventHandler<IComparable>(this.DataPointDependentValueChanged);
            dataPoint.ActualDependentValueChanged += new RoutedPropertyChangedEventHandler<IComparable>(this.DataPointActualDependentValueChanged);
            dataPoint.IndependentValueChanged += new RoutedPropertyChangedEventHandler<object>(this.DataPointIndependentValueChanged);
            dataPoint.ActualIndependentValueChanged += new RoutedPropertyChangedEventHandler<object>(this.DataPointActualIndependentValueChanged);
            dataPoint.StateChanged += new RoutedPropertyChangedEventHandler<DataPointState>(this.DataPointStateChanged);
            dataPoint.DefinitionSeriesIsSelectionEnabledHandling = true;
            ContentControl contentControl = (ContentControl)element;
            dataItem.Container = (UIElement)contentControl;
            Binding binding = new Binding("SelectionMode")
            {
                Source = (object)this,
                Converter = (IValueConverter)new DefinitionSeries.SelectionModeToSelectionEnabledConverter()
            };
            contentControl.SetBinding(Control.IsTabStopProperty, binding);
            dataPoint.SetBinding(DataPoint.IsSelectionEnabledProperty, binding);
            dataPoint.SetBinding(DataPoint.IsSelectedProperty, new Binding("IsSelected")
            {
                Source = (object)contentControl,
                Mode = BindingMode.TwoWay
            });
            dataPoint.Visibility = Visibility.Collapsed;
            dataPoint.State = DataPointState.Showing;
            this.PrepareDataPoint(dataPoint);
            contentControl.Content = (object)dataPoint;
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item to display.</param>
        private void ClearContainerForItem(DependencyObject element, object item)
        {
            DefinitionSeries.DataItem dataItem = (DefinitionSeries.DataItem)item;
            DataPoint dataPoint = dataItem.DataPoint;
            dataPoint.DependentValueChanged -= new RoutedPropertyChangedEventHandler<IComparable>(this.DataPointDependentValueChanged);
            dataPoint.ActualDependentValueChanged -= new RoutedPropertyChangedEventHandler<IComparable>(this.DataPointActualDependentValueChanged);
            dataPoint.IndependentValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.DataPointIndependentValueChanged);
            dataPoint.ActualIndependentValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.DataPointActualIndependentValueChanged);
            dataPoint.StateChanged -= new RoutedPropertyChangedEventHandler<DataPointState>(this.DataPointStateChanged);
            dataPoint.ClearValue(DataPoint.DependentValueProperty);
            dataPoint.ClearValue(DataPoint.IndependentValueProperty);
            dataPoint.ClearValue(FrameworkElement.StyleProperty);
            dataPoint.ClearValue(DataPoint.IsSelectionEnabledProperty);
            dataPoint.ClearValue(DataPoint.IsSelectedProperty);
            dataItem.Container.ClearValue(Control.IsTabStopProperty);
            dataPoint.DataContext = (object)null;
        }

        /// <summary>Prepares a DataPoint for use.</summary>
        /// <param name="dataPoint">DataPoint instance.</param>
        protected virtual void PrepareDataPoint(DataPoint dataPoint)
        {
        }

        /// <summary>Creates a DataPoint for the series.</summary>
        /// <returns>Series-appropriate DataPoint instance.</returns>
        protected abstract DataPoint CreateDataPoint();

        /// <summary>
        /// Provides an internally-accessible wrapper for calling CreateDataPoint.
        /// </summary>
        /// <returns>Series-appropriate DataPoint instance.</returns>
        internal DataPoint InternalCreateDataPoint()
        {
            return this.CreateDataPoint();
        }

        /// <summary>Handles the SizeChanged event of the ItemContainer.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ItemContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry clipGeometry = this._clipGeometry;
            double x = 0.0;
            double y = 0.0;
            Size newSize = e.NewSize;
            double width = newSize.Width;
            newSize = e.NewSize;
            double height = newSize.Height;
            Rect rect = new Rect(x, y, width, height);
            clipGeometry.Rect = rect;
            this.QueueUpdateDataItemPlacement(false, false, (IEnumerable<DefinitionSeries.DataItem>)this.DataItems);
        }

        /// <summary>
        /// Returns the DataItem corresponding to the specified DataPoint.
        /// </summary>
        /// <param name="dataPoint">Specified DataPoint.</param>
        /// <returns>Corresponding DataItem.</returns>
        protected DefinitionSeries.DataItem DataItemFromDataPoint(DataPoint dataPoint)
        {
            return this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.DataPoint == dataPoint)).Single<DefinitionSeries.DataItem>();
        }

        /// <summary>
        /// Handles the DependentValueChanged event of a DataPoint.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void DataPointDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> e)
        {
            DataPoint dataPoint = (DataPoint)sender;
            SeriesDefinition seriesDefinition = this.DataItemFromDataPoint(dataPoint).SeriesDefinition;
            if (0.0 < seriesDefinition.TransitionDuration.TotalMilliseconds)
                dataPoint.BeginAnimation(DataPoint.ActualDependentValueProperty, "ActualDependentValue", (object)e.NewValue, seriesDefinition.TransitionDuration, seriesDefinition.TransitionEasingFunction);
            else
                dataPoint.ActualDependentValue = e.NewValue;
        }

        /// <summary>
        /// Handles the ActualDependentValueChanged event of a DataPoint.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void DataPointActualDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> e)
        {
            DataPoint dataPoint = (DataPoint)sender;
            this.QueueUpdateDataItemPlacement(true, false, this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.DataPoint == dataPoint)));
        }

        /// <summary>
        /// Handles the IndependentValueChanged event of a DataPoint.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void DataPointIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataPoint dataPoint = (DataPoint)sender;
            SeriesDefinition seriesDefinition = this.DataItemFromDataPoint(dataPoint).SeriesDefinition;
            if (0.0 < seriesDefinition.TransitionDuration.TotalMilliseconds)
                dataPoint.BeginAnimation(DataPoint.ActualIndependentValueProperty, "ActualIndependentValue", e.NewValue, seriesDefinition.TransitionDuration, seriesDefinition.TransitionEasingFunction);
            else
                dataPoint.ActualIndependentValue = e.NewValue;
        }

        /// <summary>
        /// Handles the ActualIndependentValueChanged event of a DataPoint.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void DataPointActualIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataPoint dataPoint = (DataPoint)sender;
            this.QueueUpdateDataItemPlacement(false, true, this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.DataPoint == dataPoint)));
        }

        /// <summary>Handles the StateChanged event of a DataPoint.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void DataPointStateChanged(object sender, RoutedPropertyChangedEventArgs<DataPointState> e)
        {
            DataPoint dataPoint = (DataPoint)sender;
            if (DataPointState.Hidden != dataPoint.State)
                return;
            this.DataItems.Remove(this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.DataPoint == dataPoint)).Single<DefinitionSeries.DataItem>());
            this.RemovedDataItems();
        }

        /// <summary>
        /// Notifies the specified axis of changes to values plotting against it.
        /// </summary>
        /// <param name="axis">Specified axis.</param>
        protected void NotifyAxisValuesChanged(IAxis axis)
        {
            if (null == axis)
                return;
            IRangeConsumer rangeConsumer = axis as IRangeConsumer;
            if (null != rangeConsumer)
            {
                IRangeProvider provider = (IRangeProvider)this;
                rangeConsumer.RangeChanged(provider, new Range<IComparable>());
            }
            IDataConsumer dataConsumer = axis as IDataConsumer;
            if (null != dataConsumer)
            {
                IDataProvider dataProvider = (IDataProvider)this;
                dataConsumer.DataChanged(dataProvider, (IEnumerable<object>)null);
            }
        }

        /// <summary>
        /// Notifies the specified axis of changes to value margins plotting against it.
        /// </summary>
        /// <param name="axis">Specified axis.</param>
        /// <param name="valueMargins">Sequence of value margins that have changed.</param>
        protected void NotifyValueMarginsChanged(IAxis axis, IEnumerable<ValueMargin> valueMargins)
        {
            if (null == axis)
                return;
            IValueMarginConsumer valueMarginConsumer = axis as IValueMarginConsumer;
            if (null != valueMarginConsumer)
            {
                IValueMarginProvider provider = (IValueMarginProvider)this;
                valueMarginConsumer.ValueMarginsChanged(provider, valueMargins);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the SeriesDefinitions collection.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void SeriesDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SeriesDefinitionsCollectionChanged(e.Action, e.OldItems, e.OldStartingIndex, e.NewItems, e.NewStartingIndex);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the SeriesDefinitions collection.
        /// </summary>
        /// <param name="action">Type of change.</param>
        /// <param name="oldItems">Sequence of old items.</param>
        /// <param name="oldStartingIndex">Starting index of old items.</param>
        /// <param name="newItems">Sequence of new items.</param>
        /// <param name="newStartingIndex">Starting index of new items.</param>
        protected virtual void SeriesDefinitionsCollectionChanged(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex, IList newItems, int newStartingIndex)
        {
            if (null != oldItems)
            {
                foreach (SeriesDefinition definition in oldItems.CastWrapper<SeriesDefinition>())
                {
                    ISeries series = (ISeries)definition;
                    this.SeriesDefinitionItemsSourceChanged(definition, definition.ItemsSource, (IEnumerable)null);
                    this._seriesDefinitionsAsISeries.Remove((ISeries)definition);
                    this._legendItems.ChildCollections.Remove((IList)series.LegendItems);
                    this.UpdatePaletteProperties(definition);
                    series.SeriesHost = (ISeriesHost)null;
                    definition.Index = -1;
                }
            }
            if (null == newItems)
                return;
            int num = newStartingIndex;
            foreach (SeriesDefinition definition in newItems.CastWrapper<SeriesDefinition>())
            {
                ISeries series = (ISeries)definition;
                series.SeriesHost = (ISeriesHost)this;
                this.UpdatePaletteProperties(definition);
                this._legendItems.ChildCollections.Add((IList)series.LegendItems);
                this._seriesDefinitionsAsISeries.Add((ISeries)definition);
                definition.Index = num;
                this.SeriesDefinitionItemsSourceChanged(definition, (IEnumerable)null, definition.ItemsSource);
                ++num;
            }
        }

        /// <summary>
        /// Updates the palette properties of the specified SeriesDefinition.
        /// </summary>
        /// <param name="definition">Specified SeriesDefinition.</param>
        private void UpdatePaletteProperties(SeriesDefinition definition)
        {
            ResourceDictionary resourceDictionary = (ResourceDictionary)null;
            if (null != this.SeriesHost)
            {
                Type dataPointType = this.CreateDataPoint().GetType();
                using (IEnumerator<ResourceDictionary> dictionariesWhere = this.SeriesHost.GetResourceDictionariesWhere((Func<ResourceDictionary, bool>)(dictionary =>
                {
                    Style style = dictionary[(object)"DataPointStyle"] as Style;
                    if (null != style)
                        return style.TargetType != null && style.TargetType.IsAssignableFrom(dataPointType);
                    return false;
                })))
                {
                    if (dictionariesWhere.MoveNext())
                        resourceDictionary = dictionariesWhere.Current;
                }
            }
            definition.PaletteDataPointStyle = resourceDictionary != null ? resourceDictionary[(object)"DataPointStyle"] as Style : (Style)null;
            definition.PaletteDataShapeStyle = resourceDictionary != null ? resourceDictionary[(object)"DataShapeStyle"] as Style : (Style)null;
            definition.PaletteLegendItemStyle = resourceDictionary != null ? resourceDictionary[(object)"LegendItemStyle"] as Style : (Style)null;
        }

        /// <summary>
        /// Handles changes to the ItemsSource of a SeriesDefinition.
        /// </summary>
        /// <param name="definition">SeriesDefinition owner.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        internal void SeriesDefinitionItemsSourceChanged(SeriesDefinition definition, IEnumerable oldValue, IEnumerable newValue)
        {
            if (null != oldValue)
            {
                foreach (DefinitionSeries.DataItem dataItem in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition)).ToArray<DefinitionSeries.DataItem>())
                    this.DataItems.Remove(dataItem);
                this.RemovedDataItems();
            }
            if (null == newValue || null == this.SeriesHost)
                return;
            this.AddDataItems(definition, newValue.CastWrapper<object>(), 0);
        }

        /// <summary>
        /// Handles changes to the ItemsSource collection  of a SeriesDefinition.
        /// </summary>
        /// <param name="definition">SeriesDefinition owner.</param>
        /// <param name="action">Type of change.</param>
        /// <param name="oldItems">Sequence of old items.</param>
        /// <param name="oldStartingIndex">Starting index of old items.</param>
        /// <param name="newItems">Sequence of new items.</param>
        /// <param name="newStartingIndex">Starting index of new items.</param>
        internal void SeriesDefinitionItemsSourceCollectionChanged(SeriesDefinition definition, NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex, IList newItems, int newStartingIndex)
        {
            if (NotifyCollectionChangedAction.Replace == action)
            {
                foreach (DefinitionSeries.DataItem dataItem in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition && newStartingIndex <= di.Index && di.Index < newStartingIndex + newItems.Count)))
                    dataItem.Value = newItems[dataItem.Index - newStartingIndex];
            }
            else
            {
                if (NotifyCollectionChangedAction.Reset == action)
                {
                    Debug.Assert(null == oldItems, "Reset action with non-null oldItems.");
                    oldItems = (IList)this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition)).ToArray<DefinitionSeries.DataItem>();
                    oldStartingIndex = 0;
                    newItems = (IList)definition.ItemsSource.CastWrapper<object>().ToArray<object>();
                    newStartingIndex = 0;
                }
                if (null != oldItems)
                {
                    foreach (DefinitionSeries.DataItem dataItem in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition && oldStartingIndex <= di.Index && di.Index < oldStartingIndex + oldItems.Count)))
                    {
                        dataItem.Index = -1;
                        if (null != dataItem.DataPoint)
                            dataItem.DataPoint.State = DataPointState.Hiding;
                    }
                    foreach (DefinitionSeries.DataItem dataItem in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition && oldStartingIndex + oldItems.Count <= di.Index)))
                        dataItem.Index -= oldItems.Count;
                }
                if (null != newItems)
                {
                    foreach (DefinitionSeries.DataItem dataItem in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.SeriesDefinition == definition && newStartingIndex <= di.Index)))
                        dataItem.Index += newItems.Count;
                    this.AddDataItems(definition, newItems.CastWrapper<object>(), newStartingIndex);
                }
            }
            foreach (IGrouping<SeriesDefinition, DefinitionSeries.DataItem> grouping in this.DataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => 0 <= di.Index)).OrderBy<DefinitionSeries.DataItem, int>((Func<DefinitionSeries.DataItem, int>)(di => di.Index)).GroupBy<DefinitionSeries.DataItem, SeriesDefinition>((Func<DefinitionSeries.DataItem, SeriesDefinition>)(di => di.SeriesDefinition)))
            {
                object[] array = grouping.Key.ItemsSource.CastWrapper<object>().ToArray<object>();
                int index = 0;
                foreach (DefinitionSeries.DataItem dataItem in (IEnumerable<DefinitionSeries.DataItem>)grouping)
                {
                    Debug.Assert(index == dataItem.Index, "DataItem index mis-match.");
                    Debug.Assert(dataItem.Value.Equals(array[index]), "DataItem value mis-match.");
                    ++index;
                }
            }
        }

        /// <summary>
        /// Handles the ResourceDictionariesChanged event of the SeriesHost owner.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void SeriesHostResourceDictionariesChanged(object sender, EventArgs e)
        {
            foreach (SeriesDefinition seriesDefinition in this.SeriesDefinitions)
                this.UpdatePaletteProperties(seriesDefinition);
        }

        /// <summary>
        /// Creates and adds DataItems for the specified SeriesDefinition's items.
        /// </summary>
        /// <param name="definition">Specified SeriesDefinition.</param>
        /// <param name="items">Sequence of items.</param>
        /// <param name="startingIndex">Starting index.</param>
        private void AddDataItems(SeriesDefinition definition, IEnumerable<object> items, int startingIndex)
        {
            int num = startingIndex;
            foreach (object obj in items)
            {
                this.DataItems.Add(new DefinitionSeries.DataItem(definition)
                {
                    Value = obj,
                    Index = num
                });
                ++num;
            }
            this.Dispatcher.BeginInvoke(new Action(this.AddedDataItems));
        }

        /// <summary>Updates the axes after DataItems have been added.</summary>
        private void AddedDataItems()
        {
            this.EnsureAxes(false, false, true);
        }

        /// <summary>Notifies the axes after DataItems have been removed.</summary>
        private void RemovedDataItems()
        {
            this.NotifyAxisValuesChanged(this.ActualIndependentAxis);
            this.NotifyAxisValuesChanged(this.ActualDependentAxis);
        }

        /// <summary>
        /// Ensures that suitable axes are present and registered.
        /// </summary>
        /// <param name="updateDependentAxis">True if the dependent axis needs to be updated.</param>
        /// <param name="updateIndependentAxis">True if the independent axis needs to be updated.</param>
        /// <param name="unconditionallyNotifyAxes">True if both axis are to be notified unconditionally.</param>
        private void EnsureAxes(bool updateDependentAxis, bool updateIndependentAxis, bool unconditionallyNotifyAxes)
        {
            foreach (SeriesDefinition seriesDefinition in this.SeriesDefinitions)
            {
                if (null == seriesDefinition.DependentValueBinding)
                    throw new InvalidOperationException("DefinitionSeries.EnsureAxes: Missing Dependent Value Binding");
                if (null == seriesDefinition.IndependentValueBinding)
                    throw new InvalidOperationException("DefinitionSeries.EnsureAxes: Missing Independent Value Binding");
            }
            if (this.SeriesHost == null || !this.DataItems.Any<DefinitionSeries.DataItem>())
                return;
            bool flag1 = false;
            if (updateDependentAxis && null != this.ActualDependentAxis)
            {
                this.ActualDependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                this.ActualDependentAxis = (IAxis)null;
            }
            if (null == this.ActualDependentAxis)
            {
                this.ActualDependentAxis = this.DependentAxis ?? this.AcquireDependentAxis();
                this.ActualDependentAxis.RegisteredListeners.Add((IAxisListener)this);
                if (!this.SeriesHost.Axes.Contains(this.ActualDependentAxis))
                    this.SeriesHost.Axes.Add(this.ActualDependentAxis);
                flag1 = true;
            }
            bool flag2 = false;
            if (updateIndependentAxis && null != this.ActualIndependentAxis)
            {
                this.ActualIndependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                this.ActualIndependentAxis = (IAxis)null;
            }
            if (null == this.ActualIndependentAxis)
            {
                this.ActualIndependentAxis = this.IndependentAxis ?? this.AcquireIndependentAxis();
                this.ActualIndependentAxis.RegisteredListeners.Add((IAxisListener)this);
                if (!this.SeriesHost.Axes.Contains(this.ActualIndependentAxis))
                    this.SeriesHost.Axes.Add(this.ActualIndependentAxis);
                flag2 = true;
            }
            if (flag1 || flag2 || unconditionallyNotifyAxes)
                this.QueueUpdateDataItemPlacement(flag1 || unconditionallyNotifyAxes, flag2 || unconditionallyNotifyAxes, (IEnumerable<DefinitionSeries.DataItem>)this.DataItems);
        }

        /// <summary>
        /// Acquires a dependent axis suitable for use with the data values of the series.
        /// </summary>
        /// <returns>Axis instance.</returns>
        protected abstract IAxis AcquireDependentAxis();

        /// <summary>
        /// Acquires an independent axis suitable for use with the data values of the series.
        /// </summary>
        /// <returns>Axis instance.</returns>
        protected abstract IAxis AcquireIndependentAxis();

        void IAxisListener.AxisInvalidated(IAxis axis)
        {
            this.QueueUpdateDataItemPlacement(false, false, (IEnumerable<DefinitionSeries.DataItem>)this.DataItems);
        }

        /// <summary>
        /// Queues an update of DataItem placement for the next update opportunity.
        /// </summary>
        /// <param name="dependentAxisValuesChanged">True if the dependent axis values have changed.</param>
        /// <param name="independentAxisValuesChanged">True if the independent axis values have changed.</param>
        /// <param name="dataItems">Sequence of DataItems to update.</param>
        private void QueueUpdateDataItemPlacement(bool dependentAxisValuesChanged, bool independentAxisValuesChanged, IEnumerable<DefinitionSeries.DataItem> dataItems)
        {
            this._queueUpdateDataItemPlacement_DependentAxisValuesChanged |= dependentAxisValuesChanged;
            this._queueUpdateDataItemPlacement_IndependentAxisValuesChanged |= independentAxisValuesChanged;
            this._queueUpdateDataItemPlacement_DataItems.AddRange(dataItems);
            this.InvalidateArrange();
        }

        /// <summary>
        /// Called when the control needs to arrange its children.
        /// </summary>
        /// <param name="arrangeBounds">Bounds to arrange within.</param>
        /// <returns>Arranged size.</returns>
        /// <remarks>Used as a good place to dequeue queued work.</remarks>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            if (this._queueUpdateDataItemPlacement_DependentAxisValuesChanged)
            {
                this.NotifyAxisValuesChanged(this.ActualDependentAxis);
                this._queueUpdateDataItemPlacement_DependentAxisValuesChanged = false;
            }
            if (this._queueUpdateDataItemPlacement_IndependentAxisValuesChanged)
            {
                this.NotifyAxisValuesChanged(this.ActualIndependentAxis);
                this._queueUpdateDataItemPlacement_IndependentAxisValuesChanged = false;
            }
            this.UpdateDataItemPlacement(this._queueUpdateDataItemPlacement_DataItems.Distinct<DefinitionSeries.DataItem>());
            this._queueUpdateDataItemPlacement_DataItems.Clear();
            return size;
        }

        /// <summary>
        /// Updates the placement of the DataItems (data points) of the series.
        /// </summary>
        /// <param name="dataItems">DataItems in need of an update.</param>
        protected abstract void UpdateDataItemPlacement(IEnumerable<DefinitionSeries.DataItem> dataItems);

        Range<IComparable> IRangeProvider.GetRange(IRangeConsumer rangeConsumer)
        {
            return this.IRangeProviderGetRange(rangeConsumer);
        }

        /// <summary>Returns the range for the data points of the series.</summary>
        /// <param name="rangeConsumer">Consumer of the range.</param>
        /// <returns>Range of values.</returns>
        protected virtual Range<IComparable> IRangeProviderGetRange(IRangeConsumer rangeConsumer)
        {
            if (rangeConsumer != this.ActualIndependentAxis)
                throw new NotSupportedException();
            if (this.ActualIndependentAxis.CanPlot((object)0.0))
                return this.IndependentValueGroups.Select<DefinitionSeries.IndependentValueGroup, double>((Func<DefinitionSeries.IndependentValueGroup, double>)(g => ValueHelper.ToDouble(g.IndependentValue))).Where<double>((Func<double, bool>)(d => ValueHelper.CanGraph(d))).DefaultIfEmpty<double>().CastWrapper<IComparable>().GetRange<IComparable>();
            return this.IndependentValueGroups.Select<DefinitionSeries.IndependentValueGroup, DateTime>((Func<DefinitionSeries.IndependentValueGroup, DateTime>)(g => ValueHelper.ToDateTime(g.IndependentValue))).DefaultIfEmpty<DateTime>().CastWrapper<IComparable>().GetRange<IComparable>();
        }

        IEnumerable<ValueMargin> IValueMarginProvider.GetValueMargins(IValueMarginConsumer valueMarginConsumer)
        {
            return this.IValueMarginProviderGetValueMargins(valueMarginConsumer);
        }

        /// <summary>
        /// Returns the value margins for the data points of the series.
        /// </summary>
        /// <param name="valueMarginConsumer">Consumer of the value margins.</param>
        /// <returns>Sequence of value margins.</returns>
        protected virtual IEnumerable<ValueMargin> IValueMarginProviderGetValueMargins(IValueMarginConsumer valueMarginConsumer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<object> IDataProvider.GetData(IDataConsumer dataConsumer)
        {
            return this.IDataProviderGetData(dataConsumer);
        }

        /// <summary>Returns the data for the data points of the series.</summary>
        /// <param name="dataConsumer">Consumer of the data.</param>
        /// <returns>Sequence of data.</returns>
        protected virtual IEnumerable<object> IDataProviderGetData(IDataConsumer dataConsumer)
        {
            if (dataConsumer == this.ActualIndependentAxis)
                return this.IndependentValueGroups.Select<DefinitionSeries.IndependentValueGroup, object>((Func<DefinitionSeries.IndependentValueGroup, object>)(cg => cg.IndependentValue)).Distinct<object>();
            throw new NotImplementedException();
        }

        /// <summary>Gets a sequence of IndependentValueGroups.</summary>
        protected virtual IEnumerable<DefinitionSeries.IndependentValueGroup> IndependentValueGroups
        {
            get
            {
                return this.DataItems.GroupBy<DefinitionSeries.DataItem, object>((Func<DefinitionSeries.DataItem, object>)(di => di.ActualIndependentValue)).Select<IGrouping<object, DefinitionSeries.DataItem>, DefinitionSeries.IndependentValueGroup>((Func<IGrouping<object, DefinitionSeries.DataItem>, DefinitionSeries.IndependentValueGroup>)(g => new DefinitionSeries.IndependentValueGroup(g.Key, (IEnumerable<DefinitionSeries.DataItem>)g.OrderBy<DefinitionSeries.DataItem, int>((Func<DefinitionSeries.DataItem, int>)(di => di.SeriesDefinition.Index)))));
            }
        }

        /// <summary>
        /// Gets a sequence of IndependentValueGroups ordered by independent value.
        /// </summary>
        protected IEnumerable<DefinitionSeries.IndependentValueGroup> IndependentValueGroupsOrderedByIndependentValue
        {
            get
            {
                return (IEnumerable<DefinitionSeries.IndependentValueGroup>)this.IndependentValueGroups.OrderBy<DefinitionSeries.IndependentValueGroup, object>((Func<DefinitionSeries.IndependentValueGroup, object>)(g => g.IndependentValue));
            }
        }

        /// <summary>
        /// Gets a sequence of sequences of the dependent values associated with each independent value.
        /// </summary>
        protected IEnumerable<IEnumerable<double>> IndependentValueDependentValues
        {
            get
            {
                return this.IndependentValueGroups.Select<DefinitionSeries.IndependentValueGroup, DefinitionSeries.IndependentValueGroup>((Func<DefinitionSeries.IndependentValueGroup, DefinitionSeries.IndependentValueGroup>)(g =>
                {
                    g.Denominator = this.IsStacked100 ? g.DataItems.Sum<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, double>)(di => Math.Abs(ValueHelper.ToDouble((object)di.ActualDependentValue)))) : 1.0;
                    if (0.0 == g.Denominator)
                        g.Denominator = 1.0;
                    return g;
                })).Select<DefinitionSeries.IndependentValueGroup, IEnumerable<double>>((Func<DefinitionSeries.IndependentValueGroup, IEnumerable<double>>)(g => g.DataItems.Select<DefinitionSeries.DataItem, double>((Func<DefinitionSeries.DataItem, double>)(di => ValueHelper.ToDouble((object)di.ActualDependentValue) * (this.IsStacked100 ? 100.0 / g.Denominator : 1.0)))));
            }
        }

        ObservableCollection<IAxis> ISeriesHost.Axes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ObservableCollection<ISeries> ISeriesHost.Series
        {
            get
            {
                return this._seriesDefinitionsAsISeries;
            }
        }

        ObservableCollection<UIElement> ISeriesHost.ForegroundElements
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ObservableCollection<UIElement> ISeriesHost.BackgroundElements
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnumerator<ResourceDictionary> IResourceDictionaryDispenser.GetResourceDictionariesWhere(Func<ResourceDictionary, bool> predicate)
        {
            throw new NotImplementedException();
        }

        event EventHandler IResourceDictionaryDispenser.ResourceDictionariesChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Represents an independent value and the dependent values that are associated with it.
        /// </summary>
        protected class IndependentValueGroup
        {
            /// <summary>
            /// Initializes a new instance of the IndependentValueGroup class.
            /// </summary>
            /// <param name="independentValue">Independent value.</param>
            /// <param name="dataItems">Associated DataItems.</param>
            public IndependentValueGroup(object independentValue, IEnumerable<DefinitionSeries.DataItem> dataItems)
            {
                this.IndependentValue = independentValue;
                this.DataItems = dataItems;
            }

            /// <summary>Gets the independent value.</summary>
            public object IndependentValue { get; private set; }

            /// <summary>
            /// Gets a sequence of DataItems associated with the independent value.
            /// </summary>
            public IEnumerable<DefinitionSeries.DataItem> DataItems { get; private set; }

            /// <summary>
            /// Gets or sets the denominator to use when computing with this instance.
            /// </summary>
            /// <remarks>
            /// Exists here purely to simplify the the corresponding algorithm.
            /// </remarks>
            public double Denominator { get; set; }
        }

        /// <summary>
        /// Represents a single data value from a SeriesDefinition's ItemsSource.
        /// </summary>
        protected class DataItem
        {
            /// <summary>
            /// Stores a reference to a shared BindingHelper instance.
            /// </summary>
            private static readonly DefinitionSeries.BindingHelper _bindingHelper = new DefinitionSeries.BindingHelper();
            /// <summary>Stores the value of the DataItem.</summary>
            private object _value;

            /// <summary>Initializes a new instance of the DataItem class.</summary>
            /// <param name="seriesDefinition">SeriesDefinition owner.</param>
            public DataItem(SeriesDefinition seriesDefinition)
            {
                this.SeriesDefinition = seriesDefinition;
                this.CenterPoint = new Point(double.NaN, double.NaN);
            }

            /// <summary>Gets the SeriesDefinition owner of the DataItem.</summary>
            public SeriesDefinition SeriesDefinition { get; private set; }

            /// <summary>Gets or sets the value of the DataItem.</summary>
            public object Value
            {
                get
                {
                    return this._value;
                }
                set
                {
                    this._value = value;
                    if (null == this.DataPoint)
                        return;
                    this.DataPoint.DataContext = value;
                }
            }

            /// <summary>Gets or sets the index of the DataItem.</summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the DataPoint associated with the DataItem.
            /// </summary>
            public DataPoint DataPoint { get; set; }

            /// <summary>
            /// Gets or sets the container for the DataPoint within its parent ItemsControl.
            /// </summary>
            public UIElement Container { get; set; }

            /// <summary>
            /// Gets the ActualDependentValue of the DataPoint (or its equivalent).
            /// </summary>
            public IComparable ActualDependentValue
            {
                get
                {
                    if (null != this.DataPoint)
                        return this.DataPoint.ActualDependentValue;
                    return (IComparable)DefinitionSeries.DataItem._bindingHelper.EvaluateBinding(this.SeriesDefinition.DependentValueBinding, this.Value);
                }
            }

            /// <summary>
            /// Gets the ActualIndependentValue of the DataPoint (or its equivalent).
            /// </summary>
            public object ActualIndependentValue
            {
                get
                {
                    if (null != this.DataPoint)
                        return this.DataPoint.ActualIndependentValue;
                    return DefinitionSeries.DataItem._bindingHelper.EvaluateBinding(this.SeriesDefinition.IndependentValueBinding, this.Value);
                }
            }

            /// <summary>
            /// Gets or sets the ActualDependentValue of the DataPoint after adjusting for applicable stacking.
            /// </summary>
            public double ActualStackedDependentValue { get; set; }

            /// <summary>
            /// Gets or sets the center-point of the DataPoint in plot area coordinates (if relevant).
            /// </summary>
            public Point CenterPoint { get; set; }
        }

        /// <summary>
        /// Provides an easy way to evaluate a Binding against a source instance.
        /// </summary>
        private class BindingHelper : FrameworkElement
        {
            /// <summary>Identifies the Result dependency property.</summary>
            private static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(object), typeof(DefinitionSeries.BindingHelper), (PropertyMetadata)null);

            /// <summary>Evaluates a Binding against a source instance.</summary>
            /// <param name="binding">Binding to evaluate.</param>
            /// <param name="instance">Source instance.</param>
            /// <returns>Result of Binding on source instance.</returns>
            public object EvaluateBinding(Binding binding, object instance)
            {
                this.DataContext = instance;
                this.SetBinding(DefinitionSeries.BindingHelper.ResultProperty, binding);
                object obj = this.GetValue(DefinitionSeries.BindingHelper.ResultProperty);
                this.ClearValue(DefinitionSeries.BindingHelper.ResultProperty);
                this.DataContext = (object)null;
                return obj;
            }
        }

        /// <summary>
        /// Converts from a selected item to the corresponding DataItem.
        /// </summary>
        private class SelectedItemToDataItemConverter : IValueConverter
        {
            /// <summary>Stores a reference to the DataItem collection.</summary>
            private ObservableCollection<DefinitionSeries.DataItem> _dataItems;

            /// <summary>
            /// Initializes a new instance of the SelectedItemToDataItemConverter class.
            /// </summary>
            /// <param name="dataItems">Collection of DataItems.</param>
            public SelectedItemToDataItemConverter(ObservableCollection<DefinitionSeries.DataItem> dataItems)
            {
                this._dataItems = dataItems;
            }

            /// <summary>Converts a value.</summary>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>Converted value.</returns>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (object)this._dataItems.Where<DefinitionSeries.DataItem>((Func<DefinitionSeries.DataItem, bool>)(di => di.Value == value)).FirstOrDefault<DefinitionSeries.DataItem>();
            }

            /// <summary>Converts a value back.</summary>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>Converted value.</returns>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                DefinitionSeries.DataItem dataItem = value as DefinitionSeries.DataItem;
                return dataItem != null ? dataItem.Value : (object)null;
            }
        }

        /// <summary>
        /// Converts from a SeriesSelectionMode to a true/false value indicating whether selection is enabled.
        /// </summary>
        private class SelectionModeToSelectionEnabledConverter : IValueConverter
        {
            /// <summary>Converts a value.</summary>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>Converted value.</returns>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool flag = false;
                if (value is SeriesSelectionMode)
                    flag = SeriesSelectionMode.None != (SeriesSelectionMode)value;
                return (object)flag;
            }

            /// <summary>Converts a value back.</summary>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>Converted value.</returns>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
