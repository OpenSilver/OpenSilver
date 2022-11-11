
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

namespace OpenSilver.MemoryLeak;

public class GCTracker
{
    public bool IsCollected => CollectedResetEvent.WaitOne(0);

    public ManualResetEvent CollectedResetEvent { get; } = new ManualResetEvent(false);

    public void MarkAsCollected() => CollectedResetEvent.Set();
}