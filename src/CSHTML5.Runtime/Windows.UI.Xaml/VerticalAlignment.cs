
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