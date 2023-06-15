
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
using System.Runtime.CompilerServices;
using OpenSilver.Internal;
using OpenSilver.Utility;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Defines certain behavior aspects of a dependency property, including conditions
    /// it was registered with.
    /// </summary>
    public class PropertyMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class.
        /// </summary>
        public PropertyMetadata()
        {
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class, using
        /// a property default value.
        /// </summary>
        /// <param name="defaultValue">
        /// A default value for the property where this <see cref="PropertyMetadata"/> is
        /// applied.
        /// </param>
        public PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class, using
        /// a callback reference.
        /// </summary>
        /// <param name="propertyChangedCallback">
        /// A reference to the callback to call for property changed behavior.
        /// </param>
        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        {
            PropertyChangedCallback = propertyChangedCallback;
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class, using
        /// a property default value and callback reference.
        /// </summary>
        /// <param name="defaultValue">
        /// A default value for the property where this <see cref="PropertyMetadata"/> is
        /// applied.
        /// </param>
        /// <param name="propertyChangedCallback">
        /// A reference to the callback to call for property changed behavior.
        /// </param>
        public PropertyMetadata(object defaultValue,
                                PropertyChangedCallback propertyChangedCallback)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class with
        /// the specified default value and callbacks.
        /// </summary>
        /// <param name="defaultValue">
        /// The default value of the dependency property, usually provided as a value of
        /// some specific type.
        /// </param>
        /// <param name="propertyChangedCallback">
        /// Reference to a handler implementation that is to be called by the property system
        /// whenever the effective value of the property changes.
        /// </param>
        /// <param name="coerceValueCallback">
        /// Reference to a handler implementation that is to be called whenever the property
        /// system calls <see cref="DependencyObject.CoerceValue(DependencyProperty)"/>
        /// against this property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// defaultValue cannot be set to the value <see cref="DependencyProperty.UnsetValue"/>;
        /// </exception>
        public PropertyMetadata(object defaultValue,
                                PropertyChangedCallback propertyChangedCallback,
                                CoerceValueCallback coerceValueCallback)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Gets the default value for the dependency property.
        /// </summary>
        /// <returns>
        /// The default value for the dependency property.
        /// </returns>
        public object DefaultValue
        {
            get
            {
                DefaultValueFactory defaultFactory = _defaultValue as DefaultValueFactory;
                if (defaultFactory == null)
                {
                    return _defaultValue;
                }
                else
                {
                    return defaultFactory.DefaultValue;
                }
            }
            set
            {
                CheckSealed();

                _defaultValue = value;

                WriteFlag(MetadataFlags.DefaultValueModifiedID, value != DependencyProperty.UnsetValue);
            }
        }

        /// <summary>
        /// Returns true if the default value is a DefaultValueFactory
        /// </summary>
        internal bool UsingDefaultValueFactory
        {
            get
            {
                return _defaultValue is DefaultValueFactory;
            }
        }

        /// <summary>
        /// GetDefaultValue returns the default value for a given owner and property.
        /// If the default value is a DefaultValueFactory it will instantiate and cache
        /// the default value on the object.  It must never return an unfrozen default
        /// value if the owner is a frozen Freezable.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal object GetDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            Debug.Assert(owner != null && property != null,
                "Caller must provide owner and property or this method will throw in the event of a cache miss.");

            // If we are not using a DefaultValueFactory (common case)
            // just return _defaultValue
            DefaultValueFactory defaultFactory = _defaultValue as DefaultValueFactory;
            if (defaultFactory == null)
            {
                return _defaultValue;
            }

            // See if we already have a valid default value that was
            // created by a prior call to GetDefaultValue.
            object result = GetCachedDefaultValue(owner, property);

            if (result != DependencyProperty.UnsetValue)
            {
                return result;
            }

            // Otherwise we need to invoke the factory to create the DefaultValue
            // for this property.
            result = defaultFactory.CreateDefaultValue(owner, property);

            // Default value validation ensures that default values do not have
            // thread affinity. This is because a default value is typically 
            // stored in the shared property metadata and handed out to all
            // instances of the owning DependencyObject type.  
            //
            // DefaultValueFactory.CreateDefaultValue ensures that the default  
            // value has thread-affinity to the current thread.  We can thus 
            // skip that portion of the default value validation by calling
            // ValidateFactoryDefaultValue.

            property.ValidateFactoryDefaultValue(result);

            // Cache the created DefaultValue so that we can consistently hand
            // out the same default each time we are asked.
            SetCachedDefaultValue(owner, property, result);

            return result;
        }

        // Because the frugalmap is going to be stored in an uncommon field, it would get boxed
        // to avoid this boxing, skip the struct and go straight for the class contained by the
        // struct.  Given the simplicity of this scenario, we can get away with this.
        private object GetCachedDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            if (!_defaultValueFactoryCache.TryGetValue(owner, out FrugalMapBase map))
            {
                return DependencyProperty.UnsetValue;
            }

            return map.Search(property.GlobalIndex);
        }

        private void SetCachedDefaultValue(DependencyObject owner, DependencyProperty property, object value)
        {
            if (!_defaultValueFactoryCache.TryGetValue(owner, out FrugalMapBase map))
            {
                map = new SingleObjectMap();
                _defaultValueFactoryCache.Add(owner, map);
            }
            else if (map is not HashObjectMap)
            {
                FrugalMapBase newMap = new HashObjectMap();
                map.Promote(newMap);
                map = newMap;
                _defaultValueFactoryCache.Remove(owner);
                _defaultValueFactoryCache.Add(owner, map);
            }

            map.InsertEntry(property.GlobalIndex, value);
        }

        /// <summary>
        /// This method causes the DefaultValue cache to be cleared ensuring
        /// that CreateDefaultValue will be called next time this metadata
        /// is asked to participate in the DefaultValue factory pattern.
        ///
        /// This is internal so it can be accessed by subclasses of
        /// DefaultValueFactory.
        /// </summary>
        internal void ClearCachedDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            if (!_defaultValueFactoryCache.TryGetValue(owner, out FrugalMapBase map))
            {
                return;
            }

            if (map.Count == 1)
            {
                _defaultValueFactoryCache.Remove(owner);
            }
            else
            {
                map.RemoveEntry(property.GlobalIndex);
            }
        }

        /// <summary>
        ///     Whether the DefaultValue was explictly set - needed to know if the
        /// value should be used in Register.
        /// </summary>
        internal bool DefaultValueWasSet()
        {
            return IsModified(MetadataFlags.DefaultValueModifiedID);
        }

        /// <summary>
        /// Gets or sets a reference to a <see cref="Windows.PropertyChangedCallback"/> implementation
        /// specified in this metadata.
        /// </summary>
        /// <returns>
        /// A <see cref="Windows.PropertyChangedCallback"/> implementation reference.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Cannot set a metadata property once it is applied to a dependency property operation.
        /// </exception>
        public PropertyChangedCallback PropertyChangedCallback
        {
            get { return _propertyChangedCallback; }
            set { CheckSealed(); _propertyChangedCallback = value; }
        }

        /// <summary>
        /// Gets or sets a reference to a <see cref="Windows.CoerceValueCallback"/> implementation
        /// specified in this metadata.
        /// </summary>
        /// <returns>
        /// A <see cref="Windows.CoerceValueCallback"/> implementation reference.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Cannot set a metadata property once it is applied to a dependency property operation.
        /// </exception>
        public CoerceValueCallback CoerceValueCallback
        {
            get { return _coerceValueCallback; }
            set { CheckSealed(); _coerceValueCallback = value; }
        }

        /// <summary>
        /// Specialized callback for remote storage of value for read-only properties
        /// </summary>
        /// <remarks>
        /// This is used exclusively by <see cref="FrameworkElement.ActualWidth"/> and 
        /// <see cref="FrameworkElement.ActualHeight"/> to save 48 bytes of state per 
        /// FrameworkElement.
        /// </remarks>
        internal virtual GetReadOnlyValueCallback GetReadOnlyValueCallback
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new instance of this property metadata.  This method is used
        /// when metadata needs to be cloned.  After CreateInstance is called the
        /// framework will call Merge to merge metadata into the new instance.
        /// Deriving classes must override this and return a new instance of
        /// themselves.
        /// </summary>
        internal virtual PropertyMetadata CreateInstance()
        {
            return new PropertyMetadata();
        }

        //
        // Returns a copy of this property metadata by calling CreateInstance
        // and then Merge
        //
        internal PropertyMetadata Copy(DependencyProperty dp)
        {
            PropertyMetadata newMetadata = CreateInstance();
            newMetadata.InvokeMerge(this, dp);
            return newMetadata;
        }

        /// <summary>
        /// Merges this metadata with the base metadata.
        /// </summary>
        /// <param name="baseMetadata">
        /// The base metadata to merge with this instance's values.
        /// </param>
        /// <param name="dp">
        /// The dependency property to which this metadata is being applied.
        /// </param>
        internal virtual void Merge(PropertyMetadata baseMetadata, DependencyProperty dp)
        {
            if (baseMetadata == null)
            {
                throw new ArgumentNullException(nameof(baseMetadata));
            }

            CheckSealed();

            // Merge source metadata into this

            // Take source default if this default was never set
            if (!IsModified(MetadataFlags.DefaultValueModifiedID))
            {
                _defaultValue = baseMetadata.DefaultValue;
            }

            if (baseMetadata.PropertyChangedCallback != null)
            {
                // All delegates are MulticastDelegate.  Non-multicast "Delegate"
                //  was designed and is documented in MSDN.  But for all practical
                //  purposes, it was actually cut before v1.0 of the CLR shipped.

                // Build the handler list such that handlers added
                // via OverrideMetadata are called last (base invocation first)
                Delegate[] handlers = baseMetadata.PropertyChangedCallback.GetInvocationList();
                if (handlers.Length > 0)
                {
                    PropertyChangedCallback headHandler = (PropertyChangedCallback)handlers[0];
                    for (int i = 1; i < handlers.Length; i++)
                    {
                        headHandler += (PropertyChangedCallback)handlers[i];
                    }
                    headHandler += _propertyChangedCallback;
                    _propertyChangedCallback = headHandler;
                }
            }

            _coerceValueCallback ??= baseMetadata.CoerceValueCallback;
            
            GetCSSEquivalent ??= baseMetadata.GetCSSEquivalent;
            GetCSSEquivalents ??= baseMetadata.GetCSSEquivalents;
            _methodToUpdateDom ??= baseMetadata.MethodToUpdateDom;
            _methodToUpdateDom2 ??= baseMetadata.MethodToUpdateDom2;
        }

        internal void InvokeMerge(PropertyMetadata baseMetadata, DependencyProperty dp)
        {
            Merge(baseMetadata, dp);
        }

        /// <summary>
        /// Called when this metadata has been applied to a property, which indicates that
        /// the metadata is being sealed.
        /// </summary>
        /// <param name="dp">
        /// The dependency property to which the metadata has been applied.
        /// </param>
        /// <param name="targetType">
        /// The type associated with this metadata if this is type-specific metadata. If
        /// this is default metadata, this value is a null reference.
        /// </param>
        internal virtual void OnApply(DependencyProperty dp, Type targetType)
        {
        }

        /// <summary>
        /// Gets a value that determines whether the metadata has been applied to a property
        /// in some way, resulting in the immutable state of that metadata instance.
        /// </summary>
        /// <returns>
        /// true if the metadata instance is immutable; otherwise, false.
        /// </returns>
        protected bool IsSealed
        {
            get { return Sealed; }
        }

        internal void Seal(DependencyProperty dp, Type targetType)
        {
            // CALLBACK
            OnApply(dp, targetType);

            Sealed = true;
        }

        internal bool IsDefaultValueModified { get { return IsModified(MetadataFlags.DefaultValueModifiedID); } }

        internal bool IsInherited
        {
            get { return (MetadataFlags.Inherited & _flags) != 0; ; }
            set
            {
                if (value)
                {
                    _flags |= MetadataFlags.Inherited;
                }
                else
                {
                    _flags &= (~MetadataFlags.Inherited);
                }
            }
        }

        private object _defaultValue = DependencyProperty.UnsetValue;
        private PropertyChangedCallback _propertyChangedCallback;
        private CoerceValueCallback _coerceValueCallback;

        // Enhancement idea: DefaultValueFactory bit
        //    If a bit opens up in MetadataFlags we should
        //    track the factory with a bit rather than casting
        //    every time.
        internal enum MetadataFlags : uint
        {
            DefaultValueModifiedID = 0x00000001,
            SealedID = 0x00000002,
            // Unused                                    = 0x00000004,
            // Unused                                    = 0x00000008,
            Inherited = 0x00000010,
            // Unused                                    = 0x00000020,
            FW_AffectsMeasureID = 0x00000040,
            FW_AffectsArrangeID = 0x00000080,
            FW_AffectsParentMeasureID = 0x00000100,
            FW_AffectsParentArrangeID = 0x00000200,
            FW_AffectsRenderID = 0x00000400,
            // Unused                                    = 0x00000800,
            // Unused                                    = 0x00001000,
            // Unused                                    = 0x00002000,
            // Unused                                    = 0x00004000,
            // Unused                                    = 0x00008000,
            // Unused                                    = 0x00010000,
            // Unused                                    = 0x00020000,
            // Unused                                    = 0x00040000,
            // Unused                                    = 0x00080000,
            FW_InheritsModifiedID = 0x00100000,
            // Unused                                    = 0x00200000,
            // Unused                                    = 0x00400000,
            // Unused                                    = 0x00800000,
            // Unused                                    = 0x01000000,
            // Unused                                    = 0x02000000,
            // Unused                                    = 0x04000000,
            FW_ReadOnlyID = 0x08000000,
            // Unused                                    = 0x10000000,
            // Unused                                    = 0x20000000,
            // Unused                                    = 0x40000000,
            // Unused                                    = 0x80000000,
        }


        // PropertyMetadata and FrameworkPropertyMetadata.
        internal MetadataFlags _flags;

        private void SetModified(MetadataFlags id) { _flags |= id; }
        private bool IsModified(MetadataFlags id) { return (id & _flags) != 0; }

        /// <summary>
        ///     Write a flag value
        /// </summary>
        internal void WriteFlag(MetadataFlags id, bool value)
        {
            if (value)
            {
                _flags |= id;
            }
            else
            {
                _flags &= (~id);
            }
        }

        /// <summary>
        ///     Read a flag value
        /// </summary>
        internal bool ReadFlag(MetadataFlags id) { return (id & _flags) != 0; }

        internal bool Sealed
        {
            get { return ReadFlag(MetadataFlags.SealedID); }
            set { WriteFlag(MetadataFlags.SealedID, value); }
        }

        private protected void CheckSealed()
        {
            if (Sealed)
            {
                throw new InvalidOperationException("Cannot change property metadata after it has been associated with a property.");
            }
        }

        // We use this uncommon field to stash values created by our default value factory
        // in the owner's _localStore.
        private static readonly ConditionalWeakTable<DependencyObject, FrugalMapBase> _defaultValueFactoryCache = new();

        /// <summary>
        /// Gets or sets a value that indicates whether the value of the dependency property
        /// is inheritable.
        /// </summary>
        /// <returns>
        /// true if the property value is inheritable; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The metadata has already been applied to a dependency property operation, so
        /// that metadata is sealed and properties of the metadata cannot be set.
        /// </exception>
        public bool Inherits
        {
            get { return IsInherited; }
            set
            {
                CheckSealed();
                
                IsInherited = value;
                SetModified(MetadataFlags.FW_InheritsModifiedID);
            }
        }

        /// <summary>
        /// Determines if the callback method should be called when the element is added to the visual tree.
        /// </summary>
        public WhenToCallPropertyChangedEnum CallPropertyChangedWhenLoadedIntoVisualTree
        {
            get { return _whenToCallProperty; }
            set { CheckSealed(); _whenToCallProperty = value; }
        }

        /// <summary>
        /// Gets the method that is called when the object is added to the visual tree, and
        /// when the property value changes while the object is already in the visual tree.
        /// </summary>
        public MethodToUpdateDom MethodToUpdateDom
        {
            get { return _methodToUpdateDom; }
            set { CheckSealed(); _methodToUpdateDom = value; }
        }

        internal MethodToUpdateDom2 MethodToUpdateDom2
        {
            get { return _methodToUpdateDom2; }
            set { CheckSealed(); _methodToUpdateDom2 = value; }
        }

        private WhenToCallPropertyChangedEnum _whenToCallProperty;
        private MethodToUpdateDom _methodToUpdateDom;
        private MethodToUpdateDom2 _methodToUpdateDom2;
        internal CSSEquivalentGetter GetCSSEquivalent;
        internal CSSEquivalentsGetter GetCSSEquivalents;
    }

    /// <summary>
    /// GetReadOnlyValueCallback -- a very specialized callback that allows storage for read-only properties
    /// to be managed outside of the effective value store on a DO.  This optimization is restricted to read-only
    /// properties because read-only properties can only have a value explicitly set by the keeper of the key, so
    /// it eliminates the possibility of a self-managed store missing modifiers such as expressions, coercion,
    /// and animation.
    /// </summary>
    internal delegate object GetReadOnlyValueCallback(DependencyObject d);

    internal delegate CSSEquivalent CSSEquivalentGetter(DependencyObject d);
    internal delegate List<CSSEquivalent> CSSEquivalentsGetter(DependencyObject d);
    public delegate void MethodToUpdateDom(DependencyObject d, object newValue);
    internal delegate void MethodToUpdateDom2(DependencyObject d, object oldValue, object newValue);
}
