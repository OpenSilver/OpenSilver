#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies that a type, property, field, or method provides a particular export.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public partial class ExportAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ExportAttribute
        //     class, exporting the type or member marked with this attribute under the default
        //     contract name.
        public ExportAttribute()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ExportAttribute
        //     class, exporting the type or member marked with this attribute under a contract
        //     name derived from the specified type.
        //
        // Parameters:
        //   contractType:
        //     A type from which to derive the contract name that is used to export the type
        //     or member marked with this attribute, or null to use the default contract name.
        public ExportAttribute(Type contractType)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ExportAttribute
        //     class, exporting the type or member marked with this attribute under the specified
        //     contract name.
        //
        // Parameters:
        //   contractName:
        //     The contract name that is used to export the type or member marked with this
        //     attribute, or null or an empty string ("") to use the default contract name.
        public ExportAttribute(string contractName)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ExportAttribute
        //     class, exporting the specified type under the specified contract name.
        //
        // Parameters:
        //   contractName:
        //     The contract name that is used to export the type or member marked with this
        //     attribute, or null or an empty string ("") to use the default contract name.
        //
        //   contractType:
        //     The type to export.
        public ExportAttribute(string contractName, Type contractType)
        {

        }

        //
        // Summary:
        //     Gets the contract name that is used to export the type or member marked with
        //     this attribute.
        //
        // Returns:
        //     The contract name that is used to export the type or member marked with this
        //     attribute. The default value is an empty string ("").
        public string ContractName { get; private set; }
        //
        // Summary:
        //     Gets the contract type that is exported by the member that this attribute is
        //     attached to.
        //
        // Returns:
        //     The type of export that is be provided. The default value is null, which means
        //     that the type will be obtained by looking at the type on the member that this
        //     export is attached to.
        public Type ContractType { get; private set; }
    }
}
#endif