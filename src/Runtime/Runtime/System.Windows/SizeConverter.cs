

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


using System;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Size" /> object to and from other types.
    /// </summary>
    public sealed partial class SizeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Size" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Size" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Attempts to convert a specified object to an instance of <see cref="T:System.Windows.Size" />.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The instance of <see cref="T:System.Windows.Size" /> that is created from the converted <paramref name="source" />.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var sizeAsString = value.ToString();

                var splittedString = sizeAsString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splittedString.Length == 2)
                {
                    double width, height;
#if OPENSILVER
                    if (double.TryParse(splittedString[0], NumberStyles.Any, CultureInfo.InvariantCulture, out width) &&
                        double.TryParse(splittedString[1], NumberStyles.Any, CultureInfo.InvariantCulture, out height))
#else
                if (double.TryParse(splittedString[0], out width) &&
                    double.TryParse(splittedString[1], out height))
#endif
                        return new Size(width, height);
                }

                throw new FormatException(sizeAsString + " is not an eligible value for a Size");
            }

            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>Attempts to convert a <see cref="T:System.Windows.Size" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Size" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Size" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Size" />.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Thrown if <paramref name="value" /> is <see langword="null" /> or not an instance of <see cref="T:System.Windows.Size" />,
        /// or if the <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result;

            if (destinationType != null && destinationType == typeof(string) && value is Size size)
            {
                result = size.ToString();
            }
            else
            {
                result = base.ConvertTo(context, culture, value, destinationType);
            }

            return result;
        }
    }
}
