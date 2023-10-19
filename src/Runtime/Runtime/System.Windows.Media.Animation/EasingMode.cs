
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Specifies how the animation associated with an easing function interpolates.
    /// </summary>
    public enum EasingMode
    {
        /// <summary>
        /// Interpolation follows 100% interpolation minus the output of the formula
        /// associated with the easing function.
        /// </summary>
        EaseOut = 0,
        /// <summary>
        /// Interpolation follows the mathematical formula associated with the easing
        /// function.
        /// </summary>
        EaseIn = 1,
        /// <summary>
        /// Interpolation uses EaseIn for the first half of the animation and EaseOut
        /// for the second half.
        /// </summary>
        EaseInOut = 2,
    }
}