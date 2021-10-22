
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
    /// Specifies a mapping on a per-assembly basis between a XAML namespace and
    /// a CLR namespace, which is then used for type resolution by a XAML object
    /// writer or XAML schema context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class XmlnsDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlnsDefinitionAttribute"/>
        /// class.
        /// </summary>
        /// <param name="xmlNamespace">
        /// The XAML namespace identifier.
        /// </param>
        /// <param name="clrNamespace">
        /// A string that references a CLR namespace name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// xmlNamespace or clrNamespace are null.
        /// </exception>
        public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
        {
            if (xmlNamespace == null)
            {
                throw new ArgumentNullException("xmlNamespace");
            }
            if (clrNamespace == null)
            {
                throw new ArgumentNullException("clrNamespace");
            }

            XmlNamespace = xmlNamespace;
            ClrNamespace = clrNamespace;
        }

        /// <summary>
        /// Gets or sets the name of the assembly associated with the attribute.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets the XAML namespace identifier specified in this attribute.
        /// </summary>
        public string XmlNamespace { get; set; }

        /// <summary>
        /// Gets the string name of the CLR namespace specified in this attribute
        /// </summary>
        public string ClrNamespace { get; set; }
    }
}
