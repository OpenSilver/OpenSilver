
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


using System.Threading;

namespace Runtime.OpenSilver.Tests.Maintenance.MemoryLeak
{
    class ItemWithTrackableCallback
    {
        private readonly GarbageCollectorTracker _gcTracker;
        private readonly ManualResetEvent _manualResetEvent;

        public ItemWithTrackableCallback(GarbageCollectorTracker gcTracker, ManualResetEvent manualResetEvent)
        {
            _gcTracker = gcTracker;
            _manualResetEvent = manualResetEvent;
        }

        ~ItemWithTrackableCallback()
        {
            _gcTracker.MarkAsCollected();
        }

        public void Callback()
        {
            _manualResetEvent.Set();
        }
    }
}