
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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