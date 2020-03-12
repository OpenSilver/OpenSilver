

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
    [ContentProperty("Children")]
    public sealed partial class TransformGroup : Transform
    {
        public TransformGroup()
        {
            Children = TransformCollection.Empty;
        }
        
        public TransformCollection Children
        {
            get { return (TransformCollection)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(TransformCollection), typeof(TransformGroup), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            return new Point();
        }

        private void ApplyCSSChanges(TransformGroup transformGroup, TransformCollection children)
        {
            int childrenCount = (children != null) ? children.Count : 0;
            UIElement parent = transformGroup.INTERNAL_parent;
            Transform child;
            for (int i = childrenCount - 1; i > -1; i--)
            {
                child = children[i];
                if (child.INTERNAL_parent != parent)
                {
                    child.INTERNAL_parent = parent;
                }
                child.INTERNAL_ApplyCSSChanges();
            }
        }

        private void UnapplyCSSChanges(TransformGroup transformGroup, TransformCollection children)
        {
            int childrenCount = (children != null) ? children.Count : 0;
            UIElement parent = transformGroup.INTERNAL_parent;
            Transform child;
            for (int i = childrenCount - 1; i > -1; i--)
            {
                child = children[i];
                if (child.INTERNAL_parent != parent)
                {
                    child.INTERNAL_parent = parent;
                }
                child.INTERNAL_UnapplyCSSChanges();
            }
        }

        internal override void INTERNAL_ApplyCSSChanges()
        {
            ApplyCSSChanges(this, Children);
        }

        internal override void INTERNAL_UnapplyCSSChanges()
        {
            UnapplyCSSChanges(this, Children);
        }

        internal void ApplyTransformGroup(TransformCollection children)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                ApplyCSSChanges(this, children);
            }
        }

        internal void UnapplyTransformGroup(TransformCollection children)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                UnapplyCSSChanges(this, children);
            }
        }

        internal override void INTERNAL_ApplyTransform()
        {
            ApplyTransformGroup(Children);
        }

        internal override void INTERNAL_UnapplyTransform()
        {
            UnapplyTransformGroup(Children);
        }

#if WORKINPROGRESS
        public override GeneralTransform Inverse
        {
            get { return null; }
        }

        public override Rect TransformBounds(Rect rect)
        {
            return Rect.Empty;
        }

        public override bool TryTransform(Point inPoint, out Point outPoint)
        {
            outPoint = new Point();
            return false;
        }
#endif
    }
}
