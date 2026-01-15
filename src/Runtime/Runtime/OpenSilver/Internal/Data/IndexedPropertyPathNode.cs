
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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal sealed class IndexedPropertyPathNode : PropertyPathNode
{
    private const string IndexerPropertyName = "Item[]";

    private readonly string _indexStr;
    private readonly object[] _indexerArguments;
    private IndexerAccessor _accessor;
    private WeakEventToken _weakEventToken;

    private static readonly PropertyInfo _iListIndexer = typeof(IList).GetDefaultMembers()[0] as PropertyInfo;

    internal IndexedPropertyPathNode(BindingExpression listener, string index)
        : base(listener)
    {
        _indexStr = index;
        _indexerArguments = [index];
    }

    public override Type Type => _accessor?.PropertyType;

    public override string PropertyName => _accessor?.GetPropertyName(_indexStr) ?? $"[{_indexStr}]";

    public override bool IsBound => _accessor is not null;

    internal override void SetValue(object value) => _accessor?.SetValue(Source, _indexerArguments, value);

    internal override void OnUpdateValue()
    {
        if (_accessor is null)
        {
            UpdateValueAndIsBroken(DependencyProperty.UnsetValue, true);
        }
        else
        {
            object value = _accessor.GetValue(Source, _indexerArguments);
            UpdateValueAndIsBroken(value, value == DependencyProperty.UnsetValue);
        }
    }

    internal override void OnSourceChanged(object oldValue, object newValue)
    {
        if (_weakEventToken is not null)
        {
            _weakEventToken.Dispose();
            _weakEventToken = null;
        }

        ConnectToSource(newValue);
    }

    private void ConnectToSource(object source)
    {
        _accessor = GetAccessor(source, out object indexerArgument);
        _indexerArguments[0] = indexerArgument;

        if (_accessor is null) return;

        if (Listener.IsDynamic)
        {
            if (source is INotifyPropertyChanged inpc)
            {
                _weakEventToken = WeakEvent.Subscribe<IndexedPropertyPathNode, INotifyPropertyChanged, PropertyChangedEventArgs>(
                    this,
                    inpc,
                    static (instance, source, args) => instance.OnPropertyChanged(source, args),
                    static (handler, source) => source.PropertyChanged -= new PropertyChangedEventHandler(handler),
                    static (handler, source) => source.PropertyChanged += new PropertyChangedEventHandler(handler));
            }
        }
    }

    private IndexerAccessor GetAccessor(object source, out object indexerArgument)
    {
        if (source is null)
        {
            indexerArgument = _indexStr;
            return null;
        }

        int index;
        PropertyInfo indexer = null;
        Type type = source.GetType();

        // 1 - Look for an Int32 indexer
        // 2 - Look for a String indexer
        // 3 - Use indexer from IList if the Binding source implement the interface
        foreach (MemberInfo member in type.GetDefaultMembers())
        {
            if (member is not PropertyInfo property)
            {
                continue;
            }

            ParameterInfo[] parameters = property.GetIndexParameters();
            if (parameters.Length != 1)
            {
                continue;
            }

            if (parameters[0].ParameterType == typeof(int))
            {
                if (int.TryParse(_indexStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                {
                    indexerArgument = index;
                    return new ClrIndexerAccessor(property);
                }
            }
            else if (parameters[0].ParameterType == typeof(string))
            {
                indexer = property;
                // Do not exit the loop because we can still find an Int32 indexer,
                // which takes priority over this one.
            }
        }

        if (indexer is not null)
        {
            indexerArgument = _indexStr;
            return new ClrIndexerAccessor(indexer);
        }

        if (source is IList && int.TryParse(_indexStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
        {
            indexerArgument = index;
            return new ClrIndexerAccessor(_iListIndexer);
        }

        if (IsIDynamicMetaObjectProvider(source))
        {
            indexerArgument = _indexStr;
            return DynamicIndexerAccessor.GetIndexerAccessor(_indexerArguments.Length);
        }

        indexerArgument = _indexStr;
        return null;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == IndexerPropertyName && _accessor is not null)
        {
            UpdateValue(true);
        }
    }
}
