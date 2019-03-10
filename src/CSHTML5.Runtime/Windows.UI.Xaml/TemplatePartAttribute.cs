
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Style authors should be able to identify the part type used for styling the specific class.
    /// The part is usually required in the style and should have a specific predefined name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TemplatePartAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TemplatePartAttribute()
        {
        }
 
        /// <summary>
        /// Part name used by the class to indentify required element in the style
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
 
        /// <summary>
        /// Type of the element that should be used as a part with name specified in TemplatePartAttribute.Name
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
 
        private string _name;
        private Type _type;
    }
}