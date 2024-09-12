
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using OpenSilver.Internal;

namespace System.Windows.Threading;

/// <summary>
/// Provides services for managing the queue of work items for a thread.
/// </summary>
public sealed class Dispatcher
{
    private const int DefaultTickRate = 60;

    private readonly IDispatcherImpl _dispatcherImpl;
    private readonly PriorityQueue<DispatcherOperation> _queue;
    private readonly Queue<DispatcherOperation> _pendingOperations;
    private readonly object _sync = new();
    private int _disableProcessingRequests;
    private bool _isProcessingQueue;
    private int _tickRate = DefaultTickRate;

    private Dispatcher()
    {
        _queue = new((int)DispatcherPriority.Send - (int)DispatcherPriority.Inactive + 1);
        _pendingOperations = new();

        _dispatcherImpl = OpenSilver.Interop.IsRunningInTheSimulator ?
            new SimulatorDispatcher(this) : new WasmDispatcher(this);
        _dispatcherImpl.SetTickRate(DefaultTickRate);
    }

    /// <summary>
    /// Gets the <see cref="Dispatcher"/> for the thread currently executing
    /// and creates a new <see cref="Dispatcher"/> if one is not already associated
    /// with the thread.
    /// </summary>
    /// <returns>
    /// The dispatcher associated with the current thread.
    /// </returns>
    public static Dispatcher CurrentDispatcher { get; } = new Dispatcher();

    internal event EventHandler Tick;

    internal int TickRate
    {
        get => _tickRate;
        set
        {
            _tickRate = value;
            SetTickRate(_tickRate);
        }
    }

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
    public DispatcherOperation BeginInvoke(Delegate d, params object[] args) => BeginInvoke(() => d.DynamicInvoke(args));

    /// <summary>
    /// Determines whether the calling thread is the thread associated with this <see cref="Dispatcher"/>.
    /// </summary>
    /// <returns>
    /// true if the calling thread is the thread associated with this <see cref="Dispatcher"/>; otherwise, false.
    /// </returns>
    public bool CheckAccess() => _dispatcherImpl.CheckAccess();

    /// <summary>
    /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread 
    /// the <see cref="Dispatcher"/> is associated with.
    /// </summary>
    /// <param name="a">
    /// A delegate to invoke through the dispatcher.
    /// </param>
    /// <param name="priority">
    /// The priority that determines the order in which the specified callback is invoked relative to the 
    /// other pending operations in the <see cref="Dispatcher"/>.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="InvokeAsync(Action, DispatcherPriority)"/>
    /// is called, that can be used to interact with the delegate as it is pending execution in the event queue.
    /// </returns>
    public DispatcherOperation InvokeAsync(Action a, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ValidatePriority(priority);

        var operation = new DispatcherOperation(a, priority);

        lock (_sync)
        {
            if (_isProcessingQueue)
            {
                _pendingOperations.Enqueue(operation);
            }
            else
            {
                EnqueueOperation(operation);
            }
        }

        return operation;
    }

    /// <summary>
    /// Disables processing of the <see cref="Dispatcher"/> queue.
    /// </summary>
    /// <returns>
    /// A structure used to re-enable dispatcher processing.
    /// </returns>
    public DispatcherProcessingDisabled DisableProcessing()
    {
        Interlocked.Increment(ref _disableProcessingRequests);
        return new(this);
    }

    internal void EnableProcessing() => Interlocked.Decrement(ref _disableProcessingRequests);

    private void SetTickRate(int tickRate) => _dispatcherImpl.SetTickRate(tickRate);

    private void OnDispatcherTickNative()
    {
        Tick?.Invoke(this, EventArgs.Empty);

        ProcessQueue();
        ProcessPendingOperations();
    }

    private void ProcessQueue()
    {
        _isProcessingQueue = true;

        while (TryDequeueOperation(out DispatcherOperation operation))
        {
            if (operation.Status == DispatcherOperationStatus.Pending)
            {
                try
                {
                    operation.Invoke();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Dispatcher: Method execution failed: " + ex);
                }
            }
        }

        _isProcessingQueue = false;
    }

    private void ProcessPendingOperations()
    {
        while (_pendingOperations.Count > 0)
        {
            DispatcherOperation operation = _pendingOperations.Dequeue();
            lock (_sync)
            {
                EnqueueOperation(operation);
            }
        }
    }

    private void EnqueueOperation(DispatcherOperation operation) => _queue.Enqueue((int)operation.Priority, operation);

    private bool TryDequeueOperation(out DispatcherOperation operation)
    {
        while (_disableProcessingRequests == 0)
        {
            lock (_sync)
            {
                if (_queue.MaxPriority == (int)DispatcherPriority.Inactive)
                {
                    break;
                }

                operation = _queue.Dequeue();
            }

            if (operation.Status != DispatcherOperationStatus.Pending) continue;

            return true;
        }

        operation = null;
        return false;
    }

    private static void ValidatePriority(DispatcherPriority priority, [CallerArgumentExpression(nameof(priority))] string paramName = null)
    {
        if (priority < DispatcherPriority.Inactive || priority > DispatcherPriority.Send)
        {
            throw new InvalidEnumArgumentException(paramName, (int)priority, typeof(DispatcherPriority));
        }
    }

    private interface IDispatcherImpl
    {
        void SetTickRate(int tickRate);
        bool CheckAccess();
    }

    private sealed class WasmDispatcher : IDispatcherImpl
    {
        private readonly Dispatcher _dispatcher;

        public WasmDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            var jsCallback = JavaScriptCallback.Create(OnDispatcherTickNative);
            string sHandler = OpenSilver.Interop.GetVariableStringForJS(jsCallback);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.createUIDispatcher({sHandler})", false);
        }

        public bool CheckAccess() => true;

        public void SetTickRate(int tickRate) =>
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.UIDispatcher.setTickRate({tickRate.ToInvariantString()})", false);

        private void OnDispatcherTickNative() => _dispatcher.OnDispatcherTickNative();
    }

    private sealed class SimulatorDispatcher : IDispatcherImpl
    {
        private readonly Dispatcher _dispatcher;
        private readonly Timer _timer;

        public SimulatorDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            var interval = GetInterval(dispatcher._tickRate);
            _timer = new Timer(Timer_Tick, null, interval, interval);
        }

        private static int GetInterval(int tickRate) =>
            tickRate switch
            {
                > 0 => 1000 / tickRate,
                0 => 1000,
                _ => 1
            };

        private void Timer_Tick(object state) =>
            INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(() =>
            {
                try
                {
                    _dispatcher.OnDispatcherTickNative();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Dispatcher: Native Tick failed: " + ex);
                }
            });

        public void SetTickRate(int tickRate)
        {
            var interval = GetInterval(tickRate);
            _timer.Change(interval, interval);
        }

        public bool CheckAccess() => INTERNAL_Simulator.OpenSilverDispatcherCheckAccess();
    }
}
