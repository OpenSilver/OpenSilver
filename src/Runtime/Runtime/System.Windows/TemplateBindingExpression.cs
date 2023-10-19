

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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using OpenSilver.Internal.Data;

namespace System.Windows
{
    /// <summary>
    /// Supports template binding.
    /// </summary>
    public class TemplateBindingExpression : Expression
    {
        private readonly IInternalControl _source;
        private readonly DependencyProperty _sourceProperty;
        private DependencyObject _target;
        private DependencyProperty _targetProperty;
        private DependencyPropertyChangedListener _listener;
        private bool _skipTypeCheck;

        internal TemplateBindingExpression(IInternalControl templatedParent, DependencyProperty sourceDP)
        {
            _source = templatedParent ?? throw new ArgumentNullException(nameof(templatedParent));
            _sourceProperty = sourceDP ?? throw new ArgumentNullException(nameof(sourceDP));
        }

        internal override bool CanSetValue(DependencyObject d, DependencyProperty dp)
        {
            return false;
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            var value = _source.GetValue(_sourceProperty);
            if (_skipTypeCheck || ValidateValue(ref value, dp))
            {
                return value;
            }

            // Note: consider caching the default value as we should always have d == Target.
            return _targetProperty.GetDefaultValue(_target);
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            if (IsAttached)
                return;

            Debug.Assert(d != null);
            Debug.Assert(dp != null);

            IsAttached = true;

            _target = d;
            _targetProperty = dp;

            _skipTypeCheck = _targetProperty.PropertyType.IsAssignableFrom(_sourceProperty.PropertyType);
            _listener = new DependencyPropertyChangedListener((DependencyObject)_source, _sourceProperty, OnPropertyChanged);
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            if (!IsAttached)
                return;

            IsAttached = false;

            _skipTypeCheck = false;
            var listener = _listener;
            _listener = null;
            listener?.Dispose();
        }

        private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            _target.ApplyExpression(_targetProperty, this, false);
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
    }
}
