
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

using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls
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
    [ContentProperty(nameof(Child))]
    public class Border : FrameworkElement
    {
        private WeakEventListener<Border, Brush, EventArgs> _backgroundChangedListener;
        private WeakEventListener<Border, Brush, EventArgs> _borderBrushChangedListener;

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

            if (BorderBrush == null)
            {
                ChangeBorderColor(this, null, null);
            }

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(Child, this);
        }

        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background),
                typeof(Brush),
                typeof(Border),
                new PropertyMetadata(null, OnBackgroundChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var border = (Border)d;
                        _ = border.SetBackgroundAsync((Brush)newValue);
                        SetPointerEvents(border);
                    },
                });

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that fills the background of the border.
        /// </summary>
        /// <returns>
        /// The brush that fills the background.
        /// </returns>
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Border border = (Border)d;
            border.SizeChanged -= OnSizeChanged;
            if (e.NewValue is LinearGradientBrush)
            {
                border.SizeChanged += OnSizeChanged;
            }

            if (border._backgroundChangedListener != null)
            {
                border._backgroundChangedListener.Detach();
                border._backgroundChangedListener = null;
            }

            if (e.NewValue is Brush newBrush)
            {
                border._backgroundChangedListener = new(border, newBrush)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnBackgroundChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newBrush.Changed += border._backgroundChangedListener.OnEvent;
            }
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                _ = this.SetBackgroundAsync((Brush)sender);
            }
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var b = (Border)sender;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(b))
            {
                _ = b.SetBackgroundAsync(b.Background);
            }
        }

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                nameof(BorderBrush),
                typeof(Brush),
                typeof(Border),
                new PropertyMetadata(null, OnBorderBrushChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ChangeBorderColor((Border)d, oldValue as Brush, (Brush)newValue),
                });

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that is used to create the border.
        /// </summary>
        /// <returns>
        /// The brush that fills the border.
        /// </returns>
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = (Border)d;

            if (border._borderBrushChangedListener != null)
            {
                border._borderBrushChangedListener.Detach();
                border._borderBrushChangedListener = null;
            }

            if (e.NewValue is Brush newBrush)
            {
                border._borderBrushChangedListener = new(border, newBrush)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnBorderBrushChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newBrush.Changed += border._borderBrushChangedListener.OnEvent;
            }
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var brush = (Brush)sender;
                ChangeBorderColor(this, brush, brush);
            }
        }

        private static void ChangeBorderColor(Border border, Brush oldBrush, Brush newBrush)
        {
            var cssStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(border.INTERNAL_OuterDomElement);
            switch (oldBrush, newBrush)
            {
                case (GradientBrush, SolidColorBrush solid):
                    cssStyle.borderImageSource = string.Empty;
                    cssStyle.borderImageSlice = string.Empty;
                    cssStyle.borderColor = solid.INTERNAL_ToHtmlString();
                    break;

                case (_, SolidColorBrush solid):
                    cssStyle.borderColor = solid.INTERNAL_ToHtmlString();
                    break;

                case (_, LinearGradientBrush linear):
                    cssStyle.borderColor = string.Empty;
                    cssStyle.borderImageSource = linear.INTERNAL_ToHtmlString(border);
                    cssStyle.borderImageSlice = "1";
                    break;

                case (_, RadialGradientBrush radial):
                    cssStyle.borderColor = string.Empty;
                    cssStyle.borderImageSource = radial.INTERNAL_ToHtmlString(border);
                    cssStyle.borderImageSlice = "1";
                    break;

                case (_, null):
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
            // Compute the chrome size added by the various elements
            Size border = HelperCollapseThickness(BorderThickness);
            Size padding = HelperCollapseThickness(Padding);

            if (Child is UIElement child)
            {
                // Combine into total decorating size
                Size combined = new Size(border.Width + padding.Width, border.Height + padding.Height);

                // Remove size of border only from child's reference size.
                Size childConstraint = new Size(
                    Math.Max(0.0, availableSize.Width - combined.Width),
                    Math.Max(0.0, availableSize.Height - combined.Height));

                child.Measure(childConstraint);
                
                return new Size(child.DesiredSize.Width + combined.Width, child.DesiredSize.Height + combined.Height);
            }

            return new Size(border.Width + padding.Width, border.Height + padding.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //  arrange child
            if (Child is UIElement child)
            {
                Thickness borders = BorderThickness;
                Rect innerRect = new Rect(0, 0,
                    Math.Max(0.0, finalSize.Width - borders.Left - borders.Right),
                    Math.Max(0.0, finalSize.Height - borders.Top - borders.Bottom));
                Rect childRect = HelperDeflateRect(innerRect, Padding);

                child.Arrange(childRect);
            }

            return finalSize;
        }

        private static Size HelperCollapseThickness(Thickness th) => new Size(th.Left + th.Right, th.Top + th.Bottom);

        /// Helper to deflate rectangle by thickness
        private static Rect HelperDeflateRect(Rect rt, Thickness thick) =>
            new Rect(rt.Left + thick.Left,
                     rt.Top + thick.Top,
                     Math.Max(0.0, rt.Width - thick.Left - thick.Right),
                     Math.Max(0.0, rt.Height - thick.Top - thick.Bottom));

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
