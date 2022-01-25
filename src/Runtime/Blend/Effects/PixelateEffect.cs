
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
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Effect that pixelates a visual.
    /// </summary>
    public sealed class PixelateEffect : ShaderEffect
    {
        /// <summary>
        /// The explicit input for this pixel shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(PixelateEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// This property is mapped to the Pixelation variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty PixelationProperty =
            DependencyProperty.Register(
                nameof(PixelateEffect),
                typeof(double),
                typeof(PixelateEffect),
                new PropertyMetadata(0.75));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public PixelateEffect() { }

        /// <summary>
        /// Gets or sets the amount of pixelation inside the shader.
        /// </summary>
        public double Pixelation
        {
            get => (double)GetValue(PixelationProperty);
            set => SetValue(PixelationProperty, value);
        }
    }
}