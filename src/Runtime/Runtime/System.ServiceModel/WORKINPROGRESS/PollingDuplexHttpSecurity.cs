
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
    /// Specifies the security used for a Silverlight client configured with a <see cref="PollingDuplexHttpBinding"/>.
    /// </summary>
    public sealed class PollingDuplexHttpSecurity
    {
        internal PollingDuplexHttpSecurity() { }

        /// <summary>
        /// Gets or sets the mode of security for a client configured with a <see cref="PollingDuplexHttpBinding"/>.
        /// </summary>
        /// <returns>
        /// A value in the <see cref="PollingDuplexHttpSecurityMode"/> enumeration.
        /// The default value for this property is <see cref="PollingDuplexHttpSecurityMode.None"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value is not a legal value for <see cref="PollingDuplexHttpSecurityMode"/>.
        /// </exception>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpSecurityMode Mode { get; set; }
    }
}
