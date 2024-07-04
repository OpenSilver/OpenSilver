
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using OpenSilver.Internal.Media.Animation;
using Stop = (double Offset, System.Windows.Media.Color Color);

namespace System.Windows.Media
{
    /// <summary>
    /// Paints an area with a linear gradient.
    /// </summary>
    public sealed class LinearGradientBrush : GradientBrush, ICloneOnAnimation<LinearGradientBrush>
    {
        private readonly bool _isClone;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientBrush"/> class.
        /// </summary>
        public LinearGradientBrush() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientBrush"/> class
        /// that has the specified <see cref="GradientStopCollection"/> and angle.
        /// </summary>
        /// <param name="gradientStopCollection">
        /// The <see cref="GradientBrush.GradientStops"/> to set on this brush.
        /// </param>
        /// <param name="angle">
        /// A System.Double that represents the angle, in degrees, of the gradient. A value
        /// of 0 creates a horizontal gradient, and a value of 90 creates a vertical gradient.
        /// Negative values are permitted, as are values over 360 (are treated as mod 360).
        /// </param>
        public LinearGradientBrush(GradientStopCollection gradientStopCollection, double angle)
        {
            GradientStops = gradientStopCollection;
            EndPoint = EndPointFromAngle(angle);
        }

        private LinearGradientBrush(LinearGradientBrush original)
            : base(original)
        {
            _isClone = true;

            StartPoint = original.StartPoint;
            EndPoint = original.EndPoint;
        }

        /// <summary>
        /// Identifies the <see cref="EndPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(
                nameof(EndPoint),
                typeof(Point),
                typeof(LinearGradientBrush),
                new PropertyMetadata(new Point(1, 1)));

        /// <summary>
        /// Gets or sets the ending two-dimensional coordinates of the linear gradient.
        /// </summary>
        /// <returns>
        /// The ending two-dimensional coordinates of the linear gradient. The default is
        /// a  <see cref="Point"/> with value 1,1.
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
                typeof(LinearGradientBrush),
                new PropertyMetadata(new Point()));

        /// <summary>
        /// Gets or sets the starting two-dimensional coordinates of the linear gradient.
        /// </summary>
        /// <returns>
        /// The starting two-dimensional coordinates for the linear gradient. The default
        /// is a <see cref="Point"/> with value 0,0.
        /// </returns>
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValueInternal(StartPointProperty, value);
        }

        bool ICloneOnAnimation<LinearGradientBrush>.IsClone => _isClone;

        LinearGradientBrush ICloneOnAnimation<LinearGradientBrush>.Clone() => new LinearGradientBrush(this);

        private Point EndPointFromAngle(double angle)
        {
            // Convert the angle from degrees to radians
            angle = angle * (1.0 / 180.0) * Math.PI;

            return (new Point(Math.Cos(angle), Math.Sin(angle)));
        }

        internal override ValueTask<string> GetDataStringAsync(UIElement parent) => new(ToHtmlString(parent));

        internal override ISvgBrush GetSvgElement(Shape shape) => new SvgLinearGradient(shape, this);

        private (string Stops, double Alpha) ToHtmlStringForAbsoluteMappingMode(DependencyObject parent)
        {
            //Note: for a more detailed explanation on the maths in this method, see the non absolute mapping mode version.

            Point start = StartPoint;
            Point end = EndPoint;

            double startDistance;
            double endDistance;
            double alpha;

            if (end.X == start.X)
            {
                // vertical line: distance = their Y
                startDistance = start.Y;
                endDistance = end.Y;
                // setting alpha:
                alpha = -Math.PI / 2;
            }
            else if (end.Y == start.Y)
            {
                // horizontal line: distance = their X

                startDistance = start.X;
                endDistance = end.X;
                alpha = 0;
            }
            else
            {
                // this is the slope of the line L defined by StartPoint and EndPoint
                double slope = (end.Y - start.Y) / (end.X - start.X);
                // this is the slope of any line perpendicular to L
                double perpSlope = -1 / slope; 

                // this is k so that Y = perpSlope X + k is the line perpendicular P1 to
                // L intersecting on StartPoint
                double kStart = start.Y + 1 / slope * start.X;
                // same for EndPoint (P2)
                double kEnd = end.Y + 1 / slope * end.X; 

                if (slope > 0)
                {
                    // now we want the intersections between P1 and P2 and Y = slope * X
                    // slope * X  = perpSlope * X + k 
                    //  --> X = k / (slope - perpslope)

                    // this is basically X of the projection of StartPoint on the
                    // parallel to L, that passes through (0,0)
                    double XIStart = kStart / (slope - perpSlope);
                    // this is basically Y of the projection of StartPoint on the
                    // parallel to L, that passes through (0,0)
                    double YIStart = slope * XIStart; 

                    double XIEnd = kEnd / (slope - perpSlope);
                    double YIEnd = slope * XIEnd;

                    //now we get the distance between those points and (0,0) and we have their distances:
                    startDistance = Math.Sqrt(Math.Pow(XIStart, 2) + Math.Pow(YIStart, 2));
                    endDistance = Math.Sqrt(Math.Pow(XIEnd, 2) + Math.Pow(YIEnd, 2));
                }
                else
                {
                    //now we want the intersections between P1 and P2 and Y = height - slope * X

                    double height = parent is FrameworkElement fe ? fe.RenderSize.Height : 1;

                    double XIStart = (height - kStart) / (1 - 1 / slope);
                    // this is basically Y of the projection of StartPoint on the parallel to L,
                    // that passes through (0,0)
                    double YIStart = height - slope * XIStart; 

                    //same for the end point:
                    double XIEnd = (height - kEnd) / (1 - 1 / slope);
                    double YIEnd = height - slope * XIEnd;

                    // now we get the distance between those points and (width,0) or (0,height)
                    // (I don't know which one) and we have their distances:
                    startDistance = Math.Sqrt(Math.Pow(XIStart, 2) + Math.Pow(height - YIStart, 2));
                    endDistance = Math.Sqrt(Math.Pow(XIEnd, 2) + Math.Pow(height - YIEnd, 2));
                }

                // setting alpha:
                double XVariation = (end.X - start.X);
                double YVariation = (end.Y - start.Y);
                if (XVariation < 0)
                {
                    // Note: these are to "fix" the value returned by the ArcTan (the result
                    // is basically the same angle but in the opposite direction)
                    XVariation = -XVariation; 
                    YVariation = -YVariation;
                }
                alpha = Math.Atan2(XVariation, YVariation) - Math.PI / 2;
            }
            string stops = GetOffsetsString(startDistance, endDistance, "px");

            return (stops, alpha);
        }

        private (string Stops, double Alpha) ToHtmlStringForRelativeMappingMode(DependencyObject parent)
        {
            Size size = parent is FrameworkElement fe ? fe.RenderSize : new Size(1, 1);

            double startX = StartPoint.X;
            double startY = StartPoint.Y;
            double endX = EndPoint.X;
            double endY = EndPoint.Y;
            // first we get startpoint.X, Y and endPoint.X, Y are in relative coordinates (because we
            // made it that way):

            // Note: For the linear GradientBrush to work properly, we need to take into
            // consideration the positions of the StartPoint and EndPoint not only for
            // the angle, but the offsets are depending on it :
            // Ex: StartPoint at (0,0.5), EndPoint at (1,1), a gradientStop with an offset
            // of 0 will be at the (0,0.5) position, which is not what we get when putting 0%.
            // We need to get the percentages of the StartPoint and EndPoint positions depending
            // on the angle and then determine the percentages of the offsets using them.
            // Ex: if StartPoint is at percentage P1 and EndPoint is at percentage P2, then
            // Offset with O1 will be at percentage P1 + (P2-P1)*O1 (note that O1 is not a
            // percentage but a number usually between 0 and 1).

            //To know P1 and P2:
            //  - omega is the angle we would get with points (0,0) and (1,1)
            //  - alpha is the angle with the current points
            //
            //      if omega < alpha modulo Pi < Pi - omega  (meaning that the angle is such that
            //      a line L passing through (0,0) with that angle would intersect with the bottom
            //      of the area before intersecting with the right (or its symmetrical if alpha > Pi/2)
            //          then the distance that can be considered as 100% is D = Height / cos(omega)
            //      else
            //          D = Width/cos(omega)
            //
            //      We need the projection of one point on L (we'll get the percentage for the other
            //      point using directly the distance between the two points.
            //      Note: L and the line described by the points are parallel
            //      equation of L: Y = aX + b, where a is the same as the one for the points and b = 0.
            //          a = (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X)
            //      V1 vector from (0,0) to StartPoint --> V1 is basically Startpoint
            //      VUnit a unit vector on L --> VUnit is such that (1) X2 ^ 2 + Y2 ^ 2 = 1 and (2) Y2 = aX2
            //              replacing Y2 in (1): X2 ^ 2 + (aX2) ^ 2 = 1
            //                  => (1 + a^2) * X2 ^ 2 = 1
            //                  => X2 = sqrt(1/(1+a^2))
            //              replacing X2 in (2): Y2 = sqrt(a^2/1-a^2)

            //      now we need the dotproduct V1.VUnit to have the projection of the startPoint on L.
            //          dotProductStart = V1.X * VUnit.X + V1.Y * VUnit.Y

            //      The position of the projection of the StartPoint on L is :
            //          XpStart = dotProductStart * VUnit.X
            //          YpStart = dotProductStart * VUnit.Y

            //      now for the endPoint:
            //          dotProductEnd = EndPoint.X * VUnit.X + EndPoint.Y * VUnit.Y
            //      
            //      The position of the projection of the EndPoint on L is :
            //          XpEnd = dotProductEnd * VUnit.X
            //          YpEnd = dotProductEnd * VUnit.Y

            //      Now we get the percentages associated with D:
            //      StartPointPercentage = sqrt((XpStart ^ 2 + XpStart ^ 2)) / D * 100
            //      EndPointPercentage = sqrt((XpEnd ^ 2 + YpEnd ^ 2)) / D * 100


            // From these percentages, we can determine the offsets percentages:
            //  O is the offset (in percentage) we get from the GradientStop
            //  It is the percentage between StartPoint and EndPoint, so an offset of 0%
            //  is on StartPoint and an offset of 100% is on EndPoint
            //  We want to know the percentage relative to the whole element from that.
            //      Oabsolute = startPointPercentage + (endPointPercentage - startPointPercentage) * O;
            //                  offset due to start     percentage considering the total percentage of the start and end points.

            double XVariation = (endX - startX) * size.Width;
            double YVariation = (endY - startY) * size.Height;
            if (XVariation < 0)
            {
                // Note: these are to "fix" the value returned by the ArcTan (the result
                // is basically the same angle but in the opposite direction)
                XVariation = -XVariation; 
                YVariation = -YVariation;
            }

            // this will be the longest distance inside the shape for a segment with the alpha angle.
            double D; 

            // test: we make the percentages by projecting the points on the diagonal with an angle
            // that is perpendicular to the line defined by StartPoint and Endpoint:

            // we determine the equation of the line defined by StartPoint and EndPoint:
            // Y = mX + p
            // m = (Yend - Ystart) / (Xend - Xstart):
            double m = (endY - startY) / (endX - startX);
            // note: p does not matter since we only want the equation of the perpendicular and the
            // slope is all that matters for that.
            double startPointPercentage;
            double endPointPercentage;
            if (m != 0)
            {
                // perpendicular lines are such that m * mperp = -1 so mperp = -1 / m
                // the equation of the perpendicular lines is: Y = -1/m * X + k where k is to be
                // determined depending on the points.
                // the one passing through the Startpoint has kstart = Ystart + 1/m * Xstart
                double kStart = startY + 1 / m * startX;
                // same for EndPoint:
                double kEnd = endY + 1 / m * endX;

                // length of the diagonal = sqrt(width ^ 2 + height ^ 2) and we ignore the actual
                // size of the element since we want relative sizes anyway
                D = Math.Sqrt(2); 

                if (m > 0)
                //if (m != -1)
                {
                    // now we want the intersection between these lines and the diagonal (which has the equation Y = X since it goes from (0,0) to (1,1))
                    // IStart is the intersection of the perpendicular on the start point and the diagonal, with coordinates XIStart and yIstart.
                    // IStart is so that (1) XIStart = YIStart (belongs to the diagonal)
                    //                   (2) YIStart = -1/m * XIStart + kStart
                    //  We replace in (2): XIStart = -1/m * XIStart + kStart
                    //                      ==> XIStart * (1 + 1/m) = kStart
                    //                      ==> XIStart = kStart / (1 + 1/m)
                    //  YIStart = XIStart so no need for another variable.

                    double XIStart = kStart / (1 + 1 / m);
                    //same for the EndPoint:
                    double XIEnd = kEnd / (1 + 1 / m);

                    // now we get the percentage based on the distance of the IStart point to (0,0) and the size of the diagonal:
                    // Note: (2 * XIStart * XIStart) is basically (XIStart * XIStart + YIStart * YIStart)
                    // with XIStart = YIStart.
                    startPointPercentage = Math.Sqrt(2 * XIStart * XIStart) / D * 100;
                    // same for the EndPoint:
                    // Note: (2 * XIStart * XIStart) is basically (XIStart * XIStart + YIStart * YIStart)
                    // with XIStart = YIStart.
                    endPointPercentage = Math.Sqrt(2 * XIEnd * XIEnd) / D * 100;
                }
                else //m < 0 so we are going for the other diagonal
                {
                    //same as the case m > 0, except we want the projection on the other diagonal:
                    //diagonal's equation is Y = 1 - X
                    //(XIStart, YIStart) is so that (1) YIStart = 1 - XIStart
                    //                              YISTart = -1/m * XIStart + kStart ==> (2) YIStart = XIStart + kStart (m = -1)
                    // replacing in (2):
                    // 1 - XIStart = -1/m * XIStart + kStart
                    // 1 - kStart = (1 - 1/m) XIStart
                    // XIStart = (1-kStart) / (1 - 1/m)

                    double XIStart = (1 - kStart) / (1 - 1 / m);

                    //same for the end point:
                    double XIEnd = (1 - kEnd) / (1 - 1 / m);

                    //now we get the percentage based on the distance of the IStart point to (0,1) and the size of the diagonal:
                    //      startDistance = (XIStart * XIStart) + (YIStart - 1) * (YIStart - 1);
                    //  since (1), YIStart = 1 - XIStart:
                    //      (XIStart * XIStart) + (1 - XIStart - 1) * (1 - XIStart - 1)
                    //      => (XIStart * XIStart) + (- XIStart) * (- XIStart)
                    //  startDistance = sqrt(2 * XIStart * XIStart)
                    double startDistance = Math.Sqrt(2 * XIStart * XIStart);
                    startPointPercentage = startDistance / D * 100;

                    //same for end:
                    double endDistance = Math.Sqrt(2 * XIEnd * XIEnd);
                    endPointPercentage = endDistance / D * 100;
                }
            }
            else //no difference in height between the two points:
            {
                startPointPercentage = startX * 100; //shoud be divided by the width but since we consider it to be 1 anyway...
                endPointPercentage = endX * 100; //shoud be divided by the width but since we consider it to be 1 anyway...
            }

            //end of test.

            string stops = GetOffsetsString(startPointPercentage, endPointPercentage, "%");
            // Note: this is basically _angle in radians, and that takes into consideration
            // the possibility of width and height being different.
            double alpha = Math.Atan2(XVariation, YVariation) - Math.PI / 2;

            return (stops, alpha);
        }

        internal string ToHtmlString(DependencyObject parent)
        {
            List<GradientStop> stops = GradientStops.InternalItems;
            if (stops.Count == 0)
            {
                return string.Empty;
            }

            if (stops.Count == 1)
            {
                return stops[0].Color.ToHtmlString(Opacity);
            }

            // In that case, we want the whole thing to be of the color of the gradientStop
            // with the biggest offset:
            if (StartPoint == EndPoint) 
            {
                int max = 0;
                double maxOffset = stops[0].Offset;
                for (int i = 1; i < stops.Count; i++)
                {
                    GradientStop stop = stops[i];
                    double offset = stop.Offset;
                    if (offset > maxOffset)
                    {
                        max = i;
                        maxOffset = offset;
                    }
                }

                return stops[max].Color.ToHtmlString(Opacity);
            }

            (string stopsStr, double alpha) = MappingMode == BrushMappingMode.RelativeToBoundingBox ?
                ToHtmlStringForRelativeMappingMode(parent) :
                ToHtmlStringForAbsoluteMappingMode(parent);

            string gradientType = SpreadMethod == GradientSpreadMethod.Repeat ? "repeating-linear-gradient" : "linear-gradient";
            double angle = 360 - alpha * 180 / Math.PI + 90;
            return $"{gradientType}({angle.ToInvariantString()}deg, {stopsStr})";
        }

        private string GetOffsetsString(double start, double end, string unit)
        {
            Debug.Assert(GradientStops.Count > 0);

            Stop[] stops = GradientStops.GetSortedCollection();

            double distance = end - start;
            double minOffset = stops[0].Offset;
            Color minColor = stops[0].Color;
            double maxOffset = stops[stops.Length - 1].Offset;
            Color maxColor = stops[stops.Length - 1].Color;
            double opacity = Opacity;
            bool isFirst = true;

            StringBuilder sb = StringBuilderCache.Acquire();
            
            if (distance > 0)
            {
                if (minOffset != 0)
                {
                    sb.Append($"{minColor.ToHtmlString(opacity)} {start.ToInvariantString()}{unit}");
                    isFirst = false;
                }

                int index = 0;
                if (isFirst)
                {
                    Stop stop = stops[index];
                    sb.Append($"{stop.Color.ToHtmlString(opacity)} {(start + stop.Offset * distance).ToInvariantString()}{unit}");
                    index++;
                }

                for (; index < stops.Length; index++)
                {
                    Stop stop = stops[index];
                    sb.Append(',');
                    sb.Append($"{stop.Color.ToHtmlString(opacity)} {(start + stop.Offset * distance).ToInvariantString()}{unit}");
                }

                if (maxOffset != 1)
                {
                    sb.Append($", {maxColor.ToHtmlString(opacity)} {end.ToInvariantString()}{unit}");
                }
            }
            else
            {
                if (maxOffset != 1)
                {
                    sb.Append($"{maxColor.ToHtmlString(opacity)} {end.ToInvariantString()}{unit}");
                    isFirst = false;
                }

                int index = stops.Length - 1;
                if (isFirst)
                {
                    Stop stop = stops[index];
                    sb.Append($"{stop.Color.ToHtmlString(opacity)} {(start + stop.Offset * distance).ToInvariantString()}{unit}");
                    index--;
                }

                for (; index > -1; index--)
                {
                    Stop stop = stops[index];
                    sb.Append(',');
                    sb.Append($"{stop.Color.ToHtmlString(opacity)} {(start + stop.Offset * distance).ToInvariantString()}{unit}");
                }

                if (minOffset != 0)
                {
                    sb.Append($", {minColor.ToHtmlString(opacity)} {distance.ToInvariantString()}{unit}");
                }
            }

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private sealed class SvgLinearGradient : ISvgBrush
        {
            private readonly LinearGradientBrush _linearGradient;
            private readonly INTERNAL_HtmlDomElementReference _gradientRef;
            private readonly WeakEventListener<SvgLinearGradient, Brush, EventArgs> _transformChangedListener;

            public SvgLinearGradient(Shape shape, LinearGradientBrush lgb)
            {
                _linearGradient = lgb;
                _gradientRef = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(shape.DefsElement, "linearGradient");
                DrawLinearGradient();

                _transformChangedListener = new(this, lgb)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnTransformChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                lgb.TransformChanged += _transformChangedListener.OnEvent;
            }

            public string GetBrush(Shape shape) => $"url(#{_gradientRef.UniqueIdentifier})";

            public void RenderBrush() => DrawLinearGradient();

            public void DestroyBrush(Shape shape)
            {
                _transformChangedListener.Detach();
                INTERNAL_HtmlDomManager.RemoveNodeNative(_gradientRef);
            }

            private void OnTransformChanged(object sender, EventArgs e)
            {
                Transform transform = ((Brush)sender).Transform;

                if (transform is null || transform.IsIdentity)
                {
                    INTERNAL_HtmlDomManager.RemoveAttribute(_gradientRef, "gradientTransform");
                }
                else
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_gradientRef,
                        "gradientTransform",
                        MatrixTransform.MatrixToHtmlString(transform.Matrix));
                }
            }

            private void DrawLinearGradient()
            {
                Point start = _linearGradient.StartPoint;
                Point end = _linearGradient.EndPoint;
                string x1 = Math.Round(start.X, 2).ToInvariantString();
                string y1 = Math.Round(start.Y, 2).ToInvariantString();
                string x2 = Math.Round(end.X, 2).ToInvariantString();
                string y2 = Math.Round(end.Y, 2).ToInvariantString();
                string units = ConvertBrushMappingModeToString(_linearGradient.MappingMode);
                string spreadMethod = ConvertSpreadMethodToString(_linearGradient.SpreadMethod);
                string transform = _linearGradient.Transform is Transform t && !t.IsIdentity ?
                    MatrixTransform.MatrixToHtmlString(t.Matrix) : string.Empty;
                string opacity = Math.Round(_linearGradient.Opacity, 2).ToInvariantString();
                string stops = string.Join(",",
                    _linearGradient
                    .GetGradientStops()
                    .Select(s => $"{Math.Round(s.Offset, 2).ToInvariantString()},'{s.Color.ToHtmlString(1.0)}'"));

                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.drawSvgLinearGradient('{_gradientRef.UniqueIdentifier}',{x1},{y1},{x2},{y2},'{units}','{spreadMethod}','{transform}',{opacity},{stops})");
            }
        }
    }
}