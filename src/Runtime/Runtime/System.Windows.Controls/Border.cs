

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
using OpenSilver.Internal.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Markup;

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
            get { return (_child == null) ? 0 : 1; }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        internal override UIElement GetVisualChild(int index)
        {
            if ((_child == null)
                || (index != 0))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _child;
        }

        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/
        internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this._child == null)
                {
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return new SingleChildEnumerator(_child);
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

        private UIElement _child;

        /// <summary>
        /// Gets or sets the child element to draw the border around.
        /// </summary>
        public UIElement Child
        {
            get
            {
                return _child;
            }
            set
            {
                if (ReferenceEquals(_child, value))
                    return;

                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_child, this);

                RemoveVisualChild(_child);
                RemoveLogicalChild(_child);
                _child = value;
                AddLogicalChild(value);
                AddVisualChild(value);

                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(value, this);
            }
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (this.BorderBrush == null)
            {
                INTERNAL_PropertyStore.ApplyCssChanges(null, null, Border.BorderBrushProperty.GetMetadata(typeof(Border)), this);
            }

            if (!this.INTERNAL_EnableProgressiveLoading)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);
            }
            else
            {
                ProgressivelyAttachChild();
            }
        }

        private async void ProgressivelyAttachChild()
        {
            await Task.Delay(1);
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //this can happen if the Panel is detached during the delay.
                return;
            }
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);
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
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                    },
                    MethodToUpdateDom = (d, e) =>
                    {
                        UIElement.SetPointerEvents((Border)d);
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
                    //GetCSSEquivalent = (instance) => new CSSEquivalent
                    //{
                    //    Value = (inst, value) => value ?? "transparent",
                    //    Name = new List<string> { "borderColor" },
                    //}
                    GetCSSEquivalents = (instance) =>
                    {
                        var cssList = new List<CSSEquivalent>();
                        var brush = ((Border)instance).Background;
                        if (brush is LinearGradientBrush)
                        {
                            cssList.Add(new CSSEquivalent
                            {
                                Name = new List<string> { "border-image-source" },
                            });
                            cssList.Add(new CSSEquivalent
                            {
                                Name = new List<string> { "border-image-slice" },
                                Value = (d, value) => { return "1"; },
                            });
                        }
                        else
                        {
                            cssList.Add(new CSSEquivalent
                            {
                                Name = new List<string> { "borderColor" },
                                Value = (inst, value) => value ?? "transparent"
                            });

                        }
                        return cssList;
                    }
                });

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Border.BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom = BorderThickness_MethodToUpdateDom
                });

        private static void BorderThickness_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var border = (Border)d;
            var domElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(border);
            var thickness = (Thickness)newValue;
            domElement.boxSizing = "border-box";
            domElement.borderStyle = "solid"; //todo: see if we should put this somewhere else
            domElement.borderWidth = $"{thickness.Top.ToString(CultureInfo.InvariantCulture)}px {thickness.Right.ToString(CultureInfo.InvariantCulture)}px {thickness.Bottom.ToString(CultureInfo.InvariantCulture)}px {thickness.Left.ToString(CultureInfo.InvariantCulture)}px";
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
                });

        private static void CornerRadius_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var border = (Border)d;
            var cr = (CornerRadius)newValue;
            var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(border);
            domStyle.borderRadius = $"{cr.TopLeft.ToString(CultureInfo.InvariantCulture)}px {cr.TopRight.ToString(CultureInfo.InvariantCulture)}px {cr.BottomRight.ToString(CultureInfo.InvariantCulture)}px {cr.BottomLeft.ToString(CultureInfo.InvariantCulture)}px";
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
        /// Identifies the <see cref="Border.Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(Border),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom = Padding_MethodToUpdateDom
                });

        private static void Padding_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var border = (Border)d;

            if (!border.IsUnderCustomLayout)
            {
                var newPadding = (Thickness)newValue;
                var innerDomElement = border.INTERNAL_InnerDomElement;
                var styleOfInnerDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDomElement);

                // todo: if the container has a padding, add it to the margin
                styleOfInnerDomElement.boxSizing = "border-box";
                styleOfInnerDomElement.padding = $"{newPadding.Top.ToString(CultureInfo.InvariantCulture)}px {newPadding.Right.ToString(CultureInfo.InvariantCulture)}px {newPadding.Bottom.ToString(CultureInfo.InvariantCulture)}px {newPadding.Left.ToString(CultureInfo.InvariantCulture)}px";
            }
        }

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
    }
}
