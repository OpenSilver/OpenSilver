
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OpenSilver.Controls;

/// <summary>
/// Represents a minimized window item in the taskbar. Can be restyled via theme resource dictionaries.
/// </summary>
[TemplatePart(Name = PART_RestoreButton, Type = typeof(System.Windows.Controls.Primitives.ButtonBase))]
public class TaskbarItem : Control
{
    private const string PART_RestoreButton = "PART_RestoreButton";

    private Window _window;
    private System.Windows.Controls.Primitives.ButtonBase _restoreButtonPart;

    static TaskbarItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskbarItem), new PropertyMetadata(typeof(TaskbarItem)));
    }

    internal TaskbarItem(Window window)
    {
        _window = window;
        SetBinding(TitleProperty, new Binding(nameof(Window.Title)) { Source = window });
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
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(TaskbarItem),
            new PropertyMetadata(string.Empty));

    public override void OnApplyTemplate()
    {
        if (_restoreButtonPart is not null)
        {
            _restoreButtonPart.Click -= RestoreButton_Click;
        }

        base.OnApplyTemplate();

        _restoreButtonPart = GetTemplateChild(PART_RestoreButton) as System.Windows.Controls.Primitives.ButtonBase;

        if (_restoreButtonPart is not null)
        {
            _restoreButtonPart.Click += RestoreButton_Click;
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        Restore();
        e.Handled = true;
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        Restore();
    }

    private void Restore()
    {
        if (_window is not null)
        {
            _window.WindowState = WindowState.Normal;
        }
    }
}
