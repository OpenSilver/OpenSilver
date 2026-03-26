
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
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Applies a value to a property in a <see cref="Style"/>.
/// </summary>
[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
public sealed class Setter : SetterBase, ISupportInitialize
{
    private DependencyProperty _property;
    private object _value;
    private string _targetName;
    private InitializationState _initializationState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Setter"/> class.
    /// </summary>
    public Setter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Setter"/> class with the specified
    /// property and value.
    /// </summary>
    /// <param name="property">
    /// The dependency property to apply the value to.
    /// </param>
    /// <param name="value">
    /// The value to apply to the property.
    /// </param>
    public Setter(DependencyProperty property, object value)
    {
        CheckValidProperty(property);

        _property = property;
        _value = value == DependencyProperty.UnsetValue ? null : value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Setter"/> class with the specified 
    /// property, value, and target name.
    /// </summary>
    /// <param name="property">
    /// The <see cref="DependencyProperty"/> to apply the <see cref="Value"/> to.
    /// </param>
    /// <param name="value">
    /// The value to apply to the property.
    /// </param>
    /// <param name="targetName">
    /// The name of the child node this <see cref="Setter"/> is intended for.
    /// </param>
    public Setter(DependencyProperty property, object value, string targetName)
    {
        CheckValidProperty(property);

        _property = property;
        _value = value == DependencyProperty.UnsetValue ? null : value;
        _targetName = targetName;
    }

    /// <summary>
    /// Gets or sets the property to apply the <see cref="Value"/> to.
    /// </summary>
    /// <returns>
    /// A <see cref="DependencyProperty"/> to which the <see cref="Value"/>
    /// will be applied. The default is null.
    /// </returns>
    [Ambient]
    [TypeConverter(typeof(DependencyPropertyConverter))]
    public DependencyProperty Property
    {
        get => _property;
        set
        {
            CheckValidProperty(value);
            CheckSealed();
            _property = value;
        }
    }

    /// <summary>
    /// Gets or sets the value to apply to the property that is specified by the <see cref="Setter"/>.
    /// </summary>
    /// <returns>
    /// The value to apply to the property that is specified by the <see cref="Setter"/>.
    /// </returns>
    [TypeConverter(typeof(SetterTriggerConditionValueConverter))]
    public object Value
    {
        get => _value;
        set
        {
            CheckSealed();

            if (value == DependencyProperty.UnsetValue)
            {
                // Silverlight uses a DependencyProperty for Setter.Value,
                // so in case of DependencyProperty.UnsetValue, we emulate
                // a call to DependencyObject.ClearValue(...).
                _value = null;
                return;
            }

            if (value is Expression)
            {
                throw new ArgumentException(Strings.StyleValueOfExpressionNotSupported);
            }

            _value = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of the element to which this <see cref="Setter"/> applies.
    /// </summary>
    /// <returns>
    /// The name of the element. The default is null.
    /// </returns>
    /// <remarks>
    /// You can set this property to the name of any element within the scope of where the setter
    /// collection (the collection that this setter is part of) is applied. This is typically a
    /// named element that is within the template that contains this setter. This property is
    /// used in templates and is not typically used in styles.
    /// </remarks>
    [DefaultValue(null)]
    [Ambient]
    public string TargetName
    {
        get => _targetName;
        set
        {
            CheckSealed();
            _targetName = value;
        }
    }

    /// <summary>
    /// Seals this setter
    /// </summary>
    internal override void Seal()
    {
        // Do the validation that can't be done until we know all of the property
        // values.

        DependencyProperty dp = Property;
        object value = Value;

        if (dp is null)
        {
            throw new ArgumentException(string.Format(Strings.NullPropertyIllegal, "Setter.Property"));
        }

        if (string.IsNullOrEmpty(TargetName))
        {
            // Setter on container is not allowed to affect the StyleProperty.
            if (dp == FrameworkElement.StyleProperty)
            {
                throw new ArgumentException(Strings.StylePropertyInStyleNotAllowed);
            }
        }

        if (dp.IsObjectType || !dp.IsValidValue(value))
        {
            switch (value)
            {
                case Color color:
                    if (dp.PropertyType == typeof(Brush))
                    {
                        _value = new SolidColorBrush(color);
                    }
                    break;

                case MarkupExtension:
                    // Bindings and dynamic resources are allowed on setters, they will later be transformed into an expression
                    if (value is BindingBase || value is DynamicResourceExtension)
                    {
                        break;
                    }

                    if (value is ResponsiveExtension responsiveExtension)
                    {
                        _value = new ResponsiveExtension
                        {
                            Mobile = responsiveExtension.ConvertHelper(responsiveExtension.Mobile, _property),
                            Tablet = responsiveExtension.ConvertHelper(responsiveExtension.Tablet, _property),
                            Desktop = responsiveExtension.ConvertHelper(responsiveExtension.Desktop, _property),
                            Threshold = responsiveExtension.Threshold,
                        };
                        break;
                    }

                    throw new ArgumentException(string.Format(Strings.SetterValueOfMarkupExtensionNotSupported, value.GetType().Name));

                default:
                    if (!dp.IsObjectType)
                    {
                        throw new ArgumentException(string.Format(Strings.InvalidSetterValue, value, dp.OwnerType, dp.Name));
                    }
                    break;
            }
        }

        base.Seal();
    }

    private void CheckValidProperty(DependencyProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (property.ReadOnly)
        {
            // Read-only properties will not be consulting Style/Template/Trigger Setter for value.
            // Rather than silently do nothing, throw error.
            throw new ArgumentException(string.Format(Strings.ReadOnlyPropertyNotAllowed, property.Name, GetType().Name));
        }

        if (property == FrameworkElement.NameProperty)
        {
            // Note: Silverlight allows this, but will crash as soon as
            // the style is used 2 times in the visual tree.
            throw new InvalidOperationException(string.Format(Strings.CannotHavePropertyInStyle, FrameworkElement.NameProperty.Name));
        }
    }

    internal static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
    {
        if (targetObject is not Setter setter)
        {
            throw new ArgumentNullException(nameof(targetObject));
        }

        ArgumentNullException.ThrowIfNull(eventArgs);

        if (eventArgs.Member.Name == nameof(Property))
        {
            setter._initializationState ??= new();
            setter._initializationState.UnresolvedProperty = eventArgs.Value;
            setter._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            setter._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

            eventArgs.Handled = true;
        }
        else if (eventArgs.Member.Name == nameof(Value))
        {
            setter._initializationState ??= new();
            setter._initializationState.UnresolvedValue = eventArgs.Value;
            setter._initializationState.ServiceProvider = eventArgs.ServiceProvider;
            setter._initializationState.CultureInfoForTypeConverter = eventArgs.CultureInfo;

            eventArgs.Handled = true;
        }
    }

    internal static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(targetObject);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (targetObject is not Setter setter || eventArgs.Member.Name != nameof(Value))
        {
            return;
        }

        MarkupExtension me = eventArgs.MarkupExtension;

        if (me is DynamicResourceExtension || me is BindingBase || me is ResponsiveExtension)
        {
            setter.Value = me;
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
                TargetName, state.UnresolvedProperty);
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
