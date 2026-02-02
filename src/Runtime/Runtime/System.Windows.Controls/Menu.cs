// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Represents a Windows menu control that enables you to hierarchically organize elements 
/// associated with commands and event handlers.
/// </summary>
[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MenuItem))]
public class Menu : MenuBase
{
    static Menu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new PropertyMetadata(typeof(Menu)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    public Menu()
    {
    }

    /// <summary>
    /// Identifies the <see cref="IsMainMenu"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMainMenuProperty =
        DependencyProperty.Register(
            nameof(IsMainMenu),
            typeof(bool),
            typeof(Menu),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether this <see cref="Menu"/> receives a main menu activation notification.
    /// </summary>
    /// <value>
    /// <c>true</c> if the menu receives a main menu activation notification; otherwise, <c>false</c>. 
    /// The default is <c>true</c>.
    /// </value>
    /// <remarks>
    /// If there are multiple <see cref="Menu"/> controls on a page, menus that should not receive 
    /// ALT or F10 key notifications should set this property to <c>false</c>.
    /// </remarks>
    public bool IsMainMenu
    {
        get => (bool)GetValue(IsMainMenuProperty);
        set => SetValueInternal(IsMainMenuProperty, value);
    }

    /// <summary>
    /// Called when the left mouse button is pressed.
    /// </summary>
    /// <param name="e">The event data for the <see cref="UIElement.MouseLeftButtonDown"/> event.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Responds to the <see cref="UIElement.KeyDown"/> event.
    /// </summary>
    /// <param name="e">The event data for the <see cref="UIElement.KeyDown"/> event.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Up:
            case Key.Down:
                FocusNextItem(e.Key == Key.Down);
                e.Handled = true;
                break;
            case Key.Left:
            case Key.Right:
                FocusNextItem(e.Key == Key.Right);
                e.Handled = true;
                break;
            case Key.Escape:
                // Clear focus from menu
                e.Handled = true;
                break;
        }
    }

    /// <summary>
    /// Sets focus to the next item in the Menu.
    /// </summary>
    /// <param name="forward">True to move the focus forward; false to move it backward.</param>
    private void FocusNextItem(bool forward)
    {
        int count = Items.Count;
        if (count == 0)
        {
            return;
        }

        int startingIndex = forward ? -1 : count;
        if (FocusManager.GetFocusedElement() is MenuItem focusedMenuItem && this == focusedMenuItem.ParentMenuBase)
        {
            startingIndex = ItemContainerGenerator.IndexFromContainer(focusedMenuItem);
        }

        int index = startingIndex;
        do
        {
            index = (index + count + (forward ? 1 : -1)) % count;
            if (ItemContainerGenerator.ContainerFromIndex(index) is MenuItem container)
            {
                if (container.IsEnabled && container.Focus())
                {
                    break;
                }
            }
        }
        while (index != startingIndex);
    }

    /// <summary>
    /// Called when a child <see cref="MenuItem"/> is clicked.
    /// </summary>
    internal override void ChildMenuItemClicked()
    {
        // Close all submenus and exit menu mode
        base.ChildMenuItemClicked();
    }
}
