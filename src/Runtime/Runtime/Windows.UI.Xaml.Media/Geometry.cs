

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
        static Geometry()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Geometry), INTERNAL_ConvertFromString);
        }

        internal Geometry()
        {

        }

        internal Path ParentPath { get; private set; }

        internal virtual void SetParentPath(Path path)
        {
            ParentPath = path;
        }

        /// <summary>
        /// Draws the Geometry on the canvas.
        /// </summary>
        internal protected abstract void DefineInCanvas(Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize);
        internal protected abstract void GetMinMaxXY(ref double minX, 
                                                     ref double maxX, 
                                                     ref double minY, 
                                                     ref double maxY);

        internal virtual string GetFillRuleAsString()
        {
            return "evenodd";
        }

        internal static object INTERNAL_ConvertFromString(string pathAsString)
        {
            return PathGeometry.INTERNAL_ConvertFromString(pathAsString);
        }

        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", 
                                                                                                  typeof(Transform), 
                                                                                                  typeof(Geometry), 
                                                                                                  new PropertyMetadata(null, OnTransformPropertyChanged));
        public Transform Transform
        {
            get { return (Transform)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        private static void OnTransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // note: currently we can't detect when the transform's property are changing
            // (for instance changing the X property of a TranslateTransform won't refresh
            // the shape)
            Geometry geometry = (Geometry)d;
            if (geometry.ParentPath != null)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

#if WORKINPROGRESS
        [OpenSilver.NotImplemented]
        public Rect Bounds { get; private set; }
#endif
    }
}