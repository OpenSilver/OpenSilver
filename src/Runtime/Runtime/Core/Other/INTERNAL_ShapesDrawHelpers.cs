

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


using OpenSilver.Internal;
using System;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_ShapesDrawHelpers
    {
        internal static object CreateDomElementForPathAndSimilar(UIElement associatedUIElement, object parentRef, out object canvasDomElement, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, associatedUIElement);
            var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            //divStyle.overflow = "hidden";
            divStyle.lineHeight = "0"; // Line height is not needed in shapes because it causes layout issues.
            //divStyle.width = "100%";
            //divStyle.height = "100%";
            divStyle.fontSize = "0px"; //this allows this div to be as small as we want (for some reason in Firefox, what contains a canvas has a height of at least about (1 + 1/3) * fontSize)
            canvasDomElement = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("canvas", div, associatedUIElement);
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(canvasDomElement);
            style.width = "0px";
            style.height = "0px";
            return div;
        }


        //INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", sizeToApply.Height);

        /// <summary>
        /// Prepares the Shape so that its canvas has the size it should have, depending on its container, content and Stretch mode.
        /// </summary>
        /// <param name="shape">The Shape containing the canvas.</param>
        /// <param name="canvasDomElement">The canvas in the Shape.</param>
        /// <param name="minX">The minimum X-coordinate of the points in the Shape.</param>
        /// <param name="maxX">The maximum X-coordinate of the points in the Shape.</param>
        /// <param name="minY">The minimum Y-coordinate of the points in the Shape.</param>
        /// <param name="maxY">The maximum Y-coordinate of the points in the Shape.</param>
        /// <param name="stretch">The Stretch mode to apply on the Shape</param>
        /// <param name="shapeActualSize"></param>
        internal static void PrepareStretch(Shape shape, object canvasDomElement, double minX, double maxX, double minY, double maxY, Stretch stretch, out Size shapeActualSize)
        {
            var canvasStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(canvasDomElement);

            //Two steps:
            // 1) We get the size the Shape would take if Stretch = None so that we know its prefered size
            // 2) We make the element take the allowed size closest to that prefered size.

            //We get the size the Shape would take if its Stretch mode was Stretch.None:
            //in Stretch.None mode, the size is from 0 to the rightmost poit and bottommost point:
            double sizeX = maxX;
            double sizeY = maxY;

            //in other Stretched modes, the size is from the leftmost point to the rightmost point and from the topmost to the bottommost point:
            if (stretch != Stretch.None)
            {
                sizeX = maxX - minX;
                sizeY = maxY - minY;
            }
            else //we want to add the size of the shape that goes into negative values:
            {
                if (minX < 0)
                {
                    sizeX = maxX - minX;
                }
                if (minY < 0)
                {
                    sizeY = maxY - minY;
                }
            }

            double offset = shape.StrokeThickness > 0 ? shape.StrokeThickness : 1;
            //todo: (?) for the line below, replace the "+ 1" with the StrokeThickness/LineWidth
            Size sizeToApply = new Size(Math.Max(sizeX + offset, 0), Math.Max(sizeY + offset, 0)); //example: a vertical line still needs 1 pixel width

            //we apply the possible defined size of the outerDomElement of the shape:
            var style = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(shape);
            bool frameworkElementWidthWasSpecified = false;
            double frameworkElementWidth = shape.Width;
            double frameworkElementHeight = shape.Height;
            if (!double.IsNaN(frameworkElementWidth))
            {
                frameworkElementWidthWasSpecified = true;
                sizeToApply.Width = frameworkElementWidth + offset;
                style.width = sizeToApply.Width.ToInvariantString() + "px";
            }
            bool frameworkElementHeightWasSpecified = false;
            if (!double.IsNaN(frameworkElementHeight))
            {
                frameworkElementHeightWasSpecified = true;
                sizeToApply.Height = frameworkElementHeight + offset;
                style.height = sizeToApply.Height.ToInvariantString() + "px";
            }

            if (frameworkElementWidthWasSpecified && frameworkElementHeightWasSpecified)
            {
                shapeActualSize = new Size(frameworkElementWidth, frameworkElementHeight);

                //We apply the size defined earlier (size either by the Width and/or Height of the Shape (when they are set) or by the width and/or height of its content):
                INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "width", sizeToApply.Width);
                INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", sizeToApply.Height);

                canvasStyle.width = sizeToApply.Width.ToInvariantString() + "px"; // The "sizeToApply" is the size that the shape would take if it was not constrained by the parent framework element.
                canvasStyle.height = sizeToApply.Height.ToInvariantString() + "px";
            }
            else
            {
                if (shape.IsUnderCustomLayout == false)
                {
                    //We apply the size defined earlier (size either by the Width and/or Height of the Shape (when they are set) or by the width and/or height of its content):
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "width", sizeToApply.Width);
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", sizeToApply.Height);

                    canvasStyle.width = sizeToApply.Width.ToInvariantString() + "px"; // The "sizeToApply" is the size that the shape would take if it was not constrained by the parent framework element.
                    canvasStyle.height = sizeToApply.Height.ToInvariantString() + "px";

                    shapeActualSize = shape.INTERNAL_GetActualWidthAndHeight(); //Note: in case that the framework element is constrained, it won't take the size of its canvas2d content, so we then resize the canvas2d content so that the shape stretches.

                    if (!frameworkElementWidthWasSpecified)
                    {
                        canvasStyle.width = shapeActualSize.Width.ToInvariantString() + "px";
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "width", shapeActualSize.Width + offset);
                    }
                    if (!frameworkElementHeightWasSpecified)
                    {
                        canvasStyle.height = shapeActualSize.Height.ToInvariantString() + "px";
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", shapeActualSize.Height + offset);
                    }
                }
                else
                {
                    shapeActualSize = shape.VisualBounds.Size;

                    if (!frameworkElementWidthWasSpecified)
                    {
                        canvasStyle.width = shapeActualSize.Width.ToInvariantString() + "px";
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "width", shapeActualSize.Width + offset);
                    }
                    else
                    {
                        shapeActualSize.Width = frameworkElementWidth;
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "width", sizeToApply.Width);
                        canvasStyle.width = sizeToApply.Width.ToInvariantString() + "px";
                    }

                    if (!frameworkElementHeightWasSpecified)
                    {
                        canvasStyle.height = shapeActualSize.Height.ToInvariantString() + "px";
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", shapeActualSize.Height + offset);
                    }
                    else
                    {
                        shapeActualSize.Height = frameworkElementHeight;
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(canvasDomElement, "height", sizeToApply.Height);
                        canvasStyle.height = sizeToApply.Height.ToInvariantString() + "px";
                    }
                }
            }

            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            context.translate(0.5, 0.5); //makes is less blurry for some reason.
        }

        /// <summary>
        /// Gets the multiplicators and offsets to fit the Stretch value.
        /// </summary>
        /// <param name="frameworkElement">The element on which we make the stretch</param>
        /// <param name="strokeThickness">The thickness of the stroke</param>
        /// <param name="minX">the minimal value on the horizontal axis as defined by the user</param>
        /// <param name="maxX">the maximal value on the horizontal axis as defined by the user</param>
        /// <param name="minY">the minimal value on the vertical axis as defined by the user</param>
        /// <param name="maxY">the minimal value on the vertical axis as defined by the user</param>
        /// <param name="stretch">the Stretch value to apply</param>
        /// <param name="horizontalMultiplicator">the resulting value by which we have to multiply the horizontal coordinates to fit the Stretch value</param>
        /// <param name="verticalMultiplicator">the resulting value by which we have to multiply the vertical coordinates to fit the Stretch value</param>
        /// <param name="xOffsetToApplyBeforeMultiplication">the offset to apply on horizontal coordinates before applying multiplicators</param>
        /// <param name="yOffsetToApplyBeforeMultiplication">the offset to apply on vertical coordinates before applying multiplicators</param>
        /// <param name="xOffsetToApplyAfterMultiplication">the offset to apply on horizontal coordinates after applying multiplicators</param>
        /// <param name="yOffsetToApplyAfterMultiplication">the offset to apply on vertical coordinates after applying multiplicators</param>
        /// <param name="marginOffsets">the offset to apply on the Left margin of the element to compensate for the negative coordinates in the path</param>
        internal static void GetMultiplicatorsAndOffsetForStretch(FrameworkElement frameworkElement, double strokeThickness, double minX, double maxX, double minY, double maxY, Stretch stretch, Size shapeActualSize, out double horizontalMultiplicator, out double verticalMultiplicator, out double xOffsetToApplyBeforeMultiplication, out double yOffsetToApplyBeforeMultiplication, out double xOffsetToApplyAfterMultiplication, out double yOffsetToApplyAfterMultiplication, out Point marginOffsets)
        {
            //note: in winRT, the stroke thickness is included in the width of the figure --> the stroke is put inwards. In javascript, the thickness is put around the original line
            //the note above means that when we give a size for an element, changing the StrokeThickness will not change the size it takes.
            //to reproduce the winRT version, we need to:
            //  - apply an offset on the figure, equal to half the strokeThickness
            //  - change the multiplicators so that the figure takes the size of the canvas minus the strokeThickness in width and height.

            horizontalMultiplicator = 1d;
            verticalMultiplicator = 1d;
            xOffsetToApplyBeforeMultiplication = 0d;
            yOffsetToApplyBeforeMultiplication = 0d;
            xOffsetToApplyAfterMultiplication = 0d;
            yOffsetToApplyAfterMultiplication = 0d;
            if (shapeActualSize.Width > 0 && shapeActualSize.Height > 0)
            {
                //we get the size the shape would take if Stretch = None:
                double nonStretchedWidthOfThePath = maxX - minX;
                double nonStretchedHeightOfThePath = maxY - minY;
                if (nonStretchedWidthOfThePath == 0)
                {
                    nonStretchedWidthOfThePath = 1;
                }
                if (nonStretchedHeightOfThePath == 0)
                {
                    nonStretchedHeightOfThePath = 1;
                }

                if (stretch != Stretch.None)
                {
                    //we calculate by how much we need to multiply our Shape's size to fill the container in both directions:
                    horizontalMultiplicator = (shapeActualSize.Width - strokeThickness) / nonStretchedWidthOfThePath; //Note: the - strokethickness here is to make the shape dit in the container INCLUDING its stroke around it
                    verticalMultiplicator = (shapeActualSize.Height - strokeThickness) / nonStretchedHeightOfThePath; // Note: same as above.


                    if (stretch == Stretch.Uniform) //the Shape needs to grow/shrink uniformally until it exactly fits in the container
                    {
                        //we select the smallest multiplicator, which we will apply on both directions:
                        if (horizontalMultiplicator > verticalMultiplicator)
                        {
                            horizontalMultiplicator = verticalMultiplicator;
                        }
                        else
                        {
                            verticalMultiplicator = horizontalMultiplicator;
                        }
                    }
                    else if (stretch == Stretch.UniformToFill) //the Shape needs to grow/shrink uniformally until it exactly fills the container
                    {
                        //we select the biggest multiplicator, which we will apply on both directions:
                        if (horizontalMultiplicator < verticalMultiplicator)
                        {
                            horizontalMultiplicator = verticalMultiplicator;
                        }
                        else
                        {
                            verticalMultiplicator = horizontalMultiplicator;
                        }
                    }
                    //We compensate the size of the Stroke on the top and left side of the Shape because in the canvas, the stroke goes on both sides of the line while in WinRT it goes towards the inside of the Shape:
                    //Points in the Lines at the 0 coordinates (in X or in Y) will therefore be completely drawn instead of only half drawn. 
                    if (strokeThickness > 1)
                    {
                        xOffsetToApplyBeforeMultiplication = -minX;
                        yOffsetToApplyBeforeMultiplication = -minY;
                        xOffsetToApplyAfterMultiplication = strokeThickness / 2 - 0.5; //not sure what the -0.5 is for here but it is probably for antialiasing purposes?
                        yOffsetToApplyAfterMultiplication = strokeThickness / 2 - 0.5;
                    }
                    else
                    {
                        xOffsetToApplyBeforeMultiplication = -minX;
                        yOffsetToApplyBeforeMultiplication = -minY;
                        xOffsetToApplyAfterMultiplication = 0;
                        yOffsetToApplyAfterMultiplication = 0;

                    }
                    marginOffsets = new Point();
                }
                else
                {
                    double Xnormalization = minX < 0 ? -minX : 0d;
                    double Ynormalization = minY < 0 ? -minY : 0d;
                    marginOffsets = new Point(-Xnormalization, -Ynormalization);

                    xOffsetToApplyBeforeMultiplication = Xnormalization;
                    yOffsetToApplyBeforeMultiplication = Ynormalization;
                }
            }
            else
            {
                marginOffsets = new Point();
            }
        }

        internal static void PrepareLine(object canvasDomElement, Point StartPoint, Point EndPoint)
        {
            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            context.beginPath(); //this allows to state that we are drawing a new shape (not sure what it serves to but it is apparently good practice to always use it)
            context.moveTo(StartPoint.X, StartPoint.Y); //starting point of the line
            context.lineTo(EndPoint.X, EndPoint.Y); // tell the context that there should be a line from the starting point to this point



            //not set in this method:
            //context.strokeStyle = strokeAsString; //set the shape's lines color
            //context.lineWidth = path.StrokeThickness;
        }

        internal static void PrepareLines(object canvasDomElement, PointCollection points, double strokeThikness, bool isClosed)
        {
            if (points?.Count < 2) return;

            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            context.beginPath();
            context.moveTo(points[0].X, points[0].Y);

            for (int i = 1; i < points.Count; i++)
            {
                context.lineTo(points[i].X, points[i].Y);
            }

            if (isClosed)
            {
                context.closePath();
                context.fill();
            }
        }

        internal static void PrepareEllipse(object canvasDomElement, double ellipseWidth, double ellipseHeight, double centerX, double centerY)
        {
            var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
            //todo: StrokeThickness --> ?


            //solutions below have been found at http://stackoverflow.com/questions/2172798/how-to-draw-an-oval-in-html5-canvas
            //todo: check which version is the most effective (note, there are other implementation possibilities in the link above so we might check with those too)
            //todo: once todo above done, apply result on EllipseGeometry class too

            #region using bezier's curves

            //JAVASCRIPT
            //function drawEllipseWithBezier(ctx, x, y, w, h, style) {
            //    var kappa = .5522848,
            //        ox = (w / 2) * kappa, // control point offset horizontal
            //        oy = (h / 2) * kappa, // control point offset vertical
            //        xe = x + w,           // x-end
            //        ye = y + h,           // y-end
            //        xm = x + w / 2,       // x-middle
            //        ym = y + h / 2;       // y-middle

            //    ctx.save();
            //    ctx.beginPath();
            //    ctx.moveTo(x, ym);
            //    ctx.bezierCurveTo(x, ym - oy, xm - ox, y, xm, y);
            //    ctx.bezierCurveTo(xm + ox, y, xe, ym - oy, xe, ym);
            //    ctx.bezierCurveTo(xe, ym + oy, xm + ox, ye, xm, ye);
            //    ctx.bezierCurveTo(xm - ox, ye, x, ym + oy, x, ym);
            //    if(style)
            //      ctx.strokeStyle=style;
            //    ctx.stroke();
            //    ctx.restore();
            //  }
            //END OF JAVASCRIPT


            //    ctx.bezierCurveTo(x, y + h / 2 - oy, x + w / 2 - ox, y, x + w / 2, y);
            //    ctx.bezierCurveTo(x + w / 2 + ox, y, xe, y + h / 2 - oy, xe, y + h / 2);
            //    ctx.bezierCurveTo(xe, y + h / 2 + oy, x + w / 2 + ox, ye, x + w / 2, ye);
            //    ctx.bezierCurveTo(x + w / 2 - ox, ye, x, y + h / 2 + oy, x, y + h / 2);

            double kappa = .5522848;
            double ox = (ellipseWidth / 2) * kappa; // control point offset horizontal
            double oy = (ellipseHeight / 2) * kappa; // control point offset vertical

            Point topPoint = new Point(centerX, centerY - ellipseHeight / 2);
            Point leftPoint = new Point(centerX - ellipseWidth / 2, centerY);
            Point rightPoint = new Point(centerX + ellipseWidth / 2, centerY);
            Point bottomPoint = new Point(centerX, centerY + ellipseHeight / 2); //the "+ 1" are so that the ellipse is not cut on the edges. 

            context.beginPath();
            context.moveTo(leftPoint.X, leftPoint.Y); //start on the leftmost point of the ellipse
            context.bezierCurveTo(centerX - ellipseWidth / 2, centerY - oy,
                centerX - ox, centerY - ellipseHeight / 2,
                topPoint.X, topPoint.Y); //bezier to the topmost point of the ellipse
            context.bezierCurveTo(centerX + ox, centerY - ellipseHeight / 2,
                centerX + ellipseWidth / 2, centerY - oy,
                rightPoint.X, rightPoint.Y); //bezier to the rightmost point of the ellipse
            context.bezierCurveTo(centerX + ellipseWidth / 2, centerY + oy,
                centerX + ox, centerY + ellipseHeight / 2,
                bottomPoint.X, bottomPoint.Y); //bezier to the bottommost point of the ellipse
            context.bezierCurveTo(centerX - ox, centerY + ellipseHeight / 2,
                centerX - ellipseWidth / 2, centerY + oy,
                leftPoint.X, leftPoint.Y); //bezier to the first point (leftmost point of the ellipse)
            #endregion

            #region using arc and scale

            //JAVASCRIPT
            //  // cx,cy - center, r - horizontal radius X
            //function drawEllipseWithArcAndScale(ctx, cx, cy, rx, ry, style) {
            //  ctx.save(); // save state
            //  ctx.beginPath();
            //  ctx.translate(cx-rx, cy-ry);
            //  ctx.scale(rx, ry);
            //  ctx.arc(1, 1, 1, 0, 2 * Math.PI, false);
            //  ctx.restore(); // restore to original state
            //  ctx.save();
            //  if(style)
            //    ctx.strokeStyle=style;
            //  ctx.stroke();
            //  ctx.restore();
            //}
            //END OF JAVASCRIPT


            //context.save(); // save state
            //context.beginPath();
            //context.scale(actualWidth / 2, actualHeight / 2); ///2 because in context.arc, we put 1 as the radius, so 2 as the perimeter --> 2 as the initial width
            //context.arc(1, 1, 1, 0, 2 * Math.PI, false);
            //context.restore(); // restore to original state
            //context.save();
            //context.strokeStyle = strokeAsString;
            //context.lineWidth = StrokeThickness;
            //context.restore();
            //context.stroke();
            #endregion

        }

        internal static void ApplyTransformToCanvas(Transform transform, object canvasDomElement)
        {
            MatrixTransform matrixTransform = transform as MatrixTransform;
            if (matrixTransform != null)
            {
                Matrix m = matrixTransform.Matrix;

                // todo: this line gets called AFTER "translate(0.5, 0.5)" (in "INTERNAL_ShapesDrawHelpers.cs"), 
                // but instead we should apply it BEFORE, otherwise the (0.5, 0.5) translation may get amplified/distorted 
                // by the subsequent transform.

                var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);
                context.transform(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);

                //todo: also apply the transform to other geometry types.
            }
            else
            {
                if (transform != null)
                {
                    throw new NotSupportedException(string.Format("'{0}' is not currently supported in PathGeometry.", transform.GetType()));
                }
            }
        }

        internal static void FixCoordinatesGreaterThanInt32MaxValue(double minX, double minY, double maxX, double maxY, ref Transform actualTransform, ref double int32FactorX, ref double int32FactorY)
        {
            const Int32 maxValue = Int32.MaxValue;

            //---------------------------
            // Check the X axis:
            //---------------------------
            if (Math.Abs(minX) > maxValue || Math.Abs(maxX) > maxValue)
            {
                // Take the biggest number and caculate a factor so that it stays withing the range of Int32:
                double max = Math.Max(Math.Abs(minX), Math.Abs(maxX));
                int32FactorX = (maxValue / max) * 0.99; // Note: we multiply by 0.99 for a conservative margin, to make sure that the final points are not equal to maxValue.
                double invertedInt32FactorX = 1 / int32FactorX;
                Matrix scaleTransformMatrix = new Matrix(invertedInt32FactorX, 0, 0, 1, 0, 0);
                if (actualTransform != null)
                {
                    if (actualTransform is MatrixTransform)
                    {
                        actualTransform = new MatrixTransform() { Matrix = Matrix.Multiply(scaleTransformMatrix, ((MatrixTransform)actualTransform).Matrix) };
                    }
                    else
                    {
                        throw new NotSupportedException("The size of the shape exceeds Int32. This is currently supported only if the Transform is null or a MatrixTransform.");
                    }
                }
                else
                {
                    actualTransform = new MatrixTransform() { Matrix = scaleTransformMatrix };
                }
            }

            //---------------------------
            // Check the Y axis:
            //---------------------------
            if (Math.Abs(minY) > maxValue || Math.Abs(maxY) > maxValue)
            {
                // Take the biggest number and caculate a factor so that it stays withing the range of Int32:
                double max = Math.Max(Math.Abs(minY), Math.Abs(maxY));
                int32FactorY = (maxValue / max) * 0.99; // Note: we multiply by 0.99 for a conservative margin, to make sure that the final points are not equal to maxValue.
                double invertedInt32FactorY = 1 / int32FactorY;
                Matrix scaleTransformMatrix = new Matrix(1, 0, 0, invertedInt32FactorY, 0, 0);
                if (actualTransform != null)
                {
                    if (actualTransform is MatrixTransform)
                    {
                        actualTransform = new MatrixTransform() { Matrix = Matrix.Multiply(scaleTransformMatrix, ((MatrixTransform)actualTransform).Matrix) };
                    }
                    else
                    {
                        throw new NotSupportedException("The size of the shape exceeds Int32. This is currently supported only if the Transform is null or a MatrixTransform.");
                    }
                }
                else
                {
                    actualTransform = new MatrixTransform() { Matrix = scaleTransformMatrix };
                }
            }
        }
    }
}
