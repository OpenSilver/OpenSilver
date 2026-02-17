// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Represents a control that defines choices for users to select.
/// </summary>
/// <QualityBand>Preview</QualityBand>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(MenuItem))]
public abstract class MenuBase : ItemsControl
{
    private MenuItem _currentSelection;
    private bool _isMenuMode;

    static MenuBase()
    {
        EventManager.RegisterClassHandler<MenuBase>(IsSelectedChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsSelectedChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuBase"/> class.
    /// </summary>
    public MenuBase() { }

    /// <summary>
    /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
    /// </summary>
    public new static readonly DependencyProperty ItemContainerStyleProperty =
        DependencyProperty.Register(
            nameof(ItemContainerStyle),
            typeof(Style),
            typeof(MenuBase),
            null);

    /// <summary>
    /// Gets or sets the Style that is applied to the container element generated for each item.
    /// </summary>
    public new Style ItemContainerStyle
    {
        get { return (Style)GetValue(ItemContainerStyleProperty); }
        set { SetValue(ItemContainerStyleProperty, value); }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Key.Escape:
                if (CurrentSelection is not null && CurrentSelection.IsSubmenuOpen)
                {
                    CurrentSelection.SetCurrentValueInternal(MenuItem.IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                    e.Handled = true;
                }
                else
                {
                    if (IsMenuMode)
                    {
                        IsMenuMode = false;
                    }
                    else
                    {
                        CurrentSelection = null;
                    }
                    e.Handled = true;
                }
                break;
        }
    }

    /// <summary>
    /// Determines whether the specified item is, or is eligible to be, its own item container.
    /// </summary>
    /// <param name="item">The item to check whether it is an item container.</param>
    /// <returns>True if the item is a MenuItem or a Separator; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is MenuItem || item is Separator;
    }

    /// <summary>
    /// Creates or identifies the element used to display the specified item.
    /// </summary>
    /// <returns>A MenuItem.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new MenuItem();
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">Element used to display the specified item.</param>
    /// <param name="item">Specified item.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is MenuItem menuItem)
        {
            if (menuItem != item)
            {
                // Copy the ItemsControl properties from parent to child
                DataTemplate itemTemplate = ItemTemplate;
                Style itemContainerStyle = ItemContainerStyle;
                if (itemTemplate != null)
                {
                    menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                }
                if (itemContainerStyle != null && menuItem.HasDefaultValue(HeaderedItemsControl.ItemContainerStyleProperty))
                {
                    menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);
                }

                // Copy the Header properties from parent to child
                if (menuItem.HasDefaultValue(HeaderedItemsControl.HeaderProperty))
                {
                    menuItem.Header = item;
                }
                if (itemTemplate != null)
                {
                    menuItem.SetValue(HeaderedItemsControl.HeaderTemplateProperty, itemTemplate);
                }
                if (itemContainerStyle != null)
                {
                    menuItem.SetValue(HeaderedItemsControl.StyleProperty, itemContainerStyle);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the currently selected MenuItem.
    /// </summary>
    internal MenuItem CurrentSelection
    {
        get => _currentSelection;
        set
        {
            // Even if we don't have capture we should move focus when one item is already focused.
            bool wasFocused = false;

            if (_currentSelection is not null)
            {
                wasFocused = FocusManager.GetFocusedElement() == _currentSelection;
                _currentSelection.SetCurrentValueInternal(MenuItem.IsSelectedProperty, BooleanBoxes.FalseBox);
            }

            _currentSelection = value;

            if (_currentSelection is not null)
            {
                _currentSelection.SetCurrentValueInternal(MenuItem.IsSelectedProperty, BooleanBoxes.TrueBox);
                if (wasFocused)
                {
                    _currentSelection.Focus();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the menu is in menu mode (submenus open on hover).
    /// </summary>
    internal bool IsMenuMode
    {
        get => _isMenuMode;
        set
        {
            if (_isMenuMode == value)
            {
                return;
            }

            _isMenuMode = value;

            if (!value)
            {
                if (CurrentSelection is MenuItem selectedMenuItem)
                {
                    if (selectedMenuItem.IsSubmenuOpen)
                    {
                        selectedMenuItem.SetCurrentValueInternal(MenuItem.IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                    }

                    CurrentSelection = null;
                }

                Focus();
            }

            MenuModeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    internal event EventHandler MenuModeChanged;

    internal static readonly RoutedEvent IsSelectedChangedEvent =
        EventManager.RegisterRoutedEvent(
            "IsSelectedChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>),
            typeof(MenuBase));

    private static void OnIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
    {
        // We assume that within a menu the only top-level menu items are direct children of
        // the one and only top-level menu.
        if (e.OriginalSource is not MenuItem newSelectedMenuItem)
        {
            return;
        }

        MenuBase menu = (MenuBase)sender;

        // If the selected item is a child of ours, make it the current selection.
        // If the selection changes from a top-level menu item with its submenu
        // open to another, the new selection's submenu should be open.
        if (e.NewValue)
        {
            if (menu.CurrentSelection != newSelectedMenuItem && newSelectedMenuItem.LogicalParent == menu)
            {
                bool wasSubmenuOpen = false;

                if (menu.CurrentSelection is not null)
                {
                    wasSubmenuOpen = menu.CurrentSelection.IsSubmenuOpen;
                    menu.CurrentSelection.SetCurrentValueInternal(MenuItem.IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                }

                menu.CurrentSelection = newSelectedMenuItem;
                if (menu.CurrentSelection is not null && wasSubmenuOpen)
                {
                    // Only open the submenu if it's a header (i.e. has items)
                    MenuItemRole role = menu.CurrentSelection.Role;

                    if (role == MenuItemRole.SubmenuHeader || role == MenuItemRole.TopLevelHeader)
                    {
                        if (menu.CurrentSelection.IsSubmenuOpen != wasSubmenuOpen)
                        {
                            menu.CurrentSelection.SetCurrentValueInternal(MenuItem.IsSubmenuOpenProperty, BooleanBoxes.Box(wasSubmenuOpen));
                        }
                    }
                }
            }
        }
        else
        {
            // As in MenuItem.OnIsSelectedChanged, if the item is deselected
            // and it's our current selection, set CurrentSelection to null.
            if (menu.CurrentSelection == newSelectedMenuItem)
            {
                menu.CurrentSelection = null;
            }
        }

        e.Handled = true;
    }

    internal void OnMenuItemPreviewClick(MenuItem menuItem)
    {
        if (!menuItem.StaysOpenOnClick)
        {
            MenuItemRole role = menuItem.Role;

            if (role == MenuItemRole.TopLevelItem || role == MenuItemRole.SubmenuItem)
            {
                IsMenuMode = false;
            }
        }
    }
}