
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
    /// Specifies one or more types on the associated collection type that will be
    /// used to wrap foreign content.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ContentWrapperAttribute : Attribute
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
        public override object TypeId
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
