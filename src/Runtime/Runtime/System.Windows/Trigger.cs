
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies property values or performs actions conditionally
/// based on the value of another property.
/// </summary>
[ContentProperty(nameof(Setters))]
[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
public sealed class Trigger : TriggerBase, ISupportInitialize
{
    private DependencyProperty _property;
    private object _value = DependencyProperty.UnsetValue;
    private string _sourceName;
    private SetterBaseCollection _setters;
    private InitializationState _initializationState;

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
    [TypeConverter(typeof(SetterTriggerConditionValueConverter))]
    public object Value
    {
        get => _value;
        set
        {
            CheckSealed();

            _value = value switch
            {
                NullExtension => null,
                string s when s == "{x:Null}" => null,
                MarkupExtension => throw new ArgumentException(string.Format(Strings.ConditionValueOfMarkupExtensionNotSupported, value.GetType().Name)),
                Expression => throw new ArgumentException(Strings.ConditionValueOfExpressionNotSupported),
                _ => value,
            };
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
    public SetterBaseCollection Setters => _setters ??= [];

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

        // Ensure valid condition
        if (!_property.IsValidValue(_value))
        {
            throw new InvalidOperationException(string.Format(Strings.InvalidPropertyValue, _value, _property.Name));
        }

        // Freeze the condition value
        StyleHelper.SealIfSealable(_value);

        // Seal the setters collection
        ProcessSettersCollection(_setters);

        base.Seal();
    }

    internal static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
    {
        if (targetObject is not Trigger trigger)
        {
            throw new ArgumentNullException(nameof(targetObject));
        }

        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventArgs.Member.Name == nameof(Property))
        {
            trigger._initializationState ??= new();
            trigger._initializationState.UnresolvedProperty = eventArgs.Value;
            trigger._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            trigger._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

            eventArgs.Handled = true;
        }
        else if (eventArgs.Member.Name == nameof(Value))
        {
            trigger._initializationState ??= new();
            trigger._initializationState.UnresolvedValue = eventArgs.Value;
            trigger._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            trigger._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

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
