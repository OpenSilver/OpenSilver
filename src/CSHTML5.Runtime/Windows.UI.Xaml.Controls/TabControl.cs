
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
            SelectionChanged += delegate (object sender, SelectionChangedEventArgs e) { OnSelectionChanged(e); };
            //IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            //DefaultStyleKey = typeof(TabControl);

            // Set default style:
            this.DefaultStyleKey = typeof(TabControl);
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
            if (ElementTabPanelTop != null)
            {
                ElementTabPanelTop.Children.Clear();
            }

            // also clear the content if it is set to a ContentPresenter
            ContentPresenter contentHost = ElementContentTop;
            if (contentHost != null)
            {
                contentHost.Content = null;
            }

            // Get the parts
            ElementTemplateTop = GetTemplateChild(ElementTemplateTopName) as FrameworkElement;
            ElementTabPanelTop = GetTemplateChild(ElementTabPanelTopName) as TabPanel;
            ElementContentTop = GetTemplateChild(ElementContentTopName) as ContentPresenter;

            foreach (object item in Items)
            {
                TabItem tabItem = item as TabItem;
                if (tabItem == null)
                {
                    throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, item.GetType().ToString())");
                }
                AddToTabPanel(tabItem);
            }
            if (SelectedIndex >= 0)
            {
                UpdateSelectedContent(SelectedContent);
            }

            UpdateTabPanelLayout();
            ChangeVisualState(false);
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
                throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidIndex, newIndex.ToString(CultureInfo.CurrentCulture))");
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
                ContentPresenter contentHost = ElementContentTop;
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
                    throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, item.GetType().ToString())");
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
                    (oldItem == null ? new List<object> { } : new List<object> { oldItem }),
                    (newItem == null ? new List<object> { } : new List<object> { newItem }));
                //(oldItem == null ? new List<TabItem> { } : new List<TabItem> { oldItem }),
                //(newItem == null ? new List<TabItem> { } : new List<TabItem> { newItem }));
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
            FrameworkElement newTemplate = ElementTemplateTop;
            TabPanel newPanel = ElementTabPanelTop;
            ContentPresenter newContentHost = ElementContentTop;

            if (newPanel != null)
            {
                foreach (object item in Items)
                {
                    TabItem tabItem = item as TabItem;
                    if (tabItem == null)
                    {
                        throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, item.GetType().ToString())"); //todo: what is this?
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

        ///// <summary>
        ///// Updates the current selection when Items has changed.
        ///// </summary>
        ///// <param name="e">Data used by the event.</param>
        //protected void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            int newSelectedIndex = -1;
        //            foreach (object o in e.NewItems)
        //            {
        //                TabItem tabItem = o as TabItem;
        //                if (tabItem == null)
        //                {
        //                    throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, o.GetType().ToString())");
        //                }
        //                int index = Items.IndexOf(tabItem);
        //                InsertIntoTabPanel(index, tabItem);

        //                // If we are adding a selected item
        //                if (tabItem.IsSelected)
        //                {
        //                    newSelectedIndex = index;
        //                }
        //                else if (SelectedItem != GetItemAtIndex(SelectedIndex))
        //                {
        //                    newSelectedIndex = Items.IndexOf(SelectedItem);
        //                }
        //                else if ((_desiredIndex < Items.Count) && (_desiredIndex >= 0))
        //                {
        //                    // Coercion Workaround
        //                    newSelectedIndex = _desiredIndex;
        //                }

        //                tabItem.UpdateVisualState();
        //            }

        //            if (newSelectedIndex == -1)
        //            {
        //                // If we are adding many items through xaml, one could
        //                // already be specified as selected. If so, we don't
        //                // want to override the value.
        //                foreach (object item in Items)
        //                {
        //                    TabItem tabItem = item as TabItem;
        //                    if (tabItem == null)
        //                    {
        //                        throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, item.GetType().ToString())");
        //                    }

        //                    if (tabItem.IsSelected)
        //                    {
        //                        return;
        //                    }
        //                }

        //                // To follow WPF behavior, we only select the item if
        //                // the user has not explicitly set the IsSelected field
        //                // to false, or if there are 2 or more items in the
        //                // TabControl.
        //                if (Items.Count > 1 || ((Items[0] as TabItem).ReadLocalValue(TabItem.IsSelectedProperty) as bool?) != false)
        //                {
        //                    newSelectedIndex = 0;
        //                }
        //            }

        //            // When we add a new item into the selected position, 
        //            // SelectedIndex does not change, so we need to update both
        //            // the SelectedItem and the SelectedIndex.
        //            SelectedItem = GetItemAtIndex(newSelectedIndex);
        //            SelectedIndex = newSelectedIndex;
        //            break;

        //        case NotifyCollectionChangedAction.Remove:
        //            foreach (object o in e.OldItems)
        //            {
        //                TabItem tabItem = o as TabItem;
        //                RemoveFromTabPanel(tabItem);

        //                // if there are no items, the selected index is set to
        //                // -1
        //                if (Items.Count == 0)
        //                {
        //                    SelectedIndex = -1;
        //                }
        //                else if (Items.Count <= SelectedIndex)
        //                {
        //                    SelectedIndex = Items.Count - 1;
        //                }
        //                else
        //                {
        //                    SelectedItem = GetItemAtIndex(SelectedIndex);
        //                }
        //            }
        //            break;

        //        case NotifyCollectionChangedAction.Reset:
        //            ClearTabPanel();
        //            SelectedIndex = -1;

        //            // For Setting the ItemsSource
        //            foreach (object item in Items)
        //            {
        //                TabItem tabItem = item as TabItem;
        //                if (tabItem == null)
        //                {
        //                    throw new ArgumentException("string.Format(CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.TabControl_InvalidChild, item.GetType().ToString())");
        //                }
        //                AddToTabPanel(tabItem);
        //                if (tabItem.IsSelected)
        //                {
        //                    SelectedItem = tabItem;
        //                }
        //            }
        //            if (SelectedIndex == -1 && Items.Count > 0)
        //            {
        //                SelectedIndex = 0;
        //            }
        //            break;

        //        case NotifyCollectionChangedAction.Replace:
        //            break;
        //    }
        //}

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
                ContentPresenter contentHost = ElementContentTop;
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
            TabPanel panel = ElementTabPanelTop;
            if (panel != null && !panel.Children.Contains(tabItem))
            {
                panel.Children.Add(tabItem);
            }
        }

        ///// <summary>
        ///// Inherited code: Requires comment.
        ///// </summary>
        ///// <param name="index">Inherited code: Requires comment 1.</param>
        ///// <param name="tabItem">Inherited code: Requires comment 2.</param>
        //private void InsertIntoTabPanel(int index, TabItem tabItem)
        //{
        //    TabPanel panel = ElementTabPanelTop;
        //    if (panel != null && !panel.Children.Contains(tabItem))
        //    {
        //        panel.Children.Insert(index, tabItem);
        //    }
        //}

        ///// <summary>
        ///// Inherited code: Requires comment.
        ///// </summary>
        ///// <param name="tabItem">Inherited code: Requires comment 1.</param>
        //private void RemoveFromTabPanel(TabItem tabItem)
        //{
        //    TabPanel panel = ElementTabPanelTop;
        //    if (panel != null && panel.Children.Contains(tabItem))
        //    {
        //        panel.Children.Remove(tabItem);
        //    }
        //}

        ///// <summary>
        ///// Inherited code: Requires comment.
        ///// </summary>
        //private void ClearTabPanel()
        //{
        //    TabPanel panel = ElementTabPanelTop;
        //    if (panel != null)
        //    {
        //        panel.Children.Clear();
        //    }
        //}

        /// <summary>
        /// Gets or sets the TabStripPlacement Top template.
        /// </summary>
        internal FrameworkElement ElementTemplateTop { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateTopName = "TemplateTop";

        /// <summary>
        /// Gets or sets the TabPanel of the TabStripPlacement Top template.
        /// </summary>
        internal TabPanel ElementTabPanelTop { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTabPanelTopName = "TabPanelTop";

        /// <summary>
        /// Gets or sets the ContentHost of the TabStripPlacement Top template.
        /// </summary>
        internal ContentPresenter ElementContentTop { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementContentTopName = "ContentTop";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private int _desiredIndex;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _updateIndex = true;

        protected override void OnChildItemRemoved(object item)
        {
            base.OnChildItemRemoved(item);

            // Ensure that, when an item is removed from the list of items, we deselect it:
            if (this.SelectedItem == item)
            {
                SetLocalValue(SelectedIndexProperty, -1); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                SetLocalValue(SelectedItemProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                SetLocalValue(SelectedContentProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                //todo: update binding of SelectedIndex, SelectedValue, and SelectedItem
            }
        }

        protected override void UpdateItemsPanel(ItemsPanelTemplate newTemplate)
        {
            //this is to keep the implementation of UpdateItemsPanel from ItemsControl from interfering with this class' logic.
        }

        protected override void AddChildItemToVisualTree(object item)
        {
            //this is to keep the implementation of AddChildItemToVisualTree from ItemsControl from interfering with this class' logic.
            if (item is TabItem)
            {
                AddToTabPanel((TabItem)item);
            }
        }
    }
}
