
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
