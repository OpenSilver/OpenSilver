#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies that a type provides a particular export, and that subclasses of that
    //     type will also provide that export.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public partial class InheritedExportAttribute : ExportAttribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.InheritedExportAttribute
        //     class.
        public InheritedExportAttribute() : base()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.InheritedExportAttribute
        //     class with the specified contract type.
        //
        // Parameters:
        //   contractType:
        //     The type of the contract.
        public InheritedExportAttribute(Type contractType) : base(contractType)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.InheritedExportAttribute
        //     class with the specified contract name.
        //
        // Parameters:
        //   contractName:
        //     The name of the contract.
        public InheritedExportAttribute(string contractName) : base(contractName)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.InheritedExportAttribute
        //     class with the specified contract name and type.
        //
        // Parameters:
        //   contractName:
        //     The name of the contract.
        //
        //   contractType:
        //     The type of the contract.
        public InheritedExportAttribute(string contractName, Type contractType) : base(contractName, contractType)
        {

        }
    }
}
#endif