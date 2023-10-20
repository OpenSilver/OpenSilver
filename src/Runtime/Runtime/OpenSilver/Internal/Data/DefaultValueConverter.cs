// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Provide default conversion between source values and
//              target values, for data binding.  The default ValueConverter
//              typically wraps a type converter.
//

using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace OpenSilver.Internal.Data
{
    internal class DefaultValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        protected DefaultValueConverter(TypeConverter typeConverter, Type sourceType, Type targetType,
                                        bool shouldConvertFrom, bool shouldConvertTo)
        {
            _typeConverter = typeConverter;
            _sourceType = sourceType;
            _targetType = targetType;
            _shouldConvertFrom = shouldConvertFrom;
            _shouldConvertTo = shouldConvertTo;
        }

        //------------------------------------------------------
        //
        //  Internal static API
        //
        //------------------------------------------------------

        // static constructor - returns a ValueConverter suitable for converting between
        // the source and target.  The flag indicates whether targetToSource
        // conversions are actually needed.
        // if no Converter is needed, return DefaultValueConverter.ValueConverterNotNeeded marker.
        // if unable to create a DefaultValueConverter, return null to indicate error.
        internal static IValueConverter Create(Type sourceType,
                                                Type targetType,
                                                bool targetToSource)
        {
            TypeConverter typeConverter;
            Type innerType;
            bool canConvertTo, canConvertFrom;
            bool sourceIsNullable = false;
            bool targetIsNullable = false;

            // sometimes, no conversion is necessary
            if (sourceType == targetType ||
                (!targetToSource && targetType.IsAssignableFrom(sourceType)))
            {
                return ValueConverterNotNeeded;
            }

            // the type convert for System.Object is useless.  It claims it can
            // convert from string, but then throws an exception when asked to do
            // so.  So we work around it.
            if (targetType == typeof(object))
            {
                // The sourceType here might be a Nullable type: consider using
                // NullableConverter when appropriate. (uncomment following lines)
                //Type innerType = Nullable.GetUnderlyingType(sourceType);
                //if (innerType != null)
                //{
                //    return new NullableConverter(new ObjectTargetConverter(innerType),
                //                                 innerType, targetType, true, false);
                //}

                // BUG: 1109257 ObjectTargetConverter is not the best converter possible.
                return new ObjectTargetConverter(sourceType);
            }
            else if (sourceType == typeof(object))
            {
                // The targetType here might be a Nullable type: consider using
                // NullableConverter when appropriate. (uncomment following lines)
                //Type innerType = Nullable.GetUnderlyingType(targetType);
                // if (innerType != null)
                // {
                //     return new NullableConverter(new ObjectSourceConverter(innerType),
                //                                  sourceType, innerType, false, true);
                // }

                // BUG: 1109257 ObjectSourceConverter is not the best converter possible.
                return new ObjectSourceConverter(targetType);
            }

            // use System.Convert for well-known base types
            if (SystemConvertConverter.CanConvert(sourceType, targetType))
            {
                return new SystemConvertConverter(sourceType, targetType);
            }

            // Need to check for nullable types first, since NullableConverter is a bit over-eager;
            // TypeConverter for Nullable can convert e.g. Nullable<DateTime> to string
            // but it ends up doing a different conversion than the TypeConverter for the
            // generic's inner type, e.g. bug 1361977
            innerType = Nullable.GetUnderlyingType(sourceType);
            if (innerType != null)
            {
                sourceType = innerType;
                sourceIsNullable = true;
            }
            innerType = Nullable.GetUnderlyingType(targetType);
            if (innerType != null)
            {
                targetType = innerType;
                targetIsNullable = true;
            }
            if (sourceIsNullable || targetIsNullable)
            {
                // single-level recursive call to try to find a converter for basic value types
                return Create(sourceType, targetType, targetToSource);
            }

            // special case for converting IListSource to IList
            if (typeof(IListSource).IsAssignableFrom(sourceType) &&
                targetType.IsAssignableFrom(typeof(IList)))
            {
                return new ListSourceConverter();
            }

            // Interfaces are best handled on a per-instance basis.  The type may
            // not implement the interface, but an instance of a derived type may.
            if (sourceType.IsInterface || targetType.IsInterface)
            {
                return new InterfaceConverter(sourceType, targetType);
            }

            // try using the source's type converter
            typeConverter = GetConverter(sourceType);
            canConvertTo = (typeConverter != null) ? typeConverter.CanConvertTo(targetType) : false;
            canConvertFrom = (typeConverter != null) ? typeConverter.CanConvertFrom(targetType) : false;

            if ((canConvertTo || targetType.IsAssignableFrom(sourceType)) &&
                (!targetToSource || canConvertFrom || sourceType.IsAssignableFrom(targetType)))
            {
                return new SourceDefaultValueConverter(typeConverter, sourceType, targetType,
                                                       targetToSource && canConvertFrom, canConvertTo);
            }

            // if that doesn't work, try using the target's type converter
            typeConverter = GetConverter(targetType);
            canConvertTo = (typeConverter != null) ? typeConverter.CanConvertTo(sourceType) : false;
            canConvertFrom = (typeConverter != null) ? typeConverter.CanConvertFrom(sourceType) : false;

            if ((canConvertFrom || targetType.IsAssignableFrom(sourceType)) &&
                (!targetToSource || canConvertTo || sourceType.IsAssignableFrom(targetType)))
            {
                return new TargetDefaultValueConverter(typeConverter, sourceType, targetType,
                                                       canConvertFrom, targetToSource && canConvertTo);
            }

            // if nothing worked and the target type is String, use a new TypeConverter (which will
            // simply call ToString)
            if (targetType == typeof(string))
            {
                typeConverter = new TypeConverter();
                canConvertTo = true;
                canConvertFrom = false;

                return new SourceDefaultValueConverter(typeConverter, sourceType, targetType, false, true);
            }

            // nothing worked, give up
            return null;
        }

        internal static TypeConverter GetConverter(Type type)
        {
            return TypeConverterHelper.GetConverter(type);
        }

        internal static readonly IValueConverter ValueConverterNotNeeded = new ObjectTargetConverter(typeof(object));

        //------------------------------------------------------
        //
        //  Protected API
        //
        //------------------------------------------------------

        protected object ConvertFrom(object o, Type destinationType, DependencyObject targetElement, CultureInfo culture)
        {
            return ConvertHelper(o, destinationType, targetElement, culture, false);
        }

        protected object ConvertTo(object o, Type destinationType, DependencyObject targetElement, CultureInfo culture)
        {
            return ConvertHelper(o, destinationType, targetElement, culture, true);
        }

        // for lazy creation of the type converter, since GetConverter is expensive
        protected void EnsureConverter(Type type)
        {
            if (_typeConverter == null)
            {
                _typeConverter = GetConverter(type);
            }
        }

        //------------------------------------------------------
        //
        //  Private API
        //
        //------------------------------------------------------

        private object ConvertHelper(object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, bool isForward)
        {
            object value = DependencyProperty.UnsetValue;
            bool needAssignment = (isForward ? !_shouldConvertTo : !_shouldConvertFrom);
            NotSupportedException savedEx = null;

            if (!needAssignment)
            {
                try
                {
                    if (isForward)
                    {
                        value = _typeConverter.ConvertTo(null, culture, o, destinationType);
                    }
                    else
                    {
                        value = _typeConverter.ConvertFrom(null, culture, o);
                    }
                }
                catch (NotSupportedException ex)
                {
                    needAssignment = true;
                    savedEx = ex;
                }
            }

            if (needAssignment &&
                ((o != null && destinationType.IsAssignableFrom(o.GetType())) ||
                  (o == null && !destinationType.IsValueType)))
            {
                value = o;
                needAssignment = false;
            }

            if (needAssignment && savedEx != null)
                throw savedEx;

            return value;
        }

        protected Type _sourceType;
        protected Type _targetType;

        private TypeConverter _typeConverter;
        private bool _shouldConvertFrom;
        private bool _shouldConvertTo;

        static Type StringType = typeof(String);
    }

    internal sealed class SourceDefaultValueConverter : DefaultValueConverter, IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        public SourceDefaultValueConverter(TypeConverter typeConverter, Type sourceType, Type targetType,
                                           bool shouldConvertFrom, bool shouldConvertTo)
            : base(typeConverter, sourceType, targetType, shouldConvertFrom, shouldConvertTo)
        {
        }

        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertTo(o, _targetType, parameter as DependencyObject, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertFrom(o, _sourceType, parameter as DependencyObject, culture);
        }
    }

    internal sealed class TargetDefaultValueConverter : DefaultValueConverter, IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        public TargetDefaultValueConverter(TypeConverter typeConverter, Type sourceType, Type targetType,
                                           bool shouldConvertFrom, bool shouldConvertTo)
            : base(typeConverter, sourceType, targetType, shouldConvertFrom, shouldConvertTo)
        {
        }

        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertFrom(o, _targetType, parameter as DependencyObject, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertTo(o, _sourceType, parameter as DependencyObject, culture);
        }
    }

    internal sealed class SystemConvertConverter : IValueConverter
    {
        public SystemConvertConverter(Type sourceType, Type targetType)
        {
            _sourceType = sourceType;
            _targetType = targetType;
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(o, _targetType, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(o, _sourceType, culture);
        }

        // ASSUMPTION: sourceType != targetType
        public static bool CanConvert(Type sourceType, Type targetType)
        {
            // This assert is not Invariant.Assert because this will not cause
            // harm; It would just be odd.
            Debug.Assert(sourceType != targetType);

            // DateTime can only be converted to and from String type
            if (sourceType == typeof(DateTime))
                return (targetType == typeof(String));
            if (targetType == typeof(DateTime))
                return (sourceType == typeof(String));

            // Char can only be converted to a subset of supported types
            if (sourceType == typeof(Char))
                return CanConvertChar(targetType);
            if (targetType == typeof(Char))
                return CanConvertChar(sourceType);

            // Using nested loops is up to 40% more efficient than using one loop
            for (int i = 0; i < SupportedTypes.Length; ++i)
            {
                if (sourceType == SupportedTypes[i])
                {
                    ++i;    // assuming (sourceType != targetType), start at next type
                    for (; i < SupportedTypes.Length; ++i)
                    {
                        if (targetType == SupportedTypes[i])
                            return true;
                    }
                }
                else if (targetType == SupportedTypes[i])
                {
                    ++i;    // assuming (sourceType != targetType), start at next type
                    for (; i < SupportedTypes.Length; ++i)
                    {
                        if (sourceType == SupportedTypes[i])
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool CanConvertChar(Type type)
        {
            for (int i = 0; i < CharSupportedTypes.Length; ++i)
            {
                if (type == CharSupportedTypes[i])
                    return true;
            }
            return false;
        }

        Type _sourceType, _targetType;

        // list of types supported by System.Convert (from the SDK)
        static readonly Type[] SupportedTypes = {
            typeof(String),                             // put common types up front
            typeof(Int32),  typeof(Int64),  typeof(Single), typeof(Double),
            typeof(Decimal),typeof(Boolean),
            typeof(Byte),   typeof(Int16),
            typeof(UInt32), typeof(UInt64), typeof(UInt16), typeof(SByte),  // non-CLS compliant types
        };

        // list of types supported by System.Convert for Char Type(from the SDK)
        static readonly Type[] CharSupportedTypes = {
            typeof(String),                             // put common types up front
            typeof(Int32),  typeof(Int64),  typeof(Byte),   typeof(Int16),
            typeof(UInt32), typeof(UInt64), typeof(UInt16), typeof(SByte),  // non-CLS compliant types
        };
    }

    // BUG: 1109257 ObjectTargetConverter is not the best converter possible:
    // it'll use the Source type's system converter, but at conversion time,
    // the real target type's converter is another converter that can be tried.
    internal sealed class ObjectTargetConverter : DefaultValueConverter, IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        public ObjectTargetConverter(Type sourceType) :
            base(null, sourceType, typeof(object),
                 true /* shouldConvertFrom */, false /* shouldConvertTo */)
        {
        }

        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            // conversion from any type to object is easy
            return o;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            // if types are compatible, just pass the value through
            if (o == null && !_sourceType.IsValueType)
                return o;

            if (o != null && _sourceType.IsAssignableFrom(o.GetType()))
                return o;

            // if source type is string, use String.Format (string's type converter doesn't
            // do it for us - boo!)
            if (_sourceType == typeof(String))
                return String.Format(culture, "{0}", o);

            // otherwise, use system converter
            EnsureConverter(_sourceType);
            return ConvertFrom(o, _sourceType, parameter as DependencyObject, culture);
        }
    }

    // BUG: 1109257 ObjectSourceConverter is not the best converter possible.
    internal sealed class ObjectSourceConverter : DefaultValueConverter, IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        public ObjectSourceConverter(Type targetType) :
            base(null, typeof(object), targetType,
                 true /* shouldConvertFrom */, false /* shouldConvertTo */)
        {
        }

        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            // if types are compatible, just pass the value through
            if ((o != null && _targetType.IsAssignableFrom(o.GetType())) ||
                (o == null && !_targetType.IsValueType))
                return o;

            // if target type is string, use String.Format (string's type converter doesn't
            // do it for us - boo!)
            if (_targetType == typeof(String))
                return String.Format(culture, "{0}", o);

            // otherwise, use system converter
            EnsureConverter(_targetType);
            return ConvertFrom(o, _targetType, parameter as DependencyObject, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            // conversion from any type to object is easy
            return o;
        }
    }

    internal sealed class ListSourceConverter : IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------


        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            IList il = null;
            IListSource ils = o as IListSource;

            if (ils != null)
            {
                il = ils.GetList();
            }

            return il;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    internal sealed class InterfaceConverter : IValueConverter
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        internal InterfaceConverter(Type sourceType, Type targetType)
        {
            _sourceType = sourceType;
            _targetType = targetType;
        }

        //------------------------------------------------------
        //
        //  Interfaces (IValueConverter)
        //
        //------------------------------------------------------

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertTo(o, _targetType);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return ConvertTo(o, _sourceType);
        }

        private object ConvertTo(object o, Type type)
        {
            return type.IsInstanceOfType(o) ? o : null;
        }

        Type _sourceType;
        Type _targetType;
    }
}