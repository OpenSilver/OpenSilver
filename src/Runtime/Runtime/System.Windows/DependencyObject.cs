
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
using System.Diagnostics;
using System.Linq;
using CSHTML5.Internal;
using OpenSilver.Internal.Data;
using OpenSilver.Internal;

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
    public partial class DependencyObject : IInternalDependencyObject
    {
        #region Inheritance Context

        private HashSet<DependencyObject> _contextListeners;

        internal event EventHandler InheritedContextChanged;        

        private ContextStorage _contextStorage;

        internal DependencyObject InheritanceContext
        {
            get => _contextStorage?.GetContext();
            set => (_contextStorage ??= new ContextStorage()).SetContext(value);
        }

        private sealed class ContextStorage
        {
            private object _context;
            private bool _useWeakRef;

            public DependencyObject GetContext()
            {
                if (_useWeakRef)
                {
                    var wr = (WeakReference<DependencyObject>)_context;
                    if (!wr.TryGetTarget(out DependencyObject context))
                    {
                        SetContext(null);
                    }

                    return context;
                }

                return (DependencyObject)_context;
            }

            public void SetContext(DependencyObject context)
            {
                _useWeakRef = context is FrameworkElement;
                _context = _useWeakRef ? new WeakReference<DependencyObject>(context) : (object)context;
            }
        }

        internal bool CanBeInheritanceContext { get; set; }

        internal bool IsInheritanceContextSealed { get; set; }

        internal virtual bool ShouldProvideInheritanceContext(DependencyObject target, DependencyProperty property)
        {
            // We never provide an inherited context for a FrameworkElement because the DataContext takes
            // priority over inherited context.
            return target is not FrameworkElement;
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
            this.InheritanceContext = null;

            // Notify listeners that inheritance context changed
            this.OnInheritedContextChanged(EventArgs.Empty);
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
            this.InheritanceContext = context;

            // Notify listeners that inheritance context changed
            this.OnInheritedContextChanged(EventArgs.Empty);
        }

        private void OnInheritedContextChanged(EventArgs args)
        {
            if (this.InheritedContextChanged != null)
            {
                this.InheritedContextChanged(this, args);
            }

            if (this._contextListeners != null)
            {
                foreach (DependencyObject listener in this._contextListeners)
                {
                    listener.OnInheritedContextChanged(args);
                }
            }

            // Let sub-classes do their own thing
            this.OnInheritanceContextChangedCore(args);
        }

        /// <summary>
        ///     This is a means for subclasses to get notification
        ///     of InheritanceContext changes and then they can do
        ///     their own thing.
        /// </summary>
        internal virtual void OnInheritanceContextChangedCore(EventArgs args)
        {
        }

        private void ListenToInheritanceContextChanges(DependencyObject listener)
        {
            if (this._contextListeners == null)
            {
                this._contextListeners = new HashSet<DependencyObject>();
            }

            this._contextListeners.Add(listener);
        }

        private void StopListeningToInheritanceContextChanges(DependencyObject listener)
        {
            if (this._contextListeners == null)
            {
                return;
            }

            bool isListening = this._contextListeners.Contains(listener);
            if (isListening)
            {
                this._contextListeners.Remove(listener);
            }
        }

        #endregion

        private Dictionary<DependencyProperty, DependentList> _dependentListMap;
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_PropertyStorageDictionary { get; } // Contains all the properties that are either not in INTERNAL_AllInheritedProperties or in INTERNAL_UsefulInheritedProperties
        internal Dictionary<DependencyProperty, INTERNAL_PropertyStorage> INTERNAL_AllInheritedProperties { get; } // Here so that when we attach a child, the child gets all the properties that are in there (this allows the inherited properties to go all the way down even for properties that are not contained in the children)

        private DependencyObjectType _dType;

        /// <summary>
        /// Returns the DType that represents the CLR type of this instance
        /// </summary>
        internal DependencyObjectType DependencyObjectType
            => _dType ??= DependencyObjectType.FromSystemTypeInternal(GetType());

        #region Constructor
        public DependencyObject()
        {
            CanBeInheritanceContext = true;
            INTERNAL_PropertyStorageDictionary = new(DependencyPropertyComparer.Default);
            INTERNAL_AllInheritedProperties = new(DependencyPropertyComparer.Default);
        }
        #endregion

        object IInternalDependencyObject.GetValue(DependencyProperty dp) => GetValue(dp);

        void IInternalDependencyObject.SetValue(DependencyProperty dp, object value) => SetValue(dp, value);

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
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            PropertyMetadata metadata = null;

            if (dependencyProperty.ReadOnly)
            {
                metadata = dependencyProperty.GetMetadata(DependencyObjectType);

                GetReadOnlyValueCallback getValueCallback = metadata.GetReadOnlyValueCallback;
                if (getValueCallback != null)
                {
                    return getValueCallback(this);
                }
            }

            if (INTERNAL_PropertyStore.TryGetStorage(this,
                dependencyProperty,
                metadata,
                false,
                out INTERNAL_PropertyStorage storage))
            {
                return INTERNAL_PropertyStore.GetEffectiveValue(storage.Entry);
            }

            if (!dependencyProperty.IsDefaultValueChanged &&
                !dependencyProperty.IsPotentiallyUsingDefaultValueFactory)
            {
                return dependencyProperty.DefaultMetadata.DefaultValue;
            }

            metadata ??= dependencyProperty.GetMetadata(DependencyObjectType);

            return metadata.GetDefaultValue(this, dependencyProperty);
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject while not overriding a hypothetical Binding (example: when the user writes in a TextBox with a two way Binding on its Text property).
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        [Obsolete(Helper.ObsoleteMemberMessage + " Use SetCurrentValue instead.")]
        public void SetLocalValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            SetCurrentValue(dependencyProperty, value);
        }

        public void SetCurrentValue(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = SetupPropertyChange(dp);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);
            
            INTERNAL_PropertyStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                true);
        }

        [Obsolete(Helper.ObsoleteMemberMessage + " Use CoerceValue instead.")]
        public void CoerceCurrentValue(DependencyProperty dependencyProperty, PropertyMetadata propertyMetadata)
        {
            CoerceValue(dependencyProperty);
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
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, null, false, out INTERNAL_PropertyStorage storage))
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
                    if (storage.LocalStyleValue is BindingExpression be)
                    {
                        return be;
                    }
                }
                else
                {
                    if (storage.ThemeStyleValue is BindingExpression be)
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
            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, null, false, out INTERNAL_PropertyStorage storage))
            {
                return storage.LocalValue;
            }

            return DependencyProperty.UnsetValue;
        }

        internal bool HasDefaultValue(DependencyProperty dp)
        {
            return !INTERNAL_PropertyStore.TryGetStorage(this, dp, null, false, out var storage) || 
                storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.Default;
        }

        public object GetVisualStateValue(DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            if (INTERNAL_PropertyStore.TryGetStorage(this, dependencyProperty, null, false, out INTERNAL_PropertyStorage storage))
            {
                return storage.AnimatedValue;
            }

            return dependencyProperty.GetDefaultValue(this);
        }

        public void SetVisualStateValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            PropertyMetadata metadata = SetupPropertyChange(dependencyProperty);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dependencyProperty,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);
            
            INTERNAL_PropertyStore.SetAnimationValue(storage,
                this,
                dependencyProperty,
                metadata,
                value);
        }

        public void SetAnimationValue(DependencyProperty dependencyProperty, object value)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            PropertyMetadata metadata = SetupPropertyChange(dependencyProperty);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dependencyProperty,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);

            INTERNAL_PropertyStore.SetAnimationValue(storage,
                this,
                dependencyProperty,
                metadata,
                value);
        }

        public object GetAnimationValue(DependencyProperty dependencyProperty)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException(nameof(dependencyProperty));
            }

            if (INTERNAL_PropertyStore.TryGetStorage(this,
                dependencyProperty,
                null,
                false,
                out INTERNAL_PropertyStorage storage))
            {
                return storage.AnimatedValue;
            }

            return dependencyProperty.GetDefaultValue(this);
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = SetupPropertyChange(dp);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);

            INTERNAL_PropertyStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                false);
        }

        internal void SetValue(DependencyPropertyKey key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);

            INTERNAL_PropertyStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                false);
        }

        internal virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        internal void SetLocalStyleValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                value != DependencyProperty.UnsetValue,
                out INTERNAL_PropertyStorage storage))
            {
                INTERNAL_PropertyStore.SetLocalStyleValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        internal void SetThemeStyleValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                value != DependencyProperty.UnsetValue,
                out INTERNAL_PropertyStorage storage))
            {
                INTERNAL_PropertyStore.SetThemeStyleValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        /// <summary>
        /// Sets a value that states that the visuals do not reflect the value of the Dependency in C#, so the visuals should be updated even if the value doesn't change the next time it is set.
        /// </summary>
        /// <param name="dp">The DependencyProperty that needs its visual's equivalents refreshed.</param>
        internal void DirtyVisualValue(DependencyProperty dp)
        {
            Debug.Assert(dp != null);

            if (INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                dp.GetMetadata(DependencyObjectType),
                true,
                out INTERNAL_PropertyStorage storage))
            {
                INTERNAL_PropertyStore.DirtyVisualValue(storage);
            }
        }

        /// <summary>
        /// Sets the inherited value of a dependency property on a DependencyObject. Do not use this method.
        /// </summary>
        /// <param name="dp">The identifier of the dependency property to set.</param>
        /// <param name="metadata"></param>
        /// <param name="value">The new local value.</param>
        /// <param name="recursively">Specifies if the inherited value must be applied to the children of this DependencyObject.</param>
        /// <returns>true if this property's value changed, false otherwise.</returns>
        internal bool SetInheritedValue(DependencyProperty dp, PropertyMetadata metadata, object value, bool recursively)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(metadata is not null);

            INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);
            
            return INTERNAL_PropertyStore.SetInheritedValue(storage,
                this,
                dp,
                metadata,
                value,
                recursively);
        }

        /// <summary>
        /// Refreshes the value of the given DependencyProperty on this DependencyObject so that it fits the coercion that should be applied on it.
        /// </summary>
        /// <param name="dependencyProperty">The dependencyProperty whose value we want to refresh.</param>
        [Obsolete(Helper.ObsoleteMemberMessage + " Use CoerceValue.")]
        public void Coerce(DependencyProperty dependencyProperty)
        {
            CoerceValue(dependencyProperty);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);
            
            INTERNAL_PropertyStore.CoerceValueCommon(storage,
                this,
                dp,
                metadata);
        }

        internal void ResetInheritedProperties()
        {
            foreach (var kvp in INTERNAL_AllInheritedProperties.ToArray())
            {
                DependencyProperty dp = kvp.Key;
                INTERNAL_PropertyStorage storage = kvp.Value;
                
                INTERNAL_PropertyStore.SetInheritedValue(storage,
                    this,
                    dp,
                    dp.GetMetadata(DependencyObjectType),
                    DependencyProperty.UnsetValue,
                    false); // recursively

                if (storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.Default)
                {
                    // Remove storage if the effective value is the default value.
                    INTERNAL_AllInheritedProperties.Remove(dp);
                    INTERNAL_PropertyStorageDictionary.Remove(dp);
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
                return Dispatcher.CurrentDispatcher;
#else
                return CoreDispatcher.CurrentDispatcher;
#endif
            }
        }


        #region Binding related elements

        internal void ApplyExpression(DependencyProperty dp, Expression expression, bool isInStyle)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            INTERNAL_PropertyStore.TryGetStorage(this,
                dp,
                metadata,
                true,
                out INTERNAL_PropertyStorage storage);
            
            INTERNAL_PropertyStore.RefreshExpressionCommon(storage,
                this,
                dp,
                metadata,
                expression,
                isInStyle); // Set LocalStyle if Binding is from style.
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
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, metadata, false, out INTERNAL_PropertyStorage storage))
            {
                INTERNAL_PropertyStore.ClearValueCommon(storage, this, dp, metadata);
            }
        }

        internal void ClearValue(DependencyPropertyKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            if (INTERNAL_PropertyStore.TryGetStorage(this, dp, metadata, false, out INTERNAL_PropertyStorage storage))
            {
                INTERNAL_PropertyStore.ClearValueCommon(storage, this, dp, metadata);
            }
        }

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

        [OpenSilver.NotImplemented]
        public object GetAnimationBaseValue(DependencyProperty dp)
        {
            return default(object);
        }

        internal void InvalidateDependents(DependencyPropertyChangedEventArgs args)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            if (_dependentListMap.TryGetValue(args.Property, out var dependents))
            {
                if (dependents.IsEmpty)
                {
                    dependents.Clear();
                }
                else
                {
                    dependents.InvalidateDependents(this, args);
                }
            }
        }

        internal void AddDependent(DependencyProperty dp, IDependencyPropertyChangedListener dependent)
        {
            _dependentListMap ??= new();
            if (!_dependentListMap.TryGetValue(dp, out var dependents))
            {
                _dependentListMap[dp] = dependents = new();
            }
            dependents.Add(dependent);
        }

        internal void RemoveDependent(DependencyProperty dp, IDependencyPropertyChangedListener dependent)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            if (_dependentListMap.TryGetValue(dp, out var dependents))
            {
                dependents.Remove(dependent);
                if (dependents.IsEmpty)
                {
                    dependents.Clear();
                }
            }
        }

        /// <summary>
        /// Called by SetValue or ClearValue to verify that the property
        /// can be changed.
        /// </summary>
        private PropertyMetadata SetupPropertyChange(DependencyProperty dp)
        {
            Debug.Assert(dp != null);

            if (dp.ReadOnly)
            {
                throw new InvalidOperationException(
                    string.Format("'{0}' property was registered as read-only and cannot be modified without an authorization key.", dp.Name));
            }

            // Get type-specific metadata for this property
            return dp.GetMetadata(DependencyObjectType);
        }

        /// <summary>
        /// Called by SetValue or ClearValue to verify that the property
        /// can be changed.
        /// </summary>
        private PropertyMetadata SetupPropertyChange(DependencyPropertyKey key, out DependencyProperty dp)
        {
            Debug.Assert(key != null);
            
            dp = key.DependencyProperty;
            Debug.Assert(dp != null);

            dp.VerifyReadOnlyKey(key);

            // Get type-specific metadata for this property
            return dp.GetMetadata(DependencyObjectType);
        }
    }
}
