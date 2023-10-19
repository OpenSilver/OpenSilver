
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

namespace System.Windows
{
    /// <summary>
    /// Describes how a child element is vertically positioned or stretched within
    /// a parent's layout slot.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// The element is aligned to the top of the parent's layout slot.
        /// </summary>
        Top = 0,

        /// <summary>
        /// The element is aligned to the center of the parent's layout slot.
        /// </summary>
        Center = 1,
        
        /// <summary>
        /// The element is aligned to the bottom of the parent's layout slot.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// The element is stretched to fill the entire layout slot of the parent element.
        /// </summary>
        Stretch = 3,
    }
}