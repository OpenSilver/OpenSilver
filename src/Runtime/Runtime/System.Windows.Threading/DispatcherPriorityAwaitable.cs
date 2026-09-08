
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

namespace System.Windows.Threading;

/// <summary>
/// Represents an awaitable object that asynchronously yields control back to the current dispatcher 
/// and provides an opportunity for the dispatcher to process other events.
/// </summary>
public readonly struct DispatcherPriorityAwaitable
{
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherPriority _priority;

    internal DispatcherPriorityAwaitable(Dispatcher dispatcher, DispatcherPriority priority)
    {
        _dispatcher = dispatcher;
        _priority = priority;
    }

    /// <summary>
    /// Returns an object that waits for the completion of an asynchronous task.
    /// </summary>
    /// <returns>
    /// An object that waits for the completion of an asynchronous task.
    /// </returns>
    public DispatcherPriorityAwaiter GetAwaiter() => new(_dispatcher, _priority);
}
