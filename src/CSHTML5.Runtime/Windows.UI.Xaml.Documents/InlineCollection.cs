
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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Represents a collection of Inline elements.
    /// </summary>
    [ContentWrapper(typeof(Run))] // Note: this attribute prevents the XAML Designer error that says that it is not possible to put a String inside an InlineCollection.
    //[ContentWrapper(typeof(InlineUIContainer))]
    public sealed class InlineCollection : List<Inline> //IEnumerable, IList
    {
        /// <summary>
        /// Adds a string to the collection.
        /// </summary>
        /// <param name="text">The text to add.</param>
        public void Add(string text)
        {
            base.Add(new Run() { Text = text });
        }
    }
}
