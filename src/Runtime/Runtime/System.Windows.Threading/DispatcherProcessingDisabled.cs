
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

using System.Diagnostics;
using System.Threading;

namespace System.Windows.Threading;

/// <summary>
/// Represents the <see cref="Dispatcher"/> when it is in a disable state and 
/// provides a means to re-enable dispatcher processing.
/// </summary>
public struct DispatcherProcessingDisabled : IDisposable
{
    private Dispatcher _dispatcher;

    internal DispatcherProcessingDisabled(Dispatcher dispatcher)
    {
        Debug.Assert(dispatcher is not null);
        _dispatcher = dispatcher;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _dispatcher, null) is Dispatcher dispatcher)
        {
            dispatcher.EnableProcessing();
        }
    }
}
