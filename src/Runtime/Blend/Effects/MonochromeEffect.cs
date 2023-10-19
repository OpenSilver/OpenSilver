
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
    /// Effect that turns a visual into a monochrome color.
    /// </summary>
    public sealed class MonochromeEffect : ShaderEffect
    {
        /// <summary>
        /// Gets or sets the Color variable within the shader.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(MonochromeEffect),
                new PropertyMetadata(Color.FromArgb(255, 255, 255, 255)));

        /// <summary>
        /// Gets or sets the Input of the shader.
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(
                "Input",
                typeof(object),
                typeof(MonochromeEffect),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public MonochromeEffect() { }

        /// <summary>
        /// Gets or sets the Color variable within the shader.
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }
}