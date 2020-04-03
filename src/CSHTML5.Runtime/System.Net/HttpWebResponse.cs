
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if BRIDGE
using Bridge;
#endif

namespace System.Net
{
    /// <summary>
    /// Provides an HTTP-specific implementation of the System.Net.WebResponse class.
    /// </summary>
    [Serializable]
    public partial class HttpWebResponse : WebResponse//, ISerializable
    {
        private object _xmlHttpRequest;

        internal HttpWebResponse(object xmlHttpRequest)
        {
            this._xmlHttpRequest = xmlHttpRequest;
        }

        // Exceptions:
        //   System.ObjectDisposedException:
        //     The current instance has been disposed.
        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        /// 
#if !BRIDGE && !NETSTANDARD // This is the JSIL version, which doesn't have access to the "CSHTML5.Interop" class because we are in the project "DotNetForHtml5.System.dll".
        [JSIL.Meta.JSReplacement("$xmlHttpRequest.status")]
        public virtual HttpStatusCode StatusCode { get { throw new NotImplementedException(); } } //todo: maybe implement this in another way?
        
#else
#if WORKINPROGRESS
        public virtual HttpStatusCode StatusCode
        {
            get
            {
                int statusCode = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript("$0.status", _xmlHttpRequest));
                return (HttpStatusCode)statusCode;
            }
        }
#else
        public virtual HttpStatusCode StatusCode()
        {
            int statusCode = Convert.ToInt32(CSHTML5.Interop.ExecuteJavaScript("$0.status", _xmlHttpRequest));
            return (HttpStatusCode)statusCode;
        }
#endif
#endif

        // Exceptions:
        //   System.Net.ProtocolViolationException:
        //     There is no response stream.
        //
        //   System.ObjectDisposedException:
        //     The current instance has been disposed.
        /// <summary>
        /// Gets the stream that is used to read the body of the response from the server.
        /// </summary>
        /// <returns>A System.IO.Stream containing the body of the response.</returns>
        public override Stream GetResponseStream()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(this.GetResponseAsString((object)this._xmlHttpRequest));
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

#if !BRIDGE && !NETSTANDARD // This is the JSIL version, which doesn't have access to the "CSHTML5.Interop" class because we are in the project "DotNetForHtml5.System.dll".
        [JSIL.Meta.JSReplacement("$xmlHttpRequest.responseText")]
        private string GetResponseAsString(object xmlHttpRequest)
        {
            return null; // Simulator
        }
#else
        private string GetResponseAsString(object xmlHttpRequest)
        {
            return Convert.ToString(CSHTML5.Interop.ExecuteJavaScript(@"$0.responseText", xmlHttpRequest));
        }
#endif




        #region not implemented stuff
        //// Summary:
        ////     Gets the character set of the response.
        ////
        //// Returns:
        ////     A string that contains the character set of the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public string CharacterSet { get; }
        ////
        //// Summary:
        ////     Gets the method that is used to encode the body of the response.
        ////
        //// Returns:
        ////     A string that describes the method that is used to encode the body of the
        ////     response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public string ContentEncoding { get; }
        ////
        //// Summary:
        ////     Gets the length of the content returned by the request.
        ////
        //// Returns:
        ////     The number of bytes returned by the request. Content length does not include
        ////     header information.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public override long ContentLength { get; }
        ////
        //// Summary:
        ////     Gets the content type of the response.
        ////
        //// Returns:
        ////     A string that contains the content type of the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public override string ContentType { get; }
        ////
        //// Summary:
        ////     Gets or sets the cookies that are associated with this response.
        ////
        //// Returns:
        ////     A System.Net.CookieCollection that contains the cookies that are associated
        ////     with this response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public virtual CookieCollection Cookies { get; set; }
        ////
        //// Summary:
        ////     Gets the headers that are associated with this response from the server.
        ////
        //// Returns:
        ////     A System.Net.WebHeaderCollection that contains the header information returned
        ////     with the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public override WebHeaderCollection Headers { get; }
        ////
        //// Summary:
        ////     Gets a System.Boolean value that indicates whether both client and server
        ////     were authenticated.
        ////
        //// Returns:
        ////     true if mutual authentication occurred; otherwise, false.
        //public override bool IsMutuallyAuthenticated { get; }
        ////
        //// Summary:
        ////     Gets the last date and time that the contents of the response were modified.
        ////
        //// Returns:
        ////     A System.DateTime that contains the date and time that the contents of the
        ////     response were modified.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public DateTime LastModified { get; }
        ////
        //// Summary:
        ////     Gets the method that is used to return the response.
        ////
        //// Returns:
        ////     A string that contains the HTTP method that is used to return the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public virtual string Method { get; }
        ////
        //// Summary:
        ////     Gets the version of the HTTP protocol that is used in the response.
        ////
        //// Returns:
        ////     A System.Version that contains the HTTP protocol version of the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public Version ProtocolVersion { get; }
        ////
        //// Summary:
        ////     Gets the URI of the Internet resource that responded to the request.
        ////
        //// Returns:
        ////     A System.Uri that contains the URI of the Internet resource that responded
        ////     to the request.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public override Uri ResponseUri { get; }
        ////
        //// Summary:
        ////     Gets the name of the server that sent the response.
        ////
        //// Returns:
        ////     A string that contains the name of the server that sent the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public string Server { get; }
        
        ////
        //// Summary:
        ////     Gets the status description returned with the response.
        ////
        //// Returns:
        ////     A string that describes the status of the response.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public virtual string StatusDescription { get; }
        ////
        ////
        //// Returns:
        ////     Returns System.Boolean.
        //public override bool SupportsHeaders { get; }

        //// Summary:
        ////     Closes the response stream.
        //public override void Close();
        ////
        //// Summary:
        ////     Releases the unmanaged resources used by the System.Net.HttpWebResponse,
        ////     and optionally disposes of the managed resources.
        ////
        //// Parameters:
        ////   disposing:
        ////     true to release both managed and unmanaged resources; false to releases only
        ////     unmanaged resources.
        //protected override void Dispose(bool disposing);
        ////
        //// Summary:
        ////     Populates a System.Runtime.Serialization.SerializationInfo with the data
        ////     needed to serialize the target object.
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
        ////     Gets the contents of a header that was returned with the response.
        ////
        //// Parameters:
        ////   headerName:
        ////     The header value to return.
        ////
        //// Returns:
        ////     The contents of the specified header.
        ////
        //// Exceptions:
        ////   System.ObjectDisposedException:
        ////     The current instance has been disposed.
        //public string GetResponseHeader(string headerName);
        
        #endregion
    }
}
