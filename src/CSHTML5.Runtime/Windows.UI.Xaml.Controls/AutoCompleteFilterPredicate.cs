
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
