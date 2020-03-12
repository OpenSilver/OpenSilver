

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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// An abstract class that describes a gradient, composed of gradient stops.
    /// Classes that derive from GradientBrush describe different ways of interpreting
    /// gradient stops.
    /// </summary>
    [ContentProperty("GradientStops")]
    public partial class GradientBrush : Brush
    {
        //// Returns:
        ////     Specifies how the colors in a gradient are interpolated. The default is SRgbLinearInterpolation.
        ///// <summary>
        ///// Gets or sets a ColorInterpolationMode enumeration value that specifies how
        ///// the gradient's colors are interpolated.
        ///// </summary>
        //public ColorInterpolationMode ColorInterpolationMode
        //{
        //    get { return (ColorInterpolationMode)GetValue(ColorInterpolationModeProperty); }
        //    set { SetValue(ColorInterpolationModeProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the ColorInterpolationMode dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ColorInterpolationModeProperty =
        //    DependencyProperty.Register("ColorInterpolationMode", typeof(ColorInterpolationMode), typeof(GradientBrush), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the brush's gradient stops, which is a collection of the GradientStop
        /// objects associated with the brush, each of which specifies a color and an offset
        /// along the brush's gradient axis. The default is an empty GradientStopCollection.
        /// </summary>
        public GradientStopCollection GradientStops
        {
            get
            {
                GradientStopCollection collection;
                if (GetValue(GradientStopsProperty) == null)
                {
                    collection = new GradientStopCollection();
#if WORKINPROGRESS
                    collection.INTERNAL_ParentBrush = this;
#endif
                    SetValue(GradientStopsProperty, collection);
                }
                else
                {
                    collection = (GradientStopCollection)GetValue(GradientStopsProperty);
                }
                return collection;
            }
            set
            {
#if WORKINPROGRESS
                value.INTERNAL_ParentBrush = this;
#endif
                SetValue(GradientStopsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the GradientStops dependency property.
        /// </summary>
        public static readonly DependencyProperty GradientStopsProperty =
            DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), typeof(GradientBrush), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



        /// <summary>
        /// Gets or sets a BrushMappingMode enumeration value that specifies whether
        /// the positioning coordinates of the gradient brush are absolute or relative
        /// to the output area.
        /// </summary>
        public BrushMappingMode MappingMode
        {
            get { return (BrushMappingMode)GetValue(MappingModeProperty); }
            set { SetValue(MappingModeProperty, value); }
        }
        /// <summary>
        /// Identifies the MappingMode dependency property.
        /// </summary>
        public static readonly DependencyProperty MappingModeProperty =
            DependencyProperty.Register("MappingMode", typeof(BrushMappingMode), typeof(GradientBrush), new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



        /// <summary>
        /// Gets or sets the type of spread method that specifies how to draw a gradient
        /// that starts or ends inside the bounds of the object to be painted.
        /// The default is Pad.
        /// </summary>
        public GradientSpreadMethod SpreadMethod
        {
            get { return (GradientSpreadMethod)GetValue(SpreadMethodProperty); }
            set { SetValue(SpreadMethodProperty, value); }
        }

        /// <summary>
        /// Identifies the SpreadMethod dependency property.
        /// </summary>
        public static readonly DependencyProperty SpreadMethodProperty =
            DependencyProperty.Register("SpreadMethod", typeof(GradientSpreadMethod), typeof(GradientBrush), new PropertyMetadata(GradientSpreadMethod.Pad)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


    }
}
