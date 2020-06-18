#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#if !MIGRATION
using Microsoft.AspNetCore.Components;
#endif

namespace System.Windows.Threading
{
    public partial class DispatcherSynchronizationContext : SynchronizationContext
    {
        public DispatcherSynchronizationContext()
        {

        }

        public DispatcherSynchronizationContext(Dispatcher dispatcher)
        {

        }
    }
}

#endif