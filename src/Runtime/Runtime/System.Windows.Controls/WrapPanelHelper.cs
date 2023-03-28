
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
using System.Diagnostics;
using CSHTML5.Internal;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal static class WrapPanelHelper
    {
        internal static object CreateDomElement(Panel panel, Orientation orientation, object parentRef, out object domElementWhereToPlaceChildren)
        {
            Debug.Assert(panel is WrapPanel);

            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, panel, out object outerDiv);
            if (panel.IsUnderCustomLayout)
            {
                domElementWhereToPlaceChildren = outerDiv;
                return outerDiv;
            }
            else if (panel.UseCustomLayout)
            {
                outerDivStyle.position = "relative";
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, panel, out object innerDiv);
            innerDivStyle.display = "flex";
            innerDivStyle.flexWrap = "wrap";
            if (orientation == Orientation.Vertical)
            {
                innerDivStyle.width = "fit-content";
                innerDivStyle.height = "calc(100%)";
                innerDivStyle.flexDirection = "column";
            }

            domElementWhereToPlaceChildren = innerDiv;
            return outerDiv;
        }

        internal static object CreateDomChildWrapper(Panel panel, Orientation orientation, object parentRef, out object domElementWhereToPlaceChild, int index)
        {
            Debug.Assert(panel is WrapPanel);

            if (panel.UseCustomLayout)
            {
                domElementWhereToPlaceChild = null;
                return null;
            }

            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, panel, index);
            var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            divStyle.display = orientation switch
            {
                Orientation.Horizontal => "inline-flex",
                Orientation.Vertical => "block",
                _ => throw new InvalidOperationException(),
            };
            domElementWhereToPlaceChild = div;
            return div;
        }

        internal static void OnOrientationChanged(DependencyObject d, object oldValue, object newValue)
        {
            Debug.Assert(d is WrapPanel);
            var panel = (Panel)d;
            if (panel.IsUnderCustomLayout)
            {
                return;
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(panel.INTERNAL_InnerDomElement);
            switch ((Orientation)newValue)
            {
                case Orientation.Horizontal:
                    innerDivStyle.width = string.Empty;
                    innerDivStyle.height = string.Empty;
                    innerDivStyle.flexDirection = string.Empty;
                    if (!panel.UseCustomLayout)
                    {
                        UpdateChildWrappers(panel, false);
                    }
                    break;

                case Orientation.Vertical:
                    innerDivStyle.width = "fit-content";
                    innerDivStyle.height = "calc(100%)";
                    innerDivStyle.flexDirection = "column";
                    if (!panel.UseCustomLayout)
                    {
                        UpdateChildWrappers(panel, true);
                    }
                    break;
            }
        }

        private static void UpdateChildWrappers(Panel panel, bool isVertical)
        {
            if (!panel.HasChildren) return;
            foreach (UIElement child in panel.Children)
            {
                if (child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny is not null)
                {
                    var wrapperDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                        child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny);

                    if (isVertical)
                    {
                        wrapperDivStyle.display = "block";
                    }
                    else
                    {
                        wrapperDivStyle.display = "inline-flex";
                    }
                }
            }
        }
    }
}
