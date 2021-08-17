

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


#if BRIDGE
using System;
#endif
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Rect" /> object to and from other types.
    /// </summary>
    public class RectConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Rect" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Rect" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.Rect" />. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Rect" /> created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.NotSupportedException">Thrown if the specified object is <see langword="null" /> or is a type that cannot be converted to a <see cref="T:System.Windows.Rect" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var rectAsString = value.ToString();

                var split = rectAsString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 4)
                {
#if OPENSILVER
                    if (double.TryParse(split[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) &&
                        double.TryParse(split[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y) &&
                        double.TryParse(split[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var width) &&
                        double.TryParse(split[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var height))
#else
                if (double.TryParse(split[0], out var x) &&
                    double.TryParse(split[1], out var y) &&
                    double.TryParse(split[2], out var width) &&
                    double.TryParse(split[3], out var height))
#endif
                        result = new Rect(x, y, width, height);
                }
                else
                {
                    throw new FormatException($"{rectAsString} was not in the expected format: \"x, y, width, height\"");
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Rect" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Rect" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Rect" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Rect" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Thrown if <paramref name="value" /> is <see langword="null" /> or not a <see cref="T:System.Windows.Rect" />,
        /// or if the <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result;

            if (destinationType != null && destinationType == typeof(string) && value is Rect rect)
            {
                result = rect.ToString();
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
