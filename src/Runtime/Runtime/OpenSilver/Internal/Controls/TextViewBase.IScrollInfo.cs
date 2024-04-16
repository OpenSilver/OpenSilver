
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
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal partial class TextViewBase : IScrollInfo
{
    private ScrollData _scrollData;

    internal bool IsScrollClient => _scrollData is not null;

    public bool CanVerticallyScroll
    {
        get { return IsScrollClient ? _scrollData.CanVerticallyScroll : false; }
        set { if (IsScrollClient) _scrollData.CanVerticallyScroll = value; }
    }

    public bool CanHorizontallyScroll
    {
        get { return IsScrollClient ? _scrollData.CanHorizontallyScroll : false; }
        set { if (IsScrollClient) _scrollData.CanHorizontallyScroll = value; }
    }

    public double ExtentWidth => IsScrollClient ? _scrollData.ExtentWidth : 0.0;

    public double ExtentHeight => IsScrollClient ? _scrollData.ExtentHeight : 0.0;

    public double ViewportWidth => IsScrollClient ? _scrollData.ViewportWidth : 0.0;

    public double ViewportHeight => IsScrollClient ? _scrollData.ViewportHeight : 0.0;

    public double HorizontalOffset => IsScrollClient ? _scrollData.HorizontalOffset : 0.0;

    public double VerticalOffset => IsScrollClient ? _scrollData.VerticalOffset : 0.0;

    public ScrollViewer ScrollOwner
    {
        get => IsScrollClient ? _scrollData.ScrollOwner : null;
        set => (_scrollData ??= new ScrollData()).ScrollOwner = value;
    }

    public void LineDown()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset + ScrollViewer.LineDelta);
        }
    }

    public void LineLeft()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset - ScrollViewer.LineDelta);
        }
    }

    public void LineRight()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset + ScrollViewer.LineDelta);
        }
    }

    public void LineUp()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset - ScrollViewer.LineDelta);
        }
    }

    public void MouseWheelDown()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset + ScrollViewer.WheelDelta);
        }
    }

    public void MouseWheelLeft()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset - ScrollViewer.WheelDelta);
        }
    }

    public void MouseWheelRight()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset + ScrollViewer.WheelDelta);
        }
    }

    public void MouseWheelUp()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset - ScrollViewer.WheelDelta);
        }
    }

    public void PageDown()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset + _scrollData.ViewportHeight);
        }
    }

    public void PageLeft()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset - _scrollData.ViewportWidth);
        }
    }

    public void PageRight()
    {
        if (IsScrollClient)
        {
            SetHorizontalOffset(_scrollData.HorizontalOffset + _scrollData.ViewportWidth);
        }
    }

    public void PageUp()
    {
        if (IsScrollClient)
        {
            SetVerticalOffset(_scrollData.VerticalOffset - _scrollData.ViewportHeight);
        }
    }

    public void SetHorizontalOffset(double offset)
    {
        if (!IsScrollClient) return;
        if (!_scrollData.CanHorizontallyScroll) return;

        if (OuterDiv is not null)
        {
            INTERNAL_HtmlDomManager.SetDomElementProperty(OuterDiv, "scrollLeft", offset);
        }
    }

    public void SetVerticalOffset(double offset)
    {
        if (!IsScrollClient) return;
        if (!_scrollData.CanVerticallyScroll) return;

        if (OuterDiv is not null)
        {
            INTERNAL_HtmlDomManager.SetDomElementProperty(OuterDiv, "scrollTop", offset);
        }
    }

    [NotImplemented]
    public Rect MakeVisible(UIElement visual, Rect rectangle) => default;

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

        var offset = new Point(
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

    internal void UpdateOffsets(Point offset)
    {
        Debug.Assert(IsScrollClient);

        if (!DoubleUtil.AreClose(_scrollData.Offset, offset))
        {
            _scrollData.Offset = offset;
            _scrollData.ScrollOwner?.InvalidateScrollInfo();
        }
    }
}
