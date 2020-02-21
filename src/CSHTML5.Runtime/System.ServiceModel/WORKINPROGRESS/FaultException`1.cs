#if WORKINPROGRESS
namespace System.ServiceModel
{
    //
    // Summary:
    //     Used in a client application to catch contractually specified SOAP faults.
    //
    // Type parameters:
    //   TDetail:
    //     The serializable error detail type.
    public partial class FaultException<TDetail> : FaultException
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.FaultException`1 class
        //     that uses a specified detail object and SOAP fault reason, code and action values.
        //
        // Parameters:
        //   detail:
        //     The object used as the SOAP fault detail.
        //
        //   reason:
        //     The reason for the SOAP fault.
        //
        //   code:
        //     The fault code for the SOAP fault.
        //
        //   action:
        //     The action of the SOAP fault.
        public FaultException(TDetail detail, FaultReason reason, FaultCode code, string action)
        {

        }

        //
        // Summary:
        //     Gets the object that contains the detail information of the fault condition.
        //
        // Returns:
        //     The detail object that is the type parameter of the System.ServiceModel.FaultException`1
        //     object.
        public TDetail Detail { get; }

        //
        // Summary:
        //     Returns a string for the System.ServiceModel.FaultException`1 object.
        //
        // Returns:
        //     The string for the SOAP fault.
        public override string ToString()
        {
            return null;
        }
    }
}

#endif