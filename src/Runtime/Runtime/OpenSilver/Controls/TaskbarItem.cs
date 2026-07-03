
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

using OpenSilver.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OpenSilver.Controls;

/// <summary>
/// Represents a window item in the taskbar.
/// </summary>
public class TaskbarItem : Control
{
    private readonly Window _window;

    static TaskbarItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskbarItem), new PropertyMetadata(typeof(TaskbarItem)));
    }

    internal TaskbarItem(Window window)
    {
        _window = window;
        SetBinding(TitleProperty, new Binding(Window.TitleProperty) { Source = window });
    }

    /// <summary>
    /// Gets or sets the title text displayed in the taskbar item.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValueInternal(TitleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = Window.TitleProperty.AddOwner(typeof(TaskbarItem));

    /// <summary>
    /// Gets or sets whether this is the currently active window.
    /// </summary>
    public bool IsActiveWindow
    {
        get => (bool)GetValue(IsActiveWindowProperty);
        internal set => SetValueInternal(IsActiveWindowProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsActiveWindow"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveWindowProperty =
        DependencyProperty.Register(
            nameof(IsActiveWindow),
            typeof(bool),
            typeof(TaskbarItem),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <inheritdoc/>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        ActivateWindow();
        e.Handled = true;
    }

    private void ActivateWindow()
    {
        if (_window is null) return;

        if (_window.WindowState == WindowState.Minimized)
        {
            _window.RestoreFromTaskbar();
        }

        _window.Activate();
    }
}
