
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

using System.Linq;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Provides a base class for shape elements, such as <see cref="Ellipse"/>,
    /// <see cref="Polygon"/>, and <see cref="Rectangle"/>.
    /// </summary>
    public abstract class Shape : FrameworkElement
    {
        static Shape()
        {
            IsHitTestableProperty.OverrideMetadata(typeof(Shape), new PropertyMetadata(BooleanBoxes.TrueBox));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        protected Shape() { }

        /// <summary>
        /// Gets a value that represents a <see cref="Transform"/> that is applied
        /// to the geometry of a <see cref="Shape"/> prior to when it is drawn.
        /// </summary>
        /// <returns>
        /// A <see cref="Transform"/> that is applied to the geometry of a <see cref="Shape"/>
        /// prior to when it is drawn.
        /// </returns>
        public virtual Transform GeometryTransform => new MatrixTransform(StretchMatrix ?? Matrix.Identity);

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                nameof(Fill),
                typeof(Brush),
                typeof(Shape),
                new PropertyMetadata(null, OnFillChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => SetFill((Shape)d, (Brush)newValue),
                });

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how to paint the interior 
        /// of the shape.
        /// </summary>
        /// <returns>
        /// A <see cref="Brush"/> that describes how the shape's interior is painted.
        /// The default is null.
        /// </returns>
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValueInternal(FillProperty, value);
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shape = (Shape)d;

            if (shape._fillChangedListener != null)
            {
                shape._fillChangedListener.Detach();
                shape._fillChangedListener = null;
            }

            if (e.NewValue is Brush newBrush)
            {
                shape._fillChangedListener = new(shape, newBrush)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnFillChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newBrush.Changed += shape._fillChangedListener.OnEvent;
            }
        }

        private void OnFillChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_fillBrush is ISvgBrush svgBrush)
                {
                    svgBrush.RenderBrush();
                    SetSvgAttribute("fill", svgBrush.GetBrush(this));
                }
            }
        }

        private static void SetFill(Shape shape, Brush fill)
        {
            if (shape._fillBrush is not null)
            {
                shape._fillBrush.DestroyBrush(shape);
                shape._fillBrush = null;
                shape.RemoveSvgAttribute("fill");
            }

            if (fill is Brush brush && brush.GetSvgElement(shape) is ISvgBrush svgBrush)
            {
                shape._fillBrush = svgBrush;
                shape.SetSvgAttribute("fill", svgBrush.GetBrush(shape));
            }
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(Shape),
                new FrameworkPropertyMetadata(
                    Stretch.None,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets a <see cref="Stretch"/> enumeration value that describes
        /// how the shape fills its allocated space.
        /// </summary>
        /// <returns>
        /// One of the <see cref="Stretch"/> enumeration values. The default value
        /// at run time depends on the type of <see cref="Shape"/>.
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValueInternal(StretchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(Shape),
                new PropertyMetadata(null, OnStrokeChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => SetStroke((Shape)d, (Brush)newValue),
                });

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how the <see cref="Shape"/>
        /// outline is painted.
        /// </summary>
        /// <returns>
        /// A <see cref="Brush"/> that specifies how the <see cref="Shape"/> outline is 
        /// painted. The default is null.
        /// </returns>
        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValueInternal(StrokeProperty, value);
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shape = (Shape)d;

            if (shape._strokeChangedListener != null)
            {
                shape._strokeChangedListener.Detach();
                shape._strokeChangedListener = null;
            }

            if (e.NewValue is Brush newBrush)
            {
                shape._strokeChangedListener = new(shape, newBrush)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnStrokeChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                newBrush.Changed += shape._strokeChangedListener.OnEvent;
            }
        }

        private void OnStrokeChanged(object sender, EventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (_strokeBrush is ISvgBrush svgBrush)
                {
                    svgBrush.RenderBrush();
                    SetSvgAttribute("stroke", svgBrush.GetBrush(this));
                }
            }
        }

        private static void SetStroke(Shape shape, Brush stroke)
        {
            if (shape._strokeBrush is not null)
            {
                shape._strokeBrush.DestroyBrush(shape);
                shape._strokeBrush = null;
                shape.RemoveSvgAttribute("stroke");
            }

            if (stroke is Brush brush && brush.GetSvgElement(shape) is ISvgBrush svgBrush)
            {
                shape._strokeBrush = svgBrush;
                shape.SetSvgAttribute("stroke", svgBrush.GetBrush(shape));
            }
        }

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                nameof(StrokeDashArray),
                typeof(DoubleCollection),
                typeof(Shape),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<double>(
                        static () => new DoubleCollection(),
                        static (d, dp) => new DoubleCollection()),
                    null,
                    CoerceStrokeDashArray)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        if (newValue is DoubleCollection dashArray)
                        {
                            shape.SetSvgAttribute(
                                "stroke-dasharray",
                                string.Join(",", dashArray.InternalItems.Select(d => Math.Round(d, 2).ToInvariantString())));
                        }
                        else
                        {
                            shape.RemoveSvgAttribute("stroke-dasharray");
                        }
                    },
                });

        /// <summary>
        /// Gets or sets a collection of <see cref="double"/> values that indicate the pattern of
        /// dashes and gaps that is used to outline shapes.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="double"/> values that specify the pattern of dashes and gaps.
        /// </returns>
        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValueInternal(StrokeDashArrayProperty, value);
        }

        private static object CoerceStrokeDashArray(DependencyObject d, object baseValue)
        {
            return baseValue ?? new DoubleCollection();
        }

        /// <summary>
        /// Identifies the <see cref="StrokeDashCap"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register(
                nameof(StrokeDashCap),
                typeof(PenLineCap),
                typeof(Shape),
                new PropertyMetadata(PenLineCap.Flat)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        string value = (PenLineCap)newValue switch
                        {
                            PenLineCap.Square => "square",
                            PenLineCap.Round => "round",
                            PenLineCap.Triangle => "butt", // unsupported
                            _ => "butt",
                        };

                        shape.SetSvgAttribute("stroke-linecap", value);
                    },
                });

        /// <summary>
        /// Gets or sets a <see cref="PenLineCap"/> enumeration value that specifies
        /// how the ends of a dash are drawn.
        /// </summary>
        /// <returns>
        /// One of the enumeration values for <see cref="PenLineCap"/>. The default
        /// is <see cref="PenLineCap.Flat"/>.
        /// </returns>
        public PenLineCap StrokeDashCap
        {
            get => (PenLineCap)GetValue(StrokeDashCapProperty);
            set => SetValueInternal(StrokeDashCapProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeDashOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register(
                nameof(StrokeDashOffset),
                typeof(double),
                typeof(Shape),
                new PropertyMetadata(0.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        double offset = (double)newValue;
                        shape.SetSvgAttribute("stroke-dashoffset", Math.Round(offset, 2).ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets a <see cref="double"/> that specifies the distance within the dash pattern
        /// where a dash begins.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the distance within the dash pattern where a
        /// dash begins. The default value is 0.
        /// </returns>
        public double StrokeDashOffset
        {
            get => (double)GetValue(StrokeDashOffsetProperty);
            set => SetValueInternal(StrokeDashOffsetProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeEndLineCap"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register(
                nameof(StrokeEndLineCap),
                typeof(PenLineCap),
                typeof(Shape),
                new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets a <see cref="PenLineCap"/> enumeration value that describes
        /// the <see cref="Shape"/> at the end of a line.
        /// </summary>
        /// <returns>
        /// One of the enumeration values for <see cref="PenLineCap"/>. The default
        /// is <see cref="PenLineCap.Flat"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PenLineCap StrokeEndLineCap
        {
            get => (PenLineCap)GetValue(StrokeEndLineCapProperty);
            set => SetValueInternal(StrokeEndLineCapProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeLineJoin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(
                nameof(StrokeLineJoin),
                typeof(PenLineJoin),
                typeof(Shape),
                new PropertyMetadata(PenLineJoin.Miter)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        
                        string value = (PenLineJoin)newValue switch
                        {
                            PenLineJoin.Bevel => "bevel",
                            PenLineJoin.Round => "round",
                            _ => "miter",
                        };
                        
                        shape.SetSvgAttribute("stroke-linejoin", value);
                    },
                });

        /// <summary>
        /// Gets or sets a <see cref="PenLineJoin"/> enumeration value that specifies
        /// the type of join that is used at the vertices of a <see cref="Shape"/>.
        /// </summary>
        /// <returns>
        /// A value of the <see cref="PenLineJoin"/> enumeration that specifies the
        /// join appearance. The default value is <see cref="PenLineJoin.Miter"/>.
        /// </returns>
        public PenLineJoin StrokeLineJoin
        {
            get => (PenLineJoin)GetValue(StrokeLineJoinProperty);
            set => SetValueInternal(StrokeLineJoinProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeMiterLimit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register(
                nameof(StrokeMiterLimit),
                typeof(double),
                typeof(Shape),
                new PropertyMetadata(10.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        double limit = (double)newValue;
                        shape.SetSvgAttribute("stroke-miterlimit", Math.Round(limit, 2).ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets a limit on the ratio of the miter length to half the 
        /// <see cref="StrokeThickness"/> of a <see cref="Shape"/> element.
        /// </summary>
        /// <returns>
        /// The limit on the ratio of the miter length to the <see cref="StrokeThickness"/>
        /// of a <see cref="Shape"/> element. This value is always a positive number
        /// that is greater than or equal to 1.
        /// </returns>
        public double StrokeMiterLimit
        {
            get => (double)GetValue(StrokeMiterLimitProperty);
            set => SetValueInternal(StrokeMiterLimitProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeStartLineCap"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty StrokeStartLineCapProperty =
            DependencyProperty.Register(
                nameof(StrokeStartLineCap),
                typeof(PenLineCap),
                typeof(Shape),
                new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets a <see cref="PenLineCap"/> enumeration value that describes
        /// the <see cref="Shape"/> at the start of a <see cref="Stroke"/>.
        /// </summary>
        /// <returns>
        /// A value of the <see cref="PenLineCap"/> enumeration that specifies the
        /// shape at the start of a <see cref="Stroke"/>. The default is <see cref="PenLineCap.Flat"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PenLineCap StrokeStartLineCap
        {
            get => (PenLineCap)GetValue(StrokeStartLineCapProperty);
            set => SetValueInternal(StrokeStartLineCapProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(StrokeThickness),
                typeof(double),
                typeof(Shape),
                new PropertyMetadata(1.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Shape shape = (Shape)d;
                        double thickness = GetComputedStrokeThickess((double)newValue);
                        shape.SetSvgAttribute("stroke-width", Math.Round(thickness, 2).ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the width of the <see cref="Shape"/> stroke outline.
        /// </summary>
        /// <returns>
        /// The width of the <see cref="Shape"/> outline, in pixels. The default
        /// value is 0.
        /// </returns>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValueInternal(StrokeThicknessProperty, value);
        }

        internal double GetStrokeThickness()
            => Stroke is null ? 0 : GetComputedStrokeThickess(StrokeThickness);

        private static double GetComputedStrokeThickess(double strokeThickness)
        {
            if (double.IsNaN(strokeThickness) || DoubleUtil.IsZero(strokeThickness))
            {
                return 0;
            }
            else
            {
                return Math.Abs(strokeThickness);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size newSize;

            Stretch mode = Stretch;

            if (mode == Stretch.None)
            {
                newSize = GetNaturalSize();
            }
            else
            {
                newSize = GetStretchedRenderSize(mode, GetStrokeThickness(), constraint, GetDefiningGeometryBounds());
            }

            if (SizeIsInvalidOrEmpty(newSize))
            {
                // We've encountered a numerical error. Don't draw anything.
                newSize = new Size(0, 0);
            }

            return newSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size newSize;

            Stretch mode = Stretch;

            if (mode == Stretch.None)
            {
                StretchMatrix = null;

                newSize = finalSize;
            }
            else
            {
                newSize = GetStretchedRenderSizeAndSetStretchMatrix(
                    mode, GetStrokeThickness(), finalSize, GetDefiningGeometryBounds());
            }

            if (SizeIsInvalidOrEmpty(newSize))
            {
                // We've encountered a numerical error. Don't draw anything.
                newSize = new Size(0, 0);
            }

            if (StretchMatrix.HasValue)
            {
                SetSvgAttribute("transform", MatrixTransform.MatrixToHtmlString(StretchMatrix.Value));
            }
            else
            {
                RemoveSvgAttribute("transform");
            }

            return newSize;
        }

        /// <summary>
        /// Get the natural size of the geometry that defines this shape
        /// </summary>
        internal virtual Size GetNaturalSize()
        {
            Rect bounds = GetDefiningGeometryBounds();
            double margin = Math.Ceiling(GetStrokeThickness() / 2);
            return new Size(Math.Max(bounds.Right + margin, 0), Math.Max(bounds.Bottom + margin, 0));
        }

        /// <summary>
        /// Get the bonds of the geometry that defines this shape
        /// </summary>
        internal virtual Rect GetDefiningGeometryBounds() => GetBBox();

        internal Size GetStretchedRenderSize(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {
            GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds,
                out _, out _, out _, out _, out Size renderSize);

            return renderSize;
        }

        internal Size GetStretchedRenderSizeAndSetStretchMatrix(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {
            GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds,
                out double xScale, out double yScale, out double dX, out double dY, out Size renderSize);

            // Construct the matrix
            Matrix stretchMatrix = Matrix.Identity;
            stretchMatrix.ScaleAt(xScale, yScale, geometryBounds.Location.X, geometryBounds.Location.Y);
            stretchMatrix.Translate(dX, dY);
            StretchMatrix = stretchMatrix;

            return renderSize;
        }

        internal void GetStretchMetrics(
            Stretch mode,
            double strokeThickness,
            Size availableSize,
            Rect geometryBounds,
            out double xScale,
            out double yScale,
            out double dX,
            out double dY,
            out Size stretchedSize)
        {
            if (!geometryBounds.IsEmpty)
            {
                double margin = strokeThickness / 2;
                bool hasThinDimension = false;

                // Initialization for mode == Fill
                xScale = Math.Max(availableSize.Width - strokeThickness, 0);
                yScale = Math.Max(availableSize.Height - strokeThickness, 0);
                dX = margin - geometryBounds.Left;
                dY = margin - geometryBounds.Top;

                // Compute the scale factors from the geometry to the size.
                // The scale factors are ratios, and they have already been initialize to the numerators.
                // To prevent fp overflow, we need to make sure that numerator / denomiator < limit;
                // To do that without actually deviding, we check that denominator > numerator / limit.
                // We take 1/epsilon as the limit, so the check is denominator > numerator * epsilon

                // See Dev10 bug #453150.
                // If the scale is infinite in both dimensions, return the natural size.
                // If it's infinite in only one dimension, for non-fill stretch modes we constrain the size based
                // on the unconstrained dimension.
                // If our shape is "thin", i.e. a horizontal or vertical line, we can ignore non-fill stretches.
                if (geometryBounds.Width > xScale * double.Epsilon)
                {
                    xScale /= geometryBounds.Width;
                }
                else
                {
                    xScale = 1;
                    // We can ignore uniform and uniform-to-fill stretches if we have a vertical line.
                    if (geometryBounds.Width == 0)
                    {
                        hasThinDimension = true;
                    }
                }

                if (geometryBounds.Height > yScale * double.Epsilon)
                {
                    yScale /= geometryBounds.Height;
                }
                else
                {
                    yScale = 1;
                    // We can ignore uniform and uniform-to-fill stretches if we have a horizontal line.
                    if (geometryBounds.Height == 0)
                    {
                        hasThinDimension = true;
                    }
                }

                // Because this case was handled by the caller
                Debug.Assert(mode != Stretch.None);

                // We are initialized for Fill, but for the other modes
                // If one of our dimensions is thin, uniform stretches are
                // meaningless, so we treat the stretch as fill.
                if (mode != Stretch.Fill && !hasThinDimension)
                {
                    if (mode == Stretch.Uniform)
                    {
                        if (yScale > xScale)
                        {
                            // Resize to fit the size's width
                            yScale = xScale;
                        }
                        else // if xScale >= yScale
                        {
                            // Resize to fit the size's height
                            xScale = yScale;
                        }
                    }
                    else
                    {
                        Debug.Assert(mode == Stretch.UniformToFill);

                        if (xScale > yScale)
                        {
                            // Resize to fill the size vertically, spilling out horizontally
                            yScale = xScale;
                        }
                        else // if yScale >= xScale
                        {
                            // Resize to fill the size horizontally, spilling out vertically
                            xScale = yScale;
                        }
                    }
                }

                stretchedSize = new Size(geometryBounds.Width * xScale + strokeThickness, geometryBounds.Height * yScale + strokeThickness);
            }
            else
            {
                xScale = yScale = 1;
                dX = dY = 0;
                stretchedSize = new Size(0, 0);
            }
        }

        internal bool SizeIsInvalidOrEmpty(Size size)
        {
            return double.IsNaN(size.Width) || double.IsNaN(size.Height) || size.IsEmpty;
        }

        public sealed override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            (var outerDiv, SvgElement, DefsElement) = INTERNAL_HtmlDomManager.CreateShapeElementAndAppendIt(
                (INTERNAL_HtmlDomElementReference)parentRef, this);

            return outerDiv;
        }

        protected internal sealed override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();
            INTERNAL_HtmlDomManager.RemoveFromGlobalStore(SvgElement);
            SvgElement = null;
            DefsElement = null;
            _fillBrush = null;
            _strokeBrush = null;
        }

        internal void SetSvgAttribute(string attribute, string value)
        {
            Debug.Assert(SvgElement is not null);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(SvgElement, attribute, value);
        }

        internal void RemoveSvgAttribute(string attribute)
        {
            Debug.Assert(SvgElement is not null);
            INTERNAL_HtmlDomManager.RemoveAttribute(SvgElement, attribute);
        }

        internal void SetFillRuleAttribute(FillRule fillRule)
        {
            string value = fillRule switch
            {
                FillRule.Nonzero => "nonzero",
                _ => "evenodd",
            };

            SetSvgAttribute("fill-rule", value);
        }

        internal Rect GetBBox()
        {
            if (SvgElement is not null)
            {
                string sDiv = OpenSilver.Interop.GetVariableStringForJS(SvgElement);

                SVGRect bbox = JsonSerializer.Deserialize<SVGRect>(
                    OpenSilver.Interop.ExecuteJavaScriptString($"document.getBBox({sDiv});"));
                return new Rect(bbox.X, bbox.Y, bbox.Width, bbox.Height);
            }

            return new Rect();
        }

        internal sealed override void SetPointerEvents(bool hitTestable) =>
            SvgElement.Style.pointerEvents = hitTestable ? "auto" : "none";

        internal virtual string SvgTagName => "path";

        internal sealed override bool EnablePointerEventsCore => true;

        internal Matrix? StretchMatrix { get; private set; }

        internal INTERNAL_HtmlDomElementReference SvgElement { get; private set; }

        internal INTERNAL_HtmlDomElementReference DefsElement { get; private set; }

        private ISvgBrush _fillBrush;
        private ISvgBrush _strokeBrush;
        private WeakEventListener<Shape, Brush, EventArgs> _fillChangedListener;
        private WeakEventListener<Shape, Brush, EventArgs> _strokeChangedListener;

        private struct SVGRect
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
    }
}