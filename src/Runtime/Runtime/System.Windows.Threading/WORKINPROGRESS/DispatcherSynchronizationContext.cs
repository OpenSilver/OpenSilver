#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#if !MIGRATION
using Windows.UI.Core;
#endif

namespace System.Windows.Threading
{
    public partial class DispatcherSynchronizationContext : SynchronizationContext
    {
        public DispatcherSynchronizationContext()
        {

        }

#if MIGRATION
        public DispatcherSynchronizationContext(Dispatcher dispatcher)
#else
        public DispatcherSynchronizationContext(CoreDispatcher dispatcher)
#endif
        {

        }
    }
}

#endif