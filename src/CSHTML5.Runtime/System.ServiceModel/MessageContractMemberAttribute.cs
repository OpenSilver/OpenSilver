
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
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Declares the base members for System.ServiceModel.MessageBodyMemberAttribute
    /// and System.ServiceModel.MessageHeaderAttribute.
    /// </summary>
    public abstract class MessageContractMemberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.MessageContractMemberAttribute
        /// class.
        /// </summary>
        protected MessageContractMemberAttribute() { }

        /// <summary>
        /// When overridden in a derived class, gets a value that indicates whether the
        /// member has a protection level assigned.
        /// </summary>
        public bool HasProtectionLevel { get; private set; }
        
        /// <summary>
        /// Specifies the name of the element that corresponds to this member.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Specifies the namespace of the element that corresponds to this member.
        /// </summary>
        public string Namespace { get; set; }
        
        /// <summary>
        /// Specifies whether the member is to be transmitted as-is, signed, or signed
        /// and encrypted.
        /// </summary>
        public ProtectionLevel ProtectionLevel { get; set; }
    }
}

#endif