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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.System;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that allows a user to select an item from a collection
    /// of items.
    /// </summary>
    public partial class Selector : ItemsControl
    {
        private bool SelectedValueWaitsForItems;
        private bool SelectedValueDrivesSelection;
        private bool SkipCoerceSelectedItemCheck;
        private bool SyncingSelectionAndCurrency;
        private ItemInfo PendingSelectionByValue;
        private bool _canSelectMultiple = false;
        private ChangeInfo _changeInfo;

        private DispatcherTimer _searchTimeoutTimer;
        private String _searchCriteria = String.Empty;
        private TimeSpan _searchCriteriaTimeOut = TimeSpan.FromMilliseconds(500);

        // The selected items that we interact with.  Most of the time when SelectedItems
        // is in use, this is identical to the value of the SelectedItems property, but
        // differs in type, and will differ in content in the case where you set or modify
        // SelectedItems and we need to switch our selection to what was just provided.
        // This is our internal representation of the selection and generally should be modified
        // only by SelectionChanger.  Internal classes may read this for efficiency's sake
        // to avoid putting SelectedItems "in use" but we can't really expose this externally.
        private readonly InternalSelectedItemsStorage _selectedItems = new InternalSelectedItemsStorage(1, MatchExplicitEqualityComparer);

        public Selector()
        {
            ItemContainerGenerator.StatusChanged += new EventHandler(OnGeneratorStatusChanged);

            SelectedItemsImpl = new SelectedItemCollection(this);
            SelectedItemsImpl.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSelectedItemsCollectionChanged);
            SelectionChange = new SelectionChanger(this);
        }

        /// <summary>
        /// Occurs when the selection is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)this.GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex),
                typeof(int),
                typeof(Selector),
                new PropertyMetadata(-1, OnSelectedIndexChanged, CoerceSelectedIndex));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;

            // If we're in the middle of a selection change, ignore all changes
            if (!s.SelectionChange.IsActive)
            {
                int newIndex = (int)e.NewValue;
                s.SelectionChange.SelectJustThisItem(s.ItemInfoFromIndex(newIndex), true /* assumeInItemsCollection */);
            }

            s.ManageSelectedIndex_Changed(e);
        }

        private static object CoerceSelectedIndex(DependencyObject d, object value)
        {
            Selector s = (Selector)d;
            if ((value is int) && (int)value >= s.Items.Count)
            {
                return DependencyProperty.UnsetValue;
            }

            return value;
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(Selector),
                new PropertyMetadata(null, OnSelectedItemChanged, CoerceSelectedItem));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;

            if (!s.SelectionChange.IsActive)
            {
                s.SelectionChange.SelectJustThisItem(s.NewItemInfo(e.NewValue), false /* assumeInItemsCollection */);
            }
        }

        private static object CoerceSelectedItem(DependencyObject d, object value)
        {
            Selector s = (Selector)d;
            if (value == null || s.SkipCoerceSelectedItemCheck)
                return value;

            int selectedIndex = s.SelectedIndex;

            if ((selectedIndex > -1 && selectedIndex < s.Items.Count && s.Items[selectedIndex] == value)
                || s.Items.Contains(value))
            {
                return value;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Gets or sets the value of the selected item, obtained by using the SelectedValuePath.
        /// </summary>
        public object SelectedValue
        {
            get { return this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                nameof(SelectedValue),
                typeof(object),
                typeof(Selector),
                new PropertyMetadata(null, OnSelectedValueChanged, CoerceSelectedValue));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;
            ItemInfo info = s.PendingSelectionByValue;
            if (info != null)
            {
                // There's a pending selection discovered during CoerceSelectedValue.
                // If no selection change is active, now's the time to actually select it.
                try
                {
                    if (!s.SelectionChange.IsActive)
                    {
                        s.SelectedValueDrivesSelection = true;
                        s.SelectionChange.SelectJustThisItem(info, assumeInItemsCollection: true);
                    }
                }
                finally
                {
                    s.SelectedValueDrivesSelection = false;
                    s.PendingSelectionByValue = null;
                }
            }
        }

        private static object CoerceSelectedValue(DependencyObject d, object value)
        {
            Selector s = (Selector)d;

            if (s.SelectionChange.IsActive)
            {
                // If we're in the middle of a selection change, accept the value
                s.SelectedValueDrivesSelection = false;
            }
            else
            {
                // Otherwise, this is a user-initiated change to SelectedValue.
                // Find the corresponding item.
                object item = s.SelectItemWithValue(value, selectNow: false);

                // if the search fails, coerce the value to null.  Unless there
                // are no items at all, in which case wait for the items to appear
                // and search again.
                if (item == DependencyProperty.UnsetValue && ((ItemsControl)s).HasItems)
                {
                    value = null;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets or sets the property path that is used to get the SelectedValue property
        /// of the SelectedItem property.
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)this.GetValue(SelectedValuePathProperty); }
            set { this.SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                nameof(SelectedValuePath),
                typeof(string),
                typeof(Selector),
                new PropertyMetadata(string.Empty, OnSelectedValuePathChanged));

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;
            s.CoerceValue(SelectedValueProperty);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="Selector" /> should keep 
        /// the <see cref="SelectedItem" /> synchronized with the current item in the 
        /// <see cref="ItemsControl.Items" /> property.
        /// </summary>
        /// <returns>
        /// true if the <see cref="SelectedItem" /> is always synchronized with the current item;
        /// false if the <see cref="SelectedItem" /> is never synchronized with the current item;
        /// null if the <see cref="SelectedItem" /> is synchronized with the current item only if 
        /// the <see cref="Selector" /> uses a <see cref="ICollectionView" />.
        /// The default is null.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <see cref="IsSynchronizedWithCurrentItem" /> is set to true.
        /// </exception>
        public bool? IsSynchronizedWithCurrentItem
        {
            get { return (bool?)GetValue(IsSynchronizedWithCurrentItemProperty); }
            set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsSynchronizedWithCurrentItem" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
            DependencyProperty.Register(
                nameof(IsSynchronizedWithCurrentItem),
                typeof(bool?),
                typeof(Selector),
                new PropertyMetadata(null, OnIsSynchronizedWithCurrentItemChanged));

        private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = (Selector)d;
            s.SetSynchronizationWithCurrentItem();
        }

        private void SetSynchronizationWithCurrentItem()
        {
            bool? isSynchronizedWithCurrentItem = IsSynchronizedWithCurrentItem;
            bool oldSync = IsSynchronizedWithCurrentItemPrivate;
            bool newSync;

            if (isSynchronizedWithCurrentItem.HasValue)
            {
                // if there's a value, use it
                newSync = isSynchronizedWithCurrentItem.Value;
            }
            else
            {
                // when the value is null, synchronize iff selection mode is Single
                // and there's a non-default view.
                SelectionMode mode = (SelectionMode)GetValue(ListBox.SelectionModeProperty);
                newSync = (mode == SelectionMode.Single) && ItemsSource is ICollectionView;
            }

            IsSynchronizedWithCurrentItemPrivate = newSync;

            if (!oldSync && newSync)
            {
                SetSelectedToCurrent();
            }
        }

        private void SetSelectedToCurrent()
        {
            Debug.Assert(IsSynchronizedWithCurrentItemPrivate);
            if (!SyncingSelectionAndCurrency)
            {
                SyncingSelectionAndCurrency = true;

                try
                {
                    ICollectionView icv = ItemsSource as ICollectionView;
                    Debug.Assert(icv != null);

                    object item = icv.CurrentItem;

                    if (item != null && ItemGetIsSelectable(item))
                    {
                        SelectionChange.SelectJustThisItem(NewItemInfo(item, null, icv.CurrentPosition), true /* assumeInItemsCollection */);
                    }
                    else
                    {
                        // Select nothing if Currency is not set.
                        SelectionChange.SelectJustThisItem(null, false);
                    }
                }
                finally
                {
                    SyncingSelectionAndCurrency = false;
                }
            }
        }

        private void SetCurrentToSelected()
        {
            Debug.Assert(IsSynchronizedWithCurrentItemPrivate);
            if (!SyncingSelectionAndCurrency)
            {
                ICollectionView icv = ItemsSource as ICollectionView;
                if (icv == null)
                {
                    return;
                }

                SyncingSelectionAndCurrency = true;

                try
                {
                    if (_selectedItems.Count == 0)
                    {
                        // this avoid treating null as an item
                        icv.MoveCurrentToPosition(-1);
                    }
                    else
                    {
                        int index = _selectedItems[0].Index;
                        if (index >= 0)
                        {
                            // use the index if we have it, to disambiguate duplicates
                            icv.MoveCurrentToPosition(index);
                        }
                        else
                        {
                            icv.MoveCurrentTo(InternalSelectedItem);
                        }
                    }
                }
                finally
                {
                    SyncingSelectionAndCurrency = false;
                }
            }
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, ItemsSource));

            if (IsSynchronizedWithCurrentItemPrivate)
                SetSelectedToCurrent();
        }

        // True if we're really synchronizing selection and current item
        private bool IsSynchronizedWithCurrentItemPrivate { get; set; }

        /// <summary>
        /// Gets whether the <see cref="Selector"/> contains items.
        /// </summary>
        protected new bool HasItems
        {
            get { return ItemsSource != null || base.HasItems; }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            ICollectionView icv = oldValue as ICollectionView;
            if (icv != null)
            {
                icv.CurrentChanged -= new EventHandler(OnCurrentChanged);
            }

            icv = newValue as ICollectionView;
            if (icv != null)
            {
                icv.CurrentChanged += new EventHandler(OnCurrentChanged);
            }

            SetSynchronizationWithCurrentItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is SelectorItem container)
            {
                container.ParentSelector = this;
                container.IsSelected = _selectedItems.Contains(NewItemInfo(item, element));
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (element is SelectorItem container)
            {
                container.ParentSelector = null;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            CoerceValue(SelectedIndexProperty);
            CoerceValue(SelectedItemProperty);

            if (SelectedValueWaitsForItems &&
                !Object.Equals(SelectedValue, InternalSelectedValue))
            {
                // This sets the selection from SelectedValue when SelectedValue
                // was set prior to the arrival of any items to select, provided
                // that SelectedIndex or SelectedItem didn't already do it.
                SelectItemWithValue(SelectedValue, selectNow: true);
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");

                    SelectionChange.Begin();

                    try
                    {
                        ItemInfo info = NewItemInfo(e.NewItems[0], null, e.NewStartingIndex);
                        // If we added something, see if it was set be selected and sync.
                        if (InfoGetIsSelected(info))
                        {
                            SelectionChange.Select(info, true /* assumeInItemsCollection */);
                        }
                    }
                    finally
                    {
                        SelectionChange.End();
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");

                    RemoveFromSelection(e);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                        throw new NotSupportedException("Range actions are not supported.");

                    // RemoveFromSelection works, with one wrinkle.  If the
                    // replaced item was selected, the old item is in _selectedItems,
                    // but its container now holds the new item.  The Remove code will
                    // update _selectedItems correctly, except for the step that
                    // sets container.IsSelected=false.   We do that here as a special case.
                    ItemSetIsSelected(ItemInfoFromIndex(e.NewStartingIndex), false);
                    RemoveFromSelection(e);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // catastrophic update -- need to resynchronize everything.

                    // If we remove all the items we clear the deferred selection
                    if (Items.CountInternal == 0)
                        SelectionChange.CleanupDeferSelection();

                    SelectionChange.Begin();
                    try
                    {
                        // Find where previously selected items have moved to
                        LocateSelectedItems(deselectMissingItems: true);

                        // Select everything in Items that is selected but isn't in the _selectedItems.
                        if (ItemsSource == null)
                        {
                            for (int i = 0; i < Items.Count; i++)
                            {
                                ItemInfo info = ItemInfoFromIndex(i);

                                // This only works for items that know they're selected:
                                // items that are UI elements or items that have had their UI generated.
                                if (InfoGetIsSelected(info))
                                {
                                    if (!_selectedItems.Contains(info))
                                    {
                                        SelectionChange.Select(info, true /* assumeInItemsCollection */);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        SelectionChange.End();
                    }

                    break;

                default:
                    throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }
        }

        internal override void AdjustItemInfoOverride(NotifyCollectionChangedEventArgs e)
        {
            AdjustItemInfos(e, _selectedItems);
            base.AdjustItemInfoOverride(e);
        }

        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }

        internal SelectionChanger SelectionChange { get; }

        internal ObservableCollection<object> SelectedItemsImpl { get; }

        internal InternalSelectedItemsStorage SelectedItemsInternal
        {
            get { return _selectedItems; }
        }

        internal bool CanSelectMultiple
        {
            get { return _canSelectMultiple; }
            set
            {
                if (_canSelectMultiple != value)
                {
                    _canSelectMultiple = value;
                    if (!value && _selectedItems.Count > 1)
                    {
                        SelectionChange.Validate();
                    }
                }
            }
        }

        // Gets the selected item but doesn't use SelectedItem (avoids putting it "in use")
        internal object InternalSelectedItem
        {
            get
            {
                return (_selectedItems.Count == 0) ? null : _selectedItems[0].Item;
            }
        }

        /// <summary>
        /// Index of the first item in SelectedItems or (-1) if SelectedItems is empty.
        /// </summary>
        /// <value></value>
        internal int InternalSelectedIndex
        {
            get
            {
                if (_selectedItems.Count == 0)
                    return -1;

                int index = _selectedItems[0].Index;
                if (index < 0)
                {
                    index = Items.IndexOf(_selectedItems[0].Item);
                    _selectedItems[0].Index = index;
                }

                return index;
            }
        }

        private object InternalSelectedValue
        {
            get
            {
                object item = InternalSelectedItem;
                object selectedValue;

                if (item != null)
                {
                    selectedValue = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, SelectedValuePath);
                }
                else
                {
                    selectedValue = DependencyProperty.UnsetValue;
                }

                return selectedValue;
            }
        }

        // called by SelectedItemsCollection after every change event
        internal void FinishSelectedItemsChange()
        {
            // if we've deferred an inner change, do it now
            ChangeInfo changeInfo = _changeInfo;
            if (changeInfo != null)
            {
                // make sure the selection change is active
                bool inSelectionChange = SelectionChange.IsActive;

                if (!inSelectionChange)
                {
                    SelectionChange.Begin();
                }

                UpdateSelectedItems(changeInfo.ToAdd, changeInfo.ToRemove);

                if (!inSelectionChange)
                {
                    SelectionChange.End();
                }
            }
        }

        private void UpdateSelectedItems(InternalSelectedItemsStorage toAdd, InternalSelectedItemsStorage toRemove)
        {
            Debug.Assert(SelectionChange.IsActive, "SelectionChange.IsActive should be true");
            IList userSelectedItems = SelectedItemsImpl;

            _changeInfo = null;

            // Do the adds first, to avoid a transient empty state
            for (int i = 0; i < toAdd.Count; ++i)
            {
                userSelectedItems.Add(toAdd[i].Item);
            }

            // Now do the removals in reverse order, so that the indices we saved are valid
            for (int i = toRemove.Count - 1; i >= 0; --i)
            {
                userSelectedItems.RemoveAt(~toRemove[i].Index);
            }
        }

        private void UpdateSelectedItems()
        {
            // Update SelectedItems.  We don't want to invalidate the property
            // because that defeats the ability of bindings to be able to listen
            // for collection changes on that collection.  Instead we just want
            // to add all the items which are not already in the collection.

            // Note: This is currently only called from SelectionChanger where SC.IsActive will be true.
            // If this is ever called from another location, ensure that SC.IsActive is true.
            Debug.Assert(SelectionChange.IsActive, "SelectionChange.IsActive should be true");

            SelectedItemCollection userSelectedItems = (SelectedItemCollection)SelectedItemsImpl;
            if (userSelectedItems != null)
            {
                InternalSelectedItemsStorage toAdd = new InternalSelectedItemsStorage(0, MatchExplicitEqualityComparer);
                InternalSelectedItemsStorage toRemove = new InternalSelectedItemsStorage(userSelectedItems.Count, MatchExplicitEqualityComparer);
                toAdd.UsesItemHashCodes = _selectedItems.UsesItemHashCodes;
                toRemove.UsesItemHashCodes = _selectedItems.UsesItemHashCodes;

                // copy the current SelectedItems list into a fast table, attaching
                // the 1's-complement of the index to each item.  The sentinel
                // container ensures that these are treated as separate items
                for (int i = 0; i < userSelectedItems.Count; ++i)
                {
                    toRemove.Add(userSelectedItems[i], ItemInfo.SentinelContainer, ~i);
                }

                // for each entry in _selectedItems, see if it's already in SelectedItems
                using (toRemove.DeferRemove())
                {
                    ItemInfo itemInfo = new ItemInfo(null, null, -1);
                    foreach (ItemInfo e in _selectedItems)
                    {
                        itemInfo.Reset(e.Item);
                        if (toRemove.Contains(itemInfo))
                        {
                            // already present - don't remove it
                            toRemove.Remove(itemInfo);
                        }
                        else
                        {
                            // not present - mark it to be added
                            toAdd.Add(e);
                        }
                    }
                }

                // Now make the changes, if any
                if (toAdd.Count > 0 || toRemove.Count > 0)
                {
                    // if SelectedItems is in the midst of an app-initiated change,
                    // wait for the outer change to finish, then make the inner change.
                    // Otherwise, do it now.
                    if (userSelectedItems.IsChanging)
                    {
                        _changeInfo = new ChangeInfo(toAdd, toRemove);
                    }
                    else
                    {
                        UpdateSelectedItems(toAdd, toRemove);
                    }
                }
            }
        }

        private void UpdatePublicSelectionProperties()
        {
            int selectedIndex = SelectedIndex;
            if ((selectedIndex > Items.Count - 1)
                || (selectedIndex == -1 && _selectedItems.Count > 0)
                || (selectedIndex > -1
                    && (_selectedItems.Count == 0 || selectedIndex != _selectedItems[0].Index)))
            {
                SetCurrentValue(SelectedIndexProperty, InternalSelectedIndex);
            }

            if (SelectedItem != InternalSelectedItem)
            {
                try
                {
                    // We know that InternalSelectedItem is a correct value for SelectedItemProperty and
                    // should skip the coerce callback because it is expensive to call IndexOf and Contains
                    SkipCoerceSelectedItemCheck = true;
                    SetCurrentValue(SelectedItemProperty, InternalSelectedItem);
                }
                finally
                {
                    SkipCoerceSelectedItemCheck = false;
                }
            }

            if (_selectedItems.Count > 0)
            {
                // an item has been selected, so turn off the delayed
                // selection by SelectedValue (bug 452619)
                SelectedValueWaitsForItems = false;
            }

            if (!SelectedValueDrivesSelection && !SelectedValueWaitsForItems)
            {
                object desiredSelectedValue = InternalSelectedValue;
                if (desiredSelectedValue == DependencyProperty.UnsetValue)
                {
                    desiredSelectedValue = null;
                }

                if (!Equals(SelectedValue, desiredSelectedValue))
                {
                    SetCurrentValue(SelectedValueProperty, desiredSelectedValue);
                }
            }

            UpdateSelectedItems();
        }

        /// <summary>
        /// Select all items in the collection.
        /// Assumes that CanSelectMultiple is true
        /// </summary>
        internal virtual void SelectAllImpl()
        {
            Debug.Assert(CanSelectMultiple, "CanSelectMultiple should be true when calling SelectAllImpl");

            SelectionChange.Begin();
            SelectionChange.CleanupDeferSelection();
            try
            {
                int index = 0;
                foreach (object current in Items)
                {
                    ItemInfo info = NewItemInfo(current, null, index++);
                    SelectionChange.Select(info, true /* assumeInItemsCollection */);
                }
            }
            finally
            {
                SelectionChange.End();
            }
        }

        /// <summary>
        /// Raise the SelectionChanged event.
        /// </summary>
        private void InvokeSelectionChanged(List<ItemInfo> unselectedInfos, List<ItemInfo> selectedInfos)
        {
            SelectionChangedEventArgs selectionChanged = new SelectionChangedEventArgs(unselectedInfos, selectedInfos);

            selectionChanged.OriginalSource = this;

            OnSelectionChanged(selectionChanged);
        }

        private static bool ItemGetIsSelectable(object item)
        {
            return item != null;
        }

        /// <summary>
        /// Called by handlers of Selected/Unselected or CheckedChanged events to indicate that the selection state
        /// on the item has changed and selector needs to update accordingly.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="selected"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        internal void NotifyIsSelectedChanged(FrameworkElement container, bool selected)
        {
            // The selectionchanged event will fire at the end of the selection change.
            // We are here because this change was requested within the SelectionChange.
            // If there isn't a selection change going on now, we should do a SelectionChange.
            if (SelectionChange.IsActive)
            {
                return;
            }
                
            if (container != null)
            {
                object item = GetItemOrContainerFromContainer(container);
                if (item != DependencyProperty.UnsetValue)
                {
                    SetSelectedHelper(item, container, selected);
                }
            }
        }

        /// <summary>
        /// Adds/Removes the given item to the collection.  Assumes the item is in the collection.
        /// </summary>
        private void SetSelectedHelper(object item, FrameworkElement UI, bool selected)
        {
            Debug.Assert(!SelectionChange.IsActive, "SelectionChange is already active -- use SelectionChange.Select or Unselect");

            bool selectable;

            selectable = ItemGetIsSelectable(item);

            if (selectable == false && selected)
            {
                throw new InvalidOperationException("Item is not selectable.");
            }

            SelectionChange.Begin();
            try
            {
                ItemInfo info = NewItemInfo(item, UI);

                if (selected)
                {
                    SelectionChange.Select(info, true /* assumeInItemsCollection */);
                }
                else
                {
                    SelectionChange.Unselect(info);
                }
            }
            finally
            {
                SelectionChange.End();
            }
        }

        private void ItemSetIsSelected(ItemInfo info, bool value)
        {
            if (info == null)
                return;

            DependencyObject container = info.Container;

            if (container != null && container != ItemInfo.RemovedContainer)
            {
                // First check that the value is different and then set it.
                if (container is SelectorItem selectorContainer)
                {
                    if (selectorContainer.IsSelected != value)
                    {
                        container.SetCurrentValue(SelectorItem.IsSelectedProperty, value);
                    }
                }
            }
            else
            {
                // In the case where the elements added *are* the containers, set it on the item instead of doing nothing
                // once we are able to force generation, we shouldn't have to do this
                object item = info.Item;
                if (IsItemItsOwnContainerOverride(item))
                {
                    DependencyObject element = item as DependencyObject;

                    if (element != null)
                    {
                        if (element is SelectorItem selectorContainer)
                        {
                            if (selectorContainer.IsSelected != value)
                            {
                                element.SetCurrentValue(SelectorItem.IsSelectedProperty, value);
                            }
                        }
                    }
                }
            }
        }

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                AdjustNewContainers();
            }
        }

        private void AdjustNewContainers()
        {
            AdjustItemInfosAfterGeneratorChangeOverride();

            if (base.HasItems)
            {
                SelectionChange.Begin();
                try
                {
                    // Things could have been added to _selectedItems before the containers were generated, so now push
                    // the IsSelected state down onto those items.
                    for (int i = 0; i < _selectedItems.Count; i++)
                    {
                        // This could send messages back from the children, but we will ignore them b/c the selectionchange is active.
                        ItemSetIsSelected(_selectedItems[i], true);
                    }
                }
                finally
                {
                    SelectionChange.Cancel();
                }
            }
        }

        internal virtual void AdjustItemInfosAfterGeneratorChangeOverride()
        {
            AdjustItemInfosAfterGeneratorChange(_selectedItems, claimUniqueContainer: true);
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectionChange.IsActive)
            {
                return;
            }

            if (!CanSelectMultiple)
            {
                throw new InvalidOperationException("Can only change SelectedItems collection in multiple selection modes. Use SelectedItem in single select modes.");
            }

            SelectionChange.Begin();
            bool succeeded = false;

            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems.Count != 1)
                            throw new NotSupportedException("Range actions are not supported.");

                        SelectionChange.Select(NewUnresolvedItemInfo(e.NewItems[0]), false /* assumeInItemsCollection */);
                        break;
                    
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems.Count != 1)
                            throw new NotSupportedException("Range actions are not supported.");

                        SelectionChange.Unselect(NewUnresolvedItemInfo(e.OldItems[0]));
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        SelectionChange.CleanupDeferSelection();
                        for (int i = 0; i < _selectedItems.Count; i++)
                        {
                            SelectionChange.Unselect(_selectedItems[i]);
                        }

                        ObservableCollection<object> userSelectedItems = (ObservableCollection<object>)sender;

                        for (int i = 0; i < userSelectedItems.Count; i++)
                        {
                            SelectionChange.Select(NewUnresolvedItemInfo(userSelectedItems[i]), false /* assumeInItemsCollection */);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                            throw new NotSupportedException("Range actions are not supported.");

                        SelectionChange.Unselect(NewUnresolvedItemInfo(e.OldItems[0]));
                        SelectionChange.Select(NewUnresolvedItemInfo(e.NewItems[0]), false /* assumeInItemsCollection */);
                        break;

                    case NotifyCollectionChangedAction.Move:
                        break; // order within SelectedItems doesn't matter

                    default:
                        throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
                }

                SelectionChange.End();
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    SelectionChange.Cancel();
                }
            }
        }

        private object SelectItemWithValue(object value, bool selectNow)
        {
            object item;
            if (base.HasItems)
            {
                int index;
                item = FindItemWithValue(value, out index);

                ItemInfo info = NewItemInfo(item, null, index);

                if (selectNow)
                {
                    try
                    {
                        SelectedValueDrivesSelection = true;
                        // We can assume it's in the collection because we just searched
                        // through the collection to find it.
                        SelectionChange.SelectJustThisItem(info, assumeInItemsCollection: true);
                    }
                    finally
                    {
                        SelectedValueDrivesSelection = false;
                    }
                }
                else
                {
                    // when called during coercion, don't actually select until
                    // OnSelectedValueChanged, so that the new SelectedValue is
                    // fully set before raising the SelectedChanged event
                    //PendingSelectionByValueField.SetValue(this, info);
                    PendingSelectionByValue = info;
                }
            }
            else
            {
                // if there are no items, protect SelectedValue from being overwritten
                // until items show up.  This enables a SelectedValue set from markup
                // to set the initial selection when the items eventually appear.
                item = DependencyProperty.UnsetValue;
                SelectedValueWaitsForItems = true;
            }

            return item;
        }

        private object FindItemWithValue(object value, out int index)
        {
            index = -1;

            if (!base.HasItems)
            {
                return DependencyProperty.UnsetValue;
            }

            string selectedValuePath = this.SelectedValuePath;

            // optimize for case where there is no SelectedValuePath
            if (string.IsNullOrEmpty(selectedValuePath))
            {
                index = this.Items.IndexOf(value);
                if (index >= 0)
                {
                    return value;
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            index = 0;
            foreach (object item in this.Items)
            {
                object displayedItem = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, selectedValuePath);
                if (EqualsEx(displayedItem, value))
                {
                    return item;
                }
                ++index;
            }

            index = -1;
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Returns true if FrameworkElement (container) representing the item
        /// has Selector.IsSelectedProperty set to true.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool InfoGetIsSelected(ItemInfo info)
        {
            DependencyObject container = info.Container;
            if (container != null)
            {
                return (bool)container.GetValue(SelectorItem.IsSelectedProperty);
            }

            // In the case where the elements added *are* the containers, read it off the item could work too
            // once we are able to force generation, we shouldn't have to do this
            if (IsItemItsOwnContainerOverride(info.Item))
            {
                DependencyObject element = info.Item as DependencyObject;

                if (element != null)
                {
                    //return (bool)element.GetValue(Selector.IsSelectedProperty);
                    return (bool)element.GetValue(SelectorItem.IsSelectedProperty);
                }
            }

            return false;
        }

        private void RemoveFromSelection(NotifyCollectionChangedEventArgs e)
        {
            SelectionChange.Begin();
            try
            {
                // if they removed something in a selection, remove it.
                // When End() commits the changes it will update SelectedIndex.
                ItemInfo info = NewItemInfo(e.OldItems[0], ItemInfo.SentinelContainer, e.OldStartingIndex);

                // normally info.Container is reset to null (see ItemInfo.Refresh), but
                // not if the collection didn't tell us the position of the removed
                // item.  Adjust for that now, so that we don't attempt to set
                // properties on the SentinelContainer
                info.Container = null;

                if (_selectedItems.Contains(info))
                {
                    SelectionChange.Unselect(info);
                }
            }
            finally
            {
                // Here SelectedIndex will be fixed to point to the first thing in _selectedItems, so
                // the case of removing something before SelectedIndex is taken care of.
                SelectionChange.End();
            }
        }

        // Locate the selected items - i.e. assign an index to each ItemInfo in _selectedItems.
        // (This is called after a Reset event from the Items collection.)
        // If the caller provides a list, fill it with ranges describing the selection;
        // each range has the form <offset, length>.
        // Optionally remove from _selectedItems any entry for which no index can be found
        internal void LocateSelectedItems(List<Tuple<int, int>> ranges = null, bool deselectMissingItems = false)
        {
            List<int> knownIndices = new List<int>(_selectedItems.Count);
            int unknownCount = 0;
            int knownCount;

            // Step 1.  Find the known indices.
            foreach (ItemInfo info in _selectedItems)
            {
                if (info.Index < 0)
                {
                    ++unknownCount;
                }
                else
                {
                    knownIndices.Add(info.Index);
                }
            }

            // sort the list, and remember its size.   We'll be adding more to the
            // list, but we only need to search up to its current size.
            knownCount = knownIndices.Count;
            knownIndices.Sort();

            // Step 2. Walk through the Items collection, to fill in the unknown indices.
            ItemInfo key = new ItemInfo(null, ItemInfo.KeyContainer, -1);
            for (int i = 0; unknownCount > 0 && i < Items.Count; ++i)
            {
                // skip items whose index is already known
                if (knownIndices.BinarySearch(0, knownCount, i, null) >= 0)
                {
                    continue;
                }

                // see if the current item appears in _selectedItems
                key.Reset(Items[i]);
                key.Index = i;
                ItemInfo info = _selectedItems.FindMatch(key);

                if (info != null)
                {
                    // record the match
                    info.Index = i;
                    knownIndices.Add(i);
                    --unknownCount;
                }
            }

            // Step 3. Report the selection as a list of ranges
            if (ranges != null)
            {
                ranges.Clear();
                knownIndices.Sort();
                knownIndices.Add(-1);   // sentinel, to emit the last range
                int startRange = -1, endRange = -2;

                foreach (int index in knownIndices)
                {
                    if (index == endRange + 1)
                    {
                        // extend the current range
                        endRange = index;
                    }
                    else
                    {
                        // emit the current range
                        if (startRange >= 0)
                        {
                            ranges.Add(new Tuple<int, int>(startRange, endRange - startRange + 1));
                        }

                        // start a new range
                        startRange = endRange = index;
                    }
                }
            }

            // Step 4.  Remove missing items from _selectedItems
            if (deselectMissingItems)
            {
                // Note: This is currently only called from SelectionChanger where SC.IsActive will be true.
                // If this is ever called from another location, ensure that SC.IsActive is true.
                Debug.Assert(SelectionChange.IsActive, "SelectionChange.IsActive should be true");

                foreach (ItemInfo info in _selectedItems)
                {
                    if (info.Index < 0)
                    {
                        // we want to remove this ItemInfo from the selection,
                        // even if it refers to the same item as another selected ItemInfo
                        // (which can happen when SelectedItems contains duplicates).
                        // Thus we want it to compare as unequal to any ItemInfo
                        // except itself;  marking it Removed does exactly that.
                        info.Container = ItemInfo.RemovedContainer;
                        SelectionChange.Unselect(info);
                    }
                }
            }
        }

        private class ChangeInfo
        {
            public ChangeInfo(InternalSelectedItemsStorage toAdd, InternalSelectedItemsStorage toRemove)
            {
                ToAdd = toAdd;
                ToRemove = toRemove;
            }

            public InternalSelectedItemsStorage ToAdd { get; private set; }
            public InternalSelectedItemsStorage ToRemove { get; private set; }
        }

        #region Obsolete

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void ApplySelectedIndex(int index) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void ManageSelectedIndex_Changed(DependencyPropertyChangedEventArgs e) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnSelectedItemChanged(object selectedItem) { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void NotifyItemMouseEnter(SelectorItem item) { }

        /// <summary>
        /// Gets or sets the bakground color of the selected Items.
        /// </summary>
        [Obsolete]
        public Brush SelectedItemBackground
        {
            get { return (Brush)this.GetValue(Selector.SelectedItemBackgroundProperty); }
            set { this.SetValue(Selector.SelectedItemBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItemBackground dependency property
        /// </summary>
        [Obsolete]
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register(
                "SelectedItemBackground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString("#FFBADDE9"))));

        /// <summary>
        /// Gets or sets the foreground color of the selected Items.
        /// </summary>
        [Obsolete]
        public Brush SelectedItemForeground
        {
            get { return (Brush)this.GetValue(Selector.SelectedItemForegroundProperty); }
            set { this.SetValue(Selector.SelectedItemForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItemForeground dependency property
        /// </summary>
        [Obsolete]
        public static readonly DependencyProperty SelectedItemForegroundProperty =
            DependencyProperty.Register(
                "SelectedItemForeground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        [Obsolete]
        public Brush RowBackground
        {
            get { return (Brush)this.GetValue(Selector.RowBackgroundProperty); }
            set { this.SetValue(Selector.RowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the RowBackground dependency property
        /// </summary>
        [Obsolete]
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register(
                "RowBackground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));


        /// <summary>
        /// Gets or sets the foreground color of the Items that are not selected.
        /// </summary>
        [Obsolete]
        public Brush UnselectedItemForeground
        {
            get { return (Brush)this.GetValue(Selector.UnselectedItemForegroundProperty); }
            set { this.SetValue(Selector.UnselectedItemForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the UnselectedItemForeground dependency property
        /// </summary>
        [Obsolete]
        public static readonly DependencyProperty UnselectedItemForegroundProperty =
            DependencyProperty.Register(
                "UnselectedItemForeground",
                typeof(Brush),
                typeof(Selector),
                new PropertyMetadata((Brush)null));

        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        [Obsolete("Use RowBackground instead.")]
        public Brush UnselectedItemBackground
        {
            get { return this.RowBackground; }
            set { this.RowBackground = value; }
        }

        [Obsolete("Unused.")]
        protected bool ChangingSelectionProgrammatically { get; set; }

        #endregion Obsolete

        #region Search Logic
#if MIGRATION
        protected override void OnKeyDown(KeyEventArgs e)
        {
#else
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
#endif
            base.OnKeyDown(e);

            bool ctrl = GetCtrlKeyState();
            if (!ctrl)
            {
#if MIGRATION
                if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter ||
                    e.Key == Key.Escape || e.Key == Key.Tab)
#else
                if (e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete || e.Key == VirtualKey.Enter ||
                    e.Key == VirtualKey.Escape || e.Key == VirtualKey.Tab)
#endif

                {
                    _searchCriteria = string.Empty;
                }
                else
                {
                    string keyValue = !Char.IsControl((char)e.Key) ? e.Key.ToString() : string.Empty;

#if MIGRATION
                        if (e.Key >= Key.D0 && e.Key <= Key.D9)
                            keyValue = char.ToString((char)(e.Key - Key.D0 + '0'));

                        if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                            keyValue = char.ToString((char)(e.Key - Key.NumPad0 + '0'));
#else
                    if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9)
                            keyValue = char.ToString((char)(e.Key - VirtualKey.Number0 + '0'));

                    if (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9)
                        keyValue = char.ToString((char)(e.Key - VirtualKey.NumberPad0 + '0'));
#endif

                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        _searchCriteria += keyValue;
                        for (int i = 0; i < this.Items.Count; i++)
                        {
                            TypeConverterAttribute attr = (TypeConverterAttribute)Attribute.GetCustomAttribute(this.Items[i].GetType(), typeof(TypeConverterAttribute));

                            if (attr != null)
                            {
                                TypeConverter converter = (TypeConverter)Activator.CreateInstance(Type.GetType(attr.ConverterTypeName));
                                if (converter != null)
                                {
                                    if (converter.ConvertToString(this.Items[i]).StartsWith(_searchCriteria, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ScrollToFoundItem(i);
                                        return;
                                    }
                                }
                            }
                            if (this.Items[i].ToString().StartsWith(_searchCriteria, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ScrollToFoundItem(i);
                                return;
                            }
                        }
                        e.Handled = true;
                        ResetTimeout();
                    }
                }
            }
        }

        private void ScrollToFoundItem(int i)
        {
            string buffer = _searchCriteria;//to keep crietria when dropdownclosed on select
            this.SelectedIndex = i;
            this.ScrollIntoView(this.Items[i]);
            _searchCriteria = buffer;
            ResetTimeout();
        }

        public bool GetCtrlKeyState()
        {
            bool ctrl = false;
#if MIGRATION
            ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
#else
            ctrl = (Keyboard.Modifiers & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control;
#endif
            // The Apple key on a Mac is supposed to behave like the CTRL key on a PC for 
            // things like multi-select, select-all, and grid navigation.  To allow for this,
            // we set the CTRL to true if the Apple key is pressed.
#if MIGRATION
            ctrl |= (Keyboard.Modifiers & ModifierKeys.Apple) == ModifierKeys.Apple;
#else
            ctrl |= (Keyboard.Modifiers & VirtualKeyModifiers.Apple) == VirtualKeyModifiers.Apple;
#endif
            return ctrl;
        }

        private void ResetTimeout()
        {
            if (_searchTimeoutTimer == null)
            {
                _searchTimeoutTimer = new DispatcherTimer();
#if MIGRATION
                _searchTimeoutTimer.Tick += new EventHandler(OnTimeout);
#else
                _searchTimeoutTimer.Tick += new EventHandler<object>(OnTimeout);
#endif
            }
            else
            {
                _searchTimeoutTimer.Stop();
            }

            _searchTimeoutTimer.Interval = _searchCriteriaTimeOut;
            _searchTimeoutTimer.Start();
        }
#if MIGRATION
        private void OnTimeout(object sender, EventArgs e)
#else
        private void OnTimeout(object sender, object e)
#endif
        {
            ResetState();
        }

        private void ResetState()
        {
            _searchCriteria = string.Empty;

            if (_searchTimeoutTimer != null)
            {
                _searchTimeoutTimer.Stop();
            }
            _searchTimeoutTimer = null;
        }
#endregion
    }
}