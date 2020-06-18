
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
    /// Defines a strongly-typed class that corresponds to a SOAP message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed partial class MessageContractAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.MessageContractAttribute
        /// class.
        /// </summary>
        public MessageContractAttribute() { }

        /// <summary>
        /// Gets a value that indicates whether the message has a protection level.
        /// </summary>
        public bool HasProtectionLevel { get; private set; }
        
        /// <summary>
        /// Gets or sets a value that specifies whether the message body has a wrapper
        /// element.
        /// </summary>
        public bool IsWrapped { get; set; }
        
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value passed to the property when the setting is not one of the System.Net.Security.ProtectionLevel
        //     values.
        /// <summary>
        /// Gets or sets a value that specified whether the message must be encrypted,
        /// signed, or both.
        /// </summary>
        public ProtectionLevel ProtectionLevel { get; set; }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     The value is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     The value is an empty string.
        /// <summary>
        /// Gets or sets the name of the wrapper element of the message body.
        /// </summary>
        public string WrapperName { get; set; }
        
        /// <summary>
        /// Gets or sets the namespace of the message body wrapper element.
        /// </summary>
        public string WrapperNamespace { get; set; }
    }
}

#endif