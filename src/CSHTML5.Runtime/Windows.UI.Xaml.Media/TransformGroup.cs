﻿
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
            DependencyProperty.Register("Children", typeof(TransformCollection), typeof(TransformGroup), new PropertyMetadata(null));

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
