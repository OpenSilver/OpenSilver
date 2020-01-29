
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public sealed class RectangleGeometry : Geometry
    {
        internal protected override void DefineInCanvas(Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize)
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
            get
            {
                return (Rect)GetValue(RectProperty);
            }
            set
            {
                SetValue(RectProperty, value);
            }
        }

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

        public static readonly DependencyProperty RectProperty = DependencyProperty.Register("Rect", typeof(Rect), typeof(RectangleGeometry), null);

    }
#endif
}
