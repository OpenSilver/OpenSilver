
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
using System.Linq;

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
        internal sealed override Orientation LogicalOrientation => Orientation;

        internal sealed override bool HasLogicalOrientation => true;

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
                    MethodToUpdateDom2 = StackPanelHelper.OnOrientationChanged,
                });

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return StackPanelHelper.CreateDomElement(this, Orientation, parentRef, out domElementWhereToPlaceChildren);
        }

        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild, int index)
        {
            return StackPanelHelper.CreateDomChildWrapper(this, Orientation, parentRef, out domElementWhereToPlaceChild, index);
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
