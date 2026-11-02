
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
using System.Windows.Input;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal static class NativeScrollManager
{
    // Must remain in sync with MODE in cshtml5.js (osjs.nativeScroll).
    internal enum Mode
    {
        None = 0,
        Native = 1,
        Managed = 2,
    }

    // Must remain in sync with osjs.nativeScroll.onWheel.
    internal enum WheelResult
    {
        CannotScroll = 0,
        ScrolledByBrowser = 1,
        Scrolled = 2,
    }

    public static HtmlElementReference CreateContentHost(HtmlElementReference scroller)
    {
        Debug.Assert(scroller.IsConnected);

        string id = INTERNAL_HtmlDomManager.NewId();
        Interop.ExecuteJavaScriptVoidAsync($"osjs.nativeScroll.createHost('{scroller.Uid}','{id}')");
        return new HtmlElementReference(id);
    }

    // An opaque scroller is hit-testable by itself (it has a background): the pointer is not routed
    // to the elements underneath when it is over an area not covered by the content.
    public static void SetMode(HtmlElementReference scroller, Mode mode, bool canScrollX, bool canScrollY, bool isOpaque = false)
    {
        Debug.Assert(scroller.IsConnected);

        Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.nativeScroll.setMode('{scroller.Uid}',{(int)mode},{ToJs(canScrollX)},{ToJs(canScrollY)}{(isOpaque ? ",true" : string.Empty)})");
    }

    private static string ToJs(bool value) => value ? "true" : "false";

    public static void SetExtent(HtmlElementReference scroller, double width, double height)
    {
        Debug.Assert(scroller.IsConnected);

        Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.nativeScroll.setExtent('{scroller.Uid}',{width.ToInvariantString()},{height.ToInvariantString()})");
    }

    public static void ScrollTo(HtmlElementReference scroller, double x, double y)
    {
        Debug.Assert(scroller.IsConnected);

        Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.nativeScroll.scrollTo('{scroller.Uid}',{x.ToInvariantString()},{y.ToInvariantString()})");
    }

    // Relative to the current offset of the DOM element, which can be ahead of the last offset
    // reported to C# while the user is scrolling.
    public static void ScrollBy(HtmlElementReference scroller, double dx, double dy)
    {
        Debug.Assert(scroller.IsConnected);

        Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.nativeScroll.scrollBy('{scroller.Uid}',{dx.ToInvariantString()},{dy.ToInvariantString()})");
    }

    // The browser scrolls with its own (smooth) wheel behavior. At the limits, the event keeps
    // bubbling so that outer ScrollViewers can scroll.
    public static void HandleMouseWheel(HtmlElementReference scroller, MouseWheelEventArgs e)
    {
        switch (OnWheel(scroller, e))
        {
            case WheelResult.ScrolledByBrowser:
                e.Handled = true;
                e.Cancellable = false;
                break;

            case WheelResult.Scrolled:
                e.Handled = true;
                break;
        }
    }

    private static WheelResult OnWheel(HtmlElementReference scroller, MouseWheelEventArgs e)
    {
        Debug.Assert(scroller.IsConnected);
        Debug.Assert(e is not null);

        if (e.UIEventArg is null)
        {
            return WheelResult.CannotScroll;
        }

        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        return (WheelResult)Interop.ExecuteJavaScriptInt32($"osjs.nativeScroll.onWheel('{scroller.Uid}',{sArgs})");
    }

    internal static void OnScrollNative(string id, double scrollLeft, double scrollTop)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is INativeScrollInfo scroller)
        {
            scroller.OnNativeScroll(scrollLeft, scrollTop);
        }
    }
}
