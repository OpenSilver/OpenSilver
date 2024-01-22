
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
using System.Windows.Shapes;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a line drawn between two points, which can be part of a <see cref="PathFigure"/>
    /// within <see cref="Path"/> data.
    /// </summary>
    public sealed class LineSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> class.
        /// </summary>
        public LineSegment() { }

        /// <summary>
        /// Identifies the <see cref="Point"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register(
                nameof(Point),
                typeof(Point),
                typeof(LineSegment),
                new PropertyMetadata(new Point(), PropertyChanged));

        /// <summary>
        /// Gets or sets the end point of the line segment.
        /// </summary>
        /// <returns>
        /// The end point of the line segment. The default is a <see cref="Point"/> with 
        /// value 0,0.
        /// </returns>
        public Point Point
        {
            get => (Point)GetValue(PointProperty);
            set => SetValueInternal(PointProperty, value);
        }

        internal override IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#lineto_path_commands
            yield return "L";
            yield return Point.X.ToString(formatProvider);
            yield return Point.Y.ToString(formatProvider);
        }
    }
}