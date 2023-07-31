
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

using System;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a scrollable area that can contain other visible elements.
    /// </summary>
    [TemplatePart(Name = ElementScrollContentPresenterName, Type = typeof(ScrollContentPresenter))]
    [TemplatePart(Name = ElementHorizontalScrollBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = ElementVerticalScrollBarName, Type = typeof(ScrollBar))]
    public sealed class ScrollViewer : ContentControl
    {
        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

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

        internal IScrollInfo ScrollInfo { get; set; }

        /// <summary>
        /// Gets a value that contains the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// The horizontal offset of the scrolled content. The default value is 0.0.
        /// </returns>
        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            private set => SetValue(HorizontalOffsetPropertyKey, value);
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
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
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
                scrollViewer.UpdateScrollbarVisibility();
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
            get => (double)GetValue(VerticalOffsetProperty);
            private set => SetValue(VerticalOffsetPropertyKey, value);
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
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
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

            element.SetValue(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
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

            element.SetValue(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
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

            UpdateScrollbarVisibility();
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

                UpdateScrollBar(orientation, clamped);
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
        public double ScrollableHeight
        {
            get => (double)GetValue(ScrollableHeightProperty);
            private set => SetValue(ScrollableHeightPropertyKey, value);
        }

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
        public double ScrollableWidth
        {
            get => (double)GetValue(ScrollableWidthProperty);
            private set => SetValue(ScrollableWidthPropertyKey, value);
        }

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
        public double ViewportHeight
        {
            get => (double)GetValue(ViewportHeightProperty);
            private set => SetValue(ViewportHeightPropertyKey, value);
        }

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
        public double ViewportWidth
        {
            get => (double)GetValue(ViewportWidthProperty);
            private set => SetValue(ViewportWidthPropertyKey, value);
        }

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
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get => (Visibility)GetValue(ComputedHorizontalScrollBarVisibilityProperty);
            private set => SetValue(ComputedHorizontalScrollBarVisibilityPropertyKey, value);
        }

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
        public Visibility ComputedVerticalScrollBarVisibility
        {
            get => (Visibility)GetValue(ComputedVerticalScrollBarVisibilityProperty);
            private set => SetValue(ComputedVerticalScrollBarVisibilityPropertyKey, value);
        }

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
        public double ExtentHeight
        {
            get => (double)GetValue(ExtentHeightProperty);
            private set => SetValue(ExtentHeightPropertyKey, value);
        }

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
        public double ExtentWidth
        {
            get => (double)GetValue(ExtentWidthProperty);
            private set => SetValue(ExtentWidthPropertyKey, value);
        }

        /// <summary>
        /// Called when the value of properties that describe the size and location of the scroll area change.
        /// </summary>
        public void InvalidateScrollInfo()
        {
            if (ScrollInfo is not null)
            {
                // ScrollBar visibility has to be updated before ViewportHeight/Width, because
                // that will trigger ScrollBar control sizing which depends on ScrollBar ActualWidth/Height
                UpdateScrollbarVisibility();

                ExtentHeight = ScrollInfo.ExtentHeight;
                ExtentWidth = ScrollInfo.ExtentWidth;
                ViewportHeight = ScrollInfo.ViewportHeight;
                ViewportWidth = ScrollInfo.ViewportWidth;
                UpdateScrollBar(Orientation.Horizontal, ScrollInfo.HorizontalOffset);
                UpdateScrollBar(Orientation.Vertical, ScrollInfo.VerticalOffset);
            }

            if (Math.Max(0, ExtentHeight - ViewportHeight) != ScrollableHeight)
            {
                ScrollableHeight = Math.Max(0, ExtentHeight - ViewportHeight);
                InvalidateMeasure();
            }
            if (Math.Max(0, ExtentWidth - ViewportWidth) != ScrollableWidth)
            {
                ScrollableWidth = Math.Max(0, ExtentWidth - ViewportWidth);
                InvalidateMeasure();
            }
        }

        private void UpdateScrollbarVisibility()
        {
            // Update horizontal ScrollBar 
            Visibility horizontalVisibility = HorizontalScrollBarVisibility switch
            {
                ScrollBarVisibility.Visible => Visibility.Visible,
                ScrollBarVisibility.Disabled or ScrollBarVisibility.Hidden => Visibility.Collapsed,
                _ => (ScrollInfo is null || ScrollInfo.ExtentWidth <= ScrollInfo.ViewportWidth) ? Visibility.Collapsed : Visibility.Visible,
            };

            if (horizontalVisibility != ComputedHorizontalScrollBarVisibility)
            {
                ComputedHorizontalScrollBarVisibility = horizontalVisibility;
                InvalidateMeasure();
            }

            // Update vertical ScrollBar
            Visibility verticalVisibility = VerticalScrollBarVisibility switch
            {
                ScrollBarVisibility.Visible => Visibility.Visible,
                ScrollBarVisibility.Disabled or ScrollBarVisibility.Hidden => Visibility.Collapsed,
                _ => (ScrollInfo is null || ScrollInfo.ExtentHeight <= ScrollInfo.ViewportHeight) ? Visibility.Collapsed : Visibility.Visible,
            };

            if (verticalVisibility != ComputedVerticalScrollBarVisibility)
            {
                ComputedVerticalScrollBarVisibility = verticalVisibility;
                InvalidateMeasure();
            }
        }

        private void UpdateScrollBar(Orientation orientation, double value)
        {
            // Update relevant ScrollBar
            if (orientation == Orientation.Horizontal)
            {
                HorizontalOffset = value;
                if (ElementHorizontalScrollBar is not null)
                {
                    ElementHorizontalScrollBar.Value = value;
                }
            }
            else
            {
                VerticalOffset = value;
                if (ElementVerticalScrollBar is not null)
                {
                    ElementVerticalScrollBar.Value = value;
                }
            }
        }

#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(MouseButtonEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif

            if (!e.Handled && Focus())
            {
                e.Handled = true;
            }
        }

#if MIGRATION
        protected override void OnMouseWheel(MouseWheelEventArgs e)
#else
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseWheel(e);
#else
            base.OnPointerWheelChanged(e);
#endif

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
    }
}
