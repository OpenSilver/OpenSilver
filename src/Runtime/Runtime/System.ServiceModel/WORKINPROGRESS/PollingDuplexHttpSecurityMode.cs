
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Specifies the types of security that can be used with the system-provided <see cref="PollingDuplexHttpBinding"/>.
    /// </summary>
    public enum PollingDuplexHttpSecurityMode
    {
        /// <summary>
        /// The SOAP message is not secured during transfer. This is the default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Security is provided using HTTPS. The service must be configured with SSL certificates.
        /// The SOAP message is protected as a whole using HTTPS. The service is authenticated
        /// by the client using the service’s SSL certificate.
        /// </summary>
        Transport = 1,

        /// <summary>
        /// This mode provides integrity, confidentiality, and server authentication using
        /// HTTPS. The service must be configured with a certificate. Client authentication
        /// is provided by SOAP message security.This mode is applicable when the user is
        /// authenticating with a UserName or Certificate credential and there is an existing
        /// HTTPS deployment for securing message transfer.
        /// </summary>
        TransportWithMessageCredential = 3,

        /// <summary>
        /// This mode provides only HTTP-based client authentication. It does not provide
        /// message integrity or confidentiality.
        /// </summary>
        TransportCredentialOnly = 4,
    }
}