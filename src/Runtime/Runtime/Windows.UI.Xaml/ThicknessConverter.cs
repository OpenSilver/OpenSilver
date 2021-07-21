

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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Thickness" /> object to and from other types.
    /// </summary>
    public class ThicknessConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Thickness" />.
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
        /// Determines whether an instance of <see cref="T:System.Windows.Thickness" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />
        /// or <see cref="T:System.ComponentModel.Design.Serialization.InstanceDescriptor" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);
        }

        /// <summary>Attempts to create an instance of <see cref="T:System.Windows.Thickness" /> from a specified object. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>An instance of <see cref="T:System.Windows.Thickness" /> created from the converted <paramref name="value" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> object is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The example object is not a null reference and is not a valid type that can be converted to a <see cref="T:System.Windows.Thickness" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                var thicknessAsString = value.ToString();

                var splitter = ',';
                var trimmedThicknessAsString = thicknessAsString.Trim(); //we trim the string so that we don't get random spaces at the beginning and at the end act as separators (for example: Margin=" 5")

                if (!trimmedThicknessAsString.Contains(","))
                {
                    splitter = ' ';
                }

                var splittedString = trimmedThicknessAsString.Split(splitter);

                if (splittedString.Length == 1)
                {
                    if (double.TryParse(splittedString[0], out var thickness))
                    {
                        result = new Thickness(thickness);
                    }
                }
                else if (splittedString.Length == 2)
                {
                    var topAndBottom = 0d;

                    var isParseOK = double.TryParse(splittedString[0], out var leftAndRight);
                    isParseOK = isParseOK && double.TryParse(splittedString[1], out topAndBottom);

                    if (isParseOK)
                    {
                        result = new Thickness(leftAndRight, topAndBottom, leftAndRight, topAndBottom);
                    }
                }
                else if (splittedString.Length == 4)
                {
                    double top = 0d;
                    double right = 0d;
                    double bottom = 0d;

                    bool isParseOK = double.TryParse(splittedString[0], out var left);
                    isParseOK = isParseOK && double.TryParse(splittedString[1], out top);
                    isParseOK = isParseOK && double.TryParse(splittedString[2], out right);
                    isParseOK = isParseOK && double.TryParse(splittedString[3], out bottom);

                    if (isParseOK)
                    {
                        result = new Thickness(left, top, right, bottom);
                    }
                }

                if (result is null)
                {
                    throw new FormatException(thicknessAsString + " is not an eligible value for Thickness");
                }
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Attempts to convert a <see cref="T:System.Windows.Thickness" /> to a specified type. </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Thickness" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Thickness" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Thickness" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> object is not <see langword="null" /> and is not a Brush, or the <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="value" /> object is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            object result = null;

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is Thickness thickness)
            {
                if (destinationType == typeof(string))
                {
                    var listSeparator = ',';
                    var instance = NumberFormatInfo.GetInstance(cultureInfo);
                    if (instance.NumberDecimalSeparator.Length > 0 && listSeparator == instance.NumberDecimalSeparator[0])
                    {
                        listSeparator = ';';
                    }

                    // Initial capacity [64] is an estimate based on a sum of:
                    // 48 = 4x double (twelve digits is generous for the range of values likely)
                    //  8 = 4x Unit Type string (approx two characters)
                    //  4 = 4x separator characters
                    var sb = new StringBuilder(64);

                    sb.Append(Convert.ToString(thickness.Left, cultureInfo));
                    sb.Append(listSeparator);
                    sb.Append(Convert.ToString(thickness.Top, cultureInfo));
                    sb.Append(listSeparator);
                    sb.Append(Convert.ToString(thickness.Right, cultureInfo));
                    sb.Append(listSeparator);
                    sb.Append(Convert.ToString(thickness.Bottom, cultureInfo));

                    result = sb.ToString();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    var ci = typeof(Thickness).GetConstructor(new Type[] { typeof(double), typeof(double), typeof(double), typeof(double) });
                    result = new InstanceDescriptor(ci, new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom });
                }
            }
            else
            {
                throw new ArgumentException($"Unexpected paramenter type {value.GetType().FullName}.");
            }

            if (result is null)
            {
                throw new ArgumentException($"Cannot convert type {nameof(CornerRadius)} to {destinationType.FullName}.");
            }

            return result;
        }
    }
}
