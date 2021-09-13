using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#if !MIGRATION
using Windows.UI.Core;
#endif

namespace System.Windows.Threading
{
	[OpenSilver.NotImplemented]
    public partial class DispatcherSynchronizationContext : SynchronizationContext
    {
		[OpenSilver.NotImplemented]
        public DispatcherSynchronizationContext()
        {

        }

		[OpenSilver.NotImplemented]
#if MIGRATION
        public DispatcherSynchronizationContext(Dispatcher dispatcher)
#else
        public DispatcherSynchronizationContext(CoreDispatcher dispatcher)
#endif
        {

        }
    }
}
