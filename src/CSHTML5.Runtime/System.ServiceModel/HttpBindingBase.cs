
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
using System.Text;

namespace System.ServiceModel
{
    // Summary:
    //     Specifies the base HTTP binding.
    // BRIDGETODO is the class working ?
    public abstract class HttpBindingBase : Binding //, IBindingRuntimePreferences
    {
        //todo: uncomment the "DefaultValue" attributes because it can be useful for the end-user to know what the Simulator default value is (though it is useless in JavaScript, and it doesn't change the runtime because the DLL is swapped with the MS version of the same DLL during runtime in the simulator).


        //[DefaultValue(524288)]
        /// Returns:
        ///     The maximum amount of memory, in bytes, that is allocated for use by the
        ///     manager of the message buffers that receive messages from the channel.
        /// <summary>
        /// Gets or sets the maximum amount of memory, in bytes, that is allocated for
        /// use by the manager of the message buffers that receive messages from the
        /// channel.
        /// </summary>
        public long MaxBufferPoolSize { get; set; }

        //[DefaultValue(65536)]
        /// Returns:
        ///     The maximum size, in bytes, for a buffer that receives messages from the
        ///     channel.
        /// <summary>
        /// Gets or sets the maximum size, in bytes, for a buffer that receives messages
        /// from the channel.
        /// </summary>
        public int MaxBufferSize { get; set; }

        //[DefaultValue(65536)]
        /// Returns:
        ///     The maximum size, in bytes, for a message that can be received on a channel
        ///     configured with this binding.
        /// <summary>
        /// Gets or sets the maximum size, in bytes, for a message that can be received
        /// on a channel configured with this binding.
        /// </summary>
        public long MaxReceivedMessageSize { get; set; }

        //[DefaultValue(false)]
        // Returns:
        //     true if cookies are allowed; otherwise, false.
        /// <summary>
        /// Gets or sets a value that indicates whether the client accepts cookies and
        /// propagates them on future requests.
        /// </summary>
        public bool AllowCookies { get; set; }


#region Not Implemented

        //// Summary:
        ////     Gets or sets a value that indicates whether the client accepts cookies and
        ////     propagates them on future requests.
        ////
        //// Returns:
        ////     true if cookies are allowed; otherwise, false.
        //[DefaultValue(false)]
        //public bool AllowCookies { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether to bypass the proxy server for
        ////     local addresses.
        ////
        //// Returns:
        ////     true to bypass the proxy server for local addresses; otherwise, false.
        //[DefaultValue(false)]
        //public bool BypassProxyOnLocal { get; set; }
        ////
        //// Summary:
        ////     Gets the version of SOAP that is used for messages that are processed by
        ////     this binding.
        ////
        //// Returns:
        ////     The version of SOAP that is used for messages that are processed by this
        ////     binding.
        //public EnvelopeVersion EnvelopeVersion { get; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether the hostname is used to reach
        ////     the service when matching the URI.
        ////
        //// Returns:
        ////     true if the hostname is used for matching the URI; otherwise, false.
        //public HostNameComparisonMode HostNameComparisonMode { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the URI address of the HTTP proxy.
        ////
        //// Returns:
        ////     The URI address of the HTTP proxy.
        //[TypeConverter(typeof(UriTypeConverter))]
        //public Uri ProxyAddress { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the constraints on the complexity of SOAP messages that can
        ////     be processed by endpoints configured with this binding.
        ////
        //// Returns:
        ////     The constraints on the complexity of SOAP messages that can be processed
        ////     by endpoints configured with this binding.
        //public XmlDictionaryReaderQuotas ReaderQuotas { get; set; }
        ////
        //// Summary:
        ////     Gets the URI transport scheme for the channels and listeners that are configured
        ////     with this binding.
        ////
        //// Returns:
        ////     The URI transport scheme for the channels and listeners that are configured
        ////     with this binding.
        //public override string Scheme { get; }
        ////
        //// Summary:
        ////     Gets or sets the character encoding that is used for the message text.
        ////
        //// Returns:
        ////     The character encoding that is used for the message text.
        //public Encoding TextEncoding { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether messages are sent buffered or
        ////     streamed.
        ////
        //// Returns:
        ////     One of the enumeration values of System.ServiceModel.TransferMode that indicates
        ////     whether messages are sent buffered or streamed.
        //public TransferMode TransferMode { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a value that indicates whether the auto-configured HTTP proxy
        ////     of the system should be used, if available.
        ////
        //// Returns:
        ////     true if the auto-configured HTTP proxy of the system should be used, if available;
        ////     otherwise, false.
        //[DefaultValue(true)]
        //public bool UseDefaultWebProxy { get; set; }

        //// Summary:
        ////     Returns whether the constraint values placed on the complexity of SOAP message
        ////     structure should be serialized.
        ////
        //// Returns:
        ////     true if reader quotas should be serialized; otherwise, false.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool ShouldSerializeReaderQuotas();
        ////
        //// Summary:
        ////     Returns whether settings for text encoding should be serialized.
        ////
        //// Returns:
        ////     true if text encoding should be serialized; otherwise, false.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public bool ShouldSerializeTextEncoding();

#endregion
    }
}

#endif