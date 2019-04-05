
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
    /// Describes how content is resized to fill its allocated space.
    /// </summary>
    public enum Stretch
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        None = 0,

        /// <summary>
        /// The content is resized to fill the destination dimensions. The aspect ratio is not preserved.
        /// </summary>
        Fill = 1,

        /// <summary>
        /// The content is resized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        Uniform = 2,

        /// <summary>
        /// The content is resized to fill the destination dimensions while it preserves
        /// its native aspect ratio. If the aspect ratio of the destination rectangle
        /// differs from the source, the source content is clipped to fit in the destination
        /// dimensions.
        /// </summary>
        UniformToFill = 3,
    }
}