

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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Markup;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

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
    /// Draws a border, background, or both, around another object.
    /// </summary>
    /// <example>
    /// You can add a Border to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <Border Width="60"
    ///         Height="30"
    ///         CornerRadius="15"
    ///         Padding="20"
    ///         Background="Blue"
    ///         HorizontalAlignment="Left">
    ///     <!--Child here.-->
    /// </Border>
    /// </code>
    /// Or in C# (assuming we have a StackPanel Named MyStackPanel):
    /// <code lang="C#">
    /// Border myBorder = new Border();
    /// myBorder.Width = 60;
    /// myBorder.Height = 30;
    /// myBorder.CornerRadius = new CornerRadius(15);
    /// myBorder.Padding = new Thickness(20);
    /// myBorder.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
    /// myBorder.HorizontalAlignment=HorizontalAlignment.Left;
    /// MyStackPanel.Children.Add(myBorder);
    /// </code>
    /// </example>
    [ContentProperty("Child")]
    public partial class Border : FrameworkElement
    {
        /// <summary>
        /// Returns the Visual children count.
        /// </summary>
        internal override int VisualChildrenCount
        {
            get { return (Child == null) ? 0 : 1; }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        internal override UIElement GetVisualChild(int index)
        {
            if ((Child == null)
                || (index != 0))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Child;
        }

        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/
        internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Child == null)
                {
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return new SingleChildEnumerator(Child);
            }
        }

        internal override bool EnablePointerEventsCore
        {
            // We only check the Background property even if BorderBrush not null
            // and BorderThickness > 0 is a sufficient condition to enable pointer
            // events on the borders of the control.
            // There is no way right now to differentiate the Background and BorderBrush
            // as they are both defined on the same DOM element.
            get
            {
                return this.Background != null;
            }
        }

        /// <summary>
        /// Identifies the <see cref="Child"/> dependency property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(
                nameof(Child),
                typeof(UIElement),
                typeof(Border),
                new PropertyMetadata(null, OnChildChanged));

        /// <summary>
        /// Gets or sets the child element to draw the border around.
        /// </summary>
        public UIElement Child
        {
            get => (UIElement)GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Border border = (Border)d;
            UIElement oldChild = (UIElement)e.OldValue;
            UIElement newChild = (UIElement)e.NewValue;

            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, border);

            border.RemoveVisualChild(oldChild);
            border.RemoveLogicalChild(oldChild);
            border.AddLogicalChild(newChild);
            border.AddVisualChild(newChild);

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, border);

            border.InvalidateMeasure();
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (this.BorderBrush == null)
            {
                UpdateDomOnBorderBrushChanged(this, null, null);
            }

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(Child, this);
        }

        /// <summary>
        /// Gets or sets the Brush that fills the background of the border.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Border.Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background),
                typeof(Brush),
                typeof(Border),
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = (d, oldValue, newValue) =>
                    {
                        var border = (Border)d;
                        _ = Panel.RenderBackgroundAsync(border, (Brush)newValue);
                        SetPointerEvents(border);
                    },
                });

        /// <summary>
        /// Gets or sets a brush that describes the border background of a control.
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Border.BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                nameof(BorderBrush),
                typeof(Brush),
                typeof(Border),
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = UpdateDomOnBorderBrushChanged,
                });

        private static void UpdateDomOnBorderBrushChanged(DependencyObject d, object oldValue, object newValue)
        {
            var border = (Border)d;
            var cssStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(border);
            switch (newValue)
            {
                case SolidColorBrush solid:
                    if (oldValue is GradientBrush)
                    {
                        cssStyle.borderImageSource = string.Empty;
                        cssStyle.borderImageSlice = string.Empty;
                    }
                    cssStyle.borderColor = solid.INTERNAL_ToHtmlString();
                    break;

                case LinearGradientBrush linear:
                    if (oldValue is SolidColorBrush)
                    {
                        cssStyle.borderColor = string.Empty;
                    }
                    cssStyle.borderImageSource = linear.INTERNAL_ToHtmlString(border);
                    cssStyle.borderImageSlice = "1";
                    break;

                case RadialGradientBrush radial:
                    if (oldValue is SolidColorBrush)
                    {
                        cssStyle.borderColor = string.Empty;
                    }
                    cssStyle.borderImageSource = radial.INTERNAL_ToHtmlString(border);
                    cssStyle.borderImageSlice = "1";
                    break;

                case null:
                    cssStyle.borderColor = "transparent";
                    cssStyle.borderImageSource = string.Empty;
                    cssStyle.borderImageSlice = string.Empty;
                    break;

                default:
                    // ImageBrush and custom brushes are not supported.
                    // Keep using old brush.
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom = BorderThickness_MethodToUpdateDom
                },
                IsThicknessValid);

        private static void BorderThickness_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var border = (Border)d;
            var domElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(border);
            var thickness = (Thickness)newValue;
            domElement.boxSizing = "border-box";
            domElement.borderStyle = "solid"; //todo: see if we should put this somewhere else
            domElement.borderWidth = $"{thickness.Top.ToInvariantString()}px {thickness.Right.ToInvariantString()}px {thickness.Bottom.ToInvariantString()}px {thickness.Left.ToInvariantString()}px";
        }

        /// <summary>
        /// Gets or sets the radius for the corners of the border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Border.CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Border),
                new PropertyMetadata(new CornerRadius())
                {
                    MethodToUpdateDom = CornerRadius_MethodToUpdateDom
                },
                IsCornerRadiusValid);

        private static void CornerRadius_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var border = (Border)d;
            var cr = (CornerRadius)newValue;
            var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(border);
            domStyle.borderRadius = $"{cr.TopLeft.ToInvariantString()}px {cr.TopRight.ToInvariantString()}px {cr.BottomRight.ToInvariantString()}px {cr.BottomLeft.ToInvariantString()}px";
        }

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsThicknessValid);

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child == null)
            {
                return new Size();
            }

            Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);
            Size PaddingSize = new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            Child.Measure(availableSize.Subtract(BorderThicknessSize).Subtract(PaddingSize).Max(new Size()));
            return Child.DesiredSize.Add(BorderThicknessSize).Add(PaddingSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                Point PaddingLocation = new Point(Padding.Left, Padding.Top);
                Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);
                Size PaddingSize = new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
                Child.Arrange(new Rect(PaddingLocation, finalSize.Subtract(BorderThicknessSize).Subtract(PaddingSize).Max(new Size())));
            }

            return finalSize;
        }

        private static bool IsThicknessValid(object value)
        {
            Thickness t = (Thickness)value;
            return Thickness.IsValid(t, false, false, false, false);
        }

        private static bool IsCornerRadiusValid(object value)
        {
            CornerRadius cr = (CornerRadius)value;
            return CornerRadius.IsValid(cr, false, false, false, false);
        }
    }
}
