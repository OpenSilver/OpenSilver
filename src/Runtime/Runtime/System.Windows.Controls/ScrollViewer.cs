
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
using System.Windows.Threading;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a scrollable area that can contain other visible elements.
    /// </summary>
    [TemplatePart(Name = ElementScrollContentPresenterName, Type = typeof(ScrollContentPresenter))]
    [TemplatePart(Name = ElementHorizontalScrollBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = ElementVerticalScrollBarName, Type = typeof(ScrollBar))]
    public class ScrollViewer : ContentControl
    {
        internal const double LineDelta = 16.0; // Default physical amount to scroll with one Up/Down/Left/Right key
        internal const double WheelDelta = 48.0; // Default physical amount to scroll with one MouseWheel.

        internal static bool IsPanning => PanHelper.IsPanning;

        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

        private readonly PanHelper _panHelper;

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

        // Event/infrastructure
        private EventHandler _layoutUpdatedHandler;
        private IScrollInfo _scrollInfo;

        private CommandQueue _queue;

        private bool _invalidatedMeasureFromArrange;

        static ScrollViewer()
        {
            EventManager.RegisterClassHandler<ScrollViewer>(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnTouchStartThunk), true);
            EventManager.RegisterClassHandler<ScrollViewer>(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnTouchEndThunk), true);
            EventManager.RegisterClassHandler<ScrollViewer>(MouseMoveEvent, new MouseEventHandler(OnTouchMoveThunk), true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewer"/> class.
        /// </summary>
        public ScrollViewer()
        {
            DefaultStyleKey = typeof(ScrollViewer);
            _panHelper = new PanHelper(this);
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
        public void ScrollToHorizontalOffset(double offset)
        {
            double validatedOffset = ScrollContentPresenter.ValidateInputOffset(offset, nameof(offset));

            // Queue up the scroll command, which tells the content to scroll.
            // Will lead to an update of all offsets (both live and deferred).
            EnqueueCommand(Commands.SetHorizontalOffset, validatedOffset);
        }

        /// <summary>
        /// Scrolls the content that is within the <see cref="ScrollViewer"/> to the 
        /// specified vertical offset position.
        /// </summary>
        /// <param name="offset">
        /// The position that the content scrolls to.
        /// </param>
        public void ScrollToVerticalOffset(double offset)
        {
            double validatedOffset = ScrollContentPresenter.ValidateInputOffset(offset, nameof(offset));

            // Queue up the scroll command, which tells the content to scroll.
            // Will lead to an update of all offsets (both live and deferred).
            EnqueueCommand(Commands.SetVerticalOffset, validatedOffset);
        }

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

        /// <summary>
        /// Identifies the <see cref="ScrollChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ScrollChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ScrollChanged),
                RoutingStrategy.Bubble,
                typeof(ScrollChangedEventHandler),
                typeof(ScrollViewer));

        /// <summary>
        /// Occurs when changes are detected to the scroll position, extent, or viewport size.
        /// </summary>
        public event ScrollChangedEventHandler ScrollChanged
        {
            add => AddHandler(ScrollChangedEvent, value);
            remove => RemoveHandler(ScrollChangedEvent, value);
        }

        /// <summary>
        /// Called when a change in scrolling state is detected, such as a change in scroll position, extent, or viewport size.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ScrollChangedEventArgs"/> that contain information about the change in the scrolling state.
        /// </param>
        protected virtual void OnScrollChanged(ScrollChangedEventArgs e) => RaiseEvent(e);

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
                        if (horizontal)
                            ScrollToHorizontalOffset(e.NewValue);
                        else
                            ScrollToVerticalOffset(e.NewValue);
                        break;
                    case ScrollEventType.LargeDecrement:
                        if (horizontal)
                            PageLeft();
                        else
                            PageUp();
                        break;
                    case ScrollEventType.LargeIncrement:
                        if (horizontal)
                            PageRight();
                        else
                            PageDown();
                        break;
                    case ScrollEventType.SmallDecrement:
                        if (horizontal)
                            LineLeft();
                        else
                            LineUp();
                        break;
                    case ScrollEventType.SmallIncrement:
                        if (horizontal)
                            LineRight();
                        else
                            LineDown();
                        break;
                    case ScrollEventType.First:
                        if (horizontal)
                            ScrollToLeftEnd();
                        else
                            ScrollToTop();
                        break;
                    case ScrollEventType.Last:
                        if (horizontal)
                            ScrollToRightEnd();
                        else
                            ScrollToBottom();
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
                bool fInvertForRTL = FlowDirection == FlowDirection.RightToLeft;

                switch (key)
                {
                    case Key.Up:
                        LineUp();
                        break;
                    case Key.Down:
                        LineDown();
                        break;
                    case Key.Left:
                        if (fInvertForRTL)
                        {
                            LineRight();
                        }
                        else
                        {
                            LineLeft();
                        }
                        break;
                    case Key.Right:
                        if (fInvertForRTL)
                        {
                            LineLeft();
                        }
                        else
                        {
                            LineRight();
                        }
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
            // anybody can call this method even if we don't have ISI...
            if (ScrollInfo is not IScrollInfo isi)
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
                EnsureLayoutUpdatedHandler();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && Focus())
            {
                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
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

        private static void OnTouchStartThunk(object sender, MouseButtonEventArgs e) => ((ScrollViewer)sender).OnTouchStart(e);

        private void OnTouchStart(MouseButtonEventArgs e) => _panHelper.HandleMouseLeftButtonDown(e);

        private static void OnTouchEndThunk(object sender, MouseButtonEventArgs e) => ((ScrollViewer)sender).OnTouchEnd(e);

        private void OnTouchEnd(MouseButtonEventArgs e) => _panHelper.HandleMouseLeftButtonUp(e);

        private static void OnTouchMoveThunk(object sender, MouseEventArgs e) => ((ScrollViewer)sender).OnTouchMove(e);

        private void OnTouchMove(MouseEventArgs e) => _panHelper.HandleMouseMove(e);

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
                        LineUp();
                        break;
                    case Key.Down:
                        LineDown();
                        break;
                    case Key.Left:
                        LineLeft();
                        break;
                    case Key.Right:
                        LineRight();
                        break;
                    case Key.PageUp:
                        PageUp();
                        break;
                    case Key.PageDown:
                        PageDown();
                        break;
                    case Key.Home:
                        if (!control)
                            ScrollToLeftEnd();
                        else
                            ScrollToTop();
                        break;
                    case Key.End:
                        if (!control)
                            ScrollToRightEnd();
                        else
                            ScrollToBottom();
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

        /// <summary>
        /// Measures the content of a <see cref="ScrollViewer"/> element.
        /// </summary>
        /// <param name="constraint">
        /// The upper limit <see cref="Size"/> that should not be exceeded.
        /// </param>
        /// <returns>
        /// The computed desired limit <see cref="Size"/> of the <see cref="ScrollViewer"/> element.
        /// </returns>
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

        /// <summary>
        /// Arranges the content of the <see cref="ScrollViewer"/>.
        /// </summary>
        /// <param name="arrangeSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
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

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content upward by one line.
        /// </summary>
        public void LineUp() => EnqueueCommand(Commands.LineUp, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content downward by one line.
        /// </summary>
        public void LineDown() => EnqueueCommand(Commands.LineDown, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content to the left by a predetermined amount.
        /// </summary>
        public void LineLeft() => EnqueueCommand(Commands.LineLeft, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content to the right by a predetermined amount.
        /// </summary>
        public void LineRight() => EnqueueCommand(Commands.LineRight, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content upward by one page.
        /// </summary>
        public void PageUp() => EnqueueCommand(Commands.PageUp, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content downward by one page.
        /// </summary>
        public void PageDown() => EnqueueCommand(Commands.PageDown, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content to the left by one page.
        /// </summary>
        public void PageLeft() => EnqueueCommand(Commands.PageLeft, 0);

        /// <summary>
        /// Scrolls the <see cref="ScrollViewer"/> content to the right by one page.
        /// </summary>
        public void PageRight() => EnqueueCommand(Commands.PageRight, 0);

        /// <summary>
        /// Scrolls horizontally to the beginning of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToLeftEnd() => EnqueueCommand(Commands.SetHorizontalOffset, double.NegativeInfinity);

        /// <summary>
        /// Scrolls horizontally to the end of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToRightEnd() => EnqueueCommand(Commands.SetHorizontalOffset, double.PositiveInfinity);

        /// <summary>
        /// Scrolls to the beginning of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToHome()
        {
            EnqueueCommand(Commands.SetHorizontalOffset, double.NegativeInfinity);
            EnqueueCommand(Commands.SetVerticalOffset, double.NegativeInfinity);
        }

        /// <summary>
        /// Scrolls to the end of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToEnd()
        {
            EnqueueCommand(Commands.SetHorizontalOffset, double.NegativeInfinity);
            EnqueueCommand(Commands.SetVerticalOffset, double.PositiveInfinity);
        }

        /// <summary>
        /// Scrolls vertically to the beginning of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToTop() => EnqueueCommand(Commands.SetVerticalOffset, double.NegativeInfinity);

        /// <summary>
        /// Scrolls vertically to the end of the <see cref="ScrollViewer"/> content.
        /// </summary>
        public void ScrollToBottom() => EnqueueCommand(Commands.SetVerticalOffset, double.PositiveInfinity);

        //returns true if there was a command sent to ISI
        private bool ExecuteNextCommand()
        {
            IScrollInfo isi = ScrollInfo;
            if (isi is null) return false;

            Command cmd = _queue.Fetch();
            switch (cmd.Code)
            {
                case Commands.LineUp: isi.LineUp(); break;
                case Commands.LineDown: isi.LineDown(); break;
                case Commands.LineLeft: isi.LineLeft(); break;
                case Commands.LineRight: isi.LineRight(); break;

                case Commands.PageUp: isi.PageUp(); break;
                case Commands.PageDown: isi.PageDown(); break;
                case Commands.PageLeft: isi.PageLeft(); break;
                case Commands.PageRight: isi.PageRight(); break;

                case Commands.SetHorizontalOffset: isi.SetHorizontalOffset(cmd.Param); break;
                case Commands.SetVerticalOffset: isi.SetVerticalOffset(cmd.Param); break;

                case Commands.Invalid: return false;
            }
            return true;
        }

        private void EnqueueCommand(Commands code, double param)
        {
            _queue.Enqueue(new Command(code, param));
            EnsureQueueProcessing();
        }

        private void EnsureQueueProcessing()
        {
            if (!_queue.IsEmpty())
            {
                EnsureLayoutUpdatedHandler();
            }
        }

        // LayoutUpdated event handler.
        // 1. executes next queued command, if any
        // 2. If no commands to execute, updates properties and fires events
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            // if there was a command, execute it and leave the handler for the next pass
            if (ExecuteNextCommand())
            {
                InvalidateArrange();
                return;
            }

            if (ScrollInfo is IScrollInfo isi)
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

                    // Fire ScrollChange event
                    var args = new ScrollChangedEventArgs(
                        new Vector(HorizontalOffset, VerticalOffset),
                        new Vector(HorizontalOffset - oldActualHorizontalOffset, VerticalOffset - oldActualVerticalOffset),
                        new Size(ExtentWidth, ExtentHeight),
                        new Vector(ExtentWidth - oldExtentWidth, ExtentHeight - oldExtentHeight),
                        new Size(ViewportWidth, ViewportHeight),
                        new Vector(ViewportWidth - oldViewportWidth, ViewportHeight - oldViewportHeight))
                    {
                        RoutedEvent = ScrollChangedEvent,
                        Source = this,
                    };

                    try
                    {
                        OnScrollChanged(args);
                    }
                    finally
                    {
                        //
                        // Disconnect the layout listener.
                        //
                        ClearLayoutUpdatedHandler();
                    }

                    return;
                }
            }

            ClearLayoutUpdatedHandler();
        }

        private void EnsureLayoutUpdatedHandler()
        {
            if (_layoutUpdatedHandler is null)
            {
                _layoutUpdatedHandler = new EventHandler(OnLayoutUpdated);
                LayoutUpdated += _layoutUpdatedHandler;
            }
            InvalidateArrange(); //can be that there is no outstanding need to do layout - make sure it is.
        }

        private void ClearLayoutUpdatedHandler()
        {
            // If queue is not empty - then we still need that handler to make sure queue is being processed.
            if (_layoutUpdatedHandler is not null && _queue.IsEmpty())
            {
                LayoutUpdated -= _layoutUpdatedHandler;
                _layoutUpdatedHandler = null;
            }
        }

        private enum Commands
        {
            Invalid,
            LineUp,
            LineDown,
            LineLeft,
            LineRight,
            PageUp,
            PageDown,
            PageLeft,
            PageRight,
            SetHorizontalOffset,
            SetVerticalOffset,
        }

        private struct Command
        {
            internal Command(Commands code, double param)
            {
                Code = code;
                Param = param;
            }

            internal Commands Code;
            internal double Param;
        }

        // implements ring buffer of commands
        private struct CommandQueue
        {
            private const int _capacity = 32;

            //returns false if capacity is used up and entry ignored
            internal void Enqueue(Command command)
            {
                if (_lastWritePosition == _lastReadPosition) //buffer is empty
                {
                    _array = new Command[_capacity];
                    _lastWritePosition = _lastReadPosition = 0;
                }

                if (!OptimizeCommand(command)) //regular insertion, if optimization didn't happen
                {
                    _lastWritePosition = (_lastWritePosition + 1) % _capacity;

                    if (_lastWritePosition == _lastReadPosition) //buffer is full
                    {
                        // throw away the oldest entry and continue to accumulate fresh input
                        _lastReadPosition = (_lastReadPosition + 1) % _capacity;
                    }

                    _array[_lastWritePosition] = command;
                }
            }

            // this tries to "merge" the incoming command with the accumulated queue
            // for example, if we get SetHorizontalOffset incoming, all "horizontal"
            // commands in the queue get removed and replaced with incoming one,
            // since horizontal position is going to end up at the specified offset anyways.
            private bool OptimizeCommand(Command command)
            {
                if (_lastWritePosition != _lastReadPosition) //buffer has something
                {
                    if ((command.Code == Commands.SetHorizontalOffset && _array[_lastWritePosition].Code == Commands.SetHorizontalOffset) ||
                        (command.Code == Commands.SetVerticalOffset && _array[_lastWritePosition].Code == Commands.SetVerticalOffset))
                    {
                        //if the last command was "set offset" or "make visible", simply replace it and
                        //don't insert new command
                        _array[_lastWritePosition].Param = command.Param;
                        return true;
                    }
                }
                return false;
            }

            // returns Invalid command if there is no more commands
            internal Command Fetch()
            {
                if (_lastWritePosition == _lastReadPosition) //buffer is empty
                {
                    return new Command(Commands.Invalid, 0);
                }
                _lastReadPosition = (_lastReadPosition + 1) % _capacity;

                //array exists always if writePos != readPos
                Command command = _array[_lastReadPosition];

                if (_lastWritePosition == _lastReadPosition) //it was the last command
                {
                    _array = null; // make GC work. Hopefully the whole queue is processed in Gen0
                }
                return command;
            }

            internal bool IsEmpty() => _lastWritePosition == _lastReadPosition;

            private int _lastWritePosition;
            private int _lastReadPosition;
            private Command[] _array;
        }

        private sealed class PanHelper
        {
            private static PanHelper _current;
            private static readonly DispatcherTimer _recoveryTimer;

            private const double MaxInactivityPeriodMS = 3 * 1000;
            private const double RecoveryTimerIntervalMS = 2.0 / 3.0 * MaxInactivityPeriodMS;
            private const double MaxWaitForInertiaMS = 50;
            private const double MinScrollDelta = 5;
            private const double DecelerationRatio = 0.97;
            private const double VelocityThreshold = 0.5;
            private const double InertiaTimerIntervalMS = 1000.0 / 60;

            static PanHelper()
            {
                _recoveryTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(RecoveryTimerIntervalMS),
                };

                _recoveryTimer.Tick += new EventHandler(OnRecoverTimerTick);
            }

            public static bool IsPanning => _current?._isPanning ?? false;

            private static void OnRecoverTimerTick(object sender, EventArgs e)
            {
                if (_current is not PanHelper current)
                {
                    return;
                }

                if ((GetTime() - current._lastMoveTime).TotalMilliseconds > MaxInactivityPeriodMS)
                {
                    current.Cancel();
                }
            }

            private static void SetCurrent(PanHelper panHelper)
            {
                _current = panHelper;
                _recoveryTimer.IsEnabled = panHelper is not null;
            }

            private static DateTime GetTime() => DateTime.UtcNow;

            private readonly ScrollViewer _scrollViewer;
            private bool _isPanning;

            private Point _originPosition;
            private Point _previousPosition;
            private double _horizontalOffset;
            private double _verticalOffset;
            private double _velocityX;
            private double _velocityY;
            private DateTime _lastMoveTime;
            private DispatcherTimer _inertiaTimer;

            private bool IsHorizontalScrollBarVisible => _scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
            private bool IsVerticalScrollBarVisible => _scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible;
            private bool IsEnabled => this == _current;

            public PanHelper(ScrollViewer scrollViewer)
            {
                Debug.Assert(scrollViewer is not null);
                _scrollViewer = scrollViewer;
                _scrollViewer.AddHandler(UnloadedEvent, new RoutedEventHandler(OnUnloaded), true);
            }

            public void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
            {
                Cancel();

                if (e.IsTouchEvent &&
                    Pointer.Captured is null &&
                    (IsVerticalScrollBarVisible || IsHorizontalScrollBarVisible) &&
                    _scrollViewer.ScrollInfo is IScrollInfo isi &&
                    _current is null) // prevents scrolling multiple nested ScrollViewers
                {
                    SetCurrent(this);

                    _originPosition = _previousPosition = e.GetPosition(_scrollViewer);
                    _horizontalOffset = isi.HorizontalOffset;
                    _verticalOffset = isi.VerticalOffset;
                    _velocityX = 0;
                    _velocityY = 0;
                    _lastMoveTime = GetTime();
                }
            }

            public void HandleMouseMove(MouseEventArgs e)
            {
                if (!IsEnabled)
                {
                    return;
                }

                _lastMoveTime = GetTime();

                Point position = e.GetPosition(_scrollViewer);

                if (!_isPanning)
                {
                    _isPanning = Math.Abs(position.X - _originPosition.X) > MinScrollDelta ||
                                 Math.Abs(position.Y - _originPosition.Y) > MinScrollDelta;
                }

                if (_isPanning)
                {
                    Scroll(position);
                }
            }

            public void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
            {
                if (!IsEnabled)
                {
                    return;
                }

                Cancel();

                if ((GetTime() - _lastMoveTime).TotalMilliseconds < MaxWaitForInertiaMS && CanScroll(_velocityX, _velocityY))
                {
                    StartScrollingInertia();
                }
            }

            private void Scroll(Point position)
            {
                if (IsHorizontalScrollBarVisible)
                {
                    double deltaX = _previousPosition.X - position.X;
                    _velocityX = deltaX;
                    _horizontalOffset += deltaX;
                    _scrollViewer.ScrollToHorizontalOffset(_horizontalOffset);
                }

                if (IsVerticalScrollBarVisible)
                {
                    double deltaY = _previousPosition.Y - position.Y;
                    _velocityY = deltaY;
                    _verticalOffset += deltaY;
                    _scrollViewer.ScrollToVerticalOffset(_verticalOffset);
                }

                _previousPosition = position;
            }

            private void StartScrollingInertia()
            {
                if (_inertiaTimer is null)
                {
                    _inertiaTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(InertiaTimerIntervalMS),
                    };

                    _inertiaTimer.Tick += new EventHandler(OnInertiaTimerTick);
                }

                _inertiaTimer.Tag = new ScrollData
                {
                    HorizontalOffset = _horizontalOffset,
                    VerticalOffset = _verticalOffset,
                    VelocityX = _velocityX,
                    VelocityY = _velocityY,
                };

                _inertiaTimer.Start();
            }

            private void OnInertiaTimerTick(object sender, EventArgs e)
            {
                var timer = (DispatcherTimer)sender;
                var scrollData = (ScrollData)timer.Tag;

                if (IsHorizontalScrollBarVisible)
                {
                    scrollData.HorizontalOffset += scrollData.VelocityX;
                    _scrollViewer.ScrollToHorizontalOffset(scrollData.HorizontalOffset);
                    scrollData.VelocityX *= DecelerationRatio;
                }

                if (IsVerticalScrollBarVisible)
                {
                    scrollData.VerticalOffset += scrollData.VelocityY;
                    _scrollViewer.ScrollToVerticalOffset(scrollData.VerticalOffset);
                    scrollData.VelocityY *= DecelerationRatio;
                }

                if (!CanScroll(scrollData.VelocityX, scrollData.VelocityY))
                {
                    timer.Stop();
                }
            }

            private void OnUnloaded(object sender, RoutedEventArgs e) => Cancel();

            private void Cancel()
            {
                if (IsEnabled)
                {
                    SetCurrent(null);
                }

                _isPanning = false;
                _inertiaTimer?.Stop();
            }

            private bool CanScroll(double velocityX, double velocityY)
            {
                bool canScrollH = Math.Abs(velocityX) >= VelocityThreshold && IsHorizontalScrollBarVisible;
                bool canScrollV = Math.Abs(velocityY) >= VelocityThreshold && IsVerticalScrollBarVisible;

                return canScrollH || canScrollV;
            }

            private sealed class ScrollData
            {
                public double HorizontalOffset;
                public double VerticalOffset;
                public double VelocityX;
                public double VelocityY;
            }
        }
    }
}
