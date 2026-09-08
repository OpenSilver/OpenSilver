
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

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using OpenSilver;
using OpenSilver.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Xml.Linq;

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

        DefaultSynchronizationContext = new DispatcherSynchronizationContext(this);

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

    internal DispatcherSynchronizationContext DefaultSynchronizationContext { get; }

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
    public DispatcherOperation BeginInvoke(Action a) => InvokeAsync(a);

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
    /// <exception cref="ArgumentNullException">
    /// <paramref name="d"/> is null.
    /// </exception>
    public DispatcherOperation BeginInvoke(Delegate d, params object[] args) =>
        BeginInvokeImpl(DispatcherPriority.Normal, d, args, -1);

    /// <summary>
    /// Executes the specified delegate asynchronously with the specified arguments, at the specified priority, 
    /// on the thread that the <see cref="Dispatcher"/> was created on.
    /// </summary>
    /// <param name="d">
    /// The delegate to a method that takes parameters specified in args, which is pushed onto the <see cref="Dispatcher"/> 
    /// event queue.
    /// </param>
    /// <param name="priority">
    /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/> event queue, with 
    /// which the specified method is invoked.
    /// </param>
    /// <param name="args">
    /// An array of objects to pass as arguments to the given method. Can be null.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(Delegate, DispatcherPriority, object[])"/> 
    /// is called, that can be used to interact with the delegate as it is pending execution in the event queue.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="d"/> is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="priority"/> is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    public DispatcherOperation BeginInvoke(Delegate d, DispatcherPriority priority, params object[] args) =>
        BeginInvokeImpl(priority, d, args, -1);

    /// <summary>
    /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="Dispatcher"/> 
    /// is associated with.
    /// </summary>
    /// <param name="priority">
    /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/> event queue, with 
    /// which the specified method is invoked.
    /// </param>
    /// <param name="method">
    /// The delegate to a method that takes no arguments, which is pushed onto the <see cref="Dispatcher"/> event queue.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(DispatcherPriority, Delegate)"/> 
    /// is called, that can be used to interact with the delegate as it is pending execution in the event queue.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="priority"/> is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method) =>
        BeginInvokeImpl(priority, method, null, 0);

    /// <summary>
    /// Executes the specified delegate asynchronously at the specified priority and with the specified argument on 
    /// the thread the <see cref="Dispatcher"/> is associated with.
    /// </summary>
    /// <param name="priority">
    /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/> event queue, with which 
    /// the specified method is invoked.
    /// </param>
    /// <param name="method">
    /// A delegate to a method that takes one argument, which is pushed onto the <see cref="Dispatcher"/> event queue.
    /// </param>
    /// <param name="arg">
    /// The object to pass as an argument to the specified method.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(DispatcherPriority, Delegate, object)"/> 
    /// is called, that can be used to interact with the delegate as it is pending execution in the event queue.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="priority"/> is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg) =>
        BeginInvokeImpl(priority, method, arg, 1);

    /// <summary>
    /// Executes the specified delegate asynchronously at the specified priority and with the specified array of arguments on the 
    /// thread the <see cref="Dispatcher"/> is associated with.
    /// </summary>
    /// <param name="priority">
    /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/> event queue, with which the 
    /// specified method is invoked.
    /// </param>
    /// <param name="method">
    /// A delegate to a method that takes multiple arguments, which is pushed onto the <see cref="Dispatcher"/> event queue.
    /// </param>
    /// <param name="arg">
    /// The object to pass as an argument to the specified method.
    /// </param>
    /// <param name="args">
    /// An array of objects to pass as arguments to the specified method.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="BeginInvoke(DispatcherPriority, Delegate, object, object[])"/>
    /// is called, that can be used to interact with the delegate as it is pending execution in the <see cref="Dispatcher"/> queue.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="method"/> is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="priority"/> is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) =>
        BeginInvokeImpl(priority, method, CombineParameters(arg, args), -1);

    /// <summary>
    /// Determines whether the calling thread is the thread associated with this <see cref="Dispatcher"/>.
    /// </summary>
    /// <returns>
    /// true if the calling thread is the thread associated with this <see cref="Dispatcher"/>; otherwise, false.
    /// </returns>
    public bool CheckAccess() => _dispatcherImpl.CheckAccess();

    /// <summary>
    /// Determines whether the calling thread has access to this <see cref="Dispatcher"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The calling thread does not have access to this <see cref="Dispatcher"/>.
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void VerifyAccess()
    {
        if (!CheckAccess())
        {
            throw new InvalidOperationException(Strings.VerifyAccess);
        }
    }

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
    /// <exception cref="ArgumentNullException">
    /// <paramref name="a"/> is null.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="priority"/> is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    public DispatcherOperation InvokeAsync(Action a, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(a);

        ValidatePriority(priority);

        var operation = new DispatcherOperation(a, priority);
        InvokeAsyncImpl(operation);

        return operation;
    }

    /// <summary>
    /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the 
    /// <see cref="Dispatcher"/> is associated with.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="callback">
    /// A delegate to invoke through the dispatcher.
    /// </param>
    /// <param name="priority">
    /// The priority that determines the order in which the specified callback is invoked relative to the other pending 
    /// operations in the <see cref="Dispatcher"/>.
    /// </param>
    /// <returns>
    /// An object, which is returned immediately after <see cref="InvokeAsync{TResult}(Func{TResult}, DispatcherPriority)"/>
    /// is called, that can be used to interact with the delegate as it is pending execution in the event queue.
    /// </returns>
    public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(callback);

        ValidatePriority(priority);

        var operation = new DispatcherOperation<TResult>(priority, callback);
        InvokeAsyncImpl(operation);

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

    /// <summary>
    /// Determines whether the specified <see cref="DispatcherPriority"/> is a valid priority.
    /// </summary>
    /// <param name="priority">
    /// The priority to check.
    /// </param>
    /// <param name="paramName">
    /// A string that will be returned by the exception that occurs if the priority is invalid.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// priority is not a valid <see cref="DispatcherPriority"/>.
    /// </exception>
    public static void ValidatePriority(DispatcherPriority priority, [CallerArgumentExpression(nameof(priority))] string paramName = null)
    {
        if (priority < DispatcherPriority.Inactive || priority > DispatcherPriority.Send)
        {
            throw new InvalidEnumArgumentException(paramName, (int)priority, typeof(DispatcherPriority));
        }
    }

    /// <summary>
    /// Creates an awaitable object that asynchronously yields control back to the current dispatcher and 
    /// provides an opportunity for the dispatcher to process other events.
    /// </summary>
    /// <returns>
    /// An awaitable object that asynchronously yields control back to the current dispatcher and provides 
    /// an opportunity for the dispatcher to process other events.
    /// </returns>
    public static DispatcherPriorityAwaitable Yield() => Yield(DispatcherPriority.Background);

    /// <summary>
    /// Creates an awaitable object that asynchronously yields control back to the current dispatcher and 
    /// provides an opportunity for the dispatcher to process other events. The work that occurs when control 
    /// returns to the code awaiting the result of this method is scheduled with the specified priority.
    /// </summary>
    /// <param name="priority">
    /// The priority at which to schedule the continuation.
    /// </param>
    /// <returns>
    /// An awaitable object that asynchronously yields control back to the current dispatcher and provides 
    /// an opportunity for the dispatcher to process other events.
    /// </returns>
    public static DispatcherPriorityAwaitable Yield(DispatcherPriority priority)
    {
        ValidatePriority(priority);

        return new DispatcherPriorityAwaitable(CurrentDispatcher, priority);
    }

    internal void EnableProcessing() => Interlocked.Decrement(ref _disableProcessingRequests);

    private DispatcherOperation BeginInvokeImpl(DispatcherPriority priority, Delegate method, object args, int numArgs)
    {
        ArgumentNullException.ThrowIfNull(method);

        ValidatePriority(priority);

        var operation = new DispatcherOperation(method, priority, args, numArgs);
        InvokeAsyncImpl(operation);

        return operation;
    }

    private void InvokeAsyncImpl(DispatcherOperation operation)
    {
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
    }

    private static object[] CombineParameters(object arg, object[] args)
    {
        if (args is null)
        {
            return [arg, null];
        }

        object[] parameters = new object[1 + args.Length];
        parameters[0] = arg;
        args.CopyTo(parameters, 1);

        return parameters;
    }

    private void SetTickRate(int tickRate) => _dispatcherImpl.SetTickRate(tickRate);

    private void OnDispatcherTickNative()
    {
        List<Exception> unhandledExceptions = null;

        FireTickEvent(ref unhandledExceptions);
        ProcessQueue(ref unhandledExceptions);
        ProcessPendingOperations();

        if (unhandledExceptions is not null)
        {
            ExceptionDispatchInfo.Capture(GetDispatcherException(unhandledExceptions)).Throw();
        }
    }

    private void FireTickEvent(ref List<Exception> unhandledExceptions)
    {
        if (OpenSilverCompatibilityPreferences.HandleDispatcherExceptions)
        {
            var oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(DefaultSynchronizationContext);

            try
            {
                Tick?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                bool handled = Application.CallHandleException(ex);

                if (!handled)
                {
                    unhandledExceptions ??= [];
                    unhandledExceptions.Add(ex);
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
            }
        }
        else
        {
            OnTick();
        }
    }

    private void OnTick() => Tick?.Invoke(this, EventArgs.Empty);

    private void ProcessQueue(ref List<Exception> unhandledExceptions)
    {
        _isProcessingQueue = true;

        while (TryDequeueOperation(out DispatcherOperation operation))
        {
            if (operation.Status == DispatcherOperationStatus.Pending)
            {
                if (OpenSilverCompatibilityPreferences.HandleDispatcherExceptions)
                {
                    InvokeOperation(operation, ref unhandledExceptions);
                }
                else
                {
                    LegacyInvokeOperation(operation);
                }
            }
        }

        _isProcessingQueue = false;
    }

    private void InvokeOperation(DispatcherOperation operation, ref List<Exception> unhandledExceptions)
    {
        var oldSynchronizationContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(this, operation.Priority));

        try
        {
            operation.Invoke();
        }
        catch (Exception ex)
        {
            bool handled = Application.CallHandleException(ex);

            if (!handled)
            {
                unhandledExceptions ??= [];
                unhandledExceptions.Add(ex);
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
        }
    }

    private void LegacyInvokeOperation(DispatcherOperation operation)
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

    private void ProcessPendingOperations()
    {
        if (_pendingOperations.Count == 0)
        {
            return;
        }

        lock (_sync)
        {
            while (_pendingOperations.Count > 0)
            {
                DispatcherOperation operation = _pendingOperations.Dequeue();
                EnqueueOperation(operation);
            }
        }
    }

    private static Exception GetDispatcherException(List<Exception> unhandledExceptions)
    {
        Debug.Assert(unhandledExceptions is not null && unhandledExceptions.Count > 0);

        return unhandledExceptions.Count switch
        {
            1 => unhandledExceptions[0],
            _ => new AggregateException(unhandledExceptions),
        };
    }

    private void EnqueueOperation(DispatcherOperation operation) => _queue.Enqueue((int)operation.Priority, operation);

    private bool TryDequeueOperation(out DispatcherOperation operation)
    {
        lock (_sync)
        {
            while (_disableProcessingRequests == 0)
            {
                if (_queue.MaxPriority == (int)DispatcherPriority.Inactive)
                {
                    break;
                }

                operation = _queue.Dequeue();

                if (operation.Status != DispatcherOperationStatus.Pending) continue;

                return true;
            }
        }

        operation = null;
        return false;
    }

    private interface IDispatcherImpl
    {
        void SetTickRate(int tickRate);
        bool CheckAccess();
    }

    private sealed class WasmDispatcher : IDispatcherImpl
    {
        private readonly Dispatcher _dispatcher;
        private readonly string _nativeDispatcherId;

        public WasmDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            var jsCallback = JavaScriptCallback.Create(OnDispatcherTickNative, false);
            string sHandler = OpenSilver.Interop.GetVariableStringForJS(jsCallback);
            _nativeDispatcherId = OpenSilver.Interop.ExecuteJavaScriptString($"osjs.dispatcher.create({sHandler})", false);
        }

        public bool CheckAccess() => true;

        public void SetTickRate(int tickRate) =>
            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"osjs.dispatcher.setTickRate('{_nativeDispatcherId}', {tickRate.ToInvariantString()})",
                false);

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
            _timer = new Timer(OnTimerTick, null, interval, interval);
        }

        private static int GetInterval(int tickRate) =>
            tickRate switch
            {
                > 0 => 1000 / tickRate,
                0 => 1000,
                _ => 1
            };

        private void OnTimerTick(object state) =>
            INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(_dispatcher.OnDispatcherTickNative);

        public void SetTickRate(int tickRate)
        {
            var interval = GetInterval(tickRate);
            _timer.Change(interval, interval);
        }

        public bool CheckAccess() => INTERNAL_Simulator.OpenSilverDispatcherCheckAccess();
    }
}
