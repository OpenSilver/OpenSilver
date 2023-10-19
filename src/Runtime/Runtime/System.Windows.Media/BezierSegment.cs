
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
    /// Represents a cubic Bezier curve drawn between two points.
    /// </summary>
    public sealed class BezierSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BezierSegment"/> class.
        /// </summary>
        public BezierSegment() { }

        /// <summary>
        /// Identifies the <see cref="Point1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(
                nameof(Point1),
                typeof(Point),
                typeof(BezierSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the first control point of the curve.
        /// </summary>
        /// <returns>
        /// The first control point of the curve. The default is a <see cref="Point"/> with
        /// value 0,0.
        /// </returns>
        public Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Point2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(
                nameof(Point2),
                typeof(Point),
                typeof(BezierSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the second control point of the curve.
        /// </summary>
        /// <returns>
        /// The second control point of the curve.
        /// </returns>
        public Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Point3"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point3Property =
            DependencyProperty.Register(
                nameof(Point3),
                typeof(Point),
                typeof(BezierSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the end point of the curve.
        /// </summary>
        /// <returns>
        /// The end point of the curve.
        /// </returns>
        public Point Point3
        {
            get => (Point)GetValue(Point3Property);
            set => SetValue(Point3Property, value);
        }

        internal override IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#cubic_b%C3%A9zier_curve
            yield return "C";
            yield return Point1.X.ToString(formatProvider);
            yield return Point1.Y.ToString(formatProvider);
            yield return Point2.X.ToString(formatProvider);
            yield return Point2.Y.ToString(formatProvider);
            yield return Point3.X.ToString(formatProvider);
            yield return Point3.Y.ToString(formatProvider);
        }
    }
}
