
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
using System.Collections.ObjectModel;

namespace OpenSilver.Internal;

/// <summary>
///   This is a ThreadSafe ArrayList that uses Copy On Write to support consistency.
///   - When the "List" property is requested a readonly reference to the
///   list is returned and a reference to the readonly list is cached.
///   - If the "List" is requested again, the same cached reference is returned.
///   - When the list is modified, if a readonly reference is present in the
///   cache then the list is copied before it is modified and the readonly list is
///   released from the cache.
/// </summary>
internal class CopyOnWriteList<T>
{
    public CopyOnWriteList()
        : this(0)
    {
    }

    public CopyOnWriteList(int capacity)
    {
        SyncRoot = new();
        LiveList = new(capacity);
    }

    /// <summary>
    ///   Return a readonly wrapper of the list.  Note: this is NOT a copy.
    ///   A non-null _readonlyWrapper  is a "Copy on Write" flag.
    ///   Methods that change the list (eg. Add() and Remove()) are
    ///   responsible for:
    ///    1) Checking _readonlyWrapper and copying the list before modifing it.
    ///    2) Clearing _readonlyWrapper.
    /// </summary>
    public ReadOnlyCollection<T> List
    {
        get
        {
            ReadOnlyCollection<T> tempList;

            lock (SyncRoot)
            {
                tempList = _readonlyWrapper ??= LiveList.AsReadOnly();
            }

            return tempList;
        }
    }

    /// <summary>
    ///   This allows derived classes to take the lock.  This is mostly used
    ///   to extend Add() and Remove() etc.
    /// </summary>
    protected object SyncRoot { get; }

    /// <summary>
    ///  This is protected and the caller can get into real serious trouble
    ///  using this.  Because this points at the real current list without
    ///  any copy on write protection.  So the caller must really know what
    ///  they are doing.
    /// </summary>
    protected List<T> LiveList { get; private set; }

    /// <summary>
    ///   Add an object to the List.
    ///   Without any error checks.
    ///   For use by derived classes that implement there own error checks.
    /// </summary>
    protected bool Internal_Add(T obj)
    {
        DoCopyOnWriteCheck();
        LiveList.Add(obj);
        return true;
    }

    /// <summary>
    ///   Insert an object into the List at the given index.
    ///   Without any error checks.
    ///   For use by derived classes that implement there own error checks.
    /// </summary>
    protected bool Internal_Insert(int index, T obj)
    {
        DoCopyOnWriteCheck();
        LiveList.Insert(index, obj);
        return true;
    }

    /// <summary>
    ///   Remove the object at a given index from the List.
    ///   Returns true if successfully removed.
    ///   Returns false if index is outside the range of the list.
    ///
    ///  This is protected because it operates on the LiveList
    /// </summary>
    protected bool RemoveAt(int index)
    {
        // syncRoot Lock MUST be held by the caller.
        if (index < 0 || index >= LiveList.Count)
            return false;

        DoCopyOnWriteCheck();
        LiveList.RemoveAt(index);
        return true;
    }

    protected void DoCopyOnWriteCheck()
    {
        // syncRoot Lock MUST be held by the caller.
        // If we have exposed (given out) a readonly reference to this
        // version of the list, then clone a new internal copy and cut
        // the old version free.
        if (null != _readonlyWrapper)
        {
            LiveList = new(LiveList);
            _readonlyWrapper = null;
        }
    }

    private ReadOnlyCollection<T> _readonlyWrapper;
}