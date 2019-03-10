
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================



namespace System.ServiceModel
{
    using System.Threading;
    using System;
    using System.Security.Permissions;

    internal class INTERNAL_AsyncResult : IAsyncResult, IDisposable
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
