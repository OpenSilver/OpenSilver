
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Specifies the method the <see cref="VirtualizingStackPanel"/> uses
    /// to manage virtualizing its child items.
    /// </summary>
    public enum VirtualizationMode
    {
        /// <summary>
        /// Create and discard the item containers.
        /// </summary>
        Standard = 0,
        /// <summary>
        /// Reuse the item containers.
        /// </summary>
        Recycling = 1,
    }
}