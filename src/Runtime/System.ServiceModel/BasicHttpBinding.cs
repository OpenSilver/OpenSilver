
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Represents a binding that a Windows Communication Foundation (WCF) service
    /// can use to configure and expose endpoints that are able to communicate with
    /// ASMX-based Web services and clients and other services that conform to the
    /// WS-I Basic Profile 1.1.
    /// </summary>
    public partial class BasicHttpBinding : HttpBindingBase
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.BasicHttpBinding class.
        /// </summary>
        public BasicHttpBinding()
        {
            Security = new BasicHttpSecurity()
            {
                Mode = BasicHttpSecurityMode.None
            };
        }

        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.BasicHttpBinding class
        /// with a specified type of security used by the binding.
        /// </summary>
        /// <param name="securityMode">
        /// The value of System.ServiceModel.BasicHttpSecurityMode that specifies the
        /// type of security that is used with the SOAP message and for the client.
        /// </param>
        public BasicHttpBinding(BasicHttpSecurityMode securityMode)
        {
            Security = new BasicHttpSecurity()
            {
                Mode = securityMode
            };
        }

        /// <summary>
        /// Gets the type of security used with this binding.
        /// </summary>
        public BasicHttpSecurity Security { get; set; }
    }
}

#endif