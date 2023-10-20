
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
    /// Specifies one or more types on the associated collection type that will be
    /// used to wrap foreign content.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ContentWrapperAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWrapperAttribute"/>
        /// class.
        /// </summary>
        /// <param name="contentWrapper">
        /// The <see cref="Type"/> that is declared as a content wrapper for the 
        /// collection type.
        /// </param>
        public ContentWrapperAttribute(Type contentWrapper)
        {
            this.ContentWrapper = contentWrapper;
        }

        /// <summary>
        /// Gets the type that is declared as a content wrapper for the collection type
        /// associated with this attribute.
        /// </summary>
        public Type ContentWrapper { get; }

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId
        {
            get { return this; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ContentWrapperAttribute"/>
        /// is equivalent this <see cref="ContentWrapperAttribute"/> by comparing
        /// the <see cref="ContentWrapperAttribute.ContentWrapper"/> properties.
        /// </summary>
        public override bool Equals(object obj)
        {
            ContentWrapperAttribute wrapperAttribute = obj as ContentWrapperAttribute;
            if (wrapperAttribute == null)
                return false;
            else
                return this.ContentWrapper == wrapperAttribute.ContentWrapper;
        }

        /// <summary>
        /// Gets a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ContentWrapper.GetHashCode();
        }
    }
}
