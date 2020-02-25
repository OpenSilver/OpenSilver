
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


#if !OPENSILVER

#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Indicates that a method defines an operation that is part of a service contract
    /// in a Windows Communication Foundation (WCF) application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed partial class OperationContractAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.OperationContractAttribute
        /// class.
        /// </summary>
        public OperationContractAttribute() { }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The value is null.
        /// <summary>
        /// Gets or sets the WS-Addressing action of the request message.
        /// </summary>
        public string Action { get; set; }
   
        // Returns:
        //     true if the Begin<methodName>method is matched by an End<methodName> method
        //     and can be treated by the infrastructure as an operation that is implemented
        //     as an asynchronous method pair on the service interface; otherwise, false.
        //     The default is false.
        /// <summary>
        /// Indicates that an operation is implemented asynchronously using a Begin+methodName
        /// and End+methodName method pair in a service contract.
        /// </summary>
        public bool AsyncPattern { get; set; }
        
        // Returns:
        //     true if this method receives a request message and returns no reply message;
        //     otherwise, false. The default is false.
        /// <summary>
        /// Gets or sets a value that indicates whether an operation returns a reply
        /// message.
        /// </summary>
        public bool IsOneWay { get; set; }
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     System.ServiceModel.OperationContractAttribute.Name is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     The value is an empty string.
        /// <summary>
        /// Gets or sets the name of the operation.
        /// </summary>
        public string Name { get; set; }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     System.ServiceModel.OperationContractAttribute.ReplyAction is null.
        /// <summary>
        /// Gets or sets the value of the SOAP action for the reply message of the operation.
        /// </summary>
        public string ReplyAction { get; set; }
    }
}

#endif
#endif
