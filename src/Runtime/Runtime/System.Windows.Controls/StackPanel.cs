

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
using System.Collections.Generic;
using System.Linq;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

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
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    MethodToUpdateDom2 = UpdateDomOnOrientationChanged,
                });

        private static void UpdateDomOnOrientationChanged(DependencyObject d, object oldValue, object newValue)
        {
            var stackPanel = (StackPanel)d;
            if (stackPanel.IsUnderCustomLayout)
            {
                return;
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(stackPanel.INTERNAL_InnerDomElement);
            switch ((Orientation)newValue)
            {
                case Orientation.Vertical:
                    innerDivStyle.display = "block";
                    innerDivStyle.height = string.Empty;
                    if (!stackPanel.CustomLayout)
                    {
                        stackPanel.SetDisplayOnChildWrappers("block");
                    }
                    break;

                case Orientation.Horizontal:
                    innerDivStyle.display = "flex";
                    innerDivStyle.height = "100%";
                    if (!stackPanel.CustomLayout)
                    {
                        stackPanel.SetDisplayOnChildWrappers("flex");
                    }
                    break;
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out object outerDiv);

            if (IsUnderCustomLayout)
            {
                domElementWhereToPlaceChildren = outerDiv;
                return outerDiv;
            }
            else if (IsCustomLayoutRoot)
            {
                outerDivStyle.position = "relative";
            }

            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out object innerDiv);
            switch (Orientation)
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

        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild, int index)
        {
            if (IsUnderCustomLayout || IsCustomLayoutRoot)
            {
                domElementWhereToPlaceChild = null;
                return null;
            }

            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this, index);
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div).display = Orientation switch
            {
                Orientation.Vertical => "block",
                Orientation.Horizontal => "flex",
                _ => throw new InvalidOperationException(),
            };

            domElementWhereToPlaceChild = div;
            return div;
        }

        private void SetDisplayOnChildWrappers(string display)
        {
            foreach (UIElement child in Children)
            {
                if (child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny is not null)
                {
                    var wrapperDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                        child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny);
                    wrapperDivStyle.display = display;
                }
            }
        }

        private double GetCrossLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Height : size.Width;
        }

        private double GetMainLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private static Size CreateSize(Orientation orientation, double mainLength, double crossLength)
        {
            return orientation == Orientation.Horizontal ?
                new Size(mainLength, crossLength) :
                new Size(crossLength, mainLength);
        }

        private static Rect CreateRect(Orientation orientation, double mainStart, double crossStart, double mainLength, double crossLength)
        {
            return orientation == Orientation.Horizontal ?
                new Rect(mainStart, crossStart, mainLength, crossLength) :
                new Rect(crossStart, mainStart, crossLength, mainLength);
        }

        internal override bool CheckIsAutoWidth(FrameworkElement child)
        {
            if (!double.IsNaN(child.Width))
            {
                return false;
            }

            if (Orientation == Orientation.Horizontal)
            {
                return true;
            }

            if (child.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return true;
            }

            if (VisualTreeHelper.GetParent(child) is FrameworkElement parent)
            {
                return parent.CheckIsAutoWidth(this);
            }

            return false;
        }

        internal override bool CheckIsAutoHeight(FrameworkElement child)
        {
            if (!double.IsNaN(child.Height))
            {
                return false;
            }

            if (Orientation == Orientation.Vertical)
            {
                return true;
            }

            if (child.VerticalAlignment != VerticalAlignment.Stretch)
            {
                return true;
            }

            if (VisualTreeHelper.GetParent(child) is FrameworkElement parent)
            {
                return parent.CheckIsAutoHeight(this);
            }

            return false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double availableCrossLength = GetCrossLength(availableSize);
            Size measureSize = CreateSize(Orientation, Double.PositiveInfinity, availableCrossLength);

            double mainLength = 0;
            double crossLength = 0;

            UIElement[] childrens = Children.ToArray();
            foreach (UIElement child in childrens)
            {
                child.Measure(measureSize);

                //INTERNAL_HtmlDomElementReference domElementReference = (INTERNAL_HtmlDomElementReference)child.INTERNAL_OuterDomElement;
                //Console.WriteLine($"MeasureOverride StackPanel Child desired Width {domElementReference.UniqueIdentifier} {child.DesiredSize.Width}, Height {child.DesiredSize.Height}");

                mainLength += GetMainLength(child.DesiredSize);
                crossLength = Math.Max(crossLength, GetCrossLength(child.DesiredSize));
            }

            // measuredCrossLength = availableCrossLength;

            return CreateSize(Orientation, mainLength, crossLength);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double panelMainLength = Children.Select(child => GetMainLength(child.DesiredSize)).Sum();
            double panelCrossLength = GetCrossLength(finalSize);

            //Console.WriteLine($"StackPanel panelMainLength {panelMainLength}, panelCrossLength {panelCrossLength}");

            Size measureSize = CreateSize(Orientation, Double.PositiveInfinity, panelCrossLength);
            //Console.WriteLine($"StackPanel ArrangeOverride measureSize {measureSize.Width}, {measureSize.Height}");

            UIElement[] childrens = Children.ToArray();
            foreach (UIElement child in childrens)
            {
                child.Measure(measureSize);
            }
            bool isNormalFlow = FlowDirection == FlowDirection.LeftToRight;
            double childrenMainLength = 0;
            foreach (UIElement child in childrens)
            {
                double childMainLength = GetMainLength(child.DesiredSize);
                double childMainStart = isNormalFlow ? childrenMainLength : panelMainLength - childrenMainLength - childMainLength;

                //Console.WriteLine($"StackPanel ArrangeOverride childMainLength {childMainLength}, childMainStart {childMainStart}");

                child.Arrange(CreateRect(Orientation, childMainStart, 0, childMainLength, panelCrossLength));

                childrenMainLength += childMainLength;
            }

            return CreateSize(Orientation, GetMainLength(finalSize), panelCrossLength);
        }
    }
}
