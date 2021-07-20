

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


using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Media.Matrix" /> object to and from other types.
    /// type.
    /// </summary>
    public sealed partial class MatrixConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Media.Matrix" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Media.Matrix" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>Attempts to convert the specified object to a <see cref="T:System.Windows.Media.Matrix" />. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Media.Matrix" /> created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.NotSupportedException">The specified object is null or is a type that cannot be converted to a <see cref="T:System.Windows.Media.Matrix" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var source = value.ToString();

                if (source == "Identity")
                {
                    result = Matrix.Identity;
                }
                else
                {
                    var splittedString = source.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splittedString.Length == 6)
                    {
#if NETSTANDARD
                        if (double.TryParse(splittedString[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var m11) &&
                            double.TryParse(splittedString[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var m12) &&
                            double.TryParse(splittedString[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var m21) &&
                            double.TryParse(splittedString[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var m22) &&
                            double.TryParse(splittedString[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var offsetX) &&
                            double.TryParse(splittedString[5], NumberStyles.Any, CultureInfo.InvariantCulture, out var offsetY))
#elif BRIDGE
                if (double.TryParse(splittedString[0], out var m11) &&
                    double.TryParse(splittedString[1], out var m12) &&
                    double.TryParse(splittedString[2], out var m21) &&
                    double.TryParse(splittedString[3], out var m22) &&
                    double.TryParse(splittedString[4], out var offsetX) &&
                    double.TryParse(splittedString[5], out var offsetY))
#endif
                            result = new Matrix(m11, m12, m21, m22, offsetX, offsetY);
                    }
                    else
                    {
                        throw new FormatException(source + " is not an eligible value for a Matrix");
                    }
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Media.Matrix" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Media.Matrix" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Media.Matrix" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Media.Matrix" />.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType != null && value is Matrix matrix)
            {
                if (destinationType == typeof(string))
                {
                    result = matrix.ToString(null, culture);
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