
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
using System.Windows.Input;
using System.Windows.Media;
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays the content of a <see cref="ScrollViewer"/> control.
    /// </summary>
    public sealed class ScrollContentPresenter : ContentPresenter, IScrollInfo, INativeScrollInfo
    {
        private static readonly Vector UnknownOffset = new(double.NaN, double.NaN);

        private IScrollInfo _scrollInfo;
        private ScrollData _scrollData;

        // Native scrolling: the DOM element of this presenter is the browser scroller, and the
        // content host (a child DOM element sized to the extent) contains the DOM of the content.
        private HtmlElementReference _contentHost;
        private NativeScrollManager.Mode? _appliedMode;
        private bool _appliedCanScrollX;
        private bool _appliedCanScrollY;
        private bool _isNative;
        private Size _nativeExtent;
        private Vector _nativeOffset;
        private bool _isNativeOffsetRequested;
        private bool _isNativeOffsetSyncPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContentPresenter"/> class.
        /// </summary>
        public ScrollContentPresenter()
        {
            IsVisibleChanged += OnIsVisibleChanged;
        }

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
                    UpdateNativeScrolling();
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
                    UpdateNativeScrolling();
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
                _isNativeOffsetRequested = _isNative;
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
                _isNativeOffsetRequested = _isNative;
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

            UpdateNativeScrolling();
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

        bool INativeScrollInfo.IsNativeScrolling => _isNative;

        Vector INativeScrollInfo.NativeOffset => _isNative ? GetKnownNativeOffset() : default;

        internal bool IsNativeScrolling => _isNative;

        internal override bool EnablePointerEventsCore => _isNative;

        internal override HtmlElementReference ChildrenHost => _contentHost.IsConnected ? _contentHost : OuterDiv;

        protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
        {
            HtmlElementReference div = base.CreateDomElement(parent);

            if (Features.Scrolling.UseNativeScrolling)
            {
                _contentHost = NativeScrollManager.CreateContentHost(div);
            }

            return div;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            UpdateNativeScrolling();
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            // The DOM is recreated when the element is attached again.
            _contentHost = default;
            _appliedMode = null;
            _nativeExtent = default;
            _nativeOffset = UnknownOffset;
        }

        // Applies the scrolling mode to the DOM: native when this presenter scrolls its content
        // (and native scrolling is allowed), managed otherwise.
        internal void UpdateNativeScrolling()
        {
            if (!_contentHost.IsConnected)
            {
                SetIsNative(false);
                return;
            }

            NativeScrollManager.Mode mode = GetNativeScrollingMode();
            bool canScrollX = false;
            bool canScrollY = false;

            switch (mode)
            {
                case NativeScrollManager.Mode.Native:
                    canScrollX = _scrollData._canHorizontallyScroll;
                    canScrollY = _scrollData._canVerticallyScroll;
                    break;

                case NativeScrollManager.Mode.Managed:
                    // Same condition as ScrollViewer's touch panning: when no scrollbar is visible, touches
                    // must be left to outer (possibly native) scrollers.
                    if (_scrollInfo.ScrollOwner is ScrollViewer sv)
                    {
                        canScrollX = sv.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
                        canScrollY = sv.ComputedVerticalScrollBarVisibility == Visibility.Visible;
                    }
                    break;
            }

            if (_appliedMode == mode && _appliedCanScrollX == canScrollX && _appliedCanScrollY == canScrollY)
            {
                return;
            }

            _appliedMode = mode;
            _appliedCanScrollX = canScrollX;
            _appliedCanScrollY = canScrollY;

            NativeScrollManager.SetMode(OuterDiv, mode, canScrollX, canScrollY);

            _nativeExtent = default;
            _nativeOffset = UnknownOffset;
            _isNativeOffsetRequested = false;

            SetIsNative(mode == NativeScrollManager.Mode.Native);
            InvalidateArrange();
        }

        private NativeScrollManager.Mode GetNativeScrollingMode()
        {
            if (_scrollInfo is null)
            {
                return NativeScrollManager.Mode.None;
            }

            if (IsScrollClient)
            {
                return NativeScrolling.GetIsEnabled(this) ? NativeScrollManager.Mode.Native : NativeScrollManager.Mode.Managed;
            }

            // The content scrolls itself. If it does so natively, touch panning must reach it.
            return _scrollInfo is INativeScrollInfo { IsNativeScrolling: true } ?
                NativeScrollManager.Mode.None :
                NativeScrollManager.Mode.Managed;
        }

        private void SetIsNative(bool value)
        {
            if (_isNative == value)
            {
                return;
            }

            _isNative = value;

            // The scroller must be hit-testable for the browser to scroll it when the pointer is over
            // an area that is not covered by the content.
            CoerceIsHitTestable();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Browsers drop the scroll position of elements that are not rendered (display: none).
            if (_isNative && (bool)e.NewValue)
            {
                _nativeOffset = UnknownOffset;
                InvalidateArrange();
            }
        }

        private Vector GetKnownNativeOffset() =>
            new(double.IsNaN(_nativeOffset.X) ? 0.0 : _nativeOffset.X, double.IsNaN(_nativeOffset.Y) ? 0.0 : _nativeOffset.Y);

        internal void OnNativeScroll(double x, double y)
        {
            if (!_isNative || !IsScrollClient)
            {
                return;
            }

            _nativeOffset = new Vector(x, y);

            // An offset set in C# and not yet written to the DOM wins over the scroll that preceded it.
            if (_isNativeOffsetRequested)
            {
                return;
            }

            _scrollData._offset = new Vector(
                _scrollData._canHorizontallyScroll ? SnapNativeOffset(x, _scrollData._extent.Width, _scrollData._viewport.Width) : _scrollData._offset.X,
                _scrollData._canVerticallyScroll ? SnapNativeOffset(y, _scrollData._extent.Height, _scrollData._viewport.Height) : _scrollData._offset.Y);

            // The content is already at the right place: only the ScrollViewer needs to be updated.
            if (CoerceOffsets())
            {
                ScrollOwner?.InvalidateScrollInfo();
            }
        }

        // The browser reports offsets in device pixels (and Safari reports overscroll), so offsets
        // close to the bounds are snapped to them for VerticalOffset == ScrollableHeight to hold.
        internal static double SnapNativeOffset(double offset, double extent, double viewport)
        {
            double max = Math.Max(0.0, extent - viewport);
            if (offset < 1.0)
            {
                return 0.0;
            }
            if (offset > max - 1.0)
            {
                return max;
            }
            return offset;
        }

        private void ArrangeNative(Rect childRect)
        {
            Debug.Assert(_isNative && IsScrollClient);

            Size extent = childRect.Size;
            if (!DoubleUtil.AreClose(_nativeExtent, extent))
            {
                _nativeExtent = extent;
                NativeScrollManager.SetExtent(OuterDiv, extent.Width, extent.Height);
            }

            // Only offsets requested in C# are written: offsets coerced during layout are also coerced by
            // the browser, and writing them could interrupt an ongoing (inertial) scroll with a stale value.
            if (!_isNativeOffsetRequested && !IsNativeOffsetUnknown)
            {
                return;
            }

            // Written after layout, once the DOM size of this presenter and of its content are up to
            // date, otherwise the browser would clamp the offset.
            if (!_isNativeOffsetSyncPending)
            {
                _isNativeOffsetSyncPending = true;
                LayoutUpdated += OnLayoutUpdatedSyncNativeOffset;
            }
        }

        private void OnLayoutUpdatedSyncNativeOffset(object sender, EventArgs e)
        {
            LayoutUpdated -= OnLayoutUpdatedSyncNativeOffset;
            _isNativeOffsetSyncPending = false;

            bool isRequested = _isNativeOffsetRequested;
            _isNativeOffsetRequested = false;

            if (!_isNative || !IsScrollClient || !OuterDiv.IsConnected || (!isRequested && !IsNativeOffsetUnknown))
            {
                return;
            }

            Vector offset = _scrollData._computedOffset;
            if (Math.Abs(offset.X - _nativeOffset.X) < 1.0 && Math.Abs(offset.Y - _nativeOffset.Y) < 1.0)
            {
                return;
            }

            _nativeOffset = offset;
            NativeScrollManager.ScrollTo(OuterDiv, offset.X, offset.Y);
        }

        private bool IsNativeOffsetUnknown => double.IsNaN(_nativeOffset.X);

        internal bool HandleNativeMouseWheel(MouseWheelEventArgs e)
        {
            Debug.Assert(_isNative);

            switch (NativeScrollManager.OnWheel(OuterDiv, e))
            {
                case NativeScrollManager.WheelResult.ScrolledByBrowser:
                    e.Handled = true;
                    e.Cancellable = false;
                    return true;

                case NativeScrollManager.WheelResult.Scrolled:
                    e.Handled = true;
                    return true;

                default:
                    return false;
            }
        }

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

                    // In native mode, the browser offsets the content.
                    if (IsScrollClient && !_isNative)
                    {
                        childRect.X = -HorizontalOffset;
                        childRect.Y = -VerticalOffset;
                    }

                    //this is needed to stretch the child to arrange space,
                    childRect.Width = Math.Max(childRect.Width, arrangeSize.Width);
                    childRect.Height = Math.Max(childRect.Height, arrangeSize.Height);

                    child.Arrange(childRect);

                    if (_isNative)
                    {
                        ArrangeNative(childRect);
                    }
                }
            }
            else if (_isNative)
            {
                ArrangeNative(new Rect(arrangeSize));
            }

            return arrangeSize;
        }

        /// <inheritdoc />
        protected override Geometry GetLayoutClip(Size layoutSlotSize) =>
            _isNative ? null : new RectangleGeometry(new Rect(RenderSize));

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
