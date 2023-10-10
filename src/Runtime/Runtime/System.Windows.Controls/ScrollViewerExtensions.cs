// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using MouseWheelEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides useful extensions to ScrollViewer instances.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class ScrollViewerExtensions
    {
        /// <summary>
        /// The amount to scroll a ScrollViewer for a line change.
        /// </summary>
        private const double LineChange = 16.0;

        #region public attached bool IsMouseWheelScrollingEnabled
        /// <summary>
        /// Gets a value indicating whether the ScrollViewer has enabled
        /// scrolling via the mouse wheel.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <returns>
        /// A value indicating whether the ScrollViewer has enabled scrolling
        /// via the mouse wheel.
        /// </returns>
        public static bool GetIsMouseWheelScrollingEnabled(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }
            return (bool)viewer.GetValue(IsMouseWheelScrollingEnabledProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the ScrollViewer will enable
        /// scrolling via the mouse wheel.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="value">
        /// A value indicating whether the ScrollViewer will enable scrolling
        /// via the mouse wheel.
        /// </param>
        public static void SetIsMouseWheelScrollingEnabled(this ScrollViewer viewer, bool value)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }
            viewer.SetValue(IsMouseWheelScrollingEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the IsMouseWheelScrollingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsMouseWheelScrollingEnabled",
                typeof(bool),
                typeof(ScrollViewerExtensions),
                new PropertyMetadata(false, OnIsMouseWheelScrollingEnabledPropertyChanged));

        /// <summary>
        /// IsMouseWheelScrollingEnabledProperty property changed handler.
        /// </summary>
        /// <param name="d">ScrollViewerExtensions that changed its IsMouseWheelScrollingEnabled.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsMouseWheelScrollingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer source = d as ScrollViewer;
            bool enabled = (bool)e.NewValue;

            // Attach or detach from the MouseWheel event
            if (enabled)
            {
#if MIGRATION
                source.MouseWheel += OnMouseWheel;
#else
                source.PointerWheelChanged += OnMouseWheel;
#endif
            }
            else
            {
#if MIGRATION
                source.MouseWheel -= OnMouseWheel;
#else
                source.PointerWheelChanged -= OnMouseWheel;
#endif
            }
        }

        /// <summary>
        /// Handles the mouse wheel event.
        /// </summary>
        /// <param name="sender">The ScrollViewer.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;

            Debug.Assert(viewer != null, "sender should be a non-null ScrollViewer!");
            Debug.Assert(e != null, "e should not be null!");

            if (!e.Handled)
            {
#if MIGRATION
                double position = CoerceVerticalOffset(viewer, viewer.VerticalOffset - e.Delta); 
#else
                double position = CoerceVerticalOffset(viewer, viewer.VerticalOffset - e.GetCurrentPoint(null).Properties.MouseWheelDelta);
#endif
                viewer.ScrollToVerticalOffset(position);
                e.Handled = true;
            }
        }
#endregion public attached bool IsMouseWheelScrollingEnabled

        #region private attached double VerticalOffset
        /// <summary>
        /// Gets the value of the VerticalOffset attached property for a specified ScrollViewer.
        /// </summary>
        /// <param name="element">The ScrollViewer from which the property value is read.</param>
        /// <returns>The VerticalOffset property value for the ScrollViewer.</returns>
        private static double GetVerticalOffset(ScrollViewer element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (double)element.GetValue(VerticalOffsetProperty);
        }

        /// <summary>
        /// Sets the value of the VerticalOffset attached property to a specified ScrollViewer.
        /// </summary>
        /// <param name="element">The ScrollViewer to which the attached property is written.</param>
        /// <param name="value">The needed VerticalOffset value.</param>
        private static void SetVerticalOffset(ScrollViewer element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// Identifies the VerticalOffset dependency property.
        /// </summary>
        private static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerExtensions),
                new PropertyMetadata(OnVerticalOffsetPropertyChanged));

        /// <summary>
        /// VerticalOffsetProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">ScrollViewer that changed its VerticalOffset.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnVerticalOffsetPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ScrollViewer source = dependencyObject as ScrollViewer;
            if (source == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            source.ScrollToVerticalOffset((double)eventArgs.NewValue);
        }
        #endregion private attached double VerticalOffset

        #region private attached double HorizontalOffset
        /// <summary>
        /// Gets the value of the HorizontalOffset attached property for a specified ScrollViewer.
        /// </summary>
        /// <param name="element">The ScrollViewer from which the property value is read.</param>
        /// <returns>The HorizontalOffset property value for the ScrollViewer.</returns>
        private static double GetHorizontalOffset(ScrollViewer element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Sets the value of the HorizontalOffset attached property to a specified ScrollViewer.
        /// </summary>
        /// <param name="element">The ScrollViewer to which the attached property is written.</param>
        /// <param name="value">The needed HorizontalOffset value.</param>
        private static void SetHorizontalOffset(ScrollViewer element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        private static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerExtensions),
                new PropertyMetadata(OnHorizontalOffsetPropertyChanged));

        /// <summary>
        /// HorizontalOffsetProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">ScrollViewer that changed its HorizontalOffset.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnHorizontalOffsetPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ScrollViewer source = dependencyObject as ScrollViewer;
            if (source == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            source.ScrollToHorizontalOffset((double)eventArgs.NewValue);
        }
        #endregion private attached double HorizontalOffset

        /// <summary>
        /// Coerce a vertical offset to fall within the vertical bounds of a
        /// ScrollViewer.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The vertical offset to coerce.</param>
        /// <returns>
        /// The coerced vertical offset that falls within the ScrollViewer's
        /// vertical bounds.
        /// </returns>
        private static double CoerceVerticalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            return Math.Max(Math.Min(offset, viewer.ExtentHeight), 0.0);
        }

        /// <summary>
        /// Coerce a horizontal offset to fall within the horizontal bounds of a
        /// ScrollViewer.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The horizontal offset to coerce.</param>
        /// <returns>
        /// The coerced horizontal offset that falls within the ScrollViewer's
        /// horizontal bounds.
        /// </returns>
        private static double CoerceHorizontalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            return Math.Max(Math.Min(offset, viewer.ExtentWidth), 0.0);
        }

        /// <summary>
        /// Scroll a ScrollViewer vertically by a given offset.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The vertical offset to scroll.</param>
        private static void ScrollByVerticalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            offset += viewer.VerticalOffset;
            offset = CoerceVerticalOffset(viewer, offset);
            viewer.ScrollToVerticalOffset(offset);
        }

        /// <summary>
        /// Scroll a ScrollViewer horizontally by a given offset.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="offset">The horizontal offset to scroll.</param>
        private static void ScrollByHorizontalOffset(ScrollViewer viewer, double offset)
        {
            Debug.Assert(viewer != null, "viewer should not be null!");

            offset += viewer.HorizontalOffset;
            offset = CoerceHorizontalOffset(viewer, offset);
            viewer.ScrollToHorizontalOffset(offset);
        }

#if false
        /// <summary>
        /// Scroll the ScrollViewer up by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void LineUp(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByVerticalOffset(viewer, -LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer down by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void LineDown(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByVerticalOffset(viewer, LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer left by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void LineLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByHorizontalOffset(viewer, -LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer right by a line.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void LineRight(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByHorizontalOffset(viewer, LineChange);
        }

        /// <summary>
        /// Scroll the ScrollViewer up by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void PageUp(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByVerticalOffset(viewer, -viewer.ViewportHeight);
        }

        /// <summary>
        /// Scroll the ScrollViewer down by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void PageDown(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByVerticalOffset(viewer, viewer.ViewportHeight);
        }

        /// <summary>
        /// Scroll the ScrollViewer left by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void PageLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByHorizontalOffset(viewer, -viewer.ViewportWidth);
        }

        /// <summary>
        /// Scroll the ScrollViewer right by a page.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void PageRight(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            ScrollByHorizontalOffset(viewer, viewer.ViewportWidth);
        }

        /// <summary>
        /// Scroll the ScrollViewer to the top.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void ScrollToTop(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            viewer.ScrollToVerticalOffset(0);
        }

        /// <summary>
        /// Scroll the ScrollViewer to the bottom.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void ScrollToBottom(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            viewer.ScrollToVerticalOffset(viewer.ExtentHeight);
        }
#endif

        /// <summary>
        /// Scroll the ScrollViewer to the left.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void ScrollToLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            viewer.ScrollToHorizontalOffset(0);
        }

        /// <summary>
        /// Scroll the ScrollViewer to the right.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        public static void ScrollToRight(this ScrollViewer viewer)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }

            viewer.ScrollToHorizontalOffset(viewer.ExtentWidth);
        }

        /// <summary>
        /// Scroll the desired element into the ScrollViewer's viewport.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="element">The element to scroll into view.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void ScrollIntoView(this ScrollViewer viewer, FrameworkElement element)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }
            else if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            ScrollIntoView(viewer, element, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// Scroll the desired element into the ScrollViewer's viewport.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="element">The element to scroll into view.</param>
        /// <param name="horizontalMargin">The margin to add on the left or right.
        /// </param>
        /// <param name="verticalMargin">The margin to add on the top or bottom.
        /// </param>
        /// <param name="duration">The duration of the animation.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="viewer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static void ScrollIntoView(this ScrollViewer viewer, FrameworkElement element, double horizontalMargin, double verticalMargin, Duration duration)
        {
            if (viewer == null)
            {
                throw new ArgumentNullException(nameof(viewer));
            }
            else if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // Get the position of the element relative to the ScrollHost
            Rect? itemRect = element.GetBoundsRelativeTo(viewer);
            if (itemRect == null)
            {
                return;
            }

            // Scroll vertically
            double verticalOffset = viewer.VerticalOffset;
            double verticalDelta = 0;
            double hostBottom = viewer.ViewportHeight;
            double itemBottom = itemRect.Value.Bottom + verticalMargin;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }
            double itemTop = itemRect.Value.Top - verticalMargin;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }

            // Scroll horizontally
            double horizontalOffset = viewer.HorizontalOffset;
            double horizontalDelta = 0;
            double hostRight = viewer.ViewportWidth;
            double itemRight = itemRect.Value.Right + horizontalMargin;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemRight - hostRight;
                horizontalOffset += horizontalDelta;
            }
            double itemLeft = itemRect.Value.Left - horizontalMargin;
            if (itemLeft - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }

            if (duration == TimeSpan.Zero)
            {
                viewer.ScrollToVerticalOffset(verticalOffset);
                viewer.ScrollToHorizontalOffset(horizontalOffset);
            }
            else
            {
                Storyboard storyboard = new Storyboard();
                SetVerticalOffset(viewer, viewer.VerticalOffset);
                SetHorizontalOffset(viewer, viewer.HorizontalOffset);

                DoubleAnimation verticalOffsetAnimation = new DoubleAnimation { To = verticalOffset, Duration = duration };
                DoubleAnimation horizontalOffsetAnimation = new DoubleAnimation { To = verticalOffset, Duration = duration };

                Storyboard.SetTarget(verticalOffsetAnimation, viewer);
                Storyboard.SetTarget(horizontalOffsetAnimation, viewer);
                Storyboard.SetTargetProperty(horizontalOffsetAnimation, new PropertyPath(ScrollViewerExtensions.HorizontalOffsetProperty));
                Storyboard.SetTargetProperty(verticalOffsetAnimation, new PropertyPath(ScrollViewerExtensions.VerticalOffsetProperty));

                storyboard.Children.Add(verticalOffsetAnimation);
                storyboard.Children.Add(horizontalOffsetAnimation);

                storyboard.Begin();
            }
        }
    }
}
