
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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace OpenSilver.Internal.Controls;

internal partial class TextViewBase : IScrollInfo
{
    private ScrollData _scrollData;

    internal bool IsScrollClient => _scrollData is not null;

    bool IScrollInfo.CanVerticallyScroll
    {
        get => _scrollData?.CanVerticallyScroll ?? false;
        set => _scrollData?.CanVerticallyScroll = value;
    }

    bool IScrollInfo.CanHorizontallyScroll
    {
        get => _scrollData?.CanHorizontallyScroll ?? false;
        set => _scrollData?.CanHorizontallyScroll = value;
    }

    double IScrollInfo.ExtentWidth => _scrollData?.ExtentWidth ?? 0.0;

    double IScrollInfo.ExtentHeight => _scrollData?.ExtentHeight ?? 0.0;

    double IScrollInfo.ViewportWidth => _scrollData?.ViewportWidth ?? 0.0;

    double IScrollInfo.ViewportHeight => _scrollData?.ViewportHeight ?? 0.0;

    double IScrollInfo.HorizontalOffset => _scrollData?.HorizontalOffset ?? 0.0;

    double IScrollInfo.VerticalOffset => _scrollData?.VerticalOffset ?? 0.0;

    ScrollViewer IScrollInfo.ScrollOwner
    {
        get => _scrollData?.ScrollOwner;
        set => (_scrollData ??= new ScrollData()).ScrollOwner = value;
    }

    void IScrollInfo.LineUp() => _scrollData?.LineUp(this);

    void IScrollInfo.LineDown() => _scrollData?.LineDown(this);

    void IScrollInfo.LineLeft() => _scrollData?.LineLeft(this);

    void IScrollInfo.LineRight() => _scrollData?.LineRight(this);

    void IScrollInfo.MouseWheelUp() => _scrollData?.MouseWheelUp(this);

    void IScrollInfo.MouseWheelDown() => _scrollData?.MouseWheelDown(this);

    void IScrollInfo.MouseWheelLeft() => _scrollData?.MouseWheelLeft(this);

    void IScrollInfo.MouseWheelRight() => _scrollData?.MouseWheelRight(this);

    void IScrollInfo.PageUp() => _scrollData?.PageUp(this);

    void IScrollInfo.PageDown() => _scrollData?.PageDown(this);

    void IScrollInfo.PageLeft() => _scrollData?.PageLeft(this);

    void IScrollInfo.PageRight() => _scrollData?.PageRight(this);

    void IScrollInfo.SetVerticalOffset(double offset) => _scrollData?.SetVerticalOffset(this, offset);

    void IScrollInfo.SetHorizontalOffset(double offset) => _scrollData?.SetHorizontalOffset(this, offset);

    Rect IScrollInfo.MakeVisible(UIElement visual, Rect rectangle)
    {
        if (_scrollData is null)
        {
            return Rect.Empty;
        }
        else
        {
            return _scrollData.MakeVisible(this, visual, rectangle);
        }
    }

    private void ArrangeScrollData(Size arrangeSize)
    {
        if (!IsScrollClient) return;

        bool invalidateScrollInfo = false;

        if (!DoubleUtil.AreClose(_scrollData.Viewport, arrangeSize))
        {
            _scrollData.Viewport = arrangeSize;
            invalidateScrollInfo = true;
        }

        if (!DoubleUtil.AreClose(_scrollData.Extent, _contentSize))
        {
            _scrollData.Extent = _contentSize;
            invalidateScrollInfo = true;
        }

        var offset = new Vector(
            Math.Max(0, Math.Min(_scrollData.ExtentWidth - _scrollData.ViewportWidth, _scrollData.HorizontalOffset)),
            Math.Max(0, Math.Min(_scrollData.ExtentHeight - _scrollData.ViewportHeight, _scrollData.VerticalOffset)));

        if (!DoubleUtil.AreClose(offset, _scrollData.Offset))
        {
            _scrollData.Offset = offset;
            invalidateScrollInfo = true;
        }

        if (invalidateScrollInfo)
        {
            _scrollData.ScrollOwner?.InvalidateScrollInfo();
        }
    }

    internal void UpdateOffsets(Vector offset)
    {
        Debug.Assert(IsScrollClient);

        if (!DoubleUtil.AreClose(_scrollData.Offset, offset))
        {
            _scrollData.Offset = offset;
            _scrollData.ScrollOwner?.InvalidateScrollInfo();
        }
    }

    private sealed class ScrollData
    {
        internal ScrollViewer ScrollOwner;
        internal Size Viewport;
        internal Size Extent;
        internal Vector Offset;
        internal bool CanHorizontallyScroll;
        internal bool CanVerticallyScroll;

        internal double HorizontalOffset => Offset.X;
        internal double VerticalOffset => Offset.Y;
        internal double ViewportWidth => Viewport.Width;
        internal double ViewportHeight => Viewport.Height;
        internal double ExtentWidth => Extent.Width;
        internal double ExtentHeight => Extent.Height;

        internal void SetVerticalOffset(UIElement owner, double offset)
        {
            if (!CanVerticallyScroll) return;

            if (owner.OuterDiv.IsConnected)
            {
                owner.OuterDiv.SetProperty("scrollTop", offset);
            }
        }

        internal void SetHorizontalOffset(UIElement owner, double offset)
        {
            if (!CanHorizontallyScroll) return;

            if (owner.OuterDiv.IsConnected)
            {
                owner.OuterDiv.SetProperty("scrollLeft", offset);
            }
        }

        internal void LineUp(UIElement owner) => SetVerticalOffset(owner, Offset.Y - ScrollViewer.LineDelta);

        internal void LineDown(UIElement owner) => SetVerticalOffset(owner, Offset.Y + ScrollViewer.LineDelta);

        internal void LineLeft(UIElement owner) => SetHorizontalOffset(owner, Offset.X - ScrollViewer.LineDelta);

        internal void LineRight(UIElement owner) => SetHorizontalOffset(owner, Offset.X + ScrollViewer.LineDelta);

        internal void MouseWheelUp(UIElement owner) => SetVerticalOffset(owner, Offset.Y - ScrollViewer.WheelDelta);

        internal void MouseWheelDown(UIElement owner) => SetVerticalOffset(owner, Offset.Y + ScrollViewer.WheelDelta);

        internal void MouseWheelLeft(UIElement owner) => SetHorizontalOffset(owner, Offset.X - ScrollViewer.WheelDelta);

        internal void MouseWheelRight(UIElement owner) => SetHorizontalOffset(owner, Offset.X + ScrollViewer.WheelDelta);

        internal void PageUp(UIElement owner) => SetVerticalOffset(owner, Offset.Y - Viewport.Height);

        internal void PageDown(UIElement owner) => SetVerticalOffset(owner, Offset.Y + Viewport.Height);

        internal void PageLeft(UIElement owner) => SetHorizontalOffset(owner, Offset.X - Viewport.Width);

        internal void PageRight(UIElement owner) => SetHorizontalOffset(owner, Offset.X + Viewport.Width);

        internal Rect MakeVisible(UIElement owner, UIElement visual, Rect rectangle)
        {
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (rectangle.IsEmpty || visual is null || (visual != owner && !owner.IsAncestorOf(visual)))
            {
                return Rect.Empty;
            }

            // Compute the child's rect relative to (0,0) in our coordinate space.
            Matrix childTransform = visual.InternalTransformToAncestor(owner);
            rectangle.Transform(childTransform);

            // Initialize the viewport.
            var viewport = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
            rectangle.X += viewport.X;
            rectangle.Y += viewport.Y;

            // Compute the offsets required to scroll the child into view.
            double minX = ComputeScrollOffset(viewport.Left, viewport.Right, rectangle.Left, rectangle.Right);
            double minY = ComputeScrollOffset(viewport.Top, viewport.Bottom, rectangle.Top, rectangle.Bottom);

            // We have computed the scrolling offsets; scroll to them.
            SetHorizontalOffset(owner, minX);
            SetVerticalOffset(owner, minY);

            // Compute the visible rectangle of the child relative to the viewport.
            if (CanHorizontallyScroll)
            {
                viewport.X = minX;
            }
            else
            {
                rectangle.X = viewport.X;
            }

            if (CanVerticallyScroll)
            {
                viewport.Y = minY;
            }
            else
            {
                rectangle.Y = viewport.Y;
            }

            rectangle.Intersect(viewport);

            if (!rectangle.IsEmpty)
            {
                rectangle.X -= viewport.X;
                rectangle.Y -= viewport.Y;
            }

            return rectangle;
        }

        private static double ComputeScrollOffset(double topView, double bottomView, double topChild, double bottomChild)
        {
            // # CHILD POSITION             REMEDY
            // 1 Above viewport             Align top edge of child & viewport
            // 2 Below viewport             Align top edge of child & viewport
            // 3 Entirely within viewport   No scroll
            // 4 Spanning viewport          Align top edge of child & viewport
            //
            // Note: "Above viewport" = childTop above viewportTop, childBottom above viewportBottom
            //       "Below viewport" = childTop below viewportTop, childBottom below viewportBottom
            // These child thus may overlap with the viewport, but will scroll the same direction

            bool topInView = DoubleUtil.GreaterThanOrClose(topChild, topView) && DoubleUtil.LessThan(topChild, bottomView);
            bool bottomInView = DoubleUtil.LessThanOrClose(bottomChild, bottomView) && DoubleUtil.GreaterThan(bottomChild, topView);

            if (topInView && bottomInView)
            {
                return topView;
            }

            return topChild;
        }
    }
}
