

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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    /// <summary>
    /// This property determines how text is trimmed when it overflows the edge of its
    /// container.
    /// </summary>
    public enum TextTrimming
    {
        /// <summary>
        /// Default no trimming
        /// </summary>
        None = 0,

        /// <summary>
        /// Text is trimmed at a character boundary.
        /// </summary>
        CharacterEllipsis = 1,

        /// <summary>
        /// Text is trimmed at word boundary.
        /// </summary>
        WordEllipsis = 2,

        /// <summary>
        /// Text is trimmed at a pixel level, visually clipping the excess glyphs.
        /// </summary>
        Clip = 3,
    }
#endif
}
