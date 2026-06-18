
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

using System.Collections.Concurrent;
using System.Threading;

namespace OpenSilver.Internal;

internal sealed class SynchronyzedStore<T>
{
    private readonly ConcurrentDictionary<int, T> _items = [];
    private int _slot = -1;

    public int Add(T item)
    {
        int slot = Interlocked.Increment(ref _slot);
        _items.TryAdd(slot, item);
        return slot;
    }

    public void Clean(int index) => _items.TryRemove(index, out _);

    public T Get(int index) => _items.TryGetValue(index, out T item) ? item : default;
}
