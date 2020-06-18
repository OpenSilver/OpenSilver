using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace System.ServiceModel.Description
{
    //
    // Summary:
    //     A contract that characterizes an operation in terms of the messages it exchanges.
    [DebuggerDisplay("Name={name}, IsInitiating={isInitiating}, IsTerminating={isTerminating}")]
    public class OperationDescription
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Description.OperationDescription
        //     class with a specified name and contract description.
        //
        // Parameters:
        //   name:
        //     The name of the operation description.
        //
        //   declaringContract:
        //     The System.ServiceModel.Description.ContractDescription used to initialize the
        //     operation description.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name or declaringContract is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     name is empty.
        public OperationDescription(string name, ContractDescription declaringContract)
        {

        }

        //
        // Summary:
        //     Gets or sets the begin method of the operation.
        //
        // Returns:
        //     The System.Reflection.MethodInfo that provides access to the attributes and metadata
        //     of the method.
        public MethodInfo BeginMethod { get; set; }
        //
        // Summary:
        //     Gets or sets the operation behaviors associated with the operation.
        //
        // Returns:
        //     A System.Collections.Generic.KeyedByTypeCollection`1 that contains the System.ServiceModel.Description.IOperationBehavior
        //     objects associated with the operation.
        public KeyedByTypeCollection<IOperationBehavior> Behaviors { get; }
        //
        // Summary:
        //     Gets or sets the contract to which the operation belongs.
        //
        // Returns:
        //     The System.ServiceModel.Description.ContractDescription for the operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     value set is null.
        public ContractDescription DeclaringContract { get; set; }
        //
        // Summary:
        //     Gets or sets the end method of the operation.
        //
        // Returns:
        //     The System.Reflection.MethodInfo that provides access to the attributes and metadata
        //     of the method.
        public MethodInfo EndMethod { get; set; }
        //
        // Summary:
        //     Gets a collection of the descriptions of the faults associated with the operation
        //     description.
        //
        // Returns:
        //     The System.ServiceModel.Description.FaultDescriptionCollection that contains
        //     details about the faults associated with the operation description.
        public FaultDescriptionCollection Faults { get; }
        //
        // Summary:
        //     Gets a value that indicates whether an operation returns a reply message.
        //
        // Returns:
        //     true if this method receives a request message and returns no reply message;
        //     otherwise, false. The default is false.
        public bool IsOneWay { get; }
        //
        // Summary:
        //     Gets the known types associated with the operation description.
        //
        // Returns:
        //     The collection of known types associated with the operation description.
        public Collection<Type> KnownTypes { get; }
        //
        // Summary:
        //     Gets or sets the descriptions of the messages that make up the operation.
        //
        // Returns:
        //     A System.ServiceModel.Description.MessageDescriptionCollection that contains
        //     descriptions of the messages that make up the operation.
        public MessageDescriptionCollection Messages { get; }
        //
        // Summary:
        //     Gets or sets the name of the operation description.
        //
        // Returns:
        //     The name of the operation description.
        public string Name { get; }
        //
        // Summary:
        //     Gets or sets the service synchronization method of the operation description.
        //
        // Returns:
        //     The System.Reflection.MethodInfo that provides access to the attributes and metadata
        //     of the method.
        public MethodInfo SyncMethod { get; set; }
    }
}