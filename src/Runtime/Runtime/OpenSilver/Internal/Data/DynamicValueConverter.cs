// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: wrapper around default converter to dynamcially pick
//      and change value converters depending on changing source and target types
//

using System;
using System.Collections.Generic;
using System.Globalization;

#if MIGRATION
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace OpenSilver.Internal.Data
{
    // dynamically pick and switch a default value converter to convert between source and target type
    internal sealed class DynamicValueConverter : IValueConverter
    {
        internal DynamicValueConverter(bool targetToSourceNeeded)
        {
            _targetToSourceNeeded = targetToSourceNeeded;
        }

        internal DynamicValueConverter(bool targetToSourceNeeded, Type sourceType, Type targetType)
        {
            _targetToSourceNeeded = targetToSourceNeeded;
            EnsureConverter(sourceType, targetType);
        }

        internal object Convert(object value, Type targetType)
        {
#if MIGRATION
            return Convert(value, targetType, null, CultureInfo.InvariantCulture);
#else
            return Convert(value, targetType, null, string.Empty);
#endif
        }

#if MIGRATION
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            object result = DependencyProperty.UnsetValue;  // meaning: failure to convert

            if (value != null)
            {
                Type sourceType = value.GetType();
                EnsureConverter(sourceType, targetType);

                if (_converter != null)
                {
                    result = _converter.Convert(value, targetType, parameter, culture);
                }
            }
            else
            {
                if (!targetType.IsValueType)
                {
                    result = null;
                }
            }

            return result;
        }

#if MIGRATION
        public object ConvertBack(object value, Type sourceType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type sourceType, object parameter, string culture)
#endif
        {
            object result = DependencyProperty.UnsetValue;  // meaning: failure to convert

            if (value != null)
            {
                Type targetType = value.GetType();
                EnsureConverter(sourceType, targetType);

                if (_converter != null)
                {
                    result = _converter.ConvertBack(value, sourceType, parameter, culture);
                }
            }
            else
            {
                if (!sourceType.IsValueType)
                {
                    result = null;
                }
            }

            return result;
        }


        private void EnsureConverter(Type sourceType, Type targetType)
        {
            if ((_sourceType != sourceType) || (_targetType != targetType))
            {
                // types have changed - get a new converter

                if (sourceType != null && targetType != null)
                {
                    // DefaultValueConverter.Create() is more sophisticated to find correct type converters,
                    // e.g. if source/targetType is object or well-known system types.
                    // if there is any change in types, give that code to come up with the correct converter
                    _converter = GetValueConverter(sourceType, targetType, _targetToSourceNeeded);
                }
                else
                {
                    // if either type is null, no conversion is possible.
                    // Don't ask GetDefaultValueConverter - it will use null as a
                    // hashtable key, and crash (bug 110859).
                    _converter = null;
                }

                _sourceType = sourceType;
                _targetType = targetType;
            }
        }

        private static IValueConverter GetValueConverter(Type sourceType, Type targetType, bool targetToSourceNeeded)
        {
            IValueConverter converter = _converterStore[sourceType, targetType, targetToSourceNeeded];

            if (converter == null)
            {
                converter = DefaultValueConverter.Create(sourceType, targetType, targetToSourceNeeded);
                if (converter != null)
                {
                    _converterStore.Add(sourceType, targetType, targetToSourceNeeded, converter);
                }
            }

            return converter;
        }

        private Type _sourceType;
        private Type _targetType;
        private IValueConverter _converter;
        private bool _targetToSourceNeeded;

        private static readonly ValueConverterStore _converterStore = new ValueConverterStore();

        private class ValueConverterStore
        {
            private readonly Dictionary<Key, IValueConverter> _store = new Dictionary<Key, IValueConverter>();

            public IValueConverter this[Type sourceType, Type targetType, bool targetToSourceNeeded]
            {
                get
                {
                    if (_store.TryGetValue(new Key(sourceType, targetType, targetToSourceNeeded), out IValueConverter converter))
                    {
                        return converter;
                    }

                    return null;
                }
            }

            public void Add(Type sourceType, Type targetType, bool targetToSourceNeeded, IValueConverter converter)
            {
                _store.Add(new Key(sourceType, targetType, targetToSourceNeeded), converter);
            }

            private struct Key
            {
                private readonly Type _sourceType;
                private readonly Type _targetType;
                private readonly bool _targetToSourceNeeded;
                private readonly int _hashcode;

                public Key(Type sourceType, Type targetType, bool targetToSourceNeeded)
                {
                    _sourceType = sourceType;
                    _targetType = targetType;
                    _targetToSourceNeeded = targetToSourceNeeded;

                    _hashcode = _sourceType.GetHashCode() + _targetType.GetHashCode();
                }

                public override int GetHashCode() => _hashcode;

                public override bool Equals(object obj)
                {
                    if (obj is Key key)
                    {
                        return this == key;
                    }

                    return false;
                }

                public static bool operator ==(Key key1, Key key2)
                {
                    return key1._sourceType == key2._sourceType &&
                           key1._targetType == key2._targetType &&
                           key1._targetToSourceNeeded == key2._targetToSourceNeeded;
                }

                public static bool operator !=(Key key1, Key key2)
                {
                    return !(key1 == key2);
                }
            }
        }
    }
}
