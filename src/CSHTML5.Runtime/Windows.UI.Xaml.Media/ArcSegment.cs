

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Represents an elliptical arc between two points.
    /// </summary>
    public sealed partial class ArcSegment : PathSegment
    {
        private double _strokeThickness; //Note: this is required for GetMinMaxXY because for some reason, the stroke thickness needs to go on both side of the stroke instead of towards the inside like every other shape thingy.
        private Point _ellipseCenterInCircleCoordinates;
        private double _angle1, _angle2;
        private double _additionalScalingForShapetoSmallToReachEndPoint = 1; //this is for the case where the user defines start and endPoints that are too far away from each other for an ellipse of the given size to reach both points.
        private double _minX, _minY, _maxX, _maxY;
        private bool _isUpToDate = false;
        private Point _startingPoint;

        ///// <summary>
        ///// Initializes a new instance of the ArcSegment class.
        ///// </summary>
        //public ArcSegment();

        /// <summary>
        /// Gets or sets a value that indicates whether the arc should be greater than
        /// 180 degrees.
        /// </summary>
        public bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }
        /// <summary>
        /// Identifies the IsLargeArc dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(ArcSegment), new PropertyMetadata(false, IsLargeArc_Changed));

        private static void IsLargeArc_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArcSegment arc = (ArcSegment)d;
            if (e.NewValue != e.OldValue)
            {
                arc._isUpToDate = false;

                if (arc.INTERNAL_parentPath != null && arc.INTERNAL_parentPath._isLoaded)
                {
                    arc.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }

        // Returns:
        //     The point to which the arc is drawn. The default is a Point with value 0,0.
        /// <summary>
        /// Gets or sets the endpoint of the elliptical arc.
        /// </summary>
        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }
        /// <summary>
        /// Identifies the Point dependency property.
        /// </summary>
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point", typeof(Point), typeof(ArcSegment), new PropertyMetadata(new Point(), Point_Changed));

        private static void Point_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArcSegment arc = (ArcSegment)d;
            if (e.NewValue != e.OldValue)
            {
                arc._isUpToDate = false; //see if it is ok to remove this line (it was not there before ading the call to ScheduleRedraw).

                if (arc.INTERNAL_parentPath != null && arc.INTERNAL_parentPath._isLoaded)
                {
                    arc.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount (in degrees) by which the ellipse is rotated about
        /// the x-axis.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
        /// <summary>
        /// Identifies the RotationAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(ArcSegment), new PropertyMetadata(0d, RotationAngle_Changed));

        private static void RotationAngle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArcSegment arc = (ArcSegment)d;
            if (e.NewValue != e.OldValue)
            {
                arc._isUpToDate = false; //see if it is ok to remove this line (it was not there before ading the call to ScheduleRedraw).

                if (arc.INTERNAL_parentPath != null && arc.INTERNAL_parentPath._isLoaded)
                {
                    arc.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }

        // Returns:
        //     A Size structure that describes the x-radius and y-radius of the elliptical
        //     arc. The Size structure's Width value specifies the arc's x-radius; its Height
        //     value specifies the arc's y-radius. The default is a Size with value 0,0.
        /// <summary>
        /// Gets or sets the x-radius and y-radius of the arc as a Size structure.
        /// </summary>
        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        /// <summary>
        /// Identifies the Size dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(ArcSegment), new PropertyMetadata(new Size(), Size_Changed));

        private static void Size_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArcSegment arc = (ArcSegment)d;
            if (e.NewValue != e.OldValue)
            {
                arc._isUpToDate = false; //see if it is ok to remove this line (it was not there before ading the call to ScheduleRedraw).

                if (arc.INTERNAL_parentPath != null && arc.INTERNAL_parentPath._isLoaded)
                {
                    arc.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether the arc is drawn in the Clockwise
        /// or Counterclockwise direction.
        /// </summary>
        public SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }
        /// <summary>
        /// Identifies the SweepDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(ArcSegment), new PropertyMetadata(SweepDirection.Counterclockwise, SweepDirection_Changed));

        private static void SweepDirection_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArcSegment arc = (ArcSegment)d;
            if (e.NewValue != e.OldValue)
            {
                arc._isUpToDate = false; //see if it is ok to remove this line (it was not there before ading the call to ScheduleRedraw).

                if (arc.INTERNAL_parentPath != null && arc.INTERNAL_parentPath._isLoaded)
                {
                    arc.INTERNAL_parentPath.ScheduleRedraw();
                }
            }
        }


        internal override Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, Point previousLastPoint)
        {
            //HOW IT WORKS IN WINRT:
            //  - Starting point and ending point are two fixed points that will belong to the curve
            //  - we find the two possible ellipses that pass on these two points, with the given width, height and angle
            //  - We chose one ellipse depending on the IsLargeArc combined with SweepDirection property
            //  - we draw the arc connecting the two points on the ellipse starting on the startpoint and ending on the endpoint, folowing the sweepdirection.


            //we use canvas' arc method since it is the easiest way to do this. Another solution would have been bezier segments but defining the angle would be harder.
            //JAVASCRIPT
            //  // cx,cy - center, r - horizontal radius X
            //function drawEllipseWithArcAndScale(ctx, cx, cy, rx, ry, style) {
            //  ctx.save(); // save state
            //  ctx.translate(cx-rx, cy-ry);
            //  ctx.scale(rx, ry);
            //  ctx.arc(1, 1, 1, 0, 2 * Math.PI, false);
            //  //ctx.arc(centerX, centerY, Radius, StartingAngle, EndAngle, isCounterClockwise)
            //  ctx.restore(); // restore to original state
            //  ctx.save();
            //  if(style)
            //    ctx.strokeStyle=style;
            //  ctx.stroke();
            //  ctx.restore();
            //}
            //END OF JAVASCRIPT

            UpdateArcData();


            dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            context.save(); // save state
            context.translate(xOffsetToApplyBeforeMultiplication * horizontalMultiplicator + xOffsetToApplyAfterMultiplication, yOffsetToApplyBeforeMultiplication * verticalMultiplicator + yOffsetToApplyAfterMultiplication);
            context.scale(horizontalMultiplicator, verticalMultiplicator);
            context.rotate(RotationAngle * Math.PI / 180);
            double horizontalScaling = (Size.Width) / (Size.Height);
            context.scale(horizontalScaling, 1);
            //double centerX = ((center.X + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication);
            //double centerY = (center.Y + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication;
            //double centerX = center.X;
            //double centerY = center.Y;
            double centerX = _ellipseCenterInCircleCoordinates.X;
            double centerY = _ellipseCenterInCircleCoordinates.Y;
            context.arc(centerX, centerY, Size.Height * _additionalScalingForShapetoSmallToReachEndPoint, _angle1, _angle2, SweepDirection == SweepDirection.Counterclockwise);
            //ctx.arc(centerX, centerY, Radius, StartingAngle, EndAngle, isCounterClockwise)
            context.restore(); // restore to original state
            context.stroke();

            return Point;
        }

        void FlattenArc(Point originalPt1, Point originalPt2,
                double radiusX, double radiusY, double angleRotation,
                bool isLargeArc, bool isCounterclockwise, bool saveChanges, out Point center, out double angle1, out double angle2)
        {
            _startingPoint = originalPt1; //we remember the startingPoint
            _additionalScalingForShapetoSmallToReachEndPoint = 1; //resetting the scaling.


            //todo: when the user sets a size that is not enough for the arc to reach the endPoint from the startPoint, we change the size
            //      from my observations, we keep the ratio width/height



            //Method Largely inspired from the code found at http://www.charlespetzold.com/blog/2008/01/Mathematics-of-ArcSegment.html

            // Adjust for different radii and rotation angle
            //Note: 
            //Matrix matx = new Matrix();
            //matx.Rotate(-angleRotation);
            //matx.Scale(radiusY / radiusX, 1);
            //Point pt1 = matx.Transform(originalPt1);
            //Point pt2 = matx.Transform(originalPt2);

            //The resulting matrix (M1) of the lines above is:
            //  (   A cos(θ)    -A sin(θ)   )
            //  (   B sin(θ)    B cos(θ)    )
            // where:
            //  θ = -angleRotation
            //  A = scalingOnX (see below)
            //  B = 1

            double scalingOnX = radiusY / radiusX; //this is to consider a circle instead of an ellipse, which will allow us to compute the distance to the center easily and everything will be centered on it.
            double cosValue = Math.Cos(-(angleRotation * Math.PI / 180));
            double sinValue = Math.Sin(-(angleRotation * Math.PI / 180));
            //get the point's coordinates in the new coordinates (rotated and scaled)
            Point pt1 = new Point(originalPt1.X * scalingOnX * cosValue - originalPt1.Y * scalingOnX * sinValue,
                                    originalPt1.X * sinValue + originalPt1.Y * cosValue); //This would be the result of the lines commented above.
            Point pt2 = new Point(originalPt2.X * scalingOnX * cosValue - originalPt2.Y * scalingOnX * sinValue,
                                    originalPt2.X * sinValue + originalPt2.Y * cosValue); //This would be the result of the lines commented above.

            // Get info about chord that connects both points
            Point midPoint = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
            //Vector vect = pt2 - pt1;
            //double halfChord = vect.Length / 2;
            Point vect = new Point(pt2.X - pt1.X, pt2.Y - pt1.Y); //no Vector implementation yet so we have to do this by hand:
            double vectorLength = Math.Sqrt((Math.Pow(vect.X, 2) + (Math.Pow(vect.Y, 2))));
            double halfChord = vectorLength / 2;


            if (halfChord > radiusY)
            {
                //the given size does not allow us to reach both points.
                //wild guess: to get the correct scaling, we can use simple sin/cos with the rotationAngle times RadiusY, and times scalingOnX or 1, and sum those so that it is equal to halfchord. 
                //      we should not use RotationAngle as is, it should be compared to t

                //trying the most basic version of scaling. After all, we use a circle in the current context.
                _additionalScalingForShapetoSmallToReachEndPoint = halfChord / radiusY;

            }



            // Get vector from chord to center
            Point vectRotated; //note: this was a Vector in the original version.

            // (comparing two Booleans here!)
            if (isLargeArc == isCounterclockwise)
                vectRotated = new Point(-vect.Y, vect.X);
            else
                vectRotated = new Point(vect.Y, -vect.X);

            //vectRotated.Normalize();
            //this basically means dividing by the vector's length:
            vectRotated.X = vectRotated.X / vectorLength;
            vectRotated.Y = vectRotated.Y / vectorLength;

            double squaredRadiusAfterScaling = _additionalScalingForShapetoSmallToReachEndPoint * _additionalScalingForShapetoSmallToReachEndPoint * radiusY * radiusY;

            // Distance from chord to center 
            double centerDistance = Math.Sqrt(Math.Abs(squaredRadiusAfterScaling - halfChord * halfChord)); //note: we use Pythagoras here (any chord of a circle is perpendicular to the line that passes through the middle of the chord and the center of the circle).

            // Calculate center point
            //centerPoint = midPoint + centerDistance * vectRotated; //------------------------------------------CENTER POINTS --> needed
            center = new Point(midPoint.X + centerDistance * vectRotated.X,
                                        midPoint.Y + centerDistance * vectRotated.Y);
            if (saveChanges)
            {
                _ellipseCenterInCircleCoordinates = center;


                //Calculate the "border points" (the points that will have the min X, max X, min Y, or max Y):
                // result found at: http://math.stackexchange.com/questions/91132/how-to-get-the-limits-of-rotated-ellipse
                double aSquared = Math.Pow(Size.Width * _additionalScalingForShapetoSmallToReachEndPoint / 2, 2);
                double bSquared = Math.Pow(Size.Height * _additionalScalingForShapetoSmallToReachEndPoint / 2, 2);
                double cos = Math.Cos(RotationAngle * Math.PI / 180);
                double sin = Math.Sin(RotationAngle * Math.PI / 180);
                double cosSquared = Math.Pow(cos, 2);
                double sinSquared = Math.Pow(sin, 2);
                double xLimitsOffset = 2 * Math.Sqrt(aSquared * cosSquared + bSquared * sinSquared);
                double yLimitsOffset = 2 * Math.Sqrt(aSquared * sinSquared + bSquared * cosSquared);

                //we get the center of the ellipse in the end coordinates:
                // the inverse of the matrix M1 defined earlier:
                //   1/AB * (   B cos(θ)    A sin(θ)    )
                //          (   -B sin(θ)   A cos (θ)   )
                // where:
                //  θ = -angleRotation
                //  A = scalingOnX (see below)
                //  B = 1

                //double finalCenterX = cosValue / scalingOnX * (_ellipseCenterInCircleCoordinates.X + scalingOnX * _ellipseCenterInCircleCoordinates.Y);
                //double finalCenterY = sinValue / scalingOnX * (scalingOnX * _ellipseCenterInCircleCoordinates.Y - _ellipseCenterInCircleCoordinates.X);
                double finalCenterX = (1 / scalingOnX) * (_ellipseCenterInCircleCoordinates.X * cosValue + scalingOnX * _ellipseCenterInCircleCoordinates.Y * sinValue);
                double finalCenterY = (1 / scalingOnX) * (scalingOnX * _ellipseCenterInCircleCoordinates.Y * cosValue - _ellipseCenterInCircleCoordinates.X * sinValue);

                _minX = finalCenterX - xLimitsOffset - (_strokeThickness / 2);
                _maxX = finalCenterX + xLimitsOffset + (_strokeThickness / 2);
                _minY = finalCenterY - yLimitsOffset - (_strokeThickness / 2);
                _maxY = finalCenterY + yLimitsOffset + (_strokeThickness / 2);

                //now we need to get those that are actually in the arc (since the arc is not necessarily the full ellipse):
                //We can divide the min and max points in two groups: those "above" the line and those "below" it. each group corresponds to either the points of the large arc or the short one.
                //ASSUMPTION: the group that contains the point that is furthest from the line between the given points was the large arc one
                //we get the equation of the line that goes through the two given points: (We will call this line the reference line)
                //the equation is of the form y = ax + b
                // for the points (X1, Y1) and (X2,Y2):
                //      a = (Y2-Y1) / (X2-X1)
                double equationA = double.MaxValue;
                double equationB;
                List<Point> groupAbove = new List<Point>();
                List<Point> groupBelow = new List<Point>();
                if (originalPt1.X != originalPt2.X)
                {
                    equationA = (originalPt2.Y - originalPt1.Y) / (originalPt2.X - originalPt1.X);
                    equationB = originalPt1.Y - (equationA * originalPt1.X); //b = y - ax
                }
                else //the line is a vertical line
                {
                    equationB = originalPt1.X;
                }

                PutPointInCorrectGroup(equationA, equationB, new Point(_minX, _minY), groupAbove, groupBelow);
                PutPointInCorrectGroup(equationA, equationB, new Point(_minX, _maxY), groupAbove, groupBelow);
                PutPointInCorrectGroup(equationA, equationB, new Point(_maxX, _minY), groupAbove, groupBelow);
                PutPointInCorrectGroup(equationA, equationB, new Point(_maxX, _maxY), groupAbove, groupBelow);

                double aboveDistance = -1;
                foreach (Point point in groupAbove)
                {
                    double currentDistance = Math.Abs(equationA * point.X - point.Y + equationB); //Note: for the actual distance, we should divide by a constant but we only want to compare the two distances so we can keep it like that.
                    if (currentDistance > aboveDistance)
                    {
                        aboveDistance = currentDistance;
                    }
                }
                double belowDistance = -1;
                foreach (Point point in groupBelow)
                {
                    double currentDistance = Math.Abs(equationA * point.X - point.Y + equationB);
                    if (currentDistance > belowDistance)
                    {
                        belowDistance = currentDistance;
                    }
                }

                List<Point> relevantPoints;
                if (aboveDistance == belowDistance)
                {
                    bool isUpsideDown = originalPt2.X < originalPt1.X; //this being true will mean that the clockwise direction from the first point goes in the groupBelow instead of the groupAbove ad vice-versa for the CounterClockwise direction.
                    if (originalPt1.X == originalPt2.X)
                    {
                        isUpsideDown = originalPt2.Y < originalPt1.Y;
                    }
                    if (isCounterclockwise ^ isUpsideDown) //yay XOR!! "if we are going CounterClockwise AND we are not upside down OR we are going Clockwise AND we are upside down".
                    {
                        relevantPoints = groupBelow;
                    }
                    else //"if we are going Clockwise AND we are not upside down OR we are going CounterClockwise AND we are upside down".
                    {
                        relevantPoints = groupAbove;
                    }
                }
                else if (aboveDistance > belowDistance == isLargeArc)
                {
                    relevantPoints = groupAbove;
                }
                else
                {
                    relevantPoints = groupBelow;
                }

                _minX = originalPt1.X;
                _maxX = originalPt1.X;
                _minY = originalPt1.Y;
                _maxY = originalPt1.Y;
                if (originalPt2.X < _minX)
                    _minX = originalPt2.X;
                if (originalPt2.X > _maxX)
                    _maxX = originalPt2.X;
                if (originalPt2.Y < _minY)
                    _minY = originalPt2.Y;
                if (originalPt2.Y > _maxY)
                    _maxY = originalPt2.Y;
                foreach (Point point in relevantPoints)
                {
                    if (point.X < _minX)
                        _minX = point.X;
                    if (point.X > _maxX)
                        _maxX = point.X;
                    if (point.Y < _minY)
                        _minY = point.Y;
                    if (point.Y > _maxY)
                        _maxY = point.Y;
                }
            }
            // Get angles from center to the two points
            angle1 = Math.Atan2(pt1.Y - center.Y, pt1.X - center.X);
            angle2 = Math.Atan2(pt2.Y - center.Y, pt2.X - center.X);

            // (another comparison of two Booleans!)
            if (isLargeArc == (Math.Abs(angle2 - angle1) < Math.PI))
            {
                if (angle1 < angle2)
                    angle1 += 2 * Math.PI;
                else
                    angle2 += 2 * Math.PI;
            }
            if (saveChanges)
            {
                _angle1 = angle1;
                _angle2 = angle2;
                _isUpToDate = true;
            }
        }


        private void PutPointInCorrectGroup(double equationA, double equationB, Point point, List<Point> groupAbove, List<Point> groupBelow)
        {
            if (!(equationA == double.MaxValue))
            {
                double OrdinateOnXOnReferenceLine = equationA * _minX + equationB;
                if (point.Y > OrdinateOnXOnReferenceLine)
                {
                    groupAbove.Add(point);
                }
                else
                {
                    groupBelow.Add(point);
                }
            }
            else
            {
                if (point.X > equationB)
                {
                    groupAbove.Add(point);
                }
                else
                {
                    groupBelow.Add(point);
                }
            }
        }

        internal override Point GetMaxXY()
        {
            UpdateArcData();
            return new Point(_maxX, _maxY);
        }

        internal void UpdateStartPosition(Point newStartingPoint)
        {
            if (_startingPoint != newStartingPoint)
            {
                _isUpToDate = false;
                _startingPoint = newStartingPoint;
            }
        }


        internal void UpdateArcData()
        {
            if (!_isUpToDate)
            {
                double angle1, angle2;
                Point point;
                FlattenArc(_startingPoint, Point, Size.Width, Size.Height, RotationAngle, IsLargeArc, SweepDirection == Media.SweepDirection.Counterclockwise, true, out point, out angle1, out angle2);
            }
        }

        internal void UpdateStrokeThickness(double newStrokeThickness)
        {
            if (newStrokeThickness != _strokeThickness)
            {
                _strokeThickness = newStrokeThickness;
                _isUpToDate = false;
            }
        }

        internal override Point GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, Point startingPoint)
        {
            UpdateArcData();
            if (minX > _minX)
            {
                minX = _minX;
            }
            if (maxX < _maxX)
            {
                maxX = _maxX;
            }
            if (minY > _minY)
            {
                minY = _minY;
            }
            if (maxY < _maxY)
            {
                maxY = _maxY;
            }

            return Point;
        }
    }

}
