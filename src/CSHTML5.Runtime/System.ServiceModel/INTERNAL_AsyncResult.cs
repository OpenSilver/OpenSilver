

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


namespace System.ServiceModel
{
    using System.Threading;
    using System;
    using System.Security.Permissions;

    internal partial class INTERNAL_AsyncResult : IAsyncResult, IDisposable
    {
        //cf. https://rashimuddin.wordpress.com/2014/02/08/iasyncresult-asynchronous-service-in-wcf/

        private AsyncCallback _callback;
        private object _state;
#if MULTITHREADSUPPORTED
        private ManualResetEvent _manualResetEvent;
#else
        bool _isCompleted;
#endif

        public INTERNAL_AsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            _state = state;
#if MULTITHREADSUPPORTED
            _manualResetEvent = new ManualResetEvent(false);
#endif
        }

        public bool IsCompleted
        {
#if MULTITHREADSUPPORTED
            get { return _manualResetEvent.WaitOne(0, false); }
#else
            get { return _isCompleted; }
#endif
        }

        public WaitHandle AsyncWaitHandle
        {
#if MULTITHREADSUPPORTED
            get { return _manualResetEvent; }
#else
            get { return null; }
#endif
        }

        public object AsyncState
        {
            get { return _state; }
        }

#if MULTITHREADSUPPORTED
        public ManualResetEvent AsyncWait
        {
            get { return _manualResetEvent; }

        }
#endif

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void Completed()
        {
#if MULTITHREADSUPPORTED
            _manualResetEvent.Set();
#else
            _isCompleted = true;
#endif
            if (_callback != null)
                _callback(this);
        }

        public void Dispose()
        {
#if MULTITHREADSUPPORTED
            _manualResetEvent.Close();
            _manualResetEvent = null;
#endif
            _state = null;
            _callback = null;
        }
    }


}
