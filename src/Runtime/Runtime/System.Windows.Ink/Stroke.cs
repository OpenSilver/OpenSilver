
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
using System.Windows.Input;

namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a collection of points that correspond to a stylus-down, move, and stylus-up sequence.
    /// </summary>
    public sealed class Stroke : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stroke" /> class.
        /// </summary>
        public Stroke()
        {
            StylusPoints = new StylusPointCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stroke" /> class with the 
        /// specified <see cref="StylusPointCollection" />.
        /// </summary>
        /// <param name="stylusPoints">
        /// A <see cref="StylusPointCollection" /> that represents the <see cref="Stroke" />.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="stylusPoints" /> does not contain any stylus points.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stylusPoints" /> is null.
        /// </exception>
        public Stroke(StylusPointCollection stylusPoints)
        {
            if (stylusPoints == null)
            {
                throw new ArgumentNullException(nameof(stylusPoints));
            }
            if (stylusPoints.Count == 0)
            {
                throw new ArgumentException(nameof(stylusPoints));
            }

            StylusPoints = stylusPoints;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty StylusPointsProperty =
            DependencyProperty.Register(
                nameof(StylusPoints),
                typeof(StylusPointCollection),
                typeof(Stroke),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the stylus points of the <see cref="Stroke" />.
        /// </summary>
        /// <returns>
        /// The <see cref="StylusPointCollection" /> that contains the stylus points 
        /// that represent the current <see cref="Stroke" />.
        /// </returns>
        public StylusPointCollection StylusPoints
        {
            get
            {
                var points = (StylusPointCollection)GetValue(StylusPointsProperty);
                if (points == null)
                {
                    points = new StylusPointCollection();
                    SetValueInternal(StylusPointsProperty, points);
                }
                return points;
            }
            set { SetValueInternal(StylusPointsProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty DrawingAttributesProperty =
            DependencyProperty.Register(
                nameof(DrawingAttributes),
                typeof(DrawingAttributes),
                typeof(Stroke),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the properties of the stroke, such as <see cref="DrawingAttributes.Height" />, 
        /// <see cref="DrawingAttributes.Width" />, <see cref="DrawingAttributes.Color" />, or 
        /// <see cref="DrawingAttributes.OutlineColor" />. 
        /// </summary>
        /// <returns>
        /// The <see cref="DrawingAttributes" /> of the stroke.
        /// </returns>
        public DrawingAttributes DrawingAttributes
        {
            get
            {
                var attributes = (DrawingAttributes)GetValue(DrawingAttributesProperty);
                if (attributes == null)
                {
                    attributes = new DrawingAttributes();
                    SetValueInternal(DrawingAttributesProperty, attributes);
                }
                return attributes;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                SetValueInternal(DrawingAttributesProperty, value);
            }
        }
    }
}
