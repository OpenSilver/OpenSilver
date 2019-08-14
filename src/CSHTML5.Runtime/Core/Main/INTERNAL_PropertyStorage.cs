
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
        public INTERNAL_PropertyStorage(DependencyObject owner, DependencyProperty property, PropertyMetadata typeMetadata)
        {
            //_defaultValue = INTERNAL_NoValue.NoValue;
            Owner = owner;
            Property = property;
            TypeMetadata = typeMetadata;
            if (property == FrameworkElement.IsEnabledProperty || property == FrameworkElement.IsHitTestVisibleProperty)
            {
                _isIsEnabledOrIsHitTestVisibleProperty = true;
            }
            CoercedValue = INTERNAL_NoValue.NoValue;
            VisualStateValue = INTERNAL_NoValue.NoValue;
            ActiveLocalValue = new INTERNAL_LocalValue();
            LocalStyleValue = INTERNAL_NoValue.NoValue;
            ImplicitStyleValue = INTERNAL_NoValue.NoValue;
            InheritedValue = INTERNAL_NoValue.NoValue;

            // The default value is used initially for the Actual Value:
            ActualValue = typeMetadata != null ? typeMetadata.DefaultValue : null;
        }
        internal bool _isIsEnabledOrIsHitTestVisibleProperty = false;

        public DependencyObject Owner { get; private set; }
        public DependencyProperty Property { get; private set; }

        public PropertyMetadata TypeMetadata { get; set; }

        //=================
        // ACTUAL VALUE
        //=================

        /// <summary>
        /// Gets or sets the computed value that has the highest priority among the style, animation, visual state, inherited, etc. values.
        /// </summary>
        private object _actualValue;
        public object ActualValue
        {
            get
            {
                if (ActualValueIsDirty)
                {
                    _actualValue = INTERNAL_PropertyStore.ComputeActualValue(this, TypeMetadata, false);
                    ActualValueIsDirty = false;
                }

                return _actualValue;
            }
            set { _actualValue = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the ActualValue needs to be recomputer because it is no longer correct.
        /// </summary>
        public bool ActualValueIsDirty { private get; set; }

        //=================
        // SPECIFIC VALUES
        //=================
        public object CoercedValue { get; set; }
        public object VisualStateValue { get; set; }

        public object Local
        {
            get { return ActiveLocalValue.Local; }
            set { ActiveLocalValue.SetValue(value, KindOfValue.Local); }
        }
        public object AnimationValue
        {
            get { return ActiveLocalValue.Animated; }
            set { ActiveLocalValue.SetValue(value, KindOfValue.Animated); }
        }
        internal INTERNAL_LocalValue ActiveLocalValue { get; set; }
        public object LocalStyleValue { get; set; }
        public object ImplicitStyleValue { get; set; }
        public object InheritedValue { get; set; }

        //private object _defaultValue;
        //internal object DefaultValue
        //{
        //    get
        //    {
        //        EnsureDefaultValue();
        //        return _defaultValue;
        //    }
        //}

        //private void EnsureDefaultValue()
        //{
        //    if (_defaultValue == INTERNAL_NoValue.NoValue)
        //    {
        //        PropertyMetadata typeMetadata = Property.GetTypeMetaData(Property.OwnerType);
        //        if (typeMetadata != null)
        //        {
        //            _defaultValue = typeMetadata.DefaultValue;
        //        }
        //        else
        //        {
        //            _defaultValue = Property.CreateDefaultValue();
        //        }
        //    }
        //}

        public List<IPropertyChangedListener> PropertyListeners { get; set; }

        internal class INTERNAL_LocalValue
        {
            internal INTERNAL_LocalValue()
            {
                Local = INTERNAL_NoValue.NoValue;
                Animated = INTERNAL_NoValue.NoValue;
                ActiveValue = KindOfValue.Local;
            }

            internal object Local { get; set; }

            internal object Animated { get; set; }

            internal KindOfValue ActiveValue { get; set; }

            internal void SetValue(object value, KindOfValue kindOfValue)
            {
                if (kindOfValue == KindOfValue.Local)
                {
                    Local = value;
                    if (value == INTERNAL_NoValue.NoValue)
                    {
                        ActiveValue = Animated == INTERNAL_NoValue.NoValue ? KindOfValue.Local : KindOfValue.Animated;
                    }
                    else
                    {
                        ActiveValue = KindOfValue.Local;
                    }
                }
                else if (kindOfValue == KindOfValue.Animated)
                {
                    Animated = value;
                    if (value == INTERNAL_NoValue.NoValue)
                    {
                        ActiveValue = KindOfValue.Local;
                    }
                    else
                    {
                        ActiveValue = KindOfValue.Animated;
                    }
                }
            }

            internal object GetActiveLocalValue()
            {
                if (ActiveValue == KindOfValue.Local)
                {
                    return Local;
                }
                else if (ActiveValue == KindOfValue.Animated)
                {
                    return Animated;
                }
                else
                {
                    return INTERNAL_NoValue.NoValue;
                }
            }
        }
    }
}
