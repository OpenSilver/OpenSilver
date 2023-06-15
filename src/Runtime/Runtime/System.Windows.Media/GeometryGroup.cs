
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
using OpenSilver.Internal;

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
    /// Represents a composite geometry, composed of other <see cref="Geometry"/> objects.
    /// </summary>
    [ContentProperty(nameof(Children))]
    [OpenSilver.NotImplemented]
    public sealed class GeometryGroup : Geometry
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
        /// Gets or sets the <see cref="GeometryCollection"/> that contains the objects
        /// that define this <see cref="GeometryGroup"/>.
        /// </summary>
        /// <returns>
        /// A collection containing the children of this <see cref="GeometryGroup"/>.
        /// </returns>
        public GeometryCollection Children
        {
            get { return (GeometryCollection)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Children"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty = 
            DependencyProperty.Register(
                nameof(Children), 
                typeof(GeometryCollection), 
                typeof(GeometryGroup), 
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Geometry>(
                        static () => new GeometryCollection(),
                        static (d, dp) => new GeometryCollection()),
                    null,
                    CoerceChildren));

        private static object CoerceChildren(DependencyObject d, object baseValue)
        {
            return baseValue ?? new GeometryCollection();
        }

        /// <summary>
        /// Gets or sets how the intersecting areas of the objects contained in this <see cref="GeometryGroup"/> 
        /// are combined.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies how the intersecting areas are combined
        /// to form the resulting area. The default is <see cref="FillRule.EvenOdd"/>.
        /// </returns>
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

        /// <summary>
        /// Identifies the <see cref="FillRule"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillRuleProperty = 
            DependencyProperty.Register(
                nameof(FillRule), 
                typeof(FillRule), 
                typeof(GeometryGroup), 
                new PropertyMetadata(FillRule.EvenOdd));

        internal override Rect BoundsInternal
        {
            get
            {
                Rect boundsRect = Rect.Empty;

                GeometryCollection children = (GeometryCollection)GetValue(ChildrenProperty);
                if (children != null && children.Count > 0)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        boundsRect.Union(children[i].Bounds);
                    }
                }

                return boundsRect;
            }
        }
    }
}
