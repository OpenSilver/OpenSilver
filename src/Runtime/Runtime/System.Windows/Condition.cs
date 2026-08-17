
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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml.Markup;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a condition for the <see cref="MultiTrigger"/> and the <see cref="MultiDataTrigger"/>.
/// </summary>
[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
public sealed class Condition : ISupportInitialize
{
    private DependencyProperty _property;
    private BindingBase _binding;
    private object _value = DependencyProperty.UnsetValue;
    private string _sourceName;
    private bool _sealed;
    private InitializationState _initializationState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class.
    /// </summary>
    public Condition() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class with
    /// the specified property and value.
    /// </summary>
    /// <param name="conditionProperty">
    /// The property of the condition.
    /// </param>
    /// <param name="conditionValue">
    /// The value of the condition.
    /// </param>
    public Condition(DependencyProperty conditionProperty, object conditionValue)
        : this(conditionProperty, conditionValue, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class with
    /// the specified property, value, and source name.
    /// </summary>
    /// <param name="conditionProperty">
    /// The property of the condition.
    /// </param>
    /// <param name="conditionValue">
    /// The value of the condition.
    /// </param>
    /// <param name="sourceName">
    /// The source name of the condition.
    /// </param>
    public Condition(DependencyProperty conditionProperty, object conditionValue, string sourceName)
    {
        ArgumentNullException.ThrowIfNull(conditionProperty);

        if (!conditionProperty.IsValidValue(conditionValue))
        {
            throw new ArgumentException(string.Format(Strings.InvalidPropertyValue, conditionValue, conditionProperty.Name));
        }

        _property = conditionProperty;
        _value = conditionValue;
        _sourceName = sourceName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class with
    /// the specified binding and value.
    /// </summary>
    /// <param name="binding">
    /// The binding that specifies the property of the condition.
    /// </param>
    /// <param name="conditionValue">
    /// The value of the condition.
    /// </param>
    public Condition(BindingBase binding, object conditionValue)
    {
        ArgumentNullException.ThrowIfNull(binding);

        _binding = binding;
        _value = conditionValue;
    }

    /// <summary>
    /// Gets or sets the property of the condition. This is only applicable to
    /// <see cref="MultiTrigger"/> conditions.
    /// </summary>
    /// <returns>
    /// A <see cref="DependencyProperty"/> that specifies the property of the condition.
    /// The default is null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A <see cref="Condition"/> cannot have both <see cref="Property"/> and <see cref="Binding"/> set.
    /// </exception>
    [Ambient]
    [DefaultValue(null)]
    [TypeConverter(typeof(DependencyPropertyConverter))]
    public DependencyProperty Property
    {
        get => _property;
        set
        {
            CheckSealed();

            if (_binding is not null)
            {
                throw new InvalidOperationException(Strings.ConditionCannotUseBothPropertyAndBinding);
            }

            _property = value;
        }
    }

    /// <summary>
    /// Gets or sets the binding that specifies the property of the condition. This is only
    /// applicable to <see cref="MultiDataTrigger"/> conditions.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A <see cref="Condition"/> cannot have both <see cref="Property"/> and <see cref="Binding"/> set.
    /// </exception>
    [DefaultValue(null)]
    public BindingBase Binding
    {
        get => _binding;
        set
        {
            CheckSealed();

            if (_property is not null)
            {
                throw new InvalidOperationException(Strings.ConditionCannotUseBothPropertyAndBinding);
            }

            _binding = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the condition.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Expressions are not supported.
    /// </exception>
    [TypeConverter(typeof(SetterTriggerConditionValueConverter))]
    public object Value
    {
        get => _value;
        set
        {
            CheckSealed();

            if (value is MarkupExtension)
            {
                throw new ArgumentException(string.Format(Strings.ConditionValueOfMarkupExtensionNotSupported, value.GetType().Name));
            }

            if (value is Expression)
            {
                throw new ArgumentException(Strings.ConditionValueOfExpressionNotSupported);
            }

            _value = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of the element whose property causes the associated setters to be applied.
    /// This is only applicable to <see cref="MultiTrigger"/> conditions.
    /// </summary>
    /// <returns>
    /// The default property is null. If this property is null, then the property of the element
    /// being styled causes the associated setters to be applied.
    /// </returns>
    [DefaultValue(null)]
    public string SourceName
    {
        get => _sourceName;
        set
        {
            CheckSealed();
            _sourceName = value;
        }
    }

    internal void Seal(bool isPropertyTrigger)
    {
        if (_sealed)
        {
            return;
        }

        _sealed = true;

        // Validate that Property and Binding are not both set
        if (_property is not null && _binding is not null)
        {
            throw new InvalidOperationException(Strings.ConditionCannotUseBothPropertyAndBinding);
        }

        if (isPropertyTrigger)
        {
            // Property trigger - needs Property
            if (_property is null)
            {
                throw new InvalidOperationException(string.Format(Strings.NullPropertyIllegal, "Condition.Property"));
            }

            if (!_property.IsValidValue(_value))
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidPropertyValue, _value, _property.Name));
            }
        }
        else
        {
            // Data trigger - needs Binding
            if (_binding is null)
            {
                throw new InvalidOperationException(string.Format(Strings.NullPropertyIllegal, "Condition.Binding"));
            }
        }

        // Freeze the condition value
        StyleHelper.SealIfSealable(_value);
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(Condition)));
        }
    }

    public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
    {
        if (targetObject is not Condition condition)
        {
            throw new ArgumentNullException(nameof(targetObject));
        }

        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventArgs.Member.Name == nameof(Property))
        {
            condition._initializationState ??= new();
            condition._initializationState.UnresolvedProperty = eventArgs.Value;
            condition._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            condition._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

            eventArgs.Handled = true;
        }
        else if (eventArgs.Member.Name == nameof(Value))
        {
            condition._initializationState ??= new();
            condition._initializationState.UnresolvedValue = eventArgs.Value;
            condition._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            condition._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

            eventArgs.Handled = true;
        }
    }

    public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(targetObject);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (targetObject is Condition condition &&
            eventArgs.Member.Name == nameof(Binding) &&
            eventArgs.MarkupExtension is BindingBase bindingBase)
        {
            condition.Binding = bindingBase;
            eventArgs.Handled = true;
        }
    }

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
        if (_initializationState is not InitializationState state)
        {
            return;
        }

        _initializationState = null;

        if (state.UnresolvedProperty is not null)
        {
            Property = DependencyPropertyConverter.ResolveProperty(state.ServiceProvider,
                SourceName, state.UnresolvedProperty);
        }

        if (state.UnresolvedValue is not null)
        {
            Value = SetterTriggerConditionValueConverter.ResolveValue(state.ServiceProvider,
                Property, state.CultureInfoForTypeConverter, state.UnresolvedValue);
        }
    }

    private sealed class InitializationState
    {
        public object UnresolvedProperty;
        public object UnresolvedValue;
        public ITypeDescriptorContext ServiceProvider;
        public CultureInfo CultureInfoForTypeConverter;
    }
}
