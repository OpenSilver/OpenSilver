

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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using OpenSilver.Internal.Data;

namespace System.Windows;

/// <summary>
/// Supports template binding.
/// </summary>
public sealed class TemplateBindingExpression : Expression
{
    private readonly DependencyObject _source;
    private readonly DependencyProperty _sourceProperty;
    private DependencyObject _target;
    private DependencyProperty _targetProperty;
    private PropertyChangeListener _listener;
    private bool _skipTypeCheck;

    internal TemplateBindingExpression(DependencyObject templatedParent, DependencyProperty sourceDP, TemplateBindingExtension extension)
    {
        Debug.Assert(extension is not null);
        Debug.Assert(templatedParent is not null);
        Debug.Assert(sourceDP is not null);

        _source = templatedParent;
        _sourceProperty = sourceDP;
        TemplateBindingExtension = extension;
    }

    /// <summary>
    /// Gets the <see cref="Windows.TemplateBindingExtension"/> object of this expression instance.
    /// </summary>
    /// <returns>
    /// The template binding extension of this expression instance.
    /// </returns>
    public TemplateBindingExtension TemplateBindingExtension { get; }

    internal override bool CanSetValue(DependencyObject d, DependencyProperty dp) => false;

    internal override object GetValue(DependencyObject d, DependencyProperty dp)
    {
        var value = _source.GetValue(_sourceProperty);

        if (TemplateBindingExtension.Converter is IValueConverter converter)
        {
            value = converter.Convert(
                value,
                _targetProperty.PropertyType,
                TemplateBindingExtension.ConverterParameter,
                GetCulture());
        }

        if (_skipTypeCheck || ValidateValue(ref value, dp))
        {
            return value;
        }

        // Note: consider caching the default value as we should always have d == Target.
        return _targetProperty.GetDefaultValue(_target);
    }

    internal override void OnAttach(DependencyObject d, DependencyProperty dp)
    {
        Debug.Assert(d != null);
        Debug.Assert(dp != null);

        _target = d;
        _targetProperty = dp;

        _skipTypeCheck = TemplateBindingExtension.Converter is null && _targetProperty.PropertyType.IsAssignableFrom(_sourceProperty.PropertyType);
        _listener = PropertyChangeListener.CreateListener(_source, _sourceProperty, OnPropertyChanged);
    }

    internal override void OnDetach(DependencyObject d, DependencyProperty dp)
    {
        _skipTypeCheck = false;
        var listener = _listener;
        _listener = null;
        listener?.Dispose();
    }

    private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        _target.ApplyExpression(_targetProperty, this);
    }

    private bool ValidateValue(ref object value, DependencyProperty targetProperty)
    {
        if (targetProperty.IsValidValue(value))
        {
            return true;
        }

        if (value != null
            && _sourceProperty == ContentControl.ContentProperty
            && TypeConverterHelper.IsCoreType(targetProperty.OwnerType))
        {
            TypeConverter converter = TypeConverterHelper.GetBuiltInConverter(targetProperty.PropertyType);
            if (converter?.CanConvertFrom(value.GetType()) ?? false)
            {
                try
                {
                    value = converter.ConvertFrom(value);
                    return true;
                }
                catch { }
            }
        }

        return false;
    }

    private CultureInfo GetCulture()
    {
        if (_target.GetValue(FrameworkElement.LanguageProperty) is XmlLanguage xmlLanguage)
        {
            return xmlLanguage.GetCompatibleCulture();
        }

        return null;
    }
}
