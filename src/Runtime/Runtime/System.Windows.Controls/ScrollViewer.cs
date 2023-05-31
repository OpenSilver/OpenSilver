﻿
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
using CSHTML5.Internal;

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
    public sealed partial class ScrollViewer : ContentControl
    {
        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

        double _verticalOffset = 0;
        double _horizontalOffset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewer"/> class.
        /// </summary>
        public ScrollViewer()
        {
            DefaultStyleKey = typeof(ScrollViewer);

            IsVisibleChanged += ScrollViewer_IsVisibleChanged;
        }

        private void ScrollViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                UpdateDomHorizontalOffset(this.INTERNAL_OuterDomElement);
                UpdateDomVerticalOffset(this.INTERNAL_OuterDomElement);
            }
        }

        internal override FrameworkTemplate TemplateCache
        {
            get
            {
                if (IsCustomLayoutRoot || IsUnderCustomLayout)
                {
                    return base.TemplateCache;
                }

                return null;
            }
            set
            {
                base.TemplateCache = value;
            }
        }

        internal override FrameworkTemplate TemplateInternal
        {
            get
            {
                if (IsCustomLayoutRoot || IsUnderCustomLayout)
                {
                    return base.TemplateInternal;
                }

                return null;
            }
        }

        // Note: we need to force this to true because template are disabled
        // for ScrollViewers.
        internal override bool EnablePointerEventsCore
        {
            get
            {
                return true;
            }
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
            get; set;
        }

        /// <summary>
        /// Gets a value that contains the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>The horizontal offset of the scrolled content. The default value is 0.0.</returns>
        public double HorizontalOffset
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(HorizontalOffsetProperty);
                }

                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    _horizontalOffset = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollLeft");
                }
                return _horizontalOffset;
            }
            private set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                nameof(HorizontalOffset),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0, OnScrollInfoDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal <see cref="ScrollBar"/> should be displayed.
        /// </summary>
        /// <returns>A <see cref="ScrollBarVisibility"/> value that indicates whether a horizontal <see cref="ScrollBar"/> should be displayed. The default value is <see cref="ScrollBarVisibility.Hidden"/>.</returns>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached(
                nameof(HorizontalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(ScrollViewer),
                new PropertyMetadata(ScrollBarVisibility.Disabled, OnScrollBarVisibilityChanged)
                {
                    MethodToUpdateDom2 = UpdateDomOnHSBVisibilityChanged,
                });

        private static void OnScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.InvalidateMeasure();
                if (scrollViewer.ScrollInfo != null)
                {
                    scrollViewer.ScrollInfo.CanHorizontallyScroll = scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    scrollViewer.ScrollInfo.CanVerticallyScroll = scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                scrollViewer.UpdateScrollbarVisibility();
            }
        }

        private static void UpdateDomOnHSBVisibilityChanged(DependencyObject d, object oldValue, object newValue)
        {
            if (d is ScrollViewer sv && !sv.IsUnderCustomLayout)
            {
                sv.ApplyHorizontalSettings(
                    (ScrollBarVisibility)newValue,
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(sv.INTERNAL_OuterDomElement),
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(sv.INTERNAL_InnerDomElement));
            }
        }

        private static void OnScrollInfoDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets a value that contains the vertical offset of the scrolled content.
        /// </summary>
        /// <returns>The vertical offset of the scrolled content. The default value is 0.0.</returns>
        public double VerticalOffset
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(VerticalOffsetProperty);
                }

                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    _verticalOffset = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollTop");
                }

                return _verticalOffset;
            }
            private set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                nameof(VerticalOffset),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0, OnScrollInfoDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether a vertical <see cref="ScrollBar"/> should be displayed.
        /// </summary>
        /// <returns>A <see cref="ScrollBarVisibility"/> value that indicates whether a vertical <see cref="ScrollBar"/> should be displayed. The default value is <see cref="ScrollBarVisibility.Visible"/>.</returns>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        }

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached(
                nameof(VerticalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(ScrollViewer),
                new PropertyMetadata(ScrollBarVisibility.Visible, OnScrollBarVisibilityChanged)
                {
                    MethodToUpdateDom2 = UpdateDomOnVSBVisibilityChanged
                });

        private static void UpdateDomOnVSBVisibilityChanged(DependencyObject d, object oldValue, object newValue)
        {
            if (d is ScrollViewer sv && !sv.IsUnderCustomLayout)
            {
                sv.ApplyVerticalSettings(
                    (ScrollBarVisibility)newValue,
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(sv.INTERNAL_OuterDomElement),
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(sv.INTERNAL_InnerDomElement));
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out object outerDiv);
            outerDivStyle.height = "100%";
            outerDivStyle.width = "100%";

            if (!IsCustomLayoutRoot && !IsUnderCustomLayout)
            {
                outerDivStyle.overflowX = "scroll";
                outerDivStyle.overflowY = "scroll";
            }

            //Update the scrollviewer position when we insert again the scrollviewer in the visual tree
            if (_verticalOffset != 0)
            {
                UpdateDomVerticalOffset(outerDiv);
            }
            if (_horizontalOffset != 0)
            {
                UpdateDomHorizontalOffset(outerDiv);
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out object innerDiv);
            innerDivStyle.display = "grid";

            ApplyHorizontalSettings(HorizontalScrollBarVisibility, outerDivStyle, innerDivStyle);
            ApplyVerticalSettings(VerticalScrollBarVisibility, outerDivStyle, innerDivStyle);

            domElementWhereToPlaceChildren = innerDiv;
            return outerDiv;
        }

        private void ApplyHorizontalSettings(
            ScrollBarVisibility horizontalScrollBarVisibility,
            INTERNAL_HtmlDomStyleReference outerDivStyle,
            INTERNAL_HtmlDomStyleReference innerDivStyle)
        {
            // if it's under customlayout, it works with Measure & Arrange.
            if (IsCustomLayoutRoot || IsUnderCustomLayout)
                return;

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //-------------------------------------
                // Handle the "Overflow X" CSS property:
                //-------------------------------------

                switch (horizontalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        outerDivStyle.overflowX = "hidden";
                        innerDivStyle.width = "100%";
                        innerDivStyle.minWidth = string.Empty;
                        break;

                    case ScrollBarVisibility.Auto:
                        outerDivStyle.overflowX = "auto";
                        innerDivStyle.width = "fit-content";
                        innerDivStyle.minWidth = "100%";
                        break;

                    case ScrollBarVisibility.Hidden:
                        outerDivStyle.overflowX = "hidden";
                        innerDivStyle.width = "100%";
                        innerDivStyle.minWidth = string.Empty;
                        break;

                    case ScrollBarVisibility.Visible:
                        outerDivStyle.overflowX = "scroll";
                        innerDivStyle.width = "fit-content";
                        innerDivStyle.minWidth = "100%";
                        break;

                    default:
                        break;
                }
            }
        }

        private void ApplyVerticalSettings(
            ScrollBarVisibility verticalScrollBarVisibility,
            INTERNAL_HtmlDomStyleReference outerDivStyle,
            INTERNAL_HtmlDomStyleReference innerDivStyle)
        {
            // if it's under customlayout, it works with Measure & Arrange.
            if (IsCustomLayoutRoot || IsUnderCustomLayout)
                return;

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //-------------------------------------
                // Handle the "Overflow Y" CSS property:
                //-------------------------------------

                switch (verticalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        outerDivStyle.overflowY = "hidden";
                        innerDivStyle.height = "100%";
                        innerDivStyle.minHeight = string.Empty;
                        break;

                    case ScrollBarVisibility.Auto:
                        outerDivStyle.overflowY = "auto";
                        innerDivStyle.height = "fit-content";
                        innerDivStyle.minHeight = "100%";
                        break;

                    case ScrollBarVisibility.Hidden:
                        outerDivStyle.overflowY = "hidden";
                        innerDivStyle.height = "100%";
                        innerDivStyle.minHeight = string.Empty;
                        break;

                    case ScrollBarVisibility.Visible:
                        outerDivStyle.overflowY = "scroll";
                        innerDivStyle.height = "fit-content";
                        innerDivStyle.minHeight = "100%";
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Scrolls the content that is within the <see cref="ScrollViewer"/> to the specified horizontal offset position.
        /// </summary>
        /// <param name="offset">The position that the content scrolls to.</param>
        public void ScrollToHorizontalOffset(double offset)
        {
            //_horizontalOffset is there so we can remember changes of the value even if the ScrollViewer is not in the visual tree
            _horizontalOffset = offset;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                UpdateDomHorizontalOffset(INTERNAL_OuterDomElement);
            }

            SetScrollOffset(Orientation.Horizontal, offset);
        }

        /// <summary>
        /// If the ScrollViewer is Visible, sets the scrollLeft property to the value held in _horizontalOffset.
        /// If the ScrollViewer is Collapsed, it will set _isHorizontalOffsetInvalid to true,
        /// so that we know the current value of this.INTERNAL_OuterDomElement.scrollLeft is invalid and needs to be set when changing the visibility.
        /// </summary>
        /// <param name="div">The dom element on which to set the scrollLeft (normally the ScrollViewer's OuterDomElement)</param>
        internal void UpdateDomHorizontalOffset(object div)
        {
            if (div != null && Visibility == Visibility.Visible)
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(div, "scrollLeft", _horizontalOffset);
            }
        }

        /// <summary>
        /// Scrolls the content that is within the <see cref="ScrollViewer"/> to the specified vertical offset position.
        /// </summary>
        /// <param name="offset">The position that the content scrolls to.</param>
        public void ScrollToVerticalOffset(double offset)
        {
            //_verticalOffset is there so we can remember changes of the value even if the ScrollViewer is not in the visual tree
            _verticalOffset = offset;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                UpdateDomVerticalOffset(INTERNAL_OuterDomElement);
            }

            SetScrollOffset(Orientation.Vertical, offset);
        }

        /// <summary>
        /// If the ScrollViewer is Visible, sets the scrollTop property to the value held in _verticalOffset.
        /// If the ScrollViewer is Collapsed, it will set _isVerticalOffsetInvalid to true,
        /// so that we know the current value of this.INTERNAL_OuterDomElement.scrollTop is invalid and needs to be set when changing the visibility.
        /// </summary>
        /// <param name="div">The dom element on which to set the scrollTop (normally the ScrollViewer's OuterDomElement)</param>
        internal void UpdateDomVerticalOffset(object div)
        {
            if (div != null && Visibility == Visibility.Visible)
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(div, "scrollTop", _verticalOffset);
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="HorizontalScrollBarVisibility"/> dependency property from a specified element.
        /// </summary>
        /// <returns>The value of the <see cref="HorizontalScrollBarVisibility"/> dependency property.</returns>
        /// <param name="element">The element from which the property value is read.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="element"/> is null.</exception>
        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (ScrollBarVisibility)element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HorizontalScrollBarVisibility"/> dependency property to a specified element.
        /// </summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="horizontalScrollBarVisibility">The property value to set.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="element"/> is null.</exception>
        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
        }

        /// <summary>
        /// Gets the value of the <see cref="VerticalScrollBarVisibility"/> dependency property from a specified element.
        /// </summary>
        /// <returns>The value of the <see cref="VerticalScrollBarVisibility"/> dependency property.</returns>
        /// <param name="element">The element from which the property value is read.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="element"/> is null.</exception>
        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="VerticalScrollBarVisibility"/> dependency property to a specified element.
        /// </summary>
        /// <param name="element">The element on which to set the property value.</param>
        /// <param name="verticalScrollBarVisibility">The property value to set.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="element"/> is null.</exception>
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

            if (!this.IsCustomLayoutRoot && !this.IsUnderCustomLayout)
                return;

            ElementScrollContentPresenter = GetTemplateChild(ElementScrollContentPresenterName) as ScrollContentPresenter;
            ElementHorizontalScrollBar = GetTemplateChild(ElementHorizontalScrollBarName) as ScrollBar;
            ElementVerticalScrollBar = GetTemplateChild(ElementVerticalScrollBarName) as ScrollBar;

            if (null != ElementHorizontalScrollBar)
            {
                ElementHorizontalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Horizontal, e); };
            }
            if (null != ElementVerticalScrollBar)
            {
                ElementVerticalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Vertical, e); };
            }
            UpdateScrollbarVisibility();
        }

        void SetScrollOffset(Orientation orientation, double value)
        {
            if (ScrollInfo != null)
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
            if (ScrollInfo != null)
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
            if (ScrollInfo != null)
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

        /// <summary>
        /// Identifies the <see cref="ScrollableHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.Register(
                nameof(ScrollableHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets a value that represents the vertical size of the area that can be scrolled;
        /// the difference between the height of the extent and the height of the viewport.
        /// </summary>
        /// <returns>
        /// The vertical size of the area that can be scrolled. This property has no default value.
        /// </returns>
        public double ScrollableHeight
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ScrollableHeightProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollHeight - {sDomElement}.clientHeight");
                }

                return 0.0;
            }
            private set { SetValue(ScrollableHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ScrollableWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register(
                nameof(ScrollableWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets a value that represents the horizontal size of the area that can be scrolled;
        /// the difference between the width of the extent and the width of the viewport.
        /// </summary>
        /// <returns>
        /// The horizontal size of the area that can be scrolled. This property has no default value.
        /// </returns>
        public double ScrollableWidth
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ScrollableWidthProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollWidth - {sDomElement}.clientWidth");
                }

                return 0.0;
            }
            private set { SetValue(ScrollableWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ViewportHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register(
                nameof(ViewportHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets a value that contains the vertical size of the viewable content.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewable content. This property has no default value.
        /// </returns>
        public double ViewportHeight
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ViewportHeightProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.clientHeight");
                }

                return 0.0;
            }
            private set { SetValue(ViewportHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ViewportWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register(
                nameof(ViewportWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets a value that contains the horizontal size of the viewable content.
        /// </summary>
        /// <returns>The horizontal size of the viewable content. The default value is 0.0.</returns>
        public double ViewportWidth
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ViewportWidthProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.clientWidth");
                }

                return 0.0;
            }
            private set { SetValue(ViewportWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ComputedHorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ComputedHorizontalScrollBarVisibility"/> dependency property.</returns>
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), null);

        /// <summary>
        /// Gets a value that indicates whether the horizontal <see cref="ScrollBar"/> is visible.
        /// </summary>
        /// <returns>A <see cref="Visibility"/> that indicates whether the horizontal scroll bar is visible. The default value is <see cref="Visibility.Visible"/>.</returns>
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedHorizontalScrollBarVisibilityProperty); }
            private set { this.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ComputedVerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ComputedVerticalScrollBarVisibility"/> dependency property.</returns>
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), null);

        /// <summary>
        /// Gets a value that indicates whether the vertical <see cref="ScrollBar"/> is visible.
        /// </summary>
        /// <returns>A <see cref="Visibility"/> that indicates whether the vertical scroll bar is visible. The default value is <see cref="Visibility.Visible"/>.</returns>
        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedVerticalScrollBarVisibilityProperty); }
            private set { this.SetValue(ComputedVerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="ExtentHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register(
                nameof(ExtentHeight),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the vertical size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </summary>
        /// <returns>
        /// The vertical size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </returns>
        public double ExtentHeight
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ExtentHeightProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollHeight");
                }

                return 0.0;
            }
            private set { SetValue(ExtentHeightProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="ExtentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register(
                nameof(ExtentWidth),
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the horizontal size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </summary>
        /// <returns>
        /// The horizontal size of all the content for display in the <see cref="ScrollViewer"/>.
        /// </returns>
        public double ExtentWidth
        {
            get
            {
                if (UseCustomLayout)
                {
                    return (double)GetValue(ExtentWidthProperty);
                }

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
                    return OpenSilver.Interop.ExecuteJavaScriptDouble($"{sDomElement}.scrollWidth");
                }

                return 0.0;
            }
            private set { SetValue(ExtentWidthProperty, value); }
        }

        /// <summary>
        /// Called when the value of properties that describe the size and location of the scroll area change.
        /// </summary>
        public void InvalidateScrollInfo()
        {
            if (ScrollInfo != null)
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

        void UpdateScrollbarVisibility()
        {
            // Update horizontal ScrollBar 
            Visibility horizontalVisibility;
            switch (HorizontalScrollBarVisibility)
            {
                case ScrollBarVisibility.Visible:
                    horizontalVisibility = Visibility.Visible;
                    break;
                case ScrollBarVisibility.Disabled:
                case ScrollBarVisibility.Hidden:
                    horizontalVisibility = Visibility.Collapsed;
                    break;
                default:  // Avoids compiler warning about uninitialized variable
                case ScrollBarVisibility.Auto:
                    horizontalVisibility = ScrollInfo == null || ScrollInfo.ExtentWidth <= ScrollInfo.ViewportWidth ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

            if (horizontalVisibility != ComputedHorizontalScrollBarVisibility)
            {
                ComputedHorizontalScrollBarVisibility = horizontalVisibility;
                InvalidateMeasure();
            }
            // Update vertical ScrollBar
            Visibility verticalVisibility;
            switch (VerticalScrollBarVisibility)
            {
                case ScrollBarVisibility.Visible:
                    verticalVisibility = Visibility.Visible;
                    break;
                case ScrollBarVisibility.Disabled:
                case ScrollBarVisibility.Hidden:
                    verticalVisibility = Visibility.Collapsed;
                    break;
                default:  // Avoids compiler warning about uninitialized variable
                case ScrollBarVisibility.Auto:
                    verticalVisibility = ScrollInfo == null || ScrollInfo.ExtentHeight <= ScrollInfo.ViewportHeight ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

            if (verticalVisibility != ComputedVerticalScrollBarVisibility)
            {
                ComputedVerticalScrollBarVisibility = verticalVisibility;
                InvalidateMeasure();
            }
        }
        void UpdateScrollBar(Orientation orientation, double value)
        {
            try
            {
                // Update relevant ScrollBar
                if (orientation == Orientation.Horizontal)
                {
                    HorizontalOffset = value;
                    if (ElementHorizontalScrollBar != null)
                    {
                        ElementHorizontalScrollBar.Value = value;
                    }
                }
                else
                {
                    VerticalOffset = value;
                    if (ElementVerticalScrollBar != null)
                    {
                        ElementVerticalScrollBar.Value = value;
                    }
                }
            }
            finally
            {

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

            if (!e.Handled && ScrollInfo != null)
            {
#if MIGRATION
                if (e.Delta < 0)
#else
                if (e.GetCurrentPoint(null).Properties.MouseWheelDelta < 0)
#endif
                {
                    ScrollInfo.MouseWheelDown();
                }
#if MIGRATION
                else if (0 < e.Delta)
#else
                else if (0 < e.GetCurrentPoint(null).Properties.MouseWheelDelta)
#endif
                {
                    ScrollInfo.MouseWheelUp();
                }

                e.Handled = true;
            }
        }

        private bool TemplatedParentHandlesScrolling => TemplatedParent is Control c && c.HandlesScrolling;

        /// <summary>
        /// Responds to the KeyDown event. 
        /// </summary> 
        /// <param name="e">Provides data for KeyEventArgs.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (ScrollInfo != null && !e.Handled && !TemplatedParentHandlesScrolling)
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
