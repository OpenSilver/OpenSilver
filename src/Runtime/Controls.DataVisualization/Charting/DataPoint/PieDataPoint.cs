using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Represents a data point used for a pie series.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = "Slice", Type = typeof(UIElement))]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    public class PieDataPoint : DataPoint
    {
        /// <summary>Identifies the Geometry dependency property.</summary>
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>Identifies the GeometrySelection dependency property.</summary>
        public static readonly DependencyProperty GeometrySelectionProperty = DependencyProperty.Register(nameof(GeometrySelection), typeof(Geometry), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>Identifies the GeometryHighlight dependency property.</summary>
        public static readonly DependencyProperty GeometryHighlightProperty = DependencyProperty.Register(nameof(GeometryHighlight), typeof(Geometry), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>Identifies the ActualOffsetRatio dependency property.</summary>
        public static readonly DependencyProperty ActualOffsetRatioProperty = DependencyProperty.Register(nameof(ActualOffsetRatio), typeof(double), typeof(PieDataPoint), new PropertyMetadata(new PropertyChangedCallback(PieDataPoint.OnActualOffsetRatioPropertyChanged)));
        /// <summary>Identifies the ActualRatio dependency property.</summary>
        public static readonly DependencyProperty ActualRatioProperty = DependencyProperty.Register(nameof(ActualRatio), typeof(double), typeof(PieDataPoint), new PropertyMetadata(new PropertyChangedCallback(PieDataPoint.OnActualRatioPropertyChanged)));
        /// <summary>Identifies the FormattedRatio dependency property.</summary>
        public static readonly DependencyProperty FormattedRatioProperty = DependencyProperty.Register(nameof(FormattedRatio), typeof(string), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>Identifies the OffsetRatio dependency property.</summary>
        public static readonly DependencyProperty OffsetRatioProperty = DependencyProperty.Register(nameof(OffsetRatio), typeof(double), typeof(PieDataPoint), new PropertyMetadata(new PropertyChangedCallback(PieDataPoint.OnOffsetRatioPropertyChanged)));
        /// <summary>Identifies the Ratio dependency property.</summary>
        public static readonly DependencyProperty RatioProperty = DependencyProperty.Register(nameof(Ratio), typeof(double), typeof(PieDataPoint), new PropertyMetadata(new PropertyChangedCallback(PieDataPoint.OnRatioPropertyChanged)));
        /// <summary>Identifies the RatioStringFormat dependency property.</summary>
        public static readonly DependencyProperty RatioStringFormatProperty = DependencyProperty.Register(nameof(RatioStringFormat), typeof(string), typeof(PieDataPoint), new PropertyMetadata((object)null, new PropertyChangedCallback(PieDataPoint.OnRatioStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the ActualDataPointStyle dependency property.
        /// </summary>
        internal static readonly DependencyProperty ActualDataPointStyleProperty = DependencyProperty.Register(nameof(ActualDataPointStyle), typeof(Style), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>
        /// Identifies the ActualLegendItemStyle dependency property.
        /// </summary>
        internal static readonly DependencyProperty ActualLegendItemStyleProperty = DependencyProperty.Register(nameof(ActualLegendItemStyle), typeof(Style), typeof(PieDataPoint), (PropertyMetadata)null);
        /// <summary>The name of the slice template part.</summary>
        private const string SliceName = "Slice";
        /// <summary>Name of the ActualDataPointStyle property.</summary>
        internal const string ActualDataPointStyleName = "ActualDataPointStyle";

        /// <summary>
        /// Gets or sets the Geometry property which defines the shape of the
        /// data point.
        /// </summary>
        public Geometry Geometry
        {
            get
            {
                return this.GetValue(PieDataPoint.GeometryProperty) as Geometry;
            }
            set
            {
                this.SetValue(PieDataPoint.GeometryProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the Geometry which defines the shape of a point. The
        /// GeometrySelection property is a copy of the Geometry property.
        /// </summary>
        public Geometry GeometrySelection
        {
            get
            {
                return this.GetValue(PieDataPoint.GeometrySelectionProperty) as Geometry;
            }
            set
            {
                this.SetValue(PieDataPoint.GeometrySelectionProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the GeometryHighlight property which is a clone of the
        /// Geometry property.
        /// </summary>
        public Geometry GeometryHighlight
        {
            get
            {
                return this.GetValue(PieDataPoint.GeometryHighlightProperty) as Geometry;
            }
            set
            {
                this.SetValue(PieDataPoint.GeometryHighlightProperty, (object)value);
            }
        }

        /// <summary>
        /// Occurs when the actual offset ratio of the pie data point changes.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> ActualOffsetRatioChanged;

        /// <summary>
        /// Gets or sets the offset ratio that is displayed on the screen.
        /// </summary>
        public double ActualOffsetRatio
        {
            get
            {
                return (double)this.GetValue(PieDataPoint.ActualOffsetRatioProperty);
            }
            set
            {
                this.SetValue(PieDataPoint.ActualOffsetRatioProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the ActualOffsetRatioProperty property changes.
        /// </summary>
        /// <param name="d">PieDataPoint that changed its ActualOffsetRatio.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualOffsetRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieDataPoint)d).OnActualOffsetRatioPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the ActualOffsetRatioProperty property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        private void OnActualOffsetRatioPropertyChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> offsetRatioChanged = this.ActualOffsetRatioChanged;
            if (offsetRatioChanged != null)
                offsetRatioChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            PieSeries.UpdatePieDataPointGeometry(this, this.ActualWidth, this.ActualHeight);
        }

        /// <summary>
        /// An event raised when the actual ratio of the pie data point is
        /// changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> ActualRatioChanged;

        /// <summary>Gets or sets the ratio displayed on the screen.</summary>
        public double ActualRatio
        {
            get
            {
                return (double)this.GetValue(PieDataPoint.ActualRatioProperty);
            }
            set
            {
                this.SetValue(PieDataPoint.ActualRatioProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the ActualRatioProperty property changes.
        /// </summary>
        /// <param name="d">PieDataPoint that changed its ActualRatio.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieDataPoint)d).OnActualRatioPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the ActualRatioProperty property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        private void OnActualRatioPropertyChanged(double oldValue, double newValue)
        {
            if (ValueHelper.CanGraph(newValue))
            {
                RoutedPropertyChangedEventHandler<double> actualRatioChanged = this.ActualRatioChanged;
                if (actualRatioChanged != null)
                    actualRatioChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
            else
                this.ActualRatio = 0.0;
            if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            PieSeries.UpdatePieDataPointGeometry(this, this.ActualWidth, this.ActualHeight);
        }

        /// <summary>
        /// Gets the Ratio with the value of the RatioStringFormat property applied.
        /// </summary>
        public string FormattedRatio
        {
            get
            {
                return this.GetValue(PieDataPoint.FormattedRatioProperty) as string;
            }
        }

        /// <summary>
        /// An event raised when the offset ratio of the pie data point is
        /// changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> OffsetRatioChanged;

        /// <summary>Gets or sets the offset ratio of the pie data point.</summary>
        public double OffsetRatio
        {
            get
            {
                return (double)this.GetValue(PieDataPoint.OffsetRatioProperty);
            }
            set
            {
                this.SetValue(PieDataPoint.OffsetRatioProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the OffsetRatioProperty property changes.
        /// </summary>
        /// <param name="d">PieDataPoint that changed its OffsetRatio.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOffsetRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieDataPoint)d).OnOffsetRatioPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the OffsetRatioProperty property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        private void OnOffsetRatioPropertyChanged(double oldValue, double newValue)
        {
            if (ValueHelper.CanGraph(newValue))
            {
                RoutedPropertyChangedEventHandler<double> offsetRatioChanged = this.OffsetRatioChanged;
                if (offsetRatioChanged != null)
                    offsetRatioChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
                if (this.State != DataPointState.Created)
                    return;
                this.ActualOffsetRatio = newValue;
            }
            else
                this.OffsetRatio = 0.0;
        }

        /// <summary>
        /// An event raised when the ratio of the pie data point is
        /// changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> RatioChanged;

        /// <summary>
        /// Gets or sets the ratio of the total that the data point
        /// represents.
        /// </summary>
        public double Ratio
        {
            get
            {
                return (double)this.GetValue(PieDataPoint.RatioProperty);
            }
            set
            {
                this.SetValue(PieDataPoint.RatioProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the RatioProperty property changes.
        /// </summary>
        /// <param name="d">PieDataPoint that changed its Ratio.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieDataPoint)d).OnRatioPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the RatioProperty property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        private void OnRatioPropertyChanged(double oldValue, double newValue)
        {
            if (ValueHelper.CanGraph(newValue))
            {
                this.SetFormattedProperty(PieDataPoint.FormattedRatioProperty, this.RatioStringFormat, (object)newValue);
                RoutedPropertyChangedEventHandler<double> ratioChanged = this.RatioChanged;
                if (ratioChanged != null)
                    ratioChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
                if (this.State != DataPointState.Created)
                    return;
                this.ActualRatio = newValue;
            }
            else
                this.Ratio = 0.0;
        }

        /// <summary>
        /// Gets or sets the format string for the FormattedRatio property.
        /// </summary>
        public string RatioStringFormat
        {
            get
            {
                return this.GetValue(PieDataPoint.RatioStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(PieDataPoint.RatioStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the RatioStringFormatProperty property changes.
        /// </summary>
        /// <param name="d">PieDataPoint that changed its RatioStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRatioStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PieDataPoint).OnRatioStringFormatPropertyChanged(e.NewValue as string);
        }

        /// <summary>
        /// Called when the value of the RatioStringFormatProperty property changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnRatioStringFormatPropertyChanged(string newValue)
        {
            this.SetFormattedProperty(PieDataPoint.FormattedRatioProperty, newValue, (object)this.Ratio);
        }

        /// <summary>
        /// Gets or sets the actual style used for the data points.
        /// </summary>
        internal Style ActualDataPointStyle
        {
            get
            {
                return this.GetValue(PieDataPoint.ActualDataPointStyleProperty) as Style;
            }
            set
            {
                this.SetValue(PieDataPoint.ActualDataPointStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the actual style used for the legend item.
        /// </summary>
        internal Style ActualLegendItemStyle
        {
            get
            {
                return this.GetValue(PieDataPoint.ActualLegendItemStyleProperty) as Style;
            }
            set
            {
                this.SetValue(PieDataPoint.ActualLegendItemStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets the Palette-dispensed ResourceDictionary for the Series.
        /// </summary>
        protected internal ResourceDictionary PaletteResources { get; internal set; }

        /// <summary>
        /// Gets or sets the element that represents the pie slice.
        /// </summary>
        private UIElement SliceElement { get; set; }

        /// <summary>Initializes a new instance of the PieDataPoint class.</summary>
        public PieDataPoint()
        {
            this.DefaultStyleKey = (object)typeof(PieDataPoint);
            if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.ActualRatio = 0.2;
            this.SizeChanged += (SizeChangedEventHandler)((sender, e) =>
            {
                Size newSize = e.NewSize;
                double width = newSize.Width;
                newSize = e.NewSize;
                double height = newSize.Height;
                PieSeries.UpdatePieDataPointGeometry(this, width, height);
            });
        }

        /// <summary>
        /// Builds the visual tree for the PieDataPoint when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (null != this.SliceElement)
            {
                this.SliceElement.MouseEnter -= new MouseEventHandler(this.SliceElement_MouseEnter);
                this.SliceElement.MouseLeave -= new MouseEventHandler(this.SliceElement_MouseLeave);
            }
            base.OnApplyTemplate();
            this.SliceElement = this.GetTemplateChild("Slice") as UIElement;
            if (null == this.SliceElement)
                return;
            this.SliceElement.MouseEnter += new MouseEventHandler(this.SliceElement_MouseEnter);
            this.SliceElement.MouseLeave += new MouseEventHandler(this.SliceElement_MouseLeave);
        }

        /// <summary>Provides handling for the MouseEnter event.</summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
        }

        /// <summary>Provides handling for the MouseLeave event.</summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
        }

        /// <summary>Provides handling for the MouseEnter event.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">The event data.</param>
        private void SliceElement_MouseEnter(object sender, MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        /// <summary>Provides handling for the MouseLeave event.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">The event data.</param>
        private void SliceElement_MouseLeave(object sender, MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }
    }
}
