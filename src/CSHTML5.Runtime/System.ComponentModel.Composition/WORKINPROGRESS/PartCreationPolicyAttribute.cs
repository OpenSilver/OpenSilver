#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies the System.ComponentModel.Composition.PartCreationPolicyAttribute.CreationPolicy
    //     for a part.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed partial class PartCreationPolicyAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.PartCreationPolicyAttribute
        //     class with the specified creation policy.
        //
        // Parameters:
        //   creationPolicy:
        //     The creation policy to use.
        public PartCreationPolicyAttribute(CreationPolicy creationPolicy)
        {

        }

        //
        // Summary:
        //     Gets or sets a value that indicates the creation policy of the attributed part.
        //
        // Returns:
        //     One of the System.ComponentModel.Composition.PartCreationPolicyAttribute.CreationPolicy
        //     values that indicates the creation policy of the attributed part. The default
        //     is System.ComponentModel.Composition.CreationPolicy.Any.
        public CreationPolicy CreationPolicy { get; private set; }
    }
}
#endif