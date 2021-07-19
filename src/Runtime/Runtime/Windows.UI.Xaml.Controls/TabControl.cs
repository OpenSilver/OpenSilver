﻿

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that contains multiple items that share the same
    /// space on the screen.
    /// </summary>
    public partial class TabControl : ItemsControl //todo: would it not be better to inherit from Selector?
    {
        /// <summary>
        /// Initializes a new instance of the
        /// TabControl class.
        /// </summary>
        public TabControl()
        {
            SelectedIndex = -1;
#if MIGRATION
            KeyDown += delegate (object sender, KeyEventArgs e) { /*OnKeyDown(e);*/ };
#else
            KeyDown += delegate (object sender, KeyRoutedEventArgs e) { /*OnKeyDown(e);*/ };
#endif
            SelectionChanged += delegate (object sender, SelectionChangedEventArgs e) { OnSelectionChanged(e); };
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            DefaultStyleKey = typeof(TabControl);
        }

        /// <summary>
        /// Builds the visual tree for the TabControl when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // if previous items exist, we want to clear
            TabPanel panel = GetElementTabPanel();
            if (panel != null)
            {
                panel.Children.Clear();
            }

            // also clear the content if it is set to a ContentPresenter
            ContentPresenter contentHost = GetElementContent();
            if (contentHost != null)
            {
                contentHost.Content = null;
            }

            // Get the parts
            GetElements();

            foreach (object item in Items)
            {
                TabItem tabItem = item as TabItem;
                if (tabItem == null)
                {
                    throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", item.GetType().ToString(), typeof(TabItem)));
                }
                AddToTabPanel(tabItem);
            }
            if (this.SelectedIndex < 0)
            {
                this.SelectedIndex = 0;
            }
            else //(SelectedIndex >= 0)
            {
                UpdateSelectedContent(SelectedContent);
            }

            UpdateTabPanelLayout();
            ChangeVisualState(false);
        }

        public Dock TabStripPlacement
        {
            get { return (Dock)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register(
                "TabStripPlacement",
                typeof(Dock),
                typeof(TabControl),
                new PropertyMetadata(Dock.Top, OnTabStripPlacementPropertyChanged));

        private static void OnTabStripPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tc = (TabControl)d;
            if (tc == null)
            {
                throw new ArgumentException(string.Format("TabControl should not be null!"));
            }
            tc.UpdateTabPanelLayout();

            foreach (object o in tc.Items)
            {
                TabItem tabItem = o as TabItem;
                if (tabItem != null)
                {
                    tabItem.UpdateVisualState();
                }
            }
        }

        #region SelectedItem
        /// <summary>
        /// Gets or sets the currently selected TabItem.
        /// </summary>
        /// <value>
        /// The currently selected
        /// TabItem, or null if a
        /// TabItem is not selected.
        /// </value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                if (value == null || Items.Contains(value))
                {
                    SetValue(SelectedItemProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the TabControl.SelectedItem dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the TabControl.SelectedItem dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(TabControl),
                new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// SelectedItem property changed handler.
        /// </summary>
        /// <param name="d">TabControl that changed its SelectedItem.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tc = (TabControl)d;

            TabItem oldItem = e.OldValue as TabItem;
            TabItem tabItem = e.NewValue as TabItem;

            // if you select an item not in the TabControl keep the old
            // selection
            int index = (tabItem == null ? -1 : tc.Items.IndexOf(tabItem));
            if (tabItem != null && index == -1)
            {
                tc.SelectedItem = oldItem;
                return;
            }
            else
            {
                tc.SelectedIndex = index;
                tc.SelectItem(oldItem, tabItem);
            }
        }
#endregion SelectedItem

#region SelectedIndex
        /// <summary>
        /// Gets or sets the index of the currently selected TabItem.
        /// </summary>
        /// <value>
        /// The index of the currently selected TabItem, or -1 if a TabItem is not selected.
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the TabControl.SelectedIndex dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the TabControl.SelectedIndex dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(TabControl),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// SelectedIndex property changed handler.
        /// </summary>
        /// <param name="d">TabControl that changed its SelectedIndex.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tc = (TabControl)d;

            int newIndex = (int)e.NewValue;
            int oldIndex = (int)e.OldValue;

            if (newIndex < -1)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid value for property 'SelectedIndex'", newIndex.ToString()));
            }

            // Coercion Workaround
            // -------------------
            if (tc._updateIndex)
            {
                tc._desiredIndex = newIndex;
            }
            else if (!tc._updateIndex)
            {
                tc._updateIndex = true;
            }
            if (newIndex >= tc.Items.Count)
            {
                tc._updateIndex = false;
                tc.SelectedIndex = oldIndex;
                return;
            }
            // -------------------

            TabItem item = tc.GetItemAtIndex(newIndex);
            if (tc.SelectedItem != item)
            {
                tc.SelectedItem = item;
            }
        }

        /// <summary>
        /// Given the TabItem in the list of Items, we will set that item as the
        /// currently selected item, and un-select the rest of the items.
        /// </summary>
        /// <param name="oldItem">Previous TabItem that was unselected.</param>
        /// <param name="newItem">New TabItem to set as the SelectedItem.</param>
        private void SelectItem(TabItem oldItem, TabItem newItem)
        {
            if (newItem == null)
            {
                ContentPresenter contentHost = GetElementContent();
                if (contentHost != null)
                {
                    contentHost.Content = null;
                }
                SetValue(SelectedContentProperty, null);
            }

            foreach (object item in Items)
            {
                TabItem tabItem = item as TabItem;
                if (tabItem == null)
                {
                    throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", item.GetType().ToString(), typeof(TabItem)));
                }
                if (tabItem != newItem && tabItem.IsSelected)
                {
                    tabItem.IsSelected = false;
                }
                else if (tabItem == newItem)
                {
                    tabItem.IsSelected = true;
                    SetValue(SelectedContentProperty, tabItem.Content);
                }
            }

            // Fire SelectionChanged Event
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                     (oldItem == null ? new List<TabItem> { } : new List<TabItem> { oldItem }),
                     (newItem == null ? new List<TabItem> { } : new List<TabItem> { newItem }));
                handler(this, args);
            }
        }
#endregion SelectedIndex

#region SelectedContent
        /// <summary>
        /// Gets the content of the currently selected TabItem.
        /// </summary>
        /// <value>
        /// The content of the currently selected TabItem. The default is null.
        /// </value>
        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            internal set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// Identifies the TabControl.SelectedContent dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the TabControl.SelectedContent dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register(
                "SelectedContent",
                typeof(object),
                typeof(TabControl),
                new PropertyMetadata(null, OnSelectedContentChanged));

        /// <summary>
        /// SelectedContent property changed handler.
        /// </summary>
        /// <param name="d">TabControl that changed its SelectedContent.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tc = (TabControl)d;

            tc.UpdateSelectedContent(e.NewValue);
        }
#endregion SelectedContent

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">
        /// Control that triggers this property change.
        /// </param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// This will hide the templates for the TabStripPlacements that are not
        /// being displayed, and show the template for the TabStripPlacement
        /// that is currently selected.
        /// </summary>
        private void UpdateTabPanelLayout()
        {
            FrameworkElement newTemplate = GetElementTemplate();
            TabPanel newPanel = GetElementTabPanel();
            ContentPresenter newContentHost = GetElementContent();

            if (newPanel != null)
            {
                foreach (object item in Items)
                {
                    TabItem tabItem = item as TabItem;
                    if (tabItem == null)
                    {
                        throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", item.GetType().ToString(), typeof(TabItem)));
                    }
                    AddToTabPanel(tabItem);
                }
            }
            if (newContentHost != null)
            {
                newContentHost.Content = SelectedContent;
            }
            if (newTemplate != null)
            {
                newTemplate.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Update the current visual state of the TabControl.
        /// </summary>
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the TabControl.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }
        }

        /// <summary>
        /// Updates the current selection when Items has changed.
        /// </summary>
        /// <param name="e">Data used by the event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int newSelectedIndex = -1;
                    foreach (object o in e.NewItems)
                    {
                        TabItem tabItem = o as TabItem;
                        if (tabItem == null)
                        {
                            throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", o.GetType().ToString(), typeof(TabItem)));
                        }
                        int index = Items.IndexOf(tabItem);
                        InsertIntoTabPanel(index, tabItem);

                        // If we are adding a selected item
                        if (tabItem.IsSelected)
                        {
                            newSelectedIndex = index;
                        }
                        else if (SelectedItem != GetItemAtIndex(SelectedIndex))
                        {
                            newSelectedIndex = Items.IndexOf(SelectedItem);
                        }
                        else if ((_desiredIndex < Items.Count) && (_desiredIndex >= 0))
                        {
                            // Coercion Workaround
                            newSelectedIndex = _desiredIndex;
                        }

                        tabItem.UpdateVisualState();
                    }

                    if (newSelectedIndex == -1)
                    {
                        // If we are adding many items through xaml, one could
                        // already be specified as selected. If so, we don't
                        // want to override the value.
                        foreach (object item in Items)
                        {
                            TabItem tabItem = item as TabItem;
                            if (tabItem == null)
                            {
                                throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", item.GetType().ToString(), typeof(TabItem)));
                            }

                            if (tabItem.IsSelected)
                            {
                                return;
                            }
                        }

                        // To follow WPF behavior, we only select the item if
                        // the user has not explicitly set the IsSelected field
                        // to false, or if there are 2 or more items in the
                        // TabControl.
                        if (Items.Count > 1 || ((Items[0] as TabItem).ReadLocalValue(TabItem.IsSelectedProperty) as bool?) != false)
                        {
                            newSelectedIndex = 0;
                        }
                    }

                    // When we add a new item into the selected position, 
                    // SelectedIndex does not change, so we need to update both
                    // the SelectedItem and the SelectedIndex.
                    SelectedItem = GetItemAtIndex(newSelectedIndex);
                    SelectedIndex = newSelectedIndex;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object o in e.OldItems)
                    {
                        TabItem tabItem = o as TabItem;
                        RemoveFromTabPanel(tabItem);

                        // if there are no items, the selected index is set to
                        // -1
                        if (Items.Count == 0)
                        {
                            SelectedIndex = -1;
                        }
                        else if (Items.Count <= SelectedIndex)
                        {
                            SelectedIndex = Items.Count - 1;
                        }
                        else
                        {
                            SelectedItem = GetItemAtIndex(SelectedIndex);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ClearTabPanel();
                    SelectedIndex = -1;

                    // For Setting the ItemsSource
                    foreach (object item in Items)
                    {
                        TabItem tabItem = item as TabItem;
                        if (tabItem == null)
                        {
                            throw new ArgumentException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", item.GetType().ToString(), typeof(TabItem)));
                        }
                        AddToTabPanel(tabItem);
                        if (tabItem.IsSelected)
                        {
                            SelectedItem = tabItem;
                        }
                    }
                    if (SelectedIndex == -1 && Items.Count > 0)
                    {
                        SelectedIndex = 0;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;
            }
        }

        /// <summary>
        /// Occurs when the selected
        /// TabItem changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Raises the TabControl.SelectionChanged event.
        /// </summary>
        /// <param name="args">
        /// Provides data for the TabControl.SelectionChanged event.
        /// </param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
        {
        }

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e">Data used by the event.</param>
#if MIGRATION
        protected override void OnKeyDown(KeyEventArgs e)
#else
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }
            TabItem nextTabItem = null;

            int direction = 0;
            int startIndex = -1;
            switch (e.Key)
            {
#if MIGRATION
                case Key.Home:
#else
                case VirtualKey.Home:
#endif
                    direction = 1;
                    startIndex = -1;
                    break;
#if MIGRATION
                case Key.End:
#else
                case VirtualKey.End:
#endif
                    direction = -1;
                    startIndex = Items.Count;
                    break;
                default:
                    return;
            }

            nextTabItem = FindNextTabItem(startIndex, direction);

            if (nextTabItem != null && nextTabItem != SelectedItem)
            {
                e.Handled = true;
                SelectedItem = nextTabItem;
                nextTabItem.Focus();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="startIndex">Inherited code: Requires comment 1.</param>
        /// <param name="direction">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        internal TabItem FindNextTabItem(int startIndex, int direction)
        {
            //Debug.Assert(direction != 0, "The direction should not be zero!");
            TabItem nextTabItem = null;

            int index = startIndex;
            for (int i = 0; i < Items.Count; i++)
            {
                index += direction;
                if (index >= Items.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = Items.Count - 1;
                }

                TabItem tabItem = GetItemAtIndex(index);
                if (tabItem != null && tabItem.IsEnabled && tabItem.Visibility == Visibility.Visible)
                {
                    nextTabItem = tabItem;
                    break;
                }
            }
            return nextTabItem;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private TabItem GetItemAtIndex(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return null;
            }
            else
            {
                return Items[index] as TabItem;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="content">Inherited code: Requires comment 1.</param>
        private void UpdateSelectedContent(object content)
        {
            TabItem tabItem = SelectedItem as TabItem;
            if (tabItem != null)
            {
                ContentPresenter contentHost = GetElementContent();
                if (contentHost != null)
                {
                    contentHost.HorizontalAlignment = tabItem.HorizontalContentAlignment;
                    contentHost.VerticalAlignment = tabItem.VerticalContentAlignment;
                    contentHost.ContentTemplate = tabItem.ContentTemplate;
                    contentHost.Content = content;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="tabItem">Inherited code: Requires comment 1.</param>
        private void AddToTabPanel(TabItem tabItem)
        {
            TabPanel panel = GetElementTabPanel();
            if (panel != null && !panel.Children.Contains(tabItem))
            {
                panel.Children.Add(tabItem);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <param name="tabItem">Inherited code: Requires comment 2.</param>
        private void InsertIntoTabPanel(int index, TabItem tabItem)
        {
            TabPanel panel = GetElementTabPanel();
            if (panel != null && !panel.Children.Contains(tabItem))
            {
                panel.Children.Insert(index, tabItem);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="tabItem">Inherited code: Requires comment 1.</param>
        private void RemoveFromTabPanel(TabItem tabItem)
        {
            TabPanel panel = GetElementTabPanel();
            if (panel != null && panel.Children.Contains(tabItem))
            {
                panel.Children.Remove(tabItem);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void ClearTabPanel()
        {
            TabPanel panel = GetElementTabPanel();
            if (panel != null)
            {
                panel.Children.Clear();
            }
        }

        private TabPanel GetElementTabPanel()
        {
            switch (TabStripPlacement)
            {
                case Dock.Top:
                    return ElementTabPanelTop;
                case Dock.Bottom:
                    return ElementTabPanelBottom;
                case Dock.Left:
                    return ElementTabPanelLeft;
                case Dock.Right:
                    return ElementTabPanelRight;
                default:
                    return ElementTabPanelTop;
            }
        }

        private ContentPresenter GetElementContent()
        {
            switch (TabStripPlacement)
            {
                case Dock.Top:
                    return ElementContentTop;
                case Dock.Bottom:
                    return ElementContentBottom;
                case Dock.Left:
                    return ElementContentLeft;
                case Dock.Right:
                    return ElementContentRight;
                default:
                    return ElementContentTop;
            }
        }

        private FrameworkElement GetElementTemplate()
        {
            switch (TabStripPlacement)
            {
                case Dock.Top:
                    return ElementTemplateTop;
                case Dock.Bottom:
                    return ElementTemplateBottom;
                case Dock.Left:
                    return ElementTemplateLeft;
                case Dock.Right:
                    return ElementTemplateRight;
                default:
                    return ElementTemplateTop;
            }
        }

        private void GetElements()
        {
            ElementTemplateTop = GetTemplateChild(ElementTemplateTopName) as FrameworkElement;
            ElementTabPanelTop = GetTemplateChild(ElementTabPanelTopName) as TabPanel;
            ElementContentTop = GetTemplateChild(ElementContentTopName) as ContentPresenter;
            ElementTabPanelTop.Orientation = Orientation.Horizontal;

            ElementTemplateBottom = GetTemplateChild(ElementTemplateBottomName) as FrameworkElement;
            ElementTabPanelBottom = GetTemplateChild(ElementTabPanelBottomName) as TabPanel;
            ElementContentBottom = GetTemplateChild(ElementContentBottomName) as ContentPresenter;
            ElementTabPanelBottom.Orientation = Orientation.Horizontal;

            ElementTemplateLeft = GetTemplateChild(ElementTemplateLeftName) as FrameworkElement;
            ElementTabPanelLeft = GetTemplateChild(ElementTabPanelLeftName) as TabPanel;
            ElementContentLeft = GetTemplateChild(ElementContentLeftName) as ContentPresenter;
            ElementTabPanelLeft.Orientation = Orientation.Vertical;

            ElementTemplateRight = GetTemplateChild(ElementTemplateRightName) as FrameworkElement;
            ElementTabPanelRight = GetTemplateChild(ElementTabPanelRightName) as TabPanel;
            ElementContentRight = GetTemplateChild(ElementContentRightName) as ContentPresenter;
            ElementTabPanelRight.Orientation = Orientation.Vertical;
        }

        #region TopPlacement

        /// <summary>
        /// Gets or sets the TabStripPlacement Top template.
        /// </summary>
        internal FrameworkElement ElementTemplateTop { get; set; }

        /// <summary>
        /// The name of Top template.
        /// </summary>
        internal const string ElementTemplateTopName = "TemplateTop";

        /// <summary>
        /// Gets or sets the TabPanel of the TabStripPlacement Top template.
        /// </summary>
        internal TabPanel ElementTabPanelTop { get; set; }

        /// <summary>
        /// The name of TabPanel Top template.
        /// </summary>
        internal const string ElementTabPanelTopName = "TabPanelTop";

        /// <summary>
        /// Gets or sets the ContentHost of the TabStripPlacement Top template.
        /// </summary>
        internal ContentPresenter ElementContentTop { get; set; }

        /// <summary>
        /// The name of Top ContentHost.
        /// </summary>
        internal const string ElementContentTopName = "ContentTop";

        #endregion

        #region BottomPlacement

        /// <summary>
        /// Gets or sets the TabStripPlacement Bottom template.
        /// </summary>
        internal FrameworkElement ElementTemplateBottom { get; set; }

        /// <summary>
        /// The name of Bottom template.
        /// </summary>
        internal const string ElementTemplateBottomName = "TemplateBottom";

        /// <summary>
        /// Gets or sets the TabPanel of the TabStripPlacement Bottom template.
        /// </summary>
        internal TabPanel ElementTabPanelBottom { get; set; }

        /// <summary>
        /// The name of TabPanel Bottom template.
        /// </summary>
        internal const string ElementTabPanelBottomName = "TabPanelBottom";

        /// <summary>
        /// Gets or sets the ContentHost of the TabStripPlacement Bottom template.
        /// </summary>
        internal ContentPresenter ElementContentBottom { get; set; }

        /// <summary>
        /// The name of Bottom ContentHost.
        /// </summary>
        internal const string ElementContentBottomName = "ContentBottom";

        #endregion

        #region LeftPlacement

        /// <summary>
        /// Gets or sets the TabStripPlacement Left template.
        /// </summary>
        internal FrameworkElement ElementTemplateLeft { get; set; }

        /// <summary>
        /// The name of Left template.
        /// </summary>
        internal const string ElementTemplateLeftName = "TemplateLeft";

        /// <summary>
        /// Gets or sets the TabPanel of the TabStripPlacement Left template.
        /// </summary>
        internal TabPanel ElementTabPanelLeft { get; set; }

        /// <summary>
        /// The name of TabPanel Left template.
        /// </summary>
        internal const string ElementTabPanelLeftName = "TabPanelLeft";

        /// <summary>
        /// Gets or sets the ContentHost of the TabStripPlacement Left template.
        /// </summary>
        internal ContentPresenter ElementContentLeft { get; set; }

        /// <summary>
        /// The name of Left ContentHost.
        /// </summary>
        internal const string ElementContentLeftName = "ContentLeft";

        #endregion

        #region RightPlacement

        /// <summary>
        /// Gets or sets the TabStripPlacement Right template.
        /// </summary>
        internal FrameworkElement ElementTemplateRight { get; set; }

        /// <summary>
        /// The name of Right template.
        /// </summary>
        internal const string ElementTemplateRightName = "TemplateRight";

        /// <summary>
        /// Gets or sets the TabPanel of the TabStripPlacement Right template.
        /// </summary>
        internal TabPanel ElementTabPanelRight { get; set; }

        /// <summary>
        /// The name of TabPanel Right template.
        /// </summary>
        internal const string ElementTabPanelRightName = "TabPanelRight";

        /// <summary>
        /// Gets or sets the ContentHost of the TabStripPlacement Right template.
        /// </summary>
        internal ContentPresenter ElementContentRight { get; set; }

        /// <summary>
        /// The name of Right ContentHost.
        /// </summary>
        internal const string ElementContentRightName = "ContentRight";

        #endregion

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private int _desiredIndex;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _updateIndex = true;
    }
}
