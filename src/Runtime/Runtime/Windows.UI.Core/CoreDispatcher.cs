

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
#if WORKINPROGRESS
namespace System.Windows.Threading
#else
namespace System.Windows
#endif
#else
namespace Windows.UI.Core
#endif
{
#if WORKINPROGRESS
    public enum DispatcherPriority
    {
        Invalid = -1,
        Inactive = 0,
        SystemIdle = 1,
        ApplicationIdle = 2,
        ContextIdle = 3,
        Background = 4,
        Input = 5,
        Loaded = 6,
        Render = 7,
        DataBind = 8,
        Normal = 9,
        Send = 10,
    }
#endif

    /// <summary>
    /// Provides the core event message dispatcher. Instances of this type are responsible
    /// for processing the window messages and dispatching the events to the client.
    /// </summary>
#if MIGRATION
    public partial class Dispatcher
#else
    public partial class CoreDispatcher
#endif
    {
#if MIGRATION
        static Dispatcher _currentDispatcher;
#else
        static CoreDispatcher _currentDispatcher;
#endif

#if MIGRATION
        internal static Dispatcher INTERNAL_GetCurrentDispatcher()
#else
        internal static CoreDispatcher INTERNAL_GetCurrentDispatcher()
#endif
        {
            if (_currentDispatcher == null)
            {
#if MIGRATION
                _currentDispatcher = new Dispatcher();
#else
                _currentDispatcher = new CoreDispatcher();
#endif
            }
            return _currentDispatcher;
        }

        //todo: this does not exactly correspond to the actual method --> add args to BeginInvoke?
        // Summary:
        //     Executes the specified delegate asynchronously with the specified arguments
        //     on the thread that the System.Windows.Threading.Dispatcher was created on.
        //
        // Parameters:
        //   method:
        //     The delegate to a method that takes parameters specified in args, which is
        //     pushed onto the System.Windows.Threading.Dispatcher event queue.
        //
        //   args:
        //     An array of objects to pass as arguments to the given method. Can be null.
        //
        // Returns:
        //     An object, which is returned immediately after Overload:System.Windows.Threading.Dispatcher.BeginInvoke
        //     is called, that can be used to interact with the delegate as it is pending
        //     execution in the event queue.
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the System.Windows.Threading.Dispatcher was created on.
        /// </summary>
        /// <param name="method">
        /// The delegate to a method, which is
        /// pushed onto the System.Windows.Threading.Dispatcher event queue.
        /// </param>
#if WORKINPROGRESS
        public DispatcherOperation BeginInvoke(Action method)
#else
        public void BeginInvoke(Action method)
#endif
        {
            if (method == null)
                throw new ArgumentNullException("method");
#if WORKINPROGRESS
            DispatcherOperation dispatcherOperation = new DispatcherOperation();
            BeginInvokeInternal(method);
            return dispatcherOperation;
#else
            BeginInvokeInternal(method);
#endif
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("setTimeout(function(){$method();},1);")]
#else
        [Template("setTimeout(function(){ {method}(); },1)")]
#endif
        static void BeginInvokeInternal(Action method)
        {
#if CSHTML5NETSTANDARD
            //Console.WriteLine("ON BEGININVOKE CALLED");
            if (!INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("setTimeout($0, 1)",
                    (Action)(() =>
                    {
                        try
                        {
                            method();
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("DEBUG: CoreDispatcher: BeginIvokeInternal: Method excution failed: " + e);
                        }
                    }));
            }
            else
            {
#endif
            //Simulator only. We call the JavaScript "setTimeout" to queue the action on the thread, and then we call Dispatcher.BeginInvoke(...) to ensure that it runs in the UI thread.
            //CSHTML5.Interop.ExecuteJavaScriptAsync("setTimeout($0, 1)",
            //    (Action)(() =>
            //    {
            INTERNAL_Simulator.WebControlDispatcherBeginInvoke(method);
            //}));
#if CSHTML5NETSTANDARD
            }
#endif
            // Note: the implementation below was commented because it led to issues when drawing the Shape controls: sometimes some Shape controls did not render in the Simulator because the method passed "Dispatcher.Begin()" was called too early.
            /*
            global::System.Threading.Timer timer = new global::System.Threading.Timer(
                delegate(object state)
                {
                    INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        method();
                    }));
                },
                null,
                1,
                global::System.Threading.Timeout.Infinite);
             */
        }

#if WORKINPROGRESS
        private PriorityQueue<DispatcherOperation> queue;
        private int disableProcessingRequests;
        private IDisposable disableProcessingToken;
        private bool isProcessQueueScheduled;

        public Dispatcher()
        {
            queue = new PriorityQueue<DispatcherOperation>((int)DispatcherPriority.Send + 1);
            disableProcessingToken = new Disposable(EnableProcessing);
        }
        public IDisposable DisableProcessing()
        {
            disableProcessingRequests++;
            return disableProcessingToken;
        }

        private void EnableProcessing()
        {
            disableProcessingRequests--;

            if (disableProcessingRequests == 0)
            {
                ProcessQueueAsync();
            }
        }
        public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            DispatcherOperation dispatcherOperation = new DispatcherOperation(callback, priority);
            InvokeAsync(dispatcherOperation);
            return dispatcherOperation;
        }
        private void InvokeAsync(DispatcherOperation operation)
        {
            queue.Enqueue((int)operation.Priority, operation);
            ProcessQueueAsync();
        }
        private void ProcessQueueAsync()
        {
            if (isProcessQueueScheduled)
            {
                return;
            }

            isProcessQueueScheduled = true;
            Action action = () =>
            {
                isProcessQueueScheduled = false;

                DispatcherOperation operation;
                if (!TryDequeue(out operation))
                {
                    return;
                }

                if (operation.Status == DispatcherOperationStatus.Pending)
                {
                    operation.Invoke();
                    ProcessQueueAsync();
                }
            };
            action.BeginInvoke(ar => action.EndInvoke(ar), null);
            //ApplicationHost.Current.TaskScheduler.ScheduleTask(action);
        }
        private bool TryDequeue(out DispatcherOperation operation)
        {
            while (disableProcessingRequests == 0 && queue.TryPeek(out operation) && operation.Priority != DispatcherPriority.Inactive)
            {
                queue.Dequeue();

                if (operation.Status != DispatcherOperationStatus.Pending)
                {
                    continue;
                }

                return true;
            }

            operation = null;
            return false;
        }
		[OpenSilver.NotImplemented]
        public DispatcherOperation BeginInvoke(Delegate d, params object[] args)
        {
            return null;
        }

		[OpenSilver.NotImplemented]
        public bool CheckAccess()
        {
            return true;
        }
#endif
    }
}