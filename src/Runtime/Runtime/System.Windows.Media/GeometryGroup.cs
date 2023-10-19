
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

using System.Collections.Specialized;
using System.Text;
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a composite geometry, composed of other <see cref="Geometry"/> objects.
    /// </summary>
    [ContentProperty(nameof(Children))]
    public sealed class GeometryGroup : Geometry
    {
        private WeakEventListener<GeometryGroup, GeometryCollection, NotifyCollectionChangedEventArgs> _collectionChangedListener;

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
            set => SetValue(ChildrenProperty, value);
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

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RaisePathChanged();

        private void OnChildrenChanged(GeometryCollection oldChildren, GeometryCollection newChildren)
        {
            oldChildren?.SetOwner(null);

            if (_collectionChangedListener != null)
            {
                _collectionChangedListener.Detach();
                _collectionChangedListener = null;
            }

            if (newChildren is not null)
            {
                newChildren.SetOwner(this);

                _collectionChangedListener = new(this, newChildren)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnChildrenCollectionChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.CollectionChanged -= listener.OnEvent,
                };
                newChildren.CollectionChanged += _collectionChangedListener.OnEvent;
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
                new PropertyMetadata(FillRule.EvenOdd, OnFillRuleChanged));

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
            set => SetValue(FillRuleProperty, value);
        }

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

        internal override string ToPathData(IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();

            foreach (var child in Children)
            {
                var childData = child.ToPathData(formatProvider);
                sb.Append(childData);
            }

            return sb.ToString();
        }

        internal override FillRule GetFillRule() => FillRule;
    }
}
