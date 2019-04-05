
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


#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Specifies the effects of a drag-and-drop operation.
    /// </summary>
    [Flags]
    public enum DragDropEffects
    {
#if unsupported
        /// <summary>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = -2147483648,

        /// <summary>
        /// The data is copied, removed from the drag source, and scrolled in the drop
        /// target.
        /// </summary>
        All = -2147483645,
#endif
        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0,
#if unsupported
        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 4,
#endif
    }
}