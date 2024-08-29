
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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using OpenSilver.Internal.Data;

namespace System.Windows.Data;

/// <summary>
/// Contains instance information about a single instance of a <see cref="MultiBinding"/>.
/// </summary>
public sealed class MultiBindingExpression : BindingExpressionBase
{
    private readonly BindingExpressionBase[] _mutableBindingExpressions;
    private readonly object[] _values;

    private MultiBindingExpression(MultiBinding binding, BindingExpressionBase owner)
        : base(binding, owner)
    {
        Debug.Assert(binding is not null);

        _mutableBindingExpressions = new BindingExpressionBase[binding.Bindings.Count];
        _values = new object[binding.Bindings.Count];
    }

    // Create a new BindingExpression from the given Binding description
    internal static MultiBindingExpression CreateBindingExpression(DependencyProperty dp, MultiBinding binding, BindingExpressionBase owner)
    {
        if (dp.ReadOnly)
        {
            throw new ArgumentException($"'{dp.Name}' property cannot be data-bound.", nameof(dp));
        }

        // create the BindingExpression
        return new MultiBindingExpression(binding, owner);
    }

    /// <summary>
    /// Gets the collection of <see cref="BindingExpression"/> objects in this instance of 
    /// <see cref="MultiBindingExpression"/>.
    /// </summary>
    /// <returns>
    /// A read-only collection of the <see cref="BindingExpression"/> objects. Even though the 
    /// return type is a collection of <see cref="BindingExpressionBase"/> objects the returned 
    /// collection would only contain <see cref="BindingExpression"/> objects because the 
    /// <see cref="MultiBinding"/> class currently only supports <see cref="Binding"/> objects.
    /// </returns>
    public ReadOnlyCollection<BindingExpressionBase> BindingExpressions => Array.AsReadOnly(_mutableBindingExpressions);

    /// <summary>
    /// Gets the <see cref="MultiBinding"/> object from which this <see cref="MultiBindingExpression"/> is created.
    /// </summary>
    /// <returns>
    /// The <see cref="MultiBinding"/> object from which this <see cref="MultiBindingExpression"/> is created.
    /// </returns>
    public MultiBinding ParentMultiBinding => Unsafe.As<MultiBinding>(ParentBindingBase);

    /// <summary>
    /// Sends the current binding target value to the binding source properties in 
    /// <see cref="BindingMode.TwoWay"/> bindings.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="MultiBindingExpression"/> is detached from the binding target.
    /// </exception>
    public void UpdateSource()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("The Binding has been detached from its target.");
        }

        NeedsUpdate = true;
        Update();
    }

    internal override void Update()
    {
        if (!NeedsUpdate || !IsReflective || IsInTransfer)
        {
            return;
        }

        object value = GetRawProposedValue();

        value = ConvertProposedValue(value);

        if (value == DependencyProperty.UnsetValue)
        {
            return;
        }

        UpdateSource(value);
    }

    private object GetRawProposedValue() => Target.GetValue(TargetProperty);

    private object ConvertProposedValue(object value)
    {
        bool success = ConvertProposedValueImpl(value, out object result);

        // if the conversion failed, signal a validation error
        if (!success)
        {
            result = DependencyProperty.UnsetValue;
        }

        return result;
    }

    private bool ConvertProposedValueImpl(object value, out object result)
    {
        result = GetValuesForChildBindings(value);

        if (result == DependencyProperty.UnsetValue)
        {
            return false;
        }

        if (result is not object[] values)
        {
            result = DependencyProperty.UnsetValue;
            return false;
        }

        int count = Math.Min(_mutableBindingExpressions.Length, values.Length);

        // using the result of ConvertBack as the raw value, run each child binding
        // through the first two steps of the update/validate process
        bool success = true;
        for (int i = 0; i < count; ++i)
        {
            value = values[i];

            if (value != DependencyProperty.UnsetValue)
            {
                if (_mutableBindingExpressions[i] is BindingExpression bindExpr)
                {
                    bindExpr.SetValue(value);
                    value = bindExpr.GetValue(Target, TargetProperty);
                }
            }

            if (value == DependencyProperty.UnsetValue)
            {
                success = false;
            }

            values[i] = value;
        }

        result = values;
        return success;
    }

    private object GetValuesForChildBindings(object rawValue)
    {
        if (ParentMultiBinding.Converter is null)
        {
            return DependencyProperty.UnsetValue;
        }

        var targetTypes = new Type[_mutableBindingExpressions.Length];

        for (int i = 0; i < _mutableBindingExpressions.Length; i++)
        {
            if (_mutableBindingExpressions[i] is BindingExpression be && be.ParentBinding.Converter is null)
            {
                targetTypes[i] = be.ConverterSourceType;
            }
            else
            {
                targetTypes[i] = TargetProperty.PropertyType;
            }
        }

        // MultiValueConverters are always user-defined, so don't catch exceptions
        return ParentMultiBinding.Converter.ConvertBack(
            rawValue,
            targetTypes,
            ParentMultiBinding.ConverterParameter,
            ParentMultiBinding.ConverterCulture);
    }

    private void UpdateSource(object convertedValue)
    {
        object[] values = convertedValue as object[];
        int count = Math.Min(_mutableBindingExpressions.Length, values.Length);

        BeginSourceUpdate();

        try
        {
            for (int i = 0; i < count; i++)
            {
                object value = values[i];

                if (_mutableBindingExpressions[i] is BindingExpression bindExpr)
                {
                    bindExpr.UpdateSource(value);
                }
            }
        }
        finally
        {
            EndSourceUpdate();
        }
    }

    internal override bool CanSetValue(DependencyObject d, DependencyProperty dp) => IsReflective;

    internal override object GetValue(DependencyObject d, DependencyProperty dp)
    {
        for (int i = 0; i < _values.Length; i++)
        {
            _values[i] = _mutableBindingExpressions[i].GetValue(Target, TargetProperty);
        }

        object value = _values;

        if (ParentMultiBinding.Converter is not null)
        {
            value = ParentMultiBinding.Converter.Convert(_values,
                TargetProperty.PropertyType,
                ParentMultiBinding.ConverterParameter,
                ParentMultiBinding.ConverterCulture);
        }

        string stringFormat = GetEffectiveStringFormat();
        if (stringFormat is not null)
        {
            try
            {
                if (value == _values)
                {
                    value = string.Format(ParentMultiBinding.ConverterCulture, stringFormat, _values);
                }
                else
                {
                    value = string.Format(ParentMultiBinding.ConverterCulture, stringFormat, value);
                }
            }
            catch (FormatException fe)
            {
                // formatting didn't work
                HandleException(fe);
                value = DependencyProperty.UnsetValue;
            }
        }

        if (value is null)
        {
            if (ParentBindingBase.TargetNullValue is not null)
            {
                value = ConvertValue(ParentBindingBase.TargetNullValue, TargetProperty);
            }
        }

        // if the value isn't acceptable to the target property, don't use it
        if (value != DependencyProperty.UnsetValue && !TargetProperty.IsValidValue(value))
        {
            value = DependencyProperty.UnsetValue;
        }

        // if we can't obtain a value, try the fallback value.
        if (value == DependencyProperty.UnsetValue)
        {
            value = UseFallbackValue();
        }

        return value;
    }

    internal override void AttachOverride(DependencyObject d, DependencyProperty dp)
    {
        base.AttachOverride(d, dp);

        TransferIsDeferred = true;

        for (int i = 0; i < ParentMultiBinding.Bindings.Count; i++)
        {
            _mutableBindingExpressions[i] = AttachBindingExpression(ParentMultiBinding.Bindings[i]);
        }

        TransferIsDeferred = false;
    }

    internal override void DetachOverride()
    {
        foreach (BindingExpressionBase bindExpr in _mutableBindingExpressions)
        {
            bindExpr.MarkDetached();
            bindExpr.OnDetach(Target, TargetProperty);
        }

        Array.Clear(_mutableBindingExpressions, 0, _mutableBindingExpressions.Length);
        Array.Clear(_values, 0, _values.Length);

        base.DetachOverride();
    }

    /// <summary>
    /// Invalidate the given child expression.
    /// </summary>
    internal override void InvalidateChild(BindingExpressionBase bindingExpression)
    {
        if (_values is null) return;

        int index = Array.IndexOf(_mutableBindingExpressions, bindingExpression);

        if (index >= 0 && IsDynamic)
        {
            NeedsDataTransfer = true;
            Transfer();
        }
    }

    private void Transfer()
    {
        if (NeedsDataTransfer && !TransferIsDeferred)
        {
            TransferValue();
        }
    }

    // transfer a value from the source to the target
    private void TransferValue()
    {
        IsInTransfer = true;
        NeedsDataTransfer = false;

        Invalidate();

        IsInTransfer = false;
    }

    // Create a BindingExpression for position i
    private BindingExpressionBase AttachBindingExpression(BindingBase binding)
    {
        // Check if replacement bindings have the correct UpdateSourceTrigger
        MultiBinding.CheckTrigger(binding);

        BindingExpressionBase bindExpr = binding.CreateBindingExpression(Target, TargetProperty, this);
        bindExpr.IsInMultiBindingExpression = true;

        bindExpr.MarkAttached();
        bindExpr.OnAttach(Target, TargetProperty);

        return bindExpr;
    }

    private static object ConvertValue(object value, DependencyProperty dp)
    {
        object result;

        if (value == DependencyProperty.UnsetValue || dp.IsValidValue(value))
        {
            result = value;
        }
        else
        {
            result = null;
            bool success = false;
            TypeConverter converter = DefaultValueConverter.GetConverter(dp.PropertyType);
            if (converter != null && converter.CanConvertFrom(value.GetType()))
            {
                try
                {
                    result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                    success = dp.IsValidValue(result);
                }
                catch
                {
                }
            }

            if (!success)
            {
                // if can't convert it, don't use it
                result = DependencyProperty.UnsetValue;
            }
        }

        return result;
    }

    private object UseFallbackValue()
    {
        object value = DependencyProperty.UnsetValue;

        if (ParentBindingBase.FallbackValue is not null)
        {
            value = ConvertValue(ParentBindingBase.FallbackValue, TargetProperty);
        }

        if (value == DependencyProperty.UnsetValue)
        {
            value = DefaultValue;
        }

        return value;
    }
}
