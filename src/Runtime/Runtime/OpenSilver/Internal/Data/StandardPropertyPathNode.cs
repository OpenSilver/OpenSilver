
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
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal sealed class StandardPropertyPathNode : PropertyPathNode
{
    private readonly Type _resolvedType;
    private readonly string _propertyName;

    private DependencyPropertyChangedListener _dpListener;
    private WeakEventListener<StandardPropertyPathNode, INotifyPropertyChanged, PropertyChangedEventArgs> _propertyChangedListener;
    private DependencyProperty _dp;
    private PropertyInfo _prop;
    private FieldInfo _field;

    internal StandardPropertyPathNode(BindingExpression listener, string typeName, string propertyName)
        : base(listener)
    {
        _resolvedType = typeName is not null ? Type.GetType(typeName) : null;
        _propertyName = propertyName;
    }

    public override Type Type
    {
        get
        {
            if (_dp is not null)
            {
                return _dp.PropertyType;
            }

            if (_prop is not null)
            {
                return _prop.PropertyType;
            }

            if (_field is not null)
            {
                return _field.FieldType;
            }

            return null;
        }
    }

    public override string PropertyName => _propertyName;

    public override bool IsBound => _dp is not null || _prop is not null;

    internal override void SetValue(object value)
    {
        if (_dp is not null)
        {
            ((DependencyObject)Source).SetValue(_dp, value);
        }
        else if (_prop is not null)
        {
            _prop.SetValue(Source, value);
        }
        else if (_field is not null)
        {
            _field.SetValue(Source, value);
        }
    }

    internal override void OnUpdateValue()
    {
        object value;
        if (_dp is not null)
        {
            value = ((DependencyObject)Source).GetValue(_dp);
        }
        else if (_prop is not null)
        {
            value = _prop.GetValue(Source);
        }
        else if (_field is not null)
        {
            value = _field.GetValue(Source);
        }
        else
        {
            value = DependencyProperty.UnsetValue;
        }

        UpdateValueAndIsBroken(value, CheckIsBroken());
    }

    internal override void OnSourceChanged(object oldValue, object newValue)
    {
        if (_propertyChangedListener is not null)
        {
            _propertyChangedListener.Detach();
            _propertyChangedListener = null;
        }

        if (_dpListener is DependencyPropertyChangedListener listener)
        {
            _dpListener = null;
            listener.Dispose();
        }

        _dp = null;
        _prop = null;
        _field = null;

        if (Source is null) return;

        var sourceDO = newValue as DependencyObject;

        if (sourceDO is not null)
        {
            Type type = _resolvedType ?? Source.GetType();
            _dp = DependencyProperty.FromName(_propertyName, type);
        }

        if (_dp is null)
        {
            Type sourceType = Source.GetType();
            for (Type t = sourceType; t is not null; t = t.BaseType)
            {
                _prop = t.GetProperty(
                    _propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                if (_prop is not null)
                {
                    break;
                }
            }

            if (_prop is null)
            {
                // Try in case it is a simple field instead of a property:
                _field = sourceType.GetField(_propertyName);
            }
        }

        if (Listener.IsDynamic)
        {
            if (newValue is INotifyPropertyChanged inpc)
            {
                _propertyChangedListener = new(this, inpc)
                {
                    OnEventAction = static (instance, source, args) => instance.OnPropertyChanged(source, args),
                    OnDetachAction = static (listener, source) => source.PropertyChanged -= listener.OnEvent,
                };
                inpc.PropertyChanged += _propertyChangedListener.OnEvent;
            }

            if (_dp is not null)
            {
                _dpListener = new DependencyPropertyChangedListener(sourceDO, _dp, OnPropertyChanged);
            }
        }
    }

    private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => UpdateValue(true);

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName == _propertyName || string.IsNullOrEmpty(e.PropertyName)) && (_prop is not null || _field is not null))
        {
            UpdateValue(true);
        }
    }

    private bool CheckIsBroken() => Source is null || (_prop is null && _field is null && _dp is null);
}
