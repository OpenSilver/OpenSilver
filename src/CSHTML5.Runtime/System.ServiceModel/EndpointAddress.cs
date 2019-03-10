
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


#if WCF_STACK || BRIDGE

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Provides a unique network address that a client uses to communicate with
    /// a service endpoint.
    /// </summary>
    public class EndpointAddress
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