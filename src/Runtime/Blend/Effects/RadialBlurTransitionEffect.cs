
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

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Transition effect that performs a radial blur of the current visual as the new
    /// visual is introduced.
    /// </summary>
    public class RadialBlurTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public RadialBlurTransitionEffect() { }

        /// <summary>
        /// Makes a deep copy of the RadialBlurTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the RadialBlurTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new RadialBlurTransitionEffect();
        }
    }
}