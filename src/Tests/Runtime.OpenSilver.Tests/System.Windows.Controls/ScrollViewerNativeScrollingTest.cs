
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;
using OpenSilver.Internal.Controls;
using Runtime.OpenSilver.Tests;

namespace System.Windows.Controls.Tests;

[TestClass]
public class ScrollViewerNativeScrollingTest
{
    private const double ViewportSize = 100;
    private const double ContentHeight = 500;

    private sealed class Host : IDisposable
    {
        private readonly Popup _popup;
        private readonly List<string> _javascript = new();

        public Host(ScrollViewer scrollViewer, UIElement root = null)
        {
            TestSetup.ExecuteJavascript += OnExecuteJavascript;

            ScrollViewer = scrollViewer;
            _popup = new Popup { Child = root ?? scrollViewer, IsOpen = true };
            UpdateLayout();
        }

        public ScrollViewer ScrollViewer { get; }

        public ScrollContentPresenter Presenter => ScrollViewer.ElementScrollContentPresenter;

        public string PresenterId => Presenter.OuterDiv.Uid;

        public void UpdateLayout()
        {
            // The ScrollViewer executes its scrolling commands in successive layout passes.
            for (int i = 0; i < 4; i++)
            {
                ScrollViewer.UpdateLayout();
            }
        }

        // Flushes the pending (asynchronous) JavaScript and returns everything executed so far.
        public string FlushJavaScript()
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("void 0");
            string js = string.Join("\n", _javascript);
            _javascript.Clear();
            return js;
        }

        public void SimulateNativeScroll(double x, double y)
        {
            NativeScrollManager.OnScrollNative(PresenterId, x, y);
            UpdateLayout();
        }

        public void Dispose()
        {
            _popup.IsOpen = false;
            TestSetup.ExecuteJavascript -= OnExecuteJavascript;
        }

        private void OnExecuteJavascript(object sender, ExecuteJavascriptEventArgs e) => _javascript.Add(e.Javascript);
    }

    private static ScrollViewer CreateScrollViewer(out Border content)
    {
        content = new Border
        {
            Width = 50,
            Height = ContentHeight,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Background = new SolidColorBrush(Colors.Red),
        };

        return new ScrollViewer
        {
            Width = ViewportSize,
            Height = ViewportSize,
            Padding = new Thickness(0),
            BorderThickness = new Thickness(0),
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = content,
        };
    }

    [TestMethod]
    public void ScrollContentPresenter_Should_Scroll_Natively_By_Default()
    {
        using var host = new Host(CreateScrollViewer(out Border content));

        Assert.IsTrue(host.Presenter.IsNativeScrolling);
        Assert.AreSame(host.Presenter, host.ScrollViewer.ScrollInfo);
        Assert.AreEqual(ContentHeight, host.ScrollViewer.ExtentHeight);
        Assert.AreEqual(Visibility.Visible, host.ScrollViewer.ComputedVerticalScrollBarVisibility);
        Assert.AreEqual(new Point(0, 0), content.TransformToVisual(host.Presenter).Transform(new Point(0, 0)));
    }

    [TestMethod]
    public void ScrollToVerticalOffset_Should_Write_Native_Offset_And_Keep_Content_At_Origin()
    {
        using var host = new Host(CreateScrollViewer(out Border content));
        host.FlushJavaScript();

        host.ScrollViewer.ScrollToVerticalOffset(200);
        host.UpdateLayout();

        Assert.AreEqual(200, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(new Vector(0, 0), content.VisualOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,200)");
        Assert.AreEqual(new Point(0, -200), content.TransformToVisual(host.Presenter).Transform(new Point(0, 0)));
    }

    [TestMethod]
    public void Native_Scroll_Should_Update_ScrollViewer_Without_Writing_It_Back()
    {
        using var host = new Host(CreateScrollViewer(out Border content));
        int scrollChangedCount = 0;
        host.ScrollViewer.ScrollChanged += (o, e) => scrollChangedCount++;
        host.FlushJavaScript();

        host.SimulateNativeScroll(0, 150);

        Assert.AreEqual(150, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(1, scrollChangedCount);
        Assert.AreEqual(new Vector(0, 0), content.VisualOffset);
        Assert.IsFalse(host.FlushJavaScript().Contains("osjs.nativeScroll.scrollTo"));
        Assert.AreEqual(new Point(0, -150), content.TransformToVisual(host.Presenter).Transform(new Point(0, 0)));
    }

    [TestMethod]
    public void Native_Scroll_Should_Snap_To_Bounds()
    {
        using var host = new Host(CreateScrollViewer(out _));
        double scrollableHeight = host.ScrollViewer.ScrollableHeight;

        host.SimulateNativeScroll(0, scrollableHeight - 0.4);
        Assert.AreEqual(scrollableHeight, host.ScrollViewer.VerticalOffset);

        host.SimulateNativeScroll(0, scrollableHeight + 20); // overscroll (Safari)
        Assert.AreEqual(scrollableHeight, host.ScrollViewer.VerticalOffset);

        host.SimulateNativeScroll(0, 0.6);
        Assert.AreEqual(0, host.ScrollViewer.VerticalOffset);
    }

    [TestMethod]
    public void Layout_Changes_Should_Not_Write_Back_Native_Offset()
    {
        using var host = new Host(CreateScrollViewer(out Border content));
        host.SimulateNativeScroll(0, 150);
        host.FlushJavaScript();

        // The offset of an ongoing (inertial) scroll must not be overwritten by a stale value.
        content.Height = ContentHeight + 100;
        host.UpdateLayout();

        Assert.AreEqual(ContentHeight + 100, host.ScrollViewer.ExtentHeight);
        string js = host.FlushJavaScript();
        StringAssert.Contains(js, $"osjs.nativeScroll.setExtent('{host.PresenterId}',");
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollTo"));
    }

    [TestMethod]
    public void Requested_Offset_Should_Win_Over_Preceding_Native_Scroll()
    {
        using var host = new Host(CreateScrollViewer(out _));
        host.FlushJavaScript();

        // The scroll event was raised by the browser before the requested offset is written to the DOM.
        host.Presenter.SetVerticalOffset(300);
        NativeScrollManager.OnScrollNative(host.PresenterId, 0, 50);
        host.UpdateLayout();

        Assert.AreEqual(300, host.ScrollViewer.VerticalOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,300)");
    }

    [TestMethod]
    public void Offset_Should_Be_Restored_When_Visible_Again()
    {
        using var host = new Host(CreateScrollViewer(out _));
        host.SimulateNativeScroll(0, 150);

        // Browsers reset the scroll position of elements with "display: none".
        host.ScrollViewer.Visibility = Visibility.Collapsed;
        host.UpdateLayout();
        host.FlushJavaScript();

        host.ScrollViewer.Visibility = Visibility.Visible;
        host.UpdateLayout();

        Assert.AreEqual(150, host.ScrollViewer.VerticalOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,150)");
    }

    [TestMethod]
    public void Disabling_ScrollBar_Should_Coerce_Offset_And_Restore_It_When_Enabled_Again()
    {
        using var host = new Host(CreateScrollViewer(out _));
        host.SimulateNativeScroll(0, 150);

        host.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        host.UpdateLayout();
        // The browser clamps the scroll position of the scroller, which is no longer scrollable.
        host.SimulateNativeScroll(0, 0);

        Assert.AreEqual(0, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(0, host.ScrollViewer.ScrollableHeight);
        host.FlushJavaScript();

        host.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        host.UpdateLayout();

        Assert.AreEqual(150, host.ScrollViewer.VerticalOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,150)");
    }

    [TestMethod]
    public void NativeScrolling_IsEnabled_False_Should_Use_Managed_Scrolling()
    {
        ScrollViewer sv = CreateScrollViewer(out Border content);
        NativeScrolling.SetIsEnabled(sv, false);

        using var host = new Host(sv);
        string js = host.FlushJavaScript();

        Assert.IsFalse(host.Presenter.IsNativeScrolling);
        // Managed scrolling captures touches when a scrollbar is visible (touch-action: none).
        StringAssert.Contains(js, $"osjs.nativeScroll.setMode('{host.PresenterId}',2,false,true)");

        host.ScrollViewer.ScrollToVerticalOffset(100);
        host.UpdateLayout();

        Assert.AreEqual(100, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(new Vector(0, -100), content.VisualOffset);
        Assert.IsFalse(host.FlushJavaScript().Contains("osjs.nativeScroll.scrollTo"));
    }

    [TestMethod]
    public void NativeScrolling_IsEnabled_Should_Switch_Mode_Dynamically()
    {
        using var host = new Host(CreateScrollViewer(out Border content));
        host.ScrollViewer.ScrollToVerticalOffset(120);
        host.UpdateLayout();

        NativeScrolling.SetIsEnabled(host.ScrollViewer, false);
        host.UpdateLayout();

        Assert.IsFalse(host.Presenter.IsNativeScrolling);
        Assert.AreEqual(120, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(new Vector(0, -120), content.VisualOffset);

        host.FlushJavaScript();
        NativeScrolling.SetIsEnabled(host.ScrollViewer, true);
        host.UpdateLayout();

        Assert.IsTrue(host.Presenter.IsNativeScrolling);
        Assert.AreEqual(new Vector(0, 0), content.VisualOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,120)");
    }

    [TestMethod]
    public void NativeScrolling_IsEnabled_Current_Value_Should_Be_Inherited()
    {
        // DragDropTarget opts out its content this way, so that touches can start drags.
        var inner = CreateScrollViewer(out _);
        var container = new ContentControl { Content = inner };
        container.SetCurrentValue(NativeScrolling.IsEnabledProperty, false);

        using var host = new Host(new ScrollViewer { Content = container });

        Assert.IsTrue(host.Presenter.IsNativeScrolling);
        Assert.IsFalse(inner.ElementScrollContentPresenter.IsNativeScrolling);
    }

    [TestMethod]
    public void Managed_Scrolling_Without_Visible_ScrollBar_Should_Not_Capture_Touches()
    {
        ScrollViewer sv = CreateScrollViewer(out Border content);
        content.Height = 10;
        NativeScrolling.SetIsEnabled(sv, false);

        using var host = new Host(sv);

        Assert.AreEqual(Visibility.Collapsed, host.ScrollViewer.ComputedVerticalScrollBarVisibility);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setMode('{host.PresenterId}',2,false,false)");
    }

    [TestMethod]
    public void Native_Scrolling_Should_Leave_Touches_To_The_Application_While_The_Content_Fits()
    {
        ScrollViewer sv = CreateScrollViewer(out Border content);
        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        content.Height = ViewportSize;

        using var host = new Host(sv);
        string js = host.FlushJavaScript();

        Assert.IsTrue(host.Presenter.IsNativeScrolling);
        Assert.IsFalse(((INativeScrollInfo)host.Presenter).CanNativelyScroll);
        StringAssert.Contains(js, $"osjs.nativeScroll.setMode('{host.PresenterId}',1,false,false)");
        Assert.IsFalse(js.Contains($"osjs.nativeScroll.setMode('{host.PresenterId}',1,false,true)"));

        // The desired size of the presenter does not change, so the ScrollViewer is not measured again.
        content.Height = ContentHeight;
        host.UpdateLayout();

        Assert.IsTrue(((INativeScrollInfo)host.Presenter).CanNativelyScroll);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setMode('{host.PresenterId}',1,false,true)");

        host.SimulateNativeScroll(0, 150);
        host.FlushJavaScript();
        content.Height = 10;
        host.UpdateLayout();

        Assert.AreEqual(0, host.ScrollViewer.VerticalOffset);
        Assert.IsFalse(((INativeScrollInfo)host.Presenter).CanNativelyScroll);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setMode('{host.PresenterId}',1,false,false)");
    }

    private sealed class ArrangeLargerThanMeasurePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(ViewportSize, ViewportSize));
            }
            return new Size(ViewportSize, ViewportSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(0, 0, ViewportSize, 2 * ContentHeight));
            }
            return finalSize;
        }
    }

    [TestMethod]
    public void Measure_Should_Not_Toggle_Native_Axes_When_Arranged_Larger_Than_Measured()
    {
        ScrollViewer sv = CreateScrollViewer(out _);
        sv.Width = double.NaN;
        sv.Height = double.NaN;
        var panel = new ArrangeLargerThanMeasurePanel();
        panel.Children.Add(sv);

        using var host = new Host(sv, panel);
        Assert.IsFalse(((INativeScrollInfo)host.Presenter).CanNativelyScroll);
        host.FlushJavaScript();

        host.ScrollViewer.InvalidateMeasure();
        host.UpdateLayout();

        string js = host.FlushJavaScript();
        Assert.IsFalse(js.Contains("osjs.nativeScroll.setMode"), js);
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollTo"), js);
    }

    private const double ItemHeight = 50;

    private static StackPanel CreateFocusableItems(int count)
    {
        var panel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        for (int i = 0; i < count; i++)
        {
            panel.Children.Add(new Control { Width = 50, Height = ItemHeight });
        }
        return panel;
    }

    [TestMethod]
    public void Focus_Should_Bring_The_Focused_Element_Into_View_Natively()
    {
        ScrollViewer sv = CreateScrollViewer(out _);
        StackPanel items = CreateFocusableItems(10);
        sv.Content = items;

        using var host = new Host(sv);
        host.FlushJavaScript();

        Assert.IsTrue(((Control)items.Children[5]).Focus());
        host.UpdateLayout();

        Assert.AreEqual(200, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(new Vector(0, 0), items.VisualOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,200)");
    }

    [TestMethod]
    public void Focus_Should_Scroll_Minimally_From_The_Native_Offset()
    {
        ScrollViewer sv = CreateScrollViewer(out _);
        StackPanel items = CreateFocusableItems(10);
        sv.Content = items;

        using var host = new Host(sv);
        host.SimulateNativeScroll(0, 300);
        host.FlushJavaScript();

        Assert.IsTrue(((Control)items.Children[1]).Focus());
        host.UpdateLayout();

        Assert.AreEqual(50, host.ScrollViewer.VerticalOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,50)");

        Assert.IsTrue(((Control)items.Children[3]).Focus());
        host.UpdateLayout();

        Assert.AreEqual(100, host.ScrollViewer.VerticalOffset);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,100)");

        Assert.IsTrue(((Control)items.Children[2]).Focus());
        host.UpdateLayout();

        Assert.AreEqual(100, host.ScrollViewer.VerticalOffset);
        Assert.IsFalse(host.FlushJavaScript().Contains("osjs.nativeScroll.scrollTo"));
    }

    [TestMethod]
    public void Focus_Should_Bring_The_Focused_Element_Into_View_In_Nested_Native_ScrollViewers()
    {
        ScrollViewer inner = CreateScrollViewer(out _);
        StackPanel items = CreateFocusableItems(10);
        inner.Content = items;

        ScrollViewer outer = CreateScrollViewer(out _);
        outer.Content = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            Children = { new Border { Height = 300 }, inner },
        };

        using var host = new Host(outer);
        Assert.IsTrue(inner.ElementScrollContentPresenter.IsNativeScrolling);
        host.FlushJavaScript();

        Assert.IsTrue(((Control)items.Children[5]).Focus());
        host.UpdateLayout();

        Assert.AreEqual(200, inner.VerticalOffset);
        Assert.AreEqual(300, outer.VerticalOffset);
        string js = host.FlushJavaScript();
        StringAssert.Contains(js, $"osjs.nativeScroll.scrollTo('{inner.ElementScrollContentPresenter.OuterDiv.Uid}',0,200)");
        StringAssert.Contains(js, $"osjs.nativeScroll.scrollTo('{host.PresenterId}',0,300)");
    }

    [TestMethod]
    public void Feature_Disabled_Should_Use_Managed_Scrolling()
    {
        Features.Scrolling.UseNativeScrolling = false;
        try
        {
            using var host = new Host(CreateScrollViewer(out Border content));

            Assert.IsFalse(host.Presenter.IsNativeScrolling);
            Assert.IsFalse(host.FlushJavaScript().Contains("osjs.nativeScroll"));

            host.ScrollViewer.ScrollToVerticalOffset(100);
            host.UpdateLayout();
            Assert.AreEqual(new Vector(0, -100), content.VisualOffset);
        }
        finally
        {
            Features.Scrolling.UseNativeScrolling = true;
        }
    }

    [TestMethod]
    public void SnapNativeOffset_Should_Clamp_And_Snap()
    {
        Assert.AreEqual(0, ScrollContentPresenter.SnapNativeOffset(-5, 500, 100));
        Assert.AreEqual(0, ScrollContentPresenter.SnapNativeOffset(0.9, 500, 100));
        Assert.AreEqual(42.5, ScrollContentPresenter.SnapNativeOffset(42.5, 500, 100));
        Assert.AreEqual(400, ScrollContentPresenter.SnapNativeOffset(399.1, 500, 100));
        Assert.AreEqual(400, ScrollContentPresenter.SnapNativeOffset(450, 500, 100));
        Assert.AreEqual(0, ScrollContentPresenter.SnapNativeOffset(10, 50, 100));
    }

    [TestMethod]
    public void AllowScrollOnTouchMove_False_Should_Disable_Browser_Panning()
    {
        var thumb = new Thumb { Width = 10, Height = 10 };
        var border = new Border { Width = 10, Height = 10 };

        using var host = new Host(new ScrollViewer { Content = new StackPanel { Children = { thumb, border } } });
        string js = host.FlushJavaScript();

        // Thumb overrides the default value to false.
        StringAssert.Contains(js, $"'{thumb.OuterDiv.Uid}'");
        Assert.IsTrue(ContainsTouchActionNone(js, thumb.OuterDiv.Uid));
        Assert.IsFalse(ContainsTouchActionNone(js, border.OuterDiv.Uid));

        border.AllowScrollOnTouchMove = false;
        Assert.IsTrue(ContainsTouchActionNone(host.FlushJavaScript(), border.OuterDiv.Uid));
    }

    private static bool ContainsTouchActionNone(string js, string id)
    {
        foreach (string line in js.Split('\n'))
        {
            if (line.Contains($"'{id}'") && line.Contains("touch-action") && line.Contains("none"))
            {
                return true;
            }
        }
        return false;
    }
}
