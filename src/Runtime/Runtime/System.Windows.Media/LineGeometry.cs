
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

using OpenSilver.Internal.Media;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents the geometry of a line.
    /// </summary>
    public sealed class LineGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometry"/> class that has no length.
        /// </summary>
        public LineGeometry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometry"/> class that has the specified start and end points.
        /// </summary>
        /// <param name="startPoint">
        /// The start point of the line.
        /// </param>
        /// <param name="endPoint">
        /// The end point of the line.
        /// </param>
        public LineGeometry(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometry"/> class.
        /// </summary>
        /// <param name="startPoint">
        /// The start point.
        /// </param>
        /// <param name="endPoint">
        /// The end point.
        /// </param>
        /// <param name="transform">
        /// The transformation to apply to the line.
        /// </param>
        public LineGeometry(Point startPoint, Point endPoint, Transform transform)
            : this(startPoint, endPoint)
        {
            Transform = transform;
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
            set => SetValueInternal(EndPointProperty, value);
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
            set => SetValueInternal(StartPointProperty, value);
        }

        /// <summary>
        /// Gets the area of the filled region of this <see cref="LineGeometry"/> object.
        /// </summary>
        /// <param name="tolerance">
        /// The computational tolerance of error.
        /// </param>
        /// <param name="type">
        /// The specified type for interpreting the error tolerance.
        /// </param>
        /// <returns>
        /// The area of the filled region of this <see cref="LineGeometry"/> object, which is always 
        /// 0 because a line contains no area.
        /// </returns>
        public override double GetArea(double tolerance, ToleranceType type) => 0.0;

        /// <summary>
        /// Determines whether this <see cref="LineGeometry"/> object is empty.
        /// </summary>
        /// <returns>
        /// true if this <see cref="LineGeometry"/> is empty; otherwise, false.
        /// </returns>
        public override bool IsEmpty() => false;

        /// <summary>
        /// Determines whether this <see cref="LineGeometry"/> object can have curved segments.
        /// </summary>
        /// <returns>
        /// true if this <see cref="LineGeometry"/> object can have curved segments; otherwise, false.
        /// </returns>
        public override bool MayHaveCurves() => false;

        internal override Rect GetBoundsInternal()
        {
            var rect = new Rect(StartPoint, EndPoint);

            if (Transform is Transform transform && !Transform.IsIdentityTransform(transform))
            {
                rect = transform.TransformBounds(rect);
            }

            return rect;
        }

        internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
        {
            Point startPoint = StartPoint;
            Point endPoint = EndPoint;

            Matrix matrix = GetCombinedMatrix(transform);
            if (!matrix.IsIdentity)
            {
                startPoint *= matrix;
                endPoint *= matrix;
            }

            context.BeginFigure(startPoint, true, false);
            context.LineTo(endPoint, true, false);
        }
    }
}