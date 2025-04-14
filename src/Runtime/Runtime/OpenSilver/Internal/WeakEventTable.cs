
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace OpenSilver.Internal;

/// <summary>
/// This class manages the correspondence between event types and
/// event managers, in support of the "weak event listener" pattern.
/// It also stores data on behalf of the managers;  a manager can store
/// data of its own choosing, indexed by the pair (manager, source).
/// </summary>
internal sealed class WeakEventTable : DispatcherObject
{
    /// <summary>
    /// Create a new instance of WeakEventTable.
    /// </summary>
    private WeakEventTable() { }

    /// <summary>
    /// Return the WeakEventTable for the current thread
    /// </summary>
    internal static WeakEventTable CurrentWeakEventTable => _currentTable ??= new();

    /// <summary>
    /// Take a read-lock on the table, and return the IDisposable.
    /// Queries to the table should occur within a
    /// "using (Table.ReadLock) { ... }" clause, except for queries
    /// that are already within a write lock.
    /// </summary>
    internal IDisposable ReadLock => _lock.ReadLock;

    /// <summary>
    /// Take a write-lock on the table, and return the IDisposable.
    /// All modifications to the table should occur within a
    /// "using (Table.WriteLock) { ... }" clause.
    /// </summary>
    internal IDisposable WriteLock => _lock.WriteLock;

    /// <summary>
    /// Get or set the manager instance for the given type.
    /// </summary>
    internal WeakEventManager this[Type managerType]
    {
        get => _managerTable.TryGetValue(managerType, out WeakEventManager manager) ? manager : null;
        set => _managerTable[managerType] = value;
    }

    /// <summary>
    /// Get or set the manager instance for the given event.
    /// </summary>
    internal WeakEventManager this[Type eventSourceType, string eventName]
    {
        get => _eventNameTable.TryGetValue(new EventNameKey(eventSourceType, eventName), out WeakEventManager manager) ? manager : null;
        set => _eventNameTable[new EventNameKey(eventSourceType, eventName)] = value;
    }

    /// <summary>
    /// Get or set the data stored by the given manager for the given source.
    /// </summary>
    internal object this[WeakEventManager manager, object source]
    {
        get => _dataTable.TryGetValue(new EventKey(manager, source), out object result) ? result : null;
        set => _dataTable[new EventKey(manager, source, true)] = value;
    }

    /// <summary>
    /// Indicates whether cleanup is enabled.
    /// </summary>
    /// <remarks>
    /// Normally cleanup is always enabled, but a perf test environment might
    /// want to disable cleanup so that it doesn't interfere with the real
    /// perf measurements.
    /// </remarks>
    internal bool IsCleanupEnabled { get; set; } = true;

    /// <summary>
    /// Remove the data for the given manager and source.
    /// </summary>
    internal void Remove(WeakEventManager manager, object source)
    {
        var key = new EventKey(manager, source);
        if (!_inPurge)
        {
            _dataTable.Remove(key);
        }
        else
        {
            _toRemove.Add(key);
        }
    }

    /// <summary>
    /// Schedule a cleanup pass.  This can be called from any thread.
    /// </summary>
    internal void ScheduleCleanup()
    {
        // only the first request after a previous cleanup should schedule real work
        if (Interlocked.Increment(ref _cleanupRequests) == 1)
        {
            Dispatcher.InvokeAsync(CleanupOperation, DispatcherPriority.ContextIdle);
        }
    }

    /// <summary>
    /// Perform a cleanup pass.
    /// </summary>
    internal static bool Cleanup() => CurrentWeakEventTable.Purge(false);

    // run a cleanup pass
    private void CleanupOperation()
    {
        // allow new requests, even if cleanup is disabled
        Interlocked.Exchange(ref _cleanupRequests, 0);

        if (IsCleanupEnabled)
        {
            Purge(false);
        }
    }

    // remove dead entries.  When purgeAll is true, remove all entries.
    private bool Purge(bool purgeAll)
    {
        bool foundDirt = false;

        using (WriteLock)
        {
            Debug.Assert(_toRemove.Count == 0, "to-remove list should be empty");
            _inPurge = true;

            foreach (var ide in _dataTable)
            {
                EventKey key = ide.Key;
                object source = key.Source;
                foundDirt |= key.Manager.PurgeInternal(source, ide.Value, purgeAll);

                // if source has been GC'd, remove its data
                if (!purgeAll && source is null)
                {
                    _toRemove.Add(key);
                }
            }

            _inPurge = false;

            if (purgeAll)
            {
                _managerTable.Clear();
                _dataTable.Clear();
            }
            else if (_toRemove.Count > 0)
            {
                foreach (EventKey key in _toRemove)
                {
                    _dataTable.Remove(key);
                }
                _toRemove.Clear();
                _toRemove.TrimExcess();
            }
        }

        return foundDirt;
    }

    private readonly Dictionary<Type, WeakEventManager> _managerTable = [];  // maps manager type -> instance
    private readonly Dictionary<EventKey, object> _dataTable = [];     // maps EventKey -> data
    private readonly Dictionary<EventNameKey, WeakEventManager> _eventNameTable = []; // maps <Type,name> -> manager

    private readonly ReaderWriterLockWrapper _lock = new();
    private int _cleanupRequests;
    private bool _inPurge;
    private readonly List<EventKey> _toRemove = [];

    [ThreadStatic]
    private static WeakEventTable _currentTable;  // one table per thread

    // the key for the data table:  <manager, ((source)), hashcode>
    private readonly struct EventKey
    {
        internal EventKey(WeakEventManager manager, object source, bool useWeakRef)
        {
            _manager = manager;
            _source = useWeakRef ? new WeakReference<object>(source) : source;
            _hashcode = unchecked(manager.GetHashCode() + RuntimeHelpers.GetHashCode(source));
        }

        internal EventKey(WeakEventManager manager, object source)
            : this(manager, source, false)
        {
        }

        internal object Source
        {
            get
            {
                if (_source is WeakReference<object> wr)
                {
                    wr.TryGetTarget(out object source);
                    return source;
                }
                return _source;
            }
        }

        internal WeakEventManager Manager => _manager;

        public override int GetHashCode()
        {
#if DEBUG
            if (Source is object source)
            {
                int hashcode = unchecked(_manager.GetHashCode() + RuntimeHelpers.GetHashCode(source));
                Debug.Assert(hashcode == _hashcode, "hashcodes disagree");
            }
#endif

            return _hashcode;
        }

        public override bool Equals(object o) => o is EventKey key && this == key;

        public static bool operator ==(EventKey key1, EventKey key2)
        {
            if (key1._manager != key2._manager || key1._hashcode != key2._hashcode)
            {
                return false;
            }

            object s1 = key1.Source;
            object s2 = key2.Source;

            if (s1 is not null && s2 is not null)
            {
                return s1 == s2;
            }
            else
            {
                return key1._source == key2._source;
            }
        }

        public static bool operator !=(EventKey key1, EventKey key2) => !(key1 == key2);

        private readonly WeakEventManager _manager;
        private readonly object _source;             // lookup: direct ref;  In table: WeakRef
        private readonly int _hashcode;              // cached, in case source is GC'd
    }

    // the key for the event name table:  <ownerType, eventName>
    private readonly struct EventNameKey
    {
        public EventNameKey(Type eventSourceType, string eventName)
        {
            _eventSourceType = eventSourceType;
            _eventName = eventName;
        }

        public override int GetHashCode() => unchecked(_eventSourceType.GetHashCode() + _eventName.GetHashCode());

        public override bool Equals(object o) => o is EventNameKey key && this == key;

        public static bool operator ==(EventNameKey key1, EventNameKey key2)
            => key1._eventSourceType == key2._eventSourceType && key1._eventName == key2._eventName;

        public static bool operator !=(EventNameKey key1, EventNameKey key2) => !(key1 == key2);

        private readonly Type _eventSourceType;
        private readonly string _eventName;
    }

    private sealed class ReaderWriterLockWrapper
    {
        private readonly ReaderWriterLockSlim _rwLock;
        private readonly AutoReaderReleaseClass _arrc;
        private readonly AutoWriterReleaseClass _awrc;

        public ReaderWriterLockWrapper()
        {
            _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _arrc = new AutoReaderReleaseClass(this);
            _awrc = new AutoWriterReleaseClass(this);
        }

        public IDisposable ReadLock
        {
            get
            {
                _rwLock.EnterReadLock();
                return _arrc;
            }
        }

        public IDisposable WriteLock
        {
            get
            {
                _rwLock.EnterWriteLock();
                return _awrc;
            }
        }

        private void ReleaseReaderLock() => _rwLock.ExitReadLock();

        private void ReleaseWriterLock() => _rwLock.ExitWriteLock();

        private sealed class AutoWriterReleaseClass : IDisposable
        {
            public AutoWriterReleaseClass(ReaderWriterLockWrapper wrapper)
            {
                _wrapper = wrapper;
            }

            public void Dispose() => _wrapper.ReleaseWriterLock();

            private readonly ReaderWriterLockWrapper _wrapper;
        }

        private sealed class AutoReaderReleaseClass : IDisposable
        {
            public AutoReaderReleaseClass(ReaderWriterLockWrapper wrapper)
            {
                _wrapper = wrapper;
            }

            public void Dispose() => _wrapper.ReleaseReaderLock();

            private readonly ReaderWriterLockWrapper _wrapper;
        }
    }
}
