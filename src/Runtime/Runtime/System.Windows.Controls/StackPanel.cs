
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

using System.Collections.Generic;

namespace System.Windows.Controls
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
            set { SetValueInternal(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the Orientation dependency property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override Size MeasureOverride(Size constraint)
        {
            Size stackDesiredSize = new Size();
            List<UIElement> children = InternalChildren;
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
            List<UIElement> children = InternalChildren;
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
