
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows
{
    /// <summary>
    /// Represents an object that participates in the Silverlight dependency property
    /// system. <see cref="DependencyObject"/> is the immediate base class of several
    /// other important Silverlight classes, such as <see cref="UIElement"/>, <see cref="Geometry"/>,
    /// <see cref="FrameworkTemplate"/>, <see cref="Style"/>, and <see cref="ResourceDictionary"/>.
    /// </summary>
    public class DependencyObject : IDependencyObject
    {
        private Dictionary<int, DependentList> _dependentListMap;
        private Dictionary<int, Storage> _effectiveValues;
        private DependencyObjectType _dType;
        private int _inheritableEffectiveValuesCount;
        private ContextStorage _contextStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObject"/> class.
        /// </summary>
        public DependencyObject()
        {
            CanBeInheritanceContext = true;
        }

        internal event EventHandler InheritedContextChanged;

        internal Dictionary<int, Storage> EffectiveValues => _effectiveValues ??= new();

        internal int EffectiveValuesCount => _effectiveValues?.Count ?? 0;

        /// <summary>
        /// Returns the DType that represents the CLR type of this instance
        /// </summary>
        internal DependencyObjectType DependencyObjectType =>
            _dType ??= DependencyObjectType.FromSystemTypeInternal(GetType());

        internal bool CanBeInheritanceContext { get; set; }

        internal bool IsInheritanceContextSealed { get; set; }

        internal DependencyObject InheritanceContext
        {
            get => _contextStorage?.GetContext();
            set => (_contextStorage ??= new ContextStorage()).SetContext(value);
        }

        // We never provide an inherited context for a FrameworkElement because the DataContext takes
        // priority over inherited context.
        internal virtual bool ShouldProvideInheritanceContext(DependencyObject target, DependencyProperty property) =>
            target is not FrameworkElement;

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool RemoveSelfAsInheritanceContext(object value, DependencyProperty dp) =>
            value is DependencyObject doValue && RemoveSelfAsInheritanceContext(doValue, dp);

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool RemoveSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (doValue is not null
                && ShouldProvideInheritanceContext(doValue, dp)
                && CanBeInheritanceContext
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
            context.InheritedContextChanged -= OnInheritedContextChanged;

            // Reset inheritance context
            InheritanceContext = null;

            // Notify listeners that inheritance context changed
            OnInheritedContextChanged(EventArgs.Empty);
        }

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool ProvideSelfAsInheritanceContext(object value, DependencyProperty dp) =>
            value is DependencyObject doValue && ProvideSelfAsInheritanceContext(doValue, dp);

        // Note: the DependencyProperty parameter is here simply to keep the logic defined in the WPF 
        // implementation and is not used here
        internal bool ProvideSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (doValue is not null
                && ShouldProvideInheritanceContext(doValue, dp)
                && CanBeInheritanceContext
                && !doValue.IsInheritanceContextSealed)

            {
                if (doValue.InheritanceContext is not null)
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
            context.InheritedContextChanged += OnInheritedContextChanged;

            // Set the new context
            InheritanceContext = context;

            // Notify listeners that inheritance context changed
            OnInheritedContextChanged(EventArgs.Empty);
        }

        private void OnInheritedContextChanged(object sender, EventArgs args) => OnInheritedContextChanged(args);

        private void OnInheritedContextChanged(EventArgs args)
        {
            InheritedContextChanged?.Invoke(this, args);

            // Let sub-classes do their own thing
            OnInheritanceContextChangedCore(args);
        }

        /// <summary>
        /// This is a means for subclasses to get notification
        /// of InheritanceContext changes and then they can do
        /// their own thing.
        /// </summary>
        internal virtual void OnInheritanceContextChangedCore(EventArgs args)
        {
        }

        /// <summary>
        /// Returns the current effective value of a dependency property from a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The <see cref="DependencyProperty"/> identifier of the property to retrieve the value for.
        /// </param>
        /// <returns>
        /// Returns the current effective value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// dependencyProperty is null.
        /// </exception>
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

            if (GetStorage(dependencyProperty, metadata, false) is Storage storage)
            {
                return DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.FullyResolved);
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
        /// Sets the value of a dependency property without changing its value source.
        /// </summary>
        /// <param name="dp">
        /// The identifier of the dependency property to set.
        /// </param>
        /// <param name="value">
        /// The new local value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// dp is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// value was not the correct type as registered for the dp property.
        /// </exception>
        public void SetCurrentValue(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.SetCurrentValueCommon(storage,
                this,
                dp,
                metadata,
                value);
        }

        /// <summary>
        /// Returns the local value of a dependency property, if a local value is set.
        /// </summary>
        /// <param name="dp">
        /// The <see cref="DependencyProperty"/> identifier of the property for which to retrieve the
        /// local value.
        /// </param>
        /// <returns>
        /// Returns the local value, or returns the sentinel value <see cref="DependencyProperty.UnsetValue"/>
        /// if no local value is set.
        /// </returns>
        public object ReadLocalValue(DependencyProperty dp)
        {
            if (GetStorage(dp, null, false) is Storage storage)
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
            if (GetStorage(dp, null, false) is Storage storage)
            {
                return storage.LocalValue;
            }

            return DependencyProperty.UnsetValue;
        }

        internal bool HasDefaultValue(DependencyProperty dp)
        {
            return GetStorage(dp, null, false) is not Storage storage ||
                storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.Default;
        }

        internal void RefreshAnimation(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (GetStorage(dp, metadata, false) is Storage storage)
            {
                if (storage.Clock == clock)
                {
                    DependencyObjectStore.SetAnimatedValue(storage,
                        this,
                        dp,
                        metadata,
                        clock.GetCurrentValue());
                }
            }
        }

        internal void AttachAnimationClock(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetStorage(dp, metadata, true);
            storage.Clock = clock;
        }

        internal void DetachAnimationClock(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (GetStorage(dp, metadata, false) is Storage storage)
            {
                if (storage.Clock == clock)
                {
                    storage.Clock = null;
                    DependencyObjectStore.ClearAnimatedValue(storage, this, dp, metadata);
                }
            }
        }

        /// <summary>
        /// Sets the local value of a dependency property on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dp">
        /// The identifier of the dependency property to set.
        /// </param>
        /// <param name="value">
        /// The new local value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// dp is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// value was not the correct type as registered for the dp property.
        /// </exception>
        public void SetValue(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                false);
        }

        internal void SetValue(DependencyProperty dp, bool value) => SetValue(dp, BooleanBoxes.Box(value));

        internal void SetValueInternal(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                true);
        }

        internal void SetValueInternal(DependencyProperty dp, bool value) => SetValueInternal(dp, BooleanBoxes.Box(value));

        /// <summary>
        /// Sets the local value of a read-only dependency property on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="DependencyPropertyKey"/> identifier of the property to set.
        /// </param>
        /// <param name="value">
        /// The new local value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// key is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// value was not the correct type as registered for the property.
        /// </exception>
        public void SetValue(DependencyPropertyKey key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                false);
        }

        internal void SetValue(DependencyPropertyKey key, bool value) => SetValue(key, BooleanBoxes.Box(value));

        internal void SetValueInternal(DependencyPropertyKey key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                true);
        }

        internal void SetValueInternal(DependencyPropertyKey key, bool value) => SetValueInternal(key, BooleanBoxes.Box(value));

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this
        /// <see cref="DependencyObject"/> has been updated. The specific dependency 
        /// property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">
        /// Event data that will contain the dependency property identifier of interest, 
        /// the property metadata for the type, and old and new values.
        /// </param>
        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            e.Metadata?.PropertyChangedCallback?.Invoke(this, e);
        }

        internal void NotifyPropertyChange(DependencyPropertyChangedEventArgs e)
        {
            // fire change notifications
            OnPropertyChanged(e);

            // update bindings
            InvalidateDependents(e);
        }

        internal void SetLocalStyleValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (GetStorage(dp, metadata, value != DependencyProperty.UnsetValue) is Storage storage)
            {
                DependencyObjectStore.SetLocalStyleValue(storage,
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

            if (GetStorage(dp, metadata, value != DependencyProperty.UnsetValue) is Storage storage)
            {
                DependencyObjectStore.SetThemeStyleValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
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

            Storage storage = GetStorage(dp, metadata, true);

            return DependencyObjectStore.SetInheritedValue(storage,
                this,
                dp,
                metadata,
                value,
                recursively);
        }

        /// <summary>
        /// Coerces the value of the specified dependency property. This is accomplished by invoking
        /// any <see cref="CoerceValueCallback"/> function specified in property metadata for the 
        /// dependency property as it exists on the calling <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dp">
        /// The identifier for the dependency property to coerce.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// dp is null.
        /// </exception>
        public void CoerceValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.CoerceValueCommon(storage,
                this,
                dp,
                metadata);
        }

        internal static void InvalidateInheritedProperties(DependencyObject d, DependencyObject newParent)
        {
            if (newParent == null)
            {
                foreach (Storage storage in CopyInheritedStorages(d))
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                    DependencyObjectStore.SetInheritedValue(storage,
                        d,
                        dp,
                        dp.GetMetadata(d.DependencyObjectType),
                        DependencyProperty.UnsetValue,
                        false); // recursively
                }
            }
            else
            {
                foreach (Storage storage in CopyInheritedStorages(newParent))
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                    PropertyMetadata metadata = dp.GetMetadata(d.DependencyObjectType);
                    if (TreeWalkHelper.IsInheritanceNode(metadata))
                    {
                        d.SetInheritedValue(
                            dp,
                            metadata,
                            DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.FullyResolved),
                            true);
                    }
                }
            }

            static Storage[] CopyInheritedStorages(DependencyObject d)
            {
                int count = d._inheritableEffectiveValuesCount;
                if (count > 0)
                {
                    var storages = new Storage[count];
                    int i = 0;
                    foreach (KeyValuePair<int, Storage> kvp in d._effectiveValues)
                    {
                        Storage storage = kvp.Value;
                        if (storage.Inheritable)
                        {
                            storages[i++] = storage;
                            if (i == count)
                            {
                                break;
                            }
                        }
                    }
                    return storages;
                }

                return Array.Empty<Storage>();
            }
        }

        /// <summary>
        /// Gets the <see cref="Threading.Dispatcher"/> this object is associated with.
        /// </summary>
        /// <returns>
        /// The <see cref="Threading.Dispatcher"/> this object is associated with.
        /// </returns>
        public Dispatcher Dispatcher => Dispatcher.CurrentDispatcher;

        internal void ApplyExpression(DependencyProperty dp, Expression expression)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetStorage(dp, metadata, true);

            DependencyObjectStore.RefreshExpressionCommon(storage,
                this,
                dp,
                metadata,
                expression);
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

        /// <summary>
        /// Clears the local value of a dependency property.
        /// </summary>
        /// <param name="dp">
        /// The <see cref="DependencyProperty"/> identifier of the property to clear the value for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// dp is null.
        /// </exception>
        public void ClearValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            if (GetStorage(dp, metadata, false) is Storage storage)
            {
                DependencyObjectStore.ClearValueCommon(storage, this, dp, metadata);
            }
        }

        /// <summary>
        /// Clears the local value of a read-only property.
        /// The property to be cleared is specified by a <see cref="DependencyPropertyKey"/>.
        /// </summary>
        /// <param name="key">
        /// The key for the dependency property to be cleared.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// key is null.
        /// </exception>
        public void ClearValue(DependencyPropertyKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            if (GetStorage(dp, metadata, false) is Storage storage)
            {
                DependencyObjectStore.ClearValueCommon(storage, this, dp, metadata);
            }
        }

        /// <summary>
        /// Determines whether the calling thread has access to this object.
        /// </summary>
        /// <returns>
        /// true if the calling thread has access to this object; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Returns any base value established for a Silverlight dependency property, which
        /// would apply in cases where an animation is not active.
        /// </summary>
        /// <param name="dp">
        /// The identifier for the desired dependency property.
        /// </param>
        /// <returns>
        /// The returned base value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// dp is null.
        /// </exception>
        public object GetAnimationBaseValue(DependencyProperty dp)
        {
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            if (GetStorage(dp, metadata, false) is Storage storage)
            {
                return DependencyObjectStore.GetEffectiveValue(storage.Entry, RequestFlags.AnimationBaseValue);
            }

            return metadata.GetDefaultValue(this, dp);
        }

        internal void InvalidateDependents(DependencyPropertyChangedEventArgs args)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            if (_dependentListMap.TryGetValue(args.Property.GlobalIndex, out var dependents))
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

        internal void AddDependent(DependencyProperty dp, DependencyPropertyChangedListener dependent)
        {
            int propertyIndex = dp.GlobalIndex;
            _dependentListMap ??= new();
            if (!_dependentListMap.TryGetValue(propertyIndex, out var dependents))
            {
                _dependentListMap[propertyIndex] = dependents = new();
            }
            dependents.Add(dependent);
        }

        internal void RemoveDependent(DependencyProperty dp, DependencyPropertyChangedListener dependent)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            if (_dependentListMap.TryGetValue(dp.GlobalIndex, out var dependents))
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
                throw new InvalidOperationException(string.Format(Strings.ReadOnlyChangeNotAllowed, dp.Name));
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

        internal Storage GetStorage(DependencyProperty dp, PropertyMetadata metadata, bool createIfNotFound)
        {
            Storage storage = null;
            int propertyIndex = dp.GlobalIndex;
            if (_effectiveValues is not null && _effectiveValues.TryGetValue(propertyIndex, out storage))
            {
                return storage;
            }

            if (createIfNotFound)
            {
                metadata ??= dp.GetMetadata(DependencyObjectType);
                storage = Storage.CreateDefaultValueEntry(dp, metadata.Inherits, metadata.GetDefaultValue(this, dp));
                EffectiveValues.Add(propertyIndex, storage);
                if (metadata.Inherits)
                {
                    _inheritableEffectiveValuesCount++;
                }
            }

            return storage;
        }

        internal void RemoveStorage(Storage storage)
        {
            if (_effectiveValues.Remove(storage.PropertyIndex) && storage.Inheritable)
            {
                _inheritableEffectiveValuesCount--;
            }
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
                _context = _useWeakRef ? new WeakReference<DependencyObject>(context) : context;
            }
        }
    }
}
