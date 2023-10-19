
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
    /// Effect that simulates a magnifying lens.
    /// </summary>
    public sealed class MagnifyEffect : ShaderEffect
    {
        /// <summary>
        /// Gets or sets the amount variable within the shader.
        /// </summary>
        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register(
                nameof(Amount),
                typeof(double),
                typeof(MagnifyEffect),
                new PropertyMetadata(0.5));
        
        /// <summary>
        /// Gets or sets the center variable within the shader.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(MagnifyEffect),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the InnerRadius variable within the shader.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register(
                nameof(InnerRadius),
                typeof(double),
                typeof(MagnifyEffect),
                new PropertyMetadata(0.2));

        /// <summary>
        /// Gets or sets the Input used in the shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(MagnifyEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the OuterRadius variable within the shader.
        /// </summary>
        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register(
                nameof(OuterRadius),
                typeof(double),
                typeof(MagnifyEffect),
                new PropertyMetadata(0.4));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public MagnifyEffect() { }

        /// <summary>
        /// Gets or sets the ShrinkFactor. The higher the shrink factor the "smaller" the
        /// content inside the ellipse will appear.
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// Gets or sets the Center variable within the shader.
        /// </summary>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        /// <summary>
        /// Gets or sets the InnerRadius variable within the shader.
        /// </summary>
        public double InnerRadius
        {
            get => (double)GetValue(InnerRadiusProperty);
            set => SetValue(InnerRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the OuterRadius variable within the shader.
        /// </summary>
        public double OuterRadius
        {
            get => (double)GetValue(OuterRadiusProperty);
            set => SetValue(OuterRadiusProperty, value);
        }

        /// <summary>
        /// Gets the EffectMapping.
        /// </summary>
        protected override GeneralTransform EffectMapping => new MatrixTransform();
    }
}