
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

using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace System.Windows.Threading;

/// <summary>
/// Represents an object that is used to interact with an operation that has been posted to the <see cref="Dispatcher"/> queue.
/// </summary>
public class DispatcherOperation
{
    private readonly DispatcherPriority _priority;
    private readonly Delegate _method;
    private readonly object _args;
    private readonly int _numArgs;
    private readonly bool _useAsyncSemantics;

    private DispatcherOperationStatus _status;
    private object _result;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation(Action action, DispatcherPriority priority)
        : this(action, priority, null, 0, true)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation(Func<object> action, DispatcherPriority priority)
        : this(action, priority, null, 0, false)
    {
    }

    internal DispatcherOperation(Delegate method, DispatcherPriority priority, object args, int numArgs)
        : this(method, priority, args, numArgs, false)
    {
    }

    internal DispatcherOperation(Delegate method, DispatcherPriority priority, object args, int numArgs, bool useAsyncSemantics)
    {
        _method = method;
        _priority = priority;
        _args = args;
        _numArgs = numArgs;
        _useAsyncSemantics = useAsyncSemantics;
    }

    /// <summary>
    /// Occurs when the operation has completed.
    /// </summary>
    public event EventHandler Completed;

    /// <summary>
    /// Occurs when the operation is aborted.
    /// </summary>
    public event EventHandler Aborted;

    /// <summary>
    /// Gets the priority of the operation in the <see cref="Dispatcher"/> queue.
    /// </summary>
    /// <returns>
    /// The priority of the delegate on the queue.
    /// </returns>
    public DispatcherPriority Priority => _priority;

    /// <summary>
    /// Gets the current status of the operation.
    /// </summary>
    /// <returns>
    /// The status of the operation.
    /// </returns>
    public DispatcherOperationStatus Status => _status;

    /// <summary>
    /// Gets the result of the operation after it has completed.
    /// </summary>
    /// <returns>
    /// The result of the operation -or- null if the operation has not completed.
    /// </returns>
    public object Result => _result;

    internal Delegate Method => _method;

    /// <summary>
    /// Aborts the operation.
    /// </summary>
    /// <returns>
    /// true if the operation was aborted; otherwise, false.
    /// </returns>
    public bool Abort()
    {
        if (_status == DispatcherOperationStatus.Pending)
        {
            _status = DispatcherOperationStatus.Aborted;
            Aborted?.Invoke(this, EventArgs.Empty);

            return true;
        }

        return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Invoke()
    {
        if (_status != DispatcherOperationStatus.Pending)
        {
            throw new Exception($"Operation is \"{_status}\" and cannot be invoked");
        }

        _status = DispatcherOperationStatus.Executing;

        _result = InvokeImpl();
        
        _status = DispatcherOperationStatus.Completed;

        Completed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Begins the operation that is associated with this <see cref="DispatcherOperation"/>.
    /// </summary>
    /// <returns>
    /// null in all cases.
    /// </returns>
    protected virtual object InvokeDelegateCore()
    {
        Action action = (Action)_method;
        action();
        return null;
    }

    private object InvokeImpl()
    {
        object result;

        if (_useAsyncSemantics)
        {
            result = InvokeDelegateCore();
        }
        else
        {
            result = LegacyInvoke(_method, _args, _numArgs);
        }

        return result;
    }

    private static object LegacyInvoke(Delegate callback, object args, int numArgs)
    {
        object result = null;

        Debug.Assert(numArgs == 0 || numArgs == 1 || numArgs == -1);

        // Support the fast-path for certain 0-param and 1-param delegates, even of an arbitrary
        // "params object[]" is passed.
        int numArgsEx = numArgs;
        object singleArg = args;
        if (numArgs == -1)
        {
            object[] argsArr = (object[])args;
            if (argsArr is null || argsArr.Length == 0)
            {
                numArgsEx = 0;
            }
            else if (argsArr.Length == 1)
            {
                numArgsEx = 1;
                singleArg = argsArr[0];
            }
        }

        // Special-case delegates that we know about to avoid the expensive DynamicInvoke call.
        if (numArgsEx == 0)
        {
            if (callback is Action action)
            {
                action();
            }
            else
            {
                // The delegate could return anything.
                result = callback.DynamicInvoke();
            }
        }
        else if (numArgsEx == 1)
        {
            if (callback is DispatcherOperationCallback dispatcherOperationCallback)
            {
                result = dispatcherOperationCallback(singleArg);
            }
            else if (callback is SendOrPostCallback sendOrPostCallback)
            {
                sendOrPostCallback(singleArg);
            }
            else
            {
                if (numArgs == -1)
                {
                    // Explicitly pass an object[] to DynamicInvoke so that
                    // it will not try to wrap the arg in another object[].
                    result = callback.DynamicInvoke((object[])args);
                }
                else
                {
                    // By pass the args parameter as a single object,
                    // DynamicInvoke will wrap it in an object[] due to the
                    // params keyword.
                    result = callback.DynamicInvoke(args);
                }
            }
        }
        else
        {
            // Explicitly pass an object[] to DynamicInvoke so that
            // it will not try to wrap the arg in another object[].
            result = callback.DynamicInvoke((object[])args);
        }

        return result;
    }
}

public class DispatcherOperation<TResult> : DispatcherOperation
{
    internal DispatcherOperation(DispatcherPriority priority, Func<TResult> func)
        : base(func, priority, null, 0, true)
    {
    }

    /// <summary>
    /// Gets the result of the operation after it has completed.
    /// </summary>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    public new TResult Result => (TResult)base.Result;

    /// <summary>
    /// Begins the operation that is associated with this <see cref="DispatcherOperation"/>.
    /// </summary>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    protected override object InvokeDelegateCore()
    {
        Func<TResult> func = (Func<TResult>)Method;
        return func();
    }
}

/// <summary>
/// Represents a delegate to use for dispatcher operations.
/// </summary>
/// <param name="arg">
/// An argument passed to the callback.
/// </param>
/// <returns>
/// The object returned by the callback.
/// </returns>
public delegate object DispatcherOperationCallback(object arg);
