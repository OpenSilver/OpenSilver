
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



#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Represents a SOAP fault.
    /// </summary>
    [Serializable]
    //[KnownType(typeof(FaultException.FaultCodeData))]
    //[KnownType(typeof(FaultException.FaultReasonData))]
        //todo: remove the inheritance from Exception and inherit from CommunicationException instead.
    public class FaultException :Exception //: CommunicationException
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.FaultException class.
        /// </summary>
        public FaultException() { }
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified reason.
        ////
        //// Parameters:
        ////   reason:
        ////     The reason for the SOAP fault.
        //public FaultException(FaultReason reason);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified message fault values.
        ////
        //// Parameters:
        ////   fault:
        ////     The message fault that contains the default SOAP fault values.
        //public FaultException(MessageFault fault);
       
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.FaultException class
        /// with the specified fault reason.
        /// </summary>
        /// <param name="reason">The reason for the fault.</param>
        public FaultException(string reason) //todo: see if this should do this or something else. if something else, how do we set Message?
        {
            _message = reason;
        }
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified reason and fault code.
        ////
        //// Parameters:
        ////   reason:
        ////     The reason for the SOAP fault.
        ////
        ////   code:
        ////     The fault code for the SOAP fault.
        //public FaultException(FaultReason reason, FaultCode code);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified message fault values and the provided action string.
        ////
        //// Parameters:
        ////   fault:
        ////     The message fault that contains the default SOAP fault values to use.
        ////
        ////   action:
        ////     The action of the SOAP fault.
        //public FaultException(MessageFault fault, string action);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified serialization information and context when deserializing
        ////     a stream into a System.ServiceModel.FaultException object.
        ////
        //// Parameters:
        ////   info:
        ////     The serialization information necessary to reconstruct the System.ServiceModel.FaultException
        ////     object from a stream.
        ////
        ////   context:
        ////     The streaming context required to reconstruct the System.ServiceModel.FaultException
        ////     object.
        //protected FaultException(SerializationInfo info, StreamingContext context);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified reason and SOAP fault code.
        ////
        //// Parameters:
        ////   reason:
        ////     The reason for the SOAP fault.
        ////
        ////   code:
        ////     The SOAP fault code for the fault.
        //public FaultException(string reason, FaultCode code);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified reason, fault code, and action value.
        ////
        //// Parameters:
        ////   reason:
        ////     The reason for the SOAP fault.
        ////
        ////   code:
        ////     The fault code for the SOAP fault.
        ////
        ////   action:
        ////     The action value for the SOAP fault.
        //public FaultException(FaultReason reason, FaultCode code, string action);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.ServiceModel.FaultException class
        ////     using the specified reason, fault code, and action value.
        ////
        //// Parameters:
        ////   reason:
        ////     The reason for the SOAP fault.
        ////
        ////   code:
        ////     The fault code for the SOAP fault.
        ////
        ////   action:
        ////     The action value for the SOAP fault.
        //public FaultException(string reason, FaultCode code, string action);

        //// Summary:
        ////     Gets the value of the SOAP action for the fault message.
        ////
        //// Returns:
        ////     The value of the SOAP action for the fault message.
        //public string Action { get; }
        ////
        //// Summary:
        ////     Gets the fault code for the SOAP fault.
        ////
        //// Returns:
        ////     The fault code for the SOAP fault.
        //public FaultCode Code { get; }
        string _message;
        
        /// <summary>
        /// Gets the message for the exception.
        /// </summary>
        public override string Message//note: that's all we do for now in FaultException (we'll probably need more when custom faultExceptions will be supported because it will require an actual deserialization of the user's FaultException).
        {
            get { return _message; }
        } 
        ////
        //// Summary:
        ////     Gets the System.ServiceModel.FaultReason for the SOAP fault.
        ////
        //// Returns:
        ////     The reason for the SOAP fault.
        //public FaultReason Reason { get; }

        //// Summary:
        ////     Returns a System.ServiceModel.Channels.FaultException object from the specified
        ////     message fault and an array of detail types.
        ////
        //// Parameters:
        ////   messageFault:
        ////     The message fault that contains default SOAP fault information.
        ////
        ////   faultDetailTypes:
        ////     An array of types that contains fault details.
        ////
        //// Returns:
        ////     A System.ServiceModel.FaultException object that you can throw to indicate
        ////     that a SOAP fault message was received.
        //public static FaultException CreateFault(MessageFault messageFault, params Type[] faultDetailTypes);
        ////
        //// Summary:
        ////     Returns a System.ServiceModel.Channels.FaultException object from the specified
        ////     message fault, action and an array of detail types.
        ////
        //// Parameters:
        ////   messageFault:
        ////     The message fault that contains default SOAP fault information.
        ////
        ////   action:
        ////     The fault action value.
        ////
        ////   faultDetailTypes:
        ////     An array of types that contains fault details.
        ////
        //// Returns:
        ////     A System.ServiceModel.FaultException object that you can throw to indicate
        ////     that a SOAP fault message was received.
        //public static FaultException CreateFault(MessageFault messageFault, string action, params Type[] faultDetailTypes);
        ////
        //// Summary:
        ////     Returns a System.ServiceModel.Channels.MessageFault object.
        ////
        //// Returns:
        ////     The in-memory representation of a SOAP fault that can be passed to Overload:System.ServiceModel.Channels.Message.CreateMessage
        ////     to create a message that contains a fault. For System.ServiceModel.FaultException
        ////     objects that result from fault messages, this System.ServiceModel.Channels.MessageFault
        ////     object is the fault that arrives.
        //public virtual MessageFault CreateMessageFault();
        ////
        //// Summary:
        ////     Implementation of the System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
        ////     method that is called when the object is serialized into a stream.
        ////
        //// Parameters:
        ////   info:
        ////     The serialization information to which the object data is added when serialized.
        ////
        ////   context:
        ////     The destination for the serialized object.
        //[SecurityCritical]
        //public override void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}

#endif