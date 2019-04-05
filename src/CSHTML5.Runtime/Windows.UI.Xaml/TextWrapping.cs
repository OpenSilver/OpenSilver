
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