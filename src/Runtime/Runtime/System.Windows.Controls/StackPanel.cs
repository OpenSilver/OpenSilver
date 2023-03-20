
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
    public class StackPanel : Panel
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

        protected override Size MeasureOverride(Size constraint)
        {
            Size stackDesiredSize = new Size();
            UIElementCollection children = Children;
            Size layoutSlotSize = constraint;
            bool fHorizontal = Orientation == Orientation.Horizontal;

            //
            // Initialize child sizing and iterator data
            // Allow children as much size as they want along the stack.
            //
            if (fHorizontal)
            {
                layoutSlotSize.Width = double.PositiveInfinity;
            }
            else
            {
                layoutSlotSize.Height = double.PositiveInfinity;
            }

            //
            //  Iterate through children.
            //
            int count = children.Count;
            for (int i = 0; i < count; ++i)
            {
                // Get next child.
                UIElement child = children[i];

                // Measure the child.
                child.Measure(layoutSlotSize);
                Size childDesiredSize = child.DesiredSize;

                // Accumulate child size.
                if (fHorizontal)
                {
                    stackDesiredSize.Width += childDesiredSize.Width;
                    stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                }
                else
                {
                    stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                    stackDesiredSize.Height += childDesiredSize.Height;
                }
            }

            return stackDesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = Children;
            bool fHorizontal = Orientation == Orientation.Horizontal;
            Rect rcChild = new Rect(arrangeSize);
            double previousChildSize = 0.0;

            //
            // Arrange and Position Children.
            //
            int count = children.Count;
            for (int i = 0; i < count; ++i)
            {
                UIElement child = children[i];

                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = child.DesiredSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = child.DesiredSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
                }

                child.Arrange(rcChild);
            }
            return arrangeSize;
        }
    }
}
