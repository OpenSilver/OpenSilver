
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the shape at the end of a line or segment.
    /// </summary>
    public enum PenLineCap
    {
        /// <summary>
        /// A cap that does not extend past the last point of the line. Comparable to
        /// no line cap.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// A rectangle that has a height equal to the line thickness and a length equal
        /// to half the line thickness.
        /// </summary>
        Square = 1,
        /// <summary>
        /// A semicircle that has a diameter equal to the line thickness.
        /// </summary>
        Round = 2,
#if WORKINPROGRESS
        /// <summary>
        ///     Triangle - Triangle line cap.
        /// </summary>
        Triangle = 3,
#endif
    }
}