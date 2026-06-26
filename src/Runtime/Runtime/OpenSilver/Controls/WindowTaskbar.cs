
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
using System.Collections.Generic;
using System.Windows;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace OpenSilver.Controls;

/// <summary>
/// Manages a taskbar area at the bottom of the screen where window miniatures are displayed.
/// The taskbar is shown when there are 2+ windows, or when any window is minimized.
/// </summary>
internal static class WindowTaskbar
{
    private static bool _isInitialized;
    private static string _taskbarId;
    private static string _rootId;
    private static readonly Dictionary<Window, TaskbarItem> _items = new();
    private static bool _isVisible;

    private static void EnsureInitialized()
    {
        if (_isInitialized) return;

        Application app = Application.Current;
        if (app is null) return;

        _rootId = app.GetRootDiv().Uid;
        _taskbarId = $"os_taskbar_{_rootId}";

        Interop.ExecuteJavaScriptVoidAsync(
            $"(function() {{" +
            $"  var root = document.getElementById('{_rootId}');" +
            $"  var tb = document.createElement('div');" +
            $"  tb.id = '{_taskbarId}';" +
            $"  tb.style.display = 'none';" +
            $"  tb.style.flexWrap = 'wrap';" +
            $"  tb.style.alignItems = 'center';" +
            $"  tb.style.background = '#1e1e1e';" +
            $"  tb.style.zIndex = '2147483647';" +
            $"  tb.style.position = 'absolute';" +
            $"  tb.style.bottom = '0';" +
            $"  tb.style.left = '0';" +
            $"  tb.style.right = '0';" +
            $"  root.appendChild(tb);" +
            $"}})()");

        _isInitialized = true;
    }

    internal static void AddWindow(Window window)
    {
        if (_items.ContainsKey(window)) return;
        if (!window.ShowInTaskbar) return;

        EnsureInitialized();

        var item = new TaskbarItem(window);
        item.BypassLayoutPolicies = true;
        item.ParentWindow = window;

        HtmlElementReference taskbarDiv = new HtmlElementReference(_taskbarId);
        item.OuterDiv = INTERNAL_HtmlDomManager.CreateTaskbarItemRootDomElementAndAppendIt(taskbarDiv, item);

        item.IsLoadedCache = true;
        item.IsConnectedToLiveTree = true;
        item.UpdateIsRenderableCache();
        item.UpdateIsVisibleCache();
        UIElement.PropagateResumeLayout(null, item);

        item.INTERNAL_OnAttachedToVisualTree();

        item.InvalidateMeasure();
        item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        item.Arrange(new Rect(new Point(), item.DesiredSize));
        item.UpdateLayout();

        string w = item.DesiredSize.Width.ToInvariantString();
        string h = item.DesiredSize.Height.ToInvariantString();
        string itemDivId = item.OuterDiv.Uid;
        Interop.ExecuteJavaScriptVoidAsync(
            $"(function() {{ var el = document.getElementById('{itemDivId}'); " +
            $"el.style.width = '{w}px'; el.style.height = '{h}px'; }})()");

        _items[window] = item;
        UpdateVisibility();
    }

    internal static void RemoveWindow(Window window)
    {
        if (!_items.TryGetValue(window, out var item)) return;

        if (item.OuterDiv.IsConnected)
        {
            INTERNAL_HtmlDomManager.RemoveNodeNative(item.OuterDiv);
        }

        item.IsLoadedCache = false;
        item.IsConnectedToLiveTree = false;
        item.UpdateIsRenderableCache();
        item.UpdateIsVisibleCache();
        UIElement.PropagateSuspendLayout(item);

        _items.Remove(window);
        UpdateVisibility();
    }

    internal static void UpdateVisibility()
    {
        bool shouldBeVisible = _items.Count >= 2 || HasMinimizedWindow();

        if (shouldBeVisible == _isVisible) return;
        _isVisible = shouldBeVisible;

        if (_taskbarId is null) return;

        string display = shouldBeVisible ? "flex" : "none";
        Interop.ExecuteJavaScriptVoidAsync(
            $"document.getElementById('{_taskbarId}').style.display='{display}'");
    }

    private static bool HasMinimizedWindow()
    {
        if (Application.Current is not Application app) return false;

        foreach (Window w in app.Windows)
        {
            if (!w._isClosed && w.WindowState == WindowState.Minimized)
            {
                return true;
            }
        }
        return false;
    }

    internal static void OnWindowActivated(Window window)
    {
        // Update visual state of taskbar items to reflect active window
        foreach (var kvp in _items)
        {
            kvp.Value.IsActiveWindow = kvp.Key == window;
        }
    }
}
