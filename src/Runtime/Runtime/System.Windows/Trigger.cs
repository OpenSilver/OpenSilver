
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
using System.Windows.Markup;
using System.Xaml.Markup;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies property values or performs actions conditionally
/// based on the value of another property.
/// </summary>
[ContentProperty(nameof(Setters))]
public sealed class Trigger : TriggerBase
{
    private DependencyProperty _property;
    private object _value = DependencyProperty.UnsetValue;
    private string _sourceName;
    private SetterBaseCollection _setters;

    /// <summary>
    /// Initializes a new instance of the <see cref="Trigger"/> class.
    /// </summary>
    public Trigger() { }

    /// <summary>
    /// Gets or sets the property that returns the value that is compared with the 
    /// <see cref="Value"/> property of the trigger.
    /// </summary>
    /// <returns>
    /// A <see cref="DependencyProperty"/> that returns the property value of the element.
    /// The default value is null.
    /// </returns>
    [Ambient]
    [TypeConverter(typeof(DependencyPropertyConverter))]
    public DependencyProperty Property
    {
        get => _property;
        set
        {
            CheckSealed();
            _property = value;
        }
    }

    /// <summary>
    /// Gets or sets the value to be compared with the property value of the element.
    /// </summary>
    /// <returns>
    /// The default value is null. See also the Exceptions section.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Expressions, such as bindings, are not supported.
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
    /// </summary>
    /// <returns>
    /// The default is null. If this property is null, then the element being styled is the element
    /// whose property causes the associated setters to be applied.
    /// </returns>
    [DefaultValue(null)]
    [Ambient]
    public string SourceName
    {
        get => _sourceName;
        set
        {
            CheckSealed();
            _sourceName = value;
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="Setter"/> objects, which describe the property values
    /// to apply when the specified condition has been met.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SetterBaseCollection Setters => _setters ??= new SetterBaseCollection();

    /// <summary>
    /// Gets a value indicating whether this trigger has any setters.
    /// </summary>
    internal bool HasSetters => _setters is not null && _setters.InternalCount > 0;

    internal override void Seal()
    {
        if (IsSealed)
        {
            return;
        }

        if (_property is null)
        {
            throw new InvalidOperationException(string.Format(Strings.NullPropertyIllegal, "Trigger.Property"));
        }

        // Convert string value to proper type if needed (for compiler-generated code that couldn't
        // resolve the value at compile time because the service provider wasn't available)
        if (_value is string stringValue && _property.PropertyType != typeof(string))
        {
            _value = ConvertStringToPropertyType(stringValue, _property.PropertyType);
        }

        // Validate that the value is appropriate for the property
        if (_value != DependencyProperty.UnsetValue && !_property.IsValidValue(_value))
        {
            throw new InvalidOperationException(string.Format(Strings.InvalidPropertyValue, _value, _property.Name));
        }

        // Freeze the condition value
        StyleHelper.SealIfSealable(_value);

        // Seal the setters collection
        _setters?.Seal();

        base.Seal();
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

    /// <summary>
    /// Evaluates whether the trigger condition is currently satisfied for the given element.
    /// </summary>
    internal bool Evaluate(DependencyObject container)
    {
        if (_property is null)
        {
            return false;
        }

        object currentValue = container.GetValue(_property);
        return Match(currentValue, _value);
    }

    /// <summary>
    /// Compares two values for equality, handling null and UnsetValue appropriately.
    /// Also handles type conversion when the types don't match (e.g., comparing string "True" with boolean true).
    /// </summary>
    internal static bool Match(object currentValue, object triggerValue)
    {
        // Handle null and UnsetValue
        if (triggerValue == DependencyProperty.UnsetValue)
        {
            return currentValue == DependencyProperty.UnsetValue;
        }

        if (triggerValue is null)
        {
            return currentValue is null;
        }

        if (currentValue is null)
        {
            return false;
        }

        // If types match, use direct comparison
        if (triggerValue.GetType() == currentValue.GetType())
        {
            return triggerValue.Equals(currentValue);
        }

        // Try to convert triggerValue to currentValue's type for comparison
        // This handles cases like DataTrigger where binding returns bool but Value is string "True"
        if (triggerValue is string stringValue)
        {
            object convertedTriggerValue = ConvertStringToPropertyType(stringValue, currentValue.GetType());
            if (convertedTriggerValue is not null && convertedTriggerValue.GetType() == currentValue.GetType())
            {
                return convertedTriggerValue.Equals(currentValue);
            }
        }

        // Fallback: use Equals for comparison
        return triggerValue.Equals(currentValue);
    }
}
