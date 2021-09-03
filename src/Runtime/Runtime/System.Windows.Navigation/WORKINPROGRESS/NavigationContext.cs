

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

using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Represents the state of a navigation operation.
    [OpenSilver.NotImplemented]
    public sealed class NavigationContext
    {
        //
        // Summary:
        //     Gets a collection of query string values.
        //
        // Returns:
        //     A collection that contains the query string values.
        [OpenSilver.NotImplemented]
        public IDictionary<string, string> QueryString { get; }
    }
}
