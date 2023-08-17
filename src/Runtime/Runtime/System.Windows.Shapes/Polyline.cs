
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
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{
    /// <summary>
    /// Draws a series of connected straight lines.
    /// </summary>
    public sealed class Polyline : Shape
    {
        static Polyline()
        {
            StretchProperty.OverrideMetadata(typeof(Polyline), new FrameworkPropertyMetadata(Stretch.Fill));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polyline"/> class.
        /// </summary>
        public Polyline() { }

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule),
                typeof(FillRule),
                typeof(Polyline),
                new FrameworkPropertyMetadata(FillRule.EvenOdd, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value that specifies how the interior fill of the shape is determined.
        /// </summary>
        /// <returns>
        /// A value of the enumeration that specifies the fill behavior. The default is <see cref="FillRule.EvenOdd"/>.
        /// </returns>
        public FillRule FillRule
        {
            get => (FillRule)GetValue(FillRuleProperty);
            set => SetValue(FillRuleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(Polyline),
                new FrameworkPropertyMetadata(
                    new PFCDefaultValueFactory<Point>(
                        static () => new PointCollection(),
                        static (d, dp) =>
                        {
                            Polyline p = (Polyline)d;
                            var collection = new PointCollection();
                            collection.SetParentShape(p);
                            return collection;
                        }),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnPointsChanged,
                    CoercePoints));

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the <see cref="Polyline"/>.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Point"/> structures that describe the vertex points
        /// of the <see cref="Polyline"/>. The default is null.
        /// </returns>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (Polyline)d;

            if (e.OldValue is PointCollection oldCollection)
            {
                oldCollection.SetParentShape(null);
            }

            if (e.NewValue is PointCollection newCollection)
            {
                newCollection.SetParentShape(line);
            }
            
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(line))
            {
                line.InvalidateMeasure();
                line.ScheduleRedraw();
            }
        }

        private static object CoercePoints(DependencyObject d, object baseValue)
        {
            return baseValue ?? new PointCollection();
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
            => INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(
                this,
                parentRef,
                out _canvasDomElement,
                out domElementWhereToPlaceChildren);


        private void GetMinMaxXY(out double minX, out double maxX, out double minY, out double maxY)
        {
            minX = double.MaxValue;
            minY = double.MaxValue;
            maxX = double.MinValue;
            maxY = double.MinValue;

            foreach (var point in Points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }
        }

        protected internal override void Redraw()
        {
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return;
            }

            var points = (PointCollection)GetValue(PointsProperty);

            if (points == null || points.Count < 2)
            {
                return;
            }

            GetMinMaxXY(out double minX, out double maxX, out double minY, out double maxY);

            INTERNAL_ShapesDrawHelpers.PrepareStretch(this, _canvasDomElement, 0, maxX, 0, maxY, Stretch, out Size shapeActualSize);

            INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(
                this,
                StrokeThickness,
                0,
                maxX,
                0,
                maxY,
                Stretch,
                shapeActualSize,
                out double horizontalMultiplicator,
                out double verticalMultiplicator,
                out double xOffsetToApplyBeforeMultiplication,
                out double yOffsetToApplyBeforeMultiplication,
                out double xOffsetToApplyAfterMultiplication,
                out double yOffsetToApplyAfterMultiplication,
                out _marginOffsets);

            ApplyMarginToFixNegativeCoordinates(new Point());

            if (Stretch == Stretch.None)
            {
                ApplyMarginToFixNegativeCoordinates(_marginOffsets);
            }

            INTERNAL_ShapesDrawHelpers.PrepareLines(_canvasDomElement, Points, StrokeThickness, false);
            //todo: make sure the parameters below are correct.
            DrawFillAndStroke(
                this,
                FillRule == FillRule.Nonzero ? "nonzero" : "evenodd",
                xOffsetToApplyAfterMultiplication,
                yOffsetToApplyAfterMultiplication,
                xOffsetToApplyAfterMultiplication + maxX,
                yOffsetToApplyAfterMultiplication + maxY,
                horizontalMultiplicator,
                verticalMultiplicator,
                xOffsetToApplyBeforeMultiplication,
                yOffsetToApplyBeforeMultiplication,
                shapeActualSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var points = (PointCollection)GetValue(PointsProperty);
            if (points == null || points.Count < 2)
            {
                return base.MeasureOverride(availableSize);
            }

            GetMinMaxXY(out double minX, out double maxX, out double minY, out double maxY);

            return new Size(availableSize.Width.Min(maxX + StrokeThickness), availableSize.Height.Min(maxY + StrokeThickness));
        }
    }
}