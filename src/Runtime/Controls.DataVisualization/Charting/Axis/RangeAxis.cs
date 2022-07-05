// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;


#if MIGRATION
using System.Windows.Shapes;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{

    /// <summary>A margin specified for a given value.</summary>
    public struct ValueMargin
    {
        /// <summary>Gets the value that the margin is associated with.</summary>
        public object Value { get; private set; }

        /// <summary>Gets the low margin for a value.</summary>
        public double LowMargin { get; private set; }

        /// <summary>Gets the high margin for a value.</summary>
        public double HighMargin { get; private set; }

        /// <summary>Initializes a new instance of the ValueMargin class.</summary>
        /// <param name="value">The value the margin is associated with.</param>
        /// <param name="lowMargin">The lower margin.</param>
        /// <param name="highMargin">The higher margin.</param>
        public ValueMargin(object value, double lowMargin, double highMargin)
        {
            this = new ValueMargin();
            this.Value = value;
            this.LowMargin = lowMargin;
            this.HighMargin = highMargin;
        }

        /// <summary>Determines whether two value margins are equal.</summary>
        /// <param name="obj">The value margin to compare with this one.</param>
        /// <returns>A value indicating whether the two value margins are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ValueMargin))
                return false;
            ValueMargin valueMargin = (ValueMargin)obj;
            return this.Value.Equals(valueMargin.Value) && this.LowMargin.Equals(valueMargin.LowMargin) && this.HighMargin.Equals(valueMargin.HighMargin);
        }

        /// <summary>Determines whether two unit value objects are equal.</summary>
        /// <param name="left">The left value margin.</param>
        /// <param name="right">The right value margin.</param>
        /// <returns>A value indicating  whether two value margins objects are
        /// equal.</returns>
        public static bool operator ==(ValueMargin left, ValueMargin right)
        {
            return left.Equals((object)right);
        }

        /// <summary>
        /// Determines whether two value margin objects are not equal.
        /// </summary>
        /// <param name="left">The left value margin.</param>
        /// <param name="right">The right value margin.</param>
        /// <returns>A value indicating whether two value margin objects are not
        /// equal.</returns>
        public static bool operator !=(ValueMargin left, ValueMargin right)
        {
            return !left.Equals((object)right);
        }

        /// <summary>Returns the hash code of the value margin object.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.LowMargin.GetHashCode() ^ this.HighMargin.GetHashCode();
        }
    }

    /// <summary>
    /// Provides information about margins necessary for values.
    /// </summary>
    public interface IValueMarginProvider
    {
        /// <summary>Gets the margins required for values.</summary>
        /// <param name="consumer">The axis to retrieve the value margins
        /// for.</param>
        /// <returns>The margins required for values.</returns>
        IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer);
    }

    /// <summary>
    /// Consumes value margins and uses them to lay out objects.
    /// </summary>
    public interface IValueMarginConsumer
    {
        /// <summary>Updates layout to accommodate for value margins.</summary>
        /// <param name="provider">A value margin provider.</param>
        /// <param name="valueMargins">A sequence of value margins.</param>
        void ValueMarginsChanged(IValueMarginProvider provider, IEnumerable<ValueMargin> valueMargins);
    }

    /// <summary>An axis that has a range.</summary>
    public abstract class RangeAxis : DisplayAxis, IRangeAxis, IAxis, IRangeConsumer, IValueMarginConsumer
    {
        /// <summary>
        /// Identifies the MinorTickMarkStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorTickMarkStyleProperty = DependencyProperty.Register(nameof(MinorTickMarkStyle), typeof(Style), typeof(RangeAxis), new PropertyMetadata((PropertyChangedCallback)null));
        /// <summary>A pool of major tick marks.</summary>
        private ObjectPool<Line> _majorTickMarkPool;
        /// <summary>A pool of major tick marks.</summary>
        private ObjectPool<Line> _minorTickMarkPool;
        /// <summary>A pool of labels.</summary>
        private ObjectPool<Control> _labelPool;
        /// <summary>The actual range of values.</summary>
        private Range<IComparable> _actualRange;
        /// <summary>The maximum value displayed in the range axis.</summary>
        private IComparable _protectedMaximum;
        /// <summary>The minimum value displayed in the range axis.</summary>
        private IComparable _protectedMinimum;

        /// <summary>Gets or sets the minor tick mark style.</summary>
        public Style MinorTickMarkStyle
        {
            get
            {
                return this.GetValue(RangeAxis.MinorTickMarkStyleProperty) as Style;
            }
            set
            {
                this.SetValue(RangeAxis.MinorTickMarkStyleProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the actual range of values.</summary>
        protected Range<IComparable> ActualRange
        {
            get
            {
                return this._actualRange;
            }
            set
            {
                Range<IComparable> actualRange = this._actualRange;
                Range<IComparable> range = this.EnforceMaximumAndMinimum(value);
                if (actualRange.Equals(range))
                    return;
                this._actualRange = range;
                this.OnActualRangeChanged(range);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value displayed in the range axis.
        /// </summary>
        protected IComparable ProtectedMaximum
        {
            get
            {
                return this._protectedMaximum;
            }
            set
            {
                if (value != null && this.ProtectedMinimum != null && ValueHelper.Compare(this.ProtectedMinimum, value) > 0)
                    throw new InvalidOperationException("RangeAxis: Maximum Value Must Be Larger Than Or Equal To Minimum Value");
                if (object.ReferenceEquals((object)this._protectedMaximum, (object)value) || object.Equals((object)this._protectedMaximum, (object)value))
                    return;
                this._protectedMaximum = value;
                this.UpdateActualRange();
            }
        }

        /// <summary>
        /// Gets or sets the minimum value displayed in the range axis.
        /// </summary>
        protected IComparable ProtectedMinimum
        {
            get
            {
                return this._protectedMinimum;
            }
            set
            {
                if (value != null && this.ProtectedMaximum != null && ValueHelper.Compare(value, this.ProtectedMaximum) > 0)
                    throw new InvalidOperationException("RangeAxis: Minimum Value Must Be Larger Than Or Equal To Minimum Value");
                if (object.ReferenceEquals((object)this._protectedMinimum, (object)value) || object.Equals((object)this._protectedMinimum, (object)value))
                    return;
                this._protectedMinimum = value;
                this.UpdateActualRange();
            }
        }

        /// <summary>Instantiates a new instance of the RangeAxis class.</summary>
        protected RangeAxis()
        {
            this.DefaultStyleKey = (object)typeof(RangeAxis);
            this._labelPool = new ObjectPool<Control>((Func<Control>)(() => this.CreateAxisLabel()));
            this._majorTickMarkPool = new ObjectPool<Line>((Func<Line>)(() => this.CreateMajorTickMark()));
            this._minorTickMarkPool = new ObjectPool<Line>((Func<Line>)(() => this.CreateMinorTickMark()));
            SizeChangedEventHandler handler = (SizeChangedEventHandler)null;
            handler = (SizeChangedEventHandler)delegate
            {
                this.SizeChanged -= handler;
                this.UpdateActualRange();
            };
            this.SizeChanged += handler;
        }

        /// <summary>Creates a minor axis tick mark.</summary>
        /// <returns>A line to used to render a tick mark.</returns>
        protected Line CreateMinorTickMark()
        {
            return this.CreateTickMark(this.MinorTickMarkStyle);
        }

        /// <summary>Invalidates axis when the actual range changes.</summary>
        /// <param name="range">The new actual range.</param>
        protected virtual void OnActualRangeChanged(Range<IComparable> range)
        {
            this.Invalidate();
        }

        /// <summary>Returns the plot area coordinate of a given value.</summary>
        /// <param name="value">The value to return the plot area coordinate for.</param>
        /// <returns>The plot area coordinate of the given value.</returns>
        public override UnitValue GetPlotAreaCoordinate(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return this.GetPlotAreaCoordinate(value, this.ActualLength);
        }

        /// <summary>Returns the plot area coordinate of a given value.</summary>
        /// <param name="value">The value to return the plot area coordinate for.</param>
        /// <param name="length">The length of the axis.</param>
        /// <returns>The plot area coordinate of the given value.</returns>
        protected abstract UnitValue GetPlotAreaCoordinate(object value, double length);

        /// <summary>Returns the plot area coordinate of a given value.</summary>
        /// <param name="value">The value to return the plot area coordinate for.</param>
        /// <param name="currentRange">The value range to use when calculating the plot area coordinate.</param>
        /// <param name="length">The length of the axis.</param>
        /// <returns>The plot area coordinate of the given value.</returns>
        protected abstract UnitValue GetPlotAreaCoordinate(object value, Range<IComparable> currentRange, double length);

        /// <summary>Overrides the data range.</summary>
        /// <param name="range">The range to potentially override.</param>
        /// <returns>The overridden range.</returns>
        protected virtual Range<IComparable> OverrideDataRange(Range<IComparable> range)
        {
            return range;
        }

        /// <summary>
        /// Modifies a range to respect the minimum and maximum axis values.
        /// </summary>
        /// <param name="range">The range of data.</param>
        /// <returns>A range modified to  respect the minimum and maximum axis
        /// values.</returns>
        private Range<IComparable> EnforceMaximumAndMinimum(Range<IComparable> range)
        {
            if (range.HasData)
            {
                IComparable comparable1 = this.ProtectedMinimum ?? range.Minimum;
                IComparable comparable2 = this.ProtectedMaximum ?? range.Maximum;
                if (ValueHelper.Compare(comparable1, comparable2) > 0)
                {
                    IComparable comparable3 = comparable2;
                    comparable2 = comparable1;
                    comparable1 = comparable3;
                }
                return new Range<IComparable>(comparable1, comparable2);
            }
            IComparable minimum = this.ProtectedMinimum;
            IComparable maximum = this.ProtectedMaximum;
            if (this.ProtectedMinimum != null && this.ProtectedMaximum == null)
            {
                maximum = minimum;
            }
            else
            {
                if (this.ProtectedMaximum == null || this.ProtectedMinimum != null)
                    return range;
                minimum = maximum;
            }
            return new Range<IComparable>(minimum, maximum);
        }

        /// <summary>Updates the actual range displayed on the axis.</summary>
        private void UpdateActualRange()
        {
            Action a = (Action)(() =>
            {
                if (this.ProtectedMaximum == null || this.ProtectedMinimum == null)
                {
                    if (this.Orientation == AxisOrientation.None)
                    {
                        if (this.ProtectedMinimum != null)
                            this.ActualRange = this.OverrideDataRange(new Range<IComparable>(this.ProtectedMinimum, this.ProtectedMinimum));
                        else
                            this.ActualRange = this.OverrideDataRange(new Range<IComparable>(this.ProtectedMaximum, this.ProtectedMaximum));
                    }
                    else
                        this.ActualRange = this.OverrideDataRange(this.RegisteredListeners.OfType<IRangeProvider>().Select<IRangeProvider, Range<IComparable>>((Func<IRangeProvider, Range<IComparable>>)(rangeProvider => rangeProvider.GetRange((IRangeConsumer)this))).Sum<IComparable>());
                }
                else
                    this.ActualRange = new Range<IComparable>(this.ProtectedMinimum, this.ProtectedMaximum);
            });
            if (this.ActualLength == 0.0)
                this.Dispatcher.BeginInvoke(a);
            a();
        }

        /// <summary>Renders the axis as an oriented axis.</summary>
        /// <param name="availableSize">The available size.</param>
        private void RenderOriented(Size availableSize)
        {
            this._minorTickMarkPool.Reset();
            this._majorTickMarkPool.Reset();
            this._labelPool.Reset();
            double length = this.GetLength(availableSize);
            try
            {
                this.OrientedPanel.Children.Clear();
                int num1;
                if (this.ActualRange.HasData)
                {
                    Range<IComparable> actualRange = this.ActualRange;
                    IComparable minimum = actualRange.Minimum;
                    actualRange = this.ActualRange;
                    IComparable maximum = actualRange.Maximum;
                    num1 = object.Equals((object)minimum, (object)maximum) ? 1 : 0;
                }
                else
                    num1 = 1;
                if (num1 != 0)
                    return;
                foreach (object majorTickMarkValue in this.GetMajorTickMarkValues(availableSize))
                {
                    UnitValue plotAreaCoordinate = this.GetPlotAreaCoordinate(majorTickMarkValue, length);
                    if (ValueHelper.CanGraph(plotAreaCoordinate.Value))
                    {
                        Line line = this._majorTickMarkPool.Next();
                        OrientedPanel.SetCenterCoordinate((UIElement)line, plotAreaCoordinate.Value);
                        OrientedPanel.SetPriority((UIElement)line, 0);
                        this.OrientedPanel.Children.Add((UIElement)line);
                    }
                }
                foreach (object minorTickMarkValue in this.GetMinorTickMarkValues(availableSize))
                {
                    UnitValue plotAreaCoordinate = this.GetPlotAreaCoordinate(minorTickMarkValue, length);
                    if (ValueHelper.CanGraph(plotAreaCoordinate.Value))
                    {
                        Line line = this._minorTickMarkPool.Next();
                        OrientedPanel.SetCenterCoordinate((UIElement)line, plotAreaCoordinate.Value);
                        OrientedPanel.SetPriority((UIElement)line, 0);
                        this.OrientedPanel.Children.Add((UIElement)line);
                    }
                }
                int num2 = 0;
                foreach (IComparable labelValue in this.GetLabelValues(availableSize))
                {
                    UnitValue plotAreaCoordinate = this.GetPlotAreaCoordinate((object)labelValue, length);
                    if (ValueHelper.CanGraph(plotAreaCoordinate.Value))
                    {
                        Control label = this._labelPool.Next();
                        this.PrepareAxisLabel(label, (object)labelValue);
                        OrientedPanel.SetCenterCoordinate((UIElement)label, plotAreaCoordinate.Value);
                        OrientedPanel.SetPriority((UIElement)label, num2 + 1);
                        this.OrientedPanel.Children.Add((UIElement)label);
                        num2 = (num2 + 1) % 2;
                    }
                }
            }
            finally
            {
                this._minorTickMarkPool.Done();
                this._majorTickMarkPool.Done();
                this._labelPool.Done();
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
        /// Returns a sequence of the major grid line coordinates.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of the major grid line coordinates.</returns>
        protected override IEnumerable<UnitValue> GetMajorGridLineCoordinates(Size availableSize)
        {
            return this.GetMajorTickMarkValues(availableSize).Select<IComparable, UnitValue>((Func<IComparable, UnitValue>)(value => this.GetPlotAreaCoordinate((object)value))).Where<UnitValue>((Func<UnitValue, bool>)(value => ValueHelper.CanGraph(value.Value)));
        }

        /// <summary>
        /// Returns a sequence of the values at which to plot major grid lines.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of the values at which to plot major grid lines.</returns>
        protected virtual IEnumerable<IComparable> GetMajorGridLineValues(Size availableSize)
        {
            return this.GetMajorTickMarkValues(availableSize);
        }

        /// <summary>Returns a sequence of values to plot on the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        protected abstract IEnumerable<IComparable> GetMajorTickMarkValues(Size availableSize);

        /// <summary>Returns a sequence of values to plot on the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        protected virtual IEnumerable<IComparable> GetMinorTickMarkValues(Size availableSize)
        {
            yield break;
        }

        /// <summary>Returns a sequence of values to plot on the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        protected abstract IEnumerable<IComparable> GetLabelValues(Size availableSize);

        /// <summary>Returns the value range given a plot area coordinate.</summary>
        /// <param name="value">The plot area coordinate.</param>
        /// <returns>A range of values at that plot area coordinate.</returns>
        protected abstract IComparable GetValueAtPosition(UnitValue value);

        Range<IComparable> IRangeAxis.Range
        {
            get
            {
                return this.ActualRange;
            }
        }

        IComparable IRangeAxis.GetValueAtPosition(UnitValue value)
        {
            return this.GetValueAtPosition(value);
        }

        void IRangeConsumer.RangeChanged(IRangeProvider usesRangeAxis, Range<IComparable> range)
        {
            this.UpdateActualRange();
        }

        void IValueMarginConsumer.ValueMarginsChanged(IValueMarginProvider provider, IEnumerable<ValueMargin> valueMargins)
        {
            Action a = (Action)(() =>
            {
                if (this.Orientation == AxisOrientation.None || !valueMargins.Select<ValueMargin, Range<double>>((Func<ValueMargin, Range<double>>)(valueMargin =>
                {
                    double num = this.GetPlotAreaCoordinate(valueMargin.Value).Value;
                    return new Range<double>(num - valueMargin.LowMargin, num + valueMargin.HighMargin);
                })).Where<Range<double>>((Func<Range<double>, bool>)(range => range.Minimum < 0.0 || range.Maximum > this.ActualLength)).Any<Range<double>>())
                    return;
                this.UpdateActualRange();
            });
            if (this.ActualLength == 0.0)
                this.Dispatcher.BeginInvoke(a);
            else
                a();
        }

        /// <summary>
        /// If a new range provider is registered, update actual range.
        /// </summary>
        /// <param name="series">The axis listener being registered.</param>
        protected override void OnObjectRegistered(IAxisListener series)
        {
            base.OnObjectRegistered(series);
            if (!(series is IRangeProvider) && !(series is IValueMarginProvider))
                return;
            this.UpdateActualRange();
        }

        /// <summary>
        /// If a range provider is unregistered, update actual range.
        /// </summary>
        /// <param name="series">The axis listener being unregistered.</param>
        protected override void OnObjectUnregistered(IAxisListener series)
        {
            base.OnObjectUnregistered(series);
            if (!(series is IRangeProvider) && !(series is IValueMarginProvider))
                return;
            this.UpdateActualRange();
        }

        /// <summary>
        /// Create function that when given a range will return the
        /// amount in pixels by which the value margin range
        /// overlaps.  Positive numbers represent values outside the
        /// range.
        /// </summary>
        /// <param name="valueMargins">The list of value margins, coordinates, and overlaps.</param>
        /// <param name="comparableRange">The new range to use to calculate coordinates.</param>
        internal void UpdateValueMargins(IList<ValueMarginCoordinateAndOverlap> valueMargins, Range<IComparable> comparableRange)
        {
            double actualLength = this.ActualLength;
            int count = valueMargins.Count;
            for (int index = 0; index < count; ++index)
            {
                ValueMarginCoordinateAndOverlap valueMargin1 = valueMargins[index];
                ValueMarginCoordinateAndOverlap coordinateAndOverlap1 = valueMargin1;
                ValueMargin valueMargin2 = valueMargin1.ValueMargin;
                double num1 = this.GetPlotAreaCoordinate(valueMargin2.Value, comparableRange, actualLength).Value;
                coordinateAndOverlap1.Coordinate = num1;
                ValueMarginCoordinateAndOverlap coordinateAndOverlap2 = valueMargin1;
                double coordinate1 = valueMargin1.Coordinate;
                valueMargin2 = valueMargin1.ValueMargin;
                double lowMargin = valueMargin2.LowMargin;
                double num2 = -(coordinate1 - lowMargin);
                coordinateAndOverlap2.LeftOverlap = num2;
                ValueMarginCoordinateAndOverlap coordinateAndOverlap3 = valueMargin1;
                double coordinate2 = valueMargin1.Coordinate;
                valueMargin2 = valueMargin1.ValueMargin;
                double highMargin = valueMargin2.HighMargin;
                double num3 = coordinate2 + highMargin - actualLength;
                coordinateAndOverlap3.RightOverlap = num3;
            }
        }

        /// <summary>
        /// Returns the value margin, coordinate, and overlap triples that have the largest left and right overlap.
        /// </summary>
        /// <param name="valueMargins">The list of value margin, coordinate, and
        /// overlap triples.</param>
        /// <param name="maxLeftOverlapValueMargin">The value margin,
        /// coordinate, and overlap triple that has the largest left overlap.
        /// </param>
        /// <param name="maxRightOverlapValueMargin">The value margin,
        /// coordinate, and overlap triple that has the largest right overlap.
        /// </param>
        internal static void GetMaxLeftAndRightOverlap(IList<ValueMarginCoordinateAndOverlap> valueMargins, out ValueMarginCoordinateAndOverlap maxLeftOverlapValueMargin, out ValueMarginCoordinateAndOverlap maxRightOverlapValueMargin)
        {
            maxLeftOverlapValueMargin = new ValueMarginCoordinateAndOverlap();
            maxRightOverlapValueMargin = new ValueMarginCoordinateAndOverlap();
            double num1 = double.MinValue;
            double num2 = double.MinValue;
            int count = valueMargins.Count;
            for (int index = 0; index < count; ++index)
            {
                ValueMarginCoordinateAndOverlap valueMargin = valueMargins[index];
                double leftOverlap = valueMargin.LeftOverlap;
                if (leftOverlap > num1)
                {
                    num1 = leftOverlap;
                    maxLeftOverlapValueMargin = valueMargin;
                }
                double rightOverlap = valueMargin.RightOverlap;
                if (rightOverlap > num2)
                {
                    num2 = rightOverlap;
                    maxRightOverlapValueMargin = valueMargin;
                }
            }
        }

        IComparable IRangeAxis.Origin
        {
            get
            {
                return this.Origin;
            }
        }

        /// <summary>Gets the origin value on the axis.</summary>
        protected abstract IComparable Origin { get; }
    }
}