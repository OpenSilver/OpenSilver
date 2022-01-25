
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

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Effect that modifies the color tone of a visual using two colors as the sampling.
    /// </summary>
    public sealed class ColorToneEffect : ShaderEffect
    {
        /// <summary>
        /// This property is mapped to the DarkColor variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty DarkColorProperty =
            DependencyProperty.Register(
                nameof(DarkColor),
                typeof(Color),
                typeof(ColorToneEffect),
                new PropertyMetadata(Color.FromArgb(255, 51, 128, 0)));

        /// <summary>
        /// This property is mapped to the Desaturation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty DesaturationProperty =
            DependencyProperty.Register(
                nameof(DarkColor),
                typeof(double),
                typeof(ColorToneEffect),
                new PropertyMetadata(0.5));

        /// <summary>
        /// The explicit input for this pixel shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(ColorToneEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// This property is mapped to the LightColor variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty LightColorProperty =
            DependencyProperty.Register(
                nameof(DarkColor),
                typeof(Color),
                typeof(ColorToneEffect),
                new PropertyMetadata(Color.FromArgb(255, 225, 229, 128)));

        /// <summary>
        /// This property is mapped to the Tone variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty ToneAmountProperty =
            DependencyProperty.Register(
                nameof(DarkColor),
                typeof(double),
                typeof(ColorToneEffect),
                new PropertyMetadata(0.5));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public ColorToneEffect() { }

        /// <summary>
        /// Gets or sets the DarkColor variable within the shader.
        /// </summary>
        public Color DarkColor
        {
            get => (Color)GetValue(DarkColorProperty);
            set => SetValue(DarkColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the Desaturation variable within the shader.
        /// </summary>
        public double Desaturation
        {
            get => (double)GetValue(DesaturationProperty);
            set => SetValue(DesaturationProperty, value);
        }

        /// <summary>
        /// Gets or sets the LightColor variable within the shader.
        /// </summary>
        public Color LightColor
        {
            get => (Color)GetValue(LightColorProperty);
            set => SetValue(LightColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the Tone variable within the shader.
        /// </summary>
        public double ToneAmount
        {
            get => (double)GetValue(ToneAmountProperty);
            set => SetValue(ToneAmountProperty, value);
        }
    }
}