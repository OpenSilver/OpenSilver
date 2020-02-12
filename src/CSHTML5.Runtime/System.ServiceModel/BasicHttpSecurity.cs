
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



#if WCF_STACK || BRIDGE

using System;
using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// Configures the security settings of a basicHttpBinding binding.
    /// </summary>
    public sealed partial class BasicHttpSecurity
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