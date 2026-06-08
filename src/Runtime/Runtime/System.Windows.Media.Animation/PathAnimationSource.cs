
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Specifies the output property value of the path that is used to drive the animation.
/// </summary>
public enum PathAnimationSource : byte
{
    /// <summary>
    /// Specifies the x-coordinate offset during the progression along an animation sequence path.
    /// </summary>
    X = 0,

    /// <summary>
    /// Specifies the y-coordinate offset during the progression along an animation sequence path.
    /// </summary>
    Y = 1,

    /// <summary>
    /// Specifies the tangent angle of rotation during the progression along an animation sequence path.
    /// </summary>
    Angle = 2,
}
