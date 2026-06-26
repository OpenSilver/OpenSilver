
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

using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    public class Ellipse : Shape
    {
        static Ellipse()
        {
            StretchProperty.OverrideMetadata(
                typeof(Ellipse),
                new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        private Rect _rect = Rect.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> class.
        /// </summary>
        public Ellipse() { }

        /// <inheritdoc />
        public override Geometry RenderedGeometry => DefiningGeometry;

        /// <inheritdoc />
        protected override Geometry DefiningGeometry => _rect.IsEmpty ? Geometry.Empty : new EllipseGeometry(_rect);

        internal sealed override string SvgTagName => "ellipse";

        internal sealed override bool UseDefaultRendering => false;

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Stretch == Stretch.UniformToFill)
            {
                double width = Width;
                if (double.IsNaN(width))
                {
                    width = availableSize.Width;
                    if (double.IsInfinity(width))
                    {
                        width = 0.0;
                    }
                }

                double height = Height;
                if (double.IsNaN(height))
                {
                    height = availableSize.Height;
                    if (double.IsInfinity(height))
                    {
                        height = 0.0;
                    }
                }

                return new Size(width, height);
            }

            return GetNaturalSize();
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            double penThickness = GetStrokeThickness();
            double margin = penThickness / 2;

            _rect = new Rect(
                margin, // X
                margin, // Y
                Math.Max(0, finalSize.Width - penThickness),    // Width
                Math.Max(0, finalSize.Height - penThickness));  // Height

            switch (Stretch)
            {
                case Stretch.None:
                    // A 0 Rect.Width and Rect.Height rectangle
                    _rect.Width = _rect.Height = 0;
                    break;
                
                case Stretch.Uniform:
                    // The maximal square that fits in the final box
                    if (_rect.Width > _rect.Height)
                    {
                        _rect.Width = _rect.Height;
                    }
                    else
                    {
                        _rect.Height = _rect.Width;
                    }
                    break;

                case Stretch.UniformToFill:
                    // The minimal square that fills the final box
                    if (_rect.Width < _rect.Height)
                    {
                        _rect.Width = _rect.Height;
                    }
                    else
                    {
                        _rect.Height = _rect.Width;
                    }
                    break;

                case Stretch.Fill:
                default:
                    // The most common case: a rectangle that fills the box.
                    // rect has already been initialized for that.
                    break;
            }

            ArrangeNative(_rect.Width / 2, _rect.Height / 2, penThickness);

            return finalSize;
        }

        private void ArrangeNative(double rx, double ry, double penThickness)
        {
            rx = Math.Round(rx, 2);
            ry = Math.Round(ry, 2);
            penThickness = Math.Round(penThickness, 2);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.arrangeEllipse('{SvgElement.Uid}',{rx.ToInvariantString()},{ry.ToInvariantString()},{penThickness.ToInvariantString()})");
        }

        internal sealed override Size GetNaturalSize()
        {
            double width = Width;
            double height = Height;

            return new Size(double.IsNaN(width) ? 0.0 : width, double.IsNaN(height) ? 0.0 : height);
        }
    }
}