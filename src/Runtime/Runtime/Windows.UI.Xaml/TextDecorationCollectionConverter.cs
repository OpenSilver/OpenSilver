using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

#if MIGRATION
namespace System.Windows
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.TextDecorationCollection" /> object to and from other types.
    /// </summary>
    public class TextDecorationCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.TextDecorationCollection" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.TextDecorationCollection" /> can be converted to the specified type.
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
        //     source is not null and is not a valid type which can be converted to a System.Windows.TextDecorationCollection.
        /// <summary>
        /// Converts the specified object to a System.Windows.TextDecorationCollection.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.TextDecorationCollection created from converting source.</returns>
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

            return TextDecorationCollection.INTERNAL_ConvertFromString((string)value);
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.TextDecorationCollection" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.TextDecorationCollection" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.TextDecorationCollection" /> to.</param>
        /// <returns>
        /// <see langword="null" /> is always returned because <see cref="T:System.Windows.TextDecorationCollection" /> cannot be converted to any other type.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result;

            if (destinationType == typeof(InstanceDescriptor) && value is IEnumerable<TextDecoration>)
            {
                var ci = typeof(TextDecorationCollection).GetConstructor(new Type[] { typeof(IEnumerable<TextDecoration>) });

                result = new InstanceDescriptor(ci, new object[] { value });
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
#endif