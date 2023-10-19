
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

namespace System.Windows.Threading;

/// <summary>
/// Represents an operation that has been posted to the <see cref="Dispatcher"/> queue.
/// </summary>
public sealed class DispatcherOperation
{
    private readonly Action _action;

    internal DispatcherOperation() { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation(Action action, DispatcherPriority priority)
    {
        _action = action;
        Priority = priority;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DispatcherOperation(Func<object> action, DispatcherPriority priority)
    {
        _action = () => Result = action();
        Priority = priority;
    }

    public event EventHandler Completed;

    public event EventHandler Aborted;

    public DispatcherPriority Priority { get; private set; }
    
    public DispatcherOperationStatus Status { get; private set; }
    
    public object Result { get; private set; }

    public void Abort()
    {
        if (Status != DispatcherOperationStatus.Pending)
        {
            throw new Exception($"Operation is \"{Status}\" and cannot be aborted");
        }

        Status = DispatcherOperationStatus.Aborted;

        Aborted?.Invoke(this, EventArgs.Empty);
    }

    public void Invoke()
    {
        if (Status != DispatcherOperationStatus.Pending)
        {
            throw new Exception($"Operation is \"{Status}\" and cannot be invoked");
        }

        Status = DispatcherOperationStatus.Executing;
        
        _action();
        
        Status = DispatcherOperationStatus.Completed;

        Completed?.Invoke(this, EventArgs.Empty);
    }
}
