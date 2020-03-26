

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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines a method for internal navigation in an application.
    /// </summary>
    public partial interface INavigate
    {
        /// <summary>
        /// Displays the content located at the specified URI.
        /// </summary>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>true if the content was successfully displayed; otherwise, false.</returns>
        bool Navigate(Uri source);
    }
}
