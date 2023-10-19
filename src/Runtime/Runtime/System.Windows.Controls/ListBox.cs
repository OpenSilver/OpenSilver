
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
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Contains a list of selectable items.
    /// </summary>
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    public partial class ListBox : Selector
    {
        private ScrollViewer _scrollHost;
        private ItemInfo _anchorItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox"/> class.
        /// </summary>
        public ListBox()
        {
            DefaultStyleKey = typeof(ListBox);
        }

        internal sealed override bool HandlesScrolling => true;

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
            set { SetValue(SelectionModeProperty, value); }
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
        /// Identifies the <see cref="ItemContainerStyle"/> dependency
        /// property.
        /// </summary>
        public static readonly new DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty;

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
                throw new NotSupportedException("Can only call SelectAll when SelectionMode is Multiple or Extended.");
            }
        }

        public override void OnApplyTemplate()
        {
            // _scrollHost must be set before calling base
            _scrollHost = GetTemplateChild("ScrollViewer") as ScrollViewer;

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
                            && FocusManager.GetFocusedElement() is ListBoxItem listBoxItem)
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

            if (info.Container is ListBoxItem listItem)
            {
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
                    UpdateAnchorItem(ItemInfoFromContainer(item));
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

        internal ItemInfo AnchorItemInternal
        {
            get { return _anchorItem; }
            set { _anchorItem = value?.Clone(); } // clone, so that adjustments to selection and anchor don't double-adjust
        }

        internal void NotifyListItemClicked(ListBoxItem item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    
                    if (!item.IsSelected)
                    {
                        item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
                    }
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        item.SetCurrentValue(ListBoxItem.IsSelectedProperty, false);
                    }

                    UpdateAnchorItem(ItemInfoFromContainer(item));
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

        private void UpdateAnchorItem(ItemInfo info)
        {
            object item = info.Item;

            if (item == DependencyProperty.UnsetValue)
            {
                AnchorItemInternal = null;
            }
            else
            {
                AnchorItemInternal = info;
            }
        }

        private void MakeSingleSelection(ListBoxItem listItem)
        {
            if (ItemsControlFromItemContainer(listItem) == this)
            {
                ItemInfo info = ItemInfoFromContainer(listItem);

                SelectionChange.SelectJustThisItem(info, true /* assumeInItemsCollection */);

                listItem.Focus();

                UpdateAnchorItem(info);
            }
        }

        private void MakeToggleSelection(ListBoxItem item)
        {
            item.SetCurrentValue(SelectorItem.IsSelectedProperty, !item.IsSelected);

            UpdateAnchorItem(ItemInfoFromContainer(item));
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
        }

        #region Obsoletes

        [Obsolete(Helper.ObsoleteMemberMessage)]
        public Brush SelectedItemBackgroundBrush
        {
            get { return SelectedItemBackground; }
            set { SelectedItemBackground = value; }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        public Brush SelectedItemForegroundBrush
        {
            get { return SelectedItemForeground; }
            set { SelectedItemForeground = value; }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        public Brush UnselectedItemBackgroundBrush
        {
            get { return RowBackground; }
            set { RowBackground = value; }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        public Brush UnselectedItemForegroundBrush
        {
            get { return UnselectedItemForeground; }
            set { UnselectedItemForeground = value; }
        }

        #endregion Obsoletes
    }
}