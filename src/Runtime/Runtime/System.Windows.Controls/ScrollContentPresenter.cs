
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays the content of a <see cref="ScrollViewer"/> control.
    /// </summary>
    public sealed class ScrollContentPresenter : ContentPresenter, IScrollInfo
    {
        private IScrollInfo _scrollInfo;
        private ScrollData _scrollData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContentPresenter"/> class.
        /// </summary>
        public ScrollContentPresenter() { }

        /// <summary>
        /// Gets or sets the <see cref="ScrollViewer"/> element that controls scrolling
        /// behavior.
        /// </summary>
        /// <returns>
        /// The <see cref="ScrollViewer"/> element that controls scrolling behavior.
        /// </returns>
        public ScrollViewer ScrollOwner
        {
            get { return IsScrollClient ? _scrollData._scrollOwner : null; }
            set { if (IsScrollClient) _scrollData._scrollOwner = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis
        /// is possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false.
        /// </returns>
        public bool CanHorizontallyScroll
        {
            get { return IsScrollClient ? _scrollData._canHorizontallyScroll : false; }
            set
            {
                if (IsScrollClient && _scrollData._canHorizontallyScroll != value)
                {
                    _scrollData._canHorizontallyScroll = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is
        /// possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false.
        /// </returns>
        public bool CanVerticallyScroll
        {
            get { return IsScrollClient ? _scrollData._canVerticallyScroll : false; }
            set
            {
                if (IsScrollClient && _scrollData._canVerticallyScroll != value)
                {
                    _scrollData._canVerticallyScroll = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance the content has been scrolled horizontally.
        /// </summary>
        /// <returns>
        /// The distance the content has been scrolled horizontally.
        /// </returns>
        public double HorizontalOffset => IsScrollClient ? _scrollData._computedOffset.X : 0.0;

        /// <summary>
        /// Sets the distance the content has been scrolled horizontally.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled horizontally.
        /// </param>
        public void SetHorizontalOffset(double offset)
        {
            if (!IsScrollClient) return;

            double newValue = ValidateInputOffset(offset, nameof(HorizontalOffset));
            if (_scrollData._canHorizontallyScroll && !DoubleUtil.AreClose(_scrollData._offset.X, newValue))
            {
                _scrollData._offset.X = newValue;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets or sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <returns>
        /// The distance the content has been scrolled vertically.
        /// </returns>
        public double VerticalOffset => IsScrollClient ? _scrollData._computedOffset.Y : 0.0;

        /// <summary>
        /// Sets the distance the content has been scrolled vertically.
        /// </summary>
        /// <param name="offset">
        /// The distance the content has been scrolled vertically.
        /// </param>
        public void SetVerticalOffset(double offset)
        {
            if (!IsScrollClient) return;

            double newValue = ValidateInputOffset(offset, nameof(VerticalOffset));
            if (_scrollData._canVerticallyScroll && !DoubleUtil.AreClose(_scrollData._offset.Y, newValue))
            {
                _scrollData._offset.Y = newValue;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// The horizontal size of the extent.
        /// </returns>
        public double ExtentWidth => IsScrollClient ? _scrollData._extent.Width : 0.0;

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// The vertical size of the extent.
        /// </returns>
        public double ExtentHeight => IsScrollClient ? _scrollData._extent.Height : 0.0;

        /// <summary>
        /// Gets the horizontal size of the viewport.
        /// </summary>
        /// <returns>
        /// The horizontal size of the viewport.
        /// </returns>
        public double ViewportWidth => IsScrollClient ? _scrollData._viewport.Width : 0.0;

        /// <summary>
        /// Gets the vertical size of the viewport.
        /// </summary>
        /// <returns>
        /// The vertical size of the viewport.
        /// </returns>
        public double ViewportHeight => IsScrollClient ? _scrollData._viewport.Height : 0.0;

        /// <summary>
        /// Identifies the <see cref="CanContentScroll"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanContentScrollProperty =
            ScrollViewer.CanContentScrollProperty.AddOwner(
                typeof(ScrollContentPresenter),
                new PropertyMetadata(OnCanContentScrollChanged));

        /// <summary>
        /// Indicates whether the content, if it supports <see cref="IScrollInfo"/>, 
        /// should be allowed to control scrolling.
        /// </summary>
        /// <returns>
        /// true if the content is allowed to scroll; otherwise, false. A false value 
        /// indicates that the <see cref="ScrollContentPresenter"/> acts as the scrolling 
        /// client. This property has no default value.
        /// </returns>
        public bool CanContentScroll
        {
            get => (bool)GetValue(CanContentScrollProperty);
            set => SetValueInternal(CanContentScrollProperty, value);
        }

        private static void OnCanContentScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollContentPresenter scp = (ScrollContentPresenter)d;
            if (scp._scrollInfo is null)
            {
                return;
            }

            scp.HookupScrollingComponents();
            scp.InvalidateMeasure();
        }

        // Helper method to get our ScrollViewer owner and its scrolling content talking.
        // Method introduces the current owner/content, and clears a from any previous content.
        internal void HookupScrollingComponents()
        {
            // We need to introduce our IScrollInfo to our ScrollViewer (and break any previous links).

            // If our content is not an IScrollInfo, we should have selected a style that contains one.
            if (TemplatedParent is ScrollViewer scrollContainer)
            {
                IScrollInfo si = null;

                if (CanContentScroll)
                {
                    // We need to get an IScrollInfo to introduce to the ScrollViewer.
                    // 1. Try our content...
                    si = Content as IScrollInfo;

                    if (si is null)
                    {
                        if (Content is UIElement child)
                        {
                            // 2. Our child might be an ItemsPresenter.  In this case check its child for being an IScrollInfo
                            ItemsPresenter itemsPresenter = child as ItemsPresenter;
                            if (itemsPresenter is null)
                            {
                                // 3. With the change in templates for ClearTypeHint the ItemsPresenter is not guranteed to be the 
                                // immediate child. We now look for a named element instead of naively walking the descendents.
                                if (scrollContainer.TemplatedParent is FrameworkElement templatedParent)
                                {
                                    itemsPresenter = templatedParent.GetTemplateChild("ItemsPresenter") as ItemsPresenter;
                                }
                            }

                            if (itemsPresenter is not null)
                            {
                                itemsPresenter.ApplyTemplate();

                                int count = VisualTreeHelper.GetChildrenCount(itemsPresenter);
                                if (count > 0)
                                {
                                    si = VisualTreeHelper.GetChild(itemsPresenter, 0) as IScrollInfo;
                                }
                            }
                        }
                    }
                }

                // 4. As a final fallback, we use ourself.
                if (si is null)
                {
                    si = this;
                    EnsureScrollData();
                }

                // Detach any differing previous IScrollInfo from ScrollViewer
                if (si != _scrollInfo && _scrollInfo is not null)
                {
                    if (IsScrollClient) _scrollData = null;
                    else _scrollInfo.ScrollOwner = null;
                }

                // Introduce our ScrollViewer and IScrollInfo to each other.
                _scrollInfo = si;                   // At this point, we pass IsScrollClient if si == this.
                si.ScrollOwner = scrollContainer;
                scrollContainer.ScrollInfo = si;
            }
            else if (_scrollInfo is not null)
            {
                // We're not really in a valid scrolling scenario.  Break any previous references, and get us
                // back into a totally unlinked state.

                if (_scrollInfo.ScrollOwner is not null)
                {
                    _scrollInfo.ScrollOwner.ScrollInfo = null;
                }

                _scrollInfo.ScrollOwner = null;
                _scrollInfo = null;
                _scrollData = null;
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="ScrollContentPresenter"/>
        /// when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get our scrolling owner and content talking.
            HookupScrollingComponents();
        }

        private bool IsScrollClient => _scrollInfo == this;

        private ScrollData EnsureScrollData() => _scrollData ??= new ScrollData();

        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize;

            if (!IsScrollClient)
            {
                desiredSize = base.MeasureOverride(constraint);
            }
            else
            {
                var childConstraint = new Size(
                    _scrollData._canHorizontallyScroll ? double.PositiveInfinity : constraint.Width,
                    _scrollData._canVerticallyScroll ? double.PositiveInfinity : constraint.Height);

                desiredSize = base.MeasureOverride(childConstraint);
            }

            // If we're handling scrolling (as the physical scrolling client, validate properties.
            if (IsScrollClient)
            {
                UpdateExtents(constraint, desiredSize);
            }

            desiredSize.Width = Math.Min(constraint.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(constraint.Height, desiredSize.Height);

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            // Verifies IScrollInfo properties & invalidates ScrollViewer if necessary.
            if (IsScrollClient)
            {
                UpdateExtents(arrangeSize, _scrollData._extent);
            }

            int count = VisualChildrenCount;

            if (count > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    var childRect = new Rect(child.DesiredSize);

                    if (IsScrollClient)
                    {
                        childRect.X = -HorizontalOffset;
                        childRect.Y = -VerticalOffset;
                    }

                    //this is needed to stretch the child to arrange space,
                    childRect.Width = Math.Max(childRect.Width, arrangeSize.Width);
                    childRect.Height = Math.Max(childRect.Height, arrangeSize.Height);

                    child.Arrange(childRect);
                }
            }

            return arrangeSize;
        }

        /// <inheritdoc />
        protected override Geometry GetLayoutClip(Size layoutSlotSize) => new RectangleGeometry(new Rect(RenderSize));

        private void UpdateExtents(Size viewport, Size extents)
        {
            Debug.Assert(IsScrollClient);

            bool changed = !DoubleUtil.AreClose(_scrollData._viewport, viewport) || !DoubleUtil.AreClose(_scrollData._extent, extents);
            _scrollData._viewport = viewport;
            _scrollData._extent = extents;

            changed |= CoerceOffsets();

            if (changed)
            {
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        private bool CoerceOffsets()
        {
            Debug.Assert(IsScrollClient);
            var computedOffset = new Vector(
                CoerceOffset(_scrollData._offset.X, _scrollData._extent.Width, _scrollData._viewport.Width),
                CoerceOffset(_scrollData._offset.Y, _scrollData._extent.Height, _scrollData._viewport.Height));

            bool changed = !DoubleUtil.AreClose(_scrollData._computedOffset, computedOffset);
            _scrollData._computedOffset = computedOffset;

            return changed;
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content downward by
        /// one line.
        /// </summary>
        public void LineDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the left
        /// by a predetermined amount.
        /// </summary>
        public void LineLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content to the right
        /// by a predetermined amount.
        /// </summary>
        public void LineRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ScrollViewer.LineDelta);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollContentPresenter"/> content upward by
        /// one line.
        /// </summary>
        public void LineUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ScrollViewer.LineDelta);
        }

        // FIXME: how does one invoke MouseWheelUp/Down/etc? Need to figure out proper scrolling amounts
        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ScrollViewer.WheelDelta);
        }

        /// <summary>
        /// Scrolls down within the content by one page.
        /// </summary>
        public void PageDown()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scrolls left within the content by one page.
        /// </summary>
        public void PageLeft()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scrolls right within the content by one page.
        /// </summary>
        public void PageRight()
        {
            if (IsScrollClient) SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scrolls up within the content by one page.
        /// </summary>
        public void PageUp()
        {
            if (IsScrollClient) SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a visual object is visible.
        /// </summary>
        /// <param name="visual">
        /// A <see cref="UIElement"/> that becomes visible.
        /// </param>
        /// <param name="rectangle">
        /// The bounding rectangle that identifies the coordinate space to make visible.
        /// </param>
        /// <returns>
        /// A <see cref="Rect"/> that represents the visible region.
        /// </returns>
        public Rect MakeVisible(UIElement visual, Rect rectangle) => MakeVisible(visual, rectangle, true);

        /// <summary>
        /// ScrollContentPresenter implementation of <seealso cref="IScrollInfo.MakeVisible" />.
        /// </summary>
        /// <param name="visual">The Visual that should become visible</param>
        /// <param name="rectangle">A rectangle representing in the visual's coordinate space to make visible.</param>
        /// <param name="throwOnError">If true the method throws an exception when an error is encountered, otherwise the method returns Rect.Empty when an error is encountered</param>
        /// <returns>
        /// A rectangle in the IScrollInfo's coordinate space that has been made visible.
        /// Other ancestors to in turn make this new rectangle visible.
        /// The rectangle should generally be a transformed version of the input rectangle.  In some cases, like
        /// when the input rectangle cannot entirely fit in the viewport, the return value might be smaller.
        /// </returns>
        internal Rect MakeVisible(UIElement visual, Rect rectangle, bool throwOnError)
        {
            // (ScrollContentPresenter.MakeVisible can cause an exception when encountering an empty rectangle)
            // This method exists to keep ScrollContentPresenter.MakeVisible v1 behavior
            // while allowing callers of IScrollInfo.MakeVisible in the platform work around a bug
            // in the v1 behavior.
            // If this bug is fixed look for callers of IScrollInfo.MakeVisible with workarounds.
            // They should be updated remove the workarounds.

            //
            // Note: This code presently assumes we/children are layout clean.  See work item 22269 for more detail.
            //

            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (rectangle.IsEmpty || visual is null || visual == this || !IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            // Compute the child's rect relative to (0,0) in our coordinate space.
            Matrix childTransform = visual.InternalTransformToAncestor(this);

            rectangle.Transform(childTransform);

            if (!IsScrollClient || (!throwOnError && rectangle.IsEmpty))
            {
                return rectangle;
            }

            // Initialize the viewport
            var viewport = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
            rectangle.X += viewport.X;
            rectangle.Y += viewport.Y;

            // Compute the offsets required to minimally scroll the child maximally into view.
            double minX = ComputeScrollOffsetWithMinimalScroll(viewport.Left, viewport.Right, rectangle.Left, rectangle.Right);
            double minY = ComputeScrollOffsetWithMinimalScroll(viewport.Top, viewport.Bottom, rectangle.Top, rectangle.Bottom);

            // We have computed the scrolling offsets; scroll to them.
            SetHorizontalOffset(minX);
            SetVerticalOffset(minY);

            // Compute the visible rectangle of the child relative to the viewport.
            viewport.X = minX;
            viewport.Y = minY;
            rectangle.Intersect(viewport);

            if (throwOnError)
            {
                // (ScrollContentPresenter.MakeVisible can cause an exception when encountering an empty rectangle)
                // Old behavior for app compat
                rectangle.X -= viewport.X;
                rectangle.Y -= viewport.Y;
            }
            else
            {
                // (ScrollContentPresenter.MakeVisible can cause an exception when encountering an empty rectangle)
                // New correct behavior
                if (!rectangle.IsEmpty)
                {
                    rectangle.X -= viewport.X;
                    rectangle.Y -= viewport.Y;
                }
            }

            // Return the rectangle
            return rectangle;
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(
            double topView,
            double bottomView,
            double topChild,
            double bottomChild)
        {
            bool alignTop = false;
            bool alignBottom = false;
            return ComputeScrollOffsetWithMinimalScroll(topView, bottomView, topChild, bottomChild, ref alignTop, ref alignBottom);
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(
            double topView,
            double bottomView,
            double topChild,
            double bottomChild,
            ref bool alignTop,
            ref bool alignBottom)
        {
            // # CHILD POSITION       CHILD SIZE      SCROLL      REMEDY
            // 1 Above viewport       <= viewport     Down        Align top edge of child & viewport
            // 2 Above viewport       > viewport      Down        Align bottom edge of child & viewport
            // 3 Below viewport       <= viewport     Up          Align bottom edge of child & viewport
            // 4 Below viewport       > viewport      Up          Align top edge of child & viewport
            // 5 Entirely within viewport             NA          No scroll.
            // 6 Spanning viewport                    NA          No scroll.
            //
            // Note: "Above viewport" = childTop above viewportTop, childBottom above viewportBottom
            //       "Below viewport" = childTop below viewportTop, childBottom below viewportBottom
            // These child thus may overlap with the viewport, but will scroll the same direction/

            bool fAbove = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool fBelow = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);
            bool fLarger = (bottomChild - topChild) > (bottomView - topView);

            // Handle Cases:  1 & 4 above
            if ((fAbove && !fLarger) || (fBelow && fLarger) || alignTop)
            {
                alignTop = true;
                return topChild;
            }

            // Handle Cases: 2 & 3 above
            else if (fAbove || fBelow || alignBottom)
            {
                alignBottom = true;
                return bottomChild - (bottomView - topView);
            }

            // Handle cases: 5 & 6 above.
            return topView;
        }

        internal static double ValidateInputOffset(double offset, string parameterName)
        {
            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(Strings.ScrollViewer_CannotBeNaN, parameterName));
            }

            return Math.Max(0.0, offset);
        }

        // Returns an offset coerced into the [0, Extent - Viewport] range.
        internal static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > extent - viewport)
            {
                offset = extent - viewport;
            }
            if (offset < 0)
            {
                offset = 0;
            }
            return offset;
        }

        private sealed class ScrollData
        {
            internal ScrollViewer _scrollOwner;
            internal bool _canHorizontallyScroll;
            internal bool _canVerticallyScroll;
            internal Vector _offset;
            internal Vector _computedOffset;
            internal Size _viewport;
            internal Size _extent;
        }
    }
}
