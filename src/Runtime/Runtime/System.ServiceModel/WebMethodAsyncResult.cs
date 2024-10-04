
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

namespace System.ServiceModel;

internal sealed class WebMethodAsyncResult : IAsyncResult, IDisposable
{
    private AsyncCallback _callback;
    private object _state;
    private bool _isCompleted;

    public WebMethodAsyncResult(AsyncCallback callback, object state)
    {
        _callback = callback;
        _state = state;
    }

    public string XmlReturnedFromTheServer { get; set; }

    public void Completed()
    {
        _isCompleted = true;
        _callback?.Invoke(this);
    }

    bool IAsyncResult.IsCompleted => _isCompleted;

    WaitHandle IAsyncResult.AsyncWaitHandle => null;

    object IAsyncResult.AsyncState => _state;

    bool IAsyncResult.CompletedSynchronously => false;

    void IDisposable.Dispose()
    {
        _state = null;
        _callback = null;
    }
}
