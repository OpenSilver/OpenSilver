
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

using OpenSilver.Internal;

namespace System.Windows.Shapes
{
    /// <summary>
    /// Draws a straight line between two points.
    /// </summary>
    public class Line : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        public Line() { }

        /// <summary>
        /// Identifies the <see cref="X1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(
                nameof(X1),
                typeof(double),
                typeof(Line),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Line line = (Line)d;
                        double x1 = (double)newValue;
                        line.SetSvgAttribute("x1", x1.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the x-coordinate of the <see cref="Line"/> start point.
        /// </summary>
        /// <returns>
        /// The x-coordinate for the start point of the line, in pixels. The default is 0.
        /// </returns>
        public double X1
        {
            get => (double)GetValue(X1Property);
            set => SetValue(X1Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="X2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(
                nameof(X2),
                typeof(double),
                typeof(Line),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Line line = (Line)d;
                        double x2 = (double)newValue;
                        line.SetSvgAttribute("x2", x2.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the x-coordinate of the <see cref="Line"/> end point.
        /// </summary>
        /// <returns>
        /// The x-coordinate for the end point of the line, in pixels. The default is 0.
        /// </returns>
        public double X2
        {
            get => (double)GetValue(X2Property);
            set => SetValue(X2Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Y1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(
                nameof(Y1),
                typeof(double),
                typeof(Line),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Line line = (Line)d;
                        double y1 = (double)newValue;
                        line.SetSvgAttribute("y1", y1.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the y-coordinate of the <see cref="Line"/> start point.
        /// </summary>
        /// <returns>
        /// The y-coordinate for the start point of the line, in pixels. The default is 0.
        /// </returns>
        public double Y1
        {
            get => (double)GetValue(Y1Property);
            set => SetValue(Y1Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Y2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(
                nameof(Y2),
                typeof(double),
                typeof(Line),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        Line line = (Line)d;
                        double y2 = (double)newValue;
                        line.SetSvgAttribute("y2", y2.ToInvariantString());
                    },
                });

        /// <summary>
        /// Gets or sets the y-coordinate of the <see cref="Line"/> end point.
        /// </summary>
        /// <returns>
        /// The y-coordinate for the end point of the line, in pixels. The default is 0.
        /// </returns>
        public double Y2
        {
            get => (double)GetValue(Y2Property);
            set => SetValue(Y2Property, value);
        }

        internal sealed override string SvgTagName => "line";

        internal sealed override Size GetNaturalSize()
        {
            Rect bounds = GetDefiningGeometryBounds();
            double margin = Math.Ceiling(GetStrokeThickness() / 2);
            return new Size(Math.Max(bounds.Right + margin, 0), Math.Max(bounds.Bottom + margin, 0));
        }

        internal sealed override Rect GetDefiningGeometryBounds() => new Rect(new Point(X1, Y1), new Point(X2, Y2));
    }
}