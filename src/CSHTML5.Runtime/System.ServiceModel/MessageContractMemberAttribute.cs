
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
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Declares the base members for System.ServiceModel.MessageBodyMemberAttribute
    /// and System.ServiceModel.MessageHeaderAttribute.
    /// </summary>
    public abstract partial class MessageContractMemberAttribute : Attribute
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