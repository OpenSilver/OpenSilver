
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

using System.Collections.Generic;

namespace OpenSilver.Internal;

internal sealed class SynchronyzedStore<T>
{
    private readonly object _lock = new();
    private readonly Dictionary<int, T> _items;
    private int _slot;

    public SynchronyzedStore()
        : this(8192)
    {
    }

    public SynchronyzedStore(int initialCapacity)
    {
        _items = new Dictionary<int, T>(initialCapacity);
    }

    public int Add(T item)
    {
        lock (_lock)
        {
            int slot = _slot++;
            _items.Add(slot, item);
            return slot;
        }
    }

    public void Clean(int index)
    {
        lock (_lock)
        {
            _items.Remove(index);
        }
    }

    public T Get(int index) => _items.TryGetValue(index, out T value) ? value : default;
}
