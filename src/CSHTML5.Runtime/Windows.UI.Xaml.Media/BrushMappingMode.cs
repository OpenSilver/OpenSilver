
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Specifies the coordinate system used by a System.Windows.Media.Brush.
    /// </summary>
    public enum BrushMappingMode
    {
        /// <summary>
        /// The coordinate system is not relative to a bounding box. Values are interpreted
        /// directly in local space.
        /// </summary>
        Absolute = 0,
       
        /// <summary>
        /// The coordinate system is relative to a bounding box: 0 indicates 0 percent
        /// of the bounding box, and 1 indicates 100 percent of the bounding box. For
        /// example, (0.5, 0.5) describes a point in the middle of the bounding box,
        /// and (1, 1) describes a point at the bottom right of the bounding box.
        /// </summary>
        RelativeToBoundingBox = 1,
    }
}
