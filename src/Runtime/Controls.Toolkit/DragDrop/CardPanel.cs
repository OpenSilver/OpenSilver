// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq;

namespace System.Windows.Controls
{
    /// <summary>
    /// Lays out elements by overlapping each successive item on top of the other.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal class CardPanel : Panel
    {
        #region public double HorizontalMargin
        /// <summary>
        /// Gets or sets the horizontal margin between items.
        /// </summary>
        public double HorizontalMargin
        {
            get { return (double)GetValue(HorizontalMarginProperty); }
            set { SetValue(HorizontalMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalMarginProperty =
            DependencyProperty.Register(
                nameof(HorizontalMargin),
                typeof(double),
                typeof(CardPanel),
                new PropertyMetadata(4.0, OnHorizontalMarginPropertyChanged));

        /// <summary>
        /// Called when the value of the HorizontalMargin property changes.
        /// </summary>
        /// <param name="d">Control that changed its HorizontalMargin.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnHorizontalMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardPanel source = (CardPanel)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnHorizontalMarginPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the value of the HorizontalMargin property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnHorizontalMarginPropertyChanged(double oldValue, double newValue)
        {
            InvalidateArrange();
        }
        #endregion public double HorizontalMargin

        #region public double VerticalMargin
        /// <summary>
        /// Gets or sets the horizontal margin between items.
        /// </summary>
        public double VerticalMargin
        {
            get { return (double)GetValue(VerticalMarginProperty); }
            set { SetValue(VerticalMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalMarginProperty =
            DependencyProperty.Register(
                nameof(VerticalMargin),
                typeof(double),
                typeof(CardPanel),
                new PropertyMetadata(4.0, OnVerticalMarginPropertyChanged));

        /// <summary>
        /// Called when the value of the VerticalMargin property changes.
        /// </summary>
        /// <param name="d">Control that changed its VerticalMargin.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnVerticalMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardPanel source = (CardPanel)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnVerticalMarginPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the value of the VerticalMargin property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnVerticalMarginPropertyChanged(double oldValue, double newValue)
        {
            InvalidateArrange();
        }
        #endregion public double VerticalMargin

        /// <summary>
        /// Measures the children of the panel.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The size required by the children.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in this.Children)
            {
                element.Measure(availableSize);
            }

            double? width = 
                EnumerableExtensions.Zip(
                    this.Children.Select(child => child.DesiredSize.Width), 
                    EnumerableExtensions.Iterate(0.0, x => x + HorizontalMargin), 
                    (desiredWidth, offset) => desiredWidth + offset).MaxOrNullable();

            double? height = 
                EnumerableExtensions.Zip(
                    this.Children.Select(child => child.DesiredSize.Height), 
                    EnumerableExtensions.Iterate(0.0, x => x + VerticalMargin), 
                    (desiredHeight, offset) => desiredHeight + offset).MaxOrNullable();

            return new Size(width ?? 0, height ?? 0);
        }

        /// <summary>
        /// Arranges the children of the panel.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns>The size required by the children.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int cnt = this.Children.Count - 1; cnt >= 0; cnt--)
            {
                UIElement element = this.Children[cnt];
                Canvas.SetZIndex(element, this.Children.Count - cnt);
                Rect arrangeRect = new Rect(cnt * HorizontalMargin, cnt * VerticalMargin, element.DesiredSize.Width, element.DesiredSize.Height);
                element.Arrange(arrangeRect);
            }

            return finalSize;
        }
    }
}