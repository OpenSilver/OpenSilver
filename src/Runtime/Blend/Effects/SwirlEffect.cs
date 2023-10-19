
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

using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Effect that swirls the current visual.
    /// </summary>
    public sealed class SwirlEffect : ShaderEffect
    {
        /// <summary>
        /// This property is mapped to the AngleFrequency variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty AngleFrequencyProperty =
            DependencyProperty.Register(
                "AngleFrequency",
                typeof(Point),
                typeof(SwirlEffect),
                new PropertyMetadata(new Point(1.0, 1.0)));

        /// <summary>
        /// This property is mapped to the Center variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(SwirlEffect),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// The explicit input for this pixel shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(SwirlEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// This property is mapped to the TwistAmount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty TwistAmountProperty =
            DependencyProperty.Register(
                nameof(TwistAmount),
                typeof(double),
                typeof(SwirlEffect),
                new PropertyMetadata(10.0));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public SwirlEffect() { }

        /// <summary>
        /// Gets or sets the Center variable within the shader.
        /// </summary>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
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
        /// Gets the GeneralTransform for this effect.
        /// </summary>
        protected override GeneralTransform EffectMapping => new MatrixTransform();
    }
}