
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
using CSHTML5.Internal;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace OpenSilver.Internal.Controls;

internal sealed class WindowHost : FrameworkElement
{
    private readonly Window _window;
    private Border _titleBar;
    private Grid _rootGrid;

    internal WindowHost(Window window)
    {
        Debug.Assert(window is not null);

        BypassLayoutPolicies = true;

        _window = window;

        BuildVisualTree();
    }

    internal bool IsOpen { get; private set; }

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

        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_rootGrid, this);

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
        if (_titleBar is not null)
        {
            _titleBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal void UpdateTitleBarHeight(double captionHeight)
    {
        if (_rootGrid is not null && _rootGrid.RowDefinitions.Count > 0)
        {
            _rootGrid.RowDefinitions[0].Height = new GridLength(captionHeight);
        }
    }

    private void BuildVisualTree()
    {
        _rootGrid = new Grid();
        _rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
        _rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        _titleBar = CreateTitleBar();
        Grid.SetRow(_titleBar, 0);
        _rootGrid.Children.Add(_titleBar);

        Grid.SetRow(_window, 1);
        _rootGrid.Children.Add(_window);

        AddVisualChild(_rootGrid);
    }

    private Border CreateTitleBar()
    {
        var titleBar = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 120, 215)),
        };

        var titleBarGrid = new Grid();
        titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var titleText = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0),
            Foreground = new SolidColorBrush(Colors.White),
            FontSize = 12,
        };
        titleText.SetBinding(TextBlock.TextProperty,
            new Binding(Window.TitleProperty) { Source = _window });
        titleBarGrid.Children.Add(titleText);

        var minimizeButton = CreateCaptionButton("\u2013", 1);
        minimizeButton.Click += (_, _) => _window.WindowState = WindowState.Minimized;
        titleBarGrid.Children.Add(minimizeButton);

        var maximizeButton = CreateCaptionButton("\u25A1", 2);
        maximizeButton.Click += (_, _) =>
            _window.WindowState = _window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        titleBarGrid.Children.Add(maximizeButton);

        var closeButton = CreateCaptionButton("\u2715", 3);
        closeButton.Click += (_, _) => _window.Close();
        titleBarGrid.Children.Add(closeButton);

        titleBar.Child = titleBarGrid;
        titleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;

        return titleBar;
    }

    private static Button CreateCaptionButton(string glyph, int column)
    {
        var button = new Button
        {
            Content = glyph,
            FontSize = 13,
            Width = 46,
            BorderThickness = new Thickness(0),
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        Grid.SetColumn(button, column);
        return button;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_window.IsVisible)
        {
            _window.DragMove();
        }
    }

    protected override int VisualChildrenCount => 1;

    protected override UIElement GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return _rootGrid;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _rootGrid.Measure(availableSize);
        return _rootGrid.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _rootGrid.Arrange(new Rect(finalSize));
        return finalSize;
    }

    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent) =>
        throw new InvalidOperationException("'CreateDomElement' should not be called for the WindowHost object.");

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
