
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
/// Manages a taskbar area at the bottom of the main window where minimized windows are displayed.
/// Uses a CSS flex-wrap container in the second row of the RootDomElement's CSS grid.
/// Each minimized window is represented by a stylable <see cref="TaskbarItem"/> control.
/// </summary>
internal static class WindowTaskbar
{
    private static bool _isInitialized;
    private static string _taskbarId;
    private static string _rootId;
    private static readonly Dictionary<Window, TaskbarItem> _minimizedItems = new();

    private static void EnsureInitialized(Window mainWindow)
    {
        if (_isInitialized) return;

        _rootId = mainWindow.RootDomElement.Uid;
        _taskbarId = $"os_taskbar_{_rootId}";

        string windowDivId = mainWindow.OuterDiv.Uid;

        Interop.ExecuteJavaScriptVoidAsync(
            $"(function() {{" +
            $"  var root = document.getElementById('{_rootId}');" +
            $"  root.style.gridTemplateRows = '1fr auto';" +
            $"  var win = document.getElementById('{windowDivId}');" +
            $"  if (win) {{ win.style.height = 'auto'; win.style.minHeight = '0'; }}" +
            $"  var tb = document.createElement('div');" +
            $"  tb.id = '{_taskbarId}';" +
            $"  tb.style.display = 'flex';" +
            $"  tb.style.flexWrap = 'wrap';" +
            $"  tb.style.alignItems = 'center';" +
            $"  tb.style.background = '#1e1e1e';" +
            $"  root.appendChild(tb);" +
            $"}})()");

        _isInitialized = true;
    }

    internal static void MinimizeWindow(Window window)
    {
        if (_minimizedItems.ContainsKey(window)) return;

        Window mainWindow = Application.Current?.MainWindow;
        if (mainWindow is null) return;

        EnsureInitialized(mainWindow);

        var item = new TaskbarItem(window);
        item.BypassLayoutPolicies = true;
        item.ParentWindow = mainWindow;

        // Attach the TaskbarItem's DOM element inside the flex container
        HtmlElementReference taskbarDiv = new HtmlElementReference(_taskbarId);
        item.OuterDiv = INTERNAL_HtmlDomManager.CreateTaskbarItemRootDomElementAndAppendIt(taskbarDiv, item);

        item.IsLoadedCache = true;
        item.IsConnectedToLiveTree = true;
        item.UpdateIsRenderableCache();
        item.UpdateIsVisibleCache();
        UIElement.PropagateResumeLayout(null, item);

        // Trigger theme style resolution (normally done during visual tree attachment)
        item.INTERNAL_OnAttachedToVisualTree();

        item.InvalidateMeasure();
        item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        item.Arrange(new Rect(new Point(), item.DesiredSize));
        item.UpdateLayout();

        // Explicitly set the OuterDiv size since BypassLayoutPolicies skips ArrangeNative,
        // and template children are position:absolute so they don't give the parent flow height.
        string w = item.DesiredSize.Width.ToInvariantString();
        string h = item.DesiredSize.Height.ToInvariantString();
        string itemDivId = item.OuterDiv.Uid;
        Interop.ExecuteJavaScriptVoidAsync(
            $"(function() {{ var el = document.getElementById('{itemDivId}'); " +
            $"el.style.width = '{w}px'; el.style.height = '{h}px'; }})()");


        _minimizedItems[window] = item;
    }

    internal static void RestoreWindow(Window window)
    {
        if (!_minimizedItems.TryGetValue(window, out var item)) return;

        if (item.OuterDiv.IsConnected)
        {
            INTERNAL_HtmlDomManager.RemoveNodeNative(item.OuterDiv);
        }

        item.IsLoadedCache = false;
        item.IsConnectedToLiveTree = false;
        item.UpdateIsRenderableCache();
        item.UpdateIsVisibleCache();
        UIElement.PropagateSuspendLayout(item);

        _minimizedItems.Remove(window);

        if (_minimizedItems.Count == 0 && _rootId != null)
        {
            Interop.ExecuteJavaScriptVoidAsync(
                $"document.getElementById('{_rootId}').style.gridTemplateRows = '1fr'");
        }
    }
}
