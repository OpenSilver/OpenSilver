using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    public class HttpWebRequest : WebRequest //, ISerializable
    {
        #region Data
        private Uri _requestUri;
        private string _methodName;
        private WebHeaderCollection _headers;
        #endregion

        #region Constructor
        public HttpWebRequest()
        {
            this._methodName = "GET";
            this._headers = new WebHeaderCollection();
        }

        internal HttpWebRequest(Uri requestUri) : this()
        {
            this._requestUri = requestUri;
        }
        #endregion

        #region Public Properties
        public override Uri RequestUri
        {
            get
            {
                return this._requestUri;
            }
        }

        public override string Method
        {
            get
            {
                return this._methodName;
            }
            set
            {
                this._methodName = value;
            }
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                return this._headers;
            }
            set
            {
                this._headers = value;
            }
        }
        #endregion

        #region Public Methods
        public override WebResponse GetResponse()
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            var result = new HttpRequestAsyncResult(callback, state);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach(var key in this.Headers.AllKeys)
            {
                headers.Add(key, this.Headers.Get(key));
            }
            this._webRequestHelper.MakeRequest(this.RequestUri, this.Method, this, headers, null, (o, e) => callback(result), true, CredentialsMode.Disabled);
            return result;
        }

        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            HttpRequestAsyncResult castedAsyncResult = asyncResult as HttpRequestAsyncResult;
            if(castedAsyncResult == null)
            {
                throw new ArgumentException("asyncResult");
            }

            //string xmlReturnedFromTheServer = castedAsyncResult.XmlReturnedFromTheServer;
            HttpWebResponse response = new HttpWebResponse(this._webRequestHelper.GetXmlHttpRequest());
            return response;
        }
        #endregion
    }

    internal class HttpRequestAsyncResult : INTERNAL_AsyncResult
    {
        //internal string XmlReturnedFromTheServer { get; set; }

        internal HttpRequestAsyncResult(AsyncCallback callBack, object state) : base(callBack, state)
        {

        }
    }
}
