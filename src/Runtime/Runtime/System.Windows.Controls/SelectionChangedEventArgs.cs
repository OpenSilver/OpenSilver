
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
using System.Collections.Generic;

namespace System.Windows.Controls;

/// <summary>
/// Provides data for the <see cref="Primitives.Selector.SelectionChanged"/> event.
/// </summary>
public class SelectionChangedEventArgs : RoutedEventArgs
{
    private readonly object[] _addedItems;
    private readonly object[] _removedItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="removedItems">
    /// The items that were unselected.
    /// </param>
    /// <param name="addedItems">
    /// The items that were selected.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="removedItems"/> or <paramref name="addedItems"/> is null.
    /// </exception>
    public SelectionChangedEventArgs(IList removedItems, IList addedItems)
    {
        if (removedItems is null)
        {
            throw new ArgumentNullException(nameof(removedItems));
        }

        if (addedItems is null)
        {
            throw new ArgumentNullException(nameof(addedItems));
        }

        if (removedItems.Count == 0)
        {
            _removedItems = [];
        }
        else
        {
            _removedItems = new object[removedItems.Count];
            removedItems.CopyTo(_removedItems, 0);
        }

        if (addedItems.Count == 0)
        {
            _addedItems = [];
        }
        else
        {
            _addedItems = new object[addedItems.Count];
            addedItems.CopyTo(_addedItems, 0);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="removedItem">
    /// The item that was unselected.
    /// </param>
    /// <param name="addedItem">
    /// The item that was selected.
    /// </param>
    public SelectionChangedEventArgs(object removedItem, object addedItem)
    {
        _removedItems = removedItem is null ? [] : [removedItem];
        _addedItems = addedItem is null ? [] : [addedItem];
    }

    internal SelectionChangedEventArgs(List<ItemsControl.ItemInfo> unselectedInfos, List<ItemsControl.ItemInfo> selectedInfos)
    {
        if (unselectedInfos.Count == 0)
        {
            _removedItems = [];
        }
        else
        {
            _removedItems = new object[unselectedInfos.Count];
            for (int i = 0; i < unselectedInfos.Count; ++i)
            {
                _removedItems[i] = unselectedInfos[i].Item;
            }
        }

        if (selectedInfos.Count == 0)
        {
            _addedItems = [];
        }
        else
        {
            _addedItems = new object[selectedInfos.Count];
            for (int i = 0; i < selectedInfos.Count; ++i)
            {
                _addedItems[i] = selectedInfos[i].Item;
            }
        }
    }

    /// <summary>
    /// Gets a list that contains the items that were selected.
    /// </summary>
    /// <returns>
    /// The items that were selected in this event.
    /// </returns>
    public IList RemovedItems => _removedItems;

    /// <summary>
    /// Gets a list that contains the items that were unselected.
    /// </summary>
    /// <returns>
    /// The items that were unselected in this event.
    /// </returns>
    public IList AddedItems => _addedItems;
}