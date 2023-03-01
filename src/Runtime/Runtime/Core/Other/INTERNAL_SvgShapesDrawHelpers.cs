
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

using System.Windows;
using System.Windows.Shapes;

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SvgShapesDrawHelpers
    {
        internal static object CreateSvgOuterDomElement(FrameworkElement element, object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = INTERNAL_HtmlDomManager.CreateSvgDomElementAndAppendIt("svg", parentRef, element);

            if (!double.IsNaN(element.Width))
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementWhereToPlaceChildren, "width", element.Width);
            }
            if (!double.IsNaN(element.Height))
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementWhereToPlaceChildren, "height", element.Height);
            }

            // todo: handle NaN value of Width or Height
            if (!double.IsNaN(element.Width) && !double.IsNaN(element.Height))
            {
                // todo: do not clip the bounds
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementWhereToPlaceChildren, "viewBox", $"0 0 {element.Width} {element.Height}");
            }

            return domElementWhereToPlaceChildren;
        }

        internal static object CreateSvgEllipseDomElement(Shape shape, object parentRef)
        {
            var circle = INTERNAL_HtmlDomManager.CreateSvgDomElementAndAppendIt("ellipse", parentRef, shape);
            UpdateDefaultColorsToNone(shape, circle);

            // todo: handle NaN value of Width or Height
            // todo: make all calls at once

            // set radiuses of the ellipse and correct center
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "rx", shape.Width / 2);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "ry", shape.Height / 2);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "cx", "50%");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "cy", "50%");

            return circle;
        }

        internal static object CreateSvgLineDomElement(Shape shape, object parentRef)
        {
            var line = INTERNAL_HtmlDomManager.CreateSvgDomElementAndAppendIt("line", parentRef, shape);
            UpdateDefaultColorsToNone(shape, line);

            return line;
        }

        internal static object CreateSvgPolylineDomElement(Shape shape, object parentRef)
        {
            var polyline = INTERNAL_HtmlDomManager.CreateSvgDomElementAndAppendIt("polyline", parentRef, shape);
            UpdateDefaultColorsToNone(shape, polyline);

            return polyline;
        }

        // need to clear colors, because default in SVG is black
        private static void UpdateDefaultColorsToNone(Shape shape, object domElement)
        {
            if (shape.Fill is null)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElement, "fill", "none");
            }
            if (shape.Stroke is null)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElement, "stroke", "none");
            }
        }
    }
}