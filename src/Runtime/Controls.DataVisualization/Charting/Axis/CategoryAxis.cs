using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>The sort order to use when sorting categories.</summary>
    public enum CategorySortOrder
    {
        None,
        Ascending,
        Descending,
    }

    /// <summary>An axis that displays categories.</summary>
    [TemplatePart(Name = "AxisGrid", Type = typeof(Grid))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = "AxisTitle", Type = typeof(Title))]
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(AxisLabel))]
    public class CategoryAxis : DisplayAxis, ICategoryAxis, IAxis, IDataConsumer
    {
        /// <summary>Identifies the SortOrder dependency property.</summary>
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(nameof(SortOrder), typeof(CategorySortOrder), typeof(CategoryAxis), new PropertyMetadata((object)CategorySortOrder.None, new PropertyChangedCallback(CategoryAxis.OnSortOrderPropertyChanged)));
        /// <summary>A pool of major tick marks.</summary>
        private ObjectPool<Line> _majorTickMarkPool;
        /// <summary>A pool of labels.</summary>
        private ObjectPool<Control> _labelPool;

        /// <summary>Gets or sets the sort order used for the categories.</summary>
        public CategorySortOrder SortOrder
        {
            get
            {
                return (CategorySortOrder)this.GetValue(CategoryAxis.SortOrderProperty);
            }
            set
            {
                this.SetValue(CategoryAxis.SortOrderProperty, (object)value);
            }
        }

        /// <summary>SortOrderProperty property changed handler.</summary>
        /// <param name="d">CategoryAxis that changed its SortOrder.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSortOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CategoryAxis)d).OnSortOrderPropertyChanged();
        }

        /// <summary>SortOrderProperty property changed handler.</summary>
        private void OnSortOrderPropertyChanged()
        {
            this.Invalidate();
        }

        /// <summary>Gets or sets a list of categories to display.</summary>
        private IList<object> Categories { get; set; }

        /// <summary>Gets or sets the grid line coordinates to display.</summary>
        private IList<UnitValue> GridLineCoordinatesToDisplay { get; set; }

        /// <summary>
        /// Instantiates a new instance of the CategoryAxis class.
        /// </summary>
        public CategoryAxis()
        {
            this._labelPool = new ObjectPool<Control>((Func<Control>)(() => this.CreateAxisLabel()));
            this._majorTickMarkPool = new ObjectPool<Line>((Func<Line>)(() => this.CreateMajorTickMark()));
            this.Categories = (IList<object>)new List<object>();
            this.GridLineCoordinatesToDisplay = (IList<UnitValue>)new List<UnitValue>();
        }

        /// <summary>Updates categories when a series is registered.</summary>
        /// <param name="series">The series to be registered.</param>
        protected override void OnObjectRegistered(IAxisListener series)
        {
            base.OnObjectRegistered(series);
            if (!(series is IDataProvider))
                return;
            this.UpdateCategories();
        }

        /// <summary>Updates categories when a series is unregistered.</summary>
        /// <param name="series">The series to be unregistered.</param>
        protected override void OnObjectUnregistered(IAxisListener series)
        {
            base.OnObjectUnregistered(series);
            if (!(series is IDataProvider))
                return;
            this.UpdateCategories();
        }

        /// <summary>Returns range of coordinates for a given category.</summary>
        /// <param name="category">The category to return the range for.</param>
        /// <returns>The range of coordinates corresponding to the category.</returns>
        public Range<UnitValue> GetPlotAreaCoordinateRange(object category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            int num1 = this.Categories.IndexOf(category);
            if (num1 == -1)
                return new Range<UnitValue>();
            if (this.Orientation == AxisOrientation.X || this.Orientation == AxisOrientation.Y)
            {
                double num2 = Math.Max(this.ActualLength - 1.0, 0.0);
                double num3 = (double)num1 * num2 / (double)this.Categories.Count;
                double num4 = (double)(num1 + 1) * num2 / (double)this.Categories.Count;
                if (this.Orientation == AxisOrientation.X)
                    return new Range<UnitValue>(new UnitValue(num3, Unit.Pixels), new UnitValue(num4, Unit.Pixels));
                if (this.Orientation == AxisOrientation.Y)
                    return new Range<UnitValue>(new UnitValue(num2 - num4, Unit.Pixels), new UnitValue(num2 - num3, Unit.Pixels));
                return new Range<UnitValue>();
            }
            double num5 = 270.0;
            double num6 = (double)(360 / this.Categories.Count);
            double num7 = num6 / 2.0;
            int num8 = this.Categories.IndexOf(category);
            double num9 = num5 + (double)num8 * num6;
            return new Range<UnitValue>(new UnitValue(num9 - num7, Unit.Degrees), new UnitValue(num9 + num7, Unit.Degrees));
        }

        /// <summary>Returns the category at a given coordinate.</summary>
        /// <param name="position">The plot area position.</param>
        /// <returns>The category at the given plot area position.</returns>
        public object GetCategoryAtPosition(UnitValue position)
        {
            if (this.ActualLength == 0.0 || this.Categories.Count == 0)
                return (object)null;
            if (position.Unit != Unit.Pixels)
                throw new NotImplementedException();
            int index = (int)Math.Floor(position.Value / (this.ActualLength / (double)this.Categories.Count));
            if (index < 0 || index >= this.Categories.Count)
                return (object)null;
            if (this.Orientation == AxisOrientation.X)
                return this.Categories[index];
            return this.Categories[this.Categories.Count - 1 - index];
        }

        /// <summary>
        /// Updates the categories in response to an update from a registered
        /// axis data provider.
        /// </summary>
        /// <param name="dataProvider">The category axis information
        /// provider.</param>
        /// <param name="data">A sequence of categories.</param>
        public void DataChanged(IDataProvider dataProvider, IEnumerable<object> data)
        {
            this.UpdateCategories();
        }

        /// <summary>Updates the list of categories.</summary>
        private void UpdateCategories()
        {
            IEnumerable<object> source = this.RegisteredListeners.OfType<IDataProvider>().SelectMany<IDataProvider, object>((Func<IDataProvider, IEnumerable<object>>)(infoProvider => infoProvider.GetData((IDataConsumer)this))).Distinct<object>();
            if (this.SortOrder == CategorySortOrder.Ascending)
                source = (IEnumerable<object>)source.OrderBy<object, object>((Func<object, object>)(category => category));
            else if (this.SortOrder == CategorySortOrder.Descending)
                source = (IEnumerable<object>)source.OrderByDescending<object, object>((Func<object, object>)(category => category));
            this.Categories = (IList<object>)source.ToList<object>();
            this.Invalidate();
        }

        /// <summary>Returns the major axis grid line coordinates.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of the major grid line coordinates.</returns>
        protected override IEnumerable<UnitValue> GetMajorGridLineCoordinates(Size availableSize)
        {
            return (IEnumerable<UnitValue>)this.GridLineCoordinatesToDisplay;
        }

        /// <summary>The plot area coordinate of a value.</summary>
        /// <param name="value">The value for which to retrieve the plot area
        /// coordinate.</param>
        /// <returns>The plot area coordinate.</returns>
        public override UnitValue GetPlotAreaCoordinate(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Range<UnitValue> areaCoordinateRange = this.GetPlotAreaCoordinateRange(value);
            if (!areaCoordinateRange.HasData)
                return UnitValue.NaN();
            double num = areaCoordinateRange.Minimum.Value;
            return new UnitValue((areaCoordinateRange.Maximum.Value - num) / 2.0 + num, areaCoordinateRange.Minimum.Unit);
        }

        /// <summary>Creates and prepares a new axis label.</summary>
        /// <param name="value">The axis label value.</param>
        /// <returns>The axis label content control.</returns>
        private Control CreateAndPrepareAxisLabel(object value)
        {
            Control label = this._labelPool.Next();
            this.PrepareAxisLabel(label, value);
            return label;
        }

        /// <summary>Renders as an oriented axis.</summary>
        /// <param name="availableSize">The available size.</param>
        private void RenderOriented(Size availableSize)
        {
            this._labelPool.Reset();
            this._majorTickMarkPool.Reset();
            try
            {
                this.OrientedPanel.Children.Clear();
                this.GridLineCoordinatesToDisplay.Clear();
                if (this.Categories.Count <= 0)
                    return;
                double num1 = Math.Max(this.GetLength(availableSize) - 1.0, 0.0);
                Action<double> action = (Action<double>)(pos =>
                {
                    Line line = this._majorTickMarkPool.Next();
                    OrientedPanel.SetCenterCoordinate((UIElement)line, pos);
                    OrientedPanel.SetPriority((UIElement)line, 0);
                    this.GridLineCoordinatesToDisplay.Add(new UnitValue(pos, Unit.Pixels));
                    this.OrientedPanel.Children.Add((UIElement)line);
                });
                int num2 = 0;
                int num3 = 0;
                foreach (object category in (IEnumerable<object>)this.Categories)
                {
                    Control prepareAxisLabel = this.CreateAndPrepareAxisLabel(category);
                    double num4 = (double)num2 * num1 / (double)this.Categories.Count + 0.5;
                    double num5 = (double)(num2 + 1) * num1 / (double)this.Categories.Count + 0.5;
                    action(num4);
                    OrientedPanel.SetCenterCoordinate((UIElement)prepareAxisLabel, (num4 + num5) / 2.0);
                    OrientedPanel.SetPriority((UIElement)prepareAxisLabel, num3 + 1);
                    this.OrientedPanel.Children.Add((UIElement)prepareAxisLabel);
                    ++num2;
                    num3 = (num3 + 1) % 2;
                }
                action(num1 + 0.5);
            }
            finally
            {
                this._labelPool.Done();
                this._majorTickMarkPool.Done();
            }
        }

        /// <summary>
        /// Renders the axis labels, tick marks, and other visual elements.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        protected override void Render(Size availableSize)
        {
            this.RenderOriented(availableSize);
        }

        /// <summary>
        /// Returns a value indicating whether a value can be plotted on the
        /// axis.
        /// </summary>
        /// <param name="value">A value which may or may not be able to be
        /// plotted.</param>
        /// <returns>A value indicating whether a value can be plotted on the
        /// axis.</returns>
        public override bool CanPlot(object value)
        {
            return true;
        }
    }
}
