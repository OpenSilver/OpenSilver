
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace CSHTML5.Internal
{
    internal class INTERNAL_PropertyStorage
    {
        public INTERNAL_PropertyStorage(DependencyObject owner, DependencyProperty property)
        {
            Owner = owner;
            Property = property;
            if (property == FrameworkElement.IsEnabledProperty || property == FrameworkElement.IsHitTestVisibleProperty)
            {
                _isIsEnabledOrIsHitTestVisibleProperty = true;
            }
            CoercedValue = INTERNAL_NoValue.NoValue;
            VisualStateValue = INTERNAL_NoValue.NoValue;
            Local = INTERNAL_NoValue.NoValue;
            LocalStyleValue = INTERNAL_NoValue.NoValue;
            ImplicitStyleValue = INTERNAL_NoValue.NoValue;
            InheritedValue = INTERNAL_NoValue.NoValue;
        }
        internal bool _isIsEnabledOrIsHitTestVisibleProperty = false;

        public DependencyObject Owner { get; private set; }
        public DependencyProperty Property { get; private set; }
        public object CoercedValue { get; set; }
        public object VisualStateValue { get; set; }
        public object Local { get; set; }
        public object LocalStyleValue { get; set; }
        public object ImplicitStyleValue { get; set; }
        public object InheritedValue { get; set; }

        public List<IPropertyChangedListener> PropertyListeners { get; set; }
    }
}
