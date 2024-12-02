
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
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Applies a value to a property in a <see cref="Style"/>.
/// </summary>
public sealed class Setter : SetterBase, ISupportInitialize
{
    private DependencyProperty _property;
    private object _value;

    private object _unresolvedValue = null;
    private ITypeDescriptorContext _serviceProvider = null;
    private CultureInfo _cultureInfoForTypeConverter = null;

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
    [TypeConverter(typeof(SetterValueConverter))]
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
                    if (value is not BindingBase && value is not DynamicResourceExtension)
                    {
                        throw new ArgumentException(string.Format(Strings.SetterValueOfMarkupExtensionNotSupported, value.GetType().Name));
                    }
                    break;

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
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (property == FrameworkElement.NameProperty)
        {
            // Note: Silverlight allows this, but will crash as soon as
            // the style is used 2 times in the visual tree.
            throw new InvalidOperationException(string.Format(Strings.CannotHavePropertyInStyle, FrameworkElement.NameProperty.Name));
        }
    }

    internal void ReceiveTypeConverter(ITypeDescriptorContext serviceProvider, CultureInfo culture, object unresolvedValue)
    {
        _serviceProvider = serviceProvider;
        _cultureInfoForTypeConverter = culture;
        _unresolvedValue = unresolvedValue;
    }

    internal static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
    {
        if (targetObject is null)
        {
            throw new ArgumentNullException(nameof(targetObject));
        }

        if (eventArgs is null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        if (targetObject is not Setter setter || eventArgs.Member.Name != "Value")
        {
            return;
        }

        MarkupExtension me = eventArgs.MarkupExtension;

        if (me is DynamicResourceExtension || me is BindingBase)
        {
            setter.Value = me;
            eventArgs.Handled = true;
        }
    }

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
        if (_unresolvedValue != null)
        {
            try
            {
                Value = SetterValueConverter.ResolveValue(_serviceProvider,
                    Property, _cultureInfoForTypeConverter, _unresolvedValue);
            }
            finally
            {
                _unresolvedValue = null;
            }
        }

        _serviceProvider = null;
        _cultureInfoForTypeConverter = null;
    }
}
