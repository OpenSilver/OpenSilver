
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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Runtime.OpenSilver.Tests.Maintenance.MemoryLeak
{
    internal class DependencyObjectWithTrackingComponent : DependencyObject
    {
        private readonly GarbageCollectorTracker _gcTracker;

        public DependencyObjectWithTrackingComponent(GarbageCollectorTracker gcTracker)
        {
            _gcTracker = gcTracker;
        }

        ~DependencyObjectWithTrackingComponent() => _gcTracker?.MarkAsCollected();
    }
}
