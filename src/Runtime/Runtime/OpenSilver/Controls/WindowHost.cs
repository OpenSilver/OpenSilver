
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
public class WindowHost : ContentControl
{
    private const string PART_TitleBar = "PART_TitleBar";
    private const string PART_MinimizeButton = "PART_MinimizeButton";
    private const string PART_MaximizeButton = "PART_MaximizeButton";
    private const string PART_CloseButton = "PART_CloseButton";

    private Window _window;
    private FrameworkElement _titleBarPart;
    private ButtonBase _minimizeButtonPart;
    private ButtonBase _maximizeButtonPart;
    private ButtonBase _closeButtonPart;
    private bool _pendingTitleBarVisible = true;
    private double _pendingTitleBarHeight = 30;

    static WindowHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowHost), new PropertyMetadata(typeof(WindowHost)));
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

    internal void Show(HtmlElementReference overlayDiv, Window parentWindow)
    {
        IsOpen = true;

        ParentWindow = parentWindow;
        OuterDiv = INTERNAL_HtmlDomManager.CreatePopupRootDomElementAndAppendIt(overlayDiv, this);
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

        SubscribeToTemplateParts();

        if (_titleBarPart is not null)
        {
            _titleBarPart.Visibility = _pendingTitleBarVisible ? Visibility.Visible : Visibility.Collapsed;
            _titleBarPart.Height = _pendingTitleBarHeight;
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

    private void SetLayoutSize()
    {
        Size availableSize = GetAvailableSize();
        InvalidateMeasure();
        Measure(availableSize);
        Arrange(new Rect(new Point(), DesiredSize));
        UpdateLayout();
    }

    private Size GetAvailableSize()
    {
        if (ParentWindow is not null)
        {
            Rect bounds = ParentWindow.Bounds;
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                return bounds.Size;
            }
        }
        return new Size(double.PositiveInfinity, double.PositiveInfinity);
    }
}
