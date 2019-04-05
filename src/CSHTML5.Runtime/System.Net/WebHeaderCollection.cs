
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// Contains protocol headers associated with a request or response.
    /// </summary>
    [Serializable]
    public class WebHeaderCollection //: NameValueCollection, ISerializable //todo: inheritance
    {
        Dictionary<string, string> _headers;
        /// <summary>
        /// Initializes a new instance of the System.Net.WebHeaderCollection class.
        /// </summary>
        public WebHeaderCollection()
        {
            _headers = new Dictionary<string, string>();
        }


        /// <summary>
        /// Gets all header names (keys) in the collection.
        /// </summary>
        public string[] AllKeys { get { return _headers.Keys.ToArray(); } } //todo: this is supposed to be an override. //Note: I chose AllKeys and not Keys because Keys would have asked for yet another additional type.

        /// <summary>
        /// Removes all headers from the collection.
        /// </summary>
        public void Clear() { _headers.Clear(); } //todo: this is supposed to be an override.

        /// <summary>
        /// Gets the number of headers in the collection.
        /// </summary>
        public int Count { get { return _headers.Count; } } //todo: this is supposed to be an override.

        /// <summary>
        /// Returns an enumerator that can iterate through the System.Net.WebHeaderCollection instance
        /// </summary>
        /// <returns>An enumerator that can iterate through the System.Net.WebHeaderCollection instance</returns>
        public IEnumerator GetEnumerator() { return _headers.GetEnumerator(); } //todo: this is supposed to be an override.

        // Exceptions:
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpRequestHeader.
        /// <summary>
        /// Gets or sets the specified request header.
        /// </summary>
        /// <param name="header">The request header value.</param>
        /// <returns>A System.String instance containing the specified header value.</returns>
        public string this[HttpRequestHeader header] {
            get
            {
                return _headers[CSharpHeaderToHtmlHeaderConverter.Convert(header)];
            }
            set
            {
                _headers[CSharpHeaderToHtmlHeaderConverter.Convert(header)] = value;
            }
        }

        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The length of value is greater than 65535.
        //
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpResponseHeader.
        /// <summary>
        /// Gets or sets the specified response header.
        /// </summary>
        /// <param name="header">The response header value.</param>
        /// <returns>A System.String instance containing the specified header.</returns>
        public string this[HttpResponseHeader header] {
            get
            {
                return _headers[CSharpHeaderToHtmlHeaderConverter.Convert(header)];
            }
            set
            {
                _headers[CSharpHeaderToHtmlHeaderConverter.Convert(header)] = value;
            }
        }

     
        // Exceptions:
        //   System.ArgumentNullException:
        //     header is null or System.String.Empty.
        //
        //   System.ArgumentException:
        //     header does not contain a colon (:) character.The length of value is greater
        //     than 65535.-or- The name part of header is System.String.Empty or contains
        //     invalid characters.-or- header is a restricted header that should be set
        //     with a property.-or- The value part of header contains invalid characters.
        //
        //   System.ArgumentOutOfRangeException:
        //     The length the string after the colon (:) is greater than 65535.
        /// <summary>
        /// Inserts the specified header into the collection.
        /// </summary>
        /// <param name="header">The header to add, with the name and value separated by a colon.</param>
        public void Add(string header)
        {
            int splitIndex = header.IndexOf(':');
            if (splitIndex <=0) //(no colon (:) or header is empty) //todo: check for other sources of exceptions
            {
                throw new ArgumentException("header must be of the form: \"header:value\".");
            }
            string headerPart = header.Substring(0, splitIndex);
            string valuePart = header.Substring(splitIndex + 1);
            headerPart = headerPart.Trim();
            valuePart = valuePart.Trim();
            Add(headerPart, valuePart);
        }
       
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The length of value is greater than 65535.
        //
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpRequestHeader.
        /// <summary>
        /// Inserts the specified header with the specified value into the collection.
        /// </summary>
        /// <param name="header">The header to add to the collection.</param>
        /// <param name="value">The content of the header.</param>
        public void Add(HttpRequestHeader header, string value)
        {
            Add(CSharpHeaderToHtmlHeaderConverter.Convert(header), value);
        }
    
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The length of value is greater than 65535.
        //
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpResponseHeader.
        /// <summary>
        /// Inserts the specified header with the specified value into the collection.
        /// </summary>
        /// <param name="header">The header to add to the collection.</param>
        /// <param name="value">The content of the header.</param>
        public void Add(HttpResponseHeader header, string value)
        {
            Add(CSharpHeaderToHtmlHeaderConverter.Convert(header), value);
        }
     
        // Exceptions:
        //   System.ArgumentException:
        //     name is null, System.String.Empty, or contains invalid characters.-or- name
        //     is a restricted header that must be set with a property setting.-or- value
        //     contains invalid characters.
        //
        //   System.ArgumentOutOfRangeException:
        //     The length of value is greater than 65535.
        /// <summary>
        /// Inserts a header with the specified name and value into the collection.
        /// </summary>
        /// <param name="name">The header to add to the collection.</param>
        /// <param name="value">The content of the header.</param>
        public void Add(string name, string value) //todo: this is supposed to be an override.
        {
            _headers.Add(name, value);
        }

        /// <summary>
        /// Get the value of a particular header in the collection, specified by the
        /// name of the header.
        /// </summary>
        /// <param name="name">The name of the Web header.</param>
        /// <returns>A System.String holding the value of the specified header.</returns>
        public string Get(string name) //todo: this is supposed to be an override.
        {
            return _headers[name];
        }

   
        // Exceptions:
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpRequestHeader.
        /// <summary>
        /// Removes the specified header from the collection.
        /// </summary>
        /// <param name="header">The System.Net.HttpRequestHeader instance to remove from the collection.</param>
        public void Remove(HttpRequestHeader header)
        {
            Remove(CSharpHeaderToHtmlHeaderConverter.Convert(header));
        }
     
        // Exceptions:
        //   System.InvalidOperationException:
        //     This System.Net.WebHeaderCollection instance does not allow instances of
        //     System.Net.HttpResponseHeader.
        /// <summary>
        /// Removes the specified header from the collection.
        /// </summary>
        /// <param name="header">The System.Net.HttpResponseHeader instance to remove from the collection.</param>
        public void Remove(HttpResponseHeader header)
        {
            Remove(CSharpHeaderToHtmlHeaderConverter.Convert(header));
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     name is nullSystem.String.Empty.
        //
        //   System.ArgumentException:
        //     name is a restricted header.-or- name contains invalid characters.
        /// <summary>
        /// Removes the specified header from the collection.
        /// </summary>
        /// <param name="name">The name of the header to remove from the collection.</param>
        public void Remove(string name) //todo: this is supposed to be an override.
        {
            _headers.Remove(name);
        }

        
        // Exceptions:
        //   System.ArgumentNullException:
        //     name is null or System.String.Empty.
        //
        //   System.ArgumentOutOfRangeException:
        //     The length of value is greater than 65535.
        //
        //   System.ArgumentException:
        //     name is a restricted header.-or- name or value contain invalid characters.
        /// <summary>
        /// Sets the specified header to the specified value.
        /// </summary>
        /// <param name="name">The header to set.</param>
        /// <param name="value">The content of the header to set.</param>
        public void Set(string name, string value) //todo: this is supposed to be an override.
        {
            if (_headers.ContainsKey(name))
            {
                _headers[name] = value;
            }
            else
            {
                _headers.Add(name, value);
            }
        }

        #region Internal stuff

        internal Dictionary<string, string> GetHeadersAsDictionaryStringString()
        {
            return _headers;
        }

        #endregion


        #region not supported stuff

        ////
        //// Summary:
        ////     Initializes a new instance of the System.Net.WebHeaderCollection class from
        ////     the specified instances of the System.Runtime.Serialization.SerializationInfo
        ////     and System.Runtime.Serialization.StreamingContext classes.
        ////
        //// Parameters:
        ////   serializationInfo:
        ////     A System.Runtime.Serialization.SerializationInfo containing the information
        ////     required to serialize the System.Net.WebHeaderCollection.
        ////
        ////   streamingContext:
        ////     A System.Runtime.Serialization.StreamingContext containing the source of
        ////     the serialized stream associated with the new System.Net.WebHeaderCollection.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     headerName contains invalid characters.
        ////
        ////   System.ArgumentNullException:
        ////     headerName is a null reference or System.String.Empty.
        //protected WebHeaderCollection(SerializationInfo serializationInfo, StreamingContext streamingContext);

        
        ////
        //// Summary:
        ////     Gets the collection of header names (keys) in the collection.
        ////
        //// Returns:
        ////     A System.Collections.Specialized.NameObjectCollectionBase.KeysCollection
        ////     containing all header names in a Web request.
        //public override NameObjectCollectionBase.KeysCollection Keys { get; }

        
        ////
        //// Summary:
        ////     Inserts a header into the collection without checking whether the header
        ////     is on the restricted header list.
        ////
        //// Parameters:
        ////   headerName:
        ////     The header to add to the collection.
        ////
        ////   headerValue:
        ////     The content of the header.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     headerName is null, System.String.Empty, or contains invalid characters.-or-
        ////     headerValue contains invalid characters.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     headerName is not null and the length of headerValue is too long (greater
        ////     than 65,535 characters).
        //protected void AddWithoutValidate(string headerName, string headerValue);
        
        ////
        //// Summary:
        ////     Get the value of a particular header in the collection, specified by an index
        ////     into the collection.
        ////
        //// Parameters:
        ////   index:
        ////     The zero-based index of the key to get from the collection.
        ////
        //// Returns:
        ////     A System.String containing the value of the specified header.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     index is negative. -or-index exceeds the size of the collection.
        //public override string Get(int index);
        
        
        ////
        //// Summary:
        ////     Get the header name at the specified position in the collection.
        ////
        //// Parameters:
        ////   index:
        ////     The zero-based index of the key to get from the collection.
        ////
        //// Returns:
        ////     A System.String holding the header name.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     index is negative. -or-index exceeds the size of the collection.
        //public override string GetKey(int index);
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
        //public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);
        ////
        //// Summary:
        ////     Gets an array of header values stored in the index position of the header
        ////     collection.
        ////
        //// Parameters:
        ////   index:
        ////     The header index to return.
        ////
        //// Returns:
        ////     An array of header strings.
        //public override string[] GetValues(int index);
        ////
        //// Summary:
        ////     Gets an array of header values stored in a header.
        ////
        //// Parameters:
        ////   header:
        ////     The header to return.
        ////
        //// Returns:
        ////     An array of header strings.
        //public override string[] GetValues(string header);
        ////
        //// Summary:
        ////     Tests whether the specified HTTP header can be set for the request.
        ////
        //// Parameters:
        ////   headerName:
        ////     The header to test.
        ////
        //// Returns:
        ////     true if the header is restricted; otherwise false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     headerName is null or System.String.Empty.
        ////
        ////   System.ArgumentException:
        ////     headerName contains invalid characters.
        //public static bool IsRestricted(string headerName);
        ////
        //// Summary:
        ////     Tests whether the specified HTTP header can be set for the request or the
        ////     response.
        ////
        //// Parameters:
        ////   headerName:
        ////     The header to test.
        ////
        ////   response:
        ////     Does the Framework test the response or the request?
        ////
        //// Returns:
        ////     true if the header is restricted; otherwise, false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     headerName is null or System.String.Empty.
        ////
        ////   System.ArgumentException:
        ////     headerName contains invalid characters.
        //public static bool IsRestricted(string headerName, bool response);
        ////
        //// Summary:
        ////     Implements the System.Runtime.Serialization.ISerializable interface and raises
        ////     the deserialization event when the deserialization is complete.
        ////
        //// Parameters:
        ////   sender:
        ////     The source of the deserialization event.
        //public override void OnDeserialization(object sender);

        ////
        //// Summary:
        ////     Sets the specified header to the specified value.
        ////
        //// Parameters:
        ////   header:
        ////     The System.Net.HttpRequestHeader value to set.
        ////
        ////   value:
        ////     The content of the header to set.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The length of value is greater than 65535.
        ////
        ////   System.InvalidOperationException:
        ////     This System.Net.WebHeaderCollection instance does not allow instances of
        ////     System.Net.HttpRequestHeader.
        //public void Set(HttpRequestHeader header, string value);
        ////
        //// Summary:
        ////     Sets the specified header to the specified value.
        ////
        //// Parameters:
        ////   header:
        ////     The System.Net.HttpResponseHeader value to set.
        ////
        ////   value:
        ////     The content of the header to set.
        ////
        //// Exceptions:
        ////   System.ArgumentOutOfRangeException:
        ////     The length of value is greater than 65535.
        ////
        ////   System.InvalidOperationException:
        ////     This System.Net.WebHeaderCollection instance does not allow instances of
        ////     System.Net.HttpResponseHeader.
        //public void Set(HttpResponseHeader header, string value);
        

        ////
        //// Summary:
        ////     Converts the System.Net.WebHeaderCollection to a byte array..
        ////
        //// Returns:
        ////     A System.Byte array holding the header collection.
        //public byte[] ToByteArray();
        ////
        //// Summary:
        ////     This method is obsolete.
        ////
        //// Returns:
        ////     The System.String representation of the collection.
        //public override string ToString();
        #endregion
    }

    static internal class CSharpHeaderToHtmlHeaderConverter
    {
        static Dictionary<string, string> _headerStringEquivalence = new Dictionary<string, string>()
        {
            {"CacheControl", "Cache-Control"},
            {"Connection", "Connection"},
            {"KeepAlive", "Keep-Alive"},
            {"TransferEncoding", "Transfer-Encoding"},
            {"ContentLength", "Content-Length"},
            {"ContentType", "Content-Type"},
            {"ContentEncoding", "Content-Encoding"},
            {"ContentLanguage", "Content-Language"},
            {"ContentLocation", "Content-Location"},
            {"ContentMd5", "Content-MD5"},
            {"ContentRange", "Content-Range"},
            {"LastModified", "Last-Modified"},
            {"AcceptCharset", "Accept-Charset"},
            {"AcceptEncoding", "Accept-Encoding"},
            {"AcceptLanguage", "Accept-Language"},
            {"IfMatch", "If-Match"},
            {"IfModifiedSince", "If-Modified-Since"},
            {"IfNoneMatch", "If-None-Match"},
            {"IfRange", "If-Range"},
            {"IfUnmodifiedSince", "If-Unmodified-Since"},
            {"MaxForwards", "Max-Forwards"},
            {"ProxyAuthorization", "Proxy-Authorization"},
            {"UserAgent", "User-Agent"},
            {"AcceptRanges", "Accept-Ranges"},
            {"ProxyAuthenticate", "Proxy-Authenticate"},
            {"RetryAfter", "Retry-After"},
            {"SetCookie", "Set-Cookie"}
        };
        static internal string Convert(HttpRequestHeader header)
        {
            string headerAsString = header.ToString();
            return Convert(headerAsString);
        }

        static internal string Convert(HttpResponseHeader header)
        {
            string headerAsString = header.ToString();
            return Convert(headerAsString);
        }

        static string Convert(string header)
        {
            string returnString = header;
            if(_headerStringEquivalence.ContainsKey(header))
            {
                returnString = _headerStringEquivalence[header];
            }
            return returnString;
        }
    }

}
