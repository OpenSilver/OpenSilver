
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
#if WORKINPROGRESS
            AnimationValue = INTERNAL_NoValue.NoValue;
#endif
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
#if WORKINPROGRESS
        public object AnimationValue { get; set; }
#endif
        public object LocalStyleValue { get; set; }
        public object ImplicitStyleValue { get; set; }
        public object InheritedValue { get; set; }

        public List<IPropertyChangedListener> PropertyListeners { get; set; }
    }
}
