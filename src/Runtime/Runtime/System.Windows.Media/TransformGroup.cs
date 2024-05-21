
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a composite <see cref="Transform"/> composed of other <see cref="Transform"/> objects.
    /// </summary>
    [ContentProperty(nameof(Children))]
    public sealed class TransformGroup : Transform
    {
        private WeakEventListener<TransformGroup, TransformCollection, NotifyCollectionChangedEventArgs> _collectionChangedListener;

        /// <summary>
        /// Identifies the <see cref="Children"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register(
                nameof(Children),
                typeof(TransformCollection),
                typeof(TransformGroup),
                new PropertyMetadata(
                    new PFCDefaultValueFactory<Transform>(
                        static () => new TransformCollection(),
                        static (d, dp) =>
                        {
                            TransformGroup tg = (TransformGroup)d;
                            var collection = new TransformCollection();
                            tg.OnChildrenChanged(null, collection);
                            return collection;
                        }),
                    OnChildrenChanged,
                    CoerceChildren));

        /// <summary>
        /// Gets or sets the collection of child <see cref="Transform"/> objects.
        /// </summary>
        /// <returns>
        /// The collection of child <see cref="Transform"/> objects. The default is
        /// an empty collection.
        /// </returns>
        public TransformCollection Children
        {
            get => (TransformCollection)GetValue(ChildrenProperty);
            set => SetValueInternal(ChildrenProperty, value);
        }

        private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransformGroup tg = (TransformGroup)d;
            tg.OnChildrenChanged((TransformCollection)e.OldValue, (TransformCollection)e.NewValue);
            tg.OnTransformChanged();
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnTransformChanged();

        private void OnChildrenChanged(TransformCollection oldChildren, TransformCollection newChildren)
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

        private static object CoerceChildren(DependencyObject d, object baseValue)
        {
            return baseValue ?? new TransformCollection();
        }

        /// <summary>
        /// Gets the <see cref="Matrix"/> structure that describes the transformation
        /// represented by this <see cref="TransformGroup"/>.
        /// </summary>
        /// <returns>
        /// A composite of the <see cref="Transform"/> objects in this <see cref="TransformGroup"/>.
        /// </returns>
        public Matrix Value => Matrix;

        private protected override Matrix GetMatrixCore()
        {
            List<Transform> children = Children.InternalItems;
            if (children.Count == 0)
            {
                return new Matrix();
            }

            Matrix transform = children[0].Matrix;

            for (int i = 1; i < children.Count; i++)
            {
                transform = Matrix.Multiply(transform, children[i].Matrix);
            }

            return transform;
        }

        internal override bool IsIdentity
        {
            get
            {
                List<Transform> children = Children.InternalItems;
                if (children.Count == 0)
                {
                    return true;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    if (!children[i].IsIdentity)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
