
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

using System.Windows;

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Transition effect that reveals two visuals through a growing/shrinking circle.
    /// </summary>
    public sealed class CircleRevealTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property which modifies the feather amount variable within the pixel
        /// shader.
        /// </summary>
        public static readonly DependencyProperty FeatherAmountProperty =
            DependencyProperty.Register(
                nameof(FeatherAmount),
                typeof(double),
                typeof(CircleRevealTransitionEffect),
                new PropertyMetadata(0.2));

        /// <summary>
        /// Dependency property which modifies the circle movement.
        /// </summary>
        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.Register(
                nameof(Reverse),
                typeof(bool),
                typeof(CircleRevealTransitionEffect),
                new PropertyMetadata(false));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public CircleRevealTransitionEffect() { }

        /// <summary>
        /// Gets or sets the FuzzyAmount variable within the shader.
        /// </summary>
        public double FeatherAmount
        {
            get => (double)GetValue(FeatherAmountProperty);
            set => SetValue(FeatherAmountProperty, value);
        }

        /// <summary>
        /// Gets or sets playing the circle reveal backward.
        /// </summary>
        public bool Reverse
        {
            get => (bool)GetValue(ReverseProperty);
            set => SetValue(ReverseProperty, value);
        }

        /// <summary>
        /// Makes a deep copy of the CircleRevealTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the CircleRevealTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new CircleRevealTransitionEffect
            {
                FeatherAmount = FeatherAmount,
                Reverse = Reverse,
            };
        }
    }
}