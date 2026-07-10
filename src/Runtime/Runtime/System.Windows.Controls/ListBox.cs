
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

using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Contains a list of selectable items.
    /// </summary>
    [TemplatePart(Name = ScrollViewerTemplateName, Type = typeof(ScrollViewer))]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    public partial class ListBox : Selector
    {
        private const string ScrollViewerTemplateName = "ScrollViewer";

        private ScrollViewer _scrollHost;
        private ItemInfo _anchorItem;
        private WeakReference<ListBoxItem> _lastActionItem;

        static ListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBox), new PropertyMetadata(typeof(ListBox)));
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(ListBox), new PropertyMetadata(BooleanBoxes.TrueBox));

            EventManager.RegisterClassHandler<ListBox>(Keyboard.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox"/> class.
        /// </summary>
        public ListBox()
        {
            ValidateSelectionMode((SelectionMode)SelectionModeProperty.GetDefaultValue(DependencyObjectType));
        }

        /// <inheritdoc />
        protected internal override bool HandlesScrolling => true;

        internal sealed override ScrollViewer ScrollHost => _scrollHost;

        /// <summary>
        /// Gets the list of currently selected items for the <see cref="ListBox"/>
        /// control.
        /// </summary>
        /// <returns>
        /// The list of currently selected items for the <see cref="ListBox"/>.
        /// </returns>
        public IList SelectedItems
        {
            get { return SelectedItemsImpl; }
        }

        /// <summary>
        /// Gets or sets the selection behavior for the <see cref="ListBox"/> control.
        /// </summary>
        /// <returns>
        /// One of the <see cref="SelectionMode"/> values.
        /// </returns>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValueInternal(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                nameof(SelectionMode), 
                typeof(SelectionMode), 
                typeof(ListBox), 
                new PropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = (ListBox)d;
            listBox.ValidateSelectionMode(listBox.SelectionMode);
        }

        private void ValidateSelectionMode(SelectionMode mode)
        {
            CanSelectMultiple = (mode != SelectionMode.Single);
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the item containers.
        /// </summary>
        /// <returns>
        /// The style applied to the item containers. The default is null.
        /// </returns>
        public new Style ItemContainerStyle
        {
            get { return base.ItemContainerStyle; }
            set { base.ItemContainerStyle = value; }
        }

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty;

        /// <summary>
        /// Identifies the IsSelectionActive dependency property.
        /// </summary>
        new public static readonly DependencyProperty IsSelectionActiveProperty = Selector.IsSelectionActiveProperty;

        /// <summary>
        /// Causes the object to scroll into view.
        /// </summary>
        /// <param name="item">
        /// The object to scroll.
        /// </param>
        public void ScrollIntoView(object item) => ScrollIntoViewImpl(Items.IndexOf(item));

        /// <summary>
        /// Selects all the items in the <see cref="ListBox"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// <see cref="SelectionMode"/> is set to <see cref="SelectionMode.Single"/>
        /// </exception>
        public void SelectAll()
        {
            if (CanSelectMultiple)
            {
                SelectAllImpl();
            }
            else
            {
                throw new NotSupportedException(Strings.ListBoxSelectAllSelectionMode);
            }
        }

        /// <summary>
        /// Clears all the selection in a <see cref="ListBox"/>.
        /// </summary>
        public void UnselectAll() => UnselectAllImpl();

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            // In a single selection mode we want to move anchor to the selected element
            if (SelectionMode == SelectionMode.Single)
            {
                ItemInfo info = InternalSelectedInfo;
                ListBoxItem listItem = info != null ? info.Container as ListBoxItem : null;

                if (listItem != null)
                {
                    UpdateAnchorAndActionItem(info);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            // _scrollHost must be set before calling base
            _scrollHost = GetTemplateChild(ScrollViewerTemplateName) as ScrollViewer;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="ListBoxItem"/> corresponding to a specified item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own item container.
        /// </summary>
        /// <param name="item">
        /// The specified item.
        /// </param>
        /// <returns>
        /// true if the item is its own item container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListBoxItem;
        }

        private static void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;

            // Focus drives the selection when keyboardnavigation is used
            if (InputManager.Current.MostRecentInputDevice is not KeyboardDevice)
            {
                return;
            }

            // Only in case focus moves from one ListBoxItem to another we want the selection to follow focus
            ListBoxItem newListBoxItem = e.NewFocus as ListBoxItem;
            if (newListBoxItem != null && ItemsControlFromItemContainer(newListBoxItem) == listbox)
            {
                UIElement visualOldFocus = e.OldFocus as UIElement;

                if ((visualOldFocus != null && listbox.IsAncestorOf(visualOldFocus)) || visualOldFocus == listbox)
                {
                    listbox.LastActionItem = newListBoxItem;
                    listbox.MakeKeyboardSelection(newListBoxItem);
                }
            }
        }

        /// <summary>
        /// Responds to the KeyDown event. 
        /// </summary> 
        /// <param name="e">Provides data for KeyEventArgs.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                bool handled = false;
                int newFocusedIndex = -1;
                switch (e.Key)
                {
                    case Key.Space:
                        if (ModifierKeys.Alt != (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt))
                            && Keyboard.FocusedElement is ListBoxItem listBoxItem)
                        {
                            MakeKeyboardSelection(listBoxItem);
                            handled = true;
                        }
                        break;
                    case Key.Home:
                        newFocusedIndex = NavigateToStart();
                        break;
                    case Key.End:
                        newFocusedIndex = NavigateToEnd();
                        break;
                    case Key.PageUp:
                        newFocusedIndex = NavigateByPage(false);
                        break;
                    case Key.PageDown:
                        newFocusedIndex = NavigateByPage(true);
                        break;
                    case Key.Left:
                        if (IsVerticalOrientation())
                        {
                            ElementScrollViewerScrollInDirection(Key.Left);
                        }
                        else
                        {
                            newFocusedIndex = NavigateByLine(false);
                        }
                        break;
                    case Key.Up:
                        if (IsVerticalOrientation())
                        {
                            newFocusedIndex = NavigateByLine(false);
                        }
                        else
                        {
                            ElementScrollViewerScrollInDirection(Key.Up);
                        }
                        break;
                    case Key.Right:
                        if (IsVerticalOrientation())
                        {
                            ElementScrollViewerScrollInDirection(Key.Right);
                        }
                        else
                        {
                            newFocusedIndex = NavigateByLine(true);
                        }
                        break;
                    case Key.Down:
                        if (IsVerticalOrientation())
                        {
                            newFocusedIndex = NavigateByLine(true);
                        }
                        else
                        {
                            ElementScrollViewerScrollInDirection(Key.Down);
                        }
                        break;
                    default:
                        Debug.Assert(!handled);
                        break;
                }

                if (newFocusedIndex >= 0 && newFocusedIndex < Items.Count)
                {
                    if (ItemContainerGenerator.ContainerFromIndex(newFocusedIndex) is ListBoxItem listBoxItem)
                    {
                        listBoxItem.Focus();
                        MakeKeyboardSelection(listBoxItem);
                    }

                    handled = true;
                }

                if (handled)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="ListBoxAutomationPeer"/> for the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="ListBoxAutomationPeer"/> for the <see cref="ListBox"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ListBoxAutomationPeer(this);

        internal override bool FocusItem(ItemInfo info)
        {
            // Base will actually focus the item
            bool returnValue = base.FocusItem(info);

            if ((info.Container ?? ItemContainerGenerator.ContainerFromIndex(info.Index)) is ListBoxItem listItem)
            {
                LastActionItem = listItem;

                MakeKeyboardSelection(listItem);
            }

            return returnValue;
        }

        private void MakeKeyboardSelection(ListBoxItem item)
        {
            if (item == null)
            {
                return;
            }

            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    // Navigating when control is down shouldn't select the item
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                    {
                        MakeSingleSelection(item);
                    }
                    break;

                case SelectionMode.Multiple:
                    UpdateAnchorAndActionItem(ItemInfoFromContainer(item));
                    break;

                case SelectionMode.Extended:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        bool clearCurrentSelection = (Keyboard.Modifiers & ModifierKeys.Control) == 0;
                        MakeAnchorSelection(item, clearCurrentSelection);
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                    {
                        MakeSingleSelection(item);
                    }
                    break;
            }
        }

        /// <summary>
        /// Adjust ItemInfos when the Items property changes.
        /// </summary>
        internal override void AdjustItemInfoOverride(NotifyCollectionChangedEventArgs e)
        {
            AdjustItemInfo(e, _anchorItem);

            // If the anchor item is removed, drop our reference to it.
            if (_anchorItem != null && _anchorItem.Index < 0)
            {
                _anchorItem = null;
            }

            base.AdjustItemInfoOverride(e);
        }

        /// <summary>
        /// Adjust ItemInfos when the generator finishes.
        /// </summary>
        internal override void AdjustItemInfosAfterGeneratorChangeOverride()
        {
            AdjustItemInfoAfterGeneratorChange(_anchorItem);
            base.AdjustItemInfosAfterGeneratorChangeOverride();
        }

        /// <summary>
        /// Gets or sets the item that is initially selected when <see cref="SelectionMode"/> 
        /// is <see cref="SelectionMode.Extended"/>.
        /// </summary>
        /// <returns>
        /// The item that is initially selected when <see cref="SelectionMode"/> is 
        /// <see cref="SelectionMode.Extended"/>.
        /// </returns>
        protected object AnchorItem
        {
            get => AnchorItemInternal;
            set
            {
                if (value is not null && value != DependencyProperty.UnsetValue)
                {
                    ItemInfo info = NewItemInfo(value);

                    if (info.Container is not ListBoxItem listBoxItem)
                    {
                        throw new InvalidOperationException(string.Format(Strings.ListBoxInvalidAnchorItem, value));
                    }

                    AnchorItemInternal = info;
                    LastActionItem = listBoxItem;
                }
                else
                {
                    AnchorItemInternal = null;
                    LastActionItem = null;
                }
            }
        }

        internal ItemInfo AnchorItemInternal
        {
            get { return _anchorItem; }
            set { _anchorItem = value?.Clone(); } // clone, so that adjustments to selection and anchor don't double-adjust
        }

        internal ListBoxItem LastActionItem
        {
            get
            {
                if (_lastActionItem is not null && _lastActionItem.TryGetTarget(out ListBoxItem lastActionItem))
                {
                    return lastActionItem;
                }
                return null;
            }
            set
            {
                if (value is null)
                {
                    _lastActionItem = null;
                    return;
                }

                _lastActionItem = new WeakReference<ListBoxItem>(value);
            }
        }

        internal void NotifyListItemClicked(ListBoxItem item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    
                    if (!item.IsSelected)
                    {
                        item.SetCurrentValueInternal(ListBoxItem.IsSelectedProperty, BooleanBoxes.TrueBox);
                    }
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        item.SetCurrentValueInternal(ListBoxItem.IsSelectedProperty, BooleanBoxes.FalseBox);
                    }

                    UpdateAnchorAndActionItem(ItemInfoFromContainer(item));
                    break;

                case SelectionMode.Multiple:
                    MakeToggleSelection(item);
                    break;

                case SelectionMode.Extended:
                    ModifierKeys kbModifiers = Keyboard.Modifiers;
                    if (kbModifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        MakeAnchorSelection(item, false);
                    }
                    else if (kbModifiers.HasFlag(ModifierKeys.Control))
                    {
                        MakeToggleSelection(item);
                    }
                    else if (kbModifiers.HasFlag(ModifierKeys.Shift))
                    {
                        MakeAnchorSelection(item, true);
                    }
                    else
                    {
                        MakeSingleSelection(item);
                    }
                    break;
            }
        }

        private void UpdateAnchorAndActionItem(ItemInfo info)
        {
            object item = info.Item;
            ListBoxItem listItem = info.Container as ListBoxItem;

            if (item == DependencyProperty.UnsetValue)
            {
                AnchorItemInternal = null;
                LastActionItem = null;
            }
            else
            {
                AnchorItemInternal = info;
                LastActionItem = listItem;
            }

            KeyboardNavigation.SetTabOnceActiveElement(this, listItem);
        }

        private void MakeSingleSelection(ListBoxItem listItem)
        {
            if (ItemsControlFromItemContainer(listItem) == this)
            {
                ItemInfo info = ItemInfoFromContainer(listItem);

                SelectionChange.SelectJustThisItem(info, true /* assumeInItemsCollection */);

                listItem.Focus();

                UpdateAnchorAndActionItem(info);
            }
        }

        private void MakeToggleSelection(ListBoxItem item)
        {
            item.SetCurrentValueInternal(IsSelectedProperty, BooleanBoxes.Box(!item.IsSelected));

            UpdateAnchorAndActionItem(ItemInfoFromContainer(item));
        }

        private void MakeAnchorSelection(ListBoxItem actionItem, bool clearCurrent)
        {
            ItemInfo anchorInfo = AnchorItemInternal;

            if (anchorInfo == null)
            {
                if (SelectedItemsInternal.Count > 0)
                {
                    // If we haven't set the anchor, then just use the last selected item
                    AnchorItemInternal = SelectedItemsInternal[SelectedItemsInternal.Count - 1];
                }
                else
                {
                    // There was nothing selected, so take the first child element
                    AnchorItemInternal = NewItemInfo(Items[0], null, 0);
                }
            }

            // Find the indexes of the elements
            int start, end;

            start = ItemContainerGenerator.IndexFromContainer(actionItem);
            end = AnchorItemInternal.Index;

            // Ensure start is before end
            if (start > end)
            {
                int index = start;

                start = end;
                end = index;
            }

            bool beganSelectionChange = false;
            if (!SelectionChange.IsActive)
            {
                beganSelectionChange = true;
                SelectionChange.Begin();
            }
            try
            {
                if (clearCurrent)
                {
                    // Unselect items not within the selection range
                    for (int index = 0; index < SelectedItemsInternal.Count; index++)
                    {
                        ItemInfo info = SelectedItemsInternal[index];
                        int itemIndex = info.Index;

                        if ((itemIndex < start) || (end < itemIndex))
                        {
                            SelectionChange.Unselect(info);
                        }
                    }
                }

                // Select the children in the selection range
                IEnumerator enumerator = ((IEnumerable)Items).GetEnumerator();
                for (int index = 0; index <= end; index++)
                {
                    enumerator.MoveNext();
                    if (index >= start)
                    {
                        SelectionChange.Select(NewItemInfo(enumerator.Current, null, index), true /* assumeInItemsCollection */);
                    }
                }

                IDisposable d = enumerator as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                }
            }
            finally
            {
                if (beganSelectionChange)
                {
                    SelectionChange.End();
                }
            }

            LastActionItem = actionItem;
        }
    }
}