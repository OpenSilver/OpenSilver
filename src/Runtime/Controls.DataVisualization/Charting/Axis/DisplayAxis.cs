// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

#if MIGRATION

using System.Windows.Media;
using System.Windows.Shapes;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{

    /// <summary>An axis that has a range.</summary>
    public abstract class DisplayAxis : Axis, IRequireSeriesHost
    {
        /// <summary>Identifies the AxisLabelStyle dependency property.</summary>
        public static readonly DependencyProperty AxisLabelStyleProperty = DependencyProperty.Register(nameof(AxisLabelStyle), typeof(Style), typeof(DisplayAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DisplayAxis.OnAxisLabelStylePropertyChanged)));
        /// <summary>
        /// Identifies the MajorTickMarkStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorTickMarkStyleProperty = DependencyProperty.Register(nameof(MajorTickMarkStyle), typeof(Style), typeof(DisplayAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DisplayAxis.OnMajorTickMarkStylePropertyChanged)));
        /// <summary>Identifies the Title dependency property.</summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(object), typeof(DisplayAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DisplayAxis.OnTitlePropertyChanged)));
        /// <summary>Identifies the TitleStyle dependency property.</summary>
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register(nameof(TitleStyle), typeof(Style), typeof(DisplayAxis), (PropertyMetadata)null);
        /// <summary>Identifies the ShowGridLines dependency property.</summary>
        public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register(nameof(ShowGridLines), typeof(bool), typeof(DisplayAxis), new PropertyMetadata((object)false, new PropertyChangedCallback(DisplayAxis.OnShowGridLinesPropertyChanged)));
        /// <summary>Identifies the GridlineStyle dependency property.</summary>
        public static readonly DependencyProperty GridLineStyleProperty = DependencyProperty.Register(nameof(GridLineStyle), typeof(Style), typeof(DisplayAxis), (PropertyMetadata)null);
        /// <summary>Maximum intervals per 200 pixels.</summary>
        protected const double MaximumAxisIntervalsPer200Pixels = 8.0;
        /// <summary>The name of the axis grid template part.</summary>
        protected const string AxisGridName = "AxisGrid";
        /// <summary>The name of the axis title template part.</summary>
        protected const string AxisTitleName = "AxisTitle";
        /// <summary>This field stores the grid lines element.</summary>
        private DisplayAxisGridLines _gridLines;
        /// <summary>The grid used to layout the axis.</summary>
        private Grid _grid;
        /// <summary>The control used to display the axis title.</summary>
        private System.Windows.Controls.DataVisualization.Title _axisTitle;
        /// <summary>The series host.</summary>
        private ISeriesHost _seriesHost;

        /// <summary>Gets or sets the style used for the axis labels.</summary>
        public Style AxisLabelStyle
        {
            get
            {
                return this.GetValue(DisplayAxis.AxisLabelStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DisplayAxis.AxisLabelStyleProperty, (object)value);
            }
        }

        /// <summary>AxisLabelStyleProperty property changed handler.</summary>
        /// <param name="d">DisplayAxis that changed its AxisLabelStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAxisLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayAxis)d).OnAxisLabelStylePropertyChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>AxisLabelStyleProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnAxisLabelStylePropertyChanged(Style oldValue, Style newValue)
        {
        }

        /// <summary>Gets the actual length.</summary>
        protected double ActualLength
        {
            get
            {
                return this.GetLength(new Size(this.ActualWidth, this.ActualHeight));
            }
        }

        /// <summary>
        /// Returns the length of the axis given an available size.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The length of the axis given an available size.</returns>
        protected double GetLength(Size availableSize)
        {
            if (this.ActualHeight == 0.0 && this.ActualWidth == 0.0)
                return 0.0;
            if (this.Orientation == AxisOrientation.X)
                return availableSize.Width;
            if (this.Orientation == AxisOrientation.Y)
                return availableSize.Height;
            throw new InvalidOperationException("DisplayAxis.GetLength: Cannot Determine The Length Of An Axis With An Orientation Of None");
        }

        /// <summary>Gets or sets the grid lines property.</summary>
        internal DisplayAxisGridLines GridLines
        {
            get
            {
                return this._gridLines;
            }
            set
            {
                if (value == this._gridLines)
                    return;
                DisplayAxisGridLines gridLines = this._gridLines;
                this._gridLines = value;
                this.OnGridLinesPropertyChanged(gridLines, value);
            }
        }

        /// <summary>GridLinesProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnGridLinesPropertyChanged(DisplayAxisGridLines oldValue, DisplayAxisGridLines newValue)
        {
            if (this.SeriesHost != null && oldValue != null)
                this.SeriesHost.BackgroundElements.Remove((UIElement)oldValue);
            if (this.SeriesHost == null || newValue == null)
                return;
            this.SeriesHost.BackgroundElements.Add((UIElement)newValue);
        }

        /// <summary>
        /// Gets or sets the style applied to the Axis tick marks.
        /// </summary>
        /// <value>The Style applied to the Axis tick marks.</value>
        public Style MajorTickMarkStyle
        {
            get
            {
                return this.GetValue(DisplayAxis.MajorTickMarkStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DisplayAxis.MajorTickMarkStyleProperty, (object)value);
            }
        }

        /// <summary>MajorTickMarkStyleProperty property changed handler.</summary>
        /// <param name="d">DisplayAxis that changed its MajorTickMarkStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMajorTickMarkStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayAxis)d).OnMajorTickMarkStylePropertyChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>MajorTickMarkStyleProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnMajorTickMarkStylePropertyChanged(Style oldValue, Style newValue)
        {
        }

        /// <summary>Gets or sets the title property.</summary>
        public object Title
        {
            get
            {
                return this.GetValue(DisplayAxis.TitleProperty);
            }
            set
            {
                this.SetValue(DisplayAxis.TitleProperty, value);
            }
        }

        /// <summary>TitleProperty property changed handler.</summary>
        /// <param name="d">DisplayAxis that changed its Title.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayAxis)d).OnTitlePropertyChanged(e.OldValue, e.NewValue);
        }

        /// <summary>TitleProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnTitlePropertyChanged(object oldValue, object newValue)
        {
            if (this.AxisTitle == null)
                return;
            this.AxisTitle.Content = this.Title;
        }

        /// <summary>
        /// Gets or sets the LayoutTransformControl used to rotate the title.
        /// </summary>
        private LayoutTransformControl TitleLayoutTransformControl { get; set; }

        /// <summary>Gets or sets the style applied to the Axis title.</summary>
        /// <value>The Style applied to the Axis title.</value>
        public Style TitleStyle
        {
            get
            {
                return this.GetValue(DisplayAxis.TitleStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DisplayAxis.TitleStyleProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grid lines should be shown.
        /// </summary>
        public bool ShowGridLines
        {
            get
            {
                return (bool)this.GetValue(DisplayAxis.ShowGridLinesProperty);
            }
            set
            {
                this.SetValue(DisplayAxis.ShowGridLinesProperty, (object)value);
            }
        }

        /// <summary>ShowGridLinesProperty property changed handler.</summary>
        /// <param name="d">Axis that changed its ShowGridLines.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnShowGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayAxis)d).OnShowGridLinesPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>ShowGridLinesProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnShowGridLinesPropertyChanged(bool oldValue, bool newValue)
        {
            this.SetShowGridLines(newValue);
        }

        /// <summary>
        /// Creates and destroys a grid lines element based on the specified
        /// value.
        /// </summary>
        /// <param name="newValue">A value indicating whether to display grid
        /// lines or not.</param>
        private void SetShowGridLines(bool newValue)
        {
            if (newValue)
                this.GridLines = (DisplayAxisGridLines)new OrientedAxisGridLines(this);
            else
                this.GridLines = (DisplayAxisGridLines)null;
        }

        /// <summary>Gets or sets the Style of the Axis's gridlines.</summary>
        public Style GridLineStyle
        {
            get
            {
                return this.GetValue(DisplayAxis.GridLineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DisplayAxis.GridLineStyleProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the grid used to layout the axis.</summary>
        private Grid AxisGrid
        {
            get
            {
                return this._grid;
            }
            set
            {
                if (this._grid == value)
                    return;
                if (this._grid != null)
                    this._grid.Children.Clear();
                this._grid = value;
                if (this._grid != null)
                {
                    this._grid.Children.Add((UIElement)this.OrientedPanel);
                    if (this.AxisTitle != null)
                        this._grid.Children.Add((UIElement)this.AxisTitle);
                }
            }
        }

        /// <summary>Gets or sets a grid to lay out the dependent axis.</summary>
        private Grid DependentAxisGrid { get; set; }

        /// <summary>
        /// Gets the oriented panel used to layout the axis labels.
        /// </summary>
        internal OrientedPanel OrientedPanel { get; private set; }

        /// <summary>
        /// Gets or sets the title control used to display the title.
        /// </summary>
        private System.Windows.Controls.DataVisualization.Title AxisTitle
        {
            get
            {
                return this._axisTitle;
            }
            set
            {
                if (this._axisTitle == value)
                    return;
                if (this._axisTitle != null)
                    this._axisTitle.Content = (object)null;
                this._axisTitle = value;
                if (this.Title != null)
                    this._axisTitle.Content = this.Title;
            }
        }

        /// <summary>Creates a major axis tick mark.</summary>
        /// <returns>A line to used to render a tick mark.</returns>
        protected virtual Line CreateMajorTickMark()
        {
            return this.CreateTickMark(this.MajorTickMarkStyle);
        }

        /// <summary>Creates a tick mark and applies a style to it.</summary>
        /// <param name="style">The style to apply.</param>
        /// <returns>The newly created tick mark.</returns>
        protected Line CreateTickMark(Style style)
        {
            Line line = new Line();
            line.Style = style;
            if (this.Orientation == AxisOrientation.Y)
            {
                line.Y1 = 0.5;
                line.Y2 = 0.5;
            }
            else if (this.Orientation == AxisOrientation.X)
            {
                line.X1 = 0.5;
                line.X2 = 0.5;
            }
            return line;
        }

        /// <summary>
        /// This method is used to share the grid line coordinates with the
        /// internal grid lines control.
        /// </summary>
        /// <returns>A sequence of the major grid line coordinates.</returns>
        internal IEnumerable<UnitValue> InternalGetMajorGridLinePositions()
        {
            return this.GetMajorGridLineCoordinates(new Size(this.ActualWidth, this.ActualHeight));
        }

        /// <summary>
        /// Returns the coordinates to use for the grid line control.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of coordinates at which to draw grid lines.</returns>
        protected abstract IEnumerable<UnitValue> GetMajorGridLineCoordinates(Size availableSize);

        /// <summary>Instantiates a new instance of the DisplayAxis class.</summary>
        protected DisplayAxis()
        {
            this.OrientedPanel = new OrientedPanel();
#if !MIGRATION
            this.DefaultStyleKey = (object)typeof(DisplayAxis);
#endif
            this.OrientedPanel.UseLayoutRounding = true;
            this.DependentAxisGrid = new Grid();
            this.TitleLayoutTransformControl = new LayoutTransformControl();
            this.TitleLayoutTransformControl.HorizontalAlignment = HorizontalAlignment.Center;
            this.TitleLayoutTransformControl.VerticalAlignment = VerticalAlignment.Center;
            this.SizeChanged += new SizeChangedEventHandler(this.DisplayAxisSizeChanged);

#if MIGRATION
            this.DefaultStyleKey = (object)typeof(DisplayAxis);
#endif
        }

        /// <summary>If display axis has just become visible, invalidate.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void DisplayAxisSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != 0.0 || e.PreviousSize.Height != 0.0)
                return;
            this.Invalidate();
        }

        /// <summary>Creates an axis label.</summary>
        /// <returns>The new axis label.</returns>
        protected virtual Control CreateAxisLabel()
        {
            return (Control)new AxisLabel();
        }

        /// <summary>
        /// Updates the grid lines element if a suitable dependent axis has
        /// been added to a radial axis.
        /// </summary>
        protected override void OnDependentAxesCollectionChanged()
        {
            this.SetShowGridLines(this.ShowGridLines);
            base.OnDependentAxesCollectionChanged();
        }

        /// <summary>Prepares an axis label to be plotted.</summary>
        /// <param name="label">The axis label to prepare.</param>
        /// <param name="dataContext">The data context to use for the axis
        /// label.</param>
        protected virtual void PrepareAxisLabel(Control label, object dataContext)
        {
            label.DataContext = dataContext;
            label.SetStyle(this.AxisLabelStyle);
        }

        /// <summary>Retrieves template parts and configures layout.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AxisGrid = this.GetTemplateChild("AxisGrid") as Grid;
            this.AxisTitle = this.GetTemplateChild("AxisTitle") as System.Windows.Controls.DataVisualization.Title;
            if (this.AxisTitle != null && this.AxisGrid.Children.Contains((UIElement)this.AxisTitle))
            {
                this.AxisGrid.Children.Remove((UIElement)this.AxisTitle);
                this.TitleLayoutTransformControl.Child = (FrameworkElement)this.AxisTitle;
                this.AxisGrid.Children.Add((UIElement)this.TitleLayoutTransformControl);
            }
            this.ArrangeAxisGrid();
        }

        /// <summary>
        /// When the size of the oriented panel changes invalidate the axis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnOrientedPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>
        /// Arranges the grid when the location property is changed.
        /// </summary>
        /// <param name="oldValue">The old location.</param>
        /// <param name="newValue">The new location.</param>
        protected override void OnLocationPropertyChanged(AxisLocation oldValue, AxisLocation newValue)
        {
            this.ArrangeAxisGrid();
            base.OnLocationPropertyChanged(oldValue, newValue);
        }

        /// <summary>Arranges the elements in the axis grid.</summary>
        private void ArrangeAxisGrid()
        {
            if (this.AxisGrid == null)
                return;
            this.AxisGrid.ColumnDefinitions.Clear();
            this.AxisGrid.RowDefinitions.Clear();
            this.AxisGrid.Children.Clear();
            if (this.Orientation == AxisOrientation.Y)
            {
                this.OrientedPanel.Orientation = Controls.Orientation.Vertical;
                this.OrientedPanel.IsReversed = true;
                if (this.Location == AxisLocation.Left || this.Location == AxisLocation.Right)
                {
                    this.TitleLayoutTransformControl.Transform = (Transform)new RotateTransform()
                    {
                        Angle = -90.0
                    };
                    this.OrientedPanel.IsInverted = this.Location != AxisLocation.Right;
                    this.AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    this.AxisGrid.RowDefinitions.Add(new RowDefinition());
                    int num = 0;
                    if (this.AxisTitle != null)
                    {
                        this.AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        Grid.SetRow((FrameworkElement)this.TitleLayoutTransformControl, 0);
                        Grid.SetColumn((FrameworkElement)this.TitleLayoutTransformControl, 0);
                        ++num;
                    }
                    Grid.SetRow((FrameworkElement)this.OrientedPanel, 0);
                    Grid.SetColumn((FrameworkElement)this.OrientedPanel, num);
                    this.AxisGrid.Children.Add((UIElement)this.TitleLayoutTransformControl);
                    this.AxisGrid.Children.Add((UIElement)this.OrientedPanel);
                    if (this.Location == AxisLocation.Right)
                    {
                        this.AxisGrid.Mirror(Controls.Orientation.Vertical);
                        this.TitleLayoutTransformControl.Transform = (Transform)new RotateTransform()
                        {
                            Angle = 90.0
                        };
                    }
                }
            }
            else if (this.Orientation == AxisOrientation.X)
            {
                this.OrientedPanel.Orientation = Controls.Orientation.Horizontal;
                this.OrientedPanel.IsReversed = false;
                if (this.Location == AxisLocation.Top || this.Location == AxisLocation.Bottom)
                {
                    this.OrientedPanel.IsInverted = this.Location == AxisLocation.Top;
                    this.TitleLayoutTransformControl.Transform = (Transform)new RotateTransform()
                    {
                        Angle = 0.0
                    };
                    this.AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    this.AxisGrid.RowDefinitions.Add(new RowDefinition());
                    if (this.AxisTitle != null)
                    {
                        this.AxisGrid.RowDefinitions.Add(new RowDefinition());
                        Grid.SetColumn((FrameworkElement)this.TitleLayoutTransformControl, 0);
                        Grid.SetRow((FrameworkElement)this.TitleLayoutTransformControl, 1);
                    }
                    Grid.SetColumn((FrameworkElement)this.OrientedPanel, 0);
                    Grid.SetRow((FrameworkElement)this.OrientedPanel, 0);
                    this.AxisGrid.Children.Add((UIElement)this.TitleLayoutTransformControl);
                    this.AxisGrid.Children.Add((UIElement)this.OrientedPanel);
                    if (this.Location == AxisLocation.Top)
                        this.AxisGrid.Mirror(Controls.Orientation.Horizontal);
                }
            }
            this.Invalidate();
        }

        /// <summary>Renders the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The required size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.RenderAxis(availableSize);
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Reformulates the grid when the orientation is changed.  Grid is
        /// either separated into two columns or two rows.  The title is
        /// inserted with the outermost section from the edge and an oriented
        /// panel is inserted into the innermost section.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnOrientationPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            this.ArrangeAxisGrid();
            base.OnOrientationPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Updates the visual appearance of the axis when it is invalidated.
        /// </summary>
        /// <param name="args">Information for the invalidated event.</param>
        protected override void OnInvalidated(RoutedEventArgs args)
        {
            this.InvalidateMeasure();
            base.OnInvalidated(args);
        }

        /// <summary>
        /// Renders the axis if there is a valid value for orientation.
        /// </summary>
        /// <param name="availableSize">The available size in which to render
        /// the axis.</param>
        private void RenderAxis(Size availableSize)
        {
            if (this.Orientation == AxisOrientation.None || this.Location == AxisLocation.Auto)
                return;
            this.Render(availableSize);
        }

        /// <summary>
        /// Renders the axis labels, tick marks, and other visual elements.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        protected abstract void Render(Size availableSize);

        /// <summary>Invalidates the axis.</summary>
        protected void Invalidate()
        {
            this.OnInvalidated(new RoutedEventArgs());
        }

        /// <summary>Gets or sets the series host.</summary>
        public ISeriesHost SeriesHost
        {
            get
            {
                return this._seriesHost;
            }
            set
            {
                if (value == this._seriesHost)
                    return;
                ISeriesHost seriesHost = this._seriesHost;
                this._seriesHost = value;
                this.OnSeriesHostPropertyChanged(seriesHost, value);
            }
        }

        /// <summary>
        /// This method is run when the series host property is changed.
        /// </summary>
        /// <param name="oldValue">The old series host.</param>
        /// <param name="newValue">The new series host.</param>
        protected virtual void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            if (oldValue != null && this.GridLines != null)
                oldValue.BackgroundElements.Remove((UIElement)this.GridLines);
            if (newValue == null || this.GridLines == null)
                return;
            newValue.BackgroundElements.Add((UIElement)this.GridLines);
        }
    }
}