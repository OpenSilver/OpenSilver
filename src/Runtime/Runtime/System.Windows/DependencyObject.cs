
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

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using OpenSilver.Internal;
using OpenSilver.Internal.ComponentModel;
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
    [TypeDescriptionProvider(typeof(DependencyObjectProvider))]
    public partial class DependencyObject : DispatcherObject, IDependencyObject
    {
        [Flags]
        private enum Flags
        {
            InheritableEffectiveValuesCountMask = short.MaxValue,
            ModifiersMask = CanBeInheritanceContext | IsInheritanceContextSealed | UseWeakContextReference | IsSealed,
            CanBeInheritanceContext = 0x00008000,
            IsInheritanceContextSealed = 0x00010000,
            UseWeakContextReference = 0x00020000,
            IsSealed = 0x00040000,
        }

        private PropertyStore<Storage> _effectiveValues;
        private PropertyStore<DependentList> _dependentListMap;
        private DependencyObjectType _dType;
        private object _context;
        private Flags _packedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObject"/> class.
        /// </summary>
        public DependencyObject()
        {
            _packedData = Flags.CanBeInheritanceContext;
        }

        internal event EventHandler InheritedContextChanged;

        internal ReadOnlySpan<Storage> EffectiveValues => _effectiveValues is null ? [] : _effectiveValues.Span;

        internal int EffectiveValuesCount => _effectiveValues?.Count ?? 0;

        /// <summary>
        /// Gets the <see cref="Windows.DependencyObjectType"/> that wraps the CLR type of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="Windows.DependencyObjectType"/> that wraps the CLR type of this instance.
        /// </returns>
        public DependencyObjectType DependencyObjectType =>
            _dType ??= DependencyObjectType.FromSystemTypeInternal(GetType());

        internal bool CanBeInheritanceContext
        {
            get => ReadFlag(Flags.CanBeInheritanceContext);
            set => WriteFlag(Flags.CanBeInheritanceContext, value);
        }

        internal bool IsInheritanceContextSealed
        {
            get => ReadFlag(Flags.IsInheritanceContextSealed);
            set => WriteFlag(Flags.IsInheritanceContextSealed, value);
        }

        private bool UseWeakContextReference
        {
            get => ReadFlag(Flags.UseWeakContextReference);
            set => WriteFlag(Flags.UseWeakContextReference, value);
        }

        private int InheritableEffectiveValuesCount
        {
            get => (int)(_packedData & Flags.InheritableEffectiveValuesCountMask);
            set => _packedData = (_packedData & Flags.ModifiersMask) | ((Flags)value & Flags.InheritableEffectiveValuesCountMask);
        }

        private bool ReadFlag(Flags field) => (_packedData & field) != 0;

        private void WriteFlag(Flags field, bool value)
        {
            if (value)
            {
                _packedData |= field;
            }
            else
            {
                _packedData &= (~field);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is currently sealed (read-only).
        /// </summary>
        /// <returns>
        /// true if this instance is sealed; otherwise, false.
        /// </returns>
        public bool IsSealed
        {
            get => ReadFlag(Flags.IsSealed);
            private set => WriteFlag(Flags.IsSealed, value);
        }

        internal void Seal()
        {
            // Since this object no longer changes it won't be able to notify dependents
            _dependentListMap = null;

            IsSealed = true;
        }

        internal DependencyObject InheritanceContext
        {
            get
            {
                if (UseWeakContextReference)
                {
                    var wr = (WeakReference<DependencyObject>)_context;
                    if (!wr.TryGetTarget(out DependencyObject context))
                    {
                        UseWeakContextReference = false;
                        _context = null;
                    }

                    return context;
                }

                return (DependencyObject)_context;
            }
            set
            {
                bool useWeakRef = value is FrameworkElement;
                UseWeakContextReference = useWeakRef;
                _context = useWeakRef ? new WeakReference<DependencyObject>(value) : value;
            }
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
            ArgumentNullException.ThrowIfNull(dependencyProperty);

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

            if (GetStorage(dependencyProperty.GlobalIndex) is Storage storage)
            {
                return DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.FullyResolved);
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
            ArgumentNullException.ThrowIfNull(dp);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetCurrentValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                false);
        }

        internal void SetCurrentValueInternal(DependencyProperty dp, object value)
        {
            ArgumentNullException.ThrowIfNull(dp);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetCurrentValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                true);
        }

        /// <summary>
        /// Creates a specialized enumerator for determining which dependency properties have locally set values 
        /// on this <see cref="DependencyObject"/>.
        /// </summary>
        /// <returns>
        /// A specialized local value enumerator.
        /// </returns>
        public LocalValueEnumerator GetLocalValueEnumerator()
        {
            int effectiveValuesCount = EffectiveValuesCount;
            if (effectiveValuesCount == 0)
            {
                return LocalValueEnumerator.Empty;
            }

            var snapshot = new LocalValueEntry[effectiveValuesCount];
            int count = 0;

            // Iterate through the effectiveValues
            foreach (Storage storage in EffectiveValues)
            {
                if (DependencyProperty.RegisteredPropertyList[storage.PropertyIndex] is not DependencyProperty dp)
                {
                    continue;
                }

                object localValue = ReadLocalValueEntry(storage);
                if (localValue != DependencyProperty.UnsetValue)
                {
                    snapshot[count++] = new LocalValueEntry(dp, localValue);
                }
            }

            return new LocalValueEnumerator(snapshot, count);
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
            ArgumentNullException.ThrowIfNull(dp);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                return ReadLocalValueEntry(storage);
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Retrieve the local value of a property (if set).
        /// </summary>
        /// <returns>
        /// The local value. <see cref="DependencyProperty.UnsetValue"/> if no local value was set via 
        /// <see cref="SetValue(DependencyProperty, object)"/>.
        /// </returns>
        private object ReadLocalValueEntry(Storage storage)
        {
            EffectiveValueEntry entry = storage.Entry;
            object value = entry.IsCoercedWithCurrentValue ? entry.ModifiedValue.CoercedValue : entry.LocalValue;
            return value;
        }

        internal bool HasDefaultValue(DependencyProperty dp)
        {
            return GetStorage(dp.GlobalIndex) is not Storage storage ||
                storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.Default;
        }

        internal void RefreshAnimation(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            if (GetStorage(dp.GlobalIndex) is Storage storage && storage.Clock == clock)
            {
                PropertyMetadata metadata = SetupPropertyChange(dp);

                if (clock.CurrentState == ClockState.Stopped)
                {
                    DependencyObjectStore.ClearAnimatedValue(storage, this, dp, metadata);
                }
                else
                {
                    DependencyObjectStore.SetAnimatedValue(storage, this, dp, metadata, clock.GetCurrentValue());
                }
            }
        }

        internal void AttachAnimationClock(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetOrCreateStorage(dp, metadata);
            storage.Clock = clock;
        }

        internal void DetachAnimationClock(DependencyProperty dp, AnimationClock clock)
        {
            Debug.Assert(dp is not null);
            Debug.Assert(clock is not null);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                if (storage.Clock == clock)
                {
                    storage.Clock = null;

                    PropertyMetadata metadata = SetupPropertyChange(dp);
                    DependencyObjectStore.ClearAnimatedValue(storage, this, dp, metadata);
                }
            }
        }

        internal void DetachAnimationClock(DependencyProperty dp, bool clearAnimatedValue)
        {
            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                storage.Clock = null;

                if (clearAnimatedValue)
                {
                    PropertyMetadata metadata = SetupPropertyChange(dp);
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
            ArgumentNullException.ThrowIfNull(dp);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.SetOnReadOnlyObjectNotAllowed, this));
            }

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                OperationType.Unknown,
                false);
        }

        internal void SetValue(DependencyProperty dp, bool value) => SetValue(dp, BooleanBoxes.Box(value));

        internal void SetValueInternal(DependencyProperty dp, object value)
        {
            ArgumentNullException.ThrowIfNull(dp);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.SetOnReadOnlyObjectNotAllowed, this));
            }

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                OperationType.Unknown,
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
            ArgumentNullException.ThrowIfNull(key);

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.SetOnReadOnlyObjectNotAllowed, this));
            }

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                OperationType.Unknown,
                false);
        }

        internal void SetValue(DependencyPropertyKey key, bool value) => SetValue(key, BooleanBoxes.Box(value));

        internal void SetValueInternal(DependencyPropertyKey key, object value)
        {
            ArgumentNullException.ThrowIfNull(key);

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.SetOnReadOnlyObjectNotAllowed, this));
            }

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                OperationType.Unknown,
                true);
        }

        internal void SetValueInternal(DependencyPropertyKey key, bool value) => SetValueInternal(key, BooleanBoxes.Box(value));

        internal void SetMutableDefaultValue(DependencyProperty dp, object value)
        {
            // Cache the metadata object this method needed to get anyway.
            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.SetValueCommon(storage,
                this,
                dp,
                metadata,
                value,
                OperationType.ChangeMutableDefaultValue,
                true);
        }

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

            if (value == DependencyProperty.UnsetValue)
            {
                if (GetStorage(dp.GlobalIndex) is Storage storage)
                {
                    DependencyObjectStore.ClearLocalStyleValue(storage,
                        this,
                        dp,
                        metadata);
                }
            }
            else
            {
                Storage storage = GetOrCreateStorage(dp, metadata);
                DependencyObjectStore.SetLocalStyleValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        internal void SetTriggerValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (value == DependencyProperty.UnsetValue)
            {
                ClearTriggerValue(dp);
            }
            else
            {
                Storage storage = GetOrCreateStorage(dp, metadata);
                DependencyObjectStore.SetStyleTriggerValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        internal void ClearTriggerValue(DependencyProperty dp)
        {
            Debug.Assert(dp != null);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                PropertyMetadata metadata = SetupPropertyChange(dp);
                DependencyObjectStore.ClearStyleTriggerValue(storage,
                    this,
                    dp,
                    metadata);
            }
        }

        internal void SetThemeStyleTriggerValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (value == DependencyProperty.UnsetValue)
            {
                ClearThemeStyleTriggerValue(dp);
            }
            else
            {
                Storage storage = GetOrCreateStorage(dp, metadata);
                DependencyObjectStore.SetThemeStyleTriggerValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        internal void ClearThemeStyleTriggerValue(DependencyProperty dp)
        {
            Debug.Assert(dp != null);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                PropertyMetadata metadata = SetupPropertyChange(dp);
                DependencyObjectStore.ClearThemeStyleTriggerValue(storage,
                    this,
                    dp,
                    metadata);
            }
        }

        internal void SetParentTemplateTriggerValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (value == DependencyProperty.UnsetValue)
            {
                ClearParentTemplateTriggerValue(dp);
            }
            else
            {
                Storage storage = GetOrCreateStorage(dp, metadata);
                DependencyObjectStore.SetParentTemplateTriggerValue(storage,
                    this,
                    dp,
                    metadata,
                    value);
            }
        }

        internal void ClearParentTemplateTriggerValue(DependencyProperty dp)
        {
            Debug.Assert(dp != null);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                PropertyMetadata metadata = SetupPropertyChange(dp);
                DependencyObjectStore.ClearParentTemplateTriggerValue(storage,
                    this,
                    dp,
                    metadata);
            }
        }

        internal void SetThemeStyleValue(DependencyProperty dp, object value)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            if (value == DependencyProperty.UnsetValue)
            {
                if (GetStorage(dp.GlobalIndex) is Storage storage)
                {
                    DependencyObjectStore.ClearThemeStyleValue(storage,
                        this,
                        dp,
                        metadata);
                }
            }
            else
            {
                Storage storage = GetOrCreateStorage(dp, metadata);
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

            Storage storage = GetOrCreateStorage(dp, metadata);

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
            ArgumentNullException.ThrowIfNull(dp);

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            Storage storage = GetOrCreateStorage(dp, metadata);

            DependencyObjectStore.CoerceValueCommon(storage,
                this,
                dp,
                metadata);
        }

        internal static void InvalidateInheritedProperties(DependencyObject d, DependencyObject newParent)
        {
            if (newParent is null)
            {
                foreach (Storage storage in CopyInheritedStorages(d))
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                    Debug.Assert(dp is not null);

                    DependencyObjectStore.ClearInheritedValue(storage,
                        d,
                        dp,
                        dp.GetMetadata(d.DependencyObjectType),
                        false); // recursively
                }
            }
            else
            {
                foreach (Storage storage in CopyInheritedStorages(newParent))
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList[storage.PropertyIndex];
                    Debug.Assert(dp is not null);

                    PropertyMetadata metadata = dp.GetMetadata(d.DependencyObjectType);
                    if (TreeWalkHelper.IsInheritanceNode(metadata))
                    {
                        d.SetInheritedValue(
                            dp,
                            metadata,
                            DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.FullyResolved),
                            true);
                    }
                }
            }

            static Storage[] CopyInheritedStorages(DependencyObject d)
            {
                int count = d.InheritableEffectiveValuesCount;
                if (count > 0)
                {
                    var storages = new Storage[count];
                    int i = 0;
                    foreach (Storage storage in d.EffectiveValues)
                    {
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

                return [];
            }
        }

        /// <summary>
        /// Gets the <see cref="Threading.Dispatcher"/> this object is associated with.
        /// </summary>
        /// <returns>
        /// The <see cref="Threading.Dispatcher"/> this object is associated with.
        /// </returns>
        public new Dispatcher Dispatcher => base.Dispatcher;

        internal void ApplyExpression(DependencyProperty dp, Expression expression)
        {
            Debug.Assert(dp != null);

            PropertyMetadata metadata = SetupPropertyChange(dp);

            Storage storage = GetOrCreateStorage(dp, metadata);

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
            ArgumentNullException.ThrowIfNull(dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.ClearOnReadOnlyObjectNotAllowed, this));
            }

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);
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
            ArgumentNullException.ThrowIfNull(key);

            PropertyMetadata metadata = SetupPropertyChange(key, out DependencyProperty dp);

            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.ClearOnReadOnlyObjectNotAllowed, this));
            }

            if (GetStorage(dp.GlobalIndex) is Storage storage)
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
        public new bool CheckAccess() => base.CheckAccess();

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
            ArgumentNullException.ThrowIfNull(dp);

            if (GetStorage(dp.GlobalIndex) is Storage storage)
            {
                return DependencyObjectStore.GetEffectiveValue(ref storage.Entry, RequestFlags.AnimationBaseValue);
            }

            PropertyMetadata metadata = dp.GetMetadata(DependencyObjectType);

            return metadata.GetDefaultValue(this, dp);
        }

        /// <summary>
        /// Returns a value that indicates whether serialization processes should serialize the value for 
        /// the provided dependency property.
        /// </summary>
        /// <param name="dp">
        /// The identifier for the dependency property that should be serialized.
        /// </param>
        /// <returns>
        /// true if the dependency property that is supplied should be value-serialized; otherwise, false.
        /// </returns>
        protected internal virtual bool ShouldSerializeProperty(DependencyProperty dp) => ContainsValue(dp);

        /// <summary>
        /// This method is called by DependencyObjectPropertyDescriptor to determine if a value is set for a given DP.
        /// </summary>
        internal bool ContainsValue(DependencyProperty dp)
        {
            if (GetStorage(dp.GlobalIndex) is not Storage storage)
            {
                return false;
            }

            EffectiveValueEntry entry = storage.Entry;
            object value = entry.IsCoercedWithCurrentValue ? entry.ModifiedValue.CoercedValue : entry.LocalValue;
            return !ReferenceEquals(value, DependencyProperty.UnsetValue);
        }

        internal void InvalidateDependents(DependencyPropertyChangedEventArgs args)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            int entryIndex = _dependentListMap.LookupEntry(args.Property.GlobalIndex);
            if (entryIndex >= 0)
            {
                ref DependentList dependents = ref _dependentListMap[entryIndex];
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

        internal void AddDependent(DependencyProperty dp, PropertyChangeListener dependent)
        {
            // A Sealed DependencyObject does not have a Dependents list so don't bother updating it.
            Debug.Assert(!IsSealed);

            _dependentListMap ??= new PropertyStore<DependentList>(2);

            int entryIndex = _dependentListMap.LookupEntry(dp.GlobalIndex);

            if (entryIndex < 0)
            {
                entryIndex = ~entryIndex;
                _dependentListMap.InsertEntry(new DependentList(dp.GlobalIndex), entryIndex);
            }

            _dependentListMap[entryIndex].Add(dependent);
        }

        internal void RemoveDependent(DependencyProperty dp, PropertyChangeListener dependent)
        {
            if (_dependentListMap is null)
            {
                return;
            }

            int entryIndex = _dependentListMap.LookupEntry(dp.GlobalIndex);

            if (entryIndex >= 0)
            {
                ref DependentList dependents = ref _dependentListMap[entryIndex];
                dependents.Remove(dependent);

                if (dependents.IsEmpty)
                {
                    _dependentListMap.RemoveAt(entryIndex);
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

        internal Storage GetStorage(int targetIndex)
        {
            if (_effectiveValues is not null)
            {
                int entryIndex = _effectiveValues.LookupEntry(targetIndex);
                if (entryIndex >= 0)
                {
                    return _effectiveValues[entryIndex];
                }
            }

            return null;
        }

        private Storage GetOrCreateStorage(DependencyProperty dp, PropertyMetadata metadata)
        {
            int entryIndex = -1;

            if (_effectiveValues is not null)
            {
                entryIndex = _effectiveValues.LookupEntry(dp.GlobalIndex);
                if (entryIndex >= 0)
                {
                    return _effectiveValues[entryIndex];
                }
            }

            _effectiveValues ??= new PropertyStore<Storage>();

            var storage = new Storage(dp, metadata.Inherits);
            _effectiveValues.InsertEntry(storage, ~entryIndex);

            if (metadata.Inherits)
            {
                InheritableEffectiveValuesCount++;
            }

            // Note: we must insert the new storage before setting its default value. For DP using a 
            // default value factory, generating default a default value will modify _effectiveValues
            // because the value is store in an UncommonField. The result is that entryIndex may become
            // invalid.
            storage.Entry = new EffectiveValueEntry(metadata.GetDefaultValue(this, dp));

            return storage;
        }

        internal void RemoveStorage(Storage storage)
        {
            Debug.Assert(_effectiveValues is not null);

            if (_effectiveValues.Remove(storage.PropertyIndex) && storage.Inheritable)
            {
                InheritableEffectiveValuesCount--;
            }
        }

        internal Storage GetUncommonStorage(int targetIndex)
        {
            int entryIndex = -1;

            if (_effectiveValues is not null)
            {
                entryIndex = _effectiveValues.LookupEntry(targetIndex);
                if (entryIndex >= 0)
                {
                    return _effectiveValues[entryIndex];
                }
            }

            _effectiveValues ??= new PropertyStore<Storage>();

            var storage = new Storage(targetIndex);
            _effectiveValues.InsertEntry(storage, ~entryIndex);

            return storage;
        }

        internal void RemoveUncommonStorage(int targetIndex)
        {
            Debug.Assert(_effectiveValues is not null);

            _effectiveValues.Remove(targetIndex);
        }
    }
}
