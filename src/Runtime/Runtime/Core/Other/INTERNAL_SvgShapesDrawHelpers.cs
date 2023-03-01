
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

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SvgShapesDrawHelpers
    {
        internal static object CreateSvgDomElement(FrameworkElement element, object parentRef, out object domElementWhereToPlaceChildren)
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

        internal static object CreateSvgEllipseDomElement(FrameworkElement element, object parentRef)
        {
            var circle = INTERNAL_HtmlDomManager.CreateSvgDomElementAndAppendIt("ellipse", parentRef, element);

            // todo: handle NaN value of Width or Height
            // todo: make all calls at once

            // set radiuses of the ellipse and correct center
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "rx", element.Width / 2);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "ry", element.Height / 2);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "cx", "50%");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(circle, "cy", "50%");

            return circle;
        }
    }
}