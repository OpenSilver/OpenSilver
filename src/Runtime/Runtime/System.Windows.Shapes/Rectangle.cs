

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
        static Rectangle()
        {
            Shape.StretchProperty.OverrideMetadata(typeof(Rectangle), new PropertyMetadata(Stretch.Fill, Shape.Stretch_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        }

        #region former implementation
        //dynamic _innerDiv; //we use this one so that de "display:table" do not keep us from having the borders.

        ///// <summary>
        ///// Initializes a new instance of the Rectangle class.
        ///// </summary>
        //public Rectangle() : base() { }

        //protected internal override void INTERNAL_OnAttachedToVisualTree()
        //{
        //    ManageStrokeChanged();
        //    ManageStrokeThicknessChanged();
        //}

        //public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        //{
        //    object div1;

        //    dynamic outerStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out div1);
        //    outerStyle.boxSizing = "border-box";
        //    outerStyle.borderCollapse = "separate";

        //    dynamic innerStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", div1, this, out _innerDiv);
        //    innerStyle.width = "100%";
        //    innerStyle.height = "100%";
        //    innerStyle.borderStyle = "solid";
        //    innerStyle.borderWidth = "0px";

        //    domElementWhereToPlaceChildren = null;
        //    return div1;
        //}

        //protected internal override void ManageStrokeChanged()
        //{
        //    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        //    {
        //        var outerStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);

        //        if (Stroke is SolidColorBrush)
        //        {
        //            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).borderColor = ((SolidColorBrush)Stroke).INTERNAL_ToHtmlString();
        //            var strokeThickness = this.StrokeThickness;
        //            outerStyle.paddingBottom = strokeThickness * 2 + "px";
        //            outerStyle.paddingRight = strokeThickness * 2 + "px";
        //        }
        //        else if (Stroke == null)
        //        {
        //            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).borderColor = "";
        //            outerStyle.paddingBottom = "0px";
        //            outerStyle.paddingRight = "0px";
        //        }
        //        else
        //        {
        //            throw new NotSupportedException("The specified brush is not supported.");
        //        }
        //    }
        //}

        //protected internal override void ManageStrokeThicknessChanged()
        //{
        //    if (Stroke != null)
        //    {
        //        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        //        {
        //            dynamic innerStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv);
        //            innerStyle.borderWidth = StrokeThickness + "px";

        //            dynamic outerStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
        //            outerStyle.paddingBottom = StrokeThickness * 2 + "px";
        //            outerStyle.paddingRight = StrokeThickness * 2 + "px";
        //        }
        //    }
        //}

        //internal override void ManageFill_Changed()
        //{
        //    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        //        if (Fill is SolidColorBrush)
        //            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_innerDiv).backgroundColor = ((SolidColorBrush)Fill).INTERNAL_ToHtmlString();
        //}

        #endregion

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(this, parentRef, out _canvasDomElement, out domElementWhereToPlaceChildren);

            //domElementWhereToPlaceChildren = null;
            //var canvas = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("canvas", parentRef, this);
            //return canvas;
        }

        //protected internal override void INTERNAL_OnAttachedToVisualTree()
        //{
        //    ScheduleRedraw();
        //}

        protected internal override void Redraw()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                double xOffsetToApplyBeforeMultiplication;
                double yOffsetToApplyBeforeMultiplication;
                double xOffsetToApplyAfterMultiplication;
                double yOffsetToApplyAfterMultiplication;
                double sizeX;
                double sizeY;
                double horizontalMultiplicator;
                double verticalMultiplicator;
                Size shapeActualSize;
                Shape.GetShapeInfos(this, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out sizeX, out sizeY, out horizontalMultiplicator, out verticalMultiplicator, out shapeActualSize);

                ApplyMarginToFixNegativeCoordinates(new Point());
                if (Stretch == Media.Stretch.None)
                {
                    ApplyMarginToFixNegativeCoordinates(_marginOffsets);
                }

                var context = INTERNAL_HtmlDomManager.Get2dCanvasContext(_canvasDomElement);
                double x = xOffsetToApplyBeforeMultiplication + xOffsetToApplyAfterMultiplication;
                double y = yOffsetToApplyBeforeMultiplication + yOffsetToApplyAfterMultiplication;
                double width = sizeX;
                double height = sizeY;

                // Values greater than half the rectangle's width/height are treated as though equal to half the rectangle's width/height.
                // Negative values are treated as positive values.
                double radiusX = Math.Abs(RadiusX) > width / 2 ? width / 2 : Math.Abs(RadiusX);
                double radiusY = Math.Abs(RadiusY) > height / 2 ? height / 2 : Math.Abs(RadiusY);

                if (radiusX == 0 || radiusY == 0)
                {
                    // Just draw a rectangle if one of radiuses is 0
                    context.rect(x, y, width, height);
                }
                else
                {
                    context.moveTo(x + radiusX, y);
                    context.lineTo(x + width - radiusX, y);
                    context.ellipse(x + width - radiusX, y + radiusY, radiusX, radiusY, Math.PI, Math.PI / 2, Math.PI);
                    context.lineTo(x + width, y + height - radiusY);
                    context.ellipse(x + width - radiusX, y + height - radiusY, radiusX, radiusY, Math.PI, Math.PI, 3 * Math.PI / 2);
                    context.lineTo(x + radiusX, y + height);
                    context.ellipse(x + radiusX, y + height - radiusY, radiusX, radiusY, Math.PI, 3 * Math.PI / 2, 2 * Math.PI);
                    context.lineTo(x, y + radiusY);
                    context.ellipse(x + radiusX, y + radiusY, radiusX, radiusY, Math.PI, 2 * Math.PI, Math.PI / 2);
                }

                //todo: make sure the parameters below are correct.
                Shape.DrawFillAndStroke(this, "evenodd", xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, xOffsetToApplyAfterMultiplication + sizeX, yOffsetToApplyAfterMultiplication + sizeY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);

                //context.fill("evenodd"); //note: remember: always fill before stroke, otherwise the filling will hide the stroke.
                //if (StrokeThickness > 0 && Stroke != null)
                //{
                //    context.stroke();
                //}
            }
        }

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
                new PropertyMetadata(0d));

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
                new PropertyMetadata(0d));
    }
}
