
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
    /// Indicates that a method defines an operation that is part of a service contract
    /// in a Windows Communication Foundation (WCF) application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OperationContractAttribute : Attribute
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
