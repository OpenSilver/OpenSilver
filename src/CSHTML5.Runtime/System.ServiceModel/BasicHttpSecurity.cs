
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

using System;
using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Configures the security settings of a basicHttpBinding binding.
    /// </summary>
    public sealed class BasicHttpSecurity
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.BasicHttpSecurity class.
        /// </summary>
        public BasicHttpSecurity()
        {
        }

        //// Summary:
        ////     Gets the message-level security settings for a basicHttpBinding binding.
        ////
        //// Returns:
        ////     A System.ServiceModel.BasicHttpMessageSecurity, which represents the message-level
        ////     security settings for this binding.
        //public BasicHttpMessageSecurity Message { get; set; }
        
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value is not a legal value for System.ServiceModel.BasicHttpSecurityMode.
        /// <summary>
        /// Gets or sets the security mode for a basicHttpBinding binding.
        /// </summary>
        public BasicHttpSecurityMode Mode { get; set; }
        ////
        //// Summary:
        ////     Gets the transport-level security settings for a basicHttpBinding binding.
        ////
        //// Returns:
        ////     The transport-level security settings for a basicHttpBinding binding.
        //public HttpTransportSecurity Transport { get; set; }

        //// Summary:
        ////     Determines whether a message element should be serialized.
        ////
        //// Returns:
        ////     true if the message should be serialized; otherwise, false.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool ShouldSerializeMessage();
        ////
        //// Summary:
        ////     Determines whether the transport element should be serialized.
        ////
        //// Returns:
        ////     true if the transport element should be serialized; otherwise, false.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool ShouldSerializeTransport();
    }
}

#endif