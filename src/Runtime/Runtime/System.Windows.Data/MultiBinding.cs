
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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data;

/// <summary>
/// Describes a collection of <see cref="Binding"/> objects attached to a single binding target property.
/// </summary>
[ContentProperty(nameof(Bindings))]
public class MultiBinding : BindingBase
{
    private object _converterParameter;
    private CultureInfo _culture;
    private IMultiValueConverter _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiBinding"/> class.
    /// </summary>
    public MultiBinding()
    {
        Bindings = new BindingCollection(this, OnBindingCollectionChanged);
    }

    /// <summary>
    /// Gets the collection of <see cref="Binding"/> objects within this <see cref="MultiBinding"/> instance.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="Binding"/> objects. <see cref="MultiBinding"/> currently supports only 
    /// objects of type <see cref="Binding"/> and not <see cref="MultiBinding"/>. Adding a <see cref="Binding"/> 
    /// child to a <see cref="MultiBinding"/> object implicitly adds the child to the <see cref="BindingBase"/> 
    /// collection for the <see cref="MultiBinding"/> object.
    /// </returns>
    public Collection<BindingBase> Bindings { get; }

    /// <summary>
    /// Gets or sets a value that determines the timing of binding source updates.
    /// </summary>
    /// <returns>
    /// A value that determines when the binding source is updated. The default is 
    /// <see cref="UpdateSourceTrigger.Default"/>.
    /// </returns>
    public UpdateSourceTrigger UpdateSourceTrigger
    {
        get { return UpdateSourceTriggerInternal; }
        set
        {
            CheckSealed();
            UpdateSourceTriggerInternal = value;
        }
    }

    /// <summary>
    /// Gets or sets an optional parameter to pass to a converter as additional information.
    /// </summary>
    /// <returns>
    /// A parameter to pass to a converter. The default value is null.
    /// </returns>
    public object ConverterParameter
    {
        get { return _converterParameter; }
        set { CheckSealed(); _converterParameter = value; }
    }

    /// <summary>
    /// Gets or sets the <see cref="CultureInfo"/> object that applies to any converter assigned 
    /// to bindings wrapped by the <see cref="MultiBinding"/> or on the <see cref="MultiBinding"/>
    /// itself.
    /// </summary>
    /// <returns>
    /// A valid <see cref="CultureInfo"/>.
    /// </returns>
    public CultureInfo ConverterCulture
    {
        get { return _culture; }
        set { CheckSealed(); _culture = value; }
    }

    /// <summary>
    /// Gets or sets the converter to use to convert the source values to or from the target value.
    /// </summary>
    /// <returns>
    /// A value of type <see cref="IMultiValueConverter"/> that indicates the converter to use.
    /// The default value is null.
    /// </returns>
    public IMultiValueConverter Converter
    {
        get { return _converter; }
        set { CheckSealed(); _converter = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates the direction of the data flow of this binding.
    /// </summary>
    /// <returns>
    /// One of the <see cref="BindingMode"/> values. The default is <see cref="BindingMode.OneWay"/>.
    /// </returns>
    public BindingMode Mode
    {
        get
        {
            return GetFlagsWithinMask(PrivateFlags.PropagationMask) switch
            {
                PrivateFlags.TwoWay => BindingMode.TwoWay,
                PrivateFlags.OneTime => BindingMode.OneTime,
                _ => BindingMode.OneWay,
            };
        }
        set
        {
            CheckSealed();
            ChangeFlagsWithinMask(PrivateFlags.PropagationMask, FlagsFrom(value));
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report exception validation errors.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report exception validation errors; otherwise, false. The default is false.
    /// </returns>
    public bool ValidatesOnExceptions
    {
        get { return TestFlag(PrivateFlags.ValidatesOnExceptions); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.ValidatesOnExceptions, value);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report validation
    /// errors from an <see cref="IDataErrorInfo"/> implementation on the bound data entity.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report <see cref="IDataErrorInfo"/> validation errors;
    /// otherwise, false. The default is false.
    /// </returns>
    public bool ValidatesOnDataErrors
    {
        get { return TestFlag(PrivateFlags.ValidatesOnDataErrors); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.ValidatesOnDataErrors, value);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="FrameworkElement.BindingValidationError"/>
    /// event is raised on validation errors.
    /// </summary>
    /// <returns>
    /// true if the <see cref="FrameworkElement.BindingValidationError"/> event is raised; otherwise, false.
    /// The default is false.
    /// </returns>
    public bool NotifyOnValidationError
    {
        get { return TestFlag(PrivateFlags.NotifyOnValidationError); }
        set
        {
            CheckSealed();
            ChangeFlag(PrivateFlags.NotifyOnValidationError, value);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding engine will report validation errors 
    /// from an <see cref="INotifyDataErrorInfo"/> implementation on the bound data entity.
    /// </summary>
    /// <returns>
    /// true if the binding engine will report <see cref="INotifyDataErrorInfo"/> validation errors; 
    /// otherwise, false. The default is true.
    /// </returns>
    public bool ValidatesOnNotifyDataErrors
    {
        get { return ValidatesOnNotifyDataErrorsInternal; }
        set
        {
            CheckSealed();
            ValidatesOnNotifyDataErrorsInternal = value;
        }
    }

    internal override BindingExpressionBase CreateBindingExpressionOverride(
        DependencyObject target, DependencyProperty dp, BindingExpressionBase owner)
    {
        if (Converter is null && string.IsNullOrEmpty(StringFormat))
        {
            throw new InvalidOperationException(Strings.MultiBindingHasNoConverter);
        }

        for (int i = 0; i < Bindings.Count; ++i)
        {
            CheckTrigger(Bindings[i]);
        }

        return MultiBindingExpression.CreateBindingExpression(dp, this, owner);
    }

    internal static void CheckTrigger(BindingBase bb)
    {
        if (bb is Binding binding)
        {
            if (binding.UpdateSourceTrigger != UpdateSourceTrigger.PropertyChanged &&
                binding.UpdateSourceTrigger != UpdateSourceTrigger.Default)
            {
                throw new InvalidOperationException(Strings.NoUpdateSourceTriggerForInnerBindingOfMultiBinding);
            }
        }
    }

    private void OnBindingCollectionChanged() => CheckSealed();
}
