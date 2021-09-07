

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
using System.Windows.Markup;

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
    [ContentProperty("Children")]
    [OpenSilver.NotImplemented]
    public sealed partial class GeometryGroup : Geometry
    {
        internal protected override void DefineInCanvas(
            Path path, 
            object canvasDomElement, 
            double horizontalMultiplicator, 
            double verticalMultiplicator, 
            double xOffsetToApplyBeforeMultiplication, 
            double yOffsetToApplyBeforeMultiplication, 
            double xOffsetToApplyAfterMultiplication, 
            double yOffsetToApplyAfterMultiplication, 
            Size shapeActualSize)
        {
            GeometryCollection children = (GeometryCollection)GetValue(ChildrenProperty);
            if (children != null)
            {
                foreach (Geometry child in children)
                {
                    child.DefineInCanvas(
                        path, 
                        canvasDomElement, 
                        horizontalMultiplicator, 
                        verticalMultiplicator, 
                        xOffsetToApplyBeforeMultiplication, 
                        yOffsetToApplyBeforeMultiplication, 
                        xOffsetToApplyAfterMultiplication, 
                        yOffsetToApplyAfterMultiplication, 
                        shapeActualSize);
                }
            }
        }

        internal protected override void GetMinMaxXY(
            ref double minX, 
            ref double maxX, 
            ref double minY, 
            ref double maxY)
        {
            GeometryCollection children = (GeometryCollection)GetValue(ChildrenProperty);
            if (children != null)
            {
                foreach (Geometry child in children)
                {
                    child.GetMinMaxXY(
                        ref minX, 
                        ref maxX, 
                        ref minY, 
                        ref maxY);
                }
            }
        }

        /// <summary>
        ///     Children - GeometryCollection.  Default value is new FreezableDefaultValueFactory(GeometryCollection.Empty).
        /// </summary>
        public GeometryCollection Children
        {
            get
            {
                GeometryCollection value = (GeometryCollection)GetValue(ChildrenProperty);
                if (value == null)
                {
                    value = new GeometryCollection();
                    SetValue(ChildrenProperty, value);
                }
                return value;
            }
            set
            {
                SetValue(ChildrenProperty, value);
            }
        }

        public static readonly DependencyProperty ChildrenProperty = 
            DependencyProperty.Register(
                nameof(Children), 
                typeof(GeometryCollection), 
                typeof(GeometryGroup), 
                null);

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

        public static readonly DependencyProperty FillRuleProperty = 
            DependencyProperty.Register(
                nameof(FillRule), 
                typeof(FillRule), 
                typeof(GeometryGroup), 
                null);
    }
}
