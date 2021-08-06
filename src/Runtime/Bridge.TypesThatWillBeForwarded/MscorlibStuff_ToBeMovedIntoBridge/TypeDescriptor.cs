using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class TypeDescriptor
    {
        private static readonly Dictionary<Type, TypeConverter> _converterCache = new Dictionary<Type, TypeConverter>();

        private static readonly Dictionary<Type, TypeConverter> _builtInConverters = new Dictionary<Type, TypeConverter>
        {
            { typeof(Guid), new GuidConverter() },
            { typeof(TimeSpan), new TimeSpanConverter() },
            { typeof(bool), new BooleanConverter() },
            { typeof(DateTime), new DateTimeConverter() },
            { typeof(DateTimeOffset), new DateTimeOffsetConverter() },
            { typeof(object), new TypeConverter() },
            { typeof(void), new TypeConverter() },
            { typeof(string), new StringConverter() },
            { typeof(Uri), new UriTypeConverter() },
            { typeof(byte), new ByteConverter() },
            { typeof(decimal), new DecimalConverter() },
            { typeof(double), new DoubleConverter() },
            { typeof(short), new Int16Converter() },
            { typeof(int), new Int32Converter() },
            { typeof(long), new Int64Converter() },
            { typeof(sbyte), new SByteConverter() },
            { typeof(char), new CharConverter() },
            { typeof(float), new SingleConverter() },
            { typeof(ushort), new UInt16Converter() },
            { typeof(uint), new UInt32Converter() },
            { typeof(ulong), new UInt64Converter() },
        };

        public static TypeConverter GetConverter(object component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return GetConverter(component as Type ?? component.GetType());
        }

        public static TypeConverter GetConverter(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            TypeConverter converter;

            if (_builtInConverters.TryGetValue(type, out converter))
            {
                return converter;
            }

            if (_converterCache.TryGetValue(type, out converter))
            {
                return converter;
            }

            // Special cases
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                converter = new NullableConverter(type);
                _converterCache.Add(type, converter);
                return converter;
            }

            if (type.IsEnum)
            {
                converter = new EnumConverter(type);
                _converterCache.Add(type, converter);
                return converter;
            }

            for (var t = type; t != null; t = t.BaseType)
            {
                var attrs = t.GetCustomAttributes(false);
                if (attrs != null && attrs.Length > 0)
                {
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        if (attrs[i] is TypeConverterAttribute tc)
                        {
                            converter = GetConverterFromTypeName(tc.ConverterTypeName);
                            if (converter == null)
                                continue;

                            _converterCache.Add(type, converter);
                            return converter;
                        }
                    }
                }
            }

            return null;
        }

        private static TypeConverter GetConverterFromTypeName(string typeName)
        {
            var converterType = Type.GetType(typeName);
            if (converterType != null && typeof(TypeConverter).IsAssignableFrom(converterType))
            {
                return (TypeConverter)Activator.CreateInstance(converterType);
            }

            return null;
        }
    }
}
