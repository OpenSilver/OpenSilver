

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
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

namespace CSHTML5.Internal
{
    public static class INTERNAL_DispatcherHelpers
    {
        private static readonly object _lock = new object();
        private static readonly List<Action> _pendingActionsExecute = new List<Action>();
        private static bool _isDispatcherPending = false;
        private static Dispatcher _dispatcher = null;

        /// <summary>
        /// This will add the action to a list of actions to perform at once in a Dispatcher.Invoke call.
        /// This has better performance than calling Dispatcher.BeginInvoke directly, because all the
        /// queued actions are performed together in a single Dispatch.BeginInvoke call.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        public static void QueueAction(Action action)
        {
            lock (_lock)
            {
                _pendingActionsExecute.Add(action);
                
                if (_isDispatcherPending)
                {
                    return;
                }
                _isDispatcherPending = true;

                if (_dispatcher == null)
                {
                    _dispatcher = Dispatcher.INTERNAL_GetCurrentDispatcher();
                }
            }

            _dispatcher.BeginInvoke(ExecutePendingActions);
        }

        private static void ExecutePendingActions()
        {
            Action[] copy;
            lock (_lock)
            {
                if (!_isDispatcherPending)
                {
                    return;
                }

                _isDispatcherPending = false;
                copy = _pendingActionsExecute.ToArray();
                _pendingActionsExecute.Clear();
            }

            foreach (Action action in copy)
            {
                action();
            }
        }
    }
}
