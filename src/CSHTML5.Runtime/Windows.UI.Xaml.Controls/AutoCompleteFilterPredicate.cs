

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
