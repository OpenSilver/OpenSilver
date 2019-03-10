
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


using System;
using System.Runtime.Serialization;

namespace System.Net
{
    public class WebException : InvalidOperationException //, ISerializable
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