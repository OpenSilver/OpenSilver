
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

using OpenSilver.Internal;
using System.Runtime.CompilerServices;

namespace System.Windows.Threading;

/// <summary>
/// Represents an object that waits for the completion of an asynchronous task.
/// </summary>
public readonly struct DispatcherPriorityAwaiter : INotifyCompletion
{
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherPriority _priority;

    internal DispatcherPriorityAwaiter(Dispatcher dispatcher, DispatcherPriority priority)
    {
        _dispatcher = dispatcher;
        _priority = priority;
    }

    /// <summary>
    /// Gets a value that indicates whether the asynchronous task has completed.
    /// </summary>
    /// <returns>
    /// false in all cases.
    /// </returns>
    public bool IsCompleted => false;

    /// <summary>
    /// Ends the wait for the completion of the asynchronous task.
    /// </summary>
    public void GetResult() { }

    /// <summary>
    /// Sets the action to perform when the <see cref="DispatcherPriorityAwaiter"/> object 
    /// stops waiting for the asynchronous task to complete.
    /// </summary>
    /// <param name="continuation">
    /// The action to perform when the wait operation completes.
    /// </param>
    public void OnCompleted(Action continuation)
    {
        if (_dispatcher is null)
        {
            throw new InvalidOperationException(Strings.DispatcherPriorityAwaiterInvalid);
        }

        _dispatcher.InvokeAsync(continuation, _priority);
    }
}
