
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
    /// Specifies that a member is serialized as an element inside the SOAP body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class MessageBodyMemberAttribute : MessageContractMemberAttribute
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.MessageBodyMemberAttribute
        /// class.
        /// </summary>
        public MessageBodyMemberAttribute() { }

     
        // Returns:
        //     The location of the element in the SOAP body.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value passed to the property setter is a negative integer.
        /// <summary>
        /// Gets or sets a value that indicates the position in which the member is serialized
        /// into the SOAP body.
        /// </summary>
        public int Order { get; set; }
    }
}

#endif