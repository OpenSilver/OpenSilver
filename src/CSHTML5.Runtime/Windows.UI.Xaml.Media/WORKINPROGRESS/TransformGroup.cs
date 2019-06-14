
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
#if WORKINPROGRESS
    [ContentProperty("Children")]
    public sealed class TransformGroup : Transform
    {
        public TransformCollection Children
        {
            get { return (TransformCollection)GetValue(TransformGroup.ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(TransformCollection), typeof(TransformGroup), new PropertyMetadata(TransformCollection.Empty));

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            return new Point();
        }

#if WORKINPROGRESS
        private void ApplyCSSChanges(TransformGroup transformGroup, TransformCollection children)
        {
            int childrenCount = (children != null) ? children.Count : 0;
            for (int i = childrenCount - 1; i > -1; i--)
            {
                children[i].INTERNAL_ApplyCSSChanges();
            }
        }

        private void UnapplyCSSChanges(TransformGroup transformGroup, TransformCollection children)
        {
            int childrenCount = (children != null) ? children.Count : 0;
            for (int i = childrenCount - 1; i > -1; i--)
            {
                children[i].INTERNAL_UnapplyCSSChanges();
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
#endif

        internal void ApplyTransformGroup(TransformCollection children)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                foreach (Transform child in children)
                {
                    if (child.INTERNAL_parent != this.INTERNAL_parent)
                    {
                        child.INTERNAL_parent = this.INTERNAL_parent;
                    }
                }
#if WORKINPROGRESS
                ApplyCSSChanges(this, children);
#endif
            }
        }

        internal void UnapplyTransformGroup(TransformCollection children)
        {
            if (this.INTERNAL_parent != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this.INTERNAL_parent))
            {
                foreach (Transform child in children)
                {
                    if (child.INTERNAL_parent != this.INTERNAL_parent)
                    {
                        child.INTERNAL_parent = this.INTERNAL_parent;
                    }
                }
#if WORKINPROGRESS
                UnapplyCSSChanges(this, children);
#endif
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
    }
#endif
}
