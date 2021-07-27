using System;
using System.ComponentModel;
using System.Reflection;

namespace DotNetForHtml5.Core
{
    /// <summary>
    /// A utility class for converting one type to another.
    /// </summary>
    /// <remarks>
    /// Currently, ony string conversion is supported. See <see cref="Parse(string, Type)" /> for details.
    /// </remarks>
    public class ObjectBuilder
    {
        public static ObjectBuilder Singleton { get; } = new ObjectBuilder();

        /// <summary>
        /// Determines whether the conversion from <see cref="string" /> to <typeparamref name="T"/> is supported.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CanParse<T>()
        {
            return CanParse(typeof(T));
        }

        /// <summary>
        /// Determines whether the conversion from <see cref="string" /> to <paramref name="targetType" /> is supported.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public bool CanParse(Type targetType)
        {
            var converter = TypeDescriptor.GetConverter(targetType);

            return converter?.CanConvertFrom(typeof(string)) ?? false;
        }

        /// <summary>
        /// Parses a string into <typeparamref name="T"/>, if possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns>
        /// If the conversion from string is successful, this method returns a new <typeparamref name="T"/> constructed out of the <paramref name="expression" />
        /// Otherwise, an exception is thrown.
        /// </returns>
        public T Parse<T>(string expression)
        {
            return (T)Parse(expression, typeof(T));
        }

        /// <summary>
        /// Parses a string into <paramref name="targetType" />, if possible.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="targetType"></param>
        /// <returns>
        /// If the conversion from string is successful, this method returns a new <paramref name="targetType" /> constructed out of the <paramref name="expression" />
        /// Otherwise, an exception is thrown.
        /// </returns>
        public object Parse(string expression, Type targetType)
        {
            object result = null;

            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter is null)
            {
#if !BRIDGE
                throw new TargetException($"The type converter for {targetType.FullName} was not found. Make sure to decorate the {targetType.FullName} class with a {nameof(TypeConverterAttribute)}.");
#endif
            }
            else
            {
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        result = converter.ConvertFrom(expression);
                    }
                    catch (Exception)
                    {
                        // TODO: Create a ticket for implementing the CommandConverter into TelerikForOpenSilver and use the TileViewCommands class to recognize the ToggleTileState command
                        // TODO: Log?
                        result = default;
                    }
                }
                else if (targetType.IsAssignableFrom(typeof(string)))
                {
                    result = expression;
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
