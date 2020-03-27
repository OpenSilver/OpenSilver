#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies that a property, field, or parameter value should be provided by the
    //     System.ComponentModel.Composition.Hosting.CompositionContainer.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public partial class ImportAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportAttribute
        //     class, importing the export with the default contract name.
        public ImportAttribute()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportAttribute
        //     class, importing the export with the contract name derived from the specified
        //     type.
        //
        // Parameters:
        //   contractType:
        //     The type to derive the contract name of the export from, or null to use the default
        //     contract name.
        public ImportAttribute(Type contractType)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportAttribute
        //     class, importing the export with the specified contract name.
        //
        // Parameters:
        //   contractName:
        //     The contract name of the export to import, or null or an empty string ("") to
        //     use the default contract name.
        public ImportAttribute(string contractName)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportAttribute
        //     class, importing the export with the specified contract name and type.
        //
        // Parameters:
        //   contractName:
        //     The contract name of the export to import, or null or an empty string ("") to
        //     use the default contract name.
        //
        //   contractType:
        //     The type of the export to import.
        public ImportAttribute(string contractName, Type contractType)
        {

        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether the property, field, or parameter
        //     will be set to its type's default value when an export with the contract name
        //     is not present in the container.
        //
        // Returns:
        //     true if the property, field, or parameter will be set to its type's default value
        //     when there is no export with the System.ComponentModel.Composition.ImportAttribute.ContractName
        //     in the System.ComponentModel.Composition.CompositionContainer; otherwise, false.
        //     The default is false.
        public bool AllowDefault { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether the property or field will be recomposed
        //     when exports with a matching contract have changed in the container.
        //
        // Returns:
        //     true if the property or field allows recomposition when exports with a matching
        //     System.ComponentModel.Composition.ImportAttribute.ContractName are added or removed
        //     from the System.ComponentModel.Composition.CompositionContainer; otherwise, false.
        //     The default is false.
        public bool AllowRecomposition { get; set; }
        //
        // Summary:
        //     Gets the contract name of the export to import.
        //
        // Returns:
        //     The contract name of the export to import. The default is an empty string ("").
        public string ContractName { get; private set; }
        //
        // Summary:
        //     Gets the type of the export to import.
        //
        // Returns:
        //     The type of the export to import.
        public Type ContractType { get; private set; }
        //
        // Summary:
        //     Gets or sets a value that indicates that the importer requires a specific System.ComponentModel.Composition.CreationPolicy
        //     for the exports used to satisfy this import.
        //
        // Returns:
        //     One of the following values:System.ComponentModel.Composition.CreationPolicy.Any,
        //     if the importer does not require a specific System.ComponentModel.Composition.CreationPolicy.
        //     This is the default.System.ComponentModel.Composition.CreationPolicy.Shared to
        //     require that all used exports be shared by all parts in the container.System.ComponentModel.Composition.CreationPolicy.NonShared
        //     to require that all used exports be non-shared in a container. In this case,
        //     each part receives their own instance.
        public CreationPolicy RequiredCreationPolicy { get; set; }
    }
}
#endif