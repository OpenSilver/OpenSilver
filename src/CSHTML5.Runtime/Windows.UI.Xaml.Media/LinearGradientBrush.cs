

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


using CSHTML5;
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
    /// Paints an area with a linear gradient.
    /// </summary>
    public sealed partial class LinearGradientBrush : GradientBrush, ICanConvertToCSSValues
    {
        public LinearGradientBrush() { }

        /// <summary>
        /// Initializes a new instance of the LinearGradientBrush class that has the
        /// specified GradientStopCollection and angle.
        /// </summary>
        /// <param name="gradientStopCollection">The GradientStops to set on this brush.</param>
        /// <param name="angle">
        /// A System.Double that represents the angle, in degrees, of the gradient. A
        /// value of 0 creates a horizontal gradient, and a value of 90 creates a vertical
        /// gradient. Negative values are permitted, as are values over 360 (which are
        /// treated as mod 360).
        /// </param>
        public LinearGradientBrush(GradientStopCollection gradientStopCollection, double angle)
        {
            GradientStops = gradientStopCollection;

            _angle = -angle;
        }

        private double _angle; //degrees

        /// <summary>
        /// Gets or sets the ending two-dimensional coordinates of the linear gradient.
        /// </summary>
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }
        /// <summary>
        /// Identifies the EndPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(LinearGradientBrush), new PropertyMetadata(new Point(1, 1), EndPoint_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void EndPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LinearGradientBrush)d).RecalculateAngle();
        }


        /// <summary>
        /// Gets or sets the starting two-dimensional coordinates of the linear gradient.
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }
        /// <summary>
        /// Identifies the StartPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(LinearGradientBrush), new PropertyMetadata(new Point(), StartPoint_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void StartPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LinearGradientBrush)d).RecalculateAngle();
        }

        private void RecalculateAngle()
        {
            _angle = Math.Atan2(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y) * 180.0 / Math.PI - 90.0; //todo: should that not take into consideration the size of the element (meaning that if it is twice as wide than it is high, the angle will be different)?
        }

        void ToHtmlStringForAbsoluteMappingMode(DependencyObject parent, out string gradientStopsString, out double alpha)
        {
            //Note: for a more detailed explanation on the maths in this method, see the non absolute mapping mode version.

            double startDistance;
            double endDistance;

            if (EndPoint.X == StartPoint.X)
            {
                //vertical line: distance = their Y
                startDistance = StartPoint.Y;
                endDistance = EndPoint.Y;
                //setting alpha:
                alpha = -Math.PI / 2;
            }
            else if (EndPoint.Y == StartPoint.Y)
            {
                //horizontal line: distance = their X

                startDistance = StartPoint.X;
                endDistance = EndPoint.X;
                alpha = 0;
            }
            else
            {
                double slope = (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X); //this is the slope of the line L defined by StartPoint and EndPoint
                double perpSlope = -1 / slope; //this is the slope of any line perpendicular to L

                double kStart = StartPoint.Y + 1 / slope * StartPoint.X; //this is k so that Y = perpSlope X + k is the line perpendicular P1 to L intersecting on StartPoint
                double kEnd = EndPoint.Y + 1 / slope * EndPoint.X;//same for EndPoint (P2)


                if (slope > 0)
                {
                    //now we want the intersections between P1 and P2 and Y = slope * X
                    //slope * X  = perpSlope * X + k 
                    //  --> X = k / (slope - perpslope)

                    double XIStart = kStart / (slope - perpSlope); //this is basically X of the projection of StartPoint on the parallel to L, that passes through (0,0)
                    double YIStart = slope * XIStart; //this is basically Y of the projection of StartPoint on the parallel to L, that passes through (0,0)

                    double XIEnd = kEnd / (slope - perpSlope);
                    double YIEnd = slope * XIEnd;

                    //now we get the distance between those points and (0,0) and we have their distances:
                    startDistance = Math.Sqrt(Math.Pow(XIStart, 2) + Math.Pow(YIStart, 2));
                    endDistance = Math.Sqrt(Math.Pow(XIEnd, 2) + Math.Pow(YIEnd, 2));
                }
                else
                {
                    //now we want the intersections between P1 and P2 and Y = height - slope * X

                    double height = 1; //what do we do when parent is not a FrameworkElement ?
                    if (!(parent == null) && parent is FrameworkElement)
                    {
                        FrameworkElement parentAsFrameworkElement = (FrameworkElement)parent;
                        height = parentAsFrameworkElement.ActualHeight;
                    }

                    double XIStart = (height - kStart) / (1 - 1 / slope);
                    double YIStart = height - slope * XIStart; //this is basically Y of the projection of StartPoint on the parallel to L, that passes through (0,0)

                    //same for the end point:
                    double XIEnd = (height - kEnd) / (1 - 1 / slope);
                    double YIEnd = height - slope * XIEnd;


                    //now we get the distance between those points and (width,0) or (0,height) (I don't know which one) and we have their distances:
                    startDistance = Math.Sqrt(Math.Pow(XIStart, 2) + Math.Pow(height - YIStart, 2));
                    endDistance = Math.Sqrt(Math.Pow(XIEnd, 2) + Math.Pow(height - YIEnd, 2));
                }

                //setting alpha:
                double XVariation = (EndPoint.X - StartPoint.X);
                double YVariation = (EndPoint.Y - StartPoint.Y);
                if (XVariation < 0)
                {
                    XVariation = -XVariation; //Note: these are to "fix" the value returned by the ArcTan (the result is basically the same angle but in the opposite direction)
                    YVariation = -YVariation;
                }
                alpha = Math.Atan2(XVariation, YVariation) - Math.PI / 2;
            }
            double startToEndDistance = endDistance - startDistance;
            gradientStopsString = GetOffsetsString(startDistance, endDistance, startToEndDistance, "px");
        }

        void ToHtmlStringForRelativeMappingMode(DependencyObject parent, out string gradientStopsString, out double alpha)
        {
            double height = 1; //Note: 1 is ok since it is the ratio that is important
            double width = 1;
            if (!(parent == null) && parent is FrameworkElement)
            {
                FrameworkElement parentAsFrameworkElement = (FrameworkElement)parent;
                Size actualSize = parentAsFrameworkElement.INTERNAL_GetActualWidthAndHeight();
                width = actualSize.Width;
                height = actualSize.Height;
            }

            double startX = StartPoint.X;
            double startY = StartPoint.Y;
            double endX = EndPoint.X;
            double endY = EndPoint.Y;
            //first we get startpoint.X, Y and endPoint.X, Y are in relative coordinates (because we made it that way):

            //Note: For the linear GradientBrush to work properly, we need to take into consideration the positions of the StartPoint and EndPoint not only for the angle, but the offsets are depending on it :
            //      Ex: StartPoint at (0,0.5), EndPoint at (1,1), a gradientStop with an offset of 0 will be at the (0,0.5) position, which is not what we get when putting 0%.
            //We need to get the percentages of the StartPoint and EndPoint positions depending on the angle and then determine the percentages of the offsets using them.
            //      Ex: if StartPoint is at percentage P1 and EndPoint is at percentage P2, then Offset with O1 will be at percentage P1 + (P2-P1)*O1 (note that O1 is not a percentage but a number usually between 0 and 1).

            //To know P1 and P2:
            //  - omega is the angle we would get with points (0,0) and (1,1)
            //  - alpha is the angle with the current points
            //
            //      if omega < alpha modulo Pi < Pi - omega  (meaning that the angle is such that a line L passing through (0,0) with that angle would intersect with the bottom of the area before intersecting with the right (or its symmetrical if alpha > Pi/2)
            //          then the distance that can be considered as 100% is D = Height / cos(omega)
            //      else
            //          D = Width/cos(omega)
            //
            //      We need the projection of one point on L (we'll get the percentage for the other point using directly the distance between the two points.
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
            //  It is the percentage between StartPoint and EndPoint, so an offset of 0% is on StartPoint and an offset of 100% is on EndPoint
            //  We want to know the percentage relative to the whole element from that.
            //      Oabsolute = startPointPercentage + (endPointPercentage - startPointPercentage) * O;
            //                  offset due to start     percentage considering the total percentage of the start and end points.

            //double omega = Math.Abs(Math.Atan2(height, width)); //omega represents the angle that goes to 2 opposite corners of the element.
            double XVariation = (endX - startX) * width;
            double YVariation = (endY - startY) * height;
            if (XVariation < 0)
            {
                XVariation = -XVariation; //Note: these are to "fix" the value returned by the ArcTan (the result is basically the same angle but in the opposite direction)
                YVariation = -YVariation;
            }

            alpha = Math.Atan2(XVariation, YVariation) - Math.PI / 2; //Note: this is basically _angle in radians, and that takes into consideration the possibility of width and height being different.

            double D; //this will be the longest distance inside the shape for a segment with the alpha angle.

            //test: we make the percentages by projecting the points on the diagonal with an angle that is perpendicular to the line defined by StartPoint and Endpoint:

            //we determine the equation of the line defined by StartPoint and EndPoint:
            // Y = mX + p
            // m = (Yend - Ystart) / (Xend - Xstart):
            double m = (endY - startY) / (endX - startX);
            //note: p does not matter since we only want the equation of the perpendicular and the slope is all that matters for that.
            double startPointPercentage;
            double endPointPercentage;
            if (m != 0)
            {
                //perpendicular lines are such that m * mperp = -1 so mperp = -1 / m
                //the equation of the perpendicular lines is: Y = -1/m * X + k where k is to be determined depending on the points.
                //the one passing through the Startpoint has kstart = Ystart + 1/m * Xstart
                double kStart = startY + 1 / m * startX;
                //same for EndPoint:
                double kEnd = endY + 1 / m * endX;

                D = Math.Sqrt(2); //length of the diagonal = sqrt(width ^ 2 + height ^ 2) and we ignore the actual size of the element since we want relative sizes anyway

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


                    //now we get the percentage based on the distance of the IStart point to (0,0) and the size of the diagonal:
                    startPointPercentage = Math.Sqrt(2 * XIStart * XIStart) / D * 100; //Note: (2 * XIStart * XIStart) is basically (XIStart * XIStart + YIStart * YIStart) with XIStart = YIStart.
                    //same for the EndPoint:
                    endPointPercentage = Math.Sqrt(2 * XIEnd * XIEnd) / D * 100; //Note: (2 * XIStart * XIStart) is basically (XIStart * XIStart + YIStart * YIStart) with XIStart = YIStart.
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




            double startToEndPercentage = endPointPercentage - startPointPercentage; //this is only to avoid making this calculation for each gradientStop.


            gradientStopsString = GetOffsetsString(startPointPercentage, endPointPercentage, startToEndPercentage, "%");
        }

        internal List<object> INTERNAL_ToHtmlString(DependencyObject parent)
        {

            double alpha;
            string gradientStopsString;
            if (StartPoint == EndPoint) //in that case, we want the whole thing to be of the color of the gradientStop with the biggest offset:
            {
                string color = null;
                double biggestOffset = 0;
                foreach (GradientStop gradientStop in GradientStops)
                {
                    if (gradientStop.Offset > biggestOffset)
                    {
                        biggestOffset = gradientStop.Offset;
                        color = gradientStop.Color.INTERNAL_ToHtmlString(this.Opacity);
                    }
                }
                gradientStopsString = color + " 0%, " + color + " 100%";
                alpha = 0;
            }
            else
            {
                if (MappingMode == BrushMappingMode.RelativeToBoundingBox)
                {
                    ToHtmlStringForRelativeMappingMode(parent, out gradientStopsString, out alpha);
                }
                else
                {
                    ToHtmlStringForAbsoluteMappingMode(parent, out gradientStopsString, out alpha);
                }
            }

            double angle = alpha * 180 / Math.PI;


            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
            {

                //in IE and FireFox, it uses an angle in the opposite direction of that of the other browsers and 0deg is up while in other browsers it is right (or something like that).
                if (Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("(window.IE_VERSION || window.FIREFOX_VERSION)")))
                {
                    angle = 360 - angle + 90;
                }

            }


            string gradientType = this.SpreadMethod == GradientSpreadMethod.Repeat ? "repeating-linear-gradient" : "linear-gradient";
            string baseString = gradientType + "(" + ((double)angle).ToString() + "deg, " + gradientStopsString + ")"; //todo: we cast the angle to "int" to avoid issues with the decimal separator that must be "." instead of ",". When the InvariantCulture will be supported, do not cast to int and, instead, use ToString(InvariantCulture).
            List<object> returnValues = new List<object>();
            returnValues.Add("-webkit-" + baseString);
            returnValues.Add("-o-" + baseString);
            returnValues.Add("-moz-" + baseString);
            returnValues.Add(baseString);
            return returnValues;
        }

        private string GetOffsetsString(double startPointPercentage, double endPointPercentage, double startToEndPercentage, string percentageSymbol)
        {
            string gradientStopsString = "";
            bool isFirst = true;
            //Note: the offsets need to be in ascending order to work properly in html, therefore, we put them in order.
            List<Tuple<double, string>> tempList = new List<Tuple<double, string>>();

            // Note: the 4 variables defined below are required to mae sure we repeat the correct area in repeating gradien brushes (see the comment above "if (smallestOffset != 0)").
            double smallestOffset = double.MaxValue;
            string smallestColor = "";
            double biggestOffset = double.MinValue;
            string biggestColor = "";

            foreach (GradientStop gradientStop in GradientStops)
            {
                string currentColor = gradientStop.Color.INTERNAL_ToHtmlString(this.Opacity);//todo-perfs: every time, accessing the "Opacity" property may be slow.
                string str = currentColor + " " + (startPointPercentage + gradientStop.Offset * startToEndPercentage).ToString() + percentageSymbol;
                tempList.Add(new Tuple<double, string>(gradientStop.Offset, str));
                if (smallestOffset > gradientStop.Offset)
                {
                    smallestOffset = gradientStop.Offset;
                    smallestColor = currentColor;
                }
                if (biggestOffset <= gradientStop.Offset)
                {
                    biggestOffset = gradientStop.Offset;
                    biggestColor = currentColor;
                }
            }

            //Note: the expected behaviour in repeating gradient brushes is to repeat what is between startPoint and endPoint.
            //      in the browsers, they repeat what is between the first offset (percentage) to the last, so we need one for offset 0 and offset 1
            //      This is why we have the test below (and its content):
            if (startToEndPercentage > 0)
            {
                if (smallestOffset != 0)
                {
                    gradientStopsString = smallestColor + " " + startPointPercentage.ToString() + percentageSymbol;
                    isFirst = false;
                }
            }
            else //the end percentage is smaller than the start one so we want the biggest offset for the start and the smallest at the end.
            {
                if (biggestOffset != 1)
                {
                    gradientStopsString = biggestColor + " " + endPointPercentage.ToString() + percentageSymbol;
                    isFirst = false;
                }
            }

            //todo-perf: it might be faster to sort the list then read it in order instead of going through it multiple times but I don't think it would do a big difference (we are talking about offsets in a LinearGradientBrush after all...)
            while (tempList.Count > 0)
            {
                if (!isFirst)
                {
                    gradientStopsString += ", ";
                }

                int indexOfMin = 0;
                double currentMin = tempList.ElementAt(0).Item1;

                for (int i = 1; i < tempList.Count; ++i)
                {
                    if (startToEndPercentage > 0)
                    {
                        if (tempList.ElementAt(i).Item1 < currentMin)
                        {
                            indexOfMin = i;
                            currentMin = tempList.ElementAt(i).Item1;
                        }
                    }
                    else //we are going backwards.
                    {
                        if (tempList.ElementAt(i).Item1 > currentMin)
                        {
                            indexOfMin = i;
                            currentMin = tempList.ElementAt(i).Item1;
                        }
                    }
                }

                gradientStopsString += tempList.ElementAt(indexOfMin).Item2;
                tempList.RemoveAt(indexOfMin);
                isFirst = false;
            }

            //same as mentionned above, this is required to repeat the correct area:
            if (startToEndPercentage > 0)
            {
                if (biggestOffset != 1)
                {
                    gradientStopsString += ", " + biggestColor + " " + endPointPercentage.ToString() + percentageSymbol;
                }
            }
            else //same as before, we want the smallest offset at the last position.
            {
                if (smallestOffset != 0)
                {
                    gradientStopsString += ", " + smallestColor + " " + startToEndPercentage.ToString() + percentageSymbol;
                }
            }
            return gradientStopsString;
        }

        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            return (List<object>)INTERNAL_ToHtmlString(parent);
        }
    }
}