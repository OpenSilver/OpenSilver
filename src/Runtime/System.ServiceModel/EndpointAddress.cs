
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

#if !OPENSILVER

#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Provides a unique network address that a client uses to communicate with
    /// a service endpoint.
    /// </summary>
    public partial class EndpointAddress
    {
        Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
            protected set { _uri = value; }
        }
       
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.EndpointAddress class
        /// with a specified URI and headers.
        /// </summary>
        /// <param name="uri">The System.Uri that identifies the endpoint location.</param>
        /// <param name="addressHeaders">
        /// The System.Array of type System.ServiceModel.Channels.AddressHeader that
        /// contains address information used to interact with the endpoint.
        /// </param>
        public EndpointAddress(Uri uri, params AddressHeader[] addressHeaders)
        {
            _uri = uri;
        }

        public EndpointAddress(string uri)
        {
            Uri u = new Uri(uri);
            _uri = u;
        }
    }
}

#endif
#endif