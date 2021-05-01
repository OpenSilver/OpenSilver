

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Threading;
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
    public partial class DependencyObject
    {
        #region Inheritance Context

        private DependencyObject _inheritanceContext;
        private FrameworkElement _inheritedParent;
        private readonly HashSet<DependencyObject> _contextListeners = new HashSet<DependencyObject>();
        internal event EventHandler InheritedContextChanged;

        internal FrameworkElement InheritedParent
        {
            get { return this._inheritedParent; }
        }

        internal DependencyObject InheritanceContext
        {
            get { return this._inheritanceContext; }
        }

        internal bool CanBeInheritanceContext { get; set; }

        internal bool IsInheritanceContextSealed { get; set; }

        internal bool ShouldProvideInheritanceContext(DependencyObject target, DependencyProperty property)
        {
            // We never provide an inherited context for a FrameworkElement because the DataContext takes
            // priority over inherited context.
            return !(target is FrameworkElement);
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool RemoveSelfAsInheritanceContext(object value, DependencyProperty dp)
        {
            DependencyObject doValue = value as DependencyObject;
            if (doValue != null)
            {
                return RemoveSelfAsInheritanceContext(doValue, dp);
            }
            else
            {
                return false;
            }
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool RemoveSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (doValue != null
                && this.ShouldProvideInheritanceContext(doValue, dp)
                && this.CanBeInheritanceContext
                && !doValue.IsInheritanceContextSealed)
            {
                DependencyObject oldInheritanceContext = doValue.InheritanceContext;
                if (this == oldInheritanceContext)
                {
                    doValue.RemoveInheritanceContext(this, dp);

                    // Context changed
                    return true;
                }
                else
                {
                    // this object is not the inherited context for doValue
                    // Context did not change
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            // Stop listening for context changes
            context.StopListeningToInheritanceContextChanges(this);

            // Reset inheritance context
            this._inheritanceContext = null;

            // Reset inherited parent
            this._inheritedParent = null;

            // Notify listeners that inheritance context changed
            this.OnInheritedContextChanged(null);
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool ProvideSelfAsInheritanceContext(object value, DependencyProperty dp)
        {
            DependencyObject doValue = value as DependencyObject;
            if (doValue != null)
            {
                return ProvideSelfAsInheritanceContext(doValue, dp);
            }
            else
            {
                return false;
            }
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool ProvideSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (doValue != null
                && this.ShouldProvideInheritanceContext(doValue, dp)
                && this.CanBeInheritanceContext
                && !doValue.IsInheritanceContextSealed)

            {
                if (doValue.InheritanceContext != null)
                {
                    // In silverlight, there is only one inherited context for a given DependencyObject.
                    // We can only set an inherited context if there is no inherited context for the
                    // DependencyObject.
                    return false;
                }
                else
                {
                    doValue.AddInheritanceContext(this, dp);

                    // Context changed
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        private void AddInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            // Start listening for context changes
            context.ListenToInheritanceContextChanges(this);

            // Set the new context
            this._inheritanceContext = context;

            // Find the new inherited parent
            this._inheritedParent = this.ComputeInheritedParent();

            // Notify listeners that inheritance context changed
            this.OnInheritedContextChanged(this._inheritedParent);
        }

        private void OnInheritedContextChanged(FrameworkElement newParent)
        {
            if (this.InheritedContextChanged != null)
            {
                this.InheritedContextChanged(this, EventArgs.Empty);
            }
            foreach (DependencyObject listener in this._contextListeners)
            {
                listener._inheritedParent = newParent;
                listener.OnInheritedContextChanged(newParent);
            }
        }

        private FrameworkElement ComputeInheritedParent()
        {
            FrameworkElement inheritedParent;
            for (DependencyObject contextDO = this._inheritanceContext; contextDO != null; contextDO = contextDO._inheritanceContext)
            {
                if ((inheritedParent = contextDO as FrameworkElement) != null)
                {
                    return inheritedParent;
                }
            }
            return null;
        }

        private void ListenToInheritanceContextChanges(DependencyObject listener)
        {
            this._contextListeners.Add(listener);
        }

        private void StopListeningToInheritanceContextChanges(DependencyObject listener)
        {
            bool isListening = this._contextListeners.Contains(listener);
            if (isListening)
            {
                this._contextListeners.Remove(listener);
            }
        }

        #endregion

        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_PropertyStorageDictionary { get; } // Contains all the properties that are either not in INTERNAL_AllInheritedProperties or in INTERNAL_UsefulInheritedProperties
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_AllInheritedProperties { get; } // Here so that when we attach a child, the child gets all the properties that are in there (this allows the inherited properties to go all the way down even for properties that are not contained in the children)
        internal List<DependencyProperty> INTERNAL_PropertiesForWhichToCallPropertyChangedWhenLoadedIntoVisualTree; // When a UI element is added to the Visual Tree, we call "PropertyChanged" on all its set properties so that the control can refresh itself. However, when a property is not set, we don't call PropertyChanged. Unless the property is listed here.


        #region Constructor
        public DependencyObject()
        {
            this.CanBeInheritanceContext = true;
            this.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();
            this.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();
        }
        #endregion


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
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return INTERNAL_PropertyStore.GetEffectiveValue(storage);
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject while not overriding a hypothetical Binding (example: when the user writes in a TextBox with a two way Binding on its Text property).
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
#if WORKINPROGRESS
        [Obsolete("Use SetCurrentValue")]
#endif
        public void SetLocalValue(DependencyProperty dependencyProperty, object value)
        {
            this.SetCurrentValue(dependencyProperty, value);
        }

        public void SetCurrentValue(DependencyProperty dp, object value)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetValueCommon(storage, value, true);
        }

        [Obsolete("Use CoerceValue")]
        public void CoerceCurrentValue(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.CoerceValueCommon(storage);
        }

        /// <summary>
        /// Returns the local value of a dependency property, if a local value is set.
        /// </summary>
        /// <param name="dp">
        /// The DependencyProperty identifier of the property for which to retrieve the
        /// local value.
        /// </param>
        /// <returns>
        /// Returns the local value, or returns the sentinel value UnsetValue if no local
        /// value is set.
        /// </returns>
        public object ReadLocalValue(DependencyProperty dp)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, false/*don't create*/, out storage))
            {
                // In silverlight ReadLocalValue returns a BindingExpression if the value
                // is a BindingExpression set from a style's setter and the "real" local
                // value in unset. (This is not the case in WPF)
                if (storage.LocalValue != DependencyProperty.UnsetValue)
                {
                    return storage.LocalValue;
                }
                else if (storage.LocalStyleValue != DependencyProperty.UnsetValue)
                {
                    BindingExpression be = storage.LocalStyleValue as BindingExpression;
                    if (be != null)
                    {
                        return be;
                    }
                }
                else
                {
                    BindingExpression be = storage.ThemeStyleValue as BindingExpression;
                    if (be != null)
                    {
                        return be;
                    }
                }
                return storage.LocalValue;
            }
            return DependencyProperty.UnsetValue;
        }

        // This method is here to workaround the fact that in Silverlight, ReadLocalValue()
        // can return a value set from a style if it is a BindingExpression, while in WPF
        // ReadLocalValue() will only return the local value
        internal object ReadLocalValueInternal(DependencyProperty dp)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, false/*don't create*/, out storage))
            {
                return storage.LocalValue;
            }
            return DependencyProperty.UnsetValue;
        }

        public object GetVisualStateValue(DependencyProperty dependencyProperty) //todo: see if this is actually useful (to get specifically the VisualStateValue) and if so, change the GetValue into a GetVisualStateValue at the "return" line.
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return storage.AnimatedValue;
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;
        }

        public void SetVisualStateValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetAnimationValue(storage, value);
        }

        public void SetAnimationValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetAnimationValue(storage, value);
        }

        public object GetAnimationValue(DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, false/*don't create*/, out storage))
            {
                return storage.AnimatedValue;
            }
            return dependencyProperty.GetTypeMetaData(this.GetType()).DefaultValue;
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            // Verify the arguments:
            if (dp == null)
                throw new ArgumentNullException("No property specified");

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.SetValueCommon(storage, value, false);
        }

        internal void SetLocalStyleValue(DependencyProperty dp, object value)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, value != DependencyProperty.UnsetValue/*create*/, out storage))
            {
                INTERNAL_PropertyStore.SetLocalStyleValue(storage, value);
            }
        }

        internal void SetImplicitReferenceValue(DependencyProperty dp, object value)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, value != DependencyProperty.UnsetValue/*create*/, out storage))
            {
                INTERNAL_PropertyStore.SetImplicitReferenceValue(storage, value);
            }
        }

        internal void SetThemeStyleValue(DependencyProperty dp, object value)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, value != DependencyProperty.UnsetValue/*create*/, out storage))
            {
                INTERNAL_PropertyStore.SetThemeStyleValue(storage, value);
            }
        }

        /// <summary>
        /// Sets the inherited value of a dependency property on a DependencyObject. Do not use this method.
        /// </summary>
        /// <param name="dp">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        /// <param name="recursively">Specifies if the inherited value must be applied to the children of this DependencyObject.</param>
        /// <returns>true if this property's value changed, false otherwise.</returns>
        internal bool SetInheritedValue(DependencyProperty dp, object value, bool recursively)
        {
            //-----------------------
            // CALL "SET INHERITED VALUE" ON THE STORAGE:
            //-----------------------
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(this, dp, true/*create*/, out storage);
            return INTERNAL_PropertyStore.SetInheritedValue(storage, value, recursively);
        }

        /// <summary>
        /// Refreshes the value of the given DependencyProperty on this DependencyObject so that it fits the coercion that should be applied on it.
        /// </summary>
        /// <param name="dependencyProperty">The dependencyProperty whose value we want to refresh.</param>
        [Obsolete("Use CoerceValue")]
        public void Coerce(DependencyProperty dependencyProperty)
        {
            this.CoerceValue(dependencyProperty);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("No property specified.");
            }

            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.CoerceValueCommon(storage);
        }

        internal void ResetInheritedProperties()
        {
            // Copy of the keys to allow removing items from the Dictionary furing the foreach.
            INTERNAL_PropertyStorage[] storages = this.INTERNAL_AllInheritedProperties.Values.ToArray();
            foreach (INTERNAL_PropertyStorage storage in storages)
            {
                INTERNAL_PropertyStore.SetInheritedValue(storage, 
                                                         DependencyProperty.UnsetValue,
                                                         false); // recursively
                if (storage.BaseValueSourceInternal == BaseValueSourceInternal.Default &&
                    (storage.PropertyListeners == null || storage.PropertyListeners.Count == 0)) //this second test is to make sure we keep any listener working (for example Bindings would stop working if we remove an element from the Visual tree then add it back))
                {
                    // Remove storage if the effective value is the default value.
                    this.INTERNAL_AllInheritedProperties.Remove(storage.Property);
                    this.INTERNAL_PropertyStorageDictionary.Remove(storage.Property);
                }
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

        internal void ApplyExpression(DependencyProperty dp, Expression expression, bool isInStyle)
        {
            INTERNAL_PropertyStorage storage;
            INTERNAL_PropertyStore.TryGetStorage(this, dp, true/*create*/, out storage);
            INTERNAL_PropertyStore.RefreshExpressionCommon(storage, expression, isInStyle); // Set LocalStyle if Binding is from style.
        }

        internal void INTERNAL_UpdateBindingsSource()
        {
            // Note: we make a copy to avoid any errors related to the collection being modified during the "foreach" below.
            INTERNAL_PropertyStorage[] copyOfCollection = this.INTERNAL_PropertyStorageDictionary.Select(kp => kp.Value).ToArray();
            foreach (INTERNAL_PropertyStorage storage in copyOfCollection)
            {
                if (storage.IsExpression)
                {
                    (storage.LocalValue as BindingExpression)?.OnSourceAvailable();
                }
                else if (storage.IsExpressionFromStyle)
                {
                    (storage.ModifiedValue.BaseValue as BindingExpression)?.OnSourceAvailable();
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

        /// <summary>
        /// Clears the local value of a property
        /// </summary>
        public void ClearValue(DependencyProperty dp)
        {
            INTERNAL_PropertyStorage storage;
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, false/*don't create*/, out storage))
            {
                INTERNAL_PropertyStore.ClearValueCommon(storage);
            }
        }

#if WORKINPROGRESS
        [OpenSilver.NotImplemented]
        public bool CheckAccess()
        {
            bool accessAllowed = true;

            var dispatcher = Dispatcher;

            if (dispatcher != null)
            {
                accessAllowed = dispatcher.CheckAccess();
            }

            return accessAllowed;
        }
#endif
        }
}
