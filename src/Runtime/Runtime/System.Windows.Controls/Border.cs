
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
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;

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
    public class Border : FrameworkElement, IBorderElement
    {
        private UIElement _child;
        private WeakEventToken _weakBackgroundEventToken;
        private WeakEventToken _weakBorderBrushEventToken;
        private bool _refreshBackgroundOnSizeChange;

        /// <summary>
        /// Gets a value that is equal to the number of visual child elements of this instance of <see cref="Border"/>.
        /// </summary>
        /// <returns>
        /// The number of visual child elements.
        /// </returns>
        protected override int VisualChildrenCount => Child is null ? 0 : 1;

        /// <summary>
        /// Gets the child <see cref="UIElement"/> element at the specified index position.
        /// </summary>
        /// <param name="index">
        /// Index position of the child element.
        /// </param>
        /// <returns>
        /// The child element at the specified index position.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is greater than the number of visual child elements.
        /// </exception>
        protected override UIElement GetVisualChild(int index)
        {
            if (Child is null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Child;
        }

        /// <summary>
        /// Gets an enumerator that can be used to iterate the logical child elements of a <see cref="Border"/>.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate the logical child elements of a <see cref="Border"/>.
        /// </returns>
        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (Child is null)
                {
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return new SingleChildEnumerator(Child);
            }
        }

        // We only check the Background property even if BorderBrush not null
        // and BorderThickness > 0 is a sufficient condition to enable pointer
        // events on the borders of the control.
        // There is no way right now to differentiate the Background and BorderBrush
        // as they are both defined on the same DOM element.
        internal override bool EnablePointerEventsCore => Background is not null;

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
            get => _child;
            set => SetValueInternal(ChildProperty, value);
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Border border = (Border)d;
            UIElement oldChild = (UIElement)e.OldValue;
            UIElement newChild = (UIElement)e.NewValue;

            border._child = newChild;

            border.RemoveVisualChild(oldChild);
            border.RemoveLogicalChild(oldChild);
            border.AddLogicalChild(newChild);
            border.AddVisualChild(newChild);

            border.InvalidateMeasure();
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Border)d).SetBackground((Brush)newValue),
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
            set => SetValueInternal(BackgroundProperty, value);
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Border border = (Border)d;

            border._refreshBackgroundOnSizeChange = e.NewValue is LinearGradientBrush;

            if (border._weakBackgroundEventToken != null)
            {
                border._weakBackgroundEventToken.Dispose();
                border._weakBackgroundEventToken = null;
            }

            if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
            {
                border._weakBackgroundEventToken = WeakEvent.Subscribe<Border, Brush, EventArgs>(
                    border,
                    newBrush,
                    static (instance, sender, args) => instance.OnBackgroundChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
            }

            // Update pointer events
            border.CoerceIsHitTestable();
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                this.SetBackground((Brush)sender);
            }
        }

        /// <inheritdoc />
        protected internal override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);

            if (_refreshBackgroundOnSizeChange && INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                this.SetBackground(Background);
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Border)d).SetBorderColor(oldValue as Brush, (Brush)newValue),
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
            set => SetValueInternal(BorderBrushProperty, value);
        }

        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = (Border)d;

            if (border._weakBorderBrushEventToken != null)
            {
                border._weakBorderBrushEventToken.Dispose();
                border._weakBorderBrushEventToken = null;
            }

            if (e.NewValue is Brush newBrush && !newBrush.IsSealed)
            {
                border._weakBorderBrushEventToken = WeakEvent.Subscribe<Border, Brush, EventArgs>(
                    border,
                    newBrush,
                    static (instance, sender, args) => instance.OnBorderBrushChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
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
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValueInternal(BorderThicknessProperty, value); }
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Border)d).SetBorderWidth((Thickness)newValue),
                },
                IsThicknessValid);

        /// <summary>
        /// Gets or sets the radius for the corners of the border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValueInternal(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Border),
                new PropertyMetadata(new CornerRadius())
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((Border)d).SetBorderRadius((CornerRadius)newValue),
                },
                IsCornerRadiusValid);

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValueInternal(PaddingProperty, value); }
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

            // Combine into total decorating size
            Size combined = new(border.Width + padding.Width, border.Height + padding.Height);

            if (Child is UIElement child)
            {
                // Remove size of border only from child's reference size.
                Size childConstraint = new(
                    Math.Max(0.0, availableSize.Width - combined.Width),
                    Math.Max(0.0, availableSize.Height - combined.Height));

                child.Measure(childConstraint);
                
                return new Size(child.DesiredSize.Width + combined.Width, child.DesiredSize.Height + combined.Height);
            }

            return combined;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //  arrange child
            if (Child is UIElement child)
            {
                Thickness borders = BorderThickness;
                Rect innerRect = new(0, 0,
                    Math.Max(0.0, finalSize.Width - borders.Left - borders.Right),
                    Math.Max(0.0, finalSize.Height - borders.Top - borders.Bottom));
                Rect childRect = HelperDeflateRect(innerRect, Padding);

                child.Arrange(childRect);
            }

            return finalSize;
        }

        internal static Size HelperCollapseThickness(Thickness th) => new Size(th.Left + th.Right, th.Top + th.Bottom);

        /// Helper to deflate rectangle by thickness
        internal static Rect HelperDeflateRect(Rect rt, Thickness thick) =>
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

        /// <inheritdoc />
        protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
        {
            return INTERNAL_HtmlDomManager.CreateBorderDomElementAndAppendIt(parent, this);
        }
    }
}
