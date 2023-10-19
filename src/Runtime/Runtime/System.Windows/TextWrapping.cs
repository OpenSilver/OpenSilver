
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
    /// Specifies whether text wraps when it reaches the edge of its container.
    /// </summary>
    public enum TextWrapping
    {
        /// <summary>
        /// No line wrapping is performed.
        /// </summary>
        NoWrap = 1,
             
        /// <summary>
        /// Line breaking occurs if a line of text overflows beyond the available width
        /// of its container. Line breaking occurs even if the standard line-breaking
        /// algorithm cannot determine any line break opportunity, such as when a line
        /// of text includes a long word that is constrained by a fixed-width container
        /// without scrolling.
        /// </summary>
        Wrap = 2,
    }
}