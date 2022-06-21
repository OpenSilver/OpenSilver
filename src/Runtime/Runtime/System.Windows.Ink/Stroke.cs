#if MIGRATION
using System.Windows.Input;

namespace System.Windows.Ink
{
    /// <summary>Represents a collection of points that correspond to a stylus-down, move, and stylus-up sequence.</summary>
    public sealed class Stroke : DependencyObject
    {
        public static readonly DependencyProperty StylusPointsProperty =
            DependencyProperty.Register("StylusPoints", typeof(StylusPointCollection), typeof(Stroke), new PropertyMetadata());
        public static readonly DependencyProperty DrawingAttributesProperty =
            DependencyProperty.Register("DrawingAttributes", typeof(DrawingAttributes), typeof(Stroke), new PropertyMetadata());

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Ink.Stroke" /> class.</summary>
        public Stroke()
        {
            StylusPoints = new StylusPointCollection();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Ink.Stroke" /> class with the specified <see cref="T:System.Windows.Input.StylusPointCollection" />.</summary>
        /// <param name="stylusPoints">A <see cref="T:System.Windows.Input.StylusPointCollection" /> that represents the <see cref="T:System.Windows.Ink.Stroke" />.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="stylusPoints" /> does not contain any stylus points.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="stylusPoints" /> is null.</exception>
        public Stroke(StylusPointCollection stylusPoints)
        {
            if (stylusPoints == null)
                throw new ArgumentNullException(nameof(stylusPoints));
            if (stylusPoints.Count == 0)
                throw new ArgumentException(nameof(stylusPoints));

            StylusPoints = stylusPoints;
        }

        /// <summary>Gets or sets the stylus points of the <see cref="T:System.Windows.Ink.Stroke" />.</summary>
        /// <returns>The <see cref="T:System.Windows.Input.StylusPointCollection" /> that contains the stylus points that represent the current <see cref="T:System.Windows.Ink.Stroke" />.</returns>
        public StylusPointCollection StylusPoints
        {
            get
            {
                return (StylusPointCollection)this.GetValue(Stroke.StylusPointsProperty);
            }
            set
            {
                this.SetValue(Stroke.StylusPointsProperty, value);
            }
        }

        /// <summary>Gets or sets the properties of the stroke, such as <see cref="P:System.Windows.Ink.DrawingAttributes.Height" />, <see cref="P:System.Windows.Ink.DrawingAttributes.Width" />, <see cref="P:System.Windows.Ink.DrawingAttributes.Color" />, or <see cref="P:System.Windows.Ink.DrawingAttributes.OutlineColor" />. </summary>
        /// <returns>The <see cref="T:System.Windows.Ink.DrawingAttributes" /> of the stroke.</returns>
        public DrawingAttributes DrawingAttributes
        {
            get
            {
                return (DrawingAttributes)this.GetValue(Stroke.DrawingAttributesProperty);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.SetValue(Stroke.DrawingAttributesProperty, (DependencyObject)value);
            }
        }
    }
}
#endif
