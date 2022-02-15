
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

using System.Text;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Represents a binding that a Silverlight 5 client can use to configure endpoints
    /// that can communicate with Windows Communication Foundation (WCF) services that
    /// are similarly configured for duplex communication with a polling client.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class PollingDuplexHttpBinding : Binding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PollingDuplexHttpBinding"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpBinding() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingDuplexHttpBinding"/>
        /// class with a specified type of security used by the binding.
        /// </summary>
        /// <param name="securityMode">
        /// The value of <see cref="PollingDuplexHttpSecurityMode"/> that specifies
        /// the type of security that is used with the SOAP message and for the client.
        /// </param>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpBinding(PollingDuplexHttpSecurityMode securityMode) { }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.PollingDuplexHttpBinding
        /// class with a specified mode of behavior on the server in response to client polling.
        /// </summary>
        /// <param name="duplexMode">
        /// The mode of behavior exhibited on a server in response to client polling when
        /// the communication between them is configured for duplex polling.
        /// </param>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpBinding(PollingDuplexMode duplexMode) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingDuplexHttpBinding"/>
        /// class with a specified type of security and a specified mode of behavior on the
        /// server in the way it responses to client polling.
        /// </summary>
        /// <param name="securityMode">
        /// The value of <see cref="PollingDuplexHttpSecurityMode"/> that specifies
        /// the type of security that is used with the SOAP message and for the client.
        /// </param>
        /// <param name="duplexMode">
        /// The mode of behavior exhibited on a server in response to client polling when
        /// the communication between them is configured for duplex polling.
        /// </param>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpBinding(PollingDuplexHttpSecurityMode securityMode, PollingDuplexMode duplexMode) { }

        /// <summary>
        /// Gets or sets a value that specifies how the server behaves in response to client
        /// polling when the communication is configured for duplex polling.
        /// </summary>
        /// <returns>
        /// A <see cref="PollingDuplexMode"/> object whose value indicates how the server 
        /// behaves in response to client polling when the communication is configured for 
        /// duplex. The default value is <see cref="PollingDuplexMode.SingleMessagePerPoll"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PollingDuplexMode DuplexMode { get; set; }

        /// <summary>
        /// Gets the version of SOAP that is used for messages that are processed by this binding.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="EnvelopeVersion"/> that is used with this binding.
        /// The value is always SOAP 1.2.
        /// </returns>
        [OpenSilver.NotImplemented]
        public EnvelopeVersion EnvelopeVersion { get; }

        /// <summary>
        /// Gets or sets the maximum interval of time that can pass without activity on a
        /// channel before the client or server channel enters a faulted state. The default
        /// value is 10 minutes.
        /// </summary>
        /// <returns>
        /// The System.TimeSpan value that specifies the maximum interval of time that can
        /// pass without activity on a channel before the channel enters a faulted state.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value is less than zero, or it is larger than the maximum time allowed.
        /// </exception>
        [OpenSilver.NotImplemented]
        public TimeSpan InactivityTimeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum size for a buffer that receives messages from the channel.
        /// </summary>
        /// <returns>
        /// The maximum size, in bytes, of a buffer that stores messages while they are processed
        /// for an endpoint configured with this binding. The default value is 65,536 bytes.
        /// </returns>
        [OpenSilver.NotImplemented]
        public int MaxBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size for a message that can be received on a channel
        /// configured with this binding.
        /// </summary>
        /// <return>
        /// The maximum size, in bytes, for a message that is processed by the binding. The
        /// default value is 65,536 bytes.
        /// </return>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value is less than zero.
        /// </exception>
        [OpenSilver.NotImplemented]
        public long MaxReceivedMessageSize { get; set; }

        /// <summary>
        /// Gets the URI transport scheme for the channels and listeners that are configured
        /// with this binding.
        /// </summary>
        /// <returns>
        /// "https" if the security mode in the transport binding element is set to 
        /// <see cref="F:System.ServiceModel.BasicHttpSecurityMode.Transport" /> or 
        /// <see cref="F:System.ServiceModel.BasicHttpSecurityMode.TransportWithMessageCredentials" />; 
        /// otherwise, "http".
        /// </returns>
        [OpenSilver.NotImplemented]
        public override string Scheme { get; }

        /// <summary>
        /// Gets the type of security used with this binding.
        /// </summary>
        /// <returns>
        /// The <see cref="PollingDuplexHttpSecurity"/> that is used with this binding.
        /// The default value is <see cref="F:System.ServiceModel.BasicHttpSecurityMode.None" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PollingDuplexHttpSecurity Security { get; }

        /// <summary>
        /// Gets or sets the character encoding that is used for the message text.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/> that indicates the character encoding that is used.
        /// The default is <see cref="UTF8Encoding"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Encoding TextEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether text or binary encoding is used for
        /// the message.
        /// </summary>
        /// <returns>
        /// true if text encoding is used; false if binary encoding is used.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool UseTextEncoding { get; set; }

        /// <summary>
        /// Returns an ordered collection of binding elements contained in the current binding.
        /// </summary>
        /// <returns>
        /// The <see cref="BindingElementCollection"/> that contains the ordered stack of 
        /// binding elements described by the <see cref="PollingDuplexHttpBinding" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public override BindingElementCollection CreateBindingElements()
        {
            throw new NotImplementedException();
        }
    }
}
