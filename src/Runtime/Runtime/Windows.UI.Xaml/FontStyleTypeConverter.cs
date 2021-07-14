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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string exp)
            {
                return FontStyle.INTERNAL_ConvertFromString(exp);
            }

            return base.ConvertFrom(context, culture, value);
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

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                throw new NotSupportedException($"Conversion to {destinationType.FullName} is not spported.");
            }

            return value.ToString();
        }
    }
}
