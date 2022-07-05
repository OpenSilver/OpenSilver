// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Properties = OpenSilver.Controls.DataVisualization.Properties;

using System.Windows.Markup;
using System.Collections;
#if MIGRATION
using System.Windows.Controls.DataVisualization.Charting.Primitives;
#else
using System;
using Windows.UI.Xaml.Controls.DataVisualization.Charting.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// Represents a control that displays a Chart.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ChartAreaStyle", StyleTargetType = typeof(EdgePanel))]
    [StyleTypedProperty(Property = "PlotAreaStyle", StyleTargetType = typeof(Grid))]
    [ContentProperty("Series")]
    [TemplatePart(Name = "ChartArea", Type = typeof(EdgePanel))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(System.Windows.Controls.DataVisualization.Title))]
    [StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(Legend))]
    [TemplatePart(Name = "Legend", Type = typeof(Legend))]
    public class Chart : Control, ISeriesHost, IRequireSeriesHost, IResourceDictionaryDispenser
    {
        /// <summary>Identifies the ChartAreaStyle dependency property.</summary>
        public static readonly DependencyProperty ChartAreaStyleProperty = DependencyProperty.Register(nameof(ChartAreaStyle), typeof(Style), typeof(Chart), (PropertyMetadata)null);
        /// <summary>Identifies the LegendStyle dependency property.</summary>
        public static readonly DependencyProperty LegendStyleProperty = DependencyProperty.Register(nameof(LegendStyle), typeof(Style), typeof(Chart), (PropertyMetadata)null);
        /// <summary>Identifies the LegendTitle dependency property.</summary>
        public static readonly DependencyProperty LegendTitleProperty = DependencyProperty.Register(nameof(LegendTitle), typeof(object), typeof(Chart), (PropertyMetadata)null);
        /// <summary>Identifies the PlotAreaStyle dependency property.</summary>
        public static readonly DependencyProperty PlotAreaStyleProperty = DependencyProperty.Register(nameof(PlotAreaStyle), typeof(Style), typeof(Chart), (PropertyMetadata)null);
        /// <summary>Identifies the Palette dependency property.</summary>
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(nameof(Palette), typeof(Collection<ResourceDictionary>), typeof(Chart), new PropertyMetadata(new PropertyChangedCallback(Chart.OnPalettePropertyChanged)));
        /// <summary>Identifies the Title dependency property.</summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(object), typeof(Chart), (PropertyMetadata)null);
        /// <summary>Identifies the TitleStyle dependency property.</summary>
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register(nameof(TitleStyle), typeof(Style), typeof(Chart), (PropertyMetadata)null);
        /// <summary>
        /// An adapter that synchronizes changes to the ChartAreaChildren
        /// property to the ChartArea panel's children collection.
        /// </summary>
        private ObservableCollectionListAdapter<UIElement> _chartAreaChildrenListAdapter = new ObservableCollectionListAdapter<UIElement>();
        /// <summary>The collection of foreground elements.</summary>
        private ObservableCollection<UIElement> _foregroundElements = (ObservableCollection<UIElement>)new NoResetObservableCollection<UIElement>();
        /// <summary>The collection of background elements.</summary>
        private ObservableCollection<UIElement> _backgroundElements = (ObservableCollection<UIElement>)new NoResetObservableCollection<UIElement>();
        /// <summary>Axes arranged along the edges.</summary>
        private ObservableCollection<Axis> _edgeAxes = (ObservableCollection<Axis>)new NoResetObservableCollection<Axis>();
        /// <summary>Specifies the name of the ChartArea TemplatePart.</summary>
        private const string ChartAreaName = "ChartArea";
        /// <summary>Specifies the name of the legend TemplatePart.</summary>
        private const string LegendName = "Legend";
        /// <summary>Stores the collection of Axes in the Chart.</summary>
        private Collection<IAxis> _axes;
        /// <summary>
        /// Stores the collection of Series displayed by the Chart.
        /// </summary>
        private Collection<ISeries> _series;

        /// <summary>Gets or sets the chart area children collection.</summary>
        private AggregatedObservableCollection<UIElement> ChartAreaChildren { get; set; }

        /// <summary>Gets or sets a collection of Axes in the Chart.</summary>
        public Collection<IAxis> Axes
        {
            get
            {
                return this._axes;
            }
            set
            {
                throw new NotSupportedException("Chart Axes setter not supported");
            }
        }

        ObservableCollection<UIElement> ISeriesHost.ForegroundElements
        {
            get
            {
                return this.ForegroundElements;
            }
        }

        /// <summary>Gets the collection of foreground elements.</summary>
        protected ObservableCollection<UIElement> ForegroundElements
        {
            get
            {
                return this._foregroundElements;
            }
        }

        ObservableCollection<UIElement> ISeriesHost.BackgroundElements
        {
            get
            {
                return this.BackgroundElements;
            }
        }

        /// <summary>Gets the collection of background elements.</summary>
        protected ObservableCollection<UIElement> BackgroundElements
        {
            get
            {
                return this._backgroundElements;
            }
        }

        /// <summary>
        /// Gets or sets the axes that are currently in the chart.
        /// </summary>
        private IList<IAxis> InternalActualAxes { get; set; }

        /// <summary>Gets the actual axes displayed in the chart.</summary>
        public ReadOnlyCollection<IAxis> ActualAxes { get; private set; }

        /// <summary>
        /// Gets or sets the reference to the template's ChartArea.
        /// </summary>
        private EdgePanel ChartArea { get; set; }

        /// <summary>Gets or sets the reference to the Chart's Legend.</summary>
        private Legend Legend { get; set; }

        /// <summary>
        /// Gets or sets the collection of Series displayed by the Chart.
        /// </summary>
        public Collection<ISeries> Series
        {
            get
            {
                return this._series;
            }
            set
            {
                throw new NotSupportedException("Chart Series setter not supported");
            }
        }

        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's ChartArea.
        /// </summary>
        public Style ChartAreaStyle
        {
            get
            {
                return this.GetValue(Chart.ChartAreaStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Chart.ChartAreaStyleProperty, (object)value);
            }
        }

        /// <summary>Gets the collection of legend items.</summary>
        public Collection<object> LegendItems { get; private set; }

        /// <summary>Gets or sets the Style of the ISeriesHost's Legend.</summary>
        public Style LegendStyle
        {
            get
            {
                return this.GetValue(Chart.LegendStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Chart.LegendStyleProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the Title content of the Legend.</summary>
        public object LegendTitle
        {
            get
            {
                return this.GetValue(Chart.LegendTitleProperty);
            }
            set
            {
                this.SetValue(Chart.LegendTitleProperty, value);
            }
        }

        /// <summary>Gets or sets the Style of the ISeriesHost's PlotArea.</summary>
        public Style PlotAreaStyle
        {
            get
            {
                return this.GetValue(Chart.PlotAreaStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Chart.PlotAreaStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets a palette of ResourceDictionaries used by the children of the Chart.
        /// </summary>
        public Collection<ResourceDictionary> Palette
        {
            get
            {
                return this.GetValue(Chart.PaletteProperty) as Collection<ResourceDictionary>;
            }
            set
            {
                this.SetValue(Chart.PaletteProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the Palette property is changed.
        /// </summary>
        /// <param name="d">Chart that contains the changed Palette.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnPalettePropertyChanged((Collection<ResourceDictionary>)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the Palette property is changed.
        /// </summary>
        /// <param name="newValue">The new value for the Palette.</param>
        private void OnPalettePropertyChanged(Collection<ResourceDictionary> newValue)
        {
            this.ResourceDictionaryDispenser.ResourceDictionaries = (IList<ResourceDictionary>)newValue;
        }

        /// <summary>
        /// Gets or sets an object that rotates through the palette.
        /// </summary>
        private ResourceDictionaryDispenser ResourceDictionaryDispenser { get; set; }

        /// <summary>
        /// Event that is invoked when the ResourceDictionaryDispenser's collection has changed.
        /// </summary>
        public event EventHandler ResourceDictionariesChanged;

        /// <summary>Gets or sets the title displayed for the Chart.</summary>
        public object Title
        {
            get
            {
                return this.GetValue(Chart.TitleProperty);
            }
            set
            {
                this.SetValue(Chart.TitleProperty, value);
            }
        }

        /// <summary>Gets or sets the Style of the ISeriesHost's Title.</summary>
        public Style TitleStyle
        {
            get
            {
                return this.GetValue(Chart.TitleStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Chart.TitleStyleProperty, (object)value);
            }
        }

        /// <summary>Initializes a new instance of the Chart class.</summary>
        public Chart()
        {
#if !MIGRATION
            this.DefaultStyleKey = (object)typeof(Chart);
#endif
            UniqueObservableCollection<ISeries> observableCollection1 = new UniqueObservableCollection<ISeries>();
            observableCollection1.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SeriesCollectionChanged);
            this._series = (Collection<ISeries>)observableCollection1;
            UniqueObservableCollection<IAxis> persistentAxes = new UniqueObservableCollection<IAxis>();
            this._axes = (Collection<IAxis>)persistentAxes;
            ObservableCollection<IAxis> observableCollection2 = (ObservableCollection<IAxis>)new SeriesHostAxesCollection((ISeriesHost)this, persistentAxes);
            observableCollection2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ActualAxesCollectionChanged);
            this.InternalActualAxes = (IList<IAxis>)observableCollection2;
            this.ActualAxes = new ReadOnlyCollection<IAxis>(this.InternalActualAxes);
            this.LegendItems = (Collection<object>)new AggregatedObservableCollection<object>();
            this.ChartAreaChildren = new AggregatedObservableCollection<UIElement>();
            this.ChartAreaChildren.ChildCollections.Add((IList)this._edgeAxes);
            this.ChartAreaChildren.ChildCollections.Add((IList)this._backgroundElements);
            this.ChartAreaChildren.ChildCollections.Add((IList)this.Series);
            this.ChartAreaChildren.ChildCollections.Add((IList)this._foregroundElements);
            this._chartAreaChildrenListAdapter.Collection = (IEnumerable)this.ChartAreaChildren;
            this.ResourceDictionaryDispenser = new ResourceDictionaryDispenser();
            this.ResourceDictionaryDispenser.ResourceDictionariesChanged += (EventHandler)delegate
            {
                this.OnResourceDictionariesChanged(EventArgs.Empty);
            };

#if MIGRATION
            this.DefaultStyleKey = (object)typeof(Chart);
            CustomLayout = true;
#endif
        }

        /// <summary>Invokes the ResourceDictionariesChanged event.</summary>
        /// <param name="e">Event arguments.</param>
        private void OnResourceDictionariesChanged(EventArgs e)
        {
            EventHandler dictionariesChanged = this.ResourceDictionariesChanged;
            if (null == dictionariesChanged)
                return;
            dictionariesChanged((object)this, e);
        }

        /// <summary>
        /// Determines the location of an axis based on the existing axes in
        /// the chart.
        /// </summary>
        /// <param name="axis">The axis to determine the location of.</param>
        /// <returns>The location of the axis.</returns>
        private AxisLocation GetAutoAxisLocation(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X)
            {
                int num = CollectionHelper.Count(this.InternalActualAxes.OfType<Axis>().Where<Axis>((Func<Axis, bool>)(currentAxis => currentAxis.Location == AxisLocation.Top)));
                return CollectionHelper.Count(this.InternalActualAxes.OfType<Axis>().Where<Axis>((Func<Axis, bool>)(currentAxis => currentAxis.Location == AxisLocation.Bottom))) > num ? AxisLocation.Top : AxisLocation.Bottom;
            }
            if (axis.Orientation == AxisOrientation.Y)
                return CollectionHelper.Count(this.InternalActualAxes.OfType<Axis>().Where<Axis>((Func<Axis, bool>)(currentAxis => currentAxis.Location == AxisLocation.Left))) > CollectionHelper.Count(this.InternalActualAxes.OfType<Axis>().Where<Axis>((Func<Axis, bool>)(currentAxis => currentAxis.Location == AxisLocation.Right))) ? AxisLocation.Right : AxisLocation.Left;
            return AxisLocation.Auto;
        }

        /// <summary>Adds an axis to the ISeriesHost area.</summary>
        /// <param name="axis">The axis to add to the ISeriesHost area.</param>
        private void AddAxisToChartArea(Axis axis)
        {
            IRequireSeriesHost requireSeriesHost = axis as IRequireSeriesHost;
            if (requireSeriesHost != null)
                requireSeriesHost.SeriesHost = (ISeriesHost)this;
            if (axis.Location == AxisLocation.Auto)
                axis.Location = this.GetAutoAxisLocation(axis);
            Chart.SetEdge(axis);
            axis.LocationChanged += new RoutedPropertyChangedEventHandler<AxisLocation>(this.AxisLocationChanged);
            axis.OrientationChanged += new RoutedPropertyChangedEventHandler<AxisOrientation>(this.AxisOrientationChanged);
            if (axis.Location == AxisLocation.Auto)
                return;
            this._edgeAxes.Add(axis);
        }

        /// <summary>
        /// Rebuilds the chart area if an axis orientation is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void AxisOrientationChanged(object sender, RoutedPropertyChangedEventArgs<AxisOrientation> args)
        {
            Axis axis = (Axis)sender;
            axis.Location = this.GetAutoAxisLocation(axis);
        }

        /// <summary>
        /// Sets the Edge property of an axis based on its location and
        /// orientation.
        /// </summary>
        /// <param name="axis">The axis to set the edge property of.</param>
        private static void SetEdge(Axis axis)
        {
            switch (axis.Location)
            {
                case AxisLocation.Left:
                    EdgePanel.SetEdge((UIElement)axis, Edge.Left);
                    break;
                case AxisLocation.Top:
                    EdgePanel.SetEdge((UIElement)axis, Edge.Top);
                    break;
                case AxisLocation.Right:
                    EdgePanel.SetEdge((UIElement)axis, Edge.Right);
                    break;
                case AxisLocation.Bottom:
                    EdgePanel.SetEdge((UIElement)axis, Edge.Bottom);
                    break;
            }
        }

        /// <summary>
        /// Rebuild the chart area if an axis location is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void AxisLocationChanged(object sender, RoutedPropertyChangedEventArgs<AxisLocation> args)
        {
            Axis axis = (Axis)sender;
            if (args.NewValue == AxisLocation.Auto)
                throw new InvalidOperationException("Chart.AxisLocationChanged: Cant Be Changed To Auto When Hosted Inside Of A SeriesHost");
            Chart.SetEdge(axis);
            this._edgeAxes.Remove(axis);
            this._edgeAxes.Add(axis);
        }

        /// <summary>
        /// Adds a series to the plot area and injects chart services.
        /// </summary>
        /// <param name="series">The series to add to the plot area.</param>
        private void AddSeriesToPlotArea(ISeries series)
        {
            series.SeriesHost = (ISeriesHost)this;
            (this.LegendItems as AggregatedObservableCollection<object>).ChildCollections.Insert(this.Series.IndexOf(series), (IList)series.LegendItems);
        }

        /// <summary>
        /// Builds the visual tree for the Chart control when a new template
        /// is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (null != this.ChartArea)
                this.ChartArea.Children.Clear();
            if (null != this.Legend)
                this.Legend.ItemsSource = (IEnumerable)null;
            this.ChartArea = this.GetTemplateChild("ChartArea") as EdgePanel;
            this.Legend = this.GetTemplateChild("Legend") as Legend;
            if (this.ChartArea != null)
            {
                this._chartAreaChildrenListAdapter.TargetList = (IList)this.ChartArea.Children;
                this._chartAreaChildrenListAdapter.Populate();
            }
            if (this.Legend == null)
                return;
            this.Legend.ItemsSource = (IEnumerable)this.LegendItems;
        }

        /// <summary>
        /// Ensures that ISeriesHost is in a consistent state when axes collection is
        /// changed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ActualAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Axis axis in e.NewItems.OfType<Axis>())
                    this.AddAxisToChartArea(axis);
            }
            if (e.OldItems == null)
                return;
            foreach (Axis axis in e.OldItems.OfType<Axis>())
                this.RemoveAxisFromChartArea(axis);
        }

        /// <summary>Removes an axis from the Chart area.</summary>
        /// <param name="axis">The axis to remove from the ISeriesHost area.</param>
        private void RemoveAxisFromChartArea(Axis axis)
        {
            axis.LocationChanged -= new RoutedPropertyChangedEventHandler<AxisLocation>(this.AxisLocationChanged);
            axis.OrientationChanged -= new RoutedPropertyChangedEventHandler<AxisOrientation>(this.AxisOrientationChanged);
            IRequireSeriesHost requireSeriesHost = axis as IRequireSeriesHost;
            if (requireSeriesHost != null)
                requireSeriesHost.SeriesHost = (ISeriesHost)null;
            this._edgeAxes.Remove(axis);
        }

        /// <summary>Removes a series from the plot area.</summary>
        /// <param name="series">The series to remove from the plot area.</param>
        private void RemoveSeriesFromPlotArea(ISeries series)
        {
            (this.LegendItems as AggregatedObservableCollection<object>).ChildCollections.Remove((IList)series.LegendItems);
            series.SeriesHost = (ISeriesHost)null;
        }

        /// <summary>
        /// Called when the ObservableCollection.CollectionChanged property
        /// changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (null != e.OldItems)
            {
                foreach (ISeries oldItem in (IEnumerable)e.OldItems)
                {
                    ISeriesHost rootSeriesHost = oldItem as ISeriesHost;
                    if (rootSeriesHost != null)
                    {
                        foreach (IRequireGlobalSeriesIndex globalSeriesIndex in rootSeriesHost.GetDescendentSeries().OfType<IRequireGlobalSeriesIndex>())
                            globalSeriesIndex.GlobalSeriesIndexChanged(new int?());
                        rootSeriesHost.Series.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ChildSeriesCollectionChanged);
                    }
                    IRequireGlobalSeriesIndex globalSeriesIndex1 = oldItem as IRequireGlobalSeriesIndex;
                    if (globalSeriesIndex1 != null)
                        globalSeriesIndex1.GlobalSeriesIndexChanged(new int?());
                    this.RemoveSeriesFromPlotArea(oldItem);
                }
            }
            if (null != e.NewItems)
            {
                foreach (ISeries newItem in (IEnumerable)e.NewItems)
                {
                    ISeriesHost seriesHost = newItem as ISeriesHost;
                    if (null != seriesHost)
                        seriesHost.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ChildSeriesCollectionChanged);
                    this.AddSeriesToPlotArea(newItem);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
                return;
            this.OnGlobalSeriesIndexesInvalidated((object)this, new RoutedEventArgs());
        }

        /// <summary>
        /// Handles changes to the collections of child ISeries implementing ISeriesHost.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ChildSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnGlobalSeriesIndexesInvalidated((object)this, new RoutedEventArgs());
        }

        /// <summary>
        /// Returns a rotating enumerator of ResourceDictionary objects that coordinates
        /// with the dispenser object to ensure that no two enumerators are on the same
        /// item. If the dispenser is reset or its collection is changed then the
        /// enumerators are also reset.
        /// </summary>
        /// <param name="predicate">A predicate that returns a value indicating
        /// whether to return an item.</param>
        /// <returns>An enumerator of ResourceDictionaries.</returns>
        public IEnumerator<ResourceDictionary> GetResourceDictionariesWhere(Func<ResourceDictionary, bool> predicate)
        {
            return this.ResourceDictionaryDispenser.GetResourceDictionariesWhere(predicate);
        }

        /// <summary>
        /// Updates the global indexes of all descendents that require a global
        /// index.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void OnGlobalSeriesIndexesInvalidated(object sender, RoutedEventArgs args)
        {
            this.UpdateGlobalIndexes();
        }

        /// <summary>
        /// Updates the global index property of all Series that track their
        /// global index.
        /// </summary>
        private void UpdateGlobalIndexes()
        {
            this.GetDescendentSeries().OfType<IRequireGlobalSeriesIndex>().ForEachWithIndex<IRequireGlobalSeriesIndex>((Action<IRequireGlobalSeriesIndex, int>)((seriesThatTracksGlobalIndex, index) => seriesThatTracksGlobalIndex.GlobalSeriesIndexChanged(new int?(index))));
        }

        ISeriesHost IRequireSeriesHost.SeriesHost
        {
            get
            {
                return this.SeriesHost;
            }
            set
            {
                this.SeriesHost = value;
            }
        }

        /// <summary>Gets or sets the Series host of the chart.</summary>
        /// <remarks>This will always return null.</remarks>
        protected ISeriesHost SeriesHost { get; set; }

        ObservableCollection<IAxis> ISeriesHost.Axes
        {
            get
            {
                return this.InternalActualAxes as ObservableCollection<IAxis>;
            }
        }

        ObservableCollection<ISeries> ISeriesHost.Series
        {
            get
            {
                return (ObservableCollection<ISeries>)this.Series;
            }
        }
    }
}
