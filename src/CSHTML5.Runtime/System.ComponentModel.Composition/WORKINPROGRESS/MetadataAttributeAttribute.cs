#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies that a custom attribute’s properties provide metadata for exports applied
    //     to the same type, property, field, or method.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed partial class MetadataAttributeAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.MetadataAttributeAttribute
        //     class.
        public MetadataAttributeAttribute()
        {

        }
    }
}
#endif