
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
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a composite <see cref="Transform"/> composed of other <see cref="Transform"/>
    /// objects.
    /// </summary>
    [ContentProperty(nameof(Children))]
    public sealed class TransformGroup : Transform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformGroup"/> class.
        /// </summary>
        public TransformGroup()
        {
            Changed += (o, e) => INTERNAL_ApplyTransform();
        }

        /// <summary>
        /// Gets or sets the collection of child <see cref="Transform"/> objects.
        /// </summary>
        /// <returns>
        /// The collection of child <see cref="Transform"/> objects. The default is
        /// an empty collection.
        /// </returns>
        public TransformCollection Children
        {
            get { return (TransformCollection)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

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
                            collection.SetParentTransform(tg);
                            return collection;
                        }),
                    OnChildrenChanged,
                    CoerceChildren));

        private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)d;
            TransformCollection oldValue = (TransformCollection)e.OldValue;
            TransformCollection newValue = (TransformCollection)e.NewValue;
            
            oldValue?.SetParentTransform(null);
            newValue?.SetParentTransform(transformGroup);
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
        public Matrix Value => ValueInternal;

        internal override Matrix ValueInternal
        {
            get
            {
                TransformCollection children = Children;
                if ((children == null) || (children.Count == 0))
                {
                    return new Matrix();
                }

                Matrix transform = children[0].ValueInternal;

                for (int i = 1; i < children.Count; i++)
                {
                    transform = Matrix.Multiply(transform, children[i].ValueInternal);
                }

                return transform;
            }
        }

        internal override bool IsIdentity
        {
            get
            {
                TransformCollection children = (TransformCollection)GetValue(ChildrenProperty);
                if (children == null || children.Count == 0)
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

        private void ApplyCSSChanges(Matrix m)
        {
            UIElement target = INTERNAL_parent;
            if (target is not null)
            {
                INTERNAL_HtmlDomManager.SetCSSStyleProperty(
                    target.INTERNAL_OuterDomElement,
                    "transform",
                    MatrixTransform.MatrixToHtmlString(m));
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this.ValueInternal);
            }
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(Matrix.Identity);
            }
        }
    }
}
