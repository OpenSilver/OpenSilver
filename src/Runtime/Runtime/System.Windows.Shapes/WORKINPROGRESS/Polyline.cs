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
	//
	// Summary:
	//     Draws a series of connected straight lines.
	public sealed partial class Polyline : Shape
	{
		static Polyline()
		{
			Shape.StretchProperty.OverrideMetadata(typeof(Polyline), new PropertyMetadata(Stretch.Fill, Shape.Stretch_Changed)
			{ CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
		}

		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polyline.FillRule dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polyline.FillRule dependency property.
		public static readonly DependencyProperty FillRuleProperty =
			DependencyProperty.Register(
				"FillRule",
				typeof(FillRule),
				typeof(Polyline),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polyline.Points dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polyline.Points dependency property.
		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register(
				"Points",
				typeof(PointCollection),
				typeof(Polyline),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPointsChanged));
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Shapes.Polyline class.
		public Polyline()
		{
		}

        //
        // Summary:
        //     Gets or sets a value that specifies how the interior fill of the shape is determined.
        //
        // Returns:
        //     A value of the enumeration that specifies the fill behavior. The default is System.Windows.Media.FillRule.EvenOdd.
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

		//
		// Summary:
		//     Gets or sets a collection that contains the vertex points of the System.Windows.Shapes.Polyline.
		//
		// Returns:
		//     A collection of System.Windows.Point structures that describe the vertex points
		//     of the System.Windows.Shapes.Polyline. The default is null.
		public PointCollection Points
		{
			get
			{
				PointCollection points = (PointCollection)GetValue(PointsProperty);

				if (points == null)
				{
					points = new PointCollection();
					this.SetValue(PointsProperty, points);
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

			line.SetValue(PointsProperty, e.NewValue);

			if (line.IsLoaded)
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

			if (Points?.Count < 2)
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
			INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(this, StrokeThickness, 0, maxX, 0, maxY, Stretch, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out _marginOffsets);

			ApplyMarginToFixNegativeCoordinates(new Point());

			if (Stretch == Stretch.None)
			{
				ApplyMarginToFixNegativeCoordinates(_marginOffsets);
			}

			INTERNAL_ShapesDrawHelpers.PrepareLines(_canvasDomElement, Points, false);
			//todo: make sure the parameters below are correct.
			Shape.DrawFillAndStroke(this, "evenodd", xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, xOffsetToApplyAfterMultiplication + maxX, 
				yOffsetToApplyAfterMultiplication + maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, 
				yOffsetToApplyBeforeMultiplication, shapeActualSize);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (Points?.Count < 2)
			{
				return base.MeasureOverride(availableSize);
			}

			double minX, minY, maxX, maxY;
			GetMinMaxXY(out minX, out maxX, out minY, out maxY);

			return new Size(availableSize.Width.Min(maxX + StrokeThickness), availableSize.Height.Min(maxY + StrokeThickness));
		}
	}
}

