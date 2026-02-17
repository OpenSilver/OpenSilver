// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;
using OpenSilver.Internal.Commands;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Controls;

/// <summary>
/// Defines the different roles that a <see cref="MenuItem"/> can have.
/// </summary>
public enum MenuItemRole
{
    /// <summary>
    /// Top-level menu item that can invoke commands.
    /// </summary>
    TopLevelItem,

    /// <summary>
    /// Header for top-level menus.
    /// </summary>
    TopLevelHeader,

    /// <summary>
    /// Menu item in a submenu that can invoke commands.
    /// </summary>
    SubmenuItem,

    /// <summary>
    /// Header for a submenu.
    /// </summary>
    SubmenuHeader,
}

/// <summary>
/// Represents a selectable item inside a <see cref="Menu"/> or <see cref="ContextMenu"/>.
/// </summary>
/// <QualityBand>Preview</QualityBand>
[TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
[TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
[TemplatePart(Name = SubMenuPopupPartName, Type = typeof(Popup))]
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(MenuItem))]
public class MenuItem : HeaderedItemsControl, ICommandSource
{
    private const string SubMenuPopupPartName = "PART_Popup";

    private Popup _submenuPopup;
    private PopupRoot _submenuPopupRoot;
    private MenuItem _currentSelection;
    private bool _isFocused;
    private bool _canExecute = true;
    private CanExecuteChangedWeakEventListener _canExecuteChangedListener;
    private DispatcherTimer _openHierarchyTimer;

    static MenuItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new PropertyMetadata(typeof(MenuItem)));
        EventManager.RegisterClassHandler<MenuItem>(MenuBase.IsSelectedChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsSelectedChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    public MenuItem() { }

    /// <summary>
    /// Identifies the <see cref="Click"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ClickEvent =
        EventManager.RegisterRoutedEvent(
            nameof(Click),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(MenuItem));

    /// <summary>
    /// Occurs when a <see cref="MenuItem"/> is clicked.
    /// </summary>
    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    /// <summary>
    /// Identifies the <see cref="SubmenuOpened"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SubmenuOpenedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(SubmenuOpened),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(MenuItem));

    /// <summary>
    /// Occurs when the state of the <see cref="IsSubmenuOpen"/> property changes to true.
    /// </summary>
    public event RoutedEventHandler SubmenuOpened
    {
        add => AddHandler(SubmenuOpenedEvent, value);
        remove => RemoveHandler(SubmenuOpenedEvent, value);
    }

    /// <summary>
    /// Identifies the <see cref="SubmenuClosed"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SubmenuClosedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(SubmenuClosed),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(MenuItem));

    /// <summary>
    /// Occurs when the state of the <see cref="IsSubmenuOpen"/> property changes to false.
    /// </summary>
    public event RoutedEventHandler SubmenuClosed
    {
        add => AddHandler(SubmenuClosedEvent, value);
        remove => RemoveHandler(SubmenuClosedEvent, value);
    }

    /// <summary>
    /// Identifies the <see cref="StaysOpenOnClick"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StaysOpenOnClickProperty =
        DependencyProperty.Register(
            nameof(StaysOpenOnClick),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value that indicates that the submenu in which this <see cref="MenuItem"/> 
    /// is located should not close when this item is clicked.
    /// </summary>
    /// <returns>
    /// true if the submenu in which this <see cref="MenuItem"/> is located should not close when this 
    /// item is clicked; otherwise, false. The default is false.
    /// </returns>
    public bool StaysOpenOnClick
    {
        get => (bool)GetValue(StaysOpenOnClickProperty);
        set => SetValueInternal(StaysOpenOnClickProperty, value);
    }

    private static readonly DependencyPropertyKey RolePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Role),
            typeof(MenuItemRole),
            typeof(MenuItem),
            new PropertyMetadata(MenuItemRole.TopLevelItem, OnRoleChanged));

    /// <summary>
    /// Identifies the <see cref="Role"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RoleProperty = RolePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates the role of a <see cref="MenuItem"/>.
    /// </summary>
    /// <returns>
    /// One of the <see cref="MenuItemRole"/> values. The default is <see cref="MenuItemRole.TopLevelItem"/>.
    /// </returns>
    public MenuItemRole Role => (MenuItemRole)GetValue(RoleProperty);

    private static void OnRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var menuItem = (MenuItem)d;

        if (menuItem._submenuPopup is null)
        {
            return;
        }

        if (IsTopLevelRole((MenuItemRole)e.OldValue) != IsTopLevelRole((MenuItemRole)e.NewValue))
        {
            menuItem.ConfigurePopupForRole();
        }
    }

    private void UpdateRole()
    {
        MenuItemRole type;

        if (HasItems)
        {
            if (LogicalParent is Menu)
            {
                type = MenuItemRole.TopLevelHeader;
            }
            else
            {
                type = MenuItemRole.SubmenuHeader;
            }
        }
        else
        {
            if (LogicalParent is Menu)
            {
                type = MenuItemRole.TopLevelItem;
            }
            else
            {
                type = MenuItemRole.SubmenuItem;
            }
        }

        SetValueInternal(RolePropertyKey, type);
    }

    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(MenuItem),
            new PropertyMetadata(null, OnCommandChanged));

    /// <summary>
    /// Gets or sets the command associated with the menu item.
    /// </summary>
    /// <returns>
    /// The command associated with the <see cref="MenuItem"/>. The default is null.
    /// </returns>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValueInternal(CommandProperty, value);
    }

    private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ((MenuItem)o).OnCommandChanged((ICommand)e.NewValue);
    }

    /// <summary>
    /// Identifies the <see cref="CommandParameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(MenuItem),
            new PropertyMetadata(null, OnCommandParameterChanged));

    /// <summary>
    /// Gets or sets the parameter to pass to the <see cref="Command"/> property of a <see cref="MenuItem"/>.
    /// </summary>
    /// <returns>
    /// The parameter to pass to the <see cref="Command"/> property of a <see cref="MenuItem"/>. The default is null.
    /// </returns>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValueInternal(CommandParameterProperty, value);
    }

    private static void OnCommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ((MenuItem)o).UpdateCanExecute();
    }

    /// <summary>
    /// Identifies the <see cref="CommandTarget"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandTargetProperty =
        DependencyProperty.Register(
            nameof(CommandTarget),
            typeof(IInputElement),
            typeof(MenuItem),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the target element on which to raise the specified command.
    /// </summary>
    /// <returns>
    /// The element on which to raise the specified command. The default is null.
    /// </returns>
    public IInputElement CommandTarget
    {
        get => (IInputElement)GetValue(CommandTargetProperty);
        set => SetValueInternal(CommandTargetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(object),
            typeof(MenuItem),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the icon that appears in a <see cref="MenuItem"/>.
    /// </summary>
    /// <returns>
    /// The icon that appears in a <see cref="MenuItem"/>. The default value is null.
    /// </returns>
    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValueInternal(IconProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsSubmenuOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSubmenuOpenProperty =
        DependencyProperty.Register(
            nameof(IsSubmenuOpen),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsSubmenuOpenChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether the submenu of the <see cref="MenuItem"/> is open.
    /// </summary>
    /// <returns>
    /// true if the submenu of the <see cref="MenuItem"/> is open; otherwise, false. The default is false.
    /// </returns>
    public bool IsSubmenuOpen
    {
        get => (bool)GetValue(IsSubmenuOpenProperty);
        set => SetValueInternal(IsSubmenuOpenProperty, value);
    }

    private static void OnIsSubmenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var menuItem = (MenuItem)d;

        menuItem.StopOpenHierarchyTimer();

        // When opening, clear any current selection in the submenu
        if ((bool)e.NewValue)
        {
            // When menuitem's submenu opens, it should be selected.
            menuItem.SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.TrueBox);

            if (menuItem.Role == MenuItemRole.TopLevelHeader)
            {
                menuItem.SetMenuMode(true);
            }

            menuItem.CurrentSelection = null;

            menuItem.OnSubmenuOpened(new RoutedEventArgs(SubmenuOpenedEvent, menuItem));
        }
        else
        {
            if (menuItem.CurrentSelection is MenuItem selectedMenuItem)
            {
                // We're about to close the submenu -- if focus is within
                // the subtree, we need to take it back so that Focus isn't
                // left in an orphaned tree.
                if (selectedMenuItem._isFocused)
                {
                    menuItem.Focus();
                }

                if (selectedMenuItem.IsSubmenuOpen)
                {
                    selectedMenuItem.SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                }
            }

            menuItem.CurrentSelection = null;

            // No Popup in the style so fire closed now
            if (menuItem._submenuPopup is null)
            {
                menuItem.OnSubmenuClosed(new RoutedEventArgs(SubmenuClosedEvent, menuItem));
            }
        }

        menuItem.ChangeVisualState(true);
    }

    private static readonly DependencyPropertyKey IsHighlightedPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(IsHighlighted),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsHighlightedChanged));

    /// <summary>
    /// Identifies the <see cref="IsHighlighted"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether a <see cref="MenuItem"/> is highlighted.
    /// </summary>
    /// <returns>
    /// true if a <see cref="MenuItem"/> is highlighted; otherwise, false. The default is false.
    /// </returns>
    public bool IsHighlighted
    {
        get => (bool)GetValue(IsHighlightedProperty);
        protected set => SetValueInternal(IsHighlightedPropertyKey, value);
    }

    private static void OnIsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MenuItem)d).ChangeVisualState(true);
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> dependency property.
    /// </summary>
    internal static readonly DependencyProperty IsSelectedProperty =
        SelectorItem.IsSelectedProperty.AddOwner(
            typeof(MenuItem),
            new FrameworkPropertyMetadata(
                BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsSelectedChanged));

    internal bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValueInternal(IsSelectedProperty, value);
    }

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)d;
        // When IsSelected changes, IsHighlighted should reflect IsSelected
        // Note: it is okay for IsHighlighted and IsSelected to be different.
        //       Selection and highlight will separate when mousing around in
        //       a submenu when any timers are active.  Until you hover long
        //       enough and your selection is "committed", selection and highlight
        //       can disagree.
        menuItem.SetValueInternal(IsHighlightedPropertyKey, e.NewValue);

        // If IsSelected is changing to false, make sure to close
        // our submenu before doing anything.
        if ((bool)e.OldValue)
        {
            if (menuItem.IsSubmenuOpen)
            {
                menuItem.SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
            }

            // Also stop any timers immediately when we become deselected.
            menuItem.StopOpenHierarchyTimer();
        }

        menuItem.RaiseEvent(new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, MenuBase.IsSelectedChangedEvent));
    }

    /// <summary>
    ///     Called when IsSelected changed on this element or any descendant.
    /// </summary>
    private static void OnIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
    {
        // If IsSelected changed on a child of the MenuItem, change CurrentSelection
        // to the element that sent the event and handle the event.
        if (sender == e.OriginalSource)
        {
            return;
        }

        if (e.OriginalSource is not MenuItem source)
        {
            return;
        }

        MenuItem menuItem = (MenuItem)sender;

        if (e.NewValue)
        {
            // If the MenuItem is selected and it's a new item that's a child of ours,
            // change the CurrentSelection.
            if (menuItem.CurrentSelection != source && source.LogicalParent == menuItem)
            {
                if (menuItem.CurrentSelection != null && menuItem.CurrentSelection.IsSubmenuOpen)
                {
                    menuItem.CurrentSelection.SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                }

                menuItem.CurrentSelection = source;
            }
        }
        else
        {
            // If the item is no longer selected
            // If the MenuItem has been deselected and it's the CurrentSelection,
            // set our CurrentSelection to null.
            if (menuItem.CurrentSelection == source)
            {
                menuItem.CurrentSelection = null;
            }
        }

        // Mark the event as handled as long as it came from a MenuItem underneath us
        // even if we didn't necessarily do anything.
        e.Handled = true;
    }

    /// <summary>
    /// Gets a value that indicates whether or not the <see cref="MenuItem"/> is enabled.
    /// </summary>
    /// <returns>
    /// true if the <see cref="MenuItem"/> is enabled; otherwise, false.
    /// </returns>
    protected override bool IsEnabledCore => base.IsEnabledCore && CanExecute;

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        // Unhook from old popup
        if (_submenuPopup is not null)
        {
            _submenuPopup.Opened -= new EventHandler(OnSubmenuOpened);
            _submenuPopup.Closed -= new EventHandler(OnSubmenuClosed);
            _submenuPopup.OutsideClick -= new EventHandler<CancelEventArgs>(OnSubmenuOutsideClick);
        }

        DetachPopupRootHandlers();

        base.OnApplyTemplate();

        // Get the popup from template
        _submenuPopup = GetTemplateChild(SubMenuPopupPartName) as Popup;

        if (_submenuPopup is not null)
        {
            _submenuPopup.PlacementTarget = this;
            _submenuPopup.Opened += new EventHandler(OnSubmenuOpened);
            _submenuPopup.Closed += new EventHandler(OnSubmenuClosed);
            ConfigurePopupForRole();
        }

        ChangeVisualState(false);
    }

    /// <inheritdoc />
    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        _isFocused = true;
        if (!IsSelected)
        {
            SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.TrueBox);
        }

        ChangeVisualState(true);
        ItemsControlFromItemContainer(this)?.NotifyItemGotFocus(this);
    }

    /// <inheritdoc />
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        _isFocused = false;
        if (!IsSubmenuOpen && IsSelected)
        {
            SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.FalseBox);
        }

        ChangeVisualState(true);
        ItemsControlFromItemContainer(this)?.NotifyItemLostFocus(this);
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        MenuItemRole role = Role;

        if (role == MenuItemRole.TopLevelItem || role == MenuItemRole.TopLevelHeader)
        {
            if (IsInMenuMode)
            {
                // When mousing over a top-level hierarchy, it should open immediately.
                if (!IsSubmenuOpen)
                {
                    OpenHierarchy();
                }
            }
            else
            {
                if (IsMouseOver != IsSelected)
                {
                    SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.Box(IsMouseOver));
                }
            }
        }
        else
        {
            MenuItem sibling = CurrentSibling;

            if (sibling is not null && sibling.IsSubmenuOpen)
            {
                sibling.SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
            }

            if (!IsSubmenuOpen)
            {
                FocusOrSelect();
            }
            else
            {
                Debug.Assert(IsSelected, "When IsSubmenuOpen = true, IsSelected should be true as well");

                IsHighlighted = true;
            }

            if (!IsSelected || !IsSubmenuOpen)
            {
                StartOpenHierarchyTimer();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        StopOpenHierarchyTimer();

        MenuItemRole role = Role;

        if (role == MenuItemRole.TopLevelHeader || role == MenuItemRole.TopLevelItem)
        {
            if (!IsInMenuMode && IsMouseOver != IsSelected)
            {
                SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.Box(IsMouseOver));
            }
        }
        else
        {
            if (!IsSubmenuOpen)
            {
                if (IsSelected)
                {
                    SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.FalseBox);
                }
                else
                {
                    IsHighlighted = false;
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (!e.Handled)
        {
            e.Handled = true;
            HandleClick();
        }
        base.OnMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        if (!e.Handled)
        {
            e.Handled = true;

            if (InsideContextMenu)
            {
                HandleClick();
            }
        }
        base.OnMouseRightButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled)
        {
            return;
        }

        bool handled = false;

        Key key = e.Key;
        MenuItemRole role = Role;

        // In Right to Left mode we switch Right and Left keys
        if (FlowDirection == FlowDirection.RightToLeft)
        {
            if (key == Key.Right)
            {
                key = Key.Left;
            }
            else if (key == Key.Left)
            {
                key = Key.Right;
            }
        }

        switch (key)
        {
            case Key.Enter:
            case Key.Space:
                if (role == MenuItemRole.SubmenuItem || role == MenuItemRole.TopLevelItem)
                {
                    Debug.Assert(IsHighlighted, "MenuItem got Key.Enter but was not highlighted -- focus did not follow highlight?");
                    ClickItem();
                    handled = true;
                }
                else if (role == MenuItemRole.TopLevelHeader)
                {
                    // should this and the next one fire click events as well?
                    OpenSubmenuWithKeyboard();
                    handled = true;
                }
                else if (role == MenuItemRole.SubmenuHeader && !IsSubmenuOpen)
                {
                    OpenSubmenuWithKeyboard();
                    handled = true;
                }
                break;

            case Key.Right:
                if (role == MenuItemRole.SubmenuHeader && !IsSubmenuOpen)
                {
                    OpenSubmenuWithKeyboard();
                    handled = true;
                }
                break;

            case Key.Down:
                if ((role == MenuItemRole.TopLevelHeader || role == MenuItemRole.SubmenuHeader) && IsSubmenuOpen && CurrentSelection is null)
                {
                    FocusItemInternal(NavigateToStart());
                    handled = true;
                }
                break;

            case Key.Up:
                if ((role == MenuItemRole.TopLevelHeader || role == MenuItemRole.SubmenuHeader) && IsSubmenuOpen && CurrentSelection is null)
                {
                    FocusItemInternal(NavigateToEnd());
                    handled = true;
                }
                break;

            case Key.Left:
            case Key.Escape:
                if (role == MenuItemRole.SubmenuHeader || role == MenuItemRole.SubmenuItem)
                {
                    if (IsSubmenuOpen)
                    {
                        SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                        handled = true;
                    }
                }
                break;
        }

        if (!handled)
        {
            handled = MenuItemNavigate(e);
        }

        if (handled)
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Determines whether the specified item is, or is eligible to be, its own item container.
    /// </summary>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is MenuItem || item is Separator;
    }

    /// <summary>
    /// Creates or identifies the element used to display the specified item.
    /// </summary>
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new MenuItem();
    }

    /// <summary>
    /// Used to determine whether to apply a style to the item container.
    /// </summary>
    /// <param name="container">
    /// Container to which the style will be applied.
    /// </param>
    /// <param name="item">
    /// Item to which the container belongs.
    /// </param>
    /// <returns>
    /// true if the <see cref="MenuItem"/> is not a <see cref="Separator"/>; otherwise, false.
    /// </returns>
    protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
    {
        if (item is Separator)
        {
            return false;
        }

        return base.ShouldApplyItemContainerStyle(container, item);
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is MenuItem menuItem)
        {
            if (menuItem != item)
            {
                DataTemplate itemTemplate = ItemTemplate;
                Style itemContainerStyle = ItemContainerStyle;

                if (itemTemplate != null)
                {
                    menuItem.SetValueInternal(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                }
                if (itemContainerStyle != null && menuItem.ReadLocalValue(StyleProperty) == DependencyProperty.UnsetValue)
                {
                    menuItem.SetValueInternal(StyleProperty, itemContainerStyle);
                }

                if (menuItem.ReadLocalValue(HeaderProperty) == DependencyProperty.UnsetValue)
                {
                    menuItem.Header = item;
                }
            }
        }
    }

    /// <inheritdoc />
    protected internal override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);
        UpdateRole();
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        UpdateRole();
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        UpdateRole();
        base.OnItemsChanged(e);
        ChangeVisualState(true);
    }

    /// <summary>
    /// Called when a <see cref="MenuItem"/> is clicked and raises a <see cref="Click"/> event.
    /// </summary>
    protected virtual void OnClick()
    {
        if (!_isFocused)
        {
            FocusOrSelect();
        }

        if (RootMenuBase is MenuBase parentMenuBase)
        {
            parentMenuBase.OnMenuItemPreviewClick(this);
        }

        // Raise Click event
        RaiseEvent(new RoutedEventArgs(ClickEvent, this));

        // Execute command
        CommandHelpers.ExecuteCommandSource(this);
    }

    /// <summary>
    /// Called when the submenu of a <see cref="MenuItem"/> is opened.
    /// </summary>
    /// <param name="e">
    /// The event data for the <see cref="SubmenuOpened"/> event.
    /// </param>
    protected virtual void OnSubmenuOpened(RoutedEventArgs e) => RaiseEvent(e);

    /// <summary>
    /// Called when the submenu of a <see cref="MenuItem"/> is closed.
    /// </summary>
    /// <param name="e">
    /// The event data for the <see cref="SubmenuClosed"/> event.
    /// </param>
    protected virtual void OnSubmenuClosed(RoutedEventArgs e) => RaiseEvent(e);

    /// <summary>
    /// Changes to the correct visual state(s) for the control.
    /// </summary>
    /// <param name="useTransitions">True to use transitions; otherwise false.</param>
    protected virtual void ChangeVisualState(bool useTransitions)
    {
        if (!IsEnabled)
        {
            VisualStateManager.GoToState(this, VisualStates.StateDisabled, useTransitions);
        }
        else
        {
            VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
        }

        if ((_isFocused || IsHighlighted || IsSubmenuOpen) && IsEnabled)
        {
            VisualStateManager.GoToState(this, VisualStates.StateFocused, useTransitions);
        }
        else
        {
            VisualStateManager.GoToState(this, VisualStates.StateUnfocused, useTransitions);
        }
    }

    internal DependencyObject LogicalParent
    {
        get
        {
            if (Parent is not null)
            {
                return Parent;
            }

            return ItemsControlFromItemContainer(this);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is a top-level menu item (direct child of Menu).
    /// </summary>
    private bool IsTopLevel => IsTopLevelRole(Role);

    private bool InsideContextMenu => RootMenuBase is ContextMenu;

    private static bool IsTopLevelRole(MenuItemRole role) => role == MenuItemRole.TopLevelHeader || role == MenuItemRole.TopLevelItem;

    /// <summary>
    /// Gets the root MenuBase for this menu item.
    /// </summary>
    private MenuBase RootMenuBase
    {
        get
        {
            return LogicalParent switch
            {
                MenuBase parentMenuBase => parentMenuBase,
                MenuItem parentMenuItem => parentMenuItem.RootMenuBase,
                _ => null,
            };
        }
    }

    private MenuItem CurrentSelection
    {
        get => _currentSelection;
        set
        {
            if (_currentSelection != value)
            {
                _currentSelection?.SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.FalseBox);
                _currentSelection = value;
                _currentSelection?.SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.TrueBox);
            }
        }
    }

    private MenuItem CurrentSibling
    {
        get
        {
            MenuItem sibling = LogicalParent switch
            {
                MenuItem menuItemParent => menuItemParent.CurrentSelection,
                MenuBase menuParent => menuParent.CurrentSelection,
                _ => null
            };

            if (sibling == this)
            {
                sibling = null;
            }

            return sibling;
        }
    }

    private bool IsInMenuMode
    {
        get
        {
            if (LogicalParent is MenuBase parentMenu)
            {
                return parentMenu.IsMenuMode;
            }

            return false;
        }
    }

    internal void OpenSubmenuWithKeyboard()
    {
        if (OpenMenu())
        {
            int index = NavigateToStart();
            if (index >= 0 && index < Items.Count)
            {
                FocusItemInternal(index);
            }
        }
    }

    private bool OpenMenu()
    {
        if (!IsSubmenuOpen)
        {
            // Verify that the parent of the MenuItem is valid;
            if (ItemsControlFromItemContainer(this) is not ItemsControl owner)
            {
                owner = VisualTreeHelper.GetParent(this) as ItemsControl;
            }

            if (owner is not null && (owner is MenuItem || owner is MenuBase))
            {
                // Parent must be MenuItem or MenuBase in order for menus to open.
                // Otherwise, odd behavior will occur.
                SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.TrueBox);
                return true; // The value was actually changed
            }
        }

        return false;
    }

    private bool MenuItemNavigate(KeyEventArgs e)
    {
        if (ItemsControlFromItemContainer(this) is not ItemsControl parent)
        {
            return false;
        }

        Key key = e.Key;

        // In Right to Left mode we switch Right and Left keys
        if (parent.FlowDirection == FlowDirection.RightToLeft)
        {
            if (key == Key.Right)
            {
                key = Key.Left;
            }
            else if (key == Key.Left)
            {
                key = Key.Right;
            }
        }

        bool isVertical = parent.IsVerticalOrientation();

        if ((isVertical && (key == Key.Down || key == Key.Up)) || (!isVertical && (key == Key.Left || key == Key.Right)))
        {
            if (parent.HasItems)
            {
                MenuItem currentSelection = GetCurrentSelection(parent);
                int newFocusIndex = parent.NavigateByLine(currentSelection, key == Key.Down || key == Key.Right);
                MenuItem newSelection = newFocusIndex == -1 ? null : parent.ItemContainerGenerator.ContainerFromIndex(newFocusIndex) as MenuItem;

                if (newSelection is not null && currentSelection != newSelection)
                {
                    parent.FocusItemInternal(newFocusIndex);
                }

                return true;
            }
        }

        if (parent is MenuItem parentItem)
        {
            if (key == Key.Escape || (isVertical && key == Key.Left && !parentItem.IsTopLevel))
            {
                if (parentItem.IsSubmenuOpen)
                {
                    parent.SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
                    return true;
                }
            }

            return parentItem.MenuItemNavigate(e);
        }

        return false;

        static MenuItem GetCurrentSelection(ItemsControl itemsControl)
        {
            return itemsControl switch
            {
                MenuItem menuItem => menuItem.CurrentSelection,
                MenuBase menuBase => menuBase.CurrentSelection,
                _ => null,
            };
        }
    }

    private void ConfigurePopupForRole()
    {
        Debug.Assert(_submenuPopup is not null);

        if (IsTopLevel)
        {
            _submenuPopup.StayOpen = false;
            _submenuPopup.Placement = PlacementMode.Bottom;
            _submenuPopup.OutsideClick += new EventHandler<CancelEventArgs>(OnSubmenuOutsideClick);
        }
        else
        {
            _submenuPopup.StayOpen = true;
            _submenuPopup.Placement = PlacementMode.Right;
            _submenuPopup.OutsideClick -= new EventHandler<CancelEventArgs>(OnSubmenuOutsideClick);
        }
    }

    private void FocusOrSelect()
    {
        if (!_isFocused)
        {
            Focus();
        }

        if (!IsSelected)
        {
            // If it's already focused, make sure it's also selected.
            SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.TrueBox);
        }

        // If the item is selected we should ensure that it's highlighted.
        if (IsSelected && !IsHighlighted)
        {
            IsHighlighted = true;
        }
    }

    private void StartOpenHierarchyTimer()
    {
        if (_openHierarchyTimer is null)
        {
            _openHierarchyTimer = new DispatcherTimer();
            _openHierarchyTimer.Interval = TimeSpan.FromMilliseconds(400);
            _openHierarchyTimer.Tick += new EventHandler(OnOpenTimerTick);
        }

        _openHierarchyTimer.Start();
    }

    private void StopOpenHierarchyTimer() => _openHierarchyTimer?.Stop();

    private void OnOpenTimerTick(object sender, EventArgs e)
    {
        StopOpenHierarchyTimer();
        OpenHierarchy();
    }

    private void OpenHierarchy()
    {
        FocusOrSelect();

        MenuItemRole role = Role;
        if (role == MenuItemRole.TopLevelHeader || role == MenuItemRole.SubmenuHeader)
        {
            OpenMenu();
        }
    }

    private void OnSubmenuOpened(object sender, EventArgs e)
    {
        if (!IsSubmenuOpen)
        {
            SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.TrueBox);
        }

        if (IsTopLevel)
        {
            AttachPopupRootHandlers();
        }
    }

    private void OnSubmenuClosed(object sender, EventArgs e)
    {
        if (IsSubmenuOpen)
        {
            SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.FalseBox);
        }

        DetachPopupRootHandlers();

        OnSubmenuClosed(new RoutedEventArgs(SubmenuClosedEvent, this));
    }

    private void OnSubmenuOutsideClick(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        SetMenuMode(false);
    }

    private void AttachPopupRootHandlers()
    {
        DetachPopupRootHandlers();

        if (_submenuPopup is not null)
        {
            _submenuPopupRoot = _submenuPopup.PopupRoot;
            _submenuPopupRoot?.AddHandler(MouseMoveEvent, new MouseEventHandler(OnPopupRootMouseMove), true);
        }
    }

    private void DetachPopupRootHandlers()
    {
        _submenuPopupRoot?.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnPopupRootMouseMove));
        _submenuPopupRoot = null;
    }

    private void OnPopupRootMouseMove(object sender, MouseEventArgs e)
    {
        Debug.Assert(IsTopLevel);

        if (LogicalParent is not Menu root)
        {
            return;
        }

        bool allowHoverOpen = root.IsMenuMode;
        if (!allowHoverOpen)
        {
            return;
        }

        if (TryGetRootMenuItemAtPoint(e.GetPosition(null), out MenuItem menuItem) && menuItem.LogicalParent == root)
        {
            menuItem.OpenHierarchy();
        }
    }

    private static bool TryGetRootMenuItemAtPoint(Point point, out MenuItem menuItem)
    {
        menuItem = null;

        Window window = Window.Current ?? Application.Current?.MainWindow;
        if (window is null)
        {
            return false;
        }

        foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(point, window))
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (current is MenuItem mi)
                {
                    menuItem = mi;
                    return true;
                }

                current = VisualTreeHelper.GetParent(current);
            }
        }

        return false;
    }

    private void SetMenuMode(bool menuMode)
    {
        Debug.Assert(IsTopLevel, "MenuItem was not top-level");

        if (LogicalParent is MenuBase parentMenu)
        {
            parentMenu.IsMenuMode = menuMode;
        }
    }

    private void HandleClick()
    {
        MenuItemRole role = Role;

        if (role == MenuItemRole.TopLevelHeader || role == MenuItemRole.SubmenuHeader)
        {
            ClickHeader();
        }
        else
        {
            ClickItem();
        }
    }

    private void ClickHeader()
    {
        if (!_isFocused)
        {
            FocusOrSelect();
        }

        if (IsSubmenuOpen)
        {
            if (Role == MenuItemRole.TopLevelHeader)
            {
                SetMenuMode(false);
            }
        }
        else
        {
            SetCurrentValueInternal(IsSubmenuOpenProperty, BooleanBoxes.TrueBox);
        }
    }

    private void ClickItem()
    {
        try
        {
            OnClick();
        }
        finally
        {
            // When you click a top-level item, we need to exit menu mode.
            if (Role == MenuItemRole.TopLevelItem && !StaysOpenOnClick)
            {
                SetMenuMode(false);
            }
        }
    }

    private void OnCommandChanged(ICommand newCommand)
    {
        if (_canExecuteChangedListener is not null)
        {
            _canExecuteChangedListener.Detach();
            _canExecuteChangedListener = null;
        }

        if (newCommand is not null)
        {
            _canExecuteChangedListener = new CanExecuteChangedWeakEventListener(this, newCommand);
        }

        UpdateCanExecute();
    }

    private void OnCanExecuteChanged(object sender, EventArgs e) => UpdateCanExecute();

    private void UpdateCanExecute()
    {
        if (Command is not null)
        {
            CanExecute = CommandHelpers.CanExecuteCommandSource(this);
        }
        else
        {
            CanExecute = true;
        }
    }

    private bool CanExecute
    {
        get => _canExecute;
        set
        {
            if (_canExecute != value)
            {
                _canExecute = value;
                CoerceValue(IsEnabledProperty);
            }
        }
    }

    private sealed class CanExecuteChangedWeakEventListener
    {
        private readonly WeakEventToken _listener;
        private EventHandler _handler;

        public CanExecuteChangedWeakEventListener(MenuItem menuItem, ICommand command)
        {
            _listener = WeakEvent.Subscribe<MenuItem, ICommand, EventArgs>(
                menuItem,
                command,
                static (instance, sender, args) => instance.OnCanExecuteChanged(sender, args),
                (handler, source) => source.CanExecuteChanged -= _handler,
                (handler, source) =>
                {
                    _handler = new EventHandler(handler);
                    source.CanExecuteChanged += _handler;
                });
        }

        public void Detach() => _listener.Dispose();
    }
}