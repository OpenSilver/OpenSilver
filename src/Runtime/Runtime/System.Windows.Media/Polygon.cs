
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

using CSHTML5.Internal;
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
	/// Draws a polygon, which is a connected series of lines that form a closed shape.
	/// </summary>
	public sealed class Polygon : Shape
	{
		/// <summary>
		/// Identifies the <see cref="Polygon.FillRule"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty FillRuleProperty =
			DependencyProperty.Register(nameof(FillRule),
										typeof(FillRule),
										typeof(Polygon),
										new PropertyMetadata(FillRule.EvenOdd));

		/// <summary>
		/// Identifies the <see cref="Points"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register(
				nameof(Points),
				typeof(PointCollection),
				typeof(Polygon),
				new PropertyMetadata(
					new PFCDefaultValueFactory<Point>(
						static () => new PointCollection(),
						static (d, dp) => new PointCollection()),
					OnPointsChanged,
					CoercePoints));

		private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Polygon p = (Polygon)d;
			p.ScheduleRedraw();
		}

		private static object CoercePoints(DependencyObject d, object baseValue)
		{
			return baseValue ?? new PointCollection();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
		{
		}

		/// <summary>
		/// Gets or sets a value that specifies how the interior fill of the shape is determined.
		/// </summary>
		/// <returns>
		/// A value of the enumeration. The default is <see cref="FillRule.EvenOdd"/>.
		/// </returns>
		public FillRule FillRule
		{
			get { return (FillRule)GetValue(FillRuleProperty); }
			set { SetValue(FillRuleProperty, value); }
		}

		/// <summary>
		/// Gets or sets a collection that contains the vertex points of the polygon.
		/// </summary>
		/// <returns>
		/// A collection of <see cref="Point"/> structures that describes the vertex points
		/// of the polygon. The default is null. The value can be expressed as a string as
		/// described in "pointSet Grammar" below.
		/// </returns>
		public PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
		{
			return INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(this, parentRef, out _canvasDomElement, out domElementWhereToPlaceChildren);
		}
		
		override internal protected void Redraw()
		{
			if (Points.Count < 2 || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
			{
				// It is fine to have 0 or 1 points but nothing to draw in that case.
				return;
			}

			double minX = Points[0].X;
			double minY = Points[0].Y;
			double maxX = Points[0].X;
			double maxY = Points[0].Y;

			foreach (var p in Points)
			{
				if (p.X < minX) { minX = p.X; }
				if (p.Y < minY) { minY = p.Y; }
				if (p.X > maxX) { maxX = p.X; }
				if (p.Y > maxY) { maxY = p.Y; }
			}

			Size shapeActualSize;
			INTERNAL_ShapesDrawHelpers.PrepareStretch(this, _canvasDomElement, minX, maxX, minY, maxY, Stretch, out shapeActualSize);

			double horizontalMultiplicator;
			double verticalMultiplicator;
			double xOffsetToApplyBeforeMultiplication;
			double yOffsetToApplyBeforeMultiplication;
			double xOffsetToApplyAfterMultiplication;
			double yOffsetToApplyAfterMultiplication;
			INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(this, StrokeThickness, minX, maxX, minY, maxY, Stretch, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out _marginOffsets);

			ApplyMarginToFixNegativeCoordinates(new Point());

			if (Stretch == Stretch.None)
			{
				ApplyMarginToFixNegativeCoordinates(_marginOffsets);
			}

			string sContext = $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_canvasDomElement)}.getContext('2d')";

            //we remove the previous drawing:
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
				$"{sContext}.clearRect(0,0,{shapeActualSize.Width.ToInvariantString()},{shapeActualSize.Height.ToInvariantString()});");

			double opacity = Stroke == null ? 1 : Stroke.Opacity;
			object strokeValue = GetHtmlBrush(this, Stroke, opacity, minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);
			object fillValue = GetHtmlBrush(this, Fill, opacity, minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);

			if (fillValue != null)
			{
				string sFill = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(fillValue);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sContext}.fillStyle = {sFill};");
			}
			else
			{
                // If fillValue is not set it will be black.
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sContext}.fillStyle = 'transparent';");
			}

			INTERNAL_ShapesDrawHelpers.PrepareLines(_canvasDomElement, Points, StrokeThickness, true);

			if (strokeValue != null)
			{
				string sStroke = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(strokeValue);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sContext}.strokeStyle = {sStroke};");
            }

            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sContext}.lineWidth = {StrokeThickness.ToInvariantString()};");
			if (Stroke != null && StrokeThickness > 0)
			{
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sContext}.stroke();");
			}
		}
	}
}