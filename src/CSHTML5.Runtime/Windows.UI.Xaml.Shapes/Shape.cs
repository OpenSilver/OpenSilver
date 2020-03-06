

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Media;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{
    /// <summary>
    /// Provides a base class for shape elements, such as Ellipse, Polygon, and Rectangle.
    /// </summary>
    public abstract partial class Shape : FrameworkElement
    {
        internal protected object _canvasDomElement;
        internal protected Point _marginOffsets;//this is for the case where we have negative positions in the shapes.
        bool _isListeningToAncestorsVisibilityChanged; // This is for the case where the HTML canvas is hidden when we want to draw (due to a "Display:none" on one of the ancestors), so we need to wait for the HTML canvas to become visible in order to draw.

        //No need for the overrideMetadata on width and height because they call frameworkElement.HandleSizeChanged(); which causes a redraw.
        //static Shape()
        //{
        //    PropertyMetadata propertyMetadata = FrameworkElement.WidthProperty.GetTypeMetaData(typeof(FrameworkElement)).Clone();
        //    propertyMetadata.PropertyChangedCallback = Shape.Width_Changed;
        //    Shape.WidthProperty.OverrideMetadata(typeof(Shape), propertyMetadata);
        //}

#if REVAMPPOINTEREVENTS
        internal override bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
            return Fill != null;
        }
#endif

        /// <summary>
        /// Provides base class initialization behavior for Shape derived classes.
        /// </summary>
        protected Shape()
        {
            SizeChanged += Shape_SizeChanged;

            // Fix the issue with Grid implemented using html <table>, which is unable to force the size of its children:
            if (!Grid_InternalHelpers.isCSSGridSupported())
            {
                Window.Current.SizeChanged += Current_SizeChanged; //this is to make sure that the canvas does not block redimensionning when changing the window size.
                //Note: this still does not solve the issue we have when shrinking a parent element of the shape when the shape is stretched.
                //To reproduce the issue:
                // <Grid Width="300">
                //   <Rectangle Width="Auto" Stretch="Fill"/>
                // </Grid>
                // Then, after this has been drawn, programmatically reduce the size of the border => the rectangle will not become smaller because its inner html <canvas> has a fixed size and prevents its container from being smaller.
            }
        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            // Only cases where resizing the window MAY resize the shape is if the shape is "Stretched" AND it does not have a size in pixels defined:
            if ((this.HorizontalAlignment == HorizontalAlignment.Stretch && double.IsNaN(this.Width))
                || (this.VerticalAlignment == VerticalAlignment.Stretch && double.IsNaN(this.Height)))
            {
                ScheduleRedraw();
            }
        }

        void Shape_SizeChanged(object sender, SizeChangedEventArgs e) //Note: this is called when adding the shape into the visual tree.
        {
            ScheduleRedraw();
        }

        /// <summary>
        /// Gets or sets the Brush that specifies how to paint the interior of the shape.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        /// <summary>
        /// Identifies the Fill dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Shape), new PropertyMetadata(null, Fill_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });
        static void Fill_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shape = (Shape)d;
#if REVAMPPOINTEREVENTS
            INTERNAL_UpdateCssPointerEvents(shape);
#endif
            shape.ScheduleRedraw();
        }

        //
        // Summary:
        //     Gets a value that represents a Transform that is applied to the geometry
        //     of a Shape before it is drawn.
        //
        // Returns:
        //     A Transform that is applied to the geometry of a Shape before it is drawn.
        //public Transform GeometryTransform { get; }


        /// <summary>
        /// Gets or sets a Stretch enumeration value that describes how the shape fills
        /// its allocated space.
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        /// <summary>
        /// Identifies the Stretch dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Shape), new PropertyMetadata(Stretch.None, Stretch_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });
        internal protected static void Stretch_Changed(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            //note: Stretch is actually more implemented in the Redraw method of the classes that inherit from shape (Line, Ellipse, Path, Rectangle)

            //todo: remove this method?

            var shape = (Shape)i;
            Stretch newValue = (Stretch)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(shape))
            {
                dynamic shapeDom = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(shape);

                if (double.IsNaN(shape.Width))
                {
                    switch (newValue)
                    {
                        case Stretch.None:
                            shapeDom.width = "auto";
                            break;
                        case Stretch.Fill:
                            shapeDom.width = "100%";
                            break;
                        case Stretch.Uniform: //todo: make it work when the image needs to be made bigger to fill the container
                            shapeDom.maxWidth = "100%";
                            break;
                        case Stretch.UniformToFill: //todo: add a negative margin top and left so that the image is centered 
                            shapeDom.minWidth = "100%";
                            break;
                        default:
                            break;
                    }
                }
                if (double.IsNaN(shape.Height))
                {
                    switch (newValue)
                    {
                        case Stretch.None:
                            shapeDom.height = "auto";
                            break;
                        case Stretch.Fill:
                            shapeDom.height = "100%";
                            break;
                        case Stretch.Uniform: //todo: make it work when the image needs to be made bigger to fill the container
                            shapeDom.maxHeight = "100%";
                            break;
                        case Stretch.UniformToFill: //todo: add a negative margin top and left so that the image is centered 
                            shapeDom.minHeight = "100%";
                            break;
                        default:
                            break;
                    }
                }

                shape.ScheduleRedraw();
            }
        }


        /// <summary>
        /// Gets or sets the Brush that specifies how the Shape outline is painted.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Shape), new PropertyMetadata(null, Stroke_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Stroke_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Shape)d).ManageStrokeChanged();
        }

        internal protected virtual void ManageStrokeChanged()
        {
            ScheduleRedraw();
        }


        /// <summary>
        /// Gets or sets the width of the Shape stroke outline.
        /// </summary>
        public double StrokeThickness
        {
            get { return Convert.ToDouble(GetValue(StrokeThicknessProperty)); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Shape), new PropertyMetadata(1d, StrokeThickness_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void StrokeThickness_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Shape)d).ManageStrokeThicknessChanged();
        }

        internal protected virtual void ManageStrokeThicknessChanged()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(_canvasDomElement);
                context.lineWidth = StrokeThickness + "";
                ScheduleRedraw();
            }
        }

        bool _redrawPending = false;
        /// <summary>
        /// This is used to redraw only once instead of on every property change.
        /// </summary>
        internal void ScheduleRedraw()
        {
            if (!_redrawPending) // This ensures that the "BeginInvoke" method is only called once, and it is not called again until its delegate has been executed.
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    _redrawPending = true;
                    INTERNAL_DispatcherHelpers.QueueAction(() => // We use a dispatcher to avoid redrawing every time that a dependency property is set (the result is as if we waited for the last property to be set). We use "INTERNAL_DispatcherHelpers.QueueAction" instead of "Dispatcher.BeginInvoke" because it has better performance than calling Dispatcher.BeginInvoke directly.
                        {
                            _redrawPending = false;

                            // We check whether the Shape is visible in the HTML DOM tree, because if the HTML canvas is hidden (due to a "Dispay:none" on one of the ancestors), we cannot draw on it (this can be seen by hiding a canvas, drawing, and then showing it: it will appear empty):
                            if (INTERNAL_VisibilityChangedNotifier.IsElementVisible(this))
                            {
                                Redraw();

                                // Stop listening to the ancestors' Visibility_Changed, if it was listening:
                                if (_isListeningToAncestorsVisibilityChanged)
                                    INTERNAL_VisibilityChangedNotifier.StopListeningToAncestorsVisibilityChanged(this);
                                _isListeningToAncestorsVisibilityChanged = false;
                            }
                            else
                            {
                                // We listen to the Visibility_Changed event of the ancestors so as try again if a parent becomes visible:
                                if (!_isListeningToAncestorsVisibilityChanged)
                                {
                                    _isListeningToAncestorsVisibilityChanged = true;
                                    INTERNAL_VisibilityChangedNotifier.StartListeningToAncestorsVisibilityChanged(this,
                                    () =>
                                    {
                                        // Stop listening to the ancestors' Visibility_Changed:
                                        if (_isListeningToAncestorsVisibilityChanged)
                                            INTERNAL_VisibilityChangedNotifier.StopListeningToAncestorsVisibilityChanged(this);
                                        _isListeningToAncestorsVisibilityChanged = false;

                                        // Try again:
                                        ScheduleRedraw();
                                    });
                                }
                            }
                        });
                }
            }
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            // Stop listening to the ancestors' Visibility_Changed event, if it was listening:
            if (_isListeningToAncestorsVisibilityChanged)
                INTERNAL_VisibilityChangedNotifier.StopListeningToAncestorsVisibilityChanged(this);
            _isListeningToAncestorsVisibilityChanged = false;

            base.INTERNAL_OnDetachedFromVisualTree();
        }

        internal protected virtual void Redraw() { } //implemented in classes that inherit from Shape.


        public void Refresh() // Public method that end-users are supposed to call if they change the geometry and want to redraw the Path.
        {
            if (this is Path)
            {
                ((Path)this).RefreshChildrenParent();
            }
            this.ScheduleRedraw();
        }

        /// <summary>
        /// Gets or sets a PenLineCap enumeration value that describes the Shape at the
        /// start of a Stroke. The default is Flat.
        /// </summary>
        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeStartLineCap dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty =
            DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(Shape), new PropertyMetadata(PenLineCap.Flat) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        /// <summary>
        /// Gets or sets a PenLineCap enumeration value that describes the Shape at the
        /// end of a line. The default is Flat.
        /// </summary>
        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeEndLineCap dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(Shape), new PropertyMetadata(PenLineCap.Flat) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });


        /// <summary>
        /// Gets or sets a PenLineJoin enumeration value that specifies the type of join
        /// that is used at the vertices of a Shape. The default value is Miter.
        /// </summary>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeLineJoin dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(Shape), new PropertyMetadata(PenLineJoin.Miter) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });



        /// <summary>
        /// Gets or sets a limit on the ratio of the miter length to half the StrokeThickness
        /// of a Shape element. This value is always a positive number that is greater than
        /// or equal to 1.
        /// </summary>
        public double StrokeMiterLimit
        {
            get { return (double)GetValue(StrokeMiterLimitProperty); }
            set { SetValue(StrokeMiterLimitProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeMiterLimit dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(Shape), new PropertyMetadata(0d) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        internal static void GetShapeInfos(Shape shape, out double xOffsetToApplyBeforeMultiplication, out double yOffsetToApplyBeforeMultiplication, out double xOffsetToApplyAfterMultiplication, out double yOffsetToApplyAfterMultiplication, out double sizeX, out double sizeY, out double horizontalMultiplicator, out double verticalMultiplicator, out Size shapeActualSize)
        {
            double width = shape.Width;
            double height = shape.Height;
            double strokeThickness = shape.StrokeThickness;
            Stretch strech = shape.Stretch;

            double minX = 0;
            double maxX = double.IsNaN(width) ? 0 : width;
            double minY = 0;
            double maxY = double.IsNaN(height) ? 0 : height;

            INTERNAL_ShapesDrawHelpers.PrepareStretch(shape, shape._canvasDomElement, minX, maxX, minY, maxY, strech, out shapeActualSize);

            if (shape.Stroke == null)
            {
                strokeThickness = 0;
            }

            INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(shape, strokeThickness, minX, maxX, minY, maxY, strech, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out shape._marginOffsets);

            if (strech == Stretch.None)
            {
                Thickness margin = shape.Margin;
                Margin_MethodToUpdateDom(shape, new Thickness(margin.Left + shape._marginOffsets.X,
                                                              margin.Top + shape._marginOffsets.Y,
                                                              margin.Right, 
                                                              margin.Bottom));
            }
            sizeX = Math.Max(1, maxX - minX) * horizontalMultiplicator;
            sizeY = Math.Max(1, maxY - minY) * verticalMultiplicator;
        }

        /// <summary>
        /// This method calls the fill and stroke methods while isolating the possible transformations that the GetHtmlBrush does.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="fillRule"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="horizontalMultiplicator"></param>
        /// <param name="verticalMultiplicator"></param>
        /// <param name="xOffsetToApplyBeforeMultiplication"></param>
        /// <param name="yOffsetToApplyBeforeMultiplication"></param>
        /// <param name="shapeActualSize"></param>
        internal static void DrawFillAndStroke(Shape shape, string fillRule, double minX, double minY, double maxX, double maxY, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, Size shapeActualSize)
        {
            object context = CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", shape._canvasDomElement); //Note: we do not use INTERNAL_HtmlDomManager.Get2dCanvasContext here because we need to use the result in ExecuteJavaScript, which requires the value to come from a call of ExecuteJavaScript.

            //we remove the previous drawing:
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.clearRect(0, 0, $1, $2)", context, shapeActualSize.Width, shapeActualSize.Height); //todo: make sure this is correct, especially when shrinking the ellipse (width and height may already have been applied).


            //context.save() for the fill
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.save()", context);

            //FillStyle:
            double opacity = shape.Fill == null ? 1 : shape.Fill.Opacity;
            object fillValue = GetHtmlBrush(shape, shape.Fill, opacity, minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);

            if (fillValue != null)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.fillStyle = $1", context, fillValue);
            }
            else
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.fillStyle = ''", context);
            }

            if (shape.Fill != null)
            {
                //Note: I am not sure that calling fill("evenodd") works properly in Edge, the canvasRenderingContext2d has a msfillRule property which might be the intended way to use the evenodd rule when in Edge.
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.fill($1)", context, fillRule); 
            }

            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.restore();
$0.save()", context); //restore after fill then save before stroke

            //stroke
            object strokeValue = null;
            opacity = shape.Stroke == null ? 1 : shape.Stroke.Opacity;
            strokeValue = GetHtmlBrush(shape, shape.Stroke, opacity, minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);

            double strokeThickness = shape.StrokeThickness;
            if (shape.Stroke == null)
            {
                strokeThickness = 0;
            }

            //we set the colors and style of the shape:
            if (strokeValue != null && strokeThickness > 0)
            {
                double thickness = shape.StrokeThickness;
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.strokeStyle = $1", context, strokeValue);
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.lineWidth = $1", context, strokeThickness);
                DoubleCollection strokeDashArray = shape.StrokeDashArray;
                if (strokeDashArray != null)
                {
                    if (CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        //todo: put a message saying that it doesn't work in certain browsers (maybe use a static boolean to put that message only once)
                    }
                    else
                    {
                        object options = CSHTML5.Interop.ExecuteJavaScript(@"new Array()");
                        for (int i = 0; i < strokeDashArray.Count; ++i)
                        {
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = $2;
", options, i, strokeDashArray[i] * thickness);
                        }


                        //string str = "[";
                        //bool first = true;
                        //foreach (double length in strokeDashArray)
                        //{
                        //    if (first)
                        //    {
                        //        first = false;
                        //    }
                        //    else
                        //    {
                        //        str += ",";
                        //    }
                        //    str += length;
                        //}
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"
if ($0.setLineDash)
    $0.setLineDash($1)", context, options);
                        //context.setLineDash(str + "]");
                    }
                }

                if (shape.Stroke != null && shape.StrokeThickness > 0)
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.stroke()", context);
                    //context.stroke(); //todo: test if this won't cause a problem with a potential transparent brush (drawing multiple times so the shape is less transparent).
                }

                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.restore()", context);
            }
            //context.strokeStyle = strokeValue;

            //context.fillStyle = fillValue;
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.lineWidth = $1", context, shape.StrokeThickness + ""); //todo: make sure this is correct, especially when shrinking the ellipse (width and height may already have been applied).
            //context.lineWidth = shape.StrokeThickness + "";
        }


        internal static object GetHtmlBrush(Shape shape, Brush brush, double opacity, double minX, double minY, double maxX, double maxY, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, Size shapeActualSize)
        {
            object context = CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", shape._canvasDomElement); //Note: we do not use INTERNAL_HtmlDomManager.Get2dCanvasContext here because we need to use the result in ExecuteJavaScript, which requires the value to come from a call of ExecuteJavaScript.


            object returnValue = null;
            if (brush == null || brush is SolidColorBrush) //todo: make sure we want the same behaviour when it is null and when it is a SolidColorBrush (basically, check if null means default value)
            {
                if (brush != null) //if stroke is null, we want to set it as an empty string, otherwise, it is a SolidColorBrush and we want to get its color.
                {
                    returnValue = ((SolidColorBrush)brush).INTERNAL_ToHtmlString();
                    //CSHTML5.Interop.ExecuteJavaScript(@"$0.fillStyle = $1", context, fillAsString);
                }
            }
            else if (brush is LinearGradientBrush)
            {
                LinearGradientBrush fillAsLinearGradientBrush = (LinearGradientBrush)brush;

                double x0 = fillAsLinearGradientBrush.StartPoint.X;
                double x1 = fillAsLinearGradientBrush.EndPoint.X;
                double y0 = fillAsLinearGradientBrush.StartPoint.Y;
                double y1 = fillAsLinearGradientBrush.EndPoint.Y;

                if (fillAsLinearGradientBrush.MappingMode == BrushMappingMode.RelativeToBoundingBox)
                {
                    x0 = fillAsLinearGradientBrush.StartPoint.X * (xOffsetToApplyBeforeMultiplication + maxX - minX) * horizontalMultiplicator + minX;
                    x1 = fillAsLinearGradientBrush.EndPoint.X * (xOffsetToApplyBeforeMultiplication + maxX - minX) * horizontalMultiplicator + minX;
                    y0 = fillAsLinearGradientBrush.StartPoint.Y * (yOffsetToApplyBeforeMultiplication + maxY - minY) * verticalMultiplicator + minY;
                    y1 = fillAsLinearGradientBrush.EndPoint.Y * (yOffsetToApplyBeforeMultiplication + maxY - minY) * verticalMultiplicator + minY;
                }

                if (fillAsLinearGradientBrush.SpreadMethod == GradientSpreadMethod.Pad)
                {
                    returnValue = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.createLinearGradient($1,$2,$3,$4)", context, x0, y0, x1, y1);

                    foreach (GradientStop gradientStop in fillAsLinearGradientBrush.GradientStops)
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.addColorStop($1,$2)", returnValue, gradientStop.Offset, gradientStop.Color.INTERNAL_ToHtmlString(opacity));
                    }
                }
                else
                {

                    //*******************************
                    //example in js:
                    //*******************************
                    //var c = document.getElementById("myCanvas");
                    //var ctx = c.getContext("2d");

                    //var gradient = ctx.createLinearGradient(0, 0, 150, 0);
                    //gradient.addColorStop(0, "black");
                    //gradient.addColorStop(0.5, "red");
                    //gradient.addColorStop(1, "white");
                    //var cvas = document.createElement('canvas');
                    //cvas.width = 50;
                    //cvas.height = 50;
                    //cvas.style.width = 50;
                    //cvas.style.height = 50;
                    //var ctx2 = cvas.getContext("2d");
                    //ctx2.fillStyle = gradient;
                    //ctx2.fillRect(0, 0, 50, 50);

                    //ctx.rect(0, 0, 150, 100);

                    //ctx.save();
                    //ctx.rotate(20 * Math.PI / 180); //to apply the angle to the gradient (we cannot simply a
                    //ctx.translate(10, 0); //to make it "touch" the points defined.
                    //var pat = ctx.createPattern(cvas, 'repeat');
                    //ctx.fillStyle = pat;
                    //ctx.fill();
                    //ctx.restore();
                    //*******************************
                    //  end of the example
                    //*******************************

                    //to repeat the gradient, we need to create another canvas which we will use in a createPattern method.
                    //  the temporary canvas will have the gradient applied to it
                    // what we need:
                    //      the angle
                    //      the distance between StartPoint and EndPoint (the actual distance, not a relative distance)
                    //      the offset we need to apply so that the points actually have their colors at the proper position.
                    //  I think that's it.

                    double angle;
                    if (fillAsLinearGradientBrush.MappingMode == BrushMappingMode.Absolute)
                    {
                        angle = Math.Atan2(y1 - y0, x1 - x0);
                    }
                    else
                    {
                        angle = Math.Atan2((fillAsLinearGradientBrush.EndPoint.Y - fillAsLinearGradientBrush.StartPoint.Y) * (maxY - minY), (fillAsLinearGradientBrush.EndPoint.X - fillAsLinearGradientBrush.StartPoint.X) * (maxX - minX)); //I think that this is the correct angle.
                    }

                    double distance = Math.Sqrt(Math.Pow((x1 - x0), 2) + Math.Pow((y1 - y0), 2));


                    //now all we need is the offset:
                    double m = (y1 - y0) / (x1 - x0); //this the slope of the line that passes through StartPoint and EndPoint
                    double perpM = -1 / m; // this is the slope of any line perpendicular to the one defined above.

                    //the line passing through StartPoint is so that y0 = perpM * x0 + k
                    // ==> k = y0 - x0 * perpM
                    double k = y0 - x0 * perpM;

                    //we want the intersection between the line for which we determined k and the one with the equation y = mx:
                    // (1) yInter = m * xInter
                    // (2) yInter = perpM * xInter + k;
                    // replacing in (2):
                    // m * xInter = perpM * xInter + k;
                    // ==> xInter * ( m - perpM) = k;
                    // ==> xInter = k / (m - perpM)
                    // the offset is the distance with the point (0,0):
                    // offset = sqrt(xInter ^ 2 + yInter ^ 2)
                    // offset = sqrt(xInter ^ 2 + (mxInter) ^ 2)
                    // offset = sqrt(xInter ^ 2 * (1 + m ^ 2))
                    // offset = xInter * sqrt(1 + m ^ 2)

                    double xInter = k / (m - perpM);
                    double offset = xInter * Math.Sqrt(1 + m * m);

                    double shapeHeight = shapeActualSize.Height;
                    double shapeWidth = shapeActualSize.Width;
                    double tempCanvasHeight = shapeHeight > shapeWidth ? shapeHeight : shapeWidth;


                    var canvas = CSHTML5.Interop.ExecuteJavaScript(@"document.createElement('canvas')");
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.setAttribute('width', $1);
$0.setAttribute('height', $2);
$0.style.width = $1;
$0.style.height = $2", canvas, distance + "px", tempCanvasHeight + "px");

                    var ctx = CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d')", canvas);
                    var gradient = CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.createLinearGradient(0, 0, $1, 0)", ctx, distance);

                    foreach (GradientStop gradientStop in fillAsLinearGradientBrush.GradientStops)
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.addColorStop($1,$2)", gradient, gradientStop.Offset, gradientStop.Color.INTERNAL_ToHtmlString(opacity));
                    }

                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$2.fillStyle = $3;
$2.fillRect(0, 0, $4, $7);

if($0 != undefined && $0 != null) {
var context = $0.getContext('2d');
//context.save();
context.rotate($5);
context.translate($6, 0);
//var pat = context.createPattern($1, 'repeat');

//context.fillStyle = pat;
//context.fill();
//context.restore();
}
", shape._canvasDomElement, canvas, ctx, gradient, distance, angle, offset, tempCanvasHeight);


                    returnValue = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.createPattern($1, 'repeat')", context, canvas);
                }
            }
            else if (brush is RadialGradientBrush)
            {

//                  let's assume we try to incorporate RadiusX = 75, RadiusY = 50
//                  ctx.save();
//                  ctx.scale(75 / 50, 1);
//                  var grd = ctx.createRadialGradient(75, 125, 0, 75, 75, 50);
////									                x0,y0,r0,  x1,y1,r1	
//                  grd.addColorStop(0, "blue");
//                  grd.addColorStop(0.1, "white");
//                  grd.addColorStop(0.2, "purple");
//                  grd.addColorStop(0.99, "purple");
//                  grd.addColorStop(1, "black");
//                  ctx.fillStyle = grd;
//                  ctx.fillRect(10, 10, 300, 300);
//                  ctx.restore();
                //Note: (x1,y1) are the coordinates of the center of the outermost circle (corresponds to Center, while (x0,y0) are the coordinates of the center of the innermost circle (Corresponds to the GradientOrigin, the innermost circle always has a radius of 0).

                RadialGradientBrush fillAsRadialGradientBrush = (RadialGradientBrush)brush;

                double radiusScaling = fillAsRadialGradientBrush.RadiusX / fillAsRadialGradientBrush.RadiusY;
                double centerX = fillAsRadialGradientBrush.Center.X;
                double gradientOriginX = fillAsRadialGradientBrush.GradientOrigin.X;
                double centerY = fillAsRadialGradientBrush.Center.Y;
                double gradientOriginY = fillAsRadialGradientBrush.GradientOrigin.Y;
                double r = fillAsRadialGradientBrush.RadiusY;

                if (fillAsRadialGradientBrush.MappingMode == BrushMappingMode.RelativeToBoundingBox)
                {
                    centerX = fillAsRadialGradientBrush.Center.X * (xOffsetToApplyBeforeMultiplication + maxX - minX) * horizontalMultiplicator + minX;
                    gradientOriginX = fillAsRadialGradientBrush.GradientOrigin.X * (xOffsetToApplyBeforeMultiplication + maxX - minX) * horizontalMultiplicator + minX;
                    centerY = fillAsRadialGradientBrush.Center.Y * (yOffsetToApplyBeforeMultiplication + maxY - minY) * verticalMultiplicator + minY;
                    gradientOriginY = fillAsRadialGradientBrush.GradientOrigin.Y * (yOffsetToApplyBeforeMultiplication + maxY - minY) * verticalMultiplicator + minY;
                    r = r * (yOffsetToApplyBeforeMultiplication + maxY - minY) * verticalMultiplicator + minY;
                }

                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.scale($1,1)", context, radiusScaling);
                if (fillAsRadialGradientBrush.SpreadMethod == GradientSpreadMethod.Pad)
                {
                    returnValue = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.createRadialGradient($1,$2,0,$3,$4,$5)", context, gradientOriginX, gradientOriginY, centerX, centerY, r);

                    foreach (GradientStop gradientStop in fillAsRadialGradientBrush.GradientStops.OrderBy((element) => { return element.Offset; }))
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.addColorStop($1,$2)", returnValue, gradientStop.Offset, gradientStop.Color.INTERNAL_ToHtmlString(opacity));
                    }
                }
                else
                {
                    //Note: I didn't find any way to make an actually repeating radial gradient brush in a canvas so we are going to cheat by repeating the gradientBrushes while applying corrections to make it work.
                    //corrections are as follow for N repetitions:
                    //var grd = ctx.createRadialGradient(GradientOriginX, GradientOriginY, 0, CenterX, CenterY - (N-1) * Radius, Radius + (N-1) * Radius);
                    // for each gradient stop, change the Offset to offset/N
                    // then add the same gradients with an additional offset of 1/N, then 2/N until N-1 /1
                    //Note: while writing this, I realized the correction on CenterX and CenterY depended on the angle to GradientOrigin, so yeah!... fun times ahead!
                    int repeatingTimes = 5; //empirically chosen.

                    //calculating the portion of the correction that must be applied to x and y separately:
                    double xVariation = Math.Abs(fillAsRadialGradientBrush.Center.X - fillAsRadialGradientBrush.GradientOrigin.X);
                    double yVariation = Math.Abs(fillAsRadialGradientBrush.Center.Y - fillAsRadialGradientBrush.GradientOrigin.Y);
                    double xCorrection = 0;
                    double yCorrection = 0;
                    if (xVariation != 0 || yVariation != 0)
                    {
                        double vectorSize = Math.Sqrt(Math.Pow(xVariation, 2) + Math.Pow(yVariation, 2));
                        xCorrection = xVariation / vectorSize;
                        yCorrection = yVariation / vectorSize;
                    }
                    int additionalRepetitions = repeatingTimes - 1;

                    returnValue = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.createRadialGradient($1,$2,0,$3,$4,$5)", context, gradientOriginX, gradientOriginY, centerX - xCorrection * additionalRepetitions * r, centerY - yCorrection * additionalRepetitions * r, r + additionalRepetitions * r);

                    var orderedGradients = fillAsRadialGradientBrush.GradientStops.OrderBy((element) => { return element.Offset; });
                    double repetitionOffset = 1.0 / repeatingTimes;

                    for(int i = 0; i< repeatingTimes; ++i)
                    {
                        foreach (GradientStop gradientStop in orderedGradients)
                        {
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.addColorStop($1,$2)", returnValue, gradientStop.Offset / repeatingTimes + i * repetitionOffset, gradientStop.Color.INTERNAL_ToHtmlString(opacity));
                        }
                    }
                }



















            }
            else
            {
                throw new NotSupportedException("The specified brush is not supported.");
            }
            return returnValue;
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("return")]
#else
        [External]
#endif
        void ExecuteJS_SimulatorOnly(string javascript, object canvasDomElement)
        {
            string str = string.Format(@"
{0}
var cvas = document.getElementById(""{1}"");
if(cvas != undefined && cvas != null) {{
var context = cvas.getContext('2d');
context.save();
context.rotate(angle);
context.translate(offset, 0);
var pat = context.createPattern(canvas, 'repeat');

context.fillStyle = pat;
context.fill();
context.restore();
}}", javascript, ((INTERNAL_HtmlDomElementReference)canvasDomElement).UniqueIdentifier);
            INTERNAL_HtmlDomManager.ExecuteJavaScript(str);
        }

        /// <summary>
        /// Gets or sets a collection of Double values that indicates the pattern of
        /// dashes and gaps that is used to outline shapes.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get
            {
                var collection = (DoubleCollection)GetValue(StrokeDashArrayProperty);
                if (collection == null)
                {
                    collection = new DoubleCollection();
                    SetValue(StrokeDashArrayProperty, collection);
                }
                return collection;
            }
            set { SetValue(StrokeDashArrayProperty, value); }
        }
        /// <summary>
        /// Identifies the StrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(Shape), new PropertyMetadata(null, StrokeDashArray_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void StrokeDashArray_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shape shape = (Shape)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(shape))
            {
                shape.ScheduleRedraw();
                //if (shape._canvasDomElement != null)
                //{
                //    dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(shape._canvasDomElement);
                //    string str = "[";
                //    bool first = true;
                //    foreach(double length in shape.StrokeDashArray)
                //    {
                //        if(first)
                //        {
                //            first = false;
                //        }
                //        else
                //        {
                //            str += ",";
                //        }
                //        str += length;
                //    }
                //    context.setLineDash = str + "]";
                //}
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the distance within the dash pattern
        /// where a dash begins.
        /// </summary>
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the StrokeDashOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(Shape), new PropertyMetadata(0d, StrokeDashOffset_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void StrokeDashOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shape shape = (Shape)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(shape))
            {
                if (shape._canvasDomElement != null)
                {
                    dynamic context = INTERNAL_HtmlDomManager.Get2dCanvasContext(shape._canvasDomElement);
                    context.lineDashOffset = shape.StrokeDashOffset.ToString();
                }
            }
        }

        internal void ApplyMarginToFixNegativeCoordinates(Point newFixingMargin)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var styleOfcanvasElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_canvasDomElement);
                if (newFixingMargin == null) //if it is null, we want 0 everywhere
                {
                    newFixingMargin = new Point();
                }
                try
                {
                    styleOfcanvasElement.transform = "translate(" + newFixingMargin.X.ToString() + "px, " + newFixingMargin.Y.ToString() + "px)";
                }
                catch
                {
                }
                try
                {
                    styleOfcanvasElement.msTransform = "translate(" + newFixingMargin.X.ToString() + "px, " + newFixingMargin.Y.ToString() + "px)";
                }
                catch
                {
                }
                try // Prevents crash in the simulator that uses IE.
                {
                    styleOfcanvasElement.WebkitTransform = "translate(" + newFixingMargin.X.ToString() + "px, " + newFixingMargin.Y.ToString() + "px)";
                }
                catch
                {
                }
                //styleOfcanvasElement.marginTop = newFixingMargin.Y.ToString() + "px";
            }
        }

#if WORKINPROGRESS
        #region Not supported yet
        
        //
        // Summary:
        //     Gets or sets a PenLineCap enumeration value that specifies how the ends of
        //     a dash are drawn.
        //
        // Returns:
        //     One of the enumeration values for PenLineCap. The default is Flat.
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }
        //
        // Summary:
        //     Identifies the StrokeDashCap dependency property.
        //
        // Returns:
        //     The identifier for the StrokeDashCap dependency property.
        public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(Shape), new PropertyMetadata(PenLineCap.Flat));
        
        
        #endregion
#endif

    }
}