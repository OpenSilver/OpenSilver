
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
using System.Windows.Markup;
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
    [ContentProperty("Children")]
    public sealed partial class GeometryGroup : Geometry
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
        ///     Children - GeometryCollection.  Default value is new FreezableDefaultValueFactory(GeometryCollection.Empty).
        /// </summary>
        public GeometryCollection Children
        {
            get
            {
                return (GeometryCollection)GetValue(ChildrenProperty);
            }
            set
            {
                SetValue(ChildrenProperty, value);
            }
        }

        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(GeometryCollection), typeof(GeometryGroup), null);

        /// <summary>
        ///     FillRule - FillRule.  Default value is FillRule.EvenOdd.
        /// </summary>
        public FillRule FillRule
        {
            get
            {
                return (FillRule)GetValue(FillRuleProperty);
            }
            set
            {
                SetValue(FillRuleProperty, value);
            }
        }

        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register("FillRule", typeof(FillRule), typeof(GeometryGroup), null);
    }
#endif
}
