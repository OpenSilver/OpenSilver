
//-----------------------------------------------------------------------------
//  CONFIDENTIALITY NOTICE:
//  This code is the sole property of Userware and is strictly confidential.
//  Unless you have a written agreement in effect with Userware that states
//  otherwise, you are not authorized to view, copy, modify, or compile this
//  source code, and you should destroy all the copies that you possess.
//  Any redistribution in source code form is strictly prohibited.
//  Redistribution in binary form is allowed provided that you have obtained
//  prior written consent from Userware. You can contact Userware at:
//  contact@userware-solutions.com - Copyright (c) 2016 Userware
//-----------------------------------------------------------------------------


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
