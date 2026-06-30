
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
using System.Windows.Data;
using System.Windows.Input;
using CSHTML5.Internal;

namespace OpenSilver.Controls;

/// <summary>
/// Hosts a secondary <see cref="Window"/> and provides the window chrome (title bar, caption buttons).
/// This control can be restyled in theme resource dictionaries to provide different chrome appearances.
/// </summary>
[TemplatePart(Name = PART_TitleBar, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_MinimizeButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PART_MaximizeButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PART_ResizeLeft, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_ResizeRight, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_ResizeBottom, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_ResizeBottomLeft, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_ResizeBottomRight, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_ResizeGrip, Type = typeof(FrameworkElement))]
public class WindowHost : ContentControl
{
    private const string PART_TitleBar = "PART_TitleBar";
    private const string PART_MinimizeButton = "PART_MinimizeButton";
    private const string PART_MaximizeButton = "PART_MaximizeButton";
    private const string PART_CloseButton = "PART_CloseButton";
    private const string PART_ResizeLeft = "PART_ResizeLeft";
    private const string PART_ResizeRight = "PART_ResizeRight";
    private const string PART_ResizeBottom = "PART_ResizeBottom";
    private const string PART_ResizeBottomLeft = "PART_ResizeBottomLeft";
    private const string PART_ResizeBottomRight = "PART_ResizeBottomRight";
    private const string PART_ResizeGrip = "PART_ResizeGrip";

    private Window _window;
    private FrameworkElement _titleBarPart;
    private ButtonBase _minimizeButtonPart;
    private ButtonBase _maximizeButtonPart;
    private ButtonBase _closeButtonPart;
    private FrameworkElement _resizeLeftPart;
    private FrameworkElement _resizeRightPart;
    private FrameworkElement _resizeBottomPart;
    private FrameworkElement _resizeBottomLeftPart;
    private FrameworkElement _resizeBottomRightPart;
    private FrameworkElement _resizeGripPart;
    private bool _pendingTitleBarVisible = true;
    private double _pendingTitleBarHeight = 30;

    static WindowHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowHost), new PropertyMetadata(typeof(WindowHost)));
        EventManager.RegisterClassHandler<WindowHost>(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), true);
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is WindowHost host)
        {
            host._window?.Activate();
        }
    }

    internal WindowHost(Window window)
    {
        Debug.Assert(window is not null);

        BypassLayoutPolicies = true;

        _window = window;
        Content = window;

        SetBinding(TitleProperty, new Binding(nameof(Window.Title)) { Source = window });
    }

    /// <summary>
    /// Gets or sets the title text displayed in the title bar.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValueInternal(TitleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(WindowHost),
            new PropertyMetadata(string.Empty));

    internal bool IsOpen { get; private set; }

    internal Window HostedWindow => _window;

    internal void Show(HtmlElementReference overlayDiv)
    {
        IsOpen = true;

        ParentWindow = _window;
        OuterDiv = INTERNAL_HtmlDomManager.CreateWindowHostRootDomElementAndAppendIt(overlayDiv, this);
        IsLoadedCache = true;
        IsConnectedToLiveTree = true;
        UpdateIsRenderableCache();
        UpdateIsVisibleCache();
        PropagateResumeLayout(null, this);

        InvalidateMeasure();
        SetLayoutSize();
    }

    internal void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;

        if (OuterDiv.IsConnected)
        {
            INTERNAL_HtmlDomManager.RemoveNodeNative(OuterDiv);
        }

        UnloadSubTree();
    }

    private void UnloadSubTree()
    {
        INTERNAL_VisualTreeManager.DetachSecondaryWindow(this);
        IsLoadedCache = false;
        IsConnectedToLiveTree = false;
        UpdateIsRenderableCache();
        UpdateIsVisibleCache();
        PropagateSuspendLayout(this);
    }

    internal void UpdateResizeBorderThickness(Thickness thickness)
    {
        if (_resizeLeftPart is not null) _resizeLeftPart.Width = thickness.Left;
        if (_resizeRightPart is not null) _resizeRightPart.Width = thickness.Right;
        if (_resizeBottomPart is not null) _resizeBottomPart.Height = thickness.Bottom;
        if (_resizeBottomLeftPart is not null)
        {
            _resizeBottomLeftPart.Width = thickness.Left;
            _resizeBottomLeftPart.Height = thickness.Bottom;
        }
        if (_resizeBottomRightPart is not null)
        {
            _resizeBottomRightPart.Width = thickness.Right;
            _resizeBottomRightPart.Height = thickness.Bottom;
        }
    }

    internal void SetResizeBordersVisible(bool visible)
    {
        var visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        if (_resizeLeftPart is not null) _resizeLeftPart.Visibility = visibility;
        if (_resizeRightPart is not null) _resizeRightPart.Visibility = visibility;
        if (_resizeBottomPart is not null) _resizeBottomPart.Visibility = visibility;
        if (_resizeBottomLeftPart is not null) _resizeBottomLeftPart.Visibility = visibility;
        if (_resizeBottomRightPart is not null) _resizeBottomRightPart.Visibility = visibility;
    }

    internal void UpdateResizeMode(ResizeMode mode)
    {
        bool canResize = mode >= ResizeMode.CanResize;

        if (_minimizeButtonPart is not null)
        {
            _minimizeButtonPart.Visibility = mode == ResizeMode.NoResize
                ? Visibility.Collapsed : Visibility.Visible;
        }

        if (_maximizeButtonPart is not null)
        {
            _maximizeButtonPart.Visibility = mode == ResizeMode.NoResize
                ? Visibility.Collapsed : Visibility.Visible;
            _maximizeButtonPart.IsHitTestVisible = canResize;
            _maximizeButtonPart.Opacity = canResize ? 1.0 : 0.4;
        }

        var resizeVisibility = canResize ? Visibility.Visible : Visibility.Collapsed;
        if (_resizeLeftPart is not null) _resizeLeftPart.Visibility = resizeVisibility;
        if (_resizeRightPart is not null) _resizeRightPart.Visibility = resizeVisibility;
        if (_resizeBottomPart is not null) _resizeBottomPart.Visibility = resizeVisibility;
        if (_resizeBottomLeftPart is not null) _resizeBottomLeftPart.Visibility = resizeVisibility;
        if (_resizeBottomRightPart is not null) _resizeBottomRightPart.Visibility = resizeVisibility;

        if (_resizeGripPart is not null)
        {
            _resizeGripPart.Visibility = mode == ResizeMode.CanResizeWithGrip
                ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal void UpdateTitleBarVisibility(bool visible)
    {
        _pendingTitleBarVisible = visible;
        if (_titleBarPart is not null)
        {
            _titleBarPart.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal void UpdateTitleBarHeight(double captionHeight)
    {
        _pendingTitleBarHeight = captionHeight;
        if (_titleBarPart is not null)
        {
            _titleBarPart.Height = captionHeight;
        }
    }

    public override void OnApplyTemplate()
    {
        UnsubscribeFromTemplateParts();

        base.OnApplyTemplate();

        _titleBarPart = GetTemplateChild(PART_TitleBar) as FrameworkElement;
        _minimizeButtonPart = GetTemplateChild(PART_MinimizeButton) as ButtonBase;
        _maximizeButtonPart = GetTemplateChild(PART_MaximizeButton) as ButtonBase;
        _closeButtonPart = GetTemplateChild(PART_CloseButton) as ButtonBase;
        _resizeLeftPart = GetTemplateChild(PART_ResizeLeft) as FrameworkElement;
        _resizeRightPart = GetTemplateChild(PART_ResizeRight) as FrameworkElement;
        _resizeBottomPart = GetTemplateChild(PART_ResizeBottom) as FrameworkElement;
        _resizeBottomLeftPart = GetTemplateChild(PART_ResizeBottomLeft) as FrameworkElement;
        _resizeBottomRightPart = GetTemplateChild(PART_ResizeBottomRight) as FrameworkElement;
        _resizeGripPart = GetTemplateChild(PART_ResizeGrip) as FrameworkElement;

        SubscribeToTemplateParts();

        if (_titleBarPart is not null)
        {
            _titleBarPart.Visibility = _pendingTitleBarVisible ? Visibility.Visible : Visibility.Collapsed;
            _titleBarPart.Height = _pendingTitleBarHeight;
        }

        if (_window is not null)
        {
            UpdateResizeMode(_window.ResizeMode);
            var chrome = System.Windows.Shell.WindowChrome.GetWindowChrome(_window);
            if (chrome is not null)
            {
                UpdateResizeBorderThickness(chrome.ResizeBorderThickness);
            }
        }
    }

    private void SubscribeToTemplateParts()
    {
        if (_titleBarPart is not null)
        {
            _titleBarPart.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        }

        if (_minimizeButtonPart is not null)
        {
            _minimizeButtonPart.Click += MinimizeButton_Click;
        }

        if (_maximizeButtonPart is not null)
        {
            _maximizeButtonPart.Click += MaximizeButton_Click;
        }

        if (_closeButtonPart is not null)
        {
            _closeButtonPart.Click += CloseButton_Click;
        }

        if (_resizeLeftPart is not null) _resizeLeftPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        if (_resizeRightPart is not null) _resizeRightPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomPart is not null) _resizeBottomPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomLeftPart is not null) _resizeBottomLeftPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomRightPart is not null) _resizeBottomRightPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        if (_resizeGripPart is not null) _resizeGripPart.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
    }

    private void UnsubscribeFromTemplateParts()
    {
        if (_titleBarPart is not null)
        {
            _titleBarPart.MouseLeftButtonDown -= TitleBar_MouseLeftButtonDown;
        }

        if (_minimizeButtonPart is not null)
        {
            _minimizeButtonPart.Click -= MinimizeButton_Click;
        }

        if (_maximizeButtonPart is not null)
        {
            _maximizeButtonPart.Click -= MaximizeButton_Click;
        }

        if (_closeButtonPart is not null)
        {
            _closeButtonPart.Click -= CloseButton_Click;
        }

        if (_resizeLeftPart is not null) _resizeLeftPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        if (_resizeRightPart is not null) _resizeRightPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomPart is not null) _resizeBottomPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomLeftPart is not null) _resizeBottomLeftPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        if (_resizeBottomRightPart is not null) _resizeBottomRightPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        if (_resizeGripPart is not null) _resizeGripPart.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window is not null && _window.IsVisible)
        {
            _window.DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_window is not null)
        {
            _window.WindowState = WindowState.Minimized;
        }
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_window is not null)
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _window?.Close();
    }

    private void ResizeEdge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window is null || _window.WindowState == WindowState.Maximized) return;

        WindowResizeEdge edge = GetEdgeFromSender(sender);
        if (edge != (WindowResizeEdge)(-1))
        {
            _window.DragResize(edge);
            e.Handled = true;
        }
    }

    private WindowResizeEdge GetEdgeFromSender(object sender)
    {
        if (sender == _resizeLeftPart) return WindowResizeEdge.Left;
        if (sender == _resizeRightPart) return WindowResizeEdge.Right;
        if (sender == _resizeBottomPart) return WindowResizeEdge.Bottom;
        if (sender == _resizeBottomLeftPart) return WindowResizeEdge.BottomLeft;
        if (sender == _resizeBottomRightPart || sender == _resizeGripPart) return WindowResizeEdge.BottomRight;

        return (WindowResizeEdge)(-1);
    }

    internal void SetLayoutSize()
    {
        Size availableSize = GetAvailableSize();
        InvalidateMeasure();
        Measure(availableSize);
        Arrange(new Rect(new Point(), DesiredSize));
        UpdateLayout();
    }

    private Size GetAvailableSize()
    {
        if (_window is not null && _window.WindowState == WindowState.Maximized)
        {
            Window viewport = Application.Current?.MainWindow;
            if (viewport is not null)
            {
                Rect bounds = viewport.Bounds;
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    return bounds.Size;
                }
            }
        }

        // When normal, let the content determine the size
        return new Size(double.PositiveInfinity, double.PositiveInfinity);
    }
}
