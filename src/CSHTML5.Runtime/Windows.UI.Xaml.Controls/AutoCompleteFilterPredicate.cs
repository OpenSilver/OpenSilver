
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the filter used by the AutoCompleteBox
    /// control to determine whether an item is a possible match for the specified
    /// text.
    /// </summary>
    /// <typeparam name="T">The type used for filtering the AutoCompleteBox.
    /// This type can be either a string or an object.</typeparam>
    /// <param name="search">The string used as the basis for filtering.</param>
    /// <param name="item">The item that is compared with the search parameter.</param>
    /// <returns>true to indicate item is a possible match for search; otherwise false.</returns>
    public delegate bool AutoCompleteFilterPredicate<T>(string search, T item);
}
