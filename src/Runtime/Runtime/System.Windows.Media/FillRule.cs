
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

namespace System.Windows.Media
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