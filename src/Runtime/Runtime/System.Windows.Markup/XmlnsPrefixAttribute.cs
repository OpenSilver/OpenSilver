
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace System.Windows.Markup
{
    /// <summary>
    /// Specifies a recommended prefix to associate with an XML namespace when writing
    /// elements and attributes in a XAML file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class XmlnsPrefixAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlnsPrefixAttribute"/>
        /// class.
        /// </summary>
        /// <param name="xmlNamespace">
        /// The identifier of the relevant XML namespace (use a string, not a true <see cref="Uri"/>
        /// type).
        /// </param>
        /// <param name="prefix">
        /// The recommended prefix to use when mapping the XML namespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// xmlNamespace or prefix are null.
        /// </exception>
        public XmlnsPrefixAttribute(string xmlNamespace, string prefix)
        {
            this.XmlNamespace = xmlNamespace ?? throw new ArgumentNullException(nameof(xmlNamespace));
            this.Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        /// <summary>
        /// Gets the recommended prefix to associate with the XML namespace.
        /// </summary>
        /// <returns>
        /// A string that provides the recommended prefix to use when mapping the <see cref="XmlnsPrefixAttribute.XmlNamespace"/>
        /// namespace.
        /// </returns>
        public string Prefix { get; }

        /// <summary>
        /// Gets the XML namespace identifier.
        /// </summary>
        /// <returns>
        /// A string that provides the identifier for the relevant XML namespace. By convention,
        /// this is often a URI string.
        /// </returns>
        public string XmlNamespace { get; }
    }
}
