
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
using System.Windows.Media;
using System.Windows.Shell;
using CSHTML5.Internal;
using OpenSilver.Internal;

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
    private const string PART_RestoreButton = "PART_RestoreButton";
    private const string PART_CloseButton = "PART_CloseButton";
    private const string PART_ResizeTop = "PART_ResizeTop";
    private const string PART_ResizeLeft = "PART_ResizeLeft";
    private const string PART_ResizeRight = "PART_ResizeRight";
    private const string PART_ResizeBottom = "PART_ResizeBottom";
    private const string PART_ResizeTopLeft = "PART_ResizeTopLeft";
    private const string PART_ResizeTopRight = "PART_ResizeTopRight";
    private const string PART_ResizeBottomLeft = "PART_ResizeBottomLeft";
    private const string PART_ResizeBottomRight = "PART_ResizeBottomRight";
    private const string PART_ResizeGrip = "PART_ResizeGrip";
    private const string PART_ChromeButtons = "PART_ChromeButtons";
    private const string PART_ChromeMinimizeButton = "PART_ChromeMinimizeButton";
    private const string PART_ChromeMaximizeButton = "PART_ChromeMaximizeButton";
    private const string PART_ChromeRestoreButton = "PART_ChromeRestoreButton";
    private const string PART_ChromeCloseButton = "PART_ChromeCloseButton";
    private const string PART_DragArea = "PART_DragArea";
    private const string PART_ContentHost = "PART_ContentHost";

    private readonly Window _window;
    private FrameworkElement _contentHostPart;
    private FrameworkElement _titleBarPart;
    private ButtonBase _minimizeButtonPart;
    private ButtonBase _maximizeButtonPart;
    private ButtonBase _restoreButtonPart;
    private ButtonBase _closeButtonPart;
    private FrameworkElement _resizeTopPart;
    private FrameworkElement _resizeLeftPart;
    private FrameworkElement _resizeRightPart;
    private FrameworkElement _resizeBottomPart;
    private FrameworkElement _resizeTopLeftPart;
    private FrameworkElement _resizeTopRightPart;
    private FrameworkElement _resizeBottomLeftPart;
    private FrameworkElement _resizeBottomRightPart;
    private FrameworkElement _resizeGripPart;
    private FrameworkElement _chromeButtonsPart;
    private ButtonBase _chromeMinimizeButtonPart;
    private ButtonBase _chromeMaximizeButtonPart;
    private ButtonBase _chromeRestoreButtonPart;
    private ButtonBase _chromeCloseButtonPart;
    private FrameworkElement _dragAreaPart;
    private bool _hasChromeOverride;
    private double _pendingTitleBarHeight = 30;

    static WindowHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowHost), new PropertyMetadata(typeof(WindowHost)));
        EventManager.RegisterClassHandler<WindowHost>(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), true);
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) => ((WindowHost)sender)._window.Activate();

    internal WindowHost(Window window)
    {
        Debug.Assert(window is not null);

        BypassLayoutPolicies = true;

        _window = window;
        Content = window;

        SetBinding(TitleProperty, new Binding(Window.TitleProperty) { Source = window });
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        Size size = base.MeasureOverride(availableSize);
        return _contentHostPart?.DesiredSize ?? size;
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
    public static readonly DependencyProperty TitleProperty = Window.TitleProperty.AddOwner(typeof(WindowHost));

    /// <summary>
    /// Gets or sets the background brush for the window chrome (title bar).
    /// </summary>
    public Brush ChromeBackground
    {
        get => (Brush)GetValue(ChromeBackgroundProperty);
        set => SetValueInternal(ChromeBackgroundProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ChromeBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChromeBackgroundProperty =
        DependencyProperty.Register(
            nameof(ChromeBackground),
            typeof(Brush),
            typeof(WindowHost),
            new PropertyMetadata(null));


    internal bool IsOpen { get; private set; }

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

        // Build the template (and apply the chrome mode) before the first layout, so
        // GetAvailableSize sees the correct chrome state (_hasChromeOverride, title bar)
        // rather than stale defaults, which would otherwise size the host wrong until the
        // next layout pass (e.g. a resize drag).
        ApplyTemplate();

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
        INTERNAL_VisualTreeManager.DetachWindowHost(this);
        IsLoadedCache = false;
        IsConnectedToLiveTree = false;
        UpdateIsRenderableCache();
        UpdateIsVisibleCache();
        PropagateSuspendLayout(this);
    }

    internal void UpdateResizeBorderThickness(Thickness thickness)
    {
        _resizeTopPart?.Height = thickness.Top;
        _resizeLeftPart?.Width = thickness.Left;
        _resizeRightPart?.Width = thickness.Right;
        _resizeBottomPart?.Height = thickness.Bottom;
        if (_resizeTopLeftPart is not null) { _resizeTopLeftPart.Width = thickness.Left; _resizeTopLeftPart.Height = thickness.Top; }
        if (_resizeTopRightPart is not null) { _resizeTopRightPart.Width = thickness.Right; _resizeTopRightPart.Height = thickness.Top; }
        if (_resizeBottomLeftPart is not null) { _resizeBottomLeftPart.Width = thickness.Left; _resizeBottomLeftPart.Height = thickness.Bottom; }
        if (_resizeBottomRightPart is not null) { _resizeBottomRightPart.Width = thickness.Right; _resizeBottomRightPart.Height = thickness.Bottom; }
    }


    

    internal void UpdateMaximizeRestoreButton(bool isMaximized)
    {
        Visibility maxVisibility = _window.ResizeMode == ResizeMode.NoResize ? Visibility.Collapsed : Visibility.Visible;
        _maximizeButtonPart?.Visibility = isMaximized ? Visibility.Collapsed : maxVisibility;
        _restoreButtonPart?.Visibility = isMaximized ? maxVisibility : Visibility.Collapsed;
        _chromeMaximizeButtonPart?.Visibility = isMaximized ? Visibility.Collapsed : maxVisibility;
        _chromeRestoreButtonPart?.Visibility = isMaximized ? maxVisibility : Visibility.Collapsed;
    }


    #region Handling WindowChrome

    internal void ApplyWindowChromeMode()
    {
        ApplyWindowChromeMode(WindowChrome.GetWindowChrome(_window));
    }

    internal void ApplyWindowChromeMode(WindowChrome windowChrome)
    {
        
        _hasChromeOverride = windowChrome != null;
        Thickness resizeThickness = new Thickness(8);
        if (_hasChromeOverride)
        {
            _dragAreaPart?.Visibility = Visibility.Visible;
            resizeThickness = windowChrome.ResizeBorderThickness != null ? windowChrome.ResizeBorderThickness : resizeThickness;
        }
        else
        {
            _dragAreaPart?.Visibility = Visibility.Collapsed;
        }
        ApplyWindowStyle();
        UpdateResizeBorderThickness(resizeThickness);
    }

    #endregion

    #region handle WindowStyle

    internal void ApplyWindowStyle()
    {
        switch (_window.WindowStyle)
        {
            case WindowStyle.None:
                _titleBarPart?.Visibility = Visibility.Collapsed;
                _chromeButtonsPart?.Visibility = Visibility.Collapsed;
                break;
            default:
                _titleBarPart?.Visibility = _hasChromeOverride ? Visibility.Collapsed : Visibility.Visible;
                _chromeButtonsPart?.Visibility = _hasChromeOverride ? Visibility.Visible : Visibility.Collapsed;
                break;
        }
    }

    internal void HideTitleBar()
    {
        _titleBarPart?.Visibility = Visibility.Collapsed;
        _chromeButtonsPart?.Visibility = Visibility.Collapsed;
    }

    #endregion


    #region handle ResizeMode

    internal void UpdateResizeMode(ResizeMode mode)
    {
        Visibility buttonsVisibility = Visibility.Visible;
        bool isGripVisible = false;
        bool areResizeBordersVisible = true;

        bool canMaximize = true;
        double maximizeButtonOpacity = 1.0;

        switch (mode)
        {
            case ResizeMode.NoResize:
                buttonsVisibility = Visibility.Collapsed;
                areResizeBordersVisible = false;
                break;
            case ResizeMode.CanMinimize:
                canMaximize = false;
                areResizeBordersVisible = false;
                maximizeButtonOpacity = 0.4;
                break;
            case ResizeMode.CanResize:
                break;
            case ResizeMode.CanResizeWithGrip:
                isGripVisible = true;
                break;
            default:
                break;
        }

        // Buttons:
        _minimizeButtonPart?.Visibility = buttonsVisibility;
        _chromeMinimizeButtonPart?.Visibility = buttonsVisibility;
        _maximizeButtonPart?.Visibility = buttonsVisibility;
        _chromeMaximizeButtonPart?.Visibility = buttonsVisibility;
        _restoreButtonPart?.Visibility = buttonsVisibility;

        //Maximize button specifics:
        _maximizeButtonPart.IsHitTestVisible = canMaximize;
        _maximizeButtonPart.Opacity = maximizeButtonOpacity;
        _restoreButtonPart.IsHitTestVisible = canMaximize;
        _restoreButtonPart.Opacity = maximizeButtonOpacity;


        SetResizeBordersVisible(areResizeBordersVisible, isGripVisible);

        _resizeGripPart?.Visibility = mode == ResizeMode.CanResizeWithGrip ? Visibility.Visible : Visibility.Collapsed;
    }


    internal void SetResizeBordersVisible(bool visible, bool gripVisible)
    {
        var visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        var gripVisibility = (visible && gripVisible) ? Visibility.Visible : Visibility.Collapsed;

        _resizeTopPart?.Visibility = visibility;
        _resizeLeftPart?.Visibility = visibility;
        _resizeRightPart?.Visibility = visibility;
        _resizeBottomPart?.Visibility = visibility;
        _resizeTopLeftPart?.Visibility = visibility;
        _resizeTopRightPart?.Visibility = visibility;
        _resizeBottomLeftPart?.Visibility = visibility;
        _resizeBottomRightPart?.Visibility = visibility;
        _resizeGripPart?.Visibility = gripVisibility;
    }
    #endregion


    internal double GetMinDraggableMargin()
    {
        // We need at least some pixels of draggable title bar area visible.
        // Measure the buttons' total width + a small grab area.
        double buttonsWidth = 0;
        if (_minimizeButtonPart?.Visibility == Visibility.Visible)
            buttonsWidth += _minimizeButtonPart.ActualWidth;
        if (_maximizeButtonPart?.Visibility == Visibility.Visible)
            buttonsWidth += _maximizeButtonPart.ActualWidth;
        if (_restoreButtonPart?.Visibility == Visibility.Visible)
            buttonsWidth += _restoreButtonPart.ActualWidth;
        if (_closeButtonPart?.Visibility == Visibility.Visible)
            buttonsWidth += _closeButtonPart.ActualWidth;

        // Add a minimum grab area (at least 40px of draggable space beyond buttons)
        return buttonsWidth + 40;
    }


    internal void UpdateTitleBarHeight(double captionHeight)
    {
        _pendingTitleBarHeight = captionHeight;
        _titleBarPart?.Height = captionHeight;
        _dragAreaPart?.Height = captionHeight;
    }

    internal double GetTitleBarHeight()
    {
        if (_titleBarPart?.Visibility == Visibility.Visible)
        {
            return _titleBarPart.ActualHeight > 0 ? _titleBarPart.ActualHeight : _pendingTitleBarHeight;
        }
        return _pendingTitleBarHeight;
    }

    public override void OnApplyTemplate()
    {
        UnsubscribeFromTemplateParts();

        base.OnApplyTemplate();

        _contentHostPart = GetTemplateChild(PART_ContentHost) as FrameworkElement;
        _titleBarPart = GetTemplateChild(PART_TitleBar) as FrameworkElement;
        _minimizeButtonPart = GetTemplateChild(PART_MinimizeButton) as ButtonBase;
        _maximizeButtonPart = GetTemplateChild(PART_MaximizeButton) as ButtonBase;
        _restoreButtonPart = GetTemplateChild(PART_RestoreButton) as ButtonBase;
        _closeButtonPart = GetTemplateChild(PART_CloseButton) as ButtonBase;
        _resizeTopPart = GetTemplateChild(PART_ResizeTop) as FrameworkElement;
        _resizeLeftPart = GetTemplateChild(PART_ResizeLeft) as FrameworkElement;
        _resizeRightPart = GetTemplateChild(PART_ResizeRight) as FrameworkElement;
        _resizeBottomPart = GetTemplateChild(PART_ResizeBottom) as FrameworkElement;
        _resizeTopLeftPart = GetTemplateChild(PART_ResizeTopLeft) as FrameworkElement;
        _resizeTopRightPart = GetTemplateChild(PART_ResizeTopRight) as FrameworkElement;
        _resizeBottomLeftPart = GetTemplateChild(PART_ResizeBottomLeft) as FrameworkElement;
        _resizeBottomRightPart = GetTemplateChild(PART_ResizeBottomRight) as FrameworkElement;
        _resizeGripPart = GetTemplateChild(PART_ResizeGrip) as FrameworkElement;
        _chromeButtonsPart = GetTemplateChild(PART_ChromeButtons) as FrameworkElement;
        _chromeMinimizeButtonPart = GetTemplateChild(PART_ChromeMinimizeButton) as ButtonBase;
        _chromeMaximizeButtonPart = GetTemplateChild(PART_ChromeMaximizeButton) as ButtonBase;
        _chromeRestoreButtonPart = GetTemplateChild(PART_ChromeRestoreButton) as ButtonBase;
        _chromeCloseButtonPart = GetTemplateChild(PART_ChromeCloseButton) as ButtonBase;
        _dragAreaPart = GetTemplateChild(PART_DragArea) as FrameworkElement;

        SubscribeToTemplateParts();

        _titleBarPart?.Height = _pendingTitleBarHeight;
        _dragAreaPart?.Height = _pendingTitleBarHeight;


        ApplyWindowChromeMode();
        UpdateResizeMode(_window.ResizeMode);
    }

    private void SubscribeToTemplateParts()
    {
        _titleBarPart?.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        _minimizeButtonPart?.Click += MinimizeButton_Click;
        _maximizeButtonPart?.Click += MaximizeButton_Click;
        _restoreButtonPart?.Click += MaximizeButton_Click;
        _closeButtonPart?.Click += CloseButton_Click;
        _resizeTopPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeLeftPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeRightPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeBottomPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeTopLeftPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeTopRightPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeBottomLeftPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeBottomRightPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _resizeGripPart?.MouseLeftButtonDown += ResizeEdge_MouseLeftButtonDown;
        _chromeMinimizeButtonPart?.Click += MinimizeButton_Click;
        _chromeMaximizeButtonPart?.Click += MaximizeButton_Click;
        _chromeRestoreButtonPart?.Click += MaximizeButton_Click;
        _chromeCloseButtonPart?.Click += CloseButton_Click;
        _dragAreaPart?.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
    }

    private void UnsubscribeFromTemplateParts()
    {
        _titleBarPart?.MouseLeftButtonDown -= TitleBar_MouseLeftButtonDown;
        _minimizeButtonPart?.Click -= MinimizeButton_Click;
        _maximizeButtonPart?.Click -= MaximizeButton_Click;
        _restoreButtonPart?.Click -= MaximizeButton_Click;
        _closeButtonPart?.Click -= CloseButton_Click;
        _resizeTopPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeLeftPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeRightPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeBottomPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeTopLeftPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeTopRightPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeBottomLeftPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeBottomRightPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _resizeGripPart?.MouseLeftButtonDown -= ResizeEdge_MouseLeftButtonDown;
        _chromeMinimizeButtonPart?.Click -= MinimizeButton_Click;
        _chromeMaximizeButtonPart?.Click -= MaximizeButton_Click;
        _chromeRestoreButtonPart?.Click -= MaximizeButton_Click;
        _chromeCloseButtonPart?.Click -= CloseButton_Click;
        _dragAreaPart?.MouseLeftButtonDown -= DragArea_MouseLeftButtonDown;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_window.IsVisible || _window.WindowState == WindowState.Maximized) return;

        if (_window.ResizeMode >= ResizeMode.CanResize)
        {
            Point pos = e.GetPosition(_titleBarPart);
            double topThreshold = 8;
            var chrome = System.Windows.Shell.WindowChrome.GetWindowChrome(_window);
            if (chrome is not null)
            {
                topThreshold = chrome.ResizeBorderThickness.Top;
            }

            if (pos.Y < topThreshold)
            {
                double leftThreshold = topThreshold;
                double rightThreshold = _titleBarPart.ActualWidth - topThreshold;

                WindowResizeEdge edge;
                if (pos.X < leftThreshold)
                    edge = WindowResizeEdge.TopLeft;
                else if (pos.X > rightThreshold)
                    edge = WindowResizeEdge.TopRight;
                else
                    edge = WindowResizeEdge.Top;

                _window.DragResize(edge);
                e.Handled = true;
                return;
            }
        }

        _window.DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => _window.WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
        _window.WindowState = _window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseButton_Click(object sender, RoutedEventArgs e) => _window.Close();

    private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window.IsVisible)
        {
            _window.DragMove();
        }
    }

    private void ResizeEdge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window.WindowState == WindowState.Maximized)
        {
            return;
        }

        WindowResizeEdge edge = GetEdgeFromSender(sender);
        if (edge != (WindowResizeEdge)(-1))
        {
            _window.DragResize(edge);
            e.Handled = true;
        }
    }

    private WindowResizeEdge GetEdgeFromSender(object sender)
    {
        if (sender == _resizeTopPart) return WindowResizeEdge.Top;
        if (sender == _resizeLeftPart) return WindowResizeEdge.Left;
        if (sender == _resizeRightPart) return WindowResizeEdge.Right;
        if (sender == _resizeBottomPart) return WindowResizeEdge.Bottom;
        if (sender == _resizeTopLeftPart) return WindowResizeEdge.TopLeft;
        if (sender == _resizeTopRightPart) return WindowResizeEdge.TopRight;
        if (sender == _resizeBottomLeftPart) return WindowResizeEdge.BottomLeft;
        if (sender == _resizeBottomRightPart || sender == _resizeGripPart) return WindowResizeEdge.BottomRight;

        return (WindowResizeEdge)(-1);
    }

    internal void SetLayoutSize()
    {
        Size availableSize = GetAvailableSize();
        InvalidateMeasure();
        Measure(availableSize);

        if (_window.WindowState == WindowState.Maximized)
        {
            Arrange(new Rect(new Point(), availableSize));
            UpdateLayout();

            if (OuterDiv.IsConnected)
            {
                OuterDiv.SetCssStyleProperty(CssPropertyNames.Width, $"{Math.Round(availableSize.Width, 2).ToInvariantString()}px");
                OuterDiv.SetCssStyleProperty(CssPropertyNames.Height, $"{Math.Round(availableSize.Height, 2).ToInvariantString()}px");
            }
            return;
        }

        double frameW = double.IsNaN(_window.Width) ? DesiredSize.Width : availableSize.Width;
        double frameH = double.IsNaN(_window.Height) ? DesiredSize.Height : availableSize.Height;

        Arrange(new Rect(new Point(), new Size(frameW, frameH)));
        UpdateLayout();

        // Size the OuterDiv (bypass-layout, so not sized by ArrangeNative) to the frame.
        // Its CSS overflow:hidden clips the chrome/content overflow, while its own
        // box-shadow renders the frame's shadow unclipped.
        if (OuterDiv.IsConnected)
        {
            OuterDiv.SetCssStyleProperty(CssPropertyNames.Width, $"{Math.Round(frameW, 2).ToInvariantString()}px");
            OuterDiv.SetCssStyleProperty(CssPropertyNames.Height, $"{Math.Round(frameH, 2).ToInvariantString()}px");
        }
    }

    private Size GetAvailableSize()
    {
        if (_window.WindowState == WindowState.Maximized)
        {
            Window viewport = Application.Current?.MainWindow;
            if (viewport is not null)
            {
                Rect bounds = viewport.Bounds;
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    double chrome = GetStackedChromeHeight();
                    double vMaxW = _window.MaxWidth;
                    double vMaxH = _window.MaxHeight;
                    double mw = double.IsPositiveInfinity(vMaxW) ? bounds.Width : Math.Min(bounds.Width, vMaxW);
                    double mh = double.IsPositiveInfinity(vMaxH) ? bounds.Height : Math.Min(bounds.Height, vMaxH + chrome);
                    return new Size(mw, mh);
                }
            }
        }

        // Window.Width/Height (and their Max) describe the CONTENT (the inner Window). When
        // a real title bar is stacked above the content (no WindowChrome, WindowStyle != None),
        // the frame is taller by that title bar. With WindowChrome, the caption overlays the
        // content, so nothing is added.
        double chromeH = GetStackedChromeHeight();
        double w = _window.Width;
        double h = _window.Height;
        double maxW = _window.MaxWidth;
        double maxH = _window.MaxHeight;

        double availW = double.IsNaN(w) ? double.PositiveInfinity : w;
        double availH = double.IsNaN(h) ? double.PositiveInfinity : h + chromeH;

        if (!double.IsPositiveInfinity(maxW))
            availW = Math.Min(availW, maxW);
        if (!double.IsPositiveInfinity(maxH))
            availH = Math.Min(availH, maxH + chromeH);

        return new Size(availW, availH);
    }

    /// <summary>
    /// Height added by chrome that is stacked above the content (rather than overlaying it).
    /// This is the title bar height in the default chrome; it is 0 when a WindowChrome is set
    /// (the caption overlays the content) or when there is no title bar (WindowStyle.None).
    /// </summary>
    private double GetStackedChromeHeight()
    {
        if (_hasChromeOverride || _window.WindowStyle == WindowStyle.None)
        {
            return 0;
        }

        if (_titleBarPart is null || _titleBarPart.Visibility != Visibility.Visible)
        {
            return 0;
        }

        return GetTitleBarHeight();
    }
}
