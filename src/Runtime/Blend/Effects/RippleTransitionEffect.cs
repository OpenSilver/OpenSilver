
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
    /// Transition effect that simulates water ripple during transition.
    /// </summary>
    public sealed class RippleTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public RippleTransitionEffect() { }

        /// <summary>
        /// Makes a deep copy of the RippleTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the RippleTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new RippleTransitionEffect();
        }
    }
}