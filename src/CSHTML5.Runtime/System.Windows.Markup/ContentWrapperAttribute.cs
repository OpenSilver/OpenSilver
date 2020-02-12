
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
using System.Runtime.CompilerServices;

namespace System.Windows.Markup
{
    /// <summary>
    /// Specifies one or more types on the associated collection type that will be
    /// used to wrap foreign content.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed partial class ContentWrapperAttribute : Attribute
    {
        Type _contentWrapper;

        /// <summary>
        /// Initializes a new instance of the ContentWrapperAttribute
        /// class.
        /// </summary>
        /// <param name="contentWrapper">The System.Type that is declared as a content wrapper for the collection
        /// type.</param>
        public ContentWrapperAttribute(Type contentWrapper)
        {
            this._contentWrapper = contentWrapper;

        }
        /// <summary>
        /// Gets the type that is declared as a content wrapper for the collection type
        /// associated with this attribute.
        /// </summary>
        public Type ContentWrapper
        {
            get
            {
                return this._contentWrapper;
            }
        }

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
#if !BRIDGE
        public override object TypeId
#else
        public object TypeId
#endif
        {
            get
            {
                return (object)this;
            }
        }

        /// <summary>
        /// Determines whether the specified ContentWrapperAttribute
        /// is equivalent this ContentWrapperAttribute by comparing
        /// the ContentWrapperAttribute.ContentWrapper properties.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ContentWrapperAttribute wrapperAttribute = obj as ContentWrapperAttribute;
            if (wrapperAttribute == null)
                return false;
            else
                return this._contentWrapper == wrapperAttribute._contentWrapper;
        }

        /// <summary>
        /// Gets a hash code for this instance.
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode()
        {
            return this._contentWrapper.GetHashCode();
        }
    }
}
