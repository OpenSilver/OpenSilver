
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
    public sealed class GeometryGroup : Geometry
    {
        internal override void DefineInCanvas(Path path, object canvasDomElement, double horizontalMultiplicator, double verticalMultiplicator, double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, Size shapeActualSize)
        {
            throw new NotImplementedException();
        }

        internal override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
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
