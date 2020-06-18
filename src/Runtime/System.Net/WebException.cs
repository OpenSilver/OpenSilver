
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



using System;
using System.Runtime.Serialization;

namespace System.Net
{
    public partial class WebException : InvalidOperationException //, ISerializable
    {
        private WebExceptionStatus m_Status = WebExceptionStatus.UnknownError;
        private WebResponse m_Response;
        private WebExceptionInternalStatus m_InternalStatus = WebExceptionInternalStatus.RequestFatal;

        public WebException()
        {

        }

        public WebException(string message)
            : this(message, null)
        {
        }

        public WebException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public WebException(string message, WebExceptionStatus status) :
            this(message, null, status, null)
        {
        }

        internal WebException(string message, WebExceptionStatus status, WebExceptionInternalStatus internalStatus, Exception innerException) :
            this(message, innerException, status, null, internalStatus)
        {
        }

        public WebException(string message,
                            Exception innerException,
                            WebExceptionStatus status,
                            WebResponse response) :
            this(message, null, innerException, status, response)
        { }

        internal WebException(string message, string data, Exception innerException, WebExceptionStatus status, WebResponse response) :
            base(message + (data != null ? ": '" + data + "'" : ""), innerException)
        {
            m_Status = status;
            m_Response = response;
        }

        internal WebException(string message,
                            Exception innerException,
                            WebExceptionStatus status,
                            WebResponse response,
                            WebExceptionInternalStatus internalStatus) :
            this(message, null, innerException, status, response, internalStatus)
        { }

        internal WebException(string message, string data, Exception innerException, WebExceptionStatus status, WebResponse response, WebExceptionInternalStatus internalStatus) :
            base(message + (data != null ? ": '" + data + "'" : ""), innerException)
        {
            m_Status = status;
            m_Response = response;
            m_InternalStatus = internalStatus;
        }


        protected WebException(SerializationInfo serializationInfo, StreamingContext streamingContext)

        //TODOBRIDGE: if this exception is important see SerializationInfo and StreamingContext in Bridge
#if !BRIDGE
            : base(serializationInfo, streamingContext)
#endif
        {
            // m_Status = (WebExceptionStatus)serializationInfo.GetInt32("Status");
            // m_InternalStatus = (WebExceptionInternalStatus)serializationInfo.GetInt32("InternalStatus");
        }

        public WebExceptionStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        public WebResponse Response
        {
            get
            {
                return m_Response;
            }
        }

        internal WebExceptionInternalStatus InternalStatus
        {
            get
            {
                return m_InternalStatus;
            }
        }

    };

    internal enum WebExceptionInternalStatus
    {
        RequestFatal = 0,
        ServicePointFatal = 1,
        Recoverable = 2,
        Isolated = 3,
    }

}