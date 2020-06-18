#if WORKINPROGRESS
namespace System.Windows.Markup
{
    //
    // Summary:
    //     Specifies a recommended prefix to associate with an XML namespace when writing
    //     elements and attributes in a XAML file.
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed partial class XmlnsPrefixAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Markup.XmlnsPrefixAttribute
        //     class.
        //
        // Parameters:
        //   xmlNamespace:
        //     The identifier of the relevant XML namespace (use a string, not a true System.Uri
        //     type).
        //
        //   prefix:
        //     The recommended prefix to use when mapping the XML namespace.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     xmlNamespace or prefix are null.
        public XmlnsPrefixAttribute(string xmlNamespace, string prefix)
        {

        }

        //
        // Summary:
        //     Gets the recommended prefix to associate with the XML namespace.
        //
        // Returns:
        //     A string that provides the recommended prefix to use when mapping the System.Windows.Markup.XmlnsPrefixAttribute.XmlNamespace
        //     namespace.
        public string Prefix { get; private set; }
        //
        // Summary:
        //     Gets the XML namespace identifier.
        //
        // Returns:
        //     A string that provides the identifier for the relevant XML namespace. By convention,
        //     this is often a URI string.
        public string XmlNamespace { get; private set; }
    }
}

#endif