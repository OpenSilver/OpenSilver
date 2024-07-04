// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a selectable item in a <see cref="TabControl" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = ElementTemplateTopSelectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateBottomSelectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateLeftSelectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateRightSelectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateTopUnselectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateBottomUnselectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateLeftUnselectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementTemplateRightUnselectedName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementHeaderTopSelectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderBottomSelectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderLeftSelectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderRightSelectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderTopUnselectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderBottomUnselectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderLeftUnselectedName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementHeaderRightUnselectedName, Type = typeof(ContentControl))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    public class TabItem : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabItem" /> class.
        /// </summary>
        public TabItem()
            : base()
        {
            MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            MouseEnter += new MouseEventHandler(OnMouseEnter);
            MouseLeave += new MouseEventHandler(OnMouseLeave);
            GotFocus += delegate { IsFocused = true; };
            LostFocus += delegate { IsFocused = false; };
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            DefaultStyleKey = typeof(TabItem);
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="TabItem" /> when a new template
        /// is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Clear previous content from old ContentControl
            ContentControl cc = GetContentControl(IsSelected, TabStripPlacement);
            if (cc != null)
            {
                cc.Content = null;
            }

            // Get the parts
            ElementTemplateTopSelected = GetTemplateChild(ElementTemplateTopSelectedName) as FrameworkElement;
            ElementTemplateBottomSelected = GetTemplateChild(ElementTemplateBottomSelectedName) as FrameworkElement;
            ElementTemplateLeftSelected = GetTemplateChild(ElementTemplateLeftSelectedName) as FrameworkElement;
            ElementTemplateRightSelected = GetTemplateChild(ElementTemplateRightSelectedName) as FrameworkElement;
            ElementTemplateTopUnselected = GetTemplateChild(ElementTemplateTopUnselectedName) as FrameworkElement;
            ElementTemplateBottomUnselected = GetTemplateChild(ElementTemplateBottomUnselectedName) as FrameworkElement;
            ElementTemplateLeftUnselected = GetTemplateChild(ElementTemplateLeftUnselectedName) as FrameworkElement;
            ElementTemplateRightUnselected = GetTemplateChild(ElementTemplateRightUnselectedName) as FrameworkElement;

            ElementHeaderTopSelected = GetTemplateChild(ElementHeaderTopSelectedName) as ContentControl;
            ElementHeaderBottomSelected = GetTemplateChild(ElementHeaderBottomSelectedName) as ContentControl;
            ElementHeaderLeftSelected = GetTemplateChild(ElementHeaderLeftSelectedName) as ContentControl;
            ElementHeaderRightSelected = GetTemplateChild(ElementHeaderRightSelectedName) as ContentControl;
            ElementHeaderTopUnselected = GetTemplateChild(ElementHeaderTopUnselectedName) as ContentControl;
            ElementHeaderBottomUnselected = GetTemplateChild(ElementHeaderBottomUnselectedName) as ContentControl;
            ElementHeaderLeftUnselected = GetTemplateChild(ElementHeaderLeftUnselectedName) as ContentControl;
            ElementHeaderRightUnselected = GetTemplateChild(ElementHeaderRightUnselectedName) as ContentControl;

            // Load Header
            UpdateHeaderVisuals();

            // Update visuals
            ChangeVisualState(false);
        }

        /// <summary>
        /// Creates and returns an <see cref="AutomationPeer" /> for this <see cref="TabItem" />.
        /// </summary>
        /// <returns>
        /// An automation peer for this <see cref="TabItem" />.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TabItemAutomationPeer(this);
        }

        /// <summary>
        /// Gets or sets the header of the <see cref="TabItem" />.
        /// </summary>
        /// <value>
        /// The current header of the <see cref="TabItem" />.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Header" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="Header" /> dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(TabItem),
                new PropertyMetadata(OnHeaderChanged));

        /// <summary>
        /// Header property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed its Header.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ctrl = (TabItem)d;

            ctrl.HasHeader = (e.NewValue != null) ? true : false;
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="Header" /> property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The previous value of the <see cref="Header" /> property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the <see cref="Header" /> property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateHeaderVisuals()
        {
            ContentControl header = GetContentControl(IsSelected, TabStripPlacement);
            if (header != null)
            {
                header.Content = this.Header;
                header.ContentTemplate = this.HeaderTemplate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="TabItem" /> has a header.
        /// </summary>
        /// <value>
        /// True if <see cref="Header" /> is not null; otherwise, false.
        /// </value>
        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderProperty); }
            private set { SetValue(HasHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HasHeader" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="HasHeader" /> dependency property.
        /// </value>
        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register(
                nameof(HasHeader),
                typeof(bool),
                typeof(TabItem),
                null);

        /// <summary>
        /// Gets or sets the template that is used to display the content of the
        /// <see cref="TabItem" /> header.
        /// </summary>
        /// <value>
        /// The current template that is used to display <see cref="TabItem" /> 
        /// header content.
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="HeaderTemplate" /> dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(TabItem),
                new PropertyMetadata(OnHeaderTemplateChanged));

        /// <summary>
        /// HeaderTemplate property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed its HeaderTemplate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ctrl = (TabItem)d;
            ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="HeaderTemplate" /> property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The previous value of the <see cref="HeaderTemplate" /> property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the <see cref="HeaderTemplate" /> property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TabItem" /> is currently selected.
        /// </summary>
        /// <value>
        /// True if the <see cref="TabItem" /> is selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="IsSelected" /> dependency property.
        /// </value>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(TabItem),
                new PropertyMetadata(OnIsSelectedChanged));

        /// <summary>
        /// IsSelected changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed IsSelected.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;
            Debug.Assert(tabItem != null, "TabItem should not be null!");

            bool isSelected = (bool)e.NewValue;

            RoutedEventArgs args = new RoutedEventArgs();

            if (isSelected)
            {
                tabItem.OnSelected(args);
            }
            else
            {
                tabItem.OnUnselected(args);
            }

            // fire the IsSelectedChanged event for automation
            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                TabControl parentSelector = tabItem.TabControlParent;
                if (parentSelector != null)
                {
                    TabItemAutomationPeer tabItemPeer = GetTabItemAutomationPeer(tabItem);
                    if (tabItemPeer != null)
                    {
                        tabItemPeer.RaiseAutomationIsSelectedChanged(isSelected);
                    }
                }
            }

            tabItem.IsTabStop = isSelected;
            tabItem.UpdateVisualState();
        }

        /// <summary>
        /// We use this function to get the TabItemAutomationPeer associated
        /// with the TabItem.
        /// </summary>
        /// <param name="item">
        /// TabItem that we are seeking to find the AutomationPeer for.
        /// </param>
        /// <returns>
        /// The TabItemAutomationPeer for the specified TabItem.
        /// </returns>
        internal static TabItemAutomationPeer GetTabItemAutomationPeer(TabItem item)
        {
            TabControlAutomationPeer tabControlPeer = TabControlAutomationPeer.FromElement(item.TabControlParent) as TabControlAutomationPeer;

            if (tabControlPeer == null)
            {
                tabControlPeer = TabControlAutomationPeer.CreatePeerForElement(item.TabControlParent) as TabControlAutomationPeer;
            }

            if (tabControlPeer != null)
            {
                List<AutomationPeer> children = tabControlPeer.GetChildren();
                if (children != null)
                {
                    foreach (AutomationPeer peer in children)
                    {
                        TabItemAutomationPeer tabItemPeer = peer as TabItemAutomationPeer;
                        if (tabItemPeer != null && tabItemPeer.Owner == item)
                        {
                            return tabItemPeer;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Called to indicate that the <see cref="IsSelected" /> property has changed to true.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            if (TabControlParent != null)
            {
                TabControlParent.SelectedItem = this;
            }
        }

        /// <summary>
        /// Called to indicate that the <see cref="IsSelected" /> property has changed to false.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            if (TabControlParent != null && TabControlParent.SelectedItem == this)
            {
                TabControlParent.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Gets the location of the tab strip relative to the <see cref="TabItem" /> content.
        /// </summary>
        /// <value>
        /// The location of the tab strip relative to the <see cref="TabItem" /> content.
        /// </value>
        public Dock TabStripPlacement
        {
            get { return ((TabControlParent == null) ? Dock.Top : TabControlParent.TabStripPlacement); }
        }

        /// <summary>
        /// This method is invoked when the Content property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The previous <see cref="TabItem" /> content.
        /// </param>
        /// <param name="newContent">
        /// The new <see cref="TabItem" /> content.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            TabControl tabControl = TabControlParent;
            if (tabControl != null)
            {
                // If this is the selected TabItem then we should update
                // TabControl.SelectedContent
                if (IsSelected)
                {
                    tabControl.SelectedContent = newContent;
                }
            }
        }

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="KeyEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
            Key invariantKey = InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

            TabItem nextTabItem = null;

            int direction = 0;
            int startIndex = TabControlParent.Items.IndexOf(this);
            switch (invariantKey)
            {
                case Key.Right:
                case Key.Down:
                    direction = 1;
                    break;
                case Key.Left:
                case Key.Up:
                    direction = -1;
                    break;
                default:
                    return;
            }

            nextTabItem = TabControlParent.FindNextTabItem(startIndex, direction);

            if (nextTabItem != null && nextTabItem != TabControlParent.SelectedItem)
            {
                e.Handled = true;
                TabControlParent.SelectedItem = nextTabItem;
                nextTabItem.Focus();
            }
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">
        /// Control that triggers this property change.
        /// </param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "New value should be a Boolean!");
            bool isEnabled = (bool)e.NewValue;
            ContentControl header = GetContentControl(IsSelected, TabStripPlacement);
            if (header != null)
            {
                if (!isEnabled)
                {
                    _isMouseOver = false;
                }

                UpdateVisualState();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this element has logical focus.
        /// </summary>
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsFocused" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="IsFocused" /> dependency property.
        /// </value>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                nameof(IsFocused),
                typeof(bool),
                typeof(TabItem),
                new PropertyMetadata(OnIsFocusedPropertyChanged));

        /// <summary>
        /// IsFocusedProperty property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed IsFocused.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ti = d as TabItem;
            Debug.Assert(ti != null, "TabItem should not be null!");

            ti.OnIsFocusChanged(e);
        }

        /// <summary>
        /// Called when the IsFocused property changes.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// Change to the correct visual state for the TabItem.
        /// </summary>
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the TabItem.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            // Choose the appropriate TabItem template to display based on which
            // TabStripPlacement we are using and whether the item is selected.
            UpdateTabItemVisuals();

            // Update the CommonStates group
            if (!IsEnabled || (TabControlParent != null && !TabControlParent.IsEnabled))
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver && !IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the SelectionStates group
            if (IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnselected);
            }

            // Update the FocusStates group
            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateTabItemVisuals()
        {
            // update the template that is displayed
            FrameworkElement currentTemplate = GetTemplate(IsSelected, TabStripPlacement);

            if (_previousTemplate != null && _previousTemplate != currentTemplate)
            {
                _previousTemplate.Visibility = Visibility.Collapsed;
            }
            _previousTemplate = currentTemplate;
            if (currentTemplate != null)
            {
                currentTemplate.Visibility = Visibility.Visible;
            }

            // update the ContentControl's header
            ContentControl currentHeader = GetContentControl(IsSelected, TabStripPlacement);

            if (_previousHeader != null && _previousHeader != currentHeader)
            {
                _previousHeader.Content = null;
            }
            _previousHeader = currentHeader;
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Handles when the mouse leaves the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOver = false;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles when the mouse enters the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOver = true;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles the mouse left button down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && TabControlParent != null && !IsSelected && !e.Handled)
            {
                IsTabStop = true;
                e.Handled = Focus();
                TabControlParent.SelectedIndex = TabControlParent.Items.IndexOf(this);
            }
        }

        /// <summary>
        /// Gets a reference to the TabControl that holds this TabItem.  It will
        /// step up the UI tree to find the TabControl that contains this
        /// TabItem.
        /// </summary>
        private TabControl TabControlParent
        {
            get
            {
                // We need this for when the TabControl/TabItem is not in the
                // visual tree yet.
                TabControl tabCtrl = Parent as TabControl;
                if (tabCtrl != null)
                {
                    return tabCtrl;
                }

                // Once the TabControl is added to the visual tree, the
                // TabItem's parent becomes the TabPanel, so we now have to step
                // up the visual tree to find the owning TabControl.
                DependencyObject obj = this as DependencyObject;
                while (obj != null)
                {
                    TabControl tc = obj as TabControl;
                    if (tc != null)
                    {
                        return tc;
                    }
                    obj = VisualTreeHelper.GetParent(obj) as DependencyObject;
                }
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="isSelected">Inherited code: Requires comment 1.</param>
        /// <param name="tabPlacement">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        internal FrameworkElement GetTemplate(bool isSelected, Dock tabPlacement)
        {
            switch (tabPlacement)
            {
                case Dock.Top:
                    return isSelected ? ElementTemplateTopSelected : ElementTemplateTopUnselected;
                case Dock.Bottom:
                    return isSelected ? ElementTemplateBottomSelected : ElementTemplateBottomUnselected;
                case Dock.Left:
                    return isSelected ? ElementTemplateLeftSelected : ElementTemplateLeftUnselected;
                case Dock.Right:
                    return isSelected ? ElementTemplateRightSelected : ElementTemplateRightUnselected;
            }
            return null;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="isSelected">Inherited code: Requires comment 1.</param>
        /// <param name="tabPlacement">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        internal ContentControl GetContentControl(bool isSelected, Dock tabPlacement)
        {
            switch (tabPlacement)
            {
                case Dock.Top:
                    return isSelected ? ElementHeaderTopSelected : ElementHeaderTopUnselected;
                case Dock.Bottom:
                    return isSelected ? ElementHeaderBottomSelected : ElementHeaderBottomUnselected;
                case Dock.Left:
                    return isSelected ? ElementHeaderLeftSelected : ElementHeaderLeftUnselected;
                case Dock.Right:
                    return isSelected ? ElementHeaderRightSelected : ElementHeaderRightUnselected;
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the TabStripPlacement Top Selected template.
        /// </summary>
        internal FrameworkElement ElementTemplateTopSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateTopSelectedName = "TemplateTopSelected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Bottom Selected template.
        /// </summary>
        internal FrameworkElement ElementTemplateBottomSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateBottomSelectedName = "TemplateBottomSelected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Left Selected template.
        /// </summary>
        internal FrameworkElement ElementTemplateLeftSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateLeftSelectedName = "TemplateLeftSelected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Right Selected template.
        /// </summary>
        internal FrameworkElement ElementTemplateRightSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateRightSelectedName = "TemplateRightSelected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Top Unselected template.
        /// </summary>
        internal FrameworkElement ElementTemplateTopUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateTopUnselectedName = "TemplateTopUnselected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Bottom Unselected template.
        /// </summary>
        internal FrameworkElement ElementTemplateBottomUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateBottomUnselectedName = "TemplateBottomUnselected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Left Unselected template.
        /// </summary>
        internal FrameworkElement ElementTemplateLeftUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateLeftUnselectedName = "TemplateLeftUnselected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Right Unselected template.
        /// </summary>
        internal FrameworkElement ElementTemplateRightUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateRightUnselectedName = "TemplateRightUnselected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Top Selected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderTopSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderTopSelectedName = "HeaderTopSelected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Bottom Selected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderBottomSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderBottomSelectedName = "HeaderBottomSelected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Left Selected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderLeftSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderLeftSelectedName = "HeaderLeftSelected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Right Selected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderRightSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderRightSelectedName = "HeaderRightSelected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Top Unselected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderTopUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderTopUnselectedName = "HeaderTopUnselected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Bottom Unselected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderBottomUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderBottomUnselectedName = "HeaderBottomUnselected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Left Unselected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderLeftUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderLeftUnselectedName = "HeaderLeftUnselected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Right Unselected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderRightUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderRightUnselectedName = "HeaderRightUnselected";

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseOver;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private FrameworkElement _previousTemplate;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private ContentControl _previousHeader;
    }
}
