
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
    private readonly HashSet<WeakReference<TimelineClock>> _clocks = new();
    private readonly List<WeakReference<TimelineClock>> _deadClocks = new();

    public int Count => _clocks.Count;

    public void Add(TimelineClock clock)
    {
        Debug.Assert(clock is not null);
        _clocks.Add(clock.WeakReference);
    }

    public bool Remove(TimelineClock clock)
    {
        Debug.Assert(clock is not null);
        return InternalRemove(clock.WeakReference);
    }

    public Enumerator GetEnumerator() => new(this);

    // Remove dead references
    internal void Purge()
    {
        foreach (WeakReference<TimelineClock> weakReference in _deadClocks)
        {
            InternalRemove(weakReference);
        }

        _deadClocks.Clear();
    }

    private bool InternalRemove(WeakReference<TimelineClock> weakReference) => _clocks.Remove(weakReference);

    public struct Enumerator : IEnumerator<TimelineClock>
    {
        private readonly ClockCollection _collection;
        private HashSet<WeakReference<TimelineClock>>.Enumerator _enumerator;

        public Enumerator(ClockCollection collection)
        {
            Debug.Assert(collection is not null);
            _collection = collection;
            _enumerator = collection._clocks.GetEnumerator();
            Current = null;
        }

        public TimelineClock Current { get; private set; }

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                WeakReference<TimelineClock> weakReference = _enumerator.Current;
                if (!weakReference.TryGetTarget(out TimelineClock clock))
                {
                    _collection._deadClocks.Add(weakReference);
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
