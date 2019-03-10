
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

namespace System.ServiceModel
{
    /// <summary>
    /// Specifies the types of security that can be used with the system-provided
    /// System.ServiceModel.BasicHttpBinding.
    /// </summary>
    public enum BasicHttpSecurityMode
    {
        /// <summary>
        /// The SOAP message is not secured during transfer. This is the default behavior.
        /// </summary>
        None = 0,
     
        /// <summary>
        /// Security is provided using HTTPS. The service must be configured with SSL
        /// certificates. The SOAP message is protected as a whole using HTTPS. The service
        /// is authenticated by the client using the service’s SSL certificate. The client
        /// authentication is controlled through the System.ServiceModel.HttpTransportSecurity.ClientCredentialType.
        /// </summary>
        Transport = 1,
        ////
        //// Summary:
        ////     Security is provided using SOAP message security. For the System.ServiceModel.BasicHttpBinding,
        ////     the system requires that the server certificate be provided to the client
        ////     separately. The valid client credential types for this binding are UserName
        ////     and Certificate.
        //Message = 2,
        ////
        //// Summary:
        ////     Integrity, confidentiality and server authentication are provided by HTTPS.
        ////     The service must be configured with a certificate. Client authentication
        ////     is provided by means of SOAP message security. This mode is applicable when
        ////     the user is authenticating with a UserName or Certificate credential and
        ////     there is an existing HTTPS deployment for securing message transfer.
        //TransportWithMessageCredential = 3,
        ////
        //// Summary:
        ////     This mode does not provide message integrity and confidentiality. It provides
        ////     only HTTP-based client authentication. Use this mode with caution. It should
        ////     be used in environments where the transfer security is being provided by
        ////     other means (such as IPSec) and only client authentication is provided by
        ////     the Windows Communication Foundation (WCF) infrastructure.
        //TransportCredentialOnly = 4,
    }
}

#endif