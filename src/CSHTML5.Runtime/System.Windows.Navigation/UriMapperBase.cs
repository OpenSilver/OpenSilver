

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
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Represents the base class for classes that convert a requested uniform resource
    /// identifier (URI) into a new URI based on mapping rules.
    /// </summary>
    public abstract partial class UriMapperBase
    {
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Navigation.UriMapperBase
        ///// class.
        ///// </summary>
        //protected UriMapperBase();

        /// <summary>
        /// When overridden in a derived class, converts a requested uniform resource
        /// identifier (URI) to a new URI.</summary>
        /// <param name="uri">The original URI value to be mapped to a new URI.</param>
        /// <returns>A URI to use for the request instead of the value in the uri parameter.</returns>
        public abstract Uri MapUri(Uri uri);
    }
}