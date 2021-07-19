using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Transform" /> object to and from other types.
    /// </summary>
    public class TransformConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Transform" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="sourceType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Transform" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     source is null.
        //
        //   System.NotSupportedException:
        //     source is not null and is not a valid type which can be converted to a System.Windows.Media.Transform.
        /// <summary>
        /// Converts the specified object to a System.Windows.Media.Transform.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.Media.Transform created from converting source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (value.GetType() != typeof(string))
            {
                throw GetConvertFromException(value);
            }

            return Transform.Parse((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Transform" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Transform" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Transform" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Transform" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="value" /> is <see langword="null" />, <paramref name="value" /> is not a <see cref="T:System.Windows.Media.Transform" />, or <paramref name="destinationType" /> is not a string.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is Transform transform)
            {
                if (destinationType == typeof(string))
                {
                    if (context != null && context.Instance != null)
                    {
                        throw new NotSupportedException($"Conversion to {destinationType.FullName} is not supported.");
                    }

                    result = transform.ToString();
                }
            }

            if (result is null)
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
