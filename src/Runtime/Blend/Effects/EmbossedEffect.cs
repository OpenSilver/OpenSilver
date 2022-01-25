
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
    /// Effect that simulates an embossed look for the visual.
    /// </summary>
    public sealed class EmbossedEffect : ShaderEffect
    {
        /// <summary>
        /// This property is mapped to the Amount variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register(
                nameof(Amount),
                typeof(double),
                typeof(EmbossedEffect),
                new PropertyMetadata(3.0));

        /// <summary>
        /// This property is mapped to the Color variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(object),
                typeof(EmbossedEffect),
                new PropertyMetadata(Color.FromArgb(255, 128, 128, 128)));

        /// <summary>
        /// This property is mapped to the Height variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(EmbossedEffect),
                new PropertyMetadata(0.001));

        /// <summary>
        /// The explicit input for this pixel shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(EmbossedEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public EmbossedEffect() { }

        /// <summary>
        /// Gets or sets the Amount variable within the shader.
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// Gets or sets the Color variable within the shader.
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the Height variable within the shader.
        /// </summary>
        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }
    }
}