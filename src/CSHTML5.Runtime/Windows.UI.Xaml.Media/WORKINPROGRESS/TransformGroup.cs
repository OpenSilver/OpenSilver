
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================



using System.Windows.Markup;
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
            DependencyProperty.Register("Children", typeof(TransformCollection), typeof(TransformGroup), new PropertyMetadata(new TransformCollection()));

        protected override Point INTERNAL_TransformPoint(Point point)
        {
            return new Point();
        }

        internal override void INTERNAL_ApplyTransform()
        {

        }

        internal override void INTERNAL_UnapplyTransform()
        {

        }
    }
#endif
}
