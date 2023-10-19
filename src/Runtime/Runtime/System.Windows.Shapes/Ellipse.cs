
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> class.
        /// </summary>
        public Ellipse() { }

        internal sealed override string SvgTagName => "ellipse";

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
            
            double rx = Math.Max(0, finalSize.Width - penThickness) / 2;
            double ry = Math.Max(0, finalSize.Height - penThickness) / 2;

            switch (Stretch)
            {
                case Stretch.None:
                    // A 0 Rect.Width and Rect.Height rectangle
                    //rect.Width = rect.Height = 0;
                    rx = ry = 0;
                    break;
                
                case Stretch.Uniform:
                    // The maximal square that fits in the final box
                    if (rx > ry)
                    {
                        rx = ry;
                    }
                    else
                    {
                        ry = rx;
                    }
                    break;

                case Stretch.UniformToFill:
                    // The minimal square that fills the final box
                    if (rx < ry)
                    {
                        rx = ry;
                    }
                    else
                    {
                        ry = rx;
                    }
                    break;

                case Stretch.Fill:
                default:
                    // The most common case: a rectangle that fills the box.
                    // rect has already been initialized for that.
                    break;
            }

            SetSvgAttribute("cx", (rx + penThickness / 2).ToInvariantString());
            SetSvgAttribute("cy", (ry + penThickness / 2).ToInvariantString());
            SetSvgAttribute("rx", rx.ToInvariantString());
            SetSvgAttribute("ry", ry.ToInvariantString());

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