
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
    /// Determine the orientation of the blinds.
    /// </summary>
    public enum BlindOrientation
    {
        Vertical = 0,
        Horizontal = 1
    }

    /// <summary>
    /// Transition shader that simulates blinds opening when transitioning from one visual
    /// to another.
    /// </summary>
    public sealed class BlindsTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Gets or sets the number of blinds.
        /// </summary>
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register(
                nameof(Count),
                typeof(double),
                typeof(BlindsTransitionEffect),
                new PropertyMetadata(5.0));

        /// <summary>
        /// Gets or sets the orientation of the blinds.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(BlindOrientation),
                typeof(BlindsTransitionEffect),
                new PropertyMetadata(BlindOrientation.Horizontal));

        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public BlindsTransitionEffect() { }

        /// <summary>
        /// Gets or sets the number of blinds to display.
        /// </summary>
        public double Count
        {
            get => (double)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        /// <summary>
        /// Gets or sets the orientation of the blinds.
        /// </summary>
        public BlindOrientation Orientation
        {
            get => (BlindOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Makes a deep copy of the BlindsTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the BlindsTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new BlindsTransitionEffect
            {
                Orientation = Orientation,
                Count = Count,
            };
        }
    }
}