
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
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Collections.Generic;
using System.Windows.Media;

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
    public class StackPanel : Panel, IBorderElement
    {
        private WeakEventListener<StackPanel, Brush, EventArgs> _borderBrushChangedListener;

        /// <summary>
        /// Gets a value that represents the <see cref="Controls.Orientation"/> of the <see cref="StackPanel"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="Controls.Orientation"/> value.
        /// </returns>
        protected internal override Orientation LogicalOrientation => Orientation;

        /// <summary>
        /// Gets a value that indicates if this <see cref="StackPanel"/> has vertical or horizontal orientation.
        /// </summary>
        /// <returns>
        /// This property always returns true.
        /// </returns>
        protected internal override bool HasLogicalOrientation => true;

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the dimension by which child elements are stacked.
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValueInternal(OrientationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Spacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                nameof(Spacing),
                typeof(double),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets a uniform distance (in pixels) between stacked items. It is applied in the direction of 
        /// the <see cref="Orientation"/>.
        /// </summary>
        /// <returns>
        /// The uniform distance (in pixels) between stacked items.
        /// </returns>
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValueInternal(SpacingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            Border.PaddingProperty.AddOwner(
                typeof(StackPanel),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        /// <returns>
        /// The dimensions of the space between the border and its child as a <see cref="Thickness"/> value.
        /// </returns>
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValueInternal(PaddingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            Border.BorderBrushProperty.AddOwner(
                typeof(StackPanel),
                new FrameworkPropertyMetadata(null, OnBorderBrushChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((StackPanel)d).SetBorderColor(oldValue as Brush, (Brush)newValue),
                });

        /// <summary>
        /// Gets or sets a brush that describes the border fill of the panel.
        /// </summary>
        /// <returns>
        /// The brush that is used to fill the panel's border. The default is null, which is evaluated as
        /// <see cref="Colors.Transparent"/> for rendering.
        /// </returns>
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValueInternal(BorderBrushProperty, value);
        }

        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (StackPanel)d;

            if (panel._borderBrushChangedListener != null)
            {
                panel._borderBrushChangedListener.Detach();
                panel._borderBrushChangedListener = null;
            }

            if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
            {
                panel._borderBrushChangedListener = new(panel, newBrush)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnBorderBrushChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newBrush.Changed += panel._borderBrushChangedListener.OnEvent;
            }
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var brush = (Brush)sender;
                this.SetBorderColor(brush, brush);
            }
        }

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            Border.BorderThicknessProperty.AddOwner(
                typeof(StackPanel),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((StackPanel)d).SetBorderWidth((Thickness)newValue),
                });

        /// <summary>
        /// Gets or sets the border thickness of the panel.
        /// </summary>
        /// <returns>
        /// The border thickness of the panel, as a <see cref="Thickness"/> value.
        /// </returns>
        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValueInternal(BorderThicknessProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(
                typeof(StackPanel),
                new FrameworkPropertyMetadata(new CornerRadius())
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((StackPanel)d).SetBorderRadius((CornerRadius)newValue),
                });

        /// <summary>
        /// Gets or sets the radius for the corners of the panel's border.
        /// </summary>
        /// <returns>
        /// The degree to which the corners are rounded, expressed as values of the <see cref="Windows.CornerRadius"/> structure.
        /// </returns>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValueInternal(CornerRadiusProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size stackDesiredSize = new();
            List<UIElement> children = InternalChildren;
            bool fHorizontal = Orientation == Orientation.Horizontal;
            int visibleChildrenCount = 0;

            Size border = Border.HelperCollapseThickness(BorderThickness);
            Size padding = Border.HelperCollapseThickness(Padding);
            Size combined = new(border.Width + padding.Width, border.Height + padding.Height);

            Size layoutSlotSize = new(
                Math.Max(0.0, constraint.Width - combined.Width),
                Math.Max(0.0, constraint.Height - combined.Height));

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

                if (child.Visibility != Visibility.Collapsed)
                {
                    visibleChildrenCount++;
                }

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

            if (visibleChildrenCount > 1)
            {
                if (fHorizontal)
                {
                    stackDesiredSize.Width += (visibleChildrenCount - 1) * Spacing;
                }
                else
                {
                    stackDesiredSize.Height += (visibleChildrenCount - 1) * Spacing;
                }
            }

            stackDesiredSize.Width += combined.Width;
            stackDesiredSize.Height += combined.Height;

            return stackDesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            List<UIElement> children = InternalChildren;
            bool fHorizontal = Orientation == Orientation.Horizontal;
            double spacing = Spacing;
            double previousChildSize = 0.0;

            Thickness borders = BorderThickness;
            Rect paddingBox = new(0, 0,
                Math.Max(0.0, arrangeSize.Width - borders.Left - borders.Right),
                Math.Max(0.0, arrangeSize.Height - borders.Top - borders.Bottom));
            Rect contentBox = Border.HelperDeflateRect(paddingBox, Padding);
            Rect rcChild = contentBox;

            //
            // Arrange and Position Children.
            //
            bool isFirstVisibleChild = true;
            int count = children.Count;
            for (int i = 0; i < count; ++i)
            {
                UIElement child = children[i];

                bool isVisible = child.Visibility != Visibility.Collapsed;
                bool addSpacing = isVisible && !isFirstVisibleChild;

                if (isVisible)
                {
                    isFirstVisibleChild = false;
                }

                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    if (addSpacing)
                    {
                        rcChild.X += spacing;
                    }
                    previousChildSize = child.DesiredSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(contentBox.Height, child.DesiredSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    if (addSpacing)
                    {
                        rcChild.Y += spacing;
                    }
                    previousChildSize = child.DesiredSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(contentBox.Width, child.DesiredSize.Width);
                }

                child.Arrange(rcChild);
            }
            return arrangeSize;
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            return INTERNAL_HtmlDomManager.CreateBorderDomElementAndAppendIt(parentRef, this);
        }
    }
}
