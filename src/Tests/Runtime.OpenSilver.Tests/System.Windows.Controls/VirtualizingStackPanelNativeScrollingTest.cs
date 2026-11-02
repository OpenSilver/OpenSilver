
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Xaml.Context;
using Runtime.OpenSilver.Tests;

namespace System.Windows.Controls.Tests;

[TestClass]
public class VirtualizingStackPanelNativeScrollingTest
{
    private const double ViewportSize = 100;
    private const double ItemSize = 20;
    private const int ItemCount = 1000;

    private sealed class TestItemsControl : ItemsControl
    {
        public TestItemsControl(Orientation orientation)
        {
            Width = ViewportSize;
            Height = ViewportSize;

            ItemsPanel = new ItemsPanelTemplate
            {
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    (owner, context) =>
                    {
                        var panel = new VirtualizingStackPanel { Orientation = orientation };
                        panel.SetTemplatedParent(context.TemplateOwnerReference);
                        return panel;
                    }),
            };

            Template = new ControlTemplate
            {
                TargetType = typeof(TestItemsControl),
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    (owner, context) =>
                    {
                        var presenter = new ItemsPresenter();
                        presenter.SetTemplatedParent(context.TemplateOwnerReference);

                        var sv = new ScrollViewer
                        {
                            Padding = new Thickness(0),
                            BorderThickness = new Thickness(0),
                            HorizontalScrollBarVisibility = orientation == Orientation.Horizontal ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled,
                            VerticalScrollBarVisibility = orientation == Orientation.Horizontal ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto,
                            Content = presenter,
                        };
                        sv.SetTemplatedParent(context.TemplateOwnerReference);
                        return sv;
                    }),
            };
        }

        public Func<int, double> ItemSizeSelector { get; set; } = _ => ItemSize;

        public Orientation Orientation => ((VirtualizingStackPanel)ItemsHost).Orientation;

        protected override DependencyObject GetContainerForItemOverride() => new Border { Background = new SolidColorBrush(Colors.Red) };

        protected override bool IsItemItsOwnContainerOverride(object item) => false;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var border = (Border)element;
            if (Orientation == Orientation.Horizontal)
            {
                border.Width = ItemSizeSelector((int)item);
            }
            else
            {
                border.Height = ItemSizeSelector((int)item);
            }
        }
    }

    private sealed class Host : IDisposable
    {
        private readonly Popup _popup;
        private readonly List<string> _javascript = new();

        public Host(TestItemsControl itemsControl)
        {
            TestSetup.ExecuteJavascript += OnExecuteJavascript;

            ItemsControl = itemsControl;
            _popup = new Popup { Child = itemsControl, IsOpen = true };
            UpdateLayout();
        }

        public TestItemsControl ItemsControl { get; }

        public ScrollViewer ScrollViewer => (ScrollViewer)VisualTreeHelper.GetChild(ItemsControl, 0);

        public VirtualizingStackPanel Panel => (VirtualizingStackPanel)ItemsControl.ItemsHost;

        public string PanelId => Panel.OuterDiv.Uid;

        public void UpdateLayout()
        {
            // The ScrollViewer executes its scrolling commands in successive layout passes.
            for (int i = 0; i < 4; i++)
            {
                ItemsControl.UpdateLayout();
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

        // Scroll offset of the panel in the (simulated) browser.
        public double ScrollLeft { get; private set; }

        public double ScrollTop { get; private set; }

        public void SimulateNativeScroll(double x, double y)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("void 0");
            SetBrowserScrollOffset(x, y);
            NativeScrollManager.OnScrollNative(PanelId, ScrollLeft, ScrollTop);
            UpdateLayout();

            // Scroll events raised by the offsets written by the panel.
            for (int i = 0; i < 4; i++)
            {
                double left = ScrollLeft;
                double top = ScrollTop;
                OpenSilver.Interop.ExecuteJavaScriptVoid("void 0");
                if (left == ScrollLeft && top == ScrollTop)
                {
                    break;
                }

                NativeScrollManager.OnScrollNative(PanelId, ScrollLeft, ScrollTop);
                UpdateLayout();
            }
        }

        public void SimulateNativeScrollBy(double dx, double dy)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("void 0");
            SimulateNativeScroll(ScrollLeft + dx, ScrollTop + dy);
        }

        private double _extentWidth;
        private double _extentHeight;

        private void SetBrowserScrollOffset(double x, double y)
        {
            // Browsers clamp the offset to the scrollable range.
            ScrollLeft = Math.Max(0, Math.Min(x, _extentWidth - Panel.ActualWidth));
            ScrollTop = Math.Max(0, Math.Min(y, _extentHeight - Panel.ActualHeight));
        }

        private static readonly Regex ScrollCallRegex =
            new(@"osjs\.nativeScroll\.(setExtent|scrollTo|scrollBy)\('([^']+)',([^,]+),([^)]+)\)");

        private void SimulateBrowser(string javascript)
        {
            if (ItemsControl.ItemsHost is not VirtualizingStackPanel panel)
            {
                return;
            }

            foreach (Match match in ScrollCallRegex.Matches(javascript))
            {
                if (match.Groups[2].Value != panel.OuterDiv.Uid)
                {
                    continue;
                }

                double x = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                double y = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                switch (match.Groups[1].Value)
                {
                    case "setExtent":
                        _extentWidth = x;
                        _extentHeight = y;
                        SetBrowserScrollOffset(ScrollLeft, ScrollTop);
                        break;
                    case "scrollTo":
                        SetBrowserScrollOffset(x, y);
                        break;
                    case "scrollBy":
                        SetBrowserScrollOffset(ScrollLeft + x, ScrollTop + y);
                        break;
                }
            }
        }

        public Border ContainerFromIndex(int index) => (Border)ItemsControl.ItemContainerGenerator.ContainerFromIndex(index);

        public Dictionary<int, double> GetRealizedPositions()
        {
            var positions = new Dictionary<int, double>();
            foreach (UIElement child in Panel.Children)
            {
                int index = ItemsControl.ItemContainerGenerator.IndexFromContainer(child);
                positions[index] = ItemsControl.Orientation == Orientation.Horizontal ? child.VisualOffset.X : child.VisualOffset.Y;
            }
            return positions;
        }

        // Position of the container in the viewport, as rendered by the browser.
        public double GetPositionInViewport(int index)
        {
            Point p = ContainerFromIndex(index).TransformToVisual(ScrollViewer).Transform(new Point(0, 0));
            return ItemsControl.Orientation == Orientation.Horizontal ? p.X : p.Y;
        }

        public void Dispose()
        {
            _popup.IsOpen = false;
            TestSetup.ExecuteJavascript -= OnExecuteJavascript;
        }

        private void OnExecuteJavascript(object sender, ExecuteJavascriptEventArgs e)
        {
            _javascript.Add(e.Javascript);
            SimulateBrowser(e.Javascript);
        }
    }

    private static TestItemsControl CreateItemsControl(Orientation orientation = Orientation.Vertical, int itemCount = ItemCount)
    {
        return new TestItemsControl(orientation)
        {
            ItemsSource = new ObservableCollection<int>(Enumerable.Range(0, itemCount)),
        };
    }

    [TestMethod]
    public void VirtualizingStackPanel_Should_Scroll_Natively_By_Default()
    {
        using var host = new Host(CreateItemsControl());
        string js = host.FlushJavaScript();

        Assert.IsTrue(host.Panel.IsNativeScrolling);
        Assert.AreSame(host.Panel, host.ScrollViewer.ScrollInfo);
        Assert.IsFalse(host.ScrollViewer.ElementScrollContentPresenter.IsNativeScrolling);

        // Logical (item based) values, as in Silverlight.
        Assert.AreEqual(ItemCount, host.ScrollViewer.ExtentHeight);
        Assert.AreEqual(ViewportSize / ItemSize, host.ScrollViewer.ViewportHeight);
        Assert.AreEqual(0, host.ScrollViewer.VerticalOffset);

        StringAssert.Contains(js, $"osjs.nativeScroll.setMode('{host.PanelId}',1,false,true)");
        // The vertical ScrollBar of the ScrollViewer takes the rest of the width.
        StringAssert.Contains(js, $"osjs.nativeScroll.setExtent('{host.PanelId}',{host.Panel.ActualWidth.ToInvariantString()},{ItemCount * ItemSize})");
        // The ScrollContentPresenter neither scrolls nor captures touches.
        StringAssert.Contains(js, $"osjs.nativeScroll.setMode('{host.ScrollViewer.ElementScrollContentPresenter.OuterDiv.Uid}',0,false,false)");

        // The viewport and half a viewport after it are realized.
        Assert.AreEqual(8, host.Panel.Children.Count);
        Assert.AreEqual(140, host.ContainerFromIndex(7).VisualOffset.Y);
    }

    [TestMethod]
    public void Native_Scroll_Should_Update_Logical_Offset_Without_Writing_It_Back()
    {
        using var host = new Host(CreateItemsControl());
        int scrollChangedCount = 0;
        host.ScrollViewer.ScrollChanged += (o, e) => scrollChangedCount++;
        host.FlushJavaScript();

        host.SimulateNativeScroll(0, 210);

        Assert.AreEqual(10.5, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.AreEqual(ViewportSize / ItemSize, host.ScrollViewer.ViewportHeight, 1e-9);
        Assert.AreEqual(1, scrollChangedCount);

        // Containers are positioned in the content, and the browser scrolls them.
        Assert.AreEqual(200, host.ContainerFromIndex(10).VisualOffset.Y);
        Assert.AreEqual(-10, host.GetPositionInViewport(10), 1e-9);
        Assert.IsNull(host.ContainerFromIndex(0));

        string js = host.FlushJavaScript();
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollTo"));
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollBy"));
    }

    [TestMethod]
    public void Small_Native_Scroll_Should_Not_Realize_Containers()
    {
        using var host = new Host(CreateItemsControl());
        UIElement[] children = host.Panel.Children.ToArray();

        host.SimulateNativeScroll(0, 5);

        Assert.AreEqual(0.25, host.ScrollViewer.VerticalOffset, 1e-9);
        CollectionAssert.AreEqual(children, host.Panel.Children.ToArray());
    }

    [TestMethod]
    public void ScrollToVerticalOffset_Should_Write_Pixel_Offset()
    {
        using var host = new Host(CreateItemsControl());
        host.FlushJavaScript();

        host.ScrollViewer.ScrollToVerticalOffset(500.5);
        host.UpdateLayout();

        Assert.AreEqual(500.5, host.ScrollViewer.VerticalOffset, 1e-9);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PanelId}',0,10010)");
        Assert.AreEqual(10000, host.ContainerFromIndex(500).VisualOffset.Y);
        Assert.AreEqual(-10, host.GetPositionInViewport(500), 1e-9);
    }

    [TestMethod]
    public void ScrollToVerticalOffset_Should_Be_Clamped_To_The_End()
    {
        using var host = new Host(CreateItemsControl());
        host.FlushJavaScript();

        host.ScrollViewer.ScrollToVerticalOffset(5000);
        host.UpdateLayout();

        Assert.AreEqual(host.ScrollViewer.ScrollableHeight, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.AreEqual(ItemCount - ViewportSize / ItemSize, host.ScrollViewer.VerticalOffset, 1e-9);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PanelId}',0,{ItemCount * ItemSize - ViewportSize})");
        Assert.AreEqual(ViewportSize - ItemSize, host.GetPositionInViewport(ItemCount - 1), 1e-9);
    }

    [TestMethod]
    public void Native_Scroll_To_The_End_Should_Snap_To_ScrollableHeight()
    {
        using var host = new Host(CreateItemsControl());

        host.SimulateNativeScroll(0, ItemCount * ItemSize - ViewportSize - 0.4);

        Assert.AreEqual(host.ScrollViewer.ScrollableHeight, host.ScrollViewer.VerticalOffset, 1e-9);
    }

    [TestMethod]
    public void Measuring_Items_Above_The_Viewport_Should_Not_Move_Visible_Items()
    {
        // Large items at the beginning: the size of the items in the middle is overestimated.
        TestItemsControl itemsControl = CreateItemsControl();
        itemsControl.ItemSizeSelector = i => i < 50 ? 100 : ItemSize;
        using var host = new Host(itemsControl);

        host.FlushJavaScript();
        host.ScrollViewer.ScrollToVerticalOffset(600);
        host.UpdateLayout();
        Assert.AreEqual(600, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.IsTrue(host.Panel.Children.Count < 20);
        Assert.AreEqual(0, host.GetPositionInViewport(600), 1e-9);

        string js = host.FlushJavaScript();
        double position = host.ContainerFromIndex(600).VisualOffset.Y;
        StringAssert.Contains(js, $"osjs.nativeScroll.scrollTo('{host.PanelId}',0,{position.ToInvariantString()})");

        // Scroll up into items whose size is estimated.
        for (int i = 1; i <= 40; i++)
        {
            Dictionary<int, double> before = host.GetRealizedPositions();

            host.SimulateNativeScrollBy(0, -ViewportSize / 2);

            Dictionary<int, double> after = host.GetRealizedPositions();
            foreach (KeyValuePair<int, double> item in before)
            {
                if (after.TryGetValue(item.Key, out double y))
                {
                    Assert.AreEqual(item.Value, y, 1e-9, $"Item {item.Key} moved.");
                }
            }

            // The logical offset matches what the browser displays.
            int first = (int)host.ScrollViewer.VerticalOffset;
            double fraction = host.ScrollViewer.VerticalOffset - first;
            Assert.AreEqual(-fraction * ItemSize, host.GetPositionInViewport(first), 1e-6);
        }

        js = host.FlushJavaScript();
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollTo"));
        Assert.IsFalse(js.Contains("osjs.nativeScroll.scrollBy"));
    }

    [TestMethod]
    public void Estimation_Errors_Should_Be_Resolved_Near_The_Beginning()
    {
        TestItemsControl itemsControl = CreateItemsControl();
        itemsControl.ItemSizeSelector = i => i < 50 ? ItemSize : 100;
        using var host = new Host(itemsControl);

        // Small items at the beginning: the size of the items in the middle is underestimated.
        host.ScrollViewer.ScrollToVerticalOffset(100);
        host.UpdateLayout();
        host.FlushJavaScript();

        // Scroll up until the beginning of the list.
        for (int i = 0; i < 200 && host.ScrollTop > 0; i++)
        {
            host.SimulateNativeScrollBy(0, -ViewportSize / 2);

            int first = (int)host.ScrollViewer.VerticalOffset;
            double fraction = host.ScrollViewer.VerticalOffset - first;
            Assert.AreEqual(-fraction * host.ContainerFromIndex(first).ActualHeight, host.GetPositionInViewport(first), 1e-6);
        }

        Assert.AreEqual(0, host.ScrollTop);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollBy('{host.PanelId}',0,");
        Assert.AreEqual(0, host.ContainerFromIndex(0).VisualOffset.Y, 1e-9);
        Assert.AreEqual(0, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.AreEqual(ItemCount, host.ScrollViewer.ExtentHeight);
    }

    [TestMethod]
    public void MakeVisible_Should_Scroll_Minimally()
    {
        using var host = new Host(CreateItemsControl());
        host.FlushJavaScript();

        Border container = host.ContainerFromIndex(6);
        host.ScrollViewer.MakeVisible(container, new Rect(container.RenderSize));
        host.UpdateLayout();

        Assert.AreEqual(2, host.ScrollViewer.VerticalOffset, 1e-9);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PanelId}',0,40)");

        host.SimulateNativeScroll(0, 50);
        container = host.ContainerFromIndex(2);
        host.ScrollViewer.MakeVisible(container, new Rect(container.RenderSize));
        host.UpdateLayout();

        Assert.AreEqual(2, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.AreEqual(0, host.GetPositionInViewport(2), 1e-9);
    }

    [TestMethod]
    public void MakeVisible_Should_Support_Variable_Item_Sizes()
    {
        TestItemsControl itemsControl = CreateItemsControl();
        itemsControl.ItemSizeSelector = i => i % 2 == 0 ? 20 : 50;
        using var host = new Host(itemsControl);

        Border container = host.ContainerFromIndex(3);
        host.ScrollViewer.MakeVisible(container, new Rect(container.RenderSize));
        host.UpdateLayout();

        Assert.AreEqual(1.4, host.ScrollViewer.VerticalOffset, 1e-9);
        Assert.AreEqual(ViewportSize - 50, host.GetPositionInViewport(3), 1e-9);
    }

    [TestMethod]
    public void Horizontal_Orientation_Should_Scroll_Natively()
    {
        using var host = new Host(CreateItemsControl(Orientation.Horizontal));
        string js = host.FlushJavaScript();

        Assert.IsTrue(host.Panel.IsNativeScrolling);
        Assert.AreEqual(ItemCount, host.ScrollViewer.ExtentWidth);
        StringAssert.Contains(js, $"osjs.nativeScroll.setMode('{host.PanelId}',1,true,false)");

        host.ScrollViewer.ScrollToHorizontalOffset(100);
        host.UpdateLayout();

        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PanelId}',2000,0)");
        Assert.AreEqual(2000, host.ContainerFromIndex(100).VisualOffset.X);

        host.SimulateNativeScroll(2010, 0);
        Assert.AreEqual(100.5, host.ScrollViewer.HorizontalOffset, 1e-9);
    }

    [TestMethod]
    public void Removing_Items_Should_Update_Extent()
    {
        TestItemsControl itemsControl = CreateItemsControl();
        using var host = new Host(itemsControl);
        var items = (ObservableCollection<int>)itemsControl.ItemsSource;

        host.SimulateNativeScroll(0, 2000);
        host.FlushJavaScript();

        for (int i = 0; i < 10; i++)
        {
            items.RemoveAt(500);
        }
        items.Insert(0, -1);
        host.UpdateLayout();

        Assert.AreEqual(ItemCount - 9, host.ScrollViewer.ExtentHeight);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setExtent('{host.PanelId}',{host.Panel.ActualWidth.ToInvariantString()},{(ItemCount - 9) * ItemSize})");
    }

    [TestMethod]
    public void NativeScrolling_IsEnabled_False_Should_Use_Managed_Virtualization()
    {
        TestItemsControl itemsControl = CreateItemsControl();
        NativeScrolling.SetIsEnabled(itemsControl, false);
        using var host = new Host(itemsControl);
        string scpId = host.ScrollViewer.ElementScrollContentPresenter.OuterDiv.Uid;

        Assert.IsFalse(host.Panel.IsNativeScrolling);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setMode('{scpId}',2,false,true)");

        host.ScrollViewer.ScrollToVerticalOffset(10);
        host.UpdateLayout();

        Assert.AreEqual(10, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(0, host.ContainerFromIndex(10).VisualOffset.Y);
        Assert.IsFalse(host.FlushJavaScript().Contains($"osjs.nativeScroll.scrollTo('{host.PanelId}'"));
    }

    [TestMethod]
    public void NativeScrolling_IsEnabled_Should_Switch_Mode_Dynamically()
    {
        using var host = new Host(CreateItemsControl());
        host.ScrollViewer.ScrollToVerticalOffset(10);
        host.UpdateLayout();

        NativeScrolling.SetIsEnabled(host.ItemsControl, false);
        host.UpdateLayout();

        Assert.IsFalse(host.Panel.IsNativeScrolling);
        Assert.AreEqual(10, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(0, host.ContainerFromIndex(10).VisualOffset.Y);

        host.FlushJavaScript();
        NativeScrolling.SetIsEnabled(host.ItemsControl, true);
        host.UpdateLayout();

        Assert.IsTrue(host.Panel.IsNativeScrolling);
        Assert.AreEqual(10, host.ScrollViewer.VerticalOffset);
        Assert.AreEqual(200, host.ContainerFromIndex(10).VisualOffset.Y);
        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.scrollTo('{host.PanelId}',0,200)");
    }

    [TestMethod]
    public void Feature_Disabled_Should_Use_Managed_Virtualization()
    {
        Features.Scrolling.UseNativeScrolling = false;
        try
        {
            using var host = new Host(CreateItemsControl());

            Assert.IsFalse(host.Panel.IsNativeScrolling);
            Assert.IsFalse(host.FlushJavaScript().Contains("osjs.nativeScroll"));

            host.ScrollViewer.ScrollToVerticalOffset(10);
            host.UpdateLayout();
            Assert.AreEqual(0, host.ContainerFromIndex(10).VisualOffset.Y);
        }
        finally
        {
            Features.Scrolling.UseNativeScrolling = true;
        }
    }

    [TestMethod]
    public void Panel_With_Background_Should_Be_Opaque_Scroller()
    {
        using var host = new Host(CreateItemsControl());
        host.FlushJavaScript();

        host.Panel.Background = new SolidColorBrush(Colors.Transparent);

        StringAssert.Contains(host.FlushJavaScript(), $"osjs.nativeScroll.setMode('{host.PanelId}',1,false,true,true)");
    }
}
