// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls.DataVisualization.Collections;

#if MIGRATION
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif
#if !SILVERLIGHT
using System.Diagnostics.CodeAnalysis;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>Specifies the supported animation sequences.</summary>
    /// <QualityBand>Preview</QualityBand>
    public enum AnimationSequence
    {
        Simultaneous,
        FirstToLast,
        LastToFirst,
    }

    /// <summary>Describes the state a data point is in.</summary>
    public enum DataPointState
    {
        Created,
        Showing,
        Normal,
        PendingRemoval,
        Hiding,
        Hidden,
    }

    /// <summary>
    /// Represents a control that contains a dynamic data series.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class DataPointSeries : Series
    {
        /// <summary>Identifies the ItemsSource dependency property.</summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataPointSeries), new PropertyMetadata(new PropertyChangedCallback(DataPointSeries.OnItemsSourceChanged)));
        /// <summary>Queue of hide/reveal storyboards to play.</summary>
        private StoryboardQueue _storyBoardQueue = new StoryboardQueue();
        /// <summary>Data points collection sorted by object.</summary>
        private MultipleDictionary<object, DataPoint> _dataPointsByObject = new MultipleDictionary<object, DataPoint>(true, (IEqualityComparer<object>)new GenericEqualityComparer<object>((Func<object, object, bool>)((left, right) => left.Equals(right)), (Func<object, int>)(obj => obj.GetHashCode())), (IEqualityComparer<DataPoint>)new GenericEqualityComparer<DataPoint>((Func<DataPoint, DataPoint, bool>)((left, right) => object.ReferenceEquals((object)left, (object)right)), (Func<DataPoint, int>)(obj => obj.GetHashCode())));
        /// <summary>
        /// Indicates whether a call to Refresh is required when the control's
        /// size changes.
        /// </summary>
        private bool _needRefreshWhenSizeChanged = true;
        /// <summary>The name of the template part with the plot area.</summary>
        protected const string PlotAreaName = "PlotArea";
        /// <summary>
        /// The name of the DataPointStyle property and ResourceDictionary entry.
        /// </summary>
        protected const string DataPointStyleName = "DataPointStyle";
        /// <summary>
        /// The name of the LegendItemStyle property and ResourceDictionary entry.
        /// </summary>
        protected const string LegendItemStyleName = "LegendItemStyle";
        /// <summary>The name of the ActualLegendItemStyle property.</summary>
        protected internal const string ActualLegendItemStyleName = "ActualLegendItemStyle";
        /// <summary>
        /// The binding used to identify the dependent value binding.
        /// </summary>
        private Binding _dependentValueBinding;
        /// <summary>
        /// The binding used to identify the independent value binding.
        /// </summary>
        private Binding _independentValueBinding;
        /// <summary>
        /// Identifies the TransitionEasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionEasingFunctionProperty;
        /// <summary>
        /// Identifies the IsSelectionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionEnabledProperty;
        /// <summary>Identifies the AnimationSequence dependency property.</summary>
        public static readonly DependencyProperty AnimationSequenceProperty;
        /// <summary>
        /// WeakEventListener used to handle INotifyCollectionChanged events.
        /// </summary>
        private WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs> _weakEventListener;
        /// <summary>The plot area canvas.</summary>
        private Panel _plotArea;
        /// <summary>
        /// Tracks whether a call to OnSelectedItemPropertyChanged is already in progress.
        /// </summary>
        private bool _processingOnSelectedItemPropertyChanged;
        /// <summary>Identifies the SelectedItem dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty;
        /// <summary>Identifies the DataPointStyle dependency property.</summary>
        public static readonly DependencyProperty DataPointStyleProperty;
        /// <summary>Identifies the LegendItemStyle dependency property.</summary>
        public static readonly DependencyProperty LegendItemStyleProperty;
        /// <summary>
        /// Identifies the TransitionDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty;

        /// <summary>
        /// Gets or sets the Binding to use for identifying the dependent value.
        /// </summary>
        public Binding DependentValueBinding
        {
            get
            {
                return this._dependentValueBinding;
            }
            set
            {
                if (value == this._dependentValueBinding)
                    return;
                this._dependentValueBinding = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the dependent value.
        /// </summary>
        public string DependentValuePath
        {
            get
            {
                return this.DependentValueBinding != null ? this.DependentValueBinding.Path.Path : (string)null;
            }
            set
            {
                if (null == value)
                    this.DependentValueBinding = (Binding)null;
                else
                    this.DependentValueBinding = new Binding(value);
            }
        }

        /// <summary>
        /// Gets or sets the Binding to use for identifying the independent value.
        /// </summary>
        public Binding IndependentValueBinding
        {
            get
            {
                return this._independentValueBinding;
            }
            set
            {
                if (this._independentValueBinding == value)
                    return;
                this._independentValueBinding = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the independent value.
        /// </summary>
        public string IndependentValuePath
        {
            get
            {
                return this.IndependentValueBinding != null ? this.IndependentValueBinding.Path.Path : (string)null;
            }
            set
            {
                if (null == value)
                    this.IndependentValueBinding = (Binding)null;
                else
                    this.IndependentValueBinding = new Binding(value);
            }
        }

        /// <summary>
        /// Gets or sets a collection used to contain the data points of the Series.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(DataPointSeries.ItemsSourceProperty);
            }
            set
            {
                this.SetValue(DataPointSeries.ItemsSourceProperty, (object)value);
            }
        }

        /// <summary>ItemsSourceProperty property changed callback.</summary>
        /// <param name="o">Series for which the ItemsSource changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)o).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        /// <summary>Called when the ItemsSource property changes.</summary>
        /// <param name="oldValue">Old value of the ItemsSource property.</param>
        /// <param name="newValue">New value of the ItemsSource property.</param>
        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (null != oldValue as INotifyCollectionChanged && null != this._weakEventListener)
            {
                this._weakEventListener.Detach();
                this._weakEventListener = (WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs>)null;
            }
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                this._weakEventListener = new WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs>(this);
                this._weakEventListener.OnEventAction = (Action<DataPointSeries, object, NotifyCollectionChangedEventArgs>)((instance, source, eventArgs) => instance.ItemsSourceCollectionChanged(source, eventArgs));
                this._weakEventListener.OnDetachAction = (Action<WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs>>)(weakEventListener => newValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(weakEventListener.OnEvent));
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this._weakEventListener.OnEvent);
            }
            if (!this.TemplateApplied)
                return;
            this.Refresh();
        }

        /// <summary>
        /// Gets or sets the animation sequence to use for the DataPoints of the Series.
        /// </summary>
        public AnimationSequence AnimationSequence
        {
            get
            {
                return (AnimationSequence)this.GetValue(DataPointSeries.AnimationSequenceProperty);
            }
            set
            {
                this.SetValue(DataPointSeries.AnimationSequenceProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets a stream of the active data points in the plot area.
        /// </summary>
        protected virtual IEnumerable<DataPoint> ActiveDataPoints
        {
            get
            {
                return this.PlotArea != null ? this.PlotArea.Children.OfType<DataPoint>().Where<DataPoint>((Func<DataPoint, bool>)(dataPoint => dataPoint.IsActive)) : Enumerable.Empty<DataPoint>();
            }
        }

        /// <summary>
        /// Gets the number of active data points in the plot area.
        /// </summary>
        protected int ActiveDataPointCount { get; private set; }

        /// <summary>
        /// Gets or sets the easing function to use when transitioning the
        /// data points.
        /// </summary>
        public IEasingFunction TransitionEasingFunction
        {
            get
            {
                return this.GetValue(DataPointSeries.TransitionEasingFunctionProperty) as IEasingFunction;
            }
            set
            {
                this.SetValue(DataPointSeries.TransitionEasingFunctionProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether elements in the series can
        /// be selected.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get
            {
                return (bool)this.GetValue(DataPointSeries.IsSelectionEnabledProperty);
            }
            set
            {
                this.SetValue(DataPointSeries.IsSelectionEnabledProperty, (object)value);
            }
        }

        /// <summary>IsSelectionEnabledProperty property changed handler.</summary>
        /// <param name="d">DynamicSeries that changed its IsSelectionEnabled.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsSelectionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)d).OnIsSelectionEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>IsSelectionEnabledProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsSelectionEnabledPropertyChanged(bool oldValue, bool newValue)
        {
            foreach (DataPoint activeDataPoint in this.ActiveDataPoints)
                activeDataPoint.IsSelectionEnabled = newValue;
        }

        /// <summary>Gets the plot area canvas.</summary>
        internal Panel PlotArea
        {
            get
            {
                return this._plotArea;
            }
            private set
            {
                Panel plotArea = this._plotArea;
                this._plotArea = value;
                if (this._plotArea == plotArea)
                    return;
                this.OnPlotAreaChanged(plotArea, value);
            }
        }

        /// <summary>Gets the size of the plot area.</summary>
        /// <remarks>
        /// Use this method instead of PlotArea.ActualWidth/ActualHeight
        /// because the ActualWidth and ActualHeight properties are set after
        /// the SizeChanged handler runs.
        /// </remarks>
        protected Size PlotAreaSize { get; private set; }

        /// <summary>Event raised when selection has changed.</summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>Gets or sets the selected item.</summary>
        public object SelectedItem
        {
            get
            {
                return this.GetValue(DataPointSeries.SelectedItemProperty);
            }
            set
            {
                this.SetValue(DataPointSeries.SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Called when the value of the SelectedItem property changes.
        /// </summary>
        /// <param name="d">DynamicSeries that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)d).OnSelectedItemPropertyChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the value of the SelectedItem property changes.
        /// </summary>
        /// <param name="oldValue">The old selected index.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnSelectedItemPropertyChanged(object oldValue, object newValue)
        {
            DataPoint dataPoint = (DataPoint)null;
            if (null != newValue)
            {
                dataPoint = this._dataPointsByObject[newValue].Where<DataPoint>((Func<DataPoint, bool>)(dp => object.Equals(newValue, dp.DataContext) && dp.IsActive)).FirstOrDefault<DataPoint>();
                if (null == dataPoint)
                {
                    try
                    {
                        this._processingOnSelectedItemPropertyChanged = true;
                        this.SelectedItem = (object)null;
                        newValue = (object)null;
                    }
                    finally
                    {
                        this._processingOnSelectedItemPropertyChanged = false;
                    }
                }
            }
            foreach (DataPoint dataPoint1 in this.ActiveDataPoints.Where<DataPoint>((Func<DataPoint, bool>)(activeDataPoint => activeDataPoint != dataPoint && activeDataPoint.IsSelected)))
            {
                dataPoint1.IsSelectedChanged -= new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
                dataPoint1.IsSelected = false;
                dataPoint1.IsSelectedChanged += new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
            }
            if (dataPoint != null && !dataPoint.IsSelected)
            {
                dataPoint.IsSelectedChanged -= new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
                dataPoint.IsSelected = true;
                dataPoint.IsSelectedChanged += new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
            }
            if (this._processingOnSelectedItemPropertyChanged || oldValue == newValue)
                return;
            IList removedItems = (IList)new List<object>();
            if (oldValue != null)
                removedItems.Add(oldValue);
            IList addedItems = (IList)new List<object>();
            if (newValue != null)
                addedItems.Add(newValue);
            SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
            if (null != selectionChanged)
                selectionChanged((object)this, new SelectionChangedEventArgs(removedItems, addedItems));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the template has been
        /// applied.
        /// </summary>
        private bool TemplateApplied { get; set; }

        /// <summary>Gets or sets the style to use for the data points.</summary>
        public Style DataPointStyle
        {
            get
            {
                return this.GetValue(DataPointSeries.DataPointStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DataPointSeries.DataPointStyleProperty, (object)value);
            }
        }

        /// <summary>DataPointStyleProperty property changed handler.</summary>
        /// <param name="d">DataPointSingleSeriesWithAxes that changed its DataPointStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDataPointStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)d).OnDataPointStylePropertyChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>DataPointStyleProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnDataPointStylePropertyChanged(Style oldValue, Style newValue)
        {
            foreach (LegendItem legendItem in this.LegendItems.OfType<LegendItem>())
            {
                object dataContext = legendItem.DataContext;
                legendItem.DataContext = (object)null;
                legendItem.DataContext = dataContext;
            }
        }

        /// <summary>Gets or sets the style to use for the legend items.</summary>
        public Style LegendItemStyle
        {
            get
            {
                return this.GetValue(DataPointSeries.LegendItemStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DataPointSeries.LegendItemStyleProperty, (object)value);
            }
        }

        /// <summary>LegendItemStyleProperty property changed handler.</summary>
        /// <param name="d">DataPointSeries that changed its LegendItemStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLegendItemStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)d).OnLegendItemStylePropertyChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the LegendItemStyle property changes.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnLegendItemStylePropertyChanged(Style oldValue, Style newValue)
        {
        }

        /// <summary>
        /// Gets or sets the Geometry used to clip DataPoints to the PlotArea bounds.
        /// </summary>
        private RectangleGeometry ClipGeometry { get; set; }

        /// <summary>
        /// Gets or sets the duration of the value Transition animation.
        /// </summary>
        public TimeSpan TransitionDuration
        {
            get
            {
                return (TimeSpan)this.GetValue(DataPointSeries.TransitionDurationProperty);
            }
            set
            {
                this.SetValue(DataPointSeries.TransitionDurationProperty, (object)value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeries class.
        /// </summary>
        protected DataPointSeries()
        {
#if !MIGRATION
            this.DefaultStyleKey = (object)typeof(DataPointSeries);
#endif

            this.ClipGeometry = new RectangleGeometry();
            this.Clip = (Geometry)this.ClipGeometry;

#if MIGRATION
            this.DefaultStyleKey = (object)typeof(DataPointSeries);
#endif
        }

        /// <summary>
        /// Adds an object to the series host by creating a corresponding data point
        /// for it.
        /// </summary>
        /// <param name="dataContext">The object to add to the series host.</param>
        /// <returns>The data point created for the object.</returns>
        protected virtual DataPoint AddObject(object dataContext)
        {
            if (!this.ShouldCreateDataPoint(dataContext))
                return (DataPoint)null;
            DataPoint prepareDataPoint = this.CreateAndPrepareDataPoint(dataContext);
            this._dataPointsByObject.Add(dataContext, prepareDataPoint);
            this.AddDataPoint(prepareDataPoint);
            return prepareDataPoint;
        }

        /// <summary>
        /// Returns whether a data point should be created for the data context.
        /// </summary>
        /// <param name="dataContext">The data context that will be used for the
        /// data point.</param>
        /// <returns>A value indicating whether a data point should be created
        /// for the data context.</returns>
        protected virtual bool ShouldCreateDataPoint(object dataContext)
        {
            return true;
        }

        /// <summary>
        /// Returns the index at which to insert data point in the plot area
        /// child collection.
        /// </summary>
        /// <param name="dataPoint">The data point to retrieve the insertion
        /// index for.</param>
        /// <returns>The insertion index.</returns>
        protected virtual int GetInsertionIndex(DataPoint dataPoint)
        {
            return this.PlotArea.Children.Count;
        }

        /// <summary>Adds a data point to the plot area.</summary>
        /// <param name="dataPoint">The data point to add to the plot area.</param>
        protected virtual void AddDataPoint(DataPoint dataPoint)
        {
            if (dataPoint.IsSelected)
                this.Select(dataPoint);
            if (this.PlotArea == null)
                return;
            Canvas.SetLeft((UIElement)dataPoint, -3.40282346638529E+38);
            Canvas.SetTop((UIElement)dataPoint, -3.40282346638529E+38);
            dataPoint.IsSelectionEnabled = this.IsSelectionEnabled;
            this.AttachEventHandlersToDataPoint(dataPoint);
            this.PlotArea.Children.Insert(this.GetInsertionIndex(dataPoint), (UIElement)dataPoint);
            ++this.ActiveDataPointCount;
        }

        /// <summary>
        /// Retrieves the data point corresponding to the object passed as the
        /// parameter.
        /// </summary>
        /// <param name="dataContext">The data context used for the point.</param>
        /// <returns>The data point associated with the object.</returns>
        protected virtual DataPoint GetDataPoint(object dataContext)
        {
            return this._dataPointsByObject[dataContext].Where<DataPoint>((Func<DataPoint, bool>)(dp => object.Equals(dataContext, dp.DataContext))).FirstOrDefault<DataPoint>();
        }

        /// <summary>Creates and prepares a data point.</summary>
        /// <param name="dataContext">The object to use as the data context
        /// of the data point.</param>
        /// <returns>The newly created data point.</returns>
        private DataPoint CreateAndPrepareDataPoint(object dataContext)
        {
            DataPoint dataPoint = this.CreateDataPoint();
            this.PrepareDataPoint(dataPoint, dataContext);
            return dataPoint;
        }

        /// <summary>Returns a Control suitable for the Series.</summary>
        /// <returns>The DataPoint instance.</returns>
        protected abstract DataPoint CreateDataPoint();

        /// <summary>Creates a legend item.</summary>
        /// <returns>A legend item for insertion in the legend items collection.</returns>
        /// <param name="owner">The owner of the new LegendItem.</param>
        protected virtual LegendItem CreateLegendItem(DataPointSeries owner)
        {
            LegendItem legendItem = new LegendItem()
            {
                Owner = (object)owner
            };
            legendItem.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualLegendItemStyle")
            {
                Source = (object)this
            });
            legendItem.SetBinding(ContentControl.ContentProperty, new Binding("Title")
            {
                Source = (object)this
            });
            return legendItem;
        }

        /// <summary>
        /// Method that handles the ObservableCollection.CollectionChanged event for the ItemsSource property.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnItemsSourceCollectionChanged(this.ItemsSource, e);
        }

        /// <summary>
        /// Updates data points collection with items retrieved from items
        /// source and removes the old items.
        /// </summary>
        /// <param name="newItems">The items to load.</param>
        /// <param name="oldItems">The items to remove.</param>
        protected void LoadDataPoints(IEnumerable newItems, IEnumerable oldItems)
        {
            if (this.PlotArea == null || this.SeriesHost == null)
                return;
            IList<DataPoint> oldDataPoints = (IList<DataPoint>)new List<DataPoint>();
            if (oldItems != null)
            {
                if (oldItems != null)
                {
                    foreach (object oldItem in oldItems)
                    {
                        DataPoint dataPoint = this.RemoveObject(oldItem);
                        this._dataPointsByObject.Remove(oldItem, dataPoint);
                        if (dataPoint != null)
                            oldDataPoints.Add(dataPoint);
                    }
                }
                this.StaggeredStateChange((IEnumerable<DataPoint>)oldDataPoints, oldDataPoints.Count, DataPointState.Hiding);
            }
            IList<DataPoint> newDataPoints = (IList<DataPoint>)new List<DataPoint>();
            if (newItems != null)
            {
                foreach (object newItem in newItems)
                {
                    DataPoint dataPoint = this.AddObject(newItem);
                    if (dataPoint != null)
                        newDataPoints.Add(dataPoint);
                }
            }
            this.OnDataPointsChanged(newDataPoints, oldDataPoints);
        }

        /// <summary>
        /// Attaches handler plot area after loading it from XAML.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PlotArea = this.GetTemplateChild("PlotArea") as Panel;
            if (this.TemplateApplied)
                return;
            this.TemplateApplied = true;
            this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
        }

        /// <summary>
        /// Invokes an action when the plot area's layout is updated.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        internal void InvokeOnLayoutUpdated(Action action)
        {
            EventHandler handler = (EventHandler)null;
            handler = (EventHandler)delegate
            {
                this.PlotArea.LayoutUpdated -= handler;
                action();
            };
            this.PlotArea.LayoutUpdated += handler;
        }

        /// <summary>Handles changes to the SeriesHost property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);
            if (null != newValue)
                return;
            this._needRefreshWhenSizeChanged = true;
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected virtual void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            this.StaggeredStateChange((IEnumerable<DataPoint>)newDataPoints, CollectionHelper.Count(newDataPoints), DataPointState.Showing);
        }

        /// <summary>
        /// Method called when the ItemsSource collection changes.
        /// </summary>
        /// <param name="collection">New value of the collection.</param>
        /// <param name="e">Information about the change.</param>
        protected virtual void OnItemsSourceCollectionChanged(IEnumerable collection, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                IList<DataPoint> dataPointList = (IList<DataPoint>)new List<DataPoint>();
                for (int index = 0; index < e.OldItems.Count; ++index)
                {
                    DataPoint dataPoint = this._dataPointsByObject[e.OldItems[index]].Where<DataPoint>((Func<DataPoint, bool>)(dp => object.Equals(e.OldItems[index], dp.DataContext))).Except<DataPoint>((IEnumerable<DataPoint>)dataPointList).FirstOrDefault<DataPoint>();
                    if (null != dataPoint)
                    {
                        dataPointList.Add(dataPoint);
                        dataPoint.DataContext = e.NewItems[index];
                        this._dataPointsByObject.Remove(e.OldItems[index], dataPoint);
                        this._dataPointsByObject.Add(e.NewItems[index], dataPoint);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                this.LoadDataPoints((IEnumerable)e.NewItems, (IEnumerable)e.OldItems);
            else
                this.Refresh();
        }

        /// <summary>
        /// Removes items from the existing plot area and adds items to new
        /// plot area.
        /// </summary>
        /// <param name="oldValue">The previous plot area.</param>
        /// <param name="newValue">The new plot area.</param>
        protected virtual void OnPlotAreaChanged(Panel oldValue, Panel newValue)
        {
            if (oldValue != null)
            {
                foreach (DataPoint activeDataPoint in this.ActiveDataPoints)
                    oldValue.Children.Remove((UIElement)activeDataPoint);
            }
            if (newValue == null)
                return;
            foreach (DataPoint activeDataPoint in this.ActiveDataPoints)
                newValue.Children.Add((UIElement)activeDataPoint);
        }

        /// <summary>
        /// Updates the visual appearance of all the data points when the size
        /// changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private new void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PlotAreaSize = e.NewSize;
            RectangleGeometry clipGeometry = this.ClipGeometry;
            double x = 0.0;
            double y = 0.0;
            Size plotAreaSize1 = this.PlotAreaSize;
            double width1 = plotAreaSize1.Width;
            plotAreaSize1 = this.PlotAreaSize;
            double height1 = plotAreaSize1.Height;
            Rect rect = new Rect(x, y, width1, height1);
            clipGeometry.Rect = rect;
            if (null == this.PlotArea)
                return;
            Panel plotArea1 = this.PlotArea;
            Size plotAreaSize2 = this.PlotAreaSize;
            double width2 = plotAreaSize2.Width;
            plotArea1.Width = width2;
            Panel plotArea2 = this.PlotArea;
            plotAreaSize2 = this.PlotAreaSize;
            double height2 = plotAreaSize2.Height;
            plotArea2.Height = height2;
            if (this._needRefreshWhenSizeChanged)
            {
                this._needRefreshWhenSizeChanged = false;
                this.Refresh();
            }
            else
                this.UpdateDataPoints(this.ActiveDataPoints);
        }

        /// <summary>
        /// Refreshes data from data source and renders the series.
        /// </summary>
        public void Refresh()
        {
            try
            {
                this.LoadDataPoints(this.ItemsSource, (IEnumerable)this.ActiveDataPoints.Select<DataPoint, object>((Func<DataPoint, object>)(dataPoint => dataPoint.DataContext)));
            }
            catch
            {
                if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                    return;
                throw;
            }
        }

        /// <summary>
        /// Removes an object from the series host by removing its corresponding
        /// data point.
        /// </summary>
        /// <param name="dataContext">The object to remove from the series data
        /// source.</param>
        /// <returns>The data point corresponding to the removed object.</returns>
        protected virtual DataPoint RemoveObject(object dataContext)
        {
            DataPoint dataPoint = this.GetDataPoint(dataContext);
            if (dataPoint != null)
                this.RemoveDataPoint(dataPoint);
            return dataPoint;
        }

        /// <summary>Removes a data point from the plot area.</summary>
        /// <param name="dataPoint">The data point to remove.</param>
        protected virtual void RemoveDataPoint(DataPoint dataPoint)
        {
            if (dataPoint.IsSelected)
                this.Unselect(dataPoint);
            --this.ActiveDataPointCount;
            dataPoint.State = DataPointState.PendingRemoval;
        }

        /// <summary>
        /// Gets a value indicating whether all data points are being
        /// updated.
        /// </summary>
        protected bool UpdatingDataPoints { get; private set; }

        /// <summary>
        /// Updates the visual representation of all data points in the plot
        /// area.
        /// </summary>
        /// <param name="dataPoints">A sequence of data points to update.</param>
        protected virtual void UpdateDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            this.UpdatingDataPoints = true;
            this.DetachEventHandlersFromDataPoints(dataPoints);
            try
            {
                this.OnBeforeUpdateDataPoints();
                foreach (DataPoint dataPoint in dataPoints)
                    this.UpdateDataPoint(dataPoint);
                this.OnAfterUpdateDataPoints();
            }
            finally
            {
                this.AttachEventHandlersToDataPoints(dataPoints);
                this.UpdatingDataPoints = false;
            }
        }

        /// <summary>Attaches event handlers to the data points.</summary>
        /// <param name="dataPoints">A sequence of data points.</param>
        private void AttachEventHandlersToDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            foreach (DataPoint dataPoint in dataPoints)
                this.AttachEventHandlersToDataPoint(dataPoint);
        }

        /// <summary>Detaches event handlers from the data points.</summary>
        /// <param name="dataPoints">A sequence of data points.</param>
        private void DetachEventHandlersFromDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            foreach (DataPoint dataPoint in dataPoints)
                this.DetachEventHandlersFromDataPoint(dataPoint);
        }

        /// <summary>Attaches event handlers to a data point.</summary>
        /// <param name="dataPoint">The data point.</param>
        protected virtual void AttachEventHandlersToDataPoint(DataPoint dataPoint)
        {
            dataPoint.IsSelectedChanged += new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
            dataPoint.ActualDependentValueChanged += new RoutedPropertyChangedEventHandler<IComparable>(this.OnDataPointActualDependentValueChanged);
            dataPoint.ActualIndependentValueChanged += new RoutedPropertyChangedEventHandler<object>(this.OnDataPointActualIndependentValueChanged);
            dataPoint.DependentValueChanged += new RoutedPropertyChangedEventHandler<IComparable>(this.OnDataPointDependentValueChanged);
            dataPoint.IndependentValueChanged += new RoutedPropertyChangedEventHandler<object>(this.OnDataPointIndependentValueChanged);
            dataPoint.StateChanged += new RoutedPropertyChangedEventHandler<DataPointState>(this.OnDataPointStateChanged);
        }

        /// <summary>Unselects a data point.</summary>
        /// <param name="dataPoint">The data point to unselect.</param>
        private void Unselect(DataPoint dataPoint)
        {
            if (!dataPoint.DataContext.Equals(this.SelectedItem))
                return;
            this.SelectedItem = (object)null;
        }

        /// <summary>Selects a data point.</summary>
        /// <param name="dataPoint">The data point to select.</param>
        private void Select(DataPoint dataPoint)
        {
            this.SelectedItem = dataPoint.DataContext;
        }

        /// <summary>
        /// Method executed when a data point is either selected or unselected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnDataPointIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            DataPoint dataPoint = sender as DataPoint;
            if (e.NewValue)
                this.Select(dataPoint);
            else
                this.Unselect(dataPoint);
        }

        /// <summary>Detaches event handlers from a data point.</summary>
        /// <param name="dataPoint">The data point.</param>
        protected virtual void DetachEventHandlersFromDataPoint(DataPoint dataPoint)
        {
            dataPoint.IsSelectedChanged -= new RoutedPropertyChangedEventHandler<bool>(this.OnDataPointIsSelectedChanged);
            dataPoint.ActualDependentValueChanged -= new RoutedPropertyChangedEventHandler<IComparable>(this.OnDataPointActualDependentValueChanged);
            dataPoint.ActualIndependentValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.OnDataPointActualIndependentValueChanged);
            dataPoint.DependentValueChanged -= new RoutedPropertyChangedEventHandler<IComparable>(this.OnDataPointDependentValueChanged);
            dataPoint.IndependentValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.OnDataPointIndependentValueChanged);
            dataPoint.StateChanged -= new RoutedPropertyChangedEventHandler<DataPointState>(this.OnDataPointStateChanged);
        }

        /// <summary>
        /// This method that executes before data points are updated.
        /// </summary>
        protected virtual void OnBeforeUpdateDataPoints()
        {
        }

        /// <summary>
        /// This method that executes after data points are updated.
        /// </summary>
        protected virtual void OnAfterUpdateDataPoints()
        {
        }

        /// <summary>
        /// Updates the visual representation of a single data point in the plot
        /// area.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected abstract void UpdateDataPoint(DataPoint dataPoint);

        /// <summary>
        /// Prepares a data point by extracting binding it to a data context
        /// object.
        /// </summary>
        /// <param name="dataPoint">A data point.</param>
        /// <param name="dataContext">A data context object.</param>
        protected virtual void PrepareDataPoint(DataPoint dataPoint, object dataContext)
        {
            dataPoint.DataContext = dataContext;
            if (this.IndependentValueBinding != null)
                dataPoint.SetBinding(DataPoint.IndependentValueProperty, this.IndependentValueBinding);
            if (this.DependentValueBinding == null)
                dataPoint.SetBinding(DataPoint.DependentValueProperty, new Binding());
            else
                dataPoint.SetBinding(DataPoint.DependentValueProperty, this.DependentValueBinding);
        }

        /// <summary>Reveals data points using a storyboard.</summary>
        /// <param name="dataPoints">The data points to change the state of.</param>
        /// <param name="dataPointCount">The number of data points in the sequence.</param>
        /// <param name="newState">The state to change to.</param>
        private void StaggeredStateChange(IEnumerable<DataPoint> dataPoints, int dataPointCount, DataPointState newState)
        {
            if (this.PlotArea == null || dataPointCount == 0)
                return;
            Storyboard stateChangeStoryBoard = new Storyboard();
            dataPoints.ForEachWithIndex<DataPoint>((Action<DataPoint, int>)((dataPoint, count) =>
            {
                ObjectAnimationUsingKeyFrames animationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                Storyboard.SetTarget((Timeline)animationUsingKeyFrames, (DependencyObject)dataPoint);
                Storyboard.SetTargetProperty((Timeline)animationUsingKeyFrames, new PropertyPath("State", new object[0]));
                DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame();
                discreteObjectKeyFrame.Value = (object)newState;
                switch (this.AnimationSequence)
                {
                    case AnimationSequence.Simultaneous:
                        discreteObjectKeyFrame.KeyTime = (KeyTime)TimeSpan.Zero;
                        break;
                    case AnimationSequence.FirstToLast:
                        discreteObjectKeyFrame.KeyTime = (KeyTime)TimeSpan.FromMilliseconds(1000.0 * ((double)count / (double)dataPointCount));
                        break;
                    case AnimationSequence.LastToFirst:
                        discreteObjectKeyFrame.KeyTime = (KeyTime)TimeSpan.FromMilliseconds(1000.0 * ((double)(dataPointCount - count - 1) / (double)dataPointCount));
                        break;
                }
                animationUsingKeyFrames.KeyFrames.Add((ObjectKeyFrame)discreteObjectKeyFrame);
                stateChangeStoryBoard.Children.Add((Timeline)animationUsingKeyFrames);
            }));
            stateChangeStoryBoard.Duration = new Duration(this.AnimationSequence == AnimationSequence.Simultaneous ? TimeSpan.FromTicks(1L) : TimeSpan.FromMilliseconds(1001.0));
            this._storyBoardQueue.Enqueue(stateChangeStoryBoard, (EventHandler)((sender, args) => stateChangeStoryBoard.Stop()));
        }

        /// <summary>Handles data point state property change.</summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointStateChanged(object sender, RoutedPropertyChangedEventArgs<DataPointState> args)
        {
            this.OnDataPointStateChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>Handles data point state property change.</summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointStateChanged(DataPoint dataPoint, DataPointState oldValue, DataPointState newValue)
        {
            if (dataPoint.State != DataPointState.Hidden)
                return;
            this.DetachEventHandlersFromDataPoint(dataPoint);
            this.PlotArea.Children.Remove((UIElement)dataPoint);
        }

        /// <summary>
        /// Handles data point actual dependent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointActualDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> args)
        {
            this.OnDataPointActualDependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point actual dependent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointActualDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
        }

        /// <summary>
        /// Handles data point actual independent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointActualIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            this.OnDataPointActualIndependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point actual independent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
        }

        /// <summary>Handles data point dependent value property changes.</summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> args)
        {
            this.OnDataPointDependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>Handles data point dependent value property change.</summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
        }

        /// <summary>
        /// Handles data point independent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            this.OnDataPointIndependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>Handles data point independent value property change.</summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Returns a ResourceDictionaryEnumerator that returns ResourceDictionaries with a
        /// DataPointStyle having the specified TargetType or with a TargetType that is an
        /// ancestor of the specified type.
        /// </summary>
        /// <param name="dispenser">The ResourceDictionaryDispenser.</param>
        /// <param name="targetType">The TargetType.</param>
        /// <param name="takeAncestors">A value indicating whether to accept ancestors of the TargetType.</param>
        /// <returns>A ResourceDictionary enumerator.</returns>
        internal static IEnumerator<ResourceDictionary> GetResourceDictionaryWithTargetType(IResourceDictionaryDispenser dispenser, Type targetType, bool takeAncestors)
        {
            return dispenser.GetResourceDictionariesWhere((Func<ResourceDictionary, bool>)(dictionary =>
            {
                Style style = dictionary[(object)"DataPointStyle"] as Style;
                if (null != style)
                    return style.TargetType != null && (targetType == style.TargetType || takeAncestors && style.TargetType.IsAssignableFrom(targetType));
                return false;
            }));
        }

        static DataPointSeries()
        {
            string name = nameof(TransitionEasingFunction);
            Type propertyType = typeof(IEasingFunction);
            Type ownerType = typeof(DataPointSeries);
            QuadraticEase quadraticEase = new QuadraticEase();
            quadraticEase.EasingMode = EasingMode.EaseInOut;
            PropertyMetadata typeMetadata = new PropertyMetadata((object)quadraticEase);
            DataPointSeries.TransitionEasingFunctionProperty = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
            DataPointSeries.IsSelectionEnabledProperty = DependencyProperty.Register(nameof(IsSelectionEnabled), typeof(bool), typeof(DataPointSeries), new PropertyMetadata((object)false, new PropertyChangedCallback(DataPointSeries.OnIsSelectionEnabledPropertyChanged)));
            DataPointSeries.AnimationSequenceProperty = DependencyProperty.Register(nameof(AnimationSequence), typeof(AnimationSequence), typeof(DataPointSeries), new PropertyMetadata((object)AnimationSequence.Simultaneous));
            DataPointSeries.SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataPointSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPointSeries.OnSelectedItemPropertyChanged)));
            DataPointSeries.DataPointStyleProperty = DependencyProperty.Register(nameof(DataPointStyle), typeof(Style), typeof(DataPointSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPointSeries.OnDataPointStylePropertyChanged)));
            DataPointSeries.LegendItemStyleProperty = DependencyProperty.Register(nameof(LegendItemStyle), typeof(Style), typeof(DataPointSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPointSeries.OnLegendItemStylePropertyChanged)));
            DataPointSeries.TransitionDurationProperty = DependencyProperty.Register(nameof(TransitionDuration), typeof(TimeSpan), typeof(DataPointSeries), new PropertyMetadata((object)TimeSpan.FromSeconds(0.5)));
        }
    }
}