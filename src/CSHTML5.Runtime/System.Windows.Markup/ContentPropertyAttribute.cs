
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