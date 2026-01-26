
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

using System.Diagnostics;
using System.Threading;

namespace System.Windows.Threading;

/// <summary>
/// Provides a synchronization context for Windows Presentation Foundation (WPF).
/// </summary>
public class DispatcherSynchronizationContext : SynchronizationContext
{
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherPriority _priority;

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherSynchronizationContext"/> class by using the 
    /// current <see cref="Dispatcher"/>.
    /// </summary>
    public DispatcherSynchronizationContext()
        : this(Dispatcher.CurrentDispatcher, DispatcherPriority.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherSynchronizationContext"/> class by using the 
    /// specified <see cref="Dispatcher"/>.
    /// </summary>
    /// <param name="dispatcher">
    /// The <see cref="Dispatcher"/> to associate this <see cref="DispatcherSynchronizationContext"/> with.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dispatcher"/> is null.
    /// </exception>
    public DispatcherSynchronizationContext(Dispatcher dispatcher)
        : this(dispatcher, DispatcherPriority.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherSynchronizationContext"/> class by using the 
    /// specified <see cref="Dispatcher"/>.
    /// </summary>
    /// <param name="dispatcher">
    /// The <see cref="Dispatcher"/> to associate this <see cref="DispatcherSynchronizationContext"/> with.
    /// </param>
    /// <param name="priority">
    /// The priority used to send and post callback methods.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dispatcher"/> is null.
    /// </exception>
    public DispatcherSynchronizationContext(Dispatcher dispatcher, DispatcherPriority priority)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);

        Dispatcher.ValidatePriority(priority);

        _dispatcher = dispatcher;
        _priority = priority;
    }

    /// <summary>
    /// Creates a copy of this <see cref="DispatcherSynchronizationContext"/>.
    /// </summary>
    /// <returns>
    /// The copy of this synchronization context.
    /// </returns>
    public override SynchronizationContext CreateCopy() => new DispatcherSynchronizationContext(_dispatcher, _priority);

    /// <summary>
    /// Invokes the callback in the synchronization context asynchronously.
    /// </summary>
    /// <param name="d">
    /// The delegate to call.
    /// </param>
    /// <param name="state">
    /// The object passed to the delegate.
    /// </param>
    public override void Post(SendOrPostCallback d, object state) => _dispatcher.BeginInvoke(_priority, d, state);

    /// <summary>
    /// Invokes the callback in the synchronization context synchronously.
    /// </summary>
    /// <param name="d">
    /// The delegate to call.
    /// </param>
    /// <param name="state">
    /// The object passed to the delegate.
    /// </param>
    public override void Send(SendOrPostCallback d, object state)
    {
        if (_dispatcher.CheckAccess())
        {
            d(state);
        }
        else
        {
            Debug.Assert(OpenSilver.Interop.IsRunningInTheSimulator);

            var waitHandle = new ManualResetEvent(false);

            var operation = _dispatcher.BeginInvoke(
                DispatcherPriority.Send,
                new SendOrPostCallback(static (arg) =>
                {
                    (var d, var state, var waitHandle) = ((SendOrPostCallback, object, ManualResetEvent))arg;

                    try
                    {
                        d(state);
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                }),
                (d, state, waitHandle));

            waitHandle.WaitOne();
        }
    }
}
