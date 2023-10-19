
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
    /// Defines the slide orientation.
    /// </summary>
    public enum SlideDirection
    {
        LeftToRight = 0,
        RightToLeft = 1,
        TopToBottom = 2,
        BottomToTop = 3
    }

    /// <summary>
    /// Transition effect that slides the current visual away, revealing the new visual.
    /// </summary>
    public sealed class SlideInTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property that modifies the SlideAmount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty SlideDirectionProperty =
            DependencyProperty.Register(
                nameof(SlideDirection),
                typeof(SlideDirection),
                typeof(SlideInTransitionEffect),
                new PropertyMetadata(SlideDirection.LeftToRight));

        /// <summary>
        /// Dependency property that modifies the SlideAmount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty SlideNormalProperty =
            DependencyProperty.Register(
                "SlideNormal",
                typeof(Point),
                typeof(SlideInTransitionEffect),
                new PropertyMetadata(new Point(-1.0, 0.0)));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public SlideInTransitionEffect() { }

        /// <summary>
        /// Gets or sets the SlideAmount variable within the shader.
        /// </summary>
        public SlideDirection SlideDirection
        {
            get => (SlideDirection)GetValue(SlideDirectionProperty);
            set => SetValue(SlideDirectionProperty, value);
        }

        /// <summary>
        /// Makes a deep copy of the SlideInTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the SlideInTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new SlideInTransitionEffect
            {
                SlideDirection = SlideDirection,
            };
        }
    }
}