
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

using System.Collections.Generic;

namespace System.Windows.Media
{
    /// <summary>
    /// Creates a quadratic Bezier curve between two points in a <see cref="PathFigure"/>.
    /// </summary>
    public sealed class QuadraticBezierSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticBezierSegment"/> class.
        /// </summary>
        public QuadraticBezierSegment() { }

        /// <summary>
        /// Identifies the <see cref="Point1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(
                nameof(Point1),
                typeof(Point),
                typeof(QuadraticBezierSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the control point of the curve.
        /// </summary>
        /// <returns>
        /// The control point of this <see cref="QuadraticBezierSegment"/>. The default
        /// value is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValueInternal(Point1Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Point2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(
                nameof(Point2),
                typeof(Point),
                typeof(QuadraticBezierSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the end <see cref="Point"/> of this <see cref="QuadraticBezierSegment"/>.
        /// </summary>
        /// <returns>
        /// The end point of this <see cref="QuadraticBezierSegment"/>. The default
        /// value is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValueInternal(Point2Property, value);
        }

        internal override IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#quadratic_b%C3%A9zier_curve
            yield return "Q";
            yield return Point1.X.ToString(formatProvider);
            yield return Point1.Y.ToString(formatProvider);
            yield return Point2.X.ToString(formatProvider);
            yield return Point2.Y.ToString(formatProvider);
        }
    }
}
