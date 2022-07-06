﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#if DEFINITION_SERIES_COMPATIBILITY_MODE
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace System.Windows.Controls.DataVisualization.Charting.Compatible
#endif
{
    /// <summary>
    /// Control that displays values as an line chart visualization.
    /// </summary>
    /// <remarks>
    /// Based on the DefinitionSeries hierarchy.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = DataPointStyleName, StyleTargetType = typeof(LineDataPoint))]
    [StyleTypedProperty(Property = LegendItemStyleName, StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = PolylineStyleName, StyleTargetType = typeof(Polyline))]
    public class LineSeries : StackedLineSeries
    {
        /// <summary>
        /// Name of the DataPointStyle property.
        /// </summary>
        private const string DataPointStyleName = "DataPointStyle";

        /// <summary>
        /// Name of the LegendItemStyle property.
        /// </summary>
        private const string LegendItemStyleName = "LegendItemStyle";

        /// <summary>
        /// Name of the PolylineStyle property.
        /// </summary>
        private const string PolylineStyleName = "PolylineStyle";

        /// <summary>
        /// Field storing the single SeriesDefinition used by the series.
        /// </summary>
        private SeriesDefinition _definition;

        /// <summary>
        /// Initializes a new instance of the LineSeries class.
        /// </summary>
        public LineSeries()
        {
            SetBinding(DefinitionSeries.DependentAxisProperty, new Binding("DependentRangeAxis") { Source = this });
            SetBinding(DefinitionSeries.SelectionModeProperty, new Binding("IsSelectionEnabled") { Source = this, Converter = new System.Windows.Controls.DataVisualization.Charting.Compatible.SelectionEnabledToSelectionModeConverter() });
            _definition = new SeriesDefinition();
            _definition.SetBinding(SeriesDefinition.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
            _definition.SetBinding(SeriesDefinition.TitleProperty, new Binding("Title") { Source = this });
            _definition.SetBinding(SeriesDefinition.DataPointStyleProperty, new Binding(DataPointStyleName) { Source = this });
            _definition.SetBinding(SeriesDefinition.LegendItemStyleProperty, new Binding(LegendItemStyleName) { Source = this });
            _definition.SetBinding(SeriesDefinition.DataShapeStyleProperty, new Binding(PolylineStyleName) { Source = this });
            _definition.SetBinding(SeriesDefinition.TransitionDurationProperty, new Binding("TransitionDuration") { Source = this });
#if !NO_EASING_FUNCTIONS
            _definition.SetBinding(SeriesDefinition.TransitionEasingFunctionProperty, new Binding("TransitionEasingFunction") { Source = this });
#endif
            // For compatibility
            DependentValueBinding = new Binding();
            IndependentValueBinding = new Binding();
            SeriesDefinitions.Add(_definition);
        }

        /// <summary>
        /// Gets a sequence of IndependentValueGroups.
        /// </summary>
        protected override IEnumerable<IndependentValueGroup> IndependentValueGroups
        {
            get
            {
                // Base implementation groups by independent value; when plotting a single series in isolation, that's not desirable
                return DataItems
                    .Select(di => new IndependentValueGroup(di.ActualIndependentValue, new DataItem[] { di }));
            }
        }

        /// <summary>
        /// Gets or sets a sequence that provides the content of the series.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the Binding that identifies the dependent values of the series.
        /// </summary>
        public Binding DependentValueBinding
        {
            get { return _definition.DependentValueBinding; }
            set { _definition.DependentValueBinding = value; }
        }

        /// <summary>
        /// Gets or sets the Binding path that identifies the dependent values of the series.
        /// </summary>
        public string DependentValuePath
        {
            get { return _definition.DependentValuePath; }
            set { _definition.DependentValuePath = value; }
        }

        /// <summary>
        /// Gets or sets the Binding that identifies the independent values of the series.
        /// </summary>
        public Binding IndependentValueBinding
        {
            get { return _definition.IndependentValueBinding; }
            set { _definition.IndependentValueBinding = value; }
        }

        /// <summary>
        /// Gets or sets the Binding path that identifies the independent values of the series.
        /// </summary>
        public string IndependentValuePath
        {
            get { return _definition.IndependentValuePath; }
            set { _definition.IndependentValuePath = value; }
        }

        /// <summary>
        /// Gets or sets the IRangeAxis to use as the dependent axis of the series.
        /// </summary>
        public IRangeAxis DependentRangeAxis
        {
            get { return (IRangeAxis)GetValue(DependentRangeAxisProperty); }
            set { SetValue(DependentRangeAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentRangeAxisProperty =
            DependencyProperty.Register("DependentRangeAxis", typeof(IRangeAxis), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the title of the series.
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the Style to use for the DataPoints of the series.
        /// </summary>
        public Style DataPointStyle
        {
            get { return (Style)GetValue(DataPointStyleProperty); }
            set { SetValue(DataPointStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the DataPointStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DataPointStyleProperty =
            DependencyProperty.Register(DataPointStyleName, typeof(Style), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the Style to use for the LegendItem of the series.
        /// </summary>
        public Style LegendItemStyle
        {
            get { return (Style)GetValue(LegendItemStyleProperty); }
            set { SetValue(LegendItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the LegendItemStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemStyleProperty =
            DependencyProperty.Register(LegendItemStyleName, typeof(Style), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets a value indicating whether selection is enabled.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsSelectionEnabledProperty); }
            set { SetValue(IsSelectionEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSelectionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionEnabledProperty =
            DependencyProperty.Register("IsSelectionEnabled", typeof(bool), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the Style to use for the Path of the series.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Polyline", Scope = "member", Target = "System.Windows.Controls.DataVisualization.Charting.Compatible.LineSeries.#PolylineStyle", Justification = "Matches spelling of same-named framework class.")]
        public Style PolylineStyle
        {
            get { return (Style)GetValue(PolylineStyleProperty); }
            set { SetValue(PolylineStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the PolylineStyle dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Polyline", Justification = "Matches spelling of same-named framework class.")]
        public static readonly DependencyProperty PolylineStyleProperty =
            DependencyProperty.Register(PolylineStyleName, typeof(Style), typeof(LineSeries), null);

        /// <summary>
        /// Gets or sets the TimeSpan to use for the duration of data transitions.
        /// </summary>
        public TimeSpan TransitionDuration
        {
            get { return (TimeSpan)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        /// <summary>
        /// Identifies the TransitionDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register("TransitionDuration", typeof(TimeSpan), typeof(LineSeries), new PropertyMetadata(TimeSpan.FromSeconds(0.5)));

#if !NO_EASING_FUNCTIONS
        /// <summary>
        /// Gets or sets the IEasingFunction to use for data transitions.
        /// </summary>
        public IEasingFunction TransitionEasingFunction
        {
            get { return (IEasingFunction)GetValue(TransitionEasingFunctionProperty); }
            set { SetValue(TransitionEasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the TransitionEasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionEasingFunctionProperty =
            DependencyProperty.Register("TransitionEasingFunction", typeof(IEasingFunction), typeof(LineSeries), new PropertyMetadata(new QuadraticEase { EasingMode = EasingMode.EaseInOut }));
#else
        /// <summary>
        /// Gets or sets a placeholder for the TransitionEasingFunction dependency property.
        /// </summary>
        internal IEasingFunction TransitionEasingFunction { get; set; }
#endif
    }
}
