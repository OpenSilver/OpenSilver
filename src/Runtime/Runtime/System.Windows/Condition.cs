
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
public sealed class Condition
{
    private DependencyProperty _property;
    private BindingBase _binding;
    private object _value = DependencyProperty.UnsetValue;
    private string _sourceName;
    private bool _sealed;

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
    [TypeConverter(typeof(SetterValueConverter))]
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

    internal bool IsSealed => _sealed;

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

            // Convert string value to proper type if needed (for compiler-generated code that couldn't
            // resolve the value at compile time because the service provider wasn't available)
            if (_value is string stringValue && _property.PropertyType != typeof(string))
            {
                _value = ConvertStringToPropertyType(stringValue, _property.PropertyType);
            }

            if (_value != DependencyProperty.UnsetValue && !_property.IsValidValue(_value))
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

    /// <summary>
    /// Converts a string value to the specified property type using the appropriate type converter.
    /// </summary>
    private static object ConvertStringToPropertyType(string value, Type targetType)
    {
        var converter = TypeConverterHelper.GetConverter(targetType);
        if (converter is not null && converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        // Fallback: return the original string if no converter is available
        return value;
    }

    private void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(Condition)));
        }
    }
}
