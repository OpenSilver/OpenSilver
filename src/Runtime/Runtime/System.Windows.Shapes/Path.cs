
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

using System.Globalization;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Draws a series of connected lines and curves. The line and curve dimensions are
    /// declared through the <see cref="Data"/> property, and can be specified
    /// either with a Path-specific mini-language, or with an object model.
    /// </summary>
    public class Path : Shape
    {
        private bool _dirty;
        private WeakEventToken _weakEventToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        public Path() { }

        /// <summary>
        /// Identifies the <see cref="Data"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(Geometry),
                typeof(Path),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnDataChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Path path = (Path)d;
                        if (newValue is Geometry geometry)
                        {
                            path.SetSvgAttribute("d", geometry.ToPathData(CultureInfo.InvariantCulture));
                            path.SetFillRuleAttribute(geometry.GetFillRule());
                        }
                        else
                        {
                            path.RemoveSvgAttribute("d");
                        }
                    },
                });

        /// <summary>
        /// Gets or sets a <see cref="Geometry"/> that specifies the shape to be drawn.
        /// </summary>
        /// <returns>
        /// A description of the shape to be drawn.
        /// </returns>
        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            set => SetValueInternal(DataProperty, value);
        }

        // The Path's geometry lives in Data. Exposing it as the DefiningGeometry ensures
        // Shape.MeasureOverride writes the SVG "d" attribute (instead of removing it because
        // the base class sees a null DefiningGeometry for a "path" element).
        protected override Geometry DefiningGeometry => Data;

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Path path = (Path)d;

            if (path._weakEventToken != null)
            {
                path._weakEventToken.Dispose();
                path._weakEventToken = null;
            }

            if (e.NewValue is Geometry newGeometry)
            {
                path._weakEventToken = WeakEvent.Subscribe<Path, Geometry, GeometryInvalidatedEventsArgs>(
                    path,
                    newGeometry,
                    static (instance, sender, args) => instance.OnGeometryInvalidated(sender, args),
                    static (handler, source) => source.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(handler),
                    static (handler, source) => source.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(handler));
            }
        }

        private void OnGeometryInvalidated(object sender, GeometryInvalidatedEventsArgs e)
        {
            if (e.AffectsMeasure)
            {
                SetDirty();
                InvalidateMeasure();
            }

            if (e.AffectsFillRule)
            {
                if (SvgElement.IsConnected)
                {
                    SetFillRuleAttribute(((Geometry)sender).GetFillRule());
                }
            }
        }

        internal sealed override string SvgTagName => "path";

        protected override Size MeasureOverride(Size constraint)
        {
            InvalidateData();
            return base.MeasureOverride(constraint);
        }

        internal override Size GetNaturalSize()
        {
            Rect bounds = GetDefiningGeometryBounds();
            double margin = Math.Ceiling(GetStrokeThickness() / 2);
            return new Size(Math.Max(bounds.Right + margin, 0), Math.Max(bounds.Bottom + margin, 0));
        }

        private void SetDirty() => _dirty = true;

        private void InvalidateData()
        {
            if (!_dirty)
            {
                return;
            }

            _dirty = false;

            if (Data is Geometry geometry)
            {
                SetSvgAttribute("d", geometry.ToPathData(CultureInfo.InvariantCulture));
            }
            else
            {
                RemoveSvgAttribute("d");
            }
        }
    }
}