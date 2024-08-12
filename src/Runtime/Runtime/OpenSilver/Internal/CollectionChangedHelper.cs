
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
using System.Collections.Specialized;

namespace OpenSilver.Internal;

internal sealed class CollectionChangedHelper
{
    private static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);

    private int _blockReentrancyCount;

    internal event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
    /// <exception cref="InvalidOperationException"> raised when changing the collection
    /// while another collection change is still being notified to other listeners </exception>
    internal void CheckReentrancy()
    {
        if (_blockReentrancyCount > 0)
        {
            // we can allow changes if there's only one listener - the problem
            // only arises if reentrant changes make the original event args
            // invalid for later listeners.  This keeps existing code working
            // (e.g. Selector.SelectedItems).
            if (CollectionChanged?.GetInvocationList().Length > 1)
            {
                throw new InvalidOperationException("Reentrancy not allowed");
            }
        }
    }

    internal void OnCollectionReset()
    {
        if (CollectionChanged is NotifyCollectionChangedEventHandler handler)
        {
            OnCollectionChangedCore(handler, ResetCollectionChanged);
        }
    }

    internal void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (CollectionChanged is NotifyCollectionChangedEventHandler handler)
        {
            OnCollectionChangedCore(handler, e);
        }
    }

    internal void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
    {
        if (CollectionChanged is NotifyCollectionChangedEventHandler handler)
        {
            OnCollectionChangedCore(handler, new NotifyCollectionChangedEventArgs(action, item, index));
        }
    }

    internal void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
    {
        if (CollectionChanged is NotifyCollectionChangedEventHandler handler)
        {
            OnCollectionChangedCore(handler, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }
    }

    private void OnCollectionChangedCore(NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventArgs e)
    {
        _blockReentrancyCount++;
        try
        {
            handler(this, e);
        }
        finally
        {
            _blockReentrancyCount--;
        }
    }
}
