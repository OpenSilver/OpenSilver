
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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetForHtml5.Core;
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
    /// Provides a base class for objects that define geometric shapes. Geometry
    /// objects can be used for clipping regions and as geometry definitions for
    /// rendering two-dimensional graphical data as a Path.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(GeometryConverter))]
#endif
    public abstract class Geometry : DependencyObject
    {
        internal Path INTERNAL_parentPath = null;
        internal virtual void SetParentPath(Path path)
        {
            INTERNAL_parentPath = path;
        }

        ///// <summary>
        ///// Gets a Rect that specifies the axis-aligned bounding box of the Geometry.
        ///// </summary>
        //public Rect Bounds { get; }

        ///// <summary>
        ///// Gets an empty geometry object.
        ///// </summary>
        //public static Geometry Empty { get; }

        //// Returns:
        ////     The standard tolerance. The default value is 0.25.
        ///// <summary>
        ///// Gets the standard tolerance used for polygonal approximation.
        ///// </summary>
        //public static double StandardFlatteningTolerance { get; }

        //// Returns:
        ////     The transformation applied to the Geometry. Note that this value may be a
        ////     single Transform or a list of Transform items.
        ///// <summary>
        ///// Gets or sets the Transform object applied to a Geometry.
        ///// </summary>
        //public Transform Transform
        //{
        //    get { return (Transform)GetValue(TransformProperty); }
        //    set { SetValue(TransformProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the Transform dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TransformProperty =
        //    DependencyProperty.Register("Transform", typeof(Transform), typeof(Geometry), new PropertyMetadata(null, Transform_Changed));

        //private static void Transform_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Draws the Geometry on the canvas.
        /// </summary>
        internal abstract void DefineInCanvas(Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize);

        internal abstract void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY);

        static Geometry()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Geometry), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string pathAsString)
        {
            return PathGeometry.INTERNAL_ConvertFromString(pathAsString);
        }


        internal virtual void SetFill(bool newIsFilled)
        {
        }

        internal virtual string GetFillRuleAsString()
        {
            return "evenodd";
        }
    }
}