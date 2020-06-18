using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// Manages the collection of System.Net.ServicePoint objects.
    /// </summary>
    public class ServicePointManager
    {
        // Returns:
        //     One of the values defined in the System.Net.SecurityProtocolType enumeration.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The value specified to set the property is not a valid System.Net.SecurityProtocolType
        //     enumeration value.
        /// <summary>
        /// Gets or sets the security protocol used by the System.Net.ServicePoint objects
        /// managed by the System.Net.ServicePointManager object.
        /// </summary>
        public static SecurityProtocolType SecurityProtocol { get; set; }

        #region not implemented
        ///// <summary>
        ///// The default number of non-persistent connections (4) allowed on a System.Net.ServicePoint
        ///// object connected to an HTTP/1.0 or later server. This field is constant but is
        ///// no longer used in the .NET Framework 2.0.
        ///// </summary>
        //public const int DefaultNonPersistentConnectionLimit = 4;
        ///// <summary>
        ///// The default number of persistent connections (2) allowed on a System.Net.ServicePoint
        ///// object connected to an HTTP/1.1 or later server. This field is constant and is
        ///// used to initialize the System.Net.ServicePointManager.DefaultConnectionLimit
        ///// property if the value of the System.Net.ServicePointManager.DefaultConnectionLimit
        ///// property has not been set either directly or through configuration.
        ///// </summary>
        //public const int DefaultPersistentConnectionLimit = 2;

        ///// <summary>
        ///// Setting this property value to true causes all outbound TCP connections from
        ///// HttpWebRequest to use the native socket option SO_REUSE_UNICASTPORT on the socket.
        ///// This causes the underlying outgoing ports to be shared. This is useful for scenarios
        ///// where a large number of outgoing connections are made in a short time, and the
        ///// app risks running out of ports.
        ///// </summary>
        //public static bool ReusePort { get; set; }
        
        //// Returns:
        ////     A System.Net.Security.RemoteCertificateValidationCallback. The default value
        ////     is null.
        ///// <summary>
        ///// Gets or sets the callback to validate a server certificate.
        ///// </summary>
        //public static RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
       

        ///// <summary>
        ///// Gets or sets policy for server certificates.
        ///// </summary>
        //[Obsolete("CertificatePolicy is obsoleted for this type, please use ServerCertificateValidationCallback instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        //public static ICertificatePolicy CertificatePolicy { get; set; }
       
        //// Returns:
        ////     The time-out value, in milliseconds. A value of -1 indicates an infinite time-out
        ////     period. The default value is 120,000 milliseconds (two minutes).
        ///// <summary>
        ///// Gets or sets a value that indicates how long a Domain Name Service (DNS) resolution
        ///// is considered valid.
        ///// </summary>
        //public static int DnsRefreshTimeout { get; set; }
       
        //// Returns:
        ////     false if a DNS resolution always returns the first IP address for a particular
        ////     host; otherwise true. The default is false.
        ///// <summary>
        ///// Gets or sets a value that indicates whether a Domain Name Service (DNS) resolution
        ///// rotates among the applicable Internet Protocol (IP) addresses.
        ///// </summary>
        //public static bool EnableDnsRoundRobin { get; set; }
       
        //// Returns:
        ////     true to enable 100-Continue behavior. The default value is true.
        ///// <summary>
        ///// Gets or sets a System.Boolean value that determines whether 100-Continue behavior
        ///// is used.
        ///// </summary>
        //public static bool Expect100Continue { get; set; }
      
        //// Returns:
        ////     true to use the Nagle algorithm; otherwise, false. The default value is true.
        ///// <summary>
        ///// Determines whether the Nagle algorithm is used by the service points managed
        ///// by this System.Net.ServicePointManager object.
        ///// </summary>
        //public static bool UseNagleAlgorithm { get; set; }
        
        //// Returns:
        ////     The maximum idle time, in milliseconds, of a System.Net.ServicePoint object.
        ////     The default value is 100,000 milliseconds (100 seconds).
        ////
        //// Exceptions:
        ////   T:System.ArgumentOutOfRangeException:
        ////     System.Net.ServicePointManager.MaxServicePointIdleTime is less than System.Threading.Timeout.Infinite
        ////     or greater than System.Int32.MaxValue.
        ///// <summary>
        ///// Gets or sets the maximum idle time of a System.Net.ServicePoint object.
        ///// </summary>
        //public static int MaxServicePointIdleTime { get; set; }
        
        //// Returns:
        ////     The maximum number of concurrent connections allowed by a System.Net.ServicePoint
        ////     object. The default value is 2. When an app is running as an ASP.NET host, it
        ////     is not possible to alter the value of this property through the config file if
        ////     the autoConfig property is set to true. However, you can change the value programmatically
        ////     when the autoConfig property is true. Set your preferred value once, when the
        ////     AppDomain loads.
        ////
        //// Exceptions:
        ////   T:System.ArgumentOutOfRangeException:
        ////     System.Net.ServicePointManager.DefaultConnectionLimit is less than or equal to
        ////     0.
        ///// <summary>
        ///// Gets or sets the maximum number of concurrent connections allowed by a System.Net.ServicePoint
        ///// object.
        ///// </summary>
        //public static int DefaultConnectionLimit { get; set; }
      
        //// Returns:
        ////     The maximum number of System.Net.ServicePoint objects to maintain. The default
        ////     value is 0, which means there is no limit to the number of System.Net.ServicePoint
        ////     objects.
        ////
        //// Exceptions:
        ////   T:System.ArgumentOutOfRangeException:
        ////     System.Net.ServicePointManager.MaxServicePoints is less than 0 or greater than
        ////     System.Int32.MaxValue.
        ///// <summary>
        ///// Gets or sets the maximum number of System.Net.ServicePoint objects to maintain
        ///// at any time.
        ///// </summary>
        //public static int MaxServicePoints { get; set; }
       
        ////
        //// Returns:
        ////     true if the certificate revocation list is checked; otherwise, false.
        ///// <summary>
        ///// Gets or sets a System.Boolean value that indicates whether the certificate is
        ///// checked against the certificate authority revocation list.
        ///// </summary>
        //public static bool CheckCertificateRevocationList { get; set; }
       
        ///// <summary>
        ///// Gets the System.Net.Security.EncryptionPolicy for this System.Net.ServicePointManager
        ///// instance.
        ///// </summary>
        //public static EncryptionPolicy EncryptionPolicy { get; }

        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     address is null.
        ////
        ////   T:System.InvalidOperationException:
        ////     The maximum number of System.Net.ServicePoint objects defined in System.Net.ServicePointManager.MaxServicePoints
        ////     has been reached.
        ///// <summary>
        ///// Finds an existing System.Net.ServicePoint object or creates a new System.Net.ServicePoint
        ///// object to manage communications with the specified System.Uri object.
        ///// </summary>
        ///// <param name="address">A System.Uri object that contains the address of the Internet resource to contact.</param>
        ///// <param name="proxy">The proxy data for this request.</param>
        ///// <returns>The System.Net.ServicePoint object that manages communications for the request.</returns>
        //public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy);
       
        //// Exceptions:
        ////   T:System.UriFormatException:
        ////     The URI specified in uriString is invalid.
        ////
        ////   T:System.InvalidOperationException:
        ////     The maximum number of System.Net.ServicePoint objects defined in System.Net.ServicePointManager.MaxServicePoints
        ////     has been reached.
        ///// <summary>
        ///// Finds an existing System.Net.ServicePoint object or creates a new System.Net.ServicePoint
        ///// object to manage communications with the specified Uniform Resource Identifier
        ///// (URI).
        ///// </summary>
        ///// <param name="uriString">The URI of the Internet resource to be contacted.</param>
        ///// <param name="proxy">The proxy data for this request.</param>
        ///// <returns>The System.Net.ServicePoint object that manages communications for the request.</returns>
        //public static ServicePoint FindServicePoint(string uriString, IWebProxy proxy);
        
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     address is null.
        ////
        ////   T:System.InvalidOperationException:
        ////     The maximum number of System.Net.ServicePoint objects defined in System.Net.ServicePointManager.MaxServicePoints
        ////     has been reached.
        ///// <summary>
        ///// Finds an existing System.Net.ServicePoint object or creates a new System.Net.ServicePoint
        ///// object to manage communications with the specified System.Uri object.
        ///// </summary>
        ///// <param name="address">The System.Uri object of the Internet resource to contact.</param>
        ///// <returns>The System.Net.ServicePoint object that manages communications for the request.</returns>
        //public static ServicePoint FindServicePoint(Uri address);
      
        //// Exceptions:
        ////   T:System.ArgumentOutOfRangeException:
        ////     The value specified for keepAliveTime or keepAliveInterval parameter is less
        ////     than or equal to 0.
        ///// <summary>
        ///// Enables or disables the keep-alive option on a TCP connection.
        ///// </summary>
        ///// <param name="enabled">
        ///// If set to true, then the TCP keep-alive option on a TCP connection will be enabled
        ///// using the specified keepAliveTime and keepAliveInterval values. If set to false,
        ///// then the TCP keep-alive option is disabled and the remaining parameters are ignored.The
        ///// default value is false.
        ///// </param>
        ///// <param name="keepAliveTime">
        ///// Specifies the timeout, in milliseconds, with no activity until the first keep-alive
        ///// packet is sent.The value must be greater than 0. If a value of less than or equal
        ///// to zero is passed an System.ArgumentOutOfRangeException is thrown.
        ///// </param>
        ///// <param name="keepAliveInterval">
        ///// Specifies the interval, in milliseconds, between when successive keep-alive packets
        ///// are sent if no acknowledgement is received.The value must be greater than 0.
        ///// If a value of less than or equal to zero is passed an System.ArgumentOutOfRangeException
        ///// is thrown.
        ///// </param>
        //public static void SetTcpKeepAlive(bool enabled, int keepAliveTime, int keepAliveInterval);
        #endregion
    }
}
