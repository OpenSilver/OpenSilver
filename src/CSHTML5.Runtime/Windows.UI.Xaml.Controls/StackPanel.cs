

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Arranges child elements into a single line that can be oriented horizontally
    /// or vertically.
    /// </summary>
    /// <example>
    /// You can add a StackPanel with a Horizontal orientation in the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <StackPanel Orientation="Horizontal">
    ///     <!--Add content elements.-->
    /// </StackPanel>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// StackPanel stackPanel = new StackPanel();
    /// stackPanel.Orientation = Orientation.Horizontal;
    /// </code>
    /// </example>
    public partial class StackPanel : Panel
    {
        Orientation? _renderedOrientation = null;

        /// <summary>
        /// Gets or sets the dimension by which child elements are stacked.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        /// <summary>
        /// Identifies the Orientation dependency property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel), new PropertyMetadata(Orientation.Vertical, Orientation_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void Orientation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stackPanel = (StackPanel)d;
            Orientation newValue = (Orientation)e.NewValue;

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(stackPanel)
                && stackPanel._renderedOrientation.HasValue
                && stackPanel._renderedOrientation.Value != newValue)
            {
                //todo: refresh the whole stackpanel (so that we display the children in the right orientation)

                throw new NotSupportedException("Changing the orientation of a StackPanel while it is in the visual tree is not yet supported.");
            }
        }


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            //INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div).position = "absolute";
            //div.style.position = "absolute";
            //note: size must be set in the div part only (the rest will follow).

            _renderedOrientation = this.Orientation;

            if (_renderedOrientation == Orientation.Horizontal)
            {
                //------v1------//

                //wrapper for the whole stackpanel:
                //<div>
                //  <table style="height:inherit; border-collapse:collapse">
                //    <tr>
                //          ...
                //    </tr>
                //  </table>
                //</div>

                //var table = INTERNAL_HtmlDomManager.CreateDomElement("table");
                //table.style.height = "inherit";
                //table.style.borderCollapse = "collapse";

                //var tr = INTERNAL_HtmlDomManager.CreateDomElement("tr");
                //tr.style.padding = "0px";

                //INTERNAL_HtmlDomManager.AppendChild(table, tr);
                //INTERNAL_HtmlDomManager.AppendChild(div, table);


                //domElementWhereToPlaceChildren = tr;

                //------v2------//
                    //wrapper for the whole StackPanel - v2:
                    //  <div style="display:table-row">
                    //      <div style="margin-left: 0px; margin-right: auto; height: 100%">
                    //          ...
                    //      </div>
                    //  </div>


                    object outerDiv;
                    dynamic outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);

                    if (INTERNAL_HtmlDomManager.IsInternetExplorer() && !INTERNAL_HtmlDomManager.IsEdge()) //When in Internet Explorer, we need to use display:grid instead of table so that VerticalAlignment.Stretch works (cf StratX.Star in the products page) but we definitely do not want this to be used in Edge as it crashes and causes the whole app to restart (cf ShowcaseApp with CSHTML5 from v1.0 beta 13.2 to RC1 included)
                    {
                        outerDivStyle.display = !Grid_InternalHelpers.isMSGrid() ? outerDivStyle.display = "grid" : Grid_InternalHelpers.INTERNAL_CSSGRID_MS_PREFIX + "grid";
                    }
                    else
                    {
                        outerDivStyle.display = "table";
                    }
                    object innerDiv;
                    dynamic innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out innerDiv);

                    innerDivStyle.marginLeft = "0px";
                    innerDivStyle.marginRight = "auto";
                    innerDivStyle.height = "100%";
                    innerDivStyle.display = "table";

                    domElementWhereToPlaceChildren = innerDiv;

                    return outerDiv;
            }
            else
            {
#if !BRIDGE
                return base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
#else
                return CreateDomElement_WorkaroundBridgeInheritanceBug(parentRef, out domElementWhereToPlaceChildren);
#endif
            }
        }

        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild)
        {
            if (Orientation == Orientation.Horizontal)
            {
                //------v1------//


                //NOTE: here, we are in a table

                //wrapper for each child:
                //<td style="padding:0px">
                //  <div style="width: inherit;position:relative">
                //      ...(child)
                //  </div>
                //</td>

                //var td = INTERNAL_HtmlDomManager.CreateDomElement("td");
                //td.style.position = "relative";
                //td.style.padding = "0px";
                ////var div = INTERNAL_HtmlDomManager.CreateDomElement("div");
                ////div.style.height = "inherit"; //todo: find a way to make this div actually inherit the height of the td... (otherwise we cannot set its verticalAlignment)
                ////div.style.position = "relative";
                ////INTERNAL_HtmlDomManager.AppendChild(td, div);

                //domElementWhereToPlaceChild = td;

                //return td;




                //------v2------// = better because we only use divs, it's more simple and verticalAlignment.Stretch works when the stackPanel's size is hard coded (but it still doesn't work when it's not).


                //wrapper for each child - v2
                //<div style="display: table-cell;height:inherit;>
                // ...
                //</div>

                var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
                var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
                divStyle.position = "relative";
                divStyle.display = "table-cell";
                divStyle.height = "100%"; //this allow the stretched items to actually be stretched to the size of the tallest element when the stackpanel's size is only defined by this element.
                divStyle.verticalAlign = "middle"; // We use this as a default value for elements that have a "stretch" vertical alignment

                domElementWhereToPlaceChild = div;


                return div;

            }
            else if (Orientation == Orientation.Vertical) //when we arrive here, it should always be true but we never know...
            {
                //NOTE: here, we are in a div


                //wrapper for each child:
                //<div style="width: inherit">... </div>

                var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
                var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
                divStyle.position = "relative";
                divStyle.width = "100%"; // Makes it possible to do horizontal alignment of the element that will be the child of this div.

                domElementWhereToPlaceChild = div;
                return div;
            }
            else
                throw new NotSupportedException();
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            _renderedOrientation = null;

            base.INTERNAL_OnDetachedFromVisualTree();
        }

        //internal override dynamic ShowChild(UIElement child)
        //{
        //    dynamic elementToReturn = base.ShowChild(child); //we need to return this so that a class that inherits from this but doesn't create a wrapper (or a different one) is correctly handled 

        //    dynamic domChildWrapper = INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
        //    dynamic domChildWrapperStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domChildWrapper);
        //    //domChildWrapperStyle.visibility = "visible";
        //    domChildWrapperStyle.display = "block"; //todo: verify that it is not necessary to revert to the previous value instead.
        //    if (Orientation == Orientation.Horizontal)
        //    {
        //        domChildWrapperStyle.height = "100%";
        //        domChildWrapperStyle.width = "";
        //    }
        //    else
        //    {
        //        domChildWrapperStyle.height = "";
        //        domChildWrapperStyle.width = "100%";
        //    }

        //    return elementToReturn;
        //}
    }
}
