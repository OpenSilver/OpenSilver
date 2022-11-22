
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
using OpenSilver.Internal.Data;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal;

//
// The list of Dependents that depend on a Source[ID]
//
// Steps are taken to guard against list corruption due to re-entrancy when
// the Invalidation callbacks call Add / Remove.   But multi-threaded
// access is not expected and so locks are not used.
//
internal class DependentList
{
    private List<Dependent> _listStore;

    public int Count => _listStore?.Count ?? 0;

    public int Capacity => _listStore?.Capacity ?? 0;

    public Dependent this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _listStore[index];
        }
        set
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _listStore[index] = value;
        }
    }

    public void Add(Dependent dependent)
    {
        _listStore ??= new List<Dependent>();
        _listStore.Add(dependent);
    }

    public void Remove(Dependent dependent)
    {
        _listStore?.Remove(dependent);
    }

    public void Clear()
    {
        _listStore = null;
    }

    public Dependent[] ToArray()
    {
        if (_listStore is null)
        {
            return Array.Empty<Dependent>();
        }

        return _listStore.ToArray();
    }

    public void Add(IDependencyPropertyChangedListener expr)
    {
        // don't clean up every time.  This would make Add() cost O(N),
        // which would cause building a list to cost O(N^2).  yuck!
        // Clean the list less often the longer it gets.
        if (Count == Capacity)
            CleanUpDeadWeakReferences();

        Add(new Dependent(expr));
    }

    public void Remove(IDependencyPropertyChangedListener expr)
    {
        Remove(new Dependent(expr));
    }

    public bool IsEmpty
    {
        get
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i].IsValid())
                {
                    return false;
                }
            }

            // there are no valid entries.   All callers immediately discard the
            // empty DependentList in this case, so there's no need to clean out
            // the list.  We can just GC collect the WeakReferences.
            return true;
        }
    }

    public void InvalidateDependents(DependencyObject source, DependencyPropertyChangedEventArgs sourceArgs)
    {
        // Take a snapshot of the list to protect against re-entrancy via Add / Remove.
        Dependent[] snapList = ToArray();

        for (int i = 0; i < snapList.Length; i++)
        {
            //Expression expression = snapList[i].Expr;
            IDependencyPropertyChangedListener expression = snapList[i].Expr;
            if (null != expression)
            {
                expression.OnPropertyChanged(source, sourceArgs);
            }
        }
    }

    private void CleanUpDeadWeakReferences()
    {
        int newCount = 0;

        // determine how many entries are valid
        for (int i = Count - 1; i >= 0; --i)
        {
            if (this[i].IsValid())
            {
                ++newCount;
            }
        }

        // if all the entries are valid, there's nothing to do
        if (newCount == Count)
            return;

        // compact the valid entries
        Compacter compacter = new Compacter(this, newCount);
        int runStart = 0;           // starting index of current run
        bool runIsValid = false;    // whether run contains valid or invalid entries

        for (int i = 0, n = Count; i < n; ++i)
        {
            if (runIsValid != this[i].IsValid())    // run has ended
            {
                if (runIsValid)
                {
                    // emit a run of valid entries to the compacter
                    compacter.Include(runStart, i);
                }

                // start a new run
                runStart = i;
                runIsValid = !runIsValid;
            }
        }

        // emit the last run of valid entries
        if (runIsValid)
        {
            compacter.Include(runStart, Count);
        }

        // finish the job
        compacter.Finish();
    }

    private struct Compacter
    {
        private readonly List<Dependent> _listStore;
        private readonly int _newCount;
        private int _validItemCount;
        private int _previousEnd;

        public Compacter(DependentList list, int newCount)
        {
            _listStore = list._listStore;
            _newCount = newCount;
            _validItemCount = 0;
            _previousEnd = 0;
        }

        public void Include(int start, int end)
        {
            Debug.Assert(start >= _previousEnd, "Arguments out of order during Compact");
            Debug.Assert(_validItemCount + end - start <= _newCount, "Too many items copied during Compact");

            // item-by-item move
            for (int i = start; i < end; ++i)
            {
                _listStore[_validItemCount++] = _listStore[i];
            }

            _previousEnd = end;
        }

        public void Finish()
        {
            _listStore?.RemoveRange(_validItemCount, _listStore.Count - _validItemCount);
        }
    }
}

internal readonly struct Dependent
{
    private readonly WeakReference _wrEX;

    public bool IsValid()
    {
        // Expression is never null (could Assert that but throw is fine)
        return _wrEX.IsAlive;
    }

    public Dependent(IDependencyPropertyChangedListener e)
    {
        _wrEX = new WeakReference(e);
    }

    public IDependencyPropertyChangedListener Expr => (IDependencyPropertyChangedListener)_wrEX.Target;

    public override bool Equals(object o)
    {
        if (o is not Dependent d)
            return false;

        // Not equal to Dead values.
        // This is assuming that at least one of the compared items is live.
        // This assumtion comes from knowing that Equal is used by FrugalList.Remove()
        // and if you look at DependentList.Remove()'s arguments, it can only
        // be passed strong references.
        // Therefore: Items being removed (thus compared here) will not be dead.
        if (!IsValid() || !d.IsValid())
            return false;

        if (_wrEX.Target != d._wrEX.Target)
            return false;

        return true;
    }

    public static bool operator ==(Dependent first, Dependent second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(Dependent first, Dependent second)
    {
        return !first.Equals(second);
    }

    // We don't expect to need this function. [Required when overriding Equals()]
    // Write a good HashCode anyway (if not a fast one)
    public override int GetHashCode()
    {
        var ex = (IDependencyPropertyChangedListener)_wrEX.Target;
        int hashCode = (null == ex) ? 0 : ex.GetHashCode();

        return hashCode;
    }
}
