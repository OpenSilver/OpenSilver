using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// Specifies the security protocols that are supported by the Schannel security
    /// package.
    /// </summary>
    [Flags]
    public enum SecurityProtocolType
    {
        /// <summary>
        /// Specifies the system default security protocol as defined by Schannel.
        /// </summary>
        SystemDefault = 0,
        /// <summary>
        /// Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
        /// </summary>
        Ssl3 = 48,
        /// <summary>
        /// Specifies the Transport Layer Security (TLS) 1.0 security protocol.
        /// </summary>
        Tls = 192,
        /// <summary>
        /// Specifies the Transport Layer Security (TLS) 1.1 security protocol.
        /// </summary>
        Tls11 = 768,
        /// <summary>
        /// Specifies the Transport Layer Security (TLS) 1.2 security protocol.
        /// </summary>
        Tls12 = 3072
    }
}
