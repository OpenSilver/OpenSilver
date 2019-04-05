
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



#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
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