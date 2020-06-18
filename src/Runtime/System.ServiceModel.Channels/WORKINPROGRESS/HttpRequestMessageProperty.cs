#if WORKINPROGRESS
using System.Net;

namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Provides access to the HTTP request to access and respond to the additional information
    //     that is made available for requests over the HTTP protocol.
    public sealed partial class HttpRequestMessageProperty
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.HttpRequestMessageProperty
        //     class.
        public HttpRequestMessageProperty()
        {

        }

        //
        // Summary:
        //     Gets the name of the message property associated with the System.ServiceModel.Channels.HttpRequestMessageProperty
        //     class.
        //
        // Returns:
        //     The value "httpRequest".
        public static string Name { get; }
        //
        // Summary:
        //     Gets the HTTP headers from the HTTP request.
        //
        // Returns:
        //     A System.Net.WebHeaderCollection that contains the HTTP headers in the HTTP request.
        public WebHeaderCollection Headers { get; }
        //
        // Summary:
        //     Gets or sets the HTTP verb for the HTTP request.
        //
        // Returns:
        //     The HTTP verb for the HTTP request.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     value set is null.
        public string Method { get; set; }
        //
        // Summary:
        //     Gets or sets the query string for the HTTP request.
        //
        // Returns:
        //     The query string from the HTTP request.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     value set is null.
        public string QueryString { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether the body of the message is ignored
        //     and only the headers are sent.
        //
        // Returns:
        //     true if the message body is suppressed; otherwise, false. The default is false.
        public bool SuppressEntityBody { get; set; }
    }
}
#endif