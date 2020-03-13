

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

        public List<IPropertyChangedListener> PropertyListeners { get; set; }

        private bool DoesSpecificValueImpactActualValue(KindOfValue kind)
        {
            //Note: in KindOfValue, the value attributed to the enum values corresponds to their priority rank so the lower, the more priority.
            return ((kind <= KindOfValue.VisualState || this.VisualStateValue == INTERNAL_NoValue.NoValue) //means "kind has a higher priority tha VisualState or there is no VisualState value
                && (kind <= KindOfValue.Local || this.Local == INTERNAL_NoValue.NoValue || this.ActiveLocalValue.ActiveValue != KindOfValue.Local)
                && (kind <= KindOfValue.Animated || this.AnimationValue == INTERNAL_NoValue.NoValue || this.ActiveLocalValue.ActiveValue != KindOfValue.Animated)
                && (kind <= KindOfValue.LocalStyle || this.LocalStyleValue == INTERNAL_NoValue.NoValue)
                && (kind <= KindOfValue.ImplicitStyle || this.ImplicitStyleValue == INTERNAL_NoValue.NoValue)
                && (kind <= KindOfValue.Inherited || this.InheritedValue == INTERNAL_NoValue.NoValue));
        }

        private void SetValue(object value, KindOfValue kind)
        {
            switch (kind)
            {
                case KindOfValue.Coerced:
                    this.CoercedValue = value;
                    break;
                case KindOfValue.VisualState:
                    this.VisualStateValue = value;
                    break;
                case KindOfValue.Local:
                    this.Local = value;
                    break;
                case KindOfValue.Animated:
                    this.AnimationValue = value;
                    break;
                case KindOfValue.LocalStyle:
                    this.LocalStyleValue = value;
                    break;
                case KindOfValue.ImplicitStyle:
                    this.ImplicitStyleValue = value;
                    break;
                case KindOfValue.Inherited:
                    //inherited is a special case
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Changes the value of the storage for a given kind.
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="kind">kind of value to set</param>
        /// <param name="oldValue">value of previous actual value if it changed (null otherwise)</param>
        /// <returns>returns true if the actual value is impacted, false otherwise</returns>
        public bool SetValue(object value, KindOfValue kind, out object oldValue)
        {
            bool impactsActualValue = this.DoesSpecificValueImpactActualValue(kind);
            oldValue = impactsActualValue ? this.ActualValue : null;
            this.SetValue(value, kind);
            return impactsActualValue;
        }

        public bool SetInheritedValue(object value, out object oldValue)
        {
            bool impactsActualValue = this.DoesSpecificValueImpactActualValue(KindOfValue.Inherited);
            oldValue = impactsActualValue ? this.ActualValue : null;
            this.InheritedValue = value;
            return impactsActualValue;
        }

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
                switch (this.ActiveValue)
                {
                    case KindOfValue.Local:
                        return this.Local;
                    case KindOfValue.Animated:
                        return this.Animated;
                    default:
                        return INTERNAL_NoValue.NoValue;
                }
            }
        }
    }
}
