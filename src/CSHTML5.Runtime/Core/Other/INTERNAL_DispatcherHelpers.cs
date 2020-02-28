

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
#if WORKINPROGRESS
using System.Windows.Threading;
using System.Windows;
#else
using System.Windows;
#endif
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
        static List<Action> _pendingActionsExecute = new List<Action>();
        static bool _isDispatcherPending = false;
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
            _pendingActionsExecute.Add(action);

            if (!_isDispatcherPending)
            {
                _isDispatcherPending = true;

                if (_dispatcher == null)
                {
#if MIGRATION
                    _dispatcher = Dispatcher.INTERNAL_GetCurrentDispatcher();
#else
                    _dispatcher = CoreDispatcher.INTERNAL_GetCurrentDispatcher();
#endif
                }

                _dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (_isDispatcherPending) // We check again in case it has been cancelled - not sure if useful though
                        {
                            ExecutePendingActions();
                        }
                    }));
            }
        }

        static void ExecutePendingActions()
        {
            _isDispatcherPending = false;

            List<Action> copy = new List<Action>(_pendingActionsExecute);

            _pendingActionsExecute.Clear();

            foreach (Action action in copy)
            {
                action();
            }
        }
    }
}
