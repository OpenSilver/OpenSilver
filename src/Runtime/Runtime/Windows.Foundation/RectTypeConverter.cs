using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Converts instances of System.Windows.Rect to and from other data types.
    /// </summary>
    public class RectTypeConverter : TypeConverter
    {
        /// <summary>
        /// Indicates whether an object can be converted from a given type to a System.Windows.Rect.
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
            var result = default(Rect);

            if (value is string exp)
            {
                var split = exp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 4)
                {
                    double.TryParse(split[0], out var x);
                    double.TryParse(split[1], out var y);
                    double.TryParse(split[2], out var width);
                    double.TryParse(split[3], out var height);

                    result = new Rect(x, y, width, height);
                }
                else
                {
                    throw new FormatException($"The {nameof(value)} was not in the expected format: \"x, y, width, height\"");
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether System.Windows.Rect values can be converted to
        /// the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">
        /// The desired type this System.Windows.Rect is being evaluated to be
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
