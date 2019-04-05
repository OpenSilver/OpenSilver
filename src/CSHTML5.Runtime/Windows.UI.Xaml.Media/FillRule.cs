
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
    /// Specifies how the intersecting areas of PathFigure objects contained in a
    /// Geometry are combined to form the area of the Geometry.
    /// </summary>
    public enum FillRule
    {
        /// <summary>
        /// Rule that determines whether a point is in the fill region by drawing a ray
        /// from that point to infinity in any direction and counting the number of path
        /// segments within the given shape that the ray crosses. If this number is odd,
        /// the point is inside; if even, the point is outside.
        /// </summary>
        EvenOdd = 0,

        /// <summary>
        /// Rule that determines whether a point is in the fill region of the path by
        /// drawing a ray from that point to infinity in any direction and then examining
        /// the places where a segment of the shape crosses the ray. Starting with a
        /// count of zero, add one each time a segment crosses the ray from left to right
        /// and subtract one each time a path segment crosses the ray from right to left.
        /// After counting the crossings, if the result is zero then the point is outside
        /// the path. Otherwise, it is inside.
        /// </summary>
        Nonzero = 1,
    }
}