
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
        private Dictionary<DependencyProperty, Expression> _expressions;
        internal Expression GetExpression(DependencyProperty dp)
        {
            if(_expressions.ContainsKey(dp))
            {
                return _expressions[dp];
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
//            var t = Performance.now();
//#endif
            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty);
            //return dependencyProperty.Store.GetValue(storage);

            var tmp = INTERNAL_PropertyStore.GetValue(storage);
//#if PERFSTAT
//            Performance.Counter("DependencyObject.GetValue [" + dependencyProperty.Name + "]", t);
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
            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty, createAndSaveNewStorageIfNotExists: true);
#if PERFSTAT
            Performance.Counter("DependencyObject.SetLocalValue", t);
#endif
            INTERNAL_PropertyStore.SetLocalValue(storage, value);
        }

        public void CoerceCurrentValue(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty, createAndSaveNewStorageIfNotExists: true);
            INTERNAL_PropertyStore.CoerceCurrentValue(storage, propertyMetadata);
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
            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty);
            //return dependencyProperty.Store.GetValue(storage);
            return storage.Local;
        }

        public object GetVisualStateValue(DependencyProperty dependencyProperty) //todo: see if this is actually useful (to get specifically the VisualStateValue) and if so, change the GetValue into a GetVisualStateValue at the "return" line.
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty, createAndSaveNewStorageIfNotExists: true);
            return INTERNAL_PropertyStore.GetValue(storage);
        }

        public void SetVisualStateValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            var storage = INTERNAL_PropertyStore.GetStorage(this, dependencyProperty, createAndSaveNewStorageIfNotExists: true);
            INTERNAL_PropertyStore.SetVisualStateValue(storage, value);
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
                if (dependencyProperty == null)
                    throw new ArgumentNullException("No property specified");

                SetValueInternal(dependencyProperty, value);
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
            var storage = INTERNAL_PropertyStore.GetInheritedPropertyStorage(this, dependencyProperty, createAndSaveNewStorageIfNotExists: true);
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
            if (_expressions != null && _expressions.ContainsKey(dependencyProperty))
            {
                var value = _expressions[dependencyProperty];
                if (value is BindingExpression)
                {
                    return (((BindingExpression)value).ParentBinding).Clone();
                }
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
#if PERFSTAT
            var t = Performance.now();
#endif
            BindingExpression bindingExpression = new BindingExpression(binding, this, dependencyProperty);
            SetValueInternal(dependencyProperty, bindingExpression);
#if PERFSTAT
            Performance.Counter("DependencyObject.SetBinding", t);
#endif
            return bindingExpression;
        }

        void SetValueInternal(DependencyProperty dependencyProperty, object value)
        {
            object computedValue = value;
            Expression newExpression = null;
            Expression oldExpression = null;
            BindingExpression oldExpressionAsBindingExpression = null;
#if PERFSTAT
            var t = Performance.now();
#endif

            if (value is Expression)
            {
                newExpression = (Expression)value;
            }

            if (_expressions != null && _expressions.ContainsKey(dependencyProperty))
            {
                oldExpression = _expressions[dependencyProperty];
            }

            if (newExpression != null)
            {
                if (newExpression != oldExpression)
                {
                    if (newExpression.IsAttached)
                    {
                        throw new InvalidOperationException("Cannot attach an instance of Windows.UI.Xaml.Data.Expression multiple times");
                    }
                    else
                    {
                        if (oldExpression != null)
                        {
                            _expressions.Remove(dependencyProperty);
                            oldExpression.OnDetached(this);
                        }
                        if (_expressions == null)
                        {
                            _expressions = new Dictionary<DependencyProperty, Expression>();
                        }
                        _expressions.Add(dependencyProperty, newExpression);
                        newExpression.OnAttached(this);
                    }
                }
                //else (if newExpression == oldExpression) do nothing

                computedValue = newExpression.GetValue(dependencyProperty, this.GetType());
            }
            else if (oldExpression != null)
            {
                if (!oldExpression.IsUpdating
                    && !(oldExpression is BindingExpression && ((BindingExpression)oldExpression).ParentBinding.Mode == BindingMode.TwoWay)) // If mode is TwoWay, setting the property should not remove the binding. To reproduce: create a TextBox with a TwoWay binding on the property Text, and set textBox.Text = ... => the binding is preserved.
                {
                    oldExpressionAsBindingExpression = oldExpression as BindingExpression;
                    _expressions.Remove(dependencyProperty);
                    oldExpression.OnDetached(this);
                }
                else if (oldExpression is BindingExpression && ((BindingExpression)oldExpression).ParentBinding.Mode == BindingMode.OneTime) //todo: if we add the BindingExpressionBase class, change this with BindingExpressionBase.
                {
                    _expressions.Remove(dependencyProperty);
                    oldExpression.OnDetached(this);
                }
            }

            if (dependencyProperty.PropertyType == typeof(string) && computedValue != null)
            {
                computedValue = computedValue.ToString();
            }

            //If we use validation, we determine whether the value is Invalid or not.
            if (newExpression is BindingExpression)
            {
                BindingExpression bindingExpression = newExpression as BindingExpression;
                if (bindingExpression.INTERNAL_ForceValidateOnNextSetValue)
                {
                    bindingExpression.CheckInitialValueValidity(computedValue);
                }
            }

#if PERFSTAT
            Performance.Counter("DependencyObject.SetValueInternal", t);
#endif
            SetLocalValue(dependencyProperty, computedValue);
#if PERFSTAT
            t = Performance.now();
#endif

            if (oldExpressionAsBindingExpression != null && oldExpressionAsBindingExpression.ParentBinding.Mode == BindingMode.TwoWay) //note: we know that oldExpressionAsBindingExpression.IsUpdating is false because oldExpressionAsBindingExpression is only set in that case (otherwise, it is null).
            {
                oldExpressionAsBindingExpression.TryUpdateSourceObject(computedValue);
            }
        }

        internal void INTERNAL_UpdateBindingsSource()
        {
            if (_expressions != null)
            {
                foreach (Expression expression in
#if BRIDGE
                    INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_expressions)
#else
                    _expressions.Values
#endif
                    )
                {
                    BindingExpression bindingExpression = expression as BindingExpression;
                    if (bindingExpression != null)
                    {
                        bindingExpression.OnSourceAvailable();
                    }
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
