
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

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Transition effect that transitions two visuals using a cloud texture as the sampler
    /// threshold.
    /// </summary>
    public sealed class CloudRevealTransitionEffect : CloudyTransitionEffect
    {
        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public CloudRevealTransitionEffect() { }

        /// <summary>
        /// Makes a deep copy of the CloudRevealTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the CloudRevealTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new CloudRevealTransitionEffect
            {
                RandomSeed = RandomSeed,
            };
        }
    }

    /// <summary>
    /// Defines a transition shader effect that uses an image as a sampler threshold
    /// for interpolating pixel value between two visuals.
    /// </summary>
    public abstract class CloudyTransitionEffect : RandomizedTransitionEffect
    {
        /// <summary>
        /// Dependency property which modifies the CloudImage variable within the pixel shader.
        /// </summary>
        protected static readonly DependencyProperty CloudImageProperty =
            DependencyProperty.Register(
                nameof(CloudImage),
                typeof(Brush),
                typeof(CloudyTransitionEffect),
                new PropertyMetadata((object)null));

        internal CloudyTransitionEffect() { }

        /// <summary>
        /// Gets or sets the CloudImage variable within the shader used for sampling.
        /// </summary>
        protected Brush CloudImage
        {
            get => (Brush)GetValue(CloudImageProperty);
            set => SetValue(CloudImageProperty, value);
        }
    }

    /// <summary>
    /// Defines a transition shader effect that provides a random value, allowing the
    /// effect to provide variance each time the effect is run.
    /// </summary>
    public abstract class RandomizedTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property which modifies the RandomSeed variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty RandomSeedProperty =
            DependencyProperty.Register(
                nameof(RandomSeed),
                typeof(double),
                typeof(RandomizedTransitionEffect),
                new PropertyMetadata(0.0));

        internal RandomizedTransitionEffect() { }

        /// <summary>
        /// Gets or sets the RandomSeed variable within the shader.
        /// </summary>
        public double RandomSeed
        {
            get => (double)GetValue(RandomSeedProperty);
            set => SetValue(RandomSeedProperty, value);
        }
    }
}