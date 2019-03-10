
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
    public class BasicHttpBinding : HttpBindingBase
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