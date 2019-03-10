
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    public class GradientBrush : Brush
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
            DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), typeof(GradientBrush), new PropertyMetadata(null));



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
            DependencyProperty.Register("MappingMode", typeof(BrushMappingMode), typeof(GradientBrush), new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox));



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
            DependencyProperty.Register("SpreadMethod", typeof(GradientSpreadMethod), typeof(GradientBrush), new PropertyMetadata(GradientSpreadMethod.Pad));


    }
}
