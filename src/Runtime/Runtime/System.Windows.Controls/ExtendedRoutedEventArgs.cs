
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

namespace Microsoft.Windows
{
    public abstract class ExtendedRoutedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the present state of the 
        /// event handling for a routed event as it travels the route.
        /// </summary>
        public bool Handled { get; set; }

        public object OriginalSource { get; }
    }
}