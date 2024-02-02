
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

namespace OpenSilver.Internal.Data
{
    internal sealed class IndexedPropertyPathNode : PropertyPathNode
    {
        private const string IndexerPropertyName = "Item[]";

        private readonly string _indexStr;
        private readonly object[] _index;
        private PropertyInfo _indexer;
        private WeakEventListener<IndexedPropertyPathNode, INotifyPropertyChanged, PropertyChangedEventArgs> _propertyChangedListener;

        private static readonly PropertyInfo _iListIndexer = GetIListIndexer();

        internal IndexedPropertyPathNode(PropertyPathWalker listener, string index)
            : base(listener)
        {
            _indexStr = index;
            _index = new object[1] { index };
        }

        public override Type Type => _indexer?.PropertyType;

        public override string PropertyName => $"{_indexer?.Name ?? string.Empty}[{_indexStr}]";

        public override bool IsBound => _indexer != null;

        internal override void OnSourceChanged(object oldValue, object newValue)
        {
            if (Listener.ListenForChanges)
            {
                if (_propertyChangedListener != null)
                {
                    _propertyChangedListener.Detach();
                    _propertyChangedListener = null;
                }

                if (newValue is INotifyPropertyChanged inpc)
                {
                    _propertyChangedListener = new(this, inpc)
                    {
                        OnEventAction = static (instance, source, args) => instance.OnPropertyChanged(source, args),
                        OnDetachAction = static (listener, source) => source.PropertyChanged -= listener.OnEvent,
                    };
                    inpc.PropertyChanged += _propertyChangedListener.OnEvent;
                }
            }

            // todo: (?) find out how to have a listener here since it
            // is a method and not a DependencyProperty (get_Item and
            // set_Item). I guess it would be nice to be able to attach
            // to calls on set_item and handle it from there.
            _indexer = null;

            if (newValue != null)
            {
                FindIndexer(newValue.GetType());
            }
        }

        internal override void SetValue(object value)
        {
            if (_indexer != null)
            {
                TryInvokeSetMethod(value);
            }
        }

        internal override void OnUpdateValue()
        {
            if (_indexer != null)
            {
                if (TryInvokeGetMethod(out object result))
                {
                    UpdateValueAndIsBroken(result, false);
                }
            }
        }

        private bool TryInvokeSetMethod(object value)
        {
            try
            {
                _indexer.SetValue(Source, value, _index);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryInvokeGetMethod(out object result)
        {
            try
            {
                result = _indexer.GetValue(Source, _index);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IndexerPropertyName)
            {
                UpdateValue();
            }
        }

        private void FindIndexer(Type type)
        {
            // 1 - Look for an Int32 indexer
            // 2 - Look for a String indexer
            // 3 - Use indexer from IList if the Binding source implement the interface
            foreach (MemberInfo member in type.GetDefaultMembers())
            {
                if (member is not PropertyInfo property)
                    continue;

                ParameterInfo[] parameters = property.GetIndexParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                {
                    if (int.TryParse(_indexStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                    {
                        _indexer = property;
                        _index[0] = value;
                        break;
                    }
                }
                else if (parameters[0].ParameterType == typeof(string))
                {
                    _indexer = property;
                    _index[0] = _indexStr;
                    // Do not exit the loop because we can still find an Int32 indexer,
                    // which takes priority over this one.
                }
            }

            if (_indexer == null)
            {
                if (type is IList)
                {
                    _indexer = _iListIndexer;
                    _index[0] = int.Parse(_indexStr, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }
        }

        private static PropertyInfo GetIListIndexer()
        {
            return typeof(IList).GetDefaultMembers()[0] as PropertyInfo;
        }
    }
}
