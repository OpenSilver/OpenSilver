
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
using System.Globalization;
using System.Reflection;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal sealed class IndexedPropertyPathNode : PropertyPathNode
    {
        private readonly string _index;
        private int? _intIndex;
        private PropertyInfo _indexer;

        private static readonly PropertyInfo _iListIndexer = GetIListIndexer();

        internal IndexedPropertyPathNode(PropertyPathWalker listener, string index)
            : base(listener)
        {
            _index = index;
        }

        public override Type Type => _indexer?.PropertyType;

        private object Index
        {
            get
            {
                if (_intIndex.HasValue)
                {
                    return _intIndex.Value;
                }

                return _index;
            }
        }

        internal override void OnSourceChanged(object oldvalue, object newValue)
        {
            // todo: (?) find out how to have a listener here since it
            // is a method and not a DependencyProperty (get_Item and
            // set_Item). I guess it would be nice to be able to attach
            // to calls on set_item and handle it from there.
            _indexer = null;
            _intIndex = null;

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

        internal override void UpdateValue()
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
                _indexer.SetValue(Source, value, new object[1] { Index });
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
                result = _indexer.GetValue(Source, new object[1] { Index });
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

#if NETSTANDARD
        private void FindIndexer(Type type)
        {
            // 1 - Look for an Int32 indexer
            // 2 - Look for a String indexer
            // 3 - Use indexer from IList if the Binding source implement the interface
            foreach (MemberInfo member in type.GetDefaultMembers())
            {
                if (!(member is PropertyInfo property))
                    continue;

                ParameterInfo[] parameters = property.GetIndexParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                {
                    if (int.TryParse(_index, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                    {
                        _indexer = property;
                        _intIndex = value;
                        break;
                    }
                }
                else if (parameters[0].ParameterType == typeof(string))
                {
                    _indexer = property;
                    _intIndex = null;
                    // Do not exit the loop because we can still find an Int32 indexer,
                    // which takes priority over this one.
                }
            }

            if (_indexer == null)
            {
                if (type is IList)
                {
                    _indexer = _iListIndexer;
                    _intIndex = int.Parse(_index, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }
           
            if (_indexer == null)
            {
                throw new NotSupportedException("Only String and Int32 indexers with one parameters are supported.");
            }
        }

        private static PropertyInfo GetIListIndexer()
        {
            return typeof(IList).GetDefaultMembers()[0] as PropertyInfo;
        }
#elif BRIDGE
        private void FindIndexer(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                ParameterInfo[] parameters = property.GetIndexParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                {
                    if (int.TryParse(_index, out int value))
                    {
                        _indexer = property;
                        _intIndex = value;
                        break;
                    }
                }
                else if (parameters[0].ParameterType == typeof(string))
                {
                    _indexer = property;
                    _intIndex = null;
                    // Do not exit the loop because we can still find an Int32 indexer,
                    // which takes priority over this one.
                }
            }

            if (_indexer == null)
            {
                if (type is IList)
                {
                    _indexer = _iListIndexer;
                    _intIndex = int.Parse(_index);
                }
            }

            if (_indexer == null)
            {
                throw new NotSupportedException("Only String and Int32 indexers with one parameters are supported.");
            }
        }

        private static PropertyInfo GetIListIndexer()
        {
            return typeof(IList).GetProperty("Item");
        }
#endif
    }
}
