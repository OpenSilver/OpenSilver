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
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(MenuItem))]
public class Menu : MenuBase
{
    static Menu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new PropertyMetadata(typeof(Menu)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    public Menu() { }

    /// <summary>
    /// Identifies the <see cref="IsMainMenu"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
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
    [OpenSilver.NotImplemented]
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
                if (CurrentSelection is not null)
                {
                    // Only for non vertical layout Up/Down open the submenu
                    Panel itemsHost = ItemsHost;
                    bool isVertical = itemsHost is not null && itemsHost.HasLogicalOrientation && itemsHost.LogicalOrientation == Orientation.Vertical;
                    if (!isVertical)
                    {
                        CurrentSelection.OpenSubmenuWithKeyboard();
                        e.Handled = true;
                    }
                }
                break;
            case Key.Left:
            case Key.Right:
                if (CurrentSelection is not null)
                {
                    // Only for vertical layout Left/Right open the submenu
                    Panel itemsHost = ItemsHost;
                    bool isVertical = itemsHost is not null && itemsHost.HasLogicalOrientation && itemsHost.LogicalOrientation == Orientation.Vertical;
                    if (isVertical)
                    {
                        CurrentSelection.OpenSubmenuWithKeyboard();
                        e.Handled = true;
                    }
                }
                break;
        }
    }
}
