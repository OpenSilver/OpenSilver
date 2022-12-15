
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
using System.Collections.ObjectModel;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// This class represent the collection of SelectedItems in Selector. It extends the ObservableCollection by providing methods for bulk selection.
    /// </summary>
    internal sealed class SelectedItemCollection : ObservableCollection<object>
    {
#region Contructors
        /// <summary>
        /// Create a new SelectedItemCollection object which keeps a reference to the corresponding Selector
        /// </summary>
        /// <param name="selector"></param>
        public SelectedItemCollection(Selector selector)
        {
            _selector = selector;
            _changer = new Changer(this);
        }
#endregion

#region Protected Methods

        /// <summary>
        /// Clear all items from the selection. This method modifies the behavior of IList.Clear()
        /// </summary>
        protected override void ClearItems()
        {
            CheckReadOnly();
            if (_updatingSelectedItems)
            {
                foreach (ItemsControl.ItemInfo current in _selector.SelectedItemsInternal)
                {
                    _selector.SelectionChange.Unselect(current);
                }
            }
            else
            {
                using (ChangeSelectedItems())
                {
                    base.ClearItems();
                }
            }
        }

        /// <summary>
        /// Removes an item from the selection. This method modifies the behavior of IList.Remove() and IList.RemoveAt()
        /// </summary>
        protected override void RemoveItem(int index)
        {
            CheckReadOnly();
            if (_updatingSelectedItems)
            {
                _selector.SelectionChange.Unselect(_selector.NewItemInfo(this[index]));
            }
            else
            {
                using (ChangeSelectedItems())
                {
                    base.RemoveItem(index);
                }
            }
        }

        /// <summary>
        /// Inserts an item in the selection
        /// </summary>
        protected override void InsertItem(int index, object item)
        {
            CheckReadOnly();
            if (_updatingSelectedItems)
            {
                // For defered selection we should allow only Add method
                if (index == Count)
                {
                    _selector.SelectionChange.Select(_selector.NewItemInfo(item), true /* assumeInItemsCollection */);
                }
                else
                {
                    throw new InvalidOperationException("Cannot insert in a selection when selection is in progress. Use Add method.");
                }
            }
            else
            {
                using (ChangeSelectedItems())
                {
                    base.InsertItem(index, item);
                }
            }
        }

        /// <summary>
        /// Sets an item on specified index
        /// </summary>
        protected override void SetItem(int index, object item)
        {
            CheckReadOnly();
            if (_updatingSelectedItems)
            {
                throw new InvalidOperationException("Cannot set an item in a selection when selection is in progress. Use Add or Remove method.");
            }
            else
            {
                using (ChangeSelectedItems())
                {
                    base.SetItem(index, item);
                }
            }
        }

        /// <summary>
        /// Movea an item from one position to another
        /// </summary>
        /// <param name="oldIndex">index of the column which is being moved</param>
        /// <param name="newIndex">index of the column to be move to</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            CheckReadOnly();
            if (oldIndex != newIndex)
            {
                if (_updatingSelectedItems)
                {
                    throw new InvalidOperationException("Cannot move an item in a selection when selection is in progress.");
                }
                else
                {
                    using (ChangeSelectedItems())
                    {
                        base.MoveItem(oldIndex, newIndex);
                    }
                }
            }
        }

#endregion

#region Reentrant changes

        internal bool IsChanging { get { return (_changeCount > 0); } }

        private IDisposable ChangeSelectedItems()
        {
            ++_changeCount;
            return _changer;
        }

        private void FinishChange()
        {
            if (--_changeCount == 0)
            {
                _selector.FinishSelectedItemsChange();
            }
        }

        private class Changer : IDisposable
        {
            public Changer(SelectedItemCollection owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                _owner.FinishChange();
            }

            SelectedItemCollection _owner;
        }

        int _changeCount;
        Changer _changer;

#endregion Reentrant changes

#region MultiSelector methods

        /// <summary>
        /// Begin tracking selection changes. SelectedItems.Add/Remove will queue up the changes but not commit them until EndUpdateSelecteditems is called.
        /// </summary>
        internal void BeginUpdateSelectedItems()
        {
            if (_selector.SelectionChange.IsActive || _updatingSelectedItems)
            {
                throw new InvalidOperationException("Cannot begin a new selection while a selection is in progress.");
            }
            _updatingSelectedItems = true;
            _selector.SelectionChange.Begin();
        }

        /// <summary>
        /// Commit selection changes.
        /// </summary>
        internal void EndUpdateSelectedItems()
        {
            if (!_selector.SelectionChange.IsActive || !_updatingSelectedItems)
            {
                throw new InvalidOperationException("Cannot end a selection when no selection is in progress.");
            }
            _updatingSelectedItems = false;
            _selector.SelectionChange.End();
        }

        /// <summary>
        /// Returns true after BeginUpdateSelectedItems is called
        /// </summary>
        internal bool IsUpdatingSelectedItems
        {
            get
            {
                return _selector.SelectionChange.IsActive || _updatingSelectedItems;
            }
        }

        /// <summary>
        /// Add an ItemInfo to the deferred selection
        /// </summary>
        internal void Add(ItemsControl.ItemInfo info)
        {
            if (!_selector.SelectionChange.IsActive || !_updatingSelectedItems)
            {
                throw new InvalidOperationException("Cannot end a selection when no selection is in progress.");
            }

            _selector.SelectionChange.Select(info, true /* assumeInItemsCollection */);
        }

        /// <summary>
        /// Remove an ItemInfo from the deferred selection
        /// </summary>
        internal void Remove(ItemsControl.ItemInfo info)
        {
            if (!_selector.SelectionChange.IsActive || !_updatingSelectedItems)
            {
                throw new InvalidOperationException("Cannot end a selection when no selection is in progress.");
            }

            _selector.SelectionChange.Unselect(info);
        }

#endregion

#region Private data

        // Keep a reference for Selector owner
        private Selector _selector;

        // We need a flag for indicating user bulk selection mode. We cannot re-use SelectionChange.IsActive because there are cases when SelectionChange.IsActive==true and SelectedItems.Add is called internally (End()) to update the collection
        // When EndUpdateSelectedItems() is called we first reset this flag to allow SelectedItems.Add to change the collection
        private bool _updatingSelectedItems;
#endregion

        private void CheckReadOnly()
        {
            if (!_selector.CanSelectMultiple && !_selector.SelectionChange.IsActive)
            {
                throw new InvalidOperationException("Can only change SelectedItems collection in multiple selection modes. Use SelectedItem in single select modes.");
            }
        }
    }
}