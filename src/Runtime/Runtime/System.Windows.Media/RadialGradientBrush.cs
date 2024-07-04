
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
using System.Threading.Tasks;
using System.Windows.Shapes;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media
{
    /// <summary>
    /// Paints an area with a radial gradient. A focal point defines the beginning of
    /// the gradient, and a circle defines the end point of the gradient.
    /// </summary>
    public sealed class RadialGradientBrush : GradientBrush, ICloneOnAnimation<RadialGradientBrush>
    {
        private readonly bool _isClone;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class.
        /// </summary>
        public RadialGradientBrush() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class
        /// that has the specified gradient stops.
        /// </summary>
        /// <param name="gradientStopCollection">
        /// The gradient stops to set on this brush.
        /// </param>
        public RadialGradientBrush(GradientStopCollection gradientStopCollection)
        {
            GradientStops = gradientStopCollection;
        }

        private RadialGradientBrush(RadialGradientBrush original)
            : base(original)
        {
            _isClone = true;

            Center = original.Center;
            GradientOrigin = original.GradientOrigin;
            RadiusX = original.RadiusX;
            RadiusY = original.RadiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientBrush"/> class
        /// with the specified start and stop colors.
        /// </summary>
        /// <param name="startColor">
        /// Color value at the focus (<see cref="GradientOrigin"/>) of the radial gradient.
        /// </param>
        /// <param name="endColor">
        /// Color value at the outer edge of the radial gradient.
        /// </param>
        public RadialGradientBrush(Color startColor, Color endColor)
        {
            GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = startColor, Offset = 0 },
                new GradientStop { Color = endColor, Offset = 1 },
            };
        }

        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(RadialGradientBrush),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the center of the outer circle of the radial gradient.
        /// </summary>
        /// <returns>
        /// The two-dimensional point located at the center of the radial gradient. The default
        /// value is a <see cref="Point"/> with value 0.5,0.5.
        /// </returns>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValueInternal(CenterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="GradientOrigin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GradientOriginProperty =
            DependencyProperty.Register(
                nameof(GradientOrigin),
                typeof(Point),
                typeof(RadialGradientBrush),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the location of the focal point that defines the beginning of the
        /// gradient.
        /// </summary>
        /// <returns>
        /// The two-dimensional point located at the center of the radial gradient. The default
        /// value is a <see cref="Point"/> with value 0.5,0.5.
        /// </returns>
        public Point GradientOrigin
        {
            get => (Point)GetValue(GradientOriginProperty);
            set => SetValueInternal(GradientOriginProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(RadialGradientBrush),
                new PropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets the horizontal radius of the outer circle of the radial gradient.
        /// </summary>
        /// <returns>
        /// The horizontal radius of the outermost circle of the radial gradient. The default
        /// is 0.5.
        /// </returns>
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValueInternal(RadiusXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(RadialGradientBrush),
                new PropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets the vertical radius of the outer circle of a radial gradient.
        /// </summary>
        /// <returns>
        /// The vertical radius of the outer circle of a radial gradient. The default is
        /// 0.5.
        /// </returns>
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValueInternal(RadiusYProperty, value);
        }

        bool ICloneOnAnimation<RadialGradientBrush>.IsClone => _isClone;

        RadialGradientBrush ICloneOnAnimation<RadialGradientBrush>.Clone() => new RadialGradientBrush(this);

        internal override ValueTask<string> GetDataStringAsync(UIElement parent) => new(ToHtmlString(parent));

        internal override ISvgBrush GetSvgElement(Shape shape) => new SvgRadialGradient(shape, this);

        private string GetGradientStopsString()
        {
            return string.Join(", ",
                GradientStops
                .GetSortedCollection()
                .Select(gs => $"{gs.Color.ToHtmlString(Opacity)} {(gs.Offset * 100).ToInvariantString()}%"));
        }

        internal string ToHtmlString(DependencyObject parent)
        {
            // background-image: radial-gradient(50% 50% at 50% 100%,
            // blue 0%, white 10%, purple 20%, purple 99%, black 100%);
            string stops = GetGradientStopsString();

            string unit = "%";
            int multiplier = 100;
            if (MappingMode == BrushMappingMode.Absolute)
            {
                unit = "px";
                multiplier = 1;
            }

            Point center = Center;
            string gradientType = SpreadMethod == GradientSpreadMethod.Repeat ? "repeating-radial-gradient" : "radial-gradient";
            string rx = ((int)(RadiusX * multiplier)).ToInvariantString();
            string ry = ((int)(RadiusY * multiplier)).ToInvariantString();
            string cx = ((int)(center.X * multiplier)).ToInvariantString();
            string cy = ((int)(center.Y * multiplier)).ToInvariantString();
            return $"{gradientType}({rx}{unit} {ry}{unit} at {cx}{unit} {cy}{unit}, {stops})";
        }

        private sealed class SvgRadialGradient : ISvgBrush
        {
            private readonly RadialGradientBrush _radialGradient;
            private readonly INTERNAL_HtmlDomElementReference _gradientRef;
            private readonly WeakEventListener<SvgRadialGradient, Brush, EventArgs> _transformChangedListener;

            public SvgRadialGradient(Shape shape, RadialGradientBrush rgb)
            {
                _radialGradient = rgb;
                _gradientRef = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(shape.DefsElement, "radialGradient");
                DrawRadialGradient();

                _transformChangedListener = new(this, rgb)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnTransformChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                rgb.TransformChanged += _transformChangedListener.OnEvent;
            }

            public string GetBrush(Shape shape) => $"url(#{_gradientRef.UniqueIdentifier})";

            public void RenderBrush() => DrawRadialGradient();

            public void DestroyBrush(Shape shape)
            {
                _transformChangedListener.Detach();
                INTERNAL_HtmlDomManager.RemoveNodeNative(_gradientRef);
            }

            private void OnTransformChanged(object sender, EventArgs e)
            {
                Transform transform = ((Brush)sender).Transform;

                if (transform is null || transform.IsIdentity)
                {
                    INTERNAL_HtmlDomManager.RemoveAttribute(_gradientRef, "gradientTransform");
                }
                else
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_gradientRef,
                        "gradientTransform",
                        MatrixTransform.MatrixToHtmlString(transform.Matrix));
                }
            }

            private void DrawRadialGradient()
            {
                Point center = _radialGradient.Center;
                string cx = Math.Round(center.X, 2).ToInvariantString();
                string cy = Math.Round(center.Y, 2).ToInvariantString();
                // TODO: support ellipse shaped radial on SVG using a gradientTransform.
                string r = Math.Round(0.5 * Math.Max(0, _radialGradient.RadiusX + _radialGradient.RadiusY), 2).ToInvariantString();
                string units = ConvertBrushMappingModeToString(_radialGradient.MappingMode);
                string spreadMethod = ConvertSpreadMethodToString(_radialGradient.SpreadMethod);
                string transform = _radialGradient.Transform is Transform t && !t.IsIdentity ?
                    MatrixTransform.MatrixToHtmlString(t.Matrix) : string.Empty;
                string opacity = Math.Round(_radialGradient.Opacity, 2).ToInvariantString();
                string stops = string.Join(",",
                    _radialGradient
                    .GetGradientStops()
                    .Select(s => $"{Math.Round(s.Offset, 2).ToInvariantString()},'{s.Color.ToHtmlString(1.0)}'"));

                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.drawSvgRadialGradient('{_gradientRef.UniqueIdentifier}',{cx},{cy},{r},'{units}','{spreadMethod}','{transform}',{opacity},{stops})");
            }
        }
    }
}
