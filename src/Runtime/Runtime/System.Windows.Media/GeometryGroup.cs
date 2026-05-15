
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

using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using System.Windows.Markup;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a composite geometry, composed of other <see cref="Geometry"/> objects.
    /// </summary>
    [ContentProperty(nameof(Children))]
    public sealed class GeometryGroup : Geometry
    {
        private WeakEventToken _weakEventToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryGroup"/> class.
        /// </summary>
        public GeometryGroup() { }

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
                        static (d, dp) =>
                        {
                            GeometryGroup geometry = (GeometryGroup)d;
                            var collection = new GeometryCollection();
                            geometry.ProvideSelfAsInheritanceContext(collection, null);
                            geometry.OnChildrenChanged(null, collection);
                            return collection;
                        }),
                    OnChildrenChanged,
                    CoerceChildren));

        /// <summary>
        /// Gets or sets the <see cref="GeometryCollection"/> that contains the objects
        /// that define this <see cref="GeometryGroup"/>.
        /// </summary>
        /// <returns>
        /// A collection containing the children of this <see cref="GeometryGroup"/>.
        /// </returns>
        public GeometryCollection Children
        {
            get => (GeometryCollection)GetValue(ChildrenProperty);
            set => SetValueInternal(ChildrenProperty, value);
        }

        private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryGroup)d).OnChildrenChanged((GeometryCollection)e.OldValue, (GeometryCollection)e.NewValue);
            OnPathChanged(d, e);
        }

        private static object CoerceChildren(DependencyObject d, object baseValue)
        {
            return baseValue ?? new GeometryCollection();
        }

        private void OnChildrenCollectionChanged(object sender, EventArgs e) => RaisePathChanged();

        private void OnChildrenChanged(GeometryCollection oldChildren, GeometryCollection newChildren)
        {
            if (_weakEventToken != null)
            {
                _weakEventToken.Dispose();
                _weakEventToken = null;
            }

            if (newChildren is not null)
            {
                _weakEventToken = WeakEvent.Subscribe<GeometryGroup, GeometryCollection, EventArgs>(
                    this,
                    newChildren,
                    static (instance, sender, args) => instance.OnChildrenCollectionChanged(sender, args),
                    static (handler, source) => source.Changed -= new EventHandler(handler),
                    static (handler, source) => source.Changed += new EventHandler(handler));
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
                new PropertyMetadata(FillRule.EvenOdd, OnFillRuleChanged),
                ValidateEnums.IsFillRuleValid);

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
            get => (FillRule)GetValue(FillRuleProperty);
            set => SetValueInternal(FillRuleProperty, value);
        }

        /// <summary>
        /// Determines whether this <see cref="GeometryGroup"/> object is empty.
        /// </summary>
        /// <returns>
        /// true if this <see cref="GeometryGroup"/> is empty; otherwise, false.
        /// </returns>
        public override bool IsEmpty()
        {
            foreach (Geometry child in Children.InternalItems)
            {
                if (!child.IsEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        internal override bool IsObviouslyEmpty() => Children.InternalCount == 0;

        /// <summary>
        /// Determines whether this <see cref="GeometryGroup"/> object may have curved segments.
        /// </summary>
        /// <returns>
        /// true if this <see cref="GeometryGroup"/> object may have curved segments; otherwise, false.
        /// </returns>
        public override bool MayHaveCurves()
        {
            foreach (Geometry child in Children.InternalItems)
            {
                if (child.MayHaveCurves())
                {
                    return true;
                }
            }

            return false;
        }

        internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
        {
            Matrix matrix = GetCombinedMatrix(transform);

            foreach (var child in Children.InternalItems)
            {
                child.SerializeData(context, matrix);
            }
        }

        internal override FillRule GetFillRule() => FillRule;
    }
}
