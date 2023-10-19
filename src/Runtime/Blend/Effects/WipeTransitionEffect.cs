
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
    /// Define the wipe direction.
    /// </summary>
    public enum WipeDirection
    {
        LeftToRight = 0,
        RightToLeft = 1,
        TopToBottom = 2,
        BottomToTop = 3,
        TopLeftToBottomRight = 4,
        BottomRightToTopLeft = 5,
        BottomLeftToTopRight = 6,
        TopRightToBottomLeft = 7
    }

    /// <summary>
    /// Transition effect that wipes the current visual while introducing the new visual.
    /// </summary>
    public sealed class WipeTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property that modifies the FeatherAmount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty FeatherAmountProperty =
            DependencyProperty.Register(
                nameof(FeatherAmount),
                typeof(double),
                typeof(WipeTransitionEffect),
                new PropertyMetadata(0.2));

        /// <summary>
        /// Dependency property that modifies the WipeDirection variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty WipeDirectionProperty =
            DependencyProperty.Register(
                nameof(WipeDirection),
                typeof(WipeDirection),
                typeof(WipeTransitionEffect),
                new PropertyMetadata(WipeDirection.LeftToRight));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public WipeTransitionEffect() { }

        /// <summary>
        /// Gets or sets the FeatherAmount variable within the shader.
        /// </summary>
        public double FeatherAmount
        {
            get => (double)GetValue(FeatherAmountProperty);
            set => SetValue(FeatherAmountProperty, value);
        }

        /// <summary>
        /// Gets or sets the direction of the wipe.
        /// </summary>
        public WipeDirection WipeDirection
        {
            get => (WipeDirection)GetValue(WipeDirectionProperty);
            set => SetValue(WipeDirectionProperty, value);
        }

        /// <summary>
        /// Makes a deep copy of the WipeTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the WipeTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new WipeTransitionEffect
            {
                FeatherAmount = FeatherAmount,
                WipeDirection = WipeDirection,
            };
        }
    }
}
