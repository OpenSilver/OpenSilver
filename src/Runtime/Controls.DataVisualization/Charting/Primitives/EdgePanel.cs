// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#if MIGRATION
using System.Windows.Media;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting.Primitives
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting.Primitives
#endif
{
    public class EdgePanel : Panel
    {
        /// <summary>Identifies the Edge dependency property.</summary>
        public static readonly DependencyProperty EdgeProperty = DependencyProperty.RegisterAttached("Edge", typeof(Edge), typeof(EdgePanel), new PropertyMetadata((object)Edge.Center, new PropertyChangedCallback(EdgePanel.OnEdgePropertyChanged)));
        /// <summary>The maximum number of iterations.</summary>
        private const int MaximumIterations = 10;
        /// <summary>A flag that ignores a property change when set.</summary>
        private static bool _ignorePropertyChange;
        /// <summary>The left rectangle in which to render left elements.</summary>
        private Rect _leftRect;
        /// <summary>
        /// The right rectangle in which to render right elements.
        /// </summary>
        private Rect _rightRect;
        /// <summary>The top rectangle in which to render top elements.</summary>
        private Rect _topRect;
        /// <summary>
        /// The bottom rectangle in which to render bottom elements.
        /// </summary>
        private Rect _bottomRect;

        /// <summary>
        /// Gets the value of the Edge attached property for a specified
        /// UIElement.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>The Edge property value for the element.</returns>
        public static Edge GetEdge(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Edge)element.GetValue(EdgePanel.EdgeProperty);
        }

        /// <summary>
        /// Sets the value of the Edge attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="edge">The needed Edge value.</param>
        public static void SetEdge(UIElement element, Edge edge)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(EdgePanel.EdgeProperty, (object)edge);
        }

        /// <summary>EdgeProperty property changed handler.</summary>
        /// <param name="d">UIElement that changed its Edge.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnEdgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (EdgePanel._ignorePropertyChange)
            {
                EdgePanel._ignorePropertyChange = false;
            }
            else
            {
                UIElement uiElement = (UIElement)d;
                Edge newValue = (Edge)e.NewValue;
                int num;
                switch (newValue)
                {
                    case Edge.Left:
                    case Edge.Top:
                    case Edge.Right:
                    case Edge.Center:
                        num = 1;
                        break;
                    default:
                        num = newValue == Edge.Bottom ? 1 : 0;
                        break;
                }
                if (num == 0)
                {
                    EdgePanel._ignorePropertyChange = true;
                    uiElement.SetValue(EdgePanel.EdgeProperty, (object)(Edge)e.OldValue);
                    throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}", new object[1]
                    {
            (object) newValue
                    }), "value");
                }
                EdgePanel parent = VisualTreeHelper.GetParent((DependencyObject)uiElement) as EdgePanel;
                if (parent == null)
                    return;
                parent.InvalidateMeasure();
            }
        }

        /// <summary>Initializes a new instance of the EdgePanel class.</summary>
        public EdgePanel()
        {
            this.SizeChanged += new SizeChangedEventHandler(this.EdgePanelSizeChanged);
        }

        /// <summary>Invalidate measure when edge panel is resized.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void EdgePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Measures the children of a EdgePanel in anticipation of arranging
        /// them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">A maximum Size to not exceed.</param>
        /// <returns>The desired size of the EdgePanel.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            constraint = new Size(this.ActualWidth, this.ActualHeight);
            IList<UIElement> list1 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Left)).ToList<UIElement>();
            IList<UIElement> list2 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Right)).ToList<UIElement>();
            IList<UIElement> list3 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Bottom)).ToList<UIElement>();
            IList<UIElement> list4 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Top)).ToList<UIElement>();
            Rect rect = EdgePanel.SafeCreateRect(0.0, 0.0, constraint.Width, constraint.Height);
            this._leftRect = list1.Count > 0 ? rect : Rect.Empty;
            this._bottomRect = list3.Count > 0 ? rect : Rect.Empty;
            this._rightRect = list2.Count > 0 ? rect : Rect.Empty;
            this._topRect = list4.Count > 0 ? rect : Rect.Empty;
            double num1 = 0.0;
            double val2 = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            double num8 = num1;
            double num9 = val2;
            double num10 = num2;
            double num11 = num3;
            int num12 = 0;
            while (true)
            {
                if (list2.Count > 0)
                {
                    Size availableSize = new Size(constraint.Width, this._rightRect.Height);
                    foreach (UIElement uiElement in (IEnumerable<UIElement>)list2)
                        uiElement.Measure(availableSize);
                    num8 = num1;
                    num1 = list2.Select<UIElement, double>((Func<UIElement, double>)(axis => axis.DesiredSize.Width)).SumOrDefault();
                    num4 = Math.Max(num4, num1);
                    this._rightRect = EdgePanel.SafeCreateRect(constraint.Width - num1, this._rightRect.Top, num1, this._rightRect.Height);
                }
                if (list4.Count > 0)
                {
                    Size availableSize = new Size(this._topRect.Width, constraint.Height);
                    foreach (UIElement uiElement in (IEnumerable<UIElement>)list4)
                        uiElement.Measure(availableSize);
                    num10 = num2;
                    num2 = list4.Select<UIElement, double>((Func<UIElement, double>)(axis => axis.DesiredSize.Height)).SumOrDefault();
                    num6 = Math.Max(num6, num2);
                    this._topRect = EdgePanel.SafeCreateRect(this._topRect.Left, this._topRect.Top, this._topRect.Width, num2);
                }
                if (list1.Count > 0)
                {
                    Size availableSize = new Size(constraint.Width, this._leftRect.Height);
                    foreach (UIElement uiElement in (IEnumerable<UIElement>)list1)
                        uiElement.Measure(availableSize);
                    num9 = val2;
                    val2 = list1.Select<UIElement, double>((Func<UIElement, double>)(axis => axis.DesiredSize.Width)).SumOrDefault();
                    num5 = Math.Max(num5, val2);
                    this._leftRect = EdgePanel.SafeCreateRect(this._leftRect.Left, this._leftRect.Top, list1.Select<UIElement, double>((Func<UIElement, double>)(axis => axis.DesiredSize.Width)).SumOrDefault(), this._leftRect.Height);
                }
                if (list3.Count > 0)
                {
                    Size availableSize = new Size(this._bottomRect.Width, constraint.Height);
                    foreach (UIElement uiElement in (IEnumerable<UIElement>)list3)
                        uiElement.Measure(availableSize);
                    num11 = num3;
                    num3 = list3.Select<UIElement, double>((Func<UIElement, double>)(axis => axis.DesiredSize.Height)).SumOrDefault();
                    num7 = Math.Max(num7, num3);
                    this._bottomRect = EdgePanel.SafeCreateRect(this._bottomRect.Left, constraint.Height - num3, this._bottomRect.Width, num3);
                }
                Rect leftRect1 = this._leftRect;
                leftRect1.Intersect(this._rightRect);
                Rect topRect = this._topRect;
                topRect.Intersect(this._bottomRect);
                if (leftRect1.IsEmptyOrHasNoSize() && topRect.IsEmptyOrHasNoSize())
                {
                    Rect leftRect2 = this._leftRect;
                    leftRect2.Intersect(this._topRect);
                    Rect rightRect1 = this._rightRect;
                    rightRect1.Intersect(this._topRect);
                    Rect leftRect3 = this._leftRect;
                    leftRect3.Intersect(this._bottomRect);
                    Rect rightRect2 = this._rightRect;
                    rightRect2.Intersect(this._bottomRect);
                    if (!leftRect3.IsEmptyOrHasNoSize() || !rightRect2.IsEmptyOrHasNoSize() || (!leftRect2.IsEmptyOrHasNoSize() || !rightRect1.IsEmptyOrHasNoSize()) || (num11 != num3 || num9 != val2 || num8 != num1) || num10 != num2)
                    {
                        if (num12 != 10)
                        {
                            if (!leftRect3.IsEmptyOrHasNoSize())
                            {
                                this._leftRect = EdgePanel.SafeCreateRect(this._leftRect.Left, this._leftRect.Top, this._leftRect.Width, this._leftRect.Height - leftRect3.Height);
                                this._bottomRect = EdgePanel.SafeCreateRect(this._bottomRect.Left + leftRect3.Width, this._bottomRect.Top, this._bottomRect.Width - leftRect3.Width, this._bottomRect.Height);
                            }
                            if (!leftRect2.IsEmptyOrHasNoSize())
                            {
                                this._leftRect = EdgePanel.SafeCreateRect(this._leftRect.Left, this._leftRect.Top + leftRect2.Height, this._leftRect.Width, this._leftRect.Height - leftRect2.Height);
                                this._topRect = EdgePanel.SafeCreateRect(this._topRect.Left + leftRect2.Width, this._topRect.Top, this._topRect.Width - leftRect2.Width, this._topRect.Height);
                            }
                            if (!rightRect2.IsEmptyOrHasNoSize())
                            {
                                this._rightRect = EdgePanel.SafeCreateRect(this._rightRect.Left, this._rightRect.Top, this._rightRect.Width, this._rightRect.Height - rightRect2.Height);
                                this._bottomRect = EdgePanel.SafeCreateRect(this._bottomRect.Left, this._bottomRect.Top, this._bottomRect.Width - rightRect2.Width, this._bottomRect.Height);
                            }
                            if (!rightRect1.IsEmptyOrHasNoSize())
                            {
                                this._rightRect = EdgePanel.SafeCreateRect(this._rightRect.Left, this._rightRect.Top + rightRect1.Height, this._rightRect.Width, this._rightRect.Height - rightRect1.Height);
                                this._topRect = EdgePanel.SafeCreateRect(this._topRect.Left, this._topRect.Top, this._topRect.Width - rightRect1.Width, this._topRect.Height);
                            }
                            if (!this._leftRect.IsEmpty)
                                this._leftRect = new Rect(new Point(this._leftRect.Left, this._topRect.BottomOrDefault(0.0)), new Point(this._leftRect.Right, this._bottomRect.TopOrDefault(constraint.Height)));
                            if (!this._rightRect.IsEmpty)
                                this._rightRect = new Rect(new Point(this._rightRect.Left, this._topRect.BottomOrDefault(0.0)), new Point(this._rightRect.Right, this._bottomRect.TopOrDefault(constraint.Height)));
                            if (!this._bottomRect.IsEmpty)
                                this._bottomRect = new Rect(new Point(this._leftRect.RightOrDefault(0.0), this._bottomRect.Top), new Point(this._rightRect.LeftOrDefault(constraint.Width), this._bottomRect.Bottom));
                            if (!this._topRect.IsEmpty)
                                this._topRect = new Rect(new Point(this._leftRect.RightOrDefault(0.0), this._topRect.Top), new Point(this._rightRect.LeftOrDefault(constraint.Width), this._topRect.Bottom));
                            ++num12;
                        }
                        else
                            goto label_40;
                    }
                    else
                        goto label_86;
                }
                else
                    break;
            }
            return new Size();
        label_40:
            this._leftRect = EdgePanel.SafeCreateRect(0.0, num6, num5, constraint.Height - num6 - num7);
            this._rightRect = EdgePanel.SafeCreateRect(constraint.Width - num4, num6, num4, constraint.Height - num6 - num7);
            this._bottomRect = EdgePanel.SafeCreateRect(num5, constraint.Height - num7, constraint.Width - num5 - num4, num7);
            this._topRect = EdgePanel.SafeCreateRect(num5, 0.0, constraint.Width - num5 - num4, num6);
            foreach (UIElement uiElement in (IEnumerable<UIElement>)list1)
                uiElement.Measure(new Size(this._leftRect.Width, this._leftRect.Height));
            foreach (UIElement uiElement in (IEnumerable<UIElement>)list2)
                uiElement.Measure(new Size(this._rightRect.Width, this._rightRect.Height));
            foreach (UIElement uiElement in (IEnumerable<UIElement>)list3)
                uiElement.Measure(new Size(this._bottomRect.Width, this._bottomRect.Height));
            foreach (UIElement uiElement in (IEnumerable<UIElement>)list4)
                uiElement.Measure(new Size(this._topRect.Width, this._topRect.Height));
            label_86:
            Size availableSize1 = new Size(constraint.Width - this._leftRect.WidthOrDefault(0.0) - this._rightRect.WidthOrDefault(0.0), constraint.Height - this._topRect.HeightOrDefault(0.0) - this._bottomRect.HeightOrDefault(0.0));
            foreach (UIElement uiElement in this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(child => EdgePanel.GetEdge(child) == Edge.Center)))
                uiElement.Measure(availableSize1);
            return new Size();
        }

        /// <summary>
        /// Arranges the content (child elements) of a EdgePanel element.
        /// </summary>
        /// <param name="arrangeSize">
        /// The Size the EdgePanel uses to arrange its child elements.
        /// </param>
        /// <returns>The arranged size of the EdgePanel.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (arrangeSize.Width == 0.0 || arrangeSize.Height == 0.0 || !ValueHelper.CanGraph(arrangeSize.Width) || !ValueHelper.CanGraph(arrangeSize.Height))
                return arrangeSize;
            IList<UIElement> list1 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Left)).ToList<UIElement>();
            IList<UIElement> list2 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Right)).ToList<UIElement>();
            IList<UIElement> list3 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Bottom)).ToList<UIElement>();
            IList<UIElement> list4 = (IList<UIElement>)this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(element => EdgePanel.GetEdge(element) == Edge.Top)).ToList<UIElement>();
            if (!this._bottomRect.IsEmpty)
            {
                double num1 = this._bottomRect.Top;
                foreach (UIElement uiElement1 in (IEnumerable<UIElement>)list3)
                {
                    UIElement uiElement2 = uiElement1;
                    double left = this._leftRect.RightOrDefault(0.0);
                    double top = num1;
                    double width = arrangeSize.Width - this._leftRect.WidthOrDefault(0.0) - this._rightRect.WidthOrDefault(0.0);
                    Size desiredSize = uiElement1.DesiredSize;
                    double height1 = desiredSize.Height;
                    Rect rect = EdgePanel.SafeCreateRect(left, top, width, height1);
                    uiElement2.Arrange(rect);
                    double num2 = num1;
                    desiredSize = uiElement1.DesiredSize;
                    double height2 = desiredSize.Height;
                    num1 = num2 + height2;
                }
            }
            if (!this._topRect.IsEmpty)
            {
                double num1 = this._topRect.Bottom;
                foreach (UIElement uiElement1 in (IEnumerable<UIElement>)list4)
                {
                    double num2 = num1;
                    Size desiredSize = uiElement1.DesiredSize;
                    double height1 = desiredSize.Height;
                    num1 = num2 - height1;
                    UIElement uiElement2 = uiElement1;
                    double left = this._leftRect.RightOrDefault(0.0);
                    double top = num1;
                    double width = arrangeSize.Width - this._leftRect.WidthOrDefault(0.0) - this._rightRect.WidthOrDefault(0.0);
                    desiredSize = uiElement1.DesiredSize;
                    double height2 = desiredSize.Height;
                    Rect rect = EdgePanel.SafeCreateRect(left, top, width, height2);
                    uiElement2.Arrange(rect);
                }
            }
            if (!this._rightRect.IsEmpty)
            {
                double num1 = this._rightRect.Left;
                foreach (UIElement uiElement1 in (IEnumerable<UIElement>)list2)
                {
                    UIElement uiElement2 = uiElement1;
                    double left = num1;
                    double top = this._topRect.BottomOrDefault(0.0);
                    Size desiredSize = uiElement1.DesiredSize;
                    double width1 = desiredSize.Width;
                    double height = arrangeSize.Height - this._bottomRect.HeightOrDefault(0.0) - this._topRect.HeightOrDefault(0.0);
                    Rect rect = EdgePanel.SafeCreateRect(left, top, width1, height);
                    uiElement2.Arrange(rect);
                    double num2 = num1;
                    desiredSize = uiElement1.DesiredSize;
                    double width2 = desiredSize.Width;
                    num1 = num2 + width2;
                }
            }
            if (!this._leftRect.IsEmpty)
            {
                double num1 = this._leftRect.Right;
                foreach (UIElement uiElement in (IEnumerable<UIElement>)list1)
                {
                    double num2 = num1;
                    Size desiredSize = uiElement.DesiredSize;
                    double width1 = desiredSize.Width;
                    num1 = num2 - width1;
                    double left = num1;
                    double top = this._topRect.BottomOrDefault(0.0);
                    desiredSize = uiElement.DesiredSize;
                    double width2 = desiredSize.Width;
                    double height = arrangeSize.Height - this._bottomRect.HeightOrDefault(0.0) - this._topRect.HeightOrDefault(0.0);
                    Rect rect = EdgePanel.SafeCreateRect(left, top, width2, height);
                    uiElement.Arrange(rect);
                }
            }
            Rect rect1 = EdgePanel.SafeCreateRect(this._leftRect.RightOrDefault(0.0), this._topRect.BottomOrDefault(0.0), arrangeSize.Width - this._leftRect.WidthOrDefault(0.0) - this._rightRect.WidthOrDefault(0.0), arrangeSize.Height - this._topRect.HeightOrDefault(0.0) - this._bottomRect.HeightOrDefault(0.0));
            foreach (UIElement uiElement in this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>)(child => EdgePanel.GetEdge(child) == Edge.Center)))
                uiElement.Arrange(rect1);
            return arrangeSize;
        }

        /// <summary>
        /// Creates a Rect safely by forcing width/height to be valid.
        /// </summary>
        /// <param name="left">Rect left parameter.</param>
        /// <param name="top">Rect top parameter.</param>
        /// <param name="width">Rect width parameter.</param>
        /// <param name="height">Rect height parameter.</param>
        /// <returns>New Rect struct.</returns>
        private static Rect SafeCreateRect(double left, double top, double width, double height)
        {
            return new Rect(left, top, Math.Max(0.0, width), Math.Max(0.0, height));
        }
    }
}