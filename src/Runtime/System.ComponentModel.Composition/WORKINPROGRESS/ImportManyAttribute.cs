#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies that a property, field, or parameter should be populated with all matching
    //     exports by the System.ComponentModel.Composition.Hosting.CompositionContainer.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public partial class ImportManyAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportManyAttribute
        //     class, importing the set of exports with the default contract name.
        public ImportManyAttribute()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportManyAttribute
        //     class, importing the set of exports with the contract name derived from the specified
        //     type.
        //
        // Parameters:
        //   contractType:
        //     The type to derive the contract name of the exports to import, or null to use
        //     the default contract name.
        public ImportManyAttribute(Type contractType)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportManyAttribute
        //     class, importing the set of exports with the specified contract name.
        //
        // Parameters:
        //   contractName:
        //     The contract name of the exports to import, or null or an empty string ("") to
        //     use the default contract name.
        public ImportManyAttribute(string contractName)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.ImportManyAttribute
        //     class, importing the set of exports with the specified contract name and contract
        //     type.
        //
        // Parameters:
        //   contractName:
        //     The contract name of the exports to import, or null or an empty string ("") to
        //     use the default contract name.
        //
        //   contractType:
        //     The type of the export to import.
        public ImportManyAttribute(string contractName, Type contractType)
        {

        }

        //
        // Summary:
        //     Gets or sets a value indicating whether the decorated property or field will
        //     be recomposed when exports that provide the matching contract change.
        //
        // Returns:
        //     true if the property or field allows for recomposition when exports that provide
        //     the same System.ComponentModel.Composition.ImportManyAttribute.ContractName are
        //     added or removed from the System.ComponentModel.Composition.Hosting.CompositionContainer;
        //     otherwise, false.The default value is false.
        public bool AllowRecomposition { get; set; }
        //
        // Summary:
        //     Gets the contract name of the exports to import.
        //
        // Returns:
        //     The contract name of the exports to import. The default value is an empty string
        //     ("").
        public string ContractName { get; private set; }
        //
        // Summary:
        //     Gets the contract type of the export to import.
        //
        // Returns:
        //     The type of the export that this import is expecting. The default value is null,
        //     which means that the type will be obtained by looking at the type on the member
        //     that this import is attached to. If the type is System.Object, the import will
        //     match any exported type.
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