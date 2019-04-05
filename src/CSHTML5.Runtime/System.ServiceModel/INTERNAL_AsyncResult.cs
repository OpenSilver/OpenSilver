
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
