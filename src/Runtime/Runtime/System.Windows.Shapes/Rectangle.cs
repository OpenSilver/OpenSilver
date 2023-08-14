
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

using System;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
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
    public class Rectangle : Shape
    {
        static Rectangle()
        {
            StretchProperty.OverrideMetadata(
                typeof(Rectangle),
                new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        public Rectangle() { }

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(
                nameof(RadiusX),
                typeof(double),
                typeof(Rectangle),
                new PropertyMetadata(0.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Rectangle rect = (Rectangle)d;
                        double rx = (double)newValue;
                        rect.SetSvgAttribute("rx", rx.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the x-axis radius of the ellipse that is used to round the corners
        /// of the rectangle.
        /// </summary>
        /// <returns>
        /// The x-axis radius of the ellipse that is used to round the corners of the rectangle.
        /// </returns>
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(
                nameof(RadiusY),
                typeof(double),
                typeof(Rectangle),
                new PropertyMetadata(0.0)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Rectangle rect = (Rectangle)d;
                        double ry = (double)newValue;
                        rect.SetSvgAttribute("ry", ry.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the y-axis radius of the ellipse that is used to round the corners
        /// of the rectangle.
        /// </summary>
        /// <returns>
        /// The y-axis radius of the ellipse that is used to round the corners of the rectangle.
        /// The default is 0.
        /// </returns>
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }

        internal sealed override string SvgTagName => "rect";

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

        protected override Size ArrangeOverride(Size finalSize)
        {
            double penThickness = GetStrokeThickness();
            double margin = penThickness / 2;

            var rect = new Rect(
                margin,
                margin,
                Math.Max(0, finalSize.Width - penThickness),
                Math.Max(0, finalSize.Height - penThickness));

            switch (Stretch)
            {
                case Stretch.None:
                    // A 0 Rect.Width and Rect.Height rectangle
                    rect.Width = rect.Height = 0;
                    break;

                case Stretch.Uniform:
                    // The maximal square that fits in the final box
                    if (rect.Width > rect.Height)
                    {
                        rect.Width = rect.Height;
                    }
                    else
                    {
                        rect.Height = rect.Width;
                    }
                    break;

                case Stretch.UniformToFill:
                    // The minimal square that fills the final box
                    if (rect.Width < rect.Height)
                    {
                        rect.Width = rect.Height;
                    }
                    else
                    {
                        rect.Height = rect.Width;
                    }
                    break;

                case Stretch.Fill:
                default:
                    // The most common case: a rectangle that fills the box.
                    // rect has already been initialized for that.
                    break;
            }

            SetSvgAttribute("x", rect.X.ToInvariantString());
            SetSvgAttribute("y", rect.Y.ToInvariantString());
            SetSvgAttribute("width", rect.Width.ToInvariantString());
            SetSvgAttribute("height", rect.Height.ToInvariantString());

            return finalSize;
        }

        internal sealed override Size GetNaturalSize()
        {
            double width = Width;
            double height = Height;

            return new Size(double.IsNaN(width) ? 0.0 : width, double.IsNaN(height) ? 0.0 : height);
        }
    }
}
