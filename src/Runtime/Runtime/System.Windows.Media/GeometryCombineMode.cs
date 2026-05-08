
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
/// Specifies the different methods by which two geometries can be combined.
/// </summary>
public enum GeometryCombineMode
{
    /// <summary>
    /// The two regions are combined by taking the union of both. The resulting geometry is geometry A + geometry B.
    /// </summary>
    Union = 0,

    /// <summary>
    /// The two regions are combined by taking their intersection. The new area consists of the overlapping region 
    /// between the two geometries.
    /// </summary>
    Intersect = 1,

    /// <summary>
    /// The two regions are combined by taking the area that exists in the first region but not the second and the 
    /// area that exists in the second region but not the first. The new region consists of (A-B) + (B-A), where A 
    /// and B are geometries.
    /// </summary>
    Xor = 2,

    /// <summary>
    /// The second region is excluded from the first. Given two geometries, A and B, the area of geometry B is removed 
    /// from the area of geometry A, producing a region that is A-B.
    /// </summary>
    Exclude = 3,
}