
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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Provides a base class for the event manager that is used in the weak event pattern. The manager adds and 
/// removes listeners for events (or callbacks) that also use the pattern.
/// </summary>
public abstract class WeakEventManager : DispatcherObject
{
    /// <summary>
    /// Initializes base class values when it is used as the initializer by the constructor of a derived class.
    /// </summary>
    protected WeakEventManager()
    {
        _table = WeakEventTable.CurrentWeakEventTable;
    }

    // initialize static fields
    static WeakEventManager()
    {
        DeliverEventMethodInfo = typeof(WeakEventManager).GetMethod(nameof(DeliverEvent), BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Establishes a read-lock on the underlying data table, and returns an <see cref="IDisposable"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to establish a lock on the data table members and then be appropriately 
    /// disposed with a using construct.
    /// </returns>
    protected IDisposable ReadLock => Table.ReadLock;

    /// <summary>
    /// Establishes a write-lock on the underlying data table, and returns an <see cref="IDisposable"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to establish a lock on the data table members and then be appropriately
    /// disposed with a using construct.
    /// </returns>
    protected IDisposable WriteLock => Table.WriteLock;

    /// <summary>
    /// Gets or sets the data being stored for the specified source.
    /// </summary>
    /// <param name="source">
    /// The zero-based index of the requested source.
    /// </param>
    /// <returns>
    /// Data being stored by the manager for this source.
    /// </returns>
    protected object this[object source]
    {
        get => Table[this, source];
        set => Table[this, source] = value;
    }

    /// <summary>
    /// MethodInfo for the DeliverEvent method - used by generic WeakEventManager.
    /// </summary>
    internal static MethodInfo DeliverEventMethodInfo { get; }

    /// <summary>
    /// Returns a new object to contain listeners to an event.
    /// </summary>
    /// <returns>
    /// A new object to contain listeners to an event.
    /// </returns>
    protected virtual ListenerList NewListenerList() => new();

    /// <summary>
    /// When overridden in a derived class, starts listening for the event being managed. After the 
    /// <see cref="StartListening(object)"/> method is first called, the manager should be in the state of 
    /// calling <see cref="DeliverEvent(object, EventArgs)"/> or <see cref="DeliverEventToList(object, EventArgs, ListenerList)"/>
    /// whenever the relevant event from the provided source is handled.
    /// </summary>
    /// <param name="source">
    /// The source to begin listening on.
    /// </param>
    protected abstract void StartListening(object source);

    /// <summary>
    /// When overridden in a derived class, stops listening on the provided source for the event being managed.
    /// </summary>
    /// <param name="source">
    /// The source to stop listening on.
    /// </param>
    protected abstract void StopListening(object source);

    /// <summary>
    /// Returns the <see cref="WeakEventManager"/> implementation that is used for the provided type.
    /// </summary>
    /// <param name="managerType">
    /// The type to obtain the <see cref="WeakEventManager"/> for.
    /// </param>
    /// <returns>
    /// The matching <see cref="WeakEventManager"/> implementation.
    /// </returns>
    protected static WeakEventManager GetCurrentManager(Type managerType)
        => WeakEventTable.CurrentWeakEventTable[managerType];

    /// <summary>
    /// Sets the current manager for the specified manager type.
    /// </summary>
    /// <param name="managerType">
    /// The type to set the new event manager.
    /// </param>
    /// <param name="manager">
    /// The new event manager.
    /// </param>
    protected static void SetCurrentManager(Type managerType, WeakEventManager manager)
        => WeakEventTable.CurrentWeakEventTable[managerType] = manager;

    /// <summary>
    /// Get the current manager for the given event.
    /// </summary>
    internal static WeakEventManager GetCurrentManager(Type eventSourceType, string eventName)
        => WeakEventTable.CurrentWeakEventTable[eventSourceType, eventName];

    /// <summary>
    /// Set the current manager for the given event.
    /// </summary>
    internal static void SetCurrentManager(Type eventSourceType, string eventName, WeakEventManager manager)
        => WeakEventTable.CurrentWeakEventTable[eventSourceType, eventName] = manager;

    /// <summary>
    /// Removes all listeners for the specified source.
    /// </summary>
    /// <returns>
    /// The source to remove listener information for.
    /// </returns>
    protected void Remove(object source) => Table.Remove(this, source);

    /// <summary>
    /// Adds the provided listener to the provided source for the event being managed.
    /// </summary>
    /// <param name="source">
    /// The source to attach listeners to.
    /// </param>
    /// <param name="listener">
    /// The listening class (which must implement <see cref="IWeakEventListener"/>).
    /// </param>
    protected void ProtectedAddListener(object source, IWeakEventListener listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        AddListener(source, listener, null);
    }

    /// <summary>
    /// Removes a previously added listener from the provided source.
    /// </summary>
    /// <param name="source">
    /// The source to remove listeners from.
    /// </param>
    /// <param name="listener">
    /// The listening class (which must implement <see cref="IWeakEventListener"/>).
    /// </param>
    protected void ProtectedRemoveListener(object source, IWeakEventListener listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        RemoveListener(source, listener, null);
    }

    /// <summary>
    /// Adds the specified delegate as an event handler of the specified source.
    /// </summary>
    /// <param name="source">
    /// The source object that the handler delegate subscribes to.
    /// </param>
    /// <param name="handler">
    /// The delegate that handles the event that is raised by source.
    /// </param>
    protected void ProtectedAddHandler(object source, Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        AddListener(source, null, handler);
    }

    /// <summary>
    /// Removes the previously added handler from the specified source.
    /// </summary>
    /// <param name="source">
    /// The source to remove the handler from.
    /// </param>
    /// <param name="handler">
    /// The delegate to remove from source.
    /// </param>
    protected void ProtectedRemoveHandler(object source, Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        RemoveListener(source, null, handler);
    }

    private void AddListener(object source, IWeakEventListener listener, Delegate handler)
    {
        object sourceKey = source ?? StaticSource;

        using (Table.WriteLock)
        {
            if (Table[this, sourceKey] is not ListenerList list)
            {
                // no entry in the table - add a new one
                list = NewListenerList();
                Table[this, sourceKey] = list;

                // listen for the desired event
                StartListening(source);
            }

            // make sure list is ready for writing
            if (ListenerList.PrepareForWriting(ref list))
            {
                Table[this, source] = list;
            }

            // add a target to the list of listeners
            if (handler is not null)
            {
                list.AddHandler(handler);
            }
            else
            {
                list.Add(listener);
            }

            // schedule a cleanup pass (heuristic (b) described above)
            ScheduleCleanup();
        }
    }

    private void RemoveListener(object source, object target, Delegate handler)
    {
        object sourceKey = source ?? StaticSource;

        using (Table.WriteLock)
        {
            if (Table[this, sourceKey] is ListenerList list)
            {
                // make sure list is ready for writing
                if (ListenerList.PrepareForWriting(ref list))
                {
                    Table[this, sourceKey] = list;
                }

                // remove the target from the list of listeners
                if (handler is not null)
                {
                    list.RemoveHandler(handler);
                }
                else
                {
                    list.Remove((IWeakEventListener)target);
                }

                // after removing the last listener, stop listening
                if (list.IsEmpty)
                {
                    Table.Remove(this, sourceKey);

                    StopListening(source);
                }
            }
        }
    }

    /// <summary>
    /// Delivers the event being managed to each listener.
    /// </summary>
    /// <param name="sender">
    /// The object on which the event is being handled.
    /// </param>
    /// <param name="args">
    /// An <see cref="EventArgs"/> that contains the event data for the event to deliver.
    /// </param>
    protected void DeliverEvent(object sender, EventArgs args)
    {
        ListenerList list;
        object sourceKey = sender ?? StaticSource;

        // get the list of listeners
        using (Table.ReadLock)
        {
            list = (ListenerList)Table[this, sourceKey] ?? ListenerList.Empty;

            // mark the list "in use", even outside the read lock,
            // so that any writers will know not to modify it (they'll
            // modify a clone intead).
            list.BeginUse();
        }

        // deliver the event, being sure to undo the effect of BeginUse().
        try
        {
            DeliverEventToList(sender, args, list);
        }
        finally
        {
            list.EndUse();
        }
    }

    /// <summary>
    /// Delivers the event being managed to each listener in the provided list.
    /// </summary>
    /// <param name="sender">
    /// The object on which the event is being handled.
    /// </param>
    /// <param name="args">
    /// An <see cref="EventArgs"/> that contains the event data.
    /// </param>
    /// <param name="list">
    /// The provided <see cref="ListenerList"/>.
    /// </param>
    protected void DeliverEventToList(object sender, EventArgs args, ListenerList list)
    {
        bool foundStaleEntries = list.DeliverEvent(sender, args, GetType());

        // if we found stale entries, schedule a cleanup (heuristic b)
        if (foundStaleEntries)
        {
            ScheduleCleanup();
        }
    }

    /// <summary>
    /// Requests that a purge of unused entries in the underlying listener list be performed on a lower priority thread.
    /// </summary>
    protected void ScheduleCleanup() => Table.ScheduleCleanup();

    /// <summary>
    /// Removes inactive listener entries from the data list for the provided source. Returns true if some 
    /// entries were actually removed from the list.
    /// </summary>
    /// <param name="source">
    /// The source for events being listened to.
    /// </param>
    /// <param name="data">
    /// The data to check. This object is expected to be a <see cref="ListenerList"/> implementation.
    /// </param>
    /// <param name="purgeAll">
    /// true to stop listening to source, and completely remove all entries from data.
    /// </param>
    /// <returns>
    /// true if some entries were actually removed; otherwise, false.
    /// </returns>
    protected virtual bool Purge(object source, object data, bool purgeAll)
    {
        bool foundDirt = false;

        bool removeList = purgeAll || source is null;

        // remove dead entries from the list
        if (!removeList)
        {
            ListenerList list = (ListenerList)data;

            if (ListenerList.PrepareForWriting(ref list) && source is not null)
            {
                Table[this, source] = list;
            }

            if (list.Purge())
            {
                foundDirt = true;
            }

            removeList = list.IsEmpty;
        }

        // if the list is no longer needed, stop listening to the event
        if (removeList)
        {
            if (source is not null) // source may have been GC'd
            {
                StopListening(source);

                // remove the list completely (in the purgeAll case, we'll do it later)
                if (!purgeAll)
                {
                    Table.Remove(this, source);
                    foundDirt = true;
                }
            }
        }

        return foundDirt;
    }

    // this should only be called by WeakEventTable
    internal bool PurgeInternal(object source, object data, bool purgeAll) => Purge(source, data, purgeAll);

    // for use by test programs (e.g. leak detectors) that want to force
    // a cleanup pass.
    internal static bool Cleanup() => WeakEventTable.Cleanup();

    // for use by test programs (e.g. perf tests) that want to disable
    // cleanup passes temporarily.
    internal static void SetCleanupEnabled(bool value) => WeakEventTable.CurrentWeakEventTable.IsCleanupEnabled = value;

    private WeakEventTable Table => _table;

    private readonly WeakEventTable _table;
    private static readonly object StaticSource = new NamedObject("StaticSource");

    internal readonly struct Listener
    {
        public Listener(object target)
        {
            target ??= StaticSource;
            _target = new(target);
            _handler = null;
        }

        public Listener(object target, Delegate handler)
        {
            _target = new(target);
            _handler = new(handler);
        }

        public bool Matches(object target, Delegate handler) => ReferenceEquals(target, Target) && Equals(handler, Handler);

        public object Target
        {
            get
            {
                _target.TryGetTarget(out object target);
                return target;
            }
        }

        public Delegate Handler
        {
            get
            {
                if (_handler is null)
                {
                    return null;
                }

                _handler.TryGetTarget(out Delegate handler);
                return handler;
            }
        }

        public bool HasHandler => _handler is not null;

        private readonly WeakReference<object> _target;
        private readonly WeakReference<Delegate> _handler;
    }

    /// <summary>
    /// Provides a built-in collection list for storing listeners for a <see cref="WeakEventManager"/>.
    /// </summary>
    protected class ListenerList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerList"/> class.
        /// </summary>
        public ListenerList()
        {
            _list = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerList"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The number of items that should be allocated in the initial list.
        /// </param>
        public ListenerList(int capacity)
        {
            _list = new List<Listener>(capacity);
        }

        /// <summary>
        /// Gets or sets a specific listener item in the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the listener in the list.
        /// </param>
        /// <returns>
        /// The item at that index, or a null reference if no item was at that index.
        /// </returns>
        public IWeakEventListener this[int index] => (IWeakEventListener)_list[index].Target;

        internal Listener GetListener(int index) => _list[index];

        /// <summary>
        /// Gets the number of items contained in the <see cref="ListenerList"/>.
        /// </summary>
        /// <returns>
        /// The number of items contained in the <see cref="ListenerList"/>.
        /// </returns>
        public int Count => _list.Count;

        /// <summary>
        /// Gets a value that declares whether this <see cref="ListenerList"/> is empty.
        /// </summary>
        /// <returns>
        /// true if the list is empty; otherwise, false.
        /// </returns>
        public bool IsEmpty => _list.Count == 0;

        /// <summary>
        /// Gets a value that represents an empty list for purposes of comparisons.
        /// </summary>
        /// <returns>
        /// The empty list representation.
        /// </returns>
        public static ListenerList Empty { get; } = new ListenerList();

        /// <summary>
        /// Adds a <see cref="IWeakEventListener"/> object to the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="listener">
        /// The listener element to add to the <see cref="ListenerList"/>.
        /// </param>
        public void Add(IWeakEventListener listener)
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");
            _list.Add(new Listener(listener));
        }

        /// <summary>
        /// Removes the first occurrence of a listener item from the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="listener">
        /// The item to remove.
        /// </param>
        public void Remove(IWeakEventListener listener)
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");
            for (int i = _list.Count - 1; i >= 0; --i)
            {
                if (_list[i].Target == listener)
                {
                    _list.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds an event handler to the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="handler">
        /// The event handler to add to the <see cref="ListenerList"/>.
        /// </param>
        public void AddHandler(Delegate handler)
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");

            object target = handler.Target ?? StaticSource;

            // add a record to the main list
            _list.Add(new Listener(target, handler));

            AddHandlerToCWT(target, handler);
        }

        private void AddHandlerToCWT(object target, Delegate handler)
        {
            // add the handler to the CWT - this keeps the handler alive throughout
            // the lifetime of the target, without prolonging the lifetime of
            // the target
            if (!_cwt.TryGetValue(target, out object value))
            {
                // 99% case - the target only listens once
                _cwt.Add(target, handler);
            }
            else
            {
                // 1% case - the target listens multiple times
                // we store the delegates in a list
                if (value is not List<Delegate> list)
                {
                    // lazily allocate the list, and add the old handler
                    list = [value as Delegate];

                    // install the list as the CWT value
                    _cwt.Remove(target);
                    _cwt.Add(target, list);
                }

                // add the new handler to the list
                list.Add(handler);
            }
        }

        /// <summary>
        /// Removes an event handler from the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="handler">
        /// The event handler to remove from the <see cref="ListenerList"/>.
        /// </param>
        public void RemoveHandler(Delegate handler)
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");

            object target = handler.Target ?? StaticSource;

            // remove the record from the main list
            for (int i = _list.Count - 1; i >= 0; --i)
            {
                if (_list[i].Matches(target, handler))
                {
                    _list.RemoveAt(i);
                    break;
                }
            }

            // remove the handler from the CWT
            if (_cwt.TryGetValue(target, out object value))
            {
                if (value is not List<Delegate> list)
                {
                    // 99% case - the target is removing its single handler
                    _cwt.Remove(target);
                }
                else
                {
                    // 1% case - the target had multiple handlers, and is removing one
                    list.Remove(handler);
                    if (list.Count == 0)
                    {
                        _cwt.Remove(target);
                    }
                }
            }
            else
            {
                // target has been GC'd.  This probably can't happen, since the
                // target initiates the Remove.  But if it does, there's nothing
                // to do - the target is removed from the CWT automatically,
                // and the weak-ref in the main list will be removed
                // at the next Purge.
            }
        }

        /// <summary>
        /// Add the given listener to the list.
        /// </summary>
        internal void Add(Listener listener)
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");

            // no need to add if the listener has been GC'd
            if (listener.Target is not object target)
            {
                return;
            }

            _list.Add(listener);
            if (listener.HasHandler)
            {
                AddHandlerToCWT(target, listener.Handler);
            }
        }

        /// <summary>
        /// Checks to see whether the provided list is in use, and if so, sets the list reference parameter 
        /// to a copy of that list rather than the original.
        /// </summary>
        /// <param name="list">
        /// The list to check for use state and potentially copy.
        /// </param>
        /// <returns>
        /// true if the provided list was in use at the time of call and therefore the list parameter reference
        /// was reset to be a copy. false if the provided list was not in use, in which case the list parameter 
        /// reference remains unaltered.
        /// </returns>
        public static bool PrepareForWriting(ref ListenerList list)
        {
            bool inUse = list.BeginUse();
            list.EndUse();

            if (inUse)
            {
                list = list.Clone();
            }

            return inUse;
        }

        /// <summary>
        /// Delivers the event being managed to each listener in the <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="sender">
        /// The object that raised the event.
        /// </param>
        /// <param name="args">
        /// An object that contains the event data.
        /// </param>
        /// <param name="managerType">
        /// The type of the <see cref="WeakEventManager"/> that calls this method.
        /// </param>
        /// <returns>
        /// true if any of the listeners in the <see cref="ListenerList"/> refer to an object that has been garbage 
        /// collected; otherwise, false.
        /// </returns>
        public virtual bool DeliverEvent(object sender, EventArgs args, Type managerType)
        {
            bool foundStaleEntries = false;

            for (int k = 0, n = Count; k < n; ++k)
            {
                Listener listener = GetListener(k);
                foundStaleEntries |= DeliverEvent(ref listener, sender, args, managerType);
            }

            return foundStaleEntries;
        }

        internal bool DeliverEvent(ref Listener listener, object sender, EventArgs args, Type managerType)
        {
            object target = listener.Target;
            bool entryIsStale = target is null;

            if (!entryIsStale)
            {
                if (listener.HasHandler)
                {
                    EventHandler handler = (EventHandler)listener.Handler;
                    handler?.Invoke(sender, args);
                }
                else
                {
                    // legacy (4.0)
                    if (target is IWeakEventListener iwel)
                    {
                        bool handled = iwel.ReceiveWeakEvent(managerType, sender, args);

                        // if the event isn't handled, something is seriously wrong.  This
                        // means a listener registered to receive the event, but refused to
                        // handle it when it was delivered.  Such a listener is coded incorrectly.
                        if (!handled)
                        {
                            Debug.Assert(handled,
                                Strings.ListenerDidNotHandleEvent,
                                string.Format(Strings.ListenerDidNotHandleEventDetail, iwel.GetType(), managerType));
                        }
                    }
                }
            }

            return entryIsStale;
        }

        /// <summary>
        /// Removes all entries from the list where the underlying reference target is a null reference.
        /// </summary>
        /// <returns>
        /// Returns true if any entries were purged; otherwise, false.
        /// </returns>
        public bool Purge()
        {
            Debug.Assert(_users == 0, "Cannot modify a ListenerList that is in use");
            bool foundDirt = false;

            for (int j = _list.Count - 1; j >= 0; --j)
            {
                if (_list[j].Target is null)
                {
                    _list.RemoveAt(j);
                    foundDirt = true;
                }
            }

            return foundDirt;
        }

        /// <summary>
        /// Creates a modifiable clone of this <see cref="ListenerList"/>.
        /// </summary>
        /// <returns>
        /// A modifiable clone of the current object.
        /// </returns>
        public virtual ListenerList Clone()
        {
            var result = new ListenerList();
            CopyTo(result);
            return result;
        }

        /// <summary>
        /// Copies the current <see cref="ListenerList"/> to the specified <see cref="ListenerList"/>.
        /// </summary>
        /// <param name="newList">
        /// The object to copy to.
        /// </param>
        protected void CopyTo(ListenerList newList)
        {
            for (int k = 0, n = Count; k < n; ++k)
            {
                Listener listener = GetListener(k);
                object target = listener.Target;
                if (target is not null)
                {
                    if (listener.HasHandler)
                    {
                        if (listener.Handler is Delegate handler)
                        {
                            newList.AddHandler(handler);
                        }
                    }
                    else if (target is IWeakEventListener iwel)
                    {
                        newList.Add(iwel);
                    }
                }
            }
        }

        /// <summary>
        /// Declares the list to be in use. This prevents direct changes to the list during iterations of the list items.
        /// </summary>
        /// <returns>
        /// true if the list was already declared to be in use; otherwise, false.
        /// </returns>
        public bool BeginUse() => Interlocked.Increment(ref _users) != 1;

        /// <summary>
        /// Unlocks the locked state initiated by <see cref="BeginUse"/>.
        /// </summary>
        public void EndUse() => Interlocked.Decrement(ref _users);

        private readonly List<Listener> _list;  // list of listeners
        private int _users;     // number of active users
        private readonly ConditionalWeakTable<object, object> _cwt = new();
    }

    /// <summary>
    /// Provides a type-safe collection list for storing listeners for a <see cref="WeakEventManager"/>.
    /// This class defines a type parameter for the event data that is used.
    /// </summary>
    /// <typeparam name="TEventArgs">
    /// The type that holds the event data.
    /// </typeparam>
    protected class ListenerList<TEventArgs> : ListenerList
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerList{TEventArgs}"/> class.
        /// </summary>
        public ListenerList() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerList{TEventArgs}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The number of items that should be allocated in the initial list.
        /// </param>
        public ListenerList(int capacity) : base(capacity) { }

        /// <summary>
        /// Delivers the event being managed to each listener in the <see cref="ListenerList{TEventArgs}"/>.
        /// </summary>
        /// <param name="sender">
        /// The object that raised the event.
        /// </param>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        /// <param name="managerType">
        /// The type of the <see cref="WeakEventManager"/> that calls this method.
        /// </param>
        /// <returns>
        /// true if any of the listeners in the <see cref="ListenerList{TEventArgs}"/> refer to an object that has been garbage 
        /// collected; otherwise, false.
        /// </returns>
        public override bool DeliverEvent(object sender, EventArgs e, Type managerType)
        {
            TEventArgs args = (TEventArgs)e;
            bool foundStaleEntries = false;

            for (int k = 0, n = Count; k < n; ++k)
            {
                Listener listener = GetListener(k);
                if (listener.Target is not null)
                {
                    if (listener.Handler is EventHandler<TEventArgs> handler)
                    {
                        handler(sender, args);
                    }
                    else
                    {
                        // legacy (4.0)
                        foundStaleEntries |= DeliverEvent(ref listener, sender, e, managerType);
                    }
                }
                else
                {
                    foundStaleEntries = true;
                }
            }

            return foundStaleEntries;
        }

        /// <summary>
        /// Creates a modifiable clone of this <see cref="ListenerList"/>, making deep copies of the values.
        /// </summary>
        /// <returns>
        /// A modifiable clone of the current object.
        /// </returns>
        public override ListenerList Clone()
        {
            var result = new ListenerList<TEventArgs>();
            CopyTo(result);
            return result;
        }
    }
}
