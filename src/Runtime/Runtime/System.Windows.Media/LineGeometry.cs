
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

using System;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents the geometry of a line.
    /// </summary>
    public sealed class LineGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometry"/> class that
        /// has no length.
        /// </summary>
        public LineGeometry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometry"/> class that 
        /// has the specified start and end points.
        /// </summary>
        public LineGeometry(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Identifies the <see cref="EndPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(
                nameof(EndPoint),
                typeof(Point),
                typeof(LineGeometry),
                new PropertyMetadata(new Point(), OnPathChanged));

        /// <summary>
        /// Gets or sets the end point of a line.
        /// </summary>
        /// <returns>
        /// The end point of the line. The default is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StartPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(
                nameof(StartPoint),
                typeof(Point),
                typeof(LineGeometry),
                new PropertyMetadata(new Point(), OnPathChanged));

        /// <summary>
        /// Gets or sets the start point of the line.
        /// </summary>
        /// <returns>
        /// The start point of the line. The default is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        internal override Rect BoundsInternal
        {
            get
            {
                Rect rect = new Rect(StartPoint, EndPoint);

                Transform transform = Transform;

                if (transform != null && !transform.IsIdentity)
                {
                    rect = transform.TransformBounds(rect);
                }

                return rect;
            }
        }

        internal override string ToPathData(IFormatProvider formatProvider)
        {
            var p1 = StartPoint;
            var p2 = EndPoint;

            return $"M {p1.X.ToString(formatProvider)},{p1.Y.ToString(formatProvider)} L {p2.X.ToString(formatProvider)}, {p2.Y.ToString(formatProvider)} Z";
        }
    }
}