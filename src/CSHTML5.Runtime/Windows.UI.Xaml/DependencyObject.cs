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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents an object that participates in the dependency property system. DependencyObject
    /// is the immediate base class of many important UI-related classes, such as
    /// UIElement, Geometry, FrameworkTemplate, Style, and ResourceDictionary.
    /// </summary>
    public class DependencyObject
    {
#if REVAMPPOINTEREVENTS
        internal bool INTERNAL_ArePointerEventsEnabled
        {
            get
            {
                return INTERNAL_ManagePointerEventsAvailability();
            }
        }

        internal virtual bool INTERNAL_ManagePointerEventsAvailability()
        {
            return false;
        }
#endif

        private Dictionary<DependencyProperty, BindingExpression> _bindingExpressions;

        /// <summary>
        /// Returns the System.Windows.Data.BindingExpression that represents the binding
        /// on the specified property.
        /// </summary>
        /// <param name="dp">The target System.Windows.DependencyProperty to get the binding from.</param>
        /// <returns>
        /// A System.Windows.Data.BindingExpression if the target property has an active
        /// binding; otherwise, returns null.
        /// </returns>
        public BindingExpression GetBindingExpression(DependencyProperty dp)
        {
            if (_bindingExpressions == null)
            {
                _bindingExpressions = new Dictionary<DependencyProperty, BindingExpression>();
                return null;
            }

            if (_bindingExpressions.ContainsKey(dp))
            {
                return _bindingExpressions[dp];
            }
            else
            {
                return null;
            }
        }
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_PropertyStorageDictionary; // Contains all the properties that are either not in INTERNAL_AllInheritedProperties or in INTERNAL_UsefulInheritedProperties
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_AllInheritedProperties; // Here so that when we attach a child, the child gets all the properties that are in there (this allows the inherited properties to go all the way down even for properties that are not contained in the children)
        //internal List<DependencyProperty> INTERNAL_UsefulInheritedProperties; // this serves to know when we need to call "PropertyChanged" on this instance of DependencyObject when a property changes (These are filled when we call Register())

        internal List<DependencyProperty> INTERNAL_PropertiesForWhichToCallPropertyChangedWhenLoadedIntoVisualTree; // When a UI element is added to the Visual Tree, we call "PropertyChanged" on all its set properties so that the control can refresh itself. However, when a property is not set, we don't call PropertyChanged. Unless the property is listed here.

        /// <summary>
        /// Returns the current effective value of a dependency property from a DependencyObject.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The DependencyProperty identifier of the property for which to retrieve the
        /// value.
        /// </param>
        /// <returns>Returns the current effective value.</returns>
        public object GetValue(DependencyProperty dependencyProperty)
        {
//#if PERFSTAT
//          var t = Performance.now();
//#endif
            var storage = INTERNAL_PropertyStore.GetStorageIfExists(this, dependencyProperty);
            PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(this.GetType());
            var tmp = INTERNAL_PropertyStore.GetValue(storage, typeMetadata);
//#if PERFSTAT
//          Performance.Counter("DependencyObject.GetValue [" + dependencyProperty.Name + "]", t);
//#endif
            return tmp;
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject while not overriding a hypothetical Binding (example: when the user writes in a TextBox with a two way Binding on its Text property).
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        public void SetLocalValue(DependencyProperty dependencyProperty, object value)
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            var storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(this, dependencyProperty);
#if PERFSTAT
            Performance.Counter("DependencyObject.SetLocalValue", t);
#endif
            INTERNAL_PropertyStore.SetSpecificValue(storage, KindOfValue.Local, value);
        }

        public void CoerceCurrentValue(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            var storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(this, dependencyProperty);
            INTERNAL_PropertyStore.CoerceCurrentValue(storage);
        }

        /// <summary>
        /// Returns the local value of a dependency property, if a local value is set.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The DependencyProperty identifier of the property for which to retrieve the
        /// local value.
        /// </param>
        /// <returns>
        /// Returns the local value, or returns the sentinel value UnsetValue if no local
        /// value is set.
        /// </returns>
        public object ReadLocalValue(DependencyProperty dependencyProperty)
        {
            var storage = INTERNAL_PropertyStore.GetStorageIfExists(this, dependencyProperty);

            if (storage != null)
                return storage.Local;
            else
                return INTERNAL_NoValue.NoValue;
        }

        public object GetVisualStateValue(DependencyProperty dependencyProperty) //todo: see if this is actually useful (to get specifically the VisualStateValue) and if so, change the GetValue into a GetVisualStateValue at the "return" line.
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorageIfExists(this, dependencyProperty);
            PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(this.GetType());
            return INTERNAL_PropertyStore.GetValue(storage, typeMetadata);
        }

        public void SetVisualStateValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(this, dependencyProperty);
            INTERNAL_PropertyStore.SetSpecificValue(storage, KindOfValue.VisualState, value);
        }

        public void SetAnimationValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorageOrCreateNewIfNotExists(this, dependencyProperty);
            INTERNAL_PropertyStore.SetSpecificValue(storage, KindOfValue.Animated, value);
        }

        public object GetAnimationValue(DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorageIfExists(this, dependencyProperty);
            PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(this.GetType());
            return INTERNAL_PropertyStore.GetValue(storage, typeMetadata);
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
            // Verify the arguments:
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            // Convert the value to String if the property is of type String:
            object computedValue = value;
            if (dependencyProperty.PropertyType == typeof(string) && computedValue != null)
            {
                computedValue = computedValue.ToString();
            }

            // Get the previous BindingExpression in case the previous value of a property was a Binding, and detach the Binding:
            BindingExpression oldBindingExpression = null;
            if (_bindingExpressions != null
                && _bindingExpressions.ContainsKey(dependencyProperty))
            {
                oldBindingExpression = _bindingExpressions[dependencyProperty];

                if (!oldBindingExpression.IsUpdating && oldBindingExpression.ParentBinding.Mode != BindingMode.TwoWay) // If mode is TwoWay, setting the property should not remove the binding. To reproduce: create a TextBox with a TwoWay binding on the property Text, and set textBox.Text = ... => the binding is preserved.
                {
                    _bindingExpressions.Remove(dependencyProperty);
                    oldBindingExpression.OnDetached(this);
                }
                else if (oldBindingExpression.ParentBinding.Mode == BindingMode.OneTime)
                {
                    _bindingExpressions.Remove(dependencyProperty);
                    oldBindingExpression.OnDetached(this);
                }
            }

            // Set the value and raise the PropertyChanged event if necessary:
            SetLocalValue(dependencyProperty, computedValue);

            // Update the source of the Binding, in case the previous value of a property was a Binding and the Mode was "TwoWay":
            if (oldBindingExpression != null
                && oldBindingExpression.ParentBinding.Mode == BindingMode.TwoWay) //note: we know that oldBindingExpression.IsUpdating is false because oldBindingExpression is only set in that case (otherwise, it is null).
            {
                oldBindingExpression.TryUpdateSourceObject(computedValue);
            }
        }

        /// <summary>
        /// Sets the inherited value of a dependency property on a DependencyObject. Do not use this method.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        /// <param name="recursively">Specifies if the inherited value must be applied to the children of this DependencyObject.</param>
        public void SetInheritedValue(DependencyProperty dependencyProperty, object value, bool recursively)
        {
            //-----------------------
            // CALL "SET INHERITED VALUE" ON THE STORAGE:
            //-----------------------
            var storage = INTERNAL_PropertyStore.GetInheritedPropertyStorageOrCreateNewIfNotFound(this, dependencyProperty);
            INTERNAL_PropertyStore.SetInheritedValue(storage, value, recursively);
        }

        /// <summary>
        /// Refreshes the value of the given DependencyProperty on this DependencyObject so that it fits the coercion that should be applied on it.
        /// </summary>
        /// <param name="dependencyProperty">The dependencyProperty whose value we want to refresh.</param>
        public void Coerce(DependencyProperty dependencyProperty)
        {
            PropertyMetadata metadata = dependencyProperty.GetTypeMetaData(this.GetType());
            if (metadata != null && metadata.CoerceValueCallback != null) //Performance improvement: There is no point to setting a property to its own value if there is no coercion on it.
            {
                CoerceCurrentValue(dependencyProperty, metadata); //note: this will call the coerce callback method so no need to do it beforehand.
            }
        }

        internal void ResetInheritedProperties()
        {
            if (INTERNAL_AllInheritedProperties != null)
            {
                foreach (DependencyProperty property in INTERNAL_AllInheritedProperties.Keys)
                {
                    if (INTERNAL_PropertyStorageDictionary.ContainsKey(property))
                    {
                        INTERNAL_PropertyStore.ResetInheritedValue(INTERNAL_PropertyStorageDictionary[property]);
                    }
                }
                INTERNAL_AllInheritedProperties.Clear();
            }
        }

        /// <summary>
        /// Gets the CoreDispatcher that this object is associated with.
        /// </summary>
#if MIGRATION
        public Dispatcher Dispatcher
#else
        public CoreDispatcher Dispatcher
#endif
        {
            get
            {
#if MIGRATION
                return Dispatcher.INTERNAL_GetCurrentDispatcher();
#else
                return CoreDispatcher.INTERNAL_GetCurrentDispatcher();
#endif
            }
        }


        #region Binding related elements

        internal Binding INTERNAL_GetBinding(DependencyProperty dependencyProperty)
        {
            if (_bindingExpressions != null && _bindingExpressions.ContainsKey(dependencyProperty))
            {
                var value = _bindingExpressions[dependencyProperty];
                return value.ParentBinding.Clone();
            }
            return null; //todo: see if an exception would be better
        }

        /// <summary>
        /// Attaches a binding to a FrameworkElement, using the provided binding object.
        /// </summary>
        /// <param name="dependencyProperty">The dependency property identifier of the property that is data bound.</param>
        /// <param name="binding">The binding to use for the property.</param>
        /// <returns>The BindingExpression created.</returns>
        public BindingExpression SetBinding(DependencyProperty dependencyProperty, Binding binding)
        {
            // Verify the arguments:
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");
            if (binding == null)
                throw new ArgumentNullException("No binding specified");

            // Create the BindingExpression from the Binding:
            BindingExpression newBindingExpression = new BindingExpression(binding, this, dependencyProperty);

            // Apply the BindingExpression:
            ApplyBindingExpression(dependencyProperty, newBindingExpression);

            // Return the newly created BindingExpression:
            return newBindingExpression;
        }

        internal void ApplyBindingExpression(DependencyProperty dependencyProperty, BindingExpression newBindingExpression)
        {
            // Get the previous BindingExpression in case we are replacing an existing Binding:
            BindingExpression oldBindingExpression = null;
            if (_bindingExpressions != null && _bindingExpressions.ContainsKey(dependencyProperty))
            {
                oldBindingExpression = _bindingExpressions[dependencyProperty];
            }

            // Detach the previous BindingExpression, in case there was any, and remember the new BindingExpression:
            if (newBindingExpression != oldBindingExpression)
            {
                if (newBindingExpression.IsAttached)
                {
                    throw new InvalidOperationException("Cannot attach an instance of Windows.UI.Xaml.Data.BindingExpression multiple times");
                }
                else
                {
                    if (oldBindingExpression != null)
                    {
                        _bindingExpressions.Remove(dependencyProperty);
                        oldBindingExpression.OnDetached(this);
                    }
                    if (_bindingExpressions == null)
                    {
                        _bindingExpressions = new Dictionary<DependencyProperty, BindingExpression>();
                    }
                    _bindingExpressions.Add(dependencyProperty, newBindingExpression);
                    newBindingExpression.OnAttached(this);
                }
            }

            // Get the actual value using the property path specified in the Binding:
            object computedValue = newBindingExpression.GetValue(dependencyProperty, this.GetType());

            // Convert the value to String if the property is of type String:
            if (dependencyProperty.PropertyType == typeof(string) && computedValue != null)
            {
                computedValue = computedValue.ToString();
            }

            // If validation is used, determine whether the value is valid or not:
            if (newBindingExpression.INTERNAL_ForceValidateOnNextSetValue)
            {
                newBindingExpression.CheckInitialValueValidity(computedValue);
            }

            // Set the value and raise the PropertyChanged event if necessary:
            SetLocalValue(dependencyProperty, computedValue);

            // Update the source of the Binding, in case the previous value of a property was a Binding and the Mode was "TwoWay":
            if (oldBindingExpression != null && oldBindingExpression.ParentBinding.Mode == BindingMode.TwoWay) //note: we know that oldBindingExpression.IsUpdating is false because oldBindingExpression is only set in that case (otherwise, it is null).
            {
                oldBindingExpression.TryUpdateSourceObject(computedValue);
            }
        }

        internal void INTERNAL_UpdateBindingsSource()
        {
            if (_bindingExpressions != null)
            {
                foreach (BindingExpression bindingExpression in
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_bindingExpressions)
#else
                    _bindingExpressions.Values
#endif
                    )
                {
                    bindingExpression.OnSourceAvailable();
                }
            }
        }

        /// <exclude/>
        internal protected virtual void INTERNAL_OnAttachedToVisualTree()
        {
        }

        /// <exclude/>
        internal protected virtual void INTERNAL_OnDetachedFromVisualTree()
        {
            // This is particularly useful for elements to clear any references they have to DOM elements. For example, the Grid will use it to set its _tableDiv to null.
        }

        #endregion
    }
}
