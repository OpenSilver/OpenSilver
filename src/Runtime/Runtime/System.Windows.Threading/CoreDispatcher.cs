
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
using System.Threading.Tasks;
using System.Diagnostics;
using DotNetForHtml5.Core;

#if !MIGRATION
using Windows.Foundation;
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

#if MIGRATION
namespace System.Windows.Threading;
#else
namespace Windows.UI.Core;
#endif

/// <summary>
/// Provides services for managing the queue of work items for a thread.
/// </summary>
#if MIGRATION
public class Dispatcher
#else
public class CoreDispatcher
#endif
{
    private static Dispatcher _currentDispatcher;

    private readonly PriorityQueue<DispatcherOperation> _queue;
    private readonly IDisposable _disableProcessingToken;
    private int _disableProcessingRequests;
    private bool _isProcessQueueScheduled;

#if MIGRATION
    public Dispatcher()
#else
    public CoreDispatcher()
#endif
    {
        _queue = new PriorityQueue<DispatcherOperation>((int)DispatcherPriority.Send - (int)DispatcherPriority.Invalid);
        _disableProcessingToken = new DispatcherProcessingDisabled(this);
    }

    internal static Dispatcher CurrentDispatcher => _currentDispatcher ??= new Dispatcher();

    /// <summary>
    /// Executes the specified delegate asynchronously on the thread the <see cref="Dispatcher"/>
    /// is associated with.
    /// </summary>
    /// <param name="a">
    /// A delegate to a method that takes no arguments and does not return a value, which
    /// is pushed onto the <see cref="Dispatcher"/> event queue.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(Action)"/> is 
    /// called, that represents the operation that has been posted to the <see cref="Dispatcher"/>
    /// queue.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="a"/> is null.
    /// </exception>
    public DispatcherOperation BeginInvoke(Action a)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        return InvokeAsync(a, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Executes the specified delegate asynchronously with the specified array of arguments
    /// on the thread the <see cref="Dispatcher"/> is associated with.
    /// </summary>
    /// <param name="d">
    /// A delegate to a method that takes multiple arguments, which is pushed onto the
    /// <see cref="Dispatcher"/> event queue.
    /// </param>
    /// <param name="args">
    /// An array of objects to pass as arguments to the specified method.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(Delegate, object[])"/>
    /// is called, that represents the operation that has been posted to the <see cref="Dispatcher"/>
    /// queue.
    /// </returns>
    public DispatcherOperation BeginInvoke(Delegate d, params object[] args)
    {
        return BeginInvoke(() => d.DynamicInvoke(args));
    }

    /// <summary>
    /// Determines whether the calling thread is the thread associated with this <see cref="Dispatcher"/>.
    /// </summary>
    /// <returns>
    /// true if the calling thread is the thread associated with this <see cref="Dispatcher"/>; otherwise, false.
    /// </returns>
    public bool CheckAccess()
    {
        return !OpenSilver.Interop.IsRunningInTheSimulator || INTERNAL_Simulator.OpenSilverDispatcherCheckAccess();
    }

    public DispatcherOperation InvokeAsync(Action a, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return EnqueueOperation(a, priority);
    }

    public IDisposable DisableProcessing()
    {
        _disableProcessingRequests++;
        return _disableProcessingToken;
    }

    private void EnableProcessing()
    {
        if (--_disableProcessingRequests == 0)
        {
            ScheduleProcessQueue();
        }
    }

    private void ScheduleProcessQueue()
    {
        if (_isProcessQueueScheduled) return;

        _isProcessQueueScheduled = true;

        if (!OpenSilver.Interop.IsRunningInTheSimulator)
        {
            Task.Run(ProcessQueueAsync);
        }
        else
        {
            INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(() => _ = ProcessQueueAsync());
        }
    }

    private async Task ProcessQueueAsync()
    {
        while (TryDequeue(out DispatcherOperation operation))
        {
            await Task.Delay(1);

            if (operation.Status == DispatcherOperationStatus.Pending)
            {
                try
                {
                    operation.Invoke();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Dispatcher: Method excution failed: " + ex);
                }
            }
        }

        _isProcessQueueScheduled = false;
    }

    private DispatcherOperation EnqueueOperation(Action a, DispatcherPriority priority)
    {
        var operation = new DispatcherOperation(a, priority);
        _queue.Enqueue((int)operation.Priority + 1, operation);
        ScheduleProcessQueue();

        return operation;
    }

    private bool TryDequeue(out DispatcherOperation operation)
    {
        while (_disableProcessingRequests == 0 && _queue.MaxPriority - 1 > (int)DispatcherPriority.Inactive)
        {
            operation = _queue.Dequeue();

            if (operation.Status != DispatcherOperationStatus.Pending) continue;

            return true;
        }

        operation = null;
        return false;
    }

    private sealed class DispatcherProcessingDisabled : IDisposable
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherProcessingDisabled(Dispatcher dispatcher)
        {
            Debug.Assert(dispatcher != null);
            _dispatcher = dispatcher;
        }

        public void Dispose()
        {
            _dispatcher.EnableProcessing();
        }
    }
}
