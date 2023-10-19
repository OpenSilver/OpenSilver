
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

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Effect that implements a bloom illumination of a visual.
    /// </summary>
    public sealed class BloomEffect : ShaderEffect
    {
        /// <summary>
        /// This property is mapped to the BloomIntensity variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BaseBloomIntensityProperty =
            DependencyProperty.Register(
                "BaseBloomIntensity",
                typeof(Point),
                typeof(BloomEffect),
                new PropertyMetadata(new Point(1.0, 1.25)));

        /// <summary>
        /// This property is mapped to the BloomSaturation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BaseBloomSaturationProperty =
            DependencyProperty.Register(
                "BaseBloomIntensity",
                typeof(Point),
                typeof(BloomEffect),
                new PropertyMetadata(new Point(1.0, 1.0)));

        /// <summary>
        /// This property is mapped to the BaseIntensity variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BaseIntensityProperty =
            DependencyProperty.Register(
                "BaseBloomIntensity",
                typeof(double),
                typeof(BloomEffect),
                new PropertyMetadata(1.0));

        /// <summary>
        /// This property is mapped to the BaseSaturation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BaseSaturationProperty =
            DependencyProperty.Register(
                "BaseBloomIntensity",
                typeof(double),
                typeof(BloomEffect),
                new PropertyMetadata(1.0));

        /// <summary>
        /// This property is mapped to the BloomIntensity variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BloomIntensityProperty =
            DependencyProperty.Register(
                nameof(BloomIntensity),
                typeof(double),
                typeof(BloomEffect),
                new PropertyMetadata(1.25));

        /// <summary>
        /// This property is mapped to the BloomSaturation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty BloomSaturationProperty =
            DependencyProperty.Register(
                nameof(BloomSaturation),
                typeof(double),
                typeof(BloomEffect),
                new PropertyMetadata(1.0));

        /// <summary>
        /// The explicit input for this pixel shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(BloomEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// This property is mapped to the BaseSaturation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(
                nameof(Threshold),
                typeof(double),
                typeof(BloomEffect),
                new PropertyMetadata(0.25));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public BloomEffect() { }

        /// <summary>
        /// Gets or sets the base intensity.
        /// </summary>
        public double BaseIntensity
        {
            get => (double)GetValue(BaseIntensityProperty);
            set => SetValue(BaseIntensityProperty, value);
        }

        /// <summary>
        /// Gets or sets the base saturation.
        /// </summary>
        public double BaseSaturation
        {
            get => (double)GetValue(BaseSaturationProperty);
            set => SetValue(BaseSaturationProperty, value);
        }

        /// <summary>
        /// Gets or sets the bloom intensity.
        /// </summary>
        public double BloomIntensity
        {
            get => (double)GetValue(BloomIntensityProperty);
            set => SetValue(BloomIntensityProperty, value);
        }

        /// <summary>
        /// Gets or sets the bloom saturation.
        /// </summary>
        public double BloomSaturation
        {
            get => (double)GetValue(BloomIntensityProperty);
            set => SetValue(BloomIntensityProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum intensity that will be used for the bloom.
        /// </summary>
        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }
    }
}
