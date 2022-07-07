using CSHTML5.Internal;
using System;

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
    public sealed partial class Polyline : Shape
    {
        static Polyline()
        {
            StretchProperty.OverrideMetadata(typeof(Polyline), new PropertyMetadata(Stretch.Fill, Stretch_Changed));
        }

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty =
            DependencyProperty.Register(
                nameof(FillRule),
                typeof(FillRule),
                typeof(Polyline),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Identifies the <see cref="Points"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(PointCollection),
                typeof(Polyline),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPointsChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="Polyline"/> class.
        /// </summary>
        public Polyline()
        {
        }

        /// <summary>
        /// Gets or sets a value that specifies how the interior fill of the shape is determined.
        /// </summary>
        /// <returns>
        /// A value of the enumeration that specifies the fill behavior. The default is <see cref="FillRule.EvenOdd"/>.
        /// </returns>
        public FillRule FillRule
        {
            get
            {
                return (FillRule)this.GetValue(FillRuleProperty);
            }
            set
            {
                this.SetValue(FillRuleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the <see cref="Polyline"/>.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Point"/> structures that describe the vertex points
        /// of the <see cref="Polyline"/>. The default is null.
        /// </returns>
        public PointCollection Points
        {
            get
            {
                PointCollection points = (PointCollection)GetValue(PointsProperty);

                if (points == null)
                {
                    points = new PointCollection();
                    _suspendRendering = true;
                    this.SetValue(PointsProperty, points);
                    _suspendRendering = false;
                }

                return points;
            }
            set
            {
                this.SetValue(PointsProperty, value);
            }
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (Polyline)d;
            if (line == null) return;

            if (e.OldValue != null && e.OldValue is PointCollection)
                ((PointCollection)(e.OldValue)).SetParentShape(null);

            if (e.NewValue != null && e.NewValue is PointCollection)
                ((PointCollection)(e.NewValue)).SetParentShape(line);


            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(line))
            {
                line.InvalidateMeasure();
                line.ScheduleRedraw();
            }
        }


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(this, parentRef, out _canvasDomElement, out domElementWhereToPlaceChildren);
        }


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

            double minX, minY, maxX, maxY;
            GetMinMaxXY(out minX, out maxX, out minY, out maxY);

            Size shapeActualSize;
            INTERNAL_ShapesDrawHelpers.PrepareStretch(this, _canvasDomElement, 0, maxX, 0, maxY, Stretch, out shapeActualSize);

            double horizontalMultiplicator;
            double verticalMultiplicator;
            double xOffsetToApplyBeforeMultiplication;
            double yOffsetToApplyBeforeMultiplication;
            double xOffsetToApplyAfterMultiplication;
            double yOffsetToApplyAfterMultiplication;
            INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(this, StrokeThickness, 0, maxX, 0, maxY,
                Stretch, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication,
                out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out _marginOffsets);

            ApplyMarginToFixNegativeCoordinates(new Point());

            if (Stretch == Stretch.None)
            {
                ApplyMarginToFixNegativeCoordinates(_marginOffsets);
            }

            INTERNAL_ShapesDrawHelpers.PrepareLines(_canvasDomElement, Points, StrokeThickness, false);
            //todo: make sure the parameters below are correct.
            Shape.DrawFillAndStroke(this, FillRule == FillRule.Nonzero ? "nonzero" : "evenodd", xOffsetToApplyAfterMultiplication,
                yOffsetToApplyAfterMultiplication, xOffsetToApplyAfterMultiplication + maxX, yOffsetToApplyAfterMultiplication + maxY,
                horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var points = (PointCollection)GetValue(PointsProperty);
            if (points == null || points.Count < 2)
            {
                return base.MeasureOverride(availableSize);
            }

            double minX, minY, maxX, maxY;
            GetMinMaxXY(out minX, out maxX, out minY, out maxY);

            return new Size(availableSize.Width.Min(maxX + StrokeThickness), availableSize.Height.Min(maxY + StrokeThickness));
        }
    }
}