using System;
using System.ComponentModel;
using System.Reflection;

namespace DotNetForHtml5.Core
{
    public class ObjectGenerator
    {
        private static readonly ObjectGenerator Instance = new ObjectGenerator();

        private ObjectGenerator()
        {

        }

        public static ObjectGenerator Current { get => Instance; }

        public T Parse<T>(string exp)
        {
            return (T)Parse(exp, typeof(T));
        }

        public object Parse(string exp, Type targetType)
        {
            object result = null;

            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter is null)
            {
                throw new TargetException($"The type converter for {targetType.FullName} was not found. Make sure to decorate the {targetType.FullName} class with a {nameof(TypeConverterAttribute)}.");
            }
            else
            {
                if (converter.CanConvertFrom(typeof(string)))
                {
                    result = converter.ConvertFrom(exp);
                }
                else if (targetType.IsAssignableFrom(typeof(string)))
                {
                    result = exp;
                }
                else
                {
                    throw new NotSupportedException($"The converter for {targetType.FullName} is unable to convert from {nameof(String)}.");
                }
            }

            return result;
        }
    }
}
