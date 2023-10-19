
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

using System.Collections.Generic;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Shapes;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a complex shape that may be composed of arcs, curves, ellipses, lines,
    /// and rectangles.
    /// </summary>
    [ContentProperty(nameof(Figures))]
    public sealed class PathGeometry : Geometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        public PathGeometry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class with 
        /// the specified Figures.
        /// </summary>
        /// <param name="figures">
        /// The Figures of the <see cref="PathGeometry"/> which describes the contents 
        /// of the <see cref="Path"/>.
        /// </param>
        public PathGeometry(IEnumerable<PathFigure> figures)
        {
            if (figures is null)
            {
                throw new ArgumentNullException(nameof(figures));
            }

            foreach (PathFigure item in figures)
            {
                Figures.Add(item);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Figures"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FiguresProperty =
            DependencyProperty.Register(
                nameof(Figures),
                typeof(PathFigureCollection),
                typeof(PathGeometry),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<PathFigure>(
                        static () => new PathFigureCollection(),
                        static (d, dp) =>
                        {
                            PathGeometry pathGeometry = (PathGeometry)d;
                            var figures = new PathFigureCollection();
                            figures.SetParentGeometry(pathGeometry);
                            figures.CollectionChanged += new NotifyCollectionChangedEventHandler(pathGeometry.OnFiguresCollectionChanged);
                            return figures;
                        }),
                    OnFiguresChanged,
                    CoerceFigures));

        /// <summary>
        /// Gets or sets the collection of <see cref="PathFigure"/> objects that describe
        /// the contents of a path.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="PathFigure"/> objects that describe the contents
        /// of a path. Each individual <see cref="PathFigure"/> describes a shape.
        /// </returns>
        public PathFigureCollection Figures
        {
            get => (PathFigureCollection)GetValue(FiguresProperty);
            set => SetValue(FiguresProperty, value);
        }

        private static void OnFiguresChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathGeometry pathGeometry = (PathGeometry)d;
            if (e.OldValue is PathFigureCollection oldFigures)
            {
                oldFigures.SetParentGeometry(null);
                oldFigures.CollectionChanged -= new NotifyCollectionChangedEventHandler(pathGeometry.OnFiguresCollectionChanged);
            }
            if (e.NewValue is PathFigureCollection newFigures)
            {
                newFigures.SetParentGeometry(pathGeometry);
                newFigures.CollectionChanged += new NotifyCollectionChangedEventHandler(pathGeometry.OnFiguresCollectionChanged);
            }

            OnPathChanged(d, e);
        }

        private static object CoerceFigures(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PathFigureCollection();
        }

        private void OnFiguresCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RaisePathChanged();

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule),
                typeof(FillRule),
                typeof(PathGeometry),
                new PropertyMetadata(FillRule.EvenOdd, OnFillRuleChanged));

        /// <summary>
        /// Gets or sets a value that determines how the intersecting areas contained in
        /// the <see cref="PathGeometry"/> are combined.
        /// </summary>
        /// <returns>
        /// A <see cref="FillRule"/> enumeration value that indicates how the intersecting
        /// areas of the <see cref="PathGeometry"/> are combined. The default is <see cref="FillRule.EvenOdd"/>.
        /// </returns>
        public FillRule FillRule
        {
            get => (FillRule)GetValue(FillRuleProperty);
            set => SetValue(FillRuleProperty, value);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="PathGeometry"/>.
        /// </summary>
        /// <returns>
        /// A string representation of this <see cref="PathGeometry"/>.
        /// </returns>
        public override string ToString()
        {
            string figuresString = ToStreamGeometry(Figures, CultureInfo.InvariantCulture);
            if (FillRule != FillRule.EvenOdd)
            {
                figuresString = $"F1{figuresString}";
            }
            return figuresString;
        }

        internal override string ToPathData(IFormatProvider formatProvider) => ToStreamGeometry(Figures, formatProvider);

        internal override FillRule GetFillRule() => FillRule;

        /// <summary>
		/// Transform the figures collection into a SVG Path according to :
		/// https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d
		/// </summary>
		private static string ToStreamGeometry(PathFigureCollection figures, IFormatProvider formatProvider)
        {
            if (figures is null)
            {
                return string.Empty;
            }

            return string.Join(" ", GenerateDataParts(figures, formatProvider));

            static IEnumerable<string> GenerateDataParts(PathFigureCollection figures, IFormatProvider formatProvider)
            {
                foreach (var figure in figures)
                {
                    // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#moveto_path_commands
                    yield return $"M {figure.StartPoint.X.ToString(formatProvider)},{figure.StartPoint.Y.ToString(formatProvider)}";

                    foreach (var segment in figure.Segments)
                    {
                        foreach (var p in segment.ToDataStream(formatProvider))
                        {
                            yield return p;
                        }
                    }

                    if (figure.IsClosed)
                    {
                        // https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#closepath
                        yield return "Z";
                    }
                }
            }
        }
    }
}