
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

namespace System.Windows
{
    /// <summary>
    /// Describes how text is trimmed when it overflows the edge of its containing box.
    /// </summary>
    public enum TextTrimming
    {
        /// <summary>
        /// Text is not trimmed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Text is trimmed at character boundary. An ellipsis (...) is drawn in place of remaining
        /// text.
        /// </summary>
        CharacterEllipsis = 1,

        /// <summary>
        /// Text is trimmed at a word boundary. An ellipsis (...) is drawn in place of remaining
        /// text.
        /// </summary>
        WordEllipsis = 2,
    }
}
