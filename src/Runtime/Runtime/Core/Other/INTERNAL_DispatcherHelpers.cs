

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace CSHTML5.Internal
{
    public static class INTERNAL_DispatcherHelpers
    {
        /* john.torjo - I fixed the following:
         *
         SIMULATOR-caught exception: System.ArgumentException: Destination array was not long enough. Check destIndex and length, and the array's lower bounds.
           at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length, Boolean reliable)
           at System.Collections.Generic.List`1.CopyTo(T[] array, Int32 arrayIndex)
           at System.Collections.Generic.List`1..ctor(IEnumerable`1 collection)
           at CSHTML5.Internal.INTERNAL_DispatcherHelpers.ExecutePendingActions()
           at CSHTML5.Internal.INTERNAL_DispatcherHelpers.<>c.<QueueAction>b__3_0()
           at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
           at System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)

        the only way I could see this happening (before my fix), was if the list was being added to, while the 'copy' was being constructed
         *
         */
        private static object _lock = new object();
        private static List<Action> _pendingActionsExecute = new List<Action>();
        private static bool _isDispatcherPending = false;
#if MIGRATION
        static Dispatcher _dispatcher = null;
#else
        static CoreDispatcher _dispatcher = null;
#endif

        /// <summary>
        /// This will add the action to a list of actions to perform at once in a Dispatcher.Invoke call.
        /// This has better performance than calling Dispatcher.BeginInvoke directly, because all the
        /// queued actions are performed together in a single Dispatch.BeginInvoke call.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        public static void QueueAction(Action action)
        {
            lock (_lock) {
                _pendingActionsExecute.Add(action);
                if (_isDispatcherPending)
                    return;
                _isDispatcherPending = true;
            }

            if (_dispatcher == null)
            {
#if MIGRATION
                var dispatcher = Dispatcher.INTERNAL_GetCurrentDispatcher();
#else
                var dispatcher = CoreDispatcher.INTERNAL_GetCurrentDispatcher();
#endif
                lock (_lock)
                    _dispatcher = dispatcher;
            }

            _dispatcher.BeginInvoke(ExecutePendingActions);
        }

        static void ExecutePendingActions() {
            List<Action> copy;
            lock (_lock) {
                if (!_isDispatcherPending)
                    return;

                _isDispatcherPending = false;
                copy = new List<Action>(_pendingActionsExecute);
                _pendingActionsExecute.Clear();
            }

            foreach (Action action in copy)
                action();
        }
    }
}
