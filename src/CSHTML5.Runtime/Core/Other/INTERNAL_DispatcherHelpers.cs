
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
        static CoreDispatcher _dispatcher = null;

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
                    _dispatcher = CoreDispatcher.INTERNAL_GetCurrentDispatcher();

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
