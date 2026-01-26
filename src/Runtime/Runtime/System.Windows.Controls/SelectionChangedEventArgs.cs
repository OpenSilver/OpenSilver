
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
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace System.Windows.Controls;

/// <summary>
/// Provides data for the <see cref="Selector.SelectionChanged"/> event.
/// </summary>
public class SelectionChangedEventArgs : RoutedEventArgs
{
    private readonly object[] _addedItems;
    private readonly object[] _removedItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="id">
    /// The event identifier (ID).
    /// </param>
    /// <param name="removedItems">
    /// The items that were unselected.
    /// </param>
    /// <param name="addedItems">
    /// The items that were selected.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> or <paramref name="removedItems"/> or <paramref name="addedItems"/> is null.
    /// </exception>
    public SelectionChangedEventArgs(RoutedEvent id, IList removedItems, IList addedItems)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(removedItems);
        ArgumentNullException.ThrowIfNull(addedItems);

        RoutedEvent = id;

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
        : this(Selector.SelectionChangedEvent, removedItems, addedItems)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="id">
    /// The event identifier (ID).
    /// </param>
    /// <param name="removedItem">
    /// The item that was unselected.
    /// </param>
    /// <param name="addedItem">
    /// The item that was selected.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is null.
    /// </exception>
    public SelectionChangedEventArgs(RoutedEvent id, object removedItem, object addedItem)
    {
        ArgumentNullException.ThrowIfNull(id);

        RoutedEvent = id;
        _removedItems = removedItem is null ? [] : [removedItem];
        _addedItems = addedItem is null ? [] : [addedItem];
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
        : this(Selector.SelectionChangedEvent, removedItem, addedItem)
    {
    }

    internal SelectionChangedEventArgs(List<ItemsControl.ItemInfo> unselectedInfos, List<ItemsControl.ItemInfo> selectedInfos)
    {
        RoutedEvent = Selector.SelectionChangedEvent;

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

    /// <summary>
    /// Performs the proper type casting to call the type-safe <see cref="SelectionChangedEventHandler"/> 
    /// delegate for the <see cref="Selector.SelectionChanged"/> event.
    /// </summary>
    /// <param name="genericHandler">
    /// The handler to invoke.
    /// </param>
    /// <param name="genericTarget">
    /// The current object along the event's route.
    /// </param>
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        => ((SelectionChangedEventHandler)genericHandler)(genericTarget, this);
}