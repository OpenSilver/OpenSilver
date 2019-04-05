
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

namespace System.Net
{
    /// <summary>
    /// Provides a response from a Uniform Resource Identifier (URI). This is an
    /// abstract class.
    /// </summary>
    [Serializable]
    //todo: make this class implement IDisposable (I prefer not to do it before knowing what needs to be done while disposing it)
    public abstract class WebResponse// : MarshalByRefObject//, IDisposable//, ISerializable
    {
        
        //todo: implement the following then make HttpWebResponse override it

        //// Summary:
        ////     When overridden in a descendant class, gets or sets the content length of
        ////     data being received.
        ////
        //// Returns:
        ////     The number of bytes returned from the Internet resource.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to get or set the property, when the property is not
        ////     overridden in a descendant class.
        //public virtual long ContentLength { get; set; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets or sets the content type of the
        ////     data being received.
        ////
        //// Returns:
        ////     A string that contains the content type of the response.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to get or set the property, when the property is not
        ////     overridden in a descendant class.
        //public virtual string ContentType { get; set; }
        
        ////
        //// Summary:
        ////     Gets a System.Boolean value that indicates whether this response was obtained
        ////     from the cache.
        ////
        //// Returns:
        ////     true if the response was taken from the cache; otherwise, false.
        //public virtual bool IsFromCache { get; }
        ////
        //// Summary:
        ////     Gets a System.Boolean value that indicates whether mutual authentication
        ////     occurred.
        ////
        //// Returns:
        ////     true if both client and server were authenticated; otherwise, false.
        //public virtual bool IsMutuallyAuthenticated { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the URI of the Internet resource
        ////     that actually responded to the request.
        ////
        //// Returns:
        ////     An instance of the System.Uri class that contains the URI of the Internet
        ////     resource that actually responded to the request.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to get or set the property, when the property is not
        ////     overridden in a descendant class.
        //public virtual Uri ResponseUri { get; }
        ////
        ////
        //// Returns:
        ////     Returns System.Boolean.
        //public virtual bool SupportsHeaders { get; }

        // Summary:
        //     When overridden by a descendant class, closes the response stream.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     Any attempt is made to access the method, when the method is not overridden
        //     in a descendant class.
        //public virtual void Close();
        //public void Dispose();
        //protected virtual void Dispose(bool disposing);
       
        ////
        //// Summary:
        ////     When overridden in a descendant class, returns the data stream from the Internet
        ////     resource.
        ////
        //// Returns:
        ////     An instance of the System.IO.Stream class for reading data from the Internet
        ////     resource.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to access the method, when the method is not overridden
        ////     in a descendant class.
        public virtual Stream GetResponseStream()
        {
            // Not implemented, but useful in the Simulator when making the REST calls via the WebClient.

            throw new NotImplementedException();
        }


        ////
        //// Summary:
        ////     Initializes a new instance of the System.Net.WebResponse class from the specified
        ////     instances of the System.Runtime.Serialization.SerializationInfo and System.Runtime.Serialization.StreamingContext
        ////     classes.
        ////
        //// Parameters:
        ////   serializationInfo:
        ////     An instance of the System.Runtime.Serialization.SerializationInfo class that
        ////     contains the information required to serialize the new System.Net.WebRequest
        ////     instance.
        ////
        ////   streamingContext:
        ////     An instance of the System.Runtime.Serialization.StreamingContext class that
        ////     indicates the source of the serialized stream that is associated with the
        ////     new System.Net.WebRequest instance.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to access the constructor, when the constructor is not
        ////     overridden in a descendant class.
        //protected WebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext);

        ////
        //// Summary:
        ////     Populates a System.Runtime.Serialization.SerializationInfo with the data
        ////     that is needed to serialize the target object.
        ////
        //// Parameters:
        ////   serializationInfo:
        ////     The System.Runtime.Serialization.SerializationInfo to populate with data.
        ////
        ////   streamingContext:
        ////     A System.Runtime.Serialization.StreamingContext that specifies the destination
        ////     for this serialization.
        //protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext);

        ////
        //// Summary:
        ////     When overridden in a derived class, gets a collection of header name-value
        ////     pairs associated with this request.
        ////
        //// Returns:
        ////     An instance of the System.Net.WebHeaderCollection class that contains header
        ////     values associated with this response.
        ////
        //// Exceptions:
        ////   System.NotSupportedException:
        ////     Any attempt is made to get or set the property, when the property is not
        ////     overridden in a descendant class.
        //public virtual WebHeaderCollection Headers { get; }
    }
}
