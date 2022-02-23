// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Markup;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that displays a Chart.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = Chart.ChartAreaName, Type = typeof(EdgePanel))]
    [TemplatePart(Name = Chart.LegendName, Type = typeof(Legend))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [StyleTypedProperty(Property = "LegendStyle", StyleTargetType = typeof(Legend))]
    [StyleTypedProperty(Property = "ChartAreaStyle", StyleTargetType = typeof(EdgePanel))]
    [StyleTypedProperty(Property = "PlotAreaStyle", StyleTargetType = typeof(Grid))]
    [ContentProperty("Series")]
    [OpenSilver.NotImplemented]
    public partial class Chart : Control //, ISeriesHost
    {
        /// <summary>
        /// Specifies the name of the ChartArea TemplatePart.
        /// </summary>
        private const string ChartAreaName = "ChartArea";

        /// <summary>
        /// Specifies the name of the legend TemplatePart.
        /// </summary>
        private const string LegendName = "Legend";

        /// <summary>
        /// Gets or sets a collection of Axes in the Chart.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is public to work around a limitation with the XAML editing tools.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Setter is public to work around a limitation with the XAML editing tools.")]
        public Collection<IAxis> Axes
        {
            get
            {
                return _axes;
            }
            set
            {
                throw new NotSupportedException(Properties.Resources.Chart_Axes_SetterNotSupported);
            }
        }

        /// <summary>
        /// Stores the collection of Axes in the Chart.
        /// </summary>
        private Collection<IAxis> _axes;

        /// <summary>
        /// Gets or sets the axes that are currently in the chart.
        /// </summary>
        private IList<IAxis> InternalActualAxes { get; set; }

        /// <summary>
        /// Gets the actual axes displayed in the chart.
        /// </summary>
        public ReadOnlyCollection<IAxis> ActualAxes { get; private set; }

        /// <summary>
        /// Gets or sets the reference to the template's ChartArea.
        /// </summary>
        private EdgePanel ChartArea { get; set; }

        /// <summary>
        /// Gets or sets the reference to the Chart's Legend.
        /// </summary>
        private Legend Legend { get; set; }

        /// <summary>
        /// Gets or sets the collection of Series displayed by the Chart.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is public to work around a limitation with the XAML editing tools.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Setter is public to work around a limitation with the XAML editing tools.")]
        public Collection<ISeries> Series
        {
            get
            {
                return _series;
            }
            set
            {
                throw new NotSupportedException(Properties.Resources.Chart_Series_SetterNotSupported);
            }
        }

        /// <summary>
        /// Stores the collection of Series displayed by the Chart.
        /// </summary>
        private Collection<ISeries> _series;

        #region public Style ChartAreaStyle
        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's ChartArea.
        /// </summary>
        public Style ChartAreaStyle
        {
            get { return GetValue(ChartAreaStyleProperty) as Style; }
            set { SetValue(ChartAreaStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ChartAreaStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartAreaStyleProperty =
            DependencyProperty.Register(
                "ChartAreaStyle",
                typeof(Style),
                typeof(Chart),
                null);
        #endregion public Style ChartAreaStyle

        /// <summary>
        /// Gets the collection of legend items.
        /// </summary>
        public Collection<object> LegendItems { get; private set; }

        #region public Style LegendStyle
        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's Legend.
        /// </summary>
        public Style LegendStyle
        {
            get { return GetValue(LegendStyleProperty) as Style; }
            set { SetValue(LegendStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the LegendStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendStyleProperty =
            DependencyProperty.Register(
                "LegendStyle",
                typeof(Style),
                typeof(Chart),
                null);
        #endregion public Style LegendStyle

        #region public object LegendTitle
        /// <summary>
        /// Gets or sets the Title content of the Legend.
        /// </summary>
        public object LegendTitle
        {
            get { return GetValue(LegendTitleProperty); }
            set { SetValue(LegendTitleProperty, value); }
        }

        /// <summary>
        /// Identifies the LegendTitle dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleProperty =
            DependencyProperty.Register(
                "LegendTitle",
                typeof(object),
                typeof(Chart),
                null);
        #endregion public object LegendTitle

        #region public Style PlotAreaStyle
        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's PlotArea.
        /// </summary>
        public Style PlotAreaStyle
        {
            get { return GetValue(PlotAreaStyleProperty) as Style; }
            set { SetValue(PlotAreaStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the PlotAreaStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaStyleProperty =
            DependencyProperty.Register(
                "PlotAreaStyle",
                typeof(Style),
                typeof(Chart),
                null);
        #endregion public Style PlotAreaStyle

        #region public Collection<ResourceDictionary> Palette
        /// <summary>
        /// Gets or sets a palette of ResourceDictionaries used by the children of the Chart.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Want to allow this to be set from XAML.")]
        public Collection<ResourceDictionary> Palette
        {
            get { return GetValue(PaletteProperty) as Collection<ResourceDictionary>; }
            set { SetValue(PaletteProperty, value); }
        }

        /// <summary>
        /// Identifies the Palette dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(
                "Palette",
                typeof(Collection<ResourceDictionary>),
                typeof(Chart),
                new PropertyMetadata(OnPalettePropertyChanged));

        /// <summary>
        /// Called when the value of the Palette property is changed.
        /// </summary>
        /// <param name="d">Chart that contains the changed Palette.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnPalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Chart source = (Chart)d;
            Collection<ResourceDictionary> newValue = (Collection<ResourceDictionary>)e.NewValue;
            source.OnPalettePropertyChanged(newValue);
        }

        /// <summary>
        /// Called when the value of the Palette property is changed.
        /// </summary>
        /// <param name="newValue">The new value for the Palette.</param>
        [OpenSilver.NotImplemented]
        private void OnPalettePropertyChanged(Collection<ResourceDictionary> newValue)
        {
        }
        #endregion public Collection<ResourceDictionary> Palette

        /// <summary>
        /// Event that is invoked when the ResourceDictionaryDispenser's collection has changed.
        /// </summary>
        public event EventHandler ResourceDictionariesChanged;

        #region public object Title
        /// <summary>
        /// Gets or sets the title displayed for the Chart.
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(object),
                typeof(Chart),
                null);
        #endregion

        #region public Style TitleStyle
        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's Title.
        /// </summary>
        public Style TitleStyle
        {
            get { return GetValue(TitleStyleProperty) as Style; }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TitleStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register(
                "TitleStyle",
                typeof(Style),
                typeof(Chart),
                null);
        #endregion public Style TitleStyle

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the Chart class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Chart()
        {
            _axes = new Collection<IAxis>();
            _series = new Collection<ISeries>();
            InternalActualAxes = new ReadOnlyCollection<IAxis>(new List<IAxis>());
            ActualAxes = new ReadOnlyCollection<IAxis>(InternalActualAxes);
            LegendItems = new Collection<object>();
        }

        /// <summary>
        /// Invokes the ResourceDictionariesChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnResourceDictionariesChanged(EventArgs e)
        {
            // Forward event on to listeners
            EventHandler handler = ResourceDictionariesChanged;
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        /// <summary>
        /// Adds an axis to the ISeriesHost area.
        /// </summary>
        /// <param name="axis">The axis to add to the ISeriesHost area.</param>
        [OpenSilver.NotImplemented]
        private void AddAxisToChartArea(Axis axis)
        {
        }

        /// <summary>
        /// Rebuilds the chart area if an axis orientation is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        [OpenSilver.NotImplemented]
        private void AxisOrientationChanged(object sender, RoutedPropertyChangedEventArgs<AxisOrientation> args)
        {
            Axis axis = (Axis)sender;
        }

        /// <summary>
        /// Sets the Edge property of an axis based on its location and
        /// orientation.
        /// </summary>
        /// <param name="axis">The axis to set the edge property of.</param>
        [OpenSilver.NotImplemented]
        private static void SetEdge(Axis axis)
        {
        }

        /// <summary>
        /// Adds a series to the plot area and injects chart services.
        /// </summary>
        /// <param name="series">The series to add to the plot area.</param>
        [OpenSilver.NotImplemented]
        private void AddSeriesToPlotArea(ISeries series)
        {
        }

        /// <summary>
        /// Builds the visual tree for the Chart control when a new template
        /// is applied.
        /// </summary>
        [OpenSilver.NotImplemented]
        public override void OnApplyTemplate()
        {
            // Call base implementation
            base.OnApplyTemplate();

            // Unhook events from former template parts
            if (null != ChartArea)
            {
                ChartArea.Children.Clear();
            }

            if (null != Legend)
            {
                Legend.ItemsSource = null;
            }

            // Access new template parts
            ChartArea = GetTemplateChild(ChartAreaName) as EdgePanel;

            Legend = GetTemplateChild(LegendName) as Legend;

            if (ChartArea != null)
            {
                //_chartAreaChildrenListAdapter.TargetList = ChartArea.Children;
                //_chartAreaChildrenListAdapter.Populate();
            }

            if (Legend != null)
            {
                Legend.ItemsSource = this.LegendItems;
            }
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
                {
                    AddAxisToChartArea(axis);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Axis axis in e.OldItems.OfType<Axis>())
                {
                    RemoveAxisFromChartArea(axis);
                }
            }
        }

        /// <summary>
        /// Removes an axis from the Chart area.
        /// </summary>
        /// <param name="axis">The axis to remove from the ISeriesHost area.</param>
        [OpenSilver.NotImplemented]
        private void RemoveAxisFromChartArea(Axis axis)
        {
        }

        /// <summary>
        /// Removes a series from the plot area.
        /// </summary>
        /// <param name="series">The series to remove from the plot area.
        /// </param>
        [OpenSilver.NotImplemented]
        private void RemoveSeriesFromPlotArea(ISeries series)
        {
        }

        /// <summary>
        /// Called when the ObservableCollection.CollectionChanged property
        /// changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        [OpenSilver.NotImplemented]
        private void SeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles changes to the collections of child ISeries implementing ISeriesHost.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ChildSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnGlobalSeriesIndexesInvalidated(this, new RoutedEventArgs());
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
        [OpenSilver.NotImplemented]
        public IEnumerator<ResourceDictionary> GetResourceDictionariesWhere(Func<ResourceDictionary, bool> predicate)
        {
            //return ResourceDictionaryDispenser.GetResourceDictionariesWhere(predicate);
            return Enumerable.Empty<ResourceDictionary>().GetEnumerator();
        }

        /// <summary>
        /// Updates the global indexes of all descendents that require a global
        /// index.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void OnGlobalSeriesIndexesInvalidated(object sender, RoutedEventArgs args)
        {
            UpdateGlobalIndexes();
        }

        /// <summary>
        /// Updates the global index property of all Series that track their
        /// global index.
        /// </summary>
        [OpenSilver.NotImplemented]
        private void UpdateGlobalIndexes()
        {
        }
    }
}
