// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a selectable item inside a Menu or ContextMenu.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Highlighted", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplatePart(Name = SubMenuPopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = SubMenuArrowPartName, Type = typeof(UIElement))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MenuItem))]
    public class MenuItem : HeaderedItemsControl // , ICommandSource // ICommandSource not defined by Silverlight 4
    {
        private const string SubMenuPopupPartName = "PART_Popup";
        private const string SubMenuArrowPartName = "PART_Arrow";

        /// <summary>
        /// Occurs when a MenuItem is clicked.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Stores a value indicating whether this element has logical focus.
        /// </summary>
        private bool _isFocused;

        /// <summary>
        /// Reference to the submenu popup.
        /// </summary>
        private Popup _submenuPopup;

        /// <summary>
        /// Reference to the submenu arrow indicator.
        /// </summary>
        private UIElement _submenuArrow;

        /// <summary>
        /// Reference to the popup root used for global input handling.
        /// </summary>
        private PopupRoot _submenuPopupRoot;

        /// <summary>
        /// Gets or sets a reference to the MenuBase parent (for top-level items).
        /// </summary>
        internal MenuBase ParentMenuBase { get; set; }

        /// <summary>
        /// Gets or sets a reference to the parent MenuItem (for nested items).
        /// </summary>
        internal MenuItem ParentMenuItem { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a top-level menu item (direct child of Menu).
        /// </summary>
        private bool IsTopLevel => ParentMenuBase is Menu;

        /// <summary>
        /// Gets the root MenuBase for this menu item.
        /// </summary>
        private MenuBase RootMenuBase
        {
            get
            {
                MenuBase root = ParentMenuBase;
                MenuItem current = this;
                while (current != null)
                {
                    if (current.ParentMenuBase != null)
                    {
                        root = current.ParentMenuBase;
                    }
                    current = current.ParentMenuItem;
                }
                return root;
            }
        }

        /// <summary>
        /// Gets or sets the command associated with the menu item.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(MenuItem),
            new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Handles changes to the Command DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)o).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            if (null != oldValue)
            {
                oldValue.CanExecuteChanged -= HandleCanExecuteChanged;
            }
            if (null != newValue)
            {
                newValue.CanExecuteChanged += HandleCanExecuteChanged;
            }
            UpdateIsEnabled();
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the Command property of a MenuItem.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(MenuItem),
            new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Handles changes to the CommandParameter DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)o).UpdateIsEnabled();
        }

        /// <summary>
        /// Gets or sets the icon that appears in a MenuItem.
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Identifies the Icon dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(MenuItem),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsSubmenuOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSubmenuOpenProperty = DependencyProperty.Register(
            nameof(IsSubmenuOpen),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsSubmenuOpenChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the submenu of the <see cref="MenuItem"/> is open.
        /// </summary>
        public bool IsSubmenuOpen
        {
            get => (bool)GetValue(IsSubmenuOpenProperty);
            set => SetValueInternal(IsSubmenuOpenProperty, value);
        }

        private static void OnIsSubmenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menuItem = (MenuItem)d;
            bool isOpen = (bool)e.NewValue;

            if (menuItem._submenuPopup != null)
            {
                menuItem._submenuPopup.IsOpen = isOpen;
            }

            // When opening, clear any current selection in the submenu
            if (isOpen)
            {
                menuItem.CurrentSelection = null;
            }

            menuItem.ChangeVisualState(true);
        }

        /// <summary>
        /// Identifies the <see cref="IsHighlighted"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            nameof(IsHighlighted),
            typeof(bool),
            typeof(MenuItem),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsHighlightedChanged));

        /// <summary>
        /// Gets or sets whether this menu item is highlighted.
        /// </summary>
        internal bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValueInternal(IsHighlightedProperty, value);
        }

        private static void OnIsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)d).ChangeVisualState(true);
        }

        /// <summary>
        /// Currently selected child item in this submenu.
        /// </summary>
        private MenuItem _currentSelection;

        /// <summary>
        /// Gets or sets the currently selected child MenuItem.
        /// </summary>
        internal MenuItem CurrentSelection
        {
            get => _currentSelection;
            set
            {
                if (_currentSelection != value)
                {
                    if (_currentSelection != null)
                    {
                        _currentSelection.IsHighlighted = false;
                    }
                    _currentSelection = value;
                    if (_currentSelection != null)
                    {
                        _currentSelection.IsHighlighted = true;
                    }
                }
            }
        }

        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new PropertyMetadata(typeof(MenuItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        public MenuItem()
        {
            UpdateIsEnabled();
        }

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook from old popup
            if (_submenuPopup != null)
            {
                _submenuPopup.Opened -= OnSubmenuOpened;
                _submenuPopup.Closed -= OnSubmenuClosed;
                _submenuPopup.OutsideClick -= OnSubmenuOutsideClick;
            }
            DetachPopupRootHandlers();

            base.OnApplyTemplate();

            // Get the popup from template
            _submenuPopup = GetTemplateChild(SubMenuPopupPartName) as Popup;

            if (_submenuPopup != null)
            {
                _submenuPopup.PlacementTarget = this;
                // Top-level items open below, submenu items open to the right
                _submenuPopup.Placement = IsTopLevel ? PlacementMode.Bottom : PlacementMode.Right;
                // StayOpen = false so popup root can capture input for outside-click detection.
                _submenuPopup.StayOpen = false;
                _submenuPopup.Opened += OnSubmenuOpened;
                _submenuPopup.Closed += OnSubmenuClosed;
                _submenuPopup.OutsideClick += OnSubmenuOutsideClick;
            }

            // Get the arrow from template and update visibility
            _submenuArrow = GetTemplateChild(SubMenuArrowPartName) as UIElement;
            UpdateSubmenuArrowVisibility();

            ChangeVisualState(false);
        }

        /// <summary>
        /// Updates the visibility of the submenu arrow based on whether this item has children.
        /// </summary>
        private void UpdateSubmenuArrowVisibility()
        {
            if (_submenuArrow != null)
            {
                _submenuArrow.Visibility = HasItems ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnSubmenuOpened(object sender, EventArgs e)
        {
            if (!IsSubmenuOpen)
            {
                IsSubmenuOpen = true;
            }

            AttachPopupRootHandlers();
        }

        private void OnSubmenuClosed(object sender, EventArgs e)
        {
            if (IsSubmenuOpen)
            {
                IsSubmenuOpen = false;
            }

            DetachPopupRootHandlers();
        }

        private void AttachPopupRootHandlers()
        {
            if (_submenuPopup is null)
            {
                return;
            }

            PopupRoot root = _submenuPopup.PopupRoot;
            if (root is null || root == _submenuPopupRoot)
            {
                return;
            }

            DetachPopupRootHandlers();
            _submenuPopupRoot = root;
            _submenuPopupRoot.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPopupRootPreviewMouseDown), true);
            _submenuPopupRoot.AddHandler(UIElement.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(OnPopupRootPreviewMouseDown), true);
            _submenuPopupRoot.MouseMove += OnPopupRootMouseMove;
        }

        private void DetachPopupRootHandlers()
        {
            if (_submenuPopupRoot is null)
            {
                return;
            }

            _submenuPopupRoot.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPopupRootPreviewMouseDown));
            _submenuPopupRoot.RemoveHandler(UIElement.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(OnPopupRootPreviewMouseDown));
            _submenuPopupRoot.MouseMove -= OnPopupRootMouseMove;
            _submenuPopupRoot = null;
        }

        private void OnSubmenuOutsideClick(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // When clicking outside a submenu, close the entire menu hierarchy.
            // If the last click was inside the menu hierarchy, suppress outside-click closing.
            MenuBase root = RootMenuBase;
            if (root != null && root.SuppressOutsideClickClose)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = true; // Prevent default popup close behavior
            CloseAllMenus();

            if (root != null)
            {
                root.SuppressOutsideClickClose = false;
            }
        }

        private void OnPopupRootPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MenuBase root = RootMenuBase;
            if (root is null)
            {
                return;
            }

            Point point = e.GetPosition(null);
            if (TryGetMenuItemAtPoint(point, out MenuItem target, out MenuBase hitRoot) && hitRoot == root)
            {
                root.SuppressOutsideClickClose = true;

                // If menu is already open, allow switching top-level menus on click
                if (root is Menu menu && menu.IsMenuMode && target != null && target.IsTopLevel)
                {
                    OpenMenuItemOnHover(target);
                }
            }
            else
            {
                root.SuppressOutsideClickClose = false;
            }
        }

        private void OnPopupRootMouseMove(object sender, MouseEventArgs e)
        {
            MenuBase root = RootMenuBase;
            if (root is null)
            {
                return;
            }

            bool allowHoverOpen = root.IsMenuMode || root is ContextMenu;
            if (!allowHoverOpen)
            {
                return;
            }

            Point point = e.GetPosition(null);
            if (TryGetMenuItemAtPoint(point, out MenuItem target, out MenuBase hitRoot) && hitRoot == root)
            {
                OpenMenuItemOnHover(target);
            }
        }

        private static bool TryGetMenuItemAtPoint(Point point, out MenuItem menuItem, out MenuBase root)
        {
            menuItem = null;
            root = null;

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
                        root = mi.RootMenuBase;
                        return true;
                    }

                    if (root is null && current is MenuBase mb)
                    {
                        root = mb;
                    }

                    current = VisualTreeHelper.GetParent(current);
                }
            }

            return false;
        }

        private void OpenMenuItemOnHover(MenuItem target)
        {
            if (target is null || !target.IsEnabled)
            {
                return;
            }

            if (target.ParentMenuItem != null)
            {
                // Only open if parent submenu is already open
                if (!target.ParentMenuItem.IsSubmenuOpen)
                {
                    return;
                }

                target.CloseSiblingSubmenus();
                target.ParentMenuItem.CurrentSelection = target;

                if (target.HasItems)
                {
                    target.IsSubmenuOpen = true;
                }
                return;
            }

            if (target.ParentMenuBase != null)
            {
                // Top-level in Menu or ContextMenu
                if (target.ParentMenuBase is Menu menu && !menu.IsMenuMode)
                {
                    return;
                }

                target.CloseSiblingSubmenus();
                target.ParentMenuBase.CurrentSelection = target;

                if (target.HasItems)
                {
                    target.IsSubmenuOpen = true;
                    if (target.ParentMenuBase is Menu menuBase && !menuBase.IsMenuMode)
                    {
                        menuBase.EnterMenuMode();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled GotFocus event reaches this element in its route.
        /// </summary>
        /// <param name="e">A RoutedEventArgs that contains event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Raises the LostFocus routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A RoutedEventArgs that contains event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Called whenever the mouse enters a MenuItem.
        /// </summary>
        /// <param name="e">The event data for the MouseEnter event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (IsTopLevel)
            {
                // Top-level item
                MenuBase menu = ParentMenuBase;
                if (menu != null)
                {
                    // Check if we should open on hover:
                    // 1. Menu is in menu mode (OpenOnMouseEnter or IsMenuMode)
                    // 2. OR any sibling has its submenu open (user moved from one menu to another)
                    bool shouldOpenOnHover = menu.OpenOnMouseEnter || menu.IsMenuMode || HasSiblingSubmenuOpen();

                    if (shouldOpenOnHover)
                    {
                        // Menu mode: hovering opens this item and closes siblings
                        CloseSiblingSubmenus();
                        menu.CurrentSelection = this;

                        if (HasItems)
                        {
                            IsSubmenuOpen = true;
                            // Ensure menu mode is active
                            if (!menu.IsMenuMode)
                            {
                                menu.EnterMenuMode();
                            }
                        }
                    }
                    else
                    {
                        // Not in menu mode: just highlight
                        menu.CurrentSelection = this;
                    }
                }
            }
            else
            {
                // Submenu item: highlight and open if has children
                MenuItem parentItem = ParentMenuItem;
                if (parentItem != null)
                {
                    // Close siblings
                    CloseSiblingSubmenus();
                    parentItem.CurrentSelection = this;

                    // Open submenu on hover for nested items
                    if (HasItems)
                    {
                        IsSubmenuOpen = true;
                    }
                }
                else if (ParentMenuBase != null)
                {
                    // Direct child of MenuBase (ContextMenu)
                    CloseSiblingSubmenus();
                    ParentMenuBase.CurrentSelection = this;

                    if (HasItems)
                    {
                        IsSubmenuOpen = true;
                    }
                }
            }

            ChangeVisualState(true);
        }

        /// <summary>
        /// Checks if any sibling menu item has its submenu open.
        /// </summary>
        private bool HasSiblingSubmenuOpen()
        {
            ItemsControl parent = ParentMenuItem as ItemsControl ?? ParentMenuBase as ItemsControl;
            if (parent != null)
            {
                foreach (object item in parent.Items)
                {
                    MenuItem sibling = parent.ItemContainerGenerator.ContainerFromItem(item) as MenuItem;
                    if (sibling == null)
                    {
                        sibling = item as MenuItem;
                    }
                    if (sibling != null && sibling != this && sibling.IsSubmenuOpen)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Closes submenus of sibling menu items.
        /// </summary>
        private void CloseSiblingSubmenus()
        {
            // Get the parent that contains this item and its siblings
            ItemsControl parent = ParentMenuItem as ItemsControl ?? ParentMenuBase as ItemsControl;
            if (parent != null)
            {
                foreach (object item in parent.Items)
                {
                    MenuItem sibling = parent.ItemContainerGenerator.ContainerFromItem(item) as MenuItem;
                    if (sibling == null)
                    {
                        sibling = item as MenuItem;
                    }
                    if (sibling != null && sibling != this && sibling.IsSubmenuOpen)
                    {
                        sibling.IsSubmenuOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever the mouse leaves a MenuItem.
        /// </summary>
        /// <param name="e">The event data for the MouseLeave event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // Don't deselect if mouse is moving to our submenu
            if (IsSubmenuOpen && _submenuPopup != null)
            {
                // Keep highlighted while submenu is open
                return;
            }

            // If not in menu mode and this is a top-level item without open submenu, deselect
            if (IsTopLevel && ParentMenuBase != null && !ParentMenuBase.IsMenuMode && !IsSubmenuOpen)
            {
                if (ParentMenuBase.CurrentSelection == this)
                {
                    ParentMenuBase.CurrentSelection = null;
                }
            }

            ChangeVisualState(true);
        }

        /// <summary>
        /// Called when the left mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                HandleClick();
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called when the right mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseRightButtonDown event.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled && !HasItems)
            {
                e.Handled = true;
                HandleClick();
            }
            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Handles click/activation of this menu item.
        /// </summary>
        private void HandleClick()
        {
            if (HasItems)
            {
                // Has children: toggle submenu
                if (IsTopLevel)
                {
                    MenuBase menu = ParentMenuBase;
                    if (menu != null)
                    {
                        if (!IsSubmenuOpen)
                        {
                            // Open and enter menu mode
                            IsSubmenuOpen = true;
                            menu.EnterMenuMode();
                        }
                        else
                        {
                            // Close and exit menu mode
                            IsSubmenuOpen = false;
                            menu.ExitMenuMode();
                        }
                    }
                }
                else
                {
                    // Nested submenu header
                    IsSubmenuOpen = !IsSubmenuOpen;
                }
            }
            else
            {
                // Leaf item: execute click and close menus
                OnClick();
            }
        }

        /// <summary>
        /// Responds to the KeyDown event.
        /// </summary>
        /// <param name="e">The event data for the KeyDown event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Space:
                        HandleClick();
                        e.Handled = true;
                        break;

                    case Key.Right:
                        if (HasItems && !IsSubmenuOpen)
                        {
                            IsSubmenuOpen = true;
                            e.Handled = true;
                        }
                        break;

                    case Key.Left:
                        if (IsSubmenuOpen)
                        {
                            IsSubmenuOpen = false;
                            e.Handled = true;
                        }
                        break;

                    case Key.Escape:
                        if (IsSubmenuOpen)
                        {
                            IsSubmenuOpen = false;
                            e.Handled = true;
                        }
                        else if (IsTopLevel && ParentMenuBase != null)
                        {
                            ParentMenuBase.ExitMenuMode();
                            e.Handled = true;
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
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
        /// Prepares the specified element to display the specified item.
        /// </summary>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is MenuItem menuItem)
            {
                // Set parent reference for nested items
                menuItem.ParentMenuItem = this;

                if (menuItem != item)
                {
                    DataTemplate itemTemplate = ItemTemplate;
                    Style itemContainerStyle = ItemContainerStyle;

                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null && menuItem.ReadLocalValue(StyleProperty) == DependencyProperty.UnsetValue)
                    {
                        menuItem.SetValue(StyleProperty, itemContainerStyle);
                    }

                    if (menuItem.ReadLocalValue(HeaderProperty) == DependencyProperty.UnsetValue)
                    {
                        menuItem.Header = item;
                    }
                }
            }
        }

        /// <summary>
        /// Called when the Items property changes.
        /// </summary>
        /// <param name="e">The event data for the ItemsChanged event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateSubmenuArrowVisibility();
            ChangeVisualState(true);
        }

        /// <summary>
        /// Called when a MenuItem is clicked and raises a Click event.
        /// </summary>
        protected virtual void OnClick()
        {
            // Close all parent menus
            CloseAllMenus();

            // Raise Click event
            Click?.Invoke(this, new RoutedEventArgs());

            // Execute command
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        /// <summary>
        /// Closes all menus up to the root.
        /// </summary>
        private void CloseAllMenus()
        {
            // Find root menu and exit menu mode
            MenuBase root = RootMenuBase;
            if (root != null)
            {
                root.ChildMenuItemClicked();
            }
        }

        private void HandleCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateIsEnabled();
        }

        /// <summary>
        /// Updates the IsEnabled property.
        /// </summary>
        /// <remarks>
        /// WPF overrides the local value of IsEnabled according to ICommand, so Silverlight does, too.
        /// </remarks>
        private void UpdateIsEnabled()
        {
            IsEnabled = (null == Command) || Command.CanExecute(CommandParameter);
            ChangeVisualState(true);
        }

        /// <summary>
        /// Changes to the correct visual state(s) for the control.
        /// </summary>
        /// <param name="useTransitions">True to use transitions; otherwise false.</param>
        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            else if (IsHighlighted || IsSubmenuOpen)
            {
                VisualStateManager.GoToState(this, "Highlighted", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }

            if (_isFocused && IsEnabled)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }
        }
    }
}