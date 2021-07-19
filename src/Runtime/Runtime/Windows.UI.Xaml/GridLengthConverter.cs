using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.GridLength" /> object to and from other types.
    /// </summary>
    public class GridLengthConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.GridLength" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.GridLength" /> can be converted to the specified type.
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
        //     source is not null and is not a valid type which can be converted to a System.Windows.GridLength.
        /// <summary>
        /// Converts the specified object to a System.Windows.GridLength.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.GridLength created from converting source.</returns>
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

            return GridLength.INTERNAL_ConvertFromString((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.GridLength" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.GridLength" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.GridLength" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.GridLength" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///         <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///         <paramref name="value" /> is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (value != null && value is GridLength length)
            {
                if (destinationType == typeof(string))
                {
                    result = length.ToString(length, cultureInfo);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    var ci = typeof(GridLength).GetConstructor(new Type[] { typeof(double), typeof(GridUnitType) });
                    result = new InstanceDescriptor(ci, new object[] { length.Value, length.GridUnitType });
                }
            }

            if (result is null)
            {
                throw GetConvertToException(value, destinationType);
            }

            return result;
        }
    }
}
