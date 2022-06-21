#if MIGRATION
using System.Windows.Media;

namespace System.Windows.Ink
{
    /// <summary>Specifies drawing attributes that are used to draw a <see cref="T:System.Windows.Ink.Stroke" />.</summary>
    public sealed class DrawingAttributes : DependencyObject
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(DrawingAttributes), new PropertyMetadata());
        public static readonly DependencyProperty OutlineColorProperty =
            DependencyProperty.Register(nameof(OutlineColor), typeof(Color), typeof(DrawingAttributes), new PropertyMetadata());
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(DrawingAttributes), new PropertyMetadata());
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(double), typeof(DrawingAttributes), new PropertyMetadata());

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Ink.DrawingAttributes" /> class. </summary>
        public DrawingAttributes()
        {
        }

        /// <summary>Gets or sets the color that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />.</summary>
        /// <returns>The color that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />. The default is Black.</returns>
        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }
            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        /// <summary>Gets or sets the outline color that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />.</summary>
        /// <returns>The outline color of the stylus that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />. The default is Black.</returns>
        public Color OutlineColor
        {
            get
            {
                return (Color)this.GetValue(DrawingAttributes.OutlineColorProperty);
            }
            set
            {
                this.SetValue(DrawingAttributes.OutlineColorProperty, value);
            }
        }

        /// <summary>Gets or sets the width of the stylus that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />.</summary>
        /// <returns>The width of the stylus that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />. The default is 2 pixels.</returns>
        public double Width
        {
            get
            {
                return (double)this.GetValue(DrawingAttributes.WidthProperty);
            }
            set
            {
                this.SetValue(DrawingAttributes.WidthProperty, value);
            }
        }

        /// <summary>Gets or sets the height of the stylus that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />.</summary>
        /// <returns>The height of the stylus that is used to draw a <see cref="T:System.Windows.Ink.Stroke" />. The default is 2 pixels.</returns>
        public double Height
        {
            get
            {
                return (double)this.GetValue(DrawingAttributes.HeightProperty);
            }
            set
            {
                this.SetValue(DrawingAttributes.HeightProperty, value);
            }
        }
    }
}
#endif