
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
    /// Transition effect that increases or decreases pixelation between two visuals.
    /// </summary>
    public sealed class PixelateTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public PixelateTransitionEffect() { }

        /// <summary>
        /// Makes a deep copy of the PixelateTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the PixelateTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new PixelateTransitionEffect();
        }
    }
}