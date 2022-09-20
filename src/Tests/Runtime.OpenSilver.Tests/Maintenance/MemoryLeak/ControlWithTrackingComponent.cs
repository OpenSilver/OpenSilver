
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
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Runtime.OpenSilver.Tests.Maintenance.MemoryLeak
{
    internal class ControlWithTrackingComponent : Control
    {
        private readonly GarbageCollectorTracker _gcTracker;

        public ControlWithTrackingComponent(GarbageCollectorTracker gcTracker)
        {
            _gcTracker = gcTracker;
        }

        ~ControlWithTrackingComponent() => _gcTracker?.MarkAsCollected();
    }
}
