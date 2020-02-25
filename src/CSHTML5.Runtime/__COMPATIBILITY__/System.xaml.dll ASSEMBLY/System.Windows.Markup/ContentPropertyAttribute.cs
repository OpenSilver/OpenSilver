
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
using System.Runtime.CompilerServices;

namespace System.Windows.Markup
{
    /// <summary>
    /// Indicates which property of a type is the XAML content property. A XAML processor
    /// uses this information when processing XAML child elements of XAML representations
    /// of the attributed type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ContentPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ContentPropertyAttribute class.
        /// </summary>
        public ContentPropertyAttribute() { }

        /// <summary>
        /// Initializes a new instance of the ContentPropertyAttribute class with the specified name.
        /// </summary>
        /// <param name="name">The name of the ContentPropertyAttribute</param>
        public ContentPropertyAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}