

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
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Core
#endif
{
    /// <summary>
    /// Contains the argument returned by a window size change event.
    /// </summary>
    public sealed partial class WindowSizeChangedEventArgs
    {
        /// <summary>
        /// Gets or sets whether the window size event was handled.
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the new size of the window.
        /// </summary>
        public Size Size { get; internal set; }
    }
}