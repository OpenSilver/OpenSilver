
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
        /// Initializes a new instance of the <see cref="ContentPropertyAttribute"/>
        /// class.
        /// </summary>
        public ContentPropertyAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPropertyAttribute"/>
        /// class, using the specified content property name.
        /// </summary>
        /// <param name="name">
        /// The content property name.
        /// </param>
        public ContentPropertyAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the property that is the declared content property.
        /// </summary>
        public string Name { get; set; }
    }
}