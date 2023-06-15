
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
using System.Collections.Generic;
using System.Windows.Markup;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a complex shape that may be composed of arcs, curves, ellipses,
    /// lines, and rectangles.
    /// </summary>
    [ContentProperty(nameof(Figures))]
    public sealed partial class PathGeometry : Geometry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        public PathGeometry()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        /// <param name="figures">A collection of figures</param>
        public PathGeometry(IEnumerable<PathFigure> figures)
        {
            if (figures != null)
            {
                foreach (PathFigure item in figures)
                {
                    this.Figures.Add(item);
                }
            }
            else
            {
                throw new ArgumentNullException("figures");
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the collection of PathFigure objects that describe the contents
        /// of a path.
        /// </summary>
        public PathFigureCollection Figures
        {
            get { return (PathFigureCollection)GetValue(FiguresProperty); }
            set { SetValue(FiguresProperty, value); }
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
                            Geometry g = (Geometry)d;
                            var collection = new PathFigureCollection();
                            collection.SetParentPath(g.ParentPath);
                            return collection;
                        }),
                    OnFiguresChanged,
                    CoerceFigures));

        private static void OnFiguresChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathGeometry geometry = (PathGeometry)d;
            if (null != e.OldValue)
            {
                ((PathFigureCollection)e.OldValue).SetParentPath(null);
            }
            if (geometry.ParentPath != null)
            {
                if (null != e.NewValue)
                {
                    ((PathFigureCollection)e.NewValue).SetParentPath(geometry.ParentPath);
                }

                geometry.ParentPath.ScheduleRedraw();
            }
        }

        private static object CoerceFigures(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PathFigureCollection();
        }

        /// <summary>
        /// Gets or sets a value that determines how the intersecting areas contained
        /// in the PathGeometry are combined.
        /// </summary>
        public FillRule FillRule
        {
            get { return (FillRule)GetValue(FillRuleProperty); }
            set { SetValue(FillRuleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PathGeometry.FillRule"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule), 
                typeof(FillRule), 
                typeof(PathGeometry), 
                new PropertyMetadata(FillRule.EvenOdd, FillRule_Changed));

        private static void FillRule_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathGeometry geometry = (PathGeometry)d;
            if (geometry.ParentPath != null && geometry.ParentPath._isLoaded)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        #endregion

        #region Overriden Methods

        internal override void SetParentPath(Path path)
        {
            base.SetParentPath(path);
            Figures.SetParentPath(path);
        }

        internal override string GetFillRuleAsString()
        {
            return (FillRule == FillRule.Nonzero) ? "nonzero" : "evenodd";
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            foreach (PathFigure figure in Figures)
            {
                figure.GetMinMaxXY(ref minX, ref maxX, ref minY, ref maxY);
            }
        }

        /// <summary>
        /// Applies FillStyle, StrokeStyle + Adds the figures to the canvas' context, then calls the Fill method.
        /// </summary>
        internal protected override void DefineInCanvas(Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize)
        {
            //we define the Fillstyle and StrokeStyle:
            //StrokeStyle:

            if (shapeActualSize.Width > 0 && shapeActualSize.Height > 0)
            {

                //this will change the context in the canvas to draw itself.
                foreach (PathFigure figure in Figures)
                {
                    //Add the figure to the canvas:
                    figure.DefineInCanvas(xOffsetToApplyBeforeMultiplication, 
                                          yOffsetToApplyBeforeMultiplication, 
                                          xOffsetToApplyAfterMultiplication, 
                                          yOffsetToApplyAfterMultiplication, 
                                          horizontalMultiplicator, 
                                          verticalMultiplicator, 
                                          canvasDomElement, 
                                          this.ParentPath != null ? this.ParentPath.StrokeThickness : 0, // Note : this paramater is unused 
                                          shapeActualSize);
                }
            }
        }

        #endregion

        internal new static object INTERNAL_ConvertFromString(string pathAsString)
        {
            return GeometryParser.ParseGeometry(pathAsString);
        }
    }
}