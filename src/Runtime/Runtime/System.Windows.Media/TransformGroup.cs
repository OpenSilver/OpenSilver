﻿
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
using System.Collections.Generic;
using System.Globalization;
using CSHTML5.Internal;

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
    public sealed partial class TransformGroup : Transform
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
            get
            {
                var collection = (TransformCollection)GetValue(ChildrenProperty);
                if (collection == null)
                {
                    collection = new TransformCollection();
                    SetValue(ChildrenProperty, collection);
                }
                return collection;
            }
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
                new PropertyMetadata(null, OnChildrenChanged));

        private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)d;
            TransformCollection oldValue = (TransformCollection)e.OldValue;
            TransformCollection newValue = (TransformCollection)e.NewValue;
            
            oldValue?.SetParentTransform(null);
            newValue?.SetParentTransform(transformGroup);
        }

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

        private void ApplyCSSChanges(Matrix m)
        {
            var target = this.INTERNAL_parent;
            if (target != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementStyleProperty(
                target.INTERNAL_OuterDomElement,
                new List<string>(1) { "transform" },
                string.Format(CultureInfo.InvariantCulture,
                              "matrix({0}, {1}, {2}, {3}, {4}, {5})",
                              m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY));
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
