
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace System.ComponentModel
{
    internal static class TypeConverterHelper
    {
        // sentinel value used to specify we were not able to find a TypeConverter
        internal static readonly TypeConverter NullConverter = new TypeConverter();

        // key associated with NullConverter
        private static readonly object _nullConverterKey = new object();

        // key for nullable types
        private static readonly object _intrinsicNullableKey = new object();

        private static readonly Assembly _openSilverAssembly = typeof(DependencyObject).Assembly;
        
        private static readonly object _internalSyncObject = new object();
        
        private static Dictionary<Type, ReflectedTypeData> TypeData { get; } =
            new Dictionary<Type, ReflectedTypeData>();

        private static Dictionary<Type, ReflectedPropertyData[]> PropertyCache { get; } =
            new Dictionary<Type, ReflectedPropertyData[]>();

        private static Dictionary<object, IntrinsicTypeConverterData> IntrinsicTypeConverters { get; } =
            GetIntrinsicTypeConvertersData();

        // Contains Converters for types defined in the OpenSilver library.
        private static Dictionary<Type, TypeConverter> CoreTypeConverters { get; } =
            GetCoreTypeConverters();
        
        // This dictionary is used to workaround TypeConverterAttribute that are defined
        // on types from .NET that we need to ignore.
        private static Dictionary<Type, TypeConverter> OverriddenTypeConverters { get; } =
            GetOverriddenTypeConverters();

        public static TypeConverter GetConverter(Type type)
        {
            TypeConverter converter = null;

            if (IsCoreType(type))
            {
                converter = GetCoreTypeConverter(type);
            }

            if (converter == null)
            {
                // Normal lookup
                converter = GetTypeConverterFromTypeData(type);
            }

            return converter;
        }

        public static ReflectedPropertyCollection GetProperties(Type type)
        {
            ReflectedTypeData td = GetTypeData(type, true);
            return td.GetProperties();
        }

        // Helper method for TemplateBindingExpression
        internal static TypeConverter GetBuiltInConverter(Type type)
        {
            if (IsNullableType(type))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            TypeConverter converter = null;

            if (IsCoreType(type))
            {
                converter = GetCoreTypeConverter(type);
            }

            if ((converter ??= GetIntrinsicTypeConverter(type)) == NullConverter)
            {
                converter = null;
            }

            return converter;
        }

        internal static bool IsCoreType(Type type)
        {
            return type.Assembly == _openSilverAssembly;
        }

        private static TypeConverter GetCoreTypeConverter(Type type)
        {
            if (CoreTypeConverters.TryGetValue(type, out TypeConverter coreConverter))
            {
                return coreConverter;
            }

            return null;
        }

        private static TypeConverter GetOverriddenTypeConverter(Type type)
        {
            if (OverriddenTypeConverters.TryGetValue(type, out TypeConverter converter))
            {
                return converter;
            }

            return null;
        }

        private static TypeConverter GetIntrinsicTypeConverter(Type type)
        {
            if (!IntrinsicTypeConverters.TryGetValue(type, out IntrinsicTypeConverterData converterData))
            {
                if (type.IsEnum)
                {
                    converterData = IntrinsicTypeConverters[typeof(Enum)];
                }
                else if (IsNullableType(type))
                {
                    converterData = IntrinsicTypeConverters[_intrinsicNullableKey];
                }
                else
                {
                    // Uri and CultureInfo are the only types that can be derived from for which we have intrinsic converters.
                    // Check if the calling type derives from either and return the appropriate converter.

                    // We should have fetched converters for these types above.
                    Debug.Assert(type != typeof(Uri) && type != typeof(CultureInfo));

                    object key = null;

                    Type baseType = type.BaseType;
                    while (baseType != null && baseType != typeof(object))
                    {
                        if (baseType == typeof(Uri) || baseType == typeof(CultureInfo))
                        {
                            key = baseType;
                            break;
                        }

                        baseType = baseType.BaseType;
                    }

                    if (key == null)
                    {
                        key = _nullConverterKey;
                    }

                    converterData = IntrinsicTypeConverters[key];
                }
            }

            return converterData.GetOrCreateConverterInstance(type);
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static TypeConverter GetTypeConverterFromTypeData(Type type)
        {
            for (Type t = type; t != null; t = t.BaseType)
            {
                TypeConverter converter = GetTypeData(t, true).GetConverter();
                if (converter != NullConverter)
                {
                    return converter;
                }
            }

            return null;
        }

        private static ReflectedTypeData GetTypeData(Type type, bool createIfNeeded)
        {
            ReflectedTypeData td;

            lock (_internalSyncObject)
            {
                if (!TypeData.TryGetValue(type, out td) && createIfNeeded)
                {
                    td = new ReflectedTypeData(type);
                    TypeData.Add(type, td);
                }
            }

            return td;
        }

        internal static Attribute FindAttributeByType(Type attrType, Attribute[] attributes)
        {
            foreach (Attribute attr in attributes)
            {
                if (attr.GetType() == attrType)
                {
                    return attr;
                }
            }

            return null;
        }

        private static ReflectedPropertyData[] ReflectGetProperties(Type type)
        {
            Dictionary<Type, ReflectedPropertyData[]> propertyCache = PropertyCache;
            ReflectedPropertyData[] properties;

            lock (_internalSyncObject)
            {
                if (!propertyCache.TryGetValue(type, out properties))
                {
                    BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

                    // Get the type's properties. Properties may have their
                    // get and set methods individually overridden in a derived
                    // class, so if we find a missing method we need to walk
                    // down the base class chain to find it. We actually merge
                    // "new" properties of the same name, so we must preserve
                    // the member info for each method individually.
                    //
                    PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);

                    properties = new ReflectedPropertyData[propertyInfos.Length];
                    int propertyCount = 0;

                    for (int idx = 0; idx < propertyInfos.Length; idx++)
                    {
                        PropertyInfo propertyInfo = propertyInfos[idx];

                        // Today we do not support parameterized properties.
                        //
                        if (propertyInfo.GetIndexParameters().Length > 0)
                        {
                            continue;
                        }

                        MethodInfo getMethod = propertyInfo.GetGetMethod();
                        MethodInfo setMethod = propertyInfo.GetSetMethod();
                        string name = propertyInfo.Name;

                        // If the property only overrode "set", then we don't
                        // pick it up here. Rather, we just merge it in from
                        // the base class list.


                        // If a property had at least a get method, we consider it. We don't
                        // consider write-only properties.
                        //
                        if (getMethod != null)
                        {
                            properties[propertyCount++] = new ReflectedPropertyData(type, name,
                                                                                    propertyInfo.PropertyType,
                                                                                    propertyInfo, 
                                                                                    getMethod, setMethod);
                        }
                    }

                    if (propertyCount != properties.Length)
                    {
                        ReflectedPropertyData[] newProperties = new ReflectedPropertyData[propertyCount];
                        Array.Copy(properties, newProperties, propertyCount);
                        properties = newProperties;
                    }

                    propertyCache[type] = properties;
                }
            }

            return properties;
        }

        /// <summary>
        /// Initialize the <see cref="IntrinsicTypeConverters"/> property.
        /// </summary>
        private static Dictionary<object, IntrinsicTypeConverterData> GetIntrinsicTypeConvertersData()
        {
            return new Dictionary<object, IntrinsicTypeConverterData>(23)
            {
                [typeof(bool)] = new IntrinsicTypeConverterData((type) => new BooleanConverter()),
                [typeof(byte)] = new IntrinsicTypeConverterData((type) => new ByteConverter()),
                [typeof(sbyte)] = new IntrinsicTypeConverterData((type) => new SByteConverter()),
                [typeof(char)] = new IntrinsicTypeConverterData((type) => new CharConverter()),
                [typeof(double)] = new IntrinsicTypeConverterData((type) => new DoubleConverter()),
                [typeof(string)] = new IntrinsicTypeConverterData((type) => new StringConverter()),
                [typeof(int)] = new IntrinsicTypeConverterData((type) => new Int32Converter()),
                [typeof(short)] = new IntrinsicTypeConverterData((type) => new Int16Converter()),
                [typeof(long)] = new IntrinsicTypeConverterData((type) => new Int64Converter()),
                [typeof(float)] = new IntrinsicTypeConverterData((type) => new SingleConverter()),
                [typeof(ushort)] = new IntrinsicTypeConverterData((type) => new UInt16Converter()),
                [typeof(uint)] = new IntrinsicTypeConverterData((type) => new UInt32Converter()),
                [typeof(ulong)] = new IntrinsicTypeConverterData((type) => new UInt64Converter()),
                [typeof(CultureInfo)] = new IntrinsicTypeConverterData((type) => new CultureInfoConverter()),
                [typeof(DateTime)] = new IntrinsicTypeConverterData((type) => new DateTimeConverter()),
                [typeof(DateTimeOffset)] = new IntrinsicTypeConverterData((type) => new DateTimeOffsetConverter()),
                [typeof(decimal)] = new IntrinsicTypeConverterData((type) => new DecimalConverter()),
                [typeof(TimeSpan)] = new IntrinsicTypeConverterData((type) => new TimeSpanConverter()),
                [typeof(Guid)] = new IntrinsicTypeConverterData((type) => new GuidConverter()),
                [typeof(Uri)] = new IntrinsicTypeConverterData((type) => new UriTypeConverter()),
                [typeof(Enum)] = new IntrinsicTypeConverterData((type) => new EnumConverter(type), false),
                [_intrinsicNullableKey] = new IntrinsicTypeConverterData((type) => new NullableConverter2(type), false),
                [_nullConverterKey] = new IntrinsicTypeConverterData((type) => NullConverter, false),
            };
        }

        //
        // IMPORTANT: if you add or remove entries in this dictionary, you must update
        // accordingly the file "CoreTypesHelper.cs" in the Compiler project.
        //
        /// <summary>
        /// Initialize the <see cref="CoreTypeConverters"/> property.
        /// </summary>
        private static Dictionary<Type, TypeConverter> GetCoreTypeConverters()
        {
            return new Dictionary<Type, TypeConverter>(27)
            {
                [typeof(Cursor)] = new CursorConverter(),
                [typeof(KeyTime)] = new KeyTimeConverter(),
                [typeof(RepeatBehavior)] = new RepeatBehaviorConverter(),
                [typeof(Brush)] = new BrushConverter(),
                [typeof(SolidColorBrush)] = new BrushConverter(),
                [typeof(Color)] = new ColorConverter(),
                [typeof(DoubleCollection)] = new DoubleCollectionConverter(),
                [typeof(FontFamily)] = new FontFamilyConverter(),
                [typeof(Geometry)] = new GeometryConverter(),
                [typeof(PathGeometry)] = new GeometryConverter(),
                [typeof(Matrix)] = new MatrixConverter(),
                [typeof(PointCollection)] = new PointCollectionConverter(),
                [typeof(Transform)] = new TransformConverter(),
                [typeof(MatrixTransform)] = new TransformConverter(),
                [typeof(CornerRadius)] = new CornerRadiusConverter(),
                [typeof(Duration)] = new DurationConverter(),
                [typeof(FontWeight)] = new FontWeightConverter(),
                [typeof(GridLength)] = new GridLengthConverter(),
                [typeof(Point)] = new PointConverter(),
                [typeof(PropertyPath)] = new PropertyPathConverter(),
                [typeof(Rect)] = new RectConverter(),
                [typeof(Size)] = new SizeConverter(),
                [typeof(Thickness)] = new ThicknessConverter(),
#if MIGRATION
                [typeof(FontStyle)] = new FontStyleConverter(),
                [typeof(TextDecorationCollection)] = new TextDecorationCollectionConverter(),
#endif
                [typeof(CacheMode)] = new CacheModeConverter(),
                [typeof(FontStretch)] = new FontStretchConverter(),
            };
        }

        //
        // Important: must contain non null values only. In case you need to use null,
        // use NullConverter instead.
        //
        /// <summary>
        /// Initialize the <see cref="OverriddenTypeConverters"/> property.
        /// </summary>
        private static Dictionary<Type, TypeConverter> GetOverriddenTypeConverters()
        {
            return new Dictionary<Type, TypeConverter>(1)
            {
                [typeof(ICommand)] = NullConverter,
            };
        }

        /// <summary>
        /// This class contains all the reflection information for a
        /// given type.
        /// </summary>
        private sealed class ReflectedTypeData
        {
            private readonly Type _type;
            private Attribute[] _attributes;
            private TypeConverter _converter;
            private ReflectedPropertyCollection _properties;

            internal ReflectedTypeData(Type type)
            {
                _type = type;
            }

            /// <devdoc>
            ///     Retrieves custom attributes.
            /// </devdoc>
            internal Attribute[] GetAttributes()
            {
                if (_attributes == null)
                {
                    object[] attrs = _type.GetCustomAttributes(false);
                    if (attrs == null || attrs.Length == 0)
                    {
                        _attributes = new Attribute[0];
                    }
                    else
                    {
                        _attributes = new Attribute[attrs.Length];
                        attrs.CopyTo(_attributes, 0);
                    }
                }

                return _attributes;
            }

            internal TypeConverter GetConverter()
            {
                if (_converter == null)
                {
                    _converter = GetOverriddenTypeConverter(_type);

                    if (_converter == null)
                    {
                        TypeConverterAttribute typeAttr = (TypeConverterAttribute)FindAttributeByType(typeof(TypeConverterAttribute), GetAttributes());
                        if (typeAttr != null)
                        {
                            Type converterType = GetTypeFromName(typeAttr.ConverterTypeName);
                            if (converterType != null && typeof(TypeConverter).IsAssignableFrom(converterType))
                            {
                                _converter = (TypeConverter)Activator.CreateInstance(converterType);
                            }
                        }

                        if (_converter == null)
                        {
                            // We did not get a converter. Traverse up the base class chain.
                            _converter = GetIntrinsicTypeConverter(_type);
                        }
                    }

                    Debug.Assert(_converter != null, $"'{nameof(NullConverter)}' is null or '{nameof(OverriddenTypeConverters)}' contains null values.");
                }

                return _converter;
            }

            internal ReflectedPropertyCollection GetProperties()
            {
                if (_properties == null)
                {
                    ReflectedPropertyData[] propertyArray;
                    Dictionary<string, ReflectedPropertyData> propertyList = new Dictionary<string, ReflectedPropertyData>(10);
                    Type baseType = _type;
                    Type objType = typeof(object);

                    do
                    {
                        propertyArray = ReflectGetProperties(baseType);
                        foreach (ReflectedPropertyData p in propertyArray)
                        {
                            if (!propertyList.ContainsKey(p.Name))
                            {
                                propertyList.Add(p.Name, p);
                            }
                        }
                        baseType = baseType.BaseType;
                    }
                    while (baseType != null && baseType != objType);

                    _properties = new ReflectedPropertyCollection(propertyList);
                }

                return _properties;
            }

            /// <devdoc>
            ///     Retrieves a type from a name.  The Assembly of the type
            ///     that this PropertyDescriptor came from is first checked,
            ///     then a global Type.GetType is performed.
            /// </devdoc>
            private Type GetTypeFromName(string typeName)
            {

                if (typeName == null || typeName.Length == 0)
                {
                    return null;
                }

                int commaIndex = typeName.IndexOf(',');
                Type t = null;

                if (commaIndex == -1)
                {
                    t = _type.Assembly.GetType(typeName);
                }

                if (t == null)
                {
                    t = Type.GetType(typeName);
                }

                if (t == null && commaIndex != -1)
                {
                    // At design time, it's possible for us to reuse
                    // an assembly but add new types.  The app domain
                    // will cache the assembly based on identity, however,
                    // so it could be looking in the previous version
                    // of the assembly and not finding the type.  We work
                    // around this by looking for the non-assembly qualified
                    // name, which causes the domain to raise a type 
                    // resolve event.
                    //
                    t = Type.GetType(typeName.Substring(0, commaIndex));
                }

                return t;
            }
        }

        /// <summary>
        /// Provides a way to create <see cref="TypeConverter"/> instances, and cache them where applicable.
        /// </summary>
        private sealed class IntrinsicTypeConverterData
        {
            private readonly Func<Type, TypeConverter> _constructionFunc;

            private readonly bool _cacheConverterInstance;

            private TypeConverter _converterInstance;

            /// <summary>
            /// Creates a new instance of <see cref="IntrinsicTypeConverterData"/>.
            /// </summary>
            /// <param name="constructionFunc">
            /// A func that creates a new <see cref="TypeConverter"/> instance.
            /// </param>
            /// <param name="cacheConverterInstance">
            /// Indicates whether to cache created <see cref="TypeConverter"/> instances. This is false when the converter handles multiple types,
            /// specifically <see cref="EnumConverter"/>, and <see cref="NullableConverter"/>.
            /// </param>
            public IntrinsicTypeConverterData(Func<Type, TypeConverter> constructionFunc, bool cacheConverterInstance = true)
            {
                _constructionFunc = constructionFunc;
                _cacheConverterInstance = cacheConverterInstance;
            }

            public TypeConverter GetOrCreateConverterInstance(Type innerType)
            {
                if (!_cacheConverterInstance)
                {
                    return _constructionFunc(innerType);
                }

                if (_converterInstance == null)
                {
                    _converterInstance = _constructionFunc(innerType);
                }

                return _converterInstance;
            }
        }

        /// <summary>
        /// TypeConverter to convert Nullable types to and from strings or the underlying simple type.
        /// </summary>
        /// <remarks>
        /// Copy of <see cref="NullableConverter"/> except it does not use <see cref="TypeDescriptor"/> to
        /// fetch the value of <see cref="UnderlyingTypeConverter"/>.
        /// </remarks>
        internal sealed class NullableConverter2 : TypeConverter
        {
            /// <summary>
            /// Nullable converter is initialized with the underlying simple type.
            /// </summary>
            public NullableConverter2(Type type)
            {
                NullableType = type;

                UnderlyingType = Nullable.GetUnderlyingType(type);
                if (UnderlyingType == null)
                {
                    throw new ArgumentException("The specified type is not a nullable type.", nameof(type));
                }

                UnderlyingTypeConverter = GetConverter(UnderlyingType);
            }

            /// <summary>
            /// Gets a value indicating whether this converter can convert an object in the
            /// given source type to the underlying simple type or a null.
            /// </summary>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == UnderlyingType)
                {
                    return true;
                }
                else if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.CanConvertFrom(context, sourceType);
                }

                return base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            /// Converts the given value to the converter's underlying simple type or a null.
            /// </summary>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null || value.GetType() == UnderlyingType)
                {
                    return value;
                }
                else if (value is string && string.IsNullOrEmpty(value as string))
                {
                    return null;
                }
                else if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.ConvertFrom(context, culture, value);
                }

                return base.ConvertFrom(context, culture, value);
            }

            /// <summary>
            /// Gets a value indicating whether this converter can convert a value object to the destination type.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == UnderlyingType)
                {
                    return true;
                }
                else if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.CanConvertTo(context, destinationType);
                }

                return base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Converts the given value object to the destination type.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                {
                    throw new ArgumentNullException(nameof(destinationType));
                }

                if (destinationType == UnderlyingType && value != null && NullableType.IsInstanceOfType(value))
                {
                    return value;
                }
                else if (value == null)
                {
                    // Handle our own nulls here
                    if (destinationType == typeof(string))
                    {
                        return string.Empty;
                    }
                }
                else if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.ConvertTo(context, culture, value, destinationType);
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            /// <summary>
            /// </summary>
            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
            {
                if (UnderlyingTypeConverter != null)
                {
                    object instance = UnderlyingTypeConverter.CreateInstance(context, propertyValues);
                    return instance;
                }

                return base.CreateInstance(context, propertyValues);
            }

            /// <summary>
            /// Gets a value indicating whether changing a value on this object requires a call to
            /// <see cref='System.ComponentModel.TypeConverter.CreateInstance(IDictionary)'/> to create a new value,
            /// using the specified context.
            /// </summary>
            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
            {
                if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.GetCreateInstanceSupported(context);
                }

                return base.GetCreateInstanceSupported(context);
            }

#if NETSTANDARD
            /// <summary>
            /// Gets a collection of properties for the type of array specified by the value
            /// parameter using the specified context and attributes.
            /// </summary>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                if (UnderlyingTypeConverter != null)
                {
                    object unwrappedValue = value;
                    return UnderlyingTypeConverter.GetProperties(context, unwrappedValue, attributes);
                }

                return base.GetProperties(context, value, attributes);
            }
#endif

            /// <summary>
            /// Gets a value indicating whether this object supports properties using the specified context.
            /// </summary>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.GetPropertiesSupported(context);
                }

                return base.GetPropertiesSupported(context);
            }

            /// <summary>
            /// Gets a collection of standard values for the data type this type converter is designed for.
            /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (UnderlyingTypeConverter != null)
                {
                    StandardValuesCollection values = UnderlyingTypeConverter.GetStandardValues(context);
                    if (GetStandardValuesSupported(context) && values != null)
                    {
                        // Create a set of standard values around nullable instances.
                        object[] wrappedValues = new object[values.Count + 1];
                        int idx = 0;

                        wrappedValues[idx++] = null;
                        foreach (object value in values)
                        {
                            wrappedValues[idx++] = value;
                        }

                        return new StandardValuesCollection(wrappedValues);
                    }
                }

                return base.GetStandardValues(context);
            }

            /// <summary>
            /// Gets a value indicating whether the collection of standard values returned from
            /// <see cref='System.ComponentModel.TypeConverter.GetStandardValues()'/> is an exclusive
            /// list of possible values, using the specified context.
            /// </summary>
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.GetStandardValuesExclusive(context);
                }

                return base.GetStandardValuesExclusive(context);
            }

            /// <summary>
            /// Gets a value indicating whether this object supports a standard set of values that can
            /// be picked from a list using the specified context.
            /// </summary>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                if (UnderlyingTypeConverter != null)
                {
                    return UnderlyingTypeConverter.GetStandardValuesSupported(context);
                }

                return base.GetStandardValuesSupported(context);
            }

            /// <summary>
            /// Gets a value indicating whether the given value object is valid for this type.
            /// </summary>
            public override bool IsValid(ITypeDescriptorContext context, object value)
            {
                if (UnderlyingTypeConverter != null)
                {
                    object unwrappedValue = value;
                    if (unwrappedValue == null)
                    {
                        return true; // null is valid for nullable.
                    }
                    else
                    {
                        return UnderlyingTypeConverter.IsValid(context, unwrappedValue);
                    }
                }

                return base.IsValid(context, value);
            }

            /// <summary>
            /// The type this converter was initialized with.
            /// </summary>
            public Type NullableType { get; }

            /// <summary>
            /// The simple type that is represented as a nullable.
            /// </summary>
            public Type UnderlyingType { get; }

            /// <summary>
            /// Converter associated with the underlying simple type.
            /// </summary>
            public TypeConverter UnderlyingTypeConverter { get; }
        }
    }
}
