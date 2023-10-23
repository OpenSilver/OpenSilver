
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
using System.Diagnostics;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class ClockCollection
{
    private readonly HashSet<TimelineClock.ClockHandle> _handles = new();
    private readonly List<TimelineClock.ClockHandle> _deadHandles = new();

    public int Count => _handles.Count;

    public void Add(TimelineClock clock)
    {
        Debug.Assert(clock is not null);
        _handles.Add(clock.Handle);
    }

    public bool Remove(TimelineClock clock)
    {
        Debug.Assert(clock is not null);
        return InternalRemove(clock.Handle);
    }

    public Enumerator GetEnumerator() => new(this);

    // Remove dead references
    internal void Purge()
    {
        foreach (TimelineClock.ClockHandle handle in _deadHandles)
        {
            InternalRemove(handle);
        }

        _deadHandles.Clear();
    }

    private bool InternalRemove(TimelineClock.ClockHandle handle) => _handles.Remove(handle);

    public struct Enumerator : IEnumerator<TimelineClock>
    {
        private readonly ClockCollection _collection;
        private HashSet<TimelineClock.ClockHandle>.Enumerator _enumerator;

        public Enumerator(ClockCollection collection)
        {
            Debug.Assert(collection is not null);
            _collection = collection;
            _enumerator = collection._handles.GetEnumerator();
            Current = null;
        }

        public TimelineClock Current { get; private set; }

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                TimelineClock.ClockHandle handle = _enumerator.Current;
                if (!handle.TryGetTarget(out TimelineClock clock))
                {
                    _collection._deadHandles.Add(handle);
                    continue;
                }

                Current = clock;
                return true;
            }

            Current = null;
            return false;
        }

        readonly void IDisposable.Dispose() { }

        readonly object IEnumerator.Current => Current;

        readonly void IEnumerator.Reset() { }
    }
}
