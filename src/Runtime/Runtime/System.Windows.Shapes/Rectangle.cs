

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
using System.Globalization;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{

    /// <summary>
    /// Draws a rectangle shape, which can have a stroke and a fill.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <StackPanel x:Name="MyStackPanel">
    ///     <Rectangle Width="120" Height="40" Fill="#FFB9E58D" HorizontalAlignment="Left"/>
    /// </StackPanel>
    /// </code>
    /// <code lang="C#">
    /// Rectangle rect = new Rectangle() { Width = 60, Height = 30, Fill = new SolidColorBrush(Windows.UI.Colors.Blue) };
    /// MyStackPanel.Children.Add(rect);
    /// </code>
    /// </example>
    public partial class Rectangle : Shape
    {
        #region former implementation
        object _innerDiv; //we use this one so that de "display:table" do not keep us from having the borders.

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            ManageStrokeChanged();
            ManageStrokeThicknessChanged();
            OnFillChanged();
            OnRadiusChanged(); 
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            object div1;

            var outerStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
            outerStyle.boxSizing = "border-box";
            outerStyle.borderCollapse = "separate";

            var innerStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out _innerDiv);
            innerStyle.width = "100%";
            innerStyle.height = "100%";
            innerStyle.borderStyle = "solid";
            innerStyle.borderWidth = "0px";

            domElementWhereToPlaceChildren = null;
            return div1;
        }

        protected internal override void ManageStrokeChanged()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var outerStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);

                if (Stroke is SolidColorBrush)
                {
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).borderColor = ((SolidColorBrush)Stroke).INTERNAL_ToHtmlString();
                    var strokeThickness = this.StrokeThickness;
                    outerStyle.paddingBottom = strokeThickness * 2 + "px";
                    outerStyle.paddingRight = strokeThickness * 2 + "px";
                }
                else if (Stroke == null)
                {
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).borderColor = "";
                    outerStyle.paddingBottom = "0px";
                    outerStyle.paddingRight = "0px";
                }
                else
                {
                    throw new NotSupportedException("The specified brush is not supported.");
                }
            }
        }

        protected override void OnFillChanged()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this)) return;

            var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv);
            if (Fill is SolidColorBrush)
            {
                domStyle.backgroundColor = ((SolidColorBrush)Fill).INTERNAL_ToHtmlString();
            }

            string cssSize = "auto";
            switch (Stretch)
            {
                case Stretch.Fill: cssSize = "100% 100%"; break;
                case Stretch.Uniform: cssSize = "contain"; break;
                case Stretch.UniformToFill: cssSize = "cover"; break;
            }

            domStyle.backgroundSize = cssSize;
            domStyle.backgroundRepeat = "no-repeat";
            domStyle.backgroundPosition = "center center";
        }

        protected internal override void ManageStrokeThicknessChanged()
        {
            if (Stroke == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this)) return ;
            var innerStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv);
            innerStyle.borderWidth = StrokeThickness + "px";
            var outerStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
            outerStyle.paddingBottom = StrokeThickness * 2 + "px";
            outerStyle.paddingRight = StrokeThickness * 2 + "px";
        }

        

        #endregion


        /// <summary>
        /// Gets or sets the x-axis radius of the ellipse that is used to round the corners
        /// of the rectangle.
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Rectangle.RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty = 
            DependencyProperty.Register(
                nameof(RadiusX), 
                typeof(double), 
                typeof(Rectangle), 
                new PropertyMetadata(0d, RadiusChanged));

        /// <summary>
        /// Gets or sets the y-axis radius of the ellipse that is used to round the corners
        /// of the rectangle.
        /// The default is 0.
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }
        
        /// <summary>
        /// Identifies the <see cref="Rectangle.RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty = 
            DependencyProperty.Register(
                nameof(RadiusY), 
                typeof(double), 
                typeof(Rectangle), 
                new PropertyMetadata(0d, RadiusChanged));

        private static void RadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Rectangle)d).OnRadiusChanged();
        }

        private void OnRadiusChanged()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this)) return;
            var radx = ((int)Math.Round(RadiusX)).ToString(CultureInfo.InvariantCulture);
            var rady = ((int)Math.Round(RadiusY)).ToString(CultureInfo.InvariantCulture);
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).borderRadius = $"{radx}px {rady}px {radx}px {rady}px";
        }
    }
}
