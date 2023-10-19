
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

using System.ComponentModel;
using System.Windows.Media;

namespace System.Windows.Ink
{
    /// <summary>
    /// Specifies drawing attributes that are used to draw a <see cref="Stroke" />.
    /// </summary>
    public sealed class DrawingAttributes : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingAttributes" /> class.
        /// </summary>
        public DrawingAttributes()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(DrawingAttributes),
                new PropertyMetadata(Colors.Black));

        /// <summary>
        /// Gets or sets the color that is used to draw a <see cref="Stroke" />.
        /// </summary>
        /// <returns>
        /// The color that is used to draw a <see cref="Stroke" />.
        /// The default is Black.
        /// </returns>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty OutlineColorProperty =
            DependencyProperty.Register(
                nameof(OutlineColor),
                typeof(Color),
                typeof(DrawingAttributes),
                new PropertyMetadata(new Color()));

        /// <summary>
        /// Gets or sets the outline color that is used to draw a <see cref="Stroke" />.
        /// </summary>
        /// <returns>
        /// The outline color of the stylus that is used to draw a <see cref="Stroke" />.
        /// The default is Black.
        /// </returns>
        public Color OutlineColor
        {
            get => (Color)GetValue(OutlineColorProperty);
            set => SetValue(OutlineColorProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(DrawingAttributes),
                new PropertyMetadata(3.0));

        /// <summary>
        /// Gets or sets the width of the stylus that is used to draw a <see cref="Stroke" />.
        /// </summary>
        /// <returns>
        /// The width of the stylus that is used to draw a <see cref="Stroke" />.
        /// The default is 2 pixels.
        /// </returns>
        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(DrawingAttributes),
                new PropertyMetadata(3.0));

        /// <summary>
        /// Gets or sets the height of the stylus that is used to draw a <see cref="Stroke" />.
        /// </summary>
        /// <returns>
        /// The height of the stylus that is used to draw a <see cref="Stroke" />.
        /// The default is 2 pixels.
        /// </returns>
        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }
    }
}
