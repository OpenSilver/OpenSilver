
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls;

using State = (NativeScrollManager.Mode Mode, bool CanScrollX, bool CanScrollY, bool IsOpaque, bool UseNativeScrollBars);

// Native scrolling: when the panel is the scrolling panel of a ScrollViewer, its DOM element is the
// browser scroller and its content host is sized to the (estimated) pixel size of all the items.
// Realized containers are positioned at their pixel offset, computed from the sizes measured so far
// (and an estimate for the other items).
//
// Offsets exposed through IScrollInfo remain logical (in items) in the stacking direction, as in
// Silverlight: they are mapped to pixels with the size cache.
//
// When the size of an item above the first visible item turns out to differ from its estimate, the
// first visible item is kept where it is rendered (instead of writing the scroll offset of the DOM
// element, which would interrupt an inertial scroll). The difference between the DOM positions and
// the positions in the size cache is the 'shift'. It is resolved when the beginning of the list is
// close to the viewport, by moving the content and the scroll offset together.
public partial class VirtualizingStackPanel : INativeScrollInfo
{
    private readonly NativeScrollData _nativeScrollData = new();

    bool INativeScrollInfo.IsNativeScrolling => IsNativeScrolling;

    bool INativeScrollInfo.CanNativelyScroll => IsNativeScrolling && _nativeScrollData._appliedNativeState is { } state && (state.CanScrollX || state.CanScrollY);

    Vector INativeScrollInfo.NativeOffset => IsNativeScrolling ? GetKnownNativeOffset() : default;

    internal bool IsNativeScrolling => _nativeScrollData._isNative;

    internal override bool EnablePointerEventsCore => IsNativeScrolling || Background is not null;

    internal override HtmlElementReference ChildrenHost =>
        _nativeScrollData._contentHost.IsConnected ? _nativeScrollData._contentHost : OuterDiv;

    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        HtmlElementReference div = base.CreateDomElement(parent);

        if (NativeScrolling.UseNativeScrolling)
        {
            _nativeScrollData._contentHost = NativeScrollManager.CreateContentHost(div);
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
        _nativeScrollData._contentHost = default;
        _nativeScrollData._appliedNativeState = null;
        _nativeScrollData._nativeExtent = default;
        _nativeScrollData._nativeOffset = NativeScrollData.UnknownOffset;
        _nativeScrollData._nativeShift = 0.0;
        _nativeScrollData._nativeScrollAdjustment = 0.0;
    }

    /// <inheritdoc />
    protected override Geometry GetLayoutClip(Size layoutSlotSize) => IsNativeScrolling ? null : base.GetLayoutClip(layoutSlotSize);

    private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VirtualizingStackPanel)d).UpdateNativeScrolling();
    }

    private static void OnIsNativeScrollingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VirtualizingStackPanel)d).UpdateNativeScrolling();
    }

    private static void OnUseNativeScrollBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VirtualizingStackPanel)d).UpdateNativeScrolling();
    }

    // Natively scrolled when this panel scrolls the items of an ItemsControl (and native scrolling is
    // allowed). Otherwise, the ScrollContentPresenter of the ScrollViewer uses the managed scrolling.
    internal void UpdateNativeScrolling()
    {
        if (!_nativeScrollData._contentHost.IsConnected)
        {
            SetIsNative(false);
            return;
        }

        bool isNative = IsItemsHost &&
            IsScrolling &&
            (Orientation == Orientation.Horizontal ? CanHorizontallyScroll : CanVerticallyScroll) &&
            !_nativeScrollData._isNativeExtentTooLarge &&
            NativeScrolling.GetIsEnabled(this);

        // The browser takes over touches along the axes it can scroll (even when the items fit), so the
        // axes along which there is nothing to scroll are left to the application.
        State state = isNative ?
            (NativeScrollManager.Mode.Native,
             CanHorizontallyScroll && DoubleUtil.GreaterThan(_nativeScrollData._nativeExtent.Width, _nativeScrollData._arrangedSize.Width),
             CanVerticallyScroll && DoubleUtil.GreaterThan(_nativeScrollData._nativeExtent.Height, _nativeScrollData._arrangedSize.Height),
             Background is not null,
             ScrollOwner is { AllowsNativeScrollBars: true }) :
            (NativeScrollManager.Mode.None, false, false, false, false);

        if (_nativeScrollData._appliedNativeState == state)
        {
            return;
        }

        bool modeChanged = _nativeScrollData._appliedNativeState?.Mode != state.Mode;
        _nativeScrollData._appliedNativeState = state;

        NativeScrollManager.SetMode(OuterDiv, state.Mode, state.CanScrollX, state.CanScrollY, state.IsOpaque, state.UseNativeScrollBars);

        if (modeChanged)
        {
            _nativeScrollData._nativeExtent = default;
            _nativeScrollData._nativeOffset = NativeScrollData.UnknownOffset;
            _nativeScrollData._nativeShift = 0.0;
            _nativeScrollData._nativeScrollAdjustment = 0.0;
            _nativeScrollData._isNativeOffsetRequested = false;
            _nativeScrollData._realizedLastIndex = -1;
        }

        SetIsNative(isNative);
    }

    private void SetIsNative(bool value)
    {
        if (IsNativeScrolling == value)
        {
            return;
        }

        _nativeScrollData._isNative = value;

        // The scroller must be hit-testable for the browser to scroll it when the pointer is over
        // an area that is not covered by the items.
        CoerceIsHitTestable();

        // Both are ignored while being measured: in that case, this panel is measured with the new
        // mode, then the ScrollViewer updates its ScrollContentPresenter.
        InvalidateMeasure();

        // The ScrollContentPresenter must stop (or start) scrolling the panel itself.
        ScrollOwner?.InvalidateMeasure();
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Browsers drop the scroll position of elements that are not rendered (display: none).
        if (IsNativeScrolling && (bool)e.NewValue)
        {
            _nativeScrollData._nativeOffset = NativeScrollData.UnknownOffset;
            InvalidateMeasure();
        }
    }

    private Vector GetKnownNativeOffset() =>
        new(double.IsNaN(_nativeScrollData._nativeOffset.X) ? 0.0 : _nativeScrollData._nativeOffset.X,
            double.IsNaN(_nativeScrollData._nativeOffset.Y) ? 0.0 : _nativeScrollData._nativeOffset.Y);

    private bool IsNativeOffsetUnknown => double.IsNaN(_nativeScrollData._nativeOffset.X);

    // The shift is ignored: the extent grows to the total size when the shift is resolved, and the
    // mode must not change during a scroll.
    private bool IsNativeExtentTooLarge() => _nativeScrollData._itemSizes?.TotalSize > NativeScrollData.MaxNativeExtent;

    // Called during measure. The logical offset is kept, so the same items remain visible.
    private void FallBackToManagedScrolling()
    {
        _nativeScrollData._isNativeExtentTooLarge = true;
        _nativeScrollData._itemSizes = null;
        UpdateNativeScrolling();
        Debug.Assert(!IsNativeScrolling);
    }

    void INativeScrollInfo.HandleNativeMouseWheel(MouseWheelEventArgs e)
    {
        Debug.Assert(IsNativeScrolling);

        NativeScrollManager.HandleMouseWheel(OuterDiv, e);
    }

    void INativeScrollInfo.OnNativeScroll(double x, double y)
    {
        if (!IsNativeScrolling || !IsScrolling)
        {
            return;
        }

        _nativeScrollData._nativeOffset = new Vector(x, y);

        // An offset set in C# and not yet written to the DOM wins over the scroll that preceded it.
        if (_nativeScrollData._isNativeOffsetRequested || _nativeScrollData._nativeScrollAdjustment != 0.0 || _nativeScrollData._itemSizes is null)
        {
            return;
        }

        UpdateNativeScrollData();

        if (!IsNativeViewportRealized())
        {
            InvalidateMeasure();
        }
    }

    // Updates the (logical) offsets from the scroll offset of the DOM element.
    private void UpdateNativeScrollData()
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;
        ItemSizeCache sizes = _nativeScrollData._itemSizes;
        double viewport = _nativeScrollData._nativeViewportSize;

        double position = ScrollContentPresenter.SnapNativeOffset(
            isHorizontal ? _nativeScrollData._nativeOffset.X : _nativeScrollData._nativeOffset.Y,
            sizes.TotalSize + _nativeScrollData._nativeShift,
            viewport);

        double crossOffset = ScrollContentPresenter.SnapNativeOffset(
            isHorizontal ? _nativeScrollData._nativeOffset.Y : _nativeScrollData._nativeOffset.X,
            isHorizontal ? _scrollData._extent.Height : _scrollData._extent.Width,
            isHorizontal ? _scrollData._viewport.Height : _scrollData._viewport.Width);

        SetNativeScrollData(isHorizontal, position - _nativeScrollData._nativeShift, viewport, crossOffset, _scrollData._viewport, _scrollData._extent);
    }

    private void SetNativeScrollData(bool isHorizontal, double position, double viewportSize, double crossOffset, Size viewport, Size extent)
    {
        ItemSizeCache sizes = _nativeScrollData._itemSizes;
        double logicalOffset = GetLogicalPosition(sizes, position);
        double logicalViewport = double.IsInfinity(viewportSize) ?
            sizes.Count :
            GetLogicalPosition(sizes, position + viewportSize) - logicalOffset;

        if (isHorizontal)
        {
            viewport.Width = logicalViewport;
            extent.Width = sizes.Count;
        }
        else
        {
            viewport.Height = logicalViewport;
            extent.Height = sizes.Count;
        }

        Vector offset = isHorizontal ? new(logicalOffset, crossOffset) : new(crossOffset, logicalOffset);
        offset.X = ScrollContentPresenter.CoerceOffset(offset.X, extent.Width, viewport.Width);
        offset.Y = ScrollContentPresenter.CoerceOffset(offset.Y, extent.Height, viewport.Height);

        VerifyScrollingData(viewport, extent, offset);
    }

    // Item sizes vary and offsets are fractional: the offset that brings the item into view with
    // a minimal scroll is computed in pixels.
    private bool MakeVisibleNative(int childIndex, Rect r, ref Vector newOffset, ref Rect newRect)
    {
        if (!IsNativeScrolling)
        {
            return false;
        }

        ItemSizeCache sizes = _nativeScrollData._itemSizes;
        double viewport = _nativeScrollData._nativeViewportSize;
        if (sizes is null || childIndex < 0 || childIndex >= sizes.Count || double.IsInfinity(viewport))
        {
            return false;
        }

        bool isHorizontal = Orientation == Orientation.Horizontal;
        double position = GetPixelPosition(sizes, isHorizontal ? _scrollData._computedOffset.X : _scrollData._computedOffset.Y);
        double childStart = sizes.GetPosition(childIndex);
        double childEnd = childStart + sizes.GetSize(childIndex);

        double newPosition = ScrollContentPresenter.ComputeScrollOffsetWithMinimalScroll(
            position, position + viewport, childStart, childEnd);
        double logicalOffset = DoubleUtil.AreClose(newPosition, childStart) ? childIndex : GetLogicalPosition(sizes, newPosition);

        if (isHorizontal)
        {
            newOffset.X = logicalOffset;
            newRect.X = childStart - newPosition;
            newRect.Width = r.Width;
        }
        else
        {
            newOffset.Y = logicalOffset;
            newRect.Y = childStart - newPosition;
            newRect.Height = r.Height;
        }

        return true;
    }

    // Returns the pixel position in the size cache of a logical offset (in items).
    private static double GetPixelPosition(ItemSizeCache sizes, double logicalOffset)
    {
        int count = sizes.Count;
        if (count == 0 || !(logicalOffset > 0.0))
        {
            return 0.0;
        }

        if (logicalOffset >= count)
        {
            return sizes.TotalSize;
        }

        int index = (int)logicalOffset;
        return sizes.GetPosition(index) + (logicalOffset - index) * sizes.GetSize(index);
    }

    // Returns the logical offset (in items) of a pixel position in the size cache.
    internal static double GetLogicalPosition(ItemSizeCache sizes, double position)
    {
        int count = sizes.Count;
        if (count == 0 || !(position > 0.0))
        {
            return 0.0;
        }

        if (position >= sizes.TotalSize)
        {
            return count;
        }

        int index = sizes.IndexAt(position);
        double size = sizes.GetSize(index);
        return size > 0.0 ? index + Math.Min(1.0, (position - sizes.GetPosition(index)) / size) : index;
    }

    private bool IsNativeViewportRealized()
    {
        if (_nativeScrollData._realizedLastIndex < _nativeScrollData._realizedFirstIndex)
        {
            return false;
        }

        double viewport = _nativeScrollData._nativeViewportSize;
        if (double.IsInfinity(viewport))
        {
            return true;
        }

        double position = Orientation == Orientation.Horizontal ? _nativeScrollData._nativeOffset.X : _nativeScrollData._nativeOffset.Y;
        double margin = viewport * NativeScrollData.NativeCacheMargin;

        // The shift is resolved when the beginning of the list gets close.
        if (_nativeScrollData._nativeShift != 0.0 && position <= viewport * NativeScrollData.NativeCacheLength)
        {
            return false;
        }

        return (_nativeScrollData._realizedFirstIndex == 0 || _nativeScrollData._realizedStart <= position - margin) &&
            (_nativeScrollData._realizedLastIndex >= _nativeScrollData._itemSizes.Count - 1 || _nativeScrollData._realizedEnd >= position + viewport + margin);
    }

    private Size MeasureItemsHostNative(Size constraint)
    {
        Debug.Assert(IsNativeScrolling && IsScrolling);

        // Ensure we always touch ItemContainerGenerator as by accessing this property
        // we hook up to some events on it.
        EnsureGenerator();
        bool isHorizontal = Orientation == Orientation.Horizontal;
        ItemsControl owner = ItemsControl.GetItemsOwner(this);
        int itemCount = owner.Items.Count;
        Size stackDesiredSize = new();

        SetVirtualizationState(owner);

        ItemSizeCache sizes = _nativeScrollData._itemSizes ??= new ItemSizeCache();
        sizes.SetCount(itemCount);

        double viewportSize = isHorizontal ? constraint.Width : constraint.Height;
        double cacheLength = double.IsInfinity(viewportSize) ? 0.0 : viewportSize * NativeScrollData.NativeCacheLength;

        bool isRequested = _nativeScrollData._isNativeOffsetRequested || IsNativeOffsetUnknown;
        double position = 0.0;
        double shift = isRequested ? 0.0 : _nativeScrollData._nativeShift;
        int first = 0;
        int last = -1;

        IItemContainerGenerator generator = ItemContainerGenerator;
        if (itemCount > 0)
        {
            Size childConstraint = constraint;
            if (CanHorizontallyScroll || isHorizontal)
            {
                childConstraint.Width = double.PositiveInfinity;
            }
            if (CanVerticallyScroll || !isHorizontal)
            {
                childConstraint.Height = double.PositiveInfinity;
            }

            // The anchor is the first visible item: it stays where it is rendered.
            int anchor;
            double anchorFraction = 0.0;
            if (isRequested)
            {
                (anchor, anchorFraction) = ComputeFirstItemInViewportIndex(isHorizontal, itemCount);
            }
            else
            {
                position = isHorizontal ? _nativeScrollData._nativeOffset.X : _nativeScrollData._nativeOffset.Y;
                anchor = sizes.IndexAt(position - shift);

                // When scrolling backward, the items realized before must not move while visible.
                if (anchor < _nativeScrollData._realizedFirstIndex &&
                    _nativeScrollData._realizedFirstIndex <= _nativeScrollData._realizedLastIndex &&
                    _nativeScrollData._realizedLastIndex < itemCount &&
                    _nativeScrollData._realizedStart < position + viewportSize)
                {
                    anchor = _nativeScrollData._realizedFirstIndex;
                }
            }

            double anchorPosition = sizes.GetPosition(anchor) + shift;
            double end = anchorPosition;
            List<UIElement> children = UnsafeGetChildren();

            GeneratorPosition start = generator.GeneratorPositionFromIndex(anchor);
            using (generator.StartAt(start, GeneratorDirection.Forward, true))
            {
                int insertAt = start.Offset == 0 ? start.Index : start.Index + 1;

                for (int i = anchor; i < itemCount; i++, insertAt++)
                {
                    UIElement child = (UIElement)generator.GenerateNext(out bool isNewlyRealized);
                    if (isNewlyRealized || insertAt >= children.Count || children[insertAt] != child)
                    {
                        if (insertAt < children.Count)
                        {
                            InsertInternalChild(insertAt, child);
                        }
                        else
                        {
                            AddInternalChild(child);
                        }

                        generator.PrepareItemContainer(child);
                    }

                    double size = MeasureNativeChild(child, childConstraint, isHorizontal, ref stackDesiredSize);
                    sizes.SetSize(i, size);
                    end += size;
                    last = i;

                    if (i == anchor && isRequested)
                    {
                        position = anchorPosition + anchorFraction * size;
                    }

                    if (end >= position + viewportSize + cacheLength)
                    {
                        break;
                    }
                }
            }

            // Measuring the items changes the estimate, hence the positions computed with it.
            if (isRequested)
            {
                double delta = sizes.GetPosition(anchor) - anchorPosition;
                anchorPosition += delta;
                position += delta;
            }
            else
            {
                shift = anchorPosition - sizes.GetPosition(anchor);
            }

            if (!double.IsInfinity(viewportSize))
            {
                position = Math.Max(0.0, Math.Min(position, sizes.TotalSize + shift - viewportSize));
            }

            first = anchor;
            double startPosition = anchorPosition;
            if (anchor > 0 && startPosition > position - cacheLength)
            {
                start = generator.GeneratorPositionFromIndex(anchor);
                using (generator.StartAt(start, GeneratorDirection.Backward, true))
                {
                    // The first generated container is the one of the anchor.
                    generator.GenerateNext(out _);

                    int insertAt = start.Offset == 0 ? start.Index : start.Index + 1;

                    for (int i = anchor - 1; i >= 0; i--)
                    {
                        UIElement child = (UIElement)generator.GenerateNext(out bool isNewlyRealized);
                        if (isNewlyRealized || !IsChildRealized(child, insertAt, children))
                        {
                            InsertInternalChild(Math.Max(insertAt, 0), child);
                            generator.PrepareItemContainer(child);
                        }
                        else
                        {
                            insertAt--;
                        }

                        double size = MeasureNativeChild(child, childConstraint, isHorizontal, ref stackDesiredSize);
                        sizes.SetSize(i, size);
                        startPosition -= size;
                        first = i;

                        if (startPosition <= position - cacheLength)
                        {
                            break;
                        }
                    }
                }
            }

            CleanupContainers(owner, first, last - first + 1);

            // Measuring the items may have changed their estimated position: keep the anchor in place.
            shift = anchorPosition - sizes.GetPosition(anchor);
            if (shift != 0.0 && (isRequested || first == 0 || position <= cacheLength))
            {
                // The content and the scroll offset are moved together, so this is not visible.
                if (!isRequested)
                {
                    _nativeScrollData._nativeScrollAdjustment -= shift;
                }

                position -= shift;
                shift = 0.0;
            }

            if (!double.IsInfinity(viewportSize))
            {
                position = Math.Max(0.0, Math.Min(position, sizes.TotalSize + shift - viewportSize));
            }

            _nativeScrollData._realizedStart = sizes.GetPosition(first) + shift;
            _nativeScrollData._realizedEnd = sizes.GetPosition(last + 1) + shift;
        }

        _nativeScrollData._nativeShift = shift;
        _nativeScrollData._nativeViewportSize = viewportSize;
        _nativeScrollData._realizedFirstIndex = first;
        _nativeScrollData._realizedLastIndex = last;

        // Offsets that are written to the DOM element after layout.
        if (isRequested)
        {
            _nativeScrollData._nativeTargetOffset = isHorizontal ?
                new Vector(position, _scrollData._offset.Y) :
                new Vector(_scrollData._offset.X, position);
        }
        else
        {
            if (isHorizontal)
            {
                _nativeScrollData._nativeOffset.X = position;
            }
            else
            {
                _nativeScrollData._nativeOffset.Y = position;
            }
        }

        Size viewport = constraint;
        Size extent = stackDesiredSize;
        double crossOffset;
        if (isHorizontal)
        {
            crossOffset = isRequested ? _scrollData._offset.Y : _nativeScrollData._nativeOffset.Y;
            crossOffset = ScrollContentPresenter.CoerceOffset(crossOffset, extent.Height, viewport.Height);
            _nativeScrollData._nativeTargetOffset.Y = crossOffset;
        }
        else
        {
            crossOffset = isRequested ? _scrollData._offset.X : _nativeScrollData._nativeOffset.X;
            crossOffset = ScrollContentPresenter.CoerceOffset(crossOffset, extent.Width, viewport.Width);
            _nativeScrollData._nativeTargetOffset.X = crossOffset;
        }

        double snappedPosition = isRequested ?
            position :
            ScrollContentPresenter.SnapNativeOffset(position, sizes.TotalSize + shift, viewportSize);
        SetNativeScrollData(isHorizontal, snappedPosition - shift, viewportSize, crossOffset, viewport, extent);

        stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
        stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
        // The panel is the browser's scroller. Arranged larger than the viewport, it would leave part
        // of the content out of the scroll range, so a size from a larger constraint must not be kept.
        _scrollData._maxDesiredSize.Width = Math.Min(Math.Max(stackDesiredSize.Width, _scrollData._maxDesiredSize.Width), constraint.Width);
        _scrollData._maxDesiredSize.Height = Math.Min(Math.Max(stackDesiredSize.Height, _scrollData._maxDesiredSize.Height), constraint.Height);

        return _scrollData._maxDesiredSize;
    }

    private static double MeasureNativeChild(UIElement child, Size constraint, bool isHorizontal, ref Size stackDesiredSize)
    {
        child.Measure(constraint);
        Size size = child.DesiredSize;

        if (isHorizontal)
        {
            stackDesiredSize.Width += size.Width;
            stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, size.Height);
            return size.Width;
        }

        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, size.Width);
        stackDesiredSize.Height += size.Height;
        return size.Height;
    }

    private Size ArrangeItemsHostNative(Size arrangeSize)
    {
        Debug.Assert(IsNativeScrolling);

        bool isHorizontal = Orientation == Orientation.Horizontal;
        List<UIElement> children = UnsafeGetChildren();
        IItemContainerGenerator generator = ItemContainerGenerator;
        ItemSizeCache sizes = _nativeScrollData._itemSizes;

        int previousIndex = -2;
        double position = 0.0;
        for (int i = 0; i < children.Count; i++)
        {
            UIElement child = children[i];
            int index = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
            if (sizes is null || index < 0 || index >= sizes.Count)
            {
                continue;
            }

            // Containers are sorted by index, and usually contiguous.
            position = index == previousIndex + 1 ?
                position + sizes.GetSize(previousIndex) :
                sizes.GetPosition(index) + _nativeScrollData._nativeShift;
            previousIndex = index;

            Size desiredSize = child.DesiredSize;
            child.Arrange(isHorizontal ?
                new Rect(position, 0.0, desiredSize.Width, Math.Max(arrangeSize.Height, desiredSize.Height)) :
                new Rect(0.0, position, Math.Max(arrangeSize.Width, desiredSize.Width), desiredSize.Height));
        }

        if (OuterDiv.IsConnected)
        {
            double stackExtent = Math.Max(0.0, (sizes?.TotalSize ?? 0.0) + _nativeScrollData._nativeShift);
            Size extent = isHorizontal ?
                new(stackExtent, Math.Max(arrangeSize.Height, _scrollData._extent.Height)) :
                new(Math.Max(arrangeSize.Width, _scrollData._extent.Width), stackExtent);

            if (!DoubleUtil.AreClose(_nativeScrollData._nativeExtent, extent))
            {
                _nativeScrollData._nativeExtent = extent;
                NativeScrollManager.SetExtent(OuterDiv, extent.Width, extent.Height);
            }

            _nativeScrollData._arrangedSize = arrangeSize;
            UpdateNativeScrolling();

            // Written after layout, once the DOM size of this panel and of its content are up to
            // date, otherwise the browser would clamp the offset.
            if ((_nativeScrollData._isNativeOffsetRequested || IsNativeOffsetUnknown || _nativeScrollData._nativeScrollAdjustment != 0.0) && !_nativeScrollData._isNativeOffsetSyncPending)
            {
                _nativeScrollData._isNativeOffsetSyncPending = true;
                LayoutUpdated += OnLayoutUpdatedSyncNativeOffset;
            }
        }

        return arrangeSize;
    }

    private void OnLayoutUpdatedSyncNativeOffset(object sender, EventArgs e)
    {
        LayoutUpdated -= OnLayoutUpdatedSyncNativeOffset;
        _nativeScrollData._isNativeOffsetSyncPending = false;

        bool isRequested = _nativeScrollData._isNativeOffsetRequested || IsNativeOffsetUnknown;
        double adjustment = _nativeScrollData._nativeScrollAdjustment;
        _nativeScrollData._isNativeOffsetRequested = false;
        _nativeScrollData._nativeScrollAdjustment = 0.0;

        if (!IsNativeScrolling || !OuterDiv.IsConnected)
        {
            return;
        }

        if (isRequested)
        {
            Vector offset = _nativeScrollData._nativeTargetOffset;
            if (IsNativeOffsetUnknown || Math.Abs(offset.X - _nativeScrollData._nativeOffset.X) >= 1.0 || Math.Abs(offset.Y - _nativeScrollData._nativeOffset.Y) >= 1.0)
            {
                _nativeScrollData._nativeOffset = offset;
                NativeScrollManager.ScrollTo(OuterDiv, offset.X, offset.Y);
            }
        }
        else if (adjustment != 0.0)
        {
            if (Orientation == Orientation.Horizontal)
            {
                NativeScrollManager.ScrollBy(OuterDiv, adjustment, 0.0);
            }
            else
            {
                NativeScrollManager.ScrollBy(OuterDiv, 0.0, adjustment);
            }
        }
    }

    private void OnItemsChangedNative(ItemsChangedEventArgs args)
    {
        // The indices of the realized range are not valid anymore.
        _nativeScrollData._realizedLastIndex = -1;

        if (args.Action == NotifyCollectionChangedAction.Reset)
        {
            // The new items may fit in the size limit of the browser.
            _nativeScrollData._isNativeExtentTooLarge = false;
        }

        ItemSizeCache sizes = _nativeScrollData._itemSizes;
        if (sizes is null)
        {
            return;
        }

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add when args.ItemIndex >= 0 && args.ItemIndex <= sizes.Count:
                sizes.Insert(args.ItemIndex, args.ItemCount);
                break;

            case NotifyCollectionChangedAction.Remove when args.ItemIndex >= 0 && args.ItemIndex + args.ItemCount <= sizes.Count:
                sizes.Remove(args.ItemIndex, args.ItemCount);
                break;

            case NotifyCollectionChangedAction.Move when args.OldItemIndex >= 0 && args.OldItemIndex < sizes.Count &&
                args.ItemIndex >= 0 && args.ItemIndex < sizes.Count:
                sizes.Move(args.OldItemIndex, args.ItemIndex);
                break;

            case NotifyCollectionChangedAction.Replace:
                // The size of the previous item is kept as an estimate.
                break;

            default:
                sizes.Reset(ItemsControl.GetItemsOwner(this)?.Items.Count ?? 0);
                break;
        }
    }

    private void ResetNativeScrolling()
    {
        _nativeScrollData._itemSizes = null;
        _nativeScrollData._isNativeExtentTooLarge = false;
        _nativeScrollData._nativeShift = 0.0;
        _nativeScrollData._isNativeOffsetRequested = _nativeScrollData._isNative;
    }

    // Helper class to hold native scrolling data.
    // This class exists to reduce working set when native scrolling is not used.
    private sealed class NativeScrollData
    {
        internal static readonly Vector UnknownOffset = new(double.NaN, double.NaN);

        // Size of the realized area before and after the viewport, and minimum margin left before more
        // containers are realized, relatively to the size of the viewport.
        internal const double NativeCacheLength = 0.5;
        internal const double NativeCacheMargin = 0.25;

        // Browsers limit the size of elements (about 17.9 million pixels in Firefox, 33.5 million in Chrome
        // and Safari, less when zoomed in). Longer lists are scrolled by the managed code.
        internal const double MaxNativeExtent = 10_000_000;

        internal HtmlElementReference _contentHost;
        internal State? _appliedNativeState;
        internal bool _isNative;
        internal bool _isNativeExtentTooLarge;
        internal ItemSizeCache _itemSizes;
        internal Size _nativeExtent;
        internal Size _arrangedSize = new(double.PositiveInfinity, double.PositiveInfinity);
        internal Vector _nativeOffset = UnknownOffset;
        internal Vector _nativeTargetOffset;
        internal double _nativeShift;
        internal double _nativeScrollAdjustment;
        internal bool _isNativeOffsetRequested;
        internal bool _isNativeOffsetSyncPending;
        internal double _nativeViewportSize;
        internal int _realizedFirstIndex;
        internal int _realizedLastIndex = -1;
        internal double _realizedStart;
        internal double _realizedEnd;
    }
}
