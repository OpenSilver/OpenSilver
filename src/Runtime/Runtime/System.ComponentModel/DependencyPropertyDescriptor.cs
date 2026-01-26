
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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using OpenSilver.Internal.ComponentModel;

namespace System.ComponentModel;

/// <summary>
/// Provides an extension of <see cref="PropertyDescriptor"/> that accounts for the additional property 
/// characteristics of a dependency property.
/// </summary>
public sealed class DependencyPropertyDescriptor : PropertyDescriptor
{
    /// <summary>
    ///     Creates a new dependency property descriptor.  A note on perf:  We don't 
    ///     pass the property descriptor down as the default member descriptor here.  Doing
    ///     so gets the attributes off of the property descriptor, which can be costly if they
    ///     haven't been accessed yet.  Instead, we wait until someone needs to access our
    ///     Attributes property and demand create the attributes at that time.
    /// </summary>
    private DependencyPropertyDescriptor(PropertyDescriptor property, string name, Type componentType, DependencyProperty dp, bool isAttached)
        : base(name, null)
    {
        Debug.Assert(property != null || !isAttached, "Demand-load of property descriptor is only supported for direct properties");

        _property = property;
        ComponentType = componentType;
        DependencyProperty = dp;
        IsAttached = isAttached;
        Metadata = dp.GetMetadata(componentType);
    }

    /// <summary>
    /// Returns a <see cref="DependencyPropertyDescriptor"/> for a provided <see cref="PropertyDescriptor"/>.
    /// </summary>
    /// <param name="property">
    /// The <see cref="PropertyDescriptor"/> to check.
    /// </param>
    /// <returns>
    /// If the property described by property is a dependency property, returns a valid <see cref="DependencyPropertyDescriptor"/>.
    /// Otherwise, returns a <see langword="null"/> <see cref="DependencyPropertyDescriptor"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="property"/> is null.
    /// </exception>
    public static DependencyPropertyDescriptor FromProperty(PropertyDescriptor property)
    {
        ArgumentNullException.ThrowIfNull(property);

        DependencyPropertyDescriptor dpd;
        bool found;

        lock (_cache)
        {
            found = _cache.TryGetValue(property, out dpd);
        }

        if (found)
        {
            return dpd;
        }

        // Locate the dependency property.  We do this a fast way
        // by searching for InternalPropertyDescriptor, and a slow
        // way, by looking for an attribute.  The fast way works unless
        // someone has added another layer of metadata overrides to
        // TypeDescriptor.

        DependencyProperty dp = null;
        bool isAttached = false;

        if (property is DependencyObjectPropertyDescriptor idpd)
        {
            dp = idpd.DependencyProperty;
            isAttached = idpd.IsAttached;
        }
        else
        {
            if (property.Attributes[typeof(DependencyPropertyAttribute)] is DependencyPropertyAttribute dpa)
            {
                dp = dpa.DependencyProperty;
                isAttached = dpa.IsAttached;
            }
        }

        if (dp != null)
        {
            dpd = new DependencyPropertyDescriptor(property, property.Name, property.ComponentType, dp, isAttached);

            lock (_cache)
            {
                _cache[property] = dpd;
            }
        }

        return dpd;
    }


    /// <summary>
    ///     Static method that returns a DependencyPropertyDescriptor from a DependencyProperty.  The
    ///     DependencyProperty may refer to either a direct or attached property.  The targetType is the
    ///     type of object to associate with the property:  either the owner type for a direct property
    ///     or the type of object to attach to for an attached property.
    /// </summary>
    internal static DependencyPropertyDescriptor FromProperty(DependencyProperty dependencyProperty, Type ownerType, Type targetType, bool ignorePropertyType)
    {
        ArgumentNullException.ThrowIfNull(dependencyProperty);
        ArgumentNullException.ThrowIfNull(targetType);

        // We have a different codepath here for attached and direct
        // properties.  For direct properties, we route through Type
        // Descriptor because we need the underlying CLR property descriptor
        // to create our wrapped property.  For attached properties, all we
        // need is the dp and the object type and we can create an attached
        // property descriptor based on that.  We must special case attached
        // properties here because TypeDescriptor will only return attached
        // properties for instances, not types.

        DependencyPropertyDescriptor dpd = null;

        if (ownerType.GetProperty(dependencyProperty.Name) != null)
        {
            // For direct properties we don't want to get the property descriptor
            // yet because it is very expensive.  Delay it until needed.

            lock (_ignorePropertyTypeCache)
            {
                _ignorePropertyTypeCache.TryGetValue(dependencyProperty, out dpd);
            }

            if (dpd == null)
            {
                // Create a new DPD based on the type information we have.  It 
                // will fill in the property descriptor by calling TypeDescriptor
                // when needed.

                dpd = new DependencyPropertyDescriptor(null, dependencyProperty.Name, targetType, dependencyProperty, false);

                lock (_ignorePropertyTypeCache)
                {
                    _ignorePropertyTypeCache[dependencyProperty] = dpd;
                }
            }
        }
        else
        {
            if (ownerType.GetMethod("Get" + dependencyProperty.Name) == null &&
                ownerType.GetMethod("Set" + dependencyProperty.Name) == null)
            {
                return null;
            }

            // If it isn't a direct property, we treat it as attached unless it is internal.
            // We should never release internal properties to the user

            PropertyDescriptor prop = DependencyObjectProvider.GetAttachedPropertyDescriptor(dependencyProperty, targetType);
            if (prop != null)
            {
                dpd = FromProperty(prop);
            }
        }

        return dpd;
    }

    /// <summary>
    /// Returns a <see cref="DependencyPropertyDescriptor"/> for a provided dependency property and target type.
    /// </summary>
    /// <param name="dependencyProperty">
    /// The identifier for a dependency property.
    /// </param>
    /// <param name="targetType">
    /// The type of the object where the property is set.
    /// </param>
    /// <returns>
    /// A <see cref="DependencyPropertyDescriptor"/> for the provided dependency property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dependencyProperty"/> is null -or- <paramref name="targetType"/> is null.
    /// </exception>
    public static DependencyPropertyDescriptor FromProperty(DependencyProperty dependencyProperty, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(dependencyProperty);
        ArgumentNullException.ThrowIfNull(targetType);

        // We have a different codepath here for attached and direct
        // properties.  For direct properties, we route through Type
        // Descriptor because we need the underlying CLR property descriptor
        // to create our wrapped property.  For attached properties, all we
        // need is the dp and the object type and we can create an attached
        // property descriptor based on that.  We must special case attached
        // properties here because TypeDescriptor will only return attached
        // properties for instances, not types.

        DependencyPropertyDescriptor dpd = null;
        DependencyPropertyKind dpKind = DependencyObjectProvider.GetDependencyPropertyKind(dependencyProperty, targetType);

        if (dpKind.IsDirect)
        {
            // For direct properties we don't want to get the property descriptor
            // yet because it is very expensive.  Delay it until needed.

            lock (_cache)
            {
                _cache.TryGetValue(dependencyProperty, out dpd);
            }

            if (dpd == null)
            {
                // Create a new DPD based on the type information we have.  It 
                // will fill in the property descriptor by calling TypeDescriptor
                // when needed.

                dpd = new DependencyPropertyDescriptor(null, dependencyProperty.Name, targetType, dependencyProperty, false);

                lock (_cache)
                {
                    _cache[dependencyProperty] = dpd;
                }
            }
        }
        else if (!dpKind.IsInternal)
        {
            // If it isn't a direct property, we treat it as attached unless it is internal.
            // We should never release internal properties to the user

            PropertyDescriptor prop = DependencyObjectProvider.GetAttachedPropertyDescriptor(dependencyProperty, targetType);
            if (prop != null)
            {
                dpd = FromProperty(prop);
            }
        }

        return dpd;
    }

    /// <summary>
    /// Returns a <see cref="DependencyPropertyDescriptor"/> for a provided property name.
    /// </summary>
    /// <param name="name">
    /// The registered name of a dependency property or an attached property.
    /// </param>
    /// <param name="ownerType">
    /// The <see cref="Type"/> of the object that owns the property definition.
    /// </param>
    /// <param name="targetType">
    /// The <see cref="Type"/> of the object you want to set the property for.
    /// </param>
    /// <returns>
    /// The requested <see cref="DependencyPropertyDescriptor"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> is null -or- <paramref name="ownerType"/> is null -or- <paramref name="targetType"/> is null.
    /// </exception>
    public static DependencyPropertyDescriptor FromName(string name, Type ownerType, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(ownerType);
        ArgumentNullException.ThrowIfNull(targetType);

        DependencyProperty dp = DependencyProperty.FromName(name, ownerType);
        if (dp != null)
        {
            return FromProperty(dp, targetType);
        }

        return null;
    }


    /// <summary>
    /// Returns a <see cref="DependencyPropertyDescriptor"/> for a provided property name.
    /// </summary>
    /// <param name="name">
    /// The registered name of a dependency property or an attached property.
    /// </param>
    /// <param name="ownerType">
    /// The <see cref="Type"/> of the object that owns the property definition.
    /// </param>
    /// <param name="targetType">
    /// The <see cref="Type"/> of the object you want to set the property for.
    /// </param>
    /// <param name="ignorePropertyType">
    /// Specifies to ignore the property type.
    /// </param>
    /// <returns>
    /// The requested <see cref="DependencyPropertyDescriptor"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> is null -or- <paramref name="ownerType"/> is null -or- <paramref name="targetType"/> is null.
    /// </exception>
    public static DependencyPropertyDescriptor FromName(string name, Type ownerType, Type targetType, bool ignorePropertyType)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(ownerType);
        ArgumentNullException.ThrowIfNull(targetType);

        DependencyProperty dp = DependencyProperty.FromName(name, ownerType);
        if (dp != null)
        {
            if (ignorePropertyType)
            {
                try
                {
                    return FromProperty(dp, ownerType, targetType, ignorePropertyType);
                }
                catch (AmbiguousMatchException)
                {
                    return FromProperty(dp, targetType);
                }
            }
            else
            {
                return FromProperty(dp, targetType);
            }
        }

        return null;
    }

    /// <summary>
    /// Compares two <see cref="DependencyPropertyDescriptor"/> instances for equality.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="DependencyPropertyDescriptor"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// true if the values are equivalent; otherwise, false.
    /// </returns>
    public override bool Equals(object obj) =>
        obj is DependencyPropertyDescriptor dp &&
        dp.DependencyProperty == DependencyProperty &&
        dp.ComponentType == ComponentType;

    /// <summary>
    /// Returns the hash code for this <see cref="DependencyPropertyDescriptor"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    public override int GetHashCode() => DependencyProperty.GetHashCode() ^ ComponentType.GetHashCode();

    /// <summary>
    /// Converts the value of this instance to its equivalent string representation.
    /// </summary>
    /// <returns>
    /// Returns the <see cref="MemberDescriptor.Name"/> value.
    /// </returns>
    public override string ToString() => Name;

    //
    // The following methods simply route to the underlying property descriptor.
    //

    /// <summary>
    /// Returns whether resetting an object changes its value.
    /// </summary>
    /// <param name="component">
    /// The component to test for reset capability.
    /// </param>
    /// <returns>
    /// true if resetting the component changes its value; otherwise, false.
    /// </returns>
    public override bool CanResetValue(object component) => Property.CanResetValue(component);

    /// <summary>
    /// Returns the current value of the property on a component.
    /// </summary>
    /// <param name="component">
    /// The component instance.
    /// </param>
    /// <returns>
    /// The requested value.
    /// </returns>
    public override object GetValue(object component) => Property.GetValue(component);

    /// <summary>
    /// Resets the value for this property of the component to the default value.
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be reset to the default value.
    /// </param>
    public override void ResetValue(object component) => Property.ResetValue(component);

    /// <summary>
    /// Sets the value of the component to a different value.
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be set.
    /// </param>
    /// <param name="value">
    /// The new value.
    /// </param>
    public override void SetValue(object component, object value) => Property.SetValue(component, value);

    /// <summary>
    /// Indicates whether the value of this property needs to be persisted by serialization processes.
    /// </summary>
    /// <param name="component">
    /// The component with the property to be examined for persistence.
    /// </param>
    /// <returns>
    /// true if the property should be persisted; otherwise, false.
    /// </returns>
    public override bool ShouldSerializeValue(object component) => Property.ShouldSerializeValue(component);

    /// <summary>
    /// Enables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">
    /// The component to add the handler for.
    /// </param>
    /// <param name="handler">
    /// The delegate to add as a listener.
    /// </param>
    public override void AddValueChanged(object component, EventHandler handler) => Property.AddValueChanged(component, handler);

    /// <summary>
    /// Enables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">
    /// The component to add the handler for.
    /// </param>
    /// <param name="handler">
    /// The delegate to add as a listener.
    /// </param>
    public override void RemoveValueChanged(object component, EventHandler handler) => Property.RemoveValueChanged(component, handler);

    /// <summary>
    /// Returns a <see cref="PropertyDescriptorCollection"/>.
    /// </summary>
    /// <param name="instance">
    /// A component to get the properties for.
    /// </param>
    /// <param name="filter">
    /// An array of type System.Attribute to use as a filter.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyDescriptorCollection"/> with the properties that match the specified attributes for the specified component.
    /// </returns>
    public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) => Property.GetChildProperties(instance, filter);

    /// <summary>
    /// Gets an editor of the specified type.
    /// </summary>
    /// <param name="editorBaseType">
    /// The base type of editor, which is used to differentiate between multiple editors that a property supports.
    /// </param>
    /// <returns>
    /// An instance of the requested editor type, or null if an editor cannot be found.
    /// </returns>
    public override object GetEditor(Type editorBaseType) => Property.GetEditor(editorBaseType);

    /// <summary>
    /// Returns the dependency property identifier.
    /// </summary>
    /// <returns>
    /// The dependency property identifier.
    /// </returns>
    public DependencyProperty DependencyProperty { get; }

    /// <summary>
    /// Gets a value that indicates whether the property is registered as an attached property and is being used 
    /// through an attached usage.
    /// </summary>
    /// <returns>
    /// true if the property is an attached property; otherwise, false.
    /// </returns>
    public bool IsAttached { get; }

    /// <summary>
    /// Gets the metadata associated with the dependency property.
    /// </summary>
    /// <returns>
    /// The dependency property metadata.
    /// </returns>
    public PropertyMetadata Metadata { get; }

    //
    // The following properties simply route to the underlying property descriptor.
    //

    /// <summary>
    /// Gets the type of the component this property is bound to.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> that represents the type of component this property is bound to.
    /// When <see cref="GetValue(object)"/> or <see cref="SetValue(object, object)"/> are invoked, the object 
    /// specified might be an instance of this type.
    /// </returns>
    public override Type ComponentType { get; }

    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    /// <returns>
    /// true if the property is read-only; otherwise, false.
    /// </returns>
    public override bool IsReadOnly => Property.IsReadOnly;

    /// <summary>
    /// Gets the represented <see cref="Type"/> of the dependency property.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the dependency property.
    /// </returns>
    public override Type PropertyType => DependencyProperty.PropertyType;

    /// <summary>
    /// Gets the collection of attributes for this member.
    /// </summary>
    /// <returns>
    /// The <see cref="AttributeCollection"/> collection of attributes.
    /// </returns>
    public override AttributeCollection Attributes => Property.Attributes;

    /// <summary>
    /// Gets the name of the category that the member belongs to, as specified in the <see cref="CategoryAttribute"/>.
    /// </summary>
    /// <returns>
    /// The name of the category to which the member belongs. If there is no <see cref="CategoryAttribute"/>, the 
    /// category name is set to the default category, Misc.
    /// </returns>
    public override string Category => Property.Category;

    /// <summary>
    /// Gets the description of the member, as specified in the <see cref="DescriptionAttribute"/>.
    /// </summary>
    /// <returns>
    /// The description of the member. If there is no <see cref="DescriptionAttribute"/>, the property value is set to 
    /// the default, which is an empty string ("").
    /// </returns>
    public override string Description => Property.Description;

    /// <summary>
    /// Gets whether this member should be set only at design time, as specified in the <see cref="DesignOnlyAttribute"/>.
    /// </summary>
    /// <returns>
    /// true if this member should be set only at design time; false if the member can be set during run time. If there is 
    /// no <see cref="DesignOnlyAttribute"/>, the return value is the default, which is false.
    /// </returns>
    public override bool DesignTimeOnly => Property.DesignTimeOnly;

    /// <summary>
    /// Gets the name that can be displayed in a window, such as a Properties window.
    /// </summary>
    /// <returns>
    /// The name to display for the property.
    /// </returns>
    public override string DisplayName => Property.DisplayName;

    /// <summary>
    /// Gets the type converter for this property.
    /// </summary>
    /// <returns>
    /// A <see cref="TypeConverter"/> that is used to convert the <see cref="Type"/> of this property.
    /// </returns>
    public override TypeConverter Converter
    {
        get
        {
            // We only support public type converters, in order to avoid asserts.
            TypeConverter typeConverter = Property.Converter;
            if (typeConverter.GetType().IsPublic)
            {
                return typeConverter;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Gets a value that indicates the value of the <see cref="BrowsableAttribute"/> on the property.
    /// </summary>
    /// <returns>
    /// true if the <see cref="BrowsableAttribute"/> was specified on the property; otherwise, false.
    /// </returns>
    public override bool IsBrowsable => Property.IsBrowsable;

    /// <summary>
    /// Gets a value indicating whether this property should be localized, as specified in the <see cref="LocalizableAttribute"/>.
    /// </summary>
    /// <returns>
    /// true if the member is marked with the <see cref="LocalizableAttribute"/> constructor of the value true; otherwise, false.
    /// </returns>
    public override bool IsLocalizable => Property.IsLocalizable;

    /// <summary>
    /// Indicates whether value change notifications for this property may originate from outside the property 
    /// descriptor, such as from the component itself, or whether notifications will only originate from direct 
    /// calls made to <see cref="SetValue(object, object)"/>.
    /// </summary>
    /// <returns>
    /// true if notifications for this property may originate from outside the property descriptor, such as from 
    /// the component itself. false if notifications will only originate from direct calls made to <see cref="SetValue(object, object)"/>.
    /// </returns>
    public override bool SupportsChangeEvents => Property.SupportsChangeEvents;

#if false
    /// <summary>
    /// Gets or sets a callback that designers use to modify the effective value of a dependency property before 
    /// the dependency property value is stored in the dependency property engine.
    /// </summary>
    /// <returns>
    /// A callback that designers use to modify the effective value of a dependency property before the dependency 
    /// property value is stored in the dependency property engine.
    /// </returns>
    public CoerceValueCallback DesignerCoerceValueCallback
    {
        get => DependencyProperty.DesignerCoerceValueCallback;
        set => DependencyProperty.DesignerCoerceValueCallback = value;
    }
#endif

    /// <summary>
    ///     This method is called when we should clear our cached state.  The cache
    ///     may become invalid if someone adds additional type description providers.
    /// </summary>
    internal static void ClearCache()
    {
        lock (_cache)
        {
            _cache.Clear();
        }
    }

    // Return the property descriptor we're wrapping.  We may have to get
    // this on demand if it wasn't passed into our constructor
    private PropertyDescriptor Property
    {
        get
        {
            if (_property is null)
            {
                _property = TypeDescriptor.GetProperties(ComponentType)[Name];

                // This should not normally happen.  If it does, it means
                // that someone has messed around with metadata and has
                // removed this property from the type's metadata.  We know
                // that there is really a CLR property, however, because
                // we are dealing with a direct property (only direct
                // properties can have their property descriptor delay
                // loaded).  So, we can magically create one directly
                // from the CLR property through TypeDescriptor.
                _property ??= TypeDescriptor.CreateProperty(ComponentType, Name, DependencyProperty.PropertyType);
            }

            return _property;
        }
    }

    private PropertyDescriptor _property;

    // Synchronized by "_cache"
    private static readonly Dictionary<object, DependencyPropertyDescriptor> _cache = new(ReferenceEqualityComparer.Instance);
    private static readonly Dictionary<object, DependencyPropertyDescriptor> _ignorePropertyTypeCache = new(ReferenceEqualityComparer.Instance);

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        private ReferenceEqualityComparer() { }

        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}