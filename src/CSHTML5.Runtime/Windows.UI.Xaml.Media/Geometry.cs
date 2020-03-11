

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
    public abstract partial class Geometry : DependencyObject
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
        internal protected abstract void DefineInCanvas(Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize);
        internal protected abstract void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY);

        //internal abstract void DefineInCanvas(Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize);
        //internal abstract void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY);

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

#if WORKINPROGRESS
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(Geometry), null);
        public Transform Transform
        {
            get { return (Transform)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        public Rect Bounds { get; private set; }
#endif
    }
}