
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
    /// Specifies that a member is serialized as an element inside the SOAP body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public partial class MessageBodyMemberAttribute : MessageContractMemberAttribute
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