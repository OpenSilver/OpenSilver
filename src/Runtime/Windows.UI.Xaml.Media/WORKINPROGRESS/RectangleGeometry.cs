

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
using System.Windows;
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
#if WORKINPROGRESS
    public sealed partial class RectangleGeometry : Geometry
    {
        internal protected override void DefineInCanvas(Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize)
        {
            throw new NotImplementedException();
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Rect - Rect.  Default value is Rect.Empty.
        /// </summary>
        public Rect Rect
        {
            get { return (Rect)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public static readonly DependencyProperty RectProperty = DependencyProperty.Register("Rect", typeof(Rect), typeof(RectangleGeometry), null);


#if WORKINPROGRESS
        // Summary:
        //     Gets or sets the x-radius of the ellipse that is used to round the corners of
        //     the rectangle.
        //
        // Returns:
        //     The x-radius of the ellipse used to round the corners of the rectangle geometry.
        //     The default is 0.
        public double RadiusX { get; set; }
        //
        // Summary:
        //     Gets or sets the y-radius of the ellipse that is used to round the corners of
        //     the rectangle.
        //
        // Returns:
        //     The y-radius of the ellipse used to round the corners of the rectangle geometry.
        //     The default is 0.
        public double RadiusY { get; set; }
#endif


    }
#endif
}
