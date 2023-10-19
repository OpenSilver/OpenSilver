
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
    /// Specifies whether text is centered, left-aligned, or right-aligned.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// Text is centered within the container.
        /// </summary>
        Center = 0,
        
        /// <summary>
        /// Text is aligned to the left edge of the container.
        /// </summary>
        Left = 1,
        
        /// <summary>
        /// Text is aligned to the right edge of the container.
        /// </summary>
        Right = 2,
        
        /// <summary>
        /// Text is justified within the container.
        /// </summary>
        Justify = 3,
    }
}