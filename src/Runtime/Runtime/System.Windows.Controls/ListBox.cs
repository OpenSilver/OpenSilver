
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
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ModifierKeys = System.Windows.Input.ModifierKeys;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.System;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
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
        private ItemInfo _anchorItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox"/> class.
        /// </summary>
        public ListBox()
        {
            DefaultStyleKey = typeof(ListBox);
        }

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
        public void ScrollIntoView(object item)
        {
            int index = Items.IndexOf(item);
            if (index > -1)
            {
                if (ItemsHost is VirtualizingPanel vp)
                {
                    vp.BringIndexIntoViewInternal(index);
                }
                else
                {
                    if (ItemContainerGenerator.ContainerFromIndex(index) is ListBoxItem container
                        && container.INTERNAL_OuterDomElement != null)
                    {
                        OpenSilver.Interop.ExecuteJavaScript(
                            "$0.scrollIntoView({ block: 'nearest'})",
                            container.INTERNAL_OuterDomElement);
                    }
                }
            }
        }

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

        [Obsolete("Use SelectedItemBackground instead.")]
        public Brush SelectedItemBackgroundBrush
        {
            get { return SelectedItemBackground; }
            set { SelectedItemBackground = value; }
        }

        [Obsolete("Use SelectedItemForeground instead.")]
        public Brush SelectedItemForegroundBrush
        {
            get { return SelectedItemForeground; }
            set { SelectedItemForeground = value; }
        }

        [Obsolete("Use RowBackground instead.")]
        public Brush UnselectedItemBackgroundBrush
        {
            get { return RowBackground; }
            set { RowBackground = value; }
        }

        [Obsolete("Use UnselectedItemForeground instead.")]
        public Brush UnselectedItemForegroundBrush
        {
            get { return UnselectedItemForeground; }
            set { UnselectedItemForeground = value; }
        }

        #endregion Obsoletes
    }
}