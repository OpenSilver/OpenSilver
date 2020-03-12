

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
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{
    /// <summary>
    /// Draws a straight line between two points.
    /// </summary>
    public partial class Line : Shape
    {
        //internal dynamic canvasDomElement;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(this, parentRef, out _canvasDomElement, out domElementWhereToPlaceChildren);

            //domElementWhereToPlaceChildren = null;
            //var canvas = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("canvas", parentRef, this);
            //return canvas;
        }

        //protected internal override void INTERNAL_OnAttachedToVisualTree()
        //{
        //    ScheduleRedraw();
        //}

        // Returns:
        //     The x-coordinate for the start point of the line, in pixels. The default
        //     is 0.
        /// <summary>
        /// Gets or sets the x-coordinate of the Line start point.
        /// </summary>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(Line), new PropertyMetadata(0d, X1_Changed));
        private static void X1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line line = (Line)d;
            line.ScheduleRedraw();
        }

        // Returns:
        //     The x-coordinate for the end point of the line, in pixels. The default is
        //     0.
        /// <summary>
        /// Gets or sets the x-coordinate of the Line end point.
        /// </summary>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        /// <summary>
        /// Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(Line), new PropertyMetadata(0d, X2_Changed));
        private static void X2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line line = (Line)d;
            line.ScheduleRedraw();
        }

        // Returns:
        //     The y-coordinate for the start point of the line, in pixels. The default
        //     is 0.
        /// <summary>
        /// Gets or sets the y-coordinate of the Line start point.
        /// </summary>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(Line), new PropertyMetadata(0d, Y1_Changed));
        private static void Y1_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line line = (Line)d;
            line.ScheduleRedraw();
        }

        // Returns:
        //     The y-coordinate for the end point of the line, in pixels. The default is
        //     0.
        /// <summary>
        /// Gets or sets the y-coordinate of the Line end point.
        /// </summary>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(Line), new PropertyMetadata(0d, Y2_Changed));
        private static void Y2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line line = (Line)d;
            line.ScheduleRedraw();
        }

        internal void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            double maxAbs = X1 > X2 ? X1 : X2;
            double minAbs = X1 < X2 ? X1 : X2;
            double minOrd = Y1 < Y2 ? Y1 : Y2;
            double maxOrd = Y1 > Y2 ? Y1 : Y2;
            if (maxX < maxAbs)
            {
                maxX = maxAbs;
            }
            if (maxY < maxOrd)
            {
                maxY = maxOrd;
            }
            if (minX > minAbs)
            {
                minX = minAbs;
            }
            if (minY > minOrd)
            {
                minY = minOrd;
            }
        }

        override internal protected void Redraw()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                double minX = X1;
                double minY = Y1;
                double maxX = X2;
                double maxY = Y2;
                if (X1 > X2)
                {
                    minX = X2;
                    maxX = X1;
                }
                if (Y1 > Y2)
                {
                    minY = Y2;
                    maxY = Y1;
                }

                Size shapeActualSize;
                INTERNAL_ShapesDrawHelpers.PrepareStretch(this, _canvasDomElement, minX, maxX, minY, maxY, Stretch, out shapeActualSize);

                double horizontalMultiplicator;
                double verticalMultiplicator;
                double xOffsetToApplyBeforeMultiplication;
                double yOffsetToApplyBeforeMultiplication;
                double xOffsetToApplyAfterMultiplication;
                double yOffsetToApplyAfterMultiplication;
                INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(this, StrokeThickness, minX, maxX, minY, maxY, Stretch, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out _marginOffsets);

                ApplyMarginToFixNegativeCoordinates(new Point());

                if (Stretch == Stretch.None)
                {
                    ApplyMarginToFixNegativeCoordinates(_marginOffsets);
                }

                object context = CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", _canvasDomElement); //Note: we do not use INTERNAL_HtmlDomManager.Get2dCanvasContext here because we need to use the result in ExecuteJavaScript, which requires the value to come from a call of ExecuteJavaScript.

                //we remove the previous drawing:
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.clearRect(0,0, $1, $2)", context, shapeActualSize.Width, shapeActualSize.Height);


                double preparedX1 = (X1 + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication;
                double preparedX2 = (X2 + xOffsetToApplyBeforeMultiplication) * horizontalMultiplicator + xOffsetToApplyAfterMultiplication;
                double preparedY1 = (Y1 + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication;
                double preparedY2 = (Y2 + yOffsetToApplyBeforeMultiplication) * verticalMultiplicator + yOffsetToApplyAfterMultiplication;

                //todo: if possible, manage strokeStyle and lineWidth in their respective methods (Stroke_Changed and StrokeThickness_Changed) then use context.save() and context.restore() (can't get it to work yet).
                double opacity = Stroke == null ? 1 : Stroke.Opacity;
                object strokeValue = GetHtmlBrush(this, Stroke, opacity, minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);

                //we set the StrokeDashArray:
                if (strokeValue != null && StrokeThickness > 0)
                {
                    double thickness = StrokeThickness;
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.strokeStyle = $1", context, strokeValue);
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.lineWidth = $1", context, StrokeThickness);
                    if (StrokeDashArray != null)
                    {
                        if (CSHTML5.Interop.IsRunningInTheSimulator)
                        {
                            //todo: put a message saying that it doesn't work in certain browsers (maybe use a static boolean to put that message only once)
                        }
                        else
                        {
                            object options = CSHTML5.Interop.ExecuteJavaScript(@"new Array()");
                            for (int i = 0; i < StrokeDashArray.Count; ++i)
                            {
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = $2;
", options, i, StrokeDashArray[i] * thickness);
                            }

                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
if ($0.setLineDash)
    $0.setLineDash($1)", context, options);
                            //context.setLineDash(str + "]");
                        }
                    }
                }


                INTERNAL_ShapesDrawHelpers.PrepareLine(_canvasDomElement, new Point(preparedX1, preparedY1), new Point(preparedX2, preparedY2));

                if (strokeValue != null)
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.strokeStyle = $1", context, strokeValue);

                //context.strokeStyle = strokeAsString; //set the shape's lines color
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.lineWidth= $1", context, StrokeThickness.ToString());
                //context.lineWidth = StrokeThickness.ToString();
                if (Stroke != null && StrokeThickness > 0)
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.stroke()", context); //draw the line
                    //context.stroke(); //draw the line
                }
            }
        }

    }
}