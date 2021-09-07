using System.Net;

namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Defines the interface used to provide access to an optional instance of System.Net.CookieContainer
    //     that can be used to manage a collection of cookies.
    public interface IHttpCookieContainerManager
    {
        //
        // Summary:
        //     Gets or sets the System.Net.CookieContainer object to be used, if one is to be
        //     used.
        //
        // Returns:
        //     The System.Net.CookieContainer to use, if a container is to be used, or null
        //     if a container is not to be used.
        CookieContainer CookieContainer { get; set; }
    }
}
