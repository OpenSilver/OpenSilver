// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using OpenSilver.Internal;
using OpenSilver.Utility;

namespace System.Windows
{
    /// <summary>
    /// Represents a dependency property that is registered with the Silverlight 
    /// dependency property system. Dependency properties provide support for value 
    /// expressions, data binding, animation, and property change notification.
    /// </summary>
    public class DependencyProperty
    {
        /// <summary>
        /// Registers a dependency property with the specified property name, property type,
        /// owner type, and property metadata for the property.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="typeMetadata">
        /// A property metadata instance. This can contain a <see cref="PropertyChangedCallback"/>
        /// implementation reference.
        /// </param>
        /// <returns>
        /// A dependency property identifier that should be used to set the value of a public
        /// static readonly field in your class. The identifier is then used both by your
        /// own code and any third-party user code to reference the dependency property later,
        /// for operations such as setting its value programmatically, or attaching a <see cref="Binding"/>
        /// in code.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// A required parameter was null (check the exception for the name of the missing parameter).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A parameter was out of range, for instance name was an empty string.-or-Attempted
        /// to register with a propertyType that does not match a default value specified
        /// in the typeMetadata.
        /// </exception>
        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            // Forwarding
            return Register(name, propertyType, ownerType, typeMetadata, null);
        }

        /// <summary>
        /// Register a Dependency Property
        /// </summary>
        /// <param name="name">Name of property</param>
        /// <param name="propertyType">Type of the property</param>
        /// <param name="ownerType">Type that is registering the property</param>
        /// <param name="typeMetadata">Metadata to use if current type doesn't specify type-specific metadata</param>
        /// <param name="validateValueCallback">Provides additional value validation outside automatic type validation</param>
        /// <returns>Dependency Property</returns>
        internal static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            RegisterParameterValidation(name, propertyType, ownerType);

            // Register an attached property
            PropertyMetadata defaultMetadata = null;
            if (typeMetadata != null && typeMetadata.DefaultValueWasSet())
            {
                defaultMetadata = new PropertyMetadata(typeMetadata.DefaultValue);
            }

            DependencyProperty property = RegisterCommon(name, propertyType, ownerType, defaultMetadata, validateValueCallback);

            if (typeMetadata != null)
            {
                // Apply type-specific metadata to owner type only
                property.OverrideMetadata(ownerType, typeMetadata);
            }

            return property;
        }

        /// <summary>
        /// Registers a read-only dependency property, with the specified property type,
        /// owner type, and property metadata.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="typeMetadata">
        /// Property metadata for the dependency property.
        /// </param>
        /// <returns>
        /// A dependency property key that should be used to set the value of a static read-only
        /// field in your class, which is then used to reference the dependency property.
        /// </returns>
        internal static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return RegisterReadOnly(name, propertyType, ownerType, typeMetadata, null);
        }

        /// <summary>
        /// Simple registration, metadata, validation, and a read-only property
        /// key.  Calling this version restricts the property such that it can
        /// only be set via the corresponding overload of DependencyObject.SetValue.
        /// </summary>
        internal static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            RegisterParameterValidation(name, propertyType, ownerType);

            PropertyMetadata defaultMetadata = null;

            if (typeMetadata != null && typeMetadata.DefaultValueWasSet())
            {
                defaultMetadata = new PropertyMetadata(typeMetadata.DefaultValue);
            }
            else
            {
                defaultMetadata = AutoGeneratePropertyMetadata(propertyType, validateValueCallback, name, ownerType);
            }

            //  We create a DependencyPropertyKey at this point with a null property
            // and set that in the _readOnlyKey field.  This is so the property is
            // marked as requiring a key immediately.  If something fails in the
            // initialization path, the property is still marked as needing a key.
            //  This is better than the alternative of creating and setting the key
            // later, because if that code fails the read-only property would not
            // be marked read-only.  The intent of this mildly convoluted code
            // is so we fail securely.
            DependencyPropertyKey authorizationKey = new DependencyPropertyKey(null); // No property yet, use null as placeholder.

            DependencyProperty property = RegisterCommon(name, propertyType, ownerType, defaultMetadata, validateValueCallback);

            property._readOnlyKey = authorizationKey;

            authorizationKey.SetDependencyProperty(property);

            if (typeMetadata == null)
            {
                // No metadata specified, generate one so we can specify the authorized key.
                typeMetadata = AutoGeneratePropertyMetadata(propertyType, validateValueCallback, name, ownerType);
            }

            // Authorize registering type for read-only access, create key.

            // Apply type-specific metadata to owner type only
            property.OverrideMetadata(ownerType, typeMetadata, authorizationKey);

            return authorizationKey;
        }

        /// <summary>
        /// Registers a read-only attached property, with the specified property type, owner
        /// type, and property metadata.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="defaultMetadata">
        /// Property metadata for the dependency property.
        /// </param>
        /// <returns>
        /// A dependency property key that should be used to set the value of a static read-only
        /// field in your class, which is then used to reference the dependency property later.
        /// </returns>
        internal static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            return RegisterAttachedReadOnly(name, propertyType, ownerType, defaultMetadata, null);
        }

        /// <summary>
        /// Registers a read-only attached property, with the specified property type, owner
        /// type, property metadata, and a validation callback.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="defaultMetadata">
        /// Property metadata for the dependency property.
        /// </param>
        /// <param name="validateValueCallback">
        /// A reference to a user-created callback that should perform any custom validation
        /// of the dependency property value beyond typical type validation.
        /// </param>
        /// <returns>
        /// A dependency property key that should be used to set the value of a static read-only
        /// field in your class, which is then used to reference the dependency property.
        /// </returns>
        internal static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            RegisterParameterValidation(name, propertyType, ownerType);

            // Establish default metadata for all types, if none is provided
            if (defaultMetadata == null)
            {
                defaultMetadata = AutoGeneratePropertyMetadata(propertyType, validateValueCallback, name, ownerType);
            }

            //  We create a DependencyPropertyKey at this point with a null property
            // and set that in the _readOnlyKey field.  This is so the property is
            // marked as requiring a key immediately.  If something fails in the
            // initialization path, the property is still marked as needing a key.
            //  This is better than the alternative of creating and setting the key
            // later, because if that code fails the read-only property would not
            // be marked read-only.  The intent of this mildly convoluted code
            // is so we fail securely.
            DependencyPropertyKey authorizedKey = new DependencyPropertyKey(null);

            DependencyProperty property = RegisterCommon(name, propertyType, ownerType, defaultMetadata, validateValueCallback);
            
            property.IsAttached = true;

            property._readOnlyKey = authorizedKey;

            authorizedKey.SetDependencyProperty(property);

            return authorizedKey;
        }

        /// <summary>
        /// Registers an attached dependency property with the specified property name, property
        /// type, owner type, and property metadata for the property.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="defaultMetadata">
        /// A property metadata instance. This can contain a <see cref="PropertyChangedCallback"/>
        /// implementation reference.
        /// </param>
        /// <returns>
        /// A dependency property identifier that should be used to set the value of a public
        /// static readonly field in your class. That identifier is then used to reference
        /// the attached property later, for operations such as setting its value programmatically,
        /// or attaching a <see cref="Binding"/>.
        /// </returns>
        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata)
        {
            // Forwarding
            return RegisterAttached(name, propertyType, ownerType, defaultMetadata, null);
        }

        /// <summary>
        /// Registers an attached property with the specified property type, owner type,
        /// property metadata, and value validation callback for the property.
        /// </summary>
        /// <param name="name">
        /// The name of the dependency property to register.
        /// </param>
        /// <param name="propertyType">
        /// The type of the property.
        /// </param>
        /// <param name="ownerType">
        /// The owner type that is registering the dependency property.
        /// </param>
        /// <param name="defaultMetadata">
        /// Property metadata for the dependency property. This can include the default value
        /// as well as other characteristics.
        /// </param>
        /// <param name="validateValueCallback">
        /// A reference to a callback that should perform any custom validation of the dependency
        /// property value beyond typical type validation.
        /// </param>
        /// <returns>
        /// A dependency property identifier that should be used to set the value of a public
        /// static readonly field in your class. That identifier is then used to reference
        /// the dependency property later, for operations such as setting its value programmatically
        /// or obtaining metadata.
        /// </returns>
        internal static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            RegisterParameterValidation(name, propertyType, ownerType);

            DependencyProperty dp = RegisterCommon(name, propertyType, ownerType, defaultMetadata, validateValueCallback);

            dp.IsAttached = true;

            return dp;
        }

        private static void RegisterParameterValidation(
            string name,
            Type propertyType,
            Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Parameter cannot be a zero-length string.");
            }

            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }
        }

        private static DependencyProperty RegisterCommon(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            FromNameKey key = new FromNameKey(name, ownerType);

            // Establish default metadata for all types, if none is provided
            if (defaultMetadata == null)
            {
                defaultMetadata = AutoGeneratePropertyMetadata(propertyType, validateValueCallback, name, ownerType);
            }
            else // Metadata object is provided.
            {
                // If the defaultValue wasn't specified auto generate one
                if (!defaultMetadata.DefaultValueWasSet())
                {
                    defaultMetadata.DefaultValue = AutoGenerateDefaultValue(propertyType);
                }

                ValidateMetadataDefaultValue(defaultMetadata, propertyType, name, validateValueCallback);
            }

            // Create property
            DependencyProperty dp = new DependencyProperty(name, propertyType, ownerType, defaultMetadata, validateValueCallback);

            // Seal (null means being used for default metadata, calls OnApply)
            defaultMetadata.Seal(dp, null);

            if (defaultMetadata.IsInherited)
            {
                dp._packedData |= Flags.IsPotentiallyInherited;
            }

            if (defaultMetadata.UsingDefaultValueFactory)
            {
                dp._packedData |= Flags.IsPotentiallyUsingDefaultValueFactory;
            }

            // Map owner type to this property
            // Build key
            lock (Synchronized)
            {
                PropertyFromName[key] = dp;
            }

            return dp;
        }

        private static object AutoGenerateDefaultValue(Type propertyType)
        {
            // Default per-type metadata not provided, create
            object defaultValue = null;

            // Auto-assigned default value
            if (propertyType.IsValueType)
            {
                // Value-types have default-constructed type default values
                defaultValue = Activator.CreateInstance(propertyType);
            }

            return defaultValue;
        }

        private static PropertyMetadata AutoGeneratePropertyMetadata(
            Type propertyType,
            ValidateValueCallback validateValueCallback,
            string name,
            Type ownerType)
        {
            // Default per-type metadata not provided, create
            object defaultValue = AutoGenerateDefaultValue(propertyType);

            // If a validator is passed in, see if the default value makes sense.
            if (validateValueCallback != null &&
                !validateValueCallback(defaultValue))
            {
                // Didn't work - require the caller to specify one.
                throw new ArgumentException(
                    string.Format("Cannot automatically generate a valid default value for property '{0}'. Specify a default value explicitly when owner type '{1}' is registering this DependencyProperty.", name, ownerType.Name));
            }

            return new PropertyMetadata(defaultValue);
        }

        // Validate the default value in the given metadata
        private static void ValidateMetadataDefaultValue(
            PropertyMetadata defaultMetadata,
            Type propertyType,
            string propertyName,
            ValidateValueCallback validateValueCallback)
        {
            // If we are registered to use the DefaultValue factory we can
            // not validate the DefaultValue at registration time, so we
            // early exit.
            if (defaultMetadata.UsingDefaultValueFactory)
            {
                return;
            }

            ValidateDefaultValueCommon(defaultMetadata.DefaultValue, propertyType, propertyName, validateValueCallback);
        }

        // Validate the given default value, used by PropertyMetadata.GetDefaultValue()
        // when the DefaultValue factory is used.
        // These default values are allowed to have thread-affinity.
        internal void ValidateFactoryDefaultValue(object defaultValue)
        {
            ValidateDefaultValueCommon(defaultValue, PropertyType, Name, ValidateValueCallback);
        }

        private static void ValidateDefaultValueCommon(
            object defaultValue,
            Type propertyType,
            string propertyName,
            ValidateValueCallback validateValueCallback)
        {
            // Ensure default value is the correct type
            if (!IsValidType(defaultValue, propertyType))
            {
                throw new ArgumentException(
                    string.Format("Default value type does not match type of property '{0}'.", propertyName));
            }

            // An Expression used as default value won't behave as expected since
            //  it doesn't get evaluated.  We explicitly fail it here.
            if (defaultValue is BindingExpression)
            {
                throw new ArgumentException("A BindingExpression object is not a valid default value for a DependencyProperty.");
            }

            // After checking for correct type, check default value against
            //  validator (when one is given)
            if (validateValueCallback != null &&
                !validateValueCallback(defaultValue))
            {
                throw new ArgumentException(
                    string.Format("Default value for '{0}' property is not valid because ValidateValueCallback failed.", propertyName));
            }
        }

        /// <summary>
        /// Parameter validation for OverrideMetadata, includes code to force
        /// all base classes of "forType" to register their metadata so we know
        /// what we are overriding.
        /// </summary>
        private void SetupOverrideMetadata(
            Type forType,
            PropertyMetadata typeMetadata,
            out DependencyObjectType dType,
            out PropertyMetadata baseMetadata)
        {
            if (forType == null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            if (typeMetadata == null)
            {
                throw new ArgumentNullException(nameof(typeMetadata));
            }

            if (typeMetadata.Sealed)
            {
                throw new ArgumentException("Metadata is already associated with a type and property. A new one must be created.");
            }

            if (!typeof(DependencyObject).IsAssignableFrom(forType))
            {
                throw new ArgumentException(string.Format("'{0}' type must derive from DependencyObject.", forType.Name));
            }

            // Ensure default value is a correct value (if it was supplied,
            // otherwise, the default value will be taken from the base metadata
            // which was already validated)
            if (typeMetadata.IsDefaultValueModified)
            {
                // Will throw ArgumentException if fails.
                ValidateMetadataDefaultValue(typeMetadata, PropertyType, Name, ValidateValueCallback);
            }

            // Force all base classes to register their metadata
            dType = DependencyObjectType.FromSystemType(forType);

            // Get metadata for the base type
            baseMetadata = GetMetadata(dType.BaseType);

            // Make sure overriding metadata is the same type or derived type of
            // the base metadata
            if (!baseMetadata.GetType().IsAssignableFrom(typeMetadata.GetType()))
            {
                throw new ArgumentException("Metadata override and base metadata must be of the same type or derived type.");
            }
        }

        /// <summary>
        /// Specifies alternate metadata for this dependency property when it is present
        /// on instances of a specified type, overriding the metadata that existed for the
        /// dependency property as it was inherited from base types.
        /// </summary>
        /// <param name="forType">
        /// The type where this dependency property is inherited and where the provided alternate
        /// metadata will be applied.
        /// </param>
        /// <param name="typeMetadata">
        /// The metadata to apply to the dependency property on the overriding type.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// An attempt was made to override metadata on a read-only dependency property (that
        /// operation cannot be done using this signature).
        /// </exception>
        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            SetupOverrideMetadata(forType, typeMetadata, out DependencyObjectType dType, out PropertyMetadata baseMetadata);

            if (ReadOnly)
            {
                // Readonly and no DependencyPropertyKey - not allowed.
                throw new InvalidOperationException(
                    string.Format("'{0}' property was registered as read-only and its metadata cannot be overridden without an authorization key.", Name));
            }

            ProcessOverrideMetadata(forType, typeMetadata, dType, baseMetadata);
        }

        /// <summary>
        /// Supplies alternate metadata for a read-only dependency property when it is present
        /// on instances of a specified type, overriding the metadata that was provided in
        /// the initial dependency property registration. You must pass the <see cref="Windows.DependencyPropertyKey"/>
        /// for the read-only dependency property to avoid raising an exception.
        /// </summary>
        /// <param name="forType">
        /// The type where this dependency property is inherited and where the provided alternate
        /// metadata will be applied.
        /// </param>
        /// <param name="typeMetadata">
        /// The metadata to apply to the dependency property on the overriding type.
        /// </param>
        /// <param name="key">
        /// The access key for a read-only dependency property.
        /// </param>
        internal void OverrideMetadata(Type forType, PropertyMetadata typeMetadata, DependencyPropertyKey key)
        {
            SetupOverrideMetadata(forType, typeMetadata, out DependencyObjectType dType, out PropertyMetadata baseMetadata);

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (ReadOnly)
            {
                // If the property is read-only, the key must match this property
                //  and the key must match that in the base metadata.

                if (key.DependencyProperty != this)
                {
                    throw new ArgumentException(
                        string.Format("Property key is not authorized to override metadata of property '{0}'.", Name));
                }

                VerifyReadOnlyKey(key);
            }
            else
            {
                throw new InvalidOperationException(
                    "This method overrides metadata only on read-only properties. This property is not read-only.");
            }

            // Either the property doesn't require a key, or the key match was
            //  successful.  Proceed with the metadata override.
            ProcessOverrideMetadata(forType, typeMetadata, dType, baseMetadata);
        }

        /// <summary>
        /// After parameters have been validated for OverrideMetadata, this
        /// method is called to actually update the data structures.
        /// </summary>
        private void ProcessOverrideMetadata(
            Type forType,
            PropertyMetadata typeMetadata,
            DependencyObjectType dType,
            PropertyMetadata baseMetadata)
        {
            // Store per-Type metadata for this property. Locks only on Write.
            // Datastructure guaranteed to be valid for non-locking readers
            lock (Synchronized)
            {
                if (UnsetValue == _metadataMap[dType.Id])
                {
                    _metadataMap[dType.Id] = typeMetadata;
                }
                else
                {
                    throw new ArgumentException(string.Format("PropertyMetadata is already registered for type '{0}'.", forType.Name));
                }
            }

            // Merge base's metadata into this metadata
            // CALLBACK
            typeMetadata.InvokeMerge(baseMetadata, this);

            // Type metadata may no longer change (calls OnApply)
            typeMetadata.Seal(this, forType);

            if (typeMetadata.IsInherited)
            {
                _packedData |= Flags.IsPotentiallyInherited;
            }

            if (typeMetadata.DefaultValueWasSet() && (typeMetadata.DefaultValue != DefaultMetadata.DefaultValue))
            {
                _packedData |= Flags.IsDefaultValueChanged;
            }

            if (typeMetadata.UsingDefaultValueFactory)
            {
                _packedData |= Flags.IsPotentiallyUsingDefaultValueFactory;
            }
        }

        internal object GetDefaultValue(DependencyObject owner)
        {
            if (!IsDefaultValueChanged && !IsPotentiallyUsingDefaultValueFactory)
            {
                return DefaultMetadata.DefaultValue;
            }

            return GetMetadata(owner).GetDefaultValue(owner, this);
        }

        /// <summary>
        /// Retrieves the property metadata value for the dependency property as registered
        /// to the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="forType">
        /// The specific type from which to retrieve the dependency property metadata.
        /// </param>
        /// <returns>
        /// A property metadata object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="forType"/> is null.
        /// </exception>
        public PropertyMetadata GetMetadata(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException(nameof(forType));
            }
            return GetMetadata(DependencyObjectType.FromSystemType(forType));
        }

        /// <summary>
        /// Returns the metadata for this dependency property as it exists on the specified
        /// object instance.
        /// </summary>
        /// <param name="dependencyObject">
        /// A dependency object that is checked for type, to determine which type-specific
        /// version of the dependency property the metadata should come from.
        /// </param>
        /// <returns>
        /// A property metadata object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dependencyObject"/> is null.
        /// </exception>
        internal PropertyMetadata GetMetadata(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }
            return GetMetadata(dependencyObject.DependencyObjectType);
        }

        /// <summary>
        /// Returns the metadata for this dependency property as it exists on a specified
        /// type.
        /// </summary>
        /// <param name="dependencyObjectType">
        /// A specific object that records the dependency object type from which the dependency
        /// property metadata is desired.
        /// </param>
        /// <returns>
        /// A property metadata object.
        /// </returns>
        internal PropertyMetadata GetMetadata(DependencyObjectType dependencyObjectType)
        {
            // All static constructors for this DType and all base types have already
            // been run. If no overriden metadata was provided, then look up base types.
            // If no metadata found on base types, then return default

            if (null != dependencyObjectType)
            {
                // Do we in fact have any overrides at all?
                int index = _metadataMap.Count - 1;
                int Id;
                object value;

                if (index < 0)
                {
                    // No overrides or it's the base class
                    return DefaultMetadata;
                }
                else if (index == 0)
                {
                    // Only 1 override
                    _metadataMap.GetKeyValuePair(index, out Id, out value);

                    // If there is overriden metadata, then there is a base class with
                    // lower or equal Id of this class, or this class is already a base class
                    // of the overridden one. Therefore dependencyObjectType won't ever
                    // become null before we exit the while loop
                    while (dependencyObjectType.Id > Id)
                    {
                        dependencyObjectType = dependencyObjectType.BaseType;
                    }

                    if (Id == dependencyObjectType.Id)
                    {
                        // Return the override
                        return (PropertyMetadata)value;
                    }
                    // Return default metadata
                }
                else
                {
                    // We have more than 1 override for this class, so we will have to loop through
                    // both the overrides and the class Id
                    if (0 != dependencyObjectType.Id)
                    {
                        do
                        {
                            // Get the Id of the most derived class with overridden metadata
                            _metadataMap.GetKeyValuePair(index, out Id, out value);
                            --index;

                            // If the Id of this class is less than the override, then look for an override
                            // with an equal or lower Id until we run out of overrides
                            while ((dependencyObjectType.Id < Id) && (index >= 0))
                            {
                                _metadataMap.GetKeyValuePair(index, out Id, out value);
                                --index;
                            }

                            // If there is overriden metadata, then there is a base class with
                            // lower or equal Id of this class, or this class is already a base class
                            // of the overridden one. Therefore dependencyObjectType won't ever
                            // become null before we exit the while loop
                            while (dependencyObjectType.Id > Id)
                            {
                                dependencyObjectType = dependencyObjectType.BaseType;
                            }

                            if (Id == dependencyObjectType.Id)
                            {
                                // Return the override
                                return (PropertyMetadata)value;
                            }
                        }
                        while (index >= 0);
                    }
                }
            }
            return DefaultMetadata;
        }

        /// <summary>
        /// Adds another type as an owner of a dependency property that has already been
        /// registered.
        /// </summary>
        /// <param name="ownerType">
        /// The type to add as an owner of this dependency property.
        /// </param>
        /// <returns>
        /// A reference to the original <see cref="DependencyProperty"/> identifier that
        /// identifies the dependency property. This identifier should be exposed by the
        /// adding class as a public static readonly field.
        /// </returns>
        public DependencyProperty AddOwner(Type ownerType)
        {
            // Forwarding
            return AddOwner(ownerType, null);
        }

        /// <summary>
        /// Adds another type as an owner of a dependency property that has already been
        /// registered, providing dependency property metadata for the dependency property
        /// as it will exist on the provided owner type.
        /// </summary>
        /// <param name="ownerType">
        /// The type to add as owner of this dependency property.
        /// </param>
        /// <param name="typeMetadata">
        /// The metadata that qualifies the dependency property as it exists on the provided
        /// type.
        /// </param>
        /// <returns>
        /// A reference to the original <see cref="DependencyProperty"/> identifier that
        /// identifies the dependency property. This identifier should be exposed by the
        /// adding class as a public static readonly field.
        /// </returns>
        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata)
        {
            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            // Map owner type to this property
            // Build key
            FromNameKey key = new FromNameKey(Name, ownerType);

            lock (Synchronized)
            {
                if (PropertyFromName.ContainsKey(key))
                {
                    throw new ArgumentException(
                        string.Format("'{0}' property was already registered by '{1}'.", Name, ownerType.Name));
                }
            }

            if (typeMetadata != null)
            {
                OverrideMetadata(ownerType, typeMetadata);
            }

            lock (Synchronized)
            {
                PropertyFromName[key] = this;
            }

            return this;
        }
        
        /// <summary>
        /// Gets the name of the <see cref="DependencyProperty"/>.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Gets the type that the <see cref="DependencyProperty"/> uses for its value.
        /// </summary>
        internal Type PropertyType { get; }

        /// <summary>
        /// Gets the type of the object that registered the <see cref="DependencyProperty"/> with the
        /// property system, or added itself as owner of the property.
        /// </summary>
        internal Type OwnerType { get; }

        /// <summary>
        /// Gets the default metadata of the <see cref="DependencyProperty"/>.
        /// </summary>
        internal PropertyMetadata DefaultMetadata { get; }

        /// <summary>
        /// Gets the value validation callback for the <see cref="DependencyProperty"/>.
        /// </summary>
        internal ValidateValueCallback ValidateValueCallback { get; }

        /// <summary>
        /// Gets an internally generated value that uniquely identifies the <see cref="DependencyProperty"/>.
        /// </summary>
        internal int GlobalIndex { get; }

        public bool IsAttached
        {
            get { return (_packedData & Flags.IsAttached) != 0; }
            private set { _packedData |= Flags.IsAttached; }
        }

        internal bool IsObjectType
        {
            get { return (_packedData & Flags.IsObjectType) != 0; }
        }

        internal bool IsValueType
        {
            get { return (_packedData & Flags.IsValueType) != 0; }
        }

        internal bool IsStringType
        {
            get { return (_packedData & Flags.IsStringType) != 0; }
        }

        internal bool IsPotentiallyInherited
        {
            get { return (_packedData & Flags.IsPotentiallyInherited) != 0; }
        }

        internal bool IsDefaultValueChanged
        {
            get { return (_packedData & Flags.IsDefaultValueChanged) != 0; }
        }

        internal bool IsPotentiallyUsingDefaultValueFactory
        {
            get { return (_packedData & Flags.IsPotentiallyUsingDefaultValueFactory) != 0; }
        }

        /// <summary>
        /// Determines whether a specified value is acceptable for this dependency property's
        /// type, as checked against the property type provided in the original dependency
        /// property registration.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// true if the specified value is the registered property type or an acceptable
        /// derived type; otherwise, false.
        /// </returns>
        internal bool IsValidType(object value)
        {
            return IsValidType(value, PropertyType);
        }

        /// <summary>
        /// Determines whether the provided value is accepted for the type of property through
        /// basic type checking, and also potentially if it is within the allowed range of
        /// values for that type.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// true if the value is acceptable and is of the correct type or a derived type;
        /// otherwise, false.
        /// </returns>
        internal bool IsValidValue(object value)
        {
            if (!IsValidType(value, PropertyType))
            {
                return false;
            }

            if (ValidateValueCallback != null)
            {
                // CALLBACK
                return ValidateValueCallback(value);
            }

            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the dependency property identified by this
        /// <see cref="DependencyProperty"/> instance is a read-only dependency property.
        /// </summary>
        /// <returns>
        /// true if the dependency property is read-only; otherwise, false.
        /// </returns>
        internal bool ReadOnly
        {
            get
            {
                return (_readOnlyKey != null);
            }
        }

        /// <summary>
        /// Returns the DependencyPropertyKey associated with this DP.
        /// </summary>
        internal DependencyPropertyKey DependencyPropertyKey
        {
            get
            {
                return _readOnlyKey;
            }
        }

        internal void VerifyReadOnlyKey(DependencyPropertyKey candidateKey)
        {
            Debug.Assert(ReadOnly, "Why are we trying to validate read-only key on a property that is not read-only?");

            if (_readOnlyKey != candidateKey)
            {
                throw new ArgumentException(string.Format("Property key is not authorized to modify property '{0}'.", Name));
            }
        }

        /// <summary>
        /// Internal version of IsValidValue that bypasses IsValidType check;
        /// Called from SetValueInternal
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>true if value is appropriate</returns>
        internal bool IsValidValueInternal(object value)
        {
            if (ValidateValueCallback != null)
            {
                // CALLBACK
                return ValidateValueCallback(value);
            }

            return true;
        }

        /// <summary>
        /// Find a property from name
        /// </summary>
        /// <remarks>
        /// Search includes base classes of the provided type as well
        /// </remarks>
        /// <param name="name">Name of the property</param>
        /// <param name="ownerType">Owner type of the property</param>
        /// <returns>Dependency property</returns>
        internal static DependencyProperty FromName(string name, Type ownerType)
        {
            DependencyProperty dp = null;

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            FromNameKey key = new FromNameKey(name, ownerType);

            while ((dp == null) && (ownerType != null))
            {
                // Ensure static constructor of type has run
                RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);

                // Locate property
                key.UpdateNameKey(ownerType);

                lock (Synchronized)
                {
                    PropertyFromName.TryGetValue(key, out dp);
                }

                ownerType = ownerType.BaseType;
            }

            return dp;
        }

        /// <summary>
        /// Returns the string representation of the <see cref="DependencyProperty"/>.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        internal static bool IsValidType(object value, Type propertyType)
        {
            if (value == null)
            {
                // Null values are invalid for value-types
                if (propertyType.IsValueType &&
                    !(propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == NullableType))
                {
                    return false;
                }
            }
            else
            {
                // Non-null default value, ensure its the correct type
                if (!propertyType.IsInstanceOfType(value))
                {
                    return false;
                }
            }

            return true;
        }

        private sealed class FromNameKey
        {
            public FromNameKey(string name, Type ownerType)
            {
                _name = name;
                _ownerType = ownerType;

                _hashCode = _name.GetHashCode() ^ _ownerType.GetHashCode();
            }

            public void UpdateNameKey(Type ownerType)
            {
                _ownerType = ownerType;

                _hashCode = _name.GetHashCode() ^ _ownerType.GetHashCode();
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object o)
            {
                if (o is FromNameKey key)
                {
                    return Equals(key);
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(FromNameKey key)
            {
                return (_name.Equals(key._name) && (_ownerType == key._ownerType));
            }

            private readonly string _name;
            private Type _ownerType;

            private int _hashCode;
        }

        private DependencyProperty(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata defaultMetadata,
            ValidateValueCallback validateValueCallback)
        {
            Name = name;
            PropertyType = propertyType;
            OwnerType = ownerType;
            DefaultMetadata = defaultMetadata;
            ValidateValueCallback = validateValueCallback;

            lock (Synchronized)
            {
                GlobalIndex = GetUniqueGlobalIndex();

                RegisteredPropertyList.Add(this);
            }

            Flags packedData = 0;
            if (propertyType.IsValueType)
            {
                packedData |= Flags.IsValueType;
            }

            if (propertyType == typeof(object))
            {
                packedData |= Flags.IsObjectType;
            }

            if (propertyType == typeof(string))
            {
                packedData |= Flags.IsStringType;
            }

            _packedData = packedData;
        }

        // Synchronized: Covered by DependencyProperty.Synchronized
        private static int GetUniqueGlobalIndex()
        {
            // Covered by Synchronized by caller
            return GlobalIndexCount++;
        }

        /// <summary>
        /// Specifies a static value that is used by the property system rather than null
        /// to indicate that the property exists, but does not have its value set by the
        /// property system.
        /// </summary>
        /// <returns>
        /// The sentinel value for an unset value.
        /// </returns>
        public static readonly object UnsetValue = new NamedObject("DependencyProperty.UnsetValue");

        private DependencyPropertyKey _readOnlyKey;

        [Flags]
        private enum Flags : int
        {
            IsValueType = 0x01,
            IsStringType = 0x02,
            IsPotentiallyInherited = 0x04,
            IsDefaultValueChanged = 0x08,
            IsPotentiallyUsingDefaultValueFactory = 0x10,
            IsObjectType = 0x20,
            IsAttached = 0x40,
        }

        private Flags _packedData;

        // Synchronized (write locks, lock-free reads): Covered by DependencyProperty instance
        // This is a map that contains the IDs of derived classes that have overriden metadata
        /* property */
        private InsertionSortMap _metadataMap = new InsertionSortMap();

        // Synchronized (write locks, lock-free reads): Covered by DependencyProperty.Synchronized
        /* property */
        internal static readonly List<DependencyProperty> RegisteredPropertyList = new List<DependencyProperty>(768);

        // Synchronized: Covered by DependencyProperty.Synchronized
        private static readonly Dictionary<FromNameKey, DependencyProperty> PropertyFromName = new Dictionary<FromNameKey, DependencyProperty>();

        // Synchronized: Covered by DependencyProperty.Synchronized
        private static int GlobalIndexCount;

        // Global, cross-object synchronization
        private static object Synchronized = new object();

        // Nullable Type
        private static readonly Type NullableType = typeof(Nullable<>);

        /// <summary>
        ///     Returns the number of all registered properties.
        /// </summary>
        internal static int RegisteredPropertyCount
        {
            get
            {
                return RegisteredPropertyList.Count;
            }
        }

        /// <summary>
        ///     Returns an enumeration of properties that are
        ///     currently registered.
        ///     Synchronized (write locks, lock-free reads): Covered by DependencyProperty.Synchronized
        /// </summary>
        internal static IEnumerable RegisteredProperties
        {
            get
            {
                foreach (DependencyProperty dp in RegisteredPropertyList)
                {
                    if (dp != null)
                    {
                        yield return dp;
                    }
                }
            }
        }
    }
}
