// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;

namespace OpenSilver.Internal.Data;

internal sealed class DataBindEngine : DispatcherObject
{
    private DataBindEngine()
    {
    }

    internal bool IsShutDown => _viewManager is null;

    /// <summary>
    /// Return the DataBindEngine for the current thread
    /// </summary>
    internal static DataBindEngine CurrentDataBindEngine => _currentEngine ??= new DataBindEngine();

    internal ViewManager ViewManager => _viewManager;

    internal ViewRecord GetViewRecord(object collection, CollectionViewSource key, Type collectionViewType, bool createView, Func<object, object> GetSourceItem)
    {
        if (IsShutDown)
            return null;

        ViewRecord record = _viewManager.GetViewRecord(collection, key, collectionViewType, createView, GetSourceItem);

        // lacking any definitive event on which to trigger a cleanup pass,
        // we use a heuristic, namely the creation of a new view.  This suggests
        // that there is new activity, which often means that old content is
        // being replaced.  So perhaps the view table now has stale entries.
        if (record != null && !record.IsInitialized)
        {
            ScheduleCleanup();
        }

        return record;
    }

    // schedule a cleanup pass.  This can be called from any thread.
    internal void ScheduleCleanup()
    {
        // only the first request after a previous cleanup should schedule real work
        if (Interlocked.Increment(ref _cleanupRequests) == 1)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DispatcherOperationCallback(CleanupOperation), null);
        }
    }

    // return true if something was actually cleaned up
    internal bool Cleanup() => DoCleanup();

    private bool DoCleanup()
    {
        bool foundDirt = false;

        if (!IsShutDown)
        {
            foundDirt = _viewManager.Purge() || foundDirt;
        }

        return foundDirt;
    }

    // Marshal some work from a foreign thread to the UI thread
    // (e.g. PropertyChanged or CollectionChanged events)
    internal DataBindOperation Marshal(DispatcherOperationCallback method, object arg, int cost = 1)
    {
        DataBindOperation op = new DataBindOperation(method, arg, cost);
        lock (_crossThreadQueueLock)
        {
            _crossThreadQueue.Enqueue(op);
            _crossThreadCost += cost;

            _crossThreadDispatcherOperation ??= Dispatcher.InvokeAsync(
                ProcessCrossThreadRequests,
                DispatcherPriority.ContextIdle);
        }

        return op;
    }

    internal void ChangeCost(DataBindOperation op, int delta)
    {
        lock (_crossThreadQueueLock)
        {
            op.Cost += delta;
            _crossThreadCost += delta;
        }
    }

    private void ProcessCrossThreadRequests()
    {
        if (IsShutDown)
            return;

        try
        {
            long startTime = DateTime.UtcNow.Ticks;        // unit = 10^-7 sec

            while (true)
            {
                // get the next request
                DataBindOperation op;
                lock (_crossThreadQueueLock)
                {
                    if (_crossThreadQueue.Count > 0)
                    {
                        op = _crossThreadQueue.Dequeue();
                        _crossThreadCost -= op.Cost;
                    }
                    else
                    {
                        op = null;
                    }
                }

                if (op == null)
                    break;

                // do the work
                op.Invoke();

                // check the time
                if (DateTime.UtcNow.Ticks - startTime > CrossThreadThreshold)
                    break;
            }
        }
        finally
        {
            // update state even if an op throws an exception
            lock (_crossThreadQueueLock)
            {
                if (_crossThreadQueue.Count > 0)
                {
                    // if there's still more work to do, schedule a new callback
                    _crossThreadDispatcherOperation = Dispatcher.InvokeAsync(
                        ProcessCrossThreadRequests,
                        DispatcherPriority.ContextIdle);
                }
                else
                {
                    // otherwise revert to the empty state
                    _crossThreadDispatcherOperation = null;
                    _crossThreadCost = 0;
                }
            }
        }
    }

    // run a cleanup pass
    private object CleanupOperation(object arg)
    {
        // allow new requests, even if cleanup is disabled
        Interlocked.Exchange(ref _cleanupRequests, 0);

        if (!_cleanupEnabled)
        {
            return null;
        }

        Cleanup();

        return null;
    }

    private ViewManager _viewManager = new();

    private bool _cleanupEnabled = true;

    private int _cleanupRequests;

    private readonly Queue<DataBindOperation> _crossThreadQueue = new();
    private readonly object _crossThreadQueueLock = new();
    private int _crossThreadCost;
    private DispatcherOperation _crossThreadDispatcherOperation;
    internal const int CrossThreadThreshold = 50000;   // 50 msec

    [ThreadStatic]
    private static DataBindEngine _currentEngine; // one engine per thread
}
