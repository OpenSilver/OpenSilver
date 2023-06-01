
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
    public abstract class Geometry : DependencyObject
    {
        /// <summary>
        /// Gets an empty geometry object.
        /// </summary>
        /// <returns>
        /// The empty geometry object.
        /// </returns>
        public static Geometry Empty => new PathGeometry();

        internal Geometry() { }

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

        public static readonly DependencyProperty TransformProperty = 
            DependencyProperty.Register(
                nameof(Transform), 
                typeof(Transform), 
                typeof(Geometry), 
                new PropertyMetadata(null, OnTransformPropertyChanged));
        
        public Transform Transform
        {
            get => (Transform)GetValue(TransformProperty);
            set => SetValue(TransformProperty, value);
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

        /// <summary>
        /// Gets a <see cref="Rect"/> that specifies the axis-aligned bounding box of the
        /// <see cref="Geometry"/>.
        /// </summary>
        /// <returns>
        /// The axis-aligned bounding box of the <see cref="Geometry"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Rect Bounds => BoundsInternal;

        internal virtual Rect BoundsInternal => Rect.Empty;
    }
}