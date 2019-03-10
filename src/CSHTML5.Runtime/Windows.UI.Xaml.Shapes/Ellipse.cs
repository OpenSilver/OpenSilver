
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    /// Draws an ellipse.
    /// </summary>
    /// <exclude/>
    public class Ellipse : Shape
    {
        //dynamic canvasDomElement;

        static Ellipse()
        {
            Ellipse.StretchProperty.OverrideMetadata(typeof(Ellipse), new PropertyMetadata(Stretch.Fill, Shape.Stretch_Changed));
        }

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

        override internal protected void Redraw()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                double xOffsetToApplyBeforeMultiplication;
                double yOffsetToApplyBeforeMultiplication;
                double xOffsetToApplyAfterMultiplication;
                double yOffsetToApplyAfterMultiplication;
                double sizeX;
                double sizeY;
                double horizontalMultiplicator;
                double verticalMultiplicator;
                Size shapeActualSize;
                Shape.GetShapeInfos(this, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out sizeX, out sizeY, out horizontalMultiplicator, out verticalMultiplicator, out shapeActualSize);

                ApplyMarginToFixNegativeCoordinates(new Point());
                if (Stretch == Media.Stretch.None)
                {
                    ApplyMarginToFixNegativeCoordinates(_marginOffsets);
                }

                //todo: size might not perfectly match that of she designer. (so it might be very different from it in big ellipses?)
                INTERNAL_ShapesDrawHelpers.PrepareEllipse(_canvasDomElement, sizeX, sizeY, sizeX / 2 + xOffsetToApplyBeforeMultiplication + xOffsetToApplyAfterMultiplication, sizeY / 2 + yOffsetToApplyBeforeMultiplication + yOffsetToApplyAfterMultiplication);

                //todo: make sure the parameters below are correct.
                Shape.DrawFillAndStroke(this, "evenodd", xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, xOffsetToApplyAfterMultiplication + sizeX, yOffsetToApplyAfterMultiplication + sizeY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);


                //dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(_canvasDomElement);
                //if (fillValue != null || (fillValue is string && !string.IsNullOrWhiteSpace((string)fillValue)))
                //{
                //    context.fill("evenodd"); //note: remember: always fill before stroke, otherwise the filling will hide the stroke.
                //}
                //if (StrokeThickness > 0 && Stroke != null)
                //{
                //    context.stroke();
                //}
            }
        }
    }
}