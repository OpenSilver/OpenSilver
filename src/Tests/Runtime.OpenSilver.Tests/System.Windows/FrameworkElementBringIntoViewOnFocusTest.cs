
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

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;

namespace System.Windows.Tests;

[TestClass]
public class FrameworkElementBringIntoViewOnFocusTest
{
    private const double ViewportSize = 100;

    private sealed class Host : IDisposable
    {
        private readonly Popup _popup;

        public Host(ScrollViewer outer, bool nativeScrolling)
        {
            Outer = outer;
            NativeScrolling.SetIsEnabled(outer, nativeScrolling);
            _popup = new Popup { Child = outer, IsOpen = true };
            UpdateLayout();
        }

        public ScrollViewer Outer { get; }

        public void UpdateLayout()
        {
            // The ScrollViewer executes its scrolling commands in successive layout passes.
            for (int i = 0; i < 4; i++)
            {
                Outer.UpdateLayout();
            }
        }

        public void Press(UIElement element)
        {
            string id = element.OuterDiv.Uid;
            InputManager.ProcessPointerInputNative(id, (int)InputManager.EVENTS.POINTER_LEFT_DOWN, null, false, 10, 10, 0);
            InputManager.ProcessPointerInputNative(id, (int)InputManager.EVENTS.POINTER_LEFT_UP, null, false, 10, 10, 0);
            UpdateLayout();
        }

        public void Dispose() => _popup.IsOpen = false;
    }

    private static ScrollViewer CreateScrollViewer(double height, UIElement content) => new()
    {
        Width = ViewportSize,
        Height = height,
        Padding = new Thickness(0),
        BorderThickness = new Thickness(0),
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        Content = content,
    };

    private static StackPanel CreateStack(params UIElement[] children)
    {
        var panel = new StackPanel { VerticalAlignment = VerticalAlignment.Top };
        foreach (UIElement child in children)
        {
            panel.Children.Add(child);
        }
        return panel;
    }

    // An inner ScrollViewer from 60 to 140 in an outer ScrollViewer showing 0 to 100.
    private static ScrollViewer CreateNestedScrollViewers(out ScrollViewer inner, out Border innerContent)
    {
        innerContent = new Border { Height = 200 };
        inner = CreateScrollViewer(80, innerContent);
        return CreateScrollViewer(ViewportSize, CreateStack(new Border { Height = 60 }, inner));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Focus_Should_Bring_A_Partially_Visible_Nested_ScrollViewer_Into_View(bool nativeScrolling)
    {
        using var host = new Host(CreateNestedScrollViewers(out ScrollViewer inner, out _), nativeScrolling);

        Assert.IsTrue(inner.Focus());
        host.UpdateLayout();

        Assert.AreEqual(40, host.Outer.VerticalOffset);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Pressing_Inside_A_Partially_Visible_Nested_ScrollViewer_Should_Not_Scroll_The_Outer_ScrollViewer(bool nativeScrolling)
    {
        using var host = new Host(CreateNestedScrollViewers(out ScrollViewer inner, out Border innerContent), nativeScrolling);

        host.Press(innerContent);

        Assert.IsTrue(inner.IsKeyboardFocusWithin);
        Assert.AreEqual(0, host.Outer.VerticalOffset);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Pressing_An_Element_That_Focuses_Another_Element_Should_Bring_The_Focused_Element_Into_View(bool nativeScrolling)
    {
        var target = new Control { Height = 50 };
        var pressed = new Border { Height = 50 };
        pressed.MouseLeftButtonDown += (s, e) => e.Handled = target.Focus();
        var outer = CreateScrollViewer(ViewportSize, CreateStack(pressed, new Border { Height = 250 }, target));
        using var host = new Host(outer, nativeScrolling);

        host.Press(pressed);

        Assert.IsTrue(target.IsKeyboardFocused);
        Assert.AreEqual(250, host.Outer.VerticalOffset);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Focus_After_A_Press_Should_Bring_The_Focused_Element_Into_View(bool nativeScrolling)
    {
        using var host = new Host(CreateNestedScrollViewers(out ScrollViewer inner, out Border innerContent), nativeScrolling);
        host.Press(innerContent);
        Assert.IsTrue(inner.IsKeyboardFocusWithin);

        Keyboard.Focus(null);
        Assert.IsTrue(inner.Focus());
        host.UpdateLayout();

        Assert.AreEqual(40, host.Outer.VerticalOffset);
    }
}
