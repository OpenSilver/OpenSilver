using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// A panel that plots elements on a one dimensional plane.  In order to
    /// minimize collisions it moves elements further and further from the edge
    /// of the plane based on their priority.  Elements that have the same
    /// priority level are always the same distance from the edge.
    /// </summary>
    internal class OrientedPanel : Panel
    {
        /// <summary>
        /// Identifies the ActualMinimumDistanceBetweenChildren dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualMinimumDistanceBetweenChildrenProperty = DependencyProperty.Register(nameof(ActualMinimumDistanceBetweenChildren), typeof(double), typeof(OrientedPanel), new PropertyMetadata((object)0.0));
        /// <summary>
        /// Identifies the MinimumDistanceBetweenChildren dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumDistanceBetweenChildrenProperty = DependencyProperty.Register(nameof(MinimumDistanceBetweenChildren), typeof(double), typeof(OrientedPanel), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OrientedPanel.OnMinimumDistanceBetweenChildrenPropertyChanged)));
        /// <summary>Identifies the ActualLength dependency property.</summary>
        public static readonly DependencyProperty ActualLengthProperty = DependencyProperty.Register(nameof(ActualLength), typeof(double), typeof(OrientedPanel), new PropertyMetadata((object)0.0));
        /// <summary>Identifies the CenterCoordinate dependency property.</summary>
        public static readonly DependencyProperty CenterCoordinateProperty = DependencyProperty.RegisterAttached("CenterCoordinate", typeof(double), typeof(OrientedPanel), new PropertyMetadata(new PropertyChangedCallback(OrientedPanel.OnCenterCoordinatePropertyChanged)));
        /// <summary>Identifies the OffsetPadding dependency property.</summary>
        public static readonly DependencyProperty OffsetPaddingProperty = DependencyProperty.Register(nameof(OffsetPadding), typeof(double), typeof(OrientedPanel), new PropertyMetadata((object)0.0, new PropertyChangedCallback(OrientedPanel.OnOffsetPaddingPropertyChanged)));
        /// <summary>Identifies the Priority dependency property.</summary>
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.RegisterAttached("Priority", typeof(int), typeof(OrientedPanel), new PropertyMetadata(new PropertyChangedCallback(OrientedPanel.OnPriorityPropertyChanged)));
        /// <summary>Identifies the IsInverted dependency property.</summary>
        public static readonly DependencyProperty IsInvertedProperty = DependencyProperty.Register(nameof(IsInverted), typeof(bool), typeof(OrientedPanel), new PropertyMetadata((object)false, new PropertyChangedCallback(OrientedPanel.OnIsInvertedPropertyChanged)));
        /// <summary>Identifies the IsReversed dependency property.</summary>
        public static readonly DependencyProperty IsReversedProperty = DependencyProperty.Register(nameof(IsReversed), typeof(bool), typeof(OrientedPanel), new PropertyMetadata((object)false, new PropertyChangedCallback(OrientedPanel.OnIsReversedPropertyChanged)));
        /// <summary>Identifies the Orientation dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(OrientedPanel), new PropertyMetadata((object)Orientation.Horizontal, new PropertyChangedCallback(OrientedPanel.OnOrientationPropertyChanged)));

        /// <summary>Gets the actual minimum distance between children.</summary>
        public double ActualMinimumDistanceBetweenChildren
        {
            get
            {
                return (double)this.GetValue(OrientedPanel.ActualMinimumDistanceBetweenChildrenProperty);
            }
            private set
            {
                this.SetValue(OrientedPanel.ActualMinimumDistanceBetweenChildrenProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the minimum distance between children.</summary>
        public double MinimumDistanceBetweenChildren
        {
            get
            {
                return (double)this.GetValue(OrientedPanel.MinimumDistanceBetweenChildrenProperty);
            }
            set
            {
                this.SetValue(OrientedPanel.MinimumDistanceBetweenChildrenProperty, (object)value);
            }
        }

        /// <summary>
        /// MinimumDistanceBetweenChildrenProperty property changed handler.
        /// </summary>
        /// <param name="d">OrientedPanel that changed its MinimumDistanceBetweenChildren.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumDistanceBetweenChildrenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OrientedPanel)d).OnMinimumDistanceBetweenChildrenPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// MinimumDistanceBetweenChildrenProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnMinimumDistanceBetweenChildrenPropertyChanged(double oldValue, double newValue)
        {
            this.InvalidateMeasure();
        }

        /// <summary>Gets the actual length of the panel.</summary>
        public double ActualLength
        {
            get
            {
                return (double)this.GetValue(OrientedPanel.ActualLengthProperty);
            }
        }

        /// <summary>
        /// Gets the value of the CenterCoordinate attached property for a specified UIElement.
        /// </summary>
        /// <param name="element">The UIElement from which the property value is read.</param>
        /// <returns>The CenterCoordinate property value for the UIElement.</returns>
        public static double GetCenterCoordinate(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (double)element.GetValue(OrientedPanel.CenterCoordinateProperty);
        }

        /// <summary>
        /// Sets the value of the CenterCoordinate attached property to a specified UIElement.
        /// </summary>
        /// <param name="element">The UIElement to which the attached property is written.</param>
        /// <param name="value">The needed CenterCoordinate value.</param>
        public static void SetCenterCoordinate(UIElement element, double value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(OrientedPanel.CenterCoordinateProperty, (object)value);
        }

        /// <summary>CenterCoordinateProperty property changed handler.</summary>
        /// <param name="dependencyObject">UIElement that changed its CenterCoordinate.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public static void OnCenterCoordinatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (uiElement == null)
                throw new ArgumentNullException(nameof(dependencyObject));
            OrientedPanel parent = VisualTreeHelper.GetParent((DependencyObject)uiElement) as OrientedPanel;
            if (parent == null)
                return;
            parent.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets the amount of offset padding to add between items.
        /// </summary>
        public double OffsetPadding
        {
            get
            {
                return (double)this.GetValue(OrientedPanel.OffsetPaddingProperty);
            }
            set
            {
                this.SetValue(OrientedPanel.OffsetPaddingProperty, (object)value);
            }
        }

        /// <summary>OffsetPaddingProperty property changed handler.</summary>
        /// <param name="d">OrientedPanel that changed its OffsetPadding.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOffsetPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OrientedPanel)d).OnOffsetPaddingPropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>OffsetPaddingProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnOffsetPaddingPropertyChanged(double oldValue, double newValue)
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the value of the Priority attached property for a specified UIElement.
        /// </summary>
        /// <param name="element">The UIElement from which the property value is read.</param>
        /// <returns>The Priority property value for the UIElement.</returns>
        public static int GetPriority(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (int)element.GetValue(OrientedPanel.PriorityProperty);
        }

        /// <summary>
        /// Sets the value of the Priority attached property to a specified UIElement.
        /// </summary>
        /// <param name="element">The UIElement to which the attached property is written.</param>
        /// <param name="value">The needed Priority value.</param>
        public static void SetPriority(UIElement element, int value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(OrientedPanel.PriorityProperty, (object)value);
        }

        /// <summary>PriorityProperty property changed handler.</summary>
        /// <param name="dependencyObject">UIElement that changed its Priority.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public static void OnPriorityPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (uiElement == null)
                throw new ArgumentNullException(nameof(dependencyObject));
            OrientedPanel parent = VisualTreeHelper.GetParent((DependencyObject)uiElement) as OrientedPanel;
            if (parent == null)
                return;
            parent.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the panel is inverted.
        /// </summary>
        public bool IsInverted
        {
            get
            {
                return (bool)this.GetValue(OrientedPanel.IsInvertedProperty);
            }
            set
            {
                this.SetValue(OrientedPanel.IsInvertedProperty, (object)value);
            }
        }

        /// <summary>IsInvertedProperty property changed handler.</summary>
        /// <param name="d">OrientedPanel that changed its IsInverted.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsInvertedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OrientedPanel)d).OnIsInvertedPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>IsInvertedProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsInvertedPropertyChanged(bool oldValue, bool newValue)
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the direction is reversed.
        /// </summary>
        public bool IsReversed
        {
            get
            {
                return (bool)this.GetValue(OrientedPanel.IsReversedProperty);
            }
            set
            {
                this.SetValue(OrientedPanel.IsReversedProperty, (object)value);
            }
        }

        /// <summary>IsReversedProperty property changed handler.</summary>
        /// <param name="d">OrientedPanel that changed its IsReversed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsReversedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OrientedPanel)d).OnIsReversedPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>IsReversedProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsReversedPropertyChanged(bool oldValue, bool newValue)
        {
            this.InvalidateMeasure();
        }

        /// <summary>Gets or sets the orientation of the panel.</summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientedPanel.OrientationProperty);
            }
            set
            {
                this.SetValue(OrientedPanel.OrientationProperty, (object)value);
            }
        }

        /// <summary>OrientationProperty property changed handler.</summary>
        /// <param name="d">OrientedPanel that changed its Orientation.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OrientedPanel)d).OnOrientationPropertyChanged((Orientation)e.NewValue);
        }

        /// <summary>OrientationProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        protected virtual void OnOrientationPropertyChanged(Orientation newValue)
        {
            this.UpdateActualLength();
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets the offset of the edge to use for each priority group.
        /// </summary>
        private IDictionary<int, double> PriorityOffsets { get; set; }

        /// <summary>
        /// Instantiates a new instance of the OrientedPanel class.
        /// </summary>
        public OrientedPanel()
        {
            this.UpdateActualLength();
        }

        /// <summary>Updates the actual length property.</summary>
        private void UpdateActualLength()
        {
            this.SetBinding(OrientedPanel.ActualLengthProperty, new Binding(this.Orientation == Orientation.Horizontal ? "ActualWidth" : "ActualHeight")
            {
                Source = (object)this
            });
        }

        /// <summary>
        /// Returns a sequence of ranges for a given sequence of children and a
        /// length selector.
        /// </summary>
        /// <param name="children">A sequence of children.</param>
        /// <param name="lengthSelector">A function that returns a length given
        /// a UIElement.</param>
        /// <returns>A sequence of ranges.</returns>
        private static IEnumerable<Range<double>> GetRanges(IEnumerable<UIElement> children, Func<UIElement, double> lengthSelector)
        {
            return children.Select<UIElement, Range<double>>((Func<UIElement, Range<double>>)(child =>
            {
                double centerCoordinate = OrientedPanel.GetCenterCoordinate(child);
                double num = lengthSelector(child) / 2.0;
                return new Range<double>(centerCoordinate - num, centerCoordinate + num);
            }));
        }

        /// <summary>Measures children and determines necessary size.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The necessary size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double num1 = 0.0;
            if (this.Children.Count > 0)
            {
                Size availableSize1 = new Size(double.PositiveInfinity, double.PositiveInfinity);
                foreach (UIElement child in (PresentationFrameworkCollection<UIElement>)this.Children)
                    child.Measure(availableSize1);
                Func<UIElement, double> lengthSelector = (Func<UIElement, double>)null;
                Func<UIElement, double> offsetSelector = (Func<UIElement, double>)null;
                if (this.Orientation == Orientation.Horizontal)
                {
                    lengthSelector = (Func<UIElement, double>)(child => OrientedPanel.GetCorrectedDesiredSize(child).Width);
                    offsetSelector = (Func<UIElement, double>)(child => OrientedPanel.GetCorrectedDesiredSize(child).Height);
                }
                else
                {
                    lengthSelector = (Func<UIElement, double>)(child => OrientedPanel.GetCorrectedDesiredSize(child).Height);
                    offsetSelector = (Func<UIElement, double>)(child => OrientedPanel.GetCorrectedDesiredSize(child).Width);
                }
                this.ActualMinimumDistanceBetweenChildren = this.Children.CastWrapper<UIElement>().GroupBy<UIElement, int>((Func<UIElement, int>)(child => OrientedPanel.GetPriority(child))).Select<IGrouping<int, UIElement>, IGrouping<int, UIElement>>((Func<IGrouping<int, UIElement>, IGrouping<int, UIElement>>)(priorityGroup => priorityGroup)).Select(priorityGroup =>
                {
                    var data = new
                    {
                        priorityGroup = priorityGroup,
                        orderedElements = priorityGroup.OrderBy<UIElement, double>((Func<UIElement, double>)(element => OrientedPanel.GetCenterCoordinate(element))).ToList<UIElement>()
                    };
                    return data;
                }).Where(_param0 => _param0.orderedElements.Count >= 2).Select(_param1 => EnumerableFunctions.Zip<UIElement, UIElement, double>((IEnumerable<UIElement>)_param1.orderedElements, _param1.orderedElements.Skip<UIElement>(1), (Func<UIElement, UIElement, double>)((leftElement, rightElement) =>
                {
                    double num2 = lengthSelector(leftElement) / 2.0;
                    double centerCoordinate = OrientedPanel.GetCenterCoordinate(leftElement);
                    double num3 = lengthSelector(rightElement) / 2.0;
                    return OrientedPanel.GetCenterCoordinate(rightElement) - num3 - (centerCoordinate + num2);
                })).Min()).MinOrNullable<double>() ?? this.MinimumDistanceBetweenChildren;
                IEnumerable<int> list1 = (IEnumerable<int>)this.Children.CastWrapper<UIElement>().Select<UIElement, int>((Func<UIElement, int>)(child => OrientedPanel.GetPriority(child))).Distinct<int>().OrderBy<int, int>((Func<int, int>)(priority => priority)).ToList<int>();
                this.PriorityOffsets = (IDictionary<int, double>)new Dictionary<int, double>();
                foreach (int index in list1)
                    this.PriorityOffsets[index] = 0.0;
                using (IEnumerator<Tuple<int, int>> enumerator = EnumerableFunctions.Zip<int, int, Tuple<int, int>>(list1, list1.Skip<int>(1), (Func<int, int, Tuple<int, int>>)((previous, next) => new Tuple<int, int>(previous, next))).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Tuple<int, int> priorityPair = enumerator.Current;
                        IEnumerable<UIElement> list2 = (IEnumerable<UIElement>)this.Children.CastWrapper<UIElement>().Where<UIElement>((Func<UIElement, bool>)(child => OrientedPanel.GetPriority(child) == priorityPair.Item1)).ToList<UIElement>();
                        IEnumerable<Range<double>> ranges = OrientedPanel.GetRanges(list2, lengthSelector);
                        IEnumerable<Range<double>> nextPriorityRanges = OrientedPanel.GetRanges((IEnumerable<UIElement>)this.Children.CastWrapper<UIElement>().Where<UIElement>((Func<UIElement, bool>)(child => OrientedPanel.GetPriority(child) == priorityPair.Item2)).ToList<UIElement>(), lengthSelector);
                        if (ranges.SelectMany<Range<double>, Range<double>, bool>((Func<Range<double>, IEnumerable<Range<double>>>)(currentPriorityRange => nextPriorityRanges), (Func<Range<double>, Range<double>, bool>)((currentPriorityRange, nextPriorityRange) => currentPriorityRange.IntersectsWith(nextPriorityRange))).Any<bool>((Func<bool, bool>)(value => value)))
                        {
                            double num2 = list2.Select<UIElement, double>((Func<UIElement, double>)(child => offsetSelector(child))).MaxOrNullable<double>() ?? 0.0;
                            num1 += num2 + this.OffsetPadding;
                        }
                        this.PriorityOffsets[priorityPair.Item2] = num1;
                    }
                }
                num1 = this.Children.CastWrapper<UIElement>().GroupBy<UIElement, int>((Func<UIElement, int>)(child => OrientedPanel.GetPriority(child))).Select<IGrouping<int, UIElement>, double?>((Func<IGrouping<int, UIElement>, double?>)(group => group.Select<UIElement, double>((Func<UIElement, double>)(child => this.PriorityOffsets[group.Key] + offsetSelector(child))).MaxOrNullable<double>())).Where<double?>((Func<double?, bool>)(num => num.HasValue)).Select<double?, double>((Func<double?, double>)(num => num.Value)).MaxOrNullable<double>() ?? 0.0;
            }
            if (this.Orientation == Orientation.Horizontal)
                return new Size(0.0, num1);
            return new Size(num1, 0.0);
        }

        /// <summary>Arranges items according to position and priority.</summary>
        /// <param name="finalSize">The final size of the panel.</param>
        /// <returns>The final size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in (PresentationFrameworkCollection<UIElement>)this.Children)
            {
                double centerCoordinate = OrientedPanel.GetCenterCoordinate(child);
                double priorityOffset = this.PriorityOffsets[OrientedPanel.GetPriority(child)];
                double num1 = 0.0;
                double num2 = 0.0;
                double num3 = 0.0;
                double num4 = 0.0;
                Size correctedDesiredSize = OrientedPanel.GetCorrectedDesiredSize(child);
                if (this.Orientation == Orientation.Horizontal)
                {
                    num1 = finalSize.Width;
                    num3 = correctedDesiredSize.Width;
                    num4 = correctedDesiredSize.Height;
                    num2 = finalSize.Height;
                }
                else if (this.Orientation == Orientation.Vertical)
                {
                    num1 = finalSize.Height;
                    num3 = correctedDesiredSize.Height;
                    num4 = correctedDesiredSize.Width;
                    num2 = finalSize.Width;
                }
                double num5 = num3 / 2.0;
                double a1 = this.IsReversed ? num1 - Math.Round(centerCoordinate + num5) : centerCoordinate - num5;
                double a2 = this.IsInverted ? num2 - Math.Round(priorityOffset + num4) : priorityOffset;
                double num6 = Math.Min(Math.Round(a1), num1 - 1.0);
                double num7 = Math.Round(a2);
                if (this.Orientation == Orientation.Horizontal)
                    child.Arrange(new Rect(num6, num7, num3, num4));
                else if (this.Orientation == Orientation.Vertical)
                    child.Arrange(new Rect(num7, num6, num4, num3));
            }
            return finalSize;
        }

        /// <summary>
        /// Gets the "corrected" DesiredSize (for Line instances); one that is
        /// more consistent with how the elements actually render.
        /// </summary>
        /// <param name="element">UIElement to get the size for.</param>
        /// <returns>Corrected size.</returns>
        private static Size GetCorrectedDesiredSize(UIElement element)
        {
            Line line = element as Line;
            if (null != line)
                return new Size(Math.Max(line.StrokeThickness, line.X2 - line.X1), Math.Max(line.StrokeThickness, line.Y2 - line.Y1));
            return element.DesiredSize;
        }
    }
}
