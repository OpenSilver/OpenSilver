
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
    private static readonly Vector UnknownOffset = new(double.NaN, double.NaN);

    // Size of the realized area before and after the viewport, and minimum margin left before more
    // containers are realized, relatively to the size of the viewport.
    private const double NativeCacheLength = 0.5;
    private const double NativeCacheMargin = 0.25;

    private HtmlElementReference _contentHost;
    private (NativeScrollManager.Mode Mode, bool CanScrollX, bool CanScrollY, bool IsOpaque)? _appliedNativeState;
    private bool _isNative;
    private ItemSizeCache _itemSizes;
    private Size _nativeExtent;
    private Vector _nativeOffset = UnknownOffset;
    private Vector _nativeTargetOffset;
    private double _nativeShift;
    private double _nativeScrollAdjustment;
    private bool _isNativeOffsetRequested;
    private bool _isNativeOffsetSyncPending;
    private double _nativeViewportSize;
    private int _realizedFirstIndex;
    private int _realizedLastIndex = -1;
    private double _realizedStart;
    private double _realizedEnd;

    bool INativeScrollInfo.IsNativeScrolling => _isNative;

    Vector INativeScrollInfo.NativeOffset => _isNative ? GetKnownNativeOffset() : default;

    internal bool IsNativeScrolling => _isNative;

    internal override bool EnablePointerEventsCore => _isNative || Background is not null;

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
        _appliedNativeState = null;
        _nativeExtent = default;
        _nativeOffset = UnknownOffset;
        _nativeShift = 0.0;
        _nativeScrollAdjustment = 0.0;
    }

    /// <inheritdoc />
    protected override Geometry GetLayoutClip(Size layoutSlotSize) => _isNative ? null : base.GetLayoutClip(layoutSlotSize);

    // Natively scrolled when this panel scrolls the items of an ItemsControl (and native scrolling is
    // allowed). Otherwise, the ScrollContentPresenter of the ScrollViewer uses the managed scrolling.
    internal void UpdateNativeScrolling() => UpdateNativeScrolling(false);

    // During measure, the ScrollViewer updates its ScrollContentPresenter after this panel is measured.
    private void UpdateNativeScrolling(bool isMeasuring)
    {
        if (!_contentHost.IsConnected)
        {
            SetIsNative(false, isMeasuring);
            return;
        }

        bool isNative = IsItemsHost &&
            IsScrolling &&
            (Orientation == Orientation.Horizontal ? CanHorizontallyScroll : CanVerticallyScroll) &&
            NativeScrolling.GetIsEnabled(this);

        var state = isNative ?
            (NativeScrollManager.Mode.Native, CanHorizontallyScroll, CanVerticallyScroll, Background is not null) :
            (NativeScrollManager.Mode.None, false, false, false);

        if (_appliedNativeState == state)
        {
            return;
        }

        bool modeChanged = _appliedNativeState?.Mode != state.Item1;
        _appliedNativeState = state;

        NativeScrollManager.SetMode(OuterDiv, state.Item1, state.Item2, state.Item3, state.Item4);

        if (modeChanged)
        {
            _nativeExtent = default;
            _nativeOffset = UnknownOffset;
            _nativeShift = 0.0;
            _nativeScrollAdjustment = 0.0;
            _isNativeOffsetRequested = false;
            _realizedLastIndex = -1;
        }

        SetIsNative(isNative, isMeasuring);
    }

    private void SetIsNative(bool value, bool isMeasuring)
    {
        if (_isNative == value)
        {
            return;
        }

        _isNative = value;

        // The scroller must be hit-testable for the browser to scroll it when the pointer is over
        // an area that is not covered by the items.
        CoerceIsHitTestable();

        if (!isMeasuring)
        {
            InvalidateMeasure();

            // The ScrollContentPresenter must stop (or start) scrolling the panel itself.
            ScrollOwner?.InvalidateMeasure();
        }
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Browsers drop the scroll position of elements that are not rendered (display: none).
        if (_isNative && (bool)e.NewValue)
        {
            _nativeOffset = UnknownOffset;
            InvalidateMeasure();
        }
    }

    private Vector GetKnownNativeOffset() =>
        new(double.IsNaN(_nativeOffset.X) ? 0.0 : _nativeOffset.X, double.IsNaN(_nativeOffset.Y) ? 0.0 : _nativeOffset.Y);

    private bool IsNativeOffsetUnknown => double.IsNaN(_nativeOffset.X);

    void INativeScrollInfo.HandleNativeMouseWheel(MouseWheelEventArgs e)
    {
        Debug.Assert(_isNative);

        NativeScrollManager.HandleMouseWheel(OuterDiv, e);
    }

    void INativeScrollInfo.OnNativeScroll(double x, double y)
    {
        if (!_isNative || !IsScrolling)
        {
            return;
        }

        _nativeOffset = new Vector(x, y);

        // An offset set in C# and not yet written to the DOM wins over the scroll that preceded it.
        if (_isNativeOffsetRequested || _nativeScrollAdjustment != 0.0 || _itemSizes is null)
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
        ItemSizeCache sizes = _itemSizes;
        double viewport = _nativeViewportSize;

        double position = ScrollContentPresenter.SnapNativeOffset(
            isHorizontal ? _nativeOffset.X : _nativeOffset.Y,
            sizes.TotalSize + _nativeShift,
            viewport);

        double crossOffset = ScrollContentPresenter.SnapNativeOffset(
            isHorizontal ? _nativeOffset.Y : _nativeOffset.X,
            isHorizontal ? _scrollData._extent.Height : _scrollData._extent.Width,
            isHorizontal ? _scrollData._viewport.Height : _scrollData._viewport.Width);

        SetNativeScrollData(isHorizontal, position - _nativeShift, viewport, crossOffset, _scrollData._viewport, _scrollData._extent);
    }

    private void SetNativeScrollData(bool isHorizontal, double position, double viewportSize, double crossOffset, Size viewport, Size extent)
    {
        ItemSizeCache sizes = _itemSizes;
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
        ItemSizeCache sizes = _itemSizes;
        double viewport = _nativeViewportSize;
        if (!_isNative || sizes is null || childIndex < 0 || childIndex >= sizes.Count || double.IsInfinity(viewport))
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
        if (_realizedLastIndex < _realizedFirstIndex)
        {
            return false;
        }

        double viewport = _nativeViewportSize;
        if (double.IsInfinity(viewport))
        {
            return true;
        }

        double position = Orientation == Orientation.Horizontal ? _nativeOffset.X : _nativeOffset.Y;
        double margin = viewport * NativeCacheMargin;

        // The shift is resolved when the beginning of the list gets close.
        if (_nativeShift != 0.0 && position <= viewport * NativeCacheLength)
        {
            return false;
        }

        return (_realizedFirstIndex == 0 || _realizedStart <= position - margin) &&
            (_realizedLastIndex >= _itemSizes.Count - 1 || _realizedEnd >= position + viewport + margin);
    }

    private Size MeasureItemsHostNative(Size constraint)
    {
        Debug.Assert(_isNative && IsScrolling);

        // Ensure we always touch ItemContainerGenerator as by accessing this property
        // we hook up to some events on it.
        EnsureGenerator();
        bool isHorizontal = Orientation == Orientation.Horizontal;
        ItemsControl owner = ItemsControl.GetItemsOwner(this);
        int itemCount = owner.Items.Count;
        Size stackDesiredSize = new();

        SetVirtualizationState(owner);

        ItemSizeCache sizes = _itemSizes ??= new ItemSizeCache();
        sizes.SetCount(itemCount);

        double viewportSize = isHorizontal ? constraint.Width : constraint.Height;
        double cacheLength = double.IsInfinity(viewportSize) ? 0.0 : viewportSize * NativeCacheLength;

        bool isRequested = _isNativeOffsetRequested || IsNativeOffsetUnknown;
        double position = 0.0;
        double shift = isRequested ? 0.0 : _nativeShift;
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
                position = isHorizontal ? _nativeOffset.X : _nativeOffset.Y;
                anchor = sizes.IndexAt(position - shift);

                // When scrolling backward, the items realized before must not move while visible.
                if (anchor < _realizedFirstIndex && _realizedFirstIndex <= _realizedLastIndex &&
                    _realizedLastIndex < itemCount && _realizedStart < position + viewportSize)
                {
                    anchor = _realizedFirstIndex;
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
                    _nativeScrollAdjustment -= shift;
                }

                position -= shift;
                shift = 0.0;
            }

            if (!double.IsInfinity(viewportSize))
            {
                position = Math.Max(0.0, Math.Min(position, sizes.TotalSize + shift - viewportSize));
            }

            _realizedStart = sizes.GetPosition(first) + shift;
            _realizedEnd = sizes.GetPosition(last + 1) + shift;
        }

        _nativeShift = shift;
        _nativeViewportSize = viewportSize;
        _realizedFirstIndex = first;
        _realizedLastIndex = last;

        // Offsets that are written to the DOM element after layout.
        if (isRequested)
        {
            _nativeTargetOffset = isHorizontal ?
                new Vector(position, _scrollData._offset.Y) :
                new Vector(_scrollData._offset.X, position);
        }
        else
        {
            if (isHorizontal)
            {
                _nativeOffset.X = position;
            }
            else
            {
                _nativeOffset.Y = position;
            }
        }

        Size viewport = constraint;
        Size extent = stackDesiredSize;
        double crossOffset;
        if (isHorizontal)
        {
            crossOffset = isRequested ? _scrollData._offset.Y : _nativeOffset.Y;
            crossOffset = ScrollContentPresenter.CoerceOffset(crossOffset, extent.Height, viewport.Height);
            _nativeTargetOffset.Y = crossOffset;
        }
        else
        {
            crossOffset = isRequested ? _scrollData._offset.X : _nativeOffset.X;
            crossOffset = ScrollContentPresenter.CoerceOffset(crossOffset, extent.Width, viewport.Width);
            _nativeTargetOffset.X = crossOffset;
        }

        double snappedPosition = isRequested ?
            position :
            ScrollContentPresenter.SnapNativeOffset(position, sizes.TotalSize + shift, viewportSize);
        SetNativeScrollData(isHorizontal, snappedPosition - shift, viewportSize, crossOffset, viewport, extent);

        stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
        stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
        _scrollData._maxDesiredSize.Width = Math.Max(stackDesiredSize.Width, _scrollData._maxDesiredSize.Width);
        _scrollData._maxDesiredSize.Height = Math.Max(stackDesiredSize.Height, _scrollData._maxDesiredSize.Height);

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
        Debug.Assert(_isNative);

        bool isHorizontal = Orientation == Orientation.Horizontal;
        List<UIElement> children = UnsafeGetChildren();
        IItemContainerGenerator generator = ItemContainerGenerator;
        ItemSizeCache sizes = _itemSizes;

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
                sizes.GetPosition(index) + _nativeShift;
            previousIndex = index;

            Size desiredSize = child.DesiredSize;
            child.Arrange(isHorizontal ?
                new Rect(position, 0.0, desiredSize.Width, Math.Max(arrangeSize.Height, desiredSize.Height)) :
                new Rect(0.0, position, Math.Max(arrangeSize.Width, desiredSize.Width), desiredSize.Height));
        }

        if (OuterDiv.IsConnected)
        {
            double stackExtent = Math.Max(0.0, (sizes?.TotalSize ?? 0.0) + _nativeShift);
            Size extent = isHorizontal ?
                new(stackExtent, Math.Max(arrangeSize.Height, _scrollData._extent.Height)) :
                new(Math.Max(arrangeSize.Width, _scrollData._extent.Width), stackExtent);

            if (!DoubleUtil.AreClose(_nativeExtent, extent))
            {
                _nativeExtent = extent;
                NativeScrollManager.SetExtent(OuterDiv, extent.Width, extent.Height);
            }

            // Written after layout, once the DOM size of this panel and of its content are up to
            // date, otherwise the browser would clamp the offset.
            if ((_isNativeOffsetRequested || IsNativeOffsetUnknown || _nativeScrollAdjustment != 0.0) && !_isNativeOffsetSyncPending)
            {
                _isNativeOffsetSyncPending = true;
                LayoutUpdated += OnLayoutUpdatedSyncNativeOffset;
            }
        }

        return arrangeSize;
    }

    private void OnLayoutUpdatedSyncNativeOffset(object sender, EventArgs e)
    {
        LayoutUpdated -= OnLayoutUpdatedSyncNativeOffset;
        _isNativeOffsetSyncPending = false;

        bool isRequested = _isNativeOffsetRequested || IsNativeOffsetUnknown;
        double adjustment = _nativeScrollAdjustment;
        _isNativeOffsetRequested = false;
        _nativeScrollAdjustment = 0.0;

        if (!_isNative || !OuterDiv.IsConnected)
        {
            return;
        }

        if (isRequested)
        {
            Vector offset = _nativeTargetOffset;
            if (IsNativeOffsetUnknown || Math.Abs(offset.X - _nativeOffset.X) >= 1.0 || Math.Abs(offset.Y - _nativeOffset.Y) >= 1.0)
            {
                _nativeOffset = offset;
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
        _realizedLastIndex = -1;

        ItemSizeCache sizes = _itemSizes;
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
        _itemSizes = null;
        _nativeShift = 0.0;
        _isNativeOffsetRequested = _isNative;
    }
}
