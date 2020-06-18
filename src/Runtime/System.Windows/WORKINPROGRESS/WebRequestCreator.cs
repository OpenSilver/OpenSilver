#if WORKINPROGRESS

namespace System.Net.Browser
{
    //
    // Summary:
    //     Provides objects for specifying whether the browser or the client handles HTTP
    //     requests and responses.
    public static class WebRequestCreator
    {
        //
        // Summary:
        //     Gets an object that specifies browser HTTP handling for Web requests and responses.
        //
        // Returns:
        //     An System.Net.IWebRequestCreate object that specifies browser handling for Web
        //     requests and responses.
        public static IWebRequestCreate BrowserHttp { get; }
        //
        // Summary:
        //     Gets an object that specifies client HTTP handling for Web requests and responses.
        //
        // Returns:
        //     An System.Net.IWebRequestCreate object for use with Silverlight-based application
        //     that runs outside of a Web browser.
        public static IWebRequestCreate ClientHttp { get; }
    }
}
#endif