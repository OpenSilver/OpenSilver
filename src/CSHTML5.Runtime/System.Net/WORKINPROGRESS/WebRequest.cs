using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    public abstract partial class WebRequest// : MarshalByRefObject, ISerializable
    {
        #region Data
        internal INTERNAL_WebRequestHelper_JSOnly _webRequestHelper;
        #endregion

        #region Constructors
        protected WebRequest()
        {
            this._webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();
        }

        public static WebRequest Create(string requestUriString)
        {
            if (requestUriString == null)
            {
                throw new ArgumentNullException("requestUriString");
            }
            return CreateHttp(new Uri(requestUriString));
        }

        public static WebRequest Create(Uri requestUri)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }
            return CreateHttp(requestUri);
        }

        public static HttpWebRequest CreateHttp(string requestUriString)
        {
            if (requestUriString == null)
            {
                throw new ArgumentNullException("requestUriString");
            }
            return CreateHttp(new Uri(requestUriString));
        }

        public static HttpWebRequest CreateHttp(Uri requestUri)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }
            return (HttpWebRequest)CreateDefault(requestUri);
        }

        public static WebRequest CreateDefault(Uri requestUri)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }
            HttpWebRequest request = new HttpWebRequest(requestUri);
            return request;
        }
        #endregion

        #region Public Properties
        public virtual Uri RequestUri
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string Method
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual WebHeaderCollection Headers
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Public Methods
        public virtual WebResponse GetResponse()
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
