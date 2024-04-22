
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a scrollable area that can contain other visible elements.
    /// </summary>
    [TemplatePart(Name = ElementScrollContentPresenterName, Type = typeof(ScrollContentPresenter))]
    [TemplatePart(Name = ElementHorizontalScrollBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = ElementVerticalScrollBarName, Type = typeof(ScrollBar))]
    public sealed class ScrollViewer : ContentControl
    {
        internal const double LineDelta = 16.0; // Default physical amount to scroll with one Up/Down/Left/Right key
        internal const double WheelDelta = 48.0; // Default physical amount to scroll with one MouseWheel.

        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

        // Property caching
        private Visibility _scrollVisibilityX;
        private Visibility _scrollVisibilityY;

        // Scroll property values - cache of what was computed by ISI
        private double _xPositionISI;
        private double _yPositionISI;
        private double _xExtent;
        private double _yExtent;
        private double _xSize;
        private double _ySize;

        private bool _invalidatedMeasureFromArrange;
        private IScrollInfo _scrollInfo;

        private TouchInfo _touchInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewer"/> class.
        /// </summary>
        public ScrollViewer()
        {
            DefaultStyleKey = typeof(ScrollViewer);
        }

        /// <summary> 
        /// Reference to the ScrollContentPresenter child.
        /// </summary>
        internal ScrollContentPresenter ElementScrollContentPresenter { get; private set; }

        /// <summary> 
        /// Reference to the horizontal ScrollBar child. 
        /// </summary>
        internal ScrollBar ElementHorizontalScrollBar { get; private set; }

        /// <summary> 
        /// Reference to the vertical ScrollBar child.
        /// </summary>
        internal ScrollBar ElementVerticalScrollBar { get; private set; }

        internal IScrollInfo ScrollInfo
        {
            get { return _scrollInfo; }
            set
            {
                _scrollInfo = value;
                if (_scrollInfo is not null)
                {
                    _scrollInfo.CanHorizontallyScroll = HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    _scrollInfo.CanVerticallyScroll = VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
            }
        }

        /// <summary>
        /// Gets a value that contains the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// The horizontal offset of the scrolled content. The default value is 0.0.
        /// </returns>
        public double HorizontalOffset
        {
            get => _xPositionISI;
            private set => SetValueInternal(HorizontalOffsetPropertyKey, value);
        }

        private static readonly DependencyPropertyKey HorizontalOffsetPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HorizontalOffset),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = HorizontalOffsetPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal <see cref="ScrollBar"/> 
        /// should be displayed.
        /// </summary>
        /// <returns>
        /// A <see cref="ScrollBarVisibility"/> value that indicates whether a
        /// horizontal <see cref="ScrollBar"/> should be displayed. The default 
        /// value is <see cref="ScrollBarVisibility.Hidden"/>.
        /// </returns>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValueInternal(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached(
                nameof(HorizontalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(ScrollViewer),
                new PropertyMetadata(ScrollBarVisibility.Disabled, OnScrollBarVisibilityChanged));

        private static void OnScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.InvalidateMeasure();
                if (scrollViewer.ScrollInfo is not null)
                {
                    scrollViewer.ScrollInfo.CanHorizontallyScroll = scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    scrollViewer.ScrollInfo.CanVerticallyScroll = scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
            }
        }

        /// <summary>
        /// Gets a value that contains the vertical offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// The vertical offset of the scrolled content. The default value is 0.0.
        /// </returns>
        public double VerticalOffset
        {
            get => _yPositionISI;
            private set => SetValueInternal(VerticalOffsetPropertyKey, value);
        }

        private static readonly DependencyPropertyKey VerticalOffsetPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(VerticalOffset),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = VerticalOffsetPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value that indicates whether a vertical <see cref="ScrollBar"/> 
        /// should be displayed.
        /// </summary>
        /// <returns>
        /// A <see cref="ScrollBarVisibility"/> value that indicates whether a 
        /// vertical <see cref="ScrollBar"/> should be displayed. The default 
        /// value is <see cref="ScrollBarVisibility.Visible"/>.
        /// </returns>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValueInternal(VerticalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached(
                nameof(VerticalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(ScrollViewer),
                new PropertyMetadata(ScrollBarVisibility.Visible, OnScrollBarVisibilityChanged));

        /// <summary>
        /// Scrolls the content that is within the <see cref="ScrollViewer"/> to the 
        /// specified horizontal offset position.
        /// </summary>
        /// <param name="offset">
        /// The position that the content scrolls to.
        /// </param>
        public void ScrollToHorizontalOffset(double offset) => SetScrollOffset(Orientation.Horizontal, offset);

        /// <summary>
        /// Scrolls the content that is within the <see cref="ScrollViewer"/> to the 
        /// specified vertical offset position.
        /// </summary>
        /// <param name="offset">
        /// The position that the content scrolls to.
        /// </param>
        public void ScrollToVerticalOffset(double offset) => SetScrollOffset(Orientation.Vertical, offset);

        /// <summary>
        /// Gets the value of the <see cref="HorizontalScrollBarVisibility"/> dependency property 
        /// from a specified element.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </returns>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (ScrollBarVisibility)element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HorizontalScrollBarVisibility"/> dependency property 
        /// to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element on which to set the property value.
        /// </param>
        /// <param name="horizontalScrollBarVisibility">
        /// The property value to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
        }

        /// <summary>
        /// Gets the value of the <see cref="VerticalScrollBarVisibility"/> dependency property 
        /// from a specified element.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </returns>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="VerticalScrollBarVisibility"/> dependency property 
        /// to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element on which to set the property value.
        /// </param>
        /// <param name="verticalScrollBarVisibility">
        /// The property value to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementScrollContentPresenter = GetTemplateChild(ElementScrollContentPresenterName) as ScrollContentPresenter;
            ElementHorizontalScrollBar = GetTemplateChild(ElementHorizontalScrollBarName) as ScrollBar;
            ElementVerticalScrollBar = GetTemplateChild(ElementVerticalScrollBarName) as ScrollBar;

            if (ElementHorizontalScrollBar is not null)
            {
                ElementHorizontalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Horizontal, e); };
            }
            if (ElementVerticalScrollBar is not null)
            {
                ElementVerticalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Vertical, e); };
            }
        }

        private void SetScrollOffset(Orientation orientation, double value)
        {
            if (ScrollInfo is not null)
            {
                double scrollable = (orientation == Orientation.Horizontal) ? ScrollableWidth : ScrollableHeight;
                double clamped = Math.Max(value, 0);

                clamped = Math.Min(scrollable, clamped);

                // Update ScrollContentPresenter 
                if (orientation == Orientation.Horizontal)
                    ScrollInfo.SetHorizontalOffset(clamped);
                else
                    ScrollInfo.SetVerticalOffset(clamped);
            }
        }
        /// <summary> 
        /// Handles the ScrollBar.Scroll event and updates the UI.
        /// </summary>
        /// <param name="orientation">Orientation of the ScrollBar.</param> 
        /// <param name="e">A ScrollEventArgs that contains the event data.</param> 
        private void HandleScroll(Orientation orientation, ScrollEventArgs e)
        {
            if (ScrollInfo is not null)
            {
                bool horizontal = orientation == Orientation.Horizontal;

                // Calculate new offset 
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.ThumbPosition:
                    case ScrollEventType.ThumbTrack:
                        SetScrollOffset(orientation, e.NewValue);
                        break;
                    case ScrollEventType.LargeDecrement:
                        if (horizontal)
                            ScrollInfo.PageLeft();
                        else
                            ScrollInfo.PageUp();
                        break;
                    case ScrollEventType.LargeIncrement:
                        if (horizontal)
                            ScrollInfo.PageRight();
                        else
                            ScrollInfo.PageDown();
                        break;
                    case ScrollEventType.SmallDecrement:
                        if (horizontal)
                            ScrollInfo.LineLeft();
                        else
                            ScrollInfo.LineUp();
                        break;
                    case ScrollEventType.SmallIncrement:
                        if (horizontal)
                            ScrollInfo.LineRight();
                        else
                            ScrollInfo.LineDown();
                        break;
                    case ScrollEventType.First:
                        SetScrollOffset(orientation, double.MinValue);
                        break;
                    case ScrollEventType.Last:
                        SetScrollOffset(orientation, double.MaxValue);
                        break;
                }
            }
        }

        /// <summary> 
        /// Scrolls the view in the specified direction.
        /// </summary> 
        /// <param name="key">Key corresponding to the direction.</param>
        /// <remarks>Similar to WPF's corresponding ScrollViewer method.</remarks>
        internal void ScrollInDirection(Key key)
        {
            if (ScrollInfo is not null)
            {
                switch (key)
                {
                    case Key.Up:
                        ScrollInfo.LineUp();
                        break;
                    case Key.Down:
                        ScrollInfo.LineDown();
                        break;
                    case Key.Left:
                        ScrollInfo.LineLeft();
                        break;
                    case Key.Right:
                        ScrollInfo.LineRight();
                        break;
                }
            }
        }

        private static readonly DependencyPropertyKey ScrollableHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ScrollableHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="ScrollableHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollableHeightProperty = ScrollableHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that represents the vertical size of the area that can be scrolled;
        /// the difference between the height of the extent and the height of the viewport.
        /// </summary>
        /// <returns>
        /// The vertical size of the area that can be scrolled. This property has no default value.
        /// </returns>
        public double ScrollableHeight => Math.Max(0.0, ExtentHeight - ViewportHeight);

        private static readonly DependencyPropertyKey ScrollableWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ScrollableWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="ScrollableWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollableWidthProperty = ScrollableWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that represents the horizontal size of the area that can be scrolled;
        /// the difference between the width of the extent and the width of the viewport.
        /// </summary>
        /// <returns>
        /// The horizontal size of the area that can be scrolled. This property has no default value.
        /// </returns>
        public double ScrollableWidth => Math.Max(0.0, ExtentWidth - ViewportWidth);

        private static readonly DependencyPropertyKey ViewportHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ViewportHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="ViewportHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportHeightProperty = ViewportHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that contains the vertical size of the viewable content.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewable content. This property has no default value.
        /// </returns>
        public double ViewportHeight => _ySize;

        private static readonly DependencyPropertyKey ViewportWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ViewportWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="ViewportWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportWidthProperty = ViewportWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that contains the horizontal size of the viewable content.
        /// </summary>
        /// <returns>
        /// The horizontal size of the viewable content. The default value is 0.0.
        /// </returns>
        public double ViewportWidth => _xSize;

        private static readonly DependencyPropertyKey ComputedHorizontalScrollBarVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ComputedHorizontalScrollBarVisibility),
                typeof(Visibility),
                typeof(ScrollViewer),
                null);

        /// <summary>
        /// Identifies the <see cref="ComputedHorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty =
            ComputedHorizontalScrollBarVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the horizontal <see cref="ScrollBar"/> is visible.
        /// </summary>
        /// <returns>
        /// A <see cref="Visibility"/> that indicates whether the horizontal scroll bar is visible.
        /// The default value is <see cref="Visibility.Visible"/>.
        /// </returns>
        public Visibility ComputedHorizontalScrollBarVisibility => _scrollVisibilityX;

        private static readonly DependencyPropertyKey ComputedVerticalScrollBarVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ComputedVerticalScrollBarVisibility),
                typeof(Visibility),
                typeof(ScrollViewer),
                null);

        /// <summary>
        /// Identifies the <see cref="ComputedVerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty =
            ComputedVerticalScrollBarVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the vertical <see cref="ScrollBar"/> is visible.
        /// </summary>
        /// <returns>
        /// A <see cref="Visibility"/> that indicates whether the vertical scroll bar is visible.
        /// The default value is <see cref="Visibility.Visible"/>.
        /// </returns>
        public Visibility ComputedVerticalScrollBarVisibility => _scrollVisibilityY;

        private static readonly DependencyPropertyKey ExtentHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ExtentHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifier for the <see cref="ExtentHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentHeightProperty = ExtentHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the vertical size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </summary>
        /// <returns>
        /// The vertical size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </returns>
        public double ExtentHeight => _yExtent;

        private static readonly DependencyPropertyKey ExtentWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ExtentWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Identifier for the <see cref="ExtentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentWidthProperty = ExtentWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the horizontal size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </summary>
        /// <returns>
        /// The horizontal size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </returns>
        public double ExtentWidth => _xExtent;

        /// <summary>
        /// Called when the value of properties that describe the size and location of the scroll area change.
        /// </summary>
        public void InvalidateScrollInfo()
        {
            IScrollInfo isi = ScrollInfo;

            // anybody can call this method even if we don't have ISI...
            if (isi == null)
            {
                return;
            }

            // This is a public API, and is expected to be called by the
            // IScrollInfo implementation when any of the scrolling properties
            // change.  Sometimes this is done independently (not as a result
            // of laying out this ScrollViewer) and that means we should re-run
            // the logic of determining visibility of autoscrollbars, if any.
            //
            // However, invalidating measure during arrange is dangerous
            // because it could lead to layout never settling down.  This has
            // been observed with the layout rounding feature and non-standard
            // DPIs causing ScrollViewer to never settle on the visibility of
            // autoscrollbars.
            //
            // To guard against this condition, we only allow measure to be
            // invalidated from arrange once.
            //
            // We also don't invalidate measure if we are in the middle of the
            // measure pass, as the ScrollViewer will already be updating the
            // visibility of the autoscrollbars.
            if (!MeasureInProgress && (!ArrangeInProgress || !_invalidatedMeasureFromArrange))
            {
                //
                // Check if we should remove/add scrollbars.
                //
                double extent = isi.ExtentWidth;
                double viewport = isi.ViewportWidth;

                if (HorizontalScrollBarVisibility == ScrollBarVisibility.Auto &&
                    ((_scrollVisibilityX == Visibility.Collapsed && DoubleUtil.GreaterThan(extent, viewport)) ||
                     (_scrollVisibilityX == Visibility.Visible && DoubleUtil.LessThanOrClose(extent, viewport))))
                {
                    InvalidateMeasure();
                }
                else
                {
                    extent = isi.ExtentHeight;
                    viewport = isi.ViewportHeight;

                    if (VerticalScrollBarVisibility == ScrollBarVisibility.Auto &&
                        ((_scrollVisibilityY == Visibility.Collapsed && DoubleUtil.GreaterThan(extent, viewport)) ||
                         (_scrollVisibilityY == Visibility.Visible && DoubleUtil.LessThanOrClose(extent, viewport))))
                    {
                        InvalidateMeasure();
                    }
                }
            }

            // If any scrolling properties have actually changed, fire public events post-layout
            if (!DoubleUtil.AreClose(HorizontalOffset, isi.HorizontalOffset) ||
                !DoubleUtil.AreClose(VerticalOffset, isi.VerticalOffset) ||
                !DoubleUtil.AreClose(ViewportWidth, isi.ViewportWidth) ||
                !DoubleUtil.AreClose(ViewportHeight, isi.ViewportHeight) ||
                !DoubleUtil.AreClose(ExtentWidth, isi.ExtentWidth) ||
                !DoubleUtil.AreClose(ExtentHeight, isi.ExtentHeight))
            {
                double oldActualHorizontalOffset = HorizontalOffset;
                double oldActualVerticalOffset = VerticalOffset;

                double oldViewportWidth = ViewportWidth;
                double oldViewportHeight = ViewportHeight;

                double oldExtentWidth = ExtentWidth;
                double oldExtentHeight = ExtentHeight;

                double oldScrollableWidth = ScrollableWidth;
                double oldScrollableHeight = ScrollableHeight;

                bool changed = false;

                //
                // Go through scrolling properties updating values.
                //
                if (!DoubleUtil.AreClose(oldActualHorizontalOffset, isi.HorizontalOffset))
                {
                    _xPositionISI = isi.HorizontalOffset;
                    HorizontalOffset = _xPositionISI;
                    changed = true;
                }

                if (!DoubleUtil.AreClose(oldActualVerticalOffset, isi.VerticalOffset))
                {
                    _yPositionISI = isi.VerticalOffset;
                    VerticalOffset = _yPositionISI;
                    changed = true;
                }

                if (!DoubleUtil.AreClose(oldViewportWidth, isi.ViewportWidth))
                {
                    _xSize = isi.ViewportWidth;
                    SetValueInternal(ViewportWidthPropertyKey, _xSize);
                    changed = true;
                }

                if (!DoubleUtil.AreClose(oldViewportHeight, isi.ViewportHeight))
                {
                    _ySize = isi.ViewportHeight;
                    SetValueInternal(ViewportHeightPropertyKey, _ySize);
                    changed = true;
                }

                if (!DoubleUtil.AreClose(oldExtentWidth, isi.ExtentWidth))
                {
                    _xExtent = isi.ExtentWidth;
                    SetValueInternal(ExtentWidthPropertyKey, _xExtent);
                    changed = true;
                }

                if (!DoubleUtil.AreClose(oldExtentHeight, isi.ExtentHeight))
                {
                    _yExtent = isi.ExtentHeight;
                    SetValueInternal(ExtentHeightPropertyKey, _yExtent);
                    changed = true;
                }

                // ScrollableWidth/Height are dependant on Viewport and Extent set above.  This check must be done after those.
                double scrollableWidth = ScrollableWidth;
                if (!DoubleUtil.AreClose(oldScrollableWidth, ScrollableWidth))
                {
                    SetValueInternal(ScrollableWidthPropertyKey, scrollableWidth);
                    changed = true;
                }

                double scrollableHeight = ScrollableHeight;
                if (!DoubleUtil.AreClose(oldScrollableHeight, ScrollableHeight))
                {
                    SetValueInternal(ScrollableHeightPropertyKey, scrollableHeight);
                    changed = true;
                }

                if (changed)
                {
                    if (ElementHorizontalScrollBar != null && !DoubleUtil.AreClose(oldActualHorizontalOffset, HorizontalOffset))
                    {
                        ElementHorizontalScrollBar.Value = HorizontalOffset;
                    }

                    if (ElementVerticalScrollBar != null && !DoubleUtil.AreClose(oldActualVerticalOffset, VerticalOffset))
                    {
                        ElementVerticalScrollBar.Value = VerticalOffset;
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && Focus())
            {
                e.Handled = true;
            }

            if (e.IsTouchEvent)
            {
                Point position = e.GetPosition(null);
                _touchInfo = new TouchInfo
                {
                    X = position.X,
                    Y = position.Y,
                    HorizontalOffset = ScrollInfo.HorizontalOffset,
                    VerticalOffset = ScrollInfo.VerticalOffset,
                };
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _touchInfo = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!e.IsTouchEvent || Pointer.Captured is not null || ScrollInfo is null || _touchInfo is null)
            {
                return;
            }

            Point position = e.GetPosition(null);

            if (ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                double deltaX = _touchInfo.X - position.X;
                _touchInfo.HorizontalOffset += deltaX;
                ScrollToHorizontalOffset(_touchInfo.HorizontalOffset);
            }

            if (ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                double deltaY = _touchInfo.Y - position.Y;
                _touchInfo.VerticalOffset += deltaY;
                ScrollToVerticalOffset(_touchInfo.VerticalOffset);
            }

            _touchInfo.X = position.X;
            _touchInfo.Y = position.Y;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Handled || ScrollInfo is null || e.Delta == 0)
            {
                return;
            }

            if (e.Delta < 0)
            {
                if (ScrollInfo.VerticalOffset == ScrollableHeight) return;

                ScrollInfo.MouseWheelDown();
            }
            else if (e.Delta > 0)
            {
                if (ScrollInfo.VerticalOffset == 0) return;

                ScrollInfo.MouseWheelUp();
            }

            e.Handled = true;
        }

        private bool TemplatedParentHandlesScrolling => TemplatedParent is Control c && c.HandlesScrolling;

        /// <summary>
        /// Responds to the KeyDown event. 
        /// </summary> 
        /// <param name="e">
        /// Provides data for KeyEventArgs.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (ScrollInfo is not null && !e.Handled && !TemplatedParentHandlesScrolling)
            {
                // Parent is not going to handle scrolling; do so here 
                bool control = ModifierKeys.Control == (Keyboard.Modifiers & ModifierKeys.Control);
                bool handled = true;

                switch (e.Key)
                {
                    case Key.Up:
                        ScrollInfo.LineUp();
                        break;
                    case Key.Down:
                        ScrollInfo.LineDown();
                        break;
                    case Key.Left:
                        ScrollInfo.LineLeft();
                        break;
                    case Key.Right:
                        ScrollInfo.LineRight();
                        break;
                    case Key.PageUp:
                        ScrollInfo.PageUp();
                        break;
                    case Key.PageDown:
                        ScrollInfo.PageDown();
                        break;
                    case Key.Home:
                        if (!control)
                            SetScrollOffset(Orientation.Horizontal, double.MinValue);
                        else
                            SetScrollOffset(Orientation.Vertical, double.MinValue);
                        break;
                    case Key.End:
                        if (!control)
                            SetScrollOffset(Orientation.Horizontal, double.MaxValue);
                        else
                            SetScrollOffset(Orientation.Vertical, double.MaxValue);
                        break;
                    default:
                        handled = false;
                        break;
                }

                if (handled)
                    e.Handled = true;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new ScrollViewerAutomationPeer(this);

        protected override Size MeasureOverride(Size constraint)
        {
            IScrollInfo isi = ScrollInfo;

            UIElement child = VisualChildrenCount > 0 ? GetVisualChild(0) : null;
            ScrollBarVisibility vsbv = VerticalScrollBarVisibility;
            ScrollBarVisibility hsbv = HorizontalScrollBarVisibility;
            Size desiredSize = new Size();

            if (child != null)
            {
                bool vsbAuto = vsbv == ScrollBarVisibility.Auto;
                bool hsbAuto = hsbv == ScrollBarVisibility.Auto;
                bool vDisableScroll = vsbv == ScrollBarVisibility.Disabled;
                bool hDisableScroll = hsbv == ScrollBarVisibility.Disabled;
                Visibility vv = vsbv == ScrollBarVisibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                Visibility hv = hsbv == ScrollBarVisibility.Visible ? Visibility.Visible : Visibility.Collapsed;

                if (_scrollVisibilityY != vv)
                {
                    _scrollVisibilityY = vv;
                    SetValueInternal(ComputedVerticalScrollBarVisibilityPropertyKey, _scrollVisibilityY);
                }
                if (_scrollVisibilityX != hv)
                {
                    _scrollVisibilityX = hv;
                    SetValueInternal(ComputedHorizontalScrollBarVisibilityPropertyKey, _scrollVisibilityX);
                }

                if (isi != null)
                {
                    isi.CanHorizontallyScroll = !hDisableScroll;
                    isi.CanVerticallyScroll = !vDisableScroll;
                }

                child.Measure(constraint);

                // it could now be here as a result of visual template expansion that happens during Measure
                isi = ScrollInfo;

                if (isi != null && (hsbAuto || vsbAuto))
                {
                    bool makeHorizontalBarVisible = hsbAuto && DoubleUtil.GreaterThan(isi.ExtentWidth, isi.ViewportWidth);
                    bool makeVerticalBarVisible = vsbAuto && DoubleUtil.GreaterThan(isi.ExtentHeight, isi.ViewportHeight);

                    if (makeHorizontalBarVisible)
                    {
                        if (_scrollVisibilityX != Visibility.Visible)
                        {
                            _scrollVisibilityX = Visibility.Visible;
                            SetValueInternal(ComputedHorizontalScrollBarVisibilityPropertyKey, _scrollVisibilityX);
                        }
                    }

                    if (makeVerticalBarVisible)
                    {
                        if (_scrollVisibilityY != Visibility.Visible)
                        {
                            _scrollVisibilityY = Visibility.Visible;
                            SetValueInternal(ComputedVerticalScrollBarVisibilityPropertyKey, _scrollVisibilityY);
                        }
                    }

                    if (makeHorizontalBarVisible || makeVerticalBarVisible)
                    {
                        // Remeasure our visual tree.
                        // Requires this extra invalidation because we need to remeasure Grid which is not neccessarily dirty now
                        // since we only invlaidated scrollbars but we don't have LayoutUpdate loop at our disposal here
                        child.InvalidateMeasure();
                        child.Measure(constraint);
                    }

                    //if both are Auto, then appearance of one scrollbar may causes appearance of another.
                    //If we don't re-check here, we get some part of content covered by auto scrollbar and can never reach to it since
                    //another scrollbar may not appear (in cases when viewport==extent) - bug 1199443
                    if (hsbAuto && vsbAuto && makeHorizontalBarVisible != makeVerticalBarVisible)
                    {
                        bool makeHorizontalBarVisible2 = !makeHorizontalBarVisible && DoubleUtil.GreaterThan(isi.ExtentWidth, isi.ViewportWidth);
                        bool makeVerticalBarVisible2 = !makeVerticalBarVisible && DoubleUtil.GreaterThan(isi.ExtentHeight, isi.ViewportHeight);

                        if (makeHorizontalBarVisible2)
                        {
                            if (_scrollVisibilityX != Visibility.Visible)
                            {
                                _scrollVisibilityX = Visibility.Visible;
                                SetValueInternal(ComputedHorizontalScrollBarVisibilityPropertyKey, _scrollVisibilityX);
                            }
                        }
                        else if (makeVerticalBarVisible2) //only one can be true
                        {
                            if (_scrollVisibilityY != Visibility.Visible)
                            {
                                _scrollVisibilityY = Visibility.Visible;
                                SetValueInternal(ComputedVerticalScrollBarVisibilityPropertyKey, _scrollVisibilityY);
                            }
                        }

                        if (makeHorizontalBarVisible2 || makeVerticalBarVisible2)
                        {
                            // Remeasure our visual tree.
                            // Requires this extra invalidation because we need to remeasure Grid which is not neccessarily dirty now
                            // since we only invlaidated scrollbars but we don't have LayoutUpdate loop at our disposal here
                            child.InvalidateMeasure();
                            child.Measure(constraint);
                        }
                    }
                }

                desiredSize = child.DesiredSize;
            }

            if (!ArrangeDirty && _invalidatedMeasureFromArrange)
            {
                // If we invalidated measure from a previous arrange pass, but
                // if after the following measure pass we are not dirty for
                // arrange, then ArrangeOverride will not get called, and we
                // need to clean up our state here.
                _invalidatedMeasureFromArrange = false;
            }

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            bool previouslyInvalidatedMeasureFromArrange = _invalidatedMeasureFromArrange;

            Size size = base.ArrangeOverride(arrangeSize);

            if (previouslyInvalidatedMeasureFromArrange)
            {
                // If we invalidated measure from a previous arrange pass,
                // then we are not supposed to invalidate measure this time.
                Debug.Assert(!MeasureDirty);
                _invalidatedMeasureFromArrange = false;
            }
            else
            {
                _invalidatedMeasureFromArrange = MeasureDirty;
            }

            return size;
        }

        internal void LineUp() => ScrollInfo?.LineUp();

        internal void LineDown() => ScrollInfo?.LineDown();

        internal void LineLeft() => ScrollInfo?.LineLeft();

        internal void LineRight() => ScrollInfo?.LineRight();

        internal void PageUp() => ScrollInfo?.PageUp();

        internal void PageDown() => ScrollInfo?.PageDown();

        internal void PageLeft() => ScrollInfo?.PageLeft();

        internal void PageRight() => ScrollInfo?.PageRight();

        private sealed class TouchInfo
        {
            public double X;
            public double Y;
            public double HorizontalOffset;
            public double VerticalOffset;
        }
    }
}
