using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Defines the attributes of a series that is to be rendered by the DefinitionSeries class.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "DataShapeStyle", StyleTargetType = typeof(Shape))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(DataPoint))]
    public class SeriesDefinition : FrameworkElement, ISeries, IRequireSeriesHost, IRequireGlobalSeriesIndex
    {
        /// <summary>Identifies the ItemsSource dependency property.</summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(SeriesDefinition), new PropertyMetadata(new PropertyChangedCallback(SeriesDefinition.OnItemsSourceChanged)));
        /// <summary>Identifies the Title dependency property.</summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(object), typeof(SeriesDefinition), new PropertyMetadata(new PropertyChangedCallback(SeriesDefinition.OnTitleChanged)));
        /// <summary>Identifies the ActualTitle dependency property.</summary>
        public static readonly DependencyProperty ActualTitleProperty = DependencyProperty.Register(nameof(ActualTitle), typeof(object), typeof(SeriesDefinition), (PropertyMetadata)null);
        /// <summary>Identifies the DataPointStyle dependency property.</summary>
        public static readonly DependencyProperty DataPointStyleProperty = DependencyProperty.Register(nameof(DataPointStyle), typeof(Style), typeof(SeriesDefinition), new PropertyMetadata(new PropertyChangedCallback(SeriesDefinition.OnDataPointStyleChanged)));
        /// <summary>
        /// Identifies the ActualDataPointStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualDataPointStyleProperty = DependencyProperty.Register(nameof(ActualDataPointStyle), typeof(Style), typeof(SeriesDefinition), (PropertyMetadata)null);
        /// <summary>Identifies the LegendItemStyle dependency property.</summary>
        public static readonly DependencyProperty LegendItemStyleProperty = DependencyProperty.Register(nameof(LegendItemStyle), typeof(Style), typeof(SeriesDefinition), new PropertyMetadata(new PropertyChangedCallback(SeriesDefinition.OnLegendItemStyleChanged)));
        /// <summary>
        /// Identifies the ActualDataPointStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualLegendItemStyleProperty = DependencyProperty.Register(nameof(ActualLegendItemStyle), typeof(Style), typeof(SeriesDefinition), (PropertyMetadata)null);
        /// <summary>Identifies the DataShapeStyle dependency property.</summary>
        public static readonly DependencyProperty DataShapeStyleProperty = DependencyProperty.Register(nameof(DataShapeStyle), typeof(Style), typeof(SeriesDefinition), new PropertyMetadata(new PropertyChangedCallback(SeriesDefinition.OnDataShapeStyleChanged)));
        /// <summary>
        /// Identifies the ActualDataShapeStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualDataShapeStyleProperty = DependencyProperty.Register(nameof(ActualDataShapeStyle), typeof(Style), typeof(SeriesDefinition), (PropertyMetadata)null);
        /// <summary>
        /// Identifies the TransitionDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register(nameof(TransitionDuration), typeof(TimeSpan), typeof(SeriesDefinition), new PropertyMetadata((object)TimeSpan.FromSeconds(0.5)));
        /// <summary>
        /// Provides the store for the ISeries.LegendItems property.
        /// </summary>
        private readonly ObservableCollection<object> _legendItems = new ObservableCollection<object>();
        /// <summary>Name of the DataPointStyle property.</summary>
        private const string DataPointStyleName = "DataPointStyle";
        /// <summary>Name of the LegendItemStyle property.</summary>
        private const string LegendItemStyleName = "LegendItemStyle";
        /// <summary>Name of the DataShapeStyle property.</summary>
        private const string DataShapeStyleName = "DataShapeStyle";
        /// <summary>
        /// Represents the single LegendItem corresponding to the SeriesDefinition.
        /// </summary>
        private readonly LegendItem _legendItem;
        /// <summary>
        /// Keeps a reference to the WeakEventListener used to prevent leaks of collections assigned to the ItemsSource property.
        /// </summary>
        private WeakEventListener<SeriesDefinition, object, NotifyCollectionChangedEventArgs> _weakEventListener;
        /// <summary>Stores the automatic title of the series definition.</summary>
        private object _automaticTitle;
        /// <summary>
        /// Stores the DataPoint Style from the SeriesHost's Palette.
        /// </summary>
        private Style _paletteDataPointStyle;
        /// <summary>
        /// Stores the LegendItem Style from the SeriesHost's Palette.
        /// </summary>
        private Style _paletteLegendItemStyle;
        /// <summary>
        /// Stores the DataShape Style from the SeriesHost's Palette.
        /// </summary>
        private Style _paletteDataShapeStyle;
        /// <summary>
        /// The binding used to identify the dependent value binding.
        /// </summary>
        private Binding _dependentValueBinding;
        /// <summary>
        /// The binding used to identify the independent value binding.
        /// </summary>
        private Binding _independentValueBinding;
        /// <summary>Stores the SeriesHost for the series definition.</summary>
        private ISeriesHost _seriesHost;
        /// <summary>
        /// Identifies the TransitionEasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionEasingFunctionProperty;

        /// <summary>Gets or sets the index of the series definition.</summary>
        internal int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the SeriesDefinition class.
        /// </summary>
        public SeriesDefinition()
        {
            this._legendItem = new LegendItem()
            {
                Owner = (object)this
            };
            this._legendItem.SetBinding(ContentControl.ContentProperty, new Binding(nameof(ActualTitle))
            {
                Source = (object)this
            });
            this._legendItem.SetBinding(FrameworkElement.StyleProperty, new Binding(nameof(ActualLegendItemStyle))
            {
                Source = (object)this
            });
            this._legendItems.Add((object)this._legendItem);
        }

        /// <summary>
        /// Gets or sets a sequence that provides the content of the series.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(SeriesDefinition.ItemsSourceProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.ItemsSourceProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the ItemsSource dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesDefinition)o).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        /// <summary>Handles changes to the ItemsSource property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (null != oldValue as INotifyCollectionChanged && null != this._weakEventListener)
            {
                this._weakEventListener.Detach();
                this._weakEventListener = (WeakEventListener<SeriesDefinition, object, NotifyCollectionChangedEventArgs>)null;
            }
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                this._weakEventListener = new WeakEventListener<SeriesDefinition, object, NotifyCollectionChangedEventArgs>(this);
                this._weakEventListener.OnEventAction = (Action<SeriesDefinition, object, NotifyCollectionChangedEventArgs>)((instance, source, eventArgs) => instance.ItemsSourceCollectionChanged(source, eventArgs));
                this._weakEventListener.OnDetachAction = (Action<WeakEventListener<SeriesDefinition, object, NotifyCollectionChangedEventArgs>>)(weakEventListener => newValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(weakEventListener.OnEvent));
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this._weakEventListener.OnEvent);
            }
            if (null == this.ParentDefinitionSeries)
                return;
            this.ParentDefinitionSeries.SeriesDefinitionItemsSourceChanged(this, oldValue, newValue);
        }

        /// <summary>
        /// Handles the CollectionChanged event for the ItemsSource property.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments..</param>
        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (null == this.ParentDefinitionSeries)
                return;
            this.ParentDefinitionSeries.SeriesDefinitionItemsSourceCollectionChanged(this, e.Action, e.OldItems, e.OldStartingIndex, e.NewItems, e.NewStartingIndex);
        }

        /// <summary>
        /// Gets or sets the automatic title of the series definition.
        /// </summary>
        private object AutomaticTitle
        {
            get
            {
                return this._automaticTitle;
            }
            set
            {
                this._automaticTitle = value;
                this.ActualTitle = this.Title ?? this._automaticTitle;
            }
        }

        /// <summary>Gets or sets the Title of the series definition.</summary>
        public object Title
        {
            get
            {
                return this.GetValue(SeriesDefinition.TitleProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.TitleProperty, value);
            }
        }

        /// <summary>Handles changes to the Title dependency property.</summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesDefinition)o).OnTitleChanged(e.OldValue, e.NewValue);
        }

        /// <summary>Handles changes to the Title property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnTitleChanged(object oldValue, object newValue)
        {
            this.ActualTitle = newValue ?? this._automaticTitle;
        }

        /// <summary>Gets the rendered Title of the series definition.</summary>
        public object ActualTitle
        {
            get
            {
                return this.GetValue(SeriesDefinition.ActualTitleProperty);
            }
            protected set
            {
                this.SetValue(SeriesDefinition.ActualTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataPoint Style from the SeriesHost's Palette.
        /// </summary>
        internal Style PaletteDataPointStyle
        {
            get
            {
                return this._paletteDataPointStyle;
            }
            set
            {
                this._paletteDataPointStyle = value;
                this.ActualDataPointStyle = this.DataPointStyle ?? this._paletteDataPointStyle;
            }
        }

        /// <summary>
        /// Gets or sets the DataPoint Style for the series definition.
        /// </summary>
        public Style DataPointStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.DataPointStyleProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.DataPointStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the DataPointStyle dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnDataPointStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesDefinition)o).OnDataPointStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>Handles changes to the DataPointStyle property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnDataPointStyleChanged(Style oldValue, Style newValue)
        {
            this.ActualDataPointStyle = newValue ?? this._paletteDataPointStyle;
        }

        /// <summary>
        /// Gets the rendered DataPoint Style for the series definition.
        /// </summary>
        public Style ActualDataPointStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.ActualDataPointStyleProperty);
            }
            protected set
            {
                this.SetValue(SeriesDefinition.ActualDataPointStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the LegendItem Style from the SeriesHost's Palette.
        /// </summary>
        internal Style PaletteLegendItemStyle
        {
            get
            {
                return this._paletteLegendItemStyle;
            }
            set
            {
                this._paletteLegendItemStyle = value;
                this.ActualLegendItemStyle = this.LegendItemStyle ?? this._paletteLegendItemStyle;
            }
        }

        /// <summary>
        /// Gets or sets the LegendItem Style for the series definition.
        /// </summary>
        public Style LegendItemStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.LegendItemStyleProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.LegendItemStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the LegendItemStyle dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnLegendItemStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesDefinition)o).OnLegendItemStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>Handles changes to the LegendItemStyle property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnLegendItemStyleChanged(Style oldValue, Style newValue)
        {
            this.ActualLegendItemStyle = newValue ?? this._paletteLegendItemStyle;
        }

        /// <summary>
        /// Gets the rendered LegendItem Style for the series definition.
        /// </summary>
        public Style ActualLegendItemStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.ActualLegendItemStyleProperty);
            }
            protected set
            {
                this.SetValue(SeriesDefinition.ActualLegendItemStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the DataShape Style from the SeriesHost's Palette.
        /// </summary>
        internal Style PaletteDataShapeStyle
        {
            get
            {
                return this._paletteDataShapeStyle;
            }
            set
            {
                this._paletteDataShapeStyle = value;
                this.ActualDataShapeStyle = this.DataShapeStyle ?? this._paletteDataShapeStyle;
            }
        }

        /// <summary>
        /// Gets or sets the DataShape Style for the series definition.
        /// </summary>
        public Style DataShapeStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.DataShapeStyleProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.DataShapeStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Handles changes to the DataShapeStyle dependency property.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnDataShapeStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesDefinition)o).OnDataShapeStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>Handles changes to the DataShapeStyle property.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnDataShapeStyleChanged(Style oldValue, Style newValue)
        {
            this.ActualDataShapeStyle = newValue ?? this._paletteDataShapeStyle;
        }

        /// <summary>
        /// Gets the rendered DataShape Style for the series definition.
        /// </summary>
        public Style ActualDataShapeStyle
        {
            get
            {
                return (Style)this.GetValue(SeriesDefinition.ActualDataShapeStyleProperty);
            }
            protected set
            {
                this.SetValue(SeriesDefinition.ActualDataShapeStyleProperty, (object)value);
            }
        }

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
                this.Reset();
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
                this.Reset();
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

        /// <summary>Resets the display of the series definition.</summary>
        private void Reset()
        {
            if (null == this.ParentDefinitionSeries)
                return;
            this.ParentDefinitionSeries.SeriesDefinitionItemsSourceChanged(this, this.ItemsSource, this.ItemsSource);
        }

        /// <summary>Gets the SeriesHost as a DefinitionSeries instance.</summary>
        private DefinitionSeries ParentDefinitionSeries
        {
            get
            {
                return (DefinitionSeries)((IRequireSeriesHost)this).SeriesHost;
            }
        }

        ObservableCollection<object> ISeries.LegendItems
        {
            get
            {
                return this._legendItems;
            }
        }

        ISeriesHost IRequireSeriesHost.SeriesHost
        {
            get
            {
                return this._seriesHost;
            }
            set
            {
                this._seriesHost = value;
                if (!(this._seriesHost is DefinitionSeries) && null != value)
                    throw new NotSupportedException("SeriesDefinition.SeriesHost: InvalidParent");
                if (null == this._seriesHost)
                    return;
                DataPoint dataPoint = ((DefinitionSeries)this._seriesHost).InternalCreateDataPoint();
                ContentPresenter contentPresenter1 = new ContentPresenter();
                contentPresenter1.Content = (object)dataPoint;
                contentPresenter1.Width = 1.0;
                contentPresenter1.Height = 1.0;
                ContentPresenter contentPresenter2 = contentPresenter1;
                Popup popup = new Popup()
                {
                    Child = (UIElement)contentPresenter2
                };
                contentPresenter2.SizeChanged += (SizeChangedEventHandler)delegate
                {
                    popup.Child = (UIElement)null;
                    popup.IsOpen = false;
                };
                popup.IsOpen = true;
                dataPoint.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualDataPointStyle")
                {
                    Source = (object)this
                });
                this._legendItem.DataContext = (object)dataPoint;
            }
        }

        void IRequireGlobalSeriesIndex.GlobalSeriesIndexChanged(int? globalIndex)
        {
            if (!globalIndex.HasValue)
                return;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string seriesFormatString = "{0}";
            object[] objArray1 = new object[1];
            object[] objArray2 = objArray1;
            int index = 0;
            int? nullable = globalIndex;
            // FIX ME
            // ISSUE: variable of a boxed type
            // __Boxed<int?> local = (ValueType)(nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?()); // Original
            var local = (ValueType)(nullable.HasValue ? new int?(nullable.GetValueOrDefault() + 1) : new int?());
            objArray2[index] = (object)local;
            object[] objArray3 = objArray1;
            this.AutomaticTitle = (object)string.Format((IFormatProvider)currentCulture, seriesFormatString, objArray3);
        }

        /// <summary>
        /// Gets or sets the TimeSpan to use for the duration of data transitions.
        /// </summary>
        public TimeSpan TransitionDuration
        {
            get
            {
                return (TimeSpan)this.GetValue(SeriesDefinition.TransitionDurationProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.TransitionDurationProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the IEasingFunction to use for data transitions.
        /// </summary>
        public IEasingFunction TransitionEasingFunction
        {
            get
            {
                return (IEasingFunction)this.GetValue(SeriesDefinition.TransitionEasingFunctionProperty);
            }
            set
            {
                this.SetValue(SeriesDefinition.TransitionEasingFunctionProperty, (object)value);
            }
        }

        static SeriesDefinition()
        {
            string name = nameof(TransitionEasingFunction);
            Type propertyType = typeof(IEasingFunction);
            Type ownerType = typeof(SeriesDefinition);
            QuadraticEase quadraticEase = new QuadraticEase();
            quadraticEase.EasingMode = EasingMode.EaseInOut;
            PropertyMetadata typeMetadata = new PropertyMetadata((object)quadraticEase);
            SeriesDefinition.TransitionEasingFunctionProperty = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
        }
    }
}
