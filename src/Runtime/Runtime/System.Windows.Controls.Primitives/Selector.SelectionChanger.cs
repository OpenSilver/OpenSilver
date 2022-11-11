
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
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    public partial class Selector
    {
        /// <summary>
        /// Helper class for selection change batching.
        /// </summary>
        internal sealed class SelectionChanger
        {
            /// <summary>
            /// Create a new SelectionChangeHelper -- there should only be one instance per Selector.
            /// </summary>
            /// <param name="s"></param>
            internal SelectionChanger(Selector s)
            {
                _owner = s;
                _active = false;
                _toSelect = new InternalSelectedItemsStorage(1, MatchUnresolvedEqualityComparer);
                _toUnselect = new InternalSelectedItemsStorage(1, MatchUnresolvedEqualityComparer);
                _toDeferSelect = new InternalSelectedItemsStorage(1, MatchUnresolvedEqualityComparer);
            }

            /// <summary>
            /// True if there is a SelectionChange currently in progress.
            /// </summary>
            internal bool IsActive
            {
                get { return _active; }
            }

            /// <summary>
            /// Begin tracking selection changes.
            /// </summary>
            internal void Begin()
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(!_active, "Cannot begin a new SelectionChange when another one is active.");

                _active = true;
                _toSelect.Clear();
                _toUnselect.Clear();
            }

            /// <summary>
            /// Commit selection changes.
            /// </summary>
            internal void End()
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_active, "There must be a selection change active when you call SelectionChange.End()");

                List<ItemInfo> unselected = new List<ItemInfo>();
                List<ItemInfo> selected = new List<ItemInfo>();

                // We might have been asked to make changes that will put us in an invalid state.  Correct for this.
                try
                {
                    ApplyCanSelectMultiple();

                    CreateDeltaSelectionChange(unselected, selected);

                    _owner.UpdatePublicSelectionProperties();
                }
                finally
                {
                    // End the selection change -- IsActive will be false after this
                    Cleanup();
                }

                // only raise the event if there were actually any changes applied
                if (unselected.Count > 0 || selected.Count > 0)
                {
                    // see bug 1459509: update Current AFTER selection change and before raising event
                    if (_owner.IsSynchronizedWithCurrentItemPrivate)
                        _owner.SetCurrentToSelected();
                    _owner.InvokeSelectionChanged(unselected, selected);
                }
            }

            private void ApplyCanSelectMultiple()
            {
                if (!_owner.CanSelectMultiple)
                {
                    Debug.Assert(_toSelect.Count <= 1, "_toSelect.Count was > 1");

                    if (_toSelect.Count == 1) // this is all that should be selected, unselect _selectedItems
                    {
                        _toUnselect = new InternalSelectedItemsStorage(_owner._selectedItems);
                    }
                    else // _toSelect.Count == 0, and unselect all but one of _selectedItems
                    {
                        // This is when CanSelectMultiple changes from true to false.
                        if (_owner._selectedItems.Count > 1 && _owner._selectedItems.Count != _toUnselect.Count + 1)
                        {
                            // they didn't deselect enough; force deselection
                            ItemInfo selectedItem = _owner._selectedItems[0];

                            _toUnselect.Clear();
                            foreach (ItemInfo info in _owner._selectedItems)
                            {
                                if (info != selectedItem)
                                {
                                    _toUnselect.Add(info);
                                }
                            }
                        }
                    }
                }
            }

            private void CreateDeltaSelectionChange(List<ItemInfo> unselectedItems, List<ItemInfo> selectedItems)
            {
                for (int i = 0; i < _toDeferSelect.Count; i++)
                {
                    ItemInfo info = _toDeferSelect[i];
                    // If defered selected item exists in Items - move it to _toSelect
                    if (_owner.Items.Contains(info.Item))
                    {
                        _toSelect.Add(info);
                        _toDeferSelect.Remove(info);
                        i--;
                    }
                }

                if (_toUnselect.Count > 0 || _toSelect.Count > 0)
                {
                    // Step 1:  process the items to be unselected
                    // 1a:  handle the resolved items first.
                    using (_owner._selectedItems.DeferRemove())
                    {
                        if (_toUnselect.ResolvedCount > 0)
                        {
                            foreach (ItemInfo info in _toUnselect)
                            {
                                if (info.IsResolved)
                                {
                                    _owner.ItemSetIsSelected(info, false);
                                    if (_owner._selectedItems.Remove(info))
                                    {
                                        unselectedItems.Add(info);
                                    }
                                }
                            }
                        }

                        // 1b: handle unresolved items second, so they don't steal items
                        // from _selectedItems that belong to resolved items
                        if (_toUnselect.UnresolvedCount > 0)
                        {
                            foreach (ItemInfo info in _toUnselect)
                            {
                                if (!info.IsResolved)
                                {
                                    ItemInfo match = _owner._selectedItems.FindMatch(ItemInfo.Key(info));
                                    if (match != null)
                                    {
                                        _owner.ItemSetIsSelected(match, false);
                                        _owner._selectedItems.Remove(match);
                                        unselectedItems.Add(match);
                                    }
                                }
                            }
                        }
                    }

                    // Step 2:  process items to be selected
                    using (_toSelect.DeferRemove())
                    {
                        // 2a: handle the resolved items first
                        if (_toSelect.ResolvedCount > 0)
                        {
                            List<ItemInfo> toRemove = (_toSelect.UnresolvedCount > 0)
                                ? new List<ItemInfo>() : null;

                            foreach (ItemInfo info in _toSelect)
                            {
                                if (info.IsResolved)
                                {
                                    _owner.ItemSetIsSelected(info, true);
                                    if (!_owner._selectedItems.Contains(info))
                                    {
                                        _owner._selectedItems.Add(info);
                                        selectedItems.Add(info);
                                    }

                                    if (toRemove != null)
                                        toRemove.Add(info);
                                }
                            }

                            // remove the resolved items from _toSelect, so that
                            // it contains only unresolved items for step 2b
                            if (toRemove != null)
                            {
                                foreach (ItemInfo info in toRemove)
                                {
                                    _toSelect.Remove(info);
                                }
                            }
                        }

                        // 2b: handle unresolved items second, so they select different
                        // items than the ones belonging to resolved items
                        //
                        // At this point, _toSelect contains only unresolved items,
                        // each of which should be resolved to an item that is not
                        // already selected.  We do this by iterating through each
                        // item (from Items);  any item that matches something in
                        // _toSelect and is not already selected becomes selected.
                        for (int index = 0; _toSelect.UnresolvedCount > 0 && index < _owner.Items.Count; ++index)
                        {
                            ItemInfo info = _owner.NewItemInfo(_owner.Items[index], null, index);
                            ItemInfo key = new ItemInfo(info.Item, ItemInfo.KeyContainer, -1);
                            if (_toSelect.Contains(key) && !_owner._selectedItems.Contains(info))
                            {
                                _owner.ItemSetIsSelected(info, true);
                                _owner._selectedItems.Add(info);
                                selectedItems.Add(info);
                                _toSelect.Remove(key);
                            }
                        }

                        // after the loop, _toSelect may still contain leftover items.
                        // These are just abandoned;  they correspond to attempts to select
                        // (say) 5 instances of some item when Items only contains 3.
                    }
                }
            }

#if never
            private void SynchronizeSelectedIndexToSelectedItem()
            {
                if (_owner._selectedItems.Count == 0)
                {
                    _owner.SelectedIndex = -1;
                }
                else
                {
                    object selectedItem = _owner.SelectedItem;
                    object firstSelection = _owner._selectedItems[0];
 
                    // This check is only just to slightly improve perf by checking if it's in selected items before doing a reverse lookup
                    if (selectedItem == null || firstSelection != selectedItem)
                    {
                        _owner.SelectedIndex = _owner.Items.IndexOf(firstSelection);
                    }
                }
            }
#endif

            /// <summary>
            /// Queue something to be added to the selection.  Does nothing if the item is already selected.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="assumeInItemsCollection"></param>
            /// <returns>true if the Selection was queued</returns>
            internal bool Select(ItemInfo info, bool assumeInItemsCollection)
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_active, "No SelectionChange is active.");
                Debug.Assert(info != null, "parameter info should not be null");

                // Disallow selecting !IsSelectable things
                if (!ItemGetIsSelectable(info))
                    return false;

                // Disallow selecting things not in Items.FlatView
                if (!assumeInItemsCollection)
                {
                    if (!_owner.Items.Contains(info.Item))
                    {
                        // If user selected item is not in the Items yet - defer the selection
                        if (!_toDeferSelect.Contains(info))
                            _toDeferSelect.Add(info);
                        return false;
                    }
                }

                ItemInfo key = ItemInfo.Key(info);

                // To support Unselect(o) / Select(o) where o is already selected.
                if (_toUnselect.Remove(key))
                {
                    return true;
                }

                // Ignore if the item is already selected
                if (_owner._selectedItems.Contains(info))
                    return false;

                // Ignore if the item has already been requested to be selected.
                if (!key.IsKey && _toSelect.Contains(key))
                    return false;

                // enforce that we only select one thing in the CanSelectMultiple=false case.
                if (!_owner.CanSelectMultiple && _toSelect.Count > 0)
                {
                    // If it was the item telling us this, turn around and set IsSelected = false
                    // This will basically only happen in a Refresh situation where multiple items in the collection were selected but
                    // CanSelectMultiple = false.
                    foreach (ItemInfo item in _toSelect)
                    {
                        _owner.ItemSetIsSelected(item, false);
                    }
                    _toSelect.Clear();
                }

                _toSelect.Add(info);
                return true;
            }

            /// <summary>
            /// Queue something to be removed from the selection.  Does nothing if the item is not already selected.
            /// </summary>
            /// <param name="info"></param>
            /// <returns>true if the item was queued for unselection.</returns>
            internal bool Unselect(ItemInfo info)
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_active, "No SelectionChange is active.");
                Debug.Assert(info != null, "info should not be null");

                ItemInfo key = ItemInfo.Key(info);

                _toDeferSelect.Remove(info);

                // To support Select(o) / Unselect(o) where o is not already selected.
                if (_toSelect.Remove(key))
                {
                    return true;
                }

                // Ignore if the item is not already selected
                if (!_owner._selectedItems.Contains(key))
                    return false;

                // Ignore if the item has already been queued for unselection.
                if (_toUnselect.Contains(info))
                    return false;

                _toUnselect.Add(info);
                return true;
            }

            /// <summary>
            /// Makes sure that the current selection is valid; Performs a SelectionChange it if it's not.
            /// </summary>
            internal void Validate()
            {
                Begin();
                End();
            }

            /// <summary>
            /// Cancels the currently active SelectionChange.
            /// </summary>
            internal void Cancel()
            {
                Debug.Assert(_owner.CheckAccess());

                Cleanup();
            }

            internal void CleanupDeferSelection()
            {
                if (_toDeferSelect.Count > 0)
                {
                    _toDeferSelect.Clear();
                }
            }

            internal void Cleanup()
            {
                _active = false;
                if (_toSelect.Count > 0)
                {
                    _toSelect.Clear();
                }
                if (_toUnselect.Count > 0)
                {
                    _toUnselect.Clear();
                }
            }

            /// <summary>
            /// Select just this item; all other items in SelectedItems will be removed.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="assumeInItemsCollection"></param>
            internal void SelectJustThisItem(ItemInfo info, bool assumeInItemsCollection)
            {
                Begin();
                CleanupDeferSelection();

                try
                {
                    // was this item already in the selection?
                    bool isSelected = false;

                    // go backwards in case a selection is rejected; then they'll still have the same SelectedItem
                    for (int i = _owner._selectedItems.Count - 1; i >= 0; i--)
                    {
                        if (info != _owner._selectedItems[i])
                        {
                            Unselect(_owner._selectedItems[i]);
                        }
                        else
                        {
                            isSelected = true;
                        }
                    }

                    if (!isSelected && info != null && info.Item != DependencyProperty.UnsetValue)
                    {
                        Select(info, assumeInItemsCollection);
                    }
                }
                finally
                {
                    End();
                }
            }

            private Selector _owner;
            private InternalSelectedItemsStorage _toSelect;
            private InternalSelectedItemsStorage _toUnselect;
            private InternalSelectedItemsStorage _toDeferSelect; // Keep the items that cannot be selected because they are not in _owner.Items
            private bool _active;
        }

        private class ItemInfoEqualityComparer : IEqualityComparer<ItemInfo>
        {
            public ItemInfoEqualityComparer(bool matchUnresolved)
            {
                _matchUnresolved = matchUnresolved;
            }

            bool IEqualityComparer<ItemInfo>.Equals(ItemInfo x, ItemInfo y)
            {
                if (Object.ReferenceEquals(x, y))
                    return true;
                return (x == null) ? (y == null) : x.Equals(y, _matchUnresolved);
            }

            int IEqualityComparer<ItemInfo>.GetHashCode(ItemInfo x)
            {
                return x.GetHashCode();
            }

            bool _matchUnresolved;
        }

        private static readonly ItemInfoEqualityComparer MatchExplicitEqualityComparer = new ItemInfoEqualityComparer(false);
        private static readonly ItemInfoEqualityComparer MatchUnresolvedEqualityComparer = new ItemInfoEqualityComparer(true);
    }
}
