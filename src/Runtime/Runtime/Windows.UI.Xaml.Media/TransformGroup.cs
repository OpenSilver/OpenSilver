

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


using System.Windows.Markup;
using CSHTML5.Internal;
using System.Collections.Generic;
using System.Globalization;

#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
#endif


#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [ContentProperty(nameof(Children))]
    public sealed partial class TransformGroup : Transform
    {
        public TransformGroup()
        {
        }
        
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

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register(
                nameof(Children), 
                typeof(TransformCollection), 
                typeof(TransformGroup), 
                new PropertyMetadata((object)null));

        internal override Matrix Value
        {
            get
            {
                TransformCollection children = Children;
                if ((children == null) || (children.Count == 0))
                {
                    return new Matrix();
                }

                Matrix transform = children[0].Value;

                for (int i = 1; i < children.Count; i++)
                {
                    transform = Matrix.Multiply(transform, children[i].Value);
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
                ApplyCSSChanges(this.Value);
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
