
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{

#if WORK_IN_PROGRESS_HTTP_WEB_REQUEST

    // Summary:
    //     Provides an HTTP-specific implementation of the System.Net.WebRequest class.
    /// <summary>
    /// Provides an HTTP-specific implementation of the System.Net.WebRequest class.
    /// </summary>
    [Serializable]
    //[FriendAccessAllowed]
    public class HttpWebRequest : WebRequest//, ISerializable
    {
        INTERNAL_WebRequestHelper _webRequestHelper = new INTERNAL_WebRequestHelper();


        Uri _address;
        // Returns:
        //     A System.Uri that identifies the Internet resource that actually responds
        //     to the request. The default is the URI used by the System.Net.WebRequest.Create(System.String)
        //     method to initialize the request.
        /// <summary>
        /// Gets the Uniform Resource Identifier (URI) of the Internet resource that
        /// actually responds to the request.
        /// </summary>
        public Uri Address
        {
            get { return _address; }
            internal set { _address = value; }
        }



        private long _contentLength = -1;
        // Returns:
        //     The number of bytes of data to send to the Internet resource. The default
        //     is -1, which indicates the property has not been set and that there is no
        //     request data to send.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The request has been started by calling the System.Net.HttpWebRequest.GetRequestStream(),
        //     System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object),
        //     System.Net.HttpWebRequest.GetResponse(), or System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
        //     method.
        //
        //   System.ArgumentOutOfRangeException:
        //     The new System.Net.HttpWebRequest.ContentLength value is less than 0.
        /// <summary>
        /// Gets or sets the Content-length HTTP header.
        /// </summary>
        public long ContentLength//todo: this is supposed to be an override
        {
            get { return _contentLength; }
            set { _contentLength = value; }
        }

        // Returns:
        //     The value of the Content-type HTTP header. The default value is null.
        /// <summary>
        /// Gets or sets the value of the Content-type HTTP header.
        /// </summary>
        public string ContentType { get; set; } //todo: this is supposed to be an override


     
        // Returns:
        //     The request method to use to contact the Internet resource. The default value
        //     is GET.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     No method is supplied.-or- The method string contains invalid characters.
        /// <summary>
        /// Gets or sets the method for the request.
        /// </summary>
        public string Method { get; set; } //todo:this is supposed to be an override


        private Stream _stream;
        // Exceptions:
        //   System.Net.ProtocolViolationException:
        //     The System.Net.HttpWebRequest.Method property is GET or HEAD.-or- System.Net.HttpWebRequest.KeepAlive
        //     is true, System.Net.HttpWebRequest.AllowWriteStreamBuffering is false, System.Net.HttpWebRequest.ContentLength
        //     is -1, System.Net.HttpWebRequest.SendChunked is false, and System.Net.HttpWebRequest.Method
        //     is POST or PUT.
        //
        //   System.InvalidOperationException:
        //     The System.Net.HttpWebRequest.GetRequestStream() method is called more than
        //     once.-or- System.Net.HttpWebRequest.TransferEncoding is set to a value and
        //     System.Net.HttpWebRequest.SendChunked is false.
        //
        //   System.NotSupportedException:
        //     The request cache validator indicated that the response for this request
        //     can be served from the cache; however, requests that write data must not
        //     use the cache. This exception can occur if you are using a custom cache validator
        //     that is incorrectly implemented.
        //
        //   System.Net.WebException:
        //     System.Net.HttpWebRequest.Abort() was previously called.-or- The time-out
        //     period for the request expired.-or- An error occurred while processing the
        //     request.
        //
        //   System.ObjectDisposedException:
        //     In a .NET Compact Framework application, a request stream with zero content
        //     length was not obtained and closed correctly. For more information about
        //     handling zero content length requests, see Network Programming in the .NET
        //     Compact Framework.
        /// <summary>
        /// Gets a System.IO.Stream object to use to write request data.
        /// </summary>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        public Stream GetRequestStream() //todo: this is supposed to be an override
        {
            if (_stream == null)
            {
                _stream = new MemoryStream();//todo: this is supposed to be a ConnectStream (according to http://referencesource.microsoft.com/#System/net/System/Net/HttpWebRequest.cs,626a3477cff009e8, see the GetRequestStream() method
            }
            return _stream;
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     The stream is already in use by a previous call to System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object).-or-
        //     System.Net.HttpWebRequest.TransferEncoding is set to a value and System.Net.HttpWebRequest.SendChunked
        //     is false.
        //
        //   System.Net.ProtocolViolationException:
        //     System.Net.HttpWebRequest.Method is GET or HEAD, and either System.Net.HttpWebRequest.ContentLength
        //     is greater or equal to zero or System.Net.HttpWebRequest.SendChunked is true.-or-
        //     System.Net.HttpWebRequest.KeepAlive is true, System.Net.HttpWebRequest.AllowWriteStreamBuffering
        //     is false, System.Net.HttpWebRequest.ContentLength is -1, System.Net.HttpWebRequest.SendChunked
        //     is false, and System.Net.HttpWebRequest.Method is POST or PUT. -or- The System.Net.HttpWebRequest
        //     has an entity body but the System.Net.HttpWebRequest.GetResponse() method
        //     is called without calling the System.Net.HttpWebRequest.GetRequestStream()
        //     method. -or- The System.Net.HttpWebRequest.ContentLength is greater than
        //     zero, but the application does not write all of the promised data.
        //
        //   System.NotSupportedException:
        //     The request cache validator indicated that the response for this request
        //     can be served from the cache; however, this request includes data to be sent
        //     to the server. Requests that send data must not use the cache. This exception
        //     can occur if you are using a custom cache validator that is incorrectly implemented.
        //
        //   System.Net.WebException:
        //     System.Net.HttpWebRequest.Abort() was previously called.-or- The time-out
        //     period for the request expired.-or- An error occurred while processing the
        //     request.
        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public WebResponse GetResponse() //todo: this is supposed to be an override
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            //if (ContentLength != -1)
            //{
            //    headers.Add("Content-Length", ContentLength.ToString());
            //    headers.Add("Content-Type", ContentType);
            //}
            if (!string.IsNullOrWhiteSpace(ContentType))
            {
                headers.Add("Content-Type", ContentType);
            }
            _webRequestHelper.MakeRequest(Address, Method, headers,"", null, false); //todo: I don't know how to set the content(body) of a HttpWebRequest at the moment

            HttpWebResponse webResponse = new HttpWebResponse(_webRequestHelper.GetXmlHttpRequest());
            return webResponse; //note: webResponse currently has only the strict minimum
        }

        
        // Exceptions:
        //   System.NotSupportedException:
        //     The request scheme specified in requestUriString has not been registered.
        //
        //   System.ArgumentNullException:
        //     requestUriString is null.
        //
        //   System.Security.SecurityException:
        //     The caller does not have permission to connect to the requested URI or a
        //     URI that the request is redirected to.
        //
        //   System.UriFormatException:
        //     The URI specified in requestUriString is not a valid URI.
        /// <summary>
        /// Initializes a new System.Net.WebRequest instance for the specified URI scheme.
        /// </summary>
        /// <param name="address">The URI that identifies the Internet resource.</param>
        /// <returns>A System.Net.WebRequest descendant for the specific URI scheme.</returns>
        public static HttpWebRequest Create(string address) //todo: move this to the WebRequest class
        {
            HttpWebRequest request = new HttpWebRequest();
            request._address = new Uri(address);
            return request;
        }



        #region not supported stuff



        //// Summary:
        ////     Gets or sets the value of the Accept HTTP header.
        ////
        //// Returns:
        ////     The value of the Accept HTTP header. The default value is null.
        //public string Accept { get; set; }
        
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether the request should follow redirection
        ////     responses.
        ////
        //// Returns:
        ////     true if the request should automatically follow redirection responses from
        ////     the Internet resource; otherwise, false. The default value is true.
        //public virtual bool AllowAutoRedirect { get; set; }
        //public virtual bool AllowReadStreamBuffering { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to buffer the data sent to the
        ////     Internet resource.
        ////
        //// Returns:
        ////     true to enable buffering of the data sent to the Internet resource; false
        ////     to disable buffering. The default is true.
        //public virtual bool AllowWriteStreamBuffering { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the type of decompression that is used.
        ////
        //// Returns:
        ////     A T:System.Net.DecompressionMethods object that indicates the type of decompression
        ////     that is used.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The object's current state does not allow this property to be set.
        //public DecompressionMethods AutomaticDecompression { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the collection of security certificates that are associated
        ////     with this request.
        ////
        //// Returns:
        ////     The System.Security.Cryptography.X509Certificates.X509CertificateCollection
        ////     that contains the security certificates associated with this request.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The value specified for a set operation is null.
        //public X509CertificateCollection ClientCertificates { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the Connection HTTP header.
        ////
        //// Returns:
        ////     The value of the Connection HTTP header. The default value is null.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The value of System.Net.HttpWebRequest.Connection is set to Keep-alive or
        ////     Close.
        //public string Connection { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the name of the connection group for the request.
        ////
        //// Returns:
        ////     The name of the connection group for this request. The default value is null.
        //public override string ConnectionGroupName { get; set; }
        
        
        ////
        //// Summary:
        ////     Gets or sets the delegate method called when an HTTP 100-continue response
        ////     is received from the Internet resource.
        ////
        //// Returns:
        ////     A delegate that implements the callback method that executes when an HTTP
        ////     Continue response is returned from the Internet resource. The default value
        ////     is null.
        //public HttpContinueDelegate ContinueDelegate { get; set; }
        //public int ContinueTimeout { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the cookies associated with the request.
        ////
        //// Returns:
        ////     A System.Net.CookieContainer that contains the cookies associated with this
        ////     request.
        //public virtual CookieContainer CookieContainer { get; set; }
        ////
        //// Summary:
        ////     Gets or sets authentication information for the request.
        ////
        //// Returns:
        ////     An System.Net.ICredentials that contains the authentication credentials associated
        ////     with the request. The default is null.
        //public override ICredentials Credentials { get; set; }
        ////
        //// Summary:
        ////     Get or set the Date HTTP header value to use in an HTTP request.
        ////
        //// Returns:
        ////     The Date header value in the HTTP request.
        //public DateTime Date { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the default cache policy for this request.
        ////
        //// Returns:
        ////     A System.Net.Cache.HttpRequestCachePolicy that specifies the cache policy
        ////     in effect for this request when no other policy is applicable.
        //public static RequestCachePolicy DefaultCachePolicy { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the default maximum length of an HTTP error response.
        ////
        //// Returns:
        ////     An integer that represents the default maximum length of an HTTP error response.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The value is less than 0 and is not equal to -1.
        //public static int DefaultMaximumErrorResponseLength { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the default for the System.Net.HttpWebRequest.MaximumResponseHeadersLength
        ////     property.
        ////
        //// Returns:
        ////     The length, in kilobytes (1024 bytes), of the default maximum for response
        ////     headers received. The default configuration file sets this value to 64 kilobytes.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The value is not equal to -1 and is less than zero.
        //public static int DefaultMaximumResponseHeadersLength { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the Expect HTTP header.
        ////
        //// Returns:
        ////     The contents of the Expect HTTP header. The default value is null.NoteThe
        ////     value for this property is stored in System.Net.WebHeaderCollection. If WebHeaderCollection
        ////     is set, the property value is lost.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     Expect is set to a string that contains "100-continue" as a substring.
        //public string Expect { get; set; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether a response has been received from an
        ////     Internet resource.
        ////
        //// Returns:
        ////     true if a response has been received; otherwise, false.
        //public virtual bool HaveResponse { get; }
        ////
        //// Summary:
        ////     Specifies a collection of the name/value pairs that make up the HTTP headers.
        ////
        //// Returns:
        ////     A System.Net.WebHeaderCollection that contains the name/value pairs that
        ////     make up the headers for the HTTP request.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The request has been started by calling the System.Net.HttpWebRequest.GetRequestStream(),
        ////     System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object),
        ////     System.Net.HttpWebRequest.GetResponse(), or System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
        ////     method.
        //public override WebHeaderCollection Headers { get; set; }
        ////
        //// Summary:
        ////     Get or set the Host header value to use in an HTTP request independent from
        ////     the request URI.
        ////
        //// Returns:
        ////     The Host header value in the HTTP request.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The Host header cannot be set to null.
        ////
        ////   System.ArgumentException:
        ////     The Host header cannot be set to an invalid value.
        ////
        ////   System.InvalidOperationException:
        ////     The Host header cannot be set after the System.Net.HttpWebRequest has already
        ////     started to be sent.
        //public string Host { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the If-Modified-Since HTTP header.
        ////
        //// Returns:
        ////     A System.DateTime that contains the contents of the If-Modified-Since HTTP
        ////     header. The default value is the current date and time.
        //public DateTime IfModifiedSince { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to make a persistent connection
        ////     to the Internet resource.
        ////
        //// Returns:
        ////     true if the request to the Internet resource should contain a Connection
        ////     HTTP header with the value Keep-alive; otherwise, false. The default is true.
        //public bool KeepAlive { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the maximum number of redirects that the request follows.
        ////
        //// Returns:
        ////     The maximum number of redirection responses that the request follows. The
        ////     default value is 50.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The value is set to 0 or less.
        //public int MaximumAutomaticRedirections { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the maximum allowed length of the response headers.
        ////
        //// Returns:
        ////     The length, in kilobytes (1024 bytes), of the response headers.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The property is set after the request has already been submitted.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The value is less than 0 and is not equal to -1.
        //public int MaximumResponseHeadersLength { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the media type of the request.
        ////
        //// Returns:
        ////     The media type of the request. The default value is null.
        //public string MediaType { get; set; }
        
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to pipeline the request to the
        ////     Internet resource.
        ////
        //// Returns:
        ////     true if the request should be pipelined; otherwise, false. The default is
        ////     true.
        //public bool Pipelined { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to send an Authorization header
        ////     with the request.
        ////
        //// Returns:
        ////     true to send an HTTP Authorization header with requests after authentication
        ////     has taken place; otherwise, false. The default is false.
        //public override bool PreAuthenticate { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the version of HTTP to use for the request.
        ////
        //// Returns:
        ////     The HTTP version to use for the request. The default is System.Net.HttpVersion.Version11.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The HTTP version is set to a value other than 1.0 or 1.1.
        //public Version ProtocolVersion { get; set; }
        ////
        //// Summary:
        ////     Gets or sets proxy information for the request.
        ////
        //// Returns:
        ////     The System.Net.IWebProxy object to use to proxy the request. The default
        ////     value is set by calling the System.Net.GlobalProxySelection.Select property.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     System.Net.HttpWebRequest.Proxy is set to null.
        ////
        ////   System.InvalidOperationException:
        ////     The request has been started by calling System.Net.HttpWebRequest.GetRequestStream(),
        ////     System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object),
        ////     System.Net.HttpWebRequest.GetResponse(), or System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object).
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have permission for the requested operation.
        //public override IWebProxy Proxy { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a time-out in milliseconds when writing to or reading from a
        ////     stream.
        ////
        //// Returns:
        ////     The number of milliseconds before the writing or reading times out. The default
        ////     value is 300,000 milliseconds (5 minutes).
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The request has already been sent.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The value specified for a set operation is less than or equal to zero and
        ////     is not equal to System.Threading.Timeout.Infinite
        //public int ReadWriteTimeout { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the Referer HTTP header.
        ////
        //// Returns:
        ////     The value of the Referer HTTP header. The default value is null.
        //public string Referer { get; set; }
        ////
        //// Summary:
        ////     Gets the original Uniform Resource Identifier (URI) of the request.
        ////
        //// Returns:
        ////     A System.Uri that contains the URI of the Internet resource passed to the
        ////     System.Net.WebRequest.Create(System.String) method.
        //public override Uri RequestUri { get; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to send data in segments to the
        ////     Internet resource.
        ////
        //// Returns:
        ////     true to send data to the Internet resource in segments; otherwise, false.
        ////     The default value is false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The request has been started by calling the System.Net.HttpWebRequest.GetRequestStream(),
        ////     System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object),
        ////     System.Net.HttpWebRequest.GetResponse(), or System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
        ////     method.
        //public bool SendChunked { get; set; }
        //public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
        ////
        //// Summary:
        ////     Gets the service point to use for the request.
        ////
        //// Returns:
        ////     A System.Net.ServicePoint that represents the network connection to the Internet
        ////     resource.
        //public ServicePoint ServicePoint { get; }
        //public virtual bool SupportsCookieContainer { get; }
        ////
        //// Summary:
        ////     Gets or sets the time-out value in milliseconds for the System.Net.HttpWebRequest.GetResponse()
        ////     and System.Net.HttpWebRequest.GetRequestStream() methods.
        ////
        //// Returns:
        ////     The number of milliseconds to wait before the request times out. The default
        ////     value is 100,000 milliseconds (100 seconds).
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The value specified is less than zero and is not System.Threading.Timeout.Infinite.
        //public override int Timeout { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the Transfer-encoding HTTP header.
        ////
        //// Returns:
        ////     The value of the Transfer-encoding HTTP header. The default value is null.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     System.Net.HttpWebRequest.TransferEncoding is set when System.Net.HttpWebRequest.SendChunked
        ////     is false.
        ////
        ////   System.ArgumentException:
        ////     System.Net.HttpWebRequest.TransferEncoding is set to the value "Chunked".
        //public string TransferEncoding { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to allow high-speed NTLM-authenticated
        ////     connection sharing.
        ////
        //// Returns:
        ////     true to keep the authenticated connection open; otherwise, false.
        //public bool UnsafeAuthenticatedConnectionSharing { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a System.Boolean value that controls whether default credentials
        ////     are sent with requests.
        ////
        //// Returns:
        ////     true if the default credentials are used; otherwise false. The default value
        ////     is false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     You attempted to set this property after the request was sent.
        //public override bool UseDefaultCredentials { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the value of the User-agent HTTP header.
        ////
        //// Returns:
        ////     The value of the User-agent HTTP header. The default value is null.NoteThe
        ////     value for this property is stored in System.Net.WebHeaderCollection. If WebHeaderCollection
        ////     is set, the property value is lost.
        //public string UserAgent { get; set; }

        //// Summary:
        ////     Cancels a request to an Internet resource.
        //public override void Abort();
        ////
        //// Summary:
        ////     Adds a byte range header to a request for a specific range from the beginning
        ////     or end of the requested data.
        ////
        //// Parameters:
        ////   range:
        ////     The starting or ending point of the range.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(int range);
        ////
        //// Summary:
        ////     Adds a byte range header to a request for a specific range from the beginning
        ////     or end of the requested data.
        ////
        //// Parameters:
        ////   range:
        ////     The starting or ending point of the range.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(long range);
        ////
        //// Summary:
        ////     Adds a byte range header to the request for a specified range.
        ////
        //// Parameters:
        ////   from:
        ////     The position at which to start sending data.
        ////
        ////   to:
        ////     The position at which to stop sending data.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     from is greater than to-or- from or to is less than 0.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(int from, int to);
        ////
        //// Summary:
        ////     Adds a byte range header to the request for a specified range.
        ////
        //// Parameters:
        ////   from:
        ////     The position at which to start sending data.
        ////
        ////   to:
        ////     The position at which to stop sending data.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     from is greater than to-or- from or to is less than 0.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(long from, long to);
        ////
        //// Summary:
        ////     Adds a Range header to a request for a specific range from the beginning
        ////     or end of the requested data.
        ////
        //// Parameters:
        ////   rangeSpecifier:
        ////     The description of the range.
        ////
        ////   range:
        ////     The starting or ending point of the range.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     rangeSpecifier is null.
        ////
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(string rangeSpecifier, int range);
        ////
        //// Summary:
        ////     Adds a Range header to a request for a specific range from the beginning
        ////     or end of the requested data.
        ////
        //// Parameters:
        ////   rangeSpecifier:
        ////     The description of the range.
        ////
        ////   range:
        ////     The starting or ending point of the range.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     rangeSpecifier is null.
        ////
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(string rangeSpecifier, long range);
        ////
        //// Summary:
        ////     Adds a range header to a request for a specified range.
        ////
        //// Parameters:
        ////   rangeSpecifier:
        ////     The description of the range.
        ////
        ////   from:
        ////     The position at which to start sending data.
        ////
        ////   to:
        ////     The position at which to stop sending data.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     rangeSpecifier is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     from is greater than to-or- from or to is less than 0.
        ////
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(string rangeSpecifier, int from, int to);
        ////
        //// Summary:
        ////     Adds a range header to a request for a specified range.
        ////
        //// Parameters:
        ////   rangeSpecifier:
        ////     The description of the range.
        ////
        ////   from:
        ////     The position at which to start sending data.
        ////
        ////   to:
        ////     The position at which to stop sending data.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     rangeSpecifier is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     from is greater than to-or- from or to is less than 0.
        ////
        ////   System.ArgumentException:
        ////     rangeSpecifier is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     The range header could not be added.
        //public void AddRange(string rangeSpecifier, long from, long to);
        ////
        //// Summary:
        ////     Begins an asynchronous request for a System.IO.Stream object to use to write
        ////     data.
        ////
        //// Parameters:
        ////   callback:
        ////     The System.AsyncCallback delegate.
        ////
        ////   state:
        ////     The state object for this request.
        ////
        //// Returns:
        ////     An System.IAsyncResult that references the asynchronous request.
        ////
        //// Exceptions:
        ////   System.Net.ProtocolViolationException:
        ////     The System.Net.HttpWebRequest.Method property is GET or HEAD.-or- System.Net.HttpWebRequest.KeepAlive
        ////     is true, System.Net.HttpWebRequest.AllowWriteStreamBuffering is false, System.Net.HttpWebRequest.ContentLength
        ////     is -1, System.Net.HttpWebRequest.SendChunked is false, and System.Net.HttpWebRequest.Method
        ////     is POST or PUT.
        ////
        ////   System.InvalidOperationException:
        ////     The stream is being used by a previous call to System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)-or-
        ////     System.Net.HttpWebRequest.TransferEncoding is set to a value and System.Net.HttpWebRequest.SendChunked
        ////     is false.-or- The thread pool is running out of threads.
        ////
        ////   System.NotSupportedException:
        ////     The request cache validator indicated that the response for this request
        ////     can be served from the cache; however, requests that write data must not
        ////     use the cache. This exception can occur if you are using a custom cache validator
        ////     that is incorrectly implemented.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.
        ////
        ////   System.ObjectDisposedException:
        ////     In a .NET Compact Framework application, a request stream with zero content
        ////     length was not obtained and closed correctly. For more information about
        ////     handling zero content length requests, see Network Programming in the .NET
        ////     Compact Framework.
        //public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);
        ////
        //// Summary:
        ////     Begins an asynchronous request to an Internet resource.
        ////
        //// Parameters:
        ////   callback:
        ////     The System.AsyncCallback delegate
        ////
        ////   state:
        ////     The state object for this request.
        ////
        //// Returns:
        ////     An System.IAsyncResult that references the asynchronous request for a response.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The stream is already in use by a previous call to System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)-or-
        ////     System.Net.HttpWebRequest.TransferEncoding is set to a value and System.Net.HttpWebRequest.SendChunked
        ////     is false.-or- The thread pool is running out of threads.
        ////
        ////   System.Net.ProtocolViolationException:
        ////     System.Net.HttpWebRequest.Method is GET or HEAD, and either System.Net.HttpWebRequest.ContentLength
        ////     is greater than zero or System.Net.HttpWebRequest.SendChunked is true.-or-
        ////     System.Net.HttpWebRequest.KeepAlive is true, System.Net.HttpWebRequest.AllowWriteStreamBuffering
        ////     is false, and either System.Net.HttpWebRequest.ContentLength is -1, System.Net.HttpWebRequest.SendChunked
        ////     is false and System.Net.HttpWebRequest.Method is POST or PUT.-or- The System.Net.HttpWebRequest
        ////     has an entity body but the System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
        ////     method is called without calling the System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)
        ////     method. -or- The System.Net.HttpWebRequest.ContentLength is greater than
        ////     zero, but the application does not write all of the promised data.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.
        //public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state);
        ////
        //// Summary:
        ////     Ends an asynchronous request for a System.IO.Stream object to use to write
        ////     data.
        ////
        //// Parameters:
        ////   asyncResult:
        ////     The pending request for a stream.
        ////
        //// Returns:
        ////     A System.IO.Stream to use to write request data.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     asyncResult is null.
        ////
        ////   System.IO.IOException:
        ////     The request did not complete, and no stream is available.
        ////
        ////   System.ArgumentException:
        ////     asyncResult was not returned by the current instance from a call to System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object).
        ////
        ////   System.InvalidOperationException:
        ////     This method was called previously using asyncResult.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.-or- An error occurred
        ////     while processing the request.
        //public override Stream EndGetRequestStream(IAsyncResult asyncResult);
        ////
        //// Summary:
        ////     Ends an asynchronous request for a System.IO.Stream object to use to write
        ////     data and outputs the System.Net.TransportContext associated with the stream.
        ////
        //// Parameters:
        ////   asyncResult:
        ////     The pending request for a stream.
        ////
        ////   context:
        ////     The System.Net.TransportContext for the System.IO.Stream.
        ////
        //// Returns:
        ////     A System.IO.Stream to use to write request data.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     asyncResult was not returned by the current instance from a call to System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object).
        ////
        ////   System.ArgumentNullException:
        ////     asyncResult is null.
        ////
        ////   System.InvalidOperationException:
        ////     This method was called previously using asyncResult.
        ////
        ////   System.IO.IOException:
        ////     The request did not complete, and no stream is available.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.-or- An error occurred
        ////     while processing the request.
        //public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context);
        ////
        //// Summary:
        ////     Ends an asynchronous request to an Internet resource.
        ////
        //// Parameters:
        ////   asyncResult:
        ////     The pending request for a response.
        ////
        //// Returns:
        ////     A System.Net.WebResponse that contains the response from the Internet resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     asyncResult is null.
        ////
        ////   System.InvalidOperationException:
        ////     This method was called previously using asyncResult.-or- The System.Net.HttpWebRequest.ContentLength
        ////     property is greater than 0 but the data has not been written to the request
        ////     stream.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.-or- An error occurred
        ////     while processing the request.
        ////
        ////   System.ArgumentException:
        ////     asyncResult was not returned by the current instance from a call to System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object).
        //public override WebResponse EndGetResponse(IAsyncResult asyncResult);
        ////
        //// Summary:
        ////     Populates a System.Runtime.Serialization.SerializationInfo with the data
        ////     required to serialize the target object.
        ////
        //// Parameters:
        ////   serializationInfo:
        ////     The System.Runtime.Serialization.SerializationInfo to populate with data.
        ////
        ////   streamingContext:
        ////     A System.Runtime.Serialization.StreamingContext that specifies the destination
        ////     for this serialization.
        //protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);
        
        ////
        //// Summary:
        ////     Gets a System.IO.Stream object to use to write request data and outputs the
        ////     System.Net.TransportContext associated with the stream.
        ////
        //// Parameters:
        ////   context:
        ////     The System.Net.TransportContext for the System.IO.Stream.
        ////
        //// Returns:
        ////     A System.IO.Stream to use to write request data.
        ////
        //// Exceptions:
        ////   System.Exception:
        ////     The System.Net.HttpWebRequest.GetRequestStream() method was unable to obtain
        ////     the System.IO.Stream.
        ////
        ////   System.InvalidOperationException:
        ////     The System.Net.HttpWebRequest.GetRequestStream() method is called more than
        ////     once.-or- System.Net.HttpWebRequest.TransferEncoding is set to a value and
        ////     System.Net.HttpWebRequest.SendChunked is false.
        ////
        ////   System.NotSupportedException:
        ////     The request cache validator indicated that the response for this request
        ////     can be served from the cache; however, requests that write data must not
        ////     use the cache. This exception can occur if you are using a custom cache validator
        ////     that is incorrectly implemented.
        ////
        ////   System.Net.ProtocolViolationException:
        ////     The System.Net.HttpWebRequest.Method property is GET or HEAD.-or- System.Net.HttpWebRequest.KeepAlive
        ////     is true, System.Net.HttpWebRequest.AllowWriteStreamBuffering is false, System.Net.HttpWebRequest.ContentLength
        ////     is -1, System.Net.HttpWebRequest.SendChunked is false, and System.Net.HttpWebRequest.Method
        ////     is POST or PUT.
        ////
        ////   System.Net.WebException:
        ////     System.Net.HttpWebRequest.Abort() was previously called.-or- The time-out
        ////     period for the request expired.-or- An error occurred while processing the
        ////     request.
        //public Stream GetRequestStream(out TransportContext context);
        
        #endregion

        
    }

#endif
}

