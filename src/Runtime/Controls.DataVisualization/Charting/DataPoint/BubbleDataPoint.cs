namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Represents a data point used for a bubble series.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    public class BubbleDataPoint : DataPoint
    {
        /// <summary>Identifies the Size dependency property.</summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(double), typeof(BubbleDataPoint), new PropertyMetadata((object)0.0, new PropertyChangedCallback(BubbleDataPoint.OnSizePropertyChanged)));
        /// <summary>Identifies the ActualSize dependency property.</summary>
        public static readonly DependencyProperty ActualSizeProperty = DependencyProperty.Register(nameof(ActualSize), typeof(double), typeof(BubbleDataPoint), new PropertyMetadata((object)0.0, new PropertyChangedCallback(BubbleDataPoint.OnActualSizePropertyChanged)));

        /// <summary>Gets or sets the size value of the bubble data point.</summary>
        public double Size
        {
            get
            {
                return (double)this.GetValue(BubbleDataPoint.SizeProperty);
            }
            set
            {
                this.SetValue(BubbleDataPoint.SizeProperty, (object)value);
            }
        }

        /// <summary>SizeProperty property changed handler.</summary>
        /// <param name="d">BubbleDataPoint that changed its Size.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleDataPoint)d).OnSizePropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>SizeProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnSizePropertyChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> sizePropertyChanged = this.SizePropertyChanged;
            if (sizePropertyChanged != null)
                sizePropertyChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            if (this.State != DataPointState.Created)
                return;
            this.ActualSize = newValue;
        }

        /// <summary>
        /// This event is raised when the size property is changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> SizePropertyChanged;

        /// <summary>
        /// Gets or sets the actual size of the bubble data point.
        /// </summary>
        public double ActualSize
        {
            get
            {
                return (double)this.GetValue(BubbleDataPoint.ActualSizeProperty);
            }
            set
            {
                this.SetValue(BubbleDataPoint.ActualSizeProperty, (object)value);
            }
        }

        /// <summary>ActualSizeProperty property changed handler.</summary>
        /// <param name="d">BubbleDataPoint that changed its ActualSize.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleDataPoint)d).OnActualSizePropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>ActualSizeProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnActualSizePropertyChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> sizePropertyChanged = this.ActualSizePropertyChanged;
            if (sizePropertyChanged == null)
                return;
            sizePropertyChanged((object)this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
        }

        /// <summary>
        /// This event is raised when the actual size property is changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<double> ActualSizePropertyChanged;

        /// <summary>Initializes a new instance of the bubble data point.</summary>
        public BubbleDataPoint()
        {
            this.DefaultStyleKey = (object)typeof(BubbleDataPoint);
        }
    }
}