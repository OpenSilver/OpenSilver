
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
    public sealed partial class TemplatePartAttribute : Attribute
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