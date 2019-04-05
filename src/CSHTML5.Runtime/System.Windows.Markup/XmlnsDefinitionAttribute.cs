
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        // Exceptions:
        //   System.ArgumentNullException:
        //     xmlNamespace or clrNamespace are null.
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XmlnsDefinitionAttribute
        /// class.
        /// </summary>
        /// <param name="xmlNamespace">The XAML namespace identifier.</param>
        /// <param name="clrNamespace">A string that references a CLR namespace name.</param>
        public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
        {
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
