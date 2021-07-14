using System.ComponentModel;
using System.Globalization;

namespace System.Windows
{
    /// <summary>
    /// Converts instances of System.Windows.FontStyle to and from other data types.
    /// </summary>
    public class FontStyleTypeConverter : TypeConverter
    {
        /// <summary>
        /// Indicates whether an object can be converted from a given type to a System.Windows.FontStyle.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The source System.Type that is being queried for conversion support.</param>
        /// <returns>true if sourceType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether System.Windows.FontStyle values can be converted to
        /// the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type this System.Windows.FontStyle is being evaluated to be
        /// converted to.
        /// </param>
        /// <returns>true if destinationType is of type System.String; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     source is null.
        //
        //   System.NotSupportedException:
        //     source is not null and is not a valid type which can be converted to a System.Windows.FontStyle.
        /// <summary>
        /// Converts the specified object to a System.Windows.FontStyle.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The System.Windows.FontStyle created from converting source.</returns>
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

            return FontStyle.INTERNAL_ConvertFromString((string)value);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     value is null.
        //
        //   System.NotSupportedException:
        //     value is not null and is not a System.Windows.FontStyle, or if destinationType
        //     is not one of the valid destination types.
        /// <summary>
        /// Converts the specified System.Windows.FontStyle to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The System.Windows.FontStyle to convert.</param>
        /// <param name="destinationType">The type to convert the System.Windows.FontStyle to.</param>
        /// <returns>The object created from converting this System.Windows.FontStyle (a string).</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (!(value is FontStyle))
            {
                throw new NotSupportedException($"Conversion from {value.GetType().FullName} is not supported.");
            }
            else if (destinationType != typeof(string))
            {
                throw new NotSupportedException($"Conversion to {destinationType.FullName} is not supported.");
            }

            return value.ToString();
        }
    }
}
