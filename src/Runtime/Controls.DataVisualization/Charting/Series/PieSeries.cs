using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in pie
    /// format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(PieDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    public class PieSeries : DataPointSeries, IResourceDictionaryDispenser, IRequireGlobalSeriesIndex
    {
        /// <summary>Identifies the Palette dependency property.</summary>
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(nameof(Palette), typeof(Collection<ResourceDictionary>), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(PieSeries.OnPalettePropertyChanged)));
        /// <summary>
        /// A dictionary that links data points to their legend items.
        /// </summary>
        private Dictionary<DataPoint, LegendItem> _dataPointLegendItems = new Dictionary<DataPoint, LegendItem>();
        /// <summary>The pie data point style enumerator.</summary>
        private IEnumerator<ResourceDictionary> _resourceDictionaryEnumerator;

        /// <summary>
        /// Gets or sets a palette of ResourceDictionaries used by the series.
        /// </summary>
        public Collection<ResourceDictionary> Palette
        {
            get
            {
                return this.GetValue(PieSeries.PaletteProperty) as Collection<ResourceDictionary>;
            }
            set
            {
                this.SetValue(PieSeries.PaletteProperty, (object)value);
            }
        }

        /// <summary>PaletteProperty property changed handler.</summary>
        /// <param name="d">Parent that changed its Palette.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PieSeries).OnPalettePropertyChanged(e.NewValue as Collection<ResourceDictionary>);
        }

        /// <summary>PaletteProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnPalettePropertyChanged(Collection<ResourceDictionary> newValue)
        {
            this.ResourceDictionaryDispenser.ResourceDictionaries = (IList<ResourceDictionary>)newValue;
        }

        /// <summary>Initializes a new instance of the PieSeries class.</summary>
        public PieSeries()
        {
#if !MIGRATION
            this.DefaultStyleKey = (object)typeof(PieSeries);
#endif
            this.ResourceDictionaryDispenser = new ResourceDictionaryDispenser();
            this.ResourceDictionaryDispenser.ResourceDictionariesChanged += (EventHandler)delegate
            {
                this.OnResourceDictionariesChanged(EventArgs.Empty);
            };
#if MIGRATION
            this.DefaultStyleKey = (object)typeof(PieSeries);
#endif
        }

        /// <summary>Invokes the ResourceDictionariesChanged event.</summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnResourceDictionariesChanged(EventArgs e)
        {
            this.Refresh();
            EventHandler dictionariesChanged = this.ResourceDictionariesChanged;
            if (null == dictionariesChanged)
                return;
            dictionariesChanged((object)this, e);
        }

        /// <summary>
        /// Accepts a ratio of a full rotation, the x and y length and returns
        /// the 2D point using trigonometric functions.
        /// </summary>
        /// <param name="ratio">The ratio of a full rotation [0..1].</param>
        /// <param name="radiusX">The x radius.</param>
        /// <param name="radiusY">The y radius.</param>
        /// <returns>The corresponding 2D point.</returns>
        private static Point ConvertRatioOfRotationToPoint(double ratio, double radiusX, double radiusY)
        {
            double num = (ratio * 360.0 - 90.0) * (Math.PI / 180.0);
            return new Point(radiusX * Math.Cos(num), radiusY * Math.Sin(num));
        }

        /// <summary>Creates a legend item for each data point.</summary>
        /// <param name="dataPoint">The data point added.</param>
        protected override void AddDataPoint(DataPoint dataPoint)
        {
            base.AddDataPoint(dataPoint);
            PieDataPoint pieDataPoint = (PieDataPoint)dataPoint;
            int index = this.ActiveDataPoints.IndexOf((object)dataPoint) + 1;
            LegendItem pieLegendItem = this.CreatePieLegendItem(dataPoint, index);
            if (this._resourceDictionaryEnumerator == null)
                this._resourceDictionaryEnumerator = DataPointSeries.GetResourceDictionaryWithTargetType((IResourceDictionaryDispenser)this, typeof(PieDataPoint), true);
            if (this._resourceDictionaryEnumerator.MoveNext())
            {
                ResourceDictionary resourceDictionary = this._resourceDictionaryEnumerator.Current.ShallowCopy();
                pieDataPoint.PaletteResources = resourceDictionary;
                pieDataPoint.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            else
                pieDataPoint.PaletteResources = (ResourceDictionary)null;
            pieDataPoint.ActualDataPointStyle = this.DataPointStyle ?? pieDataPoint.Resources[(object)"DataPointStyle"] as Style;
            pieDataPoint.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualDataPointStyle")
            {
                Source = (object)pieDataPoint
            });
            pieDataPoint.ActualLegendItemStyle = this.LegendItemStyle ?? pieDataPoint.Resources[(object)"LegendItemStyle"] as Style;
            pieLegendItem.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualLegendItemStyle")
            {
                Source = (object)pieDataPoint
            });
            this._dataPointLegendItems[dataPoint] = pieLegendItem;
            this.LegendItems.Add((object)pieLegendItem);
            this.UpdateLegendItemIndexes();
        }

        /// <summary>
        /// Removes data point's legend item when the data point is removed.
        /// </summary>
        /// <param name="dataPoint">The data point to remove.</param>
        protected override void RemoveDataPoint(DataPoint dataPoint)
        {
            base.RemoveDataPoint(dataPoint);
            if (dataPoint == null)
                return;
            LegendItem dataPointLegendItem = this._dataPointLegendItems[dataPoint];
            this._dataPointLegendItems.Remove(dataPoint);
            this.LegendItems.Remove((object)dataPointLegendItem);
            this.UpdateLegendItemIndexes();
        }

        /// <summary>Creates a data point.</summary>
        /// <returns>A data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return (DataPoint)new PieDataPoint();
        }

        /// <summary>Gets the active pie data points.</summary>
        private IEnumerable<PieDataPoint> ActivePieDataPoints
        {
            get
            {
                return this.ActiveDataPoints.OfType<PieDataPoint>();
            }
        }

        /// <summary>Updates all ratios before data points are updated.</summary>
        protected override void OnBeforeUpdateDataPoints()
        {
            this.UpdateRatios();
            base.OnBeforeUpdateDataPoints();
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            this.UpdateDataPoints((IEnumerable<DataPoint>)newDataPoints);
            base.OnDataPointsChanged(newDataPoints, oldDataPoints);
        }

        /// <summary>
        /// Updates the indexes of all legend items when a change is made to the collection.
        /// </summary>
        private void UpdateLegendItemIndexes()
        {
            int num = 0;
            foreach (DataPoint activeDataPoint in this.ActiveDataPoints)
            {
                this._dataPointLegendItems[activeDataPoint].Content = activeDataPoint.IndependentValue ?? (object)(num + 1);
                ++num;
            }
        }

        /// <summary>Updates the ratios of each data point.</summary>
        private void UpdateRatios()
        {
            double num1 = this.ActivePieDataPoints.Select<PieDataPoint, double>((Func<PieDataPoint, double>)(pieDataPoint => Math.Abs(ValueHelper.ToDouble((object)pieDataPoint.DependentValue)))).Sum();
            double num2 = 0.0;
            foreach (PieDataPoint activePieDataPoint in this.ActivePieDataPoints)
            {
                double num3 = Math.Abs(ValueHelper.ToDouble((object)activePieDataPoint.DependentValue)) / num1;
                if (!ValueHelper.CanGraph(num3))
                    num3 = 0.0;
                activePieDataPoint.Ratio = num3;
                activePieDataPoint.OffsetRatio = num2;
                num2 += num3;
            }
        }

        /// <summary>Updates a data point.</summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            PieDataPoint pieDataPoint = (PieDataPoint)dataPoint;
            pieDataPoint.Width = this.ActualWidth;
            pieDataPoint.Height = this.ActualHeight;
            PieSeries.UpdatePieDataPointGeometry(pieDataPoint, this.ActualWidth, this.ActualHeight);
            Canvas.SetLeft((UIElement)pieDataPoint, 0.0);
            Canvas.SetTop((UIElement)pieDataPoint, 0.0);
        }

        /// <summary>Updates the PieDataPoint's Geometry property.</summary>
        /// <param name="pieDataPoint">PieDataPoint instance.</param>
        /// <param name="plotAreaWidth">PlotArea width.</param>
        /// <param name="plotAreaHeight">PlotArea height.</param>
        internal static void UpdatePieDataPointGeometry(PieDataPoint pieDataPoint, double plotAreaWidth, double plotAreaHeight)
        {
            double num = (plotAreaWidth < plotAreaHeight ? plotAreaWidth : plotAreaHeight) * 0.95 / 2.0 - 0.0;
            Point offset = new Point(plotAreaWidth / 2.0, plotAreaHeight / 2.0);
            if (pieDataPoint.ActualRatio == 1.0)
            {
                DependencyProperty[] dependencyPropertyArray = new DependencyProperty[3]
                {
          PieDataPoint.GeometryProperty,
          PieDataPoint.GeometrySelectionProperty,
          PieDataPoint.GeometryHighlightProperty
                };
                foreach (DependencyProperty dp in dependencyPropertyArray)
                {
                    Geometry geometry = (Geometry)new EllipseGeometry()
                    {
                        Center = offset,
                        RadiusX = num,
                        RadiusY = num
                    };
                    pieDataPoint.SetValue(dp, (object)geometry);
                }
            }
            else if (pieDataPoint.ActualRatio == 0.0)
            {
                pieDataPoint.Geometry = (Geometry)null;
                pieDataPoint.GeometryHighlight = (Geometry)null;
                pieDataPoint.GeometrySelection = (Geometry)null;
            }
            else
            {
                double actualRatio = pieDataPoint.ActualRatio;
                double actualOffsetRatio = pieDataPoint.ActualOffsetRatio;
                double ratio = actualOffsetRatio + actualRatio;
                Point point1 = PieSeries.ConvertRatioOfRotationToPoint(actualOffsetRatio, num, num).Translate(offset);
                Point point2 = PieSeries.ConvertRatioOfRotationToPoint(ratio, num, num).Translate(offset);
                DependencyProperty[] dependencyPropertyArray = new DependencyProperty[3]
                {
          PieDataPoint.GeometryProperty,
          PieDataPoint.GeometrySelectionProperty,
          PieDataPoint.GeometryHighlightProperty
                };
                foreach (DependencyProperty dp in dependencyPropertyArray)
                {
                    PathFigure pathFigure = new PathFigure()
                    {
                        IsClosed = true
                    };
                    pathFigure.StartPoint = offset;
                    pathFigure.Segments.Add((PathSegment)new LineSegment()
                    {
                        Point = point1
                    });
                    bool flag = ratio - actualOffsetRatio > 0.5;
                    pathFigure.Segments.Add((PathSegment)new ArcSegment()
                    {
                        Point = point2,
                        IsLargeArc = flag,
                        Size = new Size(num, num),
                        SweepDirection = SweepDirection.Clockwise
                    });
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure);
                    pieDataPoint.SetValue(dp, (object)pathGeometry);
                }
            }
        }

        /// <summary>Creates a legend item from a data point.</summary>
        /// <param name="dataPoint">The data point to use to create the legend item.</param>
        /// <param name="index">The 1-based index of the Control.</param>
        /// <returns>The series host legend item.</returns>
        protected virtual LegendItem CreatePieLegendItem(DataPoint dataPoint, int index)
        {
            LegendItem legendItem = this.CreateLegendItem((DataPointSeries)this);
            legendItem.Content = dataPoint.IndependentValue ?? (object)index;
            DataPoint dataPoint1 = this.CreateDataPoint();
            dataPoint1.DataContext = dataPoint.DataContext;
            if (null != this.PlotArea)
            {
                this.PlotArea.Children.Add((UIElement)dataPoint1);
                this.PlotArea.Children.Remove((UIElement)dataPoint1);
            }
            dataPoint1.SetBinding(FrameworkElement.StyleProperty, new Binding("ActualDataPointStyle")
            {
                Source = (object)dataPoint
            });
            legendItem.DataContext = (object)dataPoint1;
            return legendItem;
        }

        /// <summary>Attach event handlers to a data point.</summary>
        /// <param name="dataPoint">The data point.</param>
        protected override void AttachEventHandlersToDataPoint(DataPoint dataPoint)
        {
            PieDataPoint pieDataPoint = dataPoint as PieDataPoint;
            pieDataPoint.ActualRatioChanged += new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointActualRatioChanged);
            pieDataPoint.ActualOffsetRatioChanged += new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointActualOffsetRatioChanged);
            pieDataPoint.RatioChanged += new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointRatioChanged);
            pieDataPoint.OffsetRatioChanged += new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointOffsetRatioChanged);
            base.AttachEventHandlersToDataPoint(dataPoint);
        }

        /// <summary>Detaches event handlers from a data point.</summary>
        /// <param name="dataPoint">The data point.</param>
        protected override void DetachEventHandlersFromDataPoint(DataPoint dataPoint)
        {
            PieDataPoint pieDataPoint = dataPoint as PieDataPoint;
            pieDataPoint.ActualRatioChanged -= new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointActualRatioChanged);
            pieDataPoint.ActualOffsetRatioChanged -= new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointActualOffsetRatioChanged);
            pieDataPoint.RatioChanged -= new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointRatioChanged);
            pieDataPoint.OffsetRatioChanged -= new RoutedPropertyChangedEventHandler<double>(this.OnPieDataPointOffsetRatioChanged);
            base.DetachEventHandlersFromDataPoint(dataPoint);
        }

        /// <summary>This method updates the global series index property.</summary>
        /// <param name="globalIndex">The global index of the series.</param>
        public void GlobalSeriesIndexChanged(int? globalIndex)
        {
        }

        /// <summary>
        /// Updates the data point when the dependent value is changed.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
            this.UpdateRatios();
            base.OnDataPointDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Updates the data point when the independent value is changed.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            this._dataPointLegendItems[dataPoint].Content = newValue;
            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Updates the data point when the pie data point's actual ratio is
        /// changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnPieDataPointActualRatioChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            this.UpdateDataPoint(sender as DataPoint);
        }

        /// <summary>
        /// Updates the data point when the pie data point's actual offset ratio
        /// is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnPieDataPointActualOffsetRatioChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            this.UpdateDataPoint(sender as DataPoint);
        }

        /// <summary>
        /// Updates the data point when the pie data point's ratio is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnPieDataPointRatioChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            (sender as DataPoint).BeginAnimation(PieDataPoint.ActualRatioProperty, "ActualRatio", (object)args.NewValue, this.TransitionDuration, this.TransitionEasingFunction);
        }

        /// <summary>
        /// Updates the data point when the pie data point's offset ratio is
        /// changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnPieDataPointOffsetRatioChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            (sender as DataPoint).BeginAnimation(PieDataPoint.ActualOffsetRatioProperty, "ActualOffsetRatio", (object)args.NewValue, this.TransitionDuration, this.TransitionEasingFunction);
        }

        /// <summary>
        /// Gets or sets an object used to dispense styles from the style
        /// palette.
        /// </summary>
        private ResourceDictionaryDispenser ResourceDictionaryDispenser { get; set; }

        /// <summary>
        /// Event that is invoked when the ResourceDictionaryDispenser's collection has changed.
        /// </summary>
        public event EventHandler ResourceDictionariesChanged;

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
        /// Called when the value of the SeriesHost property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new series host value.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);
            if (null != oldValue)
                oldValue.ResourceDictionariesChanged -= new EventHandler(this.SeriesHostResourceDictionariesChanged);
            if (null != newValue)
                newValue.ResourceDictionariesChanged += new EventHandler(this.SeriesHostResourceDictionariesChanged);
            else if (null != this._resourceDictionaryEnumerator)
            {
                this._resourceDictionaryEnumerator.Dispose();
                this._resourceDictionaryEnumerator = (IEnumerator<ResourceDictionary>)null;
            }
            this.ResourceDictionaryDispenser.Parent = (IResourceDictionaryDispenser)newValue;
        }

        /// <summary>
        /// Handles the SeriesHost's ResourceDictionariesChanged event.
        /// </summary>
        /// <param name="sender">ISeriesHost instance.</param>
        /// <param name="e">Event args.</param>
        private void SeriesHostResourceDictionariesChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        /// <summary>DataPointStyleProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnDataPointStylePropertyChanged(Style oldValue, Style newValue)
        {
            foreach (PieDataPoint activeDataPoint in this.ActiveDataPoints)
                activeDataPoint.ActualDataPointStyle = newValue ?? activeDataPoint.Resources[(object)"DataPointStyle"] as Style;
            base.OnDataPointStylePropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the value of the LegendItemStyle property changes.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnLegendItemStylePropertyChanged(Style oldValue, Style newValue)
        {
            foreach (PieDataPoint activeDataPoint in this.ActiveDataPoints)
                activeDataPoint.ActualLegendItemStyle = newValue ?? activeDataPoint.Resources[(object)"LegendItemStyle"] as Style;
            base.OnLegendItemStylePropertyChanged(oldValue, newValue);
        }
    }
}