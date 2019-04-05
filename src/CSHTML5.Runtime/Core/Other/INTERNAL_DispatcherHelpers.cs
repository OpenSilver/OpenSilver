
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
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
    internal static class INTERNAL_DispatcherHelpers
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
        /// This has better performance than calling Dispatcher.BeginInvoke directly.
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
