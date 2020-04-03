

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Paints an area with a radial gradient. A focal point defines the beginning
    /// of the gradient, and a circle defines the end point of the gradient.
    /// </summary>
    public sealed partial class RadialGradientBrush : GradientBrush, ICanConvertToCSSValues
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class.
        /// </summary>
        public RadialGradientBrush() {}
       
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class that has the specified gradient stops.
        /// </summary>
        /// <param name="gradientStopCollection">The gradient stops to set on this brush.</param>
        public RadialGradientBrush(GradientStopCollection gradientStopCollection)
        {
            GradientStops = gradientStopCollection;
        }
  
        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.RadialGradientBrush
        /// class with the specified start and stop colors.
        /// </summary>
        /// <param name="startColor">
        /// Color value at the focus (System.Windows.Media.RadialGradientBrush.GradientOrigin)
        /// of the radial gradient.
        /// </param>
        /// <param name="endColor">Color value at the outer edge of the radial gradient.</param>
        public RadialGradientBrush(Color startColor, Color endColor)
        {
            GradientStopCollection gradientStops = new GradientStopCollection();
            gradientStops.Add(new GradientStop() { Color = startColor, Offset = 0 });
            gradientStops.Add(new GradientStop() { Color = endColor, Offset = 1 });
            GradientStops = gradientStops;
        }

        /// <summary>
        /// Gets or sets the center of the outermost circle of the radial gradient.
        /// </summary>
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Media.RadialGradientBrush.Center dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(RadialGradientBrush), new PropertyMetadata(new Point(0.5,0.5))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        // Returns:
        //     The location of the two-dimensional focal point of the gradient. The default
        //     is (0.5, 0.5). It currently only works on Shapes.
        /// <summary>
        /// Gets or sets the location of the two-dimensional focal point that defines
        /// the beginning of the gradient.
        /// </summary>
        public Point GradientOrigin
        {
            get { return (Point)GetValue(GradientOriginProperty); }
            set { SetValue(GradientOriginProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Media.RadialGradientBrush.GradientOrigin dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty GradientOriginProperty =
            DependencyProperty.Register("GradientOrigin", typeof(Point), typeof(RadialGradientBrush), new PropertyMetadata(new Point(0.5, 0.5))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the horizontal radius of the outermost circle of the radial
        /// gradient.
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Media.RadialGradientBrush.RadiusX dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(RadialGradientBrush), new PropertyMetadata(0.5)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the vertical radius of the outermost circle of a radial gradient.
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Media.RadialGradientBrush.RadiusY dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(RadialGradientBrush), new PropertyMetadata(0.5)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



        private string GetGradientStopsString()
        {
            string gradientStopsString = "";
            var orderedGradientStops = GradientStops.OrderBy((element) => { return element.Offset; }); //todo: there might be a better way of doing that since I think the OrderBy method is slow.
            bool isFirst = true;

            foreach (GradientStop gradientStop in orderedGradientStops)
            {
                if(!isFirst)
                {
                    gradientStopsString +=", " ;
                }
                gradientStopsString += gradientStop.Color.INTERNAL_ToHtmlString(this.Opacity) + gradientStop.Offset * 100 + "%";
                isFirst = false;
            }

            return gradientStopsString;
        }

        internal List<object> INTERNAL_ToHtmlString(DependencyObject parent)
        {
      //      background-image: radial-gradient(50% 50% at 50% 100%,
      //blue 0%, white 10%, purple 20%, purple 99%, black 100%);
            string gradientStopsString = GetGradientStopsString();

            string percentageSymbol = "%";
            int multiplicatorForPercentage = 100;
            if (MappingMode == BrushMappingMode.Absolute)
            {
                percentageSymbol = "px";
                multiplicatorForPercentage = 1;
            }

            string gradientString = string.Format("radial-gradient({0}{5} {1}{5} at {2}{5} {3}{5}, {4})",
                (int)(RadiusX * multiplicatorForPercentage), //{0}
                (int)(RadiusY * multiplicatorForPercentage), //{1}
                (int)(Center.X * multiplicatorForPercentage), //{2}
                (int)(Center.Y * multiplicatorForPercentage), //{3}
                gradientStopsString, //{4}
                percentageSymbol); //{5}

            if(SpreadMethod == GradientSpreadMethod.Repeat)
            {
                gradientString = "repeating-" + gradientString;
            }
            List<object> returnValues = new List<object>();
            returnValues.Add("-webkit-" + gradientString);
            returnValues.Add("-o-" + gradientString);
            returnValues.Add("-moz-" + gradientString);
            returnValues.Add(gradientString);
            return returnValues;
        }

        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            return (List<object>)INTERNAL_ToHtmlString(parent);
        }

        #region not implemented
        
        //// Returns:
        ////     A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        ////     property will be false even if the source's System.Windows.Freezable.IsFrozen
        ////     property was true.
        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.RadialGradientBrush,
        ///// making deep copies of this object's values. When copying dependency properties,
        ///// this method copies resource references and data bindings (but they might
        ///// no longer resolve) but not animations or their current values.
        ///// </summary>
        ///// <returns></returns>
        //public RadialGradientBrush Clone();

        //// Returns:
        ////     A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        ////     property will be false even if the source's System.Windows.Freezable.IsFrozen
        ////     property was true.
        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.RadialGradientBrush
        ///// object, making deep copies of this object's current values. Resource references,
        ///// data bindings, and animations are not copied, but their current values are.
        ///// </summary>
        ///// <returns></returns>
        //public RadialGradientBrush CloneCurrentValue();
        //protected override Freezable CreateInstanceCore();


        #endregion
    }
}
