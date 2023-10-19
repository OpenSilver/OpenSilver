
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
    public interface IEasingFunction
    {
        /// <summary>Transforms normalized time to control the pace of an animation.</summary>
        /// <returns>The transformed progress.</returns>
        /// <param name="normalizedTime">Normalized time (progress) of the animation.</param>
        double Ease(double normalizedTime);
    }
}
