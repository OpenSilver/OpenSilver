
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
    /// Represents the style of a font face (for example, normal or italic).
    /// </summary>
    public static class FontStyles
    {
        /// <summary>
        /// Specifies a normal, or roman, font style.
        /// </summary>
        /// <returns>
        /// A font style that represents a normal, or roman, font style.
        /// </returns>
        public static FontStyle Normal => new FontStyle(0);

        /// <summary>
        /// Specifies an oblique font style.
        /// </summary>
        /// <returns>
        /// A font style that represents an oblique font style.
        /// </returns>
        public static FontStyle Oblique => new FontStyle(1);

        /// <summary>
        /// Specifies an italic font style.
        /// </summary>
        /// <returns>
        /// A font style that represents an italic font style.
        /// </returns>
        public static FontStyle Italic => new FontStyle(2);
    }
}