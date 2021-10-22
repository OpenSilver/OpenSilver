

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// Represents the style of a font face (for example, normal or italic).
    /// </summary>
#if MIGRATION
    public static class FontStyles
    {
        public static FontStyle Normal
        {
            get { return new FontStyle(0); }
        }

        public static FontStyle Oblique
        {
            get { return new FontStyle(1); }
        }

        public static FontStyle Italic
        {
            get { return new FontStyle(2); }
        }
    }
#else
    public enum FontStyle
    {
        /// <summary>
        /// Represents a normal font style.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Represents an oblique font style.
        /// </summary>
        Oblique = 1,
        /// <summary>
        /// Represents an italic font style.
        /// </summary>
        Italic = 2,
    }
#endif
}