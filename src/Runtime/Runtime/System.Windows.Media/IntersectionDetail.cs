
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

namespace System.Windows.Media;

/// <summary>
/// Provides detailed information on the nature of a geometry intersection operation. The result is based 
/// on the intersection of the hit geometry and the target geometry (or visual).
/// </summary>
public enum IntersectionDetail
{
    /// <summary>
    /// The <see cref="IntersectionDetail"/> value is not calculated.
    /// </summary>
    NotCalculated = 0,

    /// <summary>
    /// The <see cref="Geometry"/> hit test parameter and the target visual, or geometry, do not intersect.
    /// </summary>
    Empty = 1,

    /// <summary>
    /// The target visual, or geometry, is fully inside the <see cref="Geometry"/> hit test parameter.
    /// </summary>
    FullyInside = 2,

    /// <summary>
    /// The <see cref="Geometry"/> hit test parameter is fully contained within the boundary of the target 
    /// visual or geometry.
    /// </summary>
    FullyContains = 3,

    /// <summary>
    /// The <see cref="Geometry"/> hit test parameter and the target visual, or geometry, intersect. This 
    /// means that the two elements overlap, but neither element contains the other.
    /// </summary>
    Intersects = 4,
}
