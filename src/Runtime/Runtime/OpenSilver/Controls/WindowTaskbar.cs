
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
    private static HtmlElementReference _taskbarElement;
    private static readonly Dictionary<Window, TaskbarItem> _items = new();
    private static bool _isVisible;

    private static void EnsureInitialized()
    {
        if (_isInitialized)
        {
            return;
        }

        if (Application.Current is not Application app)
        {
            return;
        }

        var _rootId = app.GetRootDiv().Uid;
        _taskbarElement = new HtmlElementReference($"os_taskbar_{_rootId}");

        Interop.ExecuteJavaScriptVoidAsync(
            $$"""
            (function () {
              const root = document.getElementById('{{_rootId}}');
              const tb = document.createElement('div');
              tb.id = '{{_taskbarElement.Uid}}';
              tb.style.display = 'none';
              tb.style.flexWrap = 'wrap';
              tb.style.alignItems = 'center';
              tb.style.background = '#1e1e1e';
              tb.style.gridRow = '2';
              tb.style.width = '100%';
              root.appendChild(tb);
            })();
            """);

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

        item.OuterDiv = INTERNAL_HtmlDomManager.CreateTaskbarItemRootDomElementAndAppendIt(_taskbarElement, item);

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

        item.OuterDiv.SetCssStyleProperty(CssPropertyNames.Width, $"{item.DesiredSize.Width.ToInvariantString()}px");
        item.OuterDiv.SetCssStyleProperty(CssPropertyNames.Height, $"{item.DesiredSize.Height.ToInvariantString()}px");

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
        bool showTaskBar = _items.Count >= 2 || HasMinimizedWindow();

        if (showTaskBar == _isVisible)
        {
            return;
        }

        _isVisible = showTaskBar;

        if (_taskbarElement.IsConnected)
        {
            _taskbarElement.SetCssStyleProperty(CssPropertyNames.Display, showTaskBar ? "flex" : "none");
        }

        Window.EnsureAllWindowsWithinBoundaries();
    }

    private static bool HasMinimizedWindow()
    {
        if (Application.Current is not Application app)
        {
            return false;
        }

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
