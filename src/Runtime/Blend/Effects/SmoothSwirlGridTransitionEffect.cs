
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
    /// Transition effect that swirls the current visual while introducing the new visual.
    /// </summary>
    public sealed class SmoothSwirlGridTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property that modifies the number of cells where pixels will be twisted.
        /// </summary>
        public static readonly DependencyProperty CellCountProperty =
            DependencyProperty.Register(
                nameof(CellCount),
                typeof(double),
                typeof(SmoothSwirlGridTransitionEffect),
                new PropertyMetadata(10.0));

        /// <summary>
        /// Dependency property that modifies the TwistAmount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty TwistAmountProperty =
            DependencyProperty.Register(
                nameof(TwistAmount),
                typeof(double),
                typeof(SmoothSwirlGridTransitionEffect),
                new PropertyMetadata(10.0));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public SmoothSwirlGridTransitionEffect() { }

        /// <summary>
        /// Creates an instance and sets the shader's twist variable to the specified values.
        /// </summary>
        /// <param name="twist">Level of swirl twist.</param>
        public SmoothSwirlGridTransitionEffect(double twist)
        {
            TwistAmount = twist;
        }

        /// <summary>
        /// Gets or sets the CellCount variable within the shader.
        /// </summary>
        public double CellCount
        {
            get => (double)GetValue(CellCountProperty);
            set => SetValue(CellCountProperty, value);
        }

        /// <summary>
        /// Gets or sets the TwistAmount variable within the shader.
        /// </summary>
        public double TwistAmount
        {
            get => (double)GetValue(TwistAmountProperty);
            set => SetValue(TwistAmountProperty, value);
        }
        
        /// <summary>
        /// Makes a deep copy of the SmoothSwirlGridTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the SmoothSwirlGridTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new SmoothSwirlGridTransitionEffect
            {
                TwistAmount = TwistAmount,
                CellCount = CellCount,
            };
        }
    }
}