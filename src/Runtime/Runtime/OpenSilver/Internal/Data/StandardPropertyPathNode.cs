
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

    private PropertyAccessor _accessor;
    private PropertyChangeListener _dpListener;
    private WeakEventToken _weakEventToken;

    internal StandardPropertyPathNode(BindingExpression listener, string typeName, string propertyName)
        : base(listener)
    {
        _resolvedType = typeName is not null ? Type.GetType(typeName) : null;
        _propertyName = propertyName;
    }

    public override Type Type => _accessor?.PropertyType;

    public override string PropertyName => _propertyName;

    public override bool IsBound => _accessor is not null;

    internal override void SetValue(object value) => _accessor?.SetValue(Source, value);

    internal override void OnUpdateValue()
    {
        if (_accessor is null)
        {
            UpdateValueAndIsBroken(DependencyProperty.UnsetValue, true);
        }
        else
        {
            UpdateValueAndIsBroken(_accessor.GetValue(Source), false);
        }
    }

    internal override void OnSourceChanged(object oldValue, object newValue)
    {
        if (_weakEventToken is not null)
        {
            _weakEventToken.Dispose();
            _weakEventToken = null;
        }

        if (_dpListener is PropertyChangeListener listener)
        {
            _dpListener = null;
            listener.Dispose();
        }

        ConnectToSource(newValue);
    }

    private void ConnectToSource(object source)
    {
        _accessor = GetAccessor(source, out DependencyProperty dp);

        if (_accessor is null) return;

        if (Listener.IsDynamic)
        {
            if (source is INotifyPropertyChanged inpc)
            {
                _weakEventToken = WeakEvent.Subscribe<StandardPropertyPathNode, INotifyPropertyChanged, PropertyChangedEventArgs>(
                    this,
                    inpc,
                    static (instance, source, args) => instance.OnPropertyChanged(source, args),
                    static (handler, source) => source.PropertyChanged -= new PropertyChangedEventHandler(handler),
                    static (handler, source) => source.PropertyChanged += new PropertyChangedEventHandler(handler));
            }

            if (dp is not null)
            {
                _dpListener = PropertyChangeListener.CreateListener((DependencyObject)source, dp, OnPropertyChanged);
            }
        }
    }

    private PropertyAccessor GetAccessor(object source, out DependencyProperty dependencyProperty)
    {
        if (source is null)
        {
            dependencyProperty = null;
            return null;
        }

        if (source is DependencyObject)
        {
            // A path can name the owner explicitly. Otherwise WPF resolves the dependency
            // property on the custom reflection type. TypeDelegator.BaseType skips the
            // wrapped CLR type, so try that type too and keep Silverlight dependency properties.
            Type reflectionType = _resolvedType ?? TypeHelper.GetReflectionType(source);
            if (DependencyProperty.FromName(_propertyName, reflectionType) is DependencyProperty dp)
            {
                dependencyProperty = dp;
                return new DependencyPropertyAccessor(dp);
            }

            Type clrType = source.GetType();
            if (_resolvedType is null && reflectionType != clrType &&
                DependencyProperty.FromName(_propertyName, clrType) is DependencyProperty clrProperty)
            {
                dependencyProperty = clrProperty;
                return new DependencyPropertyAccessor(clrProperty);
            }
        }

        dependencyProperty = null;

        Type sourceType = TypeHelper.GetReflectionType(source);

        if (GetPropertyInfo(sourceType, _propertyName) is PropertyInfo propertyInfo)
        {
            return new ClrPropertyAccessor(propertyInfo);
        }

        if (sourceType.GetField(_propertyName) is FieldInfo fieldInfo)
        {
            return new ClrFieldAccessor(fieldInfo);
        }

        if (IsIDynamicMetaObjectProvider(source))
        {
            return new DynamicPropertyAccessor(_propertyName);
        }

        return null;
    }

    private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
    {
        const BindingFlags Lookup = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        for (Type t = type; t is not null; t = t.BaseType)
        {
            if (t.GetProperty(propertyName, Lookup) is PropertyInfo propertyInfo)
            {
                return propertyInfo;
            }
        }

        return null;
    }

    private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => UpdateValue(true);

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName == _propertyName || string.IsNullOrEmpty(e.PropertyName)) && _accessor is not null)
        {
            UpdateValue(true);
        }
    }
}
