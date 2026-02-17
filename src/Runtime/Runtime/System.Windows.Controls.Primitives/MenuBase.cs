// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents a control that defines choices for users to select.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MenuItem))]
    public abstract class MenuBase : ItemsControl
    {
        /// <summary>
        /// Gets or sets the Style that is applied to the container element generated for each item.
        /// </summary>
        public new Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public new static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            "ItemContainerStyle",
            typeof(Style),
            typeof(MenuBase),
            null);

        /// <summary>
        /// Currently selected item in this menu or submenu.
        /// </summary>
        private MenuItem _currentSelection;

        /// <summary>
        /// Gets or sets the currently selected MenuItem.
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

        /// <summary>
        /// Gets or sets whether the menu is in menu mode (submenus open on hover).
        /// </summary>
        internal bool IsMenuMode { get; private set; }

        /// <summary>
        /// Gets or sets whether top-level items should open on mouse enter.
        /// </summary>
        internal bool OpenOnMouseEnter { get; set; }

        /// <summary>
        /// Tracks whether the last pointer down was inside this menu hierarchy.
        /// Used to suppress outside-click closing when interacting with the menu.
        /// </summary>
        internal bool SuppressOutsideClickClose { get; set; }

        /// <summary>
        /// Initializes a new instance of the MenuBase class.
        /// </summary>
        public MenuBase()
        {
        }

        /// <summary>
        /// Enters menu mode - submenus will open on hover.
        /// </summary>
        internal void EnterMenuMode()
        {
            if (!IsMenuMode)
            {
                IsMenuMode = true;
                OpenOnMouseEnter = true;
            }
        }

        /// <summary>
        /// Exits menu mode and closes all submenus.
        /// </summary>
        internal void ExitMenuMode()
        {
            if (IsMenuMode)
            {
                IsMenuMode = false;
                OpenOnMouseEnter = false;

                // Close current selection's submenu
                if (CurrentSelection != null)
                {
                    CurrentSelection.IsSubmenuOpen = false;
                    CurrentSelection = null;
                }
            }
        }

        /// <summary>
        /// Called when a child menu item is clicked (non-submenu item).
        /// </summary>
        internal virtual void ChildMenuItemClicked()
        {
            ExitMenuMode();
        }

        /// <summary>
        /// Determines whether the specified item is, or is eligible to be, its own item container.
        /// </summary>
        /// <param name="item">The item to check whether it is an item container.</param>
        /// <returns>True if the item is a MenuItem or a Separator; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is MenuItem) || (item is Separator));
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
            MenuItem menuItem = element as MenuItem;
            if (null != menuItem)
            {
                menuItem.ParentMenuBase = this;
                if (menuItem != item)
                {
                    // Copy the ItemsControl properties from parent to child
                    DataTemplate itemTemplate = ItemTemplate;
                    Style itemContainerStyle = ItemContainerStyle;
                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null && HasDefaultValue(menuItem, HeaderedItemsControl.ItemContainerStyleProperty))
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);
                    }

                    // Copy the Header properties from parent to child
                    if (HasDefaultValue(menuItem, HeaderedItemsControl.HeaderProperty))
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
        /// Checks whether a control has the default value for a property.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property has the default value; false otherwise.</returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }
    }
}