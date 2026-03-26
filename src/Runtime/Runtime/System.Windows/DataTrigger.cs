
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
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies property values or performs actions when the
/// bound data meets a specified condition.
/// </summary>
[ContentProperty(nameof(Setters))]
public sealed class DataTrigger : TriggerBase
{
    private BindingBase _binding;
    private object _value = DependencyProperty.UnsetValue;
    private SetterBaseCollection _setters;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTrigger"/> class.
    /// </summary>
    public DataTrigger() { }

    /// <summary>
    /// Gets or sets the binding that produces the property value of the data object.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    public BindingBase Binding
    {
        get => _binding;
        set
        {
            CheckSealed();
            _binding = value;
        }
    }

    /// <summary>
    /// Gets or sets the value to be compared with the property value of the data object.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Expressions are not supported. Bindings are not supported.
    /// </exception>
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
    /// Gets a collection of <see cref="Setter"/> objects, which describe the property values
    /// to apply when the data object meets the specified condition.
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

        if (_binding is null)
        {
            throw new InvalidOperationException(string.Format(Strings.NullPropertyIllegal, "DataTrigger.Binding"));
        }

        // Seal the setters collection
        ProcessSettersCollection(_setters);

        // Freeze the condition value
        StyleHelper.SealIfSealable(_value);

        base.Seal();
    }

    internal static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(targetObject);
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (targetObject is DataTrigger trigger &&
            eventArgs.Member.Name == nameof(Binding) &&
            eventArgs.MarkupExtension is BindingBase bindingBase)
        {
            trigger.Binding = bindingBase;
            eventArgs.Handled = true;
        }
        else
        {
            eventArgs.CallBase();
        }
    }
}
