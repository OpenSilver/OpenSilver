
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
    internal static class StackPanelHelper
    {
        internal static object CreateDomElement(Panel panel, Orientation orientation, object parentRef, out object domElementWhereToPlaceChildren)
        {
            Debug.Assert(panel is StackPanel || panel is VirtualizingStackPanel);

            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, panel, out object outerDiv);

            if (panel.IsUnderCustomLayout)
            {
                domElementWhereToPlaceChildren = outerDiv;
                return outerDiv;
            }
            else if (panel.IsCustomLayoutRoot)
            {
                outerDivStyle.position = "relative";
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, panel, out object innerDiv);
            switch (orientation)
            {
                case Orientation.Vertical:
                    innerDivStyle.display = "block";
                    innerDivStyle.height = string.Empty;
                    break;

                case Orientation.Horizontal:
                    innerDivStyle.display = "flex";
                    innerDivStyle.height = "100%";
                    break;
            }

            domElementWhereToPlaceChildren = innerDiv;

            return outerDiv;
        }

        internal static object CreateDomChildWrapper(Panel panel, Orientation orientation, object parentRef, out object domElementWhereToPlaceChild, int index)
        {
            Debug.Assert(panel is StackPanel || panel is VirtualizingStackPanel);

            if (panel.IsUnderCustomLayout || panel.IsCustomLayoutRoot)
            {
                domElementWhereToPlaceChild = null;
                return null;
            }

            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, panel, index);
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            switch (orientation)
            {
                case Orientation.Vertical:
                    style.display = "block";
                    break;

                case Orientation.Horizontal:
                    style.display = "flex";
                    style.flex = "0 0 auto";
                    break;

                default:
                    throw new InvalidOperationException();
            }

            domElementWhereToPlaceChild = div;
            return div;
        }

        internal static void OnOrientationChanged(DependencyObject d, object oldValue, object newValue)
        {
            Debug.Assert(d is StackPanel || d is VirtualizingStackPanel);
            Panel panel = (Panel)d;
            if (panel.IsUnderCustomLayout)
            {
                return;
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(panel.INTERNAL_InnerDomElement);
            switch ((Orientation)newValue)
            {
                case Orientation.Vertical:
                    innerDivStyle.display = "block";
                    innerDivStyle.height = string.Empty;
                    if (!panel.UseCustomLayout)
                    {
                        UpdateChildWrappers(panel, true);
                    }
                    break;

                case Orientation.Horizontal:
                    innerDivStyle.display = "flex";
                    innerDivStyle.height = "100%";
                    if (!panel.UseCustomLayout)
                    {
                        UpdateChildWrappers(panel, false);
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
                        wrapperDivStyle.flex = string.Empty;
                    }
                    else
                    {
                        wrapperDivStyle.display = "flex";
                        wrapperDivStyle.flex = "0 0 auto";
                    }
                }
            }
        }
    }
}
